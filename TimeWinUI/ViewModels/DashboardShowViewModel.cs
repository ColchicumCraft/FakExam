
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Controls;
using Newtonsoft.Json;
using TimeWinUI.Contracts.Services;
using TimeWinUI.Core.Models;
using TimeWinUI.Services;
using Windows.Storage;
using Windows.Storage.Pickers;
using WinRT.Interop;
using WinUIEx.Messaging;

namespace TimeWinUI.ViewModels
{
    public partial class DashboardShowViewModel : ObservableObject, IDisposable
    {
        private readonly INavigationService NavigationService;
        private readonly DispatcherQueue _dispatcherQueue;
        private Timer? _timer;

        private DashboardProfile? _profile;

        [ObservableProperty]
        private ObservableCollection<ExamRowView> _examRows = new();

        [ObservableProperty] private string _examName = string.Empty;
        [ObservableProperty] private string _message = string.Empty;

        [ObservableProperty] private string _currentTime = "--:--:--";

        [ObservableProperty] private string _currentExamName = "—";
        [ObservableProperty] private string _currentExamTimeRange = "—";
        [ObservableProperty] private string _remainingTimeText = "—";
        [ObservableProperty] private string _currentStatusText = "—";

        public DashboardShowViewModel(INavigationService navigationService)
        {
            NavigationService = navigationService;
            _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
            StartTimer();
        }

        [RelayCommand]
        private void GoBack()
        {
            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
        }

        [RelayCommand]
        private async Task LoadJsonAsync()
        {
            var picker = new FileOpenPicker();
            picker.FileTypeFilter.Add(".json");
            picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;

            var hwnd = WindowNative.GetWindowHandle(App.MainWindow);
            InitializeWithWindow.Initialize(picker, hwnd);

            StorageFile file = await picker.PickSingleFileAsync();
            if (file == null) return;

            string json = await FileIO.ReadTextAsync(file);
            var profile = JsonConvert.DeserializeObject<DashboardProfile>(json);
            if (profile == null) return;

            _profile = profile;
            ExamName = profile.ExamName ?? string.Empty;
            Message = profile.Message ?? string.Empty;

            RebuildExamRows();
            RefreshNow();
        }

        private void StartTimer()
        {
            _timer = new Timer(_ =>
            {
                _dispatcherQueue.TryEnqueue(() =>
                {
                    RefreshNow();
                });
            }, null, 0, 1000);
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
            if (_profile?.ExamInfos == null) return;

            // 按开始时间排序
            foreach (var exam in _profile.ExamInfos.OrderBy(e => e.StartTime))
            {
                var row = new ExamRowView
                {
                    Model = exam,
                    Date = exam.DisplayDate,                 // M月d日
                    Name = exam.Name ?? string.Empty,
                    Start = exam.StartTime.ToString("mm:ss"),// 仅显示 mm:ss
                    End = exam.EndTime.ToString("mm:ss"),
                    Status = "—"
                };
                ExamRows.Add(row);
            }
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
            if (_profile?.ExamInfos == null || _profile.ExamInfos.Count == 0)
            {
                CurrentExamName = "—";
                CurrentExamTimeRange = "—";
                RemainingTimeText = "—";
                CurrentStatusText = "—";
                return;
            }

            // 找到“进行中”的考试
            var inProgress = _profile.ExamInfos
                .FirstOrDefault(e => now >= e.StartTime && now < e.EndTime);

            ExamInfo? target = inProgress;

            if (target == null)
            {
                // 没有进行中，找最近的未开始
                target = _profile.ExamInfos
                    .Where(e => e.StartTime > now)
                    .OrderBy(e => e.StartTime)
                    .FirstOrDefault();
            }

            if (target == null)
            {
                // 全部已结束
                CurrentExamName = "事件均已结束";
                CurrentExamTimeRange = "—";
                RemainingTimeText = "已结束";
                CurrentStatusText = "已结束";
                return;
            }

            CurrentExamName = target.Name ?? "—";
            CurrentExamTimeRange = $"{target.StartTime:HH:mm:ss} - {target.EndTime:HH:mm:ss}";
            CurrentStatusText = EvaluateStatusText(target, now);

            // 剩余时间：进行中 -> 距离结束；未开始/即将开始 -> 距离开始；已结束 -> 已结束
            RemainingTimeText = CurrentStatusText switch
            {
                "进行中" or "即将结束" => FormatTimeSpan(target.EndTime - now),
                "未开始" or "即将开始" => FormatTimeSpan(target.StartTime - now),
                "已结束" => "已结束",
                _ => "—"
            };
        }

        private string EvaluateStatusText(ExamInfo exam, DateTime now)
        {
            var thresholdMinutes = exam.AlertTime > 0 ? exam.AlertTime : 15;
            if (now >= exam.EndTime)
                return "已结束";

            if (now < exam.StartTime)
            {
                var toStart = exam.StartTime - now;
                if (toStart.TotalMinutes <= thresholdMinutes)
                    return "即将开始";
                return "未开始";
            }

            // now 在 [Start, End) 之间
            var toEnd = exam.EndTime - now;
            if (toEnd.TotalMinutes <= thresholdMinutes)
                return "即将结束";
            return "进行中";
        }

        private static string FormatTimeSpan(TimeSpan ts)
        {
            if (ts.TotalSeconds < 0) ts = TimeSpan.Zero;
            if (ts.TotalHours >= 1)
                return $"{(int)ts.TotalHours:00}:{ts.Minutes:00}:{ts.Seconds:00}";
            return $"{ts.Minutes:00}:{ts.Seconds:00}";
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        public partial class ExamRowView : ObservableObject
        {
            public ExamInfo Model { get; set; } = default!;

            [ObservableProperty] private string _date = string.Empty;   // M月d日
            [ObservableProperty] private string _name = string.Empty;
            [ObservableProperty] private string _start = string.Empty;  // mm:ss
            [ObservableProperty] private string _end = string.Empty;    // mm:ss
            [ObservableProperty] private string _status = "—";          // 进行中/已结束/即将开始/即将结束/未开始
        }
    }
}
