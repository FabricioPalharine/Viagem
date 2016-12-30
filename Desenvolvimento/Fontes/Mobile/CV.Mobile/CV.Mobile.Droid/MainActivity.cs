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

namespace CV.Mobile.Droid
{
    [Activity(Label = "Curtindo uma Viagem", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        IContainer _IoCContainer;

        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;
            RegisterDependencies();
            SecureStorageImplementation.StoragePassword = "mudar.123";
            base.OnCreate(bundle);
            AndroidEnvironment.UnhandledExceptionRaiser += AndroidEnvironment_UnhandledExceptionRaiser; ;

            global::Xamarin.Forms.Forms.Init(this, bundle);
            Xamarin.FormsMaps.Init(this, bundle);
            UserDialogs.Init(this);
            LoadApplication(new App());
        }

        private void AndroidEnvironment_UnhandledExceptionRaiser(object sender, RaiseThrowableEventArgs e)
        {
            
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);

        }

        void RegisterDependencies()
        {
            var builder = new ContainerBuilder();

            builder.RegisterInstance(new ValidarAutenticacaoService()).As<IValidaAutenticacao>();

            //builder.RegisterInstance(new Geolocator()).As<IGeolocator>();

            //builder.RegisterInstance(new FechaAplicacaoService()).As<IFechaAplicacao>();


            //builder.RegisterInstance(new HttpClientHandlerFactory()).As<IHttpClientHandlerFactory>();

            // builder.RegisterInstance(new DatastoreFolderPathProvider()).As<IDatastoreFolderPathProvider>();

            _IoCContainer = builder.Build();

            var csl = new AutofacServiceLocator(_IoCContainer);
            ServiceLocator.SetLocatorProvider(() => csl);
        }
    }
}

