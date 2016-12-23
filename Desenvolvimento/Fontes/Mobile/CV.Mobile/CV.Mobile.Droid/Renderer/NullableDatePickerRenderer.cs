using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using CV.Mobile.Controls;
using CV.Mobile.Droid.Renderer;
using System.ComponentModel;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(NullableDatePicker), typeof(NullableDatePickerRenderer))]

namespace CV.Mobile.Droid.Renderer
{
    internal class NullableDatePickerRenderer : DatePickerRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.DatePicker> e)
        {
            base.OnElementChanged(e);

            TryShowEmptyState();
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == NullableDatePicker.NullableDateProperty.PropertyName ||
                e.PropertyName == NullableDatePicker.EmptyStateTextProperty.PropertyName)
            {
                TryShowEmptyState();
            }
        }

        private void TryShowEmptyState()
        {
            var el = Element as NullableDatePicker;
            if (el != null)
            {
                if (el.NullableDate == null)
                {
                    Control.Text = el.EmptyStateText;
                }
                
            }
        }
    }
}