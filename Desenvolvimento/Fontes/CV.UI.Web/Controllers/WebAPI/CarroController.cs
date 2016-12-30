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
    public partial class CarroController : BaseApiController
    {
        [Authorize]
        [BindJson(typeof(CriterioBusca), "json")]
        public ResultadoConsultaTipo<Carro> Get(CriterioBusca json)
        {
            ResultadoConsultaTipo<Carro> resultado = new ResultadoConsultaTipo<Carro>();
            ViagemBusiness biz = new ViagemBusiness();

            List<Carro> _itens = biz.ListarCarro(token.IdentificadorViagem.GetValueOrDefault(), json.Comentario, json.Nome, json.Tipo, json.DataInicioDe, json.DataInicioAte,
                json.DataFimDe, json.DataFimAte, json.Identificador);

            resultado.Lista = _itens;

            return resultado;
        }
        [Authorize]
        public Carro Get(int id)
        {
            ViagemBusiness biz = new ViagemBusiness();
            Carro itemCarro = biz.SelecionarCarro_Completo(id);

            itemCarro.Avaliacoes.ToList().ForEach(d => d.ItemCarro = null);
            itemCarro.Deslocamentos.ToList().ForEach(d => { d.ItemCarro = null; d.Usuarios.ToList().ForEach(e => e.ItemCarroDeslocamento = null); });
            itemCarro.Gastos.ToList().ForEach(d => { d.ItemCarro = null; d.ItemGasto.Alugueis = null; d.ItemGasto.Reabastecimentos = null; });
            foreach (var item in itemCarro.Reabastecimentos)
            {
                item.ItemCarro = null;
                foreach (var itemGasto in item.Gastos)
                {
                    itemGasto.ItemReabastecimento = null;
                    itemGasto.ItemGasto.Alugueis = null;
                    itemGasto.ItemGasto.Reabastecimentos = null;
                }
            }
            return itemCarro;
        }
        [Authorize]
        public ResultadoOperacao Post([FromBody] Carro itemCarro)
        {
            ViagemBusiness biz = new ViagemBusiness();
            itemCarro.IdentificadorViagem = token.IdentificadorViagem;
            itemCarro.DataAtualizacao = DateTime.Now;
            if (itemCarro.ItemCarroEventoDevolucao != null)
            {
                itemCarro.ItemCarroEventoDevolucao.IdentificadorCidade = biz.RetornarCidadeGeocoding(itemCarro.ItemCarroEventoDevolucao.Latitude, itemCarro.ItemCarroEventoDevolucao.Longitude);
                itemCarro.ItemCarroEventoDevolucao.DataAtualizacao = DateTime.Now;
            }
            if (itemCarro.ItemCarroEventoRetirada != null)
            {
                itemCarro.ItemCarroEventoRetirada.IdentificadorCidade = biz.RetornarCidadeGeocoding(itemCarro.ItemCarroEventoRetirada.Latitude, itemCarro.ItemCarroEventoRetirada.Longitude);
                itemCarro.ItemCarroEventoRetirada.DataAtualizacao = DateTime.Now;

            }
            biz.SalvarCarro_Evento(itemCarro);
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            itemResultado.Sucesso = biz.IsValid();
            itemResultado.Mensagens = biz.RetornarMensagens.ToArray();
            if (itemResultado.Sucesso)
            {
                itemResultado.IdentificadorRegistro = itemCarro.Identificador;
                itemCarro.Avaliacoes = null;
                itemCarro.Deslocamentos = null;
                itemResultado.ItemRegistro = itemCarro;
            }
            return itemResultado;
        }
        [Authorize]
        public ResultadoOperacao Delete(int id)
        {
            ViagemBusiness biz = new ViagemBusiness();
            Carro itemCarro = biz.SelecionarCarro_Completo(id);
            itemCarro.DataExclusao = DateTime.Now;
            itemCarro.Avaliacoes.ToList().ForEach(d => d.DataExclusao = DateTime.Now);
            itemCarro.Gastos.ToList().ForEach(d => d.DataExclusao = DateTime.Now);
            itemCarro.Deslocamentos.ToList().ForEach(d => d.DataExclusao = DateTime.Now);
            foreach (var item in itemCarro.Reabastecimentos)
            {
                item.DataExclusao = DateTime.Now;
                foreach (var itemGasto in item.Gastos)
                {
                    itemGasto.DataExclusao = DateTime.Now;
                }
            }
            biz.SalvarCarro_Completo(itemCarro);
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            itemResultado.Sucesso = biz.IsValid();
            itemResultado.Mensagens = biz.RetornarMensagens.ToArray();

            return itemResultado;
        }

        [Authorize]
        [HttpPost]
        [ActionName("SalvarCarroDeslocamento")]
        public ResultadoOperacao SalvarCarroDeslocamento([FromBody] CarroDeslocamento itemCarroDeslocamento)
        {
            ViagemBusiness biz = new ViagemBusiness();
            itemCarroDeslocamento.DataAtualizacao = DateTime.Now;
            if (itemCarroDeslocamento.ItemCarroEventoChegada != null)
            {
                itemCarroDeslocamento.ItemCarroEventoChegada.IdentificadorCidade = biz.RetornarCidadeGeocoding(itemCarroDeslocamento.ItemCarroEventoChegada.Latitude, itemCarroDeslocamento.ItemCarroEventoChegada.Longitude);
                itemCarroDeslocamento.ItemCarroEventoChegada.DataAtualizacao = DateTime.Now;
            }
            if (itemCarroDeslocamento.ItemCarroEventoPartida != null)
            {
                itemCarroDeslocamento.ItemCarroEventoPartida.IdentificadorCidade = biz.RetornarCidadeGeocoding(itemCarroDeslocamento.ItemCarroEventoPartida.Latitude, itemCarroDeslocamento.ItemCarroEventoPartida.Longitude);
                itemCarroDeslocamento.ItemCarroEventoPartida.DataAtualizacao = DateTime.Now;
            }

            biz.SalvarCarroDeslocamento_Evento(itemCarroDeslocamento);
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            itemResultado.Sucesso = biz.IsValid();
            itemResultado.Mensagens = biz.RetornarMensagens.ToArray();
            if (itemResultado.Sucesso)
            {
                itemResultado.IdentificadorRegistro = itemCarroDeslocamento.Identificador;
                var itemBase = biz.SelecionarCarroDeslocamento(itemCarroDeslocamento.Identificador);
                itemBase.Usuarios.ToList().ForEach(d => d.ItemCarroDeslocamento = null);
                itemResultado.ItemRegistro = itemBase;
            }
            return itemResultado;
        }

        [Authorize]
        [HttpGet]
        [ActionName("CarregarCarroDeslocamento")]

        public CarroDeslocamento CarregarCarroDeslocamento(int id)
        {
            ViagemBusiness biz = new ViagemBusiness();
            var itemBase = biz.SelecionarCarroDeslocamento(id);
            itemBase.Usuarios.ToList().ForEach(d => d.ItemCarroDeslocamento = null);
            return itemBase;
        }
    }
}