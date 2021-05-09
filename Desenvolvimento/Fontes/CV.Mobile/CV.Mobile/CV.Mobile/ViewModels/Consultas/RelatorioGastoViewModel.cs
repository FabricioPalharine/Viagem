using CV.Mobile.Enums;
using CV.Mobile.Helper;
using CV.Mobile.Models;
using CV.Mobile.Resources;
using CV.Mobile.Services.Api;
using CV.Mobile.Services.Data;
using CV.Mobile.Services.Settings;
using CV.Mobile.Validations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
namespace CV.Mobile.ViewModels.Consultas
{
    public class RelatorioGastoViewModel: BaseViewModel
    {
        private CriterioBusca criterioBusca = new CriterioBusca() { DataInicioDe = GlobalSetting.Instance.ViagemSelecionado.DataInicio, Moeda = GlobalSetting.Instance.ViagemSelecionado.Moeda };
        private readonly IApiService _apiService;
        private ObservableCollection<RelatorioGastos> _dados = new ObservableCollection<RelatorioGastos>();
        private readonly ISettingsService _settingsService;
        public RelatorioGastoViewModel(IApiService apiService, ISettingsService settingsService)
        {
            _apiService = apiService;
            criterioBusca.DataInicioDe = GlobalSetting.Instance.ViagemSelecionado.DataInicio;
            _settingsService = settingsService;
        }

        public ICommand PageAppearingCommand => new Command(async () =>
        {
            MessagingCenter.Instance.Unsubscribe<RelatorioGastoFiltroViewModel, CriterioBusca>(this, MessageKeys.FiltrarRelatorioGasto);
            MessagingCenter.Instance.Subscribe<RelatorioGastoFiltroViewModel, CriterioBusca>(this, MessageKeys.FiltrarRelatorioGasto,  (sender, obj) =>
            {
                criterioBusca.DataInicioAte = obj.DataInicioAte;
                criterioBusca.DataInicioDe = obj.DataInicioDe;
                criterioBusca.Tipo = obj.Tipo;
                criterioBusca.IdentificadorParticipante = obj.IdentificadorParticipante;


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
            await NavigationService.TrocarPaginaShell("RelatorioGastoFiltroPage", criterioBusca);

        });


        public ObservableCollection<RelatorioGastos> Dados
        {
            get { return _dados; }
            set { SetProperty(ref _dados, value); }
        }

        private async Task CarregarLista()
        {
            IsBusy = true;
            try
            {
                IList<RelatorioGastos> lista = await _apiService.ListarRelatorioGastos(criterioBusca);

                Dados = new ObservableCollection<RelatorioGastos>(lista);

            }
            finally
            {
                IsBusy = false;
            }


        }

    }
}

