
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using TimeWinUI.Core.Models;
using Windows.Storage;
using Windows.Storage.Pickers;
using WinRT.Interop;
using TimeWinUI.Contracts.Services;

namespace TimeWinUI.ViewModels
{
    public partial class DashboardShowViewModel : ObservableObject
    {
        private readonly INavigationService _navigationService;

        [ObservableProperty]
        private ObservableCollection<ExamInfo> _examInfos = new();

        public DashboardShowViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        // 返回
        [RelayCommand]
        private void GoBack()
        {
            if (_navigationService.CanGoBack)
                _navigationService.GoBack();
        }

        // 加载 JSON
        [RelayCommand]
        private async Task LoadJsonAsync()
        {
            var picker = new FileOpenPicker();
            picker.FileTypeFilter.Add(".json");
            picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;

            // WinUI 3 需要绑定窗口句柄
            var hwnd = WindowNative.GetWindowHandle(App.MainWindow);
            InitializeWithWindow.Initialize(picker, hwnd);

            StorageFile file = await picker.PickSingleFileAsync();
            if (file == null) return;

            string json = await FileIO.ReadTextAsync(file);
            var profile = JsonConvert.DeserializeObject<DashboardProfile>(json);

            ExamInfos.Clear();
            foreach (var exam in profile.ExamInfos)
                ExamInfos.Add(exam);
        }
    }
}
