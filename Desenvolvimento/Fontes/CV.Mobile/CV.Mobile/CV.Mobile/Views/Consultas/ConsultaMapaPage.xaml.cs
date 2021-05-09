using CV.Mobile.Models;
using CV.Mobile.ViewModels.Consultas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace CV.Mobile.Views.Consultas
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConsultaMapaPage : ContentPage
    {
        public ConsultaMapaPage()
        {
            InitializeComponent();

            MessagingCenter.Unsubscribe<ConsultaMapaViewModel, Position>(this, MessageKeys.CentralizarMapa);
            MessagingCenter.Subscribe<ConsultaMapaViewModel, Position>(this, MessageKeys.CentralizarMapa, (sender, posicao) =>
            {
                mapa.MoveToRegion(MapSpan.FromCenterAndRadius(posicao, Distance.FromKilometers(5)));

            });

        }
    }
}