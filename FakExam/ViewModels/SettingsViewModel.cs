using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FakExam.Contracts.Services;
using FakExam.Core.Models;
using FakExam.Helpers;
using Microsoft.UI.Xaml;
using Newtonsoft.Json;
using Windows.ApplicationModel;

namespace FakExam.ViewModels;

public partial class SettingsViewModel : ObservableRecipient
{
    private readonly IThemeSelectorService _themeSelectorService;
    private readonly ILocalSettingsService _localSettings;
    private const string AutoLoadKey = "AutoLoadProfileSettings";

    [ObservableProperty]
    private ElementTheme _elementTheme;

    [ObservableProperty]
    private string _versionDescription;

    // 自动加载Json配置文件
    [ObservableProperty]
    private bool _autoLoadEnabled;

    [ObservableProperty]
    private string? _autoLoadFilePath;

    [ObservableProperty]
    private bool _autoLoadIsValid;

    [ObservableProperty]
    private string _autoLoadValidationMessage = string.Empty;

    [ObservableProperty]
    private bool _canSaveAutoLoad;

    [ObservableProperty]
    private string _autoLoadStatusColor = "#FFFF00";


    public ICommand SwitchThemeCommand
    {
        get;
    }

    public SettingsViewModel(IThemeSelectorService themeSelectorService,
                             ILocalSettingsService localSettingsService)
    {
        _themeSelectorService = themeSelectorService;
        _localSettings = localSettingsService;
        _elementTheme = _themeSelectorService.Theme;
        _versionDescription = GetVersionDescription();

        SwitchThemeCommand = new RelayCommand<ElementTheme>(
            async (param) =>
            {
                if (ElementTheme != param)
                {
                    ElementTheme = param;
                    await _themeSelectorService.SetThemeAsync(param);
                }
            });
        _ = LoadAutoLoadSettingsAsync();
    }

    private async Task LoadAutoLoadSettingsAsync()
    {
        var saved = await _localSettings.ReadSettingAsync<AutoLoadProfileSettings>(AutoLoadKey)
                    ?? new AutoLoadProfileSettings();

        AutoLoadEnabled = saved.Enabled;
        AutoLoadFilePath = saved.FilePath;

        await ValidateAndUpdateStateAsync();
        if (!AutoLoadEnabled)
        {
            AutoLoadValidationMessage = "未开启";
            AutoLoadStatusColor = "#FFC107";
        }
        else if (AutoLoadIsValid)
        {
            AutoLoadValidationMessage = "已开启";
            AutoLoadStatusColor = "#4CAF50";
        }
        else {}
    }

    partial void OnAutoLoadEnabledChanged(bool value)
    {
        _ = OnAutoLoadEnabledChangedImplAsync(value);
    }

    private async Task OnAutoLoadEnabledChangedImplAsync(bool value)
    {
        if (!value)
        {
            AutoLoadIsValid = false; // 关闭下不关心有效性
            AutoLoadValidationMessage = "已保存：未开启自动加载。";
            AutoLoadStatusColor = "#4CAF50";
            await SaveAutoLoadSettingsCoreAsync(showDialog: false);
        }
        else
        {
            await ValidateAndUpdateStateAsync();
            if (AutoLoadIsValid)
            {
                await SaveAutoLoadSettingsCoreAsync(showDialog: false);
                AutoLoadValidationMessage = "验证通过，设置已保存为自动加载。";
                AutoLoadStatusColor = "#4CAF50";
            }
            else {}
        }
    }

    partial void OnAutoLoadFilePathChanged(string? value)
    {
        _ = OnAutoLoadFilePathChangedImplAsync();
    }

    private async Task OnAutoLoadFilePathChangedImplAsync()
    {
        await ValidateAndUpdateStateAsync();

        if (AutoLoadEnabled && AutoLoadIsValid)
        {
            await SaveAutoLoadSettingsCoreAsync(showDialog: false);
            AutoLoadValidationMessage = "验证通过，设置已保存为自动加载。";
            AutoLoadStatusColor = "#4CAF50";
        }
    }

    [RelayCommand]
    private async Task BrowseAutoLoadFileAsync()
    {
        var picker = new Windows.Storage.Pickers.FileOpenPicker();
        picker.FileTypeFilter.Add(".json");
        picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;

        var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);
        WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

        var file = await picker.PickSingleFileAsync();
        if (file != null)
        {
            AutoLoadFilePath = file.Path; 
        }
    }

    [RelayCommand]
    private async Task SaveAutoLoadSettingsAsync()
        => await SaveAutoLoadSettingsCoreAsync(showDialog: true);

    private async Task SaveAutoLoadSettingsCoreAsync(bool showDialog)
    {
        var toPersist = new AutoLoadProfileSettings
        {
            Enabled = AutoLoadEnabled && AutoLoadIsValid,
            FilePath = AutoLoadIsValid ? AutoLoadFilePath : null
        };

        await _localSettings.SaveSettingAsync(AutoLoadKey, toPersist);

        if (showDialog)
        {
            await App.MainWindow.ShowMessageDialogAsync(
                toPersist.Enabled
                ? "已保存：启动时将自动加载该配置文件（仅保存；此处不进行真正加载）。"
                : "已保存：未开启自动加载（路径/格式未通过验证或被关闭）。",
                "自动加载设置");
        }
        else
        {
            if (toPersist.Enabled)
            {
                if (string.IsNullOrWhiteSpace(AutoLoadValidationMessage))
                    AutoLoadValidationMessage = "已保存为自动加载。";
                    AutoLoadStatusColor = "#4CAF50";
            }
            else
            {
                if (string.IsNullOrWhiteSpace(AutoLoadValidationMessage))
                    AutoLoadValidationMessage = "已保存：未开启自动加载。";
                    AutoLoadStatusColor = "#4CAF50";
            }
        }
    }

    private async Task ValidateAndUpdateStateAsync()
    {
        AutoLoadIsValid = false;
        AutoLoadValidationMessage = string.Empty;
        AutoLoadStatusColor = "#FFFFFF";

        if (!AutoLoadEnabled)
        {
            // 关闭状态下允许保存（保存将带 Enabled=false）
            CanSaveAutoLoad = true;
            return;
        }

        if (string.IsNullOrWhiteSpace(AutoLoadFilePath))
        {
            AutoLoadValidationMessage = "请选择配置文件路径。";
            AutoLoadStatusColor = "#F44336";
            CanSaveAutoLoad = false;
            return;
        }

        if (!File.Exists(AutoLoadFilePath))
        {
            AutoLoadValidationMessage = "文件不存在。";
            AutoLoadStatusColor = "#F44336";
            CanSaveAutoLoad = false;
            return;
        }

        var (ok, message) = await ValidateProfileJsonAsync(AutoLoadFilePath!);
        AutoLoadIsValid = ok;
        AutoLoadValidationMessage = ok ? "验证通过。" : message ?? "格式不正确。";
        AutoLoadStatusColor = ok ? "#4CAF50" : "#F44336"; 
        CanSaveAutoLoad = ok;
    }

    private static async Task<(bool ok, string? message)> ValidateProfileJsonAsync(string path)
    {
        try
        {
            var json = await File.ReadAllTextAsync(path).ConfigureAwait(false);
            var settings = new JsonSerializerSettings{};

            var profile = JsonConvert.DeserializeObject<DashboardProfile>(json, settings);
            if (profile is null)
            {
                return (false, "解析失败：JSON 内容为空或结构不正确。");
            }

            if (profile.ExamInfos is null || profile.ExamInfos.Count == 0)
            {
                return (false, "解析失败：ExamInfos 为空。");
            }
            for (int i = 0; i < profile.ExamInfos.Count; i++)
            {
                var e = profile.ExamInfos[i];

                if (string.IsNullOrWhiteSpace(e.Name))
                    return (false, $"第 {i + 1} 条记录无有效 Name。");

                if (string.IsNullOrWhiteSpace(e.Start))
                    return (false, $"第 {i + 1} 条记录无有效 Start。");

                if (string.IsNullOrWhiteSpace(e.End))
                    return (false, $"第 {i + 1} 条记录无有效 End。");

                DateTime startTime, endTime;
                try
                {
                    startTime = e.StartTime; // => DateTime.ParseExact(Start, "yyyy-MM-dd HH:mm:ss", null)
                    endTime = e.EndTime;
                }
                catch (FormatException)
                {
                    return (false, $"第 {i + 1} 条记录时间格式错误：应为 yyyy-MM-dd HH:mm:ss。");
                }

                if (endTime < startTime)
                    return (false, $"第 {i + 1} 条记录的结束时间早于开始时间。");

                if (e.AlertTime < 0)
                    return (false, $"第 {i + 1} 条记录的 AlertTime 不能为负数。");
            }

            return (true, null);
        }
        catch (JsonReaderException)
        {
            return (false, "JSON 语法错误，无法读取。");
        }
        catch (JsonSerializationException jsex)
        {
            return (false, $"反序列化错误：{jsex.Message}");
        }
        catch (IOException ioex)
        {
            return (false, $"读取文件失败：{ioex.Message}");
        }
        catch (Exception ex)
        {
            return (false, $"验证失败：{ex.Message}");
        }
    }

    private static string GetVersionDescription()
    {
        Version version;

        if (RuntimeHelper.IsMSIX)
        {
            var packageVersion = Package.Current.Id.Version;
            version = new(packageVersion.Major, packageVersion.Minor, packageVersion.Build, packageVersion.Revision);
        }
        else
        {
            version = Assembly.GetExecutingAssembly().GetName().Version!;
        }

        return $"{"AppDisplayName".GetLocalized()} - {version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
    }
}