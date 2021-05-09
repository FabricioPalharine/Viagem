using CV.Mobile.Models;
using CV.Mobile.Services.Api;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace CV.Mobile.ViewModels.Viagens
{
    public class ViagemListaViewModel : BaseViewModel 
    {
        private readonly IApiService _apiService;
        private CriterioBusca criterioBusca = new CriterioBusca();
        private ObservableCollection<Viagem> _viagens = new ObservableCollection<Viagem>();
        public ViagemListaViewModel(IApiService apiService)
        {
            _apiService = apiService;
        }

        public override async Task InitializeAsync(object navigationData)
        {
            MessagingCenter.Unsubscribe<ViagemFiltroViewModel, CriterioBusca>(this, MessageKeys.FiltrarViagem);
            MessagingCenter.Subscribe<ViagemFiltroViewModel, CriterioBusca>(this, MessageKeys.FiltrarViagem, async (sender, obj) =>
            {
                criterioBusca.Aberto = obj.Aberto;
                criterioBusca.IdentificadorParticipante = obj.IdentificadorParticipante;
                criterioBusca.DataFimAte = obj.DataFimAte;
                criterioBusca.DataFimDe = obj.DataFimDe;
                criterioBusca.DataInicioAte = obj.DataInicioAte;
                criterioBusca.DataInicioDe = obj.DataInicioDe;
                criterioBusca.Nome = obj.Nome;
                await CarregarLista();

            });
            await CarregarLista();
        }

        public ICommand RecarregarListaCommand => new Command(async () =>
        {
            await CarregarLista();

        });



        public ICommand FiltrarCommand => new Command(async () =>
         {
             await NavigationService.TrocarPaginaShell("ViagemFiltroPage", criterioBusca);

         });

       public ICommand SelecionarViagemCommand => new Command<Viagem>(async (d) =>
       {
           MessagingCenter.Send<ViagemListaViewModel, Viagem>(this, MessageKeys.SelecionarViagem, d);
           await NavigationService.TrocarPaginaShell("..");
           
       });

        public ObservableCollection<Viagem> Viagens
        {
            get { return _viagens; }
            set { SetProperty(ref _viagens, value); }
        }

        private async Task CarregarLista()
        {
            IsBusy = true;
            try
            {
                var viagens = await _apiService.ListarViagens(criterioBusca);
                Viagens = new ObservableCollection<Viagem>(viagens);

            }
            finally
            {
                IsBusy = false;
            }
            

        }
    }
}
