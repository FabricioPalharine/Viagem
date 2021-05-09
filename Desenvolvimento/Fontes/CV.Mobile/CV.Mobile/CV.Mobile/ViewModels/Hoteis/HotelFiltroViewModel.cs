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

namespace CV.Mobile.ViewModels.Hoteis
{
    public class HotelFiltroViewModel: BaseViewModel
    {
        private ObservableCollection<ItemLista> _situacoes = new ObservableCollection<ItemLista>();
        private ItemLista _situacao = null;
        private Usuario _usuario = null;
        private DateTime? _dataInicioDe = null;
        private DateTime? _dataInicioAte = null;
        private DateTime? _dataFimDe = null;
        private DateTime? _dataFimAte = null;
        private string _nome = null;
        private readonly IApiService _apiService;

        public HotelFiltroViewModel(ApiService apiService)
        {
            _apiService = apiService;
        }

        public ICommand VoltarCommand => new Command(async () => await NavigationService.TrocarPaginaShell(".."));
        public ICommand FiltrarCommand => new Command(async () => await Filtrar());


        public ObservableCollection<ItemLista> Situacoes
        {
            get { return _situacoes; }
            set { SetProperty(ref _situacoes, value); }
        }

        public string Nome
        {
            get { return _nome; }
            set { SetProperty(ref _nome, value); }
        }

        public ItemLista Situacao
        {
            get { return _situacao; }
            set { SetProperty(ref _situacao, value); }
        }

        public Usuario Usuario
        {
            get { return _usuario; }
            set { SetProperty(ref _usuario, value); }
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

        public DateTime? DataFimDe
        {
            get { return _dataFimDe; }
            set { SetProperty(ref _dataFimDe, value); }
        }

        public DateTime? DataFimAte
        {
            get { return _dataFimAte; }
            set { SetProperty(ref _dataFimAte, value); }
        }

        public override Task InitializeAsync(object navigationData)
        {
            IsBusy = true;
            try
            {
                Situacoes = new ObservableCollection<ItemLista>();
                Situacoes.Add(new ItemLista() { Codigo = "1", Descricao = "Atual" });
                Situacoes.Add(new ItemLista() { Codigo = "2", Descricao = "Já Hospedado" });
                Situacoes.Add(new ItemLista() { Codigo = "3", Descricao = "Hospedagem Futura" });
                Situacoes.Add(new ItemLista() { Codigo = "4", Descricao = "Todas" });
                Situacao = Situacoes.Last();
                if (navigationData != null && navigationData is CriterioBusca criterio)
                {
                    Nome = criterio.Nome;
                    DataInicioDe = criterio.DataInicioDe;
                    DataInicioAte = criterio.DataInicioAte;
                    DataFimDe = criterio.DataFimDe;
                    DataFimAte = criterio.DataFimAte;
                    Situacao = Situacoes.Where(d => d.Codigo == criterio.Situacao.GetValueOrDefault(4).ToString()).FirstOrDefault();



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
                DataFimAte = DataFimAte,
                DataFimDe = DataFimDe,
                DataInicioAte = DataInicioAte,
                DataInicioDe = DataInicioDe,
                Situacao = Convert.ToInt32(Situacao.Codigo),
                Nome = Nome
            };
            MessagingCenter.Send<HotelFiltroViewModel, CriterioBusca>(this, MessageKeys.FiltrarHotel, itemBusca);
            await NavigationService.TrocarPaginaShell("..");
        }
    }
}
