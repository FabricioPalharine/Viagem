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
    public class ExtratoDinheiroFiltroViewModel: BaseViewModel
    {
        private ObservableCollection<ItemLista> _moedas = new ObservableCollection<ItemLista>();
        private ItemLista _moeda = null;
        private DateTime? _dataInicioDe = null;
        private readonly IApiService _apiService;

        public ExtratoDinheiroFiltroViewModel(ApiService apiService)
        {
            _apiService = apiService;
        }

        public ICommand VoltarCommand => new Command(async () => await NavigationService.TrocarPaginaShell(".."));
        public ICommand FiltrarCommand => new Command(async () => await Filtrar());


        public ObservableCollection<ItemLista> Moedas
        {
            get { return _moedas; }
            set { SetProperty(ref _moedas, value); }
        }


        public ItemLista Moeda
        {
            get { return _moeda; }
            set { SetProperty(ref _moeda, value); }
        }



        public DateTime? DataInicioDe
        {
            get { return _dataInicioDe; }
            set { SetProperty(ref _dataInicioDe, value); }
        }



        public override Task InitializeAsync(object navigationData)
        {
            IsBusy = true;
            try
            {
                Moedas = Funcoes.RetornarMoedas();
                if (navigationData != null && navigationData is CriterioBusca criterio)
                {
                    DataInicioDe = criterio.DataInicioDe;
                    if (criterio.Moeda.HasValue)
                        Moeda = _moedas.Where(d => d.Codigo == criterio.Moeda.ToString()).FirstOrDefault();
                }
            }
            finally
            {
                IsBusy = false;
            }
            return Task.FromResult(true);
        }


        private async Task Filtrar()
        {
            CriterioBusca itemBusca = new CriterioBusca()
            {
                DataInicioDe = DataInicioDe,
                Moeda = Moeda == null ? new Nullable<int>() : Convert.ToInt32(Moeda.Codigo)
            };
            MessagingCenter.Send<ExtratoDinheiroFiltroViewModel, CriterioBusca>(this, MessageKeys.FiltrarExtratoDinheiro, itemBusca);
            await NavigationService.TrocarPaginaShell("..");
        }
    }
}

