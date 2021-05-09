using System;
using CV.Mobile.iOS.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(Shell), typeof(CustomShellPageRenderer))]
namespace CV.Mobile.iOS.Renderers
{
    public class CustomShellPageRenderer : ShellRenderer
    {
        protected override void OnCurrentItemChanged()
        {
            base.OnCurrentItemChanged();
            OverrideUserInterfaceStyle = UIKit.UIUserInterfaceStyle.Light;
        }


      
    }
}
