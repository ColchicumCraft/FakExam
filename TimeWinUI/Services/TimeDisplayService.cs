using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using TimeWinUI.Contracts.Services;
using TimeWinUI.Core.Contracts.Services;
using TimeWinUI.Core.Helpers;
using TimeWinUI.Core.Models;

namespace TimeWinUI.Services;

public class TimeDisplayService : ITimeDisplayService
{
    private const string SettingsKey = "TimeDisplaySettings";
    private readonly ILocalSettingsService _localSettingsService;
    private TimeDisplaySettings _currentSettings = new();

    public TimeDisplaySettings CurrentSettings => _currentSettings;

    public TimeDisplayService(ILocalSettingsService localSettingsService)
    {
        _localSettingsService = localSettingsService;
    }

    public async Task InitializeAsync()
    {
        _currentSettings = await LoadSettingsAsync() ?? new TimeDisplaySettings();
    }

    public async Task SaveSettingsAsync(TimeDisplaySettings settings)
    {
        _currentSettings = settings;
        await _localSettingsService.SaveSettingAsync(SettingsKey, settings);
    }

    public async Task<TimeDisplaySettings> LoadSettingsAsync()
    {
        return await _localSettingsService.ReadSettingAsync<TimeDisplaySettings>(SettingsKey);
    }

    public string GetFormattedTime(DateTime time)
    {
        try
        {
            return time.ToString(_currentSettings.TimeFormat.Format);
        }
        catch
        {
            return time.ToString("HH:mm:ss");
        }
    }

    public string GetFormattedDate(DateTime time)
    {
        try
        {
            return time.ToString(_currentSettings.DateFormat.Format);
        }
        catch
        {
            return time.ToString("yyyy年MM月dd日");
        }
    }

    public string GetFormattedWeek(DateTime time)
    {
        return time.DayOfWeek switch
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

    public HorizontalAlignment GetTimeHorizontalAlignment()
    {
        return _currentSettings.Alignment.TimeAlignment switch
        {
            TimeAlignment.Left => HorizontalAlignment.Left,
            TimeAlignment.Center => HorizontalAlignment.Center,
            TimeAlignment.Right => HorizontalAlignment.Right,
            TimeAlignment.Hidden => HorizontalAlignment.Center,
            _ => HorizontalAlignment.Center
        };
    }

    public HorizontalAlignment GetDateHorizontalAlignment()
    {
        return _currentSettings.Alignment.DateAlignment switch
        {
            DateAlignment.Left => HorizontalAlignment.Left,
            DateAlignment.Center => HorizontalAlignment.Center,
            DateAlignment.Right => HorizontalAlignment.Right,
            DateAlignment.Hidden => HorizontalAlignment.Center,
            _ => HorizontalAlignment.Center
        };
    }

    public HorizontalAlignment GetWeekHorizontalAlignment()
    {
        return _currentSettings.Alignment.WeekAlignment switch
        {
            WeekAlignment.Left => HorizontalAlignment.Left,
            WeekAlignment.Center => HorizontalAlignment.Center,
            WeekAlignment.Right => HorizontalAlignment.Right,
            WeekAlignment.Hidden => HorizontalAlignment.Center,
            _ => HorizontalAlignment.Center
        };
    }

    public Visibility GetTimeVisibility()
    {
        return _currentSettings.Alignment.TimeAlignment == TimeAlignment.Hidden ?
            Visibility.Collapsed : Visibility.Visible;
    }

    public Visibility GetDateVisibility()
    {
        return _currentSettings.Alignment.DateAlignment == DateAlignment.Hidden ?
            Visibility.Collapsed : Visibility.Visible;
    }

    public Visibility GetWeekVisibility()
    {
        return _currentSettings.Alignment.WeekAlignment == WeekAlignment.Hidden ?
            Visibility.Collapsed : Visibility.Visible;
    }

    public LayoutOrder GetLayoutOrder()
    {
        return _currentSettings.LayoutOrder;
    }

    public void SetLayoutOrder(LayoutOrder layoutOrder)
    {
        _currentSettings.LayoutOrder = layoutOrder;
    }
}