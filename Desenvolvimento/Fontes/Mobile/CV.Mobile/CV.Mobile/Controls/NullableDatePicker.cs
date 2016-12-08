using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CV.Mobile.Controls
{
    public class NullableDatePicker : DatePicker
    {
        public static readonly BindableProperty NullableDateProperty = BindableProperty.Create(
            "NullableDate", typeof(DateTime?), typeof(NullableDatePicker), null, BindingMode.TwoWay);

        public static readonly BindableProperty EmptyStateTextProperty = BindableProperty.Create(
            "EmptyStateText", typeof(string), typeof(NullableDatePicker), string.Empty, BindingMode.OneWay);

        public DateTime? NullableDate
        {
            get { return (DateTime?)GetValue(NullableDateProperty); }
            set
            {
                if (value != NullableDate)
                {
                    SetValue(NullableDateProperty, value);
                    UpdateDate();
                }
            }
        }

        public string EmptyStateText
        {
            get { return (string)GetValue(EmptyStateTextProperty); }
            set { SetValue(EmptyStateTextProperty, value); }
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            UpdateDate();
        }

        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            Device.OnPlatform(() =>
            {
                if (propertyName == IsFocusedProperty.PropertyName)
                {
                    if (IsFocused)
                    {
                        if (!NullableDate.HasValue)
                        {
                            Date = (DateTime)DateProperty.DefaultValue;
                        }
                    }
                    else
                    {
                        OnPropertyChanged(DateProperty.PropertyName);
                    }
                }
            });

            if (propertyName == DateProperty.PropertyName)
            {
                NullableDate = Date;
            }

            if (propertyName == NullableDateProperty.PropertyName)
            {
                if (NullableDate.HasValue)
                {
                    Date = NullableDate.Value;
                }
            }
        }

        private void UpdateDate()
        {
            if (NullableDate.HasValue)
            {
                Date = NullableDate.Value;
            }
            else
            {
                Date = (DateTime)DateProperty.DefaultValue;
            }
        }
    }
}
