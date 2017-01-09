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
    public class ListagemListaCompraViewModel : BaseNavigationViewModel
    {
        private CriterioBusca _itemCriterioBusca;
        private CriterioBusca _itemCriterioBuscaPedido;

        private bool _ModoPesquisa = false;
        private bool _ModoPesquisaPedido = false;

        private bool _IsLoadingLista;
        private bool _IsLoadingPedido;

        private ListaCompra _ItemCompraSelecionado;
        private ListaCompra _PedidoCompraSelecionado;



        public ListagemListaCompraViewModel(Viagem pitemViagem)
        {
            ItemViagem = pitemViagem;
            ItemCriterioBusca = new CriterioBusca() { Situacao=1 } ;
            ItemCriterioBuscaPedido = new CriterioBusca() { Situacao = 1 };
            PageAppearingCommand = new Command(
                                                                   async () =>
                                                                   {                                                                       
                                                                       await CarregarListaRequisicao();
                                                                       await CarregarListaAmigos();
                                                                       await CarregarListaPedidos();
                                                                   },
                                                                   () => true);
            PesquisarCommand = new Command(
                                                                    async () => await VerificarPesquisa(),
                                                                    () => true);

            ItemTappedCommand   = new Command<ItemTappedEventArgs>(
                                                                    async (obj) => await VerificarAcaoItem(obj));
            PesquisarPedidoCompraCommand = new Command(
                                                        async () => await VerificarPesquisaRequisicao(),
                                                        () => true);
            RecarregarListaCommand = new Command(
                                                      async () =>
                                                      {
                                                          await CarregarListaPedidos();
                                                      },
                                                      () => true);

            AtualizarPedidosCommand = new Command(
                                                      async () =>
                                                      {
                                                          await CarregarListaRequisicao();
                                                      },
                                                      () => true);

            ExcluirCommand = new Command<ListaCompra>((itemCotacao) => Excluir(itemCotacao));
            EditarCommand = new Command<ItemTappedEventArgs>(async (itemCotacao) => await Editar(itemCotacao));
            AdicionarCommand = new Command(async () => await AbrirInclusao(), () => true);
            ListaStatus = new ObservableCollection<ItemLista>();
            ListaStatus.Add(new ItemLista() { Codigo = "3", Descricao = "Comprado" });
            ListaStatus.Add(new ItemLista() { Codigo = "1", Descricao = "Pendente" });
            ListaStatus.Add(new ItemLista() { Codigo = "-1", Descricao = "Todos" });

            ListaStatusPedido = new ObservableCollection<ItemLista>();
            ListaStatusPedido.Add(new ItemLista() { Codigo = "3", Descricao = "Comprado" });
            ListaStatusPedido.Add(new ItemLista() { Codigo = "1", Descricao = "Pendente" });
            ListaStatusPedido.Add(new ItemLista() { Codigo = "2", Descricao = "Ignorado" });
            ListaStatusPedido.Add(new ItemLista() { Codigo = "-1", Descricao = "Todos" });

            MessagingService.Current.Unsubscribe<ListaCompra>(MessageKeys.ManutencaoListaCompra);
            MessagingService.Current.Unsubscribe<ListaCompra>(MessageKeys.ManutencaoRequisicaoPedidoCompra);
            MessagingService.Current.Subscribe<ListaCompra>(MessageKeys.ManutencaoListaCompra, (service, cotacao) =>
            {
                IsBusy = true;

                if (ListaCompra.Where(d => d.Identificador == cotacao.Identificador).Any())
                {
                    var Posicao = ListaCompra.IndexOf(ListaCompra.Where(d => d.Identificador == cotacao.Identificador).FirstOrDefault());
                    ListaCompra.RemoveAt(Posicao);
                    ListaCompra.Insert(Posicao, cotacao);
                }
                else
                    ListaCompra.Add(cotacao);

                IsBusy = false;
            });

            MessagingService.Current.Subscribe<ListaCompra>(MessageKeys.ManutencaoRequisicaoPedidoCompra, (service, cotacao) =>
            {
                IsBusy = true;

                if (PedidosCompra.Where(d => d.Identificador == cotacao.Identificador).Any())
                {
                    var Posicao = PedidosCompra.IndexOf(ListaCompra.Where(d => d.Identificador == cotacao.Identificador).FirstOrDefault());
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

        public CriterioBusca ItemCriterioBuscaPedido
        {
            get
            {
                return _itemCriterioBuscaPedido;
            }

            set
            {
                SetProperty(ref _itemCriterioBuscaPedido, value);
            }
        }

        public Viagem ItemViagem { get; set; }
        public ObservableCollection<Usuario> ListaAmigos { get; set; }
        public ObservableCollection<ItemLista> ListaStatus { get; set; }
        public ObservableCollection<ItemLista> ListaStatusPedido { get; set; }

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


        public ObservableCollection<ListaCompra> ListaCompra { get; set; }
        public ObservableCollection<ListaCompra> PedidosCompra { get; set; }

        public Command AdicionarCommand { get; set; }
        public Command EditarCommand { get; set; }
        public Command ExcluirCommand { get; set; }
        public Command PageAppearingCommand { get; set; }
        public Command RecarregarListaCommand { get; set; }
        public Command PesquisarCommand { get; set; }
        public Command AtualizarPedidosCommand { get; set; }
        public Command ItemTappedCommand { get; set; }
        public Command PesquisarPedidoCompraCommand { get; set; }
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
                SetProperty(ref _PedidoCompraSelecionado, null);
            }
        }

        public ListaCompra ItemCompraSelecionado
        {
            get
            {
                return _ItemCompraSelecionado;
            }

            set
            {
                SetProperty(ref _ItemCompraSelecionado, null);
            }
        }

        public bool ModoPesquisaPedido
        {
            get
            {
                return _ModoPesquisaPedido;
            }

            set
            {
                SetProperty(ref _ModoPesquisaPedido, value);
            }
        }

        public bool IsLoadingPedido
        {
            get
            {
                return _IsLoadingPedido;
            }

            set
            {
                SetProperty(ref _IsLoadingPedido, value);
            }
        }

        private async Task VerificarAcaoItem(ItemTappedEventArgs itemSelecionado)
        {
            var Pagina = new EdicaoRequisicaoCompraPage() { BindingContext = new EdicaoRequisicaoCompraViewModel((ListaCompra)itemSelecionado.Item) };
            await PushAsync(Pagina);
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

        private async Task VerificarPesquisaRequisicao()
        {
            if (ModoPesquisaPedido)
            {
                if (PesquisarPedidoCompraCommand.CanExecute(null))
                    PesquisarPedidoCompraCommand.ChangeCanExecute();
                await CarregarListaRequisicao();
                PesquisarPedidoCompraCommand.ChangeCanExecute();
            }
            ModoPesquisaPedido = !ModoPesquisaPedido;

        }

        private async Task CarregarListaAmigos()
        {
            List<Usuario> Dados = new List<Usuario>();
            if (Conectado)
            {
                using (ApiService srv = new ApiService())
                {
                    Dados = await srv.ListarAmigos();

                }
            }
            else
                Dados = await DatabaseService.Database.ListarAmigos();
            ListaAmigos = new ObservableCollection<Usuario>(Dados);

            OnPropertyChanged("ListaAmigos");
        }


        private async Task CarregarListaPedidos()
        {
            List<ListaCompra> Dados = new List<Models.ListaCompra>();
            if (Conectado)
            {
                using (ApiService srv = new ApiService())
                {
                    Dados = await srv.ListarListaCompra(ItemCriterioBusca);

                }
            }
            else
            {
                Dados = await DatabaseService.Database.ListarListaCompra(ItemCriterioBuscaPedido, ItemUsuarioLogado.Codigo);

            }
            ListaCompra = new ObservableCollection<ListaCompra>(Dados);
            OnPropertyChanged("ListaCompra");
            IsLoadingLista = false;
        }

        private async Task CarregarListaRequisicao()
        {
            List<ListaCompra> Dados = new List<Models.ListaCompra>();
            if (Conectado)
            {
                using (ApiService srv = new ApiService())
                {
                    Dados = await srv.ListarPedidosCompraRecebido(ItemCriterioBuscaPedido);

                }
            }
            else
            {
                Dados = await DatabaseService.Database.ListarListaCompra_Requisicao(ItemCriterioBuscaPedido, ItemUsuarioLogado.Codigo);
            }
            PedidosCompra = new ObservableCollection<ListaCompra>(Dados);
            OnPropertyChanged("PedidosCompra");
            IsLoadingLista = false;
        }

        private void Excluir(ListaCompra itemListaCompra)
        {
            MessagingService.Current.SendMessage<MessagingServiceQuestion>(MessageKeys.DisplayQuestion, new MessagingServiceQuestion()
            {
                Title = "Confirmação",
                Question = String.Format("Excluir o item de compra do item {0} da marca {1}", itemListaCompra.Descricao, itemListaCompra.Marca),
                Positive = "Confirmar",
                Negative = "Cancelar",
                OnCompleted = new Action<bool>(async result =>
                {
                    if (!result) return;
                    ResultadoOperacao Resultado = new ResultadoOperacao();
                    if (Conectado)
                    {
                        using (ApiService srv = new ApiService())
                        {
                            Resultado = await srv.ExcluirListaCompra(itemListaCompra.Identificador);
                            base.AtualizarViagem(ItemViagemSelecionada.Identificador.GetValueOrDefault(), "LC", itemListaCompra.Identificador.GetValueOrDefault(), false);

                            var itemBanco = await DatabaseService.Database.RetornarListaCompra(itemListaCompra.Identificador);
                            if (itemBanco != null)
                            {
                                await DatabaseService.Database.ExcluirListaCompra(itemBanco);
                            }
                        }
                    }
                    else
                    {
                        if (itemListaCompra.Identificador > 0)
                        {
                            itemListaCompra.DataExclusao = DateTime.Now.ToUniversalTime();
                            itemListaCompra.AtualizadoBanco = false;
                            await DatabaseService.Database.SalvarListaCompra(itemListaCompra);
                        }
                        else
                            await DatabaseService.Database.ExcluirListaCompra(itemListaCompra);

                        Resultado.Mensagens = new MensagemErro[] { new MensagemErro() { Mensagem = "Lista Compra Salva com Sucesso" } };
                    }
                    MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.DisplayAlert, new MessagingServiceAlert()
                    {
                        Title = "Sucesso",
                        Message = String.Join(Environment.NewLine, Resultado.Mensagens.Select(d => d.Mensagem).ToArray()),
                        Cancel = "OK"
                    });
                    if (ListaCompra.Where(d => d.Identificador == itemListaCompra.Identificador).Any())
                    {
                        var Posicao = ListaCompra.IndexOf(ListaCompra.Where(d => d.Identificador == itemListaCompra.Identificador).FirstOrDefault());
                        ListaCompra.RemoveAt(Posicao);
                    }

                })
            });
        }

        private async Task Editar(ItemTappedEventArgs itemLista)
        {
            ListaCompra itemEditar = new Models.ListaCompra();
            if (Conectado)
            {
                using (ApiService srv = new ApiService())
                {
                    itemEditar = await srv.CarregarListaCompra(((ListaCompra)itemLista.Item).Identificador);

                }
            }
            else
            {
                itemEditar = await DatabaseService.Database.RetornarListaCompra(((ListaCompra)itemLista.Item).Identificador);
            }
            EdicaoListaCompraPage pagina = new EdicaoListaCompraPage() { BindingContext = new EdicaoListaCompraViewModel(itemEditar, ListaAmigos) };
            await PushAsync(pagina);
        }

        private async Task AbrirInclusao()
        {
            var itemEditar = new ListaCompra() { IdentificadorUsuario = ItemUsuarioLogado.Codigo, Status = (int) enumStatusListaCompra.Pendente, Moeda = ItemViagem.Moeda, Reembolsavel=false};
            EdicaoListaCompraPage pagina = new EdicaoListaCompraPage() { BindingContext = new EdicaoListaCompraViewModel(itemEditar, ListaAmigos) };
            await PushAsync(pagina);

        }
    }
}
