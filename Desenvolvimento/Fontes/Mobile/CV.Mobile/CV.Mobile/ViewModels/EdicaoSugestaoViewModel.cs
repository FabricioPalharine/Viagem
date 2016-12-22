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
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace CV.Mobile.ViewModels
{

    public class EdicaoSugestaoViewModel : BaseNavigationViewModel
    {
        private Sugestao _ItemSugestao;
        private MapSpan _Bounds;
        public EdicaoSugestaoViewModel(Sugestao pItemSugestao)
        {
            ItemSugestao = pItemSugestao;
            SalvarCommand = new Command(
                                async () => await Salvar(),
                                () => true);
            PageAppearingCommand = new Command(
                                                                  async () =>
                                                                  {
                                                                      await CarregarPosicao();
                                                                  },
                                                                  () => true);
            SelecionarPosicaoCommand = new Command(async () => await AbrirPosicaoMapa());

            MessagingService.Current.Subscribe<Position>(MessageKeys.SelecaoMapaConfirmacao, (service, item) =>
            {
                Bounds = MapSpan.FromCenterAndRadius(item, new Distance(5000));
                ItemSugestao.Longitude = item.Longitude;
                ItemSugestao.Latitude = item.Latitude;
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

        public Command SalvarCommand { get; set; }
        public Command PageAppearingCommand { get; set; }
        public Command SelecionarPosicaoCommand { get; set; }

        public Command<GmsSearchResults> PlaceSelectedCommand
        {
            get
            {
                return new Command<GmsSearchResults>(p =>
                {
                    ItemSugestao.Local = p.name;
                    ItemSugestao.CodigoPlace = p.place_id;
                    ItemSugestao.Longitude = p.geometry.location.lng;
                    ItemSugestao.Latitude = p.geometry.location.lat;
                    Bounds = MapSpan.FromCenterAndRadius(new Position(ItemSugestao.Latitude.GetValueOrDefault(), ItemSugestao.Longitude.GetValueOrDefault()), new Distance(5000));

                });
            }
        }

        public async Task CarregarPosicao()
        {
            if (_ItemSugestao.Latitude.HasValue && _ItemSugestao.Longitude.HasValue)
            {
                Bounds = MapSpan.FromCenterAndRadius(new Position(_ItemSugestao.Latitude.Value, _ItemSugestao.Longitude.Value), new Distance(5000));

            }
            else
            {
                var posicao = await RetornarPosicao();
                if (posicao == null)
                    posicao = new Plugin.Geolocator.Abstractions.Position() { Latitude = -23.6040963, Longitude = -46.6178018 };
                Bounds = MapSpan.FromCenterAndRadius(new Position(posicao.Latitude, posicao.Longitude), new Distance(5000));
            }

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

        private async Task Salvar()
        {
            IsBusy = true;
            SalvarCommand.ChangeCanExecute();
            try
            {
                using (ApiService srv = new ApiService())
                {
                    var Resultado = await srv.SalvarSugestao(ItemSugestao);
                    if (Resultado.Sucesso)
                    {

                        MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.DisplayAlert, new MessagingServiceAlert()
                        {
                            Title = "Sucesso",
                            Message = String.Join(Environment.NewLine, Resultado.Mensagens.Select(d => d.Mensagem).ToArray()),
                            Cancel = "OK"
                        });
                        ItemSugestao.Identificador = Resultado.IdentificadorRegistro;
                        // ItemListaCompra = JsonConvert.DeserializeXNode < ListaCompra >()
                        MessagingService.Current.SendMessage<Sugestao>(MessageKeys.ManutencaoSugestao, ItemSugestao);
                        await PopAsync();
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
    }
}
