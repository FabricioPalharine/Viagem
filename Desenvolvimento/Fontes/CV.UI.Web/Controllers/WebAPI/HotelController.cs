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

            List<Hotel> _itens = biz.ListarHotel(token.IdentificadorViagem.GetValueOrDefault(), json.DataInicioDe, json.DataInicioAte, json.DataFimDe, json.DataFimAte,
                json.Nome, json.Situacao.GetValueOrDefault(0), json.IdentificadorCidade, json.Identificador).ToList();

            resultado.Lista = _itens;

            return resultado;
        }
        [Authorize]
        public Hotel Get(int id)
        {
            ViagemBusiness biz = new ViagemBusiness();
            Hotel itemHotel = biz.SelecionarHotel_Completo(id);
            itemHotel.Avaliacoes.ToList().ForEach(d => d.ItemHotel = null);
            itemHotel.Eventos.ToList().ForEach(d => d.ItemHotel = null);
            itemHotel.Fotos.ToList().ForEach(d => { d.ItemHotel = null; d.ItemFoto.Hoteis = null; });
            itemHotel.Gastos.ToList().ForEach(d => { d.ItemHotel = null; d.ItemGasto.Hoteis = null; });
            return itemHotel;
        }
        [Authorize]
        public ResultadoOperacao Post([FromBody] Hotel itemHotel)
        {
            ViagemBusiness biz = new ViagemBusiness();
            itemHotel.IdentificadorCidade = biz.RetornarCidadeGeocoding(itemHotel.Latitude, itemHotel.Longitude);
            itemHotel.IdentificadorViagem = token.IdentificadorViagem;
            itemHotel.DataAtualizacao = DateTime.Now.ToUniversalTime();
            biz.SalvarHotel(itemHotel);
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            itemResultado.Sucesso = biz.IsValid();
            itemResultado.Mensagens = biz.RetornarMensagens.ToArray();
            if (itemResultado.Sucesso)
            {
                itemResultado.IdentificadorRegistro = itemHotel.Identificador;
                itemHotel.Avaliacoes = null;
                itemHotel.Eventos = null;
                itemResultado.ItemRegistro = itemHotel;
            }
            return itemResultado;
        }
        [Authorize]
        public ResultadoOperacao Delete(int id)
        {
            ViagemBusiness biz = new ViagemBusiness();
            Hotel itemHotel = biz.SelecionarHotel_Completo(id);
            itemHotel.DataExclusao = DateTime.Now.ToUniversalTime();
            itemHotel.Avaliacoes.ToList().ForEach(d => d.DataExclusao = DateTime.Now.ToUniversalTime());
            itemHotel.Gastos.ToList().ForEach(d => d.DataExclusao = DateTime.Now.ToUniversalTime());
            itemHotel.Fotos.ToList().ForEach(d => d.DataExclusao = DateTime.Now.ToUniversalTime());
            itemHotel.Eventos.ToList().ForEach(d => d.DataExclusao = DateTime.Now.ToUniversalTime());

            biz.SalvarHotel_Completo(itemHotel);
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            itemResultado.Sucesso = biz.IsValid();
            itemResultado.Mensagens = biz.RetornarMensagens.ToArray();

            return itemResultado;
        }

        [Authorize]
        [ActionName("CarregarFoto")]
        [HttpGet]
        public List<Hotel> CarregarFoto()
        {
            ViagemBusiness biz = new ViagemBusiness();
            return biz.ListarHotel(d => d.IdentificadorViagem == token.IdentificadorViagem);
        }

        [Authorize]
        [ActionName("SalvarHotelEvento")]
        [HttpPost]
        public ResultadoOperacao SalvarHotelEvento(HotelEvento itemEvento)
        {
            ViagemBusiness biz = new ViagemBusiness();
            biz.SalvarHotelEvento(itemEvento);

            ResultadoOperacao itemResultado = new ResultadoOperacao();
            itemResultado.Sucesso = biz.IsValid();
            itemResultado.Mensagens = biz.RetornarMensagens.ToArray();
            if (itemResultado.Sucesso)
            {
                itemResultado.IdentificadorRegistro = itemEvento.Identificador;
            }
            return itemResultado;
        }

        [Authorize]
        [ActionName("SalvarHotelEventoVerificacao")]
        [HttpPost]
        public ResultadoOperacao SalvarHotelEventoVerificacao(HotelEvento itemEvento)
        {
            ViagemBusiness biz = new ViagemBusiness();
            var itemUltimoEvento = biz.ListarHotelEvento(d => d.IdentificadorHotel == itemEvento.IdentificadorHotel && d.IdentificadorUsuario == token.IdentificadorUsuario && !d.DataSaida.HasValue).OrderByDescending(d => d.DataEntrada).FirstOrDefault();
            ResultadoOperacao itemResultado = new ResultadoOperacao();

            if (itemUltimoEvento != null && itemEvento.DataSaida.HasValue)
            {
                itemUltimoEvento.DataSaida = itemEvento.DataSaida;
                biz.SalvarHotelEvento(itemUltimoEvento);
            }
            else if (itemUltimoEvento == null && !itemEvento.DataSaida.HasValue)
                biz.SalvarHotelEvento(itemEvento);
            itemResultado.Sucesso = biz.IsValid();
            itemResultado.Mensagens = biz.RetornarMensagens.ToArray();
            if (itemResultado.Sucesso)
            {
                itemResultado.IdentificadorRegistro = itemEvento.Identificador;
            }
            return itemResultado;
        }
    }
}