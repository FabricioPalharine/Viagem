using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace CV.Mobile.Converters
{
    public class MenuVisibleConverter : BindableObject, IValueConverter
    {
        public static readonly BindableProperty ViagemAbertaProperty = BindableProperty.Create(nameof(ViagemAbertaProperty), typeof(bool), typeof(MenuVisibleConverter), true, BindingMode.TwoWay, null, OnViagemAbertaChanged);
        public static readonly BindableProperty EditaViagemProperty = BindableProperty.Create(nameof(EditaViagemProperty), typeof(bool), typeof(MenuVisibleConverter), true, BindingMode.TwoWay, null, OnViagemAbertaChanged);

        public bool ViagemAberta
        {
            get { return (bool)GetValue(ViagemAbertaProperty); }
            set { SetValue(ViagemAbertaProperty, value); }
        }

        public bool EditaViagem
        {
            get { return (bool)GetValue(EditaViagemProperty); }
            set { SetValue(EditaViagemProperty, value); }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return true;
            else if (System.Convert.ToString(value) == "Iniciar GPS" || System.Convert.ToString(value) == "Parar GPS")
                return ViagemAberta;
            else if (System.Convert.ToString(value) == "Sincronizar" || System.Convert.ToString(value) == "Abrir Viagem" || System.Convert.ToString(value) == "Fechar Viagem" || System.Convert.ToString(value) == "Fotos")
                return EditaViagem;
            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

        private static void OnViagemAbertaChanged(BindableObject bindable, object oldValue, object newValue)
        {
        }
    }
}
