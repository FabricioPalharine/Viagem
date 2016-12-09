using System;

using Xamarin.Forms;

namespace CV.Mobile.Helpers
{
    public class BooleanImageSourceConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((bool)value)
                return ImageSource.FromResource("CV.Mobile.Icones.checked24.png");
            else
                return ImageSource.FromResource("CV.Mobile.Icones.unchecked24.png");

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
