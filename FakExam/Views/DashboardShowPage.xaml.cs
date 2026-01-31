using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using FakExam.ViewModels;
using System.Diagnostics;

namespace FakExam.Views;

public sealed partial class DashboardShowPage : Page
{
    public DashboardShowViewModel ViewModel
    {
        get;
    }

    public DashboardShowPage()
    {
        ViewModel = App.GetService<DashboardShowViewModel>();
        InitializeComponent();

        // 初始布局设置
        UpdateLayoutOrder();

        // 监听属性变化
        ViewModel.PropertyChanged += ViewModel_PropertyChanged;
    }

    // 颜色选择器按钮点击事件
    private void TitleColorPickerButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && Resources["TitleColorPickerFlyout"] is Flyout flyout)
        {
            flyout.ShowAt(button);
        }
    }

    private void TitleColorConfirmButton_Click(object sender, RoutedEventArgs e)
    {
        ViewModel.ConfirmTitleColorCommand.Execute(null);
        if (Resources["TitleColorPickerFlyout"] is Flyout flyout)
        {
            flyout.Hide();
        }
    }

    private void TitleColorCancelButton_Click(object sender, RoutedEventArgs e)
    {
        if (Resources["TitleColorPickerFlyout"] is Flyout flyout)
        {
            flyout.Hide();
        }
    }

    private void MessageColorPickerButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && Resources["MessageColorPickerFlyout"] is Flyout flyout)
        {
            flyout.ShowAt(button);
        }
    }

    private void MessageColorConfirmButton_Click(object sender, RoutedEventArgs e)
    {
        ViewModel.ConfirmMessageColorCommand.Execute(null);
        if (Resources["MessageColorPickerFlyout"] is Flyout flyout)
        {
            flyout.Hide();
        }
    }

    private void MessageColorCancelButton_Click(object sender, RoutedEventArgs e)
    {
        if (Resources["MessageColorPickerFlyout"] is Flyout flyout)
        {
            flyout.Hide();
        }
    }

    private void StatusLabelColorPickerButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && Resources["StatusLabelColorPickerFlyout"] is Flyout flyout)
        {
            flyout.ShowAt(button);
        }
    }

    private void StatusLabelColorConfirmButton_Click(object sender, RoutedEventArgs e)
    {
        ViewModel.ConfirmStatusLabelColorCommand.Execute(null);
        if (Resources["StatusLabelColorPickerFlyout"] is Flyout flyout)
        {
            flyout.Hide();
        }
    }

    private void StatusLabelColorCancelButton_Click(object sender, RoutedEventArgs e)
    {
        if (Resources["StatusLabelColorPickerFlyout"] is Flyout flyout)
        {
            flyout.Hide();
        }
    }

    private void CurrentExamNameColorPickerButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && Resources["CurrentExamNameColorPickerFlyout"] is Flyout flyout)
        {
            flyout.ShowAt(button);
        }
    }

    private void CurrentExamNameColorConfirmButton_Click(object sender, RoutedEventArgs e)
    {
        ViewModel.ConfirmCurrentExamNameColorCommand.Execute(null);
        if (Resources["CurrentExamNameColorPickerFlyout"] is Flyout flyout)
        {
            flyout.Hide();
        }
    }

    private void CurrentExamNameColorCancelButton_Click(object sender, RoutedEventArgs e)
    {
        if (Resources["CurrentExamNameColorPickerFlyout"] is Flyout flyout)
        {
            flyout.Hide();
        }
    }

    private void CurrentExamTimeRangeColorPickerButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && Resources["CurrentExamTimeRangeColorPickerFlyout"] is Flyout flyout)
        {
            flyout.ShowAt(button);
        }
    }

    private void CurrentExamTimeRangeColorConfirmButton_Click(object sender, RoutedEventArgs e)
    {
        ViewModel.ConfirmCurrentExamTimeRangeColorCommand.Execute(null);
        if (Resources["CurrentExamTimeRangeColorPickerFlyout"] is Flyout flyout)
        {
            flyout.Hide();
        }
    }

    private void CurrentExamTimeRangeColorCancelButton_Click(object sender, RoutedEventArgs e)
    {
        if (Resources["CurrentExamTimeRangeColorPickerFlyout"] is Flyout flyout)
        {
            flyout.Hide();
        }
    }

    private void RemainingTimeTextColorPickerButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && Resources["RemainingTimeTextColorPickerFlyout"] is Flyout flyout)
        {
            flyout.ShowAt(button);
        }
    }

    private void RemainingTimeTextColorConfirmButton_Click(object sender, RoutedEventArgs e)
    {
        ViewModel.ConfirmRemainingTimeTextColorCommand.Execute(null);
        if (Resources["RemainingTimeTextColorPickerFlyout"] is Flyout flyout)
        {
            flyout.Hide();
        }
    }

    private void RemainingTimeTextColorCancelButton_Click(object sender, RoutedEventArgs e)
    {
        if (Resources["RemainingTimeTextColorPickerFlyout"] is Flyout flyout)
        {
            flyout.Hide();
        }
    }

    private void CurrentStatusTextColorPickerButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && Resources["CurrentStatusTextColorPickerFlyout"] is Flyout flyout)
        {
            flyout.ShowAt(button);
        }
    }

    private void CurrentStatusTextColorConfirmButton_Click(object sender, RoutedEventArgs e)
    {
        ViewModel.ConfirmCurrentStatusTextColorCommand.Execute(null);
        if (Resources["CurrentStatusTextColorPickerFlyout"] is Flyout flyout)
        {
            flyout.Hide();
        }
    }

    private void CurrentStatusTextColorCancelButton_Click(object sender, RoutedEventArgs e)
    {
        if (Resources["CurrentStatusTextColorPickerFlyout"] is Flyout flyout)
        {
            flyout.Hide();
        }
    }

    private void CurrentTimeColorPickerButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && Resources["CurrentTimeColorPickerFlyout"] is Flyout flyout)
        {
            flyout.ShowAt(button);
        }
    }

    private void CurrentTimeColorConfirmButton_Click(object sender, RoutedEventArgs e)
    {
        ViewModel.ConfirmCurrentTimeColorCommand.Execute(null);
        if (Resources["CurrentTimeColorPickerFlyout"] is Flyout flyout)
        {
            flyout.Hide();
        }
    }

    private void CurrentTimeColorCancelButton_Click(object sender, RoutedEventArgs e)
    {
        if (Resources["CurrentTimeColorPickerFlyout"] is Flyout flyout)
        {
            flyout.Hide();
        }
    }

    private void TableHeaderColorPickerButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && Resources["TableHeaderColorPickerFlyout"] is Flyout flyout)
        {
            flyout.ShowAt(button);
        }
    }

    private void TableHeaderColorConfirmButton_Click(object sender, RoutedEventArgs e)
    {
        ViewModel.ConfirmTableHeaderColorCommand.Execute(null);
        if (Resources["TableHeaderColorPickerFlyout"] is Flyout flyout)
        {
            flyout.Hide();
        }
    }

    private void TableHeaderColorCancelButton_Click(object sender, RoutedEventArgs e)
    {
        if (Resources["TableHeaderColorPickerFlyout"] is Flyout flyout)
        {
            flyout.Hide();
        }
    }

    private void TableContentColorPickerButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && Resources["TableContentColorPickerFlyout"] is Flyout flyout)
        {
            flyout.ShowAt(button);
        }
    }

    private void TableContentColorConfirmButton_Click(object sender, RoutedEventArgs e)
    {
        ViewModel.ConfirmTableContentColorCommand.Execute(null);
        if (Resources["TableContentColorPickerFlyout"] is Flyout flyout)
        {
            flyout.Hide();
        }
    }

    private void TableContentColorCancelButton_Click(object sender, RoutedEventArgs e)
    {
        if (Resources["TableContentColorPickerFlyout"] is Flyout flyout)
        {
            flyout.Hide();
        }
    }

    private void SettingsPivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is Pivot p && p.SelectedItem is PivotItem pi)
        {
            var tag = pi.Tag as string;
            if (tag == "status")
            {
                StatusPanelSettings.Visibility = Visibility.Visible;
                LayoutPanelSettings.Visibility = Visibility.Collapsed;
            }
            else if (tag == "layout")
            {
                StatusPanelSettings.Visibility = Visibility.Collapsed;
                LayoutPanelSettings.Visibility = Visibility.Visible;
            }
        }
    }

    private void UpdateLayoutOrder()
    {
        // 根据布局顺序更新列定义
        if (ViewModel.SelectedLayoutOrder == "TableOnLeft")
        {
            // 表格在左，状态在右
            Grid.SetColumn(StatusPanel, 1);
            Grid.SetColumn(TablePanel, 0);
        }
        else
        {
            // 状态在左，表格在右（默认）
            Grid.SetColumn(StatusPanel, 0);
            Grid.SetColumn(TablePanel, 1);
        }
    }

    private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ViewModel.SelectedLayoutOrder))
        {
            UpdateLayoutOrder();
        }
    }
}