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

            List<ListaCompra> _itens = biz.ListarListaCompra(token.IdentificadorUsuario, token.IdentificadorViagem, json.Situacao, json.Nome, json.IdentificadorParticipante, json.Tipo, json.Comentario).ToList();
          
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
            itemListaCompra.IdentificadorViagem = token.IdentificadorViagem;
            itemListaCompra.DataAtualizacao = DateTime.Now.ToUniversalTime();
            biz.SalvarListaCompra(itemListaCompra);
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            itemResultado.Sucesso = biz.IsValid();
            itemResultado.Mensagens = biz.RetornarMensagens.ToArray();
            if (itemResultado.Sucesso)
            {
                itemResultado.IdentificadorRegistro = itemListaCompra.Identificador;
                itemResultado.ItemRegistro = itemListaCompra;
            }
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

        [Authorize]
        [HttpGet]
        [ActionName("CarregarListaPedidos")]
        [BindJson(typeof(CriterioBusca), "json")]
        public List<ListaCompra> CarregarListaPedidos(CriterioBusca json)
        {
            ViagemBusiness biz = new ViagemBusiness();
            return biz.ListarListaCompra(json.IdentificadorParticipante, token.IdentificadorViagem, new List<int?>(new int?[] { (int) enumStatusListaCompra.Comprado, (int) enumStatusListaCompra.Pendente }), null);
        }

        [Authorize]
        [HttpGet]
        [BindJson(typeof(CriterioBusca), "json")]
        [ActionName("CarregarPedidosRecebidos")]

        public List<ListaCompra> CarregarPedidosRecebidos(CriterioBusca json)
        {
            ViagemBusiness biz = new ViagemBusiness();

            List<ListaCompra> _itens = biz.ListarListaCompra(json.IdentificadorParticipante, token.IdentificadorViagem, json.Situacao,null, token.IdentificadorUsuario, json.Tipo, json.Comentario).ToList();
            foreach (var itemLista in _itens.Where(d => d.Status == (int)enumStatusListaCompra.NaoVisto))
            {
                itemLista.Status = (int)enumStatusListaCompra.Pendente;
                itemLista.DataAtualizacao = DateTime.Now.ToUniversalTime();
                biz.SalvarListaCompra(itemLista);
            }
            return _itens;

        }
    }
}