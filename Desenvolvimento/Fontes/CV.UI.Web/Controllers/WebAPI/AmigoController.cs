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
    public partial class AmigoController : BaseApiController
    {
        [Authorize]
        [BindJson(typeof(CriterioBusca), "json")]
        public ResultadoConsultaTipo<Amigo> Get(CriterioBusca json)
        {
            ResultadoConsultaTipo<Amigo> resultado = new ResultadoConsultaTipo<Amigo>();
            ViagemBusiness biz = new ViagemBusiness();
           
            List<Amigo> _itens = biz.ListarAmigo().ToList();
resultado.TotalRegistros = _itens.Count();
            if (json.SortField != null && json.SortField.Any())
                _itens = _itens.AsQueryable().OrderByField<Amigo>(json.SortField, json.SortOrder).ToList();

            if (json.Index.HasValue && json.Count.HasValue)
                _itens = _itens.Skip(json.Index.Value).Take(json.Count.Value).ToList();
            resultado.Lista = _itens;

            return resultado;
        }
        [Authorize]
        public Amigo Get(int id)
        {
            ViagemBusiness biz = new ViagemBusiness();
            Amigo itemAmigo = biz.SelecionarAmigo(id);
          
            return itemAmigo;
        }
        [Authorize]
        public ResultadoOperacao Post([FromBody] Amigo itemAmigo)
        {
            ViagemBusiness biz = new ViagemBusiness();
                      biz.SalvarAmigo(itemAmigo);
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            itemResultado.Sucesso = biz.IsValid();
            itemResultado.Mensagens = biz.RetornarMensagens.ToArray();
            if (itemResultado.Sucesso)
                itemResultado.IdentificadorRegistro = itemAmigo.Identificador;
            return itemResultado;
        }
        [Authorize]
        public ResultadoOperacao Delete(int id)
        {
            ViagemBusiness biz = new ViagemBusiness();
            Amigo itemAmigo = biz.SelecionarAmigo(id);
            biz.ExcluirAmigo(itemAmigo);
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            itemResultado.Sucesso = biz.IsValid();
            itemResultado.Mensagens = biz.RetornarMensagens.ToArray();

            return itemResultado;
        }
    }
}