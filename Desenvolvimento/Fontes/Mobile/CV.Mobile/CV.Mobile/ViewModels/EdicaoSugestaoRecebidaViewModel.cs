using CV.Mobile.Helpers;
using CV.Mobile.Models;
using CV.Mobile.Services;
using CV.Mobile.Views;
using FormsToolkit;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TK.CustomMap;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace CV.Mobile.ViewModels
{
    public class EdicaoSugestaoRecebidaViewModel : BaseNavigationViewModel
    {
        private Sugestao _ItemSugestao;
        private bool _PermiteCancelar = true;

        private MapSpan _LimiteMapa;
        private Position _MapCenter;
        private ObservableCollection<TKCustomMapPin> _pins;
        public EdicaoSugestaoRecebidaViewModel(Sugestao pItemSugestao )
        {
            ItemSugestao = pItemSugestao;
            CancelarCommand = new Command(
                                 () =>  Cancelar(),
                                () => true);
            PageAppearingCommand = new Command(async () => await CarregarPagina(), () => true);
            AgendarCommand = new Command(async () => await AbrirAgendamento(), () => true);
              MapCenter = new Position(ItemSugestao.Latitude.GetValueOrDefault(), ItemSugestao.Longitude.GetValueOrDefault());
            LimiteMapa = MapSpan.FromCenterAndRadius(new Position(ItemSugestao.Latitude.GetValueOrDefault(), ItemSugestao.Longitude.GetValueOrDefault()), new Distance(500));

            Pins = new ObservableCollection<TKCustomMapPin>();
            TKCustomMapPin itemPin = new TKCustomMapPin() { Position= MapCenter, IsDraggable = false };
            Pins.Add(itemPin);

        }

        public Command CancelarCommand { get; set; }
        public Command PageAppearingCommand { get; set; }

        public Command AgendarCommand { get; set; }

        public async Task CarregarPagina()
        {
            await Task.Delay(200);
            PermiteCancelar = ItemSugestao.Status <= 1;
            await Task.Delay(1000);
            LimiteMapa = MapSpan.FromCenterAndRadius(new Position(ItemSugestao.Latitude.GetValueOrDefault(), ItemSugestao.Longitude.GetValueOrDefault()), new Distance(500));

        }

        public async Task AbrirAgendamento()
        {
            var ItemCalendario = new CalendarioPrevisto() { AvisarHorario = false, CodigoPlace = ItemSugestao.CodigoPlace, DataFim = DateTime.Today, DataInicio = DateTime.Today, HoraFim = new TimeSpan(0, 0, 0), HoraInicio = new TimeSpan(0, 0, 0), Latitude = ItemSugestao.Latitude, Longitude = ItemSugestao.Longitude, Nome = ItemSugestao.Local, Prioridade = 1, Tipo = ItemSugestao.Tipo };
            var Pagina = new EdicaoAgendarSugestaoPage() { BindingContext = new EdicaoAgendarSugestaoViewModel(ItemSugestao, ItemCalendario) };
            await PushAsync(Pagina);
        }
        public Sugestao ItemSugestao
        {
            get
            {
                return _ItemSugestao;
            }

            set
            {
                SetProperty(ref _ItemSugestao, value);
            }
        }

        public bool PermiteCancelar
        {
            get
            {
                return _PermiteCancelar;
            }

            set
            {
                SetProperty(ref _PermiteCancelar, value); 
            }
        }

        public MapSpan LimiteMapa
        {
            get
            {
                return _LimiteMapa;
            }

            set
            {
                SetProperty(ref _LimiteMapa, value);
            }
        }

        public ObservableCollection<TKCustomMapPin> Pins
        {
            get
            {
                return _pins;
            }

            set
            {
                _pins = value;
                OnPropertyChanged();
            }
        }

        public Position MapCenter
        {
            get
            {
                return _MapCenter;
            }

            set
            {
                _MapCenter = value;
            }
        }

        private  void Cancelar()
        {
            MessagingService.Current.SendMessage<MessagingServiceQuestion>(MessageKeys.DisplayQuestion, new MessagingServiceQuestion()
            {
                Title = "Confirmação",
                Question = String.Format("Ao cancelar essa sugestão, não será mais possível agendá-la. Deseja Continuar?"),
                Positive = "Sim",
                Negative = "Não",
                OnCompleted = new Action<bool>(async result =>
                {
                    if (!result) return;
                    ResultadoOperacao Resultado = new ResultadoOperacao();
                    ItemSugestao.Status = 3;
                    ItemSugestao.DataAtualizacao = DateTime.Now.ToUniversalTime();
                    bool Executado = false;
                    if (Conectado)
                    {
                        try { 
                        using (ApiService srv = new ApiService())
                        {
                           
                            Resultado = await srv.SalvarSugestao(ItemSugestao);

                            var itemSugestao = await DatabaseService.Database.RetornarSugestao(ItemSugestao.Identificador);
                            if (itemSugestao != null)
                            {
                                ItemSugestao.Id = itemSugestao.Id;
                            }
                            ItemSugestao.AtualizadoBanco = true;
                            
                            await DatabaseService.Database.SalvarSugestao(ItemSugestao);

                        }
                            Executado = true;
                        }
                        catch { Executado = false; }
                    }
                    if (!Executado)
                    {
                        ItemSugestao.AtualizadoBanco = false;
                        var itemSugestao = await DatabaseService.Database.RetornarSugestao(ItemSugestao.Identificador);
                        if (itemSugestao != null)
                        {
                            ItemSugestao.Id = itemSugestao.Id;
                        }
                        await DatabaseService.Database.SalvarSugestao(ItemSugestao);
                        Resultado.Mensagens = new MensagemErro[] { new MensagemErro() { Mensagem = "Sugestão salva com Sucesso" } };
                    }

                    MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.DisplayAlert, new MessagingServiceAlert()
                    {
                        Title = "Sucesso",
                        Message = String.Join(Environment.NewLine, Resultado.Mensagens.Select(d => d.Mensagem).ToArray()),
                        Cancel = "OK"
                    });
                    MessagingService.Current.SendMessage<Sugestao>(MessageKeys.ManutencaoSugestao, ItemSugestao);
                    await PopAsync();

                })
            });

           
        }
    }
}
