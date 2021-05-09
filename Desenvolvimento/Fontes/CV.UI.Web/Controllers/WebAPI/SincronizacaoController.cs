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
     
            itemSincronizar.CotacoesMoeda = biz.ListarCotacaoMoeda(d => d.IdentificadorViagem == token.IdentificadorViagem && (d.DataAtualizacao >= json.DataInicioDe || d.DataExclusao >= json.DataInicioDe)).ToList();
            itemSincronizar.Comentarios = biz.ListarComentario(d => d.IdentificadorViagem == token.IdentificadorViagem && d.IdentificadorUsuario == token.IdentificadorUsuario && (d.DataAtualizacao >= json.DataInicioDe || d.DataExclusao >= json.DataInicioDe)).ToList();
            itemSincronizar.AportesDinheiro = biz.ListarAporteDinheiro(d => d.IdentificadorUsuario == token.IdentificadorUsuario && d.IdentificadorViagem == token.IdentificadorViagem && d.IdentificadorUsuario == token.IdentificadorUsuario && (d.DataAtualizacao >= json.DataInicioDe || d.DataExclusao >= json.DataInicioDe)).ToList();
            itemSincronizar.Gastos = biz.ListarGasto(d => d.IdentificadorViagem == token.IdentificadorViagem && d.IdentificadorUsuario == token.IdentificadorUsuario && (d.DataAtualizacao >= json.DataInicioDe || d.DataExclusao >= json.DataInicioDe)).ToList();

            foreach (var item in itemSincronizar.Gastos.SelectMany(d => d.Atracoes))
                item.ItemGasto = null;
            foreach (var item in itemSincronizar.Gastos.SelectMany(d => d.Hoteis))
                item.ItemGasto = null;
            foreach (var item in itemSincronizar.Gastos.SelectMany(d => d.Refeicoes))
                item.ItemGasto = null;
            foreach (var item in itemSincronizar.Gastos.SelectMany(d => d.ViagenAereas))
                item.ItemGasto = null;

            itemSincronizar.Atracoes = biz.ListarAtracao_Completo(d => d.Avaliacoes.Where(f=>f.IdentificadorUsuario == token.IdentificadorUsuario).Any() && d.IdentificadorViagem == token.IdentificadorViagem).ToList();
            foreach (var item in itemSincronizar.Atracoes.SelectMany(d => d.Avaliacoes))
                item.ItemAtracao = null;
            foreach (var item in itemSincronizar.Atracoes)
            {
                item.ItemAtracaoPai = null;
                item.Atracoes = null;
            }
            itemSincronizar.Hoteis = biz.ListarHotel_Completo(d => d.Avaliacoes.Where(f => f.IdentificadorUsuario == token.IdentificadorUsuario).Any() && d.IdentificadorViagem == token.IdentificadorViagem && (d.DataAtualizacao >= json.DataInicioDe || d.DataExclusao >= json.DataInicioDe)).ToList();
            foreach (var item in itemSincronizar.Hoteis.SelectMany(d => d.Avaliacoes))
                item.ItemHotel = null;

            itemSincronizar.Refeicoes = biz.ListarRefeicao_Completo(d => d.Pedidos.Where(f => f.IdentificadorUsuario == token.IdentificadorUsuario).Any() && d.IdentificadorViagem == token.IdentificadorViagem && (d.DataAtualizacao >= json.DataInicioDe || d.DataExclusao >= json.DataInicioDe)).ToList();
            foreach (var item in itemSincronizar.Refeicoes.SelectMany(d => d.Pedidos))
                item.ItemRefeicao = null;

            itemSincronizar.Deslocamentos = biz.ListarViagemAerea(d => d.Avaliacoes.Where(f => f.IdentificadorUsuario == token.IdentificadorUsuario).Any() && d.IdentificadorViagem == token.IdentificadorViagem && (d.DataAtualizacao >= json.DataInicioDe || d.DataExclusao >= json.DataInicioDe)).ToList();
            foreach (var item in itemSincronizar.Deslocamentos.SelectMany(d => d.Avaliacoes))
                item.ItemViagemAerea = null;
            foreach (var item in itemSincronizar.Deslocamentos.SelectMany(d => d.Aeroportos))
                item.ItemViagemAerea = null;

        
            itemSincronizar.EventosHotel = biz.ListarHotelEvento(token.IdentificadorViagem, d => d.ItemHotel.Avaliacoes.Where(f=>f.IdentificadorUsuario == token.IdentificadorUsuario).Any() &&  (d.DataAtualizacao >= json.DataInicioDe || d.DataExclusao >= json.DataInicioDe)).ToList();

            itemSincronizar.GastosAtracao = biz.ListarGastoAtracao(token.IdentificadorViagem, d => d.ItemAtracao.Avaliacoes.Where(f => f.IdentificadorUsuario == token.IdentificadorUsuario).Any() && (d.DataAtualizacao >= json.DataInicioDe || d.DataExclusao >= json.DataInicioDe)).ToList();
            itemSincronizar.GastosHotel = biz.ListarGastoHotel(token.IdentificadorViagem, d => d.ItemHotel.Avaliacoes.Where(f => f.IdentificadorUsuario == token.IdentificadorUsuario).Any() && (d.DataAtualizacao >= json.DataInicioDe || d.DataExclusao >= json.DataInicioDe)).ToList();
            itemSincronizar.GastosRefeicao = biz.ListarGastoRefeicao(token.IdentificadorViagem, d => d.ItemRefeicao.Pedidos.Where(f => f.IdentificadorUsuario == token.IdentificadorUsuario).Any() && (d.DataAtualizacao >= json.DataInicioDe || d.DataExclusao >= json.DataInicioDe)).ToList();
            itemSincronizar.GastosDeslocamento = biz.ListarGastoViagemAerea(token.IdentificadorViagem, d => d.ItemViagemAerea.Avaliacoes.Where(f => f.IdentificadorUsuario == token.IdentificadorUsuario).Any() && (d.DataAtualizacao >= json.DataInicioDe || d.DataExclusao >= json.DataInicioDe)).ToList();

            //itemSincronizar.GastosAtracao = itemSincronizar.GastosAtracao.Where(d => !itemSincronizar.Gastos.SelectMany(f => f.Atracoes).Where(f => f.Identificador == d.Identificador).Any()).ToList();
           // itemSincronizar.GastosHotel = itemSincronizar.GastosHotel.Where(d => !itemSincronizar.Gastos.SelectMany(f => f.Hoteis).Where(f => f.Identificador == d.Identificador).Any()).ToList();
           // itemSincronizar.GastosRefeicao = itemSincronizar.GastosRefeicao.Where(d => !itemSincronizar.Gastos.SelectMany(f => f.Refeicoes).Where(f => f.Identificador == d.Identificador).Any()).ToList();
           // itemSincronizar.GastosDeslocamento = itemSincronizar.GastosDeslocamento.Where(d => !itemSincronizar.Gastos.SelectMany(f => f.ViagenAereas).Where(f => f.Identificador == d.Identificador).Any()).ToList();

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
