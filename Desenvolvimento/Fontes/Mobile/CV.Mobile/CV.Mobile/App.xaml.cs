using CV.Mobile.Models;
using CV.Mobile.Services;
using CV.Mobile.ViewModels;
using CV.Mobile.Views;
using FormsToolkit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            GmsPlace.Init(Constants.ClientAPI);
            GmsDirection.Init(Constants.ClientAPI);

            LoadingViewModel vm = new LoadingViewModel();
            MainPage = new LoadingPage() { BindingContext = vm };
        }

        static void SubscribeToDisplayAlertMessages()
        {
            MessagingService.Current.Subscribe<MessagingServiceAlert>(MessageKeys.DisplayAlert, async (service, info) =>
            {
                var task = Current?.MainPage?.DisplayAlert(info.Title, info.Message, info.Cancel);
                if (task != null)
                {
                    await task;
                    info?.OnCompleted?.Invoke();
                }
            });

            MessagingService.Current.Subscribe<MessagingServiceQuestion>(MessageKeys.DisplayQuestion, async (service, info) =>
            {
                var task = Current?.MainPage?.DisplayAlert(info.Title, info.Question, info.Positive, info.Negative);
                if (task != null)
                {
                    var result = await task;
                    info?.OnCompleted?.Invoke(result);
                }
            });
        }

        UsuarioLogado ItemUsuario { get; set; }

        public async Task GravarUsuario(int Codigo)
        {
            var itemBase = await DatabaseService.Database.CarregarUsuario(Codigo);
            if (itemBase == null)
            {
                using (ApiService srv = new ApiService())
                {
                    if (await srv.VerificarOnLine())
                    {
                        var itemUsuario = await srv.CarregarUsuario(Codigo);

                        await DatabaseService.Database.SalvarUsuarioAsync(itemUsuario);
                    }

                }
            }
        }

        public async void RedirectToMenu(UsuarioLogado itemUsuario)
        {
            ItemUsuario = itemUsuario;


            MasterDetailViewModel vm = new MasterDetailViewModel(itemUsuario);
            Viagem itemViagem = await DatabaseService.Database.GetViagemAtualAsync();
            var Pagina = new Views.MasterDetailPage(vm);

            MainPage = Pagina;
            await vm.VerificarConexaoSignalR(itemViagem);
            if (itemViagem != null)
            {
                vm.ItemViagem = itemViagem;
                vm.PreencherPaginasViagem(itemViagem);
                using (ApiService srv = new ApiService())
                {
                    if (await srv.VerificarOnLine())
                    {
                        await srv.SelecionarViagem(itemViagem.Identificador);
                    }
                }
                vm.VerificarEnvioFotos();
                vm.VerificarEnvioVideos();
                vm.VerificarSincronizacaoDados();
               
            }
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
