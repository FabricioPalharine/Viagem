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
    public partial class AtracaoController : BaseApiController
    {
        [Authorize]
        [BindJson(typeof(CriterioBusca), "json")]
        public ResultadoConsultaTipo<Atracao> Get(CriterioBusca json)
        {
            ResultadoConsultaTipo<Atracao> resultado = new ResultadoConsultaTipo<Atracao>();
            ViagemBusiness biz = new ViagemBusiness();
           
            List<Atracao> _itens = biz.ListarAtracao(token.IdentificadorViagem.GetValueOrDefault(), json.DataInicioDe, json.DataInicioAte, json.DataFimDe, json.DataFimAte,
                json.Nome, json.Tipo, json.Situacao.GetValueOrDefault(0), json.IdentificadorCidade, json.Identificador).ToList();
          
            resultado.Lista = _itens;

            return resultado;
        }

        [Authorize]
        public Atracao Get(int id)
        {
            ViagemBusiness biz = new ViagemBusiness();
            Atracao itemAtracao = biz.SelecionarAtracao_Completo(id);
            itemAtracao.Atracoes = null;
            itemAtracao.Avaliacoes.ToList().ForEach(d => d.ItemAtracao = null);
            itemAtracao.Fotos.ToList().ForEach(d => { d.ItemAtracao = null; d.ItemFoto.Atracoes = null; });
            itemAtracao.Gastos.ToList().ForEach(d => { d.ItemAtracao = null; d.ItemGasto.Atracoes = null; });
            if (itemAtracao.ItemAtracaoPai != null)
                itemAtracao.ItemAtracaoPai.Atracoes = null;
            return itemAtracao;
        }
        [Authorize]
        public ResultadoOperacao Post([FromBody] Atracao itemAtracao)
        {
            ViagemBusiness biz = new ViagemBusiness();
            itemAtracao.IdentificadorCidade =  biz.RetornarCidadeGeocoding(itemAtracao.Latitude, itemAtracao.Longitude);
            itemAtracao.IdentificadorViagem = token.IdentificadorViagem;
            itemAtracao.DataAtualizacao = DateTime.Now;
            biz.SalvarAtracao(itemAtracao);
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            itemResultado.Sucesso = biz.IsValid();
            itemResultado.Mensagens = biz.RetornarMensagens.ToArray();
            if (itemResultado.Sucesso)
            {
                itemResultado.IdentificadorRegistro = itemAtracao.Identificador;
                itemAtracao.Avaliacoes = biz.ListarAvaliacaoAtracao(d => d.IdentificadorAtracao == itemAtracao.Identificador);
                itemResultado.ItemRegistro = itemAtracao;
            }
            return itemResultado;
        }
        [Authorize]
        public ResultadoOperacao Delete(int id)
        {
            ViagemBusiness biz = new ViagemBusiness();
            Atracao itemAtracao = biz.SelecionarAtracao_Completo(id);
            itemAtracao.DataExclusao = DateTime.Now;
            itemAtracao.Avaliacoes.ToList().ForEach(d => d.DataExclusao = DateTime.Now);
            itemAtracao.Gastos.ToList().ForEach(d => d.DataExclusao = DateTime.Now);
            itemAtracao.Fotos.ToList().ForEach(d => d.DataExclusao = DateTime.Now);
            biz.SalvarAtracao_Completo(itemAtracao);
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            itemResultado.Sucesso = biz.IsValid();
            itemResultado.Mensagens = biz.RetornarMensagens.ToArray();

            return itemResultado;
        }

        [Authorize]
        [ActionName("CarregarFoto")]
        [HttpGet]
        public List<Atracao> CarregarFoto()
        {
            ViagemBusiness biz = new ViagemBusiness();
            var Lista = biz.ListarAtracao(d => d.IdentificadorViagem == token.IdentificadorViagem );
            foreach (var itemAtracai in Lista)
            {
                if (itemAtracai.ItemAtracaoPai != null)
                    itemAtracai.ItemAtracaoPai.Atracoes = null;
            }
            return Lista;
        }

        [Authorize]
        [ActionName("VerificarAtracaoAberto")]
        [HttpGet]
        public Atracao VerificarAtracaoAberto()
        {
            ViagemBusiness biz = new ViagemBusiness();
            var itemAtracao = biz.ListarAtracao(d => d.IdentificadorViagem == token.IdentificadorViagem && d.Chegada.HasValue && (!d.Partida.HasValue || d.Partida >=  DateTime.Now))
                .OrderByDescending(d=>d.Chegada).FirstOrDefault();
            itemAtracao.ItemAtracaoPai = null;
            itemAtracao.Atracoes = null;
            return itemAtracao;
        }


       
    }
}