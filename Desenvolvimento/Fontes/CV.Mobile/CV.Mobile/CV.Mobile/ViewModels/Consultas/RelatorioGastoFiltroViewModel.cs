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
    public class RelatorioGastoFiltroViewModel: BaseViewModel
    {
        private ObservableCollection<ItemLista> _tipos = new ObservableCollection<ItemLista>();
        private ItemLista _tipo = null;
        private DateTime? _dataInicioDe = null;
        private DateTime? _dataInicioAte = null;
        private readonly IApiService _apiService;
        private ObservableCollection<Usuario> _usuarios = new ObservableCollection<Usuario>();
        private Usuario _participante = null;

        public RelatorioGastoFiltroViewModel(ApiService apiService)
        {
            _apiService = apiService;
            Tipos.Add(new ItemLista() { Codigo = "", Descricao = "Todos" });
            Tipos.Add(new ItemLista() { Codigo = "A", Descricao = "Atração" });
            Tipos.Add(new ItemLista() { Codigo = "VA", Descricao = "Deslocamentos" });
            Tipos.Add(new ItemLista() { Codigo = "H", Descricao = "Hospedagem" });
            Tipos.Add(new ItemLista() { Codigo = "R", Descricao = "Refeições" });
        }

        public ICommand VoltarCommand => new Command(async () => await NavigationService.TrocarPaginaShell(".."));
        public ICommand FiltrarCommand => new Command(async () => await Filtrar());


        public ObservableCollection<ItemLista> Tipos
        {
            get { return _tipos; }
            set { SetProperty(ref _tipos, value); }
        }


        public ItemLista Tipo
        {
            get { return _tipo; }
            set { SetProperty(ref _tipo, value); }
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


        public ObservableCollection<Usuario> Usuarios
        {
            get { return _usuarios; }
            set { SetProperty(ref _usuarios, value); }
        }


        public Usuario Participante
        {
            get { return _participante; }
            set { SetProperty(ref _participante, value); }
        }

        public async override Task InitializeAsync(object navigationData)
        {
            IsBusy = true;
            try
            {
                if (GlobalSetting.Instance.AmigosViagem == null)
                    GlobalSetting.Instance.AmigosViagem = Usuarios = new ObservableRangeCollection<Usuario>(await _apiService.CarregarParticipantesAmigo());
                else
                    Usuarios = new ObservableCollection<Usuario>(GlobalSetting.Instance.AmigosViagem);

                if (navigationData != null && navigationData is CriterioBusca criterio)
                {
                    DataInicioDe = criterio.DataInicioDe;
                    DataInicioAte = criterio.DataInicioAte;
                    if (criterio.Tipo != null)
                        Tipo = _tipos.Where(d => d.Codigo == criterio.Tipo).FirstOrDefault();
                    if (criterio.IdentificadorParticipante.HasValue)
                        Participante = _usuarios.Where(d => d.Identificador == criterio.IdentificadorParticipante).FirstOrDefault();

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
                DataInicioAte = DataInicioAte,
                DataInicioDe = DataInicioDe,
                Tipo = Tipo == null ? null : Tipo.Codigo,
                IdentificadorParticipante = Participante == null ? new Nullable<int>() : Convert.ToInt32(Participante.Identificador)
            };
            MessagingCenter.Instance.Send<RelatorioGastoFiltroViewModel, CriterioBusca>(this, MessageKeys.FiltrarRelatorioGasto, itemBusca);
            await NavigationService.TrocarPaginaShell("..");
        }
    }
}
