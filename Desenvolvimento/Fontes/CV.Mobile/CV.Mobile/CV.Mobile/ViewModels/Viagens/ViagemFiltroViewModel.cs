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

namespace CV.Mobile.ViewModels.Viagens
{
    public class ViagemFiltroViewModel: BaseViewModel
    {
        private ObservableCollection<ItemLista> _situacoes = new ObservableCollection<ItemLista>();
        private ObservableCollection<Usuario> _amigos = new ObservableCollection<Usuario>();
        private ItemLista _situacao = null;
        private Usuario _usuario = null;
        private DateTime? _dataInicioDe = null;
        private DateTime? _dataInicioAte = null;
        private DateTime? _dataFimDe = null;
        private DateTime? _dataFimAte = null;
        private string _nome = null;
        private readonly IApiService _apiService;

        public ViagemFiltroViewModel(ApiService apiService)
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

        public ObservableCollection<Usuario> Amigos
        {
            get { return _amigos; }
            set { SetProperty(ref _amigos, value); }
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

        public string Nome
        {
            get { return _nome; }
            set { _nome = value; }
        }

        public override async Task InitializeAsync(object navigationData)
        {
            IsBusy = true;
            try
            {
                Situacoes = new ObservableCollection<ItemLista>();
                Situacoes.Add(new ItemLista() { Codigo = "1", Descricao = "Aberta" });
                Situacoes.Add(new ItemLista() { Codigo = "2", Descricao = "Fechada" });
                Situacoes.Add(new ItemLista() { Codigo = "3", Descricao = "Todas" });
                Situacao = Situacoes.Last();
                var amigos = await _apiService.ListarAmigos();
                Amigos = new ObservableCollection<Usuario>(amigos);
                Amigos.Insert(0, new Usuario() { Identificador = null, Nome = "Todos" });
                if (navigationData != null && navigationData is CriterioBusca criterio)
                {
                    DataInicioDe = criterio.DataInicioDe;
                    DataInicioAte = criterio.DataInicioAte;
                    DataFimDe = criterio.DataFimDe;
                    DataFimAte = criterio.DataFimAte;
                    Nome = criterio.Nome;
                    if (criterio.Aberto.HasValue)
                    {
                        string Codigo = criterio.Aberto.Value ? "1" : "2";
                        Situacao = Situacoes.Where(d => d.Codigo == Codigo).FirstOrDefault();
                    }
                    if (criterio.IdentificadorParticipante.HasValue)
                    {
                        Usuario = amigos.Where(d => d.Identificador == criterio.IdentificadorParticipante).FirstOrDefault();
                    }                  


                }
            }
            finally
            {
                IsBusy = false;
            }
        }


        private async Task Filtrar()
        {
            CriterioBusca itemBusca = new CriterioBusca()
            {
                DataFimAte = DataFimAte,
                DataFimDe = DataFimDe,
                DataInicioAte = DataInicioAte,
                DataInicioDe = DataInicioDe,
                IdentificadorParticipante = Usuario != null ? Usuario.Identificador : new Nullable<int>(),
                Aberto = Situacao != null && Situacao.Codigo != "3" ? Situacao.Codigo == "1" : new Nullable<bool>(),
                Nome=Nome
            };
            MessagingCenter.Send<ViagemFiltroViewModel, CriterioBusca>(this, MessageKeys.FiltrarViagem, itemBusca);
            await NavigationService.TrocarPaginaShell("..");
        }
    }
}
