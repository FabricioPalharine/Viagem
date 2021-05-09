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
    public class AcertoContaViewModel : BaseViewModel
    {
        private CriterioBusca criterioBusca = new CriterioBusca() {Situacao=1};
        private readonly IApiService _apiService;
        private ObservableCollection<AjusteGastoDividido> _dados = new ObservableCollection<AjusteGastoDividido>();
        private readonly ISettingsService _settingsService;
        public AcertoContaViewModel(IApiService apiService, ISettingsService settingsService)
        {
            _apiService = apiService;
            criterioBusca.DataInicioDe = GlobalSetting.Instance.ViagemSelecionado.DataInicio;
            _settingsService = settingsService;
        }

        public ICommand PageAppearingCommand => new Command(async () =>
        {
            if (Funcoes.AcessoInternet)
            {
                MessagingCenter.Unsubscribe<AcertoContaFiltroViewModel, CriterioBusca>(this, MessageKeys.FiltrarAcertoConta);
                MessagingCenter.Subscribe<AcertoContaFiltroViewModel, CriterioBusca>(this, MessageKeys.FiltrarAcertoConta,  (sender, obj) =>
                {
                   
                    criterioBusca.DataInicioDe = obj.DataInicioDe;
                    criterioBusca.DataInicioAte = obj.DataInicioAte;

                });

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
            await NavigationService.TrocarPaginaShell("AcertoContaFiltroPage", criterioBusca);

        });


        public ObservableCollection<AjusteGastoDividido> Dados
        {
            get { return _dados; }
            set { SetProperty(ref _dados, value); }
        }

        private async Task CarregarLista()
        {
            IsBusy = true;
            try
            {
                IList<AjusteGastoDividido> lista = await _apiService.ListarAjusteGastos(criterioBusca);
                Dados = new ObservableCollection<AjusteGastoDividido>(lista);

            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
