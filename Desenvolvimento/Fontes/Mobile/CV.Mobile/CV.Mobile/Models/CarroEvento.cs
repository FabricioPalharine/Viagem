using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV.Mobile.Models
{
    public class CarroEvento: ObservableObject
    {
        public int? Id { get; set; }
        public int? Identificador { get; set; }

        public bool? Inicio { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        public int? IdentificadorCidade { get; set; }

        public string NomeCidade { get; set; }
        public Cidade ItemCidade { get; set; }

        public DateTime? DataAtualizacao { get; set; }

        public DateTime? DataExclusao { get; set; }

        public DateTime? Data
        {
            get
            {
                return _Data;
            }

            set
            {
                SetProperty(ref _Data, value);
            }
        }

        public TimeSpan? Hora
        {
            get
            {
                return _Hora;
            }

            set
            {
                SetProperty(ref _Hora, value);
            }
        }

        public int? Odometro
        {
            get
            {
                return _Odometro;
            }

            set
            {
                SetProperty(ref _Odometro, value);
            }
        }

        private DateTime? _Data;
        private TimeSpan? _Hora;

        private int? _Odometro;
        public CarroEvento Clone()
        {
            return (CarroEvento)this.MemberwiseClone();
        }
    }
}
