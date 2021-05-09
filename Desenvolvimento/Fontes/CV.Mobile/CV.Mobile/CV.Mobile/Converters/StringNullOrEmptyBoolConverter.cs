using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace CV.Mobile.Converters
{
    public class StringNullOrEmptyBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool resultado = true;
            if (value == null)
                resultado = false;
            else
            {
                if (value is DateTime)
                {
                    resultado = ((DateTime)value) != DateTime.MinValue;
                }
                if (value is double || value is decimal || value is int)
                {
                    resultado = System.Convert.ToDecimal(value) != 0;
                }
                else
                {
                    var stringValue = System.Convert.ToString(value);
                    resultado = !string.IsNullOrEmpty(stringValue);
                }
            }
            if (parameter == null || System.Convert.ToBoolean(parameter))
                return resultado;
            else
                return !resultado;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
