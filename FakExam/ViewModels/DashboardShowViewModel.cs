using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FakExam.Contracts.Services;
using FakExam.Core.Contracts.Services;
using FakExam.Core.Models;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Windowing;
using Windows.UI;
using Microsoft.UI.Xaml;
using System.Diagnostics;

namespace FakExam.ViewModels;

public partial class DashboardShowViewModel : ObservableObject, IDisposable
{
    private readonly INavigationService NavigationService;
    private readonly IDashboardProfileService _profileService;
    private readonly IProfileLoaderService _profileLoader;
    private readonly IClockService _clock;
    private readonly IDashboardDisplayService _displayService;
    private readonly DispatcherQueue _dispatcherQueue;

    [ObservableProperty] private bool _isCompactOverlay = false;
    [ObservableProperty] private bool _isFullScreen = false;
    [ObservableProperty] private bool _isSettingsMode = false;

    [ObservableProperty]
    private ObservableCollection<ExamRowView> _examRows = new();

    [ObservableProperty] private string _examName = "没有加载面板配置文件";
    [ObservableProperty] private string _message = "请加载ExamSchedule格式的Json配置文件";
    [ObservableProperty] private string _currentTime = "--:--:--";
    [ObservableProperty] private string _currentExamName = "—";
    [ObservableProperty] private string _currentExamTimeRange = "—";
    [ObservableProperty] private string _remainingTimeText = "—";
    [ObservableProperty] private string _currentStatusText = "—";

    // 设置相关属性
    [ObservableProperty] private string _selectedLayoutOrder = "StatusOnLeft";
    [ObservableProperty] private bool _showDateColumn = true;
    [ObservableProperty] private bool _showNameColumn = true;
    [ObservableProperty] private bool _showStartColumn = true;
    [ObservableProperty] private bool _showEndColumn = true;
    [ObservableProperty] private bool _showStatusColumn = true;

    // 字体设置 - 标题
    [ObservableProperty] private string _titleFontFamily = "Segoe UI";
    [ObservableProperty] private double _titleFontSize = 22;
    [ObservableProperty] private string _titleFontWeight = "SemiBold";
    [ObservableProperty] private string _titleFontColorHex = "#000000";
    [ObservableProperty] private Color _selectedTitleColor = Color.FromArgb(255, 0, 0, 0);

    // 字体设置 - 信息
    [ObservableProperty] private string _messageFontFamily = "Segoe UI";
    [ObservableProperty] private double _messageFontSize = 14;
    [ObservableProperty] private string _messageFontWeight = "Normal";
    [ObservableProperty] private string _messageFontColorHex = "#6D6D6D";
    [ObservableProperty] private Color _selectedMessageColor = Color.FromArgb(255, 109, 109, 109);

    // 字体设置 - 状态标签
    [ObservableProperty] private string _statusLabelFontFamily = "Segoe UI";
    [ObservableProperty] private double _statusLabelFontSize = 14;
    [ObservableProperty] private string _statusLabelFontWeight = "Normal";
    [ObservableProperty] private string _statusLabelFontColorHex = "#6D6D6D";
    [ObservableProperty] private Color _selectedStatusLabelColor = Color.FromArgb(255, 109, 109, 109);

    // 字体设置 - 状态值
    [ObservableProperty] private string _currentExamNameFontFamily = "Segoe UI";
    [ObservableProperty] private double _currentExamNameFontSize = 18;
    [ObservableProperty] private string _currentExamNameFontWeight = "SemiBold";
    [ObservableProperty] private string _currentExamNameFontColorHex = "#000000";
    [ObservableProperty] private Color _selectedCurrentExamNameColor = Color.FromArgb(255, 0, 0, 0);

    [ObservableProperty] private string _currentExamTimeRangeFontFamily = "Segoe UI";
    [ObservableProperty] private double _currentExamTimeRangeFontSize = 16;
    [ObservableProperty] private string _currentExamTimeRangeFontWeight = "Normal";
    [ObservableProperty] private string _currentExamTimeRangeFontColorHex = "#000000";
    [ObservableProperty] private Color _selectedCurrentExamTimeRangeColor = Color.FromArgb(255, 0, 0, 0);

    [ObservableProperty] private string _remainingTimeTextFontFamily = "Segoe UI";
    [ObservableProperty] private double _remainingTimeTextFontSize = 16;
    [ObservableProperty] private string _remainingTimeTextFontWeight = "SemiBold";
    [ObservableProperty] private string _remainingTimeTextFontColorHex = "#000000";
    [ObservableProperty] private Color _selectedRemainingTimeTextColor = Color.FromArgb(255, 0, 0, 0);

    [ObservableProperty] private string _currentStatusTextFontFamily = "Segoe UI";
    [ObservableProperty] private double _currentStatusTextFontSize = 16;
    [ObservableProperty] private string _currentStatusTextFontWeight = "SemiBold";
    [ObservableProperty] private string _currentStatusTextFontColorHex = "#000000";
    [ObservableProperty] private Color _selectedCurrentStatusTextColor = Color.FromArgb(255, 0, 0, 0);

    // 字体设置 - 时间显示
    [ObservableProperty] private string _currentTimeFontFamily = "Segoe UI";
    [ObservableProperty] private double _currentTimeFontSize = 36;
    [ObservableProperty] private string _currentTimeFontWeight = "Bold";
    [ObservableProperty] private string _currentTimeFontColorHex = "#000000";
    [ObservableProperty] private Color _selectedCurrentTimeColor = Color.FromArgb(255, 0, 0, 0);

    // 字体设置 - 表格
    [ObservableProperty] private string _tableHeaderFontFamily = "Segoe UI";
    [ObservableProperty] private double _tableHeaderFontSize = 14;
    [ObservableProperty] private string _tableHeaderFontWeight = "SemiBold";
    [ObservableProperty] private string _tableHeaderFontColorHex = "#000000";
    [ObservableProperty] private Color _selectedTableHeaderColor = Color.FromArgb(255, 0, 0, 0);

    [ObservableProperty] private string _tableContentFontFamily = "Segoe UI";
    [ObservableProperty] private double _tableContentFontSize = 14;
    [ObservableProperty] private string _tableContentFontWeight = "Normal";
    [ObservableProperty] private string _tableContentFontColorHex = "#000000";
    [ObservableProperty] private Color _selectedTableContentColor = Color.FromArgb(255, 0, 0, 0);

    private DashboardDisplaySettings _currentSettings = new();
    private DashboardDisplaySettings _previewSettings = new();

    public ObservableCollection<string> FontFamilies => DisplayDataSources.FontFamilies;
    public ObservableCollection<FontWeightItem> FontWeights => DisplayDataSources.FontWeights;
    public ObservableCollection<LayoutOrderItem> LayoutOrderOptions => DisplayDataSources.DashboardLayoutOrderOptions;

    public DashboardShowViewModel(INavigationService navigationService,
                                  IDashboardProfileService profileService,
                                  IProfileLoaderService profileLoader,
                                  IClockService clock,
                                  IDashboardDisplayService displayService)
    {
        NavigationService = navigationService;
        _profileService = profileService;
        _profileLoader = profileLoader;
        _clock = clock;
        _displayService = displayService;
        _dispatcherQueue = DispatcherQueue.GetForCurrentThread();

        // 每秒刷新显示
        _clock.Tick += (_, now) =>
        {
            _dispatcherQueue.TryEnqueue(() =>
            {
                CurrentTime = now.ToString("HH:mm:ss");
                UpdateRowStatuses(now);
                UpdateCurrentBlock(now);
            });
        };

        // 配置变化时重建或刷新
        _profileService.ProfileChanged += (_, __) => _dispatcherQueue.TryEnqueue(() =>
        {
            RebuildExamRows();
            RefreshNow();
        });
        _profileService.ExamStateChanged += (_, __) => _dispatcherQueue.TryEnqueue(() => RefreshNow());

        // 加载显示设置
        LoadDisplaySettings();
    }

    [RelayCommand]
    private void GoBack()
    {
        if (NavigationService.CanGoBack) NavigationService.GoBack();
    }

    [RelayCommand]
    private async System.Threading.Tasks.Task LoadJsonAsync()
    {
        var loaded = await _profileLoader.PickAndLoadAsync();
        if (loaded)
        {
            RebuildExamRows();
            RefreshNow();
        }
    }

    private async void LoadDisplaySettings()
    {
        await _displayService.InitializeAsync();
        _currentSettings = _displayService.CurrentSettings;
        ApplySettingsToViewModel(_currentSettings);
    }

    private void RefreshNow()
    {
        var now = DateTime.Now;
        CurrentTime = now.ToString("HH:mm:ss");
        UpdateRowStatuses(now);
        UpdateCurrentBlock(now);
    }

    private void RebuildExamRows()
    {
        ExamRows.Clear();
        var profile = _profileService.CurrentProfile;
        if (profile?.ExamInfos == null) return;
        foreach (var exam in profile.ExamInfos.OrderBy(e => e.StartTime))
        {
            var row = new ExamRowView
            {
                Model = exam,
                Date = exam.DisplayDate,
                Name = exam.Name ?? string.Empty,
                Start = exam.DisplayStartTime,
                End = exam.DisplayEndTime,
                Status = "—"
            };
            ExamRows.Add(row);
        }
        ExamName = profile.ExamName ?? string.Empty;
        Message = profile.Message ?? string.Empty;
    }

    private void UpdateRowStatuses(DateTime now)
    {
        foreach (var row in ExamRows)
        {
            row.Status = EvaluateStatusText(row.Model, now);
        }
    }

    private void UpdateCurrentBlock(DateTime now)
    {
        var inProgress = _profileService.CurrentInProgressExam;
        ExamInfo? target = inProgress ?? _profileService.NextUpcomingExam;
        if (target == null)
        {
            CurrentExamName = "事件均已结束";
            CurrentExamTimeRange = "—";
            RemainingTimeText = "已结束";
            CurrentStatusText = "已结束";
            return;
        }

        CurrentExamName = target.Name ?? "—";
        CurrentExamTimeRange = $"{target.StartTime:HH:mm:ss} - {target.EndTime:HH:mm:ss}";
        CurrentStatusText = EvaluateStatusText(target, now);
        RemainingTimeText = CurrentStatusText switch
        {
            "进行中" or "即将结束" => FormatTimeSpan(target.EndTime - now),
            "未开始" or "即将开始" => FormatTimeSpan(target.StartTime - now),
            "已结束" => "已结束",
            _ => "—"
        };
    }

    private static string EvaluateStatusText(ExamInfo exam, DateTime now)
    {
        var thresholdMinutes = exam.AlertTime > 0 ? exam.AlertTime : 15;
        if (now >= exam.EndTime) return "已结束";
        if (now < exam.StartTime)
        {
            var toStart = exam.StartTime - now;
            return toStart.TotalMinutes <= thresholdMinutes ? "即将开始" : "未开始";
        }
        var toEnd = exam.EndTime - now;
        return toEnd.TotalMinutes <= thresholdMinutes ? "即将结束" : "进行中";
    }

    private static string FormatTimeSpan(TimeSpan ts)
    {
        if (ts.TotalSeconds < 0) ts = TimeSpan.Zero;
        return ts.TotalHours >= 1
            ? $"{(int)ts.TotalHours:00}:{ts.Minutes:00}:{ts.Seconds:00}"
            : $"{ts.Minutes:00}:{ts.Seconds:00}";
    }

    // 设置相关方法
    [RelayCommand]
    private void ShowSettings()
    {
        IsSettingsMode = true;
        UpdatePreviewSettings();
    }

    [RelayCommand]
    private void HideSettings()
    {
        IsSettingsMode = false;
        ApplySettingsToViewModel(_currentSettings);
    }

    [RelayCommand]
    private async System.Threading.Tasks.Task ApplySettings()
    {
        try
        {
            _currentSettings = DeepCloneSettings(_previewSettings);
            await _displayService.SaveSettingsAsync(_currentSettings);
            IsSettingsMode = false;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"保存设置失败: {ex.Message}");
        }
    }

    [RelayCommand]
    private void CancelSettings()
    {
        ApplySettingsToViewModel(_currentSettings);
        IsSettingsMode = false;
    }

    private void UpdatePreviewSettings()
    {
        _previewSettings = new DashboardDisplaySettings
        {
            LayoutOrder = SelectedLayoutOrder == "StatusOnLeft" ? DashboardLayoutOrder.StatusOnLeft : DashboardLayoutOrder.TableOnLeft,
            ColumnVisibility = new ColumnVisibilitySettings
            {
                ShowDateColumn = ShowDateColumn,
                ShowNameColumn = ShowNameColumn,
                ShowStartColumn = ShowStartColumn,
                ShowEndColumn = ShowEndColumn,
                ShowStatusColumn = ShowStatusColumn
            },
            TitleFont = new FontSettings
            {
                FontFamily = TitleFontFamily,
                FontSize = TitleFontSize,
                FontWeight = GetFontWeightValue(TitleFontWeight),
                FontColor = TitleFontColorHex
            },
            MessageFont = new FontSettings
            {
                FontFamily = MessageFontFamily,
                FontSize = MessageFontSize,
                FontWeight = GetFontWeightValue(MessageFontWeight),
                FontColor = MessageFontColorHex
            },
            StatusLabelFont = new FontSettings
            {
                FontFamily = StatusLabelFontFamily,
                FontSize = StatusLabelFontSize,
                FontWeight = GetFontWeightValue(StatusLabelFontWeight),
                FontColor = StatusLabelFontColorHex
            },
            CurrentExamNameFont = new FontSettings
            {
                FontFamily = CurrentExamNameFontFamily,
                FontSize = CurrentExamNameFontSize,
                FontWeight = GetFontWeightValue(CurrentExamNameFontWeight),
                FontColor = CurrentExamNameFontColorHex
            },
            CurrentExamTimeRangeFont = new FontSettings
            {
                FontFamily = CurrentExamTimeRangeFontFamily,
                FontSize = CurrentExamTimeRangeFontSize,
                FontWeight = GetFontWeightValue(CurrentExamTimeRangeFontWeight),
                FontColor = CurrentExamTimeRangeFontColorHex
            },
            RemainingTimeTextFont = new FontSettings
            {
                FontFamily = RemainingTimeTextFontFamily,
                FontSize = RemainingTimeTextFontSize,
                FontWeight = GetFontWeightValue(RemainingTimeTextFontWeight),
                FontColor = RemainingTimeTextFontColorHex
            },
            CurrentStatusTextFont = new FontSettings
            {
                FontFamily = CurrentStatusTextFontFamily,
                FontSize = CurrentStatusTextFontSize,
                FontWeight = GetFontWeightValue(CurrentStatusTextFontWeight),
                FontColor = CurrentStatusTextFontColorHex
            },
            CurrentTimeFont = new FontSettings
            {
                FontFamily = CurrentTimeFontFamily,
                FontSize = CurrentTimeFontSize,
                FontWeight = GetFontWeightValue(CurrentTimeFontWeight),
                FontColor = CurrentTimeFontColorHex
            },
            TableHeaderFont = new FontSettings
            {
                FontFamily = TableHeaderFontFamily,
                FontSize = TableHeaderFontSize,
                FontWeight = GetFontWeightValue(TableHeaderFontWeight),
                FontColor = TableHeaderFontColorHex
            },
            TableContentFont = new FontSettings
            {
                FontFamily = TableContentFontFamily,
                FontSize = TableContentFontSize,
                FontWeight = GetFontWeightValue(TableContentFontWeight),
                FontColor = TableContentFontColorHex
            }
        };
    }

    private void ApplySettingsToViewModel(DashboardDisplaySettings settings)
    {
        SelectedLayoutOrder = settings.LayoutOrder == DashboardLayoutOrder.StatusOnLeft ? "StatusOnLeft" : "TableOnLeft";
        ShowDateColumn = settings.ColumnVisibility.ShowDateColumn;
        ShowNameColumn = settings.ColumnVisibility.ShowNameColumn;
        ShowStartColumn = settings.ColumnVisibility.ShowStartColumn;
        ShowEndColumn = settings.ColumnVisibility.ShowEndColumn;
        ShowStatusColumn = settings.ColumnVisibility.ShowStatusColumn;

        // 标题设置
        TitleFontFamily = settings.TitleFont.FontFamily;
        TitleFontSize = settings.TitleFont.FontSize;
        TitleFontWeight = GetFontWeightDisplayName(settings.TitleFont.FontWeight);
        TitleFontColorHex = settings.TitleFont.FontColor;
        SelectedTitleColor = ParseColorFromHex(settings.TitleFont.FontColor);

        // 信息设置
        MessageFontFamily = settings.MessageFont.FontFamily;
        MessageFontSize = settings.MessageFont.FontSize;
        MessageFontWeight = GetFontWeightDisplayName(settings.MessageFont.FontWeight);
        MessageFontColorHex = settings.MessageFont.FontColor;
        SelectedMessageColor = ParseColorFromHex(settings.MessageFont.FontColor);

        // 状态标签设置
        StatusLabelFontFamily = settings.StatusLabelFont.FontFamily;
        StatusLabelFontSize = settings.StatusLabelFont.FontSize;
        StatusLabelFontWeight = GetFontWeightDisplayName(settings.StatusLabelFont.FontWeight);
        StatusLabelFontColorHex = settings.StatusLabelFont.FontColor;
        SelectedStatusLabelColor = ParseColorFromHex(settings.StatusLabelFont.FontColor);

        // 状态值设置
        CurrentExamNameFontFamily = settings.CurrentExamNameFont.FontFamily;
        CurrentExamNameFontSize = settings.CurrentExamNameFont.FontSize;
        CurrentExamNameFontWeight = GetFontWeightDisplayName(settings.CurrentExamNameFont.FontWeight);
        CurrentExamNameFontColorHex = settings.CurrentExamNameFont.FontColor;
        SelectedCurrentExamNameColor = ParseColorFromHex(settings.CurrentExamNameFont.FontColor);

        CurrentExamTimeRangeFontFamily = settings.CurrentExamTimeRangeFont.FontFamily;
        CurrentExamTimeRangeFontSize = settings.CurrentExamTimeRangeFont.FontSize;
        CurrentExamTimeRangeFontWeight = GetFontWeightDisplayName(settings.CurrentExamTimeRangeFont.FontWeight);
        CurrentExamTimeRangeFontColorHex = settings.CurrentExamTimeRangeFont.FontColor;
        SelectedCurrentExamTimeRangeColor = ParseColorFromHex(settings.CurrentExamTimeRangeFont.FontColor);

        RemainingTimeTextFontFamily = settings.RemainingTimeTextFont.FontFamily;
        RemainingTimeTextFontSize = settings.RemainingTimeTextFont.FontSize;
        RemainingTimeTextFontWeight = GetFontWeightDisplayName(settings.RemainingTimeTextFont.FontWeight);
        RemainingTimeTextFontColorHex = settings.RemainingTimeTextFont.FontColor;
        SelectedRemainingTimeTextColor = ParseColorFromHex(settings.RemainingTimeTextFont.FontColor);

        CurrentStatusTextFontFamily = settings.CurrentStatusTextFont.FontFamily;
        CurrentStatusTextFontSize = settings.CurrentStatusTextFont.FontSize;
        CurrentStatusTextFontWeight = GetFontWeightDisplayName(settings.CurrentStatusTextFont.FontWeight);
        CurrentStatusTextFontColorHex = settings.CurrentStatusTextFont.FontColor;
        SelectedCurrentStatusTextColor = ParseColorFromHex(settings.CurrentStatusTextFont.FontColor);

        // 时间显示设置
        CurrentTimeFontFamily = settings.CurrentTimeFont.FontFamily;
        CurrentTimeFontSize = settings.CurrentTimeFont.FontSize;
        CurrentTimeFontWeight = GetFontWeightDisplayName(settings.CurrentTimeFont.FontWeight);
        CurrentTimeFontColorHex = settings.CurrentTimeFont.FontColor;
        SelectedCurrentTimeColor = ParseColorFromHex(settings.CurrentTimeFont.FontColor);

        // 表格设置
        TableHeaderFontFamily = settings.TableHeaderFont.FontFamily;
        TableHeaderFontSize = settings.TableHeaderFont.FontSize;
        TableHeaderFontWeight = GetFontWeightDisplayName(settings.TableHeaderFont.FontWeight);
        TableHeaderFontColorHex = settings.TableHeaderFont.FontColor;
        SelectedTableHeaderColor = ParseColorFromHex(settings.TableHeaderFont.FontColor);

        TableContentFontFamily = settings.TableContentFont.FontFamily;
        TableContentFontSize = settings.TableContentFont.FontSize;
        TableContentFontWeight = GetFontWeightDisplayName(settings.TableContentFont.FontWeight);
        TableContentFontColorHex = settings.TableContentFont.FontColor;
        SelectedTableContentColor = ParseColorFromHex(settings.TableContentFont.FontColor);

        _previewSettings = DeepCloneSettings(settings);
    }

    private DashboardDisplaySettings DeepCloneSettings(DashboardDisplaySettings source)
    {
        return new DashboardDisplaySettings
        {
            LayoutOrder = source.LayoutOrder,
            ColumnVisibility = new ColumnVisibilitySettings
            {
                ShowDateColumn = source.ColumnVisibility.ShowDateColumn,
                ShowNameColumn = source.ColumnVisibility.ShowNameColumn,
                ShowStartColumn = source.ColumnVisibility.ShowStartColumn,
                ShowEndColumn = source.ColumnVisibility.ShowEndColumn,
                ShowStatusColumn = source.ColumnVisibility.ShowStatusColumn
            },
            TitleFont = new FontSettings
            {
                FontFamily = source.TitleFont.FontFamily,
                FontSize = source.TitleFont.FontSize,
                FontWeight = source.TitleFont.FontWeight,
                FontColor = source.TitleFont.FontColor
            },
            MessageFont = new FontSettings
            {
                FontFamily = source.MessageFont.FontFamily,
                FontSize = source.MessageFont.FontSize,
                FontWeight = source.MessageFont.FontWeight,
                FontColor = source.MessageFont.FontColor
            },
            StatusLabelFont = new FontSettings
            {
                FontFamily = source.StatusLabelFont.FontFamily,
                FontSize = source.StatusLabelFont.FontSize,
                FontWeight = source.StatusLabelFont.FontWeight,
                FontColor = source.StatusLabelFont.FontColor
            },
            CurrentExamNameFont = new FontSettings
            {
                FontFamily = source.CurrentExamNameFont.FontFamily,
                FontSize = source.CurrentExamNameFont.FontSize,
                FontWeight = source.CurrentExamNameFont.FontWeight,
                FontColor = source.CurrentExamNameFont.FontColor
            },
            CurrentExamTimeRangeFont = new FontSettings
            {
                FontFamily = source.CurrentExamTimeRangeFont.FontFamily,
                FontSize = source.CurrentExamTimeRangeFont.FontSize,
                FontWeight = source.CurrentExamTimeRangeFont.FontWeight,
                FontColor = source.CurrentExamTimeRangeFont.FontColor
            },
            RemainingTimeTextFont = new FontSettings
            {
                FontFamily = source.RemainingTimeTextFont.FontFamily,
                FontSize = source.RemainingTimeTextFont.FontSize,
                FontWeight = source.RemainingTimeTextFont.FontWeight,
                FontColor = source.RemainingTimeTextFont.FontColor
            },
            CurrentStatusTextFont = new FontSettings
            {
                FontFamily = source.CurrentStatusTextFont.FontFamily,
                FontSize = source.CurrentStatusTextFont.FontSize,
                FontWeight = source.CurrentStatusTextFont.FontWeight,
                FontColor = source.CurrentStatusTextFont.FontColor
            },
            CurrentTimeFont = new FontSettings
            {
                FontFamily = source.CurrentTimeFont.FontFamily,
                FontSize = source.CurrentTimeFont.FontSize,
                FontWeight = source.CurrentTimeFont.FontWeight,
                FontColor = source.CurrentTimeFont.FontColor
            },
            TableHeaderFont = new FontSettings
            {
                FontFamily = source.TableHeaderFont.FontFamily,
                FontSize = source.TableHeaderFont.FontSize,
                FontWeight = source.TableHeaderFont.FontWeight,
                FontColor = source.TableHeaderFont.FontColor
            },
            TableContentFont = new FontSettings
            {
                FontFamily = source.TableContentFont.FontFamily,
                FontSize = source.TableContentFont.FontSize,
                FontWeight = source.TableContentFont.FontWeight,
                FontColor = source.TableContentFont.FontColor
            }
        };
    }

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

    private Color ParseColorFromHex(string hexColor)
    {
        try
        {
            if (string.IsNullOrEmpty(hexColor) || hexColor.Length != 7 || hexColor[0] != '#')
                return Color.FromArgb(255, 0, 0, 0);
            return Color.FromArgb(
                255,
                Convert.ToByte(hexColor.Substring(1, 2), 16),
                Convert.ToByte(hexColor.Substring(3, 2), 16),
                Convert.ToByte(hexColor.Substring(5, 2), 16)
            );
        }
        catch { return Color.FromArgb(255, 0, 0, 0); }
    }

    // 颜色确认命令
    [RelayCommand] private void ConfirmTitleColor() => TitleFontColorHex = $"#{SelectedTitleColor.R:X2}{SelectedTitleColor.G:X2}{SelectedTitleColor.B:X2}";
    [RelayCommand] private void ConfirmMessageColor() => MessageFontColorHex = $"#{SelectedMessageColor.R:X2}{SelectedMessageColor.G:X2}{SelectedMessageColor.B:X2}";
    [RelayCommand] private void ConfirmStatusLabelColor() => StatusLabelFontColorHex = $"#{SelectedStatusLabelColor.R:X2}{SelectedStatusLabelColor.G:X2}{SelectedStatusLabelColor.B:X2}";
    [RelayCommand] private void ConfirmCurrentExamNameColor() => CurrentExamNameFontColorHex = $"#{SelectedCurrentExamNameColor.R:X2}{SelectedCurrentExamNameColor.G:X2}{SelectedCurrentExamNameColor.B:X2}";
    [RelayCommand] private void ConfirmCurrentExamTimeRangeColor() => CurrentExamTimeRangeFontColorHex = $"#{SelectedCurrentExamTimeRangeColor.R:X2}{SelectedCurrentExamTimeRangeColor.G:X2}{SelectedCurrentExamTimeRangeColor.B:X2}";
    [RelayCommand] private void ConfirmRemainingTimeTextColor() => RemainingTimeTextFontColorHex = $"#{SelectedRemainingTimeTextColor.R:X2}{SelectedRemainingTimeTextColor.G:X2}{SelectedRemainingTimeTextColor.B:X2}";
    [RelayCommand] private void ConfirmCurrentStatusTextColor() => CurrentStatusTextFontColorHex = $"#{SelectedCurrentStatusTextColor.R:X2}{SelectedCurrentStatusTextColor.G:X2}{SelectedCurrentStatusTextColor.B:X2}";
    [RelayCommand] private void ConfirmCurrentTimeColor() => CurrentTimeFontColorHex = $"#{SelectedCurrentTimeColor.R:X2}{SelectedCurrentTimeColor.G:X2}{SelectedCurrentTimeColor.B:X2}";
    [RelayCommand] private void ConfirmTableHeaderColor() => TableHeaderFontColorHex = $"#{SelectedTableHeaderColor.R:X2}{SelectedTableHeaderColor.G:X2}{SelectedTableHeaderColor.B:X2}";
    [RelayCommand] private void ConfirmTableContentColor() => TableContentFontColorHex = $"#{SelectedTableContentColor.R:X2}{SelectedTableContentColor.G:X2}{SelectedTableContentColor.B:X2}";

    // 其他原有命令保持不变
    [RelayCommand]
    private void GoTimeShow()
    {
        var navigationService = App.GetService<INavigationService>();
        navigationService.NavigateTo(typeof(TimeShowViewModel).FullName!);
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

    public void Dispose() 
    {
    }

    public partial class ExamRowView : ObservableObject
    {
        public ExamInfo Model { get; set; } = default!;
        [ObservableProperty] private string _date = string.Empty;
        [ObservableProperty] private string _name = string.Empty;
        [ObservableProperty] private string _start = string.Empty;
        [ObservableProperty] private string _end = string.Empty;
        [ObservableProperty] private string _status = "—";
    }
}