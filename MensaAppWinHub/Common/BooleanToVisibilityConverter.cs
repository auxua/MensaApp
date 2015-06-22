using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace MensaAppWinHub.Common
{
    public sealed class BooleanToVisibilityConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var v = (value is bool && (bool)value) ? Visibility.Visible : Visibility.Collapsed;
            return v;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value is Visibility && (Visibility)value == Visibility.Visible;
        }
    }
}