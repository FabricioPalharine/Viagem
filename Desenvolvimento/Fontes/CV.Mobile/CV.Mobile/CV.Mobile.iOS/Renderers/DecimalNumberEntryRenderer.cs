using CoreGraphics;
using Foundation;
using System;
using System.ComponentModel;
using System.Drawing;
using UIKit;
using CV.Mobile.Controls;
using CV.Mobile.iOS.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(DecimalNumberEntry), typeof(DecimalNumberEntryRenderer))]

namespace CV.Mobile.iOS.Renderers
{
    public class DecimalNumberEntryRenderer : EntryRenderer
    {

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (this.Control == null) return;
            var element = ((DecimalNumberEntry)Element);
            if (e.OldElement != null)
            {
                Control.EditingChanged -= Control_EditingChanged;
                element.ValueChanged -= Element_ValueChanged;
            }
            if (e.NewElement != null)
            {
                Control.EditingChanged += Control_EditingChanged;
                element.ValueChanged += Element_ValueChanged;
            }

            

            //this.Control.BorderStyle = UITextBorderStyle.None;
        }

        private void Element_ValueChanged(object sender, TextChangedEventArgs e)
        {
            var element = ((DecimalNumberEntry)Element);
            string strValor = element.RetornarTextoPuro(Convert.ToDecimal(element.Value).ToString(String.Concat("N", element.DecimalPlaces), element.Idioma));
            long valor = long.Parse(strValor);
            decimal valorDecimal = Convert.ToDecimal(valor) / Convert.ToDecimal(Math.Pow(10, element.DecimalPlaces));
            var newText = valorDecimal.ToString(String.Concat("N", element.DecimalPlaces), element.Idioma);
            if (newText != Control.Text)
                Control.Text = newText;
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            
          

        }

        void Control_EditingChanged(object sender, EventArgs e)
        {
            var element = ((DecimalNumberEntry)Element);
            // Oh boy, thank you internet: http://stackoverflow.com/a/34922332

            var selectedRange = Control.SelectedTextRange;
            var posicao = Control.GetOffsetFromPosition(Control.BeginningOfDocument, selectedRange.Start);
            var oldText = Control.Text;

            var TextoPuro = element.RetornarTextoPuro(oldText);
            decimal number = 0;
            if (!string.IsNullOrEmpty(TextoPuro))
                number = Convert.ToDecimal(TextoPuro) / Convert.ToDecimal(Math.Pow(10, element.DecimalPlaces));
            element.Value = number;

            var newText = number.ToString(string.Concat("N", element.DecimalPlaces), element.Idioma);

            if (newText != oldText)
                Control.Text = newText;

            var change = -1 * (oldText.Length - newText.Length);            
            var newPosition = Control.GetPosition(selectedRange.Start, (nint)change);

            if (newPosition != null) 
            {
                Control.SelectedTextRange = Control.GetTextRange(newPosition, newPosition);
            }

        }
    }
}