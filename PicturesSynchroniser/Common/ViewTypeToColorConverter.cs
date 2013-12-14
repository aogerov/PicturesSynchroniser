using System;
using PicturesSynchroniser.Services;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace PicturesSynchroniser.Common
{
    public class ViewTypeToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (!(value is ViewType))
            {
                return null;
            }

            SolidColorBrush mySolidColorBrush = new SolidColorBrush();

            var viewType = (ViewType)value;
            if (viewType == ViewType.Basic)
            {
                mySolidColorBrush.Color = Color.FromArgb(20, 109, 13, 255);
                return mySolidColorBrush;
            }
            else if (viewType == ViewType.Custom)
            {
                mySolidColorBrush.Color = Color.FromArgb(20, 232, 12, 192);
                return mySolidColorBrush;
            }
            else
            {
                mySolidColorBrush.Color = Color.FromArgb(20, 255, 34, 0);
                return mySolidColorBrush;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            // TODO: Implement this method
            return ViewType.Main;
        }
    }
}