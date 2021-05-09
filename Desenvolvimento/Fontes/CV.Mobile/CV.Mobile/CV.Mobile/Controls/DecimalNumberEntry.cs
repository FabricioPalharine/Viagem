using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace CV.Mobile.Controls
{
    public class DecimalNumberEntry : Entry
    {
        public static readonly BindableProperty ValueProperty =
           BindableProperty.Create(nameof(Value), typeof(decimal), typeof(DecimalNumberEntry), 0m, BindingMode.TwoWay, null, new BindableProperty.BindingPropertyChangedDelegate(OnValueChanged));

        public static readonly BindableProperty DecimalPlacesProperty =
    BindableProperty.Create(nameof(DecimalPlaces), typeof(int), typeof(DecimalNumberEntry), 2);

        public decimal Value
        {
            get { return (decimal)GetValue(ValueProperty); }
            set
            {
                SetValue(ValueProperty, value);
               
            }
        }

        public event EventHandler<TextChangedEventArgs> ValueChanged;



        public int DecimalPlaces
        {
            get { return (int)GetValue(DecimalPlacesProperty); }
            set { SetValue(DecimalPlacesProperty, value); }
        }

        public System.Globalization.NumberFormatInfo Idioma;
        public DecimalNumberEntry()
        {
            //ShouldReactToTextChanges = true;
            Idioma = CultureInfo.InstalledUICulture.NumberFormat;
            decimal newValue = Value;
            string strValor = RetornarTextoPuro(newValue.ToString(String.Concat("N", DecimalPlaces), Idioma));
            long valor = long.Parse(strValor);
            decimal valorDecimal = Convert.ToDecimal(valor) / Convert.ToDecimal(Math.Pow(10, DecimalPlaces));
            var newText = valorDecimal.ToString(String.Concat("N", DecimalPlaces), Idioma);
            Text = newText;

        }

        public string RetornarTextoPuro(string input)
        {
            List<char> numeros = new List<char>();
            foreach (var letra in input.ToString())
                if (char.IsDigit(letra))
                    numeros.Add(letra);
            string strValor = new string(numeros.ToArray());
            return strValor;
        }

   
   
        private static void OnValueChanged(BindableObject bindable, object oldValue, object newValue)
        {
            DecimalNumberEntry element = (DecimalNumberEntry)bindable;
           

                string strValor = element.RetornarTextoPuro(Convert.ToDecimal(newValue).ToString(String.Concat("N", element.DecimalPlaces), element.Idioma));
                long valor = long.Parse(strValor);
                decimal valorDecimal = Convert.ToDecimal(valor) / Convert.ToDecimal(Math.Pow(10, element.DecimalPlaces)); 
                var newText = valorDecimal.ToString(String.Concat("N", element.DecimalPlaces), element.Idioma);
            if (element.Text != newText)
              element.Text = newText;
            if (element.ValueChanged != null)
                element.ValueChanged(element, new TextChangedEventArgs(Convert.ToString(oldValue), newText));
          
        }
      

        
    }
}
