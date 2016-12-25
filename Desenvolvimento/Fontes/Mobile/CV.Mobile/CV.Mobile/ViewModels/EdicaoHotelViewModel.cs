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
        private MapSpan _Bounds;
        private bool _PermiteExcluir = true;
        private bool _PossoComentar = false;
        private bool _VisitaIniciada = false;
        private bool _VisitaConcluida = false;
        private HotelAvaliacao _ItemAvaliacao = new HotelAvaliacao();
        private Usuario _ParticipanteSelecionado;
        private readonly DateTime _dataMinima = new DateTime(1900, 01, 01);
        public EdicaoHotelViewModel(Hotel pItemHotel, Viagem pItemViagem)
        {

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
                    posicao = new Plugin.Geolocator.Abstractions.Position() { Latitude = -23.6040963, Longitude = -46.6178018 };
                Bounds = MapSpan.FromCenterAndRadius(new Position(posicao.Latitude, posicao.Longitude), new Distance(5000));
                ItemHotel.Longitude = posicao.Longitude;
                ItemHotel.Latitude = posicao.Latitude;
            }
           
            

        }

        private async void CarregarParticipantesViagem()
        {
            using (ApiService srv = new ApiService())
            {
                Participantes.Clear();
                var ListaUsuario = await srv.ListarParticipantesViagem();
                foreach (var itemUsuario in ListaUsuario)
                {
                    if (!ItemHotel.Identificador.HasValue || ItemHotel.Avaliacoes.Where(d => d.IdentificadorUsuario == itemUsuario.Identificador).Any())
                        itemUsuario.Selecionado = true;
                    Participantes.Add(itemUsuario);
                }
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
                        if (!ItemHotel.Avaliacoes.Where(d => d.IdentificadorUsuario == itemUsuario.Identificador).Any())
                        {
                            var itemNovaAvaliacao = new HotelAvaliacao() { IdentificadorUsuario = itemUsuario.Identificador };
                            if (itemUsuario.Identificador == ItemUsuarioLogado.Codigo)
                            {
                                itemNovaAvaliacao.Nota = ItemAvaliacao.Nota;
                                itemNovaAvaliacao.Comentario = ItemAvaliacao.Comentario;
                            }
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
              
                if (!VisitaIniciada)
                    ItemHotel.DataEntrada = null;
                else
                    ItemHotel.DataEntrada = ItemHotel.DataEntrada.GetValueOrDefault().Date.Add(ItemHotel.HoraEntrada.GetValueOrDefault());

                if (!VisitaConcluida)
                    ItemHotel.DataSaidia = null;
                else
                    ItemHotel.DataSaidia = ItemHotel.DataEntrada.GetValueOrDefault().Date.Add(ItemHotel.HoraSaida.GetValueOrDefault());
                using (ApiService srv = new ApiService())
                {
                    var Resultado = await srv.SalvarHotel(ItemHotel);
                    if (Resultado.Sucesso)
                    {

                        MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.DisplayAlert, new MessagingServiceAlert()
                        {
                            Title = "Sucesso",
                            Message = String.Join(Environment.NewLine, Resultado.Mensagens.Select(d => d.Mensagem).ToArray()),
                            Cancel = "OK"
                        });
                        ItemHotel.Identificador = Resultado.IdentificadorRegistro;
                        var Jresultado = (JObject)Resultado.ItemRegistro;
                        Hotel pItemHotel = Jresultado.ToObject<Hotel>();
 
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
                        ItemHotel.DataExclusao = DateTime.Now;
                        var Resultado = await srv.ExcluirHotel(ItemHotel.Identificador);
                        MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.DisplayAlert, new MessagingServiceAlert()
                        {
                            Title = "Sucesso",
                            Message = String.Join(Environment.NewLine, Resultado.Mensagens.Select(d => d.Mensagem).ToArray()),
                            Cancel = "OK"
                        });
                    }
                    MessagingService.Current.SendMessage<Hotel>(MessageKeys.ManutencaoHotel, ItemHotel);
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
