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
    public class GastoSelecaoViewModel : BaseViewModel
    {
        private readonly IApiService _apiService;
        private CriterioBusca criterioBusca = new CriterioBusca() { IdentificadorParticipante = GlobalSetting.Instance.UsuarioLogado.Codigo };
        private readonly IDatabase _database;
        private readonly IDataService _dataService;
        private ObservableCollection<Gasto> _aquisicoes = new ObservableCollection<Gasto>();
        private readonly ISettingsService _settingsService;
        private string NomeMensagem = MessageKeys.SelecionarGasto;
        public GastoSelecaoViewModel(IApiService apiService, IDataService dataService, IDatabase database, ISettingsService settingsService)
        {
            _apiService = apiService;
            _database = database;
            _dataService = dataService;
            _settingsService = settingsService;
        }

        public override async Task InitializeAsync(object navigationData)
        {
            MessagingCenter.Unsubscribe<GastoFiltroViewModel, CriterioBusca>(this, MessageKeys.FiltrarCusto);
            MessagingCenter.Subscribe<GastoFiltroViewModel, CriterioBusca>(this, MessageKeys.FiltrarCusto, async (sender, obj) =>
            {
                criterioBusca.Moeda = obj.Moeda;
                criterioBusca.DataInicioAte = obj.DataInicioAte;
                criterioBusca.DataInicioDe = obj.DataInicioDe;
                criterioBusca.Nome = obj.Nome;
                await CarregarLista();

            });
            if (navigationData != null && navigationData is string)
            {
                NomeMensagem = Convert.ToString(navigationData);
            }
            await CarregarLista();
        }

        public ICommand CancelarCommand => new Command(async () =>
         await NavigationService.TrocarPaginaShell(".."));



        public ICommand RecarregarListaCommand => new Command(async () =>
        {
            await CarregarLista();

        });



        public ICommand FiltrarCommand => new Command(async () =>
        {
            await NavigationService.TrocarPaginaShell("GastoFiltroPage", criterioBusca);

        });

        public ICommand SelecionarCommand => new Command<Gasto>( async (d) =>
        {
            MessagingCenter.Instance.Send<GastoSelecaoViewModel, Gasto>(this, NomeMensagem, d);
            await NavigationService.TrocarPaginaShell("..");
        });

        

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
