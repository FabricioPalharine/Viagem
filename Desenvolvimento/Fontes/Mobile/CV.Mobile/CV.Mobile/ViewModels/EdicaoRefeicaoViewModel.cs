using CV.Mobile.Helpers;
using CV.Mobile.Models;
using CV.Mobile.Services;
using CV.Mobile.Views;
using FormsToolkit;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace CV.Mobile.ViewModels
{

    public class EdicaoRefeicaoViewModel : BaseNavigationViewModel
    {
        private Refeicao _ItemRefeicao;
        private MapSpan _Bounds;
        private bool _PermiteExcluir = true;
        private bool _PossoComentar = false;

        private RefeicaoPedido _ItemAvaliacao = new RefeicaoPedido();
        private Usuario _ParticipanteSelecionado;
        private readonly DateTime _dataMinima = new DateTime(1900, 01, 01);
        private double _TamanhoGrid = 0;
        public EdicaoRefeicaoViewModel(Refeicao pItemRefeicao, Viagem pItemViagem)
        {
            if (pItemRefeicao.IdentificadorAtracao == null)
                pItemRefeicao.IdentificadorAtracao = 0;

            ItemRefeicao = pItemRefeicao;

            Participantes = new ObservableCollection<Usuario>();
            Participantes.CollectionChanged += Participantes_CollectionChanged;
            PossoComentar = !pItemRefeicao.Identificador.HasValue || pItemRefeicao.Pedidos.Where(d => d.IdentificadorUsuario == ItemUsuarioLogado.Codigo).Any();
            ItemViagem = pItemViagem;


            SalvarCommand = new Command(
                                async () => await Salvar(),
                                () => true);
            PageAppearingCommand = new Command(
                                                                  async () =>
                                                                  {
                                                                      await CarregarPosicao();
                                                                  },
                                                                  () => true);


            ExcluirCommand = new Command(() => Excluir());
            AbrirCustosCommand = new Command(async () => await AbrirJanelaCustos());

        }
        private void Participantes_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (INotifyPropertyChanged item in e.OldItems)
                {
                    //Removed items
                    item.PropertyChanged -= Item_PropertyChanged;
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (INotifyPropertyChanged item in e.NewItems)
                {
                    //Added items
                    item.PropertyChanged += Item_PropertyChanged;
                }
            }
        }

        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Selecionado")
            {
                var itemUsuario = (Usuario)sender;
                if (itemUsuario.Identificador == ItemUsuarioLogado.Codigo)
                {
                    if (itemUsuario.Selecionado)
                        PossoComentar = true;
                    else
                        PossoComentar = false;

                }
                TamanhoGrid = Participantes.Where(d => d.Selecionado).Count() * 36 + Participantes.Where(d => !d.Selecionado).Count() * 18;
            }
        }
        public Command SalvarCommand { get; set; }
        public Command PageAppearingCommand { get; set; }
        public Command ExcluirCommand { get; set; }
        public Command AbrirCustosCommand { get; set; }

        public Command OpcaoFotoCommand
        {
            get
            {
                return new Command(async () =>
                {
                    UploadFoto itemUpload = new UploadFoto() { IdentificadorRefeicao = ItemRefeicao.Identificador };
                    await CarregarAcaoFoto(itemUpload);
                });
            }
        }
        public Command<GmsSearchResults> PlaceSelectedCommand
        {
            get
            {
                return new Command<GmsSearchResults>(p =>
                {
                    ItemRefeicao.Nome = p.name;
                    ItemRefeicao.CodigoPlace = p.place_id;

                });
            }
        }

        public Viagem ItemViagem { get; set; }

        public async Task CarregarPosicao()
        {
            await Task.Delay(100);
            if (ItemRefeicao.Pedidos.Where(d => d.IdentificadorUsuario == ItemUsuarioLogado.Codigo).Any())
                ItemAvaliacao = ItemRefeicao.Pedidos.Where(d => d.IdentificadorUsuario == ItemUsuarioLogado.Codigo).FirstOrDefault();
            PermiteExcluir = ItemRefeicao.Id.HasValue || ItemRefeicao.Identificador.HasValue;
            CarregarAtracoesPai();
            CarregarParticipantesViagem();
            if (_ItemRefeicao.Latitude.HasValue && _ItemRefeicao.Longitude.HasValue)
            {
                Bounds = MapSpan.FromCenterAndRadius(new Position(_ItemRefeicao.Latitude.Value, _ItemRefeicao.Longitude.Value), new Distance(5000));


            }
            else
            {
                var posicao = await RetornarPosicao();
                if (posicao == null)
                    posicao = new Plugin.Geolocator.Abstractions.Position() { Latitude = 0, Longitude = 0 };
                Bounds = MapSpan.FromCenterAndRadius(new Position(posicao.Latitude, posicao.Longitude), new Distance(5000));
                ItemRefeicao.Longitude = posicao.Longitude;
                ItemRefeicao.Latitude = posicao.Latitude;
            }



        }

        private async void CarregarParticipantesViagem()
        {
            if (!Participantes.Any())
            {

                Participantes.Clear();
                List<Usuario> ListaUsuario = new List<Usuario>();
                if (Conectado)
                {
                    using (ApiService srv = new ApiService())
                    {

                        ListaUsuario = await srv.ListarParticipantesViagem();
                    }
                }
                else
                {
                    ListaUsuario = await DatabaseService.Database.ListarParticipanteViagem();
                }
                foreach (var itemUsuario in ListaUsuario)
                {
                    var itemPedido = ItemRefeicao.Pedidos.Where(d => d.IdentificadorUsuario == itemUsuario.Identificador).FirstOrDefault();
                    if (!ItemRefeicao.Identificador.HasValue || itemPedido != null)
                    {
                        itemUsuario.Selecionado = true;
                        if (itemPedido != null)
                            itemUsuario.Complemento = itemPedido.Pedido;
                    }
                    Participantes.Add(itemUsuario);
                }
                TamanhoGrid = Participantes.Where(d => d.Selecionado).Count() * 36 + Participantes.Where(d => !d.Selecionado).Count() * 18;
            }
        }

        public async void CarregarAtracoesPai()
        {
            List<Atracao> ListaDados = new List<Atracao>();
            if (Conectado)
            {
                using (ApiService srv = new ApiService())
                {
                    ListaDados = await srv.ListarAtracao(new CriterioBusca());

                }
            }
            else
                ListaDados = await DatabaseService.Database.ListarAtracao(new CriterioBusca());
            ListaDados.Insert(0, new Atracao() { Identificador = 0, Nome = "Sem Atração Pai" });
            ListaAtracaoPai = new ObservableCollection<Atracao>(ListaDados);
            OnPropertyChanged("ListaAtracaoPai");
            int? IdentificadorRefeicaoSelecionada = ItemRefeicao.IdentificadorAtracao;
            ItemRefeicao.IdentificadorAtracao = int.MinValue;
            await Task.Delay(100);
            ItemRefeicao.IdentificadorAtracao = IdentificadorRefeicaoSelecionada.GetValueOrDefault(0);

        }


        public Refeicao ItemRefeicao
        {
            get
            {
                return _ItemRefeicao;
            }

            set
            {
                SetProperty(ref _ItemRefeicao, value);
            }
        }

        public ObservableCollection<Atracao> ListaAtracaoPai { get; set; }
        public ObservableCollection<Usuario> Participantes { get; set; }

        public MapSpan Bounds
        {
            get
            {
                return _Bounds;
            }

            set
            {
                SetProperty(ref _Bounds, value);
            }
        }

        public bool PermiteExcluir
        {
            get
            {
                return _PermiteExcluir;
            }

            set
            {
                SetProperty(ref _PermiteExcluir, value);
            }
        }

        public bool PossoComentar
        {
            get
            {
                return _PossoComentar;
            }

            set
            {
                SetProperty(ref _PossoComentar, value);
            }
        }



        public RefeicaoPedido ItemAvaliacao
        {
            get
            {
                return _ItemAvaliacao;
            }

            set
            {
                SetProperty(ref _ItemAvaliacao, value);
            }
        }

        public Usuario ParticipanteSelecionado
        {
            get
            {
                return _ParticipanteSelecionado;
            }

            set
            {
                _ParticipanteSelecionado = null;
                OnPropertyChanged();
            }
        }

        public double TamanhoGrid
        {
            get
            {
                return _TamanhoGrid;
            }

            set
            {
                SetProperty(ref _TamanhoGrid, value);
            }
        }

        private async Task Salvar()
        {
            IsBusy = true;
            SalvarCommand.ChangeCanExecute();
            try
            {
                ItemAvaliacao.DataAtualizacao = DateTime.Now.ToUniversalTime();
                foreach (Usuario itemUsuario in Participantes)
                {
                    if (itemUsuario.Selecionado)
                    {
                        var itemPedidoAtual = ItemRefeicao.Pedidos.Where(d => d.IdentificadorUsuario == itemUsuario.Identificador).FirstOrDefault();
                        if (itemPedidoAtual == null)
                        {
                            var itemNovaAvaliacao = new RefeicaoPedido() { IdentificadorUsuario = itemUsuario.Identificador };
                            if (itemUsuario.Identificador == ItemUsuarioLogado.Codigo)
                            {
                                itemNovaAvaliacao.Nota = ItemAvaliacao.Nota;
                                itemNovaAvaliacao.Comentario = ItemAvaliacao.Comentario;
                            }
                            itemNovaAvaliacao.Pedido = itemUsuario.Complemento;
                            itemNovaAvaliacao.DataAtualizacao = DateTime.Now.ToUniversalTime();
                            ItemRefeicao.Pedidos.Add(itemNovaAvaliacao);
                        }
                        else
                        {
                            itemPedidoAtual.Pedido = itemUsuario.Complemento;
                            itemPedidoAtual.DataAtualizacao = DateTime.Now;
                        }
                    }
                    else
                    {
                        if (ItemRefeicao.Pedidos.Where(d => d.IdentificadorUsuario == itemUsuario.Identificador).Any())
                        {
                            ItemRefeicao.Pedidos.Remove(ItemRefeicao.Pedidos.Where(d => d.IdentificadorUsuario == itemUsuario.Identificador).FirstOrDefault());
                            ItemAvaliacao = new RefeicaoPedido();
                        }
                    }
                }
                if (ItemRefeicao.IdentificadorAtracao == 0)
                    ItemRefeicao.IdentificadorAtracao = null;

                ItemRefeicao.Data = DateTime.SpecifyKind(ItemRefeicao.Data.GetValueOrDefault().Date.Add(ItemRefeicao.Hora.GetValueOrDefault()), DateTimeKind.Unspecified) ;

                ResultadoOperacao Resultado = new ResultadoOperacao();
                Refeicao pItemRefeicao = null;
                if (Conectado)
                {
                    using (ApiService srv = new ApiService())
                    {
                        Resultado = await srv.SalvarRefeicao(ItemRefeicao);
                        if (Resultado.Sucesso)
                        {
                            var Jresultado = (JObject)Resultado.ItemRegistro;
                            pItemRefeicao = Jresultado.ToObject<Refeicao>();
                            AtualizarViagem(ItemViagem.Identificador.GetValueOrDefault(), "R", Resultado.IdentificadorRegistro.GetValueOrDefault(), !ItemRefeicao.Identificador.HasValue);
                            await DatabaseService.SalvarRefeicaoReplicada(pItemRefeicao);
                        }
                    }
                }
                else
                {
                    Resultado = await DatabaseService.SalvarRefeicao(ItemRefeicao);
                    if (Resultado.Sucesso)
                        pItemRefeicao = await DatabaseService.CarregarRefeicao(ItemRefeicao.Identificador);
                }

                if (Resultado.Sucesso)
                {

                    MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.DisplayAlert, new MessagingServiceAlert()
                    {
                        Title = "Sucesso",
                        Message = String.Join(Environment.NewLine, Resultado.Mensagens.Select(d => d.Mensagem).ToArray()),
                        Cancel = "OK"
                    });
                    ItemRefeicao.Identificador = Resultado.IdentificadorRegistro;

                    if (pItemRefeicao.IdentificadorAtracao == null)
                        pItemRefeicao.IdentificadorAtracao = 0;
                    if (pItemRefeicao.Gastos == null)
                        pItemRefeicao.Gastos = new MvvmHelpers.ObservableRangeCollection<GastoRefeicao>();
                    ItemRefeicao = pItemRefeicao;
                    MessagingService.Current.SendMessage<Refeicao>(MessageKeys.ManutencaoRefeicao, ItemRefeicao);
                    PermiteExcluir = true;
                }
                else if (Resultado.Mensagens != null && Resultado.Mensagens.Any())
                {
                    if (ItemRefeicao.IdentificadorAtracao == null)
                        ItemRefeicao.IdentificadorAtracao = 0;
                    MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.DisplayAlert, new MessagingServiceAlert()
                    {
                        Title = "Problemas Validação",
                        Message = String.Join(Environment.NewLine, Resultado.Mensagens.Select(d => d.Mensagem).ToArray()),
                        Cancel = "OK"
                    });

                }


            }
            finally
            {
                SalvarCommand.ChangeCanExecute();
                IsBusy = false;
            }
        }

        private void Excluir()
        {
            MessagingService.Current.SendMessage<MessagingServiceQuestion>(MessageKeys.DisplayQuestion, new MessagingServiceQuestion()
            {
                Title = "Confirmação",
                Question = String.Format("Deseja excluir essa Refeição?"),
                Positive = "Sim",
                Negative = "Não",
                OnCompleted = new Action<bool>(async result =>
                {
                    if (!result) return;

                    ResultadoOperacao Resultado = new ResultadoOperacao();
                    ItemRefeicao.DataExclusao = DateTime.Now.ToUniversalTime();
                    if (Conectado)
                    {
                        using (ApiService srv = new ApiService())
                        {
                            Resultado = await srv.ExcluirRefeicao(ItemRefeicao.Identificador);
                            await DatabaseService.ExcluirRefeicao(ItemRefeicao.Identificador, true);
                            AtualizarViagem(ItemViagem.Identificador.GetValueOrDefault(), "R", ItemRefeicao.Identificador.GetValueOrDefault(), false);
                        }
                    }
                    else
                    {
                        await DatabaseService.ExcluirRefeicao(ItemRefeicao.Identificador, false);
                        Resultado.Mensagens = new MensagemErro[] { new MensagemErro() { Mensagem = "Refeição Excluída com Sucesso " } };

                    }

                    MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.DisplayAlert, new MessagingServiceAlert()
                    {
                        Title = "Sucesso",
                        Message = String.Join(Environment.NewLine, Resultado.Mensagens.Select(d => d.Mensagem).ToArray()),
                        Cancel = "OK"
                    });


                    MessagingService.Current.SendMessage<Refeicao>(MessageKeys.ManutencaoRefeicao, ItemRefeicao);
                    await PopAsync();

                })
            });


        }

        private async Task AbrirJanelaCustos()
        {
            var Pagina = new ListagemRefeicaoCustoPage() { BindingContext = new ListagemRefeicaoCustoViewModel(ItemViagem, ItemRefeicao) };
            await PushAsync(Pagina);
        }
    }
}
