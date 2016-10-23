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
    public partial class PaisController : BaseApiController
    {
        [Authorize]
        [BindJson(typeof(CriterioBusca), "json")]
        public ResultadoConsultaTipo<Pais> Get(CriterioBusca json)
        {
            ResultadoConsultaTipo<Pais> resultado = new ResultadoConsultaTipo<Pais>();
            ViagemBusiness biz = new ViagemBusiness();
           
            List<Pais> _itens = biz.ListarPais().ToList();
resultado.TotalRegistros = _itens.Count();
            if (json.SortField != null && json.SortField.Any())
                _itens = _itens.AsQueryable().OrderByField<Pais>(json.SortField, json.SortOrder).ToList();

            if (json.Index.HasValue && json.Count.HasValue)
                _itens = _itens.Skip(json.Index.Value).Take(json.Count.Value).ToList();
            resultado.Lista = _itens;

            return resultado;
        }
        [Authorize]
        public Pais Get(int id)
        {
            ViagemBusiness biz = new ViagemBusiness();
            Pais itemPais = biz.SelecionarPais(id);
          
            return itemPais;
        }
        [Authorize]
        public ResultadoOperacao Post([FromBody] Pais itemPais)
        {
            ViagemBusiness biz = new ViagemBusiness();
                      biz.SalvarPais(itemPais);
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            itemResultado.Sucesso = biz.IsValid();
            itemResultado.Mensagens = biz.RetornarMensagens.ToArray();
            if (itemResultado.Sucesso)
                itemResultado.IdentificadorRegistro = itemPais.Identificador;
            return itemResultado;
        }
        [Authorize]
        public ResultadoOperacao Delete(int id)
        {
            ViagemBusiness biz = new ViagemBusiness();
            Pais itemPais = biz.SelecionarPais(id);
            biz.ExcluirPais(itemPais);
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            itemResultado.Sucesso = biz.IsValid();
            itemResultado.Mensagens = biz.RetornarMensagens.ToArray();

            return itemResultado;
        }
    }
}