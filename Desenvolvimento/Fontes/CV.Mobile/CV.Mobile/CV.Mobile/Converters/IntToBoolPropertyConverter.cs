using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace CV.Mobile.Converters
{
    public class IntToBoolPropertyConverter : BindableObject, IValueConverter
    {
        public static readonly BindableProperty ValorProperty = BindableProperty.Create(nameof(Valor), typeof(int), typeof(IntToBoolPropertyConverter));

        public int Valor
        {
            get { return (int)GetValue(ValorProperty); }
            set { SetValue(ValorProperty, value); }
        }


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
           
            if (parameter != null && System.Convert.ToBoolean( parameter))
                return System.Convert.ToInt32(value) != Valor;
            return System.Convert.ToInt32(value) == Valor;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
