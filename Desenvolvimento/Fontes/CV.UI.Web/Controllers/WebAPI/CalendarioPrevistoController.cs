using CV.Business;
using CV.Model;
using CV.Model.Dominio;
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
    public partial class CalendarioPrevistoController : BaseApiController
    {
        [Authorize]
        [BindJson(typeof(CriterioBusca), "json")]
        public ResultadoConsultaTipo<CalendarioPrevisto> Get(CriterioBusca json)
        {
            ResultadoConsultaTipo<CalendarioPrevisto> resultado = new ResultadoConsultaTipo<CalendarioPrevisto>();
            ViagemBusiness biz = new ViagemBusiness();
           
            List<CalendarioPrevisto> _itens = biz.ListarCalendarioPrevisto().ToList();
resultado.TotalRegistros = _itens.Count();
            if (json.SortField != null && json.SortField.Any())
                _itens = _itens.AsQueryable().OrderByField<CalendarioPrevisto>(json.SortField, json.SortOrder).ToList();

            if (json.Index.HasValue && json.Count.HasValue)
                _itens = _itens.Skip(json.Index.Value).Take(json.Count.Value).ToList();
            resultado.Lista = _itens;

            return resultado;
        }
        [Authorize]
        public CalendarioPrevisto Get(int id)
        {
            ViagemBusiness biz = new ViagemBusiness();
            CalendarioPrevisto itemCalendarioPrevisto = biz.SelecionarCalendarioPrevisto(id);
          
            return itemCalendarioPrevisto;
        }
        [Authorize]
        public ResultadoOperacao Post([FromBody] CalendarioPrevisto itemCalendarioPrevisto)
        {
            ViagemBusiness biz = new ViagemBusiness();
                      biz.SalvarCalendarioPrevisto(itemCalendarioPrevisto);
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            itemResultado.Sucesso = biz.IsValid();
            itemResultado.Mensagens = biz.RetornarMensagens.ToArray();
            if (itemResultado.Sucesso)
                itemResultado.IdentificadorRegistro = itemCalendarioPrevisto.Identificador;
            return itemResultado;
        }
        [Authorize]
        public ResultadoOperacao Delete(int id)
        {
            ViagemBusiness biz = new ViagemBusiness();
            CalendarioPrevisto itemCalendarioPrevisto = biz.SelecionarCalendarioPrevisto(id);
            biz.ExcluirCalendarioPrevisto(itemCalendarioPrevisto);
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            itemResultado.Sucesso = biz.IsValid();
            itemResultado.Mensagens = biz.RetornarMensagens.ToArray();

            return itemResultado;
        }
    }
}