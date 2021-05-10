using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV.Model
{
    public class Timeline
    {
        public string Tipo { get; set; }
        public string Comentario { get; set; }
        public string Texto { get; set; }
        public string Url { get; set; }
        public DateTime? Data { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string GoogleId { get; set; }
        public string Pedido { get; set; }

        public int? Identificador { get; set; }
        public IEnumerable<UsuarioConsulta> Usuarios { get; set; }

    }
}
