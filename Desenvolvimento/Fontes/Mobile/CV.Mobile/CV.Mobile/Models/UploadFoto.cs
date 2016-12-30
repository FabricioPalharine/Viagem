using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV.Mobile.Models
{
    public class UploadFoto
    {
        public string Base64 { get; set; }
        public string ImageMime { get; set; }
        public DateTime DataArquivo { get; set; }

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        public string CodigoGoogle { get; set; }

        public string Thumbnail { get; set; }

        public string LinkGoogle { get; set; }

        public string Comentario { get; set; }

        public int? IdentificadorRefeicao { get; set; }
        public int? IdentificadorHotel { get; set; }
        public int? IdentificadorAtracao { get; set; }
        public int? IdentificadorItemCompra { get; set; }
        public string CaminhoLocal { get; set; }
    }
}
