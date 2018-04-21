using CV.Mobile.Helpers;
using CV.Mobile.Interfaces;
using CV.Mobile.Models;
using CV.Mobile.Services;
using CV.Mobile.Views;
using Microsoft.Practices.ServiceLocation;
using MvvmHelpers;
using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Auth;
using Xamarin.Forms;

namespace CV.Mobile.ViewModels
{
    public class LoadingViewModel: BaseViewModel
    {
        public LoadingViewModel()
        {
            IsBusy = true;
            CaminhoBase = Settings.BaseWebApi;
            TapCommand = new Command(
                                                                       () =>  AbrirConfiguracaoURL(),
                                                                       () => true);

            PageAppearingCommand = new Command(
                                                                      async () => {
                                                                          var itemUsuario = await ConectarAutenticacao();
                                                                          if (itemUsuario != null)
                                                                            (App.Current as App).RedirectToMenu(itemUsuario);
                                                                      },
                                                                      () => true);

            EntrarCommand = new Command(()=>
            {
                App.Current.MainPage = new AutenticacaoPage();
            });
            GravarURLCommand = new Command(() => Settings.BaseWebApi = CaminhoBase);
                                                                   
        }


        private bool _ConfigurarURL = false;
        private bool loadFinished = false;
        public string CaminhoBase { get; set; }
        public ICommand TapCommand { get; set; }
        public ICommand PageAppearingCommand { get; set; }
        public ICommand EntrarCommand { get; set; }

        public ICommand GravarURLCommand { get; set; }

        public bool LoadFinished
        {
            get
            {
                return loadFinished;
            }

            set
            {
                SetProperty(ref loadFinished, value);
            }
        }

        public bool ConfigurarURL
        {
            get
            {
                return _ConfigurarURL;
            }

            set
            {
                SetProperty(ref _ConfigurarURL, value);
            }
        }

        private void AbrirConfiguracaoURL()
        {
            ConfigurarURL = true;
        }

        private async Task<UsuarioLogado> ConectarAutenticacao()
        {
            UsuarioLogado itemUsuario = null;
            IValidaAutenticacao _validador = ServiceLocator.Current.GetInstance<IValidaAutenticacao>();
            var autenticacao = await _validador.RetornarAutenticacaoAplicacao();
            if (autenticacao != null)
            {
                itemUsuario = new UsuarioLogado();
               
                using (ApiService srv = new ApiService())
                {
                    if (CrossConnectivity.Current.IsConnected && await srv.VerificarOnLine())
                    {
                        try
                        {
                            itemUsuario = await srv.CarregarDadosAplicativo(new UsuarioLogado() { CodigoGoogle = autenticacao.Username });
                            SalvarAmigosLocal(itemUsuario);
                        }
                        catch
                        {

                        }
                    }
                    if (string.IsNullOrEmpty(itemUsuario.CodigoGoogle))
                    {
                        itemUsuario.AuthenticationToken = autenticacao.Properties["AuthenticationToken"];
                        itemUsuario.Codigo = Convert.ToInt32(autenticacao.Properties["Codigo"]);
                        itemUsuario.CodigoGoogle = autenticacao.Username;
                        itemUsuario.Email = autenticacao.Properties["Email"];
                        itemUsuario.LinkFoto = autenticacao.Properties["LinkFoto"];
                        itemUsuario.Nome = autenticacao.Properties["Nome"];
                    }
                    
                }

            }
            IsBusy = false;
            LoadFinished = true;
            return itemUsuario;
        }

        private async void SalvarAmigosLocal(UsuarioLogado itemUsuario)
        {
            using (ApiService srv = new ApiService())
            {

                if (!string.IsNullOrEmpty(itemUsuario.CodigoGoogle))
                {
                    var ListaAmigos = await srv.ListarAmigosUsuario();
                    await DatabaseService.SalvarAmigos(ListaAmigos);
                }
            }

        }
    }
}
