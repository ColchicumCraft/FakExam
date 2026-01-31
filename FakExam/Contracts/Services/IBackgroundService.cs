using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using FakExam.Models;

namespace FakExam.Contracts.Services
{
    public interface IBackgroundService
    {
        BackgroundSettings Current { get; }
        Task InitializeAsync();
        Task SaveAsync(BackgroundSettings settings);
        Grid EnsureRootHost(Window window, out Frame frame);
        Task ApplyAsync(Window window);
    }
}
