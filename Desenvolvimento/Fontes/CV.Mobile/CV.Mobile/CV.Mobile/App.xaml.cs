using CV.Mobile.Services;
using CV.Mobile.Views;
using MediaManager;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CV.Mobile
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();
            CrossMediaManager.Current.Init();
            DependencyService.Register<MockDataStore>();
            MainPage = new LoginPage();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
