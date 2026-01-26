
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using FakExam.Models;
using FakExam.Core.Models;

namespace FakExam.Helpers
{
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
        public DataTemplate ExamTemplate
        {
            get; set;
        }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (item is DisplayItem displayItem)
            {
                return displayItem.Type switch
                {
                    DisplayItemType.Time => TimeTemplate,
                    DisplayItemType.Date => DateTemplate,
                    DisplayItemType.Exam => ExamTemplate,
                    _ => base.SelectTemplateCore(item, container)
                };
            }
            return base.SelectTemplateCore(item, container);
        }
    }
}
