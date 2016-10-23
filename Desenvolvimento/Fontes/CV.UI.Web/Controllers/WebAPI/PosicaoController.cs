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
    public partial class PosicaoController : BaseApiController
    {
        [Authorize]
        [BindJson(typeof(CriterioBusca), "json")]
        public ResultadoConsultaTipo<Posicao> Get(CriterioBusca json)
        {
            ResultadoConsultaTipo<Posicao> resultado = new ResultadoConsultaTipo<Posicao>();
            ViagemBusiness biz = new ViagemBusiness();
           
            List<Posicao> _itens = biz.ListarPosicao().ToList();
resultado.TotalRegistros = _itens.Count();
            if (json.SortField != null && json.SortField.Any())
                _itens = _itens.AsQueryable().OrderByField<Posicao>(json.SortField, json.SortOrder).ToList();

            if (json.Index.HasValue && json.Count.HasValue)
                _itens = _itens.Skip(json.Index.Value).Take(json.Count.Value).ToList();
            resultado.Lista = _itens;

            return resultado;
        }
        [Authorize]
        public Posicao Get(int id)
        {
            ViagemBusiness biz = new ViagemBusiness();
            Posicao itemPosicao = biz.SelecionarPosicao(id);
          
            return itemPosicao;
        }
        [Authorize]
        public ResultadoOperacao Post([FromBody] Posicao itemPosicao)
        {
            ViagemBusiness biz = new ViagemBusiness();
                      biz.SalvarPosicao(itemPosicao);
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            itemResultado.Sucesso = biz.IsValid();
            itemResultado.Mensagens = biz.RetornarMensagens.ToArray();
            if (itemResultado.Sucesso)
                itemResultado.IdentificadorRegistro = itemPosicao.Identificador;
            return itemResultado;
        }
        [Authorize]
        public ResultadoOperacao Delete(int id)
        {
            ViagemBusiness biz = new ViagemBusiness();
            Posicao itemPosicao = biz.SelecionarPosicao(id);
            biz.ExcluirPosicao(itemPosicao);
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            itemResultado.Sucesso = biz.IsValid();
            itemResultado.Mensagens = biz.RetornarMensagens.ToArray();

            return itemResultado;
        }
    }
}