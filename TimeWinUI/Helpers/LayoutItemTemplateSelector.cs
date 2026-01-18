using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using TimeWinUI.ViewModels;

namespace TimeWinUI.Helpers;

public class LayoutItemTemplateSelector : DataTemplateSelector
{
    public DataTemplate TimeTemplate
    {
        get; set;
    }
    public DataTemplate DateTemplate
    {
        get; set;
    }

    protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
    {
        if (item is DisplayItem displayItem)
        {
            return displayItem.Type == DisplayItemType.Time ? TimeTemplate : DateTemplate;
        }
        return base.SelectTemplateCore(item, container);
    }
}