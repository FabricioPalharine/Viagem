using System;

using Xamarin.Forms;

namespace CV.Mobile.Helpers
{
    public class TipoFontAttributeConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return System.Convert.ToString( value) == "S"? FontAttributes.Bold : FontAttributes.None;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            
                return null;
        }
    }
}
