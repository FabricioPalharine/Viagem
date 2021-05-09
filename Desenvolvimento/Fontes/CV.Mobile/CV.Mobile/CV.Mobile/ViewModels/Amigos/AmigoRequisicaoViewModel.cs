using CV.Mobile.Models;
using CV.Mobile.Resources;
using CV.Mobile.Services.Api;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace CV.Mobile.ViewModels.Amigos
{
    public class AmigoRequisicaoViewModel : BaseViewModel
    {
        private readonly IApiService _apiService;
        private ObservableCollection<RequisicaoAmizade> _listaAmigos = new ObservableCollection<RequisicaoAmizade>();
        public AmigoRequisicaoViewModel(IApiService apiService)
        {
            _apiService = apiService;
        }

        public ObservableCollection<RequisicaoAmizade> ListaAmigos
        {
            get { return _listaAmigos; }
            set { SetProperty(ref _listaAmigos, value); }
        }

        public ICommand PageAppearingCommand => new Command(async () => await CarregarLista());
        public ICommand RecarregarListaCommand => new Command(async () => await CarregarLista());

        public ICommand AprovarCommand => new Command<RequisicaoAmizade>(async (d) => await Aprovar(d));
        public ICommand ReprovarCommand => new Command<RequisicaoAmizade>(async (d) => await Reprovar(d));


        private async Task CarregarLista()
        {
            IsBusy = true;
            try
            {
                var Dados = await _apiService.ListarRequisicaoAmizade();
                ListaAmigos = new ObservableCollection<RequisicaoAmizade>(Dados);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task Reprovar(RequisicaoAmizade requisicao)
        {
            if (await DialogService.ShowConfirmAsync(AppResource.ReprovarRequisicao, AppResource.AppName, AppResource.Confirmar, AppResource.Cancelar))
            {
                IsBusy = true;
                requisicao.Status = 3;
                try
                {

                    var Resultado = await _apiService.SalvarRequisicaoAmizade(requisicao);
                    await DialogService.ShowAlertAsync(String.Join(Environment.NewLine, Resultado.Mensagens.Select(d => d.Mensagem).ToArray()),
                        AppResource.Sucesso, AppResource.Ok);

                }
                finally
                {
                    IsBusy = false;
                }
            }
            await CarregarLista();
        }

        public async Task Aprovar(RequisicaoAmizade requisicao)
        {
            IsBusy = true;
            requisicao.Status = 2;
            try
            {

                var Resultado = await _apiService.SalvarRequisicaoAmizade(requisicao);
                await DialogService.ShowAlertAsync(String.Join(Environment.NewLine, Resultado.Mensagens.Select(d => d.Mensagem).ToArray()),
                    AppResource.Sucesso, AppResource.Ok);

            }
            finally
            {
                IsBusy = false;
            }
            await CarregarLista();
        }
    }
}
