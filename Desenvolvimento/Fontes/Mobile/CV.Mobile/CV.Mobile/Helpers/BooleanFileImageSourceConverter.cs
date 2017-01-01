using System;

using Xamarin.Forms;

namespace CV.Mobile.Helpers
{
    public class BooleanFileImageSourceConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((bool)value)
                return ImageSource.FromFile("list24.png");
            else
                return ImageSource.FromFile("calendar24.png");

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
