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
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Plugin.Calendar.Models;

namespace CV.Mobile.ViewModels.Consultas
{
    public class CalendarioRealizadoViewModel: BaseViewModel
    {
        private CriterioBusca criterioBusca = new CriterioBusca() { Situacao = 1 };
        private readonly IApiService _apiService;
        private EventCollection _dados = new EventCollection();
        private readonly ISettingsService _settingsService;
        private CultureInfo _cultura = CultureInfo.InvariantCulture;
        public CalendarioRealizadoViewModel(IApiService apiService, ISettingsService settingsService)
        {
            _apiService = apiService;
            criterioBusca.DataInicioDe = GlobalSetting.Instance.ViagemSelecionado.DataInicio;
            _settingsService = settingsService;
            Cultura = CultureInfo.CreateSpecificCulture("pt-BR");
        }

        public ICommand PageAppearingCommand => new Command(async () =>
        {
            if (Funcoes.AcessoInternet)
            {
                MessagingCenter.Unsubscribe<CalendarioRealizadoFiltroViewModel, CriterioBusca>(this, MessageKeys.FiltrarCalendarioRealizado);
                MessagingCenter.Subscribe<CalendarioRealizadoFiltroViewModel, CriterioBusca>(this, MessageKeys.FiltrarCalendarioRealizado,  (sender, obj) =>
                {

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
            await NavigationService.TrocarPaginaShell("CalendarioRealizadoFiltroPage", criterioBusca);

        });


        public CultureInfo Cultura
        {
            get { return _cultura; }
            set { SetProperty(ref _cultura, value); }
        }


        public EventCollection Eventos
        {
            get { return _dados; }
            set { SetProperty(ref _dados, value); }
        }

        private async Task CarregarLista()
        {
            IsBusy = true;
            try
            {
                IList<CalendarioRealizado> lista = await _apiService.ConsultarCalendarioRealizado(criterioBusca);
                EventCollection dados = new EventCollection();
                foreach(var item in lista.GroupBy(d => d.DataInicio.Date))
                {
                    dados.Add(item.Key, item.ToList());
                }
                Eventos = dados;

            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
