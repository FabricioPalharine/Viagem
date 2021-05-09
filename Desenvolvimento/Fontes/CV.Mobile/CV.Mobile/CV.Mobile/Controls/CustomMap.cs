using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace CV.Mobile.Controls
{
    public class CustomMap : Map
    {
        public static readonly BindableProperty CustomPinsProperty =
    BindableProperty.Create(nameof(CustomPins), typeof(IEnumerable<CustomPin>), typeof(CustomMap), new ObservableCollection<CustomPin>(), BindingMode.OneWay, null, OnValueChanged);

        public static readonly BindableProperty CustomPolylinesProperty = BindableProperty.Create("CustomPolyline", typeof(IEnumerable<Polyline>), typeof(CustomMap),
                                                                                           propertyChanged: (bindableObject, oldValue, newValue) =>
                                                                                           {
                                                                                               Map map = bindableObject as Map;
                                                                                               map.MapElements.Clear();
                                                                                             
                                                                                               if (oldValue != null)
                                                                                               {
                                                                                                   var observAble = oldValue as INotifyCollectionChanged;
                                                                                                   if (observAble != null)
                                                                                                   {
                                                                                                       //observAble.CollectionChanged -= OnPolylineCollectionChanged;

                                                                                                   }
                                                                                               }
                                                                                               if (newValue != null)
                                                                                               {
                                                                                                   foreach (var linha in ((IEnumerable<Polyline>)newValue))
                                                                                                       map.MapElements.Add((Polyline)linha);
                                                                                                   var observAble = newValue as INotifyCollectionChanged;
                                                                                                   if (observAble != null)
                                                                                                   {
                                                                                                       observAble.CollectionChanged += (sender,e)=>
                                                                                                       {
                                                                                                           VerificarLinhas(map, e);
                                                                                                       };
                                                                                                   }
                                                                                               }
                                                                                           });

        private static void VerificarLinhas(Map map, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (Polyline pin in e.NewItems)
                {
                    map.MapElements.Add(pin);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (Polyline pin in e.OldItems)
                {
                    map.MapElements.Remove(pin);
                }
            }
        }

        public IEnumerable<Polyline> CustomPolylines
        {
            get => (IEnumerable<Polyline>)GetValue(CustomPolylinesProperty);
            set => SetValue(CustomPolylinesProperty, value);
        }
        public IEnumerable<CustomPin> CustomPins
        {
            get { return (IEnumerable<CustomPin>)GetValue(CustomPinsProperty); }
            set
            {
                SetValue(CustomPinsProperty, value);

            }
        }

        private static void OnValueChanged(BindableObject bindable, object oldValue, object newValue)
        {
        }
    }
}
