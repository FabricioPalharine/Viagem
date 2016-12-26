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

    public class EdicaoAtracaoViewModel : BaseNavigationViewModel
    {
        private Atracao _ItemAtracao;
        private MapSpan _Bounds;
        private bool _PermiteExcluir = true;
        private bool _PossoComentar = false;
        private bool _VisitaIniciada = false;
        private bool _VisitaConcluida = false;
        private AvaliacaoAtracao _ItemAvaliacao = new AvaliacaoAtracao();
        private Usuario _ParticipanteSelecionado;
        private readonly DateTime _dataMinima = new DateTime(1900, 01, 01);
        public EdicaoAtracaoViewModel(Atracao pItemAtracao, Viagem pItemViagem)
        {
            if (pItemAtracao.IdentificadorAtracaoPai == null)
                pItemAtracao.IdentificadorAtracaoPai = 0;
            if (pItemAtracao.HoraChegada == null)
                pItemAtracao.HoraChegada = new TimeSpan();
            if (pItemAtracao.HoraPartida == null)
                pItemAtracao.HoraPartida = new TimeSpan();
            if (!pItemAtracao.Chegada.HasValue)
                pItemAtracao.Chegada = _dataMinima;
            if (!pItemAtracao.Partida.HasValue)
                pItemAtracao.Partida = _dataMinima;
            ItemAtracao = pItemAtracao;

            Participantes = new ObservableCollection<Usuario>();
            Participantes.CollectionChanged += Participantes_CollectionChanged;
            VisitaConcluida = pItemAtracao.Partida.GetValueOrDefault(_dataMinima) != _dataMinima;
            VisitaIniciada = pItemAtracao.Chegada.GetValueOrDefault(_dataMinima) != _dataMinima;
            PossoComentar = VisitaConcluida && pItemAtracao.Avaliacoes.Where(d => d.IdentificadorUsuario == ItemUsuarioLogado.Codigo).Any();
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

            VisitaConcluidaToggledCommand = new Command<ToggledEventArgs>(
                                                                   (obj) =>  VerificarAcaoConcluidoItem(obj));

            VisitaIniciadaToggledCommand = new Command<ToggledEventArgs>(
                                                                   (obj) => VerificarAcaoIniciadaItem(obj));

            ExcluirCommand = new Command(() =>  Excluir());
            AbrirCustosCommand = new Command(async () => await AbrirJanelaCustos());
           
        }

        private void VerificarAcaoConcluidoItem(ToggledEventArgs obj)
        {
 
                if (obj.Value)
                {
                    if (ItemAtracao.Partida.GetValueOrDefault(_dataMinima) == _dataMinima)
                    {

                        ItemAtracao.Partida = DateTime.Today;
                        ItemAtracao.HoraPartida = DateTime.Now.TimeOfDay;
                    }
                }
                else
                {
                    PossoComentar = false;
                    ItemAtracao.Partida = _dataMinima;
                    ItemAtracao.HoraPartida = new TimeSpan();
                }
        }


        private void VerificarAcaoIniciadaItem(ToggledEventArgs obj)
        {
            if (obj.Value)
            {

                if (ItemAtracao.Chegada.GetValueOrDefault(_dataMinima) == _dataMinima)
                {

                    ItemAtracao.Chegada = DateTime.Today;
                    ItemAtracao.HoraChegada = DateTime.Now.TimeOfDay;
                }

            }
            else
            {
                VisitaConcluida = false;
                ItemAtracao.Chegada = _dataMinima;
                ItemAtracao.HoraChegada = new TimeSpan();
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
                        PossoComentar = VisitaConcluida;
                    else
                        PossoComentar = false;

                }
            }
        }
        public Command VisitaConcluidaToggledCommand { get; set; }
        public Command VisitaIniciadaToggledCommand { get; set; }
        public Command SalvarCommand { get; set; }
        public Command PageAppearingCommand { get; set; }
        public Command ExcluirCommand { get; set; }
        public Command AbrirCustosCommand { get; set; }
        public Command<GmsSearchResults> PlaceSelectedCommand
        {
            get
            {
                return new Command<GmsSearchResults>(p =>
                {
                    ItemAtracao.Nome = p.name;
                    ItemAtracao.CodigoPlace = p.place_id;
                    ItemAtracao.Tipo = string.Join(",", p.types);

                });
            }
        }

        public Viagem ItemViagem { get; set; }

        public async Task CarregarPosicao()
        {
            await Task.Delay(100);
            if (ItemAtracao.Avaliacoes.Where(d => d.IdentificadorUsuario == ItemUsuarioLogado.Codigo).Any())
                ItemAvaliacao = ItemAtracao.Avaliacoes.Where(d => d.IdentificadorUsuario == ItemUsuarioLogado.Codigo).FirstOrDefault();
            PermiteExcluir = ItemAtracao.Id.HasValue || ItemAtracao.Identificador.HasValue;
            CarregarAtracoesPai();
            CarregarParticipantesViagem();
            if (_ItemAtracao.Latitude.HasValue && _ItemAtracao.Longitude.HasValue)
            {
                Bounds = MapSpan.FromCenterAndRadius(new Position(_ItemAtracao.Latitude.Value, _ItemAtracao.Longitude.Value), new Distance(5000));
               

            }
            else
            {
                var posicao = await RetornarPosicao();
                if (posicao == null)
                    posicao = new Plugin.Geolocator.Abstractions.Position() { Latitude = -23.6040963, Longitude = -46.6178018 };
                Bounds = MapSpan.FromCenterAndRadius(new Position(posicao.Latitude, posicao.Longitude), new Distance(5000));
                ItemAtracao.Longitude = posicao.Longitude;
                ItemAtracao.Latitude = posicao.Latitude;
            }
           
            

        }

        private async void CarregarParticipantesViagem()
        {
            if (!Participantes.Any())
            {
                using (ApiService srv = new ApiService())
                {
                    Participantes.Clear();
                    var ListaUsuario = await srv.ListarParticipantesViagem();
                    foreach (var itemUsuario in ListaUsuario)
                    {
                        if (!ItemAtracao.Identificador.HasValue || ItemAtracao.Avaliacoes.Where(d => d.IdentificadorUsuario == itemUsuario.Identificador).Any())
                            itemUsuario.Selecionado = true;
                        Participantes.Add(itemUsuario);
                    }
                }
            }
        }

        public async void CarregarAtracoesPai()
        {
            using (ApiService srv = new ApiService())
            {
                var ListaDados = await srv.ListarAtracao(new CriterioBusca());
                ListaDados = ListaDados.Where(d => !ItemAtracao.Identificador.HasValue || d.Identificador != ItemAtracao.Identificador) .ToList();
                ListaDados.Insert(0, new Atracao() { Identificador = 0, Nome = "Sem Atração Pai" });
                ListaAtracaoPai = new ObservableCollection<Atracao>(ListaDados);
                OnPropertyChanged("ListaAtracaoPai");
                int? IdentificadorAtracaoSelecionada = ItemAtracao.IdentificadorAtracaoPai;
                ItemAtracao.IdentificadorAtracaoPai = int.MinValue;
                await Task.Delay(100);
                ItemAtracao.IdentificadorAtracaoPai = IdentificadorAtracaoSelecionada.GetValueOrDefault(0);
            }
        }


        public Atracao ItemAtracao
        {
            get
            {
                return _ItemAtracao;
            }

            set
            {
                SetProperty(ref _ItemAtracao, value);
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

        public bool VisitaIniciada
        {
            get
            {
                return _VisitaIniciada;
            }

            set
            {
                SetProperty(ref _VisitaIniciada, value);
                if (value)
                {
                  
                   
                }
                else
                {
                    VisitaConcluida = false;
                    
                }
            }
        }

        public bool VisitaConcluida
        {
            get
            {
                return _VisitaConcluida;
            }

            set
            {
                SetProperty(ref _VisitaConcluida, value);
                if (value)
                {
                   
                    PossoComentar = Participantes.Where(d => d.Identificador == ItemUsuarioLogado.Codigo && d.Selecionado).Any();
                }
                else
                {
                    PossoComentar = false;
                   
                }
            }
        }

        public AvaliacaoAtracao ItemAvaliacao
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

        private async Task Salvar()
        {
            IsBusy = true;
            SalvarCommand.ChangeCanExecute();
            try
            {
                foreach (Usuario itemUsuario in Participantes)
                {
                    if (itemUsuario.Selecionado)
                    {
                        if (!ItemAtracao.Avaliacoes.Where(d => d.IdentificadorUsuario == itemUsuario.Identificador).Any())
                        {
                            var itemNovaAvaliacao = new AvaliacaoAtracao() { IdentificadorUsuario = itemUsuario.Identificador };
                            if (itemUsuario.Identificador == ItemUsuarioLogado.Codigo)
                            {
                                itemNovaAvaliacao.Nota = ItemAvaliacao.Nota;
                                itemNovaAvaliacao.Comentario = ItemAvaliacao.Comentario;
                            }
                            itemNovaAvaliacao.DataAtualizacao = DateTime.Now.ToUniversalTime();
                            ItemAtracao.Avaliacoes.Add(itemNovaAvaliacao);
                        }
                    }
                    else
                    {
                        if (ItemAtracao.Avaliacoes.Where(d => d.IdentificadorUsuario == itemUsuario.Identificador).Any())
                        {
                            ItemAtracao.Avaliacoes.Remove(ItemAtracao.Avaliacoes.Where(d => d.IdentificadorUsuario == itemUsuario.Identificador).FirstOrDefault());
                            ItemAvaliacao = new AvaliacaoAtracao();
                        }
                    }
                }
                if (ItemAtracao.IdentificadorAtracaoPai == 0)
                    ItemAtracao.IdentificadorAtracaoPai = null;
                if (!VisitaIniciada)
                    ItemAtracao.Chegada = null;
                else
                    ItemAtracao.Chegada = ItemAtracao.Chegada.GetValueOrDefault().Date.Add(ItemAtracao.HoraChegada.GetValueOrDefault());

                if (!VisitaConcluida)
                    ItemAtracao.Partida = null;
                else
                    ItemAtracao.Partida = ItemAtracao.Chegada.GetValueOrDefault().Date.Add(ItemAtracao.HoraPartida.GetValueOrDefault());
                using (ApiService srv = new ApiService())
                {
                    var Resultado = await srv.SalvarAtracao(ItemAtracao);
                    if (Resultado.Sucesso)
                    {

                        MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.DisplayAlert, new MessagingServiceAlert()
                        {
                            Title = "Sucesso",
                            Message = String.Join(Environment.NewLine, Resultado.Mensagens.Select(d => d.Mensagem).ToArray()),
                            Cancel = "OK"
                        });
                        ItemAtracao.Identificador = Resultado.IdentificadorRegistro;
                        // var Jresultado = (JObject)Resultado.ItemRegistro;
                        //Atracao pItemAtracao = Jresultado.ToObject<Atracao>();
                        Atracao pItemAtracao = await srv.CarregarAtracao(ItemAtracao.Identificador);
                        if (pItemAtracao.IdentificadorAtracaoPai == null)
                            pItemAtracao.IdentificadorAtracaoPai = 0;
                        if (pItemAtracao.HoraChegada == null)
                            pItemAtracao.HoraChegada = new TimeSpan();
                        if (pItemAtracao.HoraPartida == null)
                            pItemAtracao.HoraPartida = new TimeSpan();
                        if (!pItemAtracao.Chegada.HasValue)
                            pItemAtracao.Chegada = _dataMinima;
                        if (!pItemAtracao.Partida.HasValue)
                            pItemAtracao.Partida = _dataMinima;
                        ItemAtracao = pItemAtracao;
                        MessagingService.Current.SendMessage<Atracao>(MessageKeys.ManutencaoAtracao, ItemAtracao);
                            PermiteExcluir = true;
                    }
                    else if (Resultado.Mensagens != null && Resultado.Mensagens.Any())
                    {
                        if (ItemAtracao.IdentificadorAtracaoPai == null)
                            ItemAtracao.IdentificadorAtracaoPai = 0;
                        MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.DisplayAlert, new MessagingServiceAlert()
                        {
                            Title = "Problemas Validação",
                            Message = String.Join(Environment.NewLine, Resultado.Mensagens.Select(d => d.Mensagem).ToArray()),
                            Cancel = "OK"
                        });

                    }
                   
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
                Question = String.Format("Deseja excluir essa visita a Atração?"),
                Positive = "Sim",
                Negative = "Não",
                OnCompleted = new Action<bool>(async result =>
                {
                    if (!result) return;
                    using (ApiService srv = new ApiService())
                    {
                        ItemAtracao.DataExclusao = DateTime.Now;
                        var Resultado = await srv.ExcluirAtracao(ItemAtracao.Identificador);
                        MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.DisplayAlert, new MessagingServiceAlert()
                        {
                            Title = "Sucesso",
                            Message = String.Join(Environment.NewLine, Resultado.Mensagens.Select(d => d.Mensagem).ToArray()),
                            Cancel = "OK"
                        });
                    }
                    MessagingService.Current.SendMessage<Atracao>(MessageKeys.ManutencaoAtracao, ItemAtracao);
                    await PopAsync();

                })
            });


        }

        private async Task AbrirJanelaCustos()
        {
            var Pagina = new ListagemAtracaoCustoPage() { BindingContext = new ListagemAtracaoCustoViewModel(ItemViagem, ItemAtracao) };
            await PushAsync(Pagina);
        }
    }
}
