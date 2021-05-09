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
        public List<Timeline> CarregarTimeline(int? IdentificadorViagem, int? IdentificadorUsuarioConsulta, int? IdentificadorUsuarioEvento, DateTime? DataMaxima, DateTime? DataMinima, int NumeroRegistros, string Tipo, int? Identificador)
        {
            var queryUsuarios = this.Context.ParticipanteViagemes.Where(d => d.IdentificadorUsuario == IdentificadorUsuarioConsulta || this.Context.Amigos.Where(e => e.IdentificadorAmigo == IdentificadorUsuarioConsulta && e.IdentificadorUsuario == d.IdentificadorUsuario).Any());
            if (IdentificadorUsuarioEvento.HasValue)
                queryUsuarios = queryUsuarios.Where(d => d.IdentificadorUsuario == IdentificadorUsuarioEvento);
            var listaInteiros = queryUsuarios.Select(d => d.IdentificadorUsuario);

            var queryConsulta = this.Context.Atracoes.Where(d => !d.DataExclusao.HasValue).Where(d => d.IdentificadorViagem == IdentificadorViagem).Where(d => d.Chegada.HasValue)
                .Where(d => d.Avaliacoes.Where(e => listaInteiros.Contains(e.IdentificadorUsuario)).Any());
            if (!string.IsNullOrEmpty(Tipo))
                queryConsulta = queryConsulta.Where(d => Tipo == "A");
            if (Identificador.HasValue)
                queryConsulta = queryConsulta.Where(d => d.Identificador == Identificador);
            var queryResultado = queryConsulta
                .Where(d => !d.Partida.HasValue || d.Partida > DbFunctions.AddMinutes(d.Chegada, 2))
                .SelectMany(d => d.Avaliacoes.Select(e => new { Identificador = d.Identificador, Data = d.Chegada, Tipo = "AtracaoChegada", Latitude = d.Latitude, Longitude = d.Longitude, Texto = d.Nome, Url = "", Comentario = "", IdentificadorUsuario = e.IdentificadorUsuario, Nota = new Nullable<int>(), NomeUsuario = e.ItemUsuario.Nome, ComentarioUsuario = "", Pedido = "", GoogleId = string.Empty }));//, Usuarios = d.Avaliacoes.Where(e => listaInteiros.Contains(e.IdentificadorUsuario)).Select(e => new UsuarioConsulta() { Comentario = "", Nome = e.ItemUsuario.Nome, Identificador = e.IdentificadorUsuario, Nota = null, Pedido = null }) });
            queryResultado = queryResultado.Union(
                queryConsulta.Where(d => d.Partida.HasValue)
                .SelectMany(d => d.Avaliacoes.Select(e => new { Identificador = d.Identificador, Data = d.Partida, Tipo = d.Partida > DbFunctions.AddMinutes(d.Chegada, 2) ? "AtracaoPartida" : "AtracaoVisita", Latitude = d.Latitude, Longitude = d.Longitude, Texto = d.Nome, Url = "", Comentario = "", IdentificadorUsuario = e.IdentificadorUsuario, Nota = e.Nota, NomeUsuario = e.ItemUsuario.Nome, ComentarioUsuario = e.Comentario, Pedido = "", GoogleId = string.Empty })));

            if (String.IsNullOrEmpty(Tipo) || Tipo == "R")
            {
                var queryRefeicao = this.Context.Refeicoes.Where(d => !d.DataExclusao.HasValue).Where(d => d.IdentificadorViagem == IdentificadorViagem)
                    .Where(d => d.Pedidos.Where(e => listaInteiros.Contains(e.IdentificadorUsuario)).Any());
                if (Identificador.HasValue)
                    queryRefeicao = queryRefeicao.Where(d => d.Identificador == Identificador);
                queryResultado = queryResultado.Union(
        queryRefeicao
        .SelectMany(d => d.Pedidos.Select(e => new { Identificador = d.Identificador, Data = d.Data, Tipo = "Refeicao", Latitude = d.Latitude, Longitude = d.Longitude, Texto = d.Nome, Url = "", Comentario = "", IdentificadorUsuario = e.IdentificadorUsuario, Nota = e.Nota, NomeUsuario = e.ItemUsuario.Nome, ComentarioUsuario = e.Comentario, Pedido = e.Pedido, GoogleId = string.Empty })));
/*                queryResultado = queryResultado.Union(
        queryRefeicao.Where(d=>d.DataTermino.HasValue)
        .SelectMany(d => d.Pedidos.Select(e => new { Identificador = d.Identificador, Data = d.DataTermino, Tipo = "RefeicaoFim", Latitude = d.Latitude, Longitude = d.Longitude, Texto = d.Nome, Url = "", Comentario = "", IdentificadorUsuario = e.IdentificadorUsuario, Nota = e.Nota, NomeUsuario = e.ItemUsuario.Nome, ComentarioUsuario = e.Comentario, Pedido = e.Pedido, GoogleId = string.Empty })));
        */
            }


            if (String.IsNullOrEmpty(Tipo) || Tipo == "H")
            {
                var queryHotel = this.Context.Hoteis.Where(d => !d.DataExclusao.HasValue).Where(d => d.IdentificadorViagem == IdentificadorViagem).Where(d => d.DataEntrada.HasValue)
    .Where(d => d.Avaliacoes.Where(e => listaInteiros.Contains(e.IdentificadorUsuario)).Any());
                if (Identificador.HasValue)
                    queryHotel = queryHotel.Where(d => d.Identificador == Identificador);

                queryResultado = queryResultado.Union(
        queryHotel
        .SelectMany(d => d.Avaliacoes.Select(e => new { Identificador = d.Identificador, Data = d.DataEntrada, Tipo = "HotelCheckIn", Latitude = d.Movel?d.Eventos.OrderBy(f=>f.DataEntrada).Select(f=> f.LatitudeEntrada).FirstOrDefault():d.Latitude, Longitude = d.Movel ? d.Eventos.OrderBy(f => f.DataEntrada).Select(f => f.LongitudeEntrada).FirstOrDefault() : d.Latitude, Texto = d.Nome, Url = "", Comentario = "", IdentificadorUsuario = e.IdentificadorUsuario, Nota = new Nullable<int>(), NomeUsuario = e.ItemUsuario.Nome, ComentarioUsuario = "", Pedido = "", GoogleId = string.Empty })));


                queryResultado = queryResultado.Union(
        queryHotel.Where(d => d.DataSaidia.HasValue)
        .SelectMany(d => d.Avaliacoes.Select(e => new { Identificador = d.Identificador, Data = d.DataSaidia, Tipo = "HotelChekckOut", Latitude = d.Movel ? d.Eventos.OrderByDescending(f => f.DataEntrada).Select(f => f.LatitudeSaida).FirstOrDefault() : d.Latitude, Longitude = d.Movel ? d.Eventos.OrderByDescending(f => f.DataEntrada).Select(f => f.LongitudeSaida).FirstOrDefault() : d.Latitude, Texto = d.Nome, Url = "", Comentario = "", IdentificadorUsuario = e.IdentificadorUsuario, Nota = e.Nota, NomeUsuario = e.ItemUsuario.Nome, ComentarioUsuario = e.Comentario, Pedido = "", GoogleId = string.Empty })));
            }
            if (String.IsNullOrEmpty(Tipo) || Tipo == "HE")
            {
                var queryHotelEvento = this.Context.HotelEventos.Where(d => !d.DataExclusao.HasValue).Where(d => d.ItemHotel.IdentificadorViagem == IdentificadorViagem).Where(d => d.DataEntrada.HasValue).Where(d => listaInteiros.Contains(d.IdentificadorUsuario));
                if (Identificador.HasValue)
                    queryHotelEvento = queryHotelEvento.Where(d => d.Identificador == Identificador);

                queryResultado = queryResultado.Union(
    queryHotelEvento.Where(d=>d.DataEntrada != d.ItemHotel.DataEntrada)
    .Select(d => new { Identificador = d.Identificador, Data = d.DataEntrada, Tipo = "HotelEntrada", Latitude = d.LatitudeEntrada, Longitude = d.LongitudeEntrada, Texto = d.ItemHotel.Nome, Url = "", Comentario = "", IdentificadorUsuario = d.IdentificadorUsuario, Nota = new Nullable<int>(), NomeUsuario = d.ItemUsuario.Nome, ComentarioUsuario = "", Pedido = "", GoogleId = string.Empty }));
                queryResultado = queryResultado.Union(
    queryHotelEvento.Where(d => d.DataSaida.HasValue).Where(d=>d.DataSaida != d.ItemHotel.DataSaidia)
    .Select(d => new { Identificador = d.Identificador, Data = d.DataSaida, Tipo = "HotelSaida", Latitude = d.LatitudeSaida, Longitude = d.LongitudeSaida, Texto = d.ItemHotel.Nome, Url = "", Comentario = "", IdentificadorUsuario = d.IdentificadorUsuario, Nota = new Nullable<int>(), NomeUsuario = d.ItemUsuario.Nome, ComentarioUsuario = "", Pedido = "", GoogleId = string.Empty }));

            }
      
            if (String.IsNullOrEmpty(Tipo) || Tipo == "T")
            {
                var queryComentario = this.Context.Comentarios.Where(d => !d.DataExclusao.HasValue).Where(d => d.IdentificadorViagem == IdentificadorViagem).Where(d => listaInteiros.Contains(d.IdentificadorUsuario));
                if (Identificador.HasValue)
                    queryComentario = queryComentario.Where(d => d.Identificador == Identificador);
                queryResultado = queryResultado.Union(
    queryComentario
    .Select(d => new { Identificador = d.Identificador, Data = d.Data, Tipo = "Comentario", Latitude = d.Latitude, Longitude = d.Longitude, Texto = "", Url = "", Comentario = d.Texto, IdentificadorUsuario = d.IdentificadorUsuario, Nota = new Nullable<int>(), NomeUsuario = d.ItemUsuario.Nome, ComentarioUsuario = "", Pedido = "", GoogleId = string.Empty }));
            }
           
            if (String.IsNullOrEmpty(Tipo) || Tipo == "F")
            {
                var queryFoto = this.Context.Fotos.Where(d => !d.DataExclusao.HasValue).Where(d => d.IdentificadorViagem == IdentificadorViagem).Where(d => listaInteiros.Contains(d.IdentificadorUsuario));
                if (Identificador.HasValue)
                    queryFoto = queryFoto.Where(d => d.Identificador == Identificador);
                queryResultado = queryResultado.Union(
    queryFoto
    .Select(d => new { Identificador = d.Identificador, Data = d.Data, Tipo = d.Video.Value ? "Video" : "Foto", Latitude = d.Latitude, Longitude = d.Longitude, Texto = "", Url = d.LinkThumbnail, Comentario = d.Comentario, IdentificadorUsuario = d.IdentificadorUsuario, Nota = new Nullable<int>(), NomeUsuario = d.ItemUsuario.Nome, ComentarioUsuario = "", Pedido = "", GoogleId =  d.CodigoFoto }));


            }
            if (String.IsNullOrEmpty(Tipo) || Tipo == "VA")
            {
                var queryAeroportos = this.Context.ViagemAereaAeroportos.Where(d => d.ItemViagemAerea.IdentificadorViagem == IdentificadorViagem).Where(d => !d.ItemViagemAerea.DataExclusao.HasValue).Where(d => d.DataChegada.HasValue)
              .Where(d => d.ItemViagemAerea.Avaliacoes.Where(e => listaInteiros.Contains(e.IdentificadorUsuario)).Any());
                if (Identificador.HasValue)
                    queryAeroportos = queryAeroportos.Where(d => d.IdentificadorViagemAerea == Identificador);

                queryResultado = queryResultado.Union(
    queryAeroportos.Where(d => !d.DataPartida.HasValue || d.DataPartida > DbFunctions.AddMinutes(d.DataChegada, 5))
    .SelectMany(d => d.ItemViagemAerea.Avaliacoes.Select(e => new
    {
        Identificador = d.Identificador,
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
        Pedido = "",
        GoogleId = string.Empty
    })));


                queryResultado = queryResultado.Union(
        queryAeroportos.Where(d => d.DataPartida.HasValue)
        .SelectMany(d => d.ItemViagemAerea.Avaliacoes.Select(e => new
        {
            Identificador = d.Identificador,
            Data = d.DataPartida,
            Tipo = (d.DataPartida > DbFunctions.AddMinutes(d.DataChegada, 5)) ?
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
            ,
            GoogleId = string.Empty
        })));


            }

            if (DataMaxima.HasValue)
                queryResultado = queryResultado.Where(d => d.Data < DataMaxima);
            else if (DataMinima.HasValue)
                queryResultado = queryResultado.Where(d => d.Data > DataMinima);

            var resultado = queryResultado.GroupBy(
                d => new { d.Comentario, d.Data, d.Latitude, d.Longitude, d.Texto, d.Tipo, d.Url, d.Identificador, d.GoogleId })
                .Select(d => new Timeline()

                {
                    Identificador = d.Key.Identificador,
                    Comentario = d.Key.Comentario,
                    Data = d.Key.Data,
                    Latitude = d.Key.Latitude,
                    Longitude = d.Key.Longitude,
                    Texto = d.Key.Texto,
                    Tipo = d.Key.Tipo,
                    Url = d.Key.Url,
                    GoogleId = d.Key.GoogleId,
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
            var listasemgrupo = queryAtracao.ToList();
            listaSaida = queryAtracao.GroupBy(d => new { CodigoGoogle = (d.CodigoPlace == null) ? string.Empty : d.CodigoPlace,
                Nome = d.Nome })
                .Select(d => new LocaisVisitados()
                {
                    CodigoCoogle = d.Key.CodigoGoogle,
                    Nome = d.Select(e => e.Nome).FirstOrDefault(),                    
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
                    Latitude = d.Where(e => e.Latitude.HasValue && e.Longitude.HasValue && !e.Movel).Select(e => e.Latitude).FirstOrDefault(),
                    Longitude = d.Where(e => e.Latitude.HasValue && e.Longitude.HasValue && !e.Movel).Select(e => e.Longitude).FirstOrDefault(),
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
            return queryAtracao.SelectMany(d => d.Avaliacoes.Where(e => !e.DataExclusao.HasValue).Select(e => new LocaisDetalhes() { Comentario = e.Comentario, DataDe = d.Chegada, DataAte = d.Partida, Nota = e.Nota, NomeUsuario = e.ItemUsuario.Nome, Distancia = d.Distancia })).ToList();
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
            return queryAtracao.SelectMany(d => d.Pedidos.Where(e => !e.DataExclusao.HasValue).Select(e => new LocaisDetalhes() { Comentario = e.Comentario, DataDe = d.Data, DataAte = d.DataTermino, Nota = e.Nota, NomeUsuario = e.ItemUsuario.Nome, Pedido = e.Pedido })).ToList();
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

       

        public List<PontoMapa> ListarPontosViagem(int? IdentificadorViagem, int? IdentificadorUsuario, DateTime? DataDe, DateTime? DataAte, string Tipo)
        {
            IQueryable<PontoMapa> queryFinal = this.Context.Viagemes.Where(d => d.Identificador == -1)
                .Select(d => new PontoMapa() { DataFim = null, DataInicio = null, Latitude = 0, Longitude = 0, Nome = null, Tipo = "P", Url = null, UrlTumbnail = null, GoogleId = null });
            if (string.IsNullOrEmpty(Tipo) || Tipo == "A")
            {
                var queryAtracao = this.Context.AvaliacaoAtracoes.Where(d => d.IdentificadorUsuario == IdentificadorUsuario)
                .Where(d => !d.DataExclusao.HasValue).Where(d => d.ItemAtracao.IdentificadorViagem == IdentificadorViagem).Where(d => d.ItemAtracao.Chegada.HasValue)
                .Where(d => !d.ItemAtracao.DataExclusao.HasValue).Where(d => d.ItemAtracao.Latitude.HasValue && d.ItemAtracao.Longitude.HasValue && (d.ItemAtracao.Longitude != 0 || d.ItemAtracao.Latitude != 0));
                if (DataDe.HasValue)
                    queryAtracao = queryAtracao.Where(d => d.ItemAtracao.Chegada >= DataDe || (d.ItemAtracao.Partida.HasValue && d.ItemAtracao.Partida >= DataDe));
                if (DataAte.HasValue)
                    queryAtracao = queryAtracao.Where(d => d.ItemAtracao.Chegada < DataAte && (!d.ItemAtracao.Partida.HasValue || d.ItemAtracao.Partida < DataAte));
                queryFinal = queryFinal.Union(queryAtracao.Select(d => new PontoMapa() { DataFim = d.ItemAtracao.Partida, DataInicio = d.ItemAtracao.Chegada, Latitude = d.ItemAtracao.Latitude, Longitude = d.ItemAtracao.Longitude, Nome = d.ItemAtracao.Nome, Tipo = "A", Url = null, UrlTumbnail = null, GoogleId = null }));
            }
            if (string.IsNullOrEmpty(Tipo) || Tipo == "H")
            {
                var queryHotel = this.Context.HotelAvaliacoes.Where(d => d.IdentificadorUsuario == IdentificadorUsuario)
                .Where(d => !d.DataExclusao.HasValue).Where(d => d.ItemHotel.IdentificadorViagem == IdentificadorViagem).Where(d => d.ItemHotel.DataEntrada.HasValue)
                .Where(d => !d.ItemHotel.DataExclusao.HasValue).Where(d => d.ItemHotel.Latitude.HasValue && d.ItemHotel.Longitude.HasValue && (d.ItemHotel.Longitude != 0 || d.ItemHotel.Latitude != 0));
                if (DataDe.HasValue)
                    queryHotel = queryHotel.Where(d => d.ItemHotel.DataEntrada >= DataDe || (d.ItemHotel.DataSaidia.HasValue && d.ItemHotel.DataSaidia >= DataDe));
                if (DataAte.HasValue)
                    queryHotel = queryHotel.Where(d => d.ItemHotel.DataEntrada < DataAte && (!d.ItemHotel.DataSaidia.HasValue || d.ItemHotel.DataSaidia < DataAte));
                queryFinal = queryFinal.Union(queryHotel.Select(d => new PontoMapa() { DataFim = d.ItemHotel.DataSaidia, DataInicio = d.ItemHotel.DataEntrada, Latitude = d.ItemHotel.Latitude, Longitude = d.ItemHotel.Longitude, Nome = d.ItemHotel.Nome, Tipo = "H", Url = null, UrlTumbnail = null, GoogleId = null }));
            }
            if (string.IsNullOrEmpty(Tipo) || Tipo == "R")
            {
                var queryRefeicao = this.Context.RefeicaoPedidos.Where(d => d.IdentificadorUsuario == IdentificadorUsuario)
                .Where(d => !d.DataExclusao.HasValue).Where(d => d.ItemRefeicao.IdentificadorViagem == IdentificadorViagem)
                .Where(d => !d.ItemRefeicao.DataExclusao.HasValue).Where(d => d.ItemRefeicao.Latitude.HasValue && d.ItemRefeicao.Longitude.HasValue && (d.ItemRefeicao.Longitude != 0 || d.ItemRefeicao.Latitude != 0));
                if (DataDe.HasValue)
                    queryRefeicao = queryRefeicao.Where(d => d.ItemRefeicao.Data >= DataDe);
                if (DataAte.HasValue)
                    queryRefeicao = queryRefeicao.Where(d => d.ItemRefeicao.Data < DataAte);
                queryFinal = queryFinal.Union(queryRefeicao.Select(d => new PontoMapa() { DataFim = null, DataInicio = d.ItemRefeicao.Data, Latitude = d.ItemRefeicao.Latitude, Longitude = d.ItemRefeicao.Longitude, Nome = d.ItemRefeicao.Nome, Tipo = "R", Url = null, UrlTumbnail = null, GoogleId = null }));
            }
           
            if (string.IsNullOrEmpty(Tipo) || Tipo == "P")
            {
                var queryRefeicao = this.Context.AvaliacaoAereas.Where(d => d.IdentificadorUsuario == IdentificadorUsuario)
                .Where(d => !d.DataExclusao.HasValue).Where(d => d.ItemViagemAerea.IdentificadorViagem == IdentificadorViagem).Where(d => !d.ItemViagemAerea.DataExclusao.HasValue)
                .SelectMany(d => d.ItemViagemAerea.Aeroportos).Where(d => !d.DataExclusao.HasValue)
                .Where(d => d.Latitude.HasValue && d.Longitude.HasValue && (d.Longitude != 0 || d.Latitude != 0));
                if (DataDe.HasValue)
                    queryRefeicao = queryRefeicao.Where(d => d.DataChegada >= DataDe || (d.DataPartida.HasValue && d.DataPartida >= DataDe));
                if (DataAte.HasValue)
                    queryRefeicao = queryRefeicao.Where(d => d.DataChegada < DataAte && (!d.DataPartida.HasValue || d.DataPartida < DataAte));
                queryFinal = queryFinal.Union(queryRefeicao.Select(d => new PontoMapa() { DataFim = d.DataPartida, DataInicio = d.DataChegada, Latitude = d.Latitude, Longitude = d.Longitude, Nome = d.Aeroporto, Tipo = "P", Url = null, UrlTumbnail = null, GoogleId = null }));
            }
            //if (string.IsNullOrEmpty(Tipo) || Tipo == "G")
            //{
            //    var queryRefeicao = this.Context.Gastos.Where(d => d.IdentificadorUsuario == IdentificadorUsuario)
            //    .Where(d => !d.DataExclusao.HasValue)
            //    .Where(d=> !d.Reabastecimentos.Any() && !d.ApenasBaixa.Value && !d.Compras.Any() && !d.Refeicoes.Any() && !d.Alugueis.Any() && !d.Hoteis.Any() && !d.ViagenAereas.Any() && !d.Atracoes.Any())
            //    .Where(d => d.Latitude.HasValue && d.Longitude.HasValue && (d.Longitude != 0 || d.Latitude != 0));
            //    if (DataDe.HasValue)
            //        queryRefeicao = queryRefeicao.Where(d => d.Data >= DataDe);
            //    if (DataAte.HasValue)
            //        queryRefeicao = queryRefeicao.Where(d => d.Data < DataAte);
            //    queryFinal = queryFinal.Union(queryRefeicao.Select(d => new PontoMapa() { DataFim = null, DataInicio = d.Data, Latitude = d.Latitude, Longitude = d.Longitude, Nome = d.Descricao, Tipo = "G", Url = null, UrlTumbnail = null }));
            //}
            if (string.IsNullOrEmpty(Tipo) || Tipo == "T")
            {
                var queryRefeicao = this.Context.Comentarios.Where(d => d.IdentificadorUsuario == IdentificadorUsuario)
                .Where(d => !d.DataExclusao.HasValue)
                 .Where(d => d.IdentificadorViagem == IdentificadorViagem)

                .Where(d => d.Latitude.HasValue && d.Longitude.HasValue && (d.Longitude != 0 || d.Latitude != 0));
                if (DataDe.HasValue)
                    queryRefeicao = queryRefeicao.Where(d => d.Data >= DataDe);
                if (DataAte.HasValue)
                    queryRefeicao = queryRefeicao.Where(d => d.Data < DataAte);
                queryFinal = queryFinal.Union(queryRefeicao.Select(d => new PontoMapa() { DataFim = null, DataInicio = d.Data, Latitude = d.Latitude, Longitude = d.Longitude, Nome = d.Texto, Tipo = "T", Url = null, UrlTumbnail = null, GoogleId = null }));
            }
            if (string.IsNullOrEmpty(Tipo) || Tipo == "F" || Tipo == "V")
            {
                var queryRefeicao = this.Context.Fotos.Where(d => d.IdentificadorUsuario == IdentificadorUsuario)
                .Where(d => !d.DataExclusao.HasValue)
                .Where(d => d.IdentificadorViagem == IdentificadorViagem)
                .Where(d => d.Latitude.HasValue && d.Longitude.HasValue && (d.Longitude != 0 || d.Latitude != 0));
                if (!string.IsNullOrEmpty(Tipo))
                    queryRefeicao = queryRefeicao.Where(d => d.Video == (Tipo == "V"));
                if (DataDe.HasValue)
                    queryRefeicao = queryRefeicao.Where(d => d.Data >= DataDe);
                if (DataAte.HasValue)
                    queryRefeicao = queryRefeicao.Where(d => d.Data < DataAte);
                queryFinal = queryFinal.Union(queryRefeicao.Select(d => new PontoMapa() { DataFim = null, DataInicio = d.Data, Latitude = d.Latitude, Longitude = d.Longitude, Nome = d.Comentario, Tipo = d.Video.Value ? "V" : "F", Url = d.LinkFoto, UrlTumbnail = d.LinkThumbnail, GoogleId = d.Video.Value ? null : d.CodigoFoto }));
            }
            if (string.IsNullOrEmpty(Tipo) || Tipo == "U")
            {
                var queryUltimoPonto = this.Context.Posicoes.Where(d => d.IdentificadorUsuario == IdentificadorUsuario)
                                    .Where(d => d.IdentificadorViagem == IdentificadorViagem)

                                    .Where(d => d.Latitude.HasValue && d.Longitude.HasValue && (d.Longitude != 0 || d.Latitude != 0));
                if (DataDe.HasValue)
                    queryUltimoPonto = queryUltimoPonto.Where(d => d.DataLocal >= DataDe);
                if (DataAte.HasValue)
                    queryUltimoPonto = queryUltimoPonto.Where(d => d.DataLocal < DataAte);
                queryUltimoPonto = queryUltimoPonto.OrderByDescending(d => d.DataLocal).Take(1);
                queryFinal = queryFinal.Union(queryUltimoPonto.Select(d => new PontoMapa() { DataFim = null, DataInicio = d.DataGMT, Latitude = d.Latitude, Longitude = d.Longitude, Nome = "Última Posição", Tipo = "U", Url = null, UrlTumbnail = null, GoogleId = null }));

            }

            return queryFinal.ToList();
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


        public List<IntervaloDeslocamento> CarregarResumoAtracao(ResumoViagem itemResumo, int? IdentificadorViagem, int? IdentificadorUsuario, DateTime? DataDe, DateTime? DataAte)
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
                .Sum(d => DbFunctions.DiffMinutes(d.ItemAtracao.Chegada, d.ItemAtracao.Partida)).GetValueOrDefault());
            itemResumo.NotaMediaAtracao = queryAtracao.Where(d => d.Nota.HasValue).Average(d => d.Nota);

            itemResumo.KmAtracoes = queryAtracao.Sum(d => d.ItemAtracao.Distancia);

            return queryAtracao.Select(d => new IntervaloDeslocamento() { Chegada = d.ItemAtracao.Chegada, Partida = d.ItemAtracao.Partida, Tipo = "A" }).ToList();
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
                .Sum(d => DbFunctions.DiffMinutes(d.DataEntrada, d.DataSaida)).GetValueOrDefault());

            itemResumo.NotaMediaHotel = queryHotel.Where(d => d.Nota.HasValue).Average(d => d.Nota);

           
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

                .Select(e => DbFunctions.DiffMinutes(e.DataChegada, e.DataPartida)).Sum().GetValueOrDefault());

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

            itemResumo.KmDeslocamento = queryViagemAerea.Sum(d => d.ItemViagemAerea.Distancia);

            ListaSaida = ListaIntervalo.Select(d => new IntervaloDeslocamento() { Tipo = "V", Chegada = d.DataChegada, Partida = d.DataPartida }).ToList();
            return ListaSaida;
        }

  
        public List<Posicao> ListarPosicao(int? IdentificadorViagem, int? IdentificadorUsuario, DateTime? DataDe, DateTime? DataAte)
        {
            var query = this.Context.Posicoes.Where(d => d.IdentificadorViagem == IdentificadorViagem).Where(d => d.IdentificadorUsuario == IdentificadorUsuario);
            if (DataDe.HasValue)
                query = query.Where(d => d.DataLocal >= DataDe);
            if (DataAte.HasValue)
                query = query.Where(d => d.DataLocal < DataAte);
            return query.OrderBy(d => d.DataLocal).AsNoTracking().ToList();
        }

        public List<ConsultaRankings> ListarRankings(int? IdentificadorViagem, int? IdentificadorUsuario, bool ApenasAmigos, int? IdentificadorAmigo, string Tipo, int? NumeroRegistros, string CodigoGoogle, string Nome)
        {
            List<ConsultaRankings> lista = new List<ConsultaRankings>();
            var queryUsuarios = this.Context.ParticipanteViagemes.Where(d => d.IdentificadorUsuario == IdentificadorUsuario || this.Context.Amigos.Where(e => e.IdentificadorAmigo == IdentificadorUsuario && e.IdentificadorUsuario == d.IdentificadorUsuario).Any());
            if (IdentificadorAmigo.HasValue)
                queryUsuarios = queryUsuarios.Where(d => d.IdentificadorUsuario == IdentificadorAmigo);
            var listaInteiros = queryUsuarios.Select(d => d.IdentificadorUsuario);
            bool Detalhe = !string.IsNullOrEmpty(CodigoGoogle) || !string.IsNullOrEmpty(Nome);
            if (string.IsNullOrEmpty(Tipo) || Tipo == "A")
            {
                var queryAtracao = this.Context.AvaliacaoAtracoes.Where(d => !d.DataExclusao.HasValue).Where(d => d.Nota.HasValue).Where(d => !d.ItemAtracao.DataExclusao.HasValue);
                if (ApenasAmigos || IdentificadorAmigo.HasValue)
                    queryAtracao = queryAtracao.Where(d => listaInteiros.Contains(d.IdentificadorUsuario));
                if (!string.IsNullOrWhiteSpace(CodigoGoogle))
                {
                    queryAtracao = queryAtracao.Where(e => e.ItemAtracao.CodigoPlace == CodigoGoogle);
                }
                else if (!string.IsNullOrEmpty(Nome))
                {
                    queryAtracao = queryAtracao.Where(e => e.ItemAtracao.CodigoPlace == null || e.ItemAtracao.CodigoPlace == string.Empty).Where(e => e.ItemAtracao.Nome == Nome);
                }
                if (IdentificadorViagem.HasValue)
                    queryAtracao = queryAtracao.Where(e => e.ItemAtracao.IdentificadorViagem == IdentificadorViagem);

                var listaSaida = queryAtracao.GroupBy(d => new { CodigoGoogle = (d.ItemAtracao.CodigoPlace == null) ? string.Empty : d.ItemAtracao.CodigoPlace, Nome = d.ItemAtracao.CodigoPlace != null && d.ItemAtracao.CodigoPlace != string.Empty ? string.Empty : d.ItemAtracao.Nome })
                    .Select(d => new ConsultaRankings()
                    {
                        CodigoGoogle = d.Key.CodigoGoogle,
                        Nome = d.Select(e => e.ItemAtracao.Nome).FirstOrDefault(),
                        Tipo = "A",
                        Media = d.Average(e => e.Nota),
                        //Avaliacoes = Detalhe ? d.Select(e => new UsuarioConsulta() { Comentario = e.Comentario, Nome = e.ItemUsuario.Nome, DataAvaliacao = e.DataAtualizacao, Nota = e.Nota }) : null,
                        NumeroAvaliacoes = d.Count()

                    });
                listaSaida = listaSaida.OrderByDescending(d => d.Media);
                if (NumeroRegistros.HasValue)
                    listaSaida = listaSaida.Take(NumeroRegistros.Value);
                lista.AddRange(listaSaida.ToList());
            }
            if (string.IsNullOrEmpty(Tipo) || Tipo == "H")
            {
                var queryAtracao = this.Context.HotelAvaliacoes.Where(d => !d.DataExclusao.HasValue).Where(d => d.Nota.HasValue).Where(d => !d.ItemHotel.DataExclusao.HasValue);
                if (ApenasAmigos || IdentificadorAmigo.HasValue)
                    queryAtracao = queryAtracao.Where(d => listaInteiros.Contains(d.IdentificadorUsuario));
                if (!string.IsNullOrWhiteSpace(CodigoGoogle))
                {
                    queryAtracao = queryAtracao.Where(e => e.ItemHotel.CodigoPlace == CodigoGoogle);
                }
                else if (!string.IsNullOrEmpty(Nome))
                {
                    queryAtracao = queryAtracao.Where(e => e.ItemHotel.CodigoPlace == null || e.ItemHotel.CodigoPlace == string.Empty).Where(e => e.ItemHotel.Nome == Nome);
                }
                if (IdentificadorViagem.HasValue)
                    queryAtracao = queryAtracao.Where(e => e.ItemHotel.IdentificadorViagem == IdentificadorViagem);

                var listaSaida = queryAtracao.GroupBy(d => new { CodigoGoogle = (d.ItemHotel.CodigoPlace == null) ? string.Empty : d.ItemHotel.CodigoPlace, Nome = d.ItemHotel.CodigoPlace != null && d.ItemHotel.CodigoPlace != string.Empty ? string.Empty : d.ItemHotel.Nome })
                    .Select(d => new ConsultaRankings()
                    {
                        CodigoGoogle = d.Key.CodigoGoogle,
                        Nome = d.Select(e => e.ItemHotel.Nome).FirstOrDefault(),
                        Tipo = "H",
                        Media = d.Average(e => e.Nota),
                        //Avaliacoes = Detalhe ? d.Select(e => new UsuarioConsulta() { Comentario = e.Comentario, Nome = e.ItemUsuario.Nome, DataAvaliacao = e.DataAtualizacao, Nota = e.Nota }) : null,
                        NumeroAvaliacoes = d.Count()

                    });
                listaSaida = listaSaida.OrderByDescending(d => d.Media);
                if (NumeroRegistros.HasValue)
                    listaSaida = listaSaida.Take(NumeroRegistros.Value);
                lista.AddRange(listaSaida.ToList());
            }
           
            if (string.IsNullOrEmpty(Tipo) || Tipo == "R")
            {
                var queryAtracao = this.Context.RefeicaoPedidos.Where(d => !d.DataExclusao.HasValue).Where(d => d.Nota.HasValue).Where(d => !d.ItemRefeicao.DataExclusao.HasValue);
                if (ApenasAmigos || IdentificadorAmigo.HasValue)
                    queryAtracao = queryAtracao.Where(d => listaInteiros.Contains(d.IdentificadorUsuario));
                if (!string.IsNullOrWhiteSpace(CodigoGoogle))
                {
                    queryAtracao = queryAtracao.Where(e => e.ItemRefeicao.CodigoPlace == CodigoGoogle);
                }
                else if (!string.IsNullOrEmpty(Nome))
                {
                    queryAtracao = queryAtracao.Where(e => e.ItemRefeicao.CodigoPlace == null || e.ItemRefeicao.CodigoPlace == string.Empty).Where(e => e.ItemRefeicao.Nome == Nome);
                }
                if (IdentificadorViagem.HasValue)
                    queryAtracao = queryAtracao.Where(e => e.ItemRefeicao.IdentificadorViagem == IdentificadorViagem);

                var listaSaida = queryAtracao.GroupBy(d => new { CodigoGoogle = (d.ItemRefeicao.CodigoPlace == null) ? string.Empty : d.ItemRefeicao.CodigoPlace, Nome = d.ItemRefeicao.CodigoPlace != null && d.ItemRefeicao.CodigoPlace != string.Empty ? string.Empty : d.ItemRefeicao.Nome })
                    .Select(d => new ConsultaRankings()
                    {
                        CodigoGoogle = d.Key.CodigoGoogle,
                        Nome = d.Select(e => e.ItemRefeicao.Nome).FirstOrDefault(),
                        Tipo = "R",
                        Media = d.Average(e => e.Nota),
                        //Avaliacoes = Detalhe ? d.Select(e => new UsuarioConsulta() { Comentario = e.Comentario, Nome = e.ItemUsuario.Nome, DataAvaliacao = e.DataAtualizacao, Nota = e.Nota }) : null,
                        NumeroAvaliacoes = d.Count()

                    });
                listaSaida = listaSaida.OrderByDescending(d => d.Media);
                if (NumeroRegistros.HasValue)
                    listaSaida = listaSaida.Take(NumeroRegistros.Value);
                lista.AddRange(listaSaida.ToList());
            }

            if (string.IsNullOrEmpty(Tipo) || Tipo == "VA")
            {
                var queryAtracao = this.Context.AvaliacaoAereas.Where(d => !d.DataExclusao.HasValue).Where(d => d.Nota.HasValue).Where(d => !d.ItemViagemAerea.DataExclusao.HasValue);
                if (ApenasAmigos || IdentificadorAmigo.HasValue)
                    queryAtracao = queryAtracao.Where(d => listaInteiros.Contains(d.IdentificadorUsuario));
                if (!string.IsNullOrEmpty(Nome))
                {
                    queryAtracao = queryAtracao.Where(e => e.ItemViagemAerea.CompanhiaAerea == Nome);
                }
                if (IdentificadorViagem.HasValue)
                    queryAtracao = queryAtracao.Where(e => e.ItemViagemAerea.IdentificadorViagem == IdentificadorViagem);

                var listaSaida = queryAtracao.GroupBy(d => new { CodigoGoogle = string.Empty, Nome = d.ItemViagemAerea.CompanhiaAerea })
                    .Select(d => new ConsultaRankings()
                    {
                        CodigoGoogle = d.Key.CodigoGoogle,
                        Nome = d.Key.Nome,
                        Tipo = "VA",
                        Media = d.Average(e => e.Nota),
                        //Avaliacoes = Detalhe ? d.Select(e => new UsuarioConsulta() { Comentario = e.Comentario, Nome = e.ItemUsuario.Nome, DataAvaliacao = e.DataAtualizacao, Nota = e.Nota }) : null,
                        NumeroAvaliacoes = d.Count()

                    });
                listaSaida = listaSaida.OrderByDescending(d => d.Media);
                if (NumeroRegistros.HasValue)
                    listaSaida = listaSaida.Take(NumeroRegistros.Value);
                lista.AddRange(listaSaida.ToList());
            }
            lista = lista.OrderByDescending(d => d.Media).ToList();
            if (NumeroRegistros.HasValue)
                lista = lista.Take(NumeroRegistros.Value).ToList();
            return lista;
        }

        public List<UsuarioConsulta> ListarAvaliacoesRankings(int? IdentificadorViagem, int? IdentificadorUsuario, bool ApenasAmigos, int? IdentificadorAmigo, string Tipo, string CodigoGoogle, string Nome)
        {
            List<UsuarioConsulta> lista = new List<UsuarioConsulta>();
            var queryUsuarios = this.Context.ParticipanteViagemes.Where(d => d.IdentificadorUsuario == IdentificadorUsuario || this.Context.Amigos.Where(e => e.IdentificadorAmigo == IdentificadorUsuario && e.IdentificadorUsuario == d.IdentificadorUsuario).Any());
            if (IdentificadorAmigo.HasValue)
                queryUsuarios = queryUsuarios.Where(d => d.IdentificadorUsuario == IdentificadorAmigo);
            var listaInteiros = queryUsuarios.Select(d => d.IdentificadorUsuario);
            if (string.IsNullOrEmpty(Tipo) || Tipo == "A")
            {
                var queryAtracao = this.Context.AvaliacaoAtracoes.Where(d => !d.DataExclusao.HasValue).Where(d => d.Nota.HasValue).Where(d => !d.ItemAtracao.DataExclusao.HasValue);
                if (ApenasAmigos || IdentificadorAmigo.HasValue)
                    queryAtracao = queryAtracao.Where(d => listaInteiros.Contains(d.IdentificadorUsuario));
                if (!string.IsNullOrWhiteSpace(CodigoGoogle))
                {
                    queryAtracao = queryAtracao.Where(e => e.ItemAtracao.CodigoPlace == CodigoGoogle);
                }
                else if (!string.IsNullOrEmpty(Nome))
                {
                    queryAtracao = queryAtracao.Where(e => e.ItemAtracao.CodigoPlace == null || e.ItemAtracao.CodigoPlace == string.Empty).Where(e => e.ItemAtracao.Nome == Nome);
                }
                if (IdentificadorViagem.HasValue)
                    queryAtracao = queryAtracao.Where(e => e.ItemAtracao.IdentificadorViagem == IdentificadorViagem);

                var listaSaida = queryAtracao.GroupBy(d => new { CodigoGoogle = (d.ItemAtracao.CodigoPlace == null) ? string.Empty : d.ItemAtracao.CodigoPlace, Nome = d.ItemAtracao.CodigoPlace != null && d.ItemAtracao.CodigoPlace != string.Empty ? string.Empty : d.ItemAtracao.Nome })
                    .Select(d => new ConsultaRankings()
                    {
                        CodigoGoogle = d.Key.CodigoGoogle,
                        Nome = d.Select(e => e.ItemAtracao.Nome).FirstOrDefault(),
                        Tipo = "A",
                        Media = d.Average(e => e.Nota),
                        Avaliacoes = d.Select(e => new UsuarioConsulta() { Comentario = e.Comentario, Nome = e.ItemUsuario.Nome, DataAvaliacao = e.DataAtualizacao, Nota = e.Nota }),
                        NumeroAvaliacoes = d.Count()

                    }).SelectMany(d => d.Avaliacoes).OrderByDescending(d => d.DataAvaliacao);

                lista.AddRange(listaSaida.ToList());
            }
            if (string.IsNullOrEmpty(Tipo) || Tipo == "H")
            {
                var queryAtracao = this.Context.HotelAvaliacoes.Where(d => !d.DataExclusao.HasValue).Where(d => d.Nota.HasValue).Where(d => !d.ItemHotel.DataExclusao.HasValue);
                if (ApenasAmigos || IdentificadorAmigo.HasValue)
                    queryAtracao = queryAtracao.Where(d => listaInteiros.Contains(d.IdentificadorUsuario));
                if (!string.IsNullOrWhiteSpace(CodigoGoogle))
                {
                    queryAtracao = queryAtracao.Where(e => e.ItemHotel.CodigoPlace == CodigoGoogle);
                }
                else if (!string.IsNullOrEmpty(Nome))
                {
                    queryAtracao = queryAtracao.Where(e => e.ItemHotel.CodigoPlace == null || e.ItemHotel.CodigoPlace == string.Empty).Where(e => e.ItemHotel.Nome == Nome);
                }
                if (IdentificadorViagem.HasValue)
                    queryAtracao = queryAtracao.Where(e => e.ItemHotel.IdentificadorViagem == IdentificadorViagem);

                var listaSaida = queryAtracao.GroupBy(d => new { CodigoGoogle = (d.ItemHotel.CodigoPlace == null) ? string.Empty : d.ItemHotel.CodigoPlace, Nome = d.ItemHotel.CodigoPlace != null && d.ItemHotel.CodigoPlace != string.Empty ? string.Empty : d.ItemHotel.Nome })
                    .Select(d => new ConsultaRankings()
                    {
                        CodigoGoogle = d.Key.CodigoGoogle,
                        Nome = d.Select(e => e.ItemHotel.Nome).FirstOrDefault(),
                        Tipo = "H",
                        Media = d.Average(e => e.Nota),
                        Avaliacoes = d.Select(e => new UsuarioConsulta() { Comentario = e.Comentario, Nome = e.ItemUsuario.Nome, DataAvaliacao = e.DataAtualizacao, Nota = e.Nota }),
                        NumeroAvaliacoes = d.Count()

                    }).SelectMany(d => d.Avaliacoes).OrderByDescending(d => d.DataAvaliacao);
                lista.AddRange(listaSaida.ToList());
            }
        
            if (string.IsNullOrEmpty(Tipo) || Tipo == "R")
            {
                var queryAtracao = this.Context.RefeicaoPedidos.Where(d => !d.DataExclusao.HasValue).Where(d => d.Nota.HasValue).Where(d => !d.ItemRefeicao.DataExclusao.HasValue);
                if (ApenasAmigos || IdentificadorAmigo.HasValue)
                    queryAtracao = queryAtracao.Where(d => listaInteiros.Contains(d.IdentificadorUsuario));
                if (!string.IsNullOrWhiteSpace(CodigoGoogle))
                {
                    queryAtracao = queryAtracao.Where(e => e.ItemRefeicao.CodigoPlace == CodigoGoogle);
                }
                else if (!string.IsNullOrEmpty(Nome))
                {
                    queryAtracao = queryAtracao.Where(e => e.ItemRefeicao.CodigoPlace == null || e.ItemRefeicao.CodigoPlace == string.Empty).Where(e => e.ItemRefeicao.Nome == Nome);
                }
                if (IdentificadorViagem.HasValue)
                    queryAtracao = queryAtracao.Where(e => e.ItemRefeicao.IdentificadorViagem == IdentificadorViagem);

                var listaSaida = queryAtracao.GroupBy(d => new { CodigoGoogle = (d.ItemRefeicao.CodigoPlace == null) ? string.Empty : d.ItemRefeicao.CodigoPlace, Nome = d.ItemRefeicao.CodigoPlace != null && d.ItemRefeicao.CodigoPlace != string.Empty ? string.Empty : d.ItemRefeicao.Nome })
                    .Select(d => new ConsultaRankings()
                    {
                        CodigoGoogle = d.Key.CodigoGoogle,
                        Nome = d.Select(e => e.ItemRefeicao.Nome).FirstOrDefault(),
                        Tipo = "R",
                        Media = d.Average(e => e.Nota),
                        Avaliacoes = d.Select(e => new UsuarioConsulta() { Comentario = e.Comentario, Nome = e.ItemUsuario.Nome, DataAvaliacao = e.DataAtualizacao, Nota = e.Nota }),
                        NumeroAvaliacoes = d.Count()

                    }).SelectMany(d => d.Avaliacoes).OrderByDescending(d => d.DataAvaliacao);
                lista.AddRange(listaSaida.ToList());
            }

            if (string.IsNullOrEmpty(Tipo) || Tipo == "VA")
            {
                var queryAtracao = this.Context.AvaliacaoAereas.Where(d => !d.DataExclusao.HasValue).Where(d => d.Nota.HasValue).Where(d => !d.ItemViagemAerea.DataExclusao.HasValue);
                if (ApenasAmigos || IdentificadorAmigo.HasValue)
                    queryAtracao = queryAtracao.Where(d => listaInteiros.Contains(d.IdentificadorUsuario));
                if (!string.IsNullOrEmpty(Nome))
                {
                    queryAtracao = queryAtracao.Where(e => e.ItemViagemAerea.CompanhiaAerea == Nome);
                }
                if (IdentificadorViagem.HasValue)
                    queryAtracao = queryAtracao.Where(e => e.ItemViagemAerea.IdentificadorViagem == IdentificadorViagem);

                var listaSaida = queryAtracao.GroupBy(d => new { CodigoGoogle = string.Empty, Nome = d.ItemViagemAerea.CompanhiaAerea })
                    .Select(d => new ConsultaRankings()
                    {
                        CodigoGoogle = d.Key.CodigoGoogle,
                        Nome = d.Key.Nome,
                        Tipo = "VA",
                        Media = d.Average(e => e.Nota),
                        Avaliacoes = d.Select(e => new UsuarioConsulta() { Comentario = e.Comentario, Nome = e.ItemUsuario.Nome, DataAvaliacao = e.DataAtualizacao, Nota = e.Nota }),
                        NumeroAvaliacoes = d.Count()

                    }).SelectMany(d => d.Avaliacoes).OrderByDescending(d => d.DataAvaliacao);
                lista.AddRange(listaSaida.ToList());
            }

            return lista;
        }

    }
}
