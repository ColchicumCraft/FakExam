using Microsoft.UI.Xaml;
using TimeWinUI.Core.Models;

namespace TimeWinUI.Contracts.Services;

public interface ITimeDisplayService
{
    TimeDisplaySettings CurrentSettings
    {
        get;
    }
    Task InitializeAsync();
    Task SaveSettingsAsync(TimeDisplaySettings settings);
    Task<TimeDisplaySettings> LoadSettingsAsync();

    string GetFormattedTime(DateTime time);
    string GetFormattedDate(DateTime time);
    string GetFormattedWeek(DateTime time);

    public HorizontalAlignment GetTimeHorizontalAlignment();
    public HorizontalAlignment GetDateHorizontalAlignment();
    public HorizontalAlignment GetWeekHorizontalAlignment();
    public Visibility GetTimeVisibility();
    public Visibility GetDateVisibility();
    public Visibility GetWeekVisibility();
}