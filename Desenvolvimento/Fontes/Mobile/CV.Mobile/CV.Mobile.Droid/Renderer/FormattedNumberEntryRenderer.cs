using System;
using System.ComponentModel;
using Android.Text;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using CV.Mobile.Controls;
using CV.Mobile.Droid.Renderer;

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
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(FormattedNumberEntry.Value)))
            {
                var element = ((FormattedNumberEntry)Element);
                // 5. Format number, and place the formatted text in newText
                //var newText = element.Value.ToString(element.Formato??String.Concat( "N",element.DecimalPlaces));
                var newText = element.Value.ToString(!string.IsNullOrEmpty(element.Formato) && !element.ShouldReactToTextChanges ? element.Formato : String.Concat("N", element.DecimalPlaces));

                // 6. Set the Text property of our control to newText
                Control.Text = newText;

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

            // 3. Take the controlís text, lets name it oldText
            var oldText = Control.Text;

            // 4. Parse oldText into a number, lets name it number
            var number = element.DumbParse(oldText);
            element.Value = number;

            // 5. Format number, and place the formatted text in newText
            var newText = number.ToString(element.Formato);

            // 6. Set the Text property of our control to newText
            Control.Text = newText;

            // 7. Calculate the new cursor position
            var change = oldText.Length - newText.Length;
            // 8. Set the new cursor position

            if (cursorPosition <= element.PosicaoVirgula || element.PosicaoVirgula < 0)
            {
                if (cursorPosition - change <= newText.Length && cursorPosition - change > 0)
                    Control.SetSelection(cursorPosition - change);

            }
            else if (cursorPosition < newText.Length)
                Control.SetSelection(cursorPosition);



            // 9. Start listening for changes on our controlís Text property
            element.ShouldReactToTextChanges = true;
        }
    }
}