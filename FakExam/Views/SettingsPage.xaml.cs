using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using FakExam.Contracts.Services;

using FakExam.ViewModels;

namespace FakExam.Views;

// TODO: Set the URL for your privacy policy by updating SettingsPage_PrivacyTermsLink.NavigateUri in Resources.resw.
public sealed partial class SettingsPage : Page
{
    public SettingsViewModel ViewModel
    {
        get;
    }

    public SettingsPage()
    {
        ViewModel = App.GetService<SettingsViewModel>();
        InitializeComponent();
    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        var navigationService = App.GetService<INavigationService>();
        if (navigationService.CanGoBack)
        {
            navigationService.GoBack();
        }
        else
        {
            navigationService.NavigateTo(typeof(TimeShowViewModel).FullName!);
        }
    }
}
