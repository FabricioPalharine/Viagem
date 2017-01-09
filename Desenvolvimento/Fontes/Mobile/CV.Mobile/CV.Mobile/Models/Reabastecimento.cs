using MvvmHelpers;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV.Mobile.Models
{
    public class Reabastecimento: ObservableObject
    {
        [PrimaryKey, AutoIncrement]

        public int? Id { get; set; }

        public int? Identificador { get; set; }

        public int? IdentificadorCarro { get; set; }

        public int? IdentificadorCidade { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        public bool Litro { get; set; }

        public decimal? QuantidadeReabastecida { get; set; }
        [Ignore]
        public ObservableRangeCollection<ReabastecimentoGasto> Gastos { get; set; }
       
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

        private DateTime? _Data;
        private TimeSpan? _Hora;
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

        public Reabastecimento Clone()
        {
            return (Reabastecimento)this.MemberwiseClone();
        }
    }
}
