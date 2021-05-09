
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using CoreGraphics;
using CoreLocation;
using CV.Mobile.Controls;
using CV.Mobile.iOS.Renderers;
using Foundation;
using MapKit;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Maps.iOS;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(CustomMap), typeof(CustomMapRenderer))]
namespace CV.Mobile.iOS.Renderers
{
    public class CustomMapRenderer : MapRenderer
    {
        private bool _isLoaded;

        //UIView customPinView;
        IEnumerable<CustomPin> customPins;
        private MKMapView nativeMap;
        private CustomMap formsMap;
        protected override void OnElementChanged(ElementChangedEventArgs<View> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null )
            {
                e.OldElement.PropertyChanged -= OnMapPropertyChanged;
                this.nativeMap.MapLoaded -= MapLoaded;

                nativeMap = Control as MKMapView;
            }

            if (e.NewElement != null)
            {
                formsMap = (CustomMap)e.NewElement;
                nativeMap = Control as MKMapView;
                customPins = formsMap.CustomPins;
                this.nativeMap.MapLoaded += MapLoaded;

            }
        }

        private void OnMapPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CustomPins")
            {
                this.UpdatePins();
            }

        }
        private void MapLoaded(object sender, EventArgs e)
        {
            if (this._isLoaded) return;

            this.UpdatePins();
            this.formsMap.PropertyChanged += OnMapPropertyChanged;

            this._isLoaded = true;
        }

        private void UpdatePins(bool firstUpdate = true)
        {
            this.nativeMap.RemoveAnnotations(this.nativeMap.Annotations);

            if (this.formsMap.CustomPins == null) return;

            foreach (var i in formsMap.CustomPins)
            {
                i.PropertyChanged -= OnPinPropertyChanged;
                this.AddPin(i);
            }

            if (firstUpdate)
            {
                var observAble = this.formsMap.CustomPins as INotifyCollectionChanged;
                if (observAble != null)
                {
                    observAble.CollectionChanged += OnCollectionChanged;
                }
            }
        }
        private void AddPin(CustomPin pin)
        {
            var annotation = new TKCustomMapAnnotation(pin);
            this.nativeMap.AddAnnotation(annotation);

            pin.PropertyChanged += OnPinPropertyChanged;
        }
        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (CustomPin pin in e.NewItems)
                {
                    this.AddPin(pin);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (CustomPin pin in e.OldItems)
                {
                    if (!this.formsMap.CustomPins.Contains(pin))
                    {
                     

                        var annotation = this.nativeMap.Annotations
                            .OfType<TKCustomMapAnnotation>()
                            .SingleOrDefault(i => i.CustomPin.Equals(pin));

                        if (annotation != null)
                        {
                            annotation.CustomPin.PropertyChanged -= OnPinPropertyChanged;

                            this.nativeMap.RemoveAnnotation(annotation);
                        }
                    }
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                foreach (var annotation in this.nativeMap.Annotations.OfType<TKCustomMapAnnotation>())
                {
                    annotation.CustomPin.PropertyChanged -= OnPinPropertyChanged;
                }
                this.UpdatePins(false);
            }
        }

        private void OnPinPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
        

            var formsPin = (CustomPin)sender;
            var annotation = this.nativeMap.Annotations
                .OfType<TKCustomMapAnnotation>()
                .SingleOrDefault(i => i.CustomPin.Equals(formsPin));

            if (annotation == null) return;

            var annotationView = this.nativeMap.ViewForAnnotation(annotation);
            if (annotationView == null) return;

            switch (e.PropertyName)
            {
                case "ImageSource":
                    this.UpdateImage(annotationView, formsPin);
                    break;
                
               /* case TKCustomMapPin.AnchorPropertyName:
                    if (formsPin.Image != null)
                    {
                        annotationView.Layer.AnchorPoint = new CGPoint(formsPin.Anchor.X, formsPin.Anchor.Y);
                    }
                    break;*/
            }
        }

        private async void UpdateImage(MKAnnotationView annotationView, CustomPin pin)
        {
            if (pin.ImageSource != null)
            {
                // If this is the case, we need to get a whole new annotation view. 
                if (annotationView.GetType() == typeof(MKPinAnnotationView))
                {
                    this.nativeMap.RemoveAnnotation(annotationView.Annotation);
                    this.nativeMap.AddAnnotation(new TKCustomMapAnnotation(pin));
                    return;
                }
                UIImage image = await pin.ImageSource.ToImage();
                Device.BeginInvokeOnMainThread(() =>
                {
                    annotationView.Image = image;
                });
            }
            else
            {
                var pinAnnotationView = annotationView as MKPinAnnotationView;
                if (pinAnnotationView != null)
                {
                    pinAnnotationView.AnimatesDrop = true;

                    var pinTintColorAvailable = pinAnnotationView.RespondsToSelector(new ObjCRuntime.Selector("pinTintColor"));

                    if (!pinTintColorAvailable)
                    {
                        return;
                    }

                    /*if (pin.DefaultPinColor != Color.Default)
                    {
                        pinAnnotationView.PinTintColor = pin.DefaultPinColor.ToUIColor();
                    }
                    else*/
                    {
                        pinAnnotationView.PinTintColor = UIColor.Red;
                    }
                }
            }
        }
    }

    [Preserve(AllMembers = true)]
    internal class TKCustomMapAnnotation : MKAnnotation
    {
        private readonly CustomPin _formsPin;

        ///<inheritdoc/>
        
        ///<inheritdoc/>
        public override CLLocationCoordinate2D Coordinate
        {
            get { return this._formsPin.Position.ToLocationCoordinate(); }
        }
        /// <summary>
        /// Gets the forms pin
        /// </summary>
        public CustomPin CustomPin
        {
            get { return this._formsPin; }
        }
        ///<inheritdoc/>
        public override void SetCoordinate(CLLocationCoordinate2D value)
        {
            this._formsPin.Position = value.ToPosition();
        }
        /// <summary>
        /// Xamarin.iOS does (still) not export <value>_original_setCoordinate</value>
        /// </summary>
        /// <param name="value">The coordinate</param>
        [Export("_original_setCoordinate:")]
        public void SetCoordinateOriginal(CLLocationCoordinate2D value)
        {
            this.SetCoordinate(value);
        }
        /// <summary>
        /// Creates a new instance of <see cref="TKCustomMapAnnotation"/>
        /// </summary>
        /// <param name="pin">The forms pin</param>
        public TKCustomMapAnnotation(CustomPin pin)
        {
            this._formsPin = pin;
        }
    }

    public static class Extensions
    {
        /// <summary>
        /// Convert <see cref="Position" /> to <see cref="CLLocationCoordinate2D"/>
        /// </summary>
        /// <param name="self">Self instance</param>
        /// <returns>iOS coordinate</returns>
        public static CLLocationCoordinate2D ToLocationCoordinate(this Position self)
        {
            return new CLLocationCoordinate2D(self.Latitude, self.Longitude);
        }
        /// <summary>
        /// Convert <see cref="CLLocationCoordinate2D" /> to <see cref="Position"/>
        /// </summary>
        /// <param name="self">Self instance</param>
        /// <returns>Forms position</returns>
        public static Position ToPosition(this CLLocationCoordinate2D self)
        {
            return new Position(self.Latitude, self.Longitude);
        }
     
        /// <summary>
        /// Converts an <see cref="ImageSource"/> to the native iOS <see cref="UIImage"/>
        /// </summary>
        /// <param name="source">Self intance</param>
        /// <returns>The UIImage</returns>
        public static async Task<UIImage> ToImage(this ImageSource source)
        {
            if (source is FileImageSource)
            {
                return await new FileImageSourceHandler().LoadImageAsync(source);
            }
            if (source is UriImageSource)
            {
                return await new ImageLoaderSourceHandler().LoadImageAsync(source);
            }
            if (source is StreamImageSource)
            {
                return await new StreamImagesourceHandler().LoadImageAsync(source);
            }
            if (source is FontImageSource)
            {
                return await new FontImageSourceHandler().LoadImageAsync(source);
            }
            return null;
        }
    }
}