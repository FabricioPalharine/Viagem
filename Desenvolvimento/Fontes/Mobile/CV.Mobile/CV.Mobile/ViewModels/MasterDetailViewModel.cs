using CV.Mobile.Models;
using CV.Mobile.Services;
using CV.Mobile.Views;
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
                using (ApiService srv = new ApiService())
                {
                    await srv.SalvarPosicao(itemPosicao);
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
                (masterPage.BindingContext as MenuViewModel).ItemViagem = value;
            }
        }

        public async Task IniciarControlePosicao()
        {
            if (locator.IsGeolocationEnabled && locator.IsGeolocationAvailable)
            {
                if (_ItemViagem != null && _ItemViagem.Edicao && _ItemViagem.Aberto)
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
