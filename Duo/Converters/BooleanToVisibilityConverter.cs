using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace Duo.Converters
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool isVisible)
            {
                return isVisible ? Visibility.Visible : Visibility.Collapsed;
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException("ConvertBack is not supported in BooleanToVisibilityConverter.");
        }
    }
}
