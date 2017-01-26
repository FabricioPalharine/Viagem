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
    }
}
