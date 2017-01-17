using CV.Business;
using CV.Model;
using CV.UI.Web.Helper;
using CV.UI.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CV.UI.Web.Controllers.WebAPI
{
    public class ConsultaController : BaseApiController
    {
        [Authorize]
        [BindJson(typeof(CriterioBusca), "json")]
        [ActionName("ConsultarExtratoMoeda")]
        [HttpGet]
        public List<ExtratoMoeda> ConsultarExtratoMoeda(CriterioBusca json)
        {
            ConsultaBusiness biz = new ConsultaBusiness();
            return biz.ConsultarExtratoMoeda(token.IdentificadorUsuario, token.IdentificadorViagem, json.Moeda, json.DataInicioDe.GetValueOrDefault());
        }
    }
}
