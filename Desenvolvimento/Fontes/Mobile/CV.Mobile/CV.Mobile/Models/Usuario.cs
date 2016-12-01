using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV.Mobile.Models
{
    public class Usuario
    {
        public int? Identificador { get; set; }
        public string EMail { get; set; }
        public string Nome { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public DateTime? DataToken { get; set; }
        public int? Lifetime { get; set; }
        public string Codigo { get; set; }

    }
}
