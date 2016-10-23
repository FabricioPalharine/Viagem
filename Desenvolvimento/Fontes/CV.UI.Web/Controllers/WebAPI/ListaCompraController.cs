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
    public partial class ListaCompraController : BaseApiController
    {
        [Authorize]
        [BindJson(typeof(CriterioBusca), "json")]
        public ResultadoConsultaTipo<ListaCompra> Get(CriterioBusca json)
        {
            ResultadoConsultaTipo<ListaCompra> resultado = new ResultadoConsultaTipo<ListaCompra>();
            ViagemBusiness biz = new ViagemBusiness();
           
            List<ListaCompra> _itens = biz.ListarListaCompra().ToList();
resultado.TotalRegistros = _itens.Count();
            if (json.SortField != null && json.SortField.Any())
                _itens = _itens.AsQueryable().OrderByField<ListaCompra>(json.SortField, json.SortOrder).ToList();

            if (json.Index.HasValue && json.Count.HasValue)
                _itens = _itens.Skip(json.Index.Value).Take(json.Count.Value).ToList();
            resultado.Lista = _itens;

            return resultado;
        }
        [Authorize]
        public ListaCompra Get(int id)
        {
            ViagemBusiness biz = new ViagemBusiness();
            ListaCompra itemListaCompra = biz.SelecionarListaCompra(id);
          
            return itemListaCompra;
        }
        [Authorize]
        public ResultadoOperacao Post([FromBody] ListaCompra itemListaCompra)
        {
            ViagemBusiness biz = new ViagemBusiness();
                      biz.SalvarListaCompra(itemListaCompra);
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            itemResultado.Sucesso = biz.IsValid();
            itemResultado.Mensagens = biz.RetornarMensagens.ToArray();
            if (itemResultado.Sucesso)
                itemResultado.IdentificadorRegistro = itemListaCompra.Identificador;
            return itemResultado;
        }
        [Authorize]
        public ResultadoOperacao Delete(int id)
        {
            ViagemBusiness biz = new ViagemBusiness();
            ListaCompra itemListaCompra = biz.SelecionarListaCompra(id);
            biz.ExcluirListaCompra(itemListaCompra);
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            itemResultado.Sucesso = biz.IsValid();
            itemResultado.Mensagens = biz.RetornarMensagens.ToArray();

            return itemResultado;
        }
    }
}