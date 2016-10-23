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
    public partial class AporteDinheiroController : BaseApiController
    {
        [Authorize]
        [BindJson(typeof(CriterioBusca), "json")]
        public ResultadoConsultaTipo<AporteDinheiro> Get(CriterioBusca json)
        {
            ResultadoConsultaTipo<AporteDinheiro> resultado = new ResultadoConsultaTipo<AporteDinheiro>();
            ViagemBusiness biz = new ViagemBusiness();
           
            List<AporteDinheiro> _itens = biz.ListarAporteDinheiro().ToList();
resultado.TotalRegistros = _itens.Count();
            if (json.SortField != null && json.SortField.Any())
                _itens = _itens.AsQueryable().OrderByField<AporteDinheiro>(json.SortField, json.SortOrder).ToList();

            if (json.Index.HasValue && json.Count.HasValue)
                _itens = _itens.Skip(json.Index.Value).Take(json.Count.Value).ToList();
            resultado.Lista = _itens;

            return resultado;
        }
        [Authorize]
        public AporteDinheiro Get(int id)
        {
            ViagemBusiness biz = new ViagemBusiness();
            AporteDinheiro itemAporteDinheiro = biz.SelecionarAporteDinheiro(id);
          
            return itemAporteDinheiro;
        }
        [Authorize]
        public ResultadoOperacao Post([FromBody] AporteDinheiro itemAporteDinheiro)
        {
            ViagemBusiness biz = new ViagemBusiness();
                      biz.SalvarAporteDinheiro(itemAporteDinheiro);
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            itemResultado.Sucesso = biz.IsValid();
            itemResultado.Mensagens = biz.RetornarMensagens.ToArray();
            if (itemResultado.Sucesso)
                itemResultado.IdentificadorRegistro = itemAporteDinheiro.Identificador;
            return itemResultado;
        }
        [Authorize]
        public ResultadoOperacao Delete(int id)
        {
            ViagemBusiness biz = new ViagemBusiness();
            AporteDinheiro itemAporteDinheiro = biz.SelecionarAporteDinheiro(id);
            biz.ExcluirAporteDinheiro(itemAporteDinheiro);
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            itemResultado.Sucesso = biz.IsValid();
            itemResultado.Mensagens = biz.RetornarMensagens.ToArray();

            return itemResultado;
        }
    }
}