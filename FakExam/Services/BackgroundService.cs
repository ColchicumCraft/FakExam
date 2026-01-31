using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using FakExam.Contracts.Services;
using FakExam.Models;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Shapes;

namespace FakExam.Services
{
    public sealed class BackgroundService : IBackgroundService
    {
        private const string SettingsKey = "AppBackgroundSettings";
        private readonly ILocalSettingsService _local;
        private BackgroundSettings _current = new();
        public BackgroundSettings Current => _current;

        public BackgroundService(ILocalSettingsService local)
        {
            _local = local;
        }

        public async Task InitializeAsync()
        {
            _current = await _local.ReadSettingAsync<BackgroundSettings>(SettingsKey) ?? new BackgroundSettings();
        }

        public async Task SaveAsync(BackgroundSettings settings)
        {
            _current = settings;
            await _local.SaveSettingAsync(SettingsKey, settings);
        }

        public Grid EnsureRootHost(Window window, out Frame frame)
        {
            if (window.Content is Frame f)
            {
                var host = new Grid();
                var bg = new Grid { Name = "BackgroundLayer", IsHitTestVisible = false };
                host.Children.Add(bg);
                host.Children.Add(f);
                window.Content = host;
                frame = f;
                return bg;
            }
            if (window.Content is Grid g)
            {
                Grid? bg = null;
                foreach (var c in g.Children)
                {
                    if (c is FrameworkElement fe && fe.Name == "BackgroundLayer") { bg = (Grid)fe; break; }
                }
                frame = null;
                foreach (var c in g.Children)
                {
                    if (c is Frame fr) { frame = fr; break; }
                }
                if (bg == null)
                {
                    bg = new Grid { Name = "BackgroundLayer", IsHitTestVisible = false };
                    g.Children.Insert(0, bg);
                }
                if (frame == null)
                {
                    frame = new Frame();
                    g.Children.Add(frame);
                }
                return bg;
            }
            var root = new Grid();
            var background = new Grid { Name = "BackgroundLayer", IsHitTestVisible = false };
            frame = new Frame();
            root.Children.Add(background);
            root.Children.Add(frame);
            window.Content = root;
            return background;
        }

        public async Task ApplyAsync(Window window)
        {
            var bg = EnsureRootHost(window, out _);
            try
            {
                switch (_current.Category)
                {
                    case BackgroundCategory.Material:
                        ApplyMaterial(window, bg);
                        break;
                    case BackgroundCategory.Color:
                        ApplyColor(window, bg);
                        break;
                    case BackgroundCategory.Image:
                        await ApplyImageAsync(window, bg);
                        break;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[BackgroundService] Apply failed: {ex.Message}");
                ApplyFallbackMica(window, bg);
            }
        }

        private void ClearBrushesForNonBackdrop(Grid backgroundLayer)
        {
            backgroundLayer.Background = null;
            backgroundLayer.Children.Clear();
        }

        private void ApplyMaterial(Window window, Grid backgroundLayer)
        {
            ClearBrushesForNonBackdrop(backgroundLayer);
            if (_current.Material == MaterialType.Acrylic)
            {
                window.SystemBackdrop = new DesktopAcrylicBackdrop();
                return;
            }
            var mica = new MicaBackdrop();
            if (_current.MicaKind == MicaSubKind.BaseAlt)
            {
                mica.Kind = MicaKind.BaseAlt;
            }
            window.SystemBackdrop = mica;
        }

        private void ApplyColor(Window window, Grid backgroundLayer)
        {
            window.SystemBackdrop = null;
            ClearBrushesForNonBackdrop(backgroundLayer);
            var color = ParseColor(_current.ColorHex ?? "#202020");
            backgroundLayer.Background = new SolidColorBrush(color);
        }

        private async Task ApplyImageAsync(Window window, Grid backgroundLayer)
        {
            window.SystemBackdrop = null;
            ClearBrushesForNonBackdrop(backgroundLayer);
            if (string.IsNullOrWhiteSpace(_current.ImagePath) || !File.Exists(_current.ImagePath))
                throw new FileNotFoundException("Image not found", _current.ImagePath);

            var bmp = new BitmapImage(new Uri(_current.ImagePath));
            var brush = new ImageBrush
            {
                ImageSource = bmp,
                Stretch = Stretch.UniformToFill,
                AlignmentX = AlignmentX.Center,
                AlignmentY = AlignmentY.Center
            };
            backgroundLayer.Background = brush;

            if (_current.Mask?.Enabled == true)
            {
                var maskColor = ParseColor(_current.Mask.MaskColorHex ?? "#000000");
                var rect = new Rectangle
                {
                    Fill = new SolidColorBrush(maskColor),
                    Opacity = Math.Clamp(_current.Mask.MaskOpacity, 0, 1)
                };
                backgroundLayer.Children.Add(rect);
            }
            await Task.CompletedTask;
        }

        private void ApplyFallbackMica(Window window, Grid backgroundLayer)
        {
            try
            {
                ClearBrushesForNonBackdrop(backgroundLayer);
                window.SystemBackdrop = new MicaBackdrop();
            }
            catch { }
        }

        private static Windows.UI.Color ParseColor(string hex)
        {
            if (string.IsNullOrWhiteSpace(hex) || hex.Length != 7 || hex[0] != '#')
                return Microsoft.UI.Colors.Black;
            byte r = Convert.ToByte(hex.Substring(1, 2), 16);
            byte g = Convert.ToByte(hex.Substring(3, 2), 16);
            byte b = Convert.ToByte(hex.Substring(5, 2), 16);
            return Windows.UI.Color.FromArgb(255, r, g, b);
        }
    }
}
