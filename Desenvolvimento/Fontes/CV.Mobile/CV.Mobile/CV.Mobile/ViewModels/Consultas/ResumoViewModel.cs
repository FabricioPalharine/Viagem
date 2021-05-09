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
    public class ResumoViewModel: BaseViewModel
    {
        private CriterioBusca criterioBusca = new CriterioBusca() { Situacao = 1 };
        private readonly IApiService _apiService;
        private ResumoViagem _itemResumo = new ResumoViagem();
        private readonly ISettingsService _settingsService;
        private bool _verGastos = false;

        public ResumoViewModel(IApiService apiService, ISettingsService settingsService)
        {
            _apiService = apiService;
            criterioBusca.DataInicioDe = GlobalSetting.Instance.ViagemSelecionado.DataInicio;
            _settingsService = settingsService;
        }

        public ICommand PageAppearingCommand => new Command(async () =>
        {
            if (Funcoes.AcessoInternet)
            {
                MessagingCenter.Unsubscribe<ResumoFiltroViewModel, CriterioBusca>(this, MessageKeys.FiltrarResumo);
                MessagingCenter.Subscribe<ResumoFiltroViewModel, CriterioBusca>(this, MessageKeys.FiltrarResumo,  (sender, obj) =>
                {
                    criterioBusca.DataInicioDe = obj.DataInicioDe;
                    criterioBusca.DataInicioAte = obj.DataInicioAte;
                    criterioBusca.IdentificadorParticipante = obj.IdentificadorParticipante;
                    //await CarregarLista();

                });
                if (!criterioBusca.IdentificadorParticipante.HasValue)
                {
                    ObservableCollection<Usuario> Usuarios = new ObservableCollection<Usuario>();
                    if (GlobalSetting.Instance.AmigosViagem == null)
                        GlobalSetting.Instance.AmigosViagem = Usuarios = new ObservableCollection<Usuario>(await _apiService.CarregarParticipantesAmigo());
                    else
                        Usuarios = new ObservableCollection<Usuario>(GlobalSetting.Instance.AmigosViagem);
                    if (Usuarios.Where(d => d.Identificador == GlobalSetting.Instance.UsuarioLogado.Codigo).Any())
                        criterioBusca.IdentificadorParticipante = GlobalSetting.Instance.UsuarioLogado.Codigo;
                    else
                        criterioBusca.IdentificadorParticipante = Usuarios.Select(d => d.Identificador).FirstOrDefault();
                }
                VerGastos = GlobalSetting.Instance.ViagemSelecionado.VejoGastos;
                await CarregarLista();
            }
            else
            {
                await DialogService.ShowAlertAsync(AppResource.AplicacaoOnlineNecessaria, AppResource.AppName, AppResource.Ok);
                await NavigationService.TrocarPaginaShell("..");
            }
        });

        public ICommand RecarregarListaCommand => new Command(async () =>
        {
            await CarregarLista();

        });



        public ICommand FiltrarCommand => new Command(async () =>
        {
            await NavigationService.TrocarPaginaShell("ResumoFiltroPage", criterioBusca);

        });


        public ResumoViagem ItemResumo
        {
            get { return _itemResumo; }
            set { SetProperty(ref _itemResumo, value); }
        }

        public bool VerGastos
        {
            get { return _verGastos; }
            set { SetProperty(ref _verGastos, value); }
        }

        private async Task CarregarLista()
        {
            IsBusy = true;
            try
            {
                IList<AjusteGastoDividido> lista = await _apiService.ListarAjusteGastos(criterioBusca);
                ItemResumo = await _apiService.CarregarResumo(criterioBusca);

            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
