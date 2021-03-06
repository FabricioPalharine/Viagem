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
    public partial class CidadeGrupoController : BaseApiController
    {
        [Authorize]
        [BindJson(typeof(CriterioBusca), "json")]
        public ResultadoConsultaTipo<CidadeGrupo> Get(CriterioBusca json)
        {
            ResultadoConsultaTipo<CidadeGrupo> resultado = new ResultadoConsultaTipo<CidadeGrupo>();
            ViagemBusiness biz = new ViagemBusiness();
           
            List<CidadeGrupo> _itens = biz.ListarCidadeGrupo().ToList();
resultado.TotalRegistros = _itens.Count();
            if (json.SortField != null && json.SortField.Any())
                _itens = _itens.AsQueryable().OrderByField<CidadeGrupo>(json.SortField, json.SortOrder).ToList();

            if (json.Index.HasValue && json.Count.HasValue)
                _itens = _itens.Skip(json.Index.Value).Take(json.Count.Value).ToList();
            resultado.Lista = _itens;

            return resultado;
        }
        [Authorize]
        public ManutencaoCidadeGrupo Get(int id)
        {
            ViagemBusiness biz = new ViagemBusiness();
            ManutencaoCidadeGrupo itemCidadeGrupo = biz.ListarManutencaoCidadeGrupo(token.IdentificadorViagem, id).FirstOrDefault();
          
            return itemCidadeGrupo;
        }
        [Authorize]
        public ResultadoOperacao Post([FromBody] ManutencaoCidadeGrupo itemCidadeGrupo)
        {
            ViagemBusiness biz = new ViagemBusiness();
            biz.SalvarCidadeGrupo(itemCidadeGrupo, token.IdentificadorViagem);
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            itemResultado.Sucesso = biz.IsValid();
            itemResultado.Mensagens = biz.RetornarMensagens.ToArray();
            if (itemResultado.Sucesso)
            {
                itemResultado.ItemRegistro = biz.SelecionarCidade( itemCidadeGrupo.IdentificadorCidade);
                itemResultado.IdentificadorRegistro = itemCidadeGrupo.IdentificadorCidade;
            }
            return itemResultado;
        }
        [Authorize]
        public ResultadoOperacao Delete(int id)
        {
            ViagemBusiness biz = new ViagemBusiness();
            var ListaExcluir = biz.ListarCidadeGrupo(token.IdentificadorViagem, id).ToList();
            biz.ExcluirCidadeGrupo_Lista(ListaExcluir);
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            itemResultado.Sucesso = biz.IsValid();
            itemResultado.Mensagens = biz.RetornarMensagens.ToArray();

            return itemResultado;
        }
    }
}