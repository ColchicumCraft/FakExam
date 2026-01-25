
using System.Threading.Tasks;
using Windows.Storage.Pickers;
using Windows.Storage;
using TimeWinUI.Core.Contracts.Services;
using TimeWinUI.Contracts.Services;
using WinRT.Interop;

namespace TimeWinUI.Services;

public sealed class ProfileLoaderService : IProfileLoaderService
{
    private readonly IDashboardProfileService _profileService;

    public ProfileLoaderService(IDashboardProfileService profileService)
    {
        _profileService = profileService;
    }

    public async Task<bool> PickAndLoadAsync()
    {
        var picker = new FileOpenPicker();
        picker.FileTypeFilter.Add(".json");
        picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;

        var hwnd = WindowNative.GetWindowHandle(App.MainWindow);
        InitializeWithWindow.Initialize(picker, hwnd);

        StorageFile file = await picker.PickSingleFileAsync();
        if (file == null) return false;

        await _profileService.LoadFromFileAsync(file.Path);
        return true;
    }
}
