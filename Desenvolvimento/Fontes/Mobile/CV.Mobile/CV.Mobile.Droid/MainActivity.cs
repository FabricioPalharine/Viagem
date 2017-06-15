using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Autofac;
using Autofac.Extras.CommonServiceLocator;
using Microsoft.Practices.ServiceLocation;
using CV.Mobile.Droid.Services;
using CV.Mobile.Interfaces;
using Plugin.SecureStorage;
using Plugin.Permissions;
using Acr.UserDialogs;
using System.Reflection;
using FFImageLoading.Forms.Droid;
using Android.Content;
using Android.Gms.Auth.Api.SignIn;
using Android.Gms.Auth.Api;

namespace CV.Mobile.Droid
{
    public class ActivityResultEventArgs : EventArgs
    {
        public int RequestCode { get; set; }
        public Result ResultCode { get; set; }
        public Intent Data { get; set; }

        public ActivityResultEventArgs() : base()
        { }
    }

    [Activity(Label = "Curtindo uma Viagem", Icon = "@drawable/icon", Theme = "@style/MainTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        IContainer _IoCContainer;
        public event EventHandler<ActivityResultEventArgs> ActivityResult = delegate { };

        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;
            RegisterDependencies();
            SecureStorageImplementation.StoragePassword = "mudar.123";
            base.OnCreate(bundle);
            AndroidEnvironment.UnhandledExceptionRaiser += AndroidEnvironment_UnhandledExceptionRaiser; ;
            CachedImageRenderer.Init();
            Behaviors.EventHandlerBehavior evt = new Behaviors.EventHandlerBehavior();
            Behaviors.InvokeCommandAction cmd = new Behaviors.InvokeCommandAction();
            FormsToolkit.Droid.Toolkit.Init();
            global::Xamarin.Forms.Forms.Init(this, bundle);
            

            Xamarin.FormsMaps.Init(this, bundle);
            var cv = typeof(Xamarin.Forms.CarouselView);
            var assembly = Assembly.Load(cv.FullName);
            UserDialogs.Init(this);
            LoadApplication(new App());
        }

        private void AndroidEnvironment_UnhandledExceptionRaiser(object sender, RaiseThrowableEventArgs e)
        {
            Android.Util.Log.Error("CV", e.Exception.Message + ";" + e.Exception.StackTrace);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);

        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            ActivityResult(this, new ActivityResultEventArgs
            {
                RequestCode = requestCode,
                ResultCode = resultCode,
                Data = data
            });
            
        
            //Log.Debug(TAG, "onActivityResult:" + requestCode + ":" + resultCode + ":" + data);

            //if (requestCode == RC_SIGN_IN)
            //{
            //    if (resultCode != Result.Ok)
            //    {
            //        mShouldResolve = false;
            //    }

            //    mIsResolving = false;
            //    mGoogleApiClient.Connect();
            //}
        }

        void RegisterDependencies()
        {
            var builder = new ContainerBuilder();

            builder.RegisterInstance(new ValidarAutenticacaoService()).As<IValidaAutenticacao>();

            builder.RegisterInstance(new FileHelper()).As<IFileHelper>();

            //builder.RegisterInstance(new FechaAplicacaoService()).As<IFechaAplicacao>();


            //builder.RegisterInstance(new HttpClientHandlerFactory()).As<IHttpClientHandlerFactory>();

            // builder.RegisterInstance(new DatastoreFolderPathProvider()).As<IDatastoreFolderPathProvider>();

            _IoCContainer = builder.Build();

            var csl = new AutofacServiceLocator(_IoCContainer);
            ServiceLocator.SetLocatorProvider(() => csl);
        }
    }
}

