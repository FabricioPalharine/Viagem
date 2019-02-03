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
            itemGasto.DataAtualizacao = DateTime.Now.ToUniversalTime();
            if (!itemGasto.IdentificadorCidade.HasValue)
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
            itemGasto.DataExclusao = DateTime.Now.ToUniversalTime();
            foreach (var item in itemGasto.Alugueis.Where(d => !d.DataExclusao.HasValue))
                item.DataExclusao = DateTime.Now.ToUniversalTime();
            foreach (var item in itemGasto.Atracoes.Where(d => !d.DataExclusao.HasValue))
                item.DataExclusao = DateTime.Now.ToUniversalTime();
            foreach (var item in itemGasto.Compras.Where(d => !d.DataExclusao.HasValue))
                item.DataExclusao = DateTime.Now.ToUniversalTime();
            foreach (var item in itemGasto.Hoteis.Where(d => !d.DataExclusao.HasValue))
                item.DataExclusao = DateTime.Now.ToUniversalTime();
            foreach (var item in itemGasto.Reabastecimentos.Where(d => !d.DataExclusao.HasValue))
                item.DataExclusao = DateTime.Now.ToUniversalTime();
            foreach (var item in itemGasto.Refeicoes.Where(d => !d.DataExclusao.HasValue))
                item.DataExclusao = DateTime.Now.ToUniversalTime();           
            foreach (var item in itemGasto.ViagenAereas.Where(d => !d.DataExclusao.HasValue))
                item.DataExclusao = DateTime.Now.ToUniversalTime();


            biz.SalvarGasto_Completo(itemGasto);
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            itemResultado.Sucesso = biz.IsValid();
            itemResultado.Mensagens = biz.RetornarMensagens.ToArray();
            if (itemResultado.Sucesso)
                itemResultado.Mensagens = new MensagemErro[] { new MensagemErro() { Mensagem = MensagemBusiness.RetornaMensagens("Viagem_ExcluirGasto_OK") } };
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

            if (itemGasto.DataExclusao.HasValue || !itemGastoBase.Atracoes.Where(f => f.IdentificadorAtracao == itemGasto.IdentificadorAtracao).Where(d => !d.DataExclusao.HasValue).Any())
            {
                biz.SalvarGastoAtracao(itemGasto);
                itemResultado.Sucesso = biz.IsValid();
                itemResultado.Mensagens = biz.RetornarMensagens.ToArray();
                if (itemResultado.Sucesso)
                {
                    itemResultado.IdentificadorRegistro = itemGasto.Identificador;
                    if (itemGasto.DataExclusao.HasValue)
                        itemResultado.Mensagens = new MensagemErro[] { new MensagemErro() { Mensagem = MensagemBusiness.RetornaMensagens("Viagem_ExcluirGastoAtracao_OK") } };

                }
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

            if (itemGasto.DataExclusao.HasValue || !itemGastoBase.Refeicoes.Where(f => f.IdentificadorRefeicao == itemGasto.IdentificadorRefeicao).Where(d => !d.DataExclusao.HasValue).Any())
            {
                biz.SalvarGastoRefeicao(itemGasto);
                itemResultado.Sucesso = biz.IsValid();
                itemResultado.Mensagens = biz.RetornarMensagens.ToArray();
                if (itemResultado.Sucesso)
                {
                    itemResultado.IdentificadorRegistro = itemGasto.Identificador;
                    if (itemGasto.DataExclusao.HasValue)
                        itemResultado.Mensagens = new MensagemErro[] { new MensagemErro() { Mensagem = MensagemBusiness.RetornaMensagens("Viagem_ExcluirGastoRefeicao_OK") } };
                }
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

            if (itemGasto.DataExclusao.HasValue || !itemGastoBase.Hoteis.Where(f => f.IdentificadorHotel == itemGasto.IdentificadorHotel).Where(d=>!d.DataExclusao.HasValue).Any())
            {
                biz.SalvarGastoHotel(itemGasto);
                itemResultado.Sucesso = biz.IsValid();
                itemResultado.Mensagens = biz.RetornarMensagens.ToArray();
                if (itemResultado.Sucesso)
                {
                    itemResultado.IdentificadorRegistro = itemGasto.Identificador;
                    if (itemGasto.DataExclusao.HasValue)
                        itemResultado.Mensagens = new MensagemErro[] { new MensagemErro() { Mensagem = MensagemBusiness.RetornaMensagens("Viagem_ExcluirGastoHotel_OK") } };
                }
            }
            return itemResultado;
        }

        [Authorize]

        [HttpPost]
        [ActionName("SalvarCustoViagemAerea")]
        public ResultadoOperacao SalvarCustoViagemAerea([FromBody] GastoViagemAerea itemGasto)
        {
            ViagemBusiness biz = new ViagemBusiness();
            var itemGastoBase = biz.SelecionarGasto_Completo(itemGasto.IdentificadorGasto);
            ResultadoOperacao itemResultado = new ResultadoOperacao();

            if (itemGasto.DataExclusao.HasValue || !itemGastoBase.ViagenAereas.Where(f => f.IdentificadorViagemAerea == itemGasto.IdentificadorViagemAerea).Where(d => !d.DataExclusao.HasValue).Any())
            {
                biz.SalvarGastoViagemAerea(itemGasto);
                itemResultado.Sucesso = biz.IsValid();
                itemResultado.Mensagens = biz.RetornarMensagens.ToArray();
                if (itemResultado.Sucesso)
                {
                    itemResultado.IdentificadorRegistro = itemGasto.Identificador;
                    if (itemGasto.DataExclusao.HasValue)
                        itemResultado.Mensagens = new MensagemErro[] { new MensagemErro() { Mensagem = MensagemBusiness.RetornaMensagens("Viagem_ExcluirGastoViagemAerea_OK") } };
                }
            }
            return itemResultado;
        }

        [Authorize]

        [HttpPost]
        [ActionName("SalvarCustoCarro")]
        public ResultadoOperacao SalvarCustoCarro([FromBody] AluguelGasto itemGasto)
        {
            ViagemBusiness biz = new ViagemBusiness();
            var itemGastoBase = biz.SelecionarGasto_Completo(itemGasto.IdentificadorGasto);
            ResultadoOperacao itemResultado = new ResultadoOperacao();

            if (itemGasto.DataExclusao.HasValue || !itemGastoBase.Alugueis.Where(f => f.IdentificadorCarro == itemGasto.IdentificadorCarro).Where(d => !d.DataExclusao.HasValue).Any())
            {
                biz.SalvarAluguelGasto(itemGasto);
                itemResultado.Sucesso = biz.IsValid();
                itemResultado.Mensagens = biz.RetornarMensagens.ToArray();
                if (itemResultado.Sucesso)
                {
                    itemResultado.IdentificadorRegistro = itemGasto.Identificador;
                    if (itemGasto.DataExclusao.HasValue)
                        itemResultado.Mensagens = new MensagemErro[] { new MensagemErro() { Mensagem = MensagemBusiness.RetornaMensagens("Viagem_ExcluirAluguelGasto_OK") } };
                }
            }
            return itemResultado;
        }

        [HttpGet]
        [Authorize]
        [ActionName("getGastoAtracao")]
        public GastoAtracao getGastoAtracao (int Identificador)
        {
            ViagemBusiness biz = new ViagemBusiness();
            return biz.SelecionarGastoAtracao(Identificador);
        }

        [HttpGet]
        [Authorize]
        [ActionName("getGastoHotel")]
        public GastoHotel getGastoHotel(int Identificador)
        {
            ViagemBusiness biz = new ViagemBusiness();
            return biz.SelecionarGastoHotel(Identificador);
        }

        [HttpGet]
        [Authorize]
        [ActionName("getGastoRefeicao")]
        public GastoRefeicao getGastoRefeicao(int Identificador)
        {
            ViagemBusiness biz = new ViagemBusiness();
            return biz.SelecionarGastoRefeicao(Identificador);
        }

        [HttpGet]
        [Authorize]
        [ActionName("getGastoViagemAerea")]
        public GastoViagemAerea getGastoViagemAerea(int Identificador)
        {
            ViagemBusiness biz = new ViagemBusiness();
            return biz.SelecionarGastoViagemAerea(Identificador);
        }

        [HttpGet]
        [Authorize]
        [ActionName("getAluguelGasto")]
        public AluguelGasto getAluguelGasto(int Identificador)
        {
            ViagemBusiness biz = new ViagemBusiness();
            return biz.SelecionarAluguelGasto(Identificador);
        }

        [HttpGet]
        [Authorize]
        [ActionName("getGastoCompra")]
        public GastoCompra getGastoCompra(int Identificador)
        {
            ViagemBusiness biz = new ViagemBusiness();
            var item = biz.SelecionarGastoCompra(Identificador);
            item.ItemGasto.Compras = null;
            item.ItensComprados = null;
            return item;
        }
    }
}