using CV.Business;
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
    public class SincronizacaoController : BaseApiController
    {
        [Authorize]
        [BindJson(typeof(CriterioBusca), "json")]

        [ActionName("RetornarAtualizacoes")]
        [HttpGet]
        public ClasseSincronizacao RetornarAtualizacoes(CriterioBusca json)
        {
            ClasseSincronizacao itemSincronizar = new ClasseSincronizacao();
            ViagemBusiness biz = new ViagemBusiness();
            Viagem itemViagem = biz.SelecionarViagem(token.IdentificadorViagem);
            itemViagem.UsuariosGastos.ToList().ForEach(d => d.ItemViagem = null);
            itemViagem.Participantes.ToList().ForEach(d => d.ItemViagem = null);

            itemSincronizar.ItemViagem = itemViagem;
            itemSincronizar.CidadesAtracao = biz.CarregarCidadeAtracao(token.IdentificadorViagem);
            itemSincronizar.CidadesComentario = biz.CarregarCidadeComentario(token.IdentificadorViagem);
            itemSincronizar.CidadesHotel = biz.CarregarCidadeHotel(token.IdentificadorViagem);
            itemSincronizar.CidadesLoja = biz.CarregarCidadeLoja(token.IdentificadorViagem);
            itemSincronizar.CidadesRefeicao = biz.CarregarCidadeRefeicao(token.IdentificadorViagem);
            itemSincronizar.CidadesSugestao = biz.CarregarCidadeSugestao(token.IdentificadorViagem);
            itemSincronizar.CidadesViagemAerea = biz.CarregarCidadeViagemAerea(token.IdentificadorViagem);

            itemSincronizar.CotacoesMoeda = biz.ListarCotacaoMoeda(d => d.IdentificadorViagem == token.IdentificadorViagem && (d.DataAtualizacao >= json.DataInicioDe || d.DataExclusao >= json.DataInicioDe)).ToList();
            itemSincronizar.Comentarios = biz.ListarComentario(d => d.IdentificadorViagem == token.IdentificadorViagem && d.IdentificadorUsuario == token.IdentificadorUsuario && (d.DataAtualizacao >= json.DataInicioDe || d.DataExclusao >= json.DataInicioDe)).ToList();
            itemSincronizar.AportesDinheiro = biz.ListarAporteDinheiro(d => d.IdentificadorUsuario == token.IdentificadorUsuario && d.IdentificadorViagem == token.IdentificadorViagem && d.IdentificadorUsuario == token.IdentificadorUsuario && (d.DataAtualizacao >= json.DataInicioDe || d.DataExclusao >= json.DataInicioDe)).ToList();
            itemSincronizar.Gastos = biz.ListarGasto(d => d.IdentificadorViagem == token.IdentificadorViagem && (d.DataAtualizacao >= json.DataInicioDe || d.DataExclusao >= json.DataInicioDe)).ToList();
            itemSincronizar.CalendariosPrevistos = biz.ListarCalendarioPrevisto(d => d.IdentificadorViagem == token.IdentificadorViagem && (d.DataAtualizacao >= json.DataInicioDe || d.DataExclusao >= json.DataInicioDe)).ToList();
            itemSincronizar.Sugestoes = biz.ListarSugestao(d => d.IdentificadorViagem == token.IdentificadorViagem && (d.DataAtualizacao >= json.DataInicioDe || d.DataExclusao >= json.DataInicioDe)).ToList();
            itemSincronizar.ListaCompra = biz.ListarListaCompra(token.IdentificadorUsuario, d => d.IdentificadorViagem == token.IdentificadorViagem && (d.DataAtualizacao >= json.DataInicioDe || d.DataExclusao >= json.DataInicioDe)).ToList();

            itemSincronizar.Atracoes = biz.ListarAtracao_Completo(d => d.IdentificadorViagem == token.IdentificadorViagem && (d.DataAtualizacao >= json.DataInicioDe || d.DataExclusao >= json.DataInicioDe)).ToList();
            foreach (var item in itemSincronizar.Atracoes.SelectMany(d => d.Avaliacoes))
                item.ItemAtracao = null;
            itemSincronizar.Hoteis = biz.ListarHotel_Completo(d => d.IdentificadorViagem == token.IdentificadorViagem && (d.DataAtualizacao >= json.DataInicioDe || d.DataExclusao >= json.DataInicioDe)).ToList();
            foreach (var item in itemSincronizar.Hoteis.SelectMany(d => d.Avaliacoes))
                item.ItemHotel = null;
            itemSincronizar.Lojas = biz.ListarLoja(d => d.IdentificadorViagem == token.IdentificadorViagem && (d.DataAtualizacao >= json.DataInicioDe || d.DataExclusao >= json.DataInicioDe)).ToList();
            foreach (var item in itemSincronizar.Lojas.SelectMany(d => d.Avaliacoes))
                item.ItemLoja = null;

            itemSincronizar.Refeicoes = biz.ListarRefeicao_Completo(d => d.IdentificadorViagem == token.IdentificadorViagem && (d.DataAtualizacao >= json.DataInicioDe || d.DataExclusao >= json.DataInicioDe)).ToList();
            foreach (var item in itemSincronizar.Refeicoes.SelectMany(d => d.Pedidos))
                item.ItemRefeicao = null;

            itemSincronizar.Carros = biz.ListarCarro(d => d.IdentificadorViagem == token.IdentificadorViagem && (d.DataAtualizacao >= json.DataInicioDe || d.DataExclusao >= json.DataInicioDe)).ToList();
            foreach (var item in itemSincronizar.Carros.SelectMany(d => d.Avaliacoes))
                item.ItemCarro = null;

            itemSincronizar.Deslocamentos = biz.ListarViagemAerea(d => d.IdentificadorViagem == token.IdentificadorViagem && (d.DataAtualizacao >= json.DataInicioDe || d.DataExclusao >= json.DataInicioDe)).ToList();
            foreach (var item in itemSincronizar.Deslocamentos.SelectMany(d => d.Avaliacoes))
                item.ItemViagemAerea = null;
            foreach (var item in itemSincronizar.Deslocamentos.SelectMany(d => d.Aeroportos))
                item.ItemViagemAerea = null;

            itemSincronizar.Compras = biz.ListarGastoCompra(token.IdentificadorViagem, d => d.ItemGasto.IdentificadorUsuario == token.IdentificadorUsuario && (d.DataAtualizacao >= json.DataInicioDe || d.DataExclusao >= json.DataInicioDe)).ToList();
            itemSincronizar.Reabastecimento = biz.ListarReabastecimento(token.IdentificadorViagem, d => (d.DataAtualizacao >= json.DataInicioDe || d.DataExclusao >= json.DataInicioDe)).ToList();
            foreach (var item in itemSincronizar.Reabastecimento.SelectMany(d => d.Gastos))
            {
                item.ItemReabastecimento = null;
                item.ItemGasto.Reabastecimentos = null;
            }

            itemSincronizar.EventosHotel = biz.ListarHotelEvento(token.IdentificadorViagem, d => (d.DataAtualizacao >= json.DataInicioDe || d.DataExclusao >= json.DataInicioDe)).ToList();
            itemSincronizar.CarroDeslocamentos =biz.ListarCarroDeslocamento(token.IdentificadorViagem, d => (d.DataAtualizacao >= json.DataInicioDe || d.DataExclusao >= json.DataInicioDe)).ToList();
            foreach (var item in itemSincronizar.CarroDeslocamentos.SelectMany(d => d.Usuarios))
                item.ItemCarroDeslocamento = null;

            itemSincronizar.ItensComprados = biz.ListarItemCompra(token.IdentificadorViagem, d => (d.DataAtualizacao >= json.DataInicioDe || d.DataExclusao >= json.DataInicioDe)).ToList();

            itemSincronizar.GastosCarro = biz.ListarAluguelGasto(token.IdentificadorViagem, d => (d.DataAtualizacao >= json.DataInicioDe || d.DataExclusao >= json.DataInicioDe)).ToList();
            itemSincronizar.GastosAtracao = biz.ListarGastoAtracao(token.IdentificadorViagem, d => (d.DataAtualizacao >= json.DataInicioDe || d.DataExclusao >= json.DataInicioDe)).ToList();
            itemSincronizar.GastosHotel = biz.ListarGastoHotel(token.IdentificadorViagem, d => (d.DataAtualizacao >= json.DataInicioDe || d.DataExclusao >= json.DataInicioDe)).ToList();
            itemSincronizar.GastosRefeicao = biz.ListarGastoRefeicao(token.IdentificadorViagem, d => (d.DataAtualizacao >= json.DataInicioDe || d.DataExclusao >= json.DataInicioDe)).ToList();
            itemSincronizar.GastosDeslocamento = biz.ListarGastoViagemAerea(token.IdentificadorViagem, d => (d.DataAtualizacao >= json.DataInicioDe || d.DataExclusao >= json.DataInicioDe)).ToList();

            itemSincronizar.GastosCarro = itemSincronizar.GastosCarro.Where(d => !itemSincronizar.Gastos.SelectMany(f => f.Alugueis).Where(f => f.Identificador == d.Identificador).Any()).ToList();
            itemSincronizar.GastosAtracao = itemSincronizar.GastosAtracao.Where(d => !itemSincronizar.Gastos.SelectMany(f => f.Atracoes).Where(f => f.Identificador == d.Identificador).Any()).ToList();
            itemSincronizar.GastosHotel = itemSincronizar.GastosHotel.Where(d => !itemSincronizar.Gastos.SelectMany(f => f.Hoteis).Where(f => f.Identificador == d.Identificador).Any()).ToList();
            itemSincronizar.GastosRefeicao = itemSincronizar.GastosRefeicao.Where(d => !itemSincronizar.Gastos.SelectMany(f => f.Refeicoes).Where(f => f.Identificador == d.Identificador).Any()).ToList();
            itemSincronizar.GastosDeslocamento = itemSincronizar.GastosDeslocamento.Where(d => !itemSincronizar.Gastos.SelectMany(f => f.ViagenAereas).Where(f => f.Identificador == d.Identificador).Any()).ToList();

            itemSincronizar.Amigos = biz.ListarAmigo(d => d.IdentificadorUsuario == token.IdentificadorUsuario && d.IdentificadorAmigo.HasValue);
            return itemSincronizar;
        }

        [Authorize]
        [ActionName("SincronizarDados")]
        [HttpPost]
        public List<DeParaIdentificador> SincronizarDados(ClasseSincronizacao itemDados)
        {
            ViagemBusiness biz = new ViagemBusiness();
            List<DeParaIdentificador> Lista = biz.SincronizarDados(itemDados, token.IdentificadorUsuario, token.IdentificadorViagem);
            return Lista;
        }
    }
}
