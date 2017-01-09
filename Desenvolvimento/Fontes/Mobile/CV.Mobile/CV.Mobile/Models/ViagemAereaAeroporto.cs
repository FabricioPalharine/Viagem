using MvvmHelpers;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace CV.Mobile.Models
{
    public class ViagemAereaAeroporto: ObservableObject
    {
        private TimeSpan? _HoraChegada;
        private TimeSpan? _HoraSaida;
        private bool _VisitaIniciada;
        private bool _VisitaConcluida;
        private readonly DateTime _dataMinima = new DateTime(1900, 01, 01);
        private MapSpan _Bounds;

        public TimeSpan? HoraChegada
        {
            get
            {
                return _HoraChegada;
            }

            set
            {
                SetProperty(ref _HoraChegada, value);
            }
        }

        public TimeSpan? HoraPartida
        {
            get
            {
                return _HoraSaida;
            }

            set
            {
                SetProperty(ref _HoraSaida, value);
            }
        }
        [PrimaryKey, AutoIncrement]

        public int? Id { get; set; }
        public int? Identificador { get; set; }

        public int? IdentificadorViagemAerea { get; set; }

        public int? IdentificadorCidade { get; set; }

        public string Aeroporto { get; set; }

        private double? _Latitude;

        private double? _Longitude;

        public int? TipoPonto { get; set; }

        private DateTime? dataChegada;

        private DateTime? dataPartida;
        [Ignore]
        public Cidade ItemCidade { get; set; }

      
        public string CodigoPlace { get; set; }

        public DateTime? DataAtualizacao { get; set; }

        public DateTime? DataExclusao { get; set; }

        public DateTime? DataChegada
        {
            get
            {
                return dataChegada;
            }

            set
            {
                
                    if (value.GetValueOrDefault(_dataMinima) == _dataMinima)
                    {
                        _VisitaIniciada = false;
                        VisitaConcluida = false;
                    }
                    else
                    {
                        _VisitaIniciada = true;
                    }
                    OnPropertyChanged("VisitaIniciada");
                
                SetProperty(ref dataChegada, value);
              
                    

            }
        }

        public DateTime? DataPartida
        {
            get
            {
                return dataPartida;
            }

            set
            {

                if (value.GetValueOrDefault(_dataMinima) == _dataMinima)
                {
                    _VisitaConcluida = false;
                }
                else
                {
                    _VisitaConcluida = true;
                }
                OnPropertyChanged("VisitaConcluida");
                SetProperty(ref dataPartida, value);
            }
        }
        [Ignore]
        public bool VisitaIniciada
        {
            get
            {
                return _VisitaIniciada;
            }

            set
            {
                SetProperty(ref _VisitaIniciada, value);
                if (value)
                {
                    if (dataChegada.GetValueOrDefault(_dataMinima) == _dataMinima)
                    {
                        dataChegada = DateTime.Today;
                        HoraChegada = DateTime.Now.TimeOfDay;
                    }
                }
                else
                {
                    dataChegada = _dataMinima;
                    HoraChegada = new TimeSpan();
                    VisitaConcluida = false;
                }
                OnPropertyChanged("DataChegada");
            }
        }
        [Ignore]
        public bool VisitaConcluida
        {
            get
            {
                return _VisitaConcluida;
            }

            set
            {
                SetProperty(ref _VisitaConcluida, value);
                if (value)
                {
                    if (dataPartida.GetValueOrDefault(_dataMinima) == _dataMinima)
                    {
                        dataPartida = DateTime.Today;
                        HoraPartida = DateTime.Now.TimeOfDay;
                    }
                }
                else
                {
                    dataPartida = _dataMinima;
                    HoraPartida = new TimeSpan();
                }
                OnPropertyChanged("DataPartida");
            }
        }

        [Ignore]
        public Command<GmsSearchResults> PlaceSelectedCommand
        {
            get
            {
                return new Command<GmsSearchResults>(p =>
                {
                    Aeroporto = p.name;
                    CodigoPlace = p.place_id;
                });
            }
        }
        [Ignore]
        public MapSpan Bounds
        {
            get
            {
                return _Bounds;
            }

            set
            {
               SetProperty(ref _Bounds ,value);
            }
        }

        public double? Latitude
        {
            get
            {
                return _Latitude;
            }

            set
            {
                SetProperty(ref _Latitude, value);
                if (Latitude.HasValue && Longitude.HasValue)
                    Bounds = MapSpan.FromCenterAndRadius(new Position(Latitude.Value, Longitude.Value), new Distance(5000));
            }
        }

        public double? Longitude
        {
            get
            {
                return _Longitude;
            }

            set
            {
                SetProperty(ref _Longitude, value);
                if (Latitude.HasValue && Longitude.HasValue)
                    Bounds = MapSpan.FromCenterAndRadius(new Position(Latitude.Value, Longitude.Value), new Distance(5000));

            }
        }

        public ViagemAereaAeroporto Clone()
        {
            return (ViagemAereaAeroporto)this.MemberwiseClone();
        }

        private bool _Atualizado = true;

        public bool AtualizadoBanco
        {
            get
            {
                return _Atualizado;
            }

            set
            {
                _Atualizado = value;
            }
        }
    }
}
