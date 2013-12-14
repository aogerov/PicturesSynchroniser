using System;
using System.Linq;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace PicturesSynchroniser.Common
{
    public class BooleanToReadyColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (value is bool && (bool)value) ? new SolidColorBrush(Colors.Orange) : null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            // TODO: Implement this method
            return null;
        }
    }
}