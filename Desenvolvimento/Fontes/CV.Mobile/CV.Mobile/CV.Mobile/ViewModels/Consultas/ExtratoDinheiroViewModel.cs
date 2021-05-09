using CV.Mobile.Enums;
using CV.Mobile.Helper;
using CV.Mobile.Models;
using CV.Mobile.Resources;
using CV.Mobile.Services.Api;
using CV.Mobile.Services.Data;
using CV.Mobile.Services.Settings;
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
    public class ExtratoDinheiroViewModel: BaseViewModel
    {

        private CriterioBusca criterioBusca = new CriterioBusca() { DataInicioDe = GlobalSetting.Instance.ViagemSelecionado.DataInicio, Moeda = GlobalSetting.Instance.ViagemSelecionado.Moeda };
        private readonly IDatabase _database;
        private ObservableCollection<ExtratoMoeda> _dados = new ObservableCollection<ExtratoMoeda>();
        private readonly ISettingsService _settingsService;
        public ExtratoDinheiroViewModel( IDatabase database, ISettingsService settingsService)
        {
            _database = database;
            criterioBusca.DataInicioDe = GlobalSetting.Instance.ViagemSelecionado.DataInicio;
            criterioBusca.Moeda = GlobalSetting.Instance.ViagemSelecionado.Moeda;
            _settingsService = settingsService;
        }

        public ICommand PageAppearingCommand => new Command(async () =>
        {
            MessagingCenter.Unsubscribe<ExtratoDinheiroFiltroViewModel, CriterioBusca>(this, MessageKeys.FiltrarExtratoDinheiro);
            MessagingCenter.Subscribe<ExtratoDinheiroFiltroViewModel, CriterioBusca>(this, MessageKeys.FiltrarExtratoDinheiro,  (sender, obj) =>
            {
                criterioBusca.Moeda = obj.Moeda;
                criterioBusca.DataInicioDe = obj.DataInicioDe;

            });
           
            await CarregarLista();
        });

        public ICommand RecarregarListaCommand => new Command(async () =>
        {
            await CarregarLista();

        });



        public ICommand FiltrarCommand => new Command(async () =>
        {
            await NavigationService.TrocarPaginaShell("ExtratoDinheiroFiltroPage", criterioBusca);

        });


        public ObservableCollection<ExtratoMoeda> Dados
        {
            get { return _dados; }
            set { SetProperty(ref _dados, value); }
        }

        private async Task CarregarLista()
        {
            IsBusy = true;
            try
            {
                IList<ExtratoMoeda> lista = await _database.ConsultarExtratoMoeda(GlobalSetting.Instance.UsuarioLogado.Codigo, GlobalSetting.Instance.ViagemSelecionado.Identificador,
                    criterioBusca.Moeda, criterioBusca.DataInicioDe.GetValueOrDefault());
               
                Dados = new ObservableCollection<ExtratoMoeda>(lista);

            }
            finally
            {
                IsBusy = false;
            }


        }

    }
}
