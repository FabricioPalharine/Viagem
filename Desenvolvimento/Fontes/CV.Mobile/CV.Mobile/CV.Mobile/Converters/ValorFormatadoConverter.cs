using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Xamarin.Forms;

namespace CV.Mobile.Converters
{
    public class ValorFormatadoConverter : BindableObject, IValueConverter
    {
        public static readonly BindableProperty MascararProperty = BindableProperty.Create(nameof(Mascarar), typeof(bool), typeof(ValorFormatadoConverter));

        public bool Mascarar
        {
            get { return (bool)GetValue(MascararProperty); }
            set { SetValue(MascararProperty, value); }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || !(value is decimal)) 
                return value;

            var valor = (decimal)value;

            string texto = valor>=0? string.Concat("R$ ", valor.ToString("N2"))
                : string.Concat("-R$ ",  Math.Abs(valor).ToString("N2"));
            if (Mascarar)
            {
                return "*****";
            }
            else
                return texto;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
           
            return null;
        }
    }
}
