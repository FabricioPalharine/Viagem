
using CV.Mobile.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV.Mobile.Models
{
    public class RelatorioGastos
    {
        public int? IdentificadorUsuario { get; set; }
        public string NomeUsuario { get; set; }
        public int? Moeda { get; set; }
        public decimal Valor { get; set; }
        public string Descricao { get; set; }

        public DateTime Data { get; set; }

        public decimal ValorReal { get; set; }
        public string MoedaSigla
        {
            get
            {
                return Moeda.HasValue ? ((enumMoeda)Moeda.Value).ToString() : null;
            }
        }

        public IEnumerable<LojaItens> Itens { get; set; }

    }
}
