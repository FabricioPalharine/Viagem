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
    public class ListagemGastoViewModel : BaseNavigationViewModel
    {
        private CriterioBusca _itemCriterioBusca;
        private bool _ModoPesquisa = false;

        private bool _IsLoadingLista;
        private Gasto _ItemSelecionado;


        public ListagemGastoViewModel(Viagem pitemViagem)
        {
            ItemViagem = pitemViagem;
            ItemCriterioBusca = new CriterioBusca() { Situacao=1, IdentificadorParticipante = ItemUsuarioLogado.Codigo};
            PageAppearingCommand = new Command(
                                                                   async () =>
                                                                   {
                                                                       await CarregarListaDados();
                                                                   },
                                                                   () => true);
            PesquisarCommand = new Command(
                                                                    async () => await VerificarPesquisa(),
                                                                    () => true);
            AdicionarCommand = new Command(
                                                                   async () => await Adicionar(),
                                                                   () => true);
            RecarregarListaCommand = new Command(
                                                      async () =>
                                                      {
                                                          await CarregarListaDados();
                                                      },
                                                      () => true);

            ItemTappedCommand = new Command<ItemTappedEventArgs>(
                                                                  async (obj) => await VerificarAcaoItem(obj));
              MessagingService.Current.Unsubscribe<Gasto>(MessageKeys.ManutencaoGasto);
            MessagingService.Current.Subscribe<Gasto>(MessageKeys.ManutencaoGasto, (service, item) =>
            {
                IsBusy = true;
                
                if (ListaDados.Where(d => d.Identificador == item.Identificador).Any())
                {
                    var Posicao = ListaDados.IndexOf(ListaDados.Where(d => d.Identificador == item.Identificador).FirstOrDefault());
                    ListaDados.RemoveAt(Posicao);
                    if (!item.DataExclusao.HasValue)
                        ListaDados.Insert(Posicao, item);
                }
                else if (!item.DataExclusao.HasValue)
                    ListaDados.Add(item);

                IsBusy = false;
            });
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
        public ObservableCollection<Cidade> ListaCidades { get; set; }
        public ObservableCollection<ItemLista> ListaStatus { get; set; }

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

      
      

        private async Task CarregarListaDados()
        {
            using (ApiService srv = new ApiService())
            {
                var Dados = await srv.ListarGasto(ItemCriterioBusca);
                ListaDados = new ObservableCollection<Gasto>(Dados);
                OnPropertyChanged("ListaDados");
            }
            IsLoadingLista = false;
        }

        private async Task VerificarAcaoItem(ItemTappedEventArgs itemSelecionado)
        {
            using (ApiService srv = new ApiService())
            {
                var ItemGasto = await srv.CarregarGasto(((Gasto)itemSelecionado.Item).Identificador);
                var Pagina = new EdicaoGastoPage() { BindingContext = new EdicaoGastoViewModel(ItemGasto) };
                await PushAsync(Pagina);
            }
        }
        private async Task Adicionar()
        {
            var ItemGasto = new Gasto()
            {
                ApenasBaixa = false,
                Data = DateTime.Today,
                Dividido = false,
                Especie = true,
                IdentificadorUsuario = ItemUsuarioLogado.Codigo,
                Moeda = ItemViagem.Moeda,
                Usuarios = new MvvmHelpers.ObservableRangeCollection<GastoDividido>(),
                Alugueis = new MvvmHelpers.ObservableRangeCollection<AluguelGasto>(),
                Hoteis = new MvvmHelpers.ObservableRangeCollection<GastoHotel>(),
                Compras = new MvvmHelpers.ObservableRangeCollection<GastoCompra>(),
                Atracoes = new MvvmHelpers.ObservableRangeCollection<GastoAtracao>(),
                Reabastecimentos = new MvvmHelpers.ObservableRangeCollection<ReabastecimentoGasto>(),
                Refeicoes = new MvvmHelpers.ObservableRangeCollection<GastoRefeicao>(),
                ViagenAereas = new MvvmHelpers.ObservableRangeCollection<GastoViagemAerea>()
            };
            using (ApiService srv = new ApiService())
            {
                var Pagina = new EdicaoGastoPage() { BindingContext = new EdicaoGastoViewModel(ItemGasto) };
                await PushAsync(Pagina);

            }
        }

    }
}
