using CV.Model.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV.Model
{
    public class ConsultaRankings
    {

        public string Nome { get; set; }
        public string Tipo { get; set; }
        public string CodigoGoogle { get; set; }
        public double? Media { get; set; }
        public int NumeroAvaliacoes { get; set; }
        public IEnumerable<UsuarioConsulta > Avaliacoes { get; set; }

        public string TipoDescricao
        {
            get
            {
                string Valor = null;
                if (Tipo == "A")
                    Valor = MensagemModelo.Atracao;
                else if (Tipo == "H")
                    Valor = MensagemModelo.Hotel;
                else if (Tipo == "L")
                    Valor = MensagemModelo.Loja;
                else if (Tipo == "R")
                    Valor = MensagemModelo.Restaurante;
                else if (Tipo == "VA")
                    Valor = MensagemModelo.CompanhiaTransporte;
                else if (Tipo == "C")
                    Valor = MensagemModelo.Locadora;
                return Valor;
            }
        }
    }
}
