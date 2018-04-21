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
    public class ListagemPedidoCompraViewModel: BaseNavigationViewModel
    {
        private CriterioBusca _itemCriterioBusca;
        private bool _ModoPesquisa = false;

        private bool _IsLoadingLista;
        private ListaCompra _PedidoCompraSelecionado;


        public ListagemPedidoCompraViewModel(Viagem pitemViagem)
        {
            ItemViagem = pitemViagem;
            ItemCriterioBusca = new CriterioBusca();
            PageAppearingCommand = new Command(
                                                                   async () =>
                                                                   {
                                                                       await CarregarListaAmigos();
                                                                       await CarregarListaPedidos();
                                                                   },
                                                                   () => true);
            PesquisarCommand = new Command(
                                                                    async () => await VerificarPesquisa(),
                                                                    () => true);
            RecarregarListaCommand = new Command(
                                                      async () =>
                                                      {
                                                          await CarregarListaPedidos();
                                                      },
                                                      () => true);

            ExcluirCommand = new Command<ListaCompra>((itemCotacao) => Excluir(itemCotacao));
            EditarCommand = new Command<ItemTappedEventArgs>(async (itemCotacao) => await Editar(itemCotacao));
            AdicionarCommand = new Command(async () => await AbrirInclusao(), () => true);
            MessagingService.Current.Unsubscribe<ListaCompra>(MessageKeys.ManutencaoPedidoCompra);
            MessagingService.Current.Subscribe<ListaCompra>(MessageKeys.ManutencaoPedidoCompra, (service, cotacao) =>
            {
                IsBusy = true;

                if (PedidosCompra.Where(d => d.Identificador == cotacao.Identificador).Any())
                {
                    var Posicao = PedidosCompra.IndexOf(PedidosCompra.Where(d => d.Identificador == cotacao.Identificador).FirstOrDefault());
                    PedidosCompra.RemoveAt(Posicao);
                    PedidosCompra.Insert(Posicao, cotacao);
                }
                else
                    PedidosCompra.Add(cotacao);

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


        public ObservableCollection<ListaCompra> PedidosCompra { get; set; }
        public Command AdicionarCommand { get; set; }
        public Command EditarCommand { get; set; }
        public Command ExcluirCommand { get; set; }
        public Command PageAppearingCommand { get; set; }
        public Command RecarregarListaCommand { get; set; }
        public Command PesquisarCommand { get; set; }

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

        public ListaCompra PedidoSelecionado
        {
            get
            {
                return _PedidoCompraSelecionado;
            }

            set
            {
                _PedidoCompraSelecionado = null;
                OnPropertyChanged();
            }
        }


        private async Task VerificarPesquisa()
        {
            if (ModoPesquisa)
            {
                if (PesquisarCommand.CanExecute(null))
                    PesquisarCommand.ChangeCanExecute();
                await CarregarListaPedidos();
                PesquisarCommand.ChangeCanExecute();
            }
            ModoPesquisa = !ModoPesquisa;

        }

        private async Task CarregarListaAmigos()
        {
            using (ApiService srv = new ApiService())
            {
                var Dados = await srv.ListarParticipantesViagem();
                
                ListaAmigos = new ObservableCollection<Usuario>(Dados);
                OnPropertyChanged("ListaAmigos");
            }
        }


        private async Task CarregarListaPedidos()
        {
            try
            {
                using (ApiService srv = new ApiService())
                {
                    var Dados = await srv.ListarListaCompra(ItemCriterioBusca);
                    PedidosCompra = new ObservableCollection<ListaCompra>(Dados);
                    OnPropertyChanged("PedidosCompra");
                }
            }
            catch { ApiService.ExibirMensagemErro(); }
            IsLoadingLista = false;
        }

        private void Excluir(ListaCompra itemListaCompra)
        {
            MessagingService.Current.SendMessage<MessagingServiceQuestion>(MessageKeys.DisplayQuestion, new MessagingServiceQuestion()
            {
                Title = "Confirmação",
                Question = String.Format("Excluir o pedido de compra do item {0} da marca {1}", itemListaCompra.Descricao, itemListaCompra.Marca),
                Positive = "Confirmar",
                Negative = "Cancelar",
                OnCompleted = new Action<bool>(async result =>
                {
                    if (!result) return;
                    try
                    {
                        using (ApiService srv = new ApiService())
                        {
                            var Resultado = await srv.ExcluirListaCompra(itemListaCompra.Identificador);
                            base.AtualizarViagem(ItemViagemSelecionada.Identificador.GetValueOrDefault(), "LC", itemListaCompra.Identificador.GetValueOrDefault(), false);

                            MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.DisplayAlert, new MessagingServiceAlert()
                            {
                                Title = "Sucesso",
                                Message = String.Join(Environment.NewLine, Resultado.Mensagens.Select(d => d.Mensagem).ToArray()),
                                Cancel = "OK"
                            });
                            if (PedidosCompra.Where(d => d.Identificador == itemListaCompra.Identificador).Any())
                            {
                                var Posicao = PedidosCompra.IndexOf(PedidosCompra.Where(d => d.Identificador == itemListaCompra.Identificador).FirstOrDefault());
                                PedidosCompra.RemoveAt(Posicao);
                            }
                        }
                    }
                    catch
                    {
                        ApiService.ExibirMensagemErro();
                    }

                })
            });
        }

        private async Task Editar(ItemTappedEventArgs itemLista)
        {
            using (ApiService srv = new ApiService())
            {
                try
                {
                    var itemEditar = await srv.CarregarListaCompra(((ListaCompra)itemLista.Item).Identificador);
                    EdicaoPedidoCompraPage pagina = new EdicaoPedidoCompraPage() { BindingContext = new EdicaoPedidoCompraViewModel(itemEditar, ListaAmigos) };
                    await PushAsync(pagina);
                }
                catch
                {
                    ApiService.ExibirMensagemErro();
                }
            }
        }

        private async Task AbrirInclusao()
        {
            var itemEditar = new ListaCompra() { IdentificadorUsuario = ItemUsuarioLogado.Codigo, Status = (int) enumStatusListaCompra.NaoVisto, Moeda = ItemViagem.Moeda, Reembolsavel=true};
            EdicaoPedidoCompraPage pagina = new EdicaoPedidoCompraPage() { BindingContext = new EdicaoPedidoCompraViewModel(itemEditar, ListaAmigos) };
            await PushAsync(pagina);

        }
    }
}
