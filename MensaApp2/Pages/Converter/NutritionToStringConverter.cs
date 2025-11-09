using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MensaCommon.DataTypes;

namespace MensaApp2.Pages.Converter
{
    public class NutritionToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!App.getConfig("ShowNutrition")) return "";
            if (value == null) return "";
            if (value is Nutrition)
            {
                Nutrition nutrition = (Nutrition)value;
                string text = "";
                text += "\t" + MensaApp2.Resources.Strings.MensaApp2Resources.Caloric + ": " + nutrition.Caloric + Environment.NewLine;
                text += "\t" + MensaApp2.Resources.Strings.MensaApp2Resources.Carbohydrates + ": " + nutrition.Carbohydrates + Environment.NewLine;
                text += "\t" + MensaApp2.Resources.Strings.MensaApp2Resources.Fat + ": " + nutrition.Fat + Environment.NewLine;
                text += "\t" + MensaApp2.Resources.Strings.MensaApp2Resources.Proteins + ": " + nutrition.Proteins;
                return text;
            }
            throw new ArgumentException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
