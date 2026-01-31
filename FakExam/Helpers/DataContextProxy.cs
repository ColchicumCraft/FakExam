using System.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;

namespace FakExam.Helpers;

public class DataContextProxy : FrameworkElement, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public DataContextProxy()
    {
        this.Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        // 获取父级 DataContext
        var parent = VisualTreeHelper.GetParent(this);
        while (parent != null && !(parent is FrameworkElement))
        {
            parent = VisualTreeHelper.GetParent(parent);
        }

        if (parent is FrameworkElement frameworkElement)
        {
            DataContext = frameworkElement.DataContext;
        }
    }

    public object? DataSource
    {
        get => GetValue(DataSourceProperty);
        set => SetValue(DataSourceProperty, value);
    }

    public static readonly DependencyProperty DataSourceProperty =
        DependencyProperty.Register(
            nameof(DataSource),
            typeof(object),
            typeof(DataContextProxy),
            new PropertyMetadata(null, OnDataSourceChanged));

    private static void OnDataSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is DataContextProxy proxy)
        {
            proxy.DataContext = e.NewValue;
        }
    }
}