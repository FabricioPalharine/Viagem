using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CV.Model;
using System.Linq.Expressions;

namespace CV.Data
{
    public partial class ViagemRepository : RepositoryBase
    {
        public List<Usuario> ListarUsuario(Expression<Func<Usuario, bool>> predicate)
        {
            return Context.Usuarios.Where(predicate).ToList();
        }

        public List<RequisicaoAmizade> ListarRequisicaoAmizade(Expression<Func<RequisicaoAmizade, bool>> predicate)
        {
            return Context.RequisicaoAmizades.Include("ItemUsuario").Where(predicate).ToList();
        }

        public List<ParticipanteViagem> ListarParticipanteViagem(Expression<Func<ParticipanteViagem, bool>> predicate)
        {
            return Context.ParticipanteViagemes.Where(predicate).ToList();
        }

        public List<Amigo> ListarAmigo(Expression<Func<Amigo, bool>> predicate)
        {
            return Context.Amigos.Where(predicate).ToList();
        }


        public List<UsuarioGasto> ListarUsuarioGasto(Expression<Func<UsuarioGasto, bool>> predicate)
        {
            return Context.UsuarioGastos.Where(predicate).ToList();
        }

        public List<Pais> ListarPais(Expression<Func<Pais, bool>> predicate)
        {
            return Context.Paises.Where(predicate).ToList();
        }

        public List<Cidade> ListarCidade(Expression<Func<Cidade, bool>> predicate)
        {
            return Context.Cidades.Where(predicate).ToList();
        }

        public List<Viagem> ListarViagem(int? IdentificadorUsuario, bool Ativa)
        {
            IQueryable<Viagem> query = this.Context.Viagemes;
            if (Ativa)
                query = query.Where(d => d.Aberto.Value);
            query = query.Where(d => d.IdentificadorUsuario == IdentificadorUsuario || d.Participantes.Where(e => e.IdentificadorUsuario == IdentificadorUsuario).Any()
            || this.Context.Amigos.Where(e => e.IdentificadorAmigo == IdentificadorUsuario).Where(e => e.IdentificadorUsuario == d.IdentificadorUsuario).Any()
            || d.Participantes.Where(f => this.Context.Amigos.Where(e => e.IdentificadorAmigo == IdentificadorUsuario).Where(e => e.IdentificadorUsuario == f.IdentificadorUsuario).Any()).Any());

            query = query.OrderBy(d => d.IdentificadorUsuario == IdentificadorUsuario || d.Participantes.Where(e => e.IdentificadorUsuario == IdentificadorUsuario).Any() ? 0 : 1);
            return query.ToList();
        }

        public Viagem SelecionarViagem_IdentificadorUsuario_IdentficadorViagem(int? IdentificadorUsuario, int? IdentificadorViagem)
        {
            IQueryable<Viagem> query = this.Context.Viagemes;

            query = query.Where(d => d.Identificador == IdentificadorViagem);
            query = query.Where(d => d.IdentificadorUsuario == IdentificadorUsuario || d.Participantes.Where(e => e.IdentificadorUsuario == IdentificadorUsuario).Any()
            || this.Context.Amigos.Where(e => e.IdentificadorAmigo == IdentificadorUsuario).Where(e => e.IdentificadorUsuario == d.IdentificadorUsuario).Any()
            || d.Participantes.Where(f => this.Context.Amigos.Where(e => e.IdentificadorAmigo == IdentificadorUsuario).Where(e => e.IdentificadorUsuario == f.IdentificadorUsuario).Any()).Any());

            return query.FirstOrDefault();
        }

        public void SalvarUsuario(Usuario itemGravar, List<RequisicaoAmizade> requisicoes, List<ParticipanteViagem> participantes, List<Amigo> amigos)
        {
            Usuario itemBase = Context.Usuarios
            .Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            if (itemBase == null)
            {
                itemBase = Context.Usuarios.Create();
                itemBase.Identificador = itemGravar.Identificador;
                Context.Entry<Usuario>(itemBase).State = System.Data.Entity.EntityState.Added;
            }
            AtualizarPropriedades<Usuario>(itemBase, itemGravar);
            foreach (var itemRequisicao in requisicoes)
            {
                var itemRequisicaoBase = Context.RequisicaoAmizades.Where(d => d.Identificador == itemRequisicao.Identificador).FirstOrDefault();
                itemRequisicaoBase.ItemUsuarioRequisitado = itemBase;
            }
            foreach (var itemRequisicao in amigos)
            {
                var itemRequisicaoBase = Context.Amigos.Where(d => d.Identificador == itemRequisicao.Identificador).FirstOrDefault();
                itemRequisicaoBase.ItemAmigo = itemBase;
            }
            foreach (var itemRequisicao in participantes)
            {
                var itemRequisicaoBase = Context.ParticipanteViagemes.Where(d => d.Identificador == itemRequisicao.Identificador).FirstOrDefault();
                itemRequisicaoBase.ItemUsuario = itemBase;
            }
            Context.SaveChanges();
            itemGravar.Identificador = itemBase.Identificador;
        }

        public List<ConsultaAmigo> ListarConsultaAmigo(int IdentificadorUsuario)
        {
            IQueryable<Amigo> querySeguidor = this.Context.Amigos.Where(d => d.IdentificadorUsuario == IdentificadorUsuario);
            IQueryable<Amigo> querySeguidos = this.Context.Amigos.Where(d => d.IdentificadorAmigo == IdentificadorUsuario);
            var ListaGrupo = querySeguidor.Select(d => new { d.IdentificadorAmigo, EMail = d.IdentificadorAmigo.HasValue ? d.ItemAmigo.EMail : d.EMail, Seguidor = true })
                .Concat(querySeguidos.Select(d => new { IdentificadorAmigo = d.IdentificadorUsuario, d.ItemUsuario.EMail, Seguidor = false }))
                .GroupBy(d => new { d.IdentificadorAmigo, d.EMail })
                .Select(d => new { d.Key.IdentificadorAmigo, d.Key.EMail, Seguidor = d.Where(e => e.Seguidor).Any(), Seguido = d.Where(e => !e.Seguidor).Any() });
            return ListaGrupo.GroupJoin(this.Context.Usuarios, d => d.IdentificadorAmigo, d => d.Identificador, (a, u) => new ConsultaAmigo() { IdentificadorUsuario = a.IdentificadorAmigo, EMail = a.EMail, Nome = u.Select(e => e.Nome).FirstOrDefault(), Seguido = a.Seguido, Seguidor = a.Seguidor }).ToList();
        }

        public void SalvarRequisicaoAmizade(RequisicaoAmizade itemGravar, Amigo itemAmigo)
        {
            RequisicaoAmizade itemBase = Context.RequisicaoAmizades
            .Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            if (itemBase == null)
            {
                itemBase = Context.RequisicaoAmizades.Create();
                Context.Entry<RequisicaoAmizade>(itemBase).State = System.Data.Entity.EntityState.Added;
            }
            AtualizarPropriedades<RequisicaoAmizade>(itemBase, itemGravar);
            if (itemAmigo != null)
            {
                Context.Amigos.Add(itemAmigo);
            }
            Context.SaveChanges();
            itemGravar.Identificador = itemBase.Identificador;
        }

        public List<Usuario> ListarUsuario_EMail(string EMail)
        {
            IQueryable<Usuario> Query = this.Context.Usuarios;
            if (!string.IsNullOrEmpty(EMail))
                Query = Query.Where(d => d.EMail == EMail);
            return Query.ToList();
        }

        public List<Usuario> ListarUsuarioAmigo(int identificadorUsuario)
        {
            IQueryable<Usuario> Query = this.Context.Usuarios;
            IQueryable<Amigo> QueryAmigo = this.Context.Amigos.Where(d => d.IdentificadorUsuario == identificadorUsuario);
            Query = Query.Where(d => QueryAmigo.Where(e => e.IdentificadorAmigo == d.Identificador).Any());
            return Query.ToList();
        }

        public List<Viagem> ListarViagem(int? IdentificadorParticipante, string Nome, bool? Aberto, DateTime? DataInicioDe, DateTime? DataFimDe, DateTime? DataInicioAte, DateTime? DataFimAte, int? IdentificadorUsuario)
        {
            IQueryable<Usuario> Query = this.Context.Usuarios;
            IQueryable<Amigo> QueryAmigo = this.Context.Amigos.Where(d => d.IdentificadorUsuario == IdentificadorUsuario);
            Query = Query.Where(d => d.Identificador == IdentificadorUsuario || QueryAmigo.Where(e => e.IdentificadorAmigo == d.Identificador).Any());
            IQueryable<Viagem> query = this.Context.Viagemes.Include("Participantes").Include("Participantes.ItemUsuario")
                .Where(d => d.Participantes.Where(e => Query.Where(f => f.Identificador == e.IdentificadorUsuario).Any()).Any());
            if (IdentificadorParticipante.HasValue)
                query = query.Where(d => d.Participantes.Where(e => e.IdentificadorUsuario == IdentificadorParticipante).Any());
            if (Aberto.HasValue)
                query = query.Where(d => d.Aberto == Aberto);
            if (DataInicioDe.HasValue)
                query = query.Where(d => d.DataInicio >= DataInicioDe);
            if (DataInicioAte.HasValue)
                query = query.Where(d => d.DataInicio <= DataInicioAte);
            if (DataFimDe.HasValue)
                query = query.Where(d => d.DataFim >= DataFimDe);
            if (DataFimAte.HasValue)
                query = query.Where(d => d.DataFim <= DataFimAte);
            if (!string.IsNullOrWhiteSpace(Nome))
                query = query.Where(d => d.Nome.Contains(Nome));
            query = query.Where(d => !d.DataExclusao.HasValue);
            return query.ToList();
        }

        public List<Foto> ListarFotos(int? IdentificadorFoto, int IdentificadorViagem, DateTime? DataDe, DateTime? DataAte, string Comentario, List<int?> IdentificadorAtracao,
            List<int?> IdentificadorHotel, List<int?> IdentificadorRefeicao, int? IdentificadorCidade, int Skip, int Count)
        {
            IQueryable<Foto> query = this.Context.Fotos.Include("Atracoes").Include("Atracoes.ItemAtracao")
                .Include("Hoteis").Include("Hoteis.ItemHotel")
                .Include("ItensCompra").Include("ItensCompra.ItemItemCompra")
                .Include("Refeicoes").Include("Refeicoes.ItemRefeicao");
            query = query.Where(d => d.IdentificadorViagem == IdentificadorViagem);
            if (IdentificadorFoto.HasValue)
                query = query.Where(d => d.Identificador == IdentificadorFoto);
            if (DataDe.HasValue)
                query = query.Where(d => d.Data >= DataDe);
            if (DataAte.HasValue)
            {
                DataAte = DataAte.GetValueOrDefault().AddDays(1);
                query = query.Where(d => d.Data < DataAte);
            }
            if (!string.IsNullOrWhiteSpace(Comentario))
                query = query.Where(d => d.Comentario.Contains(Comentario));
            if (IdentificadorAtracao != null && IdentificadorAtracao.Any())
                query = query.Where(d => d.Atracoes.Where(e => IdentificadorAtracao.Contains(e.IdentificadorAtracao)).Any());
            if (IdentificadorHotel != null && IdentificadorHotel.Any())
                query = query.Where(d => d.Hoteis.Where(e => IdentificadorHotel.Contains(e.IdentificadorHotel)).Any());
            if (IdentificadorRefeicao != null && IdentificadorRefeicao.Any())
                query = query.Where(d => d.Refeicoes.Where(e => IdentificadorRefeicao.Contains(e.IdentificadorRefeicao)).Any());
            if (IdentificadorCidade.HasValue)
                query = query.Where(d => d.IdentificadorCidade == IdentificadorCidade || this.Context.CidadeGrupos.Where(e => e.IdentificadorCidadeFilha == d.IdentificadorCidade).Where(f => f.IdentificadorViagem == IdentificadorViagem).Where(e => e.IdentificadorCidadePai == IdentificadorCidade).Any());
            query = query.OrderBy(d => d.DataAtualizacao);
            query = query.Skip(Skip).Take(Count);
            return query.ToList();
        }

        public List<Cidade> CarregarCidadeViagemFoto(int? IdentificadorViagem)
        {
            IQueryable<Cidade> query = this.Context.Fotos.Where(d => d.IdentificadorViagem == IdentificadorViagem).Select(d => d.ItemCidade);

            var queryJoin = query.GroupJoin(this.Context.CidadeGrupos.Where(d => d.IdentificadorViagem == IdentificadorViagem),
                d => d.Identificador,
                d => d.IdentificadorCidadeFilha,
                (c, g) => new { cidade = g.Any() ? g.Select(d => d.ItemCidadePai).FirstOrDefault() : c });

            return queryJoin.Where(d => d.cidade != null).Select(d => d.cidade).Distinct().OrderBy(d => d.Nome).ToList();


        }

        public List<Cidade> CarregarCidadeAtracao(int? IdentificadorViagem)
        {
            IQueryable<Cidade> query = this.Context.Atracoes.Where(d => d.IdentificadorViagem == IdentificadorViagem).Select(d => d.ItemCidade);

            var queryJoin = query.GroupJoin(this.Context.CidadeGrupos.Where(d => d.IdentificadorViagem == IdentificadorViagem),
                d => d.Identificador,
                d => d.IdentificadorCidadeFilha,
                (c, g) => new { cidade = g.Any() ? g.Select(d => d.ItemCidadePai).FirstOrDefault() : c });

            return queryJoin.Where(d => d.cidade != null).Select(d => d.cidade).Distinct().OrderBy(d => d.Nome).ToList();


        }

        public List<Cidade> CarregarCidadeRefeicao(int? IdentificadorViagem)
        {
            IQueryable<Cidade> query = this.Context.Refeicoes.Where(d => d.IdentificadorViagem == IdentificadorViagem).Select(d => d.ItemCidade);

            var queryJoin = query.GroupJoin(this.Context.CidadeGrupos.Where(d => d.IdentificadorViagem == IdentificadorViagem),
                d => d.Identificador,
                d => d.IdentificadorCidadeFilha,
                (c, g) => new { cidade = g.Any() ? g.Select(d => d.ItemCidadePai).FirstOrDefault() : c });

            return queryJoin.Where(d => d.cidade != null).Select(d => d.cidade).Distinct().OrderBy(d => d.Nome).ToList();


        }
        public List<Cidade> CarregarCidadeHotel(int? IdentificadorViagem)
        {
            IQueryable<Cidade> query = this.Context.Hoteis.Where(d => d.IdentificadorViagem == IdentificadorViagem).Select(d => d.ItemCidade);

            var queryJoin = query.GroupJoin(this.Context.CidadeGrupos.Where(d => d.IdentificadorViagem == IdentificadorViagem),
                d => d.Identificador,
                d => d.IdentificadorCidadeFilha,
                (c, g) => new { cidade = g.Any() ? g.Select(d => d.ItemCidadePai).FirstOrDefault() : c });

            return queryJoin.Where(d => d.cidade != null).Select(d => d.cidade).Distinct().OrderBy(d => d.Nome).ToList();


        }
        public List<Cidade> CarregarCidadeLoja(int? IdentificadorViagem)
        {
            IQueryable<Cidade> query = this.Context.Lojas.Where(d => d.IdentificadorViagem == IdentificadorViagem).Select(d => d.ItemCidade);

            var queryJoin = query.GroupJoin(this.Context.CidadeGrupos.Where(d => d.IdentificadorViagem == IdentificadorViagem),
                d => d.Identificador,
                d => d.IdentificadorCidadeFilha,
                (c, g) => new { cidade = g.Any() ? g.Select(d => d.ItemCidadePai).FirstOrDefault() : c });

            return queryJoin.Where(d => d.cidade != null).Select(d => d.cidade).Distinct().OrderBy(d => d.Nome).ToList();


        }
        public List<Refeicao> ListarRefeicao(Expression<Func<Refeicao, bool>> predicate)
        {
            return Context.Refeicoes.Where(d => !d.DataExclusao.HasValue).Where(predicate).ToList();
        }

        public List<Hotel> ListarHotel(Expression<Func<Hotel, bool>> predicate)
        {
            return Context.Hoteis.Where(d => !d.DataExclusao.HasValue).Where(predicate).ToList();
        }

        public List<Atracao> ListarAtracao(Expression<Func<Atracao, bool>> predicate)
        {
            return Context.Atracoes.Where(d => !d.DataExclusao.HasValue).Where(predicate).ToList();
        }

        public List<ItemCompra> ListarItemCompra(Expression<Func<ItemCompra, bool>> predicate)
        {
            return Context.ItemCompras.Where(d => !d.DataExclusao.HasValue).Where(predicate).ToList();
        }

        public List<Posicao> ListarPosicao(Expression<Func<Posicao, bool>> predicate)
        {
            return Context.Posicoes.Where(predicate).ToList();
        }

        public List<HotelEvento> ListarHotelEvento(Expression<Func<HotelEvento, bool>> predicate)
        {
            return Context.HotelEventos.Where(predicate).ToList();
        }

        public List<AvaliacaoAtracao> ListarAvaliacaoAtracao(Expression<Func<AvaliacaoAtracao, bool>> predicate)
        {
            return Context.AvaliacaoAtracoes.Where(predicate).ToList();
        }
        public List<RefeicaoPedido> ListarRefeicaoPedido(Expression<Func<RefeicaoPedido, bool>> predicate)
        {
            return Context.RefeicaoPedidos.Where(predicate).ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="IdentificadorViagem"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public List<Hotel> ListarHotelData(int? IdentificadorViagem, DateTime? data)
        {
            IQueryable<Hotel> query = this.Context.Hoteis.Where(d => d.IdentificadorViagem == IdentificadorViagem);
            query = query.Where(d => d.DataEntrada <= data).Where(d => d.DataSaidia >= data);
            IQueryable<HotelEvento> queryHotelEvento = this.Context.HotelEventos.Where(d => !d.DataExclusao.HasValue && d.DataEntrada <= data && (!d.DataSaida.HasValue || d.DataSaida > data));
            query = query.Where(d => queryHotelEvento.Where(e => d.Identificador == e.IdentificadorHotel).Any());
            return query.ToList();
        }

        public IList<Usuario> CarregarParticipantesViagem(int? IdentificadorViagem)
        {
            return this.Context.ParticipanteViagemes.Where(d => d.IdentificadorViagem == IdentificadorViagem).Select(e => e.ItemUsuario).ToList();
        }


        public List<Atracao> ListarAtracao(int IdentificadorViagem, DateTime? DataChegadaDe, DateTime? DataChegadaAte,
             DateTime? DataPartidaDe, DateTime? DataPartidaAte, string Nome, string Tipo, int Situacao, int? IdentificadorCidade, int? IdentificadorAtracao)
        {
            IQueryable<Atracao> query = this.Context.Atracoes;
            //.Include("ItemAtracaoPai").Include("Fotos.ItemFoto")
            //    .Include("Gastos").Include("Gastos.ItemGasto").Include("Gastos.ItemGasto.ItemUsuario")
            //    .Include("Avaliacoes");
            if (IdentificadorAtracao.HasValue)
                query = query.Where(d => d.Identificador == IdentificadorAtracao);
            query = query.Where(d => d.IdentificadorViagem == IdentificadorViagem);
            if (DataChegadaDe.HasValue)
                query = query.Where(d => d.Chegada >= DataChegadaDe);
            if (DataChegadaAte.HasValue)
            {
                DataChegadaAte = DataChegadaAte.GetValueOrDefault().AddDays(1);
                query = query.Where(d => d.Chegada < DataChegadaAte);
            }

            if (DataPartidaDe.HasValue)
                query = query.Where(d => d.Partida >= DataPartidaDe);
            if (DataChegadaAte.HasValue)
            {
                DataPartidaAte = DataPartidaAte.GetValueOrDefault().AddDays(1);
                query = query.Where(d => d.Partida < DataPartidaAte);
            }


            if (!string.IsNullOrWhiteSpace(Nome))
                query = query.Where(d => d.Nome.Contains(Nome));

            if (!string.IsNullOrWhiteSpace(Tipo))
                query = query.Where(d => d.Tipo.Contains(Tipo));

            if (Situacao == 1)
                query = query.Where(d => d.Chegada.HasValue && !d.Partida.HasValue);
            else if (Situacao == 2)
                query = query.Where(d => d.Chegada.HasValue && d.Partida.HasValue);
            else if (Situacao == 3)
                query = query.Where(d => !d.Chegada.HasValue);
            query = query.Where(d => !d.DataExclusao.HasValue);
            if (IdentificadorCidade.HasValue)
                query = query.Where(d => d.IdentificadorCidade == IdentificadorCidade || this.Context.CidadeGrupos.Where(e => e.IdentificadorCidadeFilha == d.IdentificadorCidade).Where(f => f.IdentificadorViagem == IdentificadorViagem).Where(e => e.IdentificadorCidadePai == IdentificadorCidade).Any());
            query = query.OrderByDescending(d => d.Chegada);



            return query.ToList();
        }

        public List<Gasto> ListarGasto(int IdentificadorViagem, DateTime? DataDe, DateTime? DataAte, string Descricao,
            int? IdentificadorUsuario, int? IdentificadorGasto)
        {
            IQueryable<Gasto> query = this.Context.Gastos.Include("ItemUsuario");
            query = query.Where(d => d.IdentificadorViagem == IdentificadorViagem);
            if (IdentificadorGasto.HasValue)
                query = query.Where(d => d.Identificador == IdentificadorGasto);
            if (IdentificadorUsuario.HasValue)
                query = query.Where(d => d.IdentificadorUsuario == IdentificadorUsuario);
            if (!string.IsNullOrEmpty(Descricao))
                query = query.Where(d => d.Descricao.Contains(Descricao));
            if (DataDe.HasValue)
                query = query.Where(d => d.Data >= DataDe);
            if (DataAte.HasValue)
            {
                DataAte = DataAte.GetValueOrDefault().AddDays(1);
                query = query.Where(d => d.Data < DataAte);
            }
            query = query.OrderByDescending(d => d.Data);
            return query.ToList();
        }

        public List<Refeicao> ListarRefeicao(int IdentificadorViagem, DateTime? DataDe, DateTime? DataAte,
             string Nome, string Tipo,  int? IdentificadorCidade, int? IdentificadorRefeicao)
        {
            IQueryable<Refeicao> query = this.Context.Refeicoes;
            //.Include("ItemAtracaoPai").Include("Fotos.ItemFoto")
            //    .Include("Gastos").Include("Gastos.ItemGasto").Include("Gastos.ItemGasto.ItemUsuario")
            //    .Include("Avaliacoes");
            if (IdentificadorRefeicao.HasValue)
                query = query.Where(d => d.Identificador == IdentificadorRefeicao);
            query = query.Where(d => d.IdentificadorViagem == IdentificadorViagem);
            if (DataDe.HasValue)
                query = query.Where(d => d.Data >= DataDe);
            if (DataAte.HasValue)
            {
                DataAte = DataAte.GetValueOrDefault().AddDays(1);
                query = query.Where(d => d.Data < DataAte);
            }                       

            if (!string.IsNullOrWhiteSpace(Nome))
                query = query.Where(d => d.Nome.Contains(Nome));

            if (!string.IsNullOrWhiteSpace(Tipo))
                query = query.Where(d => d.Tipo.Contains(Tipo));

            query = query.Where(d => !d.DataExclusao.HasValue);
            if (IdentificadorCidade.HasValue)
                query = query.Where(d => d.IdentificadorCidade == IdentificadorCidade || this.Context.CidadeGrupos.Where(e => e.IdentificadorCidadeFilha == d.IdentificadorCidade).Where(f => f.IdentificadorViagem == IdentificadorViagem).Where(e => e.IdentificadorCidadePai == IdentificadorCidade).Any());
            query = query.OrderByDescending(d => d.Data);

            return query.ToList();
        }


        public List<Hotel> ListarHotel(int IdentificadorViagem, DateTime? DataCheckInDe, DateTime? DataCheckInAte,
         DateTime? DataCheckOutDe, DateTime? DataCheckOutAte, string Nome, int Situacao, int? IdentificadorCidade, int? IdentificadorHotel)
        {
            IQueryable<Hotel> query = this.Context.Hoteis;
            //.Include("ItemAtracaoPai").Include("Fotos.ItemFoto")
            //    .Include("Gastos").Include("Gastos.ItemGasto").Include("Gastos.ItemGasto.ItemUsuario")
            //    .Include("Avaliacoes");
            if (IdentificadorHotel.HasValue)
                query = query.Where(d => d.Identificador == IdentificadorHotel);
            query = query.Where(d => d.IdentificadorViagem == IdentificadorViagem);
            if (DataCheckInDe.HasValue)
                query = query.Where(d => d.EntradaPrevista >= DataCheckInDe);
            if (DataCheckInAte.HasValue)
            {
                DataCheckInAte = DataCheckInAte.GetValueOrDefault().AddDays(1);
                query = query.Where(d => d.EntradaPrevista < DataCheckInAte);
            }

            if (DataCheckOutDe.HasValue)
                query = query.Where(d => d.SaidaPrevista >= DataCheckOutDe);
            if (DataCheckInAte.HasValue)
            {
                DataCheckOutAte = DataCheckOutAte.GetValueOrDefault().AddDays(1);
                query = query.Where(d => d.SaidaPrevista < DataCheckOutAte);
            }


            if (!string.IsNullOrWhiteSpace(Nome))
                query = query.Where(d => d.Nome.Contains(Nome));

           
            if (Situacao == 1)
                query = query.Where(d => d.DataEntrada.HasValue && !d.DataSaidia.HasValue);
            else if (Situacao == 2)
                query = query.Where(d => d.DataEntrada.HasValue && d.DataSaidia.HasValue);
            else if (Situacao == 3)
                query = query.Where(d => !d.DataEntrada.HasValue);
            query = query.Where(d => !d.DataExclusao.HasValue);
            if (IdentificadorCidade.HasValue)
                query = query.Where(d => d.IdentificadorCidade == IdentificadorCidade || this.Context.CidadeGrupos.Where(e => e.IdentificadorCidadeFilha == d.IdentificadorCidade).Where(f => f.IdentificadorViagem == IdentificadorViagem).Where(e => e.IdentificadorCidadePai == IdentificadorCidade).Any());
            query = query.OrderByDescending(d => d.EntradaPrevista);



            return query.ToList();
        }

    }
}
