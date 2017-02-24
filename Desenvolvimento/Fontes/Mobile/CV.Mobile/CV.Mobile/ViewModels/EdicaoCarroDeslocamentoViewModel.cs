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

    public class EdicaoCarroDeslocamentoViewModel : BaseNavigationViewModel
    {
        private CarroDeslocamento _ItemCarroDeslocamento;
        private bool _PermiteExcluir = true;
        private double _TamanhoGrid = 0;
        private bool _VisitaConcluida = false;
        private Usuario _ParticipanteSelecionado;
        private readonly DateTime _dataMinima = new DateTime(1900, 01, 01);
        private Plugin.Geolocator.Abstractions.Position _PosicaoAtual = null;

        public EdicaoCarroDeslocamentoViewModel(CarroDeslocamento pItemCarroDeslocamento)
        {
            ItemCarroDeslocamento = pItemCarroDeslocamento;
            if (pItemCarroDeslocamento.ItemCarroEventoChegada != null && pItemCarroDeslocamento.ItemCarroEventoChegada.Hora == null)
                pItemCarroDeslocamento.ItemCarroEventoChegada.Hora = new TimeSpan();

            if (pItemCarroDeslocamento.ItemCarroEventoChegada != null && !pItemCarroDeslocamento.ItemCarroEventoChegada.Data.HasValue)
                pItemCarroDeslocamento.ItemCarroEventoChegada.Data = _dataMinima;

            VisitaConcluida = pItemCarroDeslocamento.ItemCarroEventoChegada != null && pItemCarroDeslocamento.ItemCarroEventoChegada.Data.GetValueOrDefault(_dataMinima) != _dataMinima;

            Participantes = new ObservableCollection<Usuario>();

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
            VisitaConcluidaToggledCommand = new Command<ToggledEventArgs>(
                                                        (obj) => VerificarAcaoConcluidoItem(obj));

        }



        public Command SalvarCommand { get; set; }
        public Command PageAppearingCommand { get; set; }
        public Command ExcluirCommand { get; set; }
        public Command AbrirCustosCommand { get; set; }
        public Command VisitaConcluidaToggledCommand { get; set; }


        public ObservableCollection<Usuario> Participantes { get; set; }


        private void VerificarAcaoConcluidoItem(ToggledEventArgs obj)
        {

            if (obj.Value)
            {
                if (ItemCarroDeslocamento.ItemCarroEventoChegada.Data.GetValueOrDefault(_dataMinima) == _dataMinima)
                {

                    ItemCarroDeslocamento.ItemCarroEventoChegada.Data = DateTime.Today;
                    ItemCarroDeslocamento.ItemCarroEventoChegada.Hora = DateTime.Now.TimeOfDay;
                }
            }
            else
            {

                ItemCarroDeslocamento.ItemCarroEventoChegada.Data = _dataMinima;
                ItemCarroDeslocamento.ItemCarroEventoChegada.Hora = new TimeSpan();
            }
            
        }

        public async Task CarregarPosicao()
        {
            await Task.Delay(100);
            PermiteExcluir = ItemCarroDeslocamento.Identificador.HasValue;
            await CarregarParticipantesViagem();


            var posicao = await RetornarPosicao();
            if (posicao == null)
                posicao = new Plugin.Geolocator.Abstractions.Position() { Latitude = 0, Longitude = 0 };
            _PosicaoAtual = posicao;
            if (!ItemCarroDeslocamento.ItemCarroEventoPartida.Latitude.HasValue && !ItemCarroDeslocamento.ItemCarroEventoPartida.Longitude.HasValue)
            {
                ItemCarroDeslocamento.ItemCarroEventoPartida.Longitude = posicao.Longitude;
                ItemCarroDeslocamento.ItemCarroEventoPartida.Latitude = posicao.Latitude;
            }
        }

        private async Task CarregarParticipantesViagem()
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
                    if (ItemCarroDeslocamento.Usuarios.Where(d => d.IdentificadorUsuario == itemUsuario.Identificador).Any())
                        itemUsuario.Selecionado = true;
                    Participantes.Add(itemUsuario);
                }
                TamanhoGrid = Participantes.Count() * 18;


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

        public bool VisitaConcluida
        {
            get
            {
                return _VisitaConcluida;
            }

            set
            {
                SetProperty(ref _VisitaConcluida, value);
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



        public CarroDeslocamento ItemCarroDeslocamento
        {
            get
            {
                return _ItemCarroDeslocamento;
            }

            set
            {
                _ItemCarroDeslocamento = value;
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
                var ItemSalvar = ItemCarroDeslocamento.Clone();
                ItemSalvar.ItemCarroEventoChegada = ItemCarroDeslocamento.ItemCarroEventoChegada.Clone();
                ItemSalvar.ItemCarroEventoPartida = ItemCarroDeslocamento.ItemCarroEventoPartida.Clone();
                foreach (Usuario itemUsuario in Participantes)
                {
                    if (itemUsuario.Selecionado)
                    {
                        if (!ItemSalvar.Usuarios.Where(d => d.IdentificadorUsuario == itemUsuario.Identificador).Any())
                        {
                            var itemNovaAvaliacao = new CarroDeslocamentoUsuario() { IdentificadorUsuario = itemUsuario.Identificador };
                            ItemSalvar.Usuarios.Add(itemNovaAvaliacao);
                        }
                    }
                    else
                    {
                        if (ItemSalvar.Usuarios.Where(d => d.IdentificadorUsuario == itemUsuario.Identificador).Any())
                        {
                            ItemSalvar.Usuarios.Remove(ItemSalvar.Usuarios.Where(d => d.IdentificadorUsuario == itemUsuario.Identificador).FirstOrDefault());
                        }
                    }
                }
                ItemSalvar.ItemCarroEventoPartida.Data = DateTime.SpecifyKind(ItemSalvar.ItemCarroEventoPartida.Data.GetValueOrDefault().Date.Add(ItemSalvar.ItemCarroEventoPartida.Hora.GetValueOrDefault()), DateTimeKind.Unspecified);
                if (VisitaConcluida)
                {
                    ItemSalvar.ItemCarroEventoChegada.Data = DateTime.SpecifyKind(ItemSalvar.ItemCarroEventoChegada.Data.GetValueOrDefault().Date.Add(ItemSalvar.ItemCarroEventoChegada.Hora.GetValueOrDefault()), DateTimeKind.Unspecified);
                    if (_PosicaoAtual != null)
                    {
                        ItemSalvar.ItemCarroEventoChegada.Longitude = _PosicaoAtual.Longitude;
                        ItemSalvar.ItemCarroEventoChegada.Latitude = _PosicaoAtual.Latitude;
                    }
                }
                else
                {
                    ItemSalvar.ItemCarroEventoChegada.Data = null;
                    ItemSalvar.ItemCarroEventoChegada.Hora = null;
                    ItemSalvar.ItemCarroEventoChegada.Latitude = null;
                    ItemSalvar.ItemCarroEventoChegada.Longitude = null;
                }

                CarroDeslocamento pItemCarroDeslocamento = null;
                ResultadoOperacao Resultado = null;

                if (Conectado)
                {
                    using (ApiService srv = new ApiService())
                    {
                        Resultado = await srv.SalvarCarroDeslocamento(ItemSalvar);
                        if (Resultado.Sucesso)
                        {

                            pItemCarroDeslocamento = await srv.CarregarCarroDeslocamento(Resultado.IdentificadorRegistro);
                            AtualizarViagem(ItemViagemSelecionada.Identificador.GetValueOrDefault(), "CD", pItemCarroDeslocamento.Identificador.GetValueOrDefault(), !ItemCarroDeslocamento.Identificador.HasValue);
                            await DatabaseService.SalvarCarroDeslocamentoReplicada(pItemCarroDeslocamento);
                        }

                    }
                }
                else
                {
                    Resultado = await DatabaseService.SalvarCarroDeslocamento(ItemSalvar);
                    if (Resultado.Sucesso)
                     pItemCarroDeslocamento = await DatabaseService.CarregarCarroDeslocamento(ItemSalvar.Identificador);
                }


                if (Resultado.Sucesso)
                {
                    MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.DisplayAlert, new MessagingServiceAlert()
                    {
                        Title = "Sucesso",
                        Message = String.Join(Environment.NewLine, Resultado.Mensagens.Select(d => d.Mensagem).ToArray()),
                        Cancel = "OK"
                    });



                    PermiteExcluir = true;
                    await PopAsync();
                    MessagingService.Current.SendMessage<CarroDeslocamento>(MessageKeys.ManutencaoCarroDeslocamento, pItemCarroDeslocamento);

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
                Question = String.Format("Deseja excluir esse Deslocamento?"),
                Positive = "Sim",
                Negative = "Não",
                OnCompleted = new Action<bool>(async result =>
                {
                    if (!result) return;
                    ResultadoOperacao Resultado = new ResultadoOperacao();
                    ItemCarroDeslocamento.DataExclusao = DateTime.Now.ToUniversalTime();
                    if (Conectado)
                    {
                        using (ApiService srv = new ApiService())
                        {
                            Resultado = await srv.SalvarCarroDeslocamento(ItemCarroDeslocamento);
                            AtualizarViagem(ItemViagemSelecionada.Identificador.GetValueOrDefault(), "CD", ItemCarroDeslocamento.Identificador.GetValueOrDefault(), false);

                            await DatabaseService.ExcluirCarroDeslocamento(ItemCarroDeslocamento.Identificador, true);

                        }
                    }
                    else
                    {
                        ItemCarroDeslocamento.AtualizadoBanco = false;
                        await DatabaseService.ExcluirCarroDeslocamento(ItemCarroDeslocamento.Identificador, false);
                        Resultado.Mensagens = new MensagemErro[] { new MensagemErro() { Mensagem = "Deslocamento excluído com sucesso " } };
                    }
                    MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.DisplayAlert, new MessagingServiceAlert()
                    {
                        Title = "Sucesso",
                        Message = String.Join(Environment.NewLine, Resultado.Mensagens.Select(d => d.Mensagem).ToArray()),
                        Cancel = "OK"
                    });

                    MessagingService.Current.SendMessage<CarroDeslocamento>(MessageKeys.ManutencaoCarroDeslocamento, ItemCarroDeslocamento);
                    await PopAsync();

                })
            });


        }


    }
}
