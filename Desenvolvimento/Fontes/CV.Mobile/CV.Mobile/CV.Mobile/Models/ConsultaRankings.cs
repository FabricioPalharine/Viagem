using CV.Mobile.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV.Mobile.Models
{
    public class ConsultaRankings
    {

        public string Nome { get; set; }
        public string Tipo { get; set; }
        public string CodigoGoogle { get; set; }
        public double? Media { get; set; }
        public int NumeroAvaliacoes { get; set; }
        public IEnumerable<UsuarioConsulta> Avaliacoes { get; set; }

        public string TipoDescricao
        {
            get
            {
                string Valor = null;
                if (Tipo == "A")
                    Valor = AppResource.Atracao;
                else if (Tipo == "H")
                    Valor = AppResource.Hotel;
                else if (Tipo == "L")
                    Valor = AppResource.Loja;
                else if (Tipo == "R")
                    Valor = AppResource.Restaurante;
                else if (Tipo == "VA")
                    Valor = AppResource.CompanhiaTransporte;
                else if (Tipo == "C")
                    Valor = AppResource.Locadora;
                return Valor;
            }
        }
    }
}
