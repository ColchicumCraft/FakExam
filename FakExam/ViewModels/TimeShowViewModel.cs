using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Dispatching;
using FakExam.Contracts.Services;
using FakExam.Core.Contracts.Services;
using FakExam.Core.Models;
using FakExam.Models;

namespace FakExam.ViewModels;

public partial class TimeShowViewModel : ObservableObject, IDisposable
{
    private readonly DispatcherQueue _dispatcherQueue;
    private readonly ITimeDisplayService _timeDisplayService;
    private readonly IDashboardProfileService _profileService;
    private readonly INavigationService _navigationService;
    private readonly IClockService _clock;

    [ObservableProperty] private string _dateText = string.Empty;
    [ObservableProperty] private string _timeText = string.Empty;
    [ObservableProperty] private string _weekText = string.Empty;
    [ObservableProperty] private string _timeDisplayColor = "#0078D4";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsTimeDisplayVisible))]
    [NotifyPropertyChangedFor(nameof(IsSettingsPanelVisible))]
    private bool _isSettingsMode = false;
    public bool IsTimeDisplayVisible => !IsSettingsMode;
    public bool IsSettingsPanelVisible => IsSettingsMode;

    [ObservableProperty] private ObservableCollection<DisplayItem> _activeDisplayItems = new();

    [ObservableProperty] private string _selectedTimeFormat = "HH:mm:ss";
    [ObservableProperty] private bool _isCustomTimeFormat = false;
    [ObservableProperty] private string _customTimeFormat = "HH:mm:ss";
    [ObservableProperty] private string _selectedDateFormat = "yyyy年MM月dd日";
    [ObservableProperty] private bool _isCustomDateFormat = false;
    [ObservableProperty] private string _customDateFormat = "yyyy年MM月dd日";

    [ObservableProperty] private string _selectedTimeFontFamily = "Segoe UI";
    [ObservableProperty] private double _selectedTimeFontSize = 72;
    [ObservableProperty] private string _selectedTimeFontWeight = "Bold";
    [ObservableProperty] private string _timeFontColorHex = "#FFFFFF";
    [ObservableProperty] private string _selectedDateFontFamily = "Segoe UI";
    [ObservableProperty] private double _selectedDateFontSize = 28;
    [ObservableProperty] private string _selectedDateFontWeight = "Normal";
    [ObservableProperty] private string _dateFontColorHex = "#CCCCCC";
    [ObservableProperty] private string _selectedExamAlignment = "Center";

    [ObservableProperty] private Windows.UI.Color _selectedTimeColor = Windows.UI.Color.FromArgb(255, 255, 255, 255);
    [ObservableProperty] private Windows.UI.Color _selectedDateColor = Windows.UI.Color.FromArgb(255, 204, 204, 204);
    [ObservableProperty] private string _selectedTimeAlignment = "Center";
    [ObservableProperty] private string _selectedDateAlignment = "Center";
    [ObservableProperty] private string _selectedLayoutOrder = "DateOnTop";
    [ObservableProperty] private ExamLayoutPosition _selectedExamPosition = ExamLayoutPosition.Bottom;
    [ObservableProperty] private bool _isCompactOverlay = false;
    [ObservableProperty] private bool _isFullScreen = false;
    [ObservableProperty] private double itemsSpacing = 20;

    private TimeDisplaySettings _currentSettings = new();
    private TimeDisplaySettings _previewSettings = new();

    // 底部考试叠层
    [ObservableProperty] private bool _isExamOverlayVisible = false;
    [ObservableProperty] private string _overlayExamName = "—";
    [ObservableProperty] private string _overlayStartTimeText = "—";
    [ObservableProperty] private string _overlayEndTimeText = "—";
    [ObservableProperty] private string _overlayStatusText = "—";
    [ObservableProperty] private string _overlayRemainingText = "—";

    [ObservableProperty] private string _examLabelFontFamily = "Segoe UI";
    [ObservableProperty] private double _examLabelFontSize = 12;
    [ObservableProperty] private string _examLabelFontWeight = "Normal";
    [ObservableProperty] private string _examLabelFontColorHex = "#8A8A8A";
    [ObservableProperty] private Windows.UI.Color _selectedExamLabelColor = Windows.UI.Color.FromArgb(255,138,138,138);

    [ObservableProperty] private string _examStatusFontFamily = "Segoe UI";
    [ObservableProperty] private double _examStatusFontSize = 16;
    [ObservableProperty] private string _examStatusFontWeight = "SemiBold";
    [ObservableProperty] private string _examStatusFontColorHex = "#FFFFFF";
    [ObservableProperty] private Windows.UI.Color _selectedExamStatusColor = Windows.UI.Color.FromArgb(255,255,255,255);

    [ObservableProperty] private string _examStartFontFamily = "Segoe UI";
    [ObservableProperty] private double _examStartFontSize = 16;
    [ObservableProperty] private string _examStartFontWeight = "Normal";
    [ObservableProperty] private string _examStartFontColorHex = "#FFFFFF";
    [ObservableProperty] private Windows.UI.Color _selectedExamStartColor = Windows.UI.Color.FromArgb(255,255,255,255);

    [ObservableProperty] private string _examNameFontFamily = "Segoe UI";
    [ObservableProperty] private double _examNameFontSize = 18;
    [ObservableProperty] private string _examNameFontWeight = "SemiBold";
    [ObservableProperty] private string _examNameFontColorHex = "#FFFFFF";
    [ObservableProperty] private Windows.UI.Color _selectedExamNameColor = Windows.UI.Color.FromArgb(255,255,255,255);

    [ObservableProperty] private string _examEndFontFamily = "Segoe UI";
    [ObservableProperty] private double _examEndFontSize = 16;
    [ObservableProperty] private string _examEndFontWeight = "Normal";
    [ObservableProperty] private string _examEndFontColorHex = "#FFFFFF";
    [ObservableProperty] private Windows.UI.Color _selectedExamEndColor = Windows.UI.Color.FromArgb(255,255,255,255);

    [ObservableProperty] private string _examRemainingFontFamily = "Segoe UI";
    [ObservableProperty] private double _examRemainingFontSize = 16;
    [ObservableProperty] private string _examRemainingFontWeight = "SemiBold";
    [ObservableProperty] private string _examRemainingFontColorHex = "#FFFFFF";
    [ObservableProperty] private Windows.UI.Color _selectedExamRemainingColor = Windows.UI.Color.FromArgb(255,255,255,255);

    public ObservableCollection<string> FontFamilies => DisplayDataSources.FontFamilies;
    public ObservableCollection<FormatItem> TimeFormats => DisplayDataSources.TimeFormats;
    public ObservableCollection<FormatItem> DateFormats => DisplayDataSources.DateFormats;
    public ObservableCollection<FontWeightItem> FontWeights => DisplayDataSources.FontWeights;
    public ObservableCollection<AlignmentItem> AlignmentOptions => DisplayDataSources.AlignmentOptions;
    public ObservableCollection<LayoutOrderItem> LayoutOrderOptions => DisplayDataSources.LayoutOrderOptions;
    public ObservableCollection<ExamPositionItem> ExamPositionOptions => DisplayDataSources.ExamPositionOptions;

    public TimeShowViewModel(ITimeDisplayService timeDisplayService,
                             IDashboardProfileService profileService,
                             INavigationService navigationService,
                             IClockService clock)
    {
        _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        _timeDisplayService = timeDisplayService;
        _profileService = profileService;
        _navigationService = navigationService;
        _clock = clock;

        LoadDisplaySettings();
        UpdateTime(DateTime.Now);
        SwitchToDisplayMode();

        // 统一使用 ClockService 驱动
        _clock.Tick += OnTick;

        // 配置变化时刷新叠层
        _profileService.ProfileChanged += (_, __) => _dispatcherQueue.TryEnqueue(() => UpdateExamOverlay(DateTime.Now));
        _profileService.ExamStateChanged += (_, __) => _dispatcherQueue.TryEnqueue(() => UpdateExamOverlay(DateTime.Now));
    }

    private async void LoadDisplaySettings()
    {
        await _timeDisplayService.InitializeAsync();
        _currentSettings = _timeDisplayService.CurrentSettings;
        ApplySettingsToViewModel(_currentSettings);
    }

    private void OnTick(object? s, DateTime now)
    {
        _dispatcherQueue.TryEnqueue(() =>
        {
            UpdateTime(now);
            UpdateActiveDisplayItems();
            UpdateExamOverlay(now);
        });
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

    public void SwitchToPreviewMode()
    {
        IsSettingsMode = true;
        UpdatePreviewSettings();
        UpdateActiveDisplayItems();
    }

    public void SwitchToDisplayMode()
    {
        IsSettingsMode = false;
        UpdateActiveDisplayItems();
    }

    private void UpdateActiveDisplayItems()
    {
        ActiveDisplayItems.Clear();
        var settings = IsSettingsMode ? _previewSettings : _currentSettings;

        var timeAlignment = GetHorizontalAlignment(GetAlignmentDisplayName(settings.Alignment.TimeAlignment));
        var dateAlignment = GetHorizontalAlignment(GetAlignmentDisplayName(settings.Alignment.DateAlignment));
        var examAlignment = GetHorizontalAlignment(GetAlignmentDisplayName(settings.Alignment.ExamAlignment));

        var timeVisibility = settings.Alignment.TimeAlignment == Alignments.Hidden ? Visibility.Collapsed : Visibility.Visible;
        var dateVisibility = settings.Alignment.DateAlignment == Alignments.Hidden ? Visibility.Collapsed : Visibility.Visible;
        var examVisibility = settings.Alignment.ExamAlignment == Alignments.Hidden ? Visibility.Collapsed : Visibility.Visible;

        var timeFontWeight = GetFontWeightDisplayName(settings.TimeFont.FontWeight);
        var dateFontWeight = GetFontWeightDisplayName(settings.DateFont.FontWeight);

        var timeDisplayItem = new DisplayItem
        {
            Type = DisplayItemType.Time,
            TimeText = TimeText,
            TimeFontFamily = settings.TimeFont.FontFamily,
            TimeFontSize = settings.TimeFont.FontSize,
            TimeFontColor = settings.TimeFont.FontColor,
            TimeFontWeight = timeFontWeight,
            HorizontalAlignment = timeAlignment,
            Visibility = timeVisibility
        };

        var dateDisplayItem = new DisplayItem
        {
            Type = DisplayItemType.Date,
            DateText = DateText,
            WeekText = WeekText,
            DateFontFamily = settings.DateFont.FontFamily,
            DateFontSize = settings.DateFont.FontSize,
            DateFontColor = settings.DateFont.FontColor,
            DateFontWeight = dateFontWeight,
            HorizontalAlignment = dateAlignment,
            Visibility = dateVisibility
        };

        var examDisplayItem = new DisplayItem
        {
            Type = DisplayItemType.Exam,
            HorizontalAlignment = examAlignment,
            Visibility = examVisibility
        };

        List<DisplayItem> orderedItems = new();

        switch (settings.ExamLayoutPosition)
        {
            case ExamLayoutPosition.Top:
                orderedItems.Add(examDisplayItem);
                if (settings.LayoutOrder == LayoutOrder.DateOnTop)
                {
                    orderedItems.Add(dateDisplayItem);
                    orderedItems.Add(timeDisplayItem);
                }
                else
                {
                    orderedItems.Add(timeDisplayItem);
                    orderedItems.Add(dateDisplayItem);
                }
                break;

            case ExamLayoutPosition.Middle:
                if (settings.LayoutOrder == LayoutOrder.DateOnTop)
                {
                    orderedItems.Add(dateDisplayItem);
                    orderedItems.Add(examDisplayItem);
                    orderedItems.Add(timeDisplayItem);
                }
                else
                {
                    orderedItems.Add(timeDisplayItem);
                    orderedItems.Add(examDisplayItem);
                    orderedItems.Add(dateDisplayItem);
                }
                break;

            case ExamLayoutPosition.Bottom:
                if (settings.LayoutOrder == LayoutOrder.DateOnTop)
                {
                    orderedItems.Add(dateDisplayItem);
                    orderedItems.Add(timeDisplayItem);
                    orderedItems.Add(examDisplayItem);
                }
                else
                {
                    orderedItems.Add(timeDisplayItem);
                    orderedItems.Add(dateDisplayItem);
                    orderedItems.Add(examDisplayItem);
                }
                break;
        }

        foreach (var item in orderedItems)
        {
            ActiveDisplayItems.Add(item);
        }
    }

    private void UpdatePreviewSettings()
    {
        _previewSettings = new TimeDisplaySettings
        {
            ItemsSpacing = ItemsSpacing,
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
                TimeAlignment = GetAlignmentValue(SelectedTimeAlignment),
                DateAlignment = GetAlignmentValue(SelectedDateAlignment),
                ExamAlignment = GetAlignmentValue(SelectedExamAlignment)
            },
            LayoutOrder = SelectedLayoutOrder == "DateOnTop" ? LayoutOrder.DateOnTop : LayoutOrder.TimeOnTop,
            ExamLayoutPosition = SelectedExamPosition,

            ExamOverlay = new ExamOverlaySettings
            {
                LabelFont = new FontSettings
                {
                    FontFamily = ExamLabelFontFamily,
                    FontSize = ExamLabelFontSize,
                    FontWeight = GetFontWeightValue(ExamLabelFontWeight),
                    FontColor = ExamLabelFontColorHex
                },
                StatusFont = new FontSettings
                {
                    FontFamily = ExamStatusFontFamily,
                    FontSize = ExamStatusFontSize,
                    FontWeight = GetFontWeightValue(ExamStatusFontWeight),
                    FontColor = ExamStatusFontColorHex
                },
                StartTimeFont = new FontSettings
                {
                    FontFamily = ExamStartFontFamily,
                    FontSize = ExamStartFontSize,
                    FontWeight = GetFontWeightValue(ExamStartFontWeight),
                    FontColor = ExamStartFontColorHex
                },
                NameFont = new FontSettings
                {
                    FontFamily = ExamNameFontFamily,
                    FontSize = ExamNameFontSize,
                    FontWeight = GetFontWeightValue(ExamNameFontWeight),
                    FontColor = ExamNameFontColorHex
                },
                EndTimeFont = new FontSettings
                {
                    FontFamily = ExamEndFontFamily,
                    FontSize = ExamEndFontSize,
                    FontWeight = GetFontWeightValue(ExamEndFontWeight),
                    FontColor = ExamEndFontColorHex
                },
                RemainingFont = new FontSettings
                {
                    FontFamily = ExamRemainingFontFamily,
                    FontSize = ExamRemainingFontSize,
                    FontWeight = GetFontWeightValue(ExamRemainingFontWeight),
                    FontColor = ExamRemainingFontColorHex
                }
            }
        };
    }

    [RelayCommand] private void ShowSettings() => SwitchToPreviewMode();
    [RelayCommand] private void HideSettings() => SwitchToDisplayMode();

    [RelayCommand]
    private void ConfirmTimeColor()
    {
        TimeFontColorHex = $"#{SelectedTimeColor.R:X2}{SelectedTimeColor.G:X2}{SelectedTimeColor.B:X2}";
        if (IsSettingsMode) { UpdatePreviewSettings(); UpdateActiveDisplayItems(); }
    }

    [RelayCommand]
    private void ConfirmDateColor()
    {
        DateFontColorHex = $"#{SelectedDateColor.R:X2}{SelectedDateColor.G:X2}{SelectedDateColor.B:X2}";
        if (IsSettingsMode) { UpdatePreviewSettings(); UpdateActiveDisplayItems(); }
    }

    [RelayCommand] private void ConfirmExamLabelColor()
    {
        ExamLabelFontColorHex = $"#{SelectedExamLabelColor.R:X2}{SelectedExamLabelColor.G:X2}{SelectedExamLabelColor.B:X2}";
        if (IsSettingsMode) { UpdatePreviewSettings(); UpdateActiveDisplayItems(); }
    }
    [RelayCommand] private void ConfirmExamStatusColor()
    {
        ExamStatusFontColorHex = $"#{SelectedExamStatusColor.R:X2}{SelectedExamStatusColor.G:X2}{SelectedExamStatusColor.B:X2}";
        if (IsSettingsMode) { UpdatePreviewSettings(); UpdateActiveDisplayItems(); }
    }
    [RelayCommand] private void ConfirmExamStartColor()
    {
        ExamStartFontColorHex = $"#{SelectedExamStartColor.R:X2}{SelectedExamStartColor.G:X2}{SelectedExamStartColor.B:X2}";
        if (IsSettingsMode) { UpdatePreviewSettings(); UpdateActiveDisplayItems(); }
    }
    [RelayCommand] private void ConfirmExamNameColor()
    {
        ExamNameFontColorHex = $"#{SelectedExamNameColor.R:X2}{SelectedExamNameColor.G:X2}{SelectedExamNameColor.B:X2}";
        if (IsSettingsMode) { UpdatePreviewSettings(); UpdateActiveDisplayItems(); }
    }
    [RelayCommand] private void ConfirmExamEndColor()
    {
        ExamEndFontColorHex = $"#{SelectedExamEndColor.R:X2}{SelectedExamEndColor.G:X2}{SelectedExamEndColor.B:X2}";
        if (IsSettingsMode) { UpdatePreviewSettings(); UpdateActiveDisplayItems(); }
    }
    [RelayCommand] private void ConfirmExamRemainingColor()
    {
        ExamRemainingFontColorHex = $"#{SelectedExamRemainingColor.R:X2}{SelectedExamRemainingColor.G:X2}{SelectedExamRemainingColor.B:X2}";
        if (IsSettingsMode) { UpdatePreviewSettings(); UpdateActiveDisplayItems(); }
    }

    [RelayCommand]
    private async System.Threading.Tasks.Task ApplySettings()
    {
        try
        {
            _currentSettings = DeepCloneSettings(_previewSettings);
            await _timeDisplayService.SaveSettingsAsync(_currentSettings);
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
        ApplySettingsToViewModel(_currentSettings);
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
                presenter?.Minimize();
            }
        }
    }

    [RelayCommand]
    private void NavigateToSettings()
    {
        var navigationService = App.GetService<INavigationService>();
        navigationService.NavigateTo(typeof(SettingsViewModel).FullName!);
    }

    [RelayCommand]
    private void NavigateToDashboardShowPage()
    {
        var navigationService = App.GetService<INavigationService>();
        navigationService.NavigateTo(typeof(DashboardShowViewModel).FullName!);
    }

    private void ApplySettingsToViewModel(TimeDisplaySettings settings)
    {
        ItemsSpacing = settings.ItemsSpacing;

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

        SelectedTimeColor = ParseColorFromHex(settings.TimeFont.FontColor);
        SelectedDateColor = ParseColorFromHex(settings.DateFont.FontColor);

        SelectedTimeAlignment = GetAlignmentDisplayName(settings.Alignment.TimeAlignment);
        SelectedDateAlignment = GetAlignmentDisplayName(settings.Alignment.DateAlignment);
        SelectedLayoutOrder = settings.LayoutOrder == LayoutOrder.DateOnTop ? "DateOnTop" : "TimeOnTop";

        SelectedExamAlignment = GetAlignmentDisplayName(settings.Alignment.ExamAlignment);
        SelectedExamPosition = settings.ExamLayoutPosition;
        // 考试叠层
        settings.ExamOverlay ??= new ExamOverlaySettings();
        ExamLabelFontFamily   = settings.ExamOverlay.LabelFont.FontFamily;
        ExamLabelFontSize     = settings.ExamOverlay.LabelFont.FontSize;
        ExamLabelFontWeight   = GetFontWeightDisplayName(settings.ExamOverlay.LabelFont.FontWeight);
        ExamLabelFontColorHex = settings.ExamOverlay.LabelFont.FontColor;
        SelectedExamLabelColor = ParseColorFromHex(settings.ExamOverlay.LabelFont.FontColor);

        ExamStatusFontFamily   = settings.ExamOverlay.StatusFont.FontFamily;
        ExamStatusFontSize     = settings.ExamOverlay.StatusFont.FontSize;
        ExamStatusFontWeight   = GetFontWeightDisplayName(settings.ExamOverlay.StatusFont.FontWeight);
        ExamStatusFontColorHex = settings.ExamOverlay.StatusFont.FontColor;
        SelectedExamStatusColor = ParseColorFromHex(settings.ExamOverlay.StatusFont.FontColor);

        ExamStartFontFamily   = settings.ExamOverlay.StartTimeFont.FontFamily;
        ExamStartFontSize     = settings.ExamOverlay.StartTimeFont.FontSize;
        ExamStartFontWeight   = GetFontWeightDisplayName(settings.ExamOverlay.StartTimeFont.FontWeight);
        ExamStartFontColorHex = settings.ExamOverlay.StartTimeFont.FontColor;
        SelectedExamStartColor = ParseColorFromHex(settings.ExamOverlay.StartTimeFont.FontColor);

        ExamNameFontFamily   = settings.ExamOverlay.NameFont.FontFamily;
        ExamNameFontSize     = settings.ExamOverlay.NameFont.FontSize;
        ExamNameFontWeight   = GetFontWeightDisplayName(settings.ExamOverlay.NameFont.FontWeight);
        ExamNameFontColorHex = settings.ExamOverlay.NameFont.FontColor;
        SelectedExamNameColor = ParseColorFromHex(settings.ExamOverlay.NameFont.FontColor);

        ExamEndFontFamily   = settings.ExamOverlay.EndTimeFont.FontFamily;
        ExamEndFontSize     = settings.ExamOverlay.EndTimeFont.FontSize;
        ExamEndFontWeight   = GetFontWeightDisplayName(settings.ExamOverlay.EndTimeFont.FontWeight);
        ExamEndFontColorHex = settings.ExamOverlay.EndTimeFont.FontColor;
        SelectedExamEndColor = ParseColorFromHex(settings.ExamOverlay.EndTimeFont.FontColor);

        ExamRemainingFontFamily   = settings.ExamOverlay.RemainingFont.FontFamily;
        ExamRemainingFontSize     = settings.ExamOverlay.RemainingFont.FontSize;
        ExamRemainingFontWeight   = GetFontWeightDisplayName(settings.ExamOverlay.RemainingFont.FontWeight);
        ExamRemainingFontColorHex = settings.ExamOverlay.RemainingFont.FontColor;
        SelectedExamRemainingColor = ParseColorFromHex(settings.ExamOverlay.RemainingFont.FontColor);

        _previewSettings = DeepCloneSettings(settings);
    }

    private TimeDisplaySettings DeepCloneSettings(TimeDisplaySettings source)
    {
        return new TimeDisplaySettings
        {
            ItemsSpacing = source.ItemsSpacing,
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
                ExamAlignment = source.Alignment.ExamAlignment
            },
            LayoutOrder = source.LayoutOrder,
            ExamLayoutPosition = source.ExamLayoutPosition,
            ExamOverlay = new ExamOverlaySettings
            {
                LabelFont = new FontSettings
                {
                    FontFamily = source.ExamOverlay?.LabelFont.FontFamily ?? "Segoe UI",
                    FontSize   = source.ExamOverlay?.LabelFont.FontSize   ?? 12,
                    FontWeight = source.ExamOverlay?.LabelFont.FontWeight ?? 400,
                    FontColor  = source.ExamOverlay?.LabelFont.FontColor  ?? "#8A8A8A"
                },
                StatusFont = new FontSettings
                {
                    FontFamily = source.ExamOverlay?.StatusFont.FontFamily ?? "Segoe UI",
                    FontSize   = source.ExamOverlay?.StatusFont.FontSize   ?? 16,
                    FontWeight = source.ExamOverlay?.StatusFont.FontWeight ?? 600,
                    FontColor  = source.ExamOverlay?.StatusFont.FontColor  ?? "#FFFFFF"
                },
                StartTimeFont = new FontSettings
                {
                    FontFamily = source.ExamOverlay?.StartTimeFont.FontFamily ?? "Segoe UI",
                    FontSize   = source.ExamOverlay?.StartTimeFont.FontSize   ?? 16,
                    FontWeight = source.ExamOverlay?.StartTimeFont.FontWeight ?? 400,
                    FontColor  = source.ExamOverlay?.StartTimeFont.FontColor  ?? "#FFFFFF"
                },
                NameFont = new FontSettings
                {
                    FontFamily = source.ExamOverlay?.NameFont.FontFamily ?? "Segoe UI",
                    FontSize   = source.ExamOverlay?.NameFont.FontSize   ?? 18,
                    FontWeight = source.ExamOverlay?.NameFont.FontWeight ?? 600,
                    FontColor  = source.ExamOverlay?.NameFont.FontColor  ?? "#FFFFFF"
                },
                EndTimeFont = new FontSettings
                {
                    FontFamily = source.ExamOverlay?.EndTimeFont.FontFamily ?? "Segoe UI",
                    FontSize   = source.ExamOverlay?.EndTimeFont.FontSize   ?? 16,
                    FontWeight = source.ExamOverlay?.EndTimeFont.FontWeight ?? 400,
                    FontColor  = source.ExamOverlay?.EndTimeFont.FontColor  ?? "#FFFFFF"
                },
                RemainingFont = new FontSettings
                {
                    FontFamily = source.ExamOverlay?.RemainingFont.FontFamily ?? "Segoe UI",
                    FontSize   = source.ExamOverlay?.RemainingFont.FontSize   ?? 16,
                    FontWeight = source.ExamOverlay?.RemainingFont.FontWeight ?? 600,
                    FontColor  = source.ExamOverlay?.RemainingFont.FontColor  ?? "#FFFFFF"
                }
            }
        };
    }

    private string GetAlignmentDisplayName(Alignments alignment) => alignment switch
    {
        Alignments.Left => "Left",
        Alignments.Center => "Center",
        Alignments.Right => "Right",
        Alignments.Hidden => "Hidden",
        _ => "Center"
    };

    private Alignments GetAlignmentValue(string displayName) => displayName switch
    {
        "Left" => Alignments.Left,
        "Center" => Alignments.Center,
        "Right" => Alignments.Right,
        "Hidden" => Alignments.Hidden,
        _ => Alignments.Center
    };

    private HorizontalAlignment GetHorizontalAlignment(string alignment) => alignment switch
    {
        "Left" => HorizontalAlignment.Left,
        "Center" => HorizontalAlignment.Center,
        "Right" => HorizontalAlignment.Right,
        "Hidden" => HorizontalAlignment.Center,
        _ => HorizontalAlignment.Center
    };

    private string GetFontWeightDisplayName(int weight) => weight switch
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
    private int GetFontWeightValue(string displayName) => displayName switch
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
        catch { return Windows.UI.Color.FromArgb(255, 255, 255, 255); }
    }

    private void UpdateExamOverlay(DateTime now)
    {
        var exam = _profileService.CurrentInProgressExam ?? _profileService.NextUpcomingExam;
        if (exam == null)
        {
            IsExamOverlayVisible = false;
            return;
        }
        IsExamOverlayVisible = true;
        OverlayExamName = exam.Name ?? "—";
        OverlayStartTimeText = exam.StartTime.ToString("HH:mm:ss");
        OverlayEndTimeText = exam.EndTime.ToString("HH:mm:ss");

        var threshold = exam.AlertTime > 0 ? exam.AlertTime : 15;
        var status = EvaluateStatusText(exam.StartTime, exam.EndTime, now, threshold);
        OverlayStatusText = status;
        OverlayRemainingText = status switch
        {
            "进行中" or "即将结束" => FormatTimeSpan(exam.EndTime - now),
            "未开始" or "即将开始" => FormatTimeSpan(exam.StartTime - now),
            _ => "—"
        };
    }

    private static string EvaluateStatusText(DateTime start, DateTime end, DateTime now, int thresholdMinutes)
    {
        if (now >= end) return "已结束";
        if (now < start)
        {
            var toStart = start - now;
            return toStart.TotalMinutes <= thresholdMinutes ? "即将开始" : "未开始";
        }
        var toEnd = end - now;
        return toEnd.TotalMinutes <= thresholdMinutes ? "即将结束" : "进行中";
    }

    private static string FormatTimeSpan(TimeSpan ts)
    {
        if (ts.TotalSeconds < 0) ts = TimeSpan.Zero;
        return ts.TotalHours >= 1
            ? $"{(int)ts.TotalHours:00}:{ts.Minutes:00}:{ts.Seconds:00}"
            : $"{ts.Minutes:00}:{ts.Seconds:00}";
    }

    public void Dispose()
    {
        // 使用全局 ClockService，无需本地 Timer
    }

    partial void OnSelectedTimeFormatChanged(string value)
    {
        IsCustomTimeFormat = value == "Custom";
        UpdateTime(DateTime.Now);
        if (IsSettingsMode) { UpdatePreviewSettings(); UpdateActiveDisplayItems(); }
    }
    partial void OnSelectedDateFormatChanged(string value)
    {
        IsCustomDateFormat = value == "Custom";
        UpdateTime(DateTime.Now);
        if (IsSettingsMode) { UpdatePreviewSettings(); UpdateActiveDisplayItems(); }
    }
    partial void OnTimeFontColorHexChanged(string value)
    {
        try { SelectedTimeColor = ParseColorFromHex(value); }
        catch { SelectedTimeColor = Windows.UI.Color.FromArgb(255, 255, 255, 255); }
        if (IsSettingsMode) { UpdatePreviewSettings(); UpdateActiveDisplayItems(); }
    }
    partial void OnDateFontColorHexChanged(string value)
    {
        try { SelectedDateColor = ParseColorFromHex(value); }
        catch { SelectedDateColor = Windows.UI.Color.FromArgb(255, 204, 204, 204); }
        if (IsSettingsMode) { UpdatePreviewSettings(); UpdateActiveDisplayItems(); }
    }

    partial void OnExamLabelFontFamilyChanged(string value) { if (IsSettingsMode) { UpdatePreviewSettings(); UpdateActiveDisplayItems(); } }
    partial void OnExamLabelFontSizeChanged(double value)  { if (IsSettingsMode) { UpdatePreviewSettings(); UpdateActiveDisplayItems(); } }
    partial void OnExamLabelFontWeightChanged(string value) { if (IsSettingsMode) { UpdatePreviewSettings(); UpdateActiveDisplayItems(); } }
    partial void OnExamLabelFontColorHexChanged(string value)
    {
        try { SelectedExamLabelColor = ParseColorFromHex(value); }
        catch { SelectedExamLabelColor = Windows.UI.Color.FromArgb(255,138,138,138); }
        if (IsSettingsMode) { UpdatePreviewSettings(); UpdateActiveDisplayItems(); }
    }

    partial void OnExamStatusFontFamilyChanged(string value) { if (IsSettingsMode) { UpdatePreviewSettings(); UpdateActiveDisplayItems(); } }
    partial void OnExamStatusFontSizeChanged(double value)  { if (IsSettingsMode) { UpdatePreviewSettings(); UpdateActiveDisplayItems(); } }
    partial void OnExamStatusFontWeightChanged(string value) { if (IsSettingsMode) { UpdatePreviewSettings(); UpdateActiveDisplayItems(); } }
    partial void OnExamStatusFontColorHexChanged(string value)
    {
        try { SelectedExamStatusColor = ParseColorFromHex(value); }
        catch { SelectedExamStatusColor = Windows.UI.Color.FromArgb(255,255,255,255); }
        if (IsSettingsMode) { UpdatePreviewSettings(); UpdateActiveDisplayItems(); }
    }

    partial void OnExamStartFontFamilyChanged(string value) { if (IsSettingsMode) { UpdatePreviewSettings(); UpdateActiveDisplayItems(); } }
    partial void OnExamStartFontSizeChanged(double value)  { if (IsSettingsMode) { UpdatePreviewSettings(); UpdateActiveDisplayItems(); } }
    partial void OnExamStartFontWeightChanged(string value) { if (IsSettingsMode) { UpdatePreviewSettings(); UpdateActiveDisplayItems(); } }
    partial void OnExamStartFontColorHexChanged(string value)
    {
        try { SelectedExamStartColor = ParseColorFromHex(value); }
        catch { SelectedExamStartColor = Windows.UI.Color.FromArgb(255,255,255,255); }
        if (IsSettingsMode) { UpdatePreviewSettings(); UpdateActiveDisplayItems(); }
    }

    partial void OnExamNameFontFamilyChanged(string value) { if (IsSettingsMode) { UpdatePreviewSettings(); UpdateActiveDisplayItems(); } }
    partial void OnExamNameFontSizeChanged(double value)  { if (IsSettingsMode) { UpdatePreviewSettings(); UpdateActiveDisplayItems(); } }
    partial void OnExamNameFontWeightChanged(string value) { if (IsSettingsMode) { UpdatePreviewSettings(); UpdateActiveDisplayItems(); } }
    partial void OnExamNameFontColorHexChanged(string value)
    {
        try { SelectedExamNameColor = ParseColorFromHex(value); }
        catch { SelectedExamNameColor = Windows.UI.Color.FromArgb(255,255,255,255); }
        if (IsSettingsMode) { UpdatePreviewSettings(); UpdateActiveDisplayItems(); }
    }

    partial void OnExamEndFontFamilyChanged(string value) { if (IsSettingsMode) { UpdatePreviewSettings(); UpdateActiveDisplayItems(); } }
    partial void OnExamEndFontSizeChanged(double value)  { if (IsSettingsMode) { UpdatePreviewSettings(); UpdateActiveDisplayItems(); } }
    partial void OnExamEndFontWeightChanged(string value) { if (IsSettingsMode) { UpdatePreviewSettings(); UpdateActiveDisplayItems(); } }
    partial void OnExamEndFontColorHexChanged(string value)
    {
        try { SelectedExamEndColor = ParseColorFromHex(value); }
        catch { SelectedExamEndColor = Windows.UI.Color.FromArgb(255,255,255,255); }
        if (IsSettingsMode) { UpdatePreviewSettings(); UpdateActiveDisplayItems(); }
    }

    partial void OnExamRemainingFontFamilyChanged(string value) { if (IsSettingsMode) { UpdatePreviewSettings(); UpdateActiveDisplayItems(); } }
    partial void OnExamRemainingFontSizeChanged(double value)  { if (IsSettingsMode) { UpdatePreviewSettings(); UpdateActiveDisplayItems(); } }
    partial void OnExamRemainingFontWeightChanged(string value) { if (IsSettingsMode) { UpdatePreviewSettings(); UpdateActiveDisplayItems(); } }
    partial void OnExamRemainingFontColorHexChanged(string value)
    {
        try { SelectedExamRemainingColor = ParseColorFromHex(value); }
        catch { SelectedExamRemainingColor = Windows.UI.Color.FromArgb(255,255,255,255); }
        if (IsSettingsMode) { UpdatePreviewSettings(); UpdateActiveDisplayItems(); }
    }

    partial void OnSelectedTimeAlignmentChanged(string value)
    {
        if (IsSettingsMode) { UpdatePreviewSettings(); UpdateActiveDisplayItems(); }
    }
    partial void OnSelectedDateAlignmentChanged(string value)
    {
        if (IsSettingsMode) { UpdatePreviewSettings(); UpdateActiveDisplayItems(); }
    }
    partial void OnSelectedLayoutOrderChanged(string value)
    {
        if (IsSettingsMode) { UpdatePreviewSettings(); UpdateActiveDisplayItems(); }
    }
    partial void OnSelectedTimeFontFamilyChanged(string value)
    {
        if (IsSettingsMode) { UpdatePreviewSettings(); UpdateActiveDisplayItems(); }
    }
    partial void OnSelectedTimeFontSizeChanged(double value)
    {
        if (IsSettingsMode) { UpdatePreviewSettings(); UpdateActiveDisplayItems(); }
    }
    partial void OnSelectedDateFontFamilyChanged(string value)
    {
        if (IsSettingsMode) { UpdatePreviewSettings(); UpdateActiveDisplayItems(); }
    }
    partial void OnSelectedDateFontSizeChanged(double value)
    {
        if (IsSettingsMode) { UpdatePreviewSettings(); UpdateActiveDisplayItems(); }
    }
    partial void OnSelectedTimeFontWeightChanged(string value)
    {
        if (IsSettingsMode) { UpdatePreviewSettings(); UpdateActiveDisplayItems(); }
    }
    partial void OnSelectedDateFontWeightChanged(string value)
    {
        if (IsSettingsMode) { UpdatePreviewSettings(); UpdateActiveDisplayItems(); }
    }
    partial void OnSelectedExamAlignmentChanged(string value)
    {
        if (IsSettingsMode)
        {
            UpdatePreviewSettings();
            UpdateActiveDisplayItems();
        }
    }

    partial void OnSelectedExamPositionChanged(ExamLayoutPosition value)
    {
        if (IsSettingsMode)
        {
            UpdatePreviewSettings();
            UpdateActiveDisplayItems();
        }
    }
    partial void OnItemsSpacingChanged(double value)
    {
        if (IsSettingsMode)
        {
            UpdatePreviewSettings();
            UpdateActiveDisplayItems();
        }
    }
}
