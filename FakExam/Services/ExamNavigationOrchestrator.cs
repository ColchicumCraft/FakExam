
using System;
using System.Threading;
using Microsoft.UI.Dispatching;
using FakExam.Contracts.Services;
using FakExam.Core.Contracts.Services;
using FakExam.Core.Models;

namespace FakExam.Services;

/// <summary>
/// 统一的页面切换编排服务：只在边界时刻（开始/结束）切换一次页面，避免每秒轮询导航。
/// </summary>
public sealed class ExamNavigationOrchestrator : IExamNavigationOrchestrator, IDisposable
{
    private readonly INavigationService _navigation;
    private readonly IDashboardProfileService _profile;
    private readonly IClockService _clock;
    private Timer? _oneShot;
    private readonly DispatcherQueue _dq;

    public ExamNavigationOrchestrator(
        INavigationService navigation,
        IDashboardProfileService profile,
        IClockService clock)
    {
        _navigation = navigation;
        _profile = profile;
        _clock = clock;
        _dq = DispatcherQueue.GetForCurrentThread();
    }

    public void Initialize()
    {
        // 首次对齐页面
        NavigateToCorrectPage(DateTime.Now);
        Reschedule();

        // 配置变更或状态变化时，重算下一次触发点
        _profile.ProfileChanged += (_, __) => Reschedule();
        _profile.ExamStateChanged += (_, __) => Reschedule();
    }


    private void Reschedule()
    {
        _oneShot?.Dispose();
        var now = DateTime.Now;
        var next = GetNextBoundaryTime(_profile.CurrentInProgressExam, _profile.NextUpcomingExam);
        if (next == null) return;

        var due = next.Value - now;
        if (due < TimeSpan.Zero) due = TimeSpan.Zero;

        // clamp: System.Threading.Timer 上限 ~ 24.8 天
        var maxDue = TimeSpan.FromMilliseconds(uint.MaxValue - 1);
        if (due > maxDue) due = maxDue;

        _oneShot = new Timer(_ =>
        {
            _dq.TryEnqueue(() =>
            {
                NavigateToCorrectPage(DateTime.Now);
                Reschedule();
            });
        }, null, due, Timeout.InfiniteTimeSpan);
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
        var currentType = App.GetService<INavigationService>().Frame?.Content?.GetType();
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
