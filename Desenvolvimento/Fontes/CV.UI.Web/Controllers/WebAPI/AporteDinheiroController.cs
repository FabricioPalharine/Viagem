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
    public partial class AporteDinheiroController : BaseApiController
    {
        [Authorize]
        [BindJson(typeof(CriterioBusca), "json")]
        public ResultadoConsultaTipo<AporteDinheiro> Get(CriterioBusca json)
        {
            ResultadoConsultaTipo<AporteDinheiro> resultado = new ResultadoConsultaTipo<AporteDinheiro>();
            ViagemBusiness biz = new ViagemBusiness();
           
            List<AporteDinheiro> _itens = biz.ListarAporteDinheiro(token.IdentificadorUsuario, token.IdentificadorViagem, json.Moeda, json.DataInicioDe, json.DataInicioAte).ToList();

            resultado.Lista = _itens;

            return resultado;
        }
        [Authorize]
        public AporteDinheiro Get(int id)
        {
            ViagemBusiness biz = new ViagemBusiness();
            AporteDinheiro itemAporteDinheiro = biz.SelecionarAporteDinheiro(id);
          
            return itemAporteDinheiro;
        }
        [Authorize]
        public ResultadoOperacao Post([FromBody] AporteDinheiro itemAporteDinheiro)
        {
            ViagemBusiness biz = new ViagemBusiness();
            itemAporteDinheiro.IdentificadorViagem = token.IdentificadorViagem;
            itemAporteDinheiro.IdentificadorUsuario = token.IdentificadorUsuario;
            itemAporteDinheiro.DataAtualizacao = DateTime.Now.ToUniversalTime();
            if (itemAporteDinheiro.ItemGasto != null)
            {
                itemAporteDinheiro.ItemGasto.DataAtualizacao = DateTime.Now.ToUniversalTime();
                itemAporteDinheiro.ItemGasto.IdentificadorUsuario = token.IdentificadorUsuario;
                itemAporteDinheiro.ItemGasto.IdentificadorViagem = token.IdentificadorViagem;
                itemAporteDinheiro.ItemGasto.Data = itemAporteDinheiro.DataAporte;
            }
            biz.SalvarAporteDinheiroCompleto(itemAporteDinheiro);
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            itemResultado.Sucesso = biz.IsValid();
            itemResultado.Mensagens = biz.RetornarMensagens.ToArray();
            if (itemResultado.Sucesso)
            {
                itemResultado.IdentificadorRegistro = itemAporteDinheiro.Identificador;
                itemResultado.ItemRegistro = biz.SelecionarAporteDinheiro(itemAporteDinheiro.Identificador);
            }
            return itemResultado;
        }
        [Authorize]
        public ResultadoOperacao Delete(int id)
        {
            ViagemBusiness biz = new ViagemBusiness();
            AporteDinheiro itemAporteDinheiro = biz.SelecionarAporteDinheiro(id);
            itemAporteDinheiro.DataExclusao = null;
            if (itemAporteDinheiro.ItemGasto != null)
                itemAporteDinheiro.ItemGasto.DataExclusao = DateTime.Now.ToUniversalTime();
            biz.ExcluirAporteDinheiro(itemAporteDinheiro);
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            itemResultado.Sucesso = biz.IsValid();
            itemResultado.Mensagens = biz.RetornarMensagens.ToArray();

            return itemResultado;
        }
    }
}