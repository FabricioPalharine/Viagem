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
    public partial class UsuarioController : BaseApiController
    {
        [Authorize]
        [BindJson(typeof(CriterioBusca), "json")]
        public ResultadoConsultaTipo<Usuario> Get(CriterioBusca json)
        {
            ResultadoConsultaTipo<Usuario> resultado = new ResultadoConsultaTipo<Usuario>();
            ViagemBusiness biz = new ViagemBusiness();

            List<Usuario> _itens = biz.ListarUsuario_EMail(json.EMail).ToList();
            resultado.TotalRegistros = _itens.Count();
            if (json.SortField != null && json.SortField.Any())
                _itens = _itens.AsQueryable().OrderByField<Usuario>(json.SortField, json.SortOrder).ToList();

            if (json.Index.HasValue && json.Count.HasValue)
                _itens = _itens.Skip(json.Index.Value).Take(json.Count.Value).ToList();
            resultado.Lista = _itens;

            return resultado;
        }
        [Authorize]
        [ActionName("listaAmigos")]
        [HttpGet]
        public List<Usuario> listaAmigos()
        {
            ViagemBusiness biz = new ViagemBusiness();
            return biz.ListarUsuarioAmigo(token.IdentificadorUsuario);
        }


        [Authorize]
        public Usuario Get(int id)
        {
            ViagemBusiness biz = new ViagemBusiness();
            Usuario itemUsuario = biz.SelecionarUsuario(id);
          
            return itemUsuario;
        }
        [Authorize]
        public ResultadoOperacao Post([FromBody] Usuario itemUsuario)
        {
            ViagemBusiness biz = new ViagemBusiness();
                      biz.SalvarUsuario(itemUsuario);
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            itemResultado.Sucesso = biz.IsValid();
            itemResultado.Mensagens = biz.RetornarMensagens.ToArray();
            if (itemResultado.Sucesso)
                itemResultado.IdentificadorRegistro = itemUsuario.Identificador;
            return itemResultado;
        }
        [Authorize]
        public ResultadoOperacao Delete(int id)
        {
            ViagemBusiness biz = new ViagemBusiness();
            Usuario itemUsuario = biz.SelecionarUsuario(id);
            biz.ExcluirUsuario(itemUsuario);
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            itemResultado.Sucesso = biz.IsValid();
            itemResultado.Mensagens = biz.RetornarMensagens.ToArray();

            return itemResultado;
        }
    }
}