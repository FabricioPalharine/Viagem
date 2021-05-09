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
    public class LocaisViewModel: BaseViewModel
    {
        private CriterioBusca criterioBusca = new CriterioBusca() { Situacao = 1 };
        private readonly IApiService _apiService;
        private ObservableCollection<LocaisVisitados> _dados = new ObservableCollection<LocaisVisitados>();
        private readonly ISettingsService _settingsService;
        public LocaisViewModel(IApiService apiService, ISettingsService settingsService)
        {
            _apiService = apiService;
            criterioBusca.DataInicioDe = GlobalSetting.Instance.ViagemSelecionado.DataInicio;
            _settingsService = settingsService;
        }

        public ICommand PageAppearingCommand => new Command(async () =>
        {
            if (Funcoes.AcessoInternet)
            {
                MessagingCenter.Unsubscribe<LocaisFiltroViewModel, CriterioBusca>(this, MessageKeys.FiltrarLocais);
                MessagingCenter.Subscribe<LocaisFiltroViewModel, CriterioBusca>(this, MessageKeys.FiltrarLocais,  (sender, obj) =>
                {

                    criterioBusca.DataInicioDe = obj.DataInicioDe;
                    criterioBusca.DataInicioAte = obj.DataInicioAte;
                    //await CarregarLista();

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
            await NavigationService.TrocarPaginaShell("LocaisFiltroPage", criterioBusca);

        });

        public ICommand AbrirDetalheLocalCommand => new Command<LocaisVisitados>(async (d) =>
         {
             d.itemBusca = criterioBusca.Clone();
             await NavigationService.TrocarPaginaShell("LocaisDetalhePage", d);
         });


        public ObservableCollection<LocaisVisitados> Dados
        {
            get { return _dados; }
            set { SetProperty(ref _dados, value); }
        }

        private async Task CarregarLista()
        {
            IsBusy = true;
            try
            {
                IList<LocaisVisitados> lista = await _apiService.ListarLocaisVisitados(criterioBusca);
                Dados = new ObservableCollection<LocaisVisitados>(lista);

            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}

