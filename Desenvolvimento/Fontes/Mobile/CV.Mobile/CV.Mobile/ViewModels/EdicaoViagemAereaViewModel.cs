using CV.Mobile.Helpers;
using CV.Mobile.Models;
using CV.Mobile.Services;
using CV.Mobile.Views;
using FormsToolkit;
using MvvmHelpers;
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

    public class EdicaoViagemAereaViewModel : BaseNavigationViewModel
    {
        private ViagemAerea _ItemViagemAerea;
        private bool _PermiteExcluir = true;
        private bool _PossoComentar = false;
        private AvaliacaoAerea _ItemAvaliacao = new AvaliacaoAerea();
        private Usuario _ParticipanteSelecionado;
        private ViagemAereaAeroporto _AeroportoSelecionado;

        private readonly DateTime _dataMinima = new DateTime(1900, 01, 01);
        private Plugin.Geolocator.Abstractions.Position _PosicaoAtual = null;
        public ObservableRangeCollection<ViagemAereaAeroporto> ListaDados { get; set; }

        private ViagemAereaAeroporto _ItemOrigem;
        private ViagemAereaAeroporto _ItemDestino;
        public EdicaoViagemAereaViewModel(ViagemAerea pItemViagemAerea, Viagem pItemViagem)
        {
            AjustarDadosDataHora(pItemViagemAerea);
            ItemViagemAerea = pItemViagemAerea;

            ItemOrigem = ItemViagemAerea.Aeroportos.Where(d => d.TipoPonto == (int)enumTipoParada.Origem).FirstOrDefault();
            ItemDestino = ItemViagemAerea.Aeroportos.Where(d => d.TipoPonto == (int)enumTipoParada.Destino).FirstOrDefault();
            ItemDestino.PropertyChanged += ItemDestino_PropertyChanged;
            ItemOrigem.PropertyChanged += ItemDestino_PropertyChanged;

            ListaDados = new ObservableRangeCollection<ViagemAereaAeroporto>(ItemViagemAerea.Aeroportos.Where(d => d.TipoPonto == (int)enumTipoParada.Escala).Where(d => !d.DataExclusao.HasValue).OrderBy(d => Math.Abs(d.Identificador.GetValueOrDefault(0))));
            ListaDados.CollectionChanged += ListaDados_CollectionChanged;
            Participantes = new ObservableCollection<Usuario>();
            Participantes.CollectionChanged += Participantes_CollectionChanged;
            PossoComentar = pItemViagemAerea.Avaliacoes.Where(d => d.IdentificadorUsuario == ItemUsuarioLogado.Codigo).Any() && ItemDestino.VisitaIniciada;
            ItemViagem = pItemViagem;

            ListaTipo = new ObservableCollection<ItemLista>();
            List<ItemLista> lista = new List<ItemLista>();
            foreach (var enumerador in Enum.GetValues(typeof(enumTipoTransporte)))
            {
                var item = new ItemLista() { Codigo = Convert.ToInt32(enumerador).ToString(), Descricao = ((enumTipoTransporte)enumerador).Descricao() };
                ListaTipo.Add(item);
            }
            ListaTipo = new ObservableCollection<ItemLista>(ListaTipo.OrderBy(d => d.Descricao));

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
            ExcluirEscalaCommand = new Command<ViagemAereaAeroporto>((obj) => ExcluirEscala(obj));
            AdicionarEscalaCommand = new Command(async () => await AdicionarEscala());

            AbrirCustosCommand = new Command(async () => await AbrirJanelaCustos());

        }

        private void ListaDados_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (INotifyPropertyChanged item in e.OldItems)
                {
                    //Removed items
                    item.PropertyChanged -= ItemDestino_PropertyChanged;
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (INotifyPropertyChanged item in e.NewItems)
                {
                    //Added items
                    item.PropertyChanged += ItemDestino_PropertyChanged;
                }
            }
        }

        private void AjustarDadosDataHora(ViagemAerea pItemViagemAerea)
        {
            foreach (var itemAeroporto in pItemViagemAerea.Aeroportos)
            {
                if (itemAeroporto.DataChegada == null)
                {
                    itemAeroporto.DataChegada = _dataMinima;
                    itemAeroporto.HoraChegada = new TimeSpan();
                }
                if (itemAeroporto.DataPartida == null)
                {
                    itemAeroporto.DataPartida = _dataMinima;
                    itemAeroporto.HoraPartida = new TimeSpan();
                }
            }
        }

        private async void ItemDestino_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ViagemAereaAeroporto item = (ViagemAereaAeroporto)sender;

            if (e.PropertyName == "VisitaIniciada")
            {
                if (item.Equals(ItemDestino))
                {

                    if (item.VisitaIniciada)
                    {
                        if (Participantes.Any())
                            PossoComentar = Participantes.Where(d => d.Selecionado).Where(d => d.Identificador == ItemUsuarioLogado.Codigo).Any();
                    }
                    else
                        PossoComentar = false;
                }
                else if (item.VisitaIniciada)
                {
                    if (_PosicaoAtual != null)
                    {
                        item.Latitude = _PosicaoAtual.Latitude;
                        item.Longitude = _PosicaoAtual.Longitude;
                    }
                    var posicao = await RetornarPosicao();
                    if (posicao == null)
                        posicao = new Plugin.Geolocator.Abstractions.Position() { Latitude = -23.6040963, Longitude = -46.6178018 };
                    _PosicaoAtual = posicao;
                    item.Latitude = _PosicaoAtual.Latitude;
                    item.Longitude = _PosicaoAtual.Longitude;
                }
            }
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
                        PossoComentar = ItemDestino.VisitaIniciada;
                    else
                        PossoComentar = false;

                }
            }
        }

        public Command SalvarCommand { get; set; }
        public Command PageAppearingCommand { get; set; }
        public Command ExcluirCommand { get; set; }
        public Command AbrirCustosCommand { get; set; }
        public Command AdicionarEscalaCommand { get; set; }
        public ObservableCollection<ItemLista> ListaTipo { get; set; }
        public Command ExcluirEscalaCommand { get; set; }
        public Viagem ItemViagem { get; set; }

        public async Task CarregarPosicao()
        {
            await Task.Delay(100);
            if (ItemViagemAerea.Avaliacoes.Where(d => d.IdentificadorUsuario == ItemUsuarioLogado.Codigo).Any())
                ItemAvaliacao = ItemViagemAerea.Avaliacoes.Where(d => d.IdentificadorUsuario == ItemUsuarioLogado.Codigo).FirstOrDefault();
            PermiteExcluir = ItemViagemAerea.Id.HasValue || ItemViagemAerea.Identificador.HasValue;
            CarregarParticipantesViagem();

            var posicao = await RetornarPosicao();
            if (posicao == null)
                posicao = new Plugin.Geolocator.Abstractions.Position() { Latitude = -23.6040963, Longitude = -46.6178018 };
            _PosicaoAtual = posicao;




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
                    if (!ItemViagemAerea.Identificador.HasValue || ItemViagemAerea.Avaliacoes.Where(d => d.IdentificadorUsuario == itemUsuario.Identificador).Any())
                        itemUsuario.Selecionado = true;
                    Participantes.Add(itemUsuario);
                }

            }
        }




        public ViagemAerea ItemViagemAerea
        {
            get
            {
                return _ItemViagemAerea;
            }

            set
            {
                SetProperty(ref _ItemViagemAerea, value);
            }
        }

        public ObservableCollection<ViagemAerea> ListaViagemAereaPai { get; set; }
        public ObservableCollection<Usuario> Participantes { get; set; }

        public bool TemEscala
        {
            get
            {
                return ListaDados != null && ListaDados.Any();
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



        public AvaliacaoAerea ItemAvaliacao
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

        public ViagemAereaAeroporto AeroportoSelecionado
        {
            get
            {
                return _AeroportoSelecionado;
            }

            set
            {
                _AeroportoSelecionado = null;
                OnPropertyChanged();
            }
        }

        public ViagemAereaAeroporto ItemOrigem
        {
            get
            {
                return _ItemOrigem;
            }

            set
            {
                SetProperty(ref _ItemOrigem, value);
            }
        }

        public ViagemAereaAeroporto ItemDestino
        {
            get
            {
                return _ItemDestino;
            }

            set
            {
                SetProperty(ref _ItemDestino, value);

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
                        if (!ItemViagemAerea.Avaliacoes.Where(d => d.IdentificadorUsuario == itemUsuario.Identificador).Any())
                        {
                            var itemNovaAvaliacao = new AvaliacaoAerea() { IdentificadorUsuario = itemUsuario.Identificador };
                            if (itemUsuario.Identificador == ItemUsuarioLogado.Codigo)
                            {
                                itemNovaAvaliacao.Nota = ItemAvaliacao.Nota;
                                itemNovaAvaliacao.Comentario = ItemAvaliacao.Comentario;
                            }
                            itemNovaAvaliacao.DataAtualizacao = DateTime.Now.ToUniversalTime();
                            ItemViagemAerea.Avaliacoes.Add(itemNovaAvaliacao);
                        }
                    }
                    else
                    {
                        if (ItemViagemAerea.Avaliacoes.Where(d => d.IdentificadorUsuario == itemUsuario.Identificador).Any())
                        {
                            ItemViagemAerea.Avaliacoes.Remove(ItemViagemAerea.Avaliacoes.Where(d => d.IdentificadorUsuario == itemUsuario.Identificador).FirstOrDefault());
                            ItemAvaliacao = new AvaliacaoAerea();
                        }
                    }
                }
                List<ViagemAereaAeroporto> ListaAvaliacoesAlteradas = new List<ViagemAereaAeroporto>();
                AjustarDadosAeroporto(ListaAvaliacoesAlteradas, ItemOrigem);
                AjustarDadosAeroporto(ListaAvaliacoesAlteradas, ItemDestino);

                foreach (var itemAeroporto in ListaDados)
                {
                    AjustarDadosAeroporto(ListaAvaliacoesAlteradas, itemAeroporto);
                }
                ItemViagemAerea.Aeroportos = new ObservableRangeCollection<ViagemAereaAeroporto>(ListaAvaliacoesAlteradas);
                ViagemAerea pItemViagemAerea = null;
                ResultadoOperacao Resultado = null;

                if (Conectado)
                {
                    using (ApiService srv = new ApiService())
                    {
                        Resultado = await srv.SalvarViagemAerea(ItemViagemAerea);
                        if (Resultado.Sucesso)
                        {

                            AtualizarViagem(ItemViagemSelecionada.Identificador.GetValueOrDefault(), "VA", Resultado.IdentificadorRegistro.GetValueOrDefault(), !ItemViagemAerea.Identificador.HasValue);
                            pItemViagemAerea = await srv.CarregarViagemAerea(Resultado.IdentificadorRegistro);

                            await DatabaseService.SalvarViagemAereaReplicada(pItemViagemAerea);
                        }

                    }
                }
                else
                {
                    Resultado = await DatabaseService.SalvarViagemAerea(ItemViagemAerea);
                    if (Resultado.Sucesso)
                        pItemViagemAerea = await DatabaseService.CarregarViagemAerea(Resultado.IdentificadorRegistro);
                }


                if (Resultado.Sucesso)
                {

                    MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.DisplayAlert, new MessagingServiceAlert()
                    {
                        Title = "Sucesso",
                        Message = String.Join(Environment.NewLine, Resultado.Mensagens.Select(d => d.Mensagem).ToArray()),
                        Cancel = "OK"
                    });
                    ItemViagemAerea.Identificador = Resultado.IdentificadorRegistro;
                    // var Jresultado = (JObject)Resultado.ItemRegistro;
                    //ViagemAerea pItemViagemAerea = Jresultado.ToObject<ViagemAerea>();
                    AjustarDadosDataHora(pItemViagemAerea);
                    ListaDados = new ObservableRangeCollection<ViagemAereaAeroporto>(pItemViagemAerea.Aeroportos.Where(d => d.TipoPonto == (int)enumTipoParada.Escala).Where(d => !d.DataExclusao.HasValue).OrderBy(d => Math.Abs(d.Identificador.GetValueOrDefault(0))));
                    OnPropertyChanged("ListaDados");
                    ItemViagemAerea = pItemViagemAerea;
                    ItemOrigem = ItemViagemAerea.Aeroportos.Where(d => d.TipoPonto == (int)enumTipoParada.Origem).FirstOrDefault();
                    ItemDestino = ItemViagemAerea.Aeroportos.Where(d => d.TipoPonto == (int)enumTipoParada.Destino).FirstOrDefault();
                    ItemOrigem.PropertyChanged += ItemDestino_PropertyChanged;
                    ItemDestino.PropertyChanged += ItemDestino_PropertyChanged;
                    ListaDados.CollectionChanged += ListaDados_CollectionChanged;

                    MessagingService.Current.SendMessage<ViagemAerea>(MessageKeys.ManutencaoViagemAerea, ItemViagemAerea);
                    PermiteExcluir = true;
                }
                else if (Resultado.Mensagens != null && Resultado.Mensagens.Any())
                {
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

        private void AjustarDadosAeroporto(List<ViagemAereaAeroporto> ListaAvaliacoesAlteradas, ViagemAereaAeroporto itemAeroporto)
        {
            if (!itemAeroporto.DataExclusao.HasValue)
            {
                var itemGravar = itemAeroporto.Clone();
                if (itemGravar.DataChegada.GetValueOrDefault(_dataMinima) == _dataMinima)
                {
                    itemGravar.DataChegada = null;
                    itemGravar.HoraChegada = null;
                }
                else
                    itemGravar.DataChegada = itemGravar.DataChegada.Value.Date.Add(itemGravar.HoraChegada.GetValueOrDefault());

                if (itemGravar.DataPartida.GetValueOrDefault(_dataMinima) == _dataMinima)
                {
                    itemGravar.DataPartida = null;
                    itemGravar.HoraPartida = null;
                }
                else
                    itemGravar.DataPartida = itemGravar.DataPartida.Value.Date.Add(itemGravar.HoraPartida.GetValueOrDefault());
                ListaAvaliacoesAlteradas.Add(itemGravar);
            }
        }

        private async Task AdicionarEscala()
        {
            var UltimaEscala = ItemViagemAerea.Aeroportos.Where(d => d.TipoPonto == (int)enumTipoParada.Escala).Where(d => d.DataPartida.GetValueOrDefault(_dataMinima) == _dataMinima).FirstOrDefault();
            if (UltimaEscala != null)
            {
                MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.DisplayAlert, new MessagingServiceAlert()
                {
                    Title = "Aviso",
                    Message = "Não pode ser adicionada uma nova parada, pois a anterior ainda está em aberto",
                    Cancel = "OK"
                });
            }
            else
            {
                ViagemAereaAeroporto ponto = new ViagemAereaAeroporto() { HoraChegada = DateTime.Now.TimeOfDay, DataChegada = DateTime.Today, TipoPonto = (int)enumTipoParada.Escala, DataPartida = _dataMinima, HoraPartida = new TimeSpan() };
                if (_PosicaoAtual != null)
                {
                    ponto.Latitude = _PosicaoAtual.Latitude;
                    ponto.Longitude = _PosicaoAtual.Longitude;
                }
                ItemViagemAerea.Aeroportos.Add(ponto);
                ListaDados.Add(ponto);
                OnPropertyChanged("TemEscala");
                var posicao = await RetornarPosicao();
                if (posicao == null)
                    posicao = new Plugin.Geolocator.Abstractions.Position() { Latitude = -23.6040963, Longitude = -46.6178018 };
                _PosicaoAtual = posicao;
                ponto.Latitude = _PosicaoAtual.Latitude;
                ponto.Longitude = _PosicaoAtual.Longitude;
            }
        }

        private void ExcluirEscala(ViagemAereaAeroporto obj)
        {
            MessagingService.Current.SendMessage<MessagingServiceQuestion>(MessageKeys.DisplayQuestion, new MessagingServiceQuestion()
            {
                Title = "Confirmação",
                Question = String.Format("Deseja excluir essa Escala?"),
                Positive = "Sim",
                Negative = "Não",
                OnCompleted = new Action<bool>(result =>
               {
                   if (!result) return;
                   obj.DataExclusao = DateTime.Now.ToUniversalTime();
                   ListaDados.Remove(obj);
                   ItemViagemAerea.Aeroportos.Remove(obj);
                   OnPropertyChanged("TemEscala");

               })
            });


        }

        private void Excluir()
        {
            MessagingService.Current.SendMessage<MessagingServiceQuestion>(MessageKeys.DisplayQuestion, new MessagingServiceQuestion()
            {
                Title = "Confirmação",
                Question = String.Format("Deseja excluir esse Deslocamento?"),
                Positive = "Sim",
                Negative = "Não",
                OnCompleted = new Action<bool>(async result =>
                {
                    if (!result) return;

                    ResultadoOperacao Resultado = new ResultadoOperacao();
                    ItemViagemAerea.DataExclusao = DateTime.Now.ToUniversalTime();
                    if (Conectado)
                    {
                        using (ApiService srv = new ApiService())
                        {
                            Resultado = await srv.ExcluirViagemAerea(ItemViagemAerea.Identificador);
                            AtualizarViagem(ItemViagemAerea.Identificador.GetValueOrDefault(), "VA", ItemViagemAerea.Identificador.GetValueOrDefault(), false);

                            await DatabaseService.ExcluirViagemAerea(ItemViagemAerea.Identificador, true);

                        }
                    }
                    else
                    {
                        ItemViagemAerea.AtualizadoBanco = false;
                        await DatabaseService.ExcluirViagemAerea(ItemViagemAerea.Identificador, false);
                        Resultado.Mensagens = new MensagemErro[] { new MensagemErro() { Mensagem = "Deslocamento excluído com sucesso " } };
                    }




                    MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.DisplayAlert, new MessagingServiceAlert()
                    {
                        Title = "Sucesso",
                        Message = String.Join(Environment.NewLine, Resultado.Mensagens.Select(d => d.Mensagem).ToArray()),
                        Cancel = "OK"
                    });

                    MessagingService.Current.SendMessage<ViagemAerea>(MessageKeys.ManutencaoViagemAerea, ItemViagemAerea);
                    await PopAsync();

                })
            });


        }

        private async Task AbrirJanelaCustos()
        {
            var Pagina = new ListagemViagemAereaCustoPage() { BindingContext = new ListagemViagemAereaCustoViewModel(ItemViagem, ItemViagemAerea) };
            await PushAsync(Pagina);
        }
    }
}
