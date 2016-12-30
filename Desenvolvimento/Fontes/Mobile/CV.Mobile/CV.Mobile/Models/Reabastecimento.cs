using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV.Mobile.Models
{
    public class Reabastecimento: ObservableObject
    {
        public int? Id { get; set; }

        public int? Identificador { get; set; }

        public int? IdentificadorCarro { get; set; }

        public int? IdentificadorCidade { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        public bool Litro { get; set; }

        public decimal? QuantidadeReabastecida { get; set; }

        public ObservableRangeCollection<ReabastecimentoGasto> Gastos { get; set; }

        public Carro ItemCarro { get; set; }

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

        private DateTime? _Data;
        private TimeSpan? _Hora;


        public Reabastecimento Clone()
        {
            return (Reabastecimento)this.MemberwiseClone();
        }
    }
}
