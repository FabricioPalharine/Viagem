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
    public class MapaExibicaoViewModel : BaseViewModel
    {
        private readonly IGPSService _gps;
        private ObservableCollection<Pin> _pins = new ObservableCollection<Pin>();
        private Position? posicao = null;
        public MapaExibicaoViewModel(IGPSService gPSService)
        {
            _gps = gPSService;
        }

        public override async Task InitializeAsync(object navigationData)
        {
            if (navigationData != null && navigationData is PosicaoMapa item)
            {
               
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
                    MessagingCenter.Send<MapaExibicaoViewModel, Position>(this, MessageKeys.CentralizarMapa, position);
                    posicao = new Position(item.Latitude, item.Longitude);
                    Posicoes.Add(new Pin() { Position = posicao.Value, Label="Posição" });
                }
            }
        }

        public ICommand CancelarCommand => new Command(async () => await NavigationService.TrocarPaginaShell(".."));

      

        public ObservableCollection<Pin> Posicoes
        {
            get { return _pins; }
            set { SetProperty(ref _pins, value); }
        }

     
    }
}
