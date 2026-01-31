using Microsoft.UI.Xaml;
using FakExam.Core.Models;

namespace FakExam.Contracts.Services;

public interface IDashboardDisplayService
{
    DashboardDisplaySettings CurrentSettings
    {
        get;
    }

    Task InitializeAsync();
    Task SaveSettingsAsync(DashboardDisplaySettings settings);
    Task<DashboardDisplaySettings> LoadSettingsAsync();

    // 布局相关
    GridLength GetStatusPanelWidth();
    GridLength GetTablePanelWidth();
    bool IsStatusOnLeft();

    // 列可见性
    bool IsDateColumnVisible();
    bool IsNameColumnVisible();
    bool IsStartColumnVisible();
    bool IsEndColumnVisible();
    bool IsStatusColumnVisible();
}