using System;

using Xamarin.Forms;

namespace CV.Mobile.Helpers
{
    public class IntDecimalConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return System.Convert.ToDecimal( value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return System.Convert.ToInt32(Math.Round( System.Convert.ToDecimal(value),0));
        }
    }
}
