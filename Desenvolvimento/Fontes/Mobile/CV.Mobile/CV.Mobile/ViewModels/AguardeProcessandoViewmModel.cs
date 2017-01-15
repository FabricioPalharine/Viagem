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
    public class AguardeProcessandoViewmModel : BaseViewModel
    {
        UsuarioLogado itemUsuario = null;
        public AguardeProcessandoViewmModel( UsuarioLogado pitemUsuario)
        {
            IsBusy = true;
            itemUsuario = pitemUsuario;           
            PageAppearingCommand = new Command(
                                                                       () =>
                                                                       {
                                                                           CarregarDadosAplicacao();
                                                                       },
                                                                      () => true);

           
        }


        public ICommand PageAppearingCommand { get; set; }
       
        private async void CarregarDadosAplicacao()
        {
            MasterDetailViewModel vm = new MasterDetailViewModel(itemUsuario);
            Viagem itemViagem = await DatabaseService.Database.GetViagemAtualAsync();
            await vm.VerificarConexaoSignalR(itemViagem);
            if (itemViagem != null)
            {
                var itemCS = await DatabaseService.Database.GetControleSincronizacaoAsync();
                itemCS.SincronizadoEnvio = false;
                await DatabaseService.Database.SalvarControleSincronizacao(itemCS);
                vm.ItemViagem = itemViagem;
                //vm.PreencherPaginasViagem(itemViagem);
                using (ApiService srv = new ApiService())
                {
                    if (CrossConnectivity.Current.IsConnected && await srv.VerificarOnLine())
                    {
                        await srv.SelecionarViagem(itemViagem.Identificador);
                    }
                }
                await vm.IniciarControlePosicao();
                await vm.VerificarSincronizacaoDados();
                vm.VerificarEnvioFotos();
                vm.VerificarEnvioVideos();
            }
            var Pagina = new Views.MasterDetailPage(vm);

            IsBusy = false;
            App.Current.MainPage = Pagina;
        }

       
    }
}
