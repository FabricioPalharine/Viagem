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

namespace CV.Mobile.ViewModels.Gastos
{
    public class GastoListaViewModel: BaseViewModel
    {
        private readonly IApiService _apiService;
        private CriterioBusca criterioBusca = new CriterioBusca() { IdentificadorParticipante = GlobalSetting.Instance.UsuarioLogado.Codigo };
        private readonly IDatabase _database;
        private readonly IDataService _dataService;
        private ObservableCollection<Gasto> _aquisicoes = new ObservableCollection<Gasto>();
        private readonly ISettingsService _settingsService;
        public GastoListaViewModel(IApiService apiService, IDataService dataService, IDatabase database, ISettingsService settingsService)
        {
            _apiService = apiService;
            _database = database;
            _dataService = dataService;
            _settingsService = settingsService;
        }

        public ICommand PageAppearingCommand => new Command(async () =>
        {
            MessagingCenter.Unsubscribe<GastoFiltroViewModel, CriterioBusca>(this, MessageKeys.FiltrarCusto);
            MessagingCenter.Subscribe<GastoFiltroViewModel, CriterioBusca>(this, MessageKeys.FiltrarCusto,  (sender, obj) =>
            {
                criterioBusca.Moeda = obj.Moeda;
                criterioBusca.DataInicioAte = obj.DataInicioAte;
                criterioBusca.DataInicioDe = obj.DataInicioDe;
                criterioBusca.Nome = obj.Nome;
               //await CarregarLista();

            });
            MessagingCenter.Unsubscribe<GastoEdicaoViewModel, Gasto>(this, MessageKeys.SalvarCusto);
            MessagingCenter.Subscribe<GastoEdicaoViewModel, Gasto>(this, MessageKeys.SalvarCusto,  (sender, obj) =>
            {
               // await CarregarLista();

            });
            await CarregarLista();
        });

        public ICommand RecarregarListaCommand => new Command(async () =>
        {
            await CarregarLista();

        });



        public ICommand FiltrarCommand => new Command(async () =>
        {
            await NavigationService.TrocarPaginaShell("GastoFiltroPage", criterioBusca);

        });

        public ICommand EditarCommand => new Command<Gasto>(async (d) =>
        {

            await NavigationService.TrocarPaginaShell("GastoEdicaoPage", d);

        });

        public ICommand IncluirCommand => new Command(async () =>
        {

            await NavigationService.TrocarPaginaShell("GastoEdicaoPage", null);

        });

        public ICommand ExcluirCommand => new Command<Gasto>(async (d) =>
        {
            if (await DialogService.ShowConfirmAsync(string.Format(AppResource.ExcluirGasto, d.Descricao, d.SiglaMoeda, d.Valor.GetValueOrDefault().ToString("N2")),
                    AppResource.Confirmacao,
                    AppResource.Confirmar, AppResource.Cancelar))
            {
                await Excluir(d);
                Gastos.Remove(d);
            }

        }, (d) => !IsBusy);

        private async Task Excluir(Gasto d)
        {
            IsBusy = true;
            try
            {
                ResultadoOperacao resultado = new ResultadoOperacao();
                bool atualizadoBanco = false;

                if (Funcoes.VerificarConsultaInternet(_settingsService.AcompanhamentoOnline) && d.Identificador > 0)
                {
                    resultado = await _apiService.ExcluirGasto(d.Identificador);
                    if (resultado != null)
                    {
                        var itemBase = await _dataService.CarregarGasto(d.Identificador);
                        if (itemBase != null)
                            await _dataService.ExcluirGasto(itemBase, true);
                    }
                }
                if (!atualizadoBanco)
                {
                    var itemBase = await _dataService.CarregarGasto(d.Identificador);
                    if (itemBase != null)
                        await _dataService.ExcluirGasto(itemBase, false);
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

        public ObservableCollection<Gasto> Gastos
        {
            get { return _aquisicoes; }
            set { SetProperty(ref _aquisicoes, value); }
        }

        private async Task CarregarLista()
        {
            IsBusy = true;
            try
            {
                IList<Gasto> lista = await _database.ListarGasto(criterioBusca);
                
                Gastos = new ObservableCollection<Gasto>(lista.OrderBy(d => d.Data).ThenBy(d=>d.Hora));

            }
            finally
            {
                IsBusy = false;
            }


        }
    }
}
