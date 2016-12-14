using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Maps;
using TK.CustomMap.Overlays;
using CV.Mobile.Views;

namespace CV.Mobile.ViewModels
{
    public partial class PaginaTeste : ContentPage
    {
        public PaginaTeste()
        {
            InitializeComponent();

            var pin = new Pin()
            {
                Type = PinType.Place,
                Position = new Position(23,42),
                Label = "Teste",
                Address = "Teste"
            };

            var bounf = new MapSpan(new Position(24, 46), 10, 10);
            AcquaintanceMap.Pins.Clear();

            AcquaintanceMap.Pins.Add(pin);
        

        AcquaintanceMap.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(23, 42), Distance.FromMiles(10)));

            var googleImage = new Image
            {
                Source = "powered_by_google_on_white.png"
            };

            var searchFrom = new PlacesAutoComplete(false) { ApiToUse = PlacesAutoComplete.PlacesApi.Native,  Placeholder = "From" , Bounds= bounf };
            searchFrom.SetBinding(PlacesAutoComplete.PlaceSelectedCommandProperty, "FromSelectedCommand");
            var searchTo = new PlacesAutoComplete(false) { ApiToUse = PlacesAutoComplete.PlacesApi.Native,  Placeholder = "To", Bounds = bounf };
            searchTo.SetBinding(PlacesAutoComplete.PlaceSelectedCommandProperty, "ToSelectedCommand");

            if (Device.OS == TargetPlatform.Android)
            {
                this._baseLayout.Children.Add(
                    googleImage,
                    Constraint.Constant(10),
                    Constraint.RelativeToParent(l => l.Height - 30));
            }

            this._baseLayout.Children.Add(
                searchTo,
                yConstraint: Constraint.RelativeToView(searchFrom, (l, v) => searchFrom.HeightOfSearchBar + 10));

            this._baseLayout.Children.Add(
                searchFrom,
                Constraint.Constant(0),
                Constraint.Constant(10));

            this.BindingContext = new TesteRotaVM();

        }
    }
}
