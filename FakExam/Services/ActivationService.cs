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

        var bg = App.GetService<IBackgroundService>();
        Frame frame;
        bg.EnsureRootHost(App.MainWindow, out frame);   
        var nav = App.GetService<INavigationService>();
        nav.Frame = frame;       
        await bg.ApplyAsync(App.MainWindow);            
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
