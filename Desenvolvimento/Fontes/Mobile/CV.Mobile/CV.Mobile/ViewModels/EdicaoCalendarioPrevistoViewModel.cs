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
    public class EdicaoCalendarioPrevistoViewModel : BaseNavigationViewModel
    {
        private CalendarioPrevisto _ItemCalendarioPrevisto;
        private bool _PermiteExcluir = true;

        private MapSpan _Bounds;
        private Position _MapCenter;
        public EdicaoCalendarioPrevistoViewModel(CalendarioPrevisto pItemCalendarioPrevisto )
        {
            ItemCalendarioPrevisto = pItemCalendarioPrevisto;
            ExcluirCommand = new Command(
                                 () =>  Cancelar(),
                                () => true);
            PageAppearingCommand = new Command(async () => await CarregarPagina(), () => true);
            SalvarCommand = new Command(async () => await Salvar(), () => true);
            SelecionarPosicaoCommand = new Command(async () => await AbrirPosicaoMapa());
            ListaStatus = new ObservableCollection<ItemLista>();
            ListaStatus.Add(new ItemLista() { Codigo = "1", Descricao = "Baixa" });
            ListaStatus.Add(new ItemLista() { Codigo = "2", Descricao = "Média" });
            ListaStatus.Add(new ItemLista() { Codigo = "3", Descricao = "Alta" });
            MessagingService.Current.Unsubscribe<Position>(MessageKeys.SelecaoMapaConfirmacao);
            MessagingService.Current.Subscribe<Position>(MessageKeys.SelecaoMapaConfirmacao, (service, item) =>
            {
                Bounds = MapSpan.FromCenterAndRadius(item, new Distance(5000));
                ItemCalendarioPrevisto.Longitude = item.Longitude;
                ItemCalendarioPrevisto.Latitude = item.Latitude;
            });

        }

        private async Task AbrirPosicaoMapa()
        {
            if (Bounds == null)
                await CarregarPosicao();
            var vm = new PosicaoMapaViewModel(Bounds.Center);
            var Pagina = new PosicaoMapaPage() { BindingContext = vm };
            await PushAsync(Pagina);
        }


        private async Task Salvar()
        {
            IsBusy = true;
            SalvarCommand.ChangeCanExecute();
            try
            {
                ItemCalendarioPrevisto.DataInicio = ItemCalendarioPrevisto.DataInicio.GetValueOrDefault().Date.Add(ItemCalendarioPrevisto.HoraInicio.Value);
                ItemCalendarioPrevisto.DataFim = ItemCalendarioPrevisto.DataFim.GetValueOrDefault().Date.Add(ItemCalendarioPrevisto.HoraFim.Value);
                ResultadoOperacao Resultado = new ResultadoOperacao();
                if (Conectado)
                {
                    using (ApiService srv = new ApiService())
                    {
                        Resultado = await srv.SalvarCalendarioPrevisto(ItemCalendarioPrevisto);
                        if (Resultado.Sucesso)
                        {
                            var itemRetorno = await srv.CarregarCalendarioPrevisto(Resultado.IdentificadorRegistro);
                            var itemBase = await DatabaseService.Database.CarregarCalendarioPrevisto(Resultado.IdentificadorRegistro.GetValueOrDefault());
                            if (itemBase != null)
                                itemRetorno.Id = itemBase.Id;
                            await DatabaseService.Database.SalvarCalendarioPrevisto(itemRetorno);
                            base.AtualizarViagem(ItemViagemSelecionada.Identificador.GetValueOrDefault(), "CP", itemRetorno.Identificador.GetValueOrDefault(), !ItemCalendarioPrevisto.Identificador.HasValue);
                        }
                    }
                }
                else
                {
                    Resultado = await DatabaseService.SalvarCalendarioPrevisto(ItemCalendarioPrevisto);
                }
                if (Resultado.Sucesso)
                {

                    MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.DisplayAlert, new MessagingServiceAlert()
                    {
                        Title = "Sucesso",
                        Message = String.Join(Environment.NewLine, Resultado.Mensagens.Select(d => d.Mensagem).ToArray()),
                        Cancel = "OK"
                    });

                    // ItemListaCompra = JsonConvert.DeserializeXNode < ListaCompra >()
                    ItemCalendarioPrevisto.Identificador = Resultado.IdentificadorRegistro;
                    MessagingService.Current.SendMessage<CalendarioPrevisto>(MessageKeys.ManutencaoCalendarioPrevisto, ItemCalendarioPrevisto);
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

        public Command ExcluirCommand { get; set; }
        public Command PageAppearingCommand { get; set; }

        public Command SalvarCommand { get; set; }

        public Command SelecionarPosicaoCommand { get; set; }
        public Command<GmsSearchResults> PlaceSelectedCommand
        {
            get
            {
                return new Command<GmsSearchResults>(p =>
                {
                    ItemCalendarioPrevisto.Nome = p.name;
                    ItemCalendarioPrevisto.CodigoPlace = p.place_id;
                    ItemCalendarioPrevisto.Longitude = p.geometry.location.lng;
                    ItemCalendarioPrevisto.Latitude = p.geometry.location.lat;
                    Bounds = MapSpan.FromCenterAndRadius(new Position(ItemCalendarioPrevisto.Latitude.GetValueOrDefault(), ItemCalendarioPrevisto.Longitude.GetValueOrDefault()), new Distance(5000));

                });
            }
        }
        public ObservableCollection<ItemLista> ListaStatus { get; set; }

        public async Task CarregarPagina()
        {
            await Task.Delay(200);
            PermiteExcluir = ItemCalendarioPrevisto.Identificador.HasValue;
            if (Bounds == null)
                await CarregarPosicao();
 
        }
        public async Task CarregarPosicao()
        {
            if (_ItemCalendarioPrevisto.Latitude.HasValue && _ItemCalendarioPrevisto.Longitude.HasValue)
            {
                Bounds = MapSpan.FromCenterAndRadius(new Position(_ItemCalendarioPrevisto.Latitude.Value, _ItemCalendarioPrevisto.Longitude.Value), new Distance(5000));

            }
            else
            {
                var posicao = await RetornarPosicao();
                if (posicao == null)
                    posicao = new Plugin.Geolocator.Abstractions.Position() { Latitude = -23.6040963, Longitude = -46.6178018 };
                Bounds = MapSpan.FromCenterAndRadius(new Position(posicao.Latitude, posicao.Longitude), new Distance(5000));
            }

        }
       
        public CalendarioPrevisto ItemCalendarioPrevisto
        {
            get
            {
                return _ItemCalendarioPrevisto;
            }

            set
            {
                SetProperty(ref _ItemCalendarioPrevisto, value);
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
                Question = String.Format("Deseja excluir esse Agendamento?"),
                Positive = "Sim",
                Negative = "Não",
                OnCompleted = new Action<bool>(async result =>
                {
                    if (!result) return;
                    ItemCalendarioPrevisto.DataExclusao = DateTime.Now.ToUniversalTime();
                    ResultadoOperacao Resultado = new ResultadoOperacao();
                    if (Conectado)
                    {
                        using (ApiService srv = new ApiService())
                        {
                            Resultado = await srv.ExcluirCalendarioPrevisto(ItemCalendarioPrevisto.Identificador);
                            var itemBase = await DatabaseService.Database.CarregarCalendarioPrevisto(ItemCalendarioPrevisto.Identificador.GetValueOrDefault());
                            if (itemBase != null)

                                await DatabaseService.Database.ExcluirCalendarioPrevisto(itemBase);
                            base.AtualizarViagem(ItemViagemSelecionada.Identificador.GetValueOrDefault(), "CP", ItemCalendarioPrevisto.Identificador.GetValueOrDefault(),false);

                        }
                    }
                    else
                    {
                        var itemBase = await DatabaseService.Database.CarregarCalendarioPrevisto(ItemCalendarioPrevisto.Identificador.GetValueOrDefault());
                        if (itemBase != null)
                        {
                            if (itemBase.Identificador > 0)
                            {
                                itemBase.AtualizadoBanco = false;
                                itemBase.DataExclusao = DateTime.Now.ToUniversalTime();
                                await DatabaseService.Database.SalvarCalendarioPrevisto(itemBase);
                            }
                            else
                                await DatabaseService.Database.ExcluirCalendarioPrevisto(itemBase);

                            Resultado.Mensagens = new MensagemErro[] { new MensagemErro() { Mensagem = "Agendamento Excluído com Sucesso " } };

                        }
                    }

                    MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.DisplayAlert, new MessagingServiceAlert()
                    {
                        Title = "Sucesso",
                        Message = String.Join(Environment.NewLine, Resultado.Mensagens.Select(d => d.Mensagem).ToArray()),
                        Cancel = "OK"
                    });
                    MessagingService.Current.SendMessage<CalendarioPrevisto>(MessageKeys.ManutencaoCalendarioPrevisto, ItemCalendarioPrevisto);
                    await PopAsync();

                })
            });

           
        }
    }
}
