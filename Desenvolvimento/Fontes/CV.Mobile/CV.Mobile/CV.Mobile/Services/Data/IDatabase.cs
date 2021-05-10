﻿using CV.Mobile.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CV.Mobile.Services.Data
{
    public interface IDatabase
    {
        Task<int> SalvarViagemAsync(Viagem item);
        Task<Viagem> GetViagemAtualAsync();
        Task<ControleSincronizacao> GetControleSincronizacaoAsync();
        Task SalvarControleSincronizacao(ControleSincronizacao item);
        Task LimparBancoViagem();
        Task<Usuario> CarregarUsuario(int Id);
        Task<List<ParticipanteViagem>> ListarParticipanteViagemAsync();
        Task<int> SalvarParticipanteViagemAsync(ParticipanteViagem item);
        Task<int> ExcluirParticipanteViagemAsync(ParticipanteViagem item);
        Task SalvarPosicao(Posicao item);
        Task<int> SalvarUsuarioGastoAsync(UsuarioGasto item);
        Task<int> SalvarUsuarioAsync(Usuario item);
        Task LimparAmigos();
        Task SalvarAmigo(Amigo itemAmigo);
        Task LimparParticipantes();
        Task ExcluirAmigo(Amigo itemAmigo);
        Task ExcluirPosicao(Posicao itemAmigo);
        Task ExcluirPosicao(int UltimaPosicao);
        Task<Amigo> RetornarAmigoIdentificadorUsuario(int IdentificadorUsuario);
        Task IncluirListaAmigo(List<Amigo> Lista);
        Task SalvarCalendarioPrevisto(CalendarioPrevisto itemCalendarioPrevisto);
        Task SalvarSugestao(Sugestao itemSugestao);
        Task SalvarComentario(Comentario itemComentario);
        Task ExcluirComentario(Comentario itemComentario);
        Task ExcluirSugestao(Sugestao item);
        Task<Comentario> RetornarComentario(int? Identificador);
        Task<CotacaoMoeda> RetornarCotacaoMoeda(int? Identificador);
        Task<List<Comentario>> ListarComentario(CriterioBusca itemBusca);
        Task SalvarUploadFoto(UploadFoto itemUploadFoto);
        Task ExcluirUploadFoto(UploadFoto itemUploadFoto);
        Task<List<UploadFoto>> ListarUploadFoto_Video(bool Video);
        Task<List<UploadFoto>> ListarUploadFoto_IdentificadorRefeicao(int? IdentificadorRefeicao);
        Task<List<UploadFoto>> ListarUploadFoto_IdentificadorHotel(int? IdentificadorHotel);
        Task<List<UploadFoto>> ListarUploadFoto_IdentificadorAtracao(int? IdentificadorAtracao);
        Task<List<Sugestao>> ListarSugestao(CriterioBusca itemBusca);
        Task<List<Usuario>> ListarAmigos();
        Task<List<Usuario>> ListarParticipanteViagem();
        Task SalvarAporteDinheiro(AporteDinheiro itemAporteDinheiro);
        Task ExcluirAporteDinheiro(AporteDinheiro itemAporteDinheiro);
        Task<AporteDinheiro> RetornarAporteDinheiro(int? Identificador);
        Task<List<AporteDinheiro>> ListarAporteDinheiro(CriterioBusca itemBusca);
        Task<Gasto> RetornarGasto(int? Identificador);
        Task<List<AluguelGasto>> ListarAluguelGasto_IdentificadorGasto(int? IdentificadorGasto);
        Task<List<GastoAtracao>> ListarGastoAtracao_IdentificadorGasto(int? IdentificadorGasto);
        Task<List<GastoCompra>> ListarGastoCompra_IdentificadorGasto(int? IdentificadorGasto);
        Task<List<GastoDividido>> ListarGastoDividido_IdentificadorGasto(int? IdentificadorGasto);
        Task<List<GastoHotel>> ListarGastoHotel_IdentificadorGasto(int? IdentificadorGasto);
        Task<List<GastoRefeicao>> ListarGastoRefeicao_IdentificadorGasto(int? IdentificadorGasto);
        Task<List<GastoViagemAerea>> ListarGastoViagemAerea_IdentificadorGasto(int? IdentificadorGasto);
        Task<List<ReabastecimentoGasto>> ListarReabastecimentoGasto_IdentificadorGasto(int? IdentificadorGasto);
        Task SalvarGasto(Gasto Item);
        Task SalvarAluguelGasto(AluguelGasto Item);
        Task SalvarGastoCompra(GastoCompra Item);
        Task SalvarGastoDividido(GastoDividido Item);
        Task SalvarGastoHotel(GastoHotel Item);
        Task SalvarGastoRefeicao(GastoRefeicao Item);
        Task SalvarGastoViagemAerea(GastoViagemAerea Item);
        Task SalvarReabastecimentoGasto(ReabastecimentoGasto Item);
        Task SalvarGastoAtracao(GastoAtracao Item);
        Task SalvarCotacaoMoeda(CotacaoMoeda Item);
        Task<List<CotacaoMoeda>> ListarCotacaoMoeda(CriterioBusca itemBusca);
        Task<Sugestao> RetornarSugestao(int? Identificador);
        Task<List<ListaCompra>> ListarListaCompra_Requisicao(CriterioBusca itemBusca, int IdentificadorUsuario);
        Task<List<ListaCompra>> ListarListaCompra(CriterioBusca itemBusca, int IdentificadorUsuario);
        Task<ListaCompra> RetornarListaCompra(int? Identificador);
        Task SalvarListaCompra(ListaCompra itemListaCompra);
        Task ExcluirListaCompra(ListaCompra itemExcluir);
        Task ExcluirCotacaoMoeda(CotacaoMoeda item);
        Task ExcluirGasto(Gasto item);
        Task ExcluirAluguelGasto(AluguelGasto item);
        Task ExcluirGastoAtracao(GastoAtracao item);
        Task ExcluirGastoCompra(GastoCompra item);
        Task ExcluirGastoHotel(GastoHotel item);
        Task ExcluirGastoRefeicao(GastoRefeicao item);
        Task ExcluirGastoDividido(GastoDividido item);
        Task ExcluirGastoViagemAerea(GastoViagemAerea item);
        Task ExcluirReabastecimentoGasto(ReabastecimentoGasto item);
        Task<List<CalendarioPrevisto>> ListarCalendarioPrevisto(CriterioBusca itemBuscao);
        Task ExcluirCalendarioPrevisto(CalendarioPrevisto item);
        Task<CalendarioPrevisto> CarregarCalendarioPrevisto(int Id);
        Task<List<Atracao>> ListarAtracao(CriterioBusca itemBusca);
        Task<Atracao> RetornarAtracaoAberta();
        Task ExcluirAtracao(Atracao item);
        Task<Atracao> RetornarAtracao(int? Id);
        Task<GastoAtracao> RetornarGastoAtracao(int? Id);
        Task<ReabastecimentoGasto> RetornarReabastecimentoGasto(int? Id);
        Task SalvarAtracao(Atracao item);
        Task<List<AvaliacaoAtracao>> ListarAvaliacaoAtracao_IdentificadorAtracao(int? IdentificadorAtracao);
        Task<List<GastoAtracao>> ListarGastoAtracao_IdentificadorAtracao(int? IdentificadorAtracao);
        Task ExcluirAvaliacaoAtracao(AvaliacaoAtracao item);
        Task SalvarAvaliacaoAtracao(AvaliacaoAtracao item);
        Task<List<Gasto>> ListarGasto(CriterioBusca itemBusca);
        Task Excluir_Gasto_Filhos(int IdentificadorGasto);
        Task ExcluirGastoDividido_IdentificadorGasto(int IdentificadorGasto);
        Task<List<Refeicao>> ListarRefeicao(CriterioBusca itemBusca);
        Task ExcluirRefeicao(Refeicao item);
        Task<Refeicao> RetornarRefeicao(int? Id);
        Task<GastoRefeicao> RetornarGastoRefeicao(int? Id);
        Task<HotelEvento> RetornarHotelEvento(int? Id);
        Task SalvarRefeicao(Refeicao item);
        Task<List<RefeicaoPedido>> ListarRefeicaoPedido_IdentificadorRefeicao(int? IdentificadorRefeicao);
        Task<List<GastoRefeicao>> ListarGastoRefeicao_IdentificadorRefeicao(int? IdentificadorRefeicao);
        Task ExcluirRefeicaoPedido(RefeicaoPedido item);
        Task SalvarRefeicaoPedido(RefeicaoPedido item);
        Task<List<Hotel>> ListarHotel(CriterioBusca itemBusca);
        Task ExcluirHotel(Hotel item);
        Task<Hotel> RetornarHotel(int? Id);
        Task<GastoHotel> RetornarGastoHotel(int? Id);
        Task SalvarHotel(Hotel item);
        Task<List<HotelAvaliacao>> ListarHotelAvaliacao_IdentificadorHotel(int? IdentificadorHotel);
        Task<List<GastoHotel>> ListarGastoHotel_IdentificadorHotel(int? IdentificadorHotel);
        Task<List<HotelEvento>> ListarHotelEvento_IdentificadorHotel(int? IdentificadorHotel);
        Task ExcluirHotelAvaliacao(HotelAvaliacao item);
        Task ExcluirHotelEvento(HotelEvento item);
        Task SalvarHotelAvaliacao(HotelAvaliacao item);
        Task SalvarHotelEvento(HotelEvento item);
        Task<HotelEvento> RetornarUltimoHotelEvento_IdentificadorHotel_IdentificadorUsuario(int? IdentificadorHotel, int? IdentificadorUsuario);
        Task<AluguelGasto> RetornarAluguelGasto(int? Id);
        Task<GastoViagemAerea> RetornarGastoViagemAerea(int? Id);
        Task<List<Loja>> ListarLoja(CriterioBusca itemBusca);
        Task ExcluirLoja(Loja item);
        Task<Loja> RetornarLoja(int? Id);
        Task<GastoCompra> RetornarGastoCompra(int? Id);
        Task<AvaliacaoLoja> RetornarAvaliacaoLoja(int? Id);
        Task<AvaliacaoAtracao> RetornarAvaliacaoAtracao(int? Id);
        Task<HotelAvaliacao> RetornarHotelAvaliacao(int? Id);
        Task<RefeicaoPedido> RetornarRefeicaoPedido(int? Id);
        Task SalvarLoja(Loja item);
        Task<List<AvaliacaoLoja>> ListarAvaliacaoLoja_IdentificadorLoja(int? IdentificadorLoja);
        Task<List<GastoCompra>> ListarGastoCompra_IdentificadorLoja(int? IdentificadorLoja);
        Task<List<ItemCompra>> ListarItemCompra_IdentificadorGastoCompra(int? IdentificadorGastoCompra);
        Task ExcluirItemCompra_IdentificadorGastoCompra(int? identificador);
        Task ExcluirCarroDeslocamentoUsuario_IdentificadorCarroDeslocamento(int? IdentificadorCarroDeslocamento);
        Task ExcluirAvaliacaoLoja(AvaliacaoLoja item);
        Task SalvarAvaliacaoLoja(AvaliacaoLoja item);
        Task ExcluirItemCompra(ItemCompra item);
        Task SalvarItemCompra(ItemCompra item);
        Task<ItemCompra> RetornarItemCompra(int? Id);
        Task<List<ListaCompra>> ListarListaCompra(int IdentificadorUsuario);
        Task<List<Carro>> ListarCarro(CriterioBusca itemBusca);
        Task ExcluirCarro(Carro item);
        Task<Carro> RetornarCarro(int? Id);
        Task<AvaliacaoAluguel> RetornarAvaliacaoAluguel(int? Id);
        Task ExcluirAvaliacaoAluguel(AvaliacaoAluguel item);
        Task SalvarAvaliacaoAluguel(AvaliacaoAluguel item);
        Task SalvarCarro(Carro item);
        Task<List<AvaliacaoAluguel>> ListarAvaliacaoAluguel_IdentificadorCarro(int? IdentificadorCarro);
        Task<List<AluguelGasto>> ListarAluguelGasto_IdentificadorCarro(int? IdentificadorCarro);
        Task<CarroEvento> RetornarCarroEvento(int? Id);
        Task ExcluirCarroEvento(CarroEvento item);
        Task SalvarCarroEvento(CarroEvento item);
        Task<List<CarroDeslocamento>> ListarCarroDeslocamento_IdentificadorCarro(int? IdentificadorCarro);
        Task<CarroDeslocamento> RetornarCarroDeslocamento(int? Id);
        Task<Reabastecimento> RetornarCarroReabastecimento(int? Id);
        Task ExcluirCarroDeslocamento(CarroDeslocamento item);
        Task SalvarCarroDeslocamento(CarroDeslocamento item);
        Task<List<Reabastecimento>> ListarReabastecimento_IdentificadorCarro(int? IdentificadorCarro);
        Task<Reabastecimento> RetornarReabastecimento(int? Id);
        Task<List<ReabastecimentoGasto>> ListarReabastecimentoGasto_IdentificadorReabastecimento(int? IdentificadorReabastecimento);
        Task ExcluirReabastecimento(Reabastecimento item);
        Task SalvarReabastecimento(Reabastecimento item);
        Task<List<CarroDeslocamentoUsuario>> ListarCarroDeslocamentoUsuario_IdentificadorCarroDeslocamento(int? IdentificadorCarroDeslocamento);
        Task<CarroDeslocamentoUsuario> RetornarCarroDeslocamentoUsuario(int? Id);
        Task ExcluirCarroDeslocamentoUsuario(CarroDeslocamentoUsuario item);
        Task SalvarCarroDeslocamentoUsuario(CarroDeslocamentoUsuario item);
        Task<List<ViagemAerea>> ListarViagemAerea(CriterioBusca itemBusca);
        Task ExcluirViagemAerea(ViagemAerea item);
        Task<ViagemAerea> RetornarViagemAerea(int? Id);
        Task<AvaliacaoAerea> RetornarAvaliacaoAerea(int? Id);
        Task ExcluirAvaliacaoAerea(AvaliacaoAerea item);
        Task SalvarViagemAereaAeroporto(ViagemAereaAeroporto item);
        Task<ViagemAereaAeroporto> RetornarViagemAereaAeroporto(int? Id);
        Task ExcluirViagemAereaAeroporto(ViagemAereaAeroporto item);
        Task SalvarAvaliacaoAerea(AvaliacaoAerea item);
        Task SalvarViagemAerea(ViagemAerea item);
        Task<List<AvaliacaoAerea>> ListarAvaliacaoAerea_IdentificadorViagemAerea(int? IdentificadorViagemAerea);
        Task<List<ViagemAereaAeroporto>> ListarViagemAereaAeroporto_IdentificadorViagemAerea(int? IdentificadorViagemAerea);
        Task<List<GastoViagemAerea>> ListarGastoViagemAerea_IdentificadorViagemAerea(int? IdentificadorViagemAerea);
        Task ExcluirAvaliacaoAtracao_IdentificadorAtracao(int? identificador);
        Task IncluirAvaliacaoAtracao(List<AvaliacaoAtracao> Lista);
        Task ExcluirAvaliacaoLoja_IdentificadorLoja(int? identificador);
        Task IncluirAvaliacaoLoja(List<AvaliacaoLoja> Lista);
        Task ExcluirHotelAvaliacao_IdentificadorHotel(int? identificador);
        Task IncluirHotelAvaliacao(List<HotelAvaliacao> Lista);
        Task ExcluirAvaliacaoAluguel_IdentificadorCarro(int? identificador);
        Task IncluirAvaliacaoAluguel(List<AvaliacaoAluguel> Lista);
        Task ExcluirRefeicaoPedido_IdentificadorRefeicao(int? identificador);

        Task IncluirRefeicaoPedido(List<RefeicaoPedido> Lista);

        Task ExcluirAvaliacaoAerea_IdentificadorViagemAerea(int? identificador);
        Task IncluirAvaliacaoAerea(List<AvaliacaoAerea> Lista);
        Task ExcluirViagemAereaAeroporto_IdentificadorViagemAerea(int? identificador);
        Task IncluirViagemAereaAeroporto(List<ViagemAereaAeroporto> Lista);
        Task IncluirGastoDividido(List<GastoDividido> Lista);
        Task IncluirCarroDeslocamentoUsuario(List<CarroDeslocamentoUsuario> Lista);
        Task<List<AporteDinheiro>> ListarAporteDinheiro_Pendente();
        Task<List<CotacaoMoeda>> ListarCotacaoMoeda_Pendente();
        Task<List<Atracao>> ListarAtracao_Pendente();
        Task<List<Gasto>> ListarGasto_Pendente(List<int?> ListaIdentificadoresIgnorar);
        Task<List<CalendarioPrevisto>> ListarCalendarioPrevisto_Pendente();
        Task<List<Carro>> ListarCarro_Pendente();
        Task<List<CarroDeslocamento>> ListarCarroDeslocamento_Pendente();
        Task<List<Comentario>> ListarComentario_Pendente();
        Task<List<Reabastecimento>> ListarReabastecimento_Pendente();
        Task<List<GastoCompra>> ListarGastoCompra_Pendente();
        Task<List<AluguelGasto>> ListarAluguelGasto_Pendente();
        Task<List<GastoAtracao>> ListarGastoAtracao_Pendente();
        Task<List<GastoHotel>> ListarGastoHotel_Pendente();
        Task<List<GastoRefeicao>> ListarGastoRefeicao_Pendente();
        Task<List<GastoViagemAerea>> ListarGastoViagemAerea_Pendente();
        Task<List<ItemCompra>> ListarItemCompra_Pendente();
        Task<List<ListaCompra>> ListarListaCompra_Pendente();
        Task<List<Sugestao>> ListarSugestao_Pendente();
        Task<List<Posicao>> ListarPosicao_Pendente();
        Task<List<ViagemAerea>> ListarViagemAerea_Pendente();
        Task<List<Loja>> ListarLoja_Pendente();
        Task<List<Hotel>> ListarHotel_Pendente();
        Task<List<HotelEvento>> ListarHotelEvento_Pendente();
        Task<List<Refeicao>> ListarRefeicao_Pendente();
        Task<CalendarioPrevisto> ConsultarCalendarioAlerta();
        Task GravarUltimaPosicao(UlimaPosicao itemUltimaPosicao);
        Task<UlimaPosicao> RetornarUltimaPosicao();
        Task<List<ExtratoMoeda>> ConsultarExtratoMoeda(int? IdentificadorUsuario, int? IdentificadorViagem, int? Moeda, DateTime DataInicio);
        Task<List<Atracao>> ListarAtracaoAberta();
        Task<List<ViagemAerea>> ListarViagemAereaAberto();
        Task<Hotel> ListarHotelAtual();
    }
}