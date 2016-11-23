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
    public partial class GastoController : BaseApiController
    {
        [Authorize]
        [BindJson(typeof(CriterioBusca), "json")]
        public ResultadoConsultaTipo<Gasto> Get(CriterioBusca json)
        {
            ResultadoConsultaTipo<Gasto> resultado = new ResultadoConsultaTipo<Gasto>();
            ViagemBusiness biz = new ViagemBusiness();

            List<Gasto> _itens = biz.ListarGasto(token.IdentificadorViagem.GetValueOrDefault(), json.DataInicioDe, json.DataInicioAte, json.Nome, json.IdentificadorParticipante, json.Identificador).ToList();
            resultado.TotalRegistros = _itens.Count();
            if (json.SortField != null && json.SortField.Any())
                _itens = _itens.AsQueryable().OrderByField<Gasto>(json.SortField, json.SortOrder).ToList();

            if (json.Index.HasValue && json.Count.HasValue)
                _itens = _itens.Skip(json.Index.Value).Take(json.Count.Value).ToList();
            resultado.Lista = _itens;

            return resultado;
        }
        [Authorize]
        public Gasto Get(int id)
        {
            ViagemBusiness biz = new ViagemBusiness();
            Gasto itemGasto = biz.SelecionarGasto_Completo(id);
            foreach (var item in itemGasto.Alugueis)
                item.ItemGasto = null;
            foreach (var item in itemGasto.Atracoes)
                item.ItemGasto = null;
            foreach (var item in itemGasto.Compras)
                item.ItemGasto = null;
            foreach (var item in itemGasto.Hoteis)
                item.ItemGasto = null;
            foreach (var item in itemGasto.ViagenAereas)
                item.ItemGasto = null;
            foreach (var item in itemGasto.Reabastecimentos)
                item.ItemGasto = null;
            foreach (var item in itemGasto.Refeicoes)
                item.ItemGasto = null;
            return itemGasto;
        }
        [Authorize]
        public ResultadoOperacao Post([FromBody] Gasto itemGasto)
        {
            ViagemBusiness biz = new ViagemBusiness();
            itemGasto.IdentificadorViagem = token.IdentificadorViagem;
            itemGasto.DataAtualizacao = DateTime.Now;
            itemGasto.IdentificadorCidade = biz.RetornarCidadeGeocoding(itemGasto.Latitude, itemGasto.Longitude);
            biz.SalvarGasto_Completo(itemGasto);
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            itemResultado.Sucesso = biz.IsValid();
            itemResultado.Mensagens = biz.RetornarMensagens.ToArray();
            if (itemResultado.Sucesso)
            {
                itemResultado.IdentificadorRegistro = itemGasto.Identificador;
                itemGasto = biz.SelecionarGasto_Completo(itemGasto.Identificador);
                foreach (var item in itemGasto.Alugueis)
                    item.ItemGasto = null;
                foreach (var item in itemGasto.Atracoes)
                    item.ItemGasto = null;
                foreach (var item in itemGasto.Compras)
                    item.ItemGasto = null;
                foreach (var item in itemGasto.Hoteis)
                    item.ItemGasto = null;
                foreach (var item in itemGasto.ViagenAereas)
                    item.ItemGasto = null;
                foreach (var item in itemGasto.Reabastecimentos)
                    item.ItemGasto = null;
                foreach (var item in itemGasto.Refeicoes)
                    item.ItemGasto = null;
                itemResultado.ItemRegistro = itemGasto;
            }
            return itemResultado;
        }

        [Authorize]
        public ResultadoOperacao Delete(int id)
        {
            ViagemBusiness biz = new ViagemBusiness();
            Gasto itemGasto = biz.SelecionarGasto_Completo(id);
            itemGasto.DataExclusao = DateTime.Now;
            foreach (var item in itemGasto.Alugueis.Where(d => !d.DataExclusao.HasValue))
                item.DataExclusao = DateTime.Now;
            foreach (var item in itemGasto.Atracoes.Where(d => !d.DataExclusao.HasValue))
                item.DataExclusao = DateTime.Now;
            foreach (var item in itemGasto.Compras.Where(d => !d.DataExclusao.HasValue))
                item.DataExclusao = DateTime.Now;
            foreach (var item in itemGasto.Hoteis.Where(d => !d.DataExclusao.HasValue))
                item.DataExclusao = DateTime.Now;
            foreach (var item in itemGasto.Reabastecimentos.Where(d => !d.DataExclusao.HasValue))
                item.DataExclusao = DateTime.Now;
            foreach (var item in itemGasto.Refeicoes.Where(d => !d.DataExclusao.HasValue))
                item.DataExclusao = DateTime.Now;           
            foreach (var item in itemGasto.ViagenAereas.Where(d => !d.DataExclusao.HasValue))
                item.DataExclusao = DateTime.Now;


            biz.SalvarGasto_Completo(itemGasto);
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            itemResultado.Sucesso = biz.IsValid();
            itemResultado.Mensagens = biz.RetornarMensagens.ToArray();

            return itemResultado;
        }

        [Authorize]
        [HttpPost]
        [ActionName("SalvarCustoAtracao")]
        public ResultadoOperacao SalvarCustoAtracao([FromBody] GastoAtracao itemGasto)
        {
            ViagemBusiness biz = new ViagemBusiness();
            var itemGastoBase = biz.SelecionarGasto_Completo(itemGasto.IdentificadorGasto);
            ResultadoOperacao itemResultado = new ResultadoOperacao();

            if (itemGasto.DataExclusao.HasValue || !itemGastoBase.Atracoes.Where(f => f.IdentificadorAtracao == itemGasto.IdentificadorAtracao).Any())
            {
                biz.SalvarGastoAtracao(itemGasto);
                itemResultado.Sucesso = biz.IsValid();
                itemResultado.Mensagens = biz.RetornarMensagens.ToArray();
                if (itemResultado.Sucesso)
                    itemResultado.IdentificadorRegistro = itemGasto.Identificador;
            }
            return itemResultado;
        }

        [Authorize]
        [HttpPost]
        [ActionName("SalvarCustoRefeicao")]
        public ResultadoOperacao SalvarCustoRefeicao([FromBody] GastoRefeicao itemGasto)
        {
            ViagemBusiness biz = new ViagemBusiness();
            var itemGastoBase = biz.SelecionarGasto_Completo(itemGasto.IdentificadorGasto);
            ResultadoOperacao itemResultado = new ResultadoOperacao();

            if (itemGasto.DataExclusao.HasValue || !itemGastoBase.Refeicoes.Where(f => f.IdentificadorRefeicao == itemGasto.IdentificadorRefeicao).Any())
            {
                biz.SalvarGastoRefeicao(itemGasto);
                itemResultado.Sucesso = biz.IsValid();
                itemResultado.Mensagens = biz.RetornarMensagens.ToArray();
                if (itemResultado.Sucesso)
                    itemResultado.IdentificadorRegistro = itemGasto.Identificador;
            }
            return itemResultado;
        }

        [Authorize]
        [HttpPost]
        [ActionName("SalvarCustoHotel")]
        public ResultadoOperacao SalvarCustoHotel([FromBody] GastoHotel itemGasto)
        {
            ViagemBusiness biz = new ViagemBusiness();
            var itemGastoBase = biz.SelecionarGasto_Completo(itemGasto.IdentificadorGasto);
            ResultadoOperacao itemResultado = new ResultadoOperacao();

            if (itemGasto.DataExclusao.HasValue || !itemGastoBase.Hoteis.Where(f => f.IdentificadorHotel == itemGasto.IdentificadorHotel).Any())
            {
                biz.SalvarGastoHotel(itemGasto);
                itemResultado.Sucesso = biz.IsValid();
                itemResultado.Mensagens = biz.RetornarMensagens.ToArray();
                if (itemResultado.Sucesso)
                    itemResultado.IdentificadorRegistro = itemGasto.Identificador;
            }
            return itemResultado;
        }


        [HttpPost]
        [ActionName("SalvarCustoViagemAerea")]
        public ResultadoOperacao SalvarCustoViagemAerea([FromBody] GastoViagemAerea itemGasto)
        {
            ViagemBusiness biz = new ViagemBusiness();
            var itemGastoBase = biz.SelecionarGasto_Completo(itemGasto.IdentificadorGasto);
            ResultadoOperacao itemResultado = new ResultadoOperacao();

            if (itemGasto.DataExclusao.HasValue || !itemGastoBase.ViagenAereas.Where(f => f.IdentificadorViagemAerea == itemGasto.IdentificadorViagemAerea).Any())
            {
                biz.SalvarGastoViagemAerea(itemGasto);
                itemResultado.Sucesso = biz.IsValid();
                itemResultado.Mensagens = biz.RetornarMensagens.ToArray();
                if (itemResultado.Sucesso)
                    itemResultado.IdentificadorRegistro = itemGasto.Identificador;
            }
            return itemResultado;
        }


        [HttpPost]
        [ActionName("SalvarCustoCarro")]
        public ResultadoOperacao SalvarCustoCarro([FromBody] AluguelGasto itemGasto)
        {
            ViagemBusiness biz = new ViagemBusiness();
            var itemGastoBase = biz.SelecionarGasto_Completo(itemGasto.IdentificadorGasto);
            ResultadoOperacao itemResultado = new ResultadoOperacao();

            if (itemGasto.DataExclusao.HasValue || !itemGastoBase.Alugueis.Where(f => f.IdentificadorCarro == itemGasto.IdentificadorCarro).Any())
            {
                biz.SalvarAluguelGasto(itemGasto);
                itemResultado.Sucesso = biz.IsValid();
                itemResultado.Mensagens = biz.RetornarMensagens.ToArray();
                if (itemResultado.Sucesso)
                    itemResultado.IdentificadorRegistro = itemGasto.Identificador;
            }
            return itemResultado;
        }
    }
}