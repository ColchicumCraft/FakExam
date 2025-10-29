using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Dispatching;
using System;
using System.Threading;

namespace TimeWinUI.ViewModels;

public partial class TimeShowViewModel : ObservableObject, IDisposable
{
    private Timer _timer;
    private readonly DispatcherQueue _dispatcherQueue;

    [ObservableProperty]
    private string _dateText = string.Empty;

    [ObservableProperty]
    private string _timeText = string.Empty;

    [ObservableProperty]
    private string _weekText = string.Empty;

    [ObservableProperty]
    private string _timeDisplayColor = "#0078D4";

    public TimeShowViewModel()
    {
        _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        UpdateTime(DateTime.Now);
        StartTimer();
    }

    private void StartTimer()
    {
        _timer = new Timer(_ =>
        {
            _dispatcherQueue.TryEnqueue(() =>
            {
                UpdateTime(DateTime.Now);
            });
        }, null, 0, 1000);
    }

    private void UpdateTime(DateTime time)
    {
        DateText = time.ToString("yyyy年MM月dd日");
        TimeText = time.ToString("HH:mm:ss");
        WeekText = GetChineseDayOfWeek(time.DayOfWeek);
        UpdateTimeColor(time);
    }

    private string GetChineseDayOfWeek(DayOfWeek dayOfWeek)
    {
        return dayOfWeek switch
        {
            DayOfWeek.Sunday => "星期日",
            DayOfWeek.Monday => "星期一",
            DayOfWeek.Tuesday => "星期二",
            DayOfWeek.Wednesday => "星期三",
            DayOfWeek.Thursday => "星期四",
            DayOfWeek.Friday => "星期五",
            DayOfWeek.Saturday => "星期六",
            _ => ""
        };
    }

    private void UpdateTimeColor(DateTime time)
    {
        int hour = time.Hour;
        TimeDisplayColor = hour >= 6 && hour < 18 ? "#0078D4" : "#4C0099";
    }

    [RelayCommand]
    private void RefreshTime()
    {
        UpdateTime(DateTime.Now);
    }

    public void Unload()
    {
        Dispose();
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}