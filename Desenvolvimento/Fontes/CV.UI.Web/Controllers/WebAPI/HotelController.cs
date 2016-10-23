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
    public partial class HotelController : BaseApiController
    {
        [Authorize]
        [BindJson(typeof(CriterioBusca), "json")]
        public ResultadoConsultaTipo<Hotel> Get(CriterioBusca json)
        {
            ResultadoConsultaTipo<Hotel> resultado = new ResultadoConsultaTipo<Hotel>();
            ViagemBusiness biz = new ViagemBusiness();
           
            List<Hotel> _itens = biz.ListarHotel().ToList();
resultado.TotalRegistros = _itens.Count();
            if (json.SortField != null && json.SortField.Any())
                _itens = _itens.AsQueryable().OrderByField<Hotel>(json.SortField, json.SortOrder).ToList();

            if (json.Index.HasValue && json.Count.HasValue)
                _itens = _itens.Skip(json.Index.Value).Take(json.Count.Value).ToList();
            resultado.Lista = _itens;

            return resultado;
        }
        [Authorize]
        public Hotel Get(int id)
        {
            ViagemBusiness biz = new ViagemBusiness();
            Hotel itemHotel = biz.SelecionarHotel(id);
          
            return itemHotel;
        }
        [Authorize]
        public ResultadoOperacao Post([FromBody] Hotel itemHotel)
        {
            ViagemBusiness biz = new ViagemBusiness();
                      biz.SalvarHotel(itemHotel);
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            itemResultado.Sucesso = biz.IsValid();
            itemResultado.Mensagens = biz.RetornarMensagens.ToArray();
            if (itemResultado.Sucesso)
                itemResultado.IdentificadorRegistro = itemHotel.Identificador;
            return itemResultado;
        }
        [Authorize]
        public ResultadoOperacao Delete(int id)
        {
            ViagemBusiness biz = new ViagemBusiness();
            Hotel itemHotel = biz.SelecionarHotel(id);
            biz.ExcluirHotel(itemHotel);
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            itemResultado.Sucesso = biz.IsValid();
            itemResultado.Mensagens = biz.RetornarMensagens.ToArray();

            return itemResultado;
        }
    }
}