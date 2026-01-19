using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using TimeWinUI.ViewModels;

namespace TimeWinUI.Views;

public sealed partial class TimeShowPage : Page
{
    public TimeShowViewModel ViewModel
    {
        get;
    }

    public TimeShowPage()
    {
        ViewModel = App.GetService<TimeShowViewModel>();
        InitializeComponent();
    }

    private void TimeColorPickerButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && Resources["TimeColorPickerFlyout"] is Flyout flyout)
        {
            flyout.ShowAt(button);
        }
    }

    private void DateColorPickerButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && Resources["DateColorPickerFlyout"] is Flyout flyout)
        {
            flyout.ShowAt(button);
        }
    }

    private void TimeColorConfirmButton_Click(object sender, RoutedEventArgs e)
    {
        ViewModel.ConfirmTimeColorCommand.Execute(null);

        if (Resources["TimeColorPickerFlyout"] is Flyout flyout)
        {
            flyout.Hide();
        }
    }

    private void TimeColorCancelButton_Click(object sender, RoutedEventArgs e)
    {
        if (Resources["TimeColorPickerFlyout"] is Flyout flyout)
        {
            flyout.Hide();
        }
    }

    private void DateColorConfirmButton_Click(object sender, RoutedEventArgs e)
    {
        ViewModel.ConfirmDateColorCommand.Execute(null);

        if (Resources["DateColorPickerFlyout"] is Flyout flyout)
        {
            flyout.Hide();
        }
    }

    private void DateColorCancelButton_Click(object sender, RoutedEventArgs e)
    {
        if (Resources["DateColorPickerFlyout"] is Flyout flyout)
        {
            flyout.Hide();
        }
    }
    private void CommandBar_Closing(object sender, object e)
    {
        if (sender is CommandBar commandBar)
        {
            _ = DispatcherQueue.TryEnqueue(() =>
            {
                commandBar.IsOpen = true;
            });
        }
    }
}