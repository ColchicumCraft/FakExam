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
}
