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
    public partial class RequisicaoAmizadeController : BaseApiController
    {
        [Authorize]
        [BindJson(typeof(CriterioBusca), "json")]
        public ResultadoConsultaTipo<RequisicaoAmizade> Get(CriterioBusca json)
        {
            ResultadoConsultaTipo<RequisicaoAmizade> resultado = new ResultadoConsultaTipo<RequisicaoAmizade>();
            ViagemBusiness biz = new ViagemBusiness();

            List<RequisicaoAmizade> _itens = biz.ListarRequisicaoAmizade(d => d.IdentificadorUsuarioRequisitado == token.IdentificadorUsuario && d.Status < 2).ToList();
            resultado.TotalRegistros = _itens.Count();
            //if (json.SortField != null && json.SortField.Any())
            //    _itens = _itens.AsQueryable().OrderByField<RequisicaoAmizade>(json.SortField, json.SortOrder).ToList();

            //if (json.Index.HasValue && json.Count.HasValue)
            //    _itens = _itens.Skip(json.Index.Value).Take(json.Count.Value).ToList();
            resultado.Lista = _itens;

            return resultado;
        }
        [Authorize]
        public RequisicaoAmizade Get(int id)
        {
            ViagemBusiness biz = new ViagemBusiness();
            RequisicaoAmizade itemRequisicaoAmizade = biz.SelecionarRequisicaoAmizade(id);

            return itemRequisicaoAmizade;
        }
        [Authorize]
        public ResultadoOperacao Post([FromBody] RequisicaoAmizade itemRequisicaoAmizade)
        {
            ViagemBusiness biz = new ViagemBusiness();
            biz.SalvarRequisicaoAmizadeAprovacao(itemRequisicaoAmizade);
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            itemResultado.Sucesso = biz.IsValid();
            itemResultado.Mensagens = biz.RetornarMensagens.ToArray();
           
            return itemResultado;
        }
        [Authorize]
        public ResultadoOperacao Delete(int id)
        {
            ViagemBusiness biz = new ViagemBusiness();
            RequisicaoAmizade itemRequisicaoAmizade = biz.SelecionarRequisicaoAmizade(id);
            biz.ExcluirRequisicaoAmizade(itemRequisicaoAmizade);
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            itemResultado.Sucesso = biz.IsValid();
            itemResultado.Mensagens = biz.RetornarMensagens.ToArray();

            return itemResultado;
        }
    }
}