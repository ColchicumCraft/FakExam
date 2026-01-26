using System.Diagnostics;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using FakExam.ViewModels;

namespace FakExam.Views;

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

    private void ExamLabelColorPickerButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && Resources["ExamLabelColorPickerFlyout"] is Flyout flyout) flyout.ShowAt(button);
    }
    private void ExamLabelColorConfirmButton_Click(object sender, RoutedEventArgs e)
    {
        ViewModel.ConfirmExamLabelColorCommand.Execute(null);
        if (Resources["ExamLabelColorPickerFlyout"] is Flyout flyout) flyout.Hide();
    }
    private void ExamLabelColorCancelButton_Click(object sender, RoutedEventArgs e)
    {
        if (Resources["ExamLabelColorPickerFlyout"] is Flyout flyout) flyout.Hide();
    }

    private void ExamStatusColorPickerButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && Resources["ExamStatusColorPickerFlyout"] is Flyout flyout) flyout.ShowAt(button);
    }
    private void ExamStatusColorConfirmButton_Click(object sender, RoutedEventArgs e)
    {
        ViewModel.ConfirmExamStatusColorCommand.Execute(null);
        if (Resources["ExamStatusColorPickerFlyout"] is Flyout flyout) flyout.Hide();
    }
    private void ExamStatusColorCancelButton_Click(object sender, RoutedEventArgs e)
    {
        if (Resources["ExamStatusColorPickerFlyout"] is Flyout flyout) flyout.Hide();
    }

    private void ExamStartColorPickerButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && Resources["ExamStartColorPickerFlyout"] is Flyout flyout) flyout.ShowAt(button);
    }
    private void ExamStartColorConfirmButton_Click(object sender, RoutedEventArgs e)
    {
        ViewModel.ConfirmExamStartColorCommand.Execute(null);
        if (Resources["ExamStartColorPickerFlyout"] is Flyout flyout) flyout.Hide();
    }
    private void ExamStartColorCancelButton_Click(object sender, RoutedEventArgs e)
    {
        if (Resources["ExamStartColorPickerFlyout"] is Flyout flyout) flyout.Hide();
    }

    private void ExamNameColorPickerButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && Resources["ExamNameColorPickerFlyout"] is Flyout flyout) flyout.ShowAt(button);
    }
    private void ExamNameColorConfirmButton_Click(object sender, RoutedEventArgs e)
    {
        ViewModel.ConfirmExamNameColorCommand.Execute(null);
        if (Resources["ExamNameColorPickerFlyout"] is Flyout flyout) flyout.Hide();
    }
    private void ExamNameColorCancelButton_Click(object sender, RoutedEventArgs e)
    {
        if (Resources["ExamNameColorPickerFlyout"] is Flyout flyout) flyout.Hide();
    }

    private void ExamEndColorPickerButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && Resources["ExamEndColorPickerFlyout"] is Flyout flyout) flyout.ShowAt(button);
    }
    private void ExamEndColorConfirmButton_Click(object sender, RoutedEventArgs e)
    {
        ViewModel.ConfirmExamEndColorCommand.Execute(null);
        if (Resources["ExamEndColorPickerFlyout"] is Flyout flyout) flyout.Hide();
    }
    private void ExamEndColorCancelButton_Click(object sender, RoutedEventArgs e)
    {
        if (Resources["ExamEndColorPickerFlyout"] is Flyout flyout) flyout.Hide();
    }

    private void ExamRemainingColorPickerButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && Resources["ExamRemainingColorPickerFlyout"] is Flyout flyout) flyout.ShowAt(button);
    }
    private void ExamRemainingColorConfirmButton_Click(object sender, RoutedEventArgs e)
    {
        ViewModel.ConfirmExamRemainingColorCommand.Execute(null);
        if (Resources["ExamRemainingColorPickerFlyout"] is Flyout flyout) flyout.Hide();
    }
    private void ExamRemainingColorCancelButton_Click(object sender, RoutedEventArgs e)
    {
        if (Resources["ExamRemainingColorPickerFlyout"] is Flyout flyout) flyout.Hide();
    }

    private void SettingsPivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is Pivot p && p.SelectedItem is PivotItem pi)
        {
            var tag = pi.Tag as string;
            if (tag == "time")
            {
                Trace.WriteLine("TIME");
                TimeDatePanel.Visibility = Visibility.Visible;
                ExamPanel.Visibility = Visibility.Collapsed;
            }
            else if (tag == "exam")
            {
                Trace.WriteLine("EXAM");
                TimeDatePanel.Visibility = Visibility.Collapsed;
                ExamPanel.Visibility = Visibility.Visible;
            }
        }
    }

}
