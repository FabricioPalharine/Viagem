using CV.Mobile.Enums;
using CV.Mobile.Helper;
using CV.Mobile.Models;
using CV.Mobile.Resources;
using CV.Mobile.Services.Api;
using CV.Mobile.Services.Data;
using CV.Mobile.Services.Settings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace CV.Mobile.ViewModels.Deslocamentos
{
    public class DeslocamentoListaViewModel: BaseViewModel
    {
        private readonly IApiService _apiService;
        private CriterioBusca criterioBusca = new CriterioBusca();
        private readonly IDatabase _database;
        private readonly IDataService _dataService;
        private ObservableCollection<ViagemAerea> _viagemAereas = new ObservableCollection<ViagemAerea>();
        private readonly ISettingsService _settingsService;
        public DeslocamentoListaViewModel(IApiService apiService, IDataService dataService, IDatabase database, ISettingsService settingsService)
        {
            _apiService = apiService;
            _database = database;
            _dataService = dataService;
            _settingsService = settingsService;
        }

        public ICommand PageAppearingCommand => new Command(async () =>
        {
            MessagingCenter.Unsubscribe<DeslocamentoFiltroViewModel, CriterioBusca>(this, MessageKeys.FiltrarDeslocamento);
            MessagingCenter.Subscribe<DeslocamentoFiltroViewModel, CriterioBusca>(this, MessageKeys.FiltrarDeslocamento,  (sender, obj) =>
            {
                criterioBusca.Situacao = obj.Situacao;
                criterioBusca.DataInicioAte = obj.DataInicioAte;
                criterioBusca.DataInicioDe = obj.DataInicioDe;
                criterioBusca.DataFimAte = obj.DataFimAte;
                criterioBusca.DataFimDe = obj.DataFimDe;
                criterioBusca.Situacao = obj.Situacao;
                criterioBusca.Nome = obj.Nome;
                //await CarregarLista();

            });
            MessagingCenter.Unsubscribe<DeslocamentoEdicaoViewModel, ViagemAerea>(this, MessageKeys.SalvarDeslocamento);
            MessagingCenter.Subscribe<DeslocamentoEdicaoViewModel, ViagemAerea>(this, MessageKeys.SalvarDeslocamento,  (sender, obj) =>
            {
                //await CarregarLista();

            });
            await CarregarLista();
        });

        public ICommand RecarregarListaCommand => new Command(async () =>
        {
            await CarregarLista();

        });



        public ICommand FiltrarCommand => new Command(async () =>
        {
            await NavigationService.TrocarPaginaShell("DeslocamentoFiltroPage", criterioBusca);

        });

        public ICommand EditarCommand => new Command<ViagemAerea>(async (d) =>
        {

            await NavigationService.TrocarPaginaShell("DeslocamentoEdicaoPage", d);

        });

        public ICommand IncluirCommand => new Command(async () =>
        {

            await NavigationService.TrocarPaginaShell("DeslocamentoEdicaoPage", null);

        });

        public ICommand ExcluirCommand => new Command<ViagemAerea>(async (d) =>
        {
            if (await DialogService.ShowConfirmAsync(string.Format(AppResource.ExcluirDeslocamento, d.Descricao),
                    AppResource.Confirmacao,
                    AppResource.Confirmar, AppResource.Cancelar))
            {
                await Excluir(d);
                Deslocamentos.Remove(d);
            }

        }, (d) => !IsBusy);

        private async Task Excluir(ViagemAerea d)
        {
            IsBusy = true;
            try
            {
                ResultadoOperacao resultado = new ResultadoOperacao();
                bool atualizadoBanco = false;

                if (Funcoes.VerificarConsultaInternet(_settingsService.AcompanhamentoOnline) && d.Identificador > 0)
                {
                    resultado = await _apiService.ExcluirViagemAerea(d.Identificador);
                    if (resultado != null)
                    {
                        atualizadoBanco = true;
                        if (resultado.Sucesso)
                        {
                            await _dataService.ExcluirViagemAerea(d.Identificador, true);
                        }
                    }
                }
                if (!atualizadoBanco)
                {
                    await _dataService.ExcluirViagemAerea(d.Identificador, false);
                    resultado = new ResultadoOperacao()
                    {
                        Sucesso = true,
                        Mensagens = new MensagemErro[] { new MensagemErro() { Mensagem = AppResource.ExclusaoSucesso } }
                    };

                }
                if (resultado != null)
                    await base.ExibirResultado(resultado);

            }
            finally
            {
                IsBusy = false;
            }
        }

        public ObservableCollection<ViagemAerea> Deslocamentos
        {
            get { return _viagemAereas; }
            set { SetProperty(ref _viagemAereas, value); }
        }

        private async Task CarregarLista()
        {
            IsBusy = true;
            try
            {
                IList<ViagemAerea> lista = await _database.ListarViagemAerea(criterioBusca);

                Deslocamentos = new ObservableCollection<ViagemAerea>(lista.OrderByDescending(d => d.DataInicio));

            }
            finally
            {
                IsBusy = false;
            }


        }
    }
}