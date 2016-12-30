using CV.Mobile.Interfaces;
using CV.Mobile.Models;
using CV.Mobile.Services;
using CV.Mobile.Views;
using Microsoft.Practices.ServiceLocation;
using MvvmHelpers;
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
            TapCommand = new Command(
                                                                       async () => await AbrirConfiguracaoURL(),
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
                                                                   
        }


        private bool loadFinished = false;
        public ICommand TapCommand { get; set; }
        public ICommand PageAppearingCommand { get; set; }
        public ICommand EntrarCommand { get; set; }
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

        private async Task AbrirConfiguracaoURL()
        {
            await Task.Delay(1);
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
                    itemUsuario = await srv.CarregarDadosAplicativo(new UsuarioLogado() { CodigoGoogle = autenticacao.Username });
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
            LoadFinished = true;
            return itemUsuario;
        }
    }
}
