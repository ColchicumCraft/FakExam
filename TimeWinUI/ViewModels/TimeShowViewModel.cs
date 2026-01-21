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
using TimeWinUI.Models;

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
    [NotifyPropertyChangedFor(nameof(IsTimeDisplayVisible))]
    [NotifyPropertyChangedFor(nameof(IsSettingsPanelVisible))]
    private bool _isSettingsMode = false;

    public bool IsTimeDisplayVisible => !IsSettingsMode;
    public bool IsSettingsPanelVisible => IsSettingsMode;

    [ObservableProperty]
    private ObservableCollection<DisplayItem> _activeDisplayItems = new();

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

    // 对齐相关属性
    [ObservableProperty]
    private string _selectedTimeAlignment = "Center";

    [ObservableProperty]
    private string _selectedDateAlignment = "Center";

    [ObservableProperty]
    private string _selectedWeekAlignment = "Center";

    // 布局顺序属性
    [ObservableProperty]
    private string _selectedLayoutOrder = "DateOnTop";

    // 画中画模式下
    [ObservableProperty]
    private bool _isCompactOverlay = false;

    // 全屏模式下
    [ObservableProperty]
    private bool _isFullScreen = false;

    // 当前有效的显示设置（用于非预览模式）
    private TimeDisplaySettings _currentSettings = new();

    // 预览模式的设置（编辑中的临时设置）
    private TimeDisplaySettings _previewSettings = new();

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
        new FontWeightItem("Thin", "Thin"),
        new FontWeightItem("ExtraLight", "ExtraLight"),
        new FontWeightItem("Light", "Light"),
        new FontWeightItem("Normal", "Normal"),
        new FontWeightItem("Medium", "Medium"),
        new FontWeightItem("SemiBold", "SemiBold"),
        new FontWeightItem("Bold", "Bold"),
        new FontWeightItem("ExtraBold", "ExtraBold"),
        new FontWeightItem("Black", "Black")
    };

    public ObservableCollection<AlignmentItem> AlignmentOptions
    {
        get;
    } = new()
    {
        new AlignmentItem("居中", "Center"),
        new AlignmentItem("靠左", "Left"),
        new AlignmentItem("靠右", "Right"),
        new AlignmentItem("隐藏", "Hidden")
    };

    public ObservableCollection<LayoutOrderItem> LayoutOrderOptions
    {
        get;
    } = new()
    {
        new LayoutOrderItem("日期在上", "DateOnTop"),
        new LayoutOrderItem("时间在上", "TimeOnTop")
    };

    public TimeShowViewModel(ITimeDisplayService timeDisplayService)
    {
        _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        _timeDisplayService = timeDisplayService;

        // 初始化
        LoadDisplaySettings();
        UpdateTime(DateTime.Now);
        SwitchToDisplayMode(); // 初始切换到显示模式
        StartTimer();
    }

    private async void LoadDisplaySettings()
    {
        await _timeDisplayService.InitializeAsync();
        _currentSettings = _timeDisplayService.CurrentSettings;
        ApplySettingsToViewModel(_currentSettings);
    }

    private void ApplySettingsToViewModel(TimeDisplaySettings settings)
    {
        // 应用当前显示设置到ViewModel属性
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

        // 应用对齐设置
        SelectedTimeAlignment = GetAlignmentDisplayName(settings.Alignment.TimeAlignment);
        SelectedDateAlignment = GetAlignmentDisplayName(settings.Alignment.DateAlignment);
        SelectedWeekAlignment = GetAlignmentDisplayName(settings.Alignment.WeekAlignment);

        // 应用布局顺序
        SelectedLayoutOrder = settings.LayoutOrder == LayoutOrder.DateOnTop ? "DateOnTop" : "TimeOnTop";

        // 更新预览设置（初始与当前设置相同）
        _previewSettings = DeepCloneSettings(settings);
    }

    private TimeDisplaySettings DeepCloneSettings(TimeDisplaySettings source)
    {
        return new TimeDisplaySettings
        {
            TimeFormat = new TimeFormatSettings
            {
                Format = source.TimeFormat.Format,
                Use24Hour = source.TimeFormat.Use24Hour,
                ShowSeconds = source.TimeFormat.ShowSeconds,
                CustomFormat = source.TimeFormat.CustomFormat
            },
            DateFormat = new DateFormatSettings
            {
                Format = source.DateFormat.Format,
                ShowWeek = source.DateFormat.ShowWeek,
                ShowYear = source.DateFormat.ShowYear,
                CustomFormat = source.DateFormat.CustomFormat
            },
            TimeFont = new FontSettings
            {
                FontFamily = source.TimeFont.FontFamily,
                FontSize = source.TimeFont.FontSize,
                FontWeight = source.TimeFont.FontWeight,
                FontColor = source.TimeFont.FontColor
            },
            DateFont = new FontSettings
            {
                FontFamily = source.DateFont.FontFamily,
                FontSize = source.DateFont.FontSize,
                FontWeight = source.DateFont.FontWeight,
                FontColor = source.DateFont.FontColor
            },
            Alignment = new DisplayAlignmentSettings
            {
                TimeAlignment = source.Alignment.TimeAlignment,
                DateAlignment = source.Alignment.DateAlignment,
                WeekAlignment = source.Alignment.WeekAlignment
            },
            LayoutOrder = source.LayoutOrder
        };
    }

    private string GetAlignmentDisplayName(TimeAlignment alignment)
    {
        return alignment switch
        {
            TimeAlignment.Left => "Left",
            TimeAlignment.Center => "Center",
            TimeAlignment.Right => "Right",
            TimeAlignment.Hidden => "Hidden",
            _ => "Center"
        };
    }

    private string GetAlignmentDisplayName(DateAlignment alignment)
    {
        return alignment switch
        {
            DateAlignment.Left => "Left",
            DateAlignment.Center => "Center",
            DateAlignment.Right => "Right",
            DateAlignment.Hidden => "Hidden",
            _ => "Center"
        };
    }

    private string GetAlignmentDisplayName(WeekAlignment alignment)
    {
        return alignment switch
        {
            WeekAlignment.Left => "Left",
            WeekAlignment.Center => "Center",
            WeekAlignment.Right => "Right",
            WeekAlignment.Hidden => "Hidden",
            _ => "Center"
        };
    }

    private TimeAlignment GetTimeAlignmentValue(string displayName)
    {
        return displayName switch
        {
            "Left" => TimeAlignment.Left,
            "Center" => TimeAlignment.Center,
            "Right" => TimeAlignment.Right,
            "Hidden" => TimeAlignment.Hidden,
            _ => TimeAlignment.Center
        };
    }

    private DateAlignment GetDateAlignmentValue(string displayName)
    {
        return displayName switch
        {
            "Left" => DateAlignment.Left,
            "Center" => DateAlignment.Center,
            "Right" => DateAlignment.Right,
            "Hidden" => DateAlignment.Hidden,
            _ => DateAlignment.Center
        };
    }

    private WeekAlignment GetWeekAlignmentValue(string displayName)
    {
        return displayName switch
        {
            "Left" => WeekAlignment.Left,
            "Center" => WeekAlignment.Center,
            "Right" => WeekAlignment.Right,
            "Hidden" => WeekAlignment.Hidden,
            _ => WeekAlignment.Center
        };
    }

    private HorizontalAlignment GetHorizontalAlignment(string alignment)
    {
        return alignment switch
        {
            "Left" => HorizontalAlignment.Left,
            "Center" => HorizontalAlignment.Center,
            "Right" => HorizontalAlignment.Right,
            "Hidden" => HorizontalAlignment.Center,
            _ => HorizontalAlignment.Center
        };
    }

    private string GetFontWeightDisplayName(int weight)
    {
        return weight switch
        {
            100 => "Thin",
            200 => "ExtraLight",
            300 => "Light",
            400 => "Normal",
            500 => "Medium",
            600 => "SemiBold",
            700 => "Bold",
            800 => "ExtraBold",
            900 => "Black",
            _ => weight <= 400 ? "Normal" : weight <= 600 ? "Medium" : "Bold"
        };
    }

    private int GetFontWeightValue(string displayName)
    {
        return displayName switch
        {
            "Thin" => 100,
            "ExtraLight" => 200,
            "Light" => 300,
            "Normal" => 400,
            "Medium" => 500,
            "SemiBold" => 600,
            "Bold" => 700,
            "ExtraBold" => 800,
            "Black" => 900,
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
                UpdateActiveDisplayItems(); // 更新时间时也更新显示项
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

    // 切换到预览模式
    public void SwitchToPreviewMode()
    {
        IsSettingsMode = true;

        // 更新预览设置
        UpdatePreviewSettings();

        // 使用预览设置更新显示
        UpdateActiveDisplayItems();
    }

    // 切换到显示模式
    public void SwitchToDisplayMode()
    {
        IsSettingsMode = false;

        // 使用当前设置更新显示
        UpdateActiveDisplayItems();
    }

    // 更新活动显示项
    private void UpdateActiveDisplayItems()
    {
        ActiveDisplayItems.Clear();

        var settings = IsSettingsMode ? _previewSettings : _currentSettings;
        var timeText = TimeText;
        var dateText = DateText;
        var weekText = WeekText;

        var timeAlignment = GetHorizontalAlignment(GetAlignmentDisplayName(settings.Alignment.TimeAlignment));
        var dateAlignment = GetHorizontalAlignment(GetAlignmentDisplayName(settings.Alignment.DateAlignment));
        var weekAlignment = GetHorizontalAlignment(GetAlignmentDisplayName(settings.Alignment.WeekAlignment));

        var timeVisibility = settings.Alignment.TimeAlignment == TimeAlignment.Hidden ?
            Visibility.Collapsed : Visibility.Visible;
        var dateVisibility = settings.Alignment.DateAlignment == DateAlignment.Hidden ?
            Visibility.Collapsed : Visibility.Visible;
        var weekVisibility = settings.Alignment.WeekAlignment == WeekAlignment.Hidden ?
            Visibility.Collapsed : Visibility.Visible;

        var timeFontWeight = GetFontWeightDisplayName(settings.TimeFont.FontWeight);
        var dateFontWeight = GetFontWeightDisplayName(settings.DateFont.FontWeight);

        if (settings.LayoutOrder == LayoutOrder.DateOnTop)
        {
            // 日期在上
            ActiveDisplayItems.Add(new DisplayItem
            {
                Type = DisplayItemType.Date,
                DateText = dateText,
                WeekText = weekText,
                DateFontFamily = settings.DateFont.FontFamily,
                DateFontSize = settings.DateFont.FontSize,
                DateFontColor = settings.DateFont.FontColor,
                DateFontWeight = dateFontWeight,
                HorizontalAlignment = dateAlignment,
                Visibility = dateVisibility
            });

            ActiveDisplayItems.Add(new DisplayItem
            {
                Type = DisplayItemType.Time,
                TimeText = timeText,
                TimeFontFamily = settings.TimeFont.FontFamily,
                TimeFontSize = settings.TimeFont.FontSize,
                TimeFontColor = settings.TimeFont.FontColor,
                TimeFontWeight = timeFontWeight,
                HorizontalAlignment = timeAlignment,
                Visibility = timeVisibility
            });
        }
        else
        {
            // 时间在上
            ActiveDisplayItems.Add(new DisplayItem
            {
                Type = DisplayItemType.Time,
                TimeText = timeText,
                TimeFontFamily = settings.TimeFont.FontFamily,
                TimeFontSize = settings.TimeFont.FontSize,
                TimeFontColor = settings.TimeFont.FontColor,
                TimeFontWeight = timeFontWeight,
                HorizontalAlignment = timeAlignment,
                Visibility = timeVisibility
            });

            ActiveDisplayItems.Add(new DisplayItem
            {
                Type = DisplayItemType.Date,
                DateText = dateText,
                WeekText = weekText,
                DateFontFamily = settings.DateFont.FontFamily,
                DateFontSize = settings.DateFont.FontSize,
                DateFontColor = settings.DateFont.FontColor,
                DateFontWeight = dateFontWeight,
                HorizontalAlignment = dateAlignment,
                Visibility = dateVisibility
            });
        }
    }

    // 更新预览设置（基于当前的UI选择）
    private void UpdatePreviewSettings()
    {
        _previewSettings = new TimeDisplaySettings
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
            },
            Alignment = new DisplayAlignmentSettings
            {
                TimeAlignment = GetTimeAlignmentValue(SelectedTimeAlignment),
                DateAlignment = GetDateAlignmentValue(SelectedDateAlignment),
                WeekAlignment = GetWeekAlignmentValue(SelectedWeekAlignment)
            },
            LayoutOrder = SelectedLayoutOrder == "DateOnTop" ? LayoutOrder.DateOnTop : LayoutOrder.TimeOnTop
        };
    }

    [RelayCommand]
    private void ShowSettings()
    {
        SwitchToPreviewMode();
    }

    [RelayCommand]
    private void HideSettings()
    {
        SwitchToDisplayMode();
    }

    [RelayCommand]
    private void ConfirmTimeColor()
    {
        TimeFontColorHex = $"#{SelectedTimeColor.R:X2}{SelectedTimeColor.G:X2}{SelectedTimeColor.B:X2}";
        if (IsSettingsMode)
        {
            UpdatePreviewSettings();
            UpdateActiveDisplayItems();
        }
    }

    [RelayCommand]
    private void ConfirmDateColor()
    {
        DateFontColorHex = $"#{SelectedDateColor.R:X2}{SelectedDateColor.G:X2}{SelectedDateColor.B:X2}";
        if (IsSettingsMode)
        {
            UpdatePreviewSettings();
            UpdateActiveDisplayItems();
        }
    }

    [RelayCommand]
    private async Task ApplySettings()
    {
        try
        {
            // 保存预览设置到当前设置
            _currentSettings = DeepCloneSettings(_previewSettings);

            // 保存到服务
            await _timeDisplayService.SaveSettingsAsync(_currentSettings);

            // 切换到显示模式
            SwitchToDisplayMode();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"保存设置失败: {ex.Message}");
        }
    }

    [RelayCommand]
    private void CancelSettings()
    {
        // 恢复当前设置到ViewModel
        ApplySettingsToViewModel(_currentSettings);

        // 切换到显示模式
        SwitchToDisplayMode();
    }

    [RelayCommand]
    private void RefreshTime()
    {
        UpdateTime(DateTime.Now);
        UpdateActiveDisplayItems();
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
                    IsFullScreen = false;
                }
                else
                {
                    appWindow.SetPresenter(AppWindowPresenterKind.FullScreen);
                    IsFullScreen = true;
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
                    IsCompactOverlay = false;
                }
                else
                {
                    appWindow.SetPresenter(AppWindowPresenterKind.CompactOverlay);
                    IsCompactOverlay = true;
                }
            }
        }
    }

    [RelayCommand]
    private void MinimizeWindow()
    {
        var window = App.MainWindow;
        if (window != null)
        {
            var appWindow = window.AppWindow;
            if (appWindow != null)
            {
                var presenter = appWindow.Presenter as OverlappedPresenter;
                presenter.Minimize();
            }
        }
    }

    [RelayCommand]
    private void NavigateToSettings()
    {
        var navigationService = App.GetService<INavigationService>();
        navigationService.NavigateTo(typeof(SettingsViewModel).FullName!);
    }

    // 属性变化处理
    partial void OnSelectedTimeFormatChanged(string value)
    {
        IsCustomTimeFormat = value == "Custom";
        UpdateTime(DateTime.Now);
        if (IsSettingsMode)
        {
            UpdatePreviewSettings();
            UpdateActiveDisplayItems();
        }
    }

    partial void OnSelectedDateFormatChanged(string value)
    {
        IsCustomDateFormat = value == "Custom";
        UpdateTime(DateTime.Now);
        if (IsSettingsMode)
        {
            UpdatePreviewSettings();
            UpdateActiveDisplayItems();
        }
    }

    partial void OnTimeFontColorHexChanged(string value)
    {
        try
        {
            SelectedTimeColor = ParseColorFromHex(value);
        }
        catch
        {
            SelectedTimeColor = Windows.UI.Color.FromArgb(255, 255, 255, 255);
        }

        if (IsSettingsMode)
        {
            UpdatePreviewSettings();
            UpdateActiveDisplayItems();
        }
    }

    partial void OnDateFontColorHexChanged(string value)
    {
        try
        {
            SelectedDateColor = ParseColorFromHex(value);
        }
        catch
        {
            SelectedDateColor = Windows.UI.Color.FromArgb(255, 204, 204, 204);
        }

        if (IsSettingsMode)
        {
            UpdatePreviewSettings();
            UpdateActiveDisplayItems();
        }
    }

    partial void OnSelectedTimeAlignmentChanged(string value)
    {
        if (IsSettingsMode)
        {
            UpdatePreviewSettings();
            UpdateActiveDisplayItems();
        }
    }

    partial void OnSelectedDateAlignmentChanged(string value)
    {
        if (IsSettingsMode)
        {
            UpdatePreviewSettings();
            UpdateActiveDisplayItems();
        }
    }

    partial void OnSelectedWeekAlignmentChanged(string value)
    {
        if (IsSettingsMode)
        {
            UpdatePreviewSettings();
            UpdateActiveDisplayItems();
        }
    }

    partial void OnSelectedLayoutOrderChanged(string value)
    {
        if (IsSettingsMode)
        {
            UpdatePreviewSettings();
            UpdateActiveDisplayItems();
        }
    }

    partial void OnSelectedTimeFontFamilyChanged(string value)
    {
        if (IsSettingsMode)
        {
            UpdatePreviewSettings();
            UpdateActiveDisplayItems();
        }
    }

    partial void OnSelectedTimeFontSizeChanged(double value)
    {
        if (IsSettingsMode)
        {
            UpdatePreviewSettings();
            UpdateActiveDisplayItems();
        }
    }

    partial void OnSelectedDateFontFamilyChanged(string value)
    {
        if (IsSettingsMode)
        {
            UpdatePreviewSettings();
            UpdateActiveDisplayItems();
        }
    }

    partial void OnSelectedDateFontSizeChanged(double value)
    {
        if (IsSettingsMode)
        {
            UpdatePreviewSettings();
            UpdateActiveDisplayItems();
        }
    }

    partial void OnSelectedTimeFontWeightChanged(string value)
    {
        if (IsSettingsMode)
        {
            UpdatePreviewSettings();
            UpdateActiveDisplayItems();
        }
    }

    partial void OnSelectedDateFontWeightChanged(string value)
    {
        if (IsSettingsMode)
        {
            UpdatePreviewSettings();
            UpdateActiveDisplayItems();
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