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
    public partial class FotoController : BaseApiController
    {
        [Authorize]
        [BindJson(typeof(CriterioBusca), "json")]
        public ResultadoConsultaTipo<Foto> Get(CriterioBusca json)
        {
            ResultadoConsultaTipo<Foto> resultado = new ResultadoConsultaTipo<Foto>();
            ViagemBusiness biz = new ViagemBusiness();
           
            List<Foto> _itens = biz.ListarFoto().ToList();
resultado.TotalRegistros = _itens.Count();
            if (json.SortField != null && json.SortField.Any())
                _itens = _itens.AsQueryable().OrderByField<Foto>(json.SortField, json.SortOrder).ToList();

            if (json.Index.HasValue && json.Count.HasValue)
                _itens = _itens.Skip(json.Index.Value).Take(json.Count.Value).ToList();
            resultado.Lista = _itens;

            return resultado;
        }
        [Authorize]
        public Foto Get(int id)
        {
            ViagemBusiness biz = new ViagemBusiness();
            Foto itemFoto = biz.SelecionarFoto(id);
          
            return itemFoto;
        }
        [Authorize]
        public ResultadoOperacao Post([FromBody] Foto itemFoto)
        {
            ViagemBusiness biz = new ViagemBusiness();
                      biz.SalvarFoto(itemFoto);
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            itemResultado.Sucesso = biz.IsValid();
            itemResultado.Mensagens = biz.RetornarMensagens.ToArray();
            if (itemResultado.Sucesso)
                itemResultado.IdentificadorRegistro = itemFoto.Identificador;
            return itemResultado;
        }
        [Authorize]
        public ResultadoOperacao Delete(int id)
        {
            ViagemBusiness biz = new ViagemBusiness();
            Foto itemFoto = biz.SelecionarFoto(id);
            biz.ExcluirFoto(itemFoto);
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            itemResultado.Sucesso = biz.IsValid();
            itemResultado.Mensagens = biz.RetornarMensagens.ToArray();

            return itemResultado;
        }
    }
}