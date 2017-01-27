using CV.Model.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV.Model
{
    public class LocaisVisitados
    {
        public string CodigoCoogle { get; set; }
        public string Nome { get; set; }
        public string Tipo { get; set; }
        public int? IdentificadorCidade { get; set; }
        public string NomeCidade { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }

        public string TipoDescricao
        {
            get
            {
                return Tipo == "A" ? MensagemModelo.Atracao : Tipo == "R" ? MensagemModelo.Restaurante : Tipo == "H" ? MensagemModelo.Hotel : MensagemModelo.Loja;
            }
        }

        public List<LocaisVisitados> LocaisFilho { get; set; }
        public List<LocaisDetalhes> Detalhes { get; set; }

        public double? Media
        {
            get
            {
                if (Detalhes != null && Detalhes.Where(d=>d.Nota.HasValue).Any())
                {
                   return  Math.Round( Detalhes.Where(d => d.Nota.HasValue).Average(d => d.Nota.GetValueOrDefault()),1);
                }
                else
                    return null;
            }
        }

        public List<Foto> Fotos { get; set; }
        public List<RelatorioGastos> Gastos { get; set; }


    }
}
