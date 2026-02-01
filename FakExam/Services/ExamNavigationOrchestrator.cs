using System;
using System.Threading;
using Microsoft.UI.Dispatching;
using FakExam.Contracts.Services;
using FakExam.Core.Contracts.Services;
using FakExam.Core.Models;
using System.Diagnostics;

namespace FakExam.Services;

public sealed class ExamNavigationOrchestrator : IExamNavigationOrchestrator, IDisposable
{
    private readonly INavigationService _navigation;
    private readonly IDashboardProfileService _profile;
    private readonly IClockService _clock;
    private Timer? _oneShot;
    private DispatcherQueue? _dq;

    private bool _lastHadInProgress;
    private int _rescheduleBusy = 0;

    public ExamNavigationOrchestrator(
        INavigationService navigation,
        IDashboardProfileService profile,
        IClockService clock)
    {
        _navigation = navigation;
        _profile = profile;
        _clock = clock;
    }

    public void Initialize()
    {
        _dq = App.MainWindow.DispatcherQueue;

        // 记录初始状态
        _lastHadInProgress = _profile.CurrentInProgressExam != null;

        NavigateToCorrectPage(DateTime.Now);
        Reschedule();

        // 订阅事件
        _profile.ProfileChanged += (_, __) => Reschedule();
        _profile.ExamStateChanged += (_, __) =>
        {
            bool nowHasInProgress = _profile.CurrentInProgressExam != null;

            if (_lastHadInProgress && !nowHasInProgress)
            {
                _dq?.TryEnqueue(() =>
                {
                    var currentType = _navigation.Frame?.Content?.GetType();
                    if (currentType != typeof(FakExam.Views.DashboardShowPage))
                    {
                        Trace.WriteLine("[Orch] Edge: InProgress -> None. Navigate => Dashboard");
                        _navigation.NavigateTo(typeof(FakExam.ViewModels.DashboardShowViewModel).FullName!);
                    }
                });
            }
            else if (!_lastHadInProgress && nowHasInProgress)
            {
                _dq?.TryEnqueue(() =>
                {
                    var currentType = _navigation.Frame?.Content?.GetType();
                    if (currentType != typeof(FakExam.Views.TimeShowPage))
                    {
                        _navigation.NavigateTo(typeof(FakExam.ViewModels.TimeShowViewModel).FullName!);
                    }
                });
            }

            _lastHadInProgress = nowHasInProgress;

            Reschedule();
        };
    }

    private void Reschedule()
    {
        if (Interlocked.Exchange(ref _rescheduleBusy, 1) == 1) return;
        try
        {
            _oneShot?.Dispose();

            var now = DateTime.Now;
            var next = GetNextBoundaryTime(_profile.CurrentInProgressExam, _profile.NextUpcomingExam);

            if (next == null)
            {
                return; 
            }

            var due = next.Value - now;

            if (due <= TimeSpan.Zero)
            {
                return;
            }

            // clamp (~24.8 天)
            var maxDue = TimeSpan.FromMilliseconds(uint.MaxValue - 1);
            if (due > maxDue) due = maxDue;


            _oneShot = new Timer(_ =>
            {
                _dq?.TryEnqueue(() =>
                {
                    Trace.WriteLine("[Orch] Timer fired -> Align & chain");
                    NavigateToCorrectPage(DateTime.Now);
                    Reschedule();
                });
            }, null, due, Timeout.InfiniteTimeSpan);
        }
        finally
        {
            Interlocked.Exchange(ref _rescheduleBusy, 0);
        }
    }

    private static DateTime? GetNextBoundaryTime(ExamInfo? inProgress, ExamInfo? nextUpcoming)
    {
        if (inProgress != null) return inProgress.EndTime;
        if (nextUpcoming != null) return nextUpcoming.StartTime;
        return null;
    }

    private void NavigateToCorrectPage(DateTime now)
    {
        bool shouldTime = _profile.CurrentInProgressExam != null;
        var currentType = _navigation.Frame?.Content?.GetType();
        bool isTimeShow = currentType == typeof(FakExam.Views.TimeShowPage);
        bool isDashboard = currentType == typeof(FakExam.Views.DashboardShowPage);

        if (shouldTime && !isTimeShow)
        {
            _navigation.NavigateTo(typeof(FakExam.ViewModels.TimeShowViewModel).FullName!);
        }
        else if (!shouldTime && !isDashboard)
        {
            _navigation.NavigateTo(typeof(FakExam.ViewModels.DashboardShowViewModel).FullName!);
        }
    }

    public void Dispose() => _oneShot?.Dispose();
}
