using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FakExam.Contracts.Services;
using FakExam.Models;
using WinRT.Interop;
using Windows.Storage.Pickers;

namespace FakExam.ViewModels
{
    public partial class BackgroundSettingsViewModel : ObservableObject
    {
        private readonly IBackgroundService _bg;

        public BackgroundSettingsViewModel(IBackgroundService bg)
        {
            _bg = bg;
            LoadFromCurrent();
        }

        [ObservableProperty] private BackgroundCategory category;
        [ObservableProperty] private MaterialType material;
        [ObservableProperty] private MicaSubKind micaKind;
        [ObservableProperty] private string colorHex = "#14201b";
        [ObservableProperty] private string? imagePath;
        [ObservableProperty] private bool maskEnabled;
        [ObservableProperty] private string maskColorHex = "#000000";
        [ObservableProperty] private double maskOpacity = 0.3;

        public bool IsMaterial => Category == BackgroundCategory.Material;
        public bool IsColor => Category == BackgroundCategory.Color;
        public bool IsImage => Category == BackgroundCategory.Image;

        partial void OnCategoryChanged(BackgroundCategory value)
        {
            OnPropertyChanged(nameof(IsMaterial));
            OnPropertyChanged(nameof(IsColor));
            OnPropertyChanged(nameof(IsImage));
        }

        private void LoadFromCurrent()
        {
            var s = _bg.Current;
            Category = s.Category;
            Material = s.Material;
            MicaKind = s.MicaKind;
            ColorHex = s.ColorHex;
            ImagePath = s.ImagePath;
            MaskEnabled = s.Mask.Enabled;
            MaskColorHex = s.Mask.MaskColorHex;
            MaskOpacity = s.Mask.MaskOpacity;
        }

        private BackgroundSettings ToSettings() => new()
        {
            Category = Category,
            Material = Material,
            MicaKind = MicaKind,
            ColorHex = ColorHex,
            ImagePath = ImagePath,
            Mask = new ImageMaskSettings
            {
                Enabled = MaskEnabled,
                MaskColorHex = MaskColorHex,
                MaskOpacity = MaskOpacity
            }
        };

        [RelayCommand]
        private async Task BrowseImageAsync()
        {
            var picker = new FileOpenPicker();
            picker.FileTypeFilter.Add(".png");
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".bmp");
            var hwnd = WindowNative.GetWindowHandle(App.MainWindow);
            InitializeWithWindow.Initialize(picker, hwnd);
            var file = await picker.PickSingleFileAsync();
            if (file != null) ImagePath = file.Path;
        }

        [RelayCommand]
        private async Task ApplyAsync()
        {
            var s = ToSettings();
            await _bg.SaveAsync(s);
            await _bg.ApplyAsync(App.MainWindow);
        }

        [RelayCommand]
        private void ResetView() => LoadFromCurrent();
    }
}
