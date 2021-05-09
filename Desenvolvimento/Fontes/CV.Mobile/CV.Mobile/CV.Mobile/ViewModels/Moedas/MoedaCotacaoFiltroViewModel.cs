using CV.Mobile.Helper;
using CV.Mobile.Models;
using CV.Mobile.Services.Api;
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
    public class MoedaCotacaoFiltroViewModel : BaseViewModel
    {
        private ObservableCollection<ItemLista> _moedas = new ObservableCollection<ItemLista>();
        private ItemLista _moeda = null;
        private DateTime? _dataInicioDe = null;
        private DateTime? _dataInicioAte = null;
        private readonly IApiService _apiService;

        public MoedaCotacaoFiltroViewModel(ApiService apiService)
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

        public DateTime? DataInicioAte
        {
            get { return _dataInicioAte; }
            set { SetProperty(ref _dataInicioAte, value); }
        }

       

        public override Task InitializeAsync(object navigationData)
        {
            IsBusy = true;
            try
            {
                Moedas = Funcoes.RetornarMoedas();
                Moedas.Insert(0, new ItemLista() { Codigo = null, Descricao = "Todas" });
         
                if (navigationData != null && navigationData is CriterioBusca criterio)
                {
                    DataInicioDe = criterio.DataInicioDe;
                    DataInicioAte = criterio.DataInicioAte;
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
                DataInicioAte = DataInicioAte,
                DataInicioDe = DataInicioDe,
                Moeda = Moeda == null ? new Nullable<int>() : Convert.ToInt32( Moeda.Codigo)
            };
            MessagingCenter.Send<MoedaCotacaoFiltroViewModel, CriterioBusca>(this, MessageKeys.FiltrarCotacao, itemBusca);
            await NavigationService.TrocarPaginaShell("..");
        }
    }
}
