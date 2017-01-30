using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV.Model
{
    public class ResumoViagem
    {
        public int? AtracoesVisitadas { get; set; }
        public TimeSpan? MinutosAtracao { get; set; }

        public double? NotaMediaAtracao { get; set; }
        public decimal? TotalReaisAtracao { get; set; }

        public int? NumeroHotel { get; set; }
        public int? NoitesHotel { get; set; }
        public TimeSpan? TempoHotel { get; set; }
        public decimal? TotalReaisHospedagem { get; set; }
        public decimal? PrecoMediaNoite { get; set; }

        public double? NotaMediaHotel { get; set; }
        public int? RefeicoesRealizadas { get; set; }
        public double? NotaMediaRefeicao { get; set; }
        public decimal? TotalReaisRefeicao { get; set; }
        public decimal? PrecoMediaRefeicao { get; set; }

        public int? LojasVisitadas { get; set; }
        public decimal? TotalReaisCompra { get; set; }

        public int? ComprasRealizadas { get; set; }
        public int? ItensComprados { get; set; }
        public double? NotaMediaLoja { get; set; }
        
        public int? CarrosUtilizados { get; set; }
        public int? NumeroReabastecimento { get; set; }
        public decimal? LitrosReabastecidos { get; set; }

        public int? LocadorasUtilizadas { get; set; }
        public double? NotaMediaAluguel { get; set; }
        public TimeSpan? MinutosDeslocamentoCarro { get; set; }
        public int? KmDeslocamentoCarro { get; set; }
        public decimal? TotalReaisCarro { get; set; }
        public decimal? TotalReaisReabastecimento { get; set; }
        public decimal? PrecoMedioKM { get; set; }
        public decimal? KmLitro { get; set; }
        public int? DeslcamentosRealizados { get; set; }
        public int? CidadesRegistradas { get; set; }
        public TimeSpan? MinutosViajando { get; set; }
        public TimeSpan? MinutosAguardando { get; set; }
        public int? KmDeslocamento { get; set; }
        public double? NotaMediaDeslocamento { get; set; }
        public decimal? TotalReaisDeslocamento { get; set; }

        public int? ComentariosFeitos { get; set; }
        public int? FotosTirada { get; set; }
        public int? VideosGravados { get; set; }

        public int? KmTotaisDeslocados { get; set; }
        public int? KmCaminhados { get; set; }
    }
}
