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
    public partial class GastoController : BaseApiController
    {
        [Authorize]
        [BindJson(typeof(CriterioBusca), "json")]
        public ResultadoConsultaTipo<Gasto> Get(CriterioBusca json)
        {
            ResultadoConsultaTipo<Gasto> resultado = new ResultadoConsultaTipo<Gasto>();
            ViagemBusiness biz = new ViagemBusiness();
           
            List<Gasto> _itens = biz.ListarGasto().ToList();
resultado.TotalRegistros = _itens.Count();
            if (json.SortField != null && json.SortField.Any())
                _itens = _itens.AsQueryable().OrderByField<Gasto>(json.SortField, json.SortOrder).ToList();

            if (json.Index.HasValue && json.Count.HasValue)
                _itens = _itens.Skip(json.Index.Value).Take(json.Count.Value).ToList();
            resultado.Lista = _itens;

            return resultado;
        }
        [Authorize]
        public Gasto Get(int id)
        {
            ViagemBusiness biz = new ViagemBusiness();
            Gasto itemGasto = biz.SelecionarGasto(id);
          
            return itemGasto;
        }
        [Authorize]
        public ResultadoOperacao Post([FromBody] Gasto itemGasto)
        {
            ViagemBusiness biz = new ViagemBusiness();
                      biz.SalvarGasto(itemGasto);
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            itemResultado.Sucesso = biz.IsValid();
            itemResultado.Mensagens = biz.RetornarMensagens.ToArray();
            if (itemResultado.Sucesso)
                itemResultado.IdentificadorRegistro = itemGasto.Identificador;
            return itemResultado;
        }
        [Authorize]
        public ResultadoOperacao Delete(int id)
        {
            ViagemBusiness biz = new ViagemBusiness();
            Gasto itemGasto = biz.SelecionarGasto(id);
            biz.ExcluirGasto(itemGasto);
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            itemResultado.Sucesso = biz.IsValid();
            itemResultado.Mensagens = biz.RetornarMensagens.ToArray();

            return itemResultado;
        }
    }
}