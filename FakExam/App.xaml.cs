using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;
using FakExam.Activation;
using FakExam.Contracts.Services;
using FakExam.Core.Contracts.Services;
using FakExam.Core.Services;
using FakExam.Helpers;
using FakExam.Models;
using FakExam.Notifications;
using FakExam.Services;
using FakExam.ViewModels;
using FakExam.Views;

namespace FakExam;

public partial class App : Application
{
    public IHost Host { get; }

    public static T GetService<T>()
        where T : class
    {
        if ((App.Current as App)!.Host.Services.GetService(typeof(T)) is not T service)
        {
            throw new ArgumentException($"{typeof(T)} needs to be registered in ConfigureServices within App.xaml.cs.");
        }
        return service;
    }

    public static WindowEx MainWindow { get; } = new MainWindow();
    public static UIElement? AppTitlebar { get; set; }

    public App()
    {
        InitializeComponent();

        Host = Microsoft.Extensions.Hosting.Host.
            CreateDefaultBuilder().
            UseContentRoot(AppContext.BaseDirectory).
            ConfigureServices((context, services) =>
            {
                // Activation handlers
                services.AddTransient<ActivationHandler<LaunchActivatedEventArgs>, DefaultActivationHandler>();
                services.AddTransient<IActivationHandler, AppNotificationActivationHandler>();

                // Services
                services.AddSingleton<IAppNotificationService, AppNotificationService>();
                services.AddSingleton<ILocalSettingsService, LocalSettingsService>();
                services.AddSingleton<IThemeSelectorService, ThemeSelectorService>();
                services.AddSingleton<IActivationService, ActivationService>();
                services.AddSingleton<IPageService, PageService>();
                services.AddSingleton<INavigationService, NavigationService>();
                services.AddSingleton<ITimeDisplayService, TimeDisplayService>();
                services.AddSingleton<IDashboardDisplayService, DashboardDisplayService>();

                // 新增：背景服务
                services.AddSingleton<IBackgroundService, FakExam.Services.BackgroundService>();

                // Core Services
                services.AddSingleton<IFileService, FileService>();
                services.AddSingleton<IDashboardProfileService, DashboardProfileService>();
                services.AddSingleton<IClockService, ClockService>();

                // Cross-layer abstractions (impl in UI)
                services.AddSingleton<IProfileLoaderService, ProfileLoaderService>();
                services.AddSingleton<IExamNavigationOrchestrator, ExamNavigationOrchestrator>();

                // Views and ViewModels
                services.AddSingleton<DashboardShowViewModel>();
                services.AddTransient<DashboardShowPage>();
                services.AddTransient<SettingsViewModel>();
                services.AddTransient<SettingsPage>();
                services.AddSingleton<TimeShowViewModel>();
                services.AddTransient<TimeShowPage>();

                // Configuration
                services.Configure<LocalSettingsOptions>(context.Configuration.GetSection(nameof(LocalSettingsOptions)));
            }).
            Build();

        App.GetService<IAppNotificationService>().Initialize();
        UnhandledException += App_UnhandledException;
    }

    private void App_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
    {
        // TODO: log
    }

    protected async override void OnLaunched(LaunchActivatedEventArgs args)
    {
        base.OnLaunched(args);
        await App.GetService<IActivationService>().ActivateAsync(args);
        App.GetService<IClockService>().Start();
        App.GetService<IExamNavigationOrchestrator>().Initialize();

        var profileSvc = App.GetService<IDashboardProfileService>();
        bool noProfile = profileSvc.CurrentProfile == null
            || profileSvc.CurrentProfile.ExamInfos == null
            || profileSvc.CurrentProfile.ExamInfos.Count == 0;
        if (noProfile)
        {
            var nav = App.GetService<INavigationService>();
            nav.NavigateTo(typeof(FakExam.ViewModels.TimeShowViewModel).FullName!);
        }
    }
}
