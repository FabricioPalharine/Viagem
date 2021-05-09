using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using UIKit;
using CV.Mobile.Behaviors;
using CV.Mobile.iOS.Effects;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ResolutionGroupName("CV.Mobile")]
[assembly: ExportEffect(typeof(EntryLineColorEffect), "EntryLineColorEffect")]

namespace CV.Mobile.iOS.Effects
{
    public class EntryLineColorEffect : PlatformEffect
    {
        UIControl control;


        protected override void OnAttached()
        {
            try
            {
                control = Control as UIControl;
                UpdateLineColor();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Cannot set property on attached control. Error: ", ex.Message);
            }
        }

        protected override void OnDetached()
        {
            control = null;
        }

        protected override void OnElementPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnElementPropertyChanged(args);

            if (args.PropertyName == LineColorBehavior.LineColorProperty.PropertyName ||
                args.PropertyName == "Height")
            {
                Initialize();
                UpdateLineColor();
            }
        }

        private void Initialize()
        {
            var entry = Element as Entry;
            if (entry != null)
            {
                Control.Bounds = new CGRect(0, 0, entry.Width, entry.Height);
            }
        }

        private void UpdateLineColor()
        {
            UITextField textField = control as UITextField;
            BorderLineLayer lineLayer = control.Layer.Sublayers.OfType<BorderLineLayer>()
                                                             .FirstOrDefault();

            
                if (lineLayer == null)
                {
                    lineLayer = new BorderLineLayer();
                    lineLayer.MasksToBounds = true;
                    lineLayer.BorderWidth = 1.0f;
                    control.Layer.AddSublayer(lineLayer);
                    if (textField != null)
                        textField.BorderStyle = UITextBorderStyle.None;
                }

                lineLayer.Frame = new CGRect(0f, Control.Frame.Height - 1f, Control.Bounds.Width, 1f);
                lineLayer.BorderColor = LineColorBehavior.GetLineColor(Element).ToCGColor();
                
                if (textField != null)
                    control.TintColor = textField.TextColor;

            
        }

        private class BorderLineLayer : CALayer
        {
        }
    }
}
