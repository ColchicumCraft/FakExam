using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TimeWinUI.Contracts.Services;
using TimeWinUI.Core.Models;

namespace TimeWinUI.ViewModels;

public partial class TimeShowViewModel : ObservableObject, IDisposable
{
    private Timer _timer;
    private readonly DispatcherQueue _dispatcherQueue;
    private readonly ITimeDisplayService _timeDisplayService;

    [ObservableProperty]
    private string _dateText = string.Empty;

    [ObservableProperty]
    private string _timeText = string.Empty;

    [ObservableProperty]
    private string _weekText = string.Empty;

    [ObservableProperty]
    private string _timeDisplayColor = "#0078D4";

    [ObservableProperty]
    private string _timeFontFamily = "Segoe UI";

    [ObservableProperty]
    private double _timeFontSize = 72;

    [ObservableProperty]
    private string _dateFontFamily = "Segoe UI";

    [ObservableProperty]
    private double _dateFontSize = 28;

    [ObservableProperty]
    private string _timeFontColor = "#FFFFFF";

    [ObservableProperty]
    private string _dateFontColor = "#CCCCCC";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsTimeDisplayVisible))]
    [NotifyPropertyChangedFor(nameof(IsSettingsPanelVisible))]
    private bool _isSettingsMode = false;
    public bool IsTimeDisplayVisible => !IsSettingsMode;
    public bool IsSettingsPanelVisible => IsSettingsMode;

    [ObservableProperty]
    private string _selectedTimeFormat = "HH:mm:ss";

    [ObservableProperty]
    private bool _isCustomTimeFormat = false;

    [ObservableProperty]
    private string _customTimeFormat = "HH:mm:ss";

    [ObservableProperty]
    private string _selectedDateFormat = "yyyy年MM月dd日";

    [ObservableProperty]
    private bool _isCustomDateFormat = false;

    [ObservableProperty]
    private string _customDateFormat = "yyyy年MM月dd日";

    [ObservableProperty]
    private string _selectedTimeFontFamily = "Segoe UI";

    [ObservableProperty]
    private double _selectedTimeFontSize = 72;

    [ObservableProperty]
    private string _selectedTimeFontWeight = "Bold";

    [ObservableProperty]
    private string _timeFontColorHex = "#FFFFFF";

    [ObservableProperty]
    private string _selectedDateFontFamily = "Segoe UI";

    [ObservableProperty]
    private double _selectedDateFontSize = 28;

    [ObservableProperty]
    private string _selectedDateFontWeight = "Normal";

    [ObservableProperty]
    private string _dateFontColorHex = "#CCCCCC";

    [ObservableProperty]
    private Windows.UI.Color _selectedTimeColor = Windows.UI.Color.FromArgb(255, 255, 255, 255);

    [ObservableProperty]
    private Windows.UI.Color _selectedDateColor = Windows.UI.Color.FromArgb(255, 204, 204, 204);

    // 下拉列表数据源
    public ObservableCollection<string> FontFamilies
    {
        get;
    } = new()
    {
        "Segoe UI",
        "Microsoft YaHei UI",
        "Microsoft YaHei",
        "Arial",
        "Calibri",
        "Consolas",
        "Times New Roman",
        "SimSun",
        "SimHei",
        "KaiTi"
    };

    public ObservableCollection<FormatItem> TimeFormats
    {
        get;
    } = new()
    {
        new FormatItem("24小时制 (HH:mm:ss)", "HH:mm:ss"),
        new FormatItem("24小时制 (HH:mm)", "HH:mm"),
        new FormatItem("12小时制 (h:mm:ss tt)", "h:mm:ss tt"),
        new FormatItem("12小时制 (h:mm tt)", "h:mm tt"),
        new FormatItem("自定义", "Custom")
    };

    public ObservableCollection<FormatItem> DateFormats
    {
        get;
    } = new()
    {
        new FormatItem("yyyy年MM月dd日", "yyyy年MM月dd日"),
        new FormatItem("yyyy-MM-dd", "yyyy-MM-dd"),
        new FormatItem("MM/dd/yyyy", "MM/dd/yyyy"),
        new FormatItem("yyyy年M月d日", "yyyy年M月d日"),
        new FormatItem("自定义", "Custom")
    };

    public ObservableCollection<FontWeightItem> FontWeights
    {
        get;
    } = new()
    {
        new FontWeightItem("细体", "Light"),
        new FontWeightItem("普通", "Normal"),
        new FontWeightItem("中等", "Medium"),
        new FontWeightItem("半粗", "SemiBold"),
        new FontWeightItem("粗体", "Bold")
    };

    public TimeShowViewModel(ITimeDisplayService timeDisplayService)
    {
        _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        _timeDisplayService = timeDisplayService;

        // 加载设置
        LoadDisplaySettings();
        UpdateTime(DateTime.Now);
        StartTimer();
    }

    private async void LoadDisplaySettings()
    {
        await _timeDisplayService.InitializeAsync();
        var settings = _timeDisplayService.CurrentSettings;

        // 应用设置到属性
        ApplySettingsToViewModel(settings);
    }

    private void ApplySettingsToViewModel(TimeDisplaySettings settings)
    {
        // 应用当前显示设置
        TimeFontFamily = settings.TimeFont.FontFamily;
        TimeFontSize = settings.TimeFont.FontSize;
        DateFontFamily = settings.DateFont.FontFamily;
        DateFontSize = settings.DateFont.FontSize;
        TimeFontColor = settings.TimeFont.FontColor;
        DateFontColor = settings.DateFont.FontColor;

        // 应用设置面板的当前值
        SelectedTimeFormat = settings.TimeFormat.Format;
        SelectedTimeFontFamily = settings.TimeFont.FontFamily;
        SelectedTimeFontSize = settings.TimeFont.FontSize;
        SelectedTimeFontWeight = GetFontWeightDisplayName(settings.TimeFont.FontWeight);
        TimeFontColorHex = settings.TimeFont.FontColor;

        SelectedDateFormat = settings.DateFormat.Format;
        SelectedDateFontFamily = settings.DateFont.FontFamily;
        SelectedDateFontSize = settings.DateFont.FontSize;
        SelectedDateFontWeight = GetFontWeightDisplayName(settings.DateFont.FontWeight);
        DateFontColorHex = settings.DateFont.FontColor;

        // 解析颜色
        SelectedTimeColor = ParseColorFromHex(settings.TimeFont.FontColor);
        SelectedDateColor = ParseColorFromHex(settings.DateFont.FontColor);
    }

    private string GetFontWeightDisplayName(int weight)
    {
        return weight switch
        {
            <= 300 => "Light",
            <= 400 => "Normal",
            <= 500 => "Medium",
            <= 600 => "SemiBold",
            _ => "Bold"
        };
    }

    private int GetFontWeightValue(string displayName)
    {
        return displayName switch
        {
            "Light" => 300,
            "Normal" => 400,
            "Medium" => 500,
            "SemiBold" => 600,
            "Bold" => 700,
            _ => 400
        };
    }

    private Windows.UI.Color ParseColorFromHex(string hexColor)
    {
        try
        {
            if (string.IsNullOrEmpty(hexColor) || hexColor.Length != 7 || hexColor[0] != '#')
                return Windows.UI.Color.FromArgb(255, 255, 255, 255);

            return Windows.UI.Color.FromArgb(
                255,
                Convert.ToByte(hexColor.Substring(1, 2), 16),
                Convert.ToByte(hexColor.Substring(3, 2), 16),
                Convert.ToByte(hexColor.Substring(5, 2), 16)
            );
        }
        catch
        {
            return Windows.UI.Color.FromArgb(255, 255, 255, 255);
        }
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
        string timeFormat = IsCustomTimeFormat ? CustomTimeFormat : SelectedTimeFormat;
        string dateFormat = IsCustomDateFormat ? CustomDateFormat : SelectedDateFormat;

        try
        {
            TimeText = time.ToString(timeFormat);
            DateText = time.ToString(dateFormat);
        }
        catch (FormatException)
        {
            TimeText = time.ToString("HH:mm:ss");
            DateText = time.ToString("yyyy年MM月dd日");
        }

        WeekText = _timeDisplayService.GetFormattedWeek(time);
        UpdateTimeColor(time);
    }

    private void UpdateTimeColor(DateTime time)
    {
        int hour = time.Hour;
        TimeDisplayColor = hour >= 6 && hour < 18 ? "#0078D4" : "#4C0099";
    }

    [RelayCommand]
    private void ShowSettings()
    {
        IsSettingsMode = true;
    }

    [RelayCommand]
    private void HideSettings()
    {
        IsSettingsMode = false;
    }



    [RelayCommand]
    private void ConfirmTimeColor()
    {
        TimeFontColorHex = $"#{SelectedTimeColor.R:X2}{SelectedTimeColor.G:X2}{SelectedTimeColor.B:X2}";
    }

    [RelayCommand]
    private void ConfirmDateColor()
    {
        DateFontColorHex = $"#{SelectedDateColor.R:X2}{SelectedDateColor.G:X2}{SelectedDateColor.B:X2}";
    }


    [RelayCommand]
    private async Task ApplySettings()
    {
        try
        {
            var newSettings = new TimeDisplaySettings
            {
                TimeFormat = new TimeFormatSettings
                {
                    Format = IsCustomTimeFormat ? CustomTimeFormat : SelectedTimeFormat,
                    CustomFormat = CustomTimeFormat
                },
                DateFormat = new DateFormatSettings
                {
                    Format = IsCustomDateFormat ? CustomDateFormat : SelectedDateFormat,
                    CustomFormat = CustomDateFormat
                },
                TimeFont = new FontSettings
                {
                    FontFamily = SelectedTimeFontFamily,
                    FontSize = SelectedTimeFontSize,
                    FontWeight = GetFontWeightValue(SelectedTimeFontWeight),
                    FontColor = TimeFontColorHex
                },
                DateFont = new FontSettings
                {
                    FontFamily = SelectedDateFontFamily,
                    FontSize = SelectedDateFontSize,
                    FontWeight = GetFontWeightValue(SelectedDateFontWeight),
                    FontColor = DateFontColorHex
                }
            };

            await _timeDisplayService.SaveSettingsAsync(newSettings);

            // 更新当前显示
            TimeFontFamily = newSettings.TimeFont.FontFamily;
            TimeFontSize = newSettings.TimeFont.FontSize;
            DateFontFamily = newSettings.DateFont.FontFamily;
            DateFontSize = newSettings.DateFont.FontSize;
            TimeFontColor = newSettings.TimeFont.FontColor;
            DateFontColor = newSettings.DateFont.FontColor;

            // 关闭设置面板
            IsSettingsMode = false;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"保存设置失败: {ex.Message}");
        }
    }

    [RelayCommand]
    private void CancelSettings()
    {
        var settings = _timeDisplayService.CurrentSettings;
        ApplySettingsToViewModel(settings);

        IsSettingsMode = false;
    }

    [RelayCommand]
    private void RefreshTime()
    {
        UpdateTime(DateTime.Now);
    }

    [RelayCommand]
    private void ToggleFullScreen()
    {
        var window = App.MainWindow;
        if (window != null)
        {
            var appWindow = window.AppWindow;
            if (appWindow != null)
            {
                if (appWindow.Presenter.Kind == AppWindowPresenterKind.FullScreen)
                {
                    appWindow.SetPresenter(AppWindowPresenterKind.Overlapped);
                }
                else
                {
                    appWindow.SetPresenter(AppWindowPresenterKind.FullScreen);
                }
            }
        }
    }

    [RelayCommand]
    private void CloseApp()
    {
        var window = App.MainWindow;
        window?.Close();
    }

    [RelayCommand]
    private void ToggleCompactOverlay()
    {
        var window = App.MainWindow;
        if (window != null)
        {
            var appWindow = window.AppWindow;
            if (appWindow != null)
            {
                if (appWindow.Presenter.Kind == AppWindowPresenterKind.CompactOverlay)
                {
                    appWindow.SetPresenter(AppWindowPresenterKind.Overlapped);
                }
                else
                {
                    appWindow.SetPresenter(AppWindowPresenterKind.CompactOverlay);
                }
            }
        }
    }

    partial void OnSelectedTimeFormatChanged(string value)
    {
        IsCustomTimeFormat = value == "Custom";
    }

    partial void OnSelectedDateFormatChanged(string value)
    {
        IsCustomDateFormat = value == "Custom";
    }

    partial void OnTimeFontColorHexChanged(string value)
    {
        TimeFontColor = value;
        try
        {
            SelectedTimeColor = ParseColorFromHex(value);
        }
        catch
        {
            SelectedTimeColor = Windows.UI.Color.FromArgb(255, 255, 255, 255);
        }
    }

    partial void OnDateFontColorHexChanged(string value)
    {
        DateFontColor = value;
        try
        {
            SelectedDateColor = ParseColorFromHex(value);
        }
        catch
        {
            SelectedDateColor = Windows.UI.Color.FromArgb(255, 204, 204, 204);
        }
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