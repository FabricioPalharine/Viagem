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

namespace CV.Mobile.ViewModels.Hoteis
{
    public class HotelListaViewModel : BaseViewModel
    {
        private readonly IApiService _apiService;
        private CriterioBusca criterioBusca = new CriterioBusca();
        private readonly IDatabase _database;
        private readonly IDataService _dataService;
        private ObservableCollection<Hotel> _hoteis = new ObservableCollection<Hotel>();
        private readonly ISettingsService _settingsService;
        public HotelListaViewModel(IApiService apiService, IDataService dataService, IDatabase database, ISettingsService settingsService)
        {
            _apiService = apiService;
            _database = database;
            _dataService = dataService;
            _settingsService = settingsService;
        }

        public ICommand PageAppearingCommand => new Command(async () =>
        {
            MessagingCenter.Unsubscribe<HotelFiltroViewModel, CriterioBusca>(this, MessageKeys.FiltrarHotel);
            MessagingCenter.Subscribe<HotelFiltroViewModel, CriterioBusca>(this, MessageKeys.FiltrarHotel,  (sender, obj) =>
            {
                criterioBusca.Situacao = obj.Situacao;
                criterioBusca.DataInicioAte = obj.DataInicioAte;
                criterioBusca.DataInicioDe = obj.DataInicioDe;
                criterioBusca.DataFimAte = obj.DataFimAte;
                criterioBusca.DataFimDe = obj.DataFimDe;
                criterioBusca.Nome = obj.Nome;
                //await CarregarLista();

            });
            MessagingCenter.Unsubscribe<HotelEdicaoViewModel, Hotel>(this, MessageKeys.SalvarHotel);
            MessagingCenter.Subscribe<HotelEdicaoViewModel, Hotel>(this, MessageKeys.SalvarHotel,  (sender, obj) =>
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
            await NavigationService.TrocarPaginaShell("HotelFiltroPage", criterioBusca);

        });

        public ICommand EditarCommand => new Command<Hotel>(async (d) =>
        {

            await NavigationService.TrocarPaginaShell("HotelEdicaoPage", d);

        });

        public ICommand IncluirCommand => new Command(async () =>
        {

            await NavigationService.TrocarPaginaShell("HotelEdicaoPage", null);

        });

        public ICommand ExcluirCommand => new Command<Hotel>(async (d) =>
        {
            if (await DialogService.ShowConfirmAsync(string.Format(AppResource.ExcluirHotel, d.Nome),
                    AppResource.Confirmacao,
                    AppResource.Confirmar, AppResource.Cancelar))
            {
                await Excluir(d);
                Hoteis.Remove(d);
            }

        }, (d) => !IsBusy);

        private async Task Excluir(Hotel d)
        {
            IsBusy = true;
            try
            {
                ResultadoOperacao resultado = new ResultadoOperacao();
                bool atualizadoBanco = false;

                if (Funcoes.VerificarConsultaInternet(_settingsService.AcompanhamentoOnline) && d.Identificador > 0)
                {
                    resultado = await _apiService.ExcluirHotel(d.Identificador);
                    if (resultado != null)
                    {
                        atualizadoBanco = true;
                        if (resultado.Sucesso)
                        {
                            await _dataService.ExcluirHotel(d.Identificador, true);
                        }
                    }
                }
                if (!atualizadoBanco)
                {
                    await _dataService.ExcluirHotel(d.Identificador, false);
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

        public ObservableCollection<Hotel> Hoteis
        {
            get { return _hoteis; }
            set { SetProperty(ref _hoteis, value); }
        }

        private async Task CarregarLista()
        {
            IsBusy = true;
            try
            {
                IList<Hotel> lista = await _database.ListarHotel(criterioBusca);
                
                  
                Hoteis = new ObservableCollection<Hotel>(lista.OrderByDescending(d => d.DataEntrada));

            }
            finally
            {
                IsBusy = false;
            }


        }
    }
}



