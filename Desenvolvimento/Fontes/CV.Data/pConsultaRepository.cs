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

        public List<LocaisVisitados> CarregarLocaisVisitados(int? IdentificadorViagem, DateTime? DataDe, DateTime? DataAte, string Nome, string CodigoGoogle)
        {
            List<LocaisVisitados> listaSaida = new List<LocaisVisitados>();
            var queryAtracao = this.Context.Atracoes.Where(d => d.IdentificadorViagem == IdentificadorViagem).Where(d => !d.DataExclusao.HasValue);
            if (!string.IsNullOrWhiteSpace(CodigoGoogle))
            {
                queryAtracao = queryAtracao.Where(d => this.Context.Atracoes.Where(e => e.CodigoPlace == CodigoGoogle).
                Where(e => e.IdentificadorViagem == IdentificadorViagem).Where(e => e.Identificador == d.IdentificadorAtracaoPai).Any());
            }
            else if (!string.IsNullOrEmpty(Nome))
            {
                queryAtracao = queryAtracao.Where(d => this.Context.Atracoes.Where(e => e.CodigoPlace == null || e.CodigoPlace == string.Empty).Where(e => e.Nome == Nome).
Where(e => e.IdentificadorViagem == IdentificadorViagem).Where(e => e.Identificador == d.IdentificadorAtracaoPai).Any());

            }
            if (DataDe.HasValue)
                queryAtracao = queryAtracao.Where(d => d.Chegada >= DataDe || (d.Partida.HasValue && d.Partida >= DataDe));
            if (DataAte.HasValue)
                queryAtracao = queryAtracao.Where(d => d.Chegada < DataAte && (!d.Partida.HasValue || d.Partida < DataAte));
            listaSaida = queryAtracao.GroupBy(d => new { CodigoGoogle = (d.CodigoPlace == null) ? string.Empty : d.CodigoPlace, Nome = d.CodigoPlace != null && d.CodigoPlace != string.Empty ? string.Empty : d.Nome, IdentificadorCidade = d.IdentificadorCidade, NomeCidade = d.ItemCidade.Nome })
                .Select(d => new LocaisVisitados()
                {
                    CodigoCoogle = d.Key.CodigoGoogle,
                    Nome = d.Select(e => e.Nome).FirstOrDefault(),
                    IdentificadorCidade = d.Key.IdentificadorCidade,
                    NomeCidade = d.Key.NomeCidade,
                    Tipo = "A",
                    Latitude = d.Where(e => e.Latitude.HasValue && e.Longitude.HasValue).Select(e => e.Latitude).FirstOrDefault(),
                    Longitude = d.Where(e => e.Latitude.HasValue && e.Longitude.HasValue).Select(e => e.Longitude).FirstOrDefault(),
                }).ToList();

            if (string.IsNullOrEmpty(CodigoGoogle) && string.IsNullOrEmpty(Nome))
            {
                var queryHotel = this.Context.Hoteis.Where(d => d.IdentificadorViagem == IdentificadorViagem).Where(d => !d.DataExclusao.HasValue);
                if (DataDe.HasValue)
                    queryHotel = queryHotel.Where(d => d.DataEntrada >= DataDe || (d.DataSaidia.HasValue && d.DataSaidia >= DataDe));
                if (DataAte.HasValue)
                    queryHotel = queryHotel.Where(d => d.DataEntrada < DataAte && (!d.DataSaidia.HasValue || d.DataSaidia < DataAte));

                listaSaida = listaSaida.Union(queryHotel.GroupBy(d => new { CodigoGoogle = (d.CodigoPlace == null) ? string.Empty : d.CodigoPlace, Nome = d.CodigoPlace != null && d.CodigoPlace != string.Empty ? string.Empty : d.Nome, IdentificadorCidade = d.IdentificadorCidade, NomeCidade = d.ItemCidade.Nome })
                .Select(d => new LocaisVisitados()
                {
                    CodigoCoogle = d.Key.CodigoGoogle,
                    Nome = d.Select(e => e.Nome).FirstOrDefault(),
                    IdentificadorCidade = d.Key.IdentificadorCidade,
                    NomeCidade = d.Key.NomeCidade,
                    Tipo = "H",
                    Latitude = d.Where(e => e.Latitude.HasValue && e.Longitude.HasValue).Select(e => e.Latitude).FirstOrDefault(),
                    Longitude = d.Where(e => e.Latitude.HasValue && e.Longitude.HasValue).Select(e => e.Longitude).FirstOrDefault(),
                }).ToList()).ToList();
            }

            var queryRestaurante = this.Context.Refeicoes.Where(d => d.IdentificadorViagem == IdentificadorViagem).Where(d => !d.DataExclusao.HasValue);
            if (!string.IsNullOrWhiteSpace(CodigoGoogle))
            {
                queryRestaurante = queryRestaurante.Where(d => this.Context.Atracoes.Where(e => e.CodigoPlace == CodigoGoogle).
                Where(e => e.IdentificadorViagem == IdentificadorViagem).Where(e => e.Identificador == d.IdentificadorAtracao).Any());
            }
            else if (!string.IsNullOrEmpty(Nome))
            {
                queryRestaurante = queryRestaurante.Where(d => this.Context.Atracoes.Where(e => e.CodigoPlace == null || e.CodigoPlace == string.Empty).Where(e => e.Nome == Nome).
Where(e => e.IdentificadorViagem == IdentificadorViagem).Where(e => e.Identificador == d.IdentificadorAtracao).Any());

            }
            if (DataDe.HasValue)
                queryRestaurante = queryRestaurante.Where(d => d.Data >= DataDe);
            if (DataAte.HasValue)
                queryRestaurante = queryRestaurante.Where(d => d.Data < DataAte);
            listaSaida = listaSaida.Union(queryRestaurante.GroupBy(d => new { CodigoGoogle = (d.CodigoPlace == null) ? string.Empty : d.CodigoPlace, Nome = d.CodigoPlace != null && d.CodigoPlace != string.Empty ? string.Empty : d.Nome, IdentificadorCidade = d.IdentificadorCidade, NomeCidade = d.ItemCidade.Nome })
              .Select(d => new LocaisVisitados()
              {
                  CodigoCoogle = d.Key.CodigoGoogle,
                  Nome = d.Select(e => e.Nome).FirstOrDefault(),
                  IdentificadorCidade = d.Key.IdentificadorCidade,
                  NomeCidade = d.Key.NomeCidade,
                  Tipo = "R",
                  Latitude = d.Where(e => e.Latitude.HasValue && e.Longitude.HasValue).Select(e => e.Latitude).FirstOrDefault(),
                  Longitude = d.Where(e => e.Latitude.HasValue && e.Longitude.HasValue).Select(e => e.Longitude).FirstOrDefault(),
              }).ToList()).ToList();

            var queryLoja = this.Context.Lojas.Where(d => d.IdentificadorViagem == IdentificadorViagem).Where(d => !d.DataExclusao.HasValue);
            if (!string.IsNullOrWhiteSpace(CodigoGoogle))
            {
                queryLoja = queryLoja.Where(d => this.Context.Atracoes.Where(e => e.CodigoPlace == CodigoGoogle).
                Where(e => e.IdentificadorViagem == IdentificadorViagem).Where(e => e.Identificador == d.IdentificadorAtracao).Any());
            }
            else if (!string.IsNullOrEmpty(Nome))
            {
                queryLoja = queryLoja.Where(d => this.Context.Atracoes.Where(e => e.CodigoPlace == null || e.CodigoPlace == string.Empty).Where(e => e.Nome == Nome).
Where(e => e.IdentificadorViagem == IdentificadorViagem).Where(e => e.Identificador == d.IdentificadorAtracao).Any());

            }
            IQueryable<GastoCompra> queryCompra = this.Context.GastoCompras.Where(d => !d.DataExclusao.HasValue);
            if (DataDe.HasValue)
                queryCompra = queryCompra.Where(d => d.ItemGasto.Data >= DataDe);
            if (DataAte.HasValue)
                queryCompra = queryCompra.Where(d => d.ItemGasto.Data < DataAte);
            queryLoja = queryLoja.Where(d => queryCompra.Where(e => e.IdentificadorLoja == d.Identificador).Any());
            listaSaida = listaSaida.Union(queryLoja.GroupBy(d => new { CodigoGoogle = (d.CodigoPlace == null) ? string.Empty : d.CodigoPlace, Nome = d.CodigoPlace != null && d.CodigoPlace != string.Empty ? string.Empty : d.Nome, IdentificadorCidade = d.IdentificadorCidade, NomeCidade = d.ItemCidade.Nome })
              .Select(d => new LocaisVisitados()
              {
                  CodigoCoogle = d.Key.CodigoGoogle,
                  Nome = d.Select(e => e.Nome).FirstOrDefault(),
                  IdentificadorCidade = d.Key.IdentificadorCidade,
                  NomeCidade = d.Key.NomeCidade,
                  Tipo = "L",
                  Latitude = d.Where(e => e.Latitude.HasValue && e.Longitude.HasValue).Select(e => e.Latitude).FirstOrDefault(),
                  Longitude = d.Where(e => e.Latitude.HasValue && e.Longitude.HasValue).Select(e => e.Longitude).FirstOrDefault(),
              }).ToList()).ToList();
            return listaSaida.OrderBy(d => d.Nome).ToList();
        }

        public List<LocaisDetalhes> CarregarDetalhesAtracaoVisitada(int? IdentificadorViagem, DateTime? DataDe, DateTime? DataAte, string Nome, string CodigoGoogle)
        {
            var queryAtracao = this.Context.Atracoes.Where(d => d.IdentificadorViagem == IdentificadorViagem).Where(d => !d.DataExclusao.HasValue);
            if (!string.IsNullOrWhiteSpace(CodigoGoogle))
            {
                queryAtracao = queryAtracao.Where(d => d.CodigoPlace == CodigoGoogle);
            }
            else
            {
                queryAtracao = queryAtracao.Where(d => d.CodigoPlace == null || d.CodigoPlace == string.Empty).Where(d => d.Nome == Nome);
            }
            if (DataDe.HasValue)
                queryAtracao = queryAtracao.Where(d => d.Chegada >= DataDe || (d.Partida.HasValue && d.Partida >= DataDe));
            if (DataAte.HasValue)
                queryAtracao = queryAtracao.Where(d => d.Chegada < DataAte && (!d.Partida.HasValue || d.Partida < DataAte));
            return queryAtracao.SelectMany(d => d.Avaliacoes.Where(e => !e.DataExclusao.HasValue).Select(e => new LocaisDetalhes() { Comentario = e.Comentario, DataDe = d.Chegada, DataAte = d.Partida, Nota = e.Nota, NomeUsuario = e.ItemUsuario.Nome })).ToList();
        }

        public List<RelatorioGastos> ConsultarGastosAtracao(int? IdentificadorViagem, DateTime? DataDe, DateTime? DataAte, string Nome, string CodigoGoogle)
        {
            var queryAtracao = this.Context.Atracoes.Where(d => d.IdentificadorViagem == IdentificadorViagem).Where(e => !e.DataExclusao.HasValue);
            if (!string.IsNullOrWhiteSpace(CodigoGoogle))
            {
                queryAtracao = queryAtracao.Where(d => d.CodigoPlace == CodigoGoogle);
            }
            else
            {
                queryAtracao = queryAtracao.Where(d => d.CodigoPlace == null || d.CodigoPlace == string.Empty).Where(d => d.Nome == Nome);
            }
            if (DataDe.HasValue)
                queryAtracao = queryAtracao.Where(d => d.Chegada >= DataDe || (d.Partida.HasValue && d.Partida >= DataDe));
            if (DataAte.HasValue)
                queryAtracao = queryAtracao.Where(d => d.Chegada < DataAte && (!d.Partida.HasValue || d.Partida < DataAte));

            var ListaAtracoes = queryAtracao.Select(d => d.Identificador).Distinct().ToList();
            foreach (var Identificador in ListaAtracoes.ToList())
            {
                ListaAtracoes.AddRange(ListarAtracoesFilho(Identificador));
            }

            return this.Context.Gastos.Where(e => !e.DataExclusao.HasValue).Where(d => d.Atracoes.Where(e => !e.DataExclusao.HasValue).Where(e => ListaAtracoes.Contains(e.IdentificadorAtracao)).Any()
            || d.Refeicoes.Where(e => !e.DataExclusao.HasValue).Where(e => ListaAtracoes.Contains(e.ItemRefeicao.IdentificadorAtracao)).Any()
            || d.Compras.Where(e => !e.DataExclusao.HasValue).Where(e => ListaAtracoes.Contains(e.ItemLoja.IdentificadorAtracao)).Any()
            ).
                Select(d => new RelatorioGastos() { Data = d.Data.Value, Descricao = d.Descricao, NomeUsuario = d.ItemUsuario.Nome, Moeda = d.Moeda, Valor = d.Valor.Value }).ToList();
        }

        public List<int?> ListarAtracoesFilho(int? IdentificadorAtracao)
        {
            List<int?> listaAtracao = new List<int?>();
            var queryAtracao = this.Context.Atracoes.Where(d => d.IdentificadorAtracaoPai == IdentificadorAtracao).Where(e => !e.DataExclusao.HasValue);
            listaAtracao = queryAtracao.Select(d => d.Identificador).Distinct().ToList();
            foreach (var id in listaAtracao.ToList())
            {
                listaAtracao.AddRange(ListarAtracoesFilho(id));
            }
            return listaAtracao;

        }

        public List<Foto> ConsultarFotosAtracao(int? IdentificadorViagem, DateTime? DataDe, DateTime? DataAte, string Nome, string CodigoGoogle)
        {
            var queryAtracao = this.Context.Atracoes.Where(d => d.IdentificadorViagem == IdentificadorViagem).Where(e => !e.DataExclusao.HasValue);
            if (!string.IsNullOrWhiteSpace(CodigoGoogle))
            {
                queryAtracao = queryAtracao.Where(d => d.CodigoPlace == CodigoGoogle);
            }
            else
            {
                queryAtracao = queryAtracao.Where(d => d.CodigoPlace == null || d.CodigoPlace == string.Empty).Where(d => d.Nome == Nome);
            }
            if (DataDe.HasValue)
                queryAtracao = queryAtracao.Where(d => d.Chegada >= DataDe || (d.Partida.HasValue && d.Partida >= DataDe));
            if (DataAte.HasValue)
                queryAtracao = queryAtracao.Where(d => d.Chegada < DataAte && (!d.Partida.HasValue || d.Partida < DataAte));

            var QueryFoto = this.Context.Fotos.Where(d => d.IdentificadorViagem == IdentificadorViagem).Where(e => !e.DataExclusao.HasValue);
            if (DataDe.HasValue)
                QueryFoto = QueryFoto.Where(d => d.Data >= DataDe);
            if (DataAte.HasValue)
                QueryFoto = QueryFoto.Where(d => d.Data < DataAte);

            return QueryFoto.Where(d => d.Atracoes.Where(e => !e.DataExclusao.HasValue).Where(e => queryAtracao.Where(f => f.Identificador == e.IdentificadorAtracao).Any()).Any()).ToList();

        }

        public List<LocaisDetalhes> CarregarDetalhesHotelHospedado(int? IdentificadorViagem, DateTime? DataDe, DateTime? DataAte, string Nome, string CodigoGoogle)
        {
            var queryAtracao = this.Context.Hoteis.Where(d => d.IdentificadorViagem == IdentificadorViagem).Where(e => !e.DataExclusao.HasValue);
            if (!string.IsNullOrWhiteSpace(CodigoGoogle))
            {
                queryAtracao = queryAtracao.Where(d => d.CodigoPlace == CodigoGoogle);
            }
            else
            {
                queryAtracao = queryAtracao.Where(d => d.CodigoPlace == null || d.CodigoPlace == string.Empty).Where(d => d.Nome == Nome);
            }
            if (DataDe.HasValue)
                queryAtracao = queryAtracao.Where(d => d.DataEntrada >= DataDe || (d.DataSaidia.HasValue && d.DataSaidia >= DataDe));
            if (DataAte.HasValue)
                queryAtracao = queryAtracao.Where(d => d.DataEntrada < DataAte && (!d.DataSaidia.HasValue || d.DataSaidia < DataAte));
            return queryAtracao.SelectMany(d => d.Avaliacoes.Where(e => !e.DataExclusao.HasValue).Select(e => new LocaisDetalhes() { Comentario = e.Comentario, DataDe = d.DataEntrada, DataAte = d.DataSaidia, Nota = e.Nota, NomeUsuario = e.ItemUsuario.Nome })).ToList();
        }

        public List<RelatorioGastos> ConsultarGastosHotel(int? IdentificadorViagem, DateTime? DataDe, DateTime? DataAte, string Nome, string CodigoGoogle)
        {
            var queryAtracao = this.Context.Hoteis.Where(d => d.IdentificadorViagem == IdentificadorViagem).Where(e => !e.DataExclusao.HasValue);
            if (!string.IsNullOrWhiteSpace(CodigoGoogle))
            {
                queryAtracao = queryAtracao.Where(d => d.CodigoPlace == CodigoGoogle);
            }
            else
            {
                queryAtracao = queryAtracao.Where(d => d.CodigoPlace == null || d.CodigoPlace == string.Empty).Where(d => d.Nome == Nome);
            }
            if (DataDe.HasValue)
                queryAtracao = queryAtracao.Where(d => d.DataEntrada >= DataDe || (d.DataSaidia.HasValue && d.DataSaidia >= DataDe));
            if (DataAte.HasValue)
                queryAtracao = queryAtracao.Where(d => d.DataEntrada < DataAte && (!d.DataSaidia.HasValue || d.DataSaidia < DataAte));

            var ListaAtracoes = queryAtracao.Select(d => d.Identificador).Distinct().ToList();

            return this.Context.Gastos.Where(e => !e.DataExclusao.HasValue).Where(d => d.Hoteis.Where(e => !e.DataExclusao.HasValue).Where(e => ListaAtracoes.Contains(e.IdentificadorHotel)).Any()
            ).
            Select(d => new RelatorioGastos() { Data = d.Data.Value, Descricao = d.Descricao, NomeUsuario = d.ItemUsuario.Nome, Moeda = d.Moeda, Valor = d.Valor.Value }).ToList();
        }

        public List<Foto> ConsultarFotosHotel(int? IdentificadorViagem, DateTime? DataDe, DateTime? DataAte, string Nome, string CodigoGoogle)
        {
            var queryAtracao = this.Context.Hoteis.Where(d => d.IdentificadorViagem == IdentificadorViagem).Where(e => !e.DataExclusao.HasValue);
            if (!string.IsNullOrWhiteSpace(CodigoGoogle))
            {
                queryAtracao = queryAtracao.Where(d => d.CodigoPlace == CodigoGoogle);
            }
            else
            {
                queryAtracao = queryAtracao.Where(d => d.CodigoPlace == null || d.CodigoPlace == string.Empty).Where(d => d.Nome == Nome);
            }
            if (DataDe.HasValue)
                queryAtracao = queryAtracao.Where(d => d.DataEntrada >= DataDe || (d.DataSaidia.HasValue && d.DataSaidia >= DataDe));
            if (DataAte.HasValue)
                queryAtracao = queryAtracao.Where(d => d.DataEntrada < DataAte && (!d.DataSaidia.HasValue || d.DataSaidia < DataAte));

            var QueryFoto = this.Context.Fotos.Where(d => d.IdentificadorViagem == IdentificadorViagem).Where(e => !e.DataExclusao.HasValue);
            if (DataDe.HasValue)
                QueryFoto = QueryFoto.Where(d => d.Data >= DataDe);
            if (DataAte.HasValue)
                QueryFoto = QueryFoto.Where(d => d.Data < DataAte);

            return QueryFoto.Where(d => d.Hoteis.Where(e => !e.DataExclusao.HasValue).Where(e => queryAtracao.Where(f => f.Identificador == e.IdentificadorHotel).Any()).Any()).ToList();

        }

        public List<LocaisDetalhes> CarregarDetalhesRestaurante(int? IdentificadorViagem, DateTime? DataDe, DateTime? DataAte, string Nome, string CodigoGoogle)
        {
            var queryAtracao = this.Context.Refeicoes.Where(d => d.IdentificadorViagem == IdentificadorViagem).Where(e => !e.DataExclusao.HasValue);
            if (!string.IsNullOrWhiteSpace(CodigoGoogle))
            {
                queryAtracao = queryAtracao.Where(d => d.CodigoPlace == CodigoGoogle);
            }
            else
            {
                queryAtracao = queryAtracao.Where(d => d.CodigoPlace == null || d.CodigoPlace == string.Empty).Where(d => d.Nome == Nome);
            }
            if (DataDe.HasValue)
                queryAtracao = queryAtracao.Where(d => d.Data >= DataDe);
            if (DataAte.HasValue)
                queryAtracao = queryAtracao.Where(d => d.Data < DataAte);
            return queryAtracao.SelectMany(d => d.Pedidos.Where(e => !e.DataExclusao.HasValue).Select(e => new LocaisDetalhes() { Comentario = e.Comentario, DataDe = d.Data, Nota = e.Nota, NomeUsuario = e.ItemUsuario.Nome, Pedido = e.Pedido })).ToList();
        }

        public List<RelatorioGastos> ConsultarGastosRefeicao(int? IdentificadorViagem, DateTime? DataDe, DateTime? DataAte, string Nome, string CodigoGoogle)
        {
            var queryAtracao = this.Context.Refeicoes.Where(d => d.IdentificadorViagem == IdentificadorViagem).Where(e => !e.DataExclusao.HasValue);
            if (!string.IsNullOrWhiteSpace(CodigoGoogle))
            {
                queryAtracao = queryAtracao.Where(d => d.CodigoPlace == CodigoGoogle);
            }
            else
            {
                queryAtracao = queryAtracao.Where(d => d.CodigoPlace == null || d.CodigoPlace == string.Empty).Where(d => d.Nome == Nome);
            }
            if (DataDe.HasValue)
                queryAtracao = queryAtracao.Where(d => d.Data >= DataDe);
            if (DataAte.HasValue)
                queryAtracao = queryAtracao.Where(d => d.Data < DataAte);
            var ListaAtracoes = queryAtracao.Select(d => d.Identificador).Distinct().ToList();

            return this.Context.Gastos.Where(e => !e.DataExclusao.HasValue).Where(d => d.Refeicoes.Where(e => !e.DataExclusao.HasValue).Where(e => ListaAtracoes.Contains(e.IdentificadorRefeicao)).Any()
            ).
            Select(d => new RelatorioGastos() { Data = d.Data.Value, Descricao = d.Descricao, NomeUsuario = d.ItemUsuario.Nome, Moeda = d.Moeda, Valor = d.Valor.Value }).ToList();
        }

        public List<Foto> ConsultarFotosRefeicao(int? IdentificadorViagem, DateTime? DataDe, DateTime? DataAte, string Nome, string CodigoGoogle)
        {
            var queryAtracao = this.Context.Refeicoes.Where(d => d.IdentificadorViagem == IdentificadorViagem).Where(e => !e.DataExclusao.HasValue);
            if (!string.IsNullOrWhiteSpace(CodigoGoogle))
            {
                queryAtracao = queryAtracao.Where(d => d.CodigoPlace == CodigoGoogle);
            }
            else
            {
                queryAtracao = queryAtracao.Where(d => d.CodigoPlace == null || d.CodigoPlace == string.Empty).Where(d => d.Nome == Nome);
            }
            if (DataDe.HasValue)
                queryAtracao = queryAtracao.Where(d => d.Data >= DataDe);
            if (DataAte.HasValue)
                queryAtracao = queryAtracao.Where(d => d.Data < DataAte);

            var QueryFoto = this.Context.Fotos.Where(d => d.IdentificadorViagem == IdentificadorViagem).Where(e => !e.DataExclusao.HasValue);
            if (DataDe.HasValue)
                QueryFoto = QueryFoto.Where(d => d.Data >= DataDe);
            if (DataAte.HasValue)
                QueryFoto = QueryFoto.Where(d => d.Data < DataAte);

            return QueryFoto.Where(d => d.Refeicoes.Where(e => !e.DataExclusao.HasValue).Where(e => queryAtracao.Where(f => f.Identificador == e.IdentificadorRefeicao).Any()).Any()).ToList();

        }

        public List<RelatorioGastos> ConsultarComprasLoja(int? IdentificadorViagem, DateTime? DataDe, DateTime? DataAte, string Nome, string CodigoGoogle)
        {
            var QueryGastos = this.Context.GastoCompras.Where(d => d.ItemGasto.IdentificadorViagem == IdentificadorViagem).Where(e => !e.DataExclusao.HasValue);
            if (DataDe.HasValue)
                QueryGastos = QueryGastos.Where(d => d.ItemGasto.Data >= DataDe);
            if (DataAte.HasValue)
                QueryGastos = QueryGastos.Where(d => d.ItemGasto.Data < DataAte);

            if (!string.IsNullOrWhiteSpace(CodigoGoogle))
            {
                QueryGastos = QueryGastos.Where(d => d.ItemLoja.CodigoPlace == CodigoGoogle);
            }
            else
            {
                QueryGastos = QueryGastos.Where(d => d.ItemLoja.CodigoPlace == null || d.ItemLoja.CodigoPlace == string.Empty).Where(d => d.ItemLoja.Nome == Nome);
            }



            return QueryGastos.
             Select(d => new RelatorioGastos()
             {
                 Data = d.ItemGasto.Data.Value,
                 Descricao = d.ItemGasto.Descricao,
                 NomeUsuario = d.ItemGasto.ItemUsuario.Nome,
                 Moeda = d.ItemGasto.Moeda,
                 Valor = d.ItemGasto.Valor.Value,
                 Itens = d.ItensComprados.Where(e => !e.DataExclusao.HasValue).Select(e => new LojaItens() { Descricao = e.Descricao, Marca = e.Marca, Valor = e.Valor })
             }).ToList();
        }

        public List<CalendarioRealizado> CarregarCalendarioRealizado(int? IdentificadorViagem, int? IdentificadorUsuario)
        {


            var queryConsulta = this.Context.AvaliacaoAtracoes.Where(d => d.IdentificadorUsuario == IdentificadorUsuario)
                .Where(d => !d.DataExclusao.HasValue).Where(d => d.ItemAtracao.IdentificadorViagem == IdentificadorViagem).Where(d => d.ItemAtracao.Chegada.HasValue)
                .Where(d => !d.ItemAtracao.DataExclusao.HasValue);

            var queryResultado = queryConsulta.Select(d => new CalendarioRealizado()
            {
                Complemento = null,
                DataFim = d.ItemAtracao.Partida ?? DateTime.Now,
                DataInicio = d.ItemAtracao.Chegada.Value,
                Id = d.IdentificadorAtracao.Value,
                Nome = d.ItemAtracao.Nome,
                Tipo = "A"
            });

            var queryRefeicao = this.Context.RefeicaoPedidos.Where(d => d.IdentificadorUsuario == IdentificadorUsuario)
                .Where(d => !d.DataExclusao.HasValue).Where(d => d.ItemRefeicao.IdentificadorViagem == IdentificadorViagem)
                .Where(d => !d.ItemRefeicao.DataExclusao.HasValue);

            queryResultado = queryResultado.Union(queryRefeicao.Select(d => new CalendarioRealizado()
            {
                Complemento = d.Pedido,
                DataFim = d.ItemRefeicao.Data.Value,
                DataInicio = d.ItemRefeicao.Data.Value,
                Id = d.IdentificadorRefeicao.Value,
                Nome = d.ItemRefeicao.Nome,
                Tipo = "R"
            }));

            var queryHotel = this.Context.HotelAvaliacoes.Where(d => d.IdentificadorUsuario == IdentificadorUsuario)
              .Where(d => !d.DataExclusao.HasValue).Where(d => d.ItemHotel.IdentificadorViagem == IdentificadorViagem).Where(d => d.ItemHotel.DataEntrada.HasValue)
              .Where(d => !d.ItemHotel.DataExclusao.HasValue);

            queryResultado = queryResultado.Union(
                queryHotel.Select(d => new CalendarioRealizado()
                {
                    Complemento = null,
                    DataFim = d.ItemHotel.DataEntrada.Value,
                    DataInicio = d.ItemHotel.DataEntrada.Value,
                    Id = d.IdentificadorHotel.Value,
                    Nome = d.ItemHotel.Nome,
                    Tipo = "HCI"
                }));

            var queryHotel2 = this.Context.HotelAvaliacoes.Where(d => d.IdentificadorUsuario == IdentificadorUsuario)
              .Where(d => !d.DataExclusao.HasValue).Where(d => d.ItemHotel.IdentificadorViagem == IdentificadorViagem).Where(d => d.ItemHotel.DataEntrada.HasValue).Where(d => d.ItemHotel.DataSaidia.HasValue)
              .Where(d => !d.ItemHotel.DataExclusao.HasValue);

            queryResultado = queryResultado.Union(
               queryHotel2.Select(d => new CalendarioRealizado()
               {
                   Complemento = null,
                   DataFim = d.ItemHotel.DataSaidia.Value,
                   DataInicio = d.ItemHotel.DataSaidia.Value,
                   Id = d.IdentificadorHotel.Value,
                   Nome = d.ItemHotel.Nome,
                   Tipo = "HCO"
               }));


            var queryCarro = this.Context.AvaliacaoAlugueis.Where(d => d.IdentificadorUsuario == IdentificadorUsuario).Where(d => d.ItemCarro.Alugado.Value)
             .Where(d => !d.DataExclusao.HasValue).Where(d => d.ItemCarro.IdentificadorViagem == IdentificadorViagem).Where(d => d.ItemCarro.ItemCarroEventoRetirada.Data.HasValue)
             .Where(d => !d.ItemCarro.DataExclusao.HasValue);

            queryResultado = queryResultado.Union(
                queryCarro.Select(d => new CalendarioRealizado()
                {
                    Complemento = d.ItemCarro.Locadora,
                    DataFim = d.ItemCarro.ItemCarroEventoRetirada.Data.Value,
                    DataInicio = d.ItemCarro.ItemCarroEventoRetirada.Data.Value,
                    Id = d.IdentificadorCarro.Value,
                    Nome = d.ItemCarro.Descricao,
                    Tipo = "CR"
                }));

            var queryCarro2 = this.Context.AvaliacaoAlugueis.Where(d => d.IdentificadorUsuario == IdentificadorUsuario).Where(d => d.ItemCarro.Alugado.Value)
              .Where(d => !d.DataExclusao.HasValue).Where(d => d.ItemCarro.IdentificadorViagem == IdentificadorViagem).Where(d => d.ItemCarro.ItemCarroEventoRetirada.Data.HasValue).Where(d => d.ItemCarro.ItemCarroEventoDevolucao.Data.HasValue)
              .Where(d => !d.ItemCarro.DataExclusao.HasValue);

            queryResultado = queryResultado.Union(
               queryCarro2.Select(d => new CalendarioRealizado()
               {
                   Complemento = d.ItemCarro.Locadora,
                   DataFim = d.ItemCarro.ItemCarroEventoDevolucao.Data.Value,
                   DataInicio = d.ItemCarro.ItemCarroEventoDevolucao.Data.Value,
                   Id = d.IdentificadorCarro.Value,
                   Nome = d.ItemCarro.Descricao,
                   Tipo = "CD"
               }));



            var queryCarroDeslocamento = this.Context.CarroDeslocamentoUsuarios.Where(d => d.IdentificadorUsuario == IdentificadorUsuario)
 .Where(d => !d.ItemCarroDeslocamento.DataExclusao.HasValue).Where(d => d.ItemCarroDeslocamento.ItemCarro.IdentificadorViagem == IdentificadorViagem).Where(d => d.ItemCarroDeslocamento.ItemCarroEventoPartida.Data.HasValue)
 .Where(d => !d.ItemCarroDeslocamento.ItemCarro.DataExclusao.HasValue);

            queryResultado = queryResultado.Union(queryCarroDeslocamento.Select(d => new CalendarioRealizado()
            {
                Complemento = null,
                DataFim = d.ItemCarroDeslocamento.ItemCarroEventoChegada.Data ?? DateTime.Now,
                DataInicio = d.ItemCarroDeslocamento.ItemCarroEventoPartida.Data.Value,
                Id = d.IdentificadorCarroDeslocamento.Value,
                Nome = d.ItemCarroDeslocamento.Observacao,
                Tipo = "DC"
            }));

            var queryCompra = this.Context.GastoCompras.Where(d => d.ItemGasto.IdentificadorUsuario == IdentificadorUsuario)
  .Where(d => !d.DataExclusao.HasValue).Where(d => d.ItemLoja.IdentificadorViagem == IdentificadorViagem)
  .Where(d => !d.ItemGasto.DataExclusao.HasValue);

            queryResultado = queryResultado.Union(queryCompra.Select(d => new CalendarioRealizado()
            {
                Complemento = null,
                DataFim = d.ItemGasto.Data.Value,
                DataInicio = d.ItemGasto.Data.Value,
                Id = d.Identificador.Value,
                Nome = d.ItemLoja.Nome,
                Tipo = "L"
            }));

            var queryCarroReabastecimento = this.Context.AvaliacaoAlugueis.Where(d => d.IdentificadorUsuario == IdentificadorUsuario)
 .Where(d => !d.DataExclusao.HasValue).Where(d => d.ItemCarro.IdentificadorViagem == IdentificadorViagem)
 .Where(d => !d.ItemCarro.DataExclusao.HasValue);


            queryResultado = queryResultado.Union(
              queryCarroReabastecimento.SelectMany(d => d.ItemCarro.Reabastecimentos.Where(e => !e.DataExclusao.HasValue).Select(e => new CalendarioRealizado()
              {
                  Complemento = null,
                  DataFim = e.Data.Value,
                  DataInicio = e.Data.Value,
                  Id = e.Identificador.Value,
                  Nome = d.ItemCarro.Descricao,
                  Tipo = "RC"
              })));

            var resultado = queryResultado.ToList();

            var queryVA = this.Context.AvaliacaoAereas.Include("ItemViagemAerea").Include("ItemViagemAerea.Aeroportos").Where(d => d.IdentificadorUsuario == IdentificadorUsuario)
   .Where(d => !d.DataExclusao.HasValue).Where(d => d.ItemViagemAerea.IdentificadorViagem == IdentificadorViagem)
   .Where(d => !d.ItemViagemAerea.DataExclusao.HasValue);

            foreach (var itemViagemAerea in queryVA)
            {
                CalendarioRealizado itemViajandoAtual = null;
                foreach (var itemAeroporto in itemViagemAerea.ItemViagemAerea.Aeroportos.Where(d => d.DataChegada.HasValue).Where(d => !d.DataExclusao.HasValue).OrderBy(d => d.DataChegada))
                {
                    CalendarioRealizado itemCalendario = new CalendarioRealizado()
                    {
                        Complemento = itemAeroporto.Aeroporto,
                        Nome = itemViagemAerea.ItemViagemAerea.Descricao,
                        DataInicio = itemAeroporto.DataChegada.Value,
                        DataFim = itemAeroporto.DataPartida ?? DateTime.Now,
                        Id = itemAeroporto.Identificador.Value,
                        Tipo = itemAeroporto.TipoPonto == (int)enumTipoParada.Origem ? "VO" : itemAeroporto.TipoPonto == (int)enumTipoParada.Destino ? "VD" : "VE"
                    };
                    resultado.Add(itemCalendario);
                    if (itemViajandoAtual != null)
                        itemViajandoAtual.DataFim = itemAeroporto.DataChegada.Value;
                    if (itemAeroporto.DataPartida.HasValue && itemAeroporto.TipoPonto != (int)enumTipoParada.Destino)
                    {
                        itemViajandoAtual = new CalendarioRealizado()
                        {
                            Complemento = null,
                            Nome = itemViagemAerea.ItemViagemAerea.Descricao,
                            DataInicio = itemAeroporto.DataPartida.Value,
                            DataFim = DateTime.Now,
                            Id = itemAeroporto.Identificador.Value,
                            Tipo = "VV"
                        };
                        resultado.Add(itemViajandoAtual);
                    }
                }

            }

            return resultado;
        }


        public void CarregarResumoAtracao(ResumoViagem itemResumo, int? IdentificadorViagem, int? IdentificadorUsuario, DateTime? DataDe, DateTime? DataAte)
        {
            var queryAtracao = this.Context.AvaliacaoAtracoes.Where(d => d.IdentificadorUsuario == IdentificadorUsuario)
               .Where(d => !d.DataExclusao.HasValue).Where(d => d.ItemAtracao.IdentificadorViagem == IdentificadorViagem)
               .Where(d => !d.ItemAtracao.DataExclusao.HasValue);

            if (DataDe.HasValue)
                queryAtracao = queryAtracao.Where(d => d.ItemAtracao.Chegada >= DataDe || (d.ItemAtracao.Partida.HasValue && d.ItemAtracao.Partida >= DataDe));
            if (DataAte.HasValue)
                queryAtracao = queryAtracao.Where(d => d.ItemAtracao.Chegada < DataAte && (!d.ItemAtracao.Partida.HasValue || d.ItemAtracao.Partida < DataAte));

            itemResumo.AtracoesVisitadas = queryAtracao.Where(d => d.ItemAtracao.Chegada.HasValue).Count();
            itemResumo.MinutosAtracao = TimeSpan.FromMinutes(queryAtracao.Where(d => !d.ItemAtracao.IdentificadorAtracaoPai.HasValue).Where(d => d.ItemAtracao.Partida.HasValue)
                .Sum(d => DbFunctions.DiffMinutes( d.ItemAtracao.Chegada, d.ItemAtracao.Partida)).GetValueOrDefault());
            itemResumo.NotaMediaAtracao = queryAtracao.Where(d => d.Nota.HasValue).Average(d => d.Nota);

            var queryGasto = this.Context.Gastos.Where(d => !d.DataExclusao.HasValue).Where(d => d.IdentificadorViagem == IdentificadorViagem).Where(d => !d.ApenasBaixa.Value);

            if (IdentificadorUsuario.HasValue)
                queryGasto = queryGasto.Where(d => d.IdentificadorUsuario == IdentificadorUsuario || d.Usuarios.Where(e => e.IdentificadorUsuario == IdentificadorUsuario).Any());

            queryGasto = queryGasto.Where(d => d.Atracoes.Where(e => !e.DataExclusao.HasValue)
         .Where(e => queryAtracao.Where(f => f.IdentificadorAtracao == e.IdentificadorAtracao).Any()).Any());

            var queryAjuste
                 = queryGasto.Select(
                    d => new
                    {
                        Moeda = d.Moeda,
                        Valor = d.Valor / (d.Usuarios.Count() + 1),
                        Especie = d.Especie,
                        DataPagamento = d.DataPagamento,
                        IdentificadorUsuarioGasto = d.Identificador,
                        Data = d.Data
                    }
                    );


            var queryFinal = queryAjuste.
                GroupJoin(this.Context.CotacaoMoedas.Where(d => d.IdentificadorViagem == IdentificadorViagem).Where(d => !d.DataExclusao.HasValue), d => d.Moeda, d => d.Moeda,
                (g, cm) => new
                {
                    g.IdentificadorUsuarioGasto,
                    g.DataPagamento,
                    g.Especie,
                    g.Moeda,
                    g.Valor,
                    g.Data,
                    ValorReal = g.Moeda != (int)enumMoeda.BRL && !g.Especie.Value ? g.Valor * (cm.Where(d => d.DataCotacao <= g.DataPagamento).Any() ? cm.Where(d => d.DataCotacao <= g.DataPagamento).Select(e => e.ValorCotacao).FirstOrDefault() : 0) : g.Valor
                }).GroupJoin(this.Context.AporteDinheiros.Where(d => d.IdentificadorViagem == IdentificadorViagem).Where(d => !d.DataExclusao.HasValue),
                d => new { IdentificadorUsuario = d.IdentificadorUsuarioGasto, d.Moeda },
                           d => new { d.IdentificadorUsuario, d.Moeda },
                (g, cm) => new RelatorioGastos
                {

                    ValorReal = g.Moeda != (int)enumMoeda.BRL && g.Especie.Value ? g.Valor.Value * (cm.Where(d => d.DataAporte <= g.Data).Any() ? cm.Where(d => d.DataAporte <= g.DataPagamento).Select(e => e.Cotacao.Value).FirstOrDefault() : 0) : g.ValorReal.Value
                });

            itemResumo.TotalReaisAtracao = queryFinal.Any()? queryFinal.Sum(d => d.ValorReal):0;
        }

        public void CarregarResumoHotel(ResumoViagem itemResumo, int? IdentificadorViagem, int? IdentificadorUsuario, DateTime? DataDe, DateTime? DataAte)
        {
            var queryHotel = this.Context.HotelAvaliacoes.Where(d => d.IdentificadorUsuario == IdentificadorUsuario)
               .Where(d => !d.DataExclusao.HasValue).Where(d => d.ItemHotel.IdentificadorViagem == IdentificadorViagem)
               .Where(d => !d.ItemHotel.DataExclusao.HasValue);

            if (DataDe.HasValue)
                queryHotel = queryHotel.Where(d => d.ItemHotel.DataEntrada >= DataDe || (d.ItemHotel.DataSaidia.HasValue && d.ItemHotel.DataSaidia >= DataDe));
            if (DataAte.HasValue)
                queryHotel = queryHotel.Where(d => d.ItemHotel.DataEntrada < DataAte && (!d.ItemHotel.DataSaidia.HasValue || d.ItemHotel.DataSaidia < DataAte));

            itemResumo.NumeroHotel = queryHotel.Where(d => d.ItemHotel.DataEntrada.HasValue).Count();


            var queryHotelEvento = this.Context.HotelEventos.Where(d => d.IdentificadorUsuario == IdentificadorUsuario)
              .Where(d => !d.DataExclusao.HasValue).Where(d => d.ItemHotel.IdentificadorViagem == IdentificadorViagem)
              .Where(d => !d.ItemHotel.DataExclusao.HasValue);
            if (DataDe.HasValue)
                queryHotelEvento = queryHotelEvento.Where(d => d.DataEntrada >= DataDe || (d.DataSaida.HasValue && d.DataSaida >= DataDe));
            if (DataAte.HasValue)
                queryHotelEvento = queryHotelEvento.Where(d => d.DataEntrada < DataAte && (!d.DataSaida.HasValue || d.DataSaida < DataAte));


            itemResumo.TempoHotel = TimeSpan.FromMinutes(queryHotelEvento.Where(d => d.DataSaida.HasValue)
                .Sum(d => DbFunctions.DiffMinutes( d.DataEntrada, d.DataSaida)).GetValueOrDefault());

            itemResumo.NotaMediaHotel = queryHotel.Where(d => d.Nota.HasValue).Average(d => d.Nota);

            var queryGasto = this.Context.Gastos.Where(d => !d.DataExclusao.HasValue).Where(d => d.IdentificadorViagem == IdentificadorViagem).Where(d => !d.ApenasBaixa.Value);

            if (IdentificadorUsuario.HasValue)
                queryGasto = queryGasto.Where(d => d.IdentificadorUsuario == IdentificadorUsuario || d.Usuarios.Where(e => e.IdentificadorUsuario == IdentificadorUsuario).Any());

            queryGasto = queryGasto.Where(d => d.Hoteis.Where(e => !e.DataExclusao.HasValue)
         .Where(e => queryHotel.Where(f => f.IdentificadorHotel == e.IdentificadorHotel).Any()).Any());

            DateTime dataDeBase = DataDe ?? new DateTime(1900, 01, 01);
            DateTime dataAteBase = DataAte ?? DateTime.Today;

            itemResumo.NoitesHotel = queryHotel.Where(d => d.ItemHotel.DataEntrada.HasValue).Sum(d => DbFunctions.DiffDays(
                  d.ItemHotel.DataEntrada >= dataDeBase ? d.ItemHotel.DataEntrada : dataDeBase,
                  (d.ItemHotel.DataSaidia ?? DateTime.Today) <= dataAteBase ? d.ItemHotel.DataSaidia ?? DateTime.Today : dataAteBase
                  ));

            var queryAjuste
                 = queryGasto.Select(
                    d => new
                    {
                        Moeda = d.Moeda,
                        Valor = d.Valor / (d.Usuarios.Count() + 1),
                        Especie = d.Especie,
                        DataPagamento = d.DataPagamento,
                        IdentificadorUsuarioGasto = d.Identificador,
                        Data = d.Data
                    }
                    );


            var queryFinal = queryAjuste.
                GroupJoin(this.Context.CotacaoMoedas.Where(d => d.IdentificadorViagem == IdentificadorViagem).Where(d => !d.DataExclusao.HasValue), d => d.Moeda, d => d.Moeda,
                (g, cm) => new
                {
                    g.IdentificadorUsuarioGasto,
                    g.DataPagamento,
                    g.Especie,
                    g.Moeda,
                    g.Valor,
                    g.Data,
                    ValorReal = g.Moeda != (int)enumMoeda.BRL && !g.Especie.Value ? g.Valor * (cm.Where(d => d.DataCotacao <= g.DataPagamento).Any() ? cm.Where(d => d.DataCotacao <= g.DataPagamento).Select(e => e.ValorCotacao).FirstOrDefault() : 0) : g.Valor
                }).GroupJoin(this.Context.AporteDinheiros.Where(d => d.IdentificadorViagem == IdentificadorViagem).Where(d => !d.DataExclusao.HasValue),
                d => new { IdentificadorUsuario = d.IdentificadorUsuarioGasto, d.Moeda },
                           d => new { d.IdentificadorUsuario, d.Moeda },
                (g, cm) => new RelatorioGastos
                {

                    ValorReal = g.Moeda != (int)enumMoeda.BRL && g.Especie.Value ? g.Valor.Value * (cm.Where(d => d.DataAporte <= g.Data).Any() ? cm.Where(d => d.DataAporte <= g.DataPagamento).Select(e => e.Cotacao.Value).FirstOrDefault() : 0) : g.ValorReal.Value
                });

            itemResumo.TotalReaisHospedagem = queryFinal.Any() ? queryFinal.Sum(d => d.ValorReal) : 0;
            itemResumo.PrecoMediaNoite = itemResumo.NoitesHotel.GetValueOrDefault(0) > 0 ? itemResumo.TotalReaisHospedagem / Convert.ToDecimal(itemResumo.NoitesHotel) : 0;
        }

        public void CarregarResumoRefeicao(ResumoViagem itemResumo, int? IdentificadorViagem, int? IdentificadorUsuario, DateTime? DataDe, DateTime? DataAte)
        {
            var queryRestaurante = this.Context.RefeicaoPedidos.Where(d => d.IdentificadorUsuario == IdentificadorUsuario)
               .Where(d => !d.DataExclusao.HasValue).Where(d => d.ItemRefeicao.IdentificadorViagem == IdentificadorViagem)
               .Where(d => !d.ItemRefeicao.DataExclusao.HasValue);

            if (DataDe.HasValue)
                queryRestaurante = queryRestaurante.Where(d => d.ItemRefeicao.Data >= DataDe);
            if (DataAte.HasValue)
                queryRestaurante = queryRestaurante.Where(d => d.ItemRefeicao.Data < DataAte);

            itemResumo.RefeicoesRealizadas = queryRestaurante.Count();
            itemResumo.NotaMediaRefeicao = queryRestaurante.Where(d => d.Nota.HasValue).Average(d => d.Nota);

            var queryGasto = this.Context.Gastos.Where(d => !d.DataExclusao.HasValue).Where(d => d.IdentificadorViagem == IdentificadorViagem).Where(d => !d.ApenasBaixa.Value);

            if (IdentificadorUsuario.HasValue)
                queryGasto = queryGasto.Where(d => d.IdentificadorUsuario == IdentificadorUsuario || d.Usuarios.Where(e => e.IdentificadorUsuario == IdentificadorUsuario).Any());

            queryGasto = queryGasto.Where(d => d.Refeicoes.Where(e => !e.DataExclusao.HasValue)
         .Where(e => queryRestaurante.Where(f => f.IdentificadorRefeicao == e.IdentificadorRefeicao).Any()).Any());

            var queryAjuste
                 = queryGasto.Select(
                    d => new
                    {
                        Moeda = d.Moeda,
                        Valor = d.Valor / (d.Usuarios.Count() + 1),
                        Especie = d.Especie,
                        DataPagamento = d.DataPagamento,
                        IdentificadorUsuarioGasto = d.Identificador,
                        Data = d.Data
                    }
                    );


            var queryFinal = queryAjuste.
                GroupJoin(this.Context.CotacaoMoedas.Where(d => d.IdentificadorViagem == IdentificadorViagem).Where(d => !d.DataExclusao.HasValue), d => d.Moeda, d => d.Moeda,
                (g, cm) => new
                {
                    g.IdentificadorUsuarioGasto,
                    g.DataPagamento,
                    g.Especie,
                    g.Moeda,
                    g.Valor,
                    g.Data,
                    ValorReal = g.Moeda != (int)enumMoeda.BRL && !g.Especie.Value ? g.Valor * (cm.Where(d => d.DataCotacao <= g.DataPagamento).Any() ? cm.Where(d => d.DataCotacao <= g.DataPagamento).Select(e => e.ValorCotacao).FirstOrDefault() : 0) : g.Valor
                }).GroupJoin(this.Context.AporteDinheiros.Where(d => d.IdentificadorViagem == IdentificadorViagem).Where(d => !d.DataExclusao.HasValue),
                d => new { IdentificadorUsuario = d.IdentificadorUsuarioGasto, d.Moeda },
                           d => new { d.IdentificadorUsuario, d.Moeda },
                (g, cm) => new RelatorioGastos
                {

                    ValorReal = g.Moeda != (int)enumMoeda.BRL && g.Especie.Value ? g.Valor.Value * (cm.Where(d => d.DataAporte <= g.Data).Any() ? cm.Where(d => d.DataAporte <= g.DataPagamento).Select(e => e.Cotacao.Value).FirstOrDefault() : 0) : g.ValorReal.Value
                });

            itemResumo.TotalReaisRefeicao = queryFinal.Any() ? queryFinal.Sum(d => d.ValorReal) : 0;
            itemResumo.PrecoMediaRefeicao = itemResumo.RefeicoesRealizadas.GetValueOrDefault(0) > 0 ? itemResumo.TotalReaisRefeicao / Convert.ToDecimal(itemResumo.RefeicoesRealizadas) : 0;
        }

        public void CarregarResumoCompra(ResumoViagem itemResumo, int? IdentificadorViagem, int? IdentificadorUsuario, DateTime? DataDe, DateTime? DataAte)
        {
            var queryRestaurante = this.Context.AvaliacaoLojas.Where(d => d.IdentificadorUsuario == IdentificadorUsuario)
               .Where(d => !d.DataExclusao.HasValue).Where(d => d.ItemLoja.IdentificadorViagem == IdentificadorViagem)
               .Where(d => !d.ItemLoja.DataExclusao.HasValue);


            itemResumo.NotaMediaLoja = queryRestaurante.Where(d => d.Nota.HasValue).Average(d => d.Nota);

            var queryGasto = this.Context.Gastos.Where(d => !d.DataExclusao.HasValue).Where(d => d.IdentificadorViagem == IdentificadorViagem).Where(d => !d.ApenasBaixa.Value);

            if (IdentificadorUsuario.HasValue)
                queryGasto = queryGasto.Where(d => d.IdentificadorUsuario == IdentificadorUsuario || d.Usuarios.Where(e => e.IdentificadorUsuario == IdentificadorUsuario).Any());

            queryGasto = queryGasto.Where(d => d.Compras.Where(e => !e.DataExclusao.HasValue).Any());
            if (DataDe.HasValue)
                queryGasto = queryGasto.Where(d => d.Data >= DataDe);
            if (DataAte.HasValue)
                queryGasto = queryGasto.Where(d => d.Data < DataAte);

            itemResumo.LojasVisitadas = queryGasto.SelectMany(d => d.Compras.Select(e => e.IdentificadorLoja)).Count();
            itemResumo.ComprasRealizadas = queryGasto.Count();
            itemResumo.ItensComprados = queryGasto.SelectMany(d => d.Compras.SelectMany(e => e.ItensComprados)).Count();

            var queryAjuste
                 = queryGasto.Select(
                    d => new
                    {
                        Moeda = d.Moeda,
                        Valor = d.Valor / (d.Usuarios.Count() + 1),
                        Especie = d.Especie,
                        DataPagamento = d.DataPagamento,
                        IdentificadorUsuarioGasto = d.Identificador,
                        Data = d.Data
                    }
                    );


            var queryFinal = queryAjuste.
                GroupJoin(this.Context.CotacaoMoedas.Where(d => d.IdentificadorViagem == IdentificadorViagem).Where(d => !d.DataExclusao.HasValue), d => d.Moeda, d => d.Moeda,
                (g, cm) => new
                {
                    g.IdentificadorUsuarioGasto,
                    g.DataPagamento,
                    g.Especie,
                    g.Moeda,
                    g.Valor,
                    g.Data,
                    ValorReal = g.Moeda != (int)enumMoeda.BRL && !g.Especie.Value ? g.Valor * (cm.Where(d => d.DataCotacao <= g.DataPagamento).Any() ? cm.Where(d => d.DataCotacao <= g.DataPagamento).Select(e => e.ValorCotacao).FirstOrDefault() : 0) : g.Valor
                }).GroupJoin(this.Context.AporteDinheiros.Where(d => d.IdentificadorViagem == IdentificadorViagem).Where(d => !d.DataExclusao.HasValue),
                d => new { IdentificadorUsuario = d.IdentificadorUsuarioGasto, d.Moeda },
                           d => new { d.IdentificadorUsuario, d.Moeda },
                (g, cm) => new RelatorioGastos
                {

                    ValorReal = g.Moeda != (int)enumMoeda.BRL && g.Especie.Value ? g.Valor.Value * (cm.Where(d => d.DataAporte <= g.Data).Any() ? cm.Where(d => d.DataAporte <= g.DataPagamento).Select(e => e.Cotacao.Value).FirstOrDefault() : 0) : g.ValorReal.Value
                });

            itemResumo.TotalReaisCompra =   queryFinal.Any() ? queryFinal.Sum(d => d.ValorReal) : 0;
        }

        public void CarregarResumoDiversos(ResumoViagem itemResumo, int? IdentificadorViagem, int? IdentificadorUsuario, DateTime? DataDe, DateTime? DataAte)
        {
            var querycomentarios = this.Context.Comentarios.Where(d => d.IdentificadorUsuario == IdentificadorUsuario)
               .Where(d => !d.DataExclusao.HasValue).Where(d => d.IdentificadorViagem == IdentificadorViagem);

            if (DataDe.HasValue)
                querycomentarios = querycomentarios.Where(d => d.Data >= DataDe);
            if (DataAte.HasValue)
                querycomentarios = querycomentarios.Where(d => d.Data < DataAte);


            var queryRestaurante = this.Context.Fotos.Where(d => d.IdentificadorUsuario == IdentificadorUsuario)
               .Where(d => !d.DataExclusao.HasValue).Where(d => d.IdentificadorViagem == IdentificadorViagem);

            if (DataDe.HasValue)
                queryRestaurante = queryRestaurante.Where(d => d.Data >= DataDe);
            if (DataAte.HasValue)
                queryRestaurante = queryRestaurante.Where(d => d.Data < DataAte);

            itemResumo.ComentariosFeitos = querycomentarios.Count();
            itemResumo.FotosTirada = queryRestaurante.Where(d => !d.Video.Value).Count();
            itemResumo.VideosGravados = queryRestaurante.Where(d => d.Video.Value).Count();
        }


        public List<IntervaloDeslocamento> CarregarResumoViagemAerea(ResumoViagem itemResumo, int? IdentificadorViagem, int? IdentificadorUsuario, DateTime? DataDe, DateTime? DataAte)
        {
            List<IntervaloDeslocamento> ListaSaida = new List<IntervaloDeslocamento>();
            var queryViagemAerea = this.Context.AvaliacaoAereas.Where(d => d.IdentificadorUsuario == IdentificadorUsuario)
               .Where(d => !d.DataExclusao.HasValue).Where(d => d.ItemViagemAerea.IdentificadorViagem == IdentificadorViagem)
               .Where(d => !d.ItemViagemAerea.DataExclusao.HasValue);

            if (DataDe.HasValue)
                queryViagemAerea = queryViagemAerea.Where(d => d.ItemViagemAerea.Aeroportos.Where(e => e.TipoPonto == (int)enumTipoParada.Origem).Where(e => e.DataChegada >= DataDe).Any() ||
                (d.ItemViagemAerea.Aeroportos.Where(e => e.TipoPonto == (int)enumTipoParada.Destino).Where(e => e.DataPartida.HasValue).Any() &&
                d.ItemViagemAerea.Aeroportos.Where(e => e.TipoPonto == (int)enumTipoParada.Destino).Where(e => e.DataPartida >= DataDe).Any()));

            if (DataAte.HasValue)

                queryViagemAerea = queryViagemAerea.Where(d => d.ItemViagemAerea.Aeroportos.Where(e => e.TipoPonto == (int)enumTipoParada.Origem).Where(e => e.DataChegada <= DataAte).Any() &&
               (!d.ItemViagemAerea.Aeroportos.Where(e => e.TipoPonto == (int)enumTipoParada.Destino).Where(e => e.DataPartida.HasValue).Any() ||
               d.ItemViagemAerea.Aeroportos.Where(e => e.TipoPonto == (int)enumTipoParada.Destino).Where(e => e.DataPartida < DataAte).Any()));

            DateTime dataDeBase = DataDe ?? new DateTime(1900, 01, 01);
            DateTime dataAteBase = DataAte ?? DateTime.Today.AddDays(1);

            var queryAeroporto = queryViagemAerea.SelectMany(d => d.ItemViagemAerea.Aeroportos
           .Where(e => e.DataChegada >= dataDeBase || (e.DataPartida.HasValue && e.DataPartida >= dataDeBase))
                      .Where(e => e.DataChegada < dataAteBase && (!e.DataPartida.HasValue || e.DataPartida < dataAteBase)));

            itemResumo.MinutosAguardando = TimeSpan.FromMinutes(queryAeroporto
           .Where(e => e.DataChegada.HasValue).Where(e => e.DataPartida.HasValue)

                .Select(e => DbFunctions.DiffMinutes( e.DataChegada, e.DataPartida)).Sum().GetValueOrDefault());

            itemResumo.DeslcamentosRealizados = queryViagemAerea.Where(d => d.ItemViagemAerea.Aeroportos.Where(e => e.TipoPonto == (int)enumTipoParada.Origem).Where(e => e.DataChegada.HasValue).Any()).Count();

            itemResumo.NotaMediaDeslocamento = queryViagemAerea.Where(d => d.Nota.HasValue).Average(d => d.Nota);

            var queryGasto = this.Context.Gastos.Where(d => !d.DataExclusao.HasValue).Where(d => d.IdentificadorViagem == IdentificadorViagem).Where(d => !d.ApenasBaixa.Value);

            if (IdentificadorUsuario.HasValue)
                queryGasto = queryGasto.Where(d => d.IdentificadorUsuario == IdentificadorUsuario || d.Usuarios.Where(e => e.IdentificadorUsuario == IdentificadorUsuario).Any());

            queryGasto = queryGasto.Where(d => d.ViagenAereas.Where(e => !e.DataExclusao.HasValue)
         .Where(e => queryViagemAerea.Where(f => f.IdentificadorViagemAerea == e.IdentificadorViagemAerea).Any()).Any());

            var ListaIntervalo = queryAeroporto.Where(d => d.DataPartida.HasValue).Where(d => d.TipoPonto != (int)enumTipoParada.Destino)
                .GroupJoin(queryAeroporto.Where(d => d.DataChegada.HasValue).Where(d => d.TipoPonto != (int)enumTipoParada.Origem),
                d => d.IdentificadorViagemAerea,
                d => d.IdentificadorViagemAerea,
                (o, d) => new
                {
                    DataPartida = o.DataPartida,
                    DataChegada = d.Where(e => e.DataChegada > o.DataPartida).OrderBy(e => e.DataChegada).Select(e => e.DataChegada).FirstOrDefault()
                }
                ).ToList();

            itemResumo.MinutosViajando = TimeSpan.FromMinutes(
                ListaIntervalo.Select(d => 
                (d.DataChegada.GetValueOrDefault(DateTime.Today) <= dataAteBase ? d.DataChegada.GetValueOrDefault(DateTime.Today) : dataAteBase)
                .Subtract(d.DataPartida > dataDeBase ? d.DataPartida.GetValueOrDefault() : dataAteBase).TotalMinutes)
                .Sum());

            var queryAjuste
                 = queryGasto.Select(
                    d => new
                    {
                        Moeda = d.Moeda,
                        Valor = d.Valor / (d.Usuarios.Count() + 1),
                        Especie = d.Especie,
                        DataPagamento = d.DataPagamento,
                        IdentificadorUsuarioGasto = d.Identificador,
                        Data = d.Data
                    }
                    );


            var queryFinal = queryAjuste.
                GroupJoin(this.Context.CotacaoMoedas.Where(d => d.IdentificadorViagem == IdentificadorViagem).Where(d => !d.DataExclusao.HasValue), d => d.Moeda, d => d.Moeda,
                (g, cm) => new
                {
                    g.IdentificadorUsuarioGasto,
                    g.DataPagamento,
                    g.Especie,
                    g.Moeda,
                    g.Valor,
                    g.Data,
                    ValorReal = g.Moeda != (int)enumMoeda.BRL && !g.Especie.Value ? g.Valor * (cm.Where(d => d.DataCotacao <= g.DataPagamento).Any() ? cm.Where(d => d.DataCotacao <= g.DataPagamento).Select(e => e.ValorCotacao).FirstOrDefault() : 0) : g.Valor
                }).GroupJoin(this.Context.AporteDinheiros.Where(d => d.IdentificadorViagem == IdentificadorViagem).Where(d => !d.DataExclusao.HasValue),
                d => new { IdentificadorUsuario = d.IdentificadorUsuarioGasto, d.Moeda },
                           d => new { d.IdentificadorUsuario, d.Moeda },
                (g, cm) => new RelatorioGastos
                {

                    ValorReal = g.Moeda != (int)enumMoeda.BRL && g.Especie.Value ? g.Valor.Value * (cm.Where(d => d.DataAporte <= g.Data).Any() ? cm.Where(d => d.DataAporte <= g.DataPagamento).Select(e => e.Cotacao.Value).FirstOrDefault() : 0) : g.ValorReal.Value
                });

            itemResumo.TotalReaisDeslocamento =   queryFinal.Any() ? queryFinal.Sum(d => d.ValorReal) : 0;
            ListaSaida = ListaIntervalo.Select(d => new IntervaloDeslocamento() { Tipo = "V", Chegada = d.DataChegada, Partida = d.DataPartida }).ToList();
            return ListaSaida;
        }

        public List<IntervaloDeslocamento> CarregarResumoCarro(ResumoViagem itemResumo, int? IdentificadorViagem, int? IdentificadorUsuario, DateTime? DataDe, DateTime? DataAte)
        {
            List<IntervaloDeslocamento> ListaSaida = new List<IntervaloDeslocamento>();
            var queryViagemAerea = this.Context.AvaliacaoAlugueis.Where(d => d.IdentificadorUsuario == IdentificadorUsuario)
               .Where(d => !d.DataExclusao.HasValue).Where(d => d.ItemCarro.IdentificadorViagem == IdentificadorViagem)
               .Where(d => !d.ItemCarro.DataExclusao.HasValue);

            DateTime dataDeBase = DataDe ?? new DateTime(1900, 01, 01);
            DateTime dataAteBase = DataAte ?? DateTime.Today.AddDays(1);

            itemResumo.CarrosUtilizados = queryViagemAerea.Where(d => d.ItemCarro.Deslocamentos.Where(e=>!e.DataExclusao.HasValue) .Where(e => e.ItemCarroEventoPartida.Data >= dataDeBase || (e.ItemCarroEventoChegada.Data.HasValue && e.ItemCarroEventoChegada.Data >= dataDeBase))
            .Where(e => e.ItemCarroEventoPartida.Data < dataAteBase && (!e.ItemCarroEventoChegada.Data.HasValue || e.ItemCarroEventoChegada.Data < dataAteBase)).Any()).Count();

             itemResumo.NotaMediaAluguel = queryViagemAerea.Where(d => d.Nota.HasValue).Average(d => d.Nota);

            var queryGasto = this.Context.Gastos.Where(d => !d.DataExclusao.HasValue).Where(d => d.IdentificadorViagem == IdentificadorViagem).Where(d => !d.ApenasBaixa.Value);

            if (IdentificadorUsuario.HasValue)
                queryGasto = queryGasto.Where(d => d.IdentificadorUsuario == IdentificadorUsuario || d.Usuarios.Where(e => e.IdentificadorUsuario == IdentificadorUsuario).Any());

            queryGasto = queryGasto.Where(d => d.Alugueis.Where(e => !e.DataExclusao.HasValue)
         .Where(e => queryViagemAerea.Where(f => f.IdentificadorCarro == e.IdentificadorCarro).Any()
         ).Any()
         || d.Reabastecimentos.Where(e => !e.DataExclusao.HasValue)
         .Where(e => queryViagemAerea.Where(f => f.IdentificadorCarro == e.ItemReabastecimento.IdentificadorCarro && e.ItemReabastecimento.Data >= dataDeBase && e.ItemReabastecimento.Data <= dataAteBase).Any()
         ).Any()
         );

          

            var queryAjuste
                 = queryGasto.Select(
                    d => new
                    {
                        Moeda = d.Moeda,
                        Valor = d.Valor / (d.Usuarios.Count() + 1),
                        Especie = d.Especie,
                        DataPagamento = d.DataPagamento,
                        IdentificadorUsuarioGasto = d.Identificador,
                        Data = d.Data
                    }
                    );


            var queryFinal = queryAjuste.
                GroupJoin(this.Context.CotacaoMoedas.Where(d => d.IdentificadorViagem == IdentificadorViagem).Where(d => !d.DataExclusao.HasValue), d => d.Moeda, d => d.Moeda,
                (g, cm) => new
                {
                    g.IdentificadorUsuarioGasto,
                    g.DataPagamento,
                    g.Especie,
                    g.Moeda,
                    g.Valor,
                    g.Data,
                    ValorReal = g.Moeda != (int)enumMoeda.BRL && !g.Especie.Value ? g.Valor * (cm.Where(d => d.DataCotacao <= g.DataPagamento).Any() ? cm.Where(d => d.DataCotacao <= g.DataPagamento).Select(e => e.ValorCotacao).FirstOrDefault() : 0) : g.Valor
                }).GroupJoin(this.Context.AporteDinheiros.Where(d => d.IdentificadorViagem == IdentificadorViagem).Where(d => !d.DataExclusao.HasValue),
                d => new { IdentificadorUsuario = d.IdentificadorUsuarioGasto, d.Moeda },
                           d => new { d.IdentificadorUsuario, d.Moeda },
                (g, cm) => new RelatorioGastos
                {

                    ValorReal = g.Moeda != (int)enumMoeda.BRL && g.Especie.Value ? g.Valor.Value * (cm.Where(d => d.DataAporte <= g.Data).Any() ? cm.Where(d => d.DataAporte <= g.DataPagamento).Select(e => e.Cotacao.Value).FirstOrDefault() : 0) : g.ValorReal.Value
                });

            itemResumo.TotalReaisCarro = queryFinal.Any() ? queryFinal.Sum(d => d.ValorReal) : 0;


            queryGasto = this.Context.Gastos.Where(d => !d.DataExclusao.HasValue).Where(d => d.IdentificadorViagem == IdentificadorViagem).Where(d => !d.ApenasBaixa.Value);

            if (IdentificadorUsuario.HasValue)
                queryGasto = queryGasto.Where(d => d.IdentificadorUsuario == IdentificadorUsuario || d.Usuarios.Where(e => e.IdentificadorUsuario == IdentificadorUsuario).Any());

            queryGasto = queryGasto.Where(d=>d.Reabastecimentos.Where(e => !e.DataExclusao.HasValue)
     .Where(e => queryViagemAerea.Where(f => f.IdentificadorCarro == e.ItemReabastecimento.IdentificadorCarro && e.ItemReabastecimento.Data >= dataDeBase && e.ItemReabastecimento.Data <= dataAteBase).Any()
     ).Any()
     );



             queryAjuste
                 = queryGasto.Select(
                    d => new
                    {
                        Moeda = d.Moeda,
                        Valor = d.Valor / (d.Usuarios.Count() + 1),
                        Especie = d.Especie,
                        DataPagamento = d.DataPagamento,
                        IdentificadorUsuarioGasto = d.Identificador,
                        Data = d.Data
                    }
                    );


             queryFinal = queryAjuste.
                GroupJoin(this.Context.CotacaoMoedas.Where(d => d.IdentificadorViagem == IdentificadorViagem).Where(d => !d.DataExclusao.HasValue), d => d.Moeda, d => d.Moeda,
                (g, cm) => new
                {
                    g.IdentificadorUsuarioGasto,
                    g.DataPagamento,
                    g.Especie,
                    g.Moeda,
                    g.Valor,
                    g.Data,
                    ValorReal = g.Moeda != (int)enumMoeda.BRL && !g.Especie.Value ? g.Valor * (cm.Where(d => d.DataCotacao <= g.DataPagamento).Any() ? cm.Where(d => d.DataCotacao <= g.DataPagamento).Select(e => e.ValorCotacao).FirstOrDefault() : 0) : g.Valor
                }).GroupJoin(this.Context.AporteDinheiros.Where(d => d.IdentificadorViagem == IdentificadorViagem).Where(d => !d.DataExclusao.HasValue),
                d => new { IdentificadorUsuario = d.IdentificadorUsuarioGasto, d.Moeda },
                           d => new { d.IdentificadorUsuario, d.Moeda },
                (g, cm) => new RelatorioGastos
                {

                    ValorReal = g.Moeda != (int)enumMoeda.BRL && g.Especie.Value ? g.Valor.Value * (cm.Where(d => d.DataAporte <= g.Data).Any() ? cm.Where(d => d.DataAporte <= g.DataPagamento).Select(e => e.Cotacao.Value).FirstOrDefault() : 0) : g.ValorReal.Value
                });

            itemResumo.TotalReaisReabastecimento =   queryFinal.Any() ? queryFinal.Sum(d => d.ValorReal) : 0;

            var queryDeslocamentos = queryViagemAerea.SelectMany(d => d.ItemCarro.Deslocamentos.Where(e => !e.DataExclusao.HasValue).Where(e => e.ItemCarroEventoPartida.Data >= dataDeBase || (e.ItemCarroEventoChegada.Data.HasValue && e.ItemCarroEventoChegada.Data >= dataDeBase))
            .Where(e => e.ItemCarroEventoPartida.Data < dataAteBase && (!e.ItemCarroEventoChegada.Data.HasValue || e.ItemCarroEventoChegada.Data < dataAteBase)));

            itemResumo.MinutosDeslocamentoCarro = TimeSpan.FromMinutes(queryDeslocamentos.Where(d => d.ItemCarroEventoChegada.Data.HasValue)
               .Sum(d => DbFunctions.DiffMinutes( d.ItemCarroEventoPartida.Data, d.ItemCarroEventoChegada.Data)).GetValueOrDefault());

            itemResumo.KmDeslocamentoCarro = Convert.ToInt32(queryDeslocamentos.Where(d => d.ItemCarroEventoChegada.Data.HasValue).Where(d => d.ItemCarroEventoPartida.Odometro.HasValue)
                .Where(d => d.ItemCarroEventoChegada.Odometro.HasValue)
               .Sum(d => (d.ItemCarroEventoChegada.Odometro - d.ItemCarroEventoPartida.Odometro) * (d.ItemCarro.KM.Value ? 1 : 1.609m)));

            ListaSaida = queryDeslocamentos.Where(d => d.ItemCarroEventoChegada.Data.HasValue)
                .Select(d => new IntervaloDeslocamento() { Chegada = d.ItemCarroEventoChegada.Data, Partida = d.ItemCarroEventoPartida.Data, Tipo = "C" }).ToList();

            itemResumo.NumeroReabastecimento = queryViagemAerea.SelectMany(d => d.ItemCarro.Reabastecimentos.Where(e => !e.DataExclusao.HasValue)).Where(d => d.Data >= dataDeBase).Where(d => d.Data < dataAteBase).Count();
            itemResumo.LitrosReabastecidos = queryViagemAerea.SelectMany(d => d.ItemCarro.Reabastecimentos.Where(e => !e.DataExclusao.HasValue))
                .Where(d => d.Data >= dataDeBase).Where(d => d.Data < dataAteBase).Sum(e => e.QuantidadeReabastecida * (e.Litro.Value ? 1 : 3.78541m));

            if (DataDe.HasValue)
                queryViagemAerea = queryViagemAerea.Where(e => e.ItemCarro.ItemCarroEventoRetirada.Data >= dataDeBase || (e.ItemCarro.ItemCarroEventoDevolucao.Data.HasValue && e.ItemCarro.ItemCarroEventoDevolucao.Data >= dataDeBase));
            if (DataAte.HasValue)
                queryViagemAerea = queryViagemAerea.Where(e => e.ItemCarro.ItemCarroEventoRetirada.Data < DataAte && (!e.ItemCarro.ItemCarroEventoDevolucao.Data.HasValue && e.ItemCarro.ItemCarroEventoDevolucao.Data < DataAte));

            itemResumo.LocadorasUtilizadas = queryViagemAerea.Where(d => d.ItemCarro.Alugado.Value).GroupBy(d => d.ItemCarro.Locadora).Count();

            itemResumo.PrecoMedioKM = itemResumo.KmDeslocamentoCarro.GetValueOrDefault(0) > 0 ? itemResumo.TotalReaisCarro / Convert.ToDecimal(itemResumo.KmDeslocamentoCarro) : 0;

            itemResumo.KmLitro = itemResumo.LitrosReabastecidos.GetValueOrDefault(0) > 0 ? itemResumo.KmTotaisDeslocados / itemResumo.LitrosReabastecidos : 0;
            return ListaSaida;
        }

        public List<Posicao> ListarPosicao( int? IdentificadorViagem, int? IdentificadorUsuario, DateTime? DataDe, DateTime? DataAte)
        {
            var query = this.Context.Posicoes.Where(d => d.IdentificadorViagem == IdentificadorViagem).Where(d => d.IdentificadorUsuario == IdentificadorUsuario);
            if (DataDe.HasValue)
                query = query.Where(d => d.DataLocal >= DataDe);
            if (DataAte.HasValue)
                query = query.Where(d => d.DataLocal < DataAte);
            return query.OrderBy(d => d.DataLocal).AsNoTracking().ToList();
        }
        

    }
}
