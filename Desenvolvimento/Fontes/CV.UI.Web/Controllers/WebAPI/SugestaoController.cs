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
    public partial class SugestaoController : BaseApiController
    {
        [Authorize]
        [BindJson(typeof(CriterioBusca), "json")]
        public ResultadoConsultaTipo<Sugestao> Get(CriterioBusca json)
        {
            ResultadoConsultaTipo<Sugestao> resultado = new ResultadoConsultaTipo<Sugestao>();
            ViagemBusiness biz = new ViagemBusiness();
           
            List<Sugestao> _itens = biz.ListarSugestao().ToList();
resultado.TotalRegistros = _itens.Count();
            if (json.SortField != null && json.SortField.Any())
                _itens = _itens.AsQueryable().OrderByField<Sugestao>(json.SortField, json.SortOrder).ToList();

            if (json.Index.HasValue && json.Count.HasValue)
                _itens = _itens.Skip(json.Index.Value).Take(json.Count.Value).ToList();
            resultado.Lista = _itens;

            return resultado;
        }
        [Authorize]
        public Sugestao Get(int id)
        {
            ViagemBusiness biz = new ViagemBusiness();
            Sugestao itemSugestao = biz.SelecionarSugestao(id);
          
            return itemSugestao;
        }
        [Authorize]
        public ResultadoOperacao Post([FromBody] Sugestao itemSugestao)
        {
            ViagemBusiness biz = new ViagemBusiness();
                      biz.SalvarSugestao(itemSugestao);
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            itemResultado.Sucesso = biz.IsValid();
            itemResultado.Mensagens = biz.RetornarMensagens.ToArray();
            if (itemResultado.Sucesso)
                itemResultado.IdentificadorRegistro = itemSugestao.Identificador;
            return itemResultado;
        }
        [Authorize]
        public ResultadoOperacao Delete(int id)
        {
            ViagemBusiness biz = new ViagemBusiness();
            Sugestao itemSugestao = biz.SelecionarSugestao(id);
            biz.ExcluirSugestao(itemSugestao);
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            itemResultado.Sucesso = biz.IsValid();
            itemResultado.Mensagens = biz.RetornarMensagens.ToArray();

            return itemResultado;
        }
    }
}