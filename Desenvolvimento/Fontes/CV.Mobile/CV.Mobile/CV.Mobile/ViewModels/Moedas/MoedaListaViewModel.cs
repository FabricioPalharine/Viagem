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

namespace CV.Mobile.ViewModels.Moedas
{
    public class MoedaListaViewModel: BaseViewModel
    {
        private readonly IApiService _apiService;
        private CriterioBusca criterioBusca = new CriterioBusca();
        private readonly IDatabase _database;
        private readonly IDataService _dataService;
        private ObservableCollection<AporteDinheiro> _aquisicoes = new ObservableCollection<AporteDinheiro>();
        private readonly ISettingsService _settingsService;
        public MoedaListaViewModel(IApiService apiService, IDataService dataService, IDatabase database, ISettingsService settingsService)
        {
            _apiService = apiService;
            _database = database;
            _dataService = dataService;
            _settingsService = settingsService;
        }

        public ICommand PageAppearingCommand => new Command(async () =>
        {
            MessagingCenter.Unsubscribe<MoedaFiltroViewModel, CriterioBusca>(this, MessageKeys.FiltrarMoeda);
            MessagingCenter.Subscribe<MoedaFiltroViewModel, CriterioBusca>(this, MessageKeys.FiltrarMoeda, async (sender, obj) =>
            {
                criterioBusca.Moeda = obj.Moeda;
                criterioBusca.DataInicioAte = obj.DataInicioAte;
                criterioBusca.DataInicioDe = obj.DataInicioDe;
                await CarregarLista();

            });
            MessagingCenter.Unsubscribe<MoedaEdicaoViewModel, AporteDinheiro>(this, MessageKeys.SalvarMoeda);
            MessagingCenter.Subscribe<MoedaEdicaoViewModel, AporteDinheiro>(this, MessageKeys.SalvarMoeda, async (sender, obj) =>
            {
                await CarregarLista();

            });
            await CarregarLista();
        });

        public ICommand RecarregarListaCommand => new Command(async () =>
        {
            await CarregarLista();

        });



        public ICommand FiltrarCommand => new Command(async () =>
        {
            await NavigationService.TrocarPaginaShell("MoedaFiltroPage", criterioBusca);

        });

        public ICommand EditarCommand => new Command<AporteDinheiro>(async (d) =>
        {

            await NavigationService.TrocarPaginaShell("MoedaEdicaoPage", d);

        });

        public ICommand IncluirCommand => new Command(async () =>
        {

            await NavigationService.TrocarPaginaShell("MoedaEdicaoPage", null);

        });

        public ICommand ExcluirCommand => new Command<AporteDinheiro>(async (d) =>
        {
            if (await DialogService.ShowConfirmAsync(string.Format(AppResource.ExcluirAquisicaoMoeda, d.MoedaSigla, d.Valor.GetValueOrDefault().ToString("N2")),
                    AppResource.Confirmacao,
                    AppResource.Confirmar, AppResource.Cancelar))
            {
                await Excluir(d);
                Aquisicoes.Remove(d);
            }

        }, (d) => !IsBusy);

        private async Task Excluir(AporteDinheiro d)
        {
            IsBusy = true;
            try
            {
                ResultadoOperacao resultado = new ResultadoOperacao();
                bool atualizadoBanco = false;

                if (Funcoes.VerificarConsultaInternet(_settingsService.AcompanhamentoOnline) && d.Identificador > 0)
                {
                    resultado = await _apiService.ExcluirAporteDinheiro(d.Identificador);
                    if (resultado != null)
                    {
                        atualizadoBanco = true;
                        if (resultado.Sucesso)
                        {
                            await _dataService.ExcluirAporteDinheiro(d, true);
                        }
                    }
                }
                if (!atualizadoBanco)
                {

                    await _dataService.ExcluirAporteDinheiro(d, false);
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

        public ObservableCollection<AporteDinheiro> Aquisicoes
        {
            get { return _aquisicoes; }
            set { SetProperty(ref _aquisicoes, value); }
        }

        private async Task CarregarLista()
        {
            IsBusy = true;
            try
            {
                IList<AporteDinheiro> lista = await _database.ListarAporteDinheiro(criterioBusca);
                
                Aquisicoes = new ObservableCollection<AporteDinheiro>(lista.OrderBy(d => d.DataAporte));

            }
            finally
            {
                IsBusy = false;
            }


        }
    }
}
