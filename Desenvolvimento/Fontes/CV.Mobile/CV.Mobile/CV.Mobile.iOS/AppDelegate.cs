using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CV.Mobile.Helper;
using FFImageLoading.Forms.Platform;
using Foundation;
using MediaManager;
using UIKit;

namespace CV.Mobile.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {

            global::Xamarin.Forms.Forms.SetFlags(new[] { "CollectionView_Experimental", "Shapes_Experimental", "SwipeView_Experimental", "CarouselView_Experimental", "RadioButton_Experimental" });
            CrossMediaManager.Current.Init();
            FFImageLoading.Forms.Platform.CachedImageRenderer.Init();
            global::Xamarin.Forms.Forms.Init();
            global::Xamarin.Auth.Presenters.XamarinIOS.AuthenticationConfiguration.Init();
                CachedImageRenderer.Init();

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException; ;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
            Sharpnado.Presentation.Forms.iOS.SharpnadoInitializer.Initialize();
            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }

        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            var okAlertController = UIAlertController.Create("Crash Report", e.Exception.Message, UIAlertControllerStyle.Alert);

            //Add Action
            okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));

            // Present Alert
            this.Window.RootViewController.PresentViewController(okAlertController, true, null);
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var okAlertController = UIAlertController.Create("Crash Report", "Erro", UIAlertControllerStyle.Alert);

            //Add Action
            okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));

            // Present Alert
            this.Window.RootViewController.PresentViewController(okAlertController, true, null);
        }

        public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
        {
            // Convert NSUrl to Uri
            var uri = new Uri(url.AbsoluteString);

            // Load redirectUrl page
            AuthenticationState.Authenticator.OnPageLoading(uri);

            return true;
        }
    }
}
