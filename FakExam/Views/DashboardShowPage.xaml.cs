using Microsoft.UI.Xaml.Controls;

using FakExam.ViewModels;

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
    }
}
