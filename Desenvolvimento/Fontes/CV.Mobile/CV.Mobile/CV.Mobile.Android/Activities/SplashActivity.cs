using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;

namespace CV.Mobile.Droid.Activities
{
    [Activity(Label = "Curtindo uma Viagem", Icon = "@mipmap/ic_launcher", Theme = "@style/Theme.Splash", NoHistory = true,
         MainLauncher = true,
         ScreenOrientation = ScreenOrientation.Portrait)]
    public class SplashActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            var Codigo = Resource.Layout.splashlayout;
            SetContentView(Codigo);

            InvokeMainActivity();
            // Create your application here
        }

        private async void InvokeMainActivity()
        {
            await Task.Delay(2000);
            StartActivity(new Intent(this, typeof(MainActivity)));
        }
    }
}