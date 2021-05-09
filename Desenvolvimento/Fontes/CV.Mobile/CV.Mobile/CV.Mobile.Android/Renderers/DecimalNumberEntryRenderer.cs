using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CV.Mobile.Controls;
using CV.Mobile.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(DecimalNumberEntry), typeof(DecimalNumberEntryRenderer))]

namespace CV.Mobile.Droid.Renderers
{
    public class DecimalNumberEntryRenderer:EntryRenderer
    {
        public DecimalNumberEntryRenderer(Context context):base(context)
        {

        }

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                Control.AfterTextChanged -= Control_AfterTextChanged;
            }
            if (e.NewElement != null)
            {
                Control.AfterTextChanged += Control_AfterTextChanged;
            }
            if (e.NewElement.Keyboard == Keyboard.Numeric)
            {
                var native = Control as EditText;
                native.KeyListener = Android.Text.Method.DigitsKeyListener.GetInstance("123456789");
                native.InputType = Android.Text.InputTypes.ClassNumber ;
            }
            var element = ((DecimalNumberEntry)Element);
            //element.Formato = String.Concat("N", element.DecimalPlaces);
        }

        void Control_AfterTextChanged(object sender, AfterTextChangedEventArgs e)
        {

            var element = ((DecimalNumberEntry)Element);


            var cursorPosition = Control.SelectionStart;
            var ultimaposicao = Control.SelectionStart == Control.Text.Length;
            var oldText = Control.Text;
            var TextoPuro = element.RetornarTextoPuro(oldText);
            decimal number = 0;
            if (!string.IsNullOrEmpty(TextoPuro))
                number = Convert.ToDecimal(TextoPuro) / Convert.ToDecimal(Math.Pow(10, element.DecimalPlaces));
            element.Value = number;

            // 5. Format number, and place the formatted text in newText
            var newText = number.ToString(string.Concat("N", element.DecimalPlaces), element.Idioma);

            if (newText != oldText)
                Control.Text = newText;

            var change = newText.Length - oldText.Length;

            cursorPosition += change;
            if (cursorPosition < newText.Length && !ultimaposicao) {
                if (cursorPosition < 0) {
                    cursorPosition = 1;
                }
                Control.SetSelection(cursorPosition);
            }             
            else
                Control.SetSelection(newText.Length);
        }
    }
}