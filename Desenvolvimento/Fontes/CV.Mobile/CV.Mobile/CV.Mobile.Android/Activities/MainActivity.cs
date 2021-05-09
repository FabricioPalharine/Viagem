using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Xamarin.Essentials;
using Android.Content;
using FFImageLoading;
using Acr.UserDialogs;
using FFImageLoading.Forms.Platform;
using Plugin.CurrentActivity;
using Android.Util;
using Xamarin.Forms;
using MediaManager;

namespace CV.Mobile.Droid.Activities
{
    [Activity(Label = "Curitndo uma Viagem", Icon = "@mipmap/ic_launcher", Theme = "@style/MainTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {



        protected override void OnCreate(Bundle savedInstanceState)
        {
            //TabLayoutResource = Android.Resource.Layout.Tabbar;
            //ToolbarResource = Android.Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            CrossCurrentActivity.Current.Init(this, savedInstanceState);
            CrossMediaManager.Current.Init();
            // SecureStorageImplementation.StoragePassword = "CV.123";
            AndroidEnvironment.UnhandledExceptionRaiser += AndroidEnvironment_UnhandledExceptionRaiser;
            System.Net.ServicePointManager.ServerCertificateValidationCallback += (o, certificate, chain, errors) =>
            {
                return true;
            };
            System.Net.ServicePointManager.DnsRefreshTimeout = 0;


            // SupportActionBar.SetDisplayShowHomeEnabled(true); // Show or hide the default home button
            //SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            // SupportActionBar.SetDisplayShowCustomEnabled(true); // Enable overriding the default toolbar layout
            //SupportActionBar.SetDisplayShowTitleEnabled(false);
            FFImageLoading.Forms.Platform.CachedImageRenderer.Init(false);
            global::Xamarin.Forms.Forms.SetFlags(new[] { "CollectionView_Experimental", "Shapes_Experimental", "SwipeView_Experimental", "CarouselView_Experimental", "RadioButton_Experimental" });
            global::Xamarin.Auth.Presenters.XamarinAndroid.AuthenticationConfiguration.Init(this,savedInstanceState);
            Xamarin.Auth.CustomTabsConfiguration.CustomTabsClosingMessage = null;
            UserDialogs.Init(this);
            CachedImageRenderer.Init(false);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            
            LoadApplication(new CV.Mobile.App());
            
        }

       

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public override void OnTrimMemory([GeneratedEnum] TrimMemory level)
        {
            ImageService.Instance.InvalidateMemoryCache();
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
            base.OnTrimMemory(level);
        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
        }
        

      

        private void AndroidEnvironment_UnhandledExceptionRaiser(object sender, RaiseThrowableEventArgs e)
        {

        }
    }
}