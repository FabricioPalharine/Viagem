using CV.Mobile.Models;
using CV.Mobile.Services.GPS;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace CV.Mobile.ViewModels.Mapas
{
    public class MapaSelecaoViewModel: BaseViewModel
    {
        private string NomeMensagem = MessageKeys.SelecionarPosicao;
        private readonly IGPSService _gps;
        private ObservableCollection<Pin> _pins = new ObservableCollection<Pin>();
        private Position? posicao = null;
        public MapaSelecaoViewModel(IGPSService gPSService)
        {
            _gps = gPSService;
        }

        public override async Task InitializeAsync(object navigationData)
        {
            if (navigationData != null && navigationData is PosicaoMapa item)
            {
                if (!string.IsNullOrEmpty(item.NomeMensagem))
                    NomeMensagem = item.NomeMensagem;
                if (item.Latitude == 0 && item.Longitude == 0)
                {
                    var posicao = await _gps.RetornarPosicao();
                    if (posicao != null)
                    {
                        item.Longitude = posicao.Longitude;
                        item.Latitude = posicao.Latitude;
                    }                
                }
                if (item.Latitude != 0 || item.Longitude != 0)
                {
                    Xamarin.Forms.Maps.Position position = new Xamarin.Forms.Maps.Position(item.Latitude, item.Longitude);
                    MessagingCenter.Send<MapaSelecaoViewModel, Position>(this, MessageKeys.CentralizarMapa, position);
                    posicao = new Position(item.Latitude, item.Longitude);
                    Posicoes.Add(new Pin() { Position = posicao.Value, Label="Posição Selecionada" });
                }
            }
        }

        public ICommand CancelarCommand => new Command(async () => await NavigationService.TrocarPaginaShell(".."));
        public ICommand SelecionarCommand => new Command(async () => await SelecionarPosicao());

        public ICommand ClicarMapaCommand => new Command<MapClickedEventArgs>( (evento) =>
        {
            Posicoes.Clear();
            posicao = evento.Position;
            Posicoes.Add(new Pin() { Position = posicao.Value, Label="Posição Atual" });

        });

        public ObservableCollection<Pin> Posicoes
        {
            get { return _pins; }
            set { SetProperty(ref _pins, value); }
        }



        public async Task SelecionarPosicao()
        {
            if (posicao.HasValue)
            {
                PosicaoMapa itemPosicao = new PosicaoMapa() { Latitude = posicao.Value.Latitude, Longitude = posicao.Value.Longitude };
                MessagingCenter.Send<MapaSelecaoViewModel, PosicaoMapa>(this, NomeMensagem, itemPosicao);
            }
            await NavigationService.TrocarPaginaShell("..");

        }

        public ICommand PosicionarMapaCommand => new Command<string>( async (string query) =>
        {
            var posicoes = await Xamarin.Essentials.Geocoding.GetLocationsAsync(query);
            if (posicoes.Any())
            {
                var latitudeAtual = posicao.HasValue ? posicao.Value.Latitude : 0;
                var longitudeAtual = posicao.HasValue ? posicao.Value.Longitude : 0;
                double distanciaAtual = double.MaxValue;
                Position? posicaoAtual = null;
                foreach(var item in posicoes)
                {
                    var distacia = item.CalculateDistance(new Xamarin.Essentials.Location(latitudeAtual, longitudeAtual), DistanceUnits.Kilometers);
                    if (distacia < distanciaAtual)
                    {
                        distanciaAtual = distacia;
                        posicaoAtual = new Position(item.Latitude, item.Longitude);
                    }
                }

                posicao = posicaoAtual;
                PosicaoMapa itemPosicao = new PosicaoMapa() { Latitude = posicao.Value.Latitude, Longitude = posicao.Value.Longitude };
                MessagingCenter.Send<MapaSelecaoViewModel, Position>(this, MessageKeys.CentralizarMapa, posicao.Value);

            }
        });
    }
}
