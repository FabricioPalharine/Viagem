using CV.Business.Library;
using CV.Data;
using CV.Model;
using CV.Model.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV.Business
{
    public class ConsultaBusiness : BusinessBase
    {
        public List<ExtratoMoeda> ConsultarExtratoMoeda(int? IdentificadorUsuario, int? IdentificadorViagem, int? Moeda, DateTime DataInicio)
        {
            using (ConsultaRepository data = new ConsultaRepository())
            {
                return data.ConsultarExtratoMoeda(IdentificadorUsuario, IdentificadorViagem, Moeda, DataInicio);
            }
        }

        public List<AjusteGastoDividido> ListarGastosAcerto(int? IdentificadorViagem, int? IdentificadorUsuario, DateTime? DataInicio, DateTime? DataFim)
        {
            using (ConsultaRepository data = new ConsultaRepository())
            {
                return data.ListarGastosAcerto(IdentificadorViagem, IdentificadorUsuario, DataInicio, DataFim);
            }
        }

        public List<RelatorioGastos> ListarGastosViagem(int? IdentificadorViagem, int? IdentificadorUsuario, DateTime? DataInicio, DateTime? DataFim, string Tipo)
        {
            using (ConsultaRepository data = new ConsultaRepository())
            {
                return data.ListarGastosViagem(IdentificadorViagem, IdentificadorUsuario, DataInicio, DataFim, Tipo);
            }
        }

        public List<Timeline> CarregarTimeline(int? IdentificadorViagem, int? IdentificadorUsuarioConsulta, int? IdentificadorUsuarioEvento, DateTime? DataMaxima, DateTime? DataMinima, int NumeroRegistros)
        {
            using (ConsultaRepository data = new ConsultaRepository())
            {
                return data.CarregarTimeline(IdentificadorViagem, IdentificadorUsuarioConsulta, IdentificadorUsuarioEvento, DataMaxima, DataMinima, NumeroRegistros);
            }
        }

        public List<LocaisVisitados> CarregarLocaisVisitados(int? IdentificadorViagem, DateTime? DataDe, DateTime? DataAte)
        {
            using (ConsultaRepository data = new ConsultaRepository())
            {
                return data.CarregarLocaisVisitados(IdentificadorViagem, DataDe, DataAte, null, null);
            }
        }

        public LocaisVisitados CarregarDetalhesAtracao(int? IdentificadorViagem, DateTime? DataDe, DateTime? DataAte, string Nome, string CodigoGoogle)
        {
            LocaisVisitados itemRetorno = new LocaisVisitados();
            using (ConsultaRepository data = new ConsultaRepository())
            {
                itemRetorno.LocaisFilho = data.CarregarLocaisVisitados(IdentificadorViagem, DataDe, DataAte, Nome, CodigoGoogle);
                itemRetorno.Detalhes = data.CarregarDetalhesAtracaoVisitada(IdentificadorViagem, DataDe, DataAte, Nome, CodigoGoogle);
                itemRetorno.Gastos = data.ConsultarGastosAtracao(IdentificadorViagem, DataDe, DataAte, Nome, CodigoGoogle);
                itemRetorno.Fotos = data.ConsultarFotosAtracao(IdentificadorViagem, DataDe, DataAte, Nome, CodigoGoogle);
            }
            return itemRetorno;
        }

        public LocaisVisitados CarregarDetalhesHotel(int? IdentificadorViagem, DateTime? DataDe, DateTime? DataAte, string Nome, string CodigoGoogle)
        {
            LocaisVisitados itemRetorno = new LocaisVisitados();
            using (ConsultaRepository data = new ConsultaRepository())
            {
                itemRetorno.Detalhes = data.CarregarDetalhesHotelHospedado(IdentificadorViagem, DataDe, DataAte, Nome, CodigoGoogle);
                itemRetorno.Gastos = data.ConsultarGastosHotel(IdentificadorViagem, DataDe, DataAte, Nome, CodigoGoogle);
                itemRetorno.Fotos = data.ConsultarFotosHotel(IdentificadorViagem, DataDe, DataAte, Nome, CodigoGoogle);
            }
            return itemRetorno;
        }

        public LocaisVisitados CarregarDetalhesRefeicao(int? IdentificadorViagem, DateTime? DataDe, DateTime? DataAte, string Nome, string CodigoGoogle)
        {
            LocaisVisitados itemRetorno = new LocaisVisitados();
            using (ConsultaRepository data = new ConsultaRepository())
            {
                itemRetorno.Detalhes = data.CarregarDetalhesRestaurante(IdentificadorViagem, DataDe, DataAte, Nome, CodigoGoogle);
                itemRetorno.Gastos = data.ConsultarGastosRefeicao(IdentificadorViagem, DataDe, DataAte, Nome, CodigoGoogle);
                itemRetorno.Fotos = data.ConsultarFotosRefeicao(IdentificadorViagem, DataDe, DataAte, Nome, CodigoGoogle);
            }
            return itemRetorno;
        }

        public LocaisVisitados CarregarDetalhesLoja(int? IdentificadorViagem, DateTime? DataDe, DateTime? DataAte, string Nome, string CodigoGoogle)
        {
            LocaisVisitados itemRetorno = new LocaisVisitados();
            using (ConsultaRepository data = new ConsultaRepository())
            {
                itemRetorno.Gastos = data.ConsultarComprasLoja(IdentificadorViagem, DataDe, DataAte, Nome, CodigoGoogle);
            }
            return itemRetorno;
        }

        public List<CalendarioRealizado> CarregarCalendarioRealizado(int? IdentificadorViagem, int? IdentificadorUsuario)
        {
            using (ConsultaRepository data = new ConsultaRepository())
            {
                return data.CarregarCalendarioRealizado(IdentificadorViagem, IdentificadorUsuario);
            }
        }
    }
}
