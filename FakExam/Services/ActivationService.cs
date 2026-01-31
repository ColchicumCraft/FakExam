using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using FakExam.Activation;
using FakExam.Contracts.Services;

namespace FakExam.Services;

public class ActivationService : IActivationService
{
    private readonly ActivationHandler<LaunchActivatedEventArgs> _defaultHandler;
    private readonly IEnumerable<IActivationHandler> _activationHandlers;
    private readonly IThemeSelectorService _themeSelectorService;
    private UIElement? _shell = null;

    public ActivationService(ActivationHandler<LaunchActivatedEventArgs> defaultHandler,
                             IEnumerable<IActivationHandler> activationHandlers,
                             IThemeSelectorService themeSelectorService)
    {
        _defaultHandler = defaultHandler;
        _activationHandlers = activationHandlers;
        _themeSelectorService = themeSelectorService;
    }

    public async Task ActivateAsync(object activationArgs)
    {
        await InitializeAsync();

        // 1) 让背景服务确保“根包装层(BackgroundLayer + Frame)”已经建立，
        //    并拿到内部的 Frame
        var bg = App.GetService<IBackgroundService>();
        Frame frame;
        bg.EnsureRootHost(App.MainWindow, out frame);   // ★ 关键 1

        // 2) 把这个 Frame 显式交给 NavigationService，
        //    避免它从 App.MainWindow.Content 里再去 as Frame（已经是 Grid 了）
        var nav = App.GetService<INavigationService>();
        nav.Frame = frame;                               // ★ 关键 2

        // 3) 应用背景（若已经有包装层，ApplyAsync 内部不会重复包）
        await bg.ApplyAsync(App.MainWindow);             // ★ 关键 3

        // 4) 正常走激活与导航
        await HandleActivationAsync(activationArgs);

        App.MainWindow.Activate();
        await StartupAsync();
    }

    private async Task HandleActivationAsync(object activationArgs)
    {
        var activationHandler = _activationHandlers.FirstOrDefault(h => h.CanHandle(activationArgs));
        if (activationHandler != null)
        {
            await activationHandler.HandleAsync(activationArgs);
        }
        if (_defaultHandler.CanHandle(activationArgs))
        {
            await _defaultHandler.HandleAsync(activationArgs);
        }
    }

    private async Task InitializeAsync()
    {
        await _themeSelectorService.InitializeAsync().ConfigureAwait(false);
        // 初始化背景设置
        await App.GetService<IBackgroundService>().InitializeAsync();
    }

    private async Task StartupAsync()
    {
        await _themeSelectorService.SetRequestedThemeAsync();
        await Task.CompletedTask;
    }
}
