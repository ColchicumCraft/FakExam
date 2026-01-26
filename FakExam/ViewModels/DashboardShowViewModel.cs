using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Dispatching;
using FakExam.Contracts.Services;
using FakExam.Core.Contracts.Services;
using FakExam.Core.Models;

namespace FakExam.ViewModels;

public partial class DashboardShowViewModel : ObservableObject, IDisposable
{
    private readonly INavigationService NavigationService;
    private readonly IDashboardProfileService _profileService;
    private readonly IProfileLoaderService _profileLoader;
    private readonly IClockService _clock;
    private readonly DispatcherQueue _dispatcherQueue;

    [ObservableProperty]
    private ObservableCollection<ExamRowView> _examRows = new();

    [ObservableProperty] private string _examName = string.Empty;
    [ObservableProperty] private string _message = string.Empty;
    [ObservableProperty] private string _currentTime = "--:--:--";
    [ObservableProperty] private string _currentExamName = "—";
    [ObservableProperty] private string _currentExamTimeRange = "—";
    [ObservableProperty] private string _remainingTimeText = "—";
    [ObservableProperty] private string _currentStatusText = "—";

    public DashboardShowViewModel(INavigationService navigationService,
                                  IDashboardProfileService profileService,
                                  IProfileLoaderService profileLoader,
                                  IClockService clock)
    {
        NavigationService = navigationService;
        _profileService = profileService;
        _profileLoader = profileLoader;
        _clock = clock;
        _dispatcherQueue = DispatcherQueue.GetForCurrentThread();

        // 每秒仅用于刷新显示
        _clock.Tick += (_, now) =>
        {
            _dispatcherQueue.TryEnqueue(() =>
            {
                CurrentTime = now.ToString("HH:mm:ss");
                UpdateRowStatuses(now);
                UpdateCurrentBlock(now);
            });
        };

        // 配置变化/状态变化时重建或刷新
        _profileService.ProfileChanged += (_, __) => _dispatcherQueue.TryEnqueue(() =>
        {
            RebuildExamRows();
            RefreshNow();
        });
        _profileService.ExamStateChanged += (_, __) => _dispatcherQueue.TryEnqueue(() => RefreshNow());
    }

    [RelayCommand]
    private void GoBack()
    {
        if (NavigationService.CanGoBack) NavigationService.GoBack();
    }

    [RelayCommand]
    private async System.Threading.Tasks.Task LoadJsonAsync()
    {
        var loaded = await _profileLoader.PickAndLoadAsync();
        if (loaded)
        {
            RebuildExamRows();
            RefreshNow();
        }
    }

    private void RefreshNow()
    {
        var now = DateTime.Now;
        CurrentTime = now.ToString("HH:mm:ss");
        UpdateRowStatuses(now);
        UpdateCurrentBlock(now);
    }

    private void RebuildExamRows()
    {
        ExamRows.Clear();
        var profile = _profileService.CurrentProfile;
        if (profile?.ExamInfos == null) return;
        foreach (var exam in profile.ExamInfos.OrderBy(e => e.StartTime))
        {
            var row = new ExamRowView
            {
                Model = exam,
                Date = exam.DisplayDate,
                Name = exam.Name ?? string.Empty,
                Start = exam.StartTime.ToString("HH:mm"),
                End = exam.EndTime.ToString("HH:mm"),
                Status = "—"
            };
            ExamRows.Add(row);
        }
        ExamName = profile.ExamName ?? string.Empty;
        Message = profile.Message ?? string.Empty;
    }

    private void UpdateRowStatuses(DateTime now)
    {
        foreach (var row in ExamRows)
        {
            row.Status = EvaluateStatusText(row.Model, now);
        }
    }

    private void UpdateCurrentBlock(DateTime now)
    {
        var inProgress = _profileService.CurrentInProgressExam;
        ExamInfo? target = inProgress ?? _profileService.NextUpcomingExam;
        if (target == null)
        {
            CurrentExamName = "事件均已结束";
            CurrentExamTimeRange = "—";
            RemainingTimeText = "已结束";
            CurrentStatusText = "已结束";
            return;
        }

        CurrentExamName = target.Name ?? "—";
        CurrentExamTimeRange = $"{target.StartTime:HH:mm:ss} - {target.EndTime:HH:mm:ss}";
        CurrentStatusText = EvaluateStatusText(target, now);
        RemainingTimeText = CurrentStatusText switch
        {
            "进行中" or "即将结束" => FormatTimeSpan(target.EndTime - now),
            "未开始" or "即将开始" => FormatTimeSpan(target.StartTime - now),
            "已结束" => "已结束",
            _ => "—"
        };
    }

    private static string EvaluateStatusText(ExamInfo exam, DateTime now)
    {
        var thresholdMinutes = exam.AlertTime > 0 ? exam.AlertTime : 15;
        if (now >= exam.EndTime) return "已结束";
        if (now < exam.StartTime)
        {
            var toStart = exam.StartTime - now;
            return toStart.TotalMinutes <= thresholdMinutes ? "即将开始" : "未开始";
        }
        var toEnd = exam.EndTime - now;
        return toEnd.TotalMinutes <= thresholdMinutes ? "即将结束" : "进行中";
    }

    private static string FormatTimeSpan(TimeSpan ts)
    {
        if (ts.TotalSeconds < 0) ts = TimeSpan.Zero;
        return ts.TotalHours >= 1
            ? $"{(int)ts.TotalHours:00}:{ts.Minutes:00}:{ts.Seconds:00}"
            : $"{ts.Minutes:00}:{ts.Seconds:00}";
    }

    public void Dispose() { }

    public partial class ExamRowView : ObservableObject
    {
        public ExamInfo Model { get; set; } = default!;
        [ObservableProperty] private string _date = string.Empty;
        [ObservableProperty] private string _name = string.Empty;
        [ObservableProperty] private string _start = string.Empty;
        [ObservableProperty] private string _end = string.Empty;
        [ObservableProperty] private string _status = "—";
    }
}
