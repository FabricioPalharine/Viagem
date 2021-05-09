
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Android.Content;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Graphics;
using Android.Widget;
using CV.Mobile.Controls;
using CV.Mobile.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Maps.Android;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(CustomMap), typeof(CustomMapRenderer))]
namespace CV.Mobile.Droid.Renderers
{
    public class CustomMapRenderer : MapRenderer//, GoogleMap.IInfoWindowAdapter
    {
        IEnumerable<CustomPin> customPins;
        private GoogleMap _googleMap;
        private readonly Dictionary<CustomPin, Marker> _markers = new Dictionary<CustomPin, Marker>();
        private CustomMap FormsMap
        {
            get { return this.Element as CustomMap; }
        }
        public CustomMapRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(Xamarin.Forms.Platform.Android.ElementChangedEventArgs<Map> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                //NativeMap.InfoWindowClick -= OnInfoWindowClick;
            }

            if (e.NewElement != null)
            {
                var formsMap = (CustomMap)e.NewElement;
                customPins = formsMap.CustomPins;
                this.FormsMap.PropertyChanged += FormsMapPropertyChanged;
            }
        }

        protected override void OnMapReady(GoogleMap map)
        {
            this._googleMap = map;
            this.UpdatePins();
            base.OnMapReady(map);
        }


        private void FormsMapPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this._googleMap == null) return;
            if (e.PropertyName == "CustomPins")
            {
                this.UpdatePins();
            }
        }

        private void OnCustomPinsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this.FormsMap == null) return;
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (CustomPin pin in e.NewItems)
                {
                    if (!_markers.ContainsKey(pin))
                    {

                        this.AddPin(pin);
                    }
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (CustomPin pin in e.OldItems)
                {
                    if (!this.FormsMap.CustomPins.Where(d => d == pin).Any())
                    {
                        this.RemovePin(pin);
                    }
                }
            }
        }
        private void RemovePin(CustomPin pin, bool removeMarker = true)
        {
            if (this.FormsMap == null) return;
            var item = this._markers[pin];
            if (item == null) return;

            item.Remove();

            if (removeMarker)
            {
                this._markers.Remove(pin);
            }
        }

        private void UpdatePins(bool firstUpdate = true)
        {
            if (this.FormsMap == null) return;
            foreach (var i in this._markers)
            {
                this.RemovePin(i.Key, false);
            }
            this._markers.Clear();
            if (this.FormsMap != null && this.FormsMap.CustomPins != null)
            {
                foreach (var pin in this.FormsMap.CustomPins)
                {
                    this.AddPin(pin);
                }
                if (firstUpdate)
                {
                    var observAble = this.FormsMap.CustomPins as INotifyCollectionChanged;
                    if (observAble != null)
                    {
                        observAble.CollectionChanged += OnCustomPinsCollectionChanged;
                    }
                }
            }
        }

        private async void AddPin(CustomPin pin)
        {

            var markerWithIcon = new MarkerOptions();
            markerWithIcon.SetPosition(new LatLng(pin.Position.Latitude, pin.Position.Longitude));

            if (!string.IsNullOrWhiteSpace(pin.Label))
                markerWithIcon.SetTitle(pin.Label);
            await this.UpdateImage(pin, markerWithIcon);
            /*if (pin.ImageSource != null)
            {
                markerWithIcon.Anchor((float)pin.Anchor.X, (float)pin.Anchor.Y);
            }*/
            if (!_markers.ContainsKey(pin))
            {
                var marker = this._googleMap.AddMarker(markerWithIcon);

                this._markers.Add(pin, marker);
            }
        }

        private async Task UpdateImage(CustomPin pin, MarkerOptions markerOptions)
        {
            BitmapDescriptor bitmap;
            try
            {
                if (pin.ImageSource != null)
                {
                    try
                    {
                        var img = await pin.ImageSource.ToBitmap(this.Context);

                        bitmap = BitmapDescriptorFactory.FromBitmap(img);
                    }
                    catch (Exception ex)
                    {
                        bitmap = BitmapDescriptorFactory.DefaultMarker();
                    }

                }
                else
                {
                    /*if (pin.DefaultPinColor != Color.Default)
                    {
                        var hue = pin.DefaultPinColor.ToAndroid().GetHue();
                        bitmap = BitmapDescriptorFactory.DefaultMarker(Math.Min(hue, 359.99f));
                    }
                    else*/
                    {
                        bitmap = BitmapDescriptorFactory.DefaultMarker();
                    }
                }
            }
            catch (Exception)
            {
                bitmap = BitmapDescriptorFactory.DefaultMarker();
            }
            markerOptions.SetIcon(bitmap);
        }

        private async Task UpdateImage(CustomPin pin, Marker marker)
        {
            BitmapDescriptor bitmap;
            try
            {
                if (pin.ImageSource != null)
                {
             
                        var img = await pin.ImageSource.ToBitmap(this.Context);

                        bitmap = BitmapDescriptorFactory.FromBitmap(img);
                  
                }
                else
                {
                    /*if (pin.DefaultPinColor != Color.Default)
                    {
                        var hue = pin.DefaultPinColor.ToAndroid().GetHue();
                        bitmap = BitmapDescriptorFactory.DefaultMarker(hue);
                    }
                    else*/
                    {
                        bitmap = BitmapDescriptorFactory.DefaultMarker();
                    }
                }
            }
            catch (Exception)
            {
                bitmap = BitmapDescriptorFactory.DefaultMarker();
            }
            marker.SetIcon(bitmap);
        }
        /* protected override void OnMapReady(GoogleMap map)
         {
             base.OnMapReady(map);

             //NativeMap.InfoWindowClick += OnInfoWindowClick;
             //NativeMap.SetInfoWindowAdapter(this);
         }*/

        /* protected override MarkerOptions CreateMarker(Pin pin)
         {
             var marker = new MarkerOptions();
             marker.SetPosition(new LatLng(pin.Position.Latitude, pin.Position.Longitude));
             marker.SetTitle(pin.Label);
             marker.SetSnippet(pin.Address);
             marker.SetIcon(BitmapDescriptorFactory.FromResource(Resource.Drawable.pin));
             return marker;
         }*/


        void OnInfoWindowClick(object sender, GoogleMap.InfoWindowClickEventArgs e)
        {
            var customPin = GetCustomPin(e.Marker);
            if (customPin == null)
            {
                throw new Exception("Custom pin not found");
            }

            if (!string.IsNullOrWhiteSpace(customPin.Url))
            {
                var url = Android.Net.Uri.Parse(customPin.Url);
                var intent = new Intent(Intent.ActionView, url);
                intent.AddFlags(ActivityFlags.NewTask);
                Android.App.Application.Context.StartActivity(intent);
            }
        }

        /*public Android.Views.View GetInfoContents(Marker marker)
        {
            var inflater = Android.App.Application.Context.GetSystemService(Context.LayoutInflaterService) as Android.Views.LayoutInflater;
            if (inflater != null)
            {
                Android.Views.View view;

                var customPin = GetCustomPin(marker);
                if (customPin == null)
                {
                    throw new Exception("Custom pin not found");
                }

                if (customPin.Name.Equals("Xamarin"))
                {
                    view = inflater.Inflate(Resource.Layout.XamarinMapInfoWindow, null);
                }
                else
                {
                    view = inflater.Inflate(Resource.Layout.MapInfoWindow, null);
                }

                var infoTitle = view.FindViewById<TextView>(Resource.Id.InfoWindowTitle);
                var infoSubtitle = view.FindViewById<TextView>(Resource.Id.InfoWindowSubtitle);

                if (infoTitle != null)
                {
                    infoTitle.Text = marker.Title;
                }
                if (infoSubtitle != null)
                {
                    infoSubtitle.Text = marker.Snippet;
                }

                return view;
            }
            return null;
        }*/

        /*public Android.Views.View GetInfoWindow(Marker marker)
        {
            return null;
        }*/

        CustomPin GetCustomPin(Marker annotation)
        {
            var position = new Position(annotation.Position.Latitude, annotation.Position.Longitude);
            foreach (var pin in customPins)
            {
                if (pin.Position == position)
                {
                    return pin;
                }
            }
            return null;
        }
    }
    public static class Extensions
    {

        /// <summary>
        /// Convert a <see cref="ImageSource"/> to the native Android <see cref="Bitmap"/>
        /// </summary>
        /// <param name="source">Self instance</param>
        /// <param name="context">Android Context</param>
        /// <returns>The Bitmap</returns>
        public static async Task<Bitmap> ToBitmap(this ImageSource source, Context context)
        {
            if (source is FileImageSource)
            {
                return await new FileImageSourceHandler().LoadImageAsync(source, context);
            }
            if (source is UriImageSource)
            {
                return await new ImageLoaderSourceHandler().LoadImageAsync(source, context);
            }
            if (source is StreamImageSource)
            {

                var img = await new StreamImagesourceHandler().LoadImageAsync(source, context);
                return img;
            }        
            if (source is FontImageSource)
            {
                return await new FontImageSourceHandler().LoadImageAsync(source, context);
             }
            return null;
        }
    }
}
