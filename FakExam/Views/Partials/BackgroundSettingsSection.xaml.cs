using FakExam.ViewModels;
using Microsoft.UI.Xaml.Controls;

namespace FakExam.Views.Partials
{
    public sealed partial class BackgroundSettingsSection : UserControl
    {
        public BackgroundSettingsSection()
        {
            this.InitializeComponent();
            DataContext = new BackgroundSettingsViewModel(App.GetService<FakExam.Contracts.Services.IBackgroundService>());
        }
    }
}
