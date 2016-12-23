using System;

using Xamarin.Forms;

namespace CV.Mobile.Helpers
{
    public class IntegerImageSourceConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var valor = (int?)value;
            if (valor.HasValue && valor >= System.Convert.ToInt32(parameter))
                return ImageSource.FromResource("CV.Mobile.Icones.star_selected.png");
            else
                return ImageSource.FromResource("CV.Mobile.Icones.star_outline.png");

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
