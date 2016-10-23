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
           
            List<CotacaoMoeda> _itens = biz.ListarCotacaoMoeda().ToList();
resultado.TotalRegistros = _itens.Count();
            if (json.SortField != null && json.SortField.Any())
                _itens = _itens.AsQueryable().OrderByField<CotacaoMoeda>(json.SortField, json.SortOrder).ToList();

            if (json.Index.HasValue && json.Count.HasValue)
                _itens = _itens.Skip(json.Index.Value).Take(json.Count.Value).ToList();
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
                      biz.SalvarCotacaoMoeda(itemCotacaoMoeda);
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            itemResultado.Sucesso = biz.IsValid();
            itemResultado.Mensagens = biz.RetornarMensagens.ToArray();
            if (itemResultado.Sucesso)
                itemResultado.IdentificadorRegistro = itemCotacaoMoeda.Identificador;
            return itemResultado;
        }
        [Authorize]
        public ResultadoOperacao Delete(int id)
        {
            ViagemBusiness biz = new ViagemBusiness();
            CotacaoMoeda itemCotacaoMoeda = biz.SelecionarCotacaoMoeda(id);
            biz.ExcluirCotacaoMoeda(itemCotacaoMoeda);
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            itemResultado.Sucesso = biz.IsValid();
            itemResultado.Mensagens = biz.RetornarMensagens.ToArray();

            return itemResultado;
        }
    }
}