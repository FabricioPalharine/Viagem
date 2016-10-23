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
    public partial class ReabastecimentoController : BaseApiController
    {
        [Authorize]
        [BindJson(typeof(CriterioBusca), "json")]
        public ResultadoConsultaTipo<Reabastecimento> Get(CriterioBusca json)
        {
            ResultadoConsultaTipo<Reabastecimento> resultado = new ResultadoConsultaTipo<Reabastecimento>();
            ViagemBusiness biz = new ViagemBusiness();
           
            List<Reabastecimento> _itens = biz.ListarReabastecimento().ToList();
resultado.TotalRegistros = _itens.Count();
            if (json.SortField != null && json.SortField.Any())
                _itens = _itens.AsQueryable().OrderByField<Reabastecimento>(json.SortField, json.SortOrder).ToList();

            if (json.Index.HasValue && json.Count.HasValue)
                _itens = _itens.Skip(json.Index.Value).Take(json.Count.Value).ToList();
            resultado.Lista = _itens;

            return resultado;
        }
        [Authorize]
        public Reabastecimento Get(int id)
        {
            ViagemBusiness biz = new ViagemBusiness();
            Reabastecimento itemReabastecimento = biz.SelecionarReabastecimento(id);
          
            return itemReabastecimento;
        }
        [Authorize]
        public ResultadoOperacao Post([FromBody] Reabastecimento itemReabastecimento)
        {
            ViagemBusiness biz = new ViagemBusiness();
                      biz.SalvarReabastecimento(itemReabastecimento);
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            itemResultado.Sucesso = biz.IsValid();
            itemResultado.Mensagens = biz.RetornarMensagens.ToArray();
            if (itemResultado.Sucesso)
                itemResultado.IdentificadorRegistro = itemReabastecimento.Identificador;
            return itemResultado;
        }
        [Authorize]
        public ResultadoOperacao Delete(int id)
        {
            ViagemBusiness biz = new ViagemBusiness();
            Reabastecimento itemReabastecimento = biz.SelecionarReabastecimento(id);
            biz.ExcluirReabastecimento(itemReabastecimento);
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            itemResultado.Sucesso = biz.IsValid();
            itemResultado.Mensagens = biz.RetornarMensagens.ToArray();

            return itemResultado;
        }
    }
}