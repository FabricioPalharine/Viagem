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

namespace CV.Mobile.ViewModels.Atracoes
{
    public class AtracaoListaViewModel: BaseViewModel
    {
        private readonly IApiService _apiService;
        private CriterioBusca criterioBusca = new CriterioBusca();
        private readonly IDatabase _database;
        private readonly IDataService _dataService;
        private ObservableCollection<Atracao> _atracoes = new ObservableCollection<Atracao>();
        private readonly ISettingsService _settingsService;
        public AtracaoListaViewModel(IApiService apiService, IDataService dataService, IDatabase database, ISettingsService settingsService)
        {
            _apiService = apiService;
            _database = database;
            _dataService = dataService;
            _settingsService = settingsService;
        }

        public ICommand PageAppearingCommand => new Command( async () =>
        {
            MessagingCenter.Unsubscribe<AtracaoFiltroViewModel, CriterioBusca>(this, MessageKeys.FiltrarAtracao);
            MessagingCenter.Subscribe<AtracaoFiltroViewModel, CriterioBusca>(this, MessageKeys.FiltrarAtracao, (sender, obj) =>
            {
                criterioBusca.Situacao = obj.Situacao;
                criterioBusca.DataInicioAte = obj.DataInicioAte;
                criterioBusca.DataInicioDe = obj.DataInicioDe;
                criterioBusca.DataFimAte = obj.DataFimAte;
                criterioBusca.DataFimDe = obj.DataFimDe;
                criterioBusca.Situacao = obj.Situacao;
                criterioBusca.Nome = obj.Nome;
                

            });
            MessagingCenter.Unsubscribe<AtracaoEdicaoViewModel, Atracao>(this, MessageKeys.SalvarAtracao);
            MessagingCenter.Subscribe<AtracaoEdicaoViewModel, Atracao>(this, MessageKeys.SalvarAtracao,  (sender, obj) =>
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
            await NavigationService.TrocarPaginaShell("AtracaoFiltroPage", criterioBusca);

        });

        public ICommand EditarCommand => new Command<Atracao>(async (d) =>
        {

            await NavigationService.TrocarPaginaShell("AtracaoEdicaoPage", d);

        });

        public ICommand IncluirCommand => new Command(async () =>
        {

            await NavigationService.TrocarPaginaShell("AtracaoEdicaoPage", null);

        });

        public ICommand ExcluirCommand => new Command<Atracao>(async (d) =>
        {
            if (await DialogService.ShowConfirmAsync(string.Format(AppResource.ExcluirAtracao, d.Nome),
                    AppResource.Confirmacao,
                    AppResource.Confirmar, AppResource.Cancelar))
            {
                await Excluir(d);
            }

        }, (d) => !IsBusy);

        private async Task Excluir(Atracao d)
        {
            IsBusy = true;
            try
            {
                ResultadoOperacao resultado = new ResultadoOperacao();
                bool atualizadoBanco = false;

                if (Funcoes.VerificarConsultaInternet(_settingsService.AcompanhamentoOnline) && d.Identificador > 0)
                {
                    resultado = await _apiService.ExcluirAtracao(d.Identificador);
                    if (resultado != null)
                    {
                        atualizadoBanco = true;
                        if (resultado.Sucesso)
                        {
                            await _dataService.ExcluirAtracao(d.Identificador, true);
                        }
                    }
                }
                if (!atualizadoBanco)
                {
                    await _dataService.ExcluirAtracao(d.Identificador, false);
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

        public ObservableCollection<Atracao> Atracoes
        {
            get { return _atracoes; }
            set { SetProperty(ref _atracoes, value); }
        }

        private async Task CarregarLista()
        {
            IsBusy = true;
            try
            {
                IList<Atracao> lista = await _database.ListarAtracao(criterioBusca);
                
                Atracoes = new ObservableCollection<Atracao>(lista.OrderByDescending(d => d.Chegada).ThenByDescending(d => d.HoraChegada));

            }
            finally
            {
                IsBusy = false;
            }


        }
    }
}

