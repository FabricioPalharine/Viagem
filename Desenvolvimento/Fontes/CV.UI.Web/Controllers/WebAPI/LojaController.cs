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
    public partial class LojaController : BaseApiController
    {
        [Authorize]
        [BindJson(typeof(CriterioBusca), "json")]
        public ResultadoConsultaTipo<Loja> Get(CriterioBusca json)
        {
            ResultadoConsultaTipo<Loja> resultado = new ResultadoConsultaTipo<Loja>();
            ViagemBusiness biz = new ViagemBusiness();
           
            List<Loja> _itens = biz.ListarLoja().ToList();
resultado.TotalRegistros = _itens.Count();
            if (json.SortField != null && json.SortField.Any())
                _itens = _itens.AsQueryable().OrderByField<Loja>(json.SortField, json.SortOrder).ToList();

            if (json.Index.HasValue && json.Count.HasValue)
                _itens = _itens.Skip(json.Index.Value).Take(json.Count.Value).ToList();
            resultado.Lista = _itens;

            return resultado;
        }
        [Authorize]
        public Loja Get(int id)
        {
            ViagemBusiness biz = new ViagemBusiness();
            Loja itemLoja = biz.SelecionarLoja(id);
          
            return itemLoja;
        }
        [Authorize]
        public ResultadoOperacao Post([FromBody] Loja itemLoja)
        {
            ViagemBusiness biz = new ViagemBusiness();
                      biz.SalvarLoja(itemLoja);
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            itemResultado.Sucesso = biz.IsValid();
            itemResultado.Mensagens = biz.RetornarMensagens.ToArray();
            if (itemResultado.Sucesso)
                itemResultado.IdentificadorRegistro = itemLoja.Identificador;
            return itemResultado;
        }
        [Authorize]
        public ResultadoOperacao Delete(int id)
        {
            ViagemBusiness biz = new ViagemBusiness();
            Loja itemLoja = biz.SelecionarLoja(id);
            biz.ExcluirLoja(itemLoja);
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            itemResultado.Sucesso = biz.IsValid();
            itemResultado.Mensagens = biz.RetornarMensagens.ToArray();

            return itemResultado;
        }
    }
}