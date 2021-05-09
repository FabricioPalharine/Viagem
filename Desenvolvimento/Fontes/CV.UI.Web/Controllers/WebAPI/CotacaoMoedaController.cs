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
    public partial class CotacaoMoedaController : BaseApiController
    {
        [Authorize]
        [BindJson(typeof(CriterioBusca), "json")]
        public ResultadoConsultaTipo<CotacaoMoeda> Get(CriterioBusca json)
        {
            ResultadoConsultaTipo<CotacaoMoeda> resultado = new ResultadoConsultaTipo<CotacaoMoeda>();
            ViagemBusiness biz = new ViagemBusiness();

            List<CotacaoMoeda> _itens = biz.ListarCotacaoMoeda(d => d.IdentificadorViagem == token.IdentificadorViagem && !d.DataExclusao.HasValue).ToList();
            if (json != null)
            {
                if (json.Moeda.HasValue)
                    _itens = _itens.Where(d => d.Moeda == json.Moeda).ToList();
                if (json.DataInicioDe.HasValue)
                    _itens = _itens.Where(d => d.DataCotacao >= json.DataInicioDe).ToList();
                if (json.DataInicioAte.HasValue)
                    _itens = _itens.Where(d => d.DataCotacao >= json.DataInicioAte).ToList();
            }
            resultado.Lista = _itens;

            return resultado;
        }
        [Authorize]
        public CotacaoMoeda Get(int id)
        {
            ViagemBusiness biz = new ViagemBusiness();
            CotacaoMoeda itemCotacaoMoeda = biz.SelecionarCotacaoMoeda(id);

            return itemCotacaoMoeda;
        }
        [Authorize]
        public ResultadoOperacao Post([FromBody] CotacaoMoeda itemCotacaoMoeda)
        {
            ViagemBusiness biz = new ViagemBusiness();
            itemCotacaoMoeda.IdentificadorViagem = token.IdentificadorViagem;
            itemCotacaoMoeda.DataAtualizacao = DateTime.Now.ToUniversalTime();
            biz.SalvarCotacaoMoeda(itemCotacaoMoeda);
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            itemResultado.Sucesso = biz.IsValid();
            itemResultado.Mensagens = biz.RetornarMensagens.ToArray();
            if (itemResultado.Sucesso)
            {
                itemResultado.IdentificadorRegistro = itemCotacaoMoeda.Identificador;
                itemResultado.ItemRegistro = itemCotacaoMoeda;
            }
            return itemResultado;
        }
        [Authorize]
        public ResultadoOperacao Delete(int id)
        {
            ViagemBusiness biz = new ViagemBusiness();
            CotacaoMoeda itemCotacaoMoeda = biz.SelecionarCotacaoMoeda(id);
            itemCotacaoMoeda.DataExclusao = DateTime.Now.ToUniversalTime();
            biz.SalvarCotacaoMoeda(itemCotacaoMoeda);
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            itemResultado.Sucesso = biz.IsValid();
            itemResultado.Mensagens = biz.RetornarMensagens.ToArray();
            if (itemResultado.Sucesso)
                itemResultado.Mensagens = new MensagemErro[] { new MensagemErro() { Mensagem = MensagemBusiness.RetornaMensagens("Viagem_ExcluirCotacaoMoeda_OK") } };
            return itemResultado;
        }
    }
}