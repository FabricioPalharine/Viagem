using CV.Mobile.Models;
using CV.Mobile.Services;
using CV.Mobile.Views;
using CV.Mobile.Helpers;
using FormsToolkit;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CV.Mobile.ViewModels
{
    public class ListagemSelecaoCustoViewModel : BaseNavigationViewModel
    {
        private CriterioBusca _itemCriterioBusca;
        private bool _ModoPesquisa = true;

        private bool _IsLoadingLista;
        private Gasto _ItemSelecionado;


        public ListagemSelecaoCustoViewModel(Viagem pitemViagem)
        {
            ItemViagem = pitemViagem;
            ItemCriterioBusca = new CriterioBusca() { };
            PageAppearingCommand = new Command(
                                                                   async () =>
                                                                   {
                                                                       await CarregarListaAmigos();
                                                                       await CarregarListaDados();
                                                                   },
                                                                   () => true);
            PesquisarCommand = new Command(
                                                                    async () => await VerificarPesquisa(),
                                                                    () => true);
                RecarregarListaCommand = new Command(
                                                      async () =>
                                                      {
                                                          await CarregarListaDados();
                                                      },
                                                      () => true);

            ItemTappedCommand = new Command<ItemTappedEventArgs>(
                                                                  async (obj) => await VerificarAcaoItem(obj));
       

           
        }

        public CriterioBusca ItemCriterioBusca
        {
            get
            {
                return _itemCriterioBusca;
            }

            set
            {
                SetProperty(ref _itemCriterioBusca, value);
            }
        }

        public Viagem ItemViagem { get; set; }
        public ObservableCollection<Usuario> ListaAmigos { get; set; }

        public bool ModoPesquisa
        {
            get
            {
                return _ModoPesquisa;
            }

            set
            {
                SetProperty(ref _ModoPesquisa, value);
            }
        }


        public ObservableCollection<Gasto> ListaDados { get; set; }
        public Command PageAppearingCommand { get; set; }
        public Command RecarregarListaCommand { get; set; }
        public Command PesquisarCommand { get; set; }
        public Command ItemTappedCommand { get; set; }
        public Command AdicionarCommand { get; set; }

        public bool IsLoadingLista
        {
            get
            {
                return _IsLoadingLista;
            }

            set
            {
                SetProperty(ref _IsLoadingLista, value);
            }
        }

        public Gasto ItemSelecionado
        {
            get
            {
                return _ItemSelecionado;
            }

            set
            {
                _ItemSelecionado = null;
                OnPropertyChanged();
            }
        }


        private async Task VerificarPesquisa()
        {
            if (ModoPesquisa)
            {
                if (PesquisarCommand.CanExecute(null))
                    PesquisarCommand.ChangeCanExecute();
                await CarregarListaDados();
                PesquisarCommand.ChangeCanExecute();
            }
            ModoPesquisa = !ModoPesquisa;

        }

        private async Task CarregarListaAmigos()
        {
            List<Usuario> Dados = new List<Usuario>();
            if (Conectado)
            {
                using (ApiService srv = new ApiService())
                {
                    Dados = await srv.ListarParticipantesViagem();
                    
                }
            }
            else
                Dados = await DatabaseService.Database.ListarParticipanteViagem();
            ListaAmigos = new ObservableCollection<Usuario>(Dados);
            OnPropertyChanged("ListaAmigos");
        }
      

        private async Task CarregarListaDados()
        {
            List<Gasto> Dados = new List<Gasto>();
            if (Conectado)
            {
                using (ApiService srv = new ApiService())
                {
                    Dados = await srv.ListarGasto(ItemCriterioBusca);

                }
            }
            else
                Dados = await DatabaseService.Database.ListarGasto(ItemCriterioBusca);

            ListaDados = new ObservableCollection<Gasto>(Dados);
            OnPropertyChanged("ListaDados");
            IsLoadingLista = false;
        }

        private async Task VerificarAcaoItem(ItemTappedEventArgs itemSelecionado)
        {
            MessagingService.Current.SendMessage<Gasto>(MessageKeys.GastoSelecionado, (Gasto)itemSelecionado.Item);

            await PopAsync();

        }
     
    }
}
