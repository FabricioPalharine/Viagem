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
    public partial class RefeicaoController : BaseApiController
    {
        [Authorize]
        [BindJson(typeof(CriterioBusca), "json")]
        public ResultadoConsultaTipo<Refeicao> Get(CriterioBusca json)
        {
            ResultadoConsultaTipo<Refeicao> resultado = new ResultadoConsultaTipo<Refeicao>();
            ViagemBusiness biz = new ViagemBusiness();
           
            List<Refeicao> _itens = biz.ListarRefeicao().ToList();
resultado.TotalRegistros = _itens.Count();
            if (json.SortField != null && json.SortField.Any())
                _itens = _itens.AsQueryable().OrderByField<Refeicao>(json.SortField, json.SortOrder).ToList();

            if (json.Index.HasValue && json.Count.HasValue)
                _itens = _itens.Skip(json.Index.Value).Take(json.Count.Value).ToList();
            resultado.Lista = _itens;

            return resultado;
        }
        [Authorize]
        public Refeicao Get(int id)
        {
            ViagemBusiness biz = new ViagemBusiness();
            Refeicao itemRefeicao = biz.SelecionarRefeicao(id);
          
            return itemRefeicao;
        }
        [Authorize]
        public ResultadoOperacao Post([FromBody] Refeicao itemRefeicao)
        {
            ViagemBusiness biz = new ViagemBusiness();
                      biz.SalvarRefeicao(itemRefeicao);
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            itemResultado.Sucesso = biz.IsValid();
            itemResultado.Mensagens = biz.RetornarMensagens.ToArray();
            if (itemResultado.Sucesso)
                itemResultado.IdentificadorRegistro = itemRefeicao.Identificador;
            return itemResultado;
        }
        [Authorize]
        public ResultadoOperacao Delete(int id)
        {
            ViagemBusiness biz = new ViagemBusiness();
            Refeicao itemRefeicao = biz.SelecionarRefeicao(id);
            biz.ExcluirRefeicao(itemRefeicao);
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            itemResultado.Sucesso = biz.IsValid();
            itemResultado.Mensagens = biz.RetornarMensagens.ToArray();

            return itemResultado;
        }
    }
}