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
           
            List<CalendarioPrevisto> _itens = biz.ListarCalendarioPrevisto(token.IdentificadorViagem).ToList();

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
            itemCalendarioPrevisto.DataAtualizacao = DateTime.Now;
            itemCalendarioPrevisto.IdentificadorViagem = token.IdentificadorViagem;
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
            itemCalendarioPrevisto.DataExclusao = DateTime.Now;
            biz.SalvarCalendarioPrevisto(itemCalendarioPrevisto);
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            itemResultado.Sucesso = biz.IsValid();
            itemResultado.Mensagens = biz.RetornarMensagens.ToArray();

            return itemResultado;
        }
    }
}