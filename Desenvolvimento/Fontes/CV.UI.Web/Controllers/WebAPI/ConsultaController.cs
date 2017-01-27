﻿using CV.Business;
using CV.Model;
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
    public class ConsultaController : BaseApiController
    {
        [Authorize]
        [BindJson(typeof(CriterioBusca), "json")]
        [ActionName("ConsultarExtratoMoeda")]
        [HttpGet]
        public List<ExtratoMoeda> ConsultarExtratoMoeda(CriterioBusca json)
        {
            ConsultaBusiness biz = new ConsultaBusiness();
            return biz.ConsultarExtratoMoeda(token.IdentificadorUsuario, token.IdentificadorViagem, json.Moeda, json.DataInicioDe.GetValueOrDefault());
        }

        [Authorize]
        [BindJson(typeof(CriterioBusca), "json")]
        [ActionName("ListarGastosAcerto")]
        [HttpGet]
        public List<AjusteGastoDividido> ListarGastosAcerto(CriterioBusca json)
        {
            ConsultaBusiness biz = new ConsultaBusiness();
            DateTime? DataFim = json.DataInicioAte;
            if (DataFim.HasValue)
                DataFim = DataFim.Value.AddDays(1);
            return biz.ListarGastosAcerto(token.IdentificadorViagem, token.IdentificadorUsuario,  json.DataInicioDe, DataFim);
        }

        [Authorize]
        [BindJson(typeof(CriterioBusca), "json")]
        [ActionName("ListarRelatorioGastos")]
        [HttpGet]
        public List<RelatorioGastos> ListarRelatorioGastos(CriterioBusca json)
        {
            DateTime? DataFim = json.DataInicioAte;
            if (DataFim.HasValue)
                DataFim = DataFim.Value.AddDays(1);
            ConsultaBusiness biz = new ConsultaBusiness();
            return biz.ListarGastosViagem(token.IdentificadorViagem, json.IdentificadorParticipante, json.DataInicioDe, DataFim, json.Tipo);
        }

        [Authorize]
        [BindJson(typeof(CriterioBusca), "json")]
        [ActionName("ConsultarTimeline")]
        [HttpGet]
        public List<Timeline> ConsultarTimeline(CriterioBusca json)
        {
            ConsultaBusiness biz = new ConsultaBusiness();
            return biz.CarregarTimeline(token.IdentificadorViagem, token.IdentificadorUsuario, json.IdentificadorParticipante, json.DataInicioDe,json.DataInicioAte, json.Count.GetValueOrDefault(20));
        }

        [Authorize]
        [BindJson(typeof(CriterioBusca), "json")]
        [ActionName("ListarLocaisVisitados")]
        [HttpGet]
        public List<LocaisVisitados> ListarLocaisVisitados(CriterioBusca json)
        {
            DateTime? DataFim = json.DataInicioAte;
            if (DataFim.HasValue)
                DataFim = DataFim.Value.AddDays(1);
            ConsultaBusiness biz = new ConsultaBusiness();
            return biz.CarregarLocaisVisitados(token.IdentificadorViagem, json.DataInicioDe, DataFim);
        }


        [Authorize]
        [BindJson(typeof(CriterioBusca), "json")]
        [ActionName("ConsultarDetalheAtracao")]
        [HttpGet]
        public LocaisVisitados ConsultarDetalheAtracao(CriterioBusca json)
        {
            DateTime? DataFim = json.DataInicioAte;
            if (DataFim.HasValue)
                DataFim = DataFim.Value.AddDays(1);
            ConsultaBusiness biz = new ConsultaBusiness();
            return biz.CarregarDetalhesAtracao(token.IdentificadorViagem, json.DataInicioDe, DataFim, json.Nome, json.Comentario);
        }

        [Authorize]
        [BindJson(typeof(CriterioBusca), "json")]
        [ActionName("ConsultarDetalheHotel")]
        [HttpGet]
        public LocaisVisitados ConsultarDetalheHotel(CriterioBusca json)
        {
            DateTime? DataFim = json.DataInicioAte;
            if (DataFim.HasValue)
                DataFim = DataFim.Value.AddDays(1);
            ConsultaBusiness biz = new ConsultaBusiness();
            return biz.CarregarDetalhesHotel(token.IdentificadorViagem, json.DataInicioDe, DataFim, json.Nome, json.Comentario);
        }

        [Authorize]
        [BindJson(typeof(CriterioBusca), "json")]
        [ActionName("ConsultarDetalheRestaurante")]
        [HttpGet]
        public LocaisVisitados ConsultarDetalheRestaurante(CriterioBusca json)
        {
            DateTime? DataFim = json.DataInicioAte;
            if (DataFim.HasValue)
                DataFim = DataFim.Value.AddDays(1);
            ConsultaBusiness biz = new ConsultaBusiness();
            return biz.CarregarDetalhesRefeicao(token.IdentificadorViagem, json.DataInicioDe, DataFim, json.Nome, json.Comentario);
        }

        [Authorize]
        [BindJson(typeof(CriterioBusca), "json")]
        [ActionName("ConsultarDetalheLoja")]
        [HttpGet]
        public LocaisVisitados ConsultarDetalheLoja(CriterioBusca json)
        {
            DateTime? DataFim = json.DataInicioAte;
            if (DataFim.HasValue)
                DataFim = DataFim.Value.AddDays(1);
            ConsultaBusiness biz = new ConsultaBusiness();
            return biz.CarregarDetalhesLoja(token.IdentificadorViagem, json.DataInicioDe, DataFim, json.Nome, json.Comentario);
        }
    }
}
