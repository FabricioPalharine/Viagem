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
    public partial class RefeicaoController : BaseApiController
    {
        [Authorize]
        [BindJson(typeof(CriterioBusca), "json")]
        public ResultadoConsultaTipo<Refeicao> Get(CriterioBusca json)
        {
            ResultadoConsultaTipo<Refeicao> resultado = new ResultadoConsultaTipo<Refeicao>();
            ViagemBusiness biz = new ViagemBusiness();

            List<Refeicao> _itens = biz.ListarRefeicao(token.IdentificadorViagem.GetValueOrDefault(), json.DataInicioDe, json.DataInicioAte,
                json.Nome, json.Tipo, json.IdentificadorCidade, json.Identificador).ToList();

            resultado.Lista = _itens;

            return resultado;
        }

        [Authorize]
        public Refeicao Get(int id)
        {
            ViagemBusiness biz = new ViagemBusiness();
            Refeicao itemRefeicao = biz.SelecionarRefeicao_Completa(id);
            foreach (var item in itemRefeicao.Fotos)
            {
                item.ItemRefeicao = null;
                item.ItemFoto.Refeicoes = null;
            }

            foreach (var item in itemRefeicao.Gastos)
            {
                item.ItemRefeicao = null;
                item.ItemGasto.Refeicoes = null;
            }
            foreach (var item in itemRefeicao.Pedidos)
            {
                item.ItemRefeicao = null;
            }
            return itemRefeicao;
        }

        [Authorize]
        public ResultadoOperacao Post([FromBody] Refeicao itemRefeicao)
        {
            ViagemBusiness biz = new ViagemBusiness();
            itemRefeicao.IdentificadorCidade = biz.RetornarCidadeGeocoding(itemRefeicao.Latitude, itemRefeicao.Longitude);
            itemRefeicao.IdentificadorViagem = token.IdentificadorViagem;
            itemRefeicao.DataAtualizacao = DateTime.Now;
            biz.SalvarRefeicao(itemRefeicao);
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            itemResultado.Sucesso = biz.IsValid();
            itemResultado.Mensagens = biz.RetornarMensagens.ToArray();
            if (itemResultado.Sucesso)
            {
                itemResultado.IdentificadorRegistro = itemRefeicao.Identificador;
                itemRefeicao.Pedidos = biz.ListarRefeicaoPedido(d => d.IdentificadorRefeicao == itemRefeicao.Identificador);
                itemResultado.ItemRegistro = itemRefeicao;
            }
            return itemResultado;
        }
        [Authorize]
        public ResultadoOperacao Delete(int id)
        {
            ViagemBusiness biz = new ViagemBusiness();
            Refeicao itemRefeicao = biz.SelecionarRefeicao_Completa(id);
            itemRefeicao.DataExclusao = DateTime.Now;
            itemRefeicao.Pedidos.ToList().ForEach(d => d.DataExclusao = DateTime.Now);
            itemRefeicao.Gastos.ToList().ForEach(d => d.DataExclusao = DateTime.Now);
            itemRefeicao.Fotos.ToList().ForEach(d => d.DataExclusao = DateTime.Now);
            biz.SalvarRefeicao_Completo(itemRefeicao);
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            itemResultado.Sucesso = biz.IsValid();
            itemResultado.Mensagens = biz.RetornarMensagens.ToArray();

            return itemResultado;
        }

        [Authorize]
        [ActionName("CarregarFoto")]
        [HttpGet]
        public List<Refeicao> CarregarFoto()
        {
            ViagemBusiness biz = new ViagemBusiness();
            return biz.ListarRefeicao(d => d.IdentificadorViagem == token.IdentificadorViagem );
        }
    }
}