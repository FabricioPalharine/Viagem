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
    public partial class ViagemController : BaseApiController
    {
        [Authorize]
        [BindJson(typeof(CriterioBusca), "json")]
        public ResultadoConsultaTipo<Viagem> Get(CriterioBusca json)
        {
            ResultadoConsultaTipo<Viagem> resultado = new ResultadoConsultaTipo<Viagem>();
            ViagemBusiness biz = new ViagemBusiness();

            List<Viagem> _itens = biz.ListarViagem(json.IdentificadorParticipante, json.Nome, json.Aberto, json.DataInicioDe, json.DataInicioAte, json.DataFimDe, json.DataFimAte, token.IdentificadorUsuario).ToList();
            resultado.TotalRegistros = _itens.Count();
            if (json.SortField != null && json.SortField.Any())
                _itens = _itens.AsQueryable().OrderByField<Viagem>(json.SortField, json.SortOrder).ToList();

            if (json.Index.HasValue && json.Count.HasValue)
                _itens = _itens.Skip(json.Index.Value).Take(json.Count.Value).ToList();
            resultado.Lista = _itens;
            resultado.Lista.ForEach(d => d.Participantes.ToList().ForEach(f => f.ItemViagem = null));
            return resultado;
        }
        [Authorize]
        public Viagem Get(int id)
        {
            ViagemBusiness biz = new ViagemBusiness();
            Viagem itemViagem = biz.SelecionarViagem(id);
            itemViagem.UsuariosGastos.ToList().ForEach(d => d.ItemViagem = null);
            itemViagem.Participantes.ToList().ForEach(d => d.ItemViagem = null);

            return itemViagem;
        }
        [Authorize]
        public ResultadoOperacao Post([FromBody] Viagem itemViagem)
        {
            ViagemBusiness biz = new ViagemBusiness();
            itemViagem.DataAlteracao = DateTime.Now;
            biz.SalvarViagem_Completa_Album(itemViagem);
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            itemResultado.Sucesso = biz.IsValid();
            itemResultado.Mensagens = biz.RetornarMensagens.ToArray();
            if (itemResultado.Sucesso)
                itemResultado.IdentificadorRegistro = itemViagem.Identificador;
            return itemResultado;
        }

        [Authorize]
        public ResultadoOperacao Delete(int id)
        {
            ViagemBusiness biz = new ViagemBusiness();
            Viagem itemViagem = biz.SelecionarViagem(id);
            biz.ExcluirViagem(itemViagem);
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            itemResultado.Sucesso = biz.IsValid();
            itemResultado.Mensagens = biz.RetornarMensagens.ToArray();

            return itemResultado;
        }

        [Authorize]
        [ActionName("CarregarParticipantes")]
        [HttpGet]
        public List<Usuario> CarregarParticipantes()
        {
            ViagemBusiness biz = new ViagemBusiness();
            var lista = biz.CarregarParticipantesViagem(token.IdentificadorViagem);
            return lista.ToList();
        }
    }
}