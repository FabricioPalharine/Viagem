using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CV.Model;
using CV.Model.Dominio;
using System.Data.Entity;

namespace CV.Data
{
    public partial class ConsultaRepository : RepositoryBase
    {
        public List<ExtratoMoeda> ConsultarExtratoMoeda(int? IdentificadorUsuario, int? IdentificadorViagem, int? Moeda, DateTime DataInicio)
        {
            var queryInicial = this.Context.AporteDinheiros.Where(d => d.IdentificadorUsuario == IdentificadorUsuario).Where(d => !d.DataExclusao.HasValue).Where(d => d.IdentificadorViagem == IdentificadorViagem)
                .Where(d => d.Moeda == Moeda).Where(d => d.DataAporte < DataInicio).Select(d => new { Data = d.DataAporte.Value, Valor = d.Valor.Value });
            queryInicial = queryInicial.Union(this.Context.Gastos.Where(d => d.IdentificadorUsuario == IdentificadorUsuario).Where(d => !d.DataExclusao.HasValue).Where(d => d.IdentificadorViagem == IdentificadorViagem)
                .Where(d => d.Moeda == Moeda).Where(d => d.Data < DataInicio).Where(d => d.Especie.Value).Select(d => new { Data = d.Data.Value, Valor = d.Valor.Value * -1 }));

            var queryAtual = this.Context.AporteDinheiros.Where(d => d.IdentificadorUsuario == IdentificadorUsuario).Where(d => !d.DataExclusao.HasValue).Where(d => d.IdentificadorViagem == IdentificadorViagem)
                .Where(d => d.Moeda == Moeda).Where(d => d.DataAporte >= DataInicio).Select(d => new { Data = d.DataAporte.Value, Valor = d.Valor.Value, Descricao = "Aporte Moeda" });
            queryAtual = queryAtual.Union(this.Context.Gastos.Where(d => d.IdentificadorUsuario == IdentificadorUsuario).Where(d => !d.DataExclusao.HasValue).Where(d => d.IdentificadorViagem == IdentificadorViagem)
                .Where(d => d.Moeda == Moeda).Where(d => d.Data >= DataInicio).Where(d => d.Especie.Value).Select(d => new { Data = d.Data.Value, Valor = d.Valor.Value * -1, Descricao = d.Descricao }));

            decimal ValorInicial = 0;
            if (queryInicial.Count() > 0)
                ValorInicial = queryInicial.Sum(d => d.Valor);
            List<ExtratoMoeda> Lista = new List<ExtratoMoeda>();
            Lista.Add(new ExtratoMoeda() { Data = DataInicio, TipoLinha = "S", Descricao = "Saldo Inicial", Valor = ValorInicial });
            Lista.AddRange(queryAtual.Select(d => new ExtratoMoeda() { Data = d.Data, Descricao = d.Descricao, TipoLinha = "V", Valor = d.Valor }));
            decimal ValorAnterior = ValorInicial;
            DateTime UltimaData = DataInicio;
            var ListaGrupo = Lista.Where(d => d.TipoLinha == "V").GroupBy(d => d.Data.Date);
            foreach (var itemGrupo in ListaGrupo)
            {
                ValorAnterior = ValorAnterior + itemGrupo.Sum(e => e.Valor);
                UltimaData = itemGrupo.Key;
                Lista.Add(new ExtratoMoeda() { Data = itemGrupo.Key.AddDays(1).AddMilliseconds(-1), Descricao = (UltimaData == DateTime.Today ? "Saldo Atual" : "Saldo Dia"), TipoLinha = "S", Valor = ValorAnterior });
            }
            if (UltimaData != DateTime.Today)
                Lista.Add(new ExtratoMoeda() { Data = DateTime.Today.AddDays(1).AddMilliseconds(-1), Descricao = "Saldo Atual", TipoLinha = "S", Valor = ValorAnterior });

            return Lista.OrderByDescending(d => d.Data).ToList();

        }


        public List<AjusteGastoDividido> ListarGastosAcerto(int? IdentificadorViagem, int? IdentificadorUsuario, DateTime? DataInicio, DateTime? DataFim)
        {
            var QueryGastosOrigem = this.Context.Gastos.Where(d => !d.DataExclusao.HasValue).Where(d => d.IdentificadorViagem == IdentificadorViagem).Where(d => d.IdentificadorUsuario == IdentificadorUsuario)
                .Where(d => d.Dividido.Value);
            if (DataInicio.HasValue)
                QueryGastosOrigem = QueryGastosOrigem.Where(d => d.Data >= DataInicio);
            if (DataFim.HasValue)
                QueryGastosOrigem = QueryGastosOrigem.Where(d => d.Data < DataFim);
            var queryGastoUsuario = QueryGastosOrigem.SelectMany(d => d.Usuarios.Select(e => new
            {
                IdentificadorUsuario = IdentificadorUsuario,
                IdentificadorUsuarioComparar = e.IdentificadorUsuario,
                Moeda = d.Moeda,
                ValorPago = d.Valor.Value / (d.Usuarios.Count() + 1),
                ValorRecebido = 0m
            }));

            var QueryCompraPaga = this.Context.ItemCompras.Where(d => !d.DataExclusao.HasValue).Where(d => d.Reembolsavel.Value).Where(d => d.ItemGastoCompra.ItemGasto.IdentificadorViagem == IdentificadorViagem)
                .Where(d => d.ItemGastoCompra.ItemGasto.IdentificadorUsuario == IdentificadorUsuario).Where(d => d.ItemGastoCompra.ItemGasto.ItemViagem.Participantes.Where(e => e.IdentificadorUsuario == d.IdentificadorUsuario).Any());
            if (DataInicio.HasValue)
                QueryCompraPaga = QueryCompraPaga.Where(d => d.ItemGastoCompra.ItemGasto.Data >= DataInicio);
            if (DataFim.HasValue)
                QueryCompraPaga = QueryCompraPaga.Where(d => d.ItemGastoCompra.ItemGasto.Data < DataFim);
            queryGastoUsuario = queryGastoUsuario.Concat(QueryCompraPaga.Select(d => new
            {
                IdentificadorUsuario = IdentificadorUsuario,
                IdentificadorUsuarioComparar = d.IdentificadorUsuario,
                Moeda = d.ItemGastoCompra.ItemGasto.Moeda,
                ValorPago = d.Valor.Value,
                ValorRecebido = 0m
            }));

            var QueryCompraRecebida = this.Context.ItemCompras.Where(d => !d.DataExclusao.HasValue).Where(d => d.Reembolsavel.Value).Where(d => d.ItemGastoCompra.ItemGasto.IdentificadorViagem == IdentificadorViagem)
                .Where(d => d.IdentificadorUsuario == IdentificadorUsuario);
            if (DataInicio.HasValue)
                QueryCompraRecebida = QueryCompraRecebida.Where(d => d.ItemGastoCompra.ItemGasto.Data >= DataInicio);
            if (DataFim.HasValue)
                QueryCompraRecebida = QueryCompraRecebida.Where(d => d.ItemGastoCompra.ItemGasto.Data < DataFim);
            queryGastoUsuario = queryGastoUsuario.Concat(QueryCompraRecebida.Select(d => new
            {
                IdentificadorUsuario = d.IdentificadorUsuario,
                IdentificadorUsuarioComparar = IdentificadorUsuario,
                Moeda = d.ItemGastoCompra.ItemGasto.Moeda,
                ValorPago = 0m,
                ValorRecebido = d.Valor.Value
            }));

            var QueryGastosDestinos = this.Context.Gastos.Where(d => !d.DataExclusao.HasValue).Where(d => d.IdentificadorViagem == IdentificadorViagem)
    .Where(d => d.Dividido.Value);
            if (DataInicio.HasValue)
                QueryGastosOrigem = QueryGastosOrigem.Where(d => d.Data >= DataInicio);
            if (DataFim.HasValue)
                QueryGastosOrigem = QueryGastosOrigem.Where(d => d.Data < DataFim);


            queryGastoUsuario = queryGastoUsuario.Concat(QueryGastosOrigem.SelectMany(d => d.Usuarios.Where(e => e.IdentificadorUsuario == IdentificadorUsuario).Select(e => new
            {
                IdentificadorUsuario = d.IdentificadorUsuario,
                IdentificadorUsuarioComparar = IdentificadorUsuario,
                Moeda = d.Moeda,
                ValorPago = 0m,
                ValorRecebido = d.Valor.Value / (d.Usuarios.Count() + 1),
            })));

            var queryFinal = queryGastoUsuario.GroupBy(d => new { d.IdentificadorUsuarioComparar, d.Moeda }).
                Select(d => new { IdentificadorUsuario = d.Key.IdentificadorUsuarioComparar, Moeda = d.Key.Moeda, ValorPago = d.Sum(e => e.ValorPago), ValorRecebido = d.Sum(e => e.ValorRecebido) })
                .Join(this.Context.Usuarios, d => d.IdentificadorUsuario, d => d.Identificador,
                (g, u) => new AjusteGastoDividido() { IdentificadorUsuario = u.Identificador, Moeda = g.Moeda, NomeUsuario = u.Nome, ValorPago = g.ValorPago, ValorRecebido = g.ValorRecebido });



            return queryFinal.ToList();
        }

        public List<RelatorioGastos> ListarGastosViagem(int? IdentificadorViagem, int? IdentificadorUsuario, DateTime? DataInicio, DateTime? DataFim, string Tipo)
        {
            var queryGasto = this.Context.Gastos.Where(d => !d.DataExclusao.HasValue).Where(d => d.IdentificadorViagem == IdentificadorViagem).Where(d => !d.ApenasBaixa.Value);
            if (DataInicio.HasValue)
                queryGasto = queryGasto.Where(d => d.Data >= DataInicio);
            if (DataFim.HasValue)
                queryGasto = queryGasto.Where(d => d.Data < DataFim);
            if (IdentificadorUsuario.HasValue)
                queryGasto = queryGasto.Where(d => d.IdentificadorUsuario == IdentificadorUsuario || d.Usuarios.Where(e => e.IdentificadorUsuario == IdentificadorUsuario).Any());
            if (!string.IsNullOrEmpty(Tipo))
            {
                if (Tipo == "A")
                    queryGasto = queryGasto.Where(d => d.Atracoes.Where(e => !e.DataExclusao.HasValue).Any());
                if (Tipo == "C")
                    queryGasto = queryGasto.Where(d => d.Alugueis.Where(e => !e.DataExclusao.HasValue).Any());
                if (Tipo == "R")
                    queryGasto = queryGasto.Where(d => d.Refeicoes.Where(e => !e.DataExclusao.HasValue).Any());
                if (Tipo == "H")
                    queryGasto = queryGasto.Where(d => d.Hoteis.Where(e => !e.DataExclusao.HasValue).Any());
                if (Tipo == "VA")
                    queryGasto = queryGasto.Where(d => d.ViagenAereas.Where(e => !e.DataExclusao.HasValue).Any());
                if (Tipo == "L")
                    queryGasto = queryGasto.Where(d => d.Compras.Where(e => !e.DataExclusao.HasValue).Any());
                if (Tipo == "CR")
                    queryGasto = queryGasto.Where(d => d.Reabastecimentos.Where(e => !e.DataExclusao.HasValue).Any());
            }
            var queryAjuste = queryGasto.Select(d => new
            {
                IdentificadorUsuario = d.IdentificadorUsuario,
                Moeda = d.Moeda,
                Valor = d.Valor,
                Descricao = d.Descricao,
                Especie = d.Especie,
                DataPagamento = d.DataPagamento,
                IdentificadorUsuarioGasto = d.Identificador,
                Data = d.Data
            });
            if (IdentificadorUsuario.HasValue)
            {
                queryAjuste = queryGasto.Select(
                    d => new
                    {
                        IdentificadorUsuario = IdentificadorUsuario,
                        Moeda = d.Moeda,
                        Valor = d.Valor / (d.Usuarios.Count() + 1),
                        Descricao = d.Descricao,
                        Especie = d.Especie,
                        DataPagamento = d.DataPagamento,
                        IdentificadorUsuarioGasto = d.Identificador,
                        Data = d.Data
                    }
                    );
            }

            var queryFinal = queryAjuste.Join(this.Context.Usuarios, d => d.IdentificadorUsuario, d => d.Identificador,
                (g, u) => new { g.IdentificadorUsuario, g.Moeda, g.Valor, g.DataPagamento, g.Descricao, g.Especie, NomeUsuario = u.Nome, g.IdentificadorUsuarioGasto, g.Data })
                .GroupJoin(this.Context.CotacaoMoedas.Where(d => d.IdentificadorViagem == IdentificadorViagem).Where(d => !d.DataExclusao.HasValue), d => d.Moeda, d => d.Moeda,
                (g, cm) => new
                {
                    g.IdentificadorUsuarioGasto,
                    g.DataPagamento,
                    g.Descricao,
                    g.Especie,
                    g.IdentificadorUsuario,
                    g.Moeda,
                    g.NomeUsuario,
                    g.Valor,
                    g.Data,
                    ValorReal = g.Moeda != (int)enumMoeda.BRL && !g.Especie.Value ? g.Valor * (cm.Where(d => d.DataCotacao <= g.DataPagamento).Any() ? cm.Where(d => d.DataCotacao <= g.DataPagamento).Select(e => e.ValorCotacao).FirstOrDefault() : 0) : g.Valor
                }).GroupJoin(this.Context.AporteDinheiros.Where(d => d.IdentificadorViagem == IdentificadorViagem).Where(d => !d.DataExclusao.HasValue),
                d => new { IdentificadorUsuario = d.IdentificadorUsuarioGasto, d.Moeda },
                           d => new { d.IdentificadorUsuario, d.Moeda },
                (g, cm) => new RelatorioGastos
                {
                    Descricao = g.Descricao,
                    IdentificadorUsuario = g.IdentificadorUsuario,
                    Data = g.Data.Value,
                    Moeda = g.Moeda,
                    NomeUsuario = g.NomeUsuario,
                    Valor = g.Valor.Value,
                    ValorReal = g.Moeda != (int)enumMoeda.BRL && g.Especie.Value ? g.Valor.Value * (cm.Where(d => d.DataAporte <= g.Data).Any() ? cm.Where(d => d.DataAporte <= g.DataPagamento).Select(e => e.Cotacao.Value).FirstOrDefault() : 0) : g.ValorReal.Value
                });

            return queryFinal.ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="IdentificadorViagem"></param>
        /// <param name="IdentificadorUsuarioConsulta"></param>
        /// <param name="IdentificadorUsuarioEvento"></param>
        /// <param name="DataMaxima"></param>
        /// <param name="DataMinima"></param>
        /// <param name="NumeroRegistros"></param>
        /// <returns></returns>
        public List<Timeline> CarregarTimeline(int? IdentificadorViagem, int? IdentificadorUsuarioConsulta, int? IdentificadorUsuarioEvento, DateTime? DataMaxima, DateTime? DataMinima, int NumeroRegistros)
        {
            var queryUsuarios = this.Context.ParticipanteViagemes.Where(d => d.IdentificadorUsuario == IdentificadorUsuarioConsulta || this.Context.Amigos.Where(e => e.IdentificadorAmigo == IdentificadorUsuarioConsulta && e.IdentificadorUsuario == d.IdentificadorUsuario).Any());
            if (IdentificadorUsuarioEvento.HasValue)
                queryUsuarios = queryUsuarios.Where(d => d.IdentificadorUsuario == IdentificadorUsuarioEvento);
            var listaInteiros = queryUsuarios.Select(d => d.IdentificadorUsuario);

            var queryConsulta = this.Context.Atracoes.Where(d => !d.DataExclusao.HasValue).Where(d => d.IdentificadorViagem == IdentificadorViagem).Where(d => d.Chegada.HasValue)
                .Where(d => d.Avaliacoes.Where(e => listaInteiros.Contains(e.IdentificadorUsuario)).Any());
            var teste = queryConsulta.ToList();
            var queryResultado = queryConsulta
                .Where(d => !d.Partida.HasValue || d.Partida > DbFunctions.AddMinutes(d.Chegada, 2))
                .SelectMany(d => d.Avaliacoes.Select(e => new { Data = d.Chegada, Tipo = "AtracaoChegada", Latitude = d.Latitude, Longitude = d.Longitude, Texto = d.Nome, Url = "", Comentario = "", IdentificadorUsuario = e.IdentificadorUsuario, Nota = new Nullable<int>(), NomeUsuario = e.ItemUsuario.Nome, ComentarioUsuario = "", Pedido = "" }));//, Usuarios = d.Avaliacoes.Where(e => listaInteiros.Contains(e.IdentificadorUsuario)).Select(e => new UsuarioConsulta() { Comentario = "", Nome = e.ItemUsuario.Nome, Identificador = e.IdentificadorUsuario, Nota = null, Pedido = null }) });
            queryResultado = queryResultado.Union(
                queryConsulta.Where(d => d.Partida.HasValue)
                .SelectMany(d => d.Avaliacoes.Select(e => new { Data = d.Partida, Tipo = d.Partida > DbFunctions.AddMinutes(d.Chegada, 2) ? "AtracaoPartida" : "AtracaoVisita", Latitude = d.Latitude, Longitude = d.Longitude, Texto = d.Nome, Url = "", Comentario = "", IdentificadorUsuario = e.IdentificadorUsuario, Nota = e.Nota, NomeUsuario = e.ItemUsuario.Nome, ComentarioUsuario = e.Comentario, Pedido = "" })));


            var queryRefeicao = this.Context.Refeicoes.Where(d => !d.DataExclusao.HasValue).Where(d => d.IdentificadorViagem == IdentificadorViagem)
                .Where(d => d.Pedidos.Where(e => listaInteiros.Contains(e.IdentificadorUsuario)).Any());

            queryResultado = queryResultado.Union(
    queryRefeicao
    .SelectMany(d => d.Pedidos.Select(e => new { Data = d.Data, Tipo = "Refeicao", Latitude = d.Latitude, Longitude = d.Longitude, Texto = d.Nome, Url = "", Comentario = "", IdentificadorUsuario = e.IdentificadorUsuario, Nota = e.Nota, NomeUsuario = e.ItemUsuario.Nome, ComentarioUsuario = e.Comentario, Pedido = e.Pedido })));


            var queryHotel = this.Context.Hoteis.Where(d => !d.DataExclusao.HasValue).Where(d => d.IdentificadorViagem == IdentificadorViagem).Where(d => d.DataEntrada.HasValue)
    .Where(d => d.Avaliacoes.Where(e => listaInteiros.Contains(e.IdentificadorUsuario)).Any());

            queryResultado = queryResultado.Union(
    queryHotel
    .SelectMany(d => d.Avaliacoes.Select(e => new { Data = d.DataEntrada, Tipo = "HotelCheckIn", Latitude = d.Latitude, Longitude = d.Longitude, Texto = d.Nome, Url = "", Comentario = "", IdentificadorUsuario = e.IdentificadorUsuario, Nota = new Nullable<int>(), NomeUsuario = e.ItemUsuario.Nome, ComentarioUsuario = "", Pedido = "" })));


            queryResultado = queryResultado.Union(
    queryHotel.Where(d => d.DataSaidia.HasValue)
    .SelectMany(d => d.Avaliacoes.Select(e => new { Data = d.DataSaidia, Tipo = "HotelChekckOut", Latitude = d.Latitude, Longitude = d.Longitude, Texto = d.Nome, Url = "", Comentario = "", IdentificadorUsuario = e.IdentificadorUsuario, Nota = e.Nota, NomeUsuario = e.ItemUsuario.Nome, ComentarioUsuario = e.Comentario, Pedido = "" })));

            var queryHotelEvento = this.Context.HotelEventos.Where(d => !d.DataExclusao.HasValue).Where(d => d.ItemHotel.IdentificadorViagem == IdentificadorViagem).Where(d => d.DataEntrada.HasValue).Where(d => listaInteiros.Contains(d.IdentificadorUsuario));

            queryResultado = queryResultado.Union(
queryHotelEvento
.Select(d => new { Data = d.DataEntrada, Tipo = "HotelEntrada", Latitude = d.ItemHotel.Latitude, Longitude = d.ItemHotel.Longitude, Texto = d.ItemHotel.Nome, Url = "", Comentario = "", IdentificadorUsuario = d.IdentificadorUsuario, Nota = new Nullable<int>(), NomeUsuario = d.ItemUsuario.Nome, ComentarioUsuario = "", Pedido = "" }));
            queryResultado = queryResultado.Union(
queryHotelEvento.Where(d => d.DataSaida.HasValue)
.Select(d => new { Data = d.DataSaida, Tipo = "HotelSaida", Latitude = d.ItemHotel.Latitude, Longitude = d.ItemHotel.Longitude, Texto = d.ItemHotel.Nome, Url = "", Comentario = "", IdentificadorUsuario = d.IdentificadorUsuario, Nota = new Nullable<int>(), NomeUsuario = d.ItemUsuario.Nome, ComentarioUsuario = "", Pedido = "" }));


            var queryCarro = this.Context.Carros.Where(d => d.Alugado.Value).Where(d => !d.DataExclusao.HasValue).Where(d => d.IdentificadorViagem == IdentificadorViagem).Where(d => d.ItemCarroEventoRetirada.Data.HasValue)
    .Where(d => d.Avaliacoes.Where(e => listaInteiros.Contains(e.IdentificadorUsuario)).Any());


            queryResultado = queryResultado.Union(
queryCarro
.SelectMany(d => d.Avaliacoes.Select(e => new { Data = d.ItemCarroEventoRetirada.Data, Tipo = "CarroRetirada", Latitude = d.ItemCarroEventoRetirada.Latitude, Longitude = d.ItemCarroEventoRetirada.Longitude, Texto = d.Descricao, Url = "", Comentario = d.Locadora, IdentificadorUsuario = e.IdentificadorUsuario, Nota = new Nullable<int>(), NomeUsuario = e.ItemUsuario.Nome, ComentarioUsuario = "", Pedido = "" })));


            queryResultado = queryResultado.Union(
    queryCarro.Where(d => d.ItemCarroEventoDevolucao.Data.HasValue)
    .SelectMany(d => d.Avaliacoes.Select(e => new { Data = d.ItemCarroEventoDevolucao.Data, Tipo = "CarroDevolucao", Latitude = d.ItemCarroEventoDevolucao.Latitude, Longitude = d.ItemCarroEventoDevolucao.Longitude, Texto = d.Descricao, Url = "", Comentario = d.Locadora, IdentificadorUsuario = e.IdentificadorUsuario, Nota = e.Nota, NomeUsuario = e.ItemUsuario.Nome, ComentarioUsuario = e.Comentario, Pedido = "" })));

            var queryCarroDeslocamento = this.Context.CarroDeslocamentos.Where(d => !d.DataExclusao.HasValue).Where(d => d.ItemCarro.IdentificadorViagem == IdentificadorViagem).Where(d => d.ItemCarroEventoPartida.Data.HasValue)
    .Where(d => d.Usuarios.Where(e => listaInteiros.Contains(e.IdentificadorUsuario)).Any());

            queryResultado = queryResultado.Union(
         queryCarroDeslocamento
         .SelectMany(d => d.Usuarios.Select(e => new { Data = d.ItemCarroEventoPartida.Data, Tipo = "CarroPartida", Latitude = d.ItemCarroEventoPartida.Latitude, Longitude = d.ItemCarroEventoPartida.Longitude, Texto = d.ItemCarro.Descricao, Url = "", Comentario = d.Observacao, IdentificadorUsuario = e.IdentificadorUsuario, Nota = new Nullable<int>(), NomeUsuario = e.ItemUsuario.Nome, ComentarioUsuario = "", Pedido = "" })));

            queryResultado = queryResultado.Union(
         queryCarroDeslocamento.Where(d => d.ItemCarroEventoChegada.Data.HasValue)
         .SelectMany(d => d.Usuarios.Select(e => new { Data = d.ItemCarroEventoChegada.Data, Tipo = "CarroChegada", Latitude = d.ItemCarroEventoChegada.Latitude, Longitude = d.ItemCarroEventoChegada.Longitude, Texto = d.ItemCarro.Descricao, Url = "", Comentario = d.Observacao, IdentificadorUsuario = e.IdentificadorUsuario, Nota = new Nullable<int>(), NomeUsuario = e.ItemUsuario.Nome, ComentarioUsuario = "", Pedido = "" })));

            var queryCarroReabastecimento = this.Context.Reabastecimentos.Where(d => !d.DataExclusao.HasValue).Where(d => d.ItemCarro.IdentificadorViagem == IdentificadorViagem).Where(d => d.Data.HasValue)
    .Where(d => d.ItemCarro.Avaliacoes.Where(e => listaInteiros.Contains(e.IdentificadorUsuario)).Any());
            queryResultado = queryResultado.Union(
                queryCarroReabastecimento
                .Select(d => new { Data = d.Data, Tipo = "Reabastecimento", Latitude = d.Latitude, Longitude = d.Longitude, Texto = d.ItemCarro.Descricao, Url = "", Comentario = "", IdentificadorUsuario = d.ItemCarro.ItemViagem.ItemUsuario.Identificador, Nota = new Nullable<int>(), NomeUsuario = d.ItemCarro.ItemViagem.ItemUsuario.Nome, ComentarioUsuario = "", Pedido = "" }));


            var queryComentario = this.Context.Comentarios.Where(d => !d.DataExclusao.HasValue).Where(d => d.IdentificadorViagem == IdentificadorViagem).Where(d => listaInteiros.Contains(d.IdentificadorUsuario));

            queryResultado = queryResultado.Union(
queryComentario
.Select(d => new { Data = d.Data, Tipo = "Comentario", Latitude = d.Latitude, Longitude = d.Longitude, Texto = "", Url = "", Comentario = d.Texto, IdentificadorUsuario = d.IdentificadorUsuario, Nota = new Nullable<int>(), NomeUsuario = d.ItemUsuario.Nome, ComentarioUsuario = "", Pedido = "" }));


            var queryCompra = this.Context.GastoCompras.Where(d => !d.DataExclusao.HasValue).Where(d => d.ItemGasto.IdentificadorViagem == IdentificadorViagem).Where(d => listaInteiros.Contains(d.ItemGasto.IdentificadorUsuario));

            queryResultado = queryResultado.Union(
queryCompra
.Select(d => new { Data = d.ItemGasto.Data, Tipo = "Compra", Latitude = d.ItemLoja.Latitude, Longitude = d.ItemLoja.Longitude, Texto = d.ItemLoja.Nome, Url = "", Comentario = d.ItemGasto.Descricao, IdentificadorUsuario = d.ItemGasto.IdentificadorUsuario, Nota = new Nullable<int>(), NomeUsuario = d.ItemGasto.ItemUsuario.Nome, ComentarioUsuario = "", Pedido = "" }));

            var queryFoto = this.Context.Fotos.Where(d => !d.DataExclusao.HasValue).Where(d => d.IdentificadorViagem == IdentificadorViagem).Where(d => listaInteiros.Contains(d.IdentificadorUsuario));

            queryResultado = queryResultado.Union(
queryFoto
.Select(d => new { Data = d.Data, Tipo = d.Video.Value ? "Video" : "Foto", Latitude = d.Latitude, Longitude = d.Longitude, Texto = "", Url = d.LinkFoto, Comentario = d.Comentario, IdentificadorUsuario = d.IdentificadorUsuario, Nota = new Nullable<int>(), NomeUsuario = d.ItemUsuario.Nome, ComentarioUsuario = "", Pedido = "" }));



            var queryAeroportos = this.Context.ViagemAereaAeroportos.Where(d => d.ItemViagemAerea.IdentificadorViagem == IdentificadorViagem).Where(d => !d.ItemViagemAerea.DataExclusao.HasValue).Where(d => d.DataChegada.HasValue)
              .Where(d => d.ItemViagemAerea.Avaliacoes.Where(e => listaInteiros.Contains(e.IdentificadorUsuario)).Any());


            queryResultado = queryResultado.Union(
queryAeroportos.Where(d => !d.DataPartida.HasValue || d.DataPartida > DbFunctions.AddMinutes(d.DataChegada, 2))
.SelectMany(d => d.ItemViagemAerea.Avaliacoes.Select(e => new
{
    Data = d.DataChegada,
    Tipo = d.TipoPonto == (int)enumTipoParada.Origem ? "DeslocamentoChegadaOrigem" : d.TipoPonto == (int)enumTipoParada.Destino ? "DeslocamentoChegadaDestino" : "DeslocamentoChegadaEscala",
    Latitude = d.Latitude,
    Longitude = d.Longitude,
    Texto = d.ItemViagemAerea.Descricao,
    Url = d.ItemViagemAerea.CompanhiaAerea,
    Comentario = d.Aeroporto,
    IdentificadorUsuario = e.IdentificadorUsuario,
    Nota = new Nullable<int>(),
    NomeUsuario = e.ItemUsuario.Nome,
    ComentarioUsuario = "",
    Pedido = ""
})));


            queryResultado = queryResultado.Union(
    queryAeroportos.Where(d => d.DataPartida.HasValue)
    .SelectMany(d => d.ItemViagemAerea.Avaliacoes.Select(e => new
    {
        Data = d.DataPartida,
        Tipo = (d.DataPartida > DbFunctions.AddMinutes(d.DataChegada, 2)) ?
            d.TipoPonto == (int)enumTipoParada.Origem ? "DeslocamentoPartidaOrigem" : d.TipoPonto == (int)enumTipoParada.Destino ? "DeslocamentoPartidaDestino" : "DeslocamentoPartidaEscala" :
            d.TipoPonto == (int)enumTipoParada.Origem ? "DeslocamentoOrigem" : d.TipoPonto == (int)enumTipoParada.Destino ? "DeslocamentoDestino" : "DeslocamentoEscala",
        Latitude = d.Latitude,
        Longitude = d.Longitude,
        Texto = d.ItemViagemAerea.Descricao,
        Url = d.ItemViagemAerea.CompanhiaAerea,
        Comentario = d.Aeroporto,
        IdentificadorUsuario = e.IdentificadorUsuario,
        Nota = d.TipoPonto == (int)enumTipoParada.Destino ? e.Nota : new Nullable<int>(),
        NomeUsuario = e.ItemUsuario.Nome,
        ComentarioUsuario = d.TipoPonto == (int)enumTipoParada.Destino ? e.Comentario : null,
        Pedido = ""
    })));




            if (DataMaxima.HasValue)
                queryResultado = queryResultado.Where(d => d.Data < DataMaxima);
            else if (DataMinima.HasValue)
                queryResultado = queryResultado.Where(d => d.Data > DataMinima);

            var resultado = queryResultado.GroupBy(
                d => new { d.Comentario, d.Data, d.Latitude, d.Longitude, d.Texto, d.Tipo, d.Url })
                .Select(d => new Timeline()

                {
                    Comentario = d.Key.Comentario,
                    Data = d.Key.Data,
                    Latitude = d.Key.Latitude,
                    Longitude = d.Key.Longitude,
                    Texto = d.Key.Texto,
                    Tipo = d.Key.Tipo,
                    Url = d.Key.Url,
                    Usuarios = d.Select(e => new UsuarioConsulta() { Comentario = e.ComentarioUsuario, Identificador = e.IdentificadorUsuario, Nome = e.NomeUsuario, Nota = e.Nota, Pedido = e.Pedido })
                })
                .OrderByDescending(d => d.Data)
                .Take(NumeroRegistros).ToList();

            return resultado;
        }
    }
}
