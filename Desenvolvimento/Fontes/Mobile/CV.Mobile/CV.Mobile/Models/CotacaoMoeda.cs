using CV.Mobile.Helpers;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV.Mobile.Models
{
    public class CotacaoMoeda
    {
        [PrimaryKey, AutoIncrement]

        public int? Id { get; set; }
        public int? Identificador { get; set; }
        public int? Moeda { get; set; }
        public DateTime? DataCotacao { get; set; }
        public decimal? ValorCotacao { get; set; }
        public int? IdentificadorViagem { get; set; }
        public DateTime? DataAtualizacao { get; set; }

        public DateTime? DataExclusao { get; set; }
        [Ignore]

        public string SiglaMoeda
        {
            get
            {
                if (Moeda.HasValue)
                    return ((enumMoeda)Moeda).ToString();
                else
                    return null;
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
        public CotacaoMoeda Clone()
        {
            return (CotacaoMoeda)this.MemberwiseClone();
        }
    }
}
