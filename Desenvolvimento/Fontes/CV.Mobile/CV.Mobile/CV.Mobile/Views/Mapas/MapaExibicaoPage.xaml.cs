using CV.Mobile.Models;
using CV.Mobile.ViewModels.Mapas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace CV.Mobile.Views.Mapas
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MapaExibicaoPage : ContentPage
    {
        public MapaExibicaoPage()
        {

            InitializeComponent();
            MessagingCenter.Unsubscribe<MapaExibicaoViewModel, Position>(this, MessageKeys.CentralizarMapa);
            MessagingCenter.Subscribe<MapaExibicaoViewModel, Position>(this, MessageKeys.CentralizarMapa, (sender, posicao) =>
             {
                 mapa.MoveToRegion(MapSpan.FromCenterAndRadius(posicao, Distance.FromKilometers(10)));

             });
        }
    }
}