using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TK.CustomMap;
using TK.CustomMap.Api;
using TK.CustomMap.Overlays;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace CV.Mobile.ViewModels
{
    public class TesteRotaVM
    {
        private IPlaceResult _fromPlace, _toPlace;
        private Position _from, _to;

        public ObservableCollection<TKCustomMapPin> Pins { get; private set; }
        public ObservableCollection<TKRoute> Routes { get; private set; }
        public MapSpan Bounds { get; private set; }

        public Command<IPlaceResult> FromSelectedCommand
        {
            get
            {
                return new Command<IPlaceResult>(async (p) =>
                {
                    if (Device.OS == TargetPlatform.iOS)
                    {
                        TKNativeiOSPlaceResult placeResult = (TKNativeiOSPlaceResult)p;
                        this._fromPlace = placeResult;
                        this._from = placeResult.Details.Coordinate;
                    }
                    else
                    {
                        TKNativeAndroidPlaceResult placeResult = (TKNativeAndroidPlaceResult)p;
                        this._fromPlace = placeResult;
                        var details = await TKNativePlacesApi.Instance.GetDetails(placeResult.PlaceId);

                        this._from = details.Coordinate;
                    }
                });
            }
        }
        public Command<IPlaceResult> ToSelectedCommand
        {
            get
            {
                return new Command<IPlaceResult>(async (p) =>
                {
                    if (Device.OS == TargetPlatform.iOS)
                    {
                        TKNativeiOSPlaceResult placeResult = (TKNativeiOSPlaceResult)p;
                        this._toPlace = placeResult;
                        this._to = placeResult.Details.Coordinate;
                    }
                    else
                    {
                        TKNativeAndroidPlaceResult placeResult = (TKNativeAndroidPlaceResult)p;
                        this._toPlace = placeResult;
                        var details = await TKNativePlacesApi.Instance.GetDetails(placeResult.PlaceId);

                        this._to = details.Coordinate;
                    }
                });
            }
        }

     
        public TesteRotaVM()
        {
            this.Routes = new ObservableCollection<TKRoute>();
            this.Pins = new ObservableCollection<TKCustomMapPin>();
            this.Bounds = new MapSpan(new Position(24,46), 10,10);
        }
    }
}





