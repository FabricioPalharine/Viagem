using CV.Mobile.Helpers;
using CV.Mobile.Models;
using CV.Mobile.Services;
using CV.Mobile.Views;
using FormsToolkit;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Plugin.Geolocator.Abstractions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;


namespace CV.Mobile.ViewModels
{

    public class EdicaoCarroViewModel : BaseNavigationViewModel
    {
        private Carro _ItemCarro;

        private bool _PermiteExcluir = true;
        private bool _PossoComentar = false;
        private bool _VisitaIniciada = false;
        private bool _VisitaConcluida = false;
        private AvaliacaoAluguel _ItemAvaliacao = new AvaliacaoAluguel();
        private Usuario _ParticipanteSelecionado;
        private readonly DateTime _dataMinima = new DateTime(1900, 01, 01);
        private Position _UltimaPosicao = null;

        public EdicaoCarroViewModel(Carro pItemCarro, Viagem pItemViagem)
        {
            if (pItemCarro.ItemCarroEventoRetirada.Hora == null)
                pItemCarro.ItemCarroEventoRetirada.Hora = new TimeSpan();
            if (pItemCarro.ItemCarroEventoDevolucao.Hora == null)
                pItemCarro.ItemCarroEventoDevolucao.Hora = new TimeSpan();
            if (!pItemCarro.ItemCarroEventoRetirada.Data.HasValue)
                pItemCarro.ItemCarroEventoRetirada.Data = _dataMinima;
            if (!pItemCarro.ItemCarroEventoDevolucao.Data.HasValue)
                pItemCarro.ItemCarroEventoDevolucao.Data = _dataMinima;
            ItemCarro = pItemCarro;

            Participantes = new ObservableCollection<Usuario>();
            Participantes.CollectionChanged += Participantes_CollectionChanged;
            VisitaConcluida = pItemCarro.ItemCarroEventoDevolucao.Data.GetValueOrDefault(_dataMinima) != _dataMinima && ItemCarro.Alugado;
            VisitaIniciada = pItemCarro.ItemCarroEventoRetirada.Data.GetValueOrDefault(_dataMinima) != _dataMinima && ItemCarro.Alugado;
            PossoComentar = VisitaConcluida && pItemCarro.Avaliacoes.Where(d => d.IdentificadorUsuario == ItemUsuarioLogado.Codigo).Any() && ItemCarro.Alugado;
            ItemViagem = pItemViagem;
            ItemCarro.PropertyChanged += ItemCarro_PropertyChanged;

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
                                                                   (obj) => VerificarAcaoConcluidoItem(obj));

            VisitaIniciadaToggledCommand = new Command<ToggledEventArgs>(
                                                                   (obj) => VerificarAcaoIniciadaItem(obj));
            AbrirDeslocamentoCommand = new Command(async () => await AbrirJanelaDeslocamento());
            AbrirReabastecimentoCommand = new Command(async () => await AbrirJanelaReabastecimento());

            ExcluirCommand = new Command(() => Excluir());
            AbrirCustosCommand = new Command(async () => await AbrirJanelaCustos());

        }

        private void ItemCarro_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Alugado")
            {
                if (ItemCarro.Alugado)
                {
                    PossoComentar = Participantes.Where(d => d.Identificador == ItemUsuarioLogado.Codigo && d.Selecionado).Any() && VisitaConcluida;
                    ItemCarro.ItemCarroEventoRetirada.Hora = new TimeSpan();
                    ItemCarro.ItemCarroEventoDevolucao.Hora = new TimeSpan();
                    ItemCarro.ItemCarroEventoRetirada.Data = _dataMinima;
                    ItemCarro.ItemCarroEventoDevolucao.Data = _dataMinima;
                }
                else
                {
                    VisitaConcluida = false;
                    VisitaIniciada = false;
                    PossoComentar = false;
                }
            }
        }



        private void VerificarAcaoConcluidoItem(ToggledEventArgs obj)
        {

            if (obj.Value)
            {
                if (ItemCarro.ItemCarroEventoDevolucao.Data.GetValueOrDefault(_dataMinima) == _dataMinima)
                {

                    ItemCarro.ItemCarroEventoDevolucao.Data = DateTime.Today;
                    ItemCarro.ItemCarroEventoDevolucao.Hora = DateTime.Now.TimeOfDay;
                    int? UltimoOdometro = null;
                    if (ItemCarro.Deslocamentos.Where(d => !d.DataExclusao.HasValue).Where(d => d.ItemCarroEventoChegada.Data.HasValue).Any())
                        UltimoOdometro = ItemCarro.Deslocamentos.Where(d => !d.DataExclusao.HasValue).Where(d => d.ItemCarroEventoChegada.Data.HasValue).Where(d => d.ItemCarroEventoChegada.Odometro.GetValueOrDefault(0) > 0).Max(d => d.ItemCarroEventoChegada.Odometro);
                    ItemCarro.ItemCarroEventoDevolucao.Odometro = UltimoOdometro;
                }
            }
            else
            {
                PossoComentar = false;
                ItemCarro.ItemCarroEventoDevolucao.Data = _dataMinima;
                ItemCarro.ItemCarroEventoDevolucao.Hora = new TimeSpan();
                ItemCarro.ItemCarroEventoDevolucao.Odometro = null;
            }
        }


        private void VerificarAcaoIniciadaItem(ToggledEventArgs obj)
        {
            if (obj.Value)
            {

                if (ItemCarro.ItemCarroEventoRetirada.Data.GetValueOrDefault(_dataMinima) == _dataMinima)
                {

                    ItemCarro.ItemCarroEventoRetirada.Data = DateTime.Today;
                    ItemCarro.ItemCarroEventoRetirada.Hora = DateTime.Now.TimeOfDay;
                }

            }
            else
            {
                VisitaConcluida = false;
                ItemCarro.ItemCarroEventoRetirada.Data = _dataMinima;
                ItemCarro.ItemCarroEventoRetirada.Hora = new TimeSpan();
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
                        PossoComentar = VisitaConcluida && ItemCarro.Alugado;
                    else
                        PossoComentar = false;

                }
            }
        }

        public Command AbrirReabastecimentoCommand { get; set; }
        public Command AbrirDeslocamentoCommand { get; set; }
        public Command VisitaConcluidaToggledCommand { get; set; }
        public Command VisitaIniciadaToggledCommand { get; set; }
        public Command SalvarCommand { get; set; }
        public Command PageAppearingCommand { get; set; }
        public Command ExcluirCommand { get; set; }
        public Command AbrirCustosCommand { get; set; }


        public Viagem ItemViagem { get; set; }

        public async Task CarregarPosicao()
        {
            await Task.Delay(100);
            if (ItemCarro.Avaliacoes.Where(d => d.IdentificadorUsuario == ItemUsuarioLogado.Codigo).Any())
                ItemAvaliacao = ItemCarro.Avaliacoes.Where(d => d.IdentificadorUsuario == ItemUsuarioLogado.Codigo).FirstOrDefault();
            PermiteExcluir = ItemCarro.Id.HasValue || ItemCarro.Identificador.HasValue;
            CarregarParticipantesViagem();

            var posicao = await RetornarPosicao();
            if (posicao == null)
                posicao = new Plugin.Geolocator.Abstractions.Position() { Latitude = -23.6040963, Longitude = -46.6178018 };
            _UltimaPosicao = posicao;

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
                    if (!ItemCarro.Identificador.HasValue || ItemCarro.Avaliacoes.Where(d => d.IdentificadorUsuario == itemUsuario.Identificador).Any())
                        itemUsuario.Selecionado = true;
                    Participantes.Add(itemUsuario);
                }

            }
        }




        public Carro ItemCarro
        {
            get
            {
                return _ItemCarro;
            }

            set
            {
                SetProperty(ref _ItemCarro, value);
            }
        }

        public ObservableCollection<Carro> ListaCarroPai { get; set; }
        public ObservableCollection<Usuario> Participantes { get; set; }



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
                        PossoComentar = Participantes.Where(d => d.Identificador == ItemUsuarioLogado.Codigo && d.Selecionado).Any() && ItemCarro.Alugado;
                }
                else
                {
                    PossoComentar = false;

                }
            }
        }

        public AvaliacaoAluguel ItemAvaliacao
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
                        if (!ItemCarro.Avaliacoes.Where(d => d.IdentificadorUsuario == itemUsuario.Identificador).Any())
                        {
                            var itemNovaAvaliacao = new AvaliacaoAluguel() { IdentificadorUsuario = itemUsuario.Identificador };
                            if (itemUsuario.Identificador == ItemUsuarioLogado.Codigo)
                            {
                                itemNovaAvaliacao.Nota = ItemAvaliacao.Nota;
                                itemNovaAvaliacao.Comentario = ItemAvaliacao.Comentario;
                            }
                            itemNovaAvaliacao.DataAtualizacao = DateTime.Now.ToUniversalTime();
                            ItemCarro.Avaliacoes.Add(itemNovaAvaliacao);
                        }
                    }
                    else
                    {
                        if (ItemCarro.Avaliacoes.Where(d => d.IdentificadorUsuario == itemUsuario.Identificador).Any())
                        {
                            ItemCarro.Avaliacoes.Remove(ItemCarro.Avaliacoes.Where(d => d.IdentificadorUsuario == itemUsuario.Identificador).FirstOrDefault());
                            ItemAvaliacao = new AvaliacaoAluguel();
                        }
                    }
                }
                if (ItemCarro.Alugado)
                {
                    if (!VisitaIniciada)
                    {
                        ItemCarro.ItemCarroEventoRetirada.Data = null;
                        ItemCarro.ItemCarroEventoRetirada.Latitude = ItemCarro.ItemCarroEventoDevolucao.Longitude = null;
                        ItemCarro.ItemCarroEventoRetirada.Odometro = null;
                    }
                    else
                    {
                        ItemCarro.ItemCarroEventoRetirada.Data = ItemCarro.ItemCarroEventoRetirada.Data.GetValueOrDefault().Date.Add(ItemCarro.ItemCarroEventoRetirada.Hora.GetValueOrDefault());
                        if (_UltimaPosicao != null)
                        {
                            ItemCarro.ItemCarroEventoRetirada.Latitude = _UltimaPosicao.Latitude;
                            ItemCarro.ItemCarroEventoRetirada.Latitude = _UltimaPosicao.Longitude;
                        }
                    }

                    if (!VisitaConcluida)
                    {
                        ItemCarro.ItemCarroEventoDevolucao.Data = null;
                        ItemCarro.ItemCarroEventoDevolucao.Latitude = ItemCarro.ItemCarroEventoDevolucao.Longitude = null;
                        ItemCarro.ItemCarroEventoDevolucao.Odometro = null;
                    }
                    else
                    {
                        ItemCarro.ItemCarroEventoDevolucao.Data = ItemCarro.ItemCarroEventoDevolucao.Data.GetValueOrDefault().Date.Add(ItemCarro.ItemCarroEventoDevolucao.Hora.GetValueOrDefault());
                        if (_UltimaPosicao != null)
                        {
                            ItemCarro.ItemCarroEventoDevolucao.Latitude = _UltimaPosicao.Latitude;
                            ItemCarro.ItemCarroEventoDevolucao.Latitude = _UltimaPosicao.Longitude;
                        }
                    }

                }
                else
                {
                    ItemCarro.ItemCarroEventoRetirada.Data = null;
                    ItemCarro.ItemCarroEventoRetirada.Latitude = ItemCarro.ItemCarroEventoDevolucao.Longitude = null;
                    ItemCarro.ItemCarroEventoRetirada.Odometro = null;
                    ItemCarro.ItemCarroEventoDevolucao.Data = null;
                    ItemCarro.ItemCarroEventoDevolucao.Latitude = ItemCarro.ItemCarroEventoDevolucao.Longitude = null;
                    ItemCarro.ItemCarroEventoDevolucao.Odometro = null;
                }



                ResultadoOperacao Resultado = null;
                Carro pItemCarro = null;
                if (Conectado)
                {
                    using (ApiService srv = new ApiService())
                    {
                        Resultado = await srv.SalvarCarro(ItemCarro);
                        if (Resultado.Sucesso)
                        {

                            pItemCarro = await srv.CarregarCarro(Resultado.IdentificadorRegistro);
                            AtualizarViagem(ItemViagemSelecionada.Identificador.GetValueOrDefault(), "C", pItemCarro.Identificador.GetValueOrDefault(), !ItemCarro.Identificador.HasValue);
                            await DatabaseService.SalvarCarroReplicada(pItemCarro);
                        }

                    }
                }
                else
                {
                    Resultado = await DatabaseService.SalvarCarro(ItemCarro);
                    pItemCarro = await DatabaseService.CarregarCarro(ItemCarro.Identificador);
                }
                if (Resultado.Sucesso)
                {

                    MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.DisplayAlert, new MessagingServiceAlert()
                    {
                        Title = "Sucesso",
                        Message = String.Join(Environment.NewLine, Resultado.Mensagens.Select(d => d.Mensagem).ToArray()),
                        Cancel = "OK"
                    });
                    ItemCarro.Identificador = Resultado.IdentificadorRegistro;
                    // var Jresultado = (JObject)Resultado.ItemRegistro;


                    if (pItemCarro.ItemCarroEventoRetirada.Hora == null)
                        pItemCarro.ItemCarroEventoRetirada.Hora = new TimeSpan();
                    if (pItemCarro.ItemCarroEventoDevolucao.Hora == null)
                        pItemCarro.ItemCarroEventoDevolucao.Hora = new TimeSpan();
                    if (!pItemCarro.ItemCarroEventoRetirada.Data.HasValue)
                        pItemCarro.ItemCarroEventoRetirada.Data = _dataMinima;
                    if (!pItemCarro.ItemCarroEventoDevolucao.Data.HasValue)
                        pItemCarro.ItemCarroEventoDevolucao.Data = _dataMinima;
                    ItemCarro = pItemCarro;
                    MessagingService.Current.SendMessage<Carro>(MessageKeys.ManutencaoCarro, ItemCarro);
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

        private void Excluir()
        {
            MessagingService.Current.SendMessage<MessagingServiceQuestion>(MessageKeys.DisplayQuestion, new MessagingServiceQuestion()
            {
                Title = "Confirmação",
                Question = String.Format("Deseja excluir esse Carro?"),
                Positive = "Sim",
                Negative = "Não",
                OnCompleted = new Action<bool>(async result =>
                {
                    if (!result) return;
                    ResultadoOperacao Resultado = new ResultadoOperacao();
                    ItemCarro.DataExclusao = DateTime.Now.ToUniversalTime();

                    if (Conectado)
                    {
                        using (ApiService srv = new ApiService())
                        {
                            Resultado = await srv.ExcluirCarro(ItemCarro.Identificador);
                            AtualizarViagem(ItemViagemSelecionada.Identificador.GetValueOrDefault(), "C", ItemCarro.Identificador.GetValueOrDefault(), false);

                            await DatabaseService.ExcluirCarro(ItemCarro.Identificador, true);
                        }
                    }
                    else
                    {
                        await DatabaseService.ExcluirCarro(ItemCarro.Identificador, false);
                        Resultado.Mensagens = new MensagemErro[] { new MensagemErro() { Mensagem = "Carro excluído com sucesso " } };

                    }

                    MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.DisplayAlert, new MessagingServiceAlert()
                    {
                        Title = "Sucesso",
                        Message = String.Join(Environment.NewLine, Resultado.Mensagens.Select(d => d.Mensagem).ToArray()),
                        Cancel = "OK"
                    });

                    MessagingService.Current.SendMessage<Carro>(MessageKeys.ManutencaoCarro, ItemCarro);
                    await PopAsync();

                })
            });


        }

        private async Task AbrirJanelaCustos()
        {
            var Pagina = new ListagemCarroCustoPage() { BindingContext = new ListagemCarroCustoViewModel(ItemViagem, ItemCarro) };
            await PushAsync(Pagina);
        }

        private async Task AbrirJanelaDeslocamento()
        {
            var Pagina = new ListagemCarroDeslocamentoPage() { BindingContext = new ListagemCarroDeslocamentoViewModel(ItemViagem, ItemCarro) };
            await PushAsync(Pagina);
        }

        private async Task AbrirJanelaReabastecimento()
        {
            var Pagina = new ListagemReabastecimentoPage() { BindingContext = new ListagemReabastecimentoViewModel(ItemViagem, ItemCarro) };
            await PushAsync(Pagina);
        }
    }
}
