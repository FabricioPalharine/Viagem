using System;

using Xamarin.Forms;

namespace CV.Mobile.Controls
{
    public class FormattedNumberEntry : Entry
    {
        public static readonly BindableProperty ValueProperty =
            BindableProperty.Create(nameof(Value), typeof(decimal), typeof(FormattedNumberEntry), 0m, BindingMode.TwoWay);

        public static readonly BindableProperty DecimalPlacesProperty =
    BindableProperty.Create(nameof(DecimalPlaces), typeof(int), typeof(FormattedNumberEntry), 2);

        public decimal Value
        {
            get { return (decimal)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public int DecimalPlaces
        {
            get { return (int)GetValue(DecimalPlacesProperty); }
            set { SetValue(DecimalPlacesProperty, value); }
        }

        public bool ShouldReactToTextChanges { get; set; }

        public FormattedNumberEntry()
        {
            ShouldReactToTextChanges = true;
        }

        public decimal DumbParse(string input)
        {
            if (input == null) return 0;
            bool temDecimal = false;
            int posicao = 0;
            decimal number = 0;
            long multiply = 1;
            Formato = "N0";
            _PosicaoVirgula = int.MinValue;
            for (int i = input.Length - 1; i >= 0; i--)
            {
                if (Char.IsDigit(input[i]))
                {
                    number += (input[i] - '0') * (multiply);
                    multiply *= 10;
                    posicao++;

                }
                else if (input[i].ToString() == System.Globalization.CultureInfo.CurrentUICulture.NumberFormat.NumberDecimalSeparator)
                {
                    if (DecimalPlaces > 0 && !temDecimal)
                    {
                        _PosicaoVirgula = i;
                        decimal indiceDivisao = Convert.ToDecimal(Math.Pow(10, posicao));
                        number = Math.Truncate((number / indiceDivisao) * Convert.ToDecimal(Math.Pow(10, DecimalPlaces))) / Convert.ToDecimal(Math.Pow(10, DecimalPlaces));
                        multiply = 1;
                        Formato = posicao >= DecimalPlaces ? String.Concat("N", DecimalPlaces) : String.Concat("N", posicao == 0 ? 1 : posicao);
                    }
                }
            }
            return number;
        }

        private int? _PosicaoVirgula = int.MinValue;


        public string Formato
        {
            get; set;
        }

        public int? PosicaoVirgula
        {
            get
            {
                return _PosicaoVirgula;
            }

            set
            {
                _PosicaoVirgula = value;
            }
        }
    }
}
