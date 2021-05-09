using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace CV.Mobile.Converters
{
    public class InverseBoolConverterParent : BindableObject, IValueConverter
    {
        public static readonly BindableProperty ParentEnabledProperty = BindableProperty.Create(nameof(ParentEnabledProperty), typeof(bool), typeof(InverseBoolConverterParent));

        public bool ParentEnabled
        {
            get { return (bool)GetValue(ParentEnabledProperty); }
            set { SetValue(ParentEnabledProperty, value); }
        }


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool))
            {
                throw new InvalidOperationException("The target must be a boolean");
            }

            return !(bool)value && ParentEnabled;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
