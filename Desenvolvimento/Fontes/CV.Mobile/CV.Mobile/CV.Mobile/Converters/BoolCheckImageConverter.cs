using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using CV.Mobile.Helper;
using CV.Mobile.Resources;
using Xamarin.Forms;

namespace CV.Mobile.Converters
{
    public class BoolCheckImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool))
            {
                throw new InvalidOperationException("The target must be a boolean");
            }

            return (bool)value?IconFont.CheckboxMarked:IconFont.CheckboxBlank;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
