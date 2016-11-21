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
    public partial class CidadeController : BaseApiController
    {
        [Authorize]
        [BindJson(typeof(CriterioBusca), "json")]
        public ResultadoConsultaTipo<Cidade> Get(CriterioBusca json)
        {
            ResultadoConsultaTipo<Cidade> resultado = new ResultadoConsultaTipo<Cidade>();
            ViagemBusiness biz = new ViagemBusiness();
           
            List<Cidade> _itens = biz.ListarCidade().ToList();
resultado.TotalRegistros = _itens.Count();
            if (json.SortField != null && json.SortField.Any())
                _itens = _itens.AsQueryable().OrderByField<Cidade>(json.SortField, json.SortOrder).ToList();

            if (json.Index.HasValue && json.Count.HasValue)
                _itens = _itens.Skip(json.Index.Value).Take(json.Count.Value).ToList();
            resultado.Lista = _itens;

            return resultado;
        }
        [Authorize]
        public Cidade Get(int id)
        {
            ViagemBusiness biz = new ViagemBusiness();
            Cidade itemCidade = biz.SelecionarCidade(id);
          
            return itemCidade;
        }
        [Authorize]
        public ResultadoOperacao Post([FromBody] Cidade itemCidade)
        {
            ViagemBusiness biz = new ViagemBusiness();
                      biz.SalvarCidade(itemCidade);
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            itemResultado.Sucesso = biz.IsValid();
            itemResultado.Mensagens = biz.RetornarMensagens.ToArray();
            if (itemResultado.Sucesso)
                itemResultado.IdentificadorRegistro = itemCidade.Identificador;
            return itemResultado;
        }
        [Authorize]
        public ResultadoOperacao Delete(int id)
        {
            ViagemBusiness biz = new ViagemBusiness();
            Cidade itemCidade = biz.SelecionarCidade(id);
            biz.ExcluirCidade(itemCidade);
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            itemResultado.Sucesso = biz.IsValid();
            itemResultado.Mensagens = biz.RetornarMensagens.ToArray();

            return itemResultado;
        }

        [Authorize]
        [ActionName("CarregarFoto")]
        [HttpGet]
        public List<Cidade> CarregarFoto()
        {
            ViagemBusiness biz = new ViagemBusiness();
            var lista = biz.CarregarCidadeViagemFoto(token.IdentificadorViagem);
            return lista;
        }

        [Authorize]
        [ActionName("CarregarAtracao")]
        [HttpGet]
        public List<Cidade> CarregarAtracao()
        {
            ViagemBusiness biz = new ViagemBusiness();
            var lista = biz.CarregarCidadeAtracao(token.IdentificadorViagem);
            return lista;
        }

        [Authorize]
        [ActionName("CarregarRefeicao")]
        [HttpGet]
        public List<Cidade> CarregarRefeicao()
        {
            ViagemBusiness biz = new ViagemBusiness();
            var lista = biz.CarregarCidadeRefeicao(token.IdentificadorViagem);
            return lista;
        }

        [Authorize]
        [ActionName("CarregarHotel")]
        [HttpGet]
        public List<Cidade> CarregarHotel()
        {
            ViagemBusiness biz = new ViagemBusiness();
            var lista = biz.CarregarCidadeHotel(token.IdentificadorViagem);
            return lista;
        }



        [Authorize]
        [ActionName("CarregarViagemAerea")]
        [HttpGet]
        public List<Cidade> CarregarViagemAerea()
        {
            ViagemBusiness biz = new ViagemBusiness();
            var lista = biz.CarregarCidadeViagemAerea(token.IdentificadorViagem);
            return lista;
        }

        [Authorize]
        [ActionName("CarregarLoja")]
        [HttpGet]
        public List<Cidade> CarregarLoja()
        {
            ViagemBusiness biz = new ViagemBusiness();
            var lista = biz.CarregarCidadeLoja(token.IdentificadorViagem);
            return lista;
        }
    }
}