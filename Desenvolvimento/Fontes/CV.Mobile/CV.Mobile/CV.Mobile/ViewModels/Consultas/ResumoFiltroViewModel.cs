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
    public class ResumoFiltroViewModel : BaseViewModel
    {

        private DateTime? _dataInicioDe = null;
        private DateTime? _dataInicioAte = null;
        private readonly IApiService _apiService;
        private ObservableCollection<Usuario> _usuarios = new ObservableCollection<Usuario>();
        private Usuario _participante = null;

        public ResumoFiltroViewModel(ApiService apiService)
        {
            _apiService = apiService;

        }

        public ICommand VoltarCommand => new Command(async () => await NavigationService.TrocarPaginaShell(".."));
        public ICommand FiltrarCommand => new Command(async () => await Filtrar());





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
                IdentificadorParticipante = Participante == null ? new Nullable<int>() : Convert.ToInt32(Participante.Identificador)
            };
            MessagingCenter.Send<ResumoFiltroViewModel, CriterioBusca>(this, MessageKeys.FiltrarResumo, itemBusca);
            await NavigationService.TrocarPaginaShell("..");
        }
    }

}


