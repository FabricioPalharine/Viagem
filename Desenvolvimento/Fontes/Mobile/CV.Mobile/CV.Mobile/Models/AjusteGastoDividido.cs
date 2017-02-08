using CV.Mobile.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV.Mobile.Models
{
    public class AjusteGastoDividido
    {
        public int? IdentificadorUsuario { get; set; }
        public string NomeUsuario { get; set; }
        public int? Moeda { get; set; }
        public decimal ValorPago { get; set; }
        public decimal ValorRecebido { get; set; }

        public decimal Ajuste
        {
            get
            {
                return ValorPago - ValorRecebido;
            }
        }

        public string MoedaSigla
        {
            get
            {
                return Moeda.HasValue ? ((enumMoeda)Moeda.Value).ToString() : null;
            }
        }

    }
}
