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

    public class EdicaoHotelViewModel : BaseNavigationViewModel
    {
        private Hotel _ItemHotel;
        private Hotel _ItemHotelOriginal;
        private double _TamanhoGrid ;
        private MapSpan _Bounds;
        private bool _PermiteExcluir = true;
        private bool _PossoComentar = false;
        private bool _VisitaIniciada = false;
        private bool _VisitaConcluida = false;
        private HotelAvaliacao _ItemAvaliacao = new HotelAvaliacao();
        private Usuario _ParticipanteSelecionado;
        private readonly DateTime _dataMinima = new DateTime(1900, 01, 01);
        private string _TextoComandoTrocar = "Cheguei Hotel";
        public EdicaoHotelViewModel(Hotel pItemHotel, Viagem pItemViagem)
        {
            _ItemHotelOriginal = pItemHotel.Clone();
            if (pItemHotel.HoraEntrada == null)
                pItemHotel.HoraEntrada = new TimeSpan();
            if (pItemHotel.HoraSaida == null)
                pItemHotel.HoraSaida = new TimeSpan();
            if (!pItemHotel.DataEntrada.HasValue)
                pItemHotel.DataEntrada = _dataMinima;
            if (!pItemHotel.DataSaidia.HasValue)
                pItemHotel.DataSaidia = _dataMinima;
            ItemHotel = pItemHotel;

            Participantes = new ObservableCollection<Usuario>();
            Participantes.CollectionChanged += Participantes_CollectionChanged;
            VisitaConcluida = pItemHotel.DataSaidia.GetValueOrDefault(_dataMinima) != _dataMinima;
            VisitaIniciada = pItemHotel.DataEntrada.GetValueOrDefault(_dataMinima) != _dataMinima;
            PossoComentar = VisitaConcluida && pItemHotel.Avaliacoes.Where(d => d.IdentificadorUsuario == ItemUsuarioLogado.Codigo).Any();
            ItemViagem = pItemViagem;


            SalvarCommand = new Command(
                                async () => await Salvar(),
                                () => !IsBusy);
            PageAppearingCommand = new Command(
                                                                  async () =>
                                                                  {
                                                                      await CarregarPosicao();
                                                                  },
                                                                  () => true);

            VisitaConcluidaToggledCommand = new Command<ToggledEventArgs>(
                                                                   (obj) => VerificarAcaoConcluidoItem(obj));

            VisitaIniciadaToggledCommand = new Command<ToggledEventArgs>(
                                                                   (obj) => VerificarAcaoIniciadaItem(obj));
            TrocarSituacaoCommand = new Command(async () => await TrocarSituacaoEvento());

            ExcluirCommand = new Command(() => Excluir());
            AbrirCustosCommand = new Command(async () => await AbrirJanelaCustos());
            if (ItemHotel.Eventos != null)
            {
                bool NoHotel = _ItemHotel.Eventos.Where(d => d.IdentificadorUsuario == ItemUsuarioLogado.Codigo).Where(d => !d.DataSaida.HasValue).Any();
                TextoComandoTrocar = !NoHotel ? "Cheguei Hotel" : "Deixei Hotel";
            }
        }

        private async Task TrocarSituacaoEvento()
        {
            if (Conectado)
            {
                using (ApiService srv = new ApiService())
                {
                    var itemHotel = await srv.CarregarHotel(_ItemHotel.Identificador);
                    bool NoHotel = itemHotel.Eventos.Where(d => d.IdentificadorUsuario == ItemUsuarioLogado.Codigo).Where(d => !d.DataSaida.HasValue).Any();
                    HotelEvento itemEvento = new HotelEvento() { DataEntrada = DateTime.Now, IdentificadorHotel = itemHotel.Identificador, IdentificadorUsuario = ItemUsuarioLogado.Codigo, DataAtualizacao = DateTime.Now.ToUniversalTime() };
                    if (NoHotel)
                        itemEvento.DataSaida = DateTime.Now;
                    var Resultado = await srv.SalvarHotelEvento(itemEvento);
                    TextoComandoTrocar = NoHotel ? "Cheguei Hotel" : "Deixei Hotel";
                    if (Resultado.Sucesso)
                    {
                        var Jresultado = (JObject)Resultado.ItemRegistro;
                        var pItemEvento = Jresultado.ToObject<HotelEvento>();
                        var itemBanco = await DatabaseService.Database.RetornarHotelEvento(pItemEvento.Identificador);
                        if (itemBanco != null)
                            pItemEvento.Id = itemBanco.Id;
                        await DatabaseService.Database.SalvarHotelEvento(pItemEvento);
                        AtualizarViagem(ItemViagem.Identificador.GetValueOrDefault(), "HE", pItemEvento.Identificador.GetValueOrDefault(), false);
                    }
                }
            }
            else
            {
                var itemHotel = await DatabaseService.CarregarHotel(_ItemHotel.Identificador);
                var itemEvento = itemHotel.Eventos.Where(d => d.IdentificadorUsuario == ItemUsuarioLogado.Codigo).Where(d => !d.DataSaida.HasValue).OrderByDescending(d => d.DataEntrada).FirstOrDefault();
                bool NoHotel = itemEvento != null;
                if (itemEvento == null)
                    itemEvento = new HotelEvento() { DataEntrada = DateTime.Now, IdentificadorHotel = itemHotel.Identificador, IdentificadorUsuario = ItemUsuarioLogado.Codigo, DataAtualizacao = DateTime.Now.ToUniversalTime(), AtualizadoBanco = false };
                else
                {
                    itemEvento.DataSaida = DateTime.Now;
                    itemEvento.DataAtualizacao = DateTime.Now.ToUniversalTime();
                    itemEvento.AtualizadoBanco = false;
                }
                await DatabaseService.Database.SalvarHotelEvento(itemEvento);
                TextoComandoTrocar = NoHotel ? "Cheguei Hotel" : "Deixei Hotel";

            }
        }

        private void VerificarAcaoConcluidoItem(ToggledEventArgs obj)
        {

            if (obj.Value)
            {
                if (ItemHotel.DataSaidia.GetValueOrDefault(_dataMinima) == _dataMinima)
                {

                    ItemHotel.DataSaidia = DateTime.Today;
                    ItemHotel.HoraSaida = DateTime.Now.TimeOfDay;
                }
            }
            else
            {
                PossoComentar = false;
                ItemHotel.DataSaidia = _dataMinima;
                ItemHotel.HoraSaida = new TimeSpan();
            }
        }


        private void VerificarAcaoIniciadaItem(ToggledEventArgs obj)
        {
            if (obj.Value)
            {

                if (ItemHotel.DataEntrada.GetValueOrDefault(_dataMinima) == _dataMinima)
                {

                    ItemHotel.DataEntrada = DateTime.Today;
                    ItemHotel.HoraEntrada = DateTime.Now.TimeOfDay;
                }

            }
            else
            {
                VisitaConcluida = false;
                ItemHotel.DataEntrada = _dataMinima;
                ItemHotel.HoraEntrada = new TimeSpan();
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

        public Command TrocarSituacaoCommand { get; set; }
        public Command VisitaConcluidaToggledCommand { get; set; }
        public Command VisitaIniciadaToggledCommand { get; set; }
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
                    UploadFoto itemUpload = new UploadFoto() { IdentificadorHotel = ItemHotel.Identificador };
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
                    ItemHotel.Nome = p.name;
                    ItemHotel.CodigoPlace = p.place_id;

                });
            }
        }

        public Viagem ItemViagem { get; set; }

        public async Task CarregarPosicao()
        {
            await Task.Delay(100);
            if (ItemHotel.Avaliacoes.Where(d => d.IdentificadorUsuario == ItemUsuarioLogado.Codigo).Any())
                ItemAvaliacao = ItemHotel.Avaliacoes.Where(d => d.IdentificadorUsuario == ItemUsuarioLogado.Codigo).FirstOrDefault();
            PermiteExcluir = ItemHotel.Id.HasValue || ItemHotel.Identificador.HasValue;
            CarregarParticipantesViagem();
            if (_ItemHotel.Latitude.HasValue && _ItemHotel.Longitude.HasValue)
            {
                Bounds = MapSpan.FromCenterAndRadius(new Position(_ItemHotel.Latitude.Value, _ItemHotel.Longitude.Value), new Distance(5000));


            }
            else
            {
                var posicao = await RetornarPosicao();
                if (posicao == null)
                    posicao = new Plugin.Geolocator.Abstractions.Position() { Latitude = 0, Longitude = 0 };
                Bounds = MapSpan.FromCenterAndRadius(new Position(posicao.Latitude, posicao.Longitude), new Distance(5000));
                ItemHotel.Longitude = posicao.Longitude;
                ItemHotel.Latitude = posicao.Latitude;
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
                    if (!ItemHotel.Identificador.HasValue || ItemHotel.Avaliacoes.Where(d => d.IdentificadorUsuario == itemUsuario.Identificador).Any())
                        itemUsuario.Selecionado = true;
                    Participantes.Add(itemUsuario);
                }

                TamanhoGrid = Participantes.Count() * 24;


            }
        }




        public Hotel ItemHotel
        {
            get
            {
                return _ItemHotel;
            }

            set
            {
                SetProperty(ref _ItemHotel, value);
            }
        }

        public ObservableCollection<Hotel> ListaHotelPai { get; set; }
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
                    if (Participantes.Any())
                        PossoComentar = Participantes.Where(d => d.Identificador == ItemUsuarioLogado.Codigo && d.Selecionado).Any();
                }
                else
                {
                    PossoComentar = false;

                }
            }
        }

        public HotelAvaliacao ItemAvaliacao
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

        public string TextoComandoTrocar
        {
            get
            {
                return _TextoComandoTrocar;
            }

            set
            {
                SetProperty(ref _TextoComandoTrocar, value);
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
               SetProperty(ref  _TamanhoGrid , value);
            }
        }

        private async Task Salvar()
        {
            IsBusy = true;
            SalvarCommand.ChangeCanExecute();


            try
            {
                ItemAvaliacao.DataAtualizacao = DateTime.Now.ToUniversalTime();
                Hotel pItemHotel = new Hotel();
                if (Conectado && ItemHotel.Identificador.HasValue && ItemHotel.Identificador >0)
                {
                    using (ApiService srv = new ApiService())
                    {
                        pItemHotel = await srv.CarregarHotel(ItemHotel.Identificador.GetValueOrDefault(-1));
                    }
                }
                else
                {
                    pItemHotel = await DatabaseService.CarregarHotel(ItemHotel.Identificador.GetValueOrDefault(-1));
                }
                if (pItemHotel != null)
                    ItemHotel.Eventos = pItemHotel.Eventos;
                List<HotelEvento> EventosAlerta = new List<HotelEvento>();
                bool NovoCheckin = false;
                foreach (Usuario itemUsuario in Participantes)
                {
                    if (itemUsuario.Selecionado)
                    {
                        if (!ItemHotel.Avaliacoes.Where(d => d.IdentificadorUsuario == itemUsuario.Identificador).Any())
                        {
                            var itemNovaAvaliacao = new HotelAvaliacao() { IdentificadorUsuario = itemUsuario.Identificador };
                            if (itemUsuario.Identificador == ItemUsuarioLogado.Codigo)
                            {
                                itemNovaAvaliacao.Nota = ItemAvaliacao.Nota;
                                itemNovaAvaliacao.Comentario = ItemAvaliacao.Comentario;
                            }
                            itemNovaAvaliacao.DataAtualizacao = DateTime.Now.ToUniversalTime();
                            ItemHotel.Avaliacoes.Add(itemNovaAvaliacao);
                        }
                    }
                    else
                    {
                        if (ItemHotel.Avaliacoes.Where(d => d.IdentificadorUsuario == itemUsuario.Identificador).Any())
                        {
                            ItemHotel.Avaliacoes.Remove(ItemHotel.Avaliacoes.Where(d => d.IdentificadorUsuario == itemUsuario.Identificador).FirstOrDefault());
                            ItemAvaliacao = new HotelAvaliacao();
                        }
                    }
                }
                if (ItemHotel.Eventos == null)
                    ItemHotel.Eventos = new MvvmHelpers.ObservableRangeCollection<HotelEvento>();
                if (VisitaIniciada && !_ItemHotelOriginal.DataEntrada.HasValue && !VisitaConcluida)
                {

                    foreach (var item in ItemHotel.Avaliacoes.Where(d => !d.DataExclusao.HasValue))
                    {
                        NovoCheckin = true;
                        var itemEvento = new HotelEvento() { DataAtualizacao = DateTime.Now.ToUniversalTime(), DataEntrada = DateTime.Now, IdentificadorUsuario = item.IdentificadorUsuario };
                        ItemHotel.Eventos.Add(itemEvento);
                    }
                    TextoComandoTrocar = "Deixei Hotel";

                }
                if (!VisitaIniciada)
                    ItemHotel.DataEntrada = null;
                else
                    ItemHotel.DataEntrada = DateTime.SpecifyKind(ItemHotel.DataEntrada.GetValueOrDefault().Date.Add(ItemHotel.HoraEntrada.GetValueOrDefault()), DateTimeKind.Unspecified);

                if (!VisitaConcluida)
                    ItemHotel.DataSaidia = null;
                else
                    ItemHotel.DataSaidia = DateTime.SpecifyKind(ItemHotel.DataSaidia.GetValueOrDefault().Date.Add(ItemHotel.HoraSaida.GetValueOrDefault()), DateTimeKind.Unspecified) ;

                if (VisitaConcluida && !_ItemHotelOriginal.DataSaidia.HasValue && ItemHotel.Eventos != null)
                {
                    foreach (var item in ItemHotel.Eventos.Where(d => !d.DataSaida.HasValue && !d.DataExclusao.HasValue))
                    {
                        item.DataSaida = DateTime.Now;
                        item.DataAtualizacao = DateTime.Now.ToUniversalTime();
                        EventosAlerta.Add(item);
                    }
                }
                ResultadoOperacao Resultado = new ResultadoOperacao();

                if (Conectado)
                {
                    using (ApiService srv = new ApiService())
                    {
                        Resultado = await srv.SalvarHotel(ItemHotel);
                        if (Resultado.Sucesso)
                        {
                            pItemHotel = await srv.CarregarHotel(Resultado.IdentificadorRegistro);
                            AtualizarViagem(ItemViagem.Identificador.GetValueOrDefault(), "H", Resultado.IdentificadorRegistro.GetValueOrDefault(), !ItemHotel.Identificador.HasValue);
                            await DatabaseService.SalvarHotelReplicada(pItemHotel);
                            if (NovoCheckin)
                                foreach (var itemEvento in pItemHotel.Eventos)
                                {
                                    AtualizarViagem(ItemViagem.Identificador.GetValueOrDefault(), "HE", itemEvento.Identificador.GetValueOrDefault(), true);

                                }
                            foreach (var itemEvento in EventosAlerta)
                                AtualizarViagem(ItemViagem.Identificador.GetValueOrDefault(), "HE", itemEvento.Identificador.GetValueOrDefault(), false);

                        }
                    }
                }
                else
                {
                    Resultado = await DatabaseService.SalvarHotel(ItemHotel);
                    if (Resultado.Sucesso)
                        pItemHotel = await DatabaseService.CarregarHotel(ItemHotel.Identificador);
                }


                if (Resultado.Sucesso)
                {

                    MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.DisplayAlert, new MessagingServiceAlert()
                    {
                        Title = "Sucesso",
                        Message = String.Join(Environment.NewLine, Resultado.Mensagens.Select(d => d.Mensagem).ToArray()),
                        Cancel = "OK"
                    });
                    ItemHotel.Identificador = Resultado.IdentificadorRegistro;
                    // var Jresultado = (JObject)Resultado.ItemRegistro;
                    if (pItemHotel.Gastos == null)
                        pItemHotel.Gastos = new MvvmHelpers.ObservableRangeCollection<GastoHotel>();
                    if (pItemHotel.Eventos == null)
                        pItemHotel.Eventos = new MvvmHelpers.ObservableRangeCollection<HotelEvento>();
                    if (pItemHotel.HoraEntrada == null)
                        pItemHotel.HoraEntrada = new TimeSpan();
                    if (pItemHotel.HoraSaida == null)
                        pItemHotel.HoraSaida = new TimeSpan();
                    if (!pItemHotel.DataEntrada.HasValue)
                        pItemHotel.DataEntrada = _dataMinima;
                    if (!pItemHotel.DataSaidia.HasValue)
                        pItemHotel.DataSaidia = _dataMinima;
                    ItemHotel = pItemHotel;
                    MessagingService.Current.SendMessage<Hotel>(MessageKeys.ManutencaoHotel, ItemHotel);
                    MessagingService.Current.SendMessage<Hotel>(MessageKeys.ManutencaoHotelSelecao, ItemHotel);

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
                IsBusy = false;

                SalvarCommand.ChangeCanExecute();
            }
        }

        private void Excluir()
        {
            MessagingService.Current.SendMessage<MessagingServiceQuestion>(MessageKeys.DisplayQuestion, new MessagingServiceQuestion()
            {
                Title = "Confirmação",
                Question = String.Format("Deseja excluir essa hospedagem no Hotel?"),
                Positive = "Sim",
                Negative = "Não",
                OnCompleted = new Action<bool>(async result =>
                {
                    if (!result) return;
                    ResultadoOperacao Resultado = new ResultadoOperacao();
                    ItemHotel.DataExclusao = DateTime.Now.ToUniversalTime();
                    if (Conectado)
                    {
                        using (ApiService srv = new ApiService())
                        {
                            Resultado = await srv.ExcluirHotel(ItemHotel.Identificador);
                            await DatabaseService.ExcluirHotel(ItemHotel.Identificador, true);
                            AtualizarViagem(ItemViagem.Identificador.GetValueOrDefault(), "H", ItemHotel.Identificador.GetValueOrDefault(), false);
                        }
                    }
                    else
                    {
                        await DatabaseService.ExcluirHotel(ItemHotel.Identificador, false);
                        Resultado.Mensagens = new MensagemErro[] { new MensagemErro() { Mensagem = "Hotel Excluído com Sucesso " } };

                    }
                    MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.DisplayAlert, new MessagingServiceAlert()
                    {
                        Title = "Sucesso",
                        Message = String.Join(Environment.NewLine, Resultado.Mensagens.Select(d => d.Mensagem).ToArray()),
                        Cancel = "OK"
                    });

                    MessagingService.Current.SendMessage<Hotel>(MessageKeys.ManutencaoHotel, ItemHotel);
                    MessagingService.Current.SendMessage<Hotel>(MessageKeys.ManutencaoHotelSelecao, ItemHotel);

                    await PopAsync();

                })
            });


        }

        private async Task AbrirJanelaCustos()
        {
            var Pagina = new ListagemHotelCustoPage() { BindingContext = new ListagemHotelCustoViewModel(ItemViagem, ItemHotel) };
            await PushAsync(Pagina);
        }
    }
}
