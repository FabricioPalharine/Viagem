using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using CV.Mobile.Resources;
using Xamarin.Forms;

namespace CV.Mobile.Converters
{
    public class SimNaoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool))
            {
                throw new InvalidOperationException("The target must be a boolean");
            }

            return (bool)value?AppResource.Sim:AppResource.Nao;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
