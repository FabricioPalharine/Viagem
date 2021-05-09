using CV.Mobile.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CV.Mobile.Services.Data
{
    public interface IDataService
    {
        void SincronizarParticipanteViagem(Viagem itemViagem);
        Task SalvarAmigos(List<Amigo> Amigos);
        Task<bool> ExisteEnvioPendente();
        void AjustarAmigo(ConsultaAmigo itemAmigo);
        Task<ResultadoOperacao> SalvarAgendamentoSugestao(AgendarSugestao itemAgenda);
        Task<ResultadoOperacao> SalvarCalendarioPrevisto(CalendarioPrevisto itemCalendarioPrevisto);
        Task<ResultadoOperacao> SalvarListaCompra(ListaCompra itemListaCompra);
        void AdicionarAmigoBase(ConsultaAmigo itemAmigo);
        Task<ResultadoOperacao> SalvarAporteDinheiro(AporteDinheiro itemAporteDinheiro);
        void SalvarGastoSincronizado(Gasto itemGasto);
        Task<ResultadoOperacao> SalvarItemCompra(ItemCompra itemItemCompra, int? _IdentificadorListaCompraInicial);
        Task<ResultadoOperacao> SalvarGasto(Gasto itemGasto);
        Task GravarDadosAporte(AporteDinheiro itemAporteDinheiro);
        Task<ResultadoOperacao> SalvarComentario(Comentario itemComentario);
        Task<AporteDinheiro> CarregarAporteDinheiro(int? identificador);
        Task<Gasto> CarregarGasto(int? identificadorGasto);
        Task ExcluirAporteDinheiro(AporteDinheiro item, bool AtualizadoBanco);
        Task AtualizarBancoRecepcaoAcao(string tipo, int identificador, UsuarioLogado itemUsuario);
        Task ExcluirGasto(Gasto itemGasto, bool AtualizadoBanco);
        Task AjustarDePara(ClasseSincronizacao itemEnvio, List<DeParaIdentificador> resultadoSincronizacao);
        Task SincronizarDadosServidorLocal(ControleSincronizacao itemCS, ClasseSincronizacao dadosSincronizar, UsuarioLogado itemUsuario, DateTime DataSincronizacao);
        Task<ResultadoOperacao> SalvarCotacaoMoeda(CotacaoMoeda itemCotacaoMoeda);
        Task<Atracao> CarregarAtracao(int? Identificador);
        Task ExcluirAtracao(int? Identificador, bool Sincronizado);
        Task<ResultadoOperacao> SalvarAtracao(Atracao itemAtracao);
        Task SalvarAtracaoReplicada(Atracao itemAtracao);
        Task<Refeicao> CarregarRefeicao(int? Identificador);
        Task ExcluirRefeicao(int? Identificador, bool Sincronizado);
        Task<ResultadoOperacao> SalvarRefeicao(Refeicao itemRefeicao);
        Task SalvarRefeicaoReplicada(Refeicao itemRefeicao);
        Task<Hotel> CarregarHotel(int? Identificador);
        Task ExcluirHotel(int? Identificador, bool Sincronizado);
        Task<ResultadoOperacao> SalvarHotel(Hotel itemHotel);
        Task SalvarHotelReplicada(Hotel itemHotel);
        Task<Loja> CarregarLoja(int? Identificador);
        Task ExcluirLoja(int? Identificador, bool Sincronizado);
        Task ExcluirGastoCompra(int? IdentificadorCompra, bool Sincronizado);
        Task<ResultadoOperacao> SalvarLoja(Loja itemLoja);
        Task SalvarLojaReplicada(Loja itemLoja);
        Task<GastoCompra> CarregarGastoCompra(int? Identificador);
        Task SalvarGastoCompraReplicada(GastoCompra itemGasto);
        Task<ResultadoOperacao> SalvarGastoCompra(GastoCompra itemGastoCompra);
        Task<Carro> CarregarCarro(int? Identificador);
        Task ExcluirCarro(int? Identificador, bool Sincronizado);
        Task<ResultadoOperacao> SalvarCarro(Carro itemCarro);
        Task SalvarCarroReplicada(Carro itemCarro);
        Task ExcluirCarroDeslocamento(int? Identificador, bool Sincronizado);
        Task SalvarCarroDeslocamentoReplicada(CarroDeslocamento itemCarro);
        Task<ResultadoOperacao> SalvarCarroDeslocamento(CarroDeslocamento itemCarro);
        Task<CarroDeslocamento> CarregarCarroDeslocamento(int? Identificador);
        Task<Reabastecimento> CarregarReabastecimento(int? Identificador);
        Task ExcluirReabastecimento(int? Identificador, bool Sincronizado);
        Task SalvarReabastecimentoReplicada(Reabastecimento itemCarro);
        Task<ResultadoOperacao> SalvarReabastecimento(Reabastecimento itemReabastecimento);
        Task<ViagemAerea> CarregarViagemAerea(int? Identificador);
        Task ExcluirViagemAerea(int? Identificador, bool Sincronizado);
        Task<ResultadoOperacao> SalvarViagemAerea(ViagemAerea itemViagemAerea);
        Task SalvarViagemAereaReplicada(ViagemAerea itemViagemAerea);
        Task<ClasseSincronizacao> CarregarDadosEnvioSincronizar();
    }
}
