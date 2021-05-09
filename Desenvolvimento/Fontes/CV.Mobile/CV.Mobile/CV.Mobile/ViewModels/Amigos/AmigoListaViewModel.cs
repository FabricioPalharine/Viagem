using CV.Mobile.Models;
using CV.Mobile.Resources;
using CV.Mobile.Services.Api;
using CV.Mobile.Services.Data;
using CV.Mobile.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace CV.Mobile.ViewModels.Amigos
{
    public class AmigoListaViewModel : BaseViewModel
    {
        private readonly IApiService _apiService;
        private readonly IDataService _dataService;
        private List<ConsultaAmigo> _listaAmigos = new List<ConsultaAmigo>();

        public AmigoListaViewModel(IApiService apiService, IDataService dataService)
        {
            _apiService = apiService;
            _dataService = dataService;
            MessagingCenter.Unsubscribe<AmigoAdicaoViewModel>(this, MessageKeys.AmigoAdicionado);
            MessagingCenter.Subscribe<AmigoAdicaoViewModel>(this, MessageKeys.AmigoAdicionado, async (d) =>
              {
                  await PreencherListaAmigos();

              });
        }

        public ICommand PageAppearingCommand => new Command(async () =>
        {
            if (Connectivity.NetworkAccess == NetworkAccess.None || Connectivity.NetworkAccess == NetworkAccess.Unknown)
            {
                await DialogService.ShowAlertAsync(AppResource.AplicacaoOnlineNecessaria, AppResource.AppName, AppResource.Voltar);
                await NavigationService.TrocarPaginaShell("..");
            }
            else
                await PreencherListaAmigos();
        });

        public ICommand RecarregarListaCommand => new Command(async () => await PreencherListaAmigos());
        public ICommand TrocarSeguidoCommand => new Command<ConsultaAmigo>(async (d) => await TrocarSeguido(d));

        public ICommand TrocarSeguidorCommand => new Command<ConsultaAmigo>(async (d) => await TrocarSeguidor(d));

        public ICommand AdicionarAmigoCommand => new Command(async () => await NavigationService.TrocarPaginaShell("AmigoAdicaoPage"));
        public List<ConsultaAmigo> ListaAmigos
        {
            get { return _listaAmigos; }
            set { SetProperty(ref _listaAmigos, value); }
        }

        private async Task PreencherListaAmigos()
        {
            IsBusy = true;
            try
            {
                var Dados = await _apiService.ListarAmigosConsulta();
                ListaAmigos = Dados;
            }
            finally
            {
                IsBusy = false;
            }
        }
        public ConsultaAmigo Item { get; set; } = new ConsultaAmigo();

        private async Task TrocarSeguido(ConsultaAmigo d)
        {
            string Mensagem = !d.Seguido ? AppResource.MarcarSeguido : AppResource.DesmarcarSeguido;
            d.Acao = !d.Seguidor ? 3 : 4;
            var resultado = await DialogService.ShowConfirmAsync(Mensagem, AppResource.AppName, AppResource.Sim, AppResource.Nao);
            if (resultado)
            {
                await GravarAlteracaoAmigo(d);
            }
           
        }
        private async Task TrocarSeguidor(ConsultaAmigo d)
        {
            string Mensagem = !d.Seguidor ? AppResource.MarcarSeguidora : AppResource.DesmarcarSeguidora;
            d.Acao = !d.Seguidor ? 1 : 2;
            var resultado = await DialogService.ShowConfirmAsync(Mensagem, AppResource.AppName, AppResource.Sim, AppResource.Nao);
            if (resultado)
            {
                await GravarAlteracaoAmigo(d);
            }
           
        }

        private async Task GravarAlteracaoAmigo(ConsultaAmigo itemAmigo)
        {
            IsBusy = true;
            try
            {

                if (itemAmigo.Acao == 1 || itemAmigo.Acao == 2)
                {
                    _dataService.AjustarAmigo(itemAmigo);
                }
                var Resultado = await _apiService.RequisicaoAmizade(itemAmigo);
                await DialogService.ShowAlertAsync(String.Join(Environment.NewLine, Resultado.Mensagens.Select(d => d.Mensagem).ToArray()),
                    AppResource.Sucesso,
                    AppResource.Ok);
 
            }
            finally
            {
                IsBusy = false;
            }

            await PreencherListaAmigos();

        }
    }
}
