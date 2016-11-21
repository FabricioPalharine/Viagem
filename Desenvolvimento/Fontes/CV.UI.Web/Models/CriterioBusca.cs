using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CV.UI.Web.Models
{
    public class CriterioBusca
    {

        public int? Index { get; set; }
        public int? Count { get; set; }
        public string[] SortField { get; set; }
        public string[] SortOrder { get; set; }
        public string EMail { get; set; }

        public string Nome { get; set; }

        public int? IdentificadorParticipante { get; set; }

        public DateTime? DataInicioDe { get; set; }

        public DateTime? DataInicioAte { get; set; }
        public DateTime? DataFimDe { get; set; }
        public DateTime? DataFimAte { get; set; }

        public bool? Aberto { get; set; }

        public string Comentario { get; set; }
        public List<int?> ListaAtracoes { get; set; }
        public List<int?> ListaHoteis { get; set; }
        public List<int?> ListaRefeicoes { get; set; }
        public int? IdentificadorCidade { get; set; }
        public int? Identificador { get; set; }
        public string Tipo { get; set; }
        public int? Situacao { get; set; }

        public int? TipoInteiro { get; set; }
        public int? IdentificadorCidade2 { get; set; }

    }
}