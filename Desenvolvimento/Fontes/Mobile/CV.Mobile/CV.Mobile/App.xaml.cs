﻿using CV.Mobile.Models;
using CV.Mobile.ViewModels;
using CV.Mobile.Views;
using FormsToolkit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TK.CustomMap.Api.Google;
using Xamarin.Forms;

namespace CV.Mobile
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            SubscribeToDisplayAlertMessages();
            GmsPlace.Init("AIzaSyAlUpOpwZWS_ZGlMAtB6lY76oy1QBWk97g");
            GmsDirection.Init("AIzaSyAlUpOpwZWS_ZGlMAtB6lY76oy1QBWk97g");

            LoadingViewModel vm = new LoadingViewModel();
            MainPage = new LoadingPage() { BindingContext = vm };
        }

        static void SubscribeToDisplayAlertMessages()
        {
            MessagingService.Current.Subscribe<MessagingServiceAlert>(MessageKeys.DisplayAlert, async (service, info) => {
                var task = Current?.MainPage?.DisplayAlert(info.Title, info.Message, info.Cancel);
                if (task != null)
                {
                    await task;
                    info?.OnCompleted?.Invoke();
                }
            });

            MessagingService.Current.Subscribe<MessagingServiceQuestion>(MessageKeys.DisplayQuestion, async (service, info) => {
                var task = Current?.MainPage?.DisplayAlert(info.Title, info.Question, info.Positive, info.Negative);
                if (task != null)
                {
                    var result = await task;
                    info?.OnCompleted?.Invoke(result);
                }
            });
        }

        UsuarioLogado ItemUsuario { get; set; }

        public void RedirectToMenu(UsuarioLogado itemUsuario)
        {
            ItemUsuario = itemUsuario;
            MasterDetailViewModel vm = new MasterDetailViewModel(itemUsuario);
            var Pagina = new Views.MasterDetailPage(vm);

            MainPage = Pagina;
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
