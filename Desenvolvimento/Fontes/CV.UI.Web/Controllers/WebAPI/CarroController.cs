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
    public partial class CarroController : BaseApiController
    {
        [Authorize]
        [BindJson(typeof(CriterioBusca), "json")]
        public ResultadoConsultaTipo<Carro> Get(CriterioBusca json)
        {
            ResultadoConsultaTipo<Carro> resultado = new ResultadoConsultaTipo<Carro>();
            ViagemBusiness biz = new ViagemBusiness();
           
            List<Carro> _itens = biz.ListarCarro().ToList();
resultado.TotalRegistros = _itens.Count();
            if (json.SortField != null && json.SortField.Any())
                _itens = _itens.AsQueryable().OrderByField<Carro>(json.SortField, json.SortOrder).ToList();

            if (json.Index.HasValue && json.Count.HasValue)
                _itens = _itens.Skip(json.Index.Value).Take(json.Count.Value).ToList();
            resultado.Lista = _itens;

            return resultado;
        }
        [Authorize]
        public Carro Get(int id)
        {
            ViagemBusiness biz = new ViagemBusiness();
            Carro itemCarro = biz.SelecionarCarro(id);
          
            return itemCarro;
        }
        [Authorize]
        public ResultadoOperacao Post([FromBody] Carro itemCarro)
        {
            ViagemBusiness biz = new ViagemBusiness();
                      biz.SalvarCarro(itemCarro);
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            itemResultado.Sucesso = biz.IsValid();
            itemResultado.Mensagens = biz.RetornarMensagens.ToArray();
            if (itemResultado.Sucesso)
                itemResultado.IdentificadorRegistro = itemCarro.Identificador;
            return itemResultado;
        }
        [Authorize]
        public ResultadoOperacao Delete(int id)
        {
            ViagemBusiness biz = new ViagemBusiness();
            Carro itemCarro = biz.SelecionarCarro(id);
            biz.ExcluirCarro(itemCarro);
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            itemResultado.Sucesso = biz.IsValid();
            itemResultado.Mensagens = biz.RetornarMensagens.ToArray();

            return itemResultado;
        }
    }
}