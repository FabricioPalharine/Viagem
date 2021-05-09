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

namespace CV.Mobile.ViewModels.Deslocamentos
{
    public class DeslocamentoFiltroViewModel: BaseViewModel
    {
        private ObservableCollection<ItemLista> _situacoes = new ObservableCollection<ItemLista>();
        private ItemLista _situacao = null;
        private Usuario _usuario = null;
        private DateTime? _dataInicioDe = null;
        private DateTime? _dataInicioAte = null;
        private DateTime? _dataFimDe = null;
        private DateTime? _dataFimAte = null;
        private string _nome = null;
        private ObservableCollection<ItemLista> _tipos = new ObservableCollection<ItemLista>();
        private ItemLista _tipo = null;

        private readonly IApiService _apiService;

        public DeslocamentoFiltroViewModel(ApiService apiService)
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

        public ObservableCollection<ItemLista> Tipos
        {
            get { return _tipos; }
            set { SetProperty(ref _tipos, value); }
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


        public ItemLista Tipo
        {
            get { return _tipo; }
            set { SetProperty(ref _tipo, value); }
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

                Tipos = Funcoes.RetornarTiposDeslocamento();
                Tipos.Insert(0, new ItemLista() { Codigo = null, Descricao = "Todos" });
                Situacoes = new ObservableCollection<ItemLista>();
                Situacoes.Add(new ItemLista() { Codigo = "1", Descricao = "Viajando" });
                Situacoes.Add(new ItemLista() { Codigo = "2", Descricao = "Terminada" });
                Situacoes.Add(new ItemLista() { Codigo = "3", Descricao = "Futura" });
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
                    Tipo = Tipos.Where(d => d.Codigo == Convert.ToString(criterio.TipoInteiro)).FirstOrDefault();


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
                Nome = Nome,
                TipoInteiro=Tipo != null && !string.IsNullOrEmpty(Tipo.Codigo)?Convert.ToInt32(Tipo.Codigo):new Nullable<int>()
            };
            MessagingCenter.Send<DeslocamentoFiltroViewModel, CriterioBusca>(this, MessageKeys.FiltrarDeslocamento, itemBusca);
            await NavigationService.TrocarPaginaShell("..");
        }
    }
}
