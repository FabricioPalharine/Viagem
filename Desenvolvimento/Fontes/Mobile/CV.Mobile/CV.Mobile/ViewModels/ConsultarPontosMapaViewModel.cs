using CV.Mobile.Models;
using CV.Mobile.Services;
using CV.Mobile.Views;
using CV.Mobile.Helpers;
using FormsToolkit;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using MvvmHelpers;
using TK.CustomMap;
using Xamarin.Forms.Maps;
using TK.CustomMap.Overlays;

namespace CV.Mobile.ViewModels
{
    public class ConsultarPontosMapaViewModel : BaseNavigationViewModel
    {
        private CriterioBusca _itemCriterioBusca;

        private bool _IsLoadingLista;
        public ObservableRangeCollection<Usuario> ListaUsuario { get; set; }
        private ObservableCollection<TKCustomMapPin> _pins;
        private ObservableCollection<TKPolyline> _polylines;

      
        private MapSpan _LimiteMapa;
        private Position _MapCenter;
        public ConsultarPontosMapaViewModel()
        {
            _MapCenter = new Position(0, 0);
            _LimiteMapa = MapSpan.FromCenterAndRadius(_MapCenter, new Distance( 500000));
            ItemCriterioBusca = new CriterioBusca() { };

            PesquisarCommand = new Command(
                                                                    async () =>
                                                                    {
                                                                        await CarregarListaDados();
                                                                        var Pagina = new ConsultarMapaExibicaoPage() { BindingContext = this };
                                                                        await PushAsync(Pagina);
                                                                    },
                                                                    () => true);

            ListaTipo = new ObservableCollection<ItemLista>();
            ListaTipo.Add(new ItemLista() { Codigo = "", Descricao = "Todos" });
            ListaTipo.Add(new ItemLista() { Codigo = "A", Descricao = "Atração" });
            ListaTipo.Add(new ItemLista() { Codigo = "CR", Descricao = "Retirada Carro" });
            ListaTipo.Add(new ItemLista() { Codigo = "CD", Descricao = "Devolução Carro" });
            ListaTipo.Add(new ItemLista() { Codigo = "L", Descricao = "Loja" });
            ListaTipo.Add(new ItemLista() { Codigo = "RC", Descricao = "Reabastecimento" });
            ListaTipo.Add(new ItemLista() { Codigo = "P", Descricao = "Paradas Viagem" });
            ListaTipo.Add(new ItemLista() { Codigo = "T", Descricao = "Comentário" });
            ListaTipo.Add(new ItemLista() { Codigo = "V", Descricao = "Vídeo" });
            ListaTipo.Add(new ItemLista() { Codigo = "F", Descricao = "Foto" });
            ListaTipo.Add(new ItemLista() { Codigo = "R", Descricao = "Restaurante" });
            ListaTipo.Add(new ItemLista() { Codigo = "D", Descricao = "Trajetos" });
            ListaTipo.Add(new ItemLista() { Codigo = "U", Descricao = "Última Posição" });

            Pins = new ObservableCollection<TKCustomMapPin>();

            Polylines = new ObservableCollection<TKPolyline>();
        }

        public Command PageAppearingCommand
        {
            get
            {
                return new Command(
                                async () =>
                                {
                                    await CarregarListaUsuarios();
                                },
                                () => true);
            }
        }

        private async Task CarregarListaUsuarios()
        {
            if (ListaUsuario == null)
            {
                using (ApiService srv = new ApiService())
                {
                    ListaUsuario = new ObservableRangeCollection<Usuario>(await srv.CarregarParticipantesAmigo());
                    OnPropertyChanged("ListaUsuario");


                    if (ListaUsuario.Where(d => d.Identificador == ItemUsuarioLogado.Codigo).Any())
                        ItemCriterioBusca.IdentificadorParticipante = ItemUsuarioLogado.Codigo;
                    else
                        ItemCriterioBusca.IdentificadorParticipante = ListaUsuario.Select(d => d.Identificador).FirstOrDefault();
                }
            }
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

        public ObservableCollection<TKPolyline> Polylines
        {
            get
            {
                return _polylines;
            }

            set
            {
                _polylines = value;
                OnPropertyChanged();
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

        public ObservableCollection<ItemLista> ListaTipo { get; set; }
        public CriterioBusca ItemCriterioBusca
        {
            get
            {
                return _itemCriterioBusca;
            }

            set
            {
                SetProperty(ref _itemCriterioBusca, value);
            }
        }


        public ObservableCollection<PontoMapa> ListaDados { get; set; }
        public ObservableCollection<LinhaMapa> ListaLinhas { get; set; }

        public Command PesquisarCommand { get; set; }

        public bool IsLoadingLista
        {
            get
            {
                return _IsLoadingLista;
            }

            set
            {
                SetProperty(ref _IsLoadingLista, value);
            }
        }


        public  MapSpan GetCentralGeoCoordinate(
        IList<Position> geoCoordinates)
        {
            if (geoCoordinates.Count == 1)
            {
                return MapSpan.FromCenterAndRadius(geoCoordinates.First(), new Distance(5000));
            }

            double x = 0;
            double y = 0;
            double z = 0;

            foreach (var geoCoordinate in geoCoordinates)
            {
                var latitude = geoCoordinate.Latitude * Math.PI / 180;
                var longitude = geoCoordinate.Longitude * Math.PI / 180;

                x += Math.Cos(latitude) * Math.Cos(longitude);
                y += Math.Cos(latitude) * Math.Sin(longitude);
                z += Math.Sin(latitude);
            }

            var total = geoCoordinates.Count;

            x = x / total;
            y = y / total;
            z = z / total;

            var centralLongitude = Math.Atan2(y, x);
            var centralSquareRoot = Math.Sqrt(x * x + y * y);
            var centralLatitude = Math.Atan2(z, centralSquareRoot);

            var Center = new Position(centralLatitude * 180 / Math.PI, centralLongitude * 180 / Math.PI);
            var MaxLatitude = geoCoordinates.Max(d => d.Latitude);
            var MinLatitude = geoCoordinates.Min(d => d.Latitude);
            var MaxLongitude = geoCoordinates.Max(d => d.Latitude);
            var MinLongitude = geoCoordinates.Min(d => d.Latitude);

            return new MapSpan(Center, MaxLatitude - MinLatitude, MaxLongitude - MinLongitude);
            
        }

        private async Task CarregarListaDados()
        {
            List<PontoMapa> Dados = new List<PontoMapa>();
            List<LinhaMapa> Linhas = new List<LinhaMapa>();
            using (ApiService srv = new ApiService())
            {
                Dados = await srv.ListarPontosViagem(ItemCriterioBusca);
                Linhas = await srv.ListarLinhasViagem(ItemCriterioBusca);
            }

            ListaDados = new ObservableCollection<PontoMapa>(Dados);
            ListaLinhas = new ObservableCollection<LinhaMapa>(Linhas);
            OnPropertyChanged("ListaDados");
            OnPropertyChanged("ListaLinhas");
            Pins.Clear();
            foreach (var itemPonto in Dados)
            {
                TKCustomMapPin itemPin = new TKCustomMapPin();
                itemPin.IsDraggable = false;
                itemPin.Position = new Position(itemPonto.Latitude.GetValueOrDefault(0), itemPonto.Longitude.GetValueOrDefault(0));
                //itemPin.Image = ImageSource.FromFile("hotels.png");
                itemPin.Image = ImageSource.FromFile((itemPonto.Tipo == "A" ? "entertainment" :
                    itemPonto.Tipo == "CR" || itemPonto.Tipo == "CD" ? "automotive" :
                    itemPonto.Tipo == "L" ? "shopping" :
                    itemPonto.Tipo == "H" ? "hotels" :
                    itemPonto.Tipo == "RC" ? "tiresaccessories" :
                    itemPonto.Tipo == "R" ? "restaurants" :
                    itemPonto.Tipo == "P" ? "transport" :
                    itemPonto.Tipo == "T" ? "cookbooks" :
                    itemPonto.Tipo == "U" ? "professional" :

                    itemPonto.Tipo == "V" || itemPonto.Tipo == "F" ? "photography" : "pin") + ".png");
                itemPin.ShowCallout = true;
                itemPin.Anchor = new Point(0.5, 1);
                if (itemPonto.Tipo == "V" || itemPonto.Tipo == "F")
                    itemPin.Title = itemPonto.UrlTumbnail;
                else
                    itemPin.Title = itemPonto.Nome;
                itemPin.Subtitle = itemPonto.Periodo;
                Pins.Add(itemPin);
                
            }
            foreach (var itemPonto in Linhas)
            {

                TKPolyline polyline = new TKPolyline()
                {
                    Color = itemPonto.Tipo == "C" ? Color.Blue : itemPonto.Tipo == "D" ? Color.Green : Color.Red,
                    LineWidth = 10,
                    LineCoordinates =  itemPonto.Pontos.Select(d => new Position(d.Latitude.GetValueOrDefault(), d.Longitude.GetValueOrDefault())).ToList(),
                };
                if (itemPonto.Pontos.Count > 500)
                {
                    polyline.LineCoordinates = new List<Position>();
                    polyline.LineCoordinates.Add(new Position(itemPonto.Pontos.FirstOrDefault().Latitude.GetValueOrDefault(), itemPonto.Pontos.FirstOrDefault().Longitude.GetValueOrDefault()));
                    for (decimal k =0;k<itemPonto.Pontos.Count;k+= itemPonto.Pontos.Count()/500m)
                    {
                        polyline.LineCoordinates.Add(new Position(itemPonto.Pontos.ElementAt(Convert.ToInt32(k)).Latitude.GetValueOrDefault(), itemPonto.Pontos.ElementAt(Convert.ToInt32(k)).Longitude.GetValueOrDefault()));
                        if (itemPonto.Pontos.ElementAt(Convert.ToInt32(k)).Longitude < -46.64093234)
                        {

                        }
                    }
                    polyline.LineCoordinates.Add(new Position(itemPonto.Pontos.LastOrDefault().Latitude.GetValueOrDefault(), itemPonto.Pontos.LastOrDefault().Longitude.GetValueOrDefault()));

                }
                Polylines.Add(polyline);
            }
            await Task.Delay(2000);
            // var teste = GetCentralGeoCoordinate(Dados.Select(d => new Position(d.Latitude.GetValueOrDefault(), d.Longitude.GetValueOrDefault())).ToList());
            //LimiteMapa = GetCentralGeoCoordinate(Dados.Select(d => new Position(d.Latitude.GetValueOrDefault(), d.Longitude.GetValueOrDefault())).ToList());
            if (ListaDados.Any())
                MapCenter = ListaDados.OrderByDescending(d => d.DataInicio).Select(d => new Position(d.Latitude.GetValueOrDefault(), d.Longitude.GetValueOrDefault())).FirstOrDefault();
            IsLoadingLista = false;
        }


    }
}
