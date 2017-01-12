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
    public partial class ComentarioController : BaseApiController
    {
        [Authorize]
        [BindJson(typeof(CriterioBusca), "json")]
        public ResultadoConsultaTipo<Comentario> Get(CriterioBusca json)
        {
            ResultadoConsultaTipo<Comentario> resultado = new ResultadoConsultaTipo<Comentario>();
            ViagemBusiness biz = new ViagemBusiness();

            List<Comentario> _itens = biz.ListarComentario(token.IdentificadorUsuario, token.IdentificadorViagem.GetValueOrDefault(), json.DataInicioDe, json.DataInicioAte,
                json.IdentificadorCidade, json.Identificador).ToList();

            resultado.Lista = _itens;

            return resultado;
        }
        [Authorize]
        public Comentario Get(int id)
        {
            ViagemBusiness biz = new ViagemBusiness();
            Comentario itemComentario = biz.SelecionarComentario(id);
          
            return itemComentario;
        }

        [Authorize]
        public ResultadoOperacao Post([FromBody] Comentario itemComentario)
        {
            ViagemBusiness biz = new ViagemBusiness();
            itemComentario.DataAtualizacao = DateTime.Now.ToUniversalTime();
            itemComentario.IdentificadorUsuario = token.IdentificadorUsuario;
            itemComentario.IdentificadorViagem = token.IdentificadorViagem;
            itemComentario.IdentificadorCidade = biz.RetornarCidadeGeocoding(itemComentario.Latitude, itemComentario.Longitude);

            biz.SalvarComentario(itemComentario);
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            itemResultado.Sucesso = biz.IsValid();
            itemResultado.Mensagens = biz.RetornarMensagens.ToArray();
            if (itemResultado.Sucesso)
                itemResultado.IdentificadorRegistro = itemComentario.Identificador;
            return itemResultado;
        }
        [Authorize]
        public ResultadoOperacao Delete(int id)
        {
            ViagemBusiness biz = new ViagemBusiness();
            Comentario itemComentario = biz.SelecionarComentario(id);
            itemComentario.DataExclusao = DateTime.Now.ToUniversalTime();
            biz.SalvarComentario(itemComentario);
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            itemResultado.Sucesso = biz.IsValid();
            itemResultado.Mensagens = biz.RetornarMensagens.ToArray();
            if (itemResultado.Sucesso)
                itemResultado.Mensagens = new MensagemErro[] { new MensagemErro() { Mensagem = MensagemBusiness.RetornaMensagens("Viagem_ExcluirComentario_OK") } };
            return itemResultado;
        }
    }
}