using Microsoft.UI.Xaml;
using FakExam.Core.Models;

namespace FakExam.Contracts.Services;

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

    public Visibility GetTimeVisibility();
    public Visibility GetDateVisibility();

    LayoutOrder GetLayoutOrder();
    void SetLayoutOrder(LayoutOrder layoutOrder);
}