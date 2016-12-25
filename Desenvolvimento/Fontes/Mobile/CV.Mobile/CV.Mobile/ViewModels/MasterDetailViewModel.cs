using CV.Mobile.Models;
using CV.Mobile.Services;
using CV.Mobile.Views;
using FormsToolkit;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CV.Mobile.ViewModels
{
    public class MasterDetailViewModel : BaseNavigationViewModel
    {
        private MenuPage masterPage;
        private Page detailPage;
        private UsuarioLogado _ItemUsuario;
        private Viagem _ItemViagem;
        private IGeolocator locator;
        private Position ultimaPosicao = null;
        private Hotel _hotelAtual = null;
        private bool HotelDentro = false;

        public MasterDetailViewModel(UsuarioLogado itemUsuario)
        {
            locator = CrossGeolocator.Current;
            locator.DesiredAccuracy = 50;
            locator.AllowsBackgroundUpdates = true;
            locator.PositionChanged += Locator_PositionChanged;
            MenuViewModel vmMenu = new MenuViewModel(itemUsuario);
            _ItemUsuario = itemUsuario;
            vmMenu.ItemMenuSelecionado += async (itemMenu, novoStack) => { await SelecionarItemMenu(itemMenu, novoStack); };
            var paginaMenu = new MenuPage() { BindingContext = vmMenu };
            MasterPage = paginaMenu;
            var paginaDetalhe = new MenuInicialPage() { BindingContext = new MenuInicialViewModel() };
            DetailPage = paginaDetalhe;
            MessagingService.Current.Subscribe<Hotel>(MessageKeys.ManutencaoHotel, (service, item) =>
            {
                var hotel = _hotelAtual ?? new Hotel();
               if (item.Identificador == (int) hotel.Identificador)
                {
                    if (item.DataSaidia.HasValue || item.DataExclusao.HasValue)
                        _hotelAtual = null;
                    else if (item.DataEntrada.HasValue)
                    {
                        _hotelAtual = item;
                        HotelDentro = _hotelAtual.Eventos.Where(d => d.IdentificadorUsuario == ItemUsuarioLogado.Codigo).Where(d => !d.DataSaida.HasValue).Any();

                    }
                }
               else
                {
                    if (item.DataEntrada.HasValue && !item.DataExclusao.HasValue && !item.DataSaidia.HasValue)
                    {
                        _hotelAtual = item;
                        HotelDentro = _hotelAtual.Eventos.Where(d => d.IdentificadorUsuario == ItemUsuarioLogado.Codigo).Where(d => !d.DataSaida.HasValue).Any();

                    }
                }
            });
        }

        private async void Locator_PositionChanged(object sender, PositionEventArgs e)
        {
            if (_ItemViagem != null)
            {
                Posicao itemPosicao = new Posicao()
                {
                    DataGMT = e.Position.Timestamp.DateTime,
                    DataLocal = DateTime.Now,
                    IdentificadorUsuario = _ItemUsuario.Codigo,
                    IdentificadorViagem = _ItemViagem.Identificador,
                    Latitude = e.Position.Latitude,
                    Longitude = e.Position.Longitude,
                    Velocidade = e.Position.Speed
                    
                };
                ultimaPosicao = e.Position;
                using (ApiService srv = new ApiService())
                {
                    await srv.SalvarPosicao(itemPosicao);
                }
            }
            if (_hotelAtual != null && _hotelAtual.Raio > 0 && _hotelAtual.Latitude.HasValue && _hotelAtual.Longitude.HasValue)
            {
                var DistanciaAtual = GetDistanceTo(new Position() { Longitude = _hotelAtual.Longitude.Value, Latitude = _hotelAtual.Longitude.Value }, e.Position);
                bool estaDentro = DistanciaAtual.Meters <= _hotelAtual.Raio;
                if (estaDentro != HotelDentro)
                {
                    HotelEvento itemEvento = new HotelEvento() { DataEntrada = DateTime.Now, IdentificadorHotel = _hotelAtual.Identificador, IdentificadorUsuario = ItemUsuarioLogado.Codigo };
                    if (estaDentro)
                        itemEvento.DataSaida = DateTime.Now;
                    using (ApiService srv = new ApiService())
                    {
                        await srv.SalvarHotelEvento(itemEvento);
                    }

                }
            }
        }

        public MenuPage MasterPage
        {
            get
            {
                return masterPage;
            }

            set
            {
                SetProperty(ref masterPage, value);
            }
        }

        public Page DetailPage
        {
            get
            {
                return detailPage;
            }

            set
            {
                SetProperty(ref detailPage, value);
            }
        }

        public bool IsPresented
        {
            get
            {
                return isPresented;
            }

            set
            {
                SetProperty(ref isPresented, value);
            }
        }

        public UsuarioLogado ItemUsuario
        {
            get
            {
                return _ItemUsuario;
            }

            set
            {
                SetProperty(ref _ItemUsuario, value);
            }
        }

        public Viagem ItemViagem
        {
            get
            {
                return _ItemViagem;
            }

            set
            {
                SetProperty(ref _ItemViagem, value);
                CarregarHotelAtual();
                (masterPage.BindingContext as MenuViewModel).ItemViagem = value;
            }
        }

        private async void CarregarHotelAtual()
        {
            using (ApiService srv = new ApiService())
            {
                var Hoteis = await srv.ListarHotel(new CriterioBusca() { Situacao = 1 });
                if (Hoteis.Any())
                {
                    _hotelAtual = await srv.CarregarHotel(Hoteis.OrderByDescending(d => d.DataEntrada).Select(d => d.Identificador).FirstOrDefault());
                    HotelDentro = _hotelAtual.Eventos.Where(d => d.IdentificadorUsuario == ItemUsuarioLogado.Codigo).Where(d => !d.DataSaida.HasValue).Any();
                }
            }
        }

        public Xamarin.Forms.Maps.Distance GetDistanceTo(Position posicao, Position other)
        {
            var d1 = posicao.Latitude * (Math.PI / 180.0);
            var num1 = posicao.Longitude * (Math.PI / 180.0);
            var d2 = other.Latitude * (Math.PI / 180.0);
            var num2 = other.Longitude * (Math.PI / 180.0) - num1;
            var d3 = Math.Pow(Math.Sin((d2 - d1) / 2.0), 2.0) +
                     Math.Cos(d1) * Math.Cos(d2) * Math.Pow(Math.Sin(num2 / 2.0), 2.0);

            var meters = 6376500.0 * (2.0 * Math.Atan2(Math.Sqrt(d3), Math.Sqrt(1.0 - d3)));
            return Xamarin.Forms.Maps.Distance.FromMeters(meters);
        }

        public async Task IniciarControlePosicao()
        {
            if (locator.IsGeolocationEnabled && locator.IsGeolocationAvailable)
            {
                if (_ItemViagem != null && _ItemViagem.Edicao && _ItemViagem.Aberto && _ItemViagem.DataInicio < DateTime.Now)
                {
                    if (!locator.IsListening)
                        await locator.StartListeningAsync(15, 1, true);
                }
                else
                {
                    if (locator.IsListening)
                        await locator.StopListeningAsync();
                }
            }
        }

        public async Task<Position> RetornarPosicaoGPS()
        {
            if (ultimaPosicao == null)
            {
                try
                {
                    var position = await locator.GetPositionAsync(timeoutMilliseconds: 10000);
                    return position;
                }
                catch
                {
                    return null;
                }
            }
            else
                return ultimaPosicao;
        }

        private bool isPresented;

        private async Task SelecionarItemMenu(Page itemMenu, bool NovoStack)
        {
            (Application.Current.MainPage as Xamarin.Forms.MasterDetailPage).IsPresented = false;

            if (NovoStack)
            {
                (Application.Current.MainPage as Xamarin.Forms.MasterDetailPage).Detail = new NavigationPage(itemMenu);
            }
            else
            {
                await PushAsync(itemMenu);
            }

        }
    }
}
