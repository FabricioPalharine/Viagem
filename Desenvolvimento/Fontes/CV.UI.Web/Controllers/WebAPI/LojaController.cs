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

            List<Loja> _itens = biz.ListarLoja(token.IdentificadorViagem.GetValueOrDefault(), json.DataInicioDe, json.DataInicioAte,
                json.Nome, json.IdentificadorCidade, json.Identificador).ToList();

            resultado.Lista = _itens;

            return resultado;

        }
        [Authorize]
        public Loja Get(int id)
        {
            ViagemBusiness biz = new ViagemBusiness();
            Loja itemLoja = biz.SelecionarLoja_Completo(id);
            

            foreach (var item in itemLoja.Compras)
            {
                item.ItemLoja = null;
                item.ItemGasto.Compras = null;
                foreach (var itemItemCompra in item.ItensComprados)
                {
                    itemItemCompra.ItemGastoCompra = null;
                    foreach (var itemFoto in itemItemCompra.Fotos)
                    {
                        itemFoto.ItemItemCompra = null;
                        itemFoto.ItemFoto.ItensCompra = null;
                    }
                }
            }
            foreach (var item in itemLoja.Avaliacoes)
            {
                item.ItemLoja = null;
            }
            return itemLoja;
        }
        [Authorize]
        public ResultadoOperacao Post([FromBody] Loja itemLoja)
        {
            ViagemBusiness biz = new ViagemBusiness();
            itemLoja.IdentificadorCidade = biz.RetornarCidadeGeocoding(itemLoja.Latitude, itemLoja.Longitude);
            itemLoja.IdentificadorViagem = token.IdentificadorViagem;
            itemLoja.DataAtualizacao = DateTime.Now.ToUniversalTime();
            biz.SalvarLoja(itemLoja);
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            itemResultado.Sucesso = biz.IsValid();
            itemResultado.Mensagens = biz.RetornarMensagens.ToArray();
            if (itemResultado.Sucesso)
            {
                itemResultado.IdentificadorRegistro = itemLoja.Identificador;
                itemLoja.Avaliacoes = null;
                itemResultado.ItemRegistro = itemLoja;
            }
            return itemResultado;
        }

        [Authorize]
        public ResultadoOperacao Delete(int id)
        {
            ViagemBusiness biz = new ViagemBusiness();
            Loja itemLoja = biz.SelecionarLoja_Completo(id);
            itemLoja.DataExclusao = DateTime.Now.ToUniversalTime();
            foreach (var item in itemLoja.Compras.Where(d => !d.DataExclusao.HasValue))
            {
                item.DataExclusao = DateTime.Now.ToUniversalTime();
                item.ItemGasto.DataExclusao = DateTime.Now.ToUniversalTime();
                foreach (var itemItemCompra in item.ItensComprados)
                {
                    itemItemCompra.DataExclusao = DateTime.Now.ToUniversalTime();
                    foreach (var itemFoto in itemItemCompra.Fotos)
                    {
                        itemFoto.DataExclusao = DateTime.Now.ToUniversalTime();
                    }
                }
            }
            foreach (var item in itemLoja.Avaliacoes.Where(d=>!d.DataExclusao.HasValue))
            {
                item.DataExclusao = DateTime.Now.ToUniversalTime();
            }

            biz.SalvarLoja_Completo(itemLoja);
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            itemResultado.Sucesso = biz.IsValid();
            itemResultado.Mensagens = biz.RetornarMensagens.ToArray();

            return itemResultado;
        }

        [Authorize]
        [ActionName("CarregarFoto")]
        [HttpGet]
        public List<ItemCompra> CarregarFoto()
        {
            ViagemBusiness biz = new ViagemBusiness();
            return biz.ListarItemCompra(d => d.ItemGastoCompra.ItemLoja.IdentificadorViagem == token.IdentificadorViagem);
        }

        [Authorize]
        [ActionName("saveCompra")]
        [HttpPost]
        public ResultadoOperacao saveCompra(GastoCompra itemCompra)
        {
            ViagemBusiness biz = new ViagemBusiness();
            itemCompra.DataAtualizacao = DateTime.Now.ToUniversalTime();
            itemCompra.ItemGasto.DataAtualizacao = DateTime.Now.ToUniversalTime();

            biz.SalvarGastoCompra_Completo(itemCompra);
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            itemResultado.Sucesso = biz.IsValid();
            itemResultado.Mensagens = biz.RetornarMensagens.ToArray();
            if (itemResultado.Sucesso)
            {
                itemResultado.IdentificadorRegistro = itemCompra.Identificador;
                GastoCompra item = biz.SelecionarGastoCompra(itemCompra.Identificador);
                item.ItemLoja = null;
                item.ItemGasto.Compras = null;
                foreach (var itemItemCompra in item.ItensComprados)
                {
                    itemItemCompra.ItemGastoCompra = null;
                    foreach (var itemFoto in itemItemCompra.Fotos)
                    {
                        itemFoto.ItemItemCompra = null;
                        itemFoto.ItemFoto.ItensCompra = null;
                    }
                }
                itemResultado.ItemRegistro = item;
            }
            return itemResultado;
        }

        [Authorize]
        [ActionName("excluirCompra")]
        [HttpPost]
        public ResultadoOperacao excluirCompra(GastoCompra itemCompra)
        {
            ViagemBusiness biz = new ViagemBusiness();
            GastoCompra item = biz.SelecionarGastoCompra(itemCompra.Identificador);
            itemCompra.DataExclusao = DateTime.Now.ToUniversalTime();
            itemCompra.ItemGasto.DataExclusao = DateTime.Now.ToUniversalTime();
            foreach (var itemItemCompra in item.ItensComprados)
            {
                itemItemCompra.DataExclusao = DateTime.Now.ToUniversalTime();
                foreach (var itemFoto in itemItemCompra.Fotos)
                {
                    itemFoto.DataExclusao = DateTime.Now.ToUniversalTime();
                }
            }
            biz.SalvarGastoCompra_Item_Completo(itemCompra);

            ResultadoOperacao itemResultado = new ResultadoOperacao();
            itemResultado.Sucesso = biz.IsValid();
            itemResultado.Mensagens = biz.RetornarMensagens.ToArray();

            return itemResultado;
        }

        [Authorize]
        [ActionName("SalvarItemCompra")]
        [HttpPost]
        public ResultadoOperacao SalvarItemCompra(ItemCompra itemItemCompra)
        {
            ViagemBusiness biz = new ViagemBusiness();
            ItemCompra itemOriginal = null;
            itemItemCompra.DataAtualizacao = DateTime.Now.ToUniversalTime();
            if (itemItemCompra.Identificador.HasValue)
                itemOriginal = biz.SelecionarItemCompra(itemItemCompra.Identificador);

            biz.SalvarItemCompra(itemItemCompra);
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            itemResultado.Sucesso = biz.IsValid();
            itemResultado.Mensagens = biz.RetornarMensagens.ToArray();
            if (biz.IsValid())
            {
                itemResultado.IdentificadorRegistro = itemItemCompra.Identificador;
                int? IdentificadorListaCompra = null;
                if (itemOriginal != null)
                    IdentificadorListaCompra = itemOriginal.IdentificadorListaCompra;
                if (IdentificadorListaCompra.HasValue && (itemItemCompra.IdentificadorListaCompra != IdentificadorListaCompra || itemItemCompra.DataExclusao.HasValue))
                {
                    ListaCompra itemLista = biz.SelecionarListaCompra(IdentificadorListaCompra);
                    itemLista.Status = (int)enumStatusListaCompra.Pendente;
                    biz.SalvarListaCompra(itemLista);
                }
                else if (itemItemCompra.IdentificadorListaCompra.HasValue && !itemItemCompra.DataExclusao.HasValue && itemItemCompra.IdentificadorListaCompra != IdentificadorListaCompra)
                {
                    ListaCompra itemLista = biz.SelecionarListaCompra(itemItemCompra.IdentificadorListaCompra);
                    itemLista.Status = (int)enumStatusListaCompra.Pendente;
                    biz.SalvarListaCompra(itemLista);
                }
            }
            return itemResultado;
            
            
        }
    }
}