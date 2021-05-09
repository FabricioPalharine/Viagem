using System;
using CV.Mobile.iOS.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ContentPage), typeof(CustomPageRenderer))]
namespace CV.Mobile.iOS.Renderers
{
    public class CustomPageRenderer: PageRenderer
    {
        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);
            OverrideUserInterfaceStyle = UIKit.UIUserInterfaceStyle.Light;
        }
    }
}
