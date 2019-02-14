using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV.Model
{
    public class PontoMapa
    {
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string Tipo { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public string Nome { get; set; }

        public string UrlTumbnail { get; set; }
        public string Url { get; set; }
        public string GoogleId { get; set; }
        public string Periodo
        {
            get
            {
                if (DataFim.HasValue)
                    return String.Concat(DataInicio.GetValueOrDefault().ToString("dd/MM/yyyy"), " - ", DataFim.GetValueOrDefault().ToString("dd/MM/yyyy"));
                else
                    return DataInicio.GetValueOrDefault().ToString("dd/MM/yyyy");
            }
        }
    }
}
