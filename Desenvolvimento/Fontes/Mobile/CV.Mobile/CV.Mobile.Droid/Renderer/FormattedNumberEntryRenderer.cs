using System;
using System.ComponentModel;
using Android.Text;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using CV.Mobile.Controls;
using CV.Mobile.Droid.Renderer;
using Android.Widget;

[assembly: ExportRenderer(typeof(FormattedNumberEntry), typeof(FormattedNumberEntryRenderer))]

namespace CV.Mobile.Droid.Renderer
{
    public class FormattedNumberEntryRenderer : EntryRenderer
    {
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
                native.InputType = Android.Text.InputTypes.ClassNumber;
            }
            var element = ((FormattedNumberEntry)Element);
            element.Formato = String.Concat("N", element.DecimalPlaces);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(FormattedNumberEntry.Value)))
            {
                var element = ((FormattedNumberEntry)Element);
                // 5. Format number, and place the formatted text in newText
                //var newText = element.Value.ToString(element.Formato??String.Concat( "N",element.DecimalPlaces));
                if (element.Value.HasValue)
                {
                    var newText = element.Value.Value.ToString(String.Concat("N", element.DecimalPlaces));

                    // 6. Set the Text property of our control to newText
                    Control.Text = newText;
                }
                else
                    Control.Text = string.Empty;

            }
            else if (!e.PropertyName.Equals(nameof(FormattedNumberEntry.PosicaoVirgula)))
            {
                base.OnElementPropertyChanged(sender, e);
            }

        }

        void Control_AfterTextChanged(object sender, AfterTextChangedEventArgs e)
        {

            var element = ((FormattedNumberEntry)Element);

            // 1. Stop listening for changes on our control Text property
            if (!element.ShouldReactToTextChanges) return;
            element.ShouldReactToTextChanges = false;

            // 2. Get the current cursor position
            var cursorPosition = Control.SelectionStart;
            var ultimaposicao = Control.SelectionStart == Control.Text.Length;
            // 3. Take the control’s text, lets name it oldText
            var oldText = Control.Text;
            var TextoPuro = oldText.Replace(".", string.Empty).Replace(",", string.Empty);
            // 4. Parse oldText into a number, lets name it number
            decimal? number = null;
            if (!string.IsNullOrEmpty(TextoPuro))
                number = Convert.ToDecimal(TextoPuro) / Convert.ToDecimal(Math.Pow(10, element.DecimalPlaces));
            element.Value = number;

            // 5. Format number, and place the formatted text in newText
            var newText = number.HasValue ? number.Value.ToString(element.Formato, element.Idioma) : string.Empty;

            // 6. Set the Text property of our control to newText
            Control.Text = newText;

            // 7. Calculate the new cursor position
            var change = newText.Length - oldText.Length;
            // 8. Set the new cursor position

            //if (cursorPosition <= element.PosicaoVirgula || element.PosicaoVirgula < 0)
            //{
            //    if (cursorPosition - change <= newText.Length && cursorPosition - change > 0)
            //        Control.SetSelection(cursorPosition - change);

            //}

            if (change > 0)
                cursorPosition += change;
            if (cursorPosition < newText.Length && !ultimaposicao)
                Control.SetSelection(cursorPosition);
            else
                Control.SetSelection(newText.Length);



            // 9. Start listening for changes on our control’s Text property
            element.ShouldReactToTextChanges = true;
        }
    }
}
