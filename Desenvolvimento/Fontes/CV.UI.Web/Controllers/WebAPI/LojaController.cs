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
            if (!itemLoja.IdentificadorCidade.HasValue)
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
       
            foreach (var item in itemLoja.Avaliacoes.Where(d=>!d.DataExclusao.HasValue))
            {
                item.DataExclusao = DateTime.Now.ToUniversalTime();
            }

            biz.SalvarLoja_Completo(itemLoja);
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            itemResultado.Sucesso = biz.IsValid();
            itemResultado.Mensagens = biz.RetornarMensagens.ToArray();
            if (itemResultado.Sucesso)
                itemResultado.Mensagens = new MensagemErro[] { new MensagemErro() { Mensagem = MensagemBusiness.RetornaMensagens("Viagem_ExcluirLoja_OK") } };

            return itemResultado;
        }


        [Authorize]
        [ActionName("CarregarItemCompra")]
        [HttpGet]
        public ItemCompra CarregarItemCompra(int? Id)
        {
            ViagemBusiness biz = new ViagemBusiness();
            return biz.SelecionarItemCompra(Id);
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
                    itemLista.DataAtualizacao = DateTime.Now.ToUniversalTime();
                    biz.SalvarListaCompra(itemLista);
                }
                else if (itemItemCompra.IdentificadorListaCompra.HasValue && !itemItemCompra.DataExclusao.HasValue && itemItemCompra.IdentificadorListaCompra != IdentificadorListaCompra)
                {
                    ListaCompra itemLista = biz.SelecionarListaCompra(itemItemCompra.IdentificadorListaCompra);
                    itemLista.Status = (int)enumStatusListaCompra.Comprado;
                    itemLista.DataAtualizacao = DateTime.Now.ToUniversalTime();

                    biz.SalvarListaCompra(itemLista);
                }
            }
            if (itemResultado.Sucesso && itemItemCompra.DataExclusao.HasValue)
                itemResultado.Mensagens = new MensagemErro[] { new MensagemErro() { Mensagem = MensagemBusiness.RetornaMensagens("Viagem_ExcluirItemCompra_OK") } };

            return itemResultado;
            
            
        }
    }
}