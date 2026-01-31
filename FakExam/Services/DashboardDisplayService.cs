using FakExam.Contracts.Services;
using FakExam.Core.Contracts.Services;
using FakExam.Core.Helpers;
using FakExam.Core.Models;
using Microsoft.UI.Xaml;

namespace FakExam.Services;

public class DashboardDisplayService : IDashboardDisplayService
{
    private const string SettingsKey = "DashboardDisplaySettings";
    private readonly ILocalSettingsService _localSettingsService;
    private DashboardDisplaySettings _currentSettings = new();

    public DashboardDisplaySettings CurrentSettings => _currentSettings;

    public DashboardDisplayService(ILocalSettingsService localSettingsService)
    {
        _localSettingsService = localSettingsService;
    }

    public async Task InitializeAsync()
    {
        _currentSettings = await LoadSettingsAsync() ?? new DashboardDisplaySettings();
    }

    public async Task SaveSettingsAsync(DashboardDisplaySettings settings)
    {
        _currentSettings = settings;
        await _localSettingsService.SaveSettingAsync(SettingsKey, settings);
    }

    public async Task<DashboardDisplaySettings> LoadSettingsAsync()
    {
        return await _localSettingsService.ReadSettingAsync<DashboardDisplaySettings>(SettingsKey);
    }

    public GridLength GetStatusPanelWidth()
    {
        return new GridLength(360, GridUnitType.Pixel); 
    }

    public GridLength GetTablePanelWidth()
    {
        return new GridLength(1, GridUnitType.Star); 
    }

    public bool IsStatusOnLeft()
    {
        return _currentSettings.LayoutOrder == DashboardLayoutOrder.StatusOnLeft;
    }

    public bool IsDateColumnVisible() => _currentSettings.ColumnVisibility.ShowDateColumn;
    public bool IsNameColumnVisible() => _currentSettings.ColumnVisibility.ShowNameColumn;
    public bool IsStartColumnVisible() => _currentSettings.ColumnVisibility.ShowStartColumn;
    public bool IsEndColumnVisible() => _currentSettings.ColumnVisibility.ShowEndColumn;
    public bool IsStatusColumnVisible() => _currentSettings.ColumnVisibility.ShowStatusColumn;
}