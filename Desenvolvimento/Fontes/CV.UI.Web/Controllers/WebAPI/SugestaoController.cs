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
           
            List<Sugestao> _itens = biz.ListarSugestao(token.IdentificadorUsuario, token.IdentificadorViagem, json.Nome, json.Tipo, json.IdentificadorCidade, null).ToList();
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
            if (!itemSugestao.IdentificadorUsuario.HasValue)
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

        [Authorize]
        [HttpGet]
        [ActionName("listarConsulta")]
        [BindJson(typeof(CriterioBusca), "json")]
        public List<Sugestao> listarConsulta(CriterioBusca json)
        {
            ViagemBusiness biz = new ViagemBusiness();
            List<Sugestao> _itens = biz.ListarSugestao(json.IdentificadorParticipante, token.IdentificadorViagem, json.Nome, json.Tipo, json.IdentificadorCidade, json.Situacao).ToList();
            foreach(var itemSugestao in _itens.Where(d=>d.Status == 0))
            {
                itemSugestao.Status = 1;
                biz.SalvarSugestao(itemSugestao);
            }
            return _itens;
        }

        [Authorize]
        [HttpPost]
        [ActionName("AgendarSugestao")]
        public ResultadoOperacao AgendarSugestao([FromBody] AgendarSugestao itemAgendar)
        {
            ViagemBusiness biz = new ViagemBusiness();
            itemAgendar.itemCalendario.Nome = itemAgendar.itemSugestao.Local;
            itemAgendar.itemCalendario.Tipo = itemAgendar.itemSugestao.Tipo;
            itemAgendar.itemCalendario.Latitude = itemAgendar.itemSugestao.Latitude;
            itemAgendar.itemCalendario.Longitude = itemAgendar.itemSugestao.Longitude;
            itemAgendar.itemCalendario.CodigoPlace = itemAgendar.itemSugestao.CodigoPlace;

            itemAgendar.itemCalendario.IdentificadorViagem = token.IdentificadorViagem;
            itemAgendar.itemCalendario.DataAtualizacao = DateTime.Now;
            biz.SalvarCalendarioPrevisto(itemAgendar.itemCalendario);
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            itemResultado.Sucesso = biz.IsValid();
            itemResultado.Mensagens = biz.RetornarMensagens.ToArray();
            if (itemResultado.Sucesso)
            {
                itemResultado.IdentificadorRegistro = itemAgendar.itemCalendario.Identificador;
                itemAgendar.itemSugestao.Status = 2;
                itemAgendar.itemSugestao.DataAtualizacao = DateTime.Now;
                biz.SalvarSugestao(itemAgendar.itemSugestao);
            }
            return itemResultado;
        }
    }
}