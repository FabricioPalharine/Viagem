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
    public class MoedaCotacaoViewModel : BaseViewModel 
    {
        private readonly IApiService _apiService;
        private CriterioBusca criterioBusca = new CriterioBusca();
        private readonly IDatabase _database;
        private readonly IDataService _dataService;
        private ObservableCollection<CotacaoMoeda> _cotacoes = new ObservableCollection<CotacaoMoeda>();
        private readonly ISettingsService _settingsService;
        public MoedaCotacaoViewModel(IApiService apiService, IDataService dataService, IDatabase database, ISettingsService settingsService )
        {
            _apiService = apiService;
            _database = database;
            _dataService = dataService;
            _settingsService = settingsService;
        }

        public ICommand PageAppearingCommand => new Command(async () =>
        {
            MessagingCenter.Unsubscribe<MoedaCotacaoFiltroViewModel, CriterioBusca>(this, MessageKeys.FiltrarCotacao);
            MessagingCenter.Subscribe<MoedaCotacaoFiltroViewModel, CriterioBusca>(this, MessageKeys.FiltrarCotacao, async (sender, obj) =>
            {
                criterioBusca.Moeda = obj.Moeda;
                criterioBusca.DataInicioAte = obj.DataInicioAte;
                criterioBusca.DataInicioDe = obj.DataInicioDe;
                await CarregarLista();

            });
            MessagingCenter.Unsubscribe<MoedaCotacaoEdicaoViewModel, CotacaoMoeda>(this, MessageKeys.SalvarCotacao);
            MessagingCenter.Subscribe<MoedaCotacaoEdicaoViewModel, CotacaoMoeda>(this, MessageKeys.SalvarCotacao, async (sender, obj) =>
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
            await NavigationService.TrocarPaginaShell("MoedaCotacaoFiltroPage", criterioBusca);

        });

        public ICommand EditarCommand => new Command<CotacaoMoeda>(async (d) =>
        {
            
            await NavigationService.TrocarPaginaShell("MoedaCotacaoEdicaoPage", d);

        });

        public ICommand IncluirCommand => new Command(async () =>
        {

            await NavigationService.TrocarPaginaShell("MoedaCotacaoEdicaoPage", null);

        });

        public ICommand ExcluirCommand => new Command<CotacaoMoeda>(async (d) =>
        {
            if (await DialogService.ShowConfirmAsync(string.Format( AppResource.ExcluirCotacao, ((enumMoeda?) d.Moeda).GetValueOrDefault().Descricao(), d.DataCotacao.GetValueOrDefault().ToString("dd/MM/yyyy")),
                    AppResource.Confirmacao,
                    AppResource.Confirmar, AppResource.Cancelar))
            {
                await Excluir(d);
            }           

        },(d)=>!IsBusy);

        private async Task Excluir(CotacaoMoeda d)
        {
            IsBusy = true;
            try
            {
                ResultadoOperacao resultado = new ResultadoOperacao();
                bool atualizadoBanco = false;

                if (Funcoes.VerificarConsultaInternet(_settingsService.AcompanhamentoOnline) && d.Identificador > 0)
                {
                    resultado = await _apiService.ExcluirCotacaoMoeda(d.Identificador);
                    if (resultado != null)
                    {
                        atualizadoBanco = true;
                       if (resultado.Sucesso)
                        {
                            var itemAjustar = await _database.RetornarCotacaoMoeda(d.Identificador);
                            if (itemAjustar != null)
                                await _database.ExcluirCotacaoMoeda(itemAjustar);
                        }
                    }
                }
                if (!atualizadoBanco )
                {

                    if (d.Identificador > 0)
                    {
                        var cotacaoMoeda = await _database.RetornarCotacaoMoeda(d.Identificador);
                        cotacaoMoeda.DataExclusao = DateTime.UtcNow;
                        cotacaoMoeda.AtualizadoBanco = false;
                        await _database.SalvarCotacaoMoeda(cotacaoMoeda);
                    }
                    else
                        await _database.ExcluirCotacaoMoeda(d);
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

        public ObservableCollection<CotacaoMoeda> Cotacoes
        {
            get { return _cotacoes; }
            set { SetProperty(ref _cotacoes, value); }
        }

        private async Task CarregarLista()
        {
            IsBusy = true;
            try
            {
                IList<CotacaoMoeda> cotacoes = await _database.ListarCotacaoMoeda(criterioBusca);
                if (Funcoes.VerificarConsultaInternet(_settingsService.AcompanhamentoOnline ))
                {
                    IList<CotacaoMoeda> cotacoesOnline = await _apiService.ListarCotacaoMoeda(criterioBusca);
                    foreach(var cotacao in cotacoes.Where(d=>!d.AtualizadoBanco))
                    {
                        if (!cotacao.Identificador.HasValue)
                            cotacoesOnline.Add(cotacao);
                        else
                        {
                            var itemCotacao = cotacoesOnline.Where(d => d.Identificador == cotacao.Identificador).FirstOrDefault();
                            if (itemCotacao == null)
                                cotacoesOnline.Add(cotacao);
                            else if (itemCotacao.DataAtualizacao < cotacao.DataAtualizacao)
                            {
                                itemCotacao.Id = cotacao.Id;
                                itemCotacao.Moeda = cotacao.Moeda;
                                itemCotacao.DataCotacao = cotacao.DataCotacao;
                                itemCotacao.ValorCotacao = cotacao.ValorCotacao;
                            }
                        }
                    }
                    cotacoes = cotacoesOnline;
                }
                Cotacoes = new ObservableCollection<CotacaoMoeda>(cotacoes.OrderBy(d=>d.DataCotacao));

            }
            finally
            {
                IsBusy = false;
            }


        }
    }
}
