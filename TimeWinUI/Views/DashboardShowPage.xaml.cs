using Microsoft.UI.Xaml.Controls;

using TimeWinUI.ViewModels;

namespace TimeWinUI.Views;

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
    }
}
