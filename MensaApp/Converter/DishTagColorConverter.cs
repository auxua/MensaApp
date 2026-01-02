using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace MensaApp.Converter
{
    public class DishTagColorConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var name = (value as string)?.Trim().ToLowerInvariant() ?? "";

            return name switch
            {
                "vegan" => Colors.Green,
                "veggie" => Colors.DarkGreen,
                "schwein" => Colors.Red,
                "rind" => Colors.Brown,
                "geflügel" => Colors.Orange,
                "gefluegel" => Colors.Orange,   // Fallback: Umlaut missing
                "fisch" => Colors.Blue,
                _ => Colors.Gray
            };
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }

}
