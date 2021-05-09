using CV.Mobile.Enums;
using CV.Mobile.Helper;
using CV.Mobile.Models;
using CV.Mobile.Resources;
using CV.Mobile.Services.Api;
using CV.Mobile.Services.Data;
using CV.Mobile.Services.Fotos;
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
    public class GaleriaFotoViewModel: BaseViewModel
    {
        private CriterioBusca criterioBusca = new CriterioBusca() { Count = 500000, Index = 0 };
        private readonly IApiService _apiService;
        private ObservableCollection<Foto> _dados = new ObservableCollection<Foto>();
        private readonly ISettingsService _settingsService;
        private readonly IFoto _foto;
        public GaleriaFotoViewModel(IApiService apiService, ISettingsService settingsService, IFoto foto)
        {
            _apiService = apiService;
            _foto = foto;
            criterioBusca.DataInicioDe = GlobalSetting.Instance.ViagemSelecionado.DataInicio;
            _settingsService = settingsService;
        }

        public ICommand PageAppearingCommand => new Command(async () =>
        {
            if (Funcoes.AcessoInternet)
            {
                MessagingCenter.Unsubscribe<GaleriaFotoViewModel, CriterioBusca>(this, MessageKeys.FiltrarFotos);
                MessagingCenter.Subscribe<GaleriaFotoViewModel, CriterioBusca>(this, MessageKeys.FiltrarFotos,  (sender, obj) =>
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
            await NavigationService.TrocarPaginaShell("GaleriaFotoFiltroPage", criterioBusca);

        });


        public ICommand AbrirImagemCommand => new Command<Foto>(async (d) =>
        {
            await NavigationService.TrocarPaginaShell("ConsultaFotoDetalhePage", d);
        });

        public ObservableCollection<Foto> Dados
        {
            get { return _dados; }
            set { SetProperty(ref _dados, value); }
        }

        private async Task CarregarLista()
        {
            IsBusy = true;
            try
            {
                IList<Foto> lista = await _apiService.ListarFoto(criterioBusca);
                await _foto.UpdateMediaData(lista.ToList());
                Dados = new ObservableCollection<Foto>(lista);

            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
