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
        this.Loaded += OnLoad;
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

    public void OnLoad(object sender, RoutedEventArgs e)
    {
        InitializeThemeComboBox();
    }

    private void InitializeThemeComboBox()
    {
        if (ViewModel?.ElementTheme != null)
        {
            var theme = ViewModel.ElementTheme.ToString();

            foreach (ComboBoxItem item in ThemeComboBox.Items)
            {
                if (item.Tag?.ToString() == theme)
                {
                    ThemeComboBox.SelectedItem = item;
                    break;
                }
            }
        }
    }

    private void ThemeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ThemeComboBox.SelectedItem is ComboBoxItem selectedItem)
        {
            var themeString = selectedItem.Tag?.ToString();

            if (Enum.TryParse<ElementTheme>(themeString, out var theme))
            {
                if (ViewModel?.ElementTheme != theme)
                {
                    ViewModel.SwitchThemeCommand.Execute(theme);
                }
            }
        }
    }
}
