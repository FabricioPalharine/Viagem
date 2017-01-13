using MvvmHelpers;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV.Mobile.Models
{
    public class Comentario : ObservableObject
    {
        private double? _Latitude;
        private double? _Longitude;
        private DateTime? _Data;

        private TimeSpan? _Hora;
        private string _Texto;

        [PrimaryKey, AutoIncrement]

        public int? Id { get; set; }
        public int? Identificador { get; set; }
        public int? IdentificadorViagem { get; set; }
        public DateTime? DataAtualizacao { get; set; }
        public DateTime? DataExclusao { get; set; }
        
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

        public int? IdentificadorCidade { get; set; }
        public string NomeCidade { get; set; }

        public string Texto
        {
            get
            {
                return _Texto;
            }

            set
            {
                SetProperty(ref _Texto, value);

            }
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

        public int? IdentificadorUsuario { get; set; }
        public Comentario Clone()
        {
            return (Comentario)this.MemberwiseClone();
        }
    }
}
