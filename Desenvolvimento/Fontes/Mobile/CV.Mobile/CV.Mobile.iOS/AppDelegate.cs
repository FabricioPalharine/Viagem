using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;
using Autofac;
using Autofac.Extras.CommonServiceLocator;
using Microsoft.Practices.ServiceLocation;
using CV.Mobile.iOS.Services;
using CV.Mobile.Interfaces;
using Xamarin;
using TK.CustomMap.iOSUnified;

namespace CV.Mobile.iOS
{

    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        IContainer _IoCContainer;

        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();
            FormsMaps.Init();
            TKCustomMapRenderer.InitMapRenderer();
            NativePlacesApi.Init();


            LoadApplication(new App());
            RegisterDependencies();
            return base.FinishedLaunching(app, options);
        }

        void RegisterDependencies()
        {
            var builder = new ContainerBuilder();

            builder.RegisterInstance(new ValidarAutenticacaoService()).As<IValidaAutenticacao>();

            //builder.RegisterInstance(new Geolocator()).As<IGeolocator>();

            //builder.RegisterInstance(new FechaAplicacaoService()).As<IFechaAplicacao>();

            _IoCContainer = builder.Build();

            var csl = new AutofacServiceLocator(_IoCContainer);
            ServiceLocator.SetLocatorProvider(() => csl);
        }
    }
}
