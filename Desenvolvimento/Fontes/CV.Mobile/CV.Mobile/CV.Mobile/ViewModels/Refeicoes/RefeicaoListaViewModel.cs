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

namespace CV.Mobile.ViewModels.Refeicoes
{
    public class RefeicaoListaViewModel : BaseViewModel
    {

        private readonly IApiService _apiService;
        private CriterioBusca criterioBusca = new CriterioBusca();
        private readonly IDatabase _database;
        private readonly IDataService _dataService;
        private ObservableCollection<Refeicao> _refeicoes = new ObservableCollection<Refeicao>();
        private readonly ISettingsService _settingsService;
        public RefeicaoListaViewModel(IApiService apiService, IDataService dataService, IDatabase database, ISettingsService settingsService)
        {
            _apiService = apiService;
            _database = database;
            _dataService = dataService;
            _settingsService = settingsService;
        }

        public ICommand PageAppearingCommand => new Command(async () =>
        {
            MessagingCenter.Unsubscribe<RefeicaoFiltroViewModel, CriterioBusca>(this, MessageKeys.FiltrarRestaurante);
            MessagingCenter.Subscribe<RefeicaoFiltroViewModel, CriterioBusca>(this, MessageKeys.FiltrarRestaurante, async (sender, obj) =>
            {
                criterioBusca.DataInicioAte = obj.DataInicioAte;
                criterioBusca.DataInicioDe = obj.DataInicioDe;
                criterioBusca.Nome = obj.Nome;
                criterioBusca.Tipo = obj.Tipo;
                await CarregarLista();

            });
            MessagingCenter.Unsubscribe<RefeicaoEdicaoViewModel, Refeicao>(this, MessageKeys.SalvarRestaurante);
            MessagingCenter.Subscribe<RefeicaoEdicaoViewModel, Refeicao>(this, MessageKeys.SalvarRestaurante, async (sender, obj) =>
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
            await NavigationService.TrocarPaginaShell("RefeicaoFiltroPage", criterioBusca);

        });

        public ICommand EditarCommand => new Command<Refeicao>(async (d) =>
        {

            await NavigationService.TrocarPaginaShell("RefeicaoEdicaoPage", d);

        });

        public ICommand IncluirCommand => new Command(async () =>
        {

            await NavigationService.TrocarPaginaShell("RefeicaoEdicaoPage", null);

        });

        public ICommand ExcluirCommand => new Command<Refeicao>(async (d) =>
        {
            if (await DialogService.ShowConfirmAsync(string.Format(AppResource.ExcluirRefeição, d.Nome),
                    AppResource.Confirmacao,
                    AppResource.Confirmar, AppResource.Cancelar))
            {
                await Excluir(d);
                Refeicoes.Remove(d);
            }

        }, (d) => !IsBusy);

        private async Task Excluir(Refeicao d)
        {
            IsBusy = true;
            try
            {
                ResultadoOperacao resultado = new ResultadoOperacao();
                bool atualizadoBanco = false;

                if (Funcoes.VerificarConsultaInternet(_settingsService.AcompanhamentoOnline) && d.Identificador > 0)
                {
                    resultado = await _apiService.ExcluirRefeicao(d.Identificador);
                    if (resultado != null)
                    {
                        atualizadoBanco = true;
                        if (resultado.Sucesso)
                        {
                            await _dataService.ExcluirRefeicao(d.Identificador, true);
                        }
                    }
                }
                if (!atualizadoBanco)
                {
                    await _dataService.ExcluirRefeicao(d.Identificador, false);
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

        public ObservableCollection<Refeicao> Refeicoes
        {
            get { return _refeicoes; }
            set { SetProperty(ref _refeicoes, value); }
        }

        private async Task CarregarLista()
        {
            IsBusy = true;
            try
            {
                IList<Refeicao> lista = await _database.ListarRefeicao(criterioBusca);

                Refeicoes = new ObservableCollection<Refeicao>(lista.OrderByDescending(d => d.Data));

            }
            finally
            {
                IsBusy = false;
            }


        }
    }
}

