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


namespace CV.Mobile.ViewModels.Comentarios
{
    public class ComentarioFiltroViewModel: BaseViewModel
    {
        private DateTime? _dataInicioDe = null;
        private DateTime? _dataInicioAte = null;
        private readonly IApiService _apiService;

        public ComentarioFiltroViewModel(ApiService apiService)
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



        public override async Task InitializeAsync(object navigationData)
        {
            IsBusy = true;
            try
            {

                if (navigationData != null && navigationData is CriterioBusca criterio)
                {
                    DataInicioDe = criterio.DataInicioDe;
                    DataInicioAte = criterio.DataInicioAte;
                }
            }
            finally
            {
                IsBusy = false;
            }
            await base.InitializeAsync(navigationData);
        }


        private async Task Filtrar()
        {
            CriterioBusca itemBusca = new CriterioBusca()
            {
                DataInicioAte = DataInicioAte,
                DataInicioDe = DataInicioDe,
            };
            MessagingCenter.Send<ComentarioFiltroViewModel, CriterioBusca>(this, MessageKeys.FiltrarComentario, itemBusca);
            await NavigationService.TrocarPaginaShell("..");
        }
    }
}
