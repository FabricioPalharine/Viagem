using MvvmHelpers;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV.Mobile.Models
{
    public class HotelEvento : ObservableObject
    {

        [PrimaryKey, AutoIncrement]

        public int? Id { get; set; }
        public int? Identificador { get; set; }
        public int? IdentificadorHotel { get; set; }
        public int? IdentificadorUsuario { get; set; }
        public string NomeUsuario { get; set; }
        [Ignore]
        public Usuario ItemUsuario { get; set; }
        public DateTime? DataAtualizacao { get; set; }
        public DateTime? DataExclusao { get; set; }

        public DateTime? DataEntrada
        {
            get
            {
                return _DataEntrada;
            }

            set
            {
                SetProperty(ref _DataEntrada, value);
            }
        }

        public DateTime? DataSaida
        {
            get
            {
                return _DataSaidia;
            }

            set
            {
                SetProperty(ref _DataSaidia, value);

            }
        }

        public TimeSpan? HoraEntrada
        {
            get
            {
                return _HoraEntrada;
            }

            set
            {
                SetProperty(ref _HoraEntrada, value);
            }
        }

        public TimeSpan? HoraSaida
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

        private TimeSpan? _HoraEntrada;
        private TimeSpan? _HoraSaida;
        private DateTime? _DataEntrada;

        private DateTime? _DataSaidia;
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
