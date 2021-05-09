using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using CV.Mobile.Resources;
using Xamarin.Forms;

namespace CV.Mobile.Converters
{
    public class BoolTextoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool) || parameter == null)
            {
                return string.Empty;
            }
            string[] parametros = System.Convert.ToString(parameter).Split('|');
            if (parametros.Length > 1)
            return (bool)value ? parametros[1] : parametros[0];
            return (bool)value ? parametros[0] : string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
