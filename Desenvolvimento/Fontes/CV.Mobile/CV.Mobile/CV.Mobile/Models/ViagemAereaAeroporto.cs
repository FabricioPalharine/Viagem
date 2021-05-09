
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;


namespace CV.Mobile.Models
{
    public class ViagemAereaAeroporto: ObservableObject
    {
        private TimeSpan? _HoraChegada;
        private TimeSpan? _HoraSaida;
        private readonly DateTime _dataMinima = new DateTime(1900, 01, 01);

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

                SetProperty(ref dataPartida, value);
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
       
        public double? Latitude
        {
            get
            {
                return _Latitude;
            }

            set
            {
                SetProperty(ref _Latitude, value);
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
