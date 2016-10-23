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
    public partial class ViagemAereaController : BaseApiController
    {
        [Authorize]
        [BindJson(typeof(CriterioBusca), "json")]
        public ResultadoConsultaTipo<ViagemAerea> Get(CriterioBusca json)
        {
            ResultadoConsultaTipo<ViagemAerea> resultado = new ResultadoConsultaTipo<ViagemAerea>();
            ViagemBusiness biz = new ViagemBusiness();
           
            List<ViagemAerea> _itens = biz.ListarViagemAerea().ToList();
resultado.TotalRegistros = _itens.Count();
            if (json.SortField != null && json.SortField.Any())
                _itens = _itens.AsQueryable().OrderByField<ViagemAerea>(json.SortField, json.SortOrder).ToList();

            if (json.Index.HasValue && json.Count.HasValue)
                _itens = _itens.Skip(json.Index.Value).Take(json.Count.Value).ToList();
            resultado.Lista = _itens;

            return resultado;
        }
        [Authorize]
        public ViagemAerea Get(int id)
        {
            ViagemBusiness biz = new ViagemBusiness();
            ViagemAerea itemViagemAerea = biz.SelecionarViagemAerea(id);
          
            return itemViagemAerea;
        }
        [Authorize]
        public ResultadoOperacao Post([FromBody] ViagemAerea itemViagemAerea)
        {
            ViagemBusiness biz = new ViagemBusiness();
                      biz.SalvarViagemAerea(itemViagemAerea);
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            itemResultado.Sucesso = biz.IsValid();
            itemResultado.Mensagens = biz.RetornarMensagens.ToArray();
            if (itemResultado.Sucesso)
                itemResultado.IdentificadorRegistro = itemViagemAerea.Identificador;
            return itemResultado;
        }
        [Authorize]
        public ResultadoOperacao Delete(int id)
        {
            ViagemBusiness biz = new ViagemBusiness();
            ViagemAerea itemViagemAerea = biz.SelecionarViagemAerea(id);
            biz.ExcluirViagemAerea(itemViagemAerea);
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            itemResultado.Sucesso = biz.IsValid();
            itemResultado.Mensagens = biz.RetornarMensagens.ToArray();

            return itemResultado;
        }
    }
}