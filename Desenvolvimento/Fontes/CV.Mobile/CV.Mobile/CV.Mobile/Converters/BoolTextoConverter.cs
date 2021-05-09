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
                throw new InvalidOperationException("The target must be a boolean");
            }
            string[] parametros = System.Convert.ToString(parameter).Split('|');

            return (bool)value ? parametros[1] : parametros[0];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
