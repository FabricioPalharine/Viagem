using System;
using Xamarin.Forms;

namespace CV.Mobile.Controls
{
    public class CustomMenuItem : MenuItem
    {
        public static readonly BindableProperty IsVisibleProperty =
    BindableProperty.Create(nameof(IsVisible), typeof(bool), typeof(CustomMenuItem), true);

        public bool IsVisible
        {
            get { return (bool)GetValue(IsVisibleProperty); }
            set
            {
                SetValue(IsVisibleProperty, value);

            }
        }
    }
}