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
           
            List<Sugestao> _itens = biz.ListarSugestao(token.IdentificadorUsuario, token.IdentificadorViagem, json.Nome, json.Tipo).ToList();
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
            itemSugestao.DataAtualizacao = DateTime.Now;
            itemSugestao.IdentificadorViagem = token.IdentificadorViagem;
            itemSugestao.IdentificadorUsuario = token.IdentificadorUsuario;
            itemSugestao.IdentificadorCidade = biz.RetornarCidadeGeocoding(itemSugestao.Latitude, itemSugestao.Longitude);
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
            itemSugestao.DataExclusao = DateTime.Now;
            biz.SalvarSugestao(itemSugestao);
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            itemResultado.Sucesso = biz.IsValid();
            itemResultado.Mensagens = biz.RetornarMensagens.ToArray();

            return itemResultado;
        }
    }
}