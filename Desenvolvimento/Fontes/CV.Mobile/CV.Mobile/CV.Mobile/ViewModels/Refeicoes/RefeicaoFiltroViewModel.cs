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

namespace CV.Mobile.ViewModels.Refeicoes
{
    public class RefeicaoFiltroViewModel:BaseViewModel
    {
        private DateTime? _dataInicioDe = null;
        private DateTime? _dataInicioAte = null;
        private string _nome = null;
        private string _tipo = null;

        private readonly IApiService _apiService;

        public RefeicaoFiltroViewModel(ApiService apiService)
        {
            _apiService = apiService;
        }

        public ICommand VoltarCommand => new Command(async () => await NavigationService.TrocarPaginaShell(".."));
        public ICommand FiltrarCommand => new Command(async () => await Filtrar());




        public string Nome
        {
            get { return _nome; }
            set { SetProperty(ref _nome, value); }
        }
        public string Tipo
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



        public override Task InitializeAsync(object navigationData)
        {
            IsBusy = true;
            try
            {

                if (navigationData != null && navigationData is CriterioBusca criterio)
                {
                    Nome = criterio.Nome;
                    DataInicioDe = criterio.DataInicioDe;
                    DataInicioAte = criterio.DataInicioAte;
                    Tipo = criterio.Tipo;


                }
            }
            finally
            {
                IsBusy = false;
            }
            return Task.FromResult(true);
        }


        private async Task Filtrar()
        {
            CriterioBusca itemBusca = new CriterioBusca()
            {

                DataInicioAte = DataInicioAte,
                DataInicioDe = DataInicioDe,
                Tipo = Tipo,
                Nome = Nome
            };
            MessagingCenter.Send<RefeicaoFiltroViewModel, CriterioBusca>(this, MessageKeys.FiltrarRestaurante, itemBusca);
            await NavigationService.TrocarPaginaShell("..");
        }
    }
}
