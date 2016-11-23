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
    public partial class ViagemAereaController : BaseApiController
    {
        [Authorize]
        [BindJson(typeof(CriterioBusca), "json")]
        public ResultadoConsultaTipo<ViagemAerea> Get(CriterioBusca json)
        {
            ResultadoConsultaTipo<ViagemAerea> resultado = new ResultadoConsultaTipo<ViagemAerea>();
            ViagemBusiness biz = new ViagemBusiness();

            List<ViagemAerea> _itens = biz.ListarViagemAerea(token.IdentificadorViagem.GetValueOrDefault(), json.DataInicioDe, json.DataInicioAte, json.Nome, json.TipoInteiro, json.Situacao.GetValueOrDefault(), json.IdentificadorCidade, json.IdentificadorCidade2, json.Identificador);

            resultado.Lista = _itens;

            return resultado;
        }
        [Authorize]
        public ViagemAerea Get(int id)
        {
            ViagemBusiness biz = new ViagemBusiness();
            ViagemAerea itemViagemAerea = biz.SelecionarViagemAerea_Completa(id);
            itemViagemAerea.Avaliacoes.ToList().ForEach(d => d.ItemViagemAerea = null);
            itemViagemAerea.Aeroportos.ToList().ForEach(d => { d.ItemViagemAerea = null; });
            itemViagemAerea.Gastos.ToList().ForEach(d => { d.ItemViagemAerea = null; d.ItemGasto.ViagenAereas = null; });
            return itemViagemAerea;
        }
        [Authorize]
        public ResultadoOperacao Post([FromBody] ViagemAerea itemViagemAerea)
        {
            ViagemBusiness biz = new ViagemBusiness();

            itemViagemAerea.IdentificadorViagem = token.IdentificadorViagem;
            itemViagemAerea.DataAtualizacao = DateTime.Now;
            foreach (var itemAeroporto in itemViagemAerea.Aeroportos)
            {
                itemAeroporto.IdentificadorCidade = biz.RetornarCidadeGeocoding(itemAeroporto.Latitude, itemAeroporto.Longitude);
                itemAeroporto.DataAtualizacao = DateTime.Now;
            }
            itemViagemAerea.Descricao = String.Concat(itemViagemAerea.CompanhiaAerea, " - ", itemViagemAerea.Aeroportos.Where(d => d.TipoPonto == (int)enumTipoParada.Origem).Select(d => d.Aeroporto).FirstOrDefault(), " - ", itemViagemAerea.Aeroportos.Where(d => d.TipoPonto == (int)enumTipoParada.Destino).Select(d => d.Aeroporto).FirstOrDefault());
            biz.SalvarViagemAerea(itemViagemAerea);
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            itemResultado.Sucesso = biz.IsValid();
            itemResultado.Mensagens = biz.RetornarMensagens.ToArray();
            if (itemResultado.Sucesso)
            {
                itemResultado.IdentificadorRegistro = itemViagemAerea.Identificador;
                itemViagemAerea.Avaliacoes = null;
                itemViagemAerea.Aeroportos = null;
                itemResultado.ItemRegistro = itemViagemAerea;
            }
            return itemResultado;
        }
        [Authorize]
        public ResultadoOperacao Delete(int id)
        {
            ViagemBusiness biz = new ViagemBusiness();
            ViagemAerea itemViagemAerea = biz.SelecionarViagemAerea_Completa(id);
            itemViagemAerea.DataExclusao = DateTime.Now;
            itemViagemAerea.Avaliacoes.ToList().ForEach(d => d.DataExclusao = DateTime.Now);
            itemViagemAerea.Gastos.ToList().ForEach(d => d.DataExclusao = DateTime.Now);
            itemViagemAerea.Aeroportos.ToList().ForEach(d => d.DataExclusao = DateTime.Now);
            biz.SalvarViagemAerea_Completa(itemViagemAerea);
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            itemResultado.Sucesso = biz.IsValid();
            itemResultado.Mensagens = biz.RetornarMensagens.ToArray();

            return itemResultado;
        }
    }
}