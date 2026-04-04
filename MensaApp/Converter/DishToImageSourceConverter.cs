using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using MensaPortable;


namespace MensaApp.Converter
{
    
    public class DishToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Dish dish)
            {
                if (dish.Name == "Info")
                {
                    Application.Current.Resources.TryGetValue("IconMore", out object icon);
                    return icon;
                }
                else if (dish.IsSideDish)
                {
                    Application.Current.Resources.TryGetValue("IconAddition", out object icon);
                    return icon;
                }
                else
                {
                    Application.Current.Resources.TryGetValue("IconFood", out object icon);
                    return icon;
                }
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}
