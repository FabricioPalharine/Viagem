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
        public List<ExtratoMoeda> ConsultarExtratoMoeda(int? IdentificadorUsuario,int? IdentificadorViagem, int? Moeda, DateTime DataInicio)
        {
            using (ConsultaRepository data = new ConsultaRepository())
            {
                return data.ConsultarExtratoMoeda(IdentificadorUsuario, IdentificadorViagem, Moeda, DataInicio);
            }
        }
    }
}
