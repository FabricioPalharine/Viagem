using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace CV.Mobile.Converters
{
    public class IntToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            
            if ((value is int || value is Enum)  && parameter != null &&  int.TryParse( parameter.ToString(),  out var parameterInt))
            {
                int valor1 = System.Convert.ToInt32(value);
              
                return valor1 == parameterInt;
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
