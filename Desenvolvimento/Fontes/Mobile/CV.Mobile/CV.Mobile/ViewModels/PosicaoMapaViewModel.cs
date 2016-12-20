using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.Maps;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using TK.CustomMap;
using TK.CustomMap.Api;
using TK.CustomMap.Api.Google;
using TK.CustomMap.Api.OSM;
using FormsToolkit;
using CV.Mobile.Models;

namespace CV.Mobile.ViewModels
{
    public class PosicaoMapaViewModel: BaseNavigationViewModel
    {
        private MapSpan _LimiteMapa;
        private Position _posicaoSelecionada;
        private ObservableCollection<TKCustomMapPin> _pins;
        private Position _MapCenter;

        public PosicaoMapaViewModel(Position pPosicaoSelecionada)
        {
            _posicaoSelecionada = pPosicaoSelecionada;
            _LimiteMapa = MapSpan.FromCenterAndRadius(pPosicaoSelecionada, new Distance(20000));
            MapCenter = new Position(pPosicaoSelecionada.Latitude, pPosicaoSelecionada.Longitude);
            Pins = new ObservableCollection<TKCustomMapPin>();
            TKCustomMapPin itemPin = new TKCustomMapPin() { Position = new Position(pPosicaoSelecionada.Latitude, pPosicaoSelecionada.Longitude), IsDraggable=false };
            Pins.Add(itemPin);
        }

        public MapSpan LimiteMapa
        {
            get
            {
                return _LimiteMapa;
            }

            set
            {
                SetProperty(ref _LimiteMapa, value);
            }
        }

        public ObservableCollection<TKCustomMapPin> Pins
        {
            get
            {
                return _pins;
            }

            set
            {
                _pins = value;
                OnPropertyChanged();
            }
        }

        public Command<IPlaceResult> EnderecoMapaSelecionadoCommand
        {
            get
            {
                return new Command<IPlaceResult>(async p =>
                {
                    var gmsResult = p as GmsPlacePrediction;
                    if (gmsResult != null)
                    {
                        var details = await GmsPlace.Instance.GetDetails(gmsResult.PlaceId);
                        this.MapCenter = new Position(details.Item.Geometry.Location.Latitude, details.Item.Geometry.Location.Longitude);
                        return;
                    }
                    var osmResult = p as OsmNominatimResult;
                    if (osmResult != null)
                    {
                        this.MapCenter = new Position(osmResult.Latitude, osmResult.Longitude);
                        return;
                    }

                    if (Device.OS == TargetPlatform.Android)
                    {
                        var prediction = (TKNativeAndroidPlaceResult)p;

                        var details = await TKNativePlacesApi.Instance.GetDetails(prediction.PlaceId);

                        this.MapCenter = details.Coordinate;
                    }
                    else if (Device.OS == TargetPlatform.iOS)
                    {
                        var prediction = (TKNativeiOSPlaceResult)p;

                        this.MapCenter = prediction.Details.Coordinate;
                    }
                });
            }
        }

        public Command ConfirmarPosicaoCommand
        {
            get
            {
                return new Command( async () =>
                {
                    MessagingService.Current.SendMessage<Position>(MessageKeys.SelecaoMapaConfirmacao, Pins.FirstOrDefault().Position);
                    await PopAsync();
                });
            }
        }


        public Command<Position> MapClickedCommand
        {
            get
            {
                return new Command<Position>((positon) =>
                {
                    if (Pins.Any())
                    {
                        TKCustomMapPin itemPin = Pins.FirstOrDefault();
                        itemPin.Position = new Position(positon.Latitude, positon.Longitude);
                    }
                    else
                    {
                        TKCustomMapPin itemPin = new TKCustomMapPin() { Position = new Position(positon.Latitude, positon.Longitude), IsDraggable=false };
                        Pins.Add(itemPin);

                    }
                });
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
                SetProperty(ref _MapCenter, value);
            }
        }
    }
}
