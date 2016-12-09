using CV.Mobile.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV.Mobile.Models
{
    public class CotacaoMoeda
    {
        public int? Id { get; set; }
        public int? Identificador { get; set; }
        public int? Moeda { get; set; }
        public DateTime? DataCotacao { get; set; }
        public decimal? ValorCotacao { get; set; }
        public int? IdentificadorViagem { get; set; }
        public DateTime? DataAtualizacao { get; set; }

        public DateTime? DataExclusao { get; set; }
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
    }
}
