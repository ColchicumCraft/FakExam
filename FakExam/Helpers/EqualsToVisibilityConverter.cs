using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

namespace FakExam.Helpers
{
    public class EqualsToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string? param = parameter as string;
            if (value == null || param == null) return Visibility.Collapsed;
            return string.Equals(value.ToString(), param, StringComparison.OrdinalIgnoreCase) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
            => throw new NotImplementedException();
    }

    public class EnumConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            // enum -> string name
            return value?.ToString() ?? string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            // string -> enum (type from parameter)
            if (parameter is string typeName && value is string name)
            {
                var type = Type.GetType(typeName);
                if (type != null && type.IsEnum)
                {
                    return Enum.Parse(type, name);
                }
            }
            return value;
        }
    }
}
