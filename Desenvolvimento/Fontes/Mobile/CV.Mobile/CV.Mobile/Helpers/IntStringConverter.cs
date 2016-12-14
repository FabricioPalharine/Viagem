using System;

using Xamarin.Forms;

namespace CV.Mobile.Helpers
{
    public class IntStringConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return System.Convert.ToString( value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null && System.Convert.ToString(value) != string.Empty) 
                return System.Convert.ToInt32(value);
            else
                return null;
        }
    }
}
