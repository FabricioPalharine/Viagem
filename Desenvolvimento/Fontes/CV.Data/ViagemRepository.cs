using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.Linq.Expressions;
using System.Data.Entity.Infrastructure;
using CV.Model;

namespace CV.Data
{
    public partial class ViagemRepository : RepositoryBase
    {
        public AluguelGasto SelecionarAluguelGasto(int? Identificador)
        {
            IQueryable<AluguelGasto> query = Context.AluguelGastos
;
            if (Identificador.HasValue)
                query = query.Where(d => d.Identificador == Identificador);
            return query.FirstOrDefault();
        }
        public IList<AluguelGasto> ListarAluguelGasto()
        {
            IQueryable<AluguelGasto> query = Context.AluguelGastos
;
            return query.ToList();
        }
        public void SalvarAluguelGasto(AluguelGasto itemGravar)
        {
            AluguelGasto itemBase = Context.AluguelGastos
            .Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            if (itemBase == null)
            {
                itemBase = Context.AluguelGastos.Create();
                Context.Entry<AluguelGasto>(itemBase).State = System.Data.Entity.EntityState.Added;
            }
            AtualizarPropriedades<AluguelGasto>(itemBase, itemGravar);
            Context.SaveChanges();
            itemGravar.Identificador = itemBase.Identificador;
        }
        public void ExcluirAluguelGasto(AluguelGasto itemGravar)
        {
            AluguelGasto itemExcluir = Context.AluguelGastos
        .Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            Context.Entry<AluguelGasto>(itemExcluir).State = EntityState.Deleted;
            Context.SaveChanges();
        }
        public Reabastecimento SelecionarReabastecimento(int? Identificador)
        {
            IQueryable<Reabastecimento> query = Context.Reabastecimentos
.Include("Gastos").Include("Gastos.ItemGasto").Include("Gastos.ItemGasto.Usuarios").Include("Gastos.ItemGasto.Usuarios.ItemUsuario");
            if (Identificador.HasValue)
                query = query.Where(d => d.Identificador == Identificador);
            return query.FirstOrDefault();
        }
        public IList<Reabastecimento> ListarReabastecimento()
        {
            IQueryable<Reabastecimento> query = Context.Reabastecimentos
;
            return query.ToList();
        }
        public void SalvarReabastecimento(Reabastecimento itemGravar)
        {
            Reabastecimento itemBase = Context.Reabastecimentos
.Include("Gastos").Include("Gastos.ItemGasto").Include("Gastos.ItemGasto.Usuarios").Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            if (itemBase == null)
            {
                itemBase = Context.Reabastecimentos.Create();
                itemBase.Gastos = new List<ReabastecimentoGasto>();
                Context.Entry<Reabastecimento>(itemBase).State = System.Data.Entity.EntityState.Added;
            }
            AtualizarPropriedades<Reabastecimento>(itemBase, itemGravar);
            foreach (ReabastecimentoGasto itemReabastecimentoGasto in new List<ReabastecimentoGasto>(itemBase.Gastos))
            {
                if (!itemGravar.Gastos.Where(f => f.Identificador == itemReabastecimentoGasto.Identificador).Any())
                {
                    Context.Entry<ReabastecimentoGasto>(itemReabastecimentoGasto).State = EntityState.Deleted;
                }
            }
            foreach (ReabastecimentoGasto itemReabastecimentoGasto in new List<ReabastecimentoGasto>(itemGravar.Gastos))
            {
                ReabastecimentoGasto itemBaseReabastecimentoGasto = !itemReabastecimentoGasto.Identificador.HasValue ? null : itemBase.Gastos.Where(f => f.Identificador == itemReabastecimentoGasto.Identificador).FirstOrDefault();
                if (itemBaseReabastecimentoGasto == null)
                {
                    itemBaseReabastecimentoGasto = Context.ReabastecimentoGastos.Create();
                    itemBase.Gastos.Add(itemBaseReabastecimentoGasto);
                }
                AtualizarPropriedades<ReabastecimentoGasto>(itemBaseReabastecimentoGasto, itemReabastecimentoGasto);
                Gasto itemGasto = itemReabastecimentoGasto.ItemGasto;
                Gasto itemBaseGasto = null;
                if (itemGasto != null)
                {
                    itemBaseGasto = Context.Gastos.Where(f => f.Identificador == itemGasto.Identificador).FirstOrDefault();
                    if (itemBaseGasto == null)
                    {
                        itemBaseGasto = Context.Gastos.Create();
                        Context.Entry<Gasto>(itemBaseGasto).State = System.Data.Entity.EntityState.Added;
                    }
                    AtualizarPropriedades<Gasto>(itemBaseGasto, itemGasto);
                    itemBaseReabastecimentoGasto.ItemGasto = itemBaseGasto;
                    foreach (GastoDividido itemGastoDividido in new List<GastoDividido>(itemBaseGasto.Usuarios))
                    {
                        if (!itemGasto.Usuarios.Where(f => f.Identificador == itemGastoDividido.Identificador).Any())
                        {
                            Context.Entry<GastoDividido>(itemGastoDividido).State = EntityState.Deleted;
                        }
                    }
                    foreach (GastoDividido itemGastoDividido in new List<GastoDividido>(itemGasto.Usuarios))
                    {
                        GastoDividido itemBaseGastoDividido = !itemGastoDividido.Identificador.HasValue ? null : itemBaseGasto.Usuarios.Where(f => f.Identificador == itemGastoDividido.Identificador).FirstOrDefault();
                        if (itemBaseGastoDividido == null)
                        {
                            itemBaseGastoDividido = Context.GastoDivididos.Create();
                            itemBaseGasto.Usuarios.Add(itemBaseGastoDividido);
                        }
                        AtualizarPropriedades<GastoDividido>(itemBaseGastoDividido, itemGastoDividido);
                    }
                }
            }
            Context.SaveChanges();
            itemGravar.Identificador = itemBase.Identificador;
        }
        public void ExcluirReabastecimento(Reabastecimento itemGravar)
        {
            Reabastecimento itemExcluir = Context.Reabastecimentos
        .Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            Context.Entry<Reabastecimento>(itemExcluir).State = EntityState.Deleted;
            Context.SaveChanges();
        }
        public Amigo SelecionarAmigo(int? Identificador)
        {
            IQueryable<Amigo> query = Context.Amigos
;
            if (Identificador.HasValue)
                query = query.Where(d => d.Identificador == Identificador);
            return query.FirstOrDefault();
        }
        public IList<Amigo> ListarAmigo()
        {
            IQueryable<Amigo> query = Context.Amigos
;
            return query.ToList();
        }
        public void SalvarAmigo(Amigo itemGravar)
        {
            Amigo itemBase = Context.Amigos
            .Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            if (itemBase == null)
            {
                itemBase = Context.Amigos.Create();
                Context.Entry<Amigo>(itemBase).State = System.Data.Entity.EntityState.Added;
            }
            AtualizarPropriedades<Amigo>(itemBase, itemGravar);
            Context.SaveChanges();
            itemGravar.Identificador = itemBase.Identificador;
        }
        public void ExcluirAmigo(Amigo itemGravar)
        {
            Amigo itemExcluir = Context.Amigos
        .Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            Context.Entry<Amigo>(itemExcluir).State = EntityState.Deleted;
            Context.SaveChanges();
        }
        public AporteDinheiro SelecionarAporteDinheiro(int? Identificador)
        {
            IQueryable<AporteDinheiro> query = Context.AporteDinheiros
.Include("ItemGasto");
            if (Identificador.HasValue)
                query = query.Where(d => d.Identificador == Identificador);
            return query.FirstOrDefault();
        }
        public IList<AporteDinheiro> ListarAporteDinheiro()
        {
            IQueryable<AporteDinheiro> query = Context.AporteDinheiros
;
            return query.ToList();
        }
        public void SalvarAporteDinheiro(AporteDinheiro itemGravar)
        {
            AporteDinheiro itemBase = Context.AporteDinheiros
.Include("ItemGasto").Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            if (itemBase == null)
            {
                itemBase = Context.AporteDinheiros.Create();
                Context.Entry<AporteDinheiro>(itemBase).State = System.Data.Entity.EntityState.Added;
            }
            AtualizarPropriedades<AporteDinheiro>(itemBase, itemGravar);
            Gasto itemGasto = itemGravar.ItemGasto;
            Gasto itemBaseGasto = null;
            if (itemGasto != null)
            {
                itemBaseGasto = Context.Gastos.Where(f => f.Identificador == itemGasto.Identificador).FirstOrDefault();
                if (itemBaseGasto == null)
                {
                    itemBaseGasto = Context.Gastos.Create();
                    Context.Entry<Gasto>(itemBaseGasto).State = System.Data.Entity.EntityState.Added;
                }
                AtualizarPropriedades<Gasto>(itemBaseGasto, itemGasto);
                itemBase.ItemGasto = itemBaseGasto;
            }
            Context.SaveChanges();
            itemGravar.Identificador = itemBase.Identificador;
        }
        public void ExcluirAporteDinheiro(AporteDinheiro itemGravar)
        {
            AporteDinheiro itemExcluir = Context.AporteDinheiros
        .Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            Context.Entry<AporteDinheiro>(itemExcluir).State = EntityState.Deleted;
            Context.SaveChanges();
        }
        public Atracao SelecionarAtracao(int? Identificador)
        {
            IQueryable<Atracao> query = Context.Atracoes
;
            if (Identificador.HasValue)
                query = query.Where(d => d.Identificador == Identificador);
            return query.FirstOrDefault();
        }
        public IList<Atracao> ListarAtracao()
        {
            IQueryable<Atracao> query = Context.Atracoes
;
            return query.ToList();
        }
        public void SalvarAtracao(Atracao itemGravar)
        {
            Atracao itemBase = Context.Atracoes
.Include("Avaliacoes").Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            if (itemBase == null)
            {
                itemBase = Context.Atracoes.Create();
                itemBase.Avaliacoes = new List<AvaliacaoAtracao>();
                Context.Entry<Atracao>(itemBase).State = System.Data.Entity.EntityState.Added;
            }
            AtualizarPropriedades<Atracao>(itemBase, itemGravar);
            foreach (AvaliacaoAtracao itemAvaliacaoAtracao in new List<AvaliacaoAtracao>(itemBase.Avaliacoes))
            {
                if (!itemGravar.Avaliacoes.Where(f => f.Identificador == itemAvaliacaoAtracao.Identificador).Any())
                {
                    Context.Entry<AvaliacaoAtracao>(itemAvaliacaoAtracao).State = EntityState.Deleted;
                }
            }
            foreach (AvaliacaoAtracao itemAvaliacaoAtracao in new List<AvaliacaoAtracao>(itemGravar.Avaliacoes))
            {
                AvaliacaoAtracao itemBaseAvaliacaoAtracao = !itemAvaliacaoAtracao.Identificador.HasValue ? null : itemBase.Avaliacoes.Where(f => f.Identificador == itemAvaliacaoAtracao.Identificador).FirstOrDefault();
                if (itemBaseAvaliacaoAtracao == null)
                {
                    itemBaseAvaliacaoAtracao = Context.AvaliacaoAtracoes.Create();
                    itemBase.Avaliacoes.Add(itemBaseAvaliacaoAtracao);
                }
                AtualizarPropriedades<AvaliacaoAtracao>(itemBaseAvaliacaoAtracao, itemAvaliacaoAtracao);
            }
            Context.SaveChanges();
            itemGravar.Identificador = itemBase.Identificador;
        }
        public void ExcluirAtracao(Atracao itemGravar)
        {
            Atracao itemExcluir = Context.Atracoes
        .Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            Context.Entry<Atracao>(itemExcluir).State = EntityState.Deleted;
            Context.SaveChanges();
        }
        public AvaliacaoAerea SelecionarAvaliacaoAerea(int? Identificador)
        {
            IQueryable<AvaliacaoAerea> query = Context.AvaliacaoAereas
;
            if (Identificador.HasValue)
                query = query.Where(d => d.Identificador == Identificador);
            return query.FirstOrDefault();
        }
        public IList<AvaliacaoAerea> ListarAvaliacaoAerea()
        {
            IQueryable<AvaliacaoAerea> query = Context.AvaliacaoAereas
;
            return query.ToList();
        }
        public void SalvarAvaliacaoAerea(AvaliacaoAerea itemGravar)
        {
            AvaliacaoAerea itemBase = Context.AvaliacaoAereas
            .Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            if (itemBase == null)
            {
                itemBase = Context.AvaliacaoAereas.Create();
                Context.Entry<AvaliacaoAerea>(itemBase).State = System.Data.Entity.EntityState.Added;
            }
            AtualizarPropriedades<AvaliacaoAerea>(itemBase, itemGravar);
            Context.SaveChanges();
            itemGravar.Identificador = itemBase.Identificador;
        }
        public void ExcluirAvaliacaoAerea(AvaliacaoAerea itemGravar)
        {
            AvaliacaoAerea itemExcluir = Context.AvaliacaoAereas
        .Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            Context.Entry<AvaliacaoAerea>(itemExcluir).State = EntityState.Deleted;
            Context.SaveChanges();
        }
        public AvaliacaoAluguel SelecionarAvaliacaoAluguel(int? Identificador)
        {
            IQueryable<AvaliacaoAluguel> query = Context.AvaliacaoAlugueis
;
            if (Identificador.HasValue)
                query = query.Where(d => d.Identificador == Identificador);
            return query.FirstOrDefault();
        }
        public IList<AvaliacaoAluguel> ListarAvaliacaoAluguel()
        {
            IQueryable<AvaliacaoAluguel> query = Context.AvaliacaoAlugueis
;
            return query.ToList();
        }
        public void SalvarAvaliacaoAluguel(AvaliacaoAluguel itemGravar)
        {
            AvaliacaoAluguel itemBase = Context.AvaliacaoAlugueis
            .Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            if (itemBase == null)
            {
                itemBase = Context.AvaliacaoAlugueis.Create();
                Context.Entry<AvaliacaoAluguel>(itemBase).State = System.Data.Entity.EntityState.Added;
            }
            AtualizarPropriedades<AvaliacaoAluguel>(itemBase, itemGravar);
            Context.SaveChanges();
            itemGravar.Identificador = itemBase.Identificador;
        }
        public void ExcluirAvaliacaoAluguel(AvaliacaoAluguel itemGravar)
        {
            AvaliacaoAluguel itemExcluir = Context.AvaliacaoAlugueis
        .Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            Context.Entry<AvaliacaoAluguel>(itemExcluir).State = EntityState.Deleted;
            Context.SaveChanges();
        }
        public AvaliacaoAtracao SelecionarAvaliacaoAtracao(int? Identificador)
        {
            IQueryable<AvaliacaoAtracao> query = Context.AvaliacaoAtracoes
;
            if (Identificador.HasValue)
                query = query.Where(d => d.Identificador == Identificador);
            return query.FirstOrDefault();
        }
        public IList<AvaliacaoAtracao> ListarAvaliacaoAtracao()
        {
            IQueryable<AvaliacaoAtracao> query = Context.AvaliacaoAtracoes
;
            return query.ToList();
        }
        public void SalvarAvaliacaoAtracao(AvaliacaoAtracao itemGravar)
        {
            AvaliacaoAtracao itemBase = Context.AvaliacaoAtracoes
            .Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            if (itemBase == null)
            {
                itemBase = Context.AvaliacaoAtracoes.Create();
                Context.Entry<AvaliacaoAtracao>(itemBase).State = System.Data.Entity.EntityState.Added;
            }
            AtualizarPropriedades<AvaliacaoAtracao>(itemBase, itemGravar);
            Context.SaveChanges();
            itemGravar.Identificador = itemBase.Identificador;
        }
        public void ExcluirAvaliacaoAtracao(AvaliacaoAtracao itemGravar)
        {
            AvaliacaoAtracao itemExcluir = Context.AvaliacaoAtracoes
        .Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            Context.Entry<AvaliacaoAtracao>(itemExcluir).State = EntityState.Deleted;
            Context.SaveChanges();
        }
        public CalendarioPrevisto SelecionarCalendarioPrevisto(int? Identificador)
        {
            IQueryable<CalendarioPrevisto> query = Context.CalendarioPrevistos
;
            if (Identificador.HasValue)
                query = query.Where(d => d.Identificador == Identificador);
            return query.FirstOrDefault();
        }
        public IList<CalendarioPrevisto> ListarCalendarioPrevisto()
        {
            IQueryable<CalendarioPrevisto> query = Context.CalendarioPrevistos
;
            return query.ToList();
        }
        public void SalvarCalendarioPrevisto(CalendarioPrevisto itemGravar)
        {
            CalendarioPrevisto itemBase = Context.CalendarioPrevistos
            .Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            if (itemBase == null)
            {
                itemBase = Context.CalendarioPrevistos.Create();
                Context.Entry<CalendarioPrevisto>(itemBase).State = System.Data.Entity.EntityState.Added;
            }
            AtualizarPropriedades<CalendarioPrevisto>(itemBase, itemGravar);
            Context.SaveChanges();
            itemGravar.Identificador = itemBase.Identificador;
        }
        public void ExcluirCalendarioPrevisto(CalendarioPrevisto itemGravar)
        {
            CalendarioPrevisto itemExcluir = Context.CalendarioPrevistos
        .Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            Context.Entry<CalendarioPrevisto>(itemExcluir).State = EntityState.Deleted;
            Context.SaveChanges();
        }
        public Carro SelecionarCarro(int? Identificador)
        {
            IQueryable<Carro> query = Context.Carros
;
            if (Identificador.HasValue)
                query = query.Where(d => d.Identificador == Identificador);
            return query.FirstOrDefault();
        }
        public IList<Carro> ListarCarro()
        {
            IQueryable<Carro> query = Context.Carros
;
            return query.ToList();
        }
        public void SalvarCarro(Carro itemGravar)
        {
            Carro itemBase = Context.Carros
.Include("Avaliacoes").Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            if (itemBase == null)
            {
                itemBase = Context.Carros.Create();
                itemBase.Avaliacoes = new List<AvaliacaoAluguel>();
                Context.Entry<Carro>(itemBase).State = System.Data.Entity.EntityState.Added;
            }
            AtualizarPropriedades<Carro>(itemBase, itemGravar);
            foreach (AvaliacaoAluguel itemAvaliacaoAluguel in new List<AvaliacaoAluguel>(itemBase.Avaliacoes))
            {
                if (!itemGravar.Avaliacoes.Where(f => f.Identificador == itemAvaliacaoAluguel.Identificador).Any())
                {
                    Context.Entry<AvaliacaoAluguel>(itemAvaliacaoAluguel).State = EntityState.Deleted;
                }
            }
            foreach (AvaliacaoAluguel itemAvaliacaoAluguel in new List<AvaliacaoAluguel>(itemGravar.Avaliacoes))
            {
                AvaliacaoAluguel itemBaseAvaliacaoAluguel = !itemAvaliacaoAluguel.Identificador.HasValue ? null : itemBase.Avaliacoes.Where(f => f.Identificador == itemAvaliacaoAluguel.Identificador).FirstOrDefault();
                if (itemBaseAvaliacaoAluguel == null)
                {
                    itemBaseAvaliacaoAluguel = Context.AvaliacaoAlugueis.Create();
                    itemBase.Avaliacoes.Add(itemBaseAvaliacaoAluguel);
                }
                AtualizarPropriedades<AvaliacaoAluguel>(itemBaseAvaliacaoAluguel, itemAvaliacaoAluguel);
            }
            Context.SaveChanges();
            itemGravar.Identificador = itemBase.Identificador;
        }
        public void ExcluirCarro(Carro itemGravar)
        {
            Carro itemExcluir = Context.Carros
        .Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            Context.Entry<Carro>(itemExcluir).State = EntityState.Deleted;
            Context.SaveChanges();
        }
        public CarroEvento SelecionarCarroEvento(int? Identificador)
        {
            IQueryable<CarroEvento> query = Context.CarroEventos
;
            if (Identificador.HasValue)
                query = query.Where(d => d.Identificador == Identificador);
            return query.FirstOrDefault();
        }
        public IList<CarroEvento> ListarCarroEvento()
        {
            IQueryable<CarroEvento> query = Context.CarroEventos
;
            return query.ToList();
        }
        public void SalvarCarroEvento(CarroEvento itemGravar)
        {
            CarroEvento itemBase = Context.CarroEventos
            .Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            if (itemBase == null)
            {
                itemBase = Context.CarroEventos.Create();
                Context.Entry<CarroEvento>(itemBase).State = System.Data.Entity.EntityState.Added;
            }
            AtualizarPropriedades<CarroEvento>(itemBase, itemGravar);
            Context.SaveChanges();
            itemGravar.Identificador = itemBase.Identificador;
        }
        public void ExcluirCarroEvento(CarroEvento itemGravar)
        {
            CarroEvento itemExcluir = Context.CarroEventos
        .Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            Context.Entry<CarroEvento>(itemExcluir).State = EntityState.Deleted;
            Context.SaveChanges();
        }
        public Cidade SelecionarCidade(int? Identificador)
        {
            IQueryable<Cidade> query = Context.Cidades
.Include("ItemPais");
            if (Identificador.HasValue)
                query = query.Where(d => d.Identificador == Identificador);
            return query.FirstOrDefault();
        }
        public IList<Cidade> ListarCidade()
        {
            IQueryable<Cidade> query = Context.Cidades
;
            return query.ToList();
        }
        public void SalvarCidade(Cidade itemGravar)
        {
            Cidade itemBase = Context.Cidades
            .Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            if (itemBase == null)
            {
                itemBase = Context.Cidades.Create();
                Context.Entry<Cidade>(itemBase).State = System.Data.Entity.EntityState.Added;
            }
            AtualizarPropriedades<Cidade>(itemBase, itemGravar);
            Context.SaveChanges();
            itemGravar.Identificador = itemBase.Identificador;
        }
        public void ExcluirCidade(Cidade itemGravar)
        {
            Cidade itemExcluir = Context.Cidades
        .Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            Context.Entry<Cidade>(itemExcluir).State = EntityState.Deleted;
            Context.SaveChanges();
        }
        public CidadeGrupo SelecionarCidadeGrupo(int? Identificador)
        {
            IQueryable<CidadeGrupo> query = Context.CidadeGrupos
;
            if (Identificador.HasValue)
                query = query.Where(d => d.Identificador == Identificador);
            return query.FirstOrDefault();
        }
        public IList<CidadeGrupo> ListarCidadeGrupo()
        {
            IQueryable<CidadeGrupo> query = Context.CidadeGrupos
;
            return query.ToList();
        }
        public void SalvarCidadeGrupo(CidadeGrupo itemGravar)
        {
            CidadeGrupo itemBase = Context.CidadeGrupos
            .Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            if (itemBase == null)
            {
                itemBase = Context.CidadeGrupos.Create();
                Context.Entry<CidadeGrupo>(itemBase).State = System.Data.Entity.EntityState.Added;
            }
            AtualizarPropriedades<CidadeGrupo>(itemBase, itemGravar);
            Context.SaveChanges();
            itemGravar.Identificador = itemBase.Identificador;
        }
        public void ExcluirCidadeGrupo(CidadeGrupo itemGravar)
        {
            CidadeGrupo itemExcluir = Context.CidadeGrupos
        .Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            Context.Entry<CidadeGrupo>(itemExcluir).State = EntityState.Deleted;
            Context.SaveChanges();
        }
        public Comentario SelecionarComentario(int? Identificador)
        {
            IQueryable<Comentario> query = Context.Comentarios
;
            if (Identificador.HasValue)
                query = query.Where(d => d.Identificador == Identificador);
            return query.FirstOrDefault();
        }
        public IList<Comentario> ListarComentario()
        {
            IQueryable<Comentario> query = Context.Comentarios
;
            return query.ToList();
        }
        public void SalvarComentario(Comentario itemGravar)
        {
            Comentario itemBase = Context.Comentarios
            .Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            if (itemBase == null)
            {
                itemBase = Context.Comentarios.Create();
                Context.Entry<Comentario>(itemBase).State = System.Data.Entity.EntityState.Added;
            }
            AtualizarPropriedades<Comentario>(itemBase, itemGravar);
            Context.SaveChanges();
            itemGravar.Identificador = itemBase.Identificador;
        }
        public void ExcluirComentario(Comentario itemGravar)
        {
            Comentario itemExcluir = Context.Comentarios
        .Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            Context.Entry<Comentario>(itemExcluir).State = EntityState.Deleted;
            Context.SaveChanges();
        }
        public CotacaoMoeda SelecionarCotacaoMoeda(int? Identificador)
        {
            IQueryable<CotacaoMoeda> query = Context.CotacaoMoedas
;
            if (Identificador.HasValue)
                query = query.Where(d => d.Identificador == Identificador);
            return query.FirstOrDefault();
        }
        public IList<CotacaoMoeda> ListarCotacaoMoeda()
        {
            IQueryable<CotacaoMoeda> query = Context.CotacaoMoedas
;
            return query.ToList();
        }
        public void SalvarCotacaoMoeda(CotacaoMoeda itemGravar)
        {
            CotacaoMoeda itemBase = Context.CotacaoMoedas
            .Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            if (itemBase == null)
            {
                itemBase = Context.CotacaoMoedas.Create();
                Context.Entry<CotacaoMoeda>(itemBase).State = System.Data.Entity.EntityState.Added;
            }
            AtualizarPropriedades<CotacaoMoeda>(itemBase, itemGravar);
            Context.SaveChanges();
            itemGravar.Identificador = itemBase.Identificador;
        }
        public void ExcluirCotacaoMoeda(CotacaoMoeda itemGravar)
        {
            CotacaoMoeda itemExcluir = Context.CotacaoMoedas
        .Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            Context.Entry<CotacaoMoeda>(itemExcluir).State = EntityState.Deleted;
            Context.SaveChanges();
        }
        public Foto SelecionarFoto(int? Identificador)
        {
            IQueryable<Foto> query = Context.Fotos
;
            if (Identificador.HasValue)
                query = query.Where(d => d.Identificador == Identificador);
            return query.FirstOrDefault();
        }
        public IList<Foto> ListarFoto()
        {
            IQueryable<Foto> query = Context.Fotos
;
            return query.ToList();
        }
        public void SalvarFoto(Foto itemGravar)
        {
            Foto itemBase = Context.Fotos
            .Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            if (itemBase == null)
            {
                itemBase = Context.Fotos.Create();
                Context.Entry<Foto>(itemBase).State = System.Data.Entity.EntityState.Added;
            }
            AtualizarPropriedades<Foto>(itemBase, itemGravar);
            Context.SaveChanges();
            itemGravar.Identificador = itemBase.Identificador;
        }
        public void ExcluirFoto(Foto itemGravar)
        {
            Foto itemExcluir = Context.Fotos
.Include("Atracoes").Include("Hoteis").Include("ItensCompra").Include("Refeicoes").Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            foreach (FotoAtracao itemFotoAtracao in new List<FotoAtracao>(itemExcluir.Atracoes))
            {
                Context.Entry<FotoAtracao>(itemFotoAtracao).State = EntityState.Deleted;
            }
            foreach (FotoHotel itemFotoHotel in new List<FotoHotel>(itemExcluir.Hoteis))
            {
                Context.Entry<FotoHotel>(itemFotoHotel).State = EntityState.Deleted;
            }
            foreach (FotoItemCompra itemFotoItemCompra in new List<FotoItemCompra>(itemExcluir.ItensCompra))
            {
                Context.Entry<FotoItemCompra>(itemFotoItemCompra).State = EntityState.Deleted;
            }
            foreach (FotoRefeicao itemFotoRefeicao in new List<FotoRefeicao>(itemExcluir.Refeicoes))
            {
                Context.Entry<FotoRefeicao>(itemFotoRefeicao).State = EntityState.Deleted;
            }
            Context.Entry<Foto>(itemExcluir).State = EntityState.Deleted;
            Context.SaveChanges();
        }
        public FotoAtracao SelecionarFotoAtracao(int? Identificador)
        {
            IQueryable<FotoAtracao> query = Context.FotoAtracoes
;
            if (Identificador.HasValue)
                query = query.Where(d => d.Identificador == Identificador);
            return query.FirstOrDefault();
        }
        public IList<FotoAtracao> ListarFotoAtracao()
        {
            IQueryable<FotoAtracao> query = Context.FotoAtracoes
;
            return query.ToList();
        }
        public void SalvarFotoAtracao(FotoAtracao itemGravar)
        {
            FotoAtracao itemBase = Context.FotoAtracoes
            .Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            if (itemBase == null)
            {
                itemBase = Context.FotoAtracoes.Create();
                Context.Entry<FotoAtracao>(itemBase).State = System.Data.Entity.EntityState.Added;
            }
            AtualizarPropriedades<FotoAtracao>(itemBase, itemGravar);
            Context.SaveChanges();
            itemGravar.Identificador = itemBase.Identificador;
        }
        public void ExcluirFotoAtracao(FotoAtracao itemGravar)
        {
            FotoAtracao itemExcluir = Context.FotoAtracoes
        .Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            Context.Entry<FotoAtracao>(itemExcluir).State = EntityState.Deleted;
            Context.SaveChanges();
        }
        public FotoHotel SelecionarFotoHotel(int? Identificador)
        {
            IQueryable<FotoHotel> query = Context.FotoHoteis
;
            if (Identificador.HasValue)
                query = query.Where(d => d.Identificador == Identificador);
            return query.FirstOrDefault();
        }
        public IList<FotoHotel> ListarFotoHotel()
        {
            IQueryable<FotoHotel> query = Context.FotoHoteis
;
            return query.ToList();
        }
        public void SalvarFotoHotel(FotoHotel itemGravar)
        {
            FotoHotel itemBase = Context.FotoHoteis
            .Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            if (itemBase == null)
            {
                itemBase = Context.FotoHoteis.Create();
                Context.Entry<FotoHotel>(itemBase).State = System.Data.Entity.EntityState.Added;
            }
            AtualizarPropriedades<FotoHotel>(itemBase, itemGravar);
            Context.SaveChanges();
            itemGravar.Identificador = itemBase.Identificador;
        }
        public void ExcluirFotoHotel(FotoHotel itemGravar)
        {
            FotoHotel itemExcluir = Context.FotoHoteis
        .Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            Context.Entry<FotoHotel>(itemExcluir).State = EntityState.Deleted;
            Context.SaveChanges();
        }
        public FotoItemCompra SelecionarFotoItemCompra(int? Identificador)
        {
            IQueryable<FotoItemCompra> query = Context.FotoItemCompras
;
            if (Identificador.HasValue)
                query = query.Where(d => d.Identificador == Identificador);
            return query.FirstOrDefault();
        }
        public IList<FotoItemCompra> ListarFotoItemCompra()
        {
            IQueryable<FotoItemCompra> query = Context.FotoItemCompras
;
            return query.ToList();
        }
        public void SalvarFotoItemCompra(FotoItemCompra itemGravar)
        {
            FotoItemCompra itemBase = Context.FotoItemCompras
            .Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            if (itemBase == null)
            {
                itemBase = Context.FotoItemCompras.Create();
                Context.Entry<FotoItemCompra>(itemBase).State = System.Data.Entity.EntityState.Added;
            }
            AtualizarPropriedades<FotoItemCompra>(itemBase, itemGravar);
            Context.SaveChanges();
            itemGravar.Identificador = itemBase.Identificador;
        }
        public void ExcluirFotoItemCompra(FotoItemCompra itemGravar)
        {
            FotoItemCompra itemExcluir = Context.FotoItemCompras
        .Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            Context.Entry<FotoItemCompra>(itemExcluir).State = EntityState.Deleted;
            Context.SaveChanges();
        }
        public FotoRefeicao SelecionarFotoRefeicao(int? Identificador)
        {
            IQueryable<FotoRefeicao> query = Context.FotoRefeicoes
;
            if (Identificador.HasValue)
                query = query.Where(d => d.Identificador == Identificador);
            return query.FirstOrDefault();
        }
        public IList<FotoRefeicao> ListarFotoRefeicao()
        {
            IQueryable<FotoRefeicao> query = Context.FotoRefeicoes
;
            return query.ToList();
        }
        public void SalvarFotoRefeicao(FotoRefeicao itemGravar)
        {
            FotoRefeicao itemBase = Context.FotoRefeicoes
            .Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            if (itemBase == null)
            {
                itemBase = Context.FotoRefeicoes.Create();
                Context.Entry<FotoRefeicao>(itemBase).State = System.Data.Entity.EntityState.Added;
            }
            AtualizarPropriedades<FotoRefeicao>(itemBase, itemGravar);
            Context.SaveChanges();
            itemGravar.Identificador = itemBase.Identificador;
        }
        public void ExcluirFotoRefeicao(FotoRefeicao itemGravar)
        {
            FotoRefeicao itemExcluir = Context.FotoRefeicoes
        .Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            Context.Entry<FotoRefeicao>(itemExcluir).State = EntityState.Deleted;
            Context.SaveChanges();
        }
        public Gasto SelecionarGasto(int? Identificador)
        {
            IQueryable<Gasto> query = Context.Gastos
;
            if (Identificador.HasValue)
                query = query.Where(d => d.Identificador == Identificador);
            return query.FirstOrDefault();
        }
        public IList<Gasto> ListarGasto()
        {
            IQueryable<Gasto> query = Context.Gastos
;
            return query.ToList();
        }
        public void SalvarGasto(Gasto itemGravar)
        {
            Gasto itemBase = Context.Gastos
            .Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            if (itemBase == null)
            {
                itemBase = Context.Gastos.Create();
                Context.Entry<Gasto>(itemBase).State = System.Data.Entity.EntityState.Added;
            }
            AtualizarPropriedades<Gasto>(itemBase, itemGravar);
            Context.SaveChanges();
            itemGravar.Identificador = itemBase.Identificador;
        }
        public void ExcluirGasto(Gasto itemGravar)
        {
            Gasto itemExcluir = Context.Gastos
        .Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            Context.Entry<Gasto>(itemExcluir).State = EntityState.Deleted;
            Context.SaveChanges();
        }

        public GastoHotel SelecionarGastoHotel(int? Identificador)
        {
            IQueryable<GastoHotel> query = Context.GastoHoteis
;
            if (Identificador.HasValue)
                query = query.Where(d => d.Identificador == Identificador);
            return query.FirstOrDefault();
        }
        public IList<GastoHotel> ListarGastoHotel()
        {
            IQueryable<GastoHotel> query = Context.GastoHoteis
;
            return query.ToList();
        }
        public void SalvarGastoHotel(GastoHotel itemGravar)
        {
            GastoHotel itemBase = Context.GastoHoteis
            .Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            if (itemBase == null)
            {
                itemBase = Context.GastoHoteis.Create();
                Context.Entry<GastoHotel>(itemBase).State = System.Data.Entity.EntityState.Added;
            }
            AtualizarPropriedades<GastoHotel>(itemBase, itemGravar);
            Context.SaveChanges();
            itemGravar.Identificador = itemBase.Identificador;
        }
        public void ExcluirGastoHotel(GastoHotel itemGravar)
        {
            GastoHotel itemExcluir = Context.GastoHoteis
        .Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            Context.Entry<GastoHotel>(itemExcluir).State = EntityState.Deleted;
            Context.SaveChanges();
        }
        public GastoRefeicao SelecionarGastoRefeicao(int? Identificador)
        {
            IQueryable<GastoRefeicao> query = Context.GastoRefeicoes
;
            if (Identificador.HasValue)
                query = query.Where(d => d.Identificador == Identificador);
            return query.FirstOrDefault();
        }
        public IList<GastoRefeicao> ListarGastoRefeicao()
        {
            IQueryable<GastoRefeicao> query = Context.GastoRefeicoes
;
            return query.ToList();
        }
        public void SalvarGastoRefeicao(GastoRefeicao itemGravar)
        {
            GastoRefeicao itemBase = Context.GastoRefeicoes
            .Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            if (itemBase == null)
            {
                itemBase = Context.GastoRefeicoes.Create();
                Context.Entry<GastoRefeicao>(itemBase).State = System.Data.Entity.EntityState.Added;
            }
            AtualizarPropriedades<GastoRefeicao>(itemBase, itemGravar);
            Context.SaveChanges();
            itemGravar.Identificador = itemBase.Identificador;
        }
        public void ExcluirGastoRefeicao(GastoRefeicao itemGravar)
        {
            GastoRefeicao itemExcluir = Context.GastoRefeicoes
        .Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            Context.Entry<GastoRefeicao>(itemExcluir).State = EntityState.Deleted;
            Context.SaveChanges();
        }
        public GastoViagemAerea SelecionarGastoViagemAerea(int? Identificador)
        {
            IQueryable<GastoViagemAerea> query = Context.GastoViagemAereas
;
            if (Identificador.HasValue)
                query = query.Where(d => d.Identificador == Identificador);
            return query.FirstOrDefault();
        }
        public IList<GastoViagemAerea> ListarGastoViagemAerea()
        {
            IQueryable<GastoViagemAerea> query = Context.GastoViagemAereas
;
            return query.ToList();
        }
        public void SalvarGastoViagemAerea(GastoViagemAerea itemGravar)
        {
            GastoViagemAerea itemBase = Context.GastoViagemAereas
            .Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            if (itemBase == null)
            {
                itemBase = Context.GastoViagemAereas.Create();
                Context.Entry<GastoViagemAerea>(itemBase).State = System.Data.Entity.EntityState.Added;
            }
            AtualizarPropriedades<GastoViagemAerea>(itemBase, itemGravar);
            Context.SaveChanges();
            itemGravar.Identificador = itemBase.Identificador;
        }
        public void ExcluirGastoViagemAerea(GastoViagemAerea itemGravar)
        {
            GastoViagemAerea itemExcluir = Context.GastoViagemAereas
        .Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            Context.Entry<GastoViagemAerea>(itemExcluir).State = EntityState.Deleted;
            Context.SaveChanges();
        }
        public Hotel SelecionarHotel(int? Identificador)
        {
            IQueryable<Hotel> query = Context.Hoteis
;
            if (Identificador.HasValue)
                query = query.Where(d => d.Identificador == Identificador);
            return query.FirstOrDefault();
        }
        public IList<Hotel> ListarHotel()
        {
            IQueryable<Hotel> query = Context.Hoteis
;
            return query.ToList();
        }
        public void SalvarHotel(Hotel itemGravar)
        {
            Hotel itemBase = Context.Hoteis
.Include("Avaliacoes").Include("Eventos").Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            if (itemBase == null)
            {
                itemBase = Context.Hoteis.Create();
                itemBase.Avaliacoes = new List<HotelAvaliacao>();
                itemBase.Eventos = new List<HotelEvento>();
                Context.Entry<Hotel>(itemBase).State = System.Data.Entity.EntityState.Added;
            }
            AtualizarPropriedades<Hotel>(itemBase, itemGravar);
            foreach (HotelAvaliacao itemHotelAvaliacao in new List<HotelAvaliacao>(itemBase.Avaliacoes))
            {
                if (!itemGravar.Avaliacoes.Where(f => f.Identificador == itemHotelAvaliacao.Identificador).Any())
                {
                    Context.Entry<HotelAvaliacao>(itemHotelAvaliacao).State = EntityState.Deleted;
                }
            }
            foreach (HotelAvaliacao itemHotelAvaliacao in new List<HotelAvaliacao>(itemGravar.Avaliacoes))
            {
                HotelAvaliacao itemBaseHotelAvaliacao = !itemHotelAvaliacao.Identificador.HasValue ? null : itemBase.Avaliacoes.Where(f => f.Identificador == itemHotelAvaliacao.Identificador).FirstOrDefault();
                if (itemBaseHotelAvaliacao == null)
                {
                    itemBaseHotelAvaliacao = Context.HotelAvaliacoes.Create();
                    itemBase.Avaliacoes.Add(itemBaseHotelAvaliacao);
                }
                AtualizarPropriedades<HotelAvaliacao>(itemBaseHotelAvaliacao, itemHotelAvaliacao);
            }
            foreach (HotelEvento itemHotelEvento in new List<HotelEvento>(itemBase.Eventos))
            {
                if (!itemGravar.Eventos.Where(f => f.Identificador == itemHotelEvento.Identificador).Any())
                {
                    Context.Entry<HotelEvento>(itemHotelEvento).State = EntityState.Deleted;
                }
            }
            foreach (HotelEvento itemHotelEvento in new List<HotelEvento>(itemGravar.Eventos))
            {
                HotelEvento itemBaseHotelEvento = !itemHotelEvento.Identificador.HasValue ? null : itemBase.Eventos.Where(f => f.Identificador == itemHotelEvento.Identificador).FirstOrDefault();
                if (itemBaseHotelEvento == null)
                {
                    itemBaseHotelEvento = Context.HotelEventos.Create();
                    itemBase.Eventos.Add(itemBaseHotelEvento);
                }
                AtualizarPropriedades<HotelEvento>(itemBaseHotelEvento, itemHotelEvento);
            }
            Context.SaveChanges();
            itemGravar.Identificador = itemBase.Identificador;
        }
        public void ExcluirHotel(Hotel itemGravar)
        {
            Hotel itemExcluir = Context.Hoteis
        .Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            Context.Entry<Hotel>(itemExcluir).State = EntityState.Deleted;
            Context.SaveChanges();
        }
        public HotelAvaliacao SelecionarHotelAvaliacao(int? Identificador)
        {
            IQueryable<HotelAvaliacao> query = Context.HotelAvaliacoes
;
            if (Identificador.HasValue)
                query = query.Where(d => d.Identificador == Identificador);
            return query.FirstOrDefault();
        }
        public IList<HotelAvaliacao> ListarHotelAvaliacao()
        {
            IQueryable<HotelAvaliacao> query = Context.HotelAvaliacoes
;
            return query.ToList();
        }
        public void SalvarHotelAvaliacao(HotelAvaliacao itemGravar)
        {
            HotelAvaliacao itemBase = Context.HotelAvaliacoes
            .Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            if (itemBase == null)
            {
                itemBase = Context.HotelAvaliacoes.Create();
                Context.Entry<HotelAvaliacao>(itemBase).State = System.Data.Entity.EntityState.Added;
            }
            AtualizarPropriedades<HotelAvaliacao>(itemBase, itemGravar);
            Context.SaveChanges();
            itemGravar.Identificador = itemBase.Identificador;
        }
        public void ExcluirHotelAvaliacao(HotelAvaliacao itemGravar)
        {
            HotelAvaliacao itemExcluir = Context.HotelAvaliacoes
        .Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            Context.Entry<HotelAvaliacao>(itemExcluir).State = EntityState.Deleted;
            Context.SaveChanges();
        }
        public ItemCompra SelecionarItemCompra(int? Identificador)
        {
            IQueryable<ItemCompra> query = Context.ItemCompras.Include("ItemUsuario");

            if (Identificador.HasValue)
                query = query.Where(d => d.Identificador == Identificador);
            return query.FirstOrDefault();
        }
        public IList<ItemCompra> ListarItemCompra()
        {
            IQueryable<ItemCompra> query = Context.ItemCompras
;
            return query.ToList();
        }
        public void SalvarItemCompra(ItemCompra itemGravar)
        {
            ItemCompra itemBase = Context.ItemCompras
            .Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            if (itemBase == null)
            {
                itemBase = Context.ItemCompras.Create();
                Context.Entry<ItemCompra>(itemBase).State = System.Data.Entity.EntityState.Added;
            }
            AtualizarPropriedades<ItemCompra>(itemBase, itemGravar);
            Context.SaveChanges();
            itemGravar.Identificador = itemBase.Identificador;
        }
        public void ExcluirItemCompra(ItemCompra itemGravar)
        {
            ItemCompra itemExcluir = Context.ItemCompras
        .Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            Context.Entry<ItemCompra>(itemExcluir).State = EntityState.Deleted;
            Context.SaveChanges();
        }
        public ListaCompra SelecionarListaCompra(int? Identificador)
        {
            IQueryable<ListaCompra> query = Context.ListaCompras
;
            if (Identificador.HasValue)
                query = query.Where(d => d.Identificador == Identificador);
            return query.FirstOrDefault();
        }
        public IList<ListaCompra> ListarListaCompra()
        {
            IQueryable<ListaCompra> query = Context.ListaCompras
;
            return query.ToList();
        }
        public void SalvarListaCompra(ListaCompra itemGravar)
        {
            ListaCompra itemBase = Context.ListaCompras
            .Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            if (itemBase == null)
            {
                itemBase = Context.ListaCompras.Create();
                Context.Entry<ListaCompra>(itemBase).State = System.Data.Entity.EntityState.Added;
            }
            AtualizarPropriedades<ListaCompra>(itemBase, itemGravar);
            Context.SaveChanges();
            itemGravar.Identificador = itemBase.Identificador;
        }
        public void ExcluirListaCompra(ListaCompra itemGravar)
        {
            ListaCompra itemExcluir = Context.ListaCompras
        .Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            Context.Entry<ListaCompra>(itemExcluir).State = EntityState.Deleted;
            Context.SaveChanges();
        }
        public Loja SelecionarLoja(int? Identificador)
        {
            IQueryable<Loja> query = Context.Lojas
;
            if (Identificador.HasValue)
                query = query.Where(d => d.Identificador == Identificador);
            return query.FirstOrDefault();
        }
        public IList<Loja> ListarLoja()
        {
            IQueryable<Loja> query = Context.Lojas
;
            return query.ToList();
        }
        public void SalvarLoja(Loja itemGravar)
        {
            Loja itemBase = Context.Lojas
.Include("Avaliacoes").Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            if (itemBase == null)
            {
                itemBase = Context.Lojas.Create();
                itemBase.Avaliacoes = new List<AvaliacaoLoja>();
                Context.Entry<Loja>(itemBase).State = System.Data.Entity.EntityState.Added;
            }
            AtualizarPropriedades<Loja>(itemBase, itemGravar);
            foreach (AvaliacaoLoja itemAvaliacaoLoja in new List<AvaliacaoLoja>(itemBase.Avaliacoes))
            {
                if (!itemGravar.Avaliacoes.Where(f => f.Identificador == itemAvaliacaoLoja.Identificador).Any())
                {
                    Context.Entry<AvaliacaoLoja>(itemAvaliacaoLoja).State = EntityState.Deleted;
                }
            }
            foreach (AvaliacaoLoja itemAvaliacaoLoja in new List<AvaliacaoLoja>(itemGravar.Avaliacoes))
            {
                AvaliacaoLoja itemBaseAvaliacaoLoja = !itemAvaliacaoLoja.Identificador.HasValue ? null : itemBase.Avaliacoes.Where(f => f.Identificador == itemAvaliacaoLoja.Identificador).FirstOrDefault();
                if (itemBaseAvaliacaoLoja == null)
                {
                    itemBaseAvaliacaoLoja = Context.AvaliacaoLojas.Create();
                    itemBase.Avaliacoes.Add(itemBaseAvaliacaoLoja);
                }
                AtualizarPropriedades<AvaliacaoLoja>(itemBaseAvaliacaoLoja, itemAvaliacaoLoja);
            }
            Context.SaveChanges();
            itemGravar.Identificador = itemBase.Identificador;
        }
        public void ExcluirLoja(Loja itemGravar)
        {
            Loja itemExcluir = Context.Lojas
        .Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            Context.Entry<Loja>(itemExcluir).State = EntityState.Deleted;
            Context.SaveChanges();
        }
        public Pais SelecionarPais(int? Identificador)
        {
            IQueryable<Pais> query = Context.Paises
;
            if (Identificador.HasValue)
                query = query.Where(d => d.Identificador == Identificador);
            return query.FirstOrDefault();
        }
        public IList<Pais> ListarPais()
        {
            IQueryable<Pais> query = Context.Paises
;
            return query.ToList();
        }
        public void SalvarPais(Pais itemGravar)
        {
            Pais itemBase = Context.Paises
            .Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            if (itemBase == null)
            {
                itemBase = Context.Paises.Create();
                Context.Entry<Pais>(itemBase).State = System.Data.Entity.EntityState.Added;
            }
            AtualizarPropriedades<Pais>(itemBase, itemGravar);
            Context.SaveChanges();
            itemGravar.Identificador = itemBase.Identificador;
        }
        public void ExcluirPais(Pais itemGravar)
        {
            Pais itemExcluir = Context.Paises
        .Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            Context.Entry<Pais>(itemExcluir).State = EntityState.Deleted;
            Context.SaveChanges();
        }
        public ParticipanteViagem SelecionarParticipanteViagem(int? Identificador)
        {
            IQueryable<ParticipanteViagem> query = Context.ParticipanteViagemes
;
            if (Identificador.HasValue)
                query = query.Where(d => d.Identificador == Identificador);
            return query.FirstOrDefault();
        }
        public IList<ParticipanteViagem> ListarParticipanteViagem()
        {
            IQueryable<ParticipanteViagem> query = Context.ParticipanteViagemes
;
            return query.ToList();
        }
        public void SalvarParticipanteViagem(ParticipanteViagem itemGravar)
        {
            ParticipanteViagem itemBase = Context.ParticipanteViagemes
            .Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            if (itemBase == null)
            {
                itemBase = Context.ParticipanteViagemes.Create();
                Context.Entry<ParticipanteViagem>(itemBase).State = System.Data.Entity.EntityState.Added;
            }
            AtualizarPropriedades<ParticipanteViagem>(itemBase, itemGravar);
            Context.SaveChanges();
            itemGravar.Identificador = itemBase.Identificador;
        }
        public void ExcluirParticipanteViagem(ParticipanteViagem itemGravar)
        {
            ParticipanteViagem itemExcluir = Context.ParticipanteViagemes
        .Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            Context.Entry<ParticipanteViagem>(itemExcluir).State = EntityState.Deleted;
            Context.SaveChanges();
        }
        public Posicao SelecionarPosicao(int? Identificador)
        {
            IQueryable<Posicao> query = Context.Posicoes
;
            if (Identificador.HasValue)
                query = query.Where(d => d.Identificador == Identificador);
            return query.FirstOrDefault();
        }
        public IList<Posicao> ListarPosicao()
        {
            IQueryable<Posicao> query = Context.Posicoes
;
            return query.ToList();
        }
        public void SalvarPosicao(Posicao itemGravar)
        {
            Posicao itemBase = Context.Posicoes
            .Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            if (itemBase == null)
            {
                itemBase = Context.Posicoes.Create();
                Context.Entry<Posicao>(itemBase).State = System.Data.Entity.EntityState.Added;
            }
            AtualizarPropriedades<Posicao>(itemBase, itemGravar);
            Context.SaveChanges();
            itemGravar.Identificador = itemBase.Identificador;
        }
        public void ExcluirPosicao(Posicao itemGravar)
        {
            Posicao itemExcluir = Context.Posicoes
        .Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            Context.Entry<Posicao>(itemExcluir).State = EntityState.Deleted;
            Context.SaveChanges();
        }
        public ReabastecimentoGasto SelecionarReabastecimentoGasto(int? Identificador)
        {
            IQueryable<ReabastecimentoGasto> query = Context.ReabastecimentoGastos
;
            if (Identificador.HasValue)
                query = query.Where(d => d.Identificador == Identificador);
            return query.FirstOrDefault();
        }
        public IList<ReabastecimentoGasto> ListarReabastecimentoGasto()
        {
            IQueryable<ReabastecimentoGasto> query = Context.ReabastecimentoGastos
;
            return query.ToList();
        }
        public void SalvarReabastecimentoGasto(ReabastecimentoGasto itemGravar)
        {
            ReabastecimentoGasto itemBase = Context.ReabastecimentoGastos
            .Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            if (itemBase == null)
            {
                itemBase = Context.ReabastecimentoGastos.Create();
                Context.Entry<ReabastecimentoGasto>(itemBase).State = System.Data.Entity.EntityState.Added;
            }
            AtualizarPropriedades<ReabastecimentoGasto>(itemBase, itemGravar);
            Context.SaveChanges();
            itemGravar.Identificador = itemBase.Identificador;
        }
        public void ExcluirReabastecimentoGasto(ReabastecimentoGasto itemGravar)
        {
            ReabastecimentoGasto itemExcluir = Context.ReabastecimentoGastos
        .Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            Context.Entry<ReabastecimentoGasto>(itemExcluir).State = EntityState.Deleted;
            Context.SaveChanges();
        }
        public Refeicao SelecionarRefeicao(int? Identificador)
        {
            IQueryable<Refeicao> query = Context.Refeicoes
;
            if (Identificador.HasValue)
                query = query.Where(d => d.Identificador == Identificador);
            return query.FirstOrDefault();
        }
        public IList<Refeicao> ListarRefeicao()
        {
            IQueryable<Refeicao> query = Context.Refeicoes
;
            return query.ToList();
        }
        public void SalvarRefeicao(Refeicao itemGravar)
        {
            Refeicao itemBase = Context.Refeicoes
.Include("Pedidos").Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            if (itemBase == null)
            {
                itemBase = Context.Refeicoes.Create();
                itemBase.Pedidos = new List<RefeicaoPedido>();
                Context.Entry<Refeicao>(itemBase).State = System.Data.Entity.EntityState.Added;
            }
            AtualizarPropriedades<Refeicao>(itemBase, itemGravar);
            foreach (RefeicaoPedido itemRefeicaoPedido in new List<RefeicaoPedido>(itemBase.Pedidos))
            {
                if (!itemGravar.Pedidos.Where(f => f.Identificador == itemRefeicaoPedido.Identificador).Any())
                {
                    Context.Entry<RefeicaoPedido>(itemRefeicaoPedido).State = EntityState.Deleted;
                }
            }
            foreach (RefeicaoPedido itemRefeicaoPedido in new List<RefeicaoPedido>(itemGravar.Pedidos))
            {
                RefeicaoPedido itemBaseRefeicaoPedido = !itemRefeicaoPedido.Identificador.HasValue ? null : itemBase.Pedidos.Where(f => f.Identificador == itemRefeicaoPedido.Identificador).FirstOrDefault();
                if (itemBaseRefeicaoPedido == null)
                {
                    itemBaseRefeicaoPedido = Context.RefeicaoPedidos.Create();
                    itemBase.Pedidos.Add(itemBaseRefeicaoPedido);
                }
                AtualizarPropriedades<RefeicaoPedido>(itemBaseRefeicaoPedido, itemRefeicaoPedido);
            }
            Context.SaveChanges();
            itemGravar.Identificador = itemBase.Identificador;
        }
        public void ExcluirRefeicao(Refeicao itemGravar)
        {
            Refeicao itemExcluir = Context.Refeicoes
        .Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            Context.Entry<Refeicao>(itemExcluir).State = EntityState.Deleted;
            Context.SaveChanges();
        }
        public RequisicaoAmizade SelecionarRequisicaoAmizade(int? Identificador)
        {
            IQueryable<RequisicaoAmizade> query = Context.RequisicaoAmizades
;
            if (Identificador.HasValue)
                query = query.Where(d => d.Identificador == Identificador);
            return query.FirstOrDefault();
        }
        public IList<RequisicaoAmizade> ListarRequisicaoAmizade()
        {
            IQueryable<RequisicaoAmizade> query = Context.RequisicaoAmizades
;
            return query.ToList();
        }
        public void SalvarRequisicaoAmizade(RequisicaoAmizade itemGravar)
        {
            RequisicaoAmizade itemBase = Context.RequisicaoAmizades
            .Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            if (itemBase == null)
            {
                itemBase = Context.RequisicaoAmizades.Create();
                Context.Entry<RequisicaoAmizade>(itemBase).State = System.Data.Entity.EntityState.Added;
            }
            AtualizarPropriedades<RequisicaoAmizade>(itemBase, itemGravar);
            Context.SaveChanges();
            itemGravar.Identificador = itemBase.Identificador;
        }
        public void ExcluirRequisicaoAmizade(RequisicaoAmizade itemGravar)
        {
            RequisicaoAmizade itemExcluir = Context.RequisicaoAmizades
        .Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            Context.Entry<RequisicaoAmizade>(itemExcluir).State = EntityState.Deleted;
            Context.SaveChanges();
        }
        public Sugestao SelecionarSugestao(int? Identificador)
        {
            IQueryable<Sugestao> query = Context.Sugestoes
;
            if (Identificador.HasValue)
                query = query.Where(d => d.Identificador == Identificador);
            return query.FirstOrDefault();
        }
        public IList<Sugestao> ListarSugestao()
        {
            IQueryable<Sugestao> query = Context.Sugestoes
;
            return query.ToList();
        }
        public void SalvarSugestao(Sugestao itemGravar)
        {
            Sugestao itemBase = Context.Sugestoes
            .Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            if (itemBase == null)
            {
                itemBase = Context.Sugestoes.Create();
                Context.Entry<Sugestao>(itemBase).State = System.Data.Entity.EntityState.Added;
            }
            AtualizarPropriedades<Sugestao>(itemBase, itemGravar);
            Context.SaveChanges();
            itemGravar.Identificador = itemBase.Identificador;
        }
        public void ExcluirSugestao(Sugestao itemGravar)
        {
            Sugestao itemExcluir = Context.Sugestoes
        .Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            Context.Entry<Sugestao>(itemExcluir).State = EntityState.Deleted;
            Context.SaveChanges();
        }
        public Usuario SelecionarUsuario(int? Identificador)
        {
            IQueryable<Usuario> query = Context.Usuarios
;
            if (Identificador.HasValue)
                query = query.Where(d => d.Identificador == Identificador);
            return query.FirstOrDefault();
        }
        public IList<Usuario> ListarUsuario()
        {
            IQueryable<Usuario> query = Context.Usuarios
;
            return query.ToList();
        }
        public void SalvarUsuario(Usuario itemGravar)
        {
            Usuario itemBase = Context.Usuarios
            .Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            if (itemBase == null)
            {
                itemBase = Context.Usuarios.Create();
                Context.Entry<Usuario>(itemBase).State = System.Data.Entity.EntityState.Added;
            }
            AtualizarPropriedades<Usuario>(itemBase, itemGravar);
            Context.SaveChanges();
            itemGravar.Identificador = itemBase.Identificador;
        }
        public void ExcluirUsuario(Usuario itemGravar)
        {
            Usuario itemExcluir = Context.Usuarios
        .Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            Context.Entry<Usuario>(itemExcluir).State = EntityState.Deleted;
            Context.SaveChanges();
        }
        public Viagem SelecionarViagem(int? Identificador)
        {
            IQueryable<Viagem> query = Context.Viagemes
.Include("Participantes").Include("Participantes.ItemUsuario").Include("UsuariosGastos").Include("UsuariosGastos.ItemUsuario");
            if (Identificador.HasValue)
                query = query.Where(d => d.Identificador == Identificador);
            return query.FirstOrDefault();
        }
        public IList<Viagem> ListarViagem()
        {
            IQueryable<Viagem> query = Context.Viagemes
;
            return query.ToList();
        }
        public void SalvarViagem(Viagem itemGravar)
        {
            Viagem itemBase = Context.Viagemes
            .Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            if (itemBase == null)
            {
                itemBase = Context.Viagemes.Create();
                Context.Entry<Viagem>(itemBase).State = System.Data.Entity.EntityState.Added;
            }
            AtualizarPropriedades<Viagem>(itemBase, itemGravar);
            Context.SaveChanges();
            itemGravar.Identificador = itemBase.Identificador;
        }
        public void ExcluirViagem(Viagem itemGravar)
        {
            Viagem itemExcluir = Context.Viagemes
        .Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            Context.Entry<Viagem>(itemExcluir).State = EntityState.Deleted;
            Context.SaveChanges();
        }
        public ViagemAerea SelecionarViagemAerea(int? Identificador)
        {
            IQueryable<ViagemAerea> query = Context.ViagemAereas
;
            if (Identificador.HasValue)
                query = query.Where(d => d.Identificador == Identificador);
            return query.FirstOrDefault();
        }
        public IList<ViagemAerea> ListarViagemAerea()
        {
            IQueryable<ViagemAerea> query = Context.ViagemAereas
;
            return query.ToList();
        }
        public void SalvarViagemAerea(ViagemAerea itemGravar)
        {
            Dictionary<ViagemAereaAeroporto, ViagemAereaAeroporto> DeParaCodigos = new Dictionary<ViagemAereaAeroporto, ViagemAereaAeroporto>();
            ViagemAerea itemBase = Context.ViagemAereas
.Include("Aeroportos").Include("Avaliacoes").Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            if (itemBase == null)
            {
                itemBase = Context.ViagemAereas.Create();
                itemBase.Aeroportos = new List<ViagemAereaAeroporto>();
                itemBase.Avaliacoes = new List<AvaliacaoAerea>();
                Context.Entry<ViagemAerea>(itemBase).State = System.Data.Entity.EntityState.Added;
            }
            AtualizarPropriedades<ViagemAerea>(itemBase, itemGravar);
            foreach (ViagemAereaAeroporto itemViagemAereaAeroporto in new List<ViagemAereaAeroporto>(itemBase.Aeroportos))
            {
                if (!itemGravar.Aeroportos.Where(f => f.Identificador == itemViagemAereaAeroporto.Identificador).Any())
                {
                    Context.Entry<ViagemAereaAeroporto>(itemViagemAereaAeroporto).State = EntityState.Deleted;
                }
            }
            foreach (ViagemAereaAeroporto itemViagemAereaAeroporto in new List<ViagemAereaAeroporto>(itemGravar.Aeroportos))
            {
                ViagemAereaAeroporto itemBaseViagemAereaAeroporto = !itemViagemAereaAeroporto.Identificador.HasValue ? null : itemBase.Aeroportos.Where(f => f.Identificador == itemViagemAereaAeroporto.Identificador).FirstOrDefault();
                if (itemBaseViagemAereaAeroporto == null)
                {
                    itemBaseViagemAereaAeroporto = Context.ViagemAereaAeroportos.Create();
                    itemBase.Aeroportos.Add(itemBaseViagemAereaAeroporto);
                }
                AtualizarPropriedades<ViagemAereaAeroporto>(itemBaseViagemAereaAeroporto, itemViagemAereaAeroporto);
                DeParaCodigos.Add(itemViagemAereaAeroporto, itemBaseViagemAereaAeroporto);
            }
            foreach (AvaliacaoAerea itemAvaliacaoAerea in new List<AvaliacaoAerea>(itemBase.Avaliacoes))
            {
                if (!itemGravar.Avaliacoes.Where(f => f.Identificador == itemAvaliacaoAerea.Identificador).Any())
                {
                    Context.Entry<AvaliacaoAerea>(itemAvaliacaoAerea).State = EntityState.Deleted;
                }
            }
            foreach (AvaliacaoAerea itemAvaliacaoAerea in new List<AvaliacaoAerea>(itemGravar.Avaliacoes))
            {
                AvaliacaoAerea itemBaseAvaliacaoAerea = !itemAvaliacaoAerea.Identificador.HasValue ? null : itemBase.Avaliacoes.Where(f => f.Identificador == itemAvaliacaoAerea.Identificador).FirstOrDefault();
                if (itemBaseAvaliacaoAerea == null)
                {
                    itemBaseAvaliacaoAerea = Context.AvaliacaoAereas.Create();
                    itemBase.Avaliacoes.Add(itemBaseAvaliacaoAerea);
                }
                AtualizarPropriedades<AvaliacaoAerea>(itemBaseAvaliacaoAerea, itemAvaliacaoAerea);
            }
            Context.SaveChanges();
            itemGravar.Identificador = itemBase.Identificador;
            foreach (var ParDados in DeParaCodigos)
                ParDados.Key.Identificador = ParDados.Value.Identificador;
        }
        public void ExcluirViagemAerea(ViagemAerea itemGravar)
        {
            ViagemAerea itemExcluir = Context.ViagemAereas
        .Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            Context.Entry<ViagemAerea>(itemExcluir).State = EntityState.Deleted;
            Context.SaveChanges();
        }
        public ViagemAereaAeroporto SelecionarViagemAereaAeroporto(int? Identificador)
        {
            IQueryable<ViagemAereaAeroporto> query = Context.ViagemAereaAeroportos
;
            if (Identificador.HasValue)
                query = query.Where(d => d.Identificador == Identificador);
            return query.FirstOrDefault();
        }
        public IList<ViagemAereaAeroporto> ListarViagemAereaAeroporto()
        {
            IQueryable<ViagemAereaAeroporto> query = Context.ViagemAereaAeroportos
;
            return query.ToList();
        }
        public void SalvarViagemAereaAeroporto(ViagemAereaAeroporto itemGravar)
        {
            ViagemAereaAeroporto itemBase = Context.ViagemAereaAeroportos
            .Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            if (itemBase == null)
            {
                itemBase = Context.ViagemAereaAeroportos.Create();
                Context.Entry<ViagemAereaAeroporto>(itemBase).State = System.Data.Entity.EntityState.Added;
            }
            AtualizarPropriedades<ViagemAereaAeroporto>(itemBase, itemGravar);
            Context.SaveChanges();
            itemGravar.Identificador = itemBase.Identificador;
        }
        public void ExcluirViagemAereaAeroporto(ViagemAereaAeroporto itemGravar)
        {
            ViagemAereaAeroporto itemExcluir = Context.ViagemAereaAeroportos
        .Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            Context.Entry<ViagemAereaAeroporto>(itemExcluir).State = EntityState.Deleted;
            Context.SaveChanges();
        }
        public RefeicaoPedido SelecionarRefeicaoPedido(int? Identificador)
        {
            IQueryable<RefeicaoPedido> query = Context.RefeicaoPedidos
;
            if (Identificador.HasValue)
                query = query.Where(d => d.Identificador == Identificador);
            return query.FirstOrDefault();
        }
        public IList<RefeicaoPedido> ListarRefeicaoPedido()
        {
            IQueryable<RefeicaoPedido> query = Context.RefeicaoPedidos
;
            return query.ToList();
        }
        public void SalvarRefeicaoPedido(RefeicaoPedido itemGravar)
        {
            RefeicaoPedido itemBase = Context.RefeicaoPedidos
            .Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            if (itemBase == null)
            {
                itemBase = Context.RefeicaoPedidos.Create();
                Context.Entry<RefeicaoPedido>(itemBase).State = System.Data.Entity.EntityState.Added;
            }
            AtualizarPropriedades<RefeicaoPedido>(itemBase, itemGravar);
            Context.SaveChanges();
            itemGravar.Identificador = itemBase.Identificador;
        }
        public void ExcluirRefeicaoPedido(RefeicaoPedido itemGravar)
        {
            RefeicaoPedido itemExcluir = Context.RefeicaoPedidos
        .Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            Context.Entry<RefeicaoPedido>(itemExcluir).State = EntityState.Deleted;
            Context.SaveChanges();
        }
        public AvaliacaoLoja SelecionarAvaliacaoLoja(int? Identificador)
        {
            IQueryable<AvaliacaoLoja> query = Context.AvaliacaoLojas
;
            if (Identificador.HasValue)
                query = query.Where(d => d.Identificador == Identificador);
            return query.FirstOrDefault();
        }
        public IList<AvaliacaoLoja> ListarAvaliacaoLoja()
        {
            IQueryable<AvaliacaoLoja> query = Context.AvaliacaoLojas
;
            return query.ToList();
        }
        public void SalvarAvaliacaoLoja(AvaliacaoLoja itemGravar)
        {
            AvaliacaoLoja itemBase = Context.AvaliacaoLojas
            .Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            if (itemBase == null)
            {
                itemBase = Context.AvaliacaoLojas.Create();
                Context.Entry<AvaliacaoLoja>(itemBase).State = System.Data.Entity.EntityState.Added;
            }
            AtualizarPropriedades<AvaliacaoLoja>(itemBase, itemGravar);
            Context.SaveChanges();
            itemGravar.Identificador = itemBase.Identificador;
        }
        public void ExcluirAvaliacaoLoja(AvaliacaoLoja itemGravar)
        {
            AvaliacaoLoja itemExcluir = Context.AvaliacaoLojas
        .Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            Context.Entry<AvaliacaoLoja>(itemExcluir).State = EntityState.Deleted;
            Context.SaveChanges();
        }
        public UsuarioGasto SelecionarUsuarioGasto(int? Identificador)
        {
            IQueryable<UsuarioGasto> query = Context.UsuarioGastos
;
            if (Identificador.HasValue)
                query = query.Where(d => d.Identificador == Identificador);
            return query.FirstOrDefault();
        }
        public IList<UsuarioGasto> ListarUsuarioGasto()
        {
            IQueryable<UsuarioGasto> query = Context.UsuarioGastos
;
            return query.ToList();
        }
        public void SalvarUsuarioGasto(UsuarioGasto itemGravar)
        {
            UsuarioGasto itemBase = Context.UsuarioGastos
            .Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            if (itemBase == null)
            {
                itemBase = Context.UsuarioGastos.Create();
                Context.Entry<UsuarioGasto>(itemBase).State = System.Data.Entity.EntityState.Added;
            }
            AtualizarPropriedades<UsuarioGasto>(itemBase, itemGravar);
            Context.SaveChanges();
            itemGravar.Identificador = itemBase.Identificador;
        }
        public void ExcluirUsuarioGasto(UsuarioGasto itemGravar)
        {
            UsuarioGasto itemExcluir = Context.UsuarioGastos
        .Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            Context.Entry<UsuarioGasto>(itemExcluir).State = EntityState.Deleted;
            Context.SaveChanges();
        }
        public HotelEvento SelecionarHotelEvento(int? Identificador)
        {
            IQueryable<HotelEvento> query = Context.HotelEventos
;
            if (Identificador.HasValue)
                query = query.Where(d => d.Identificador == Identificador);
            return query.FirstOrDefault();
        }
        public IList<HotelEvento> ListarHotelEvento()
        {
            IQueryable<HotelEvento> query = Context.HotelEventos
;
            return query.ToList();
        }
        public void SalvarHotelEvento(HotelEvento itemGravar)
        {
            HotelEvento itemBase = Context.HotelEventos
            .Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            if (itemBase == null)
            {
                itemBase = Context.HotelEventos.Create();
                Context.Entry<HotelEvento>(itemBase).State = System.Data.Entity.EntityState.Added;
            }
            AtualizarPropriedades<HotelEvento>(itemBase, itemGravar);
            Context.SaveChanges();
            itemGravar.Identificador = itemBase.Identificador;
        }
        public void ExcluirHotelEvento(HotelEvento itemGravar)
        {
            HotelEvento itemExcluir = Context.HotelEventos
        .Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            Context.Entry<HotelEvento>(itemExcluir).State = EntityState.Deleted;
            Context.SaveChanges();
        }
        public void SalvarViagem_Completa(Viagem itemGravar)
        {
            Viagem itemBase = Context.Viagemes
.Include("Participantes").Include("UsuariosGastos").Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            if (itemBase == null)
            {
                itemBase = Context.Viagemes.Create();
                itemBase.Participantes = new List<ParticipanteViagem>();
                itemBase.UsuariosGastos = new List<UsuarioGasto>();
                Context.Entry<Viagem>(itemBase).State = System.Data.Entity.EntityState.Added;
            }
            AtualizarPropriedades<Viagem>(itemBase, itemGravar);
            foreach (ParticipanteViagem itemParticipanteViagem in new List<ParticipanteViagem>(itemBase.Participantes))
            {
                if (!itemGravar.Participantes.Where(f => f.Identificador == itemParticipanteViagem.Identificador).Any())
                {
                    Context.Entry<ParticipanteViagem>(itemParticipanteViagem).State = EntityState.Deleted;
                }
            }
            foreach (ParticipanteViagem itemParticipanteViagem in new List<ParticipanteViagem>(itemGravar.Participantes))
            {
                ParticipanteViagem itemBaseParticipanteViagem = !itemParticipanteViagem.Identificador.HasValue ? null : itemBase.Participantes.Where(f => f.Identificador == itemParticipanteViagem.Identificador).FirstOrDefault();
                if (itemBaseParticipanteViagem == null)
                {
                    itemBaseParticipanteViagem = Context.ParticipanteViagemes.Create();
                    itemBase.Participantes.Add(itemBaseParticipanteViagem);
                }
                AtualizarPropriedades<ParticipanteViagem>(itemBaseParticipanteViagem, itemParticipanteViagem);
            }
            foreach (UsuarioGasto itemUsuarioGasto in new List<UsuarioGasto>(itemBase.UsuariosGastos))
            {
                if (!itemGravar.UsuariosGastos.Where(f => f.Identificador == itemUsuarioGasto.Identificador).Any())
                {
                    Context.Entry<UsuarioGasto>(itemUsuarioGasto).State = EntityState.Deleted;
                }
            }
            foreach (UsuarioGasto itemUsuarioGasto in new List<UsuarioGasto>(itemGravar.UsuariosGastos))
            {
                UsuarioGasto itemBaseUsuarioGasto = !itemUsuarioGasto.Identificador.HasValue ? null : itemBase.UsuariosGastos.Where(f => f.Identificador == itemUsuarioGasto.Identificador).FirstOrDefault();
                if (itemBaseUsuarioGasto == null)
                {
                    itemBaseUsuarioGasto = Context.UsuarioGastos.Create();
                    itemBase.UsuariosGastos.Add(itemBaseUsuarioGasto);
                }
                AtualizarPropriedades<UsuarioGasto>(itemBaseUsuarioGasto, itemUsuarioGasto);
            }
            Context.SaveChanges();
            itemGravar.Identificador = itemBase.Identificador;
        }
        public Foto SelecionarFoto_Completa(int? Identificador)
        {
            IQueryable<Foto> query = Context.Fotos
.Include("Atracoes").Include("Atracoes.ItemAtracao").Include("Hoteis").Include("Hoteis.ItemHotel").Include("ItensCompra").Include("ItensCompra.ItemItemCompra").Include("Refeicoes").Include("Refeicoes.ItemRefeicao");
            if (Identificador.HasValue)
                query = query.Where(d => d.Identificador == Identificador);
            return query.FirstOrDefault();
        }
        public void SalvarFoto_Completa(Foto itemGravar)
        {
            Foto itemBase = Context.Fotos
.Include("Atracoes").Include("Hoteis").Include("Refeicoes").Include("ItensCompra").Include("FotoUsuarios").Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            if (itemBase == null)
            {
                itemBase = Context.Fotos.Create();
                itemBase.Atracoes = new List<FotoAtracao>();
                itemBase.Hoteis = new List<FotoHotel>();
                itemBase.Refeicoes = new List<FotoRefeicao>();
                itemBase.ItensCompra = new List<FotoItemCompra>();
                itemBase.FotoUsuarios = new List<FotoUsuario>();
                Context.Entry<Foto>(itemBase).State = System.Data.Entity.EntityState.Added;
            }
            AtualizarPropriedades<Foto>(itemBase, itemGravar);
            foreach (FotoAtracao itemFotoAtracao in new List<FotoAtracao>(itemBase.Atracoes))
            {
                if (!itemGravar.Atracoes.Where(f => f.Identificador == itemFotoAtracao.Identificador).Any())
                {
                    Context.Entry<FotoAtracao>(itemFotoAtracao).State = EntityState.Deleted;
                }
            }
            foreach (FotoAtracao itemFotoAtracao in new List<FotoAtracao>(itemGravar.Atracoes))
            {
                FotoAtracao itemBaseFotoAtracao = !itemFotoAtracao.Identificador.HasValue ? null : itemBase.Atracoes.Where(f => f.Identificador == itemFotoAtracao.Identificador).FirstOrDefault();
                if (itemBaseFotoAtracao == null)
                {
                    itemBaseFotoAtracao = Context.FotoAtracoes.Create();
                    itemBase.Atracoes.Add(itemBaseFotoAtracao);
                }
                AtualizarPropriedades<FotoAtracao>(itemBaseFotoAtracao, itemFotoAtracao);
            }
            foreach (FotoHotel itemFotoHotel in new List<FotoHotel>(itemBase.Hoteis))
            {
                if (!itemGravar.Hoteis.Where(f => f.Identificador == itemFotoHotel.Identificador).Any())
                {
                    Context.Entry<FotoHotel>(itemFotoHotel).State = EntityState.Deleted;
                }
            }
            foreach (FotoHotel itemFotoHotel in new List<FotoHotel>(itemGravar.Hoteis))
            {
                FotoHotel itemBaseFotoHotel = !itemFotoHotel.Identificador.HasValue ? null : itemBase.Hoteis.Where(f => f.Identificador == itemFotoHotel.Identificador).FirstOrDefault();
                if (itemBaseFotoHotel == null)
                {
                    itemBaseFotoHotel = Context.FotoHoteis.Create();
                    itemBase.Hoteis.Add(itemBaseFotoHotel);
                }
                AtualizarPropriedades<FotoHotel>(itemBaseFotoHotel, itemFotoHotel);
            }
            foreach (FotoRefeicao itemFotoRefeicao in new List<FotoRefeicao>(itemBase.Refeicoes))
            {
                if (!itemGravar.Refeicoes.Where(f => f.Identificador == itemFotoRefeicao.Identificador).Any())
                {
                    Context.Entry<FotoRefeicao>(itemFotoRefeicao).State = EntityState.Deleted;
                }
            }
            foreach (FotoRefeicao itemFotoRefeicao in new List<FotoRefeicao>(itemGravar.Refeicoes))
            {
                FotoRefeicao itemBaseFotoRefeicao = !itemFotoRefeicao.Identificador.HasValue ? null : itemBase.Refeicoes.Where(f => f.Identificador == itemFotoRefeicao.Identificador).FirstOrDefault();
                if (itemBaseFotoRefeicao == null)
                {
                    itemBaseFotoRefeicao = Context.FotoRefeicoes.Create();
                    itemBase.Refeicoes.Add(itemBaseFotoRefeicao);
                }
                AtualizarPropriedades<FotoRefeicao>(itemBaseFotoRefeicao, itemFotoRefeicao);
            }
            foreach (FotoItemCompra itemFotoItemCompra in new List<FotoItemCompra>(itemBase.ItensCompra))
            {
                if (!itemGravar.ItensCompra.Where(f => f.Identificador == itemFotoItemCompra.Identificador).Any())
                {
                    Context.Entry<FotoItemCompra>(itemFotoItemCompra).State = EntityState.Deleted;
                }
            }
            foreach (FotoItemCompra itemFotoItemCompra in new List<FotoItemCompra>(itemGravar.ItensCompra))
            {
                FotoItemCompra itemBaseFotoItemCompra = !itemFotoItemCompra.Identificador.HasValue ? null : itemBase.ItensCompra.Where(f => f.Identificador == itemFotoItemCompra.Identificador).FirstOrDefault();
                if (itemBaseFotoItemCompra == null)
                {
                    itemBaseFotoItemCompra = Context.FotoItemCompras.Create();
                    itemBase.ItensCompra.Add(itemBaseFotoItemCompra);
                }
                AtualizarPropriedades<FotoItemCompra>(itemBaseFotoItemCompra, itemFotoItemCompra);
            }
            
            Context.SaveChanges();
            itemGravar.Identificador = itemBase.Identificador;
        }

        public void AdicionarFotosUsuarios(List<FotoUsuario> fotoUsuarios)
        {
            foreach(var item in fotoUsuarios)
            {
                if (!Context.FotoUsuarios.Where(d => d.IdentificadorUsuario == item.IdentificadorUsuario).Where(d => d.IdentificadorFoto == item.IdentificadorFoto).Any())
                {
                    var itemBase = Context.FotoUsuarios.Create();
                    Context.Entry<FotoUsuario>(itemBase).State = System.Data.Entity.EntityState.Added;
                    AtualizarPropriedades<FotoUsuario>(itemBase, item);

                }

            }
            Context.SaveChanges();

        }

        public void SalvarAtracao_Completo(Atracao itemGravar)
        {
            Atracao itemBase = Context.Atracoes
.Include("Avaliacoes").Include("Fotos").Include("Gastos").Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            if (itemBase == null)
            {
                itemBase = Context.Atracoes.Create();
                itemBase.Avaliacoes = new List<AvaliacaoAtracao>();
                itemBase.Fotos = new List<FotoAtracao>();
                itemBase.Gastos = new List<GastoAtracao>();
                Context.Entry<Atracao>(itemBase).State = System.Data.Entity.EntityState.Added;
            }
            AtualizarPropriedades<Atracao>(itemBase, itemGravar);
            foreach (AvaliacaoAtracao itemAvaliacaoAtracao in new List<AvaliacaoAtracao>(itemBase.Avaliacoes))
            {
                if (!itemGravar.Avaliacoes.Where(f => f.Identificador == itemAvaliacaoAtracao.Identificador).Any())
                {
                    Context.Entry<AvaliacaoAtracao>(itemAvaliacaoAtracao).State = EntityState.Deleted;
                }
            }
            foreach (AvaliacaoAtracao itemAvaliacaoAtracao in new List<AvaliacaoAtracao>(itemGravar.Avaliacoes))
            {
                AvaliacaoAtracao itemBaseAvaliacaoAtracao = !itemAvaliacaoAtracao.Identificador.HasValue ? null : itemBase.Avaliacoes.Where(f => f.Identificador == itemAvaliacaoAtracao.Identificador).FirstOrDefault();
                if (itemBaseAvaliacaoAtracao == null)
                {
                    itemBaseAvaliacaoAtracao = Context.AvaliacaoAtracoes.Create();
                    itemBase.Avaliacoes.Add(itemBaseAvaliacaoAtracao);
                }
                AtualizarPropriedades<AvaliacaoAtracao>(itemBaseAvaliacaoAtracao, itemAvaliacaoAtracao);
            }
            foreach (FotoAtracao itemFotoAtracao in new List<FotoAtracao>(itemBase.Fotos))
            {
                if (!itemGravar.Fotos.Where(f => f.Identificador == itemFotoAtracao.Identificador).Any())
                {
                    Context.Entry<FotoAtracao>(itemFotoAtracao).State = EntityState.Deleted;
                }
            }
            foreach (FotoAtracao itemFotoAtracao in new List<FotoAtracao>(itemGravar.Fotos))
            {
                FotoAtracao itemBaseFotoAtracao = !itemFotoAtracao.Identificador.HasValue ? null : itemBase.Fotos.Where(f => f.Identificador == itemFotoAtracao.Identificador).FirstOrDefault();
                if (itemBaseFotoAtracao == null)
                {
                    itemBaseFotoAtracao = Context.FotoAtracoes.Create();
                    itemBase.Fotos.Add(itemBaseFotoAtracao);
                }
                AtualizarPropriedades<FotoAtracao>(itemBaseFotoAtracao, itemFotoAtracao);
            }
            foreach (GastoAtracao itemGastoAtracao in new List<GastoAtracao>(itemBase.Gastos))
            {
                if (!itemGravar.Gastos.Where(f => f.Identificador == itemGastoAtracao.Identificador).Any())
                {
                    Context.Entry<GastoAtracao>(itemGastoAtracao).State = EntityState.Deleted;
                }
            }
            foreach (GastoAtracao itemGastoAtracao in new List<GastoAtracao>(itemGravar.Gastos))
            {
                GastoAtracao itemBaseGastoAtracao = !itemGastoAtracao.Identificador.HasValue ? null : itemBase.Gastos.Where(f => f.Identificador == itemGastoAtracao.Identificador).FirstOrDefault();
                if (itemBaseGastoAtracao == null)
                {
                    itemBaseGastoAtracao = Context.GastoAtracoes.Create();
                    itemBase.Gastos.Add(itemBaseGastoAtracao);
                }
                AtualizarPropriedades<GastoAtracao>(itemBaseGastoAtracao, itemGastoAtracao);
            }
            Context.SaveChanges();
            itemGravar.Identificador = itemBase.Identificador;
        }
        public Atracao SelecionarAtracao_Completo(int? Identificador)
        {
            IQueryable<Atracao> query = Context.Atracoes
.Include("Avaliacoes").Include("Fotos").Include("Fotos.ItemFoto").Include("Gastos").Include("Gastos.ItemGasto").Include("Gastos.ItemGasto.ItemUsuario").Include("ItemAtracaoPai");
            if (Identificador.HasValue)
                query = query.Where(d => d.Identificador == Identificador);
            return query.FirstOrDefault();
        }
        public GastoAtracao SelecionarGastoAtracao(int? Identificador)
        {
            IQueryable<GastoAtracao> query = Context.GastoAtracoes
;
            if (Identificador.HasValue)
                query = query.Where(d => d.Identificador == Identificador);
            return query.FirstOrDefault();
        }
        public IList<GastoAtracao> ListarGastoAtracao()
        {
            IQueryable<GastoAtracao> query = Context.GastoAtracoes
;
            return query.ToList();
        }
        public void SalvarGastoAtracao(GastoAtracao itemGravar)
        {

            GastoAtracao itemBase = Context.GastoAtracoes
            .Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            if (itemBase == null)
            {
                itemBase = Context.GastoAtracoes.Create();
                Context.Entry<GastoAtracao>(itemBase).State = System.Data.Entity.EntityState.Added;
            }
            AtualizarPropriedades<GastoAtracao>(itemBase, itemGravar);
            Context.SaveChanges();
            itemGravar.Identificador = itemBase.Identificador;
        }
        public void ExcluirGastoAtracao(GastoAtracao itemGravar)
        {
            GastoAtracao itemExcluir = Context.GastoAtracoes
        .Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            Context.Entry<GastoAtracao>(itemExcluir).State = EntityState.Deleted;
            Context.SaveChanges();
        }
        public GastoDividido SelecionarGastoDividido(int? Identificador)
        {
            IQueryable<GastoDividido> query = Context.GastoDivididos
;
            if (Identificador.HasValue)
                query = query.Where(d => d.Identificador == Identificador);
            return query.FirstOrDefault();
        }
        public IList<GastoDividido> ListarGastoDividido()
        {
            IQueryable<GastoDividido> query = Context.GastoDivididos
;
            return query.ToList();
        }
        public void SalvarGastoDividido(GastoDividido itemGravar)
        {
            GastoDividido itemBase = Context.GastoDivididos
            .Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            if (itemBase == null)
            {
                itemBase = Context.GastoDivididos.Create();
                Context.Entry<GastoDividido>(itemBase).State = System.Data.Entity.EntityState.Added;
            }
            AtualizarPropriedades<GastoDividido>(itemBase, itemGravar);
            Context.SaveChanges();
            itemGravar.Identificador = itemBase.Identificador;
        }
        public void ExcluirGastoDividido(GastoDividido itemGravar)
        {
            GastoDividido itemExcluir = Context.GastoDivididos
        .Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            Context.Entry<GastoDividido>(itemExcluir).State = EntityState.Deleted;
            Context.SaveChanges();
        }
        public void SalvarGasto_Completo(Gasto itemGravar)
        {
            Gasto itemBase = Context.Gastos
.Include("Alugueis").Include("Atracoes").Include("Hoteis").Include("Refeicoes").Include("Usuarios").Include("ViagenAereas").Include("Reabastecimentos").Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            if (itemBase == null)
            {
                itemBase = Context.Gastos.Create();
                itemBase.Alugueis = new List<AluguelGasto>();
                itemBase.Atracoes = new List<GastoAtracao>();
                itemBase.Hoteis = new List<GastoHotel>();
                itemBase.Refeicoes = new List<GastoRefeicao>();
                itemBase.Usuarios = new List<GastoDividido>();
                itemBase.ViagenAereas = new List<GastoViagemAerea>();
                itemBase.Reabastecimentos = new List<ReabastecimentoGasto>();
                Context.Entry<Gasto>(itemBase).State = System.Data.Entity.EntityState.Added;
            }
            AtualizarPropriedades<Gasto>(itemBase, itemGravar);
            foreach (AluguelGasto itemAluguelGasto in new List<AluguelGasto>(itemBase.Alugueis))
            {
                if (!itemGravar.Alugueis.Where(f => f.Identificador == itemAluguelGasto.Identificador).Any())
                {
                    Context.Entry<AluguelGasto>(itemAluguelGasto).State = EntityState.Deleted;
                }
            }
            foreach (AluguelGasto itemAluguelGasto in new List<AluguelGasto>(itemGravar.Alugueis))
            {
                AluguelGasto itemBaseAluguelGasto = !itemAluguelGasto.Identificador.HasValue ? null : itemBase.Alugueis.Where(f => f.Identificador == itemAluguelGasto.Identificador).FirstOrDefault();
                if (itemBaseAluguelGasto == null)
                {
                    itemBaseAluguelGasto = Context.AluguelGastos.Create();
                    itemBase.Alugueis.Add(itemBaseAluguelGasto);
                }
                AtualizarPropriedades<AluguelGasto>(itemBaseAluguelGasto, itemAluguelGasto);
            }
            foreach (GastoAtracao itemGastoAtracao in new List<GastoAtracao>(itemBase.Atracoes))
            {
                if (!itemGravar.Atracoes.Where(f => f.Identificador == itemGastoAtracao.Identificador).Any())
                {
                    Context.Entry<GastoAtracao>(itemGastoAtracao).State = EntityState.Deleted;
                }
            }
            foreach (GastoAtracao itemGastoAtracao in new List<GastoAtracao>(itemGravar.Atracoes))
            {
                GastoAtracao itemBaseGastoAtracao = !itemGastoAtracao.Identificador.HasValue ? null : itemBase.Atracoes.Where(f => f.Identificador == itemGastoAtracao.Identificador).FirstOrDefault();
                if (itemBaseGastoAtracao == null)
                {
                    itemBaseGastoAtracao = Context.GastoAtracoes.Create();
                    itemBase.Atracoes.Add(itemBaseGastoAtracao);
                }
                AtualizarPropriedades<GastoAtracao>(itemBaseGastoAtracao, itemGastoAtracao);
            }
            
            foreach (GastoHotel itemGastoHotel in new List<GastoHotel>(itemBase.Hoteis))
            {
                if (!itemGravar.Hoteis.Where(f => f.Identificador == itemGastoHotel.Identificador).Any())
                {
                    Context.Entry<GastoHotel>(itemGastoHotel).State = EntityState.Deleted;
                }
            }
            foreach (GastoHotel itemGastoHotel in new List<GastoHotel>(itemGravar.Hoteis))
            {
                GastoHotel itemBaseGastoHotel = !itemGastoHotel.Identificador.HasValue ? null : itemBase.Hoteis.Where(f => f.Identificador == itemGastoHotel.Identificador).FirstOrDefault();
                if (itemBaseGastoHotel == null)
                {
                    itemBaseGastoHotel = Context.GastoHoteis.Create();
                    itemBase.Hoteis.Add(itemBaseGastoHotel);
                }
                AtualizarPropriedades<GastoHotel>(itemBaseGastoHotel, itemGastoHotel);
            }
            foreach (GastoRefeicao itemGastoRefeicao in new List<GastoRefeicao>(itemBase.Refeicoes))
            {
                if (!itemGravar.Refeicoes.Where(f => f.Identificador == itemGastoRefeicao.Identificador).Any())
                {
                    Context.Entry<GastoRefeicao>(itemGastoRefeicao).State = EntityState.Deleted;
                }
            }
            foreach (GastoRefeicao itemGastoRefeicao in new List<GastoRefeicao>(itemGravar.Refeicoes))
            {
                GastoRefeicao itemBaseGastoRefeicao = !itemGastoRefeicao.Identificador.HasValue ? null : itemBase.Refeicoes.Where(f => f.Identificador == itemGastoRefeicao.Identificador).FirstOrDefault();
                if (itemBaseGastoRefeicao == null)
                {
                    itemBaseGastoRefeicao = Context.GastoRefeicoes.Create();
                    itemBase.Refeicoes.Add(itemBaseGastoRefeicao);
                }
                AtualizarPropriedades<GastoRefeicao>(itemBaseGastoRefeicao, itemGastoRefeicao);
            }
            foreach (GastoDividido itemGastoDividido in new List<GastoDividido>(itemBase.Usuarios))
            {
                if (!itemGravar.Usuarios.Where(f => f.Identificador == itemGastoDividido.Identificador).Any())
                {
                    Context.Entry<GastoDividido>(itemGastoDividido).State = EntityState.Deleted;
                }
            }
            foreach (GastoDividido itemGastoDividido in new List<GastoDividido>(itemGravar.Usuarios))
            {
                GastoDividido itemBaseGastoDividido = !itemGastoDividido.Identificador.HasValue ? null : itemBase.Usuarios.Where(f => f.Identificador == itemGastoDividido.Identificador).FirstOrDefault();
                if (itemBaseGastoDividido == null)
                {
                    itemBaseGastoDividido = Context.GastoDivididos.Create();
                    itemBase.Usuarios.Add(itemBaseGastoDividido);
                }
                AtualizarPropriedades<GastoDividido>(itemBaseGastoDividido, itemGastoDividido);
            }
            foreach (GastoViagemAerea itemGastoViagemAerea in new List<GastoViagemAerea>(itemBase.ViagenAereas))
            {
                if (!itemGravar.ViagenAereas.Where(f => f.Identificador == itemGastoViagemAerea.Identificador).Any())
                {
                    Context.Entry<GastoViagemAerea>(itemGastoViagemAerea).State = EntityState.Deleted;
                }
            }
            foreach (GastoViagemAerea itemGastoViagemAerea in new List<GastoViagemAerea>(itemGravar.ViagenAereas))
            {
                GastoViagemAerea itemBaseGastoViagemAerea = !itemGastoViagemAerea.Identificador.HasValue ? null : itemBase.ViagenAereas.Where(f => f.Identificador == itemGastoViagemAerea.Identificador).FirstOrDefault();
                if (itemBaseGastoViagemAerea == null)
                {
                    itemBaseGastoViagemAerea = Context.GastoViagemAereas.Create();
                    itemBase.ViagenAereas.Add(itemBaseGastoViagemAerea);
                }
                AtualizarPropriedades<GastoViagemAerea>(itemBaseGastoViagemAerea, itemGastoViagemAerea);
            }
            foreach (ReabastecimentoGasto itemReabastecimentoGasto in new List<ReabastecimentoGasto>(itemBase.Reabastecimentos))
            {
                if (!itemGravar.Reabastecimentos.Where(f => f.Identificador == itemReabastecimentoGasto.Identificador).Any())
                {
                    Context.Entry<ReabastecimentoGasto>(itemReabastecimentoGasto).State = EntityState.Deleted;
                }
            }
            foreach (ReabastecimentoGasto itemReabastecimentoGasto in new List<ReabastecimentoGasto>(itemGravar.Reabastecimentos))
            {
                ReabastecimentoGasto itemBaseReabastecimentoGasto = !itemReabastecimentoGasto.Identificador.HasValue ? null : itemBase.Reabastecimentos.Where(f => f.Identificador == itemReabastecimentoGasto.Identificador).FirstOrDefault();
                if (itemBaseReabastecimentoGasto == null)
                {
                    itemBaseReabastecimentoGasto = Context.ReabastecimentoGastos.Create();
                    itemBase.Reabastecimentos.Add(itemBaseReabastecimentoGasto);
                }
                AtualizarPropriedades<ReabastecimentoGasto>(itemBaseReabastecimentoGasto, itemReabastecimentoGasto);
            }
            Context.SaveChanges();
            itemGravar.Identificador = itemBase.Identificador;
        }
        public Gasto SelecionarGasto_Completo(int? Identificador)
        {
            IQueryable<Gasto> query = Context.Gastos
.Include("Alugueis").Include("Atracoes").Include("Hoteis").Include("Refeicoes").Include("Usuarios").Include("ViagenAereas").Include("Reabastecimentos");
            if (Identificador.HasValue)
                query = query.Where(d => d.Identificador == Identificador);
            return query.FirstOrDefault();
        }
        public Refeicao SelecionarRefeicao_Completa(int? Identificador)
        {
            IQueryable<Refeicao> query = Context.Refeicoes
.Include("Fotos").Include("Fotos.ItemFoto").Include("Gastos").Include("Gastos.ItemGasto").Include("Gastos.ItemGasto.ItemUsuario").Include("Pedidos").Include("Pedidos.ItemUsuario").Include("ItemAtracao");
            if (Identificador.HasValue)
                query = query.Where(d => d.Identificador == Identificador);
            return query.FirstOrDefault();
        }
        public void SalvarRefeicao_Completo(Refeicao itemGravar)
        {
            Refeicao itemBase = Context.Refeicoes
.Include("Fotos").Include("Gastos").Include("Pedidos").Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            if (itemBase == null)
            {
                itemBase = Context.Refeicoes.Create();
                itemBase.Fotos = new List<FotoRefeicao>();
                itemBase.Gastos = new List<GastoRefeicao>();
                itemBase.Pedidos = new List<RefeicaoPedido>();
                Context.Entry<Refeicao>(itemBase).State = System.Data.Entity.EntityState.Added;
            }
            AtualizarPropriedades<Refeicao>(itemBase, itemGravar);
            foreach (FotoRefeicao itemFotoRefeicao in new List<FotoRefeicao>(itemBase.Fotos))
            {
                if (!itemGravar.Fotos.Where(f => f.Identificador == itemFotoRefeicao.Identificador).Any())
                {
                    Context.Entry<FotoRefeicao>(itemFotoRefeicao).State = EntityState.Deleted;
                }
            }
            foreach (FotoRefeicao itemFotoRefeicao in new List<FotoRefeicao>(itemGravar.Fotos))
            {
                FotoRefeicao itemBaseFotoRefeicao = !itemFotoRefeicao.Identificador.HasValue ? null : itemBase.Fotos.Where(f => f.Identificador == itemFotoRefeicao.Identificador).FirstOrDefault();
                if (itemBaseFotoRefeicao == null)
                {
                    itemBaseFotoRefeicao = Context.FotoRefeicoes.Create();
                    itemBase.Fotos.Add(itemBaseFotoRefeicao);
                }
                AtualizarPropriedades<FotoRefeicao>(itemBaseFotoRefeicao, itemFotoRefeicao);
            }
            foreach (GastoRefeicao itemGastoRefeicao in new List<GastoRefeicao>(itemBase.Gastos))
            {
                if (!itemGravar.Gastos.Where(f => f.Identificador == itemGastoRefeicao.Identificador).Any())
                {
                    Context.Entry<GastoRefeicao>(itemGastoRefeicao).State = EntityState.Deleted;
                }
            }
            foreach (GastoRefeicao itemGastoRefeicao in new List<GastoRefeicao>(itemGravar.Gastos))
            {
                GastoRefeicao itemBaseGastoRefeicao = !itemGastoRefeicao.Identificador.HasValue ? null : itemBase.Gastos.Where(f => f.Identificador == itemGastoRefeicao.Identificador).FirstOrDefault();
                if (itemBaseGastoRefeicao == null)
                {
                    itemBaseGastoRefeicao = Context.GastoRefeicoes.Create();
                    itemBase.Gastos.Add(itemBaseGastoRefeicao);
                }
                AtualizarPropriedades<GastoRefeicao>(itemBaseGastoRefeicao, itemGastoRefeicao);
            }
            foreach (RefeicaoPedido itemRefeicaoPedido in new List<RefeicaoPedido>(itemBase.Pedidos))
            {
                if (!itemGravar.Pedidos.Where(f => f.Identificador == itemRefeicaoPedido.Identificador).Any())
                {
                    Context.Entry<RefeicaoPedido>(itemRefeicaoPedido).State = EntityState.Deleted;
                }
            }
            foreach (RefeicaoPedido itemRefeicaoPedido in new List<RefeicaoPedido>(itemGravar.Pedidos))
            {
                RefeicaoPedido itemBaseRefeicaoPedido = !itemRefeicaoPedido.Identificador.HasValue ? null : itemBase.Pedidos.Where(f => f.Identificador == itemRefeicaoPedido.Identificador).FirstOrDefault();
                if (itemBaseRefeicaoPedido == null)
                {
                    itemBaseRefeicaoPedido = Context.RefeicaoPedidos.Create();
                    itemBase.Pedidos.Add(itemBaseRefeicaoPedido);
                }
                AtualizarPropriedades<RefeicaoPedido>(itemBaseRefeicaoPedido, itemRefeicaoPedido);
            }
            Context.SaveChanges();
            itemGravar.Identificador = itemBase.Identificador;
        }
        public Hotel SelecionarHotel_Completo(int? Identificador)
        {
            IQueryable<Hotel> query = Context.Hoteis
.Include("Avaliacoes").Include("Avaliacoes.ItemUsuario").Include("Eventos").Include("Eventos.ItemUsuario").Include("Fotos").Include("Fotos.ItemFoto").Include("Gastos").Include("Gastos.ItemGasto").Include("Gastos.ItemGasto.ItemUsuario");
            if (Identificador.HasValue)
                query = query.Where(d => d.Identificador == Identificador);
            return query.FirstOrDefault();
        }
        public void SalvarHotel_Completo(Hotel itemGravar)
        {
            Hotel itemBase = Context.Hoteis
.Include("Avaliacoes").Include("Eventos").Include("Fotos").Include("Gastos").Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            if (itemBase == null)
            {
                itemBase = Context.Hoteis.Create();
                itemBase.Avaliacoes = new List<HotelAvaliacao>();
                itemBase.Eventos = new List<HotelEvento>();
                itemBase.Fotos = new List<FotoHotel>();
                itemBase.Gastos = new List<GastoHotel>();
                Context.Entry<Hotel>(itemBase).State = System.Data.Entity.EntityState.Added;
            }
            AtualizarPropriedades<Hotel>(itemBase, itemGravar);
            foreach (HotelAvaliacao itemHotelAvaliacao in new List<HotelAvaliacao>(itemBase.Avaliacoes))
            {
                if (!itemGravar.Avaliacoes.Where(f => f.Identificador == itemHotelAvaliacao.Identificador).Any())
                {
                    Context.Entry<HotelAvaliacao>(itemHotelAvaliacao).State = EntityState.Deleted;
                }
            }
            foreach (HotelAvaliacao itemHotelAvaliacao in new List<HotelAvaliacao>(itemGravar.Avaliacoes))
            {
                HotelAvaliacao itemBaseHotelAvaliacao = !itemHotelAvaliacao.Identificador.HasValue ? null : itemBase.Avaliacoes.Where(f => f.Identificador == itemHotelAvaliacao.Identificador).FirstOrDefault();
                if (itemBaseHotelAvaliacao == null)
                {
                    itemBaseHotelAvaliacao = Context.HotelAvaliacoes.Create();
                    itemBase.Avaliacoes.Add(itemBaseHotelAvaliacao);
                }
                AtualizarPropriedades<HotelAvaliacao>(itemBaseHotelAvaliacao, itemHotelAvaliacao);
            }
            foreach (HotelEvento itemHotelEvento in new List<HotelEvento>(itemBase.Eventos))
            {
                if (!itemGravar.Eventos.Where(f => f.Identificador == itemHotelEvento.Identificador).Any())
                {
                    Context.Entry<HotelEvento>(itemHotelEvento).State = EntityState.Deleted;
                }
            }
            foreach (HotelEvento itemHotelEvento in new List<HotelEvento>(itemGravar.Eventos))
            {
                HotelEvento itemBaseHotelEvento = !itemHotelEvento.Identificador.HasValue ? null : itemBase.Eventos.Where(f => f.Identificador == itemHotelEvento.Identificador).FirstOrDefault();
                if (itemBaseHotelEvento == null)
                {
                    itemBaseHotelEvento = Context.HotelEventos.Create();
                    itemBase.Eventos.Add(itemBaseHotelEvento);
                }
                AtualizarPropriedades<HotelEvento>(itemBaseHotelEvento, itemHotelEvento);
            }
            foreach (FotoHotel itemFotoHotel in new List<FotoHotel>(itemBase.Fotos))
            {
                if (!itemGravar.Fotos.Where(f => f.Identificador == itemFotoHotel.Identificador).Any())
                {
                    Context.Entry<FotoHotel>(itemFotoHotel).State = EntityState.Deleted;
                }
            }
            foreach (FotoHotel itemFotoHotel in new List<FotoHotel>(itemGravar.Fotos))
            {
                FotoHotel itemBaseFotoHotel = !itemFotoHotel.Identificador.HasValue ? null : itemBase.Fotos.Where(f => f.Identificador == itemFotoHotel.Identificador).FirstOrDefault();
                if (itemBaseFotoHotel == null)
                {
                    itemBaseFotoHotel = Context.FotoHoteis.Create();
                    itemBase.Fotos.Add(itemBaseFotoHotel);
                }
                AtualizarPropriedades<FotoHotel>(itemBaseFotoHotel, itemFotoHotel);
            }
            foreach (GastoHotel itemGastoHotel in new List<GastoHotel>(itemBase.Gastos))
            {
                if (!itemGravar.Gastos.Where(f => f.Identificador == itemGastoHotel.Identificador).Any())
                {
                    Context.Entry<GastoHotel>(itemGastoHotel).State = EntityState.Deleted;
                }
            }
            foreach (GastoHotel itemGastoHotel in new List<GastoHotel>(itemGravar.Gastos))
            {
                GastoHotel itemBaseGastoHotel = !itemGastoHotel.Identificador.HasValue ? null : itemBase.Gastos.Where(f => f.Identificador == itemGastoHotel.Identificador).FirstOrDefault();
                if (itemBaseGastoHotel == null)
                {
                    itemBaseGastoHotel = Context.GastoHoteis.Create();
                    itemBase.Gastos.Add(itemBaseGastoHotel);
                }
                AtualizarPropriedades<GastoHotel>(itemBaseGastoHotel, itemGastoHotel);
            }
            Context.SaveChanges();
            itemGravar.Identificador = itemBase.Identificador;
        }
        public void SalvarViagemAerea_Completa(ViagemAerea itemGravar)
        {
            ViagemAerea itemBase = Context.ViagemAereas
.Include("Aeroportos").Include("Avaliacoes").Include("Gastos").Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            if (itemBase == null)
            {
                itemBase = Context.ViagemAereas.Create();
                itemBase.Aeroportos = new List<ViagemAereaAeroporto>();
                itemBase.Avaliacoes = new List<AvaliacaoAerea>();
                itemBase.Gastos = new List<GastoViagemAerea>();
                Context.Entry<ViagemAerea>(itemBase).State = System.Data.Entity.EntityState.Added;
            }
            AtualizarPropriedades<ViagemAerea>(itemBase, itemGravar);
            foreach (ViagemAereaAeroporto itemViagemAereaAeroporto in new List<ViagemAereaAeroporto>(itemBase.Aeroportos))
            {
                if (!itemGravar.Aeroportos.Where(f => f.Identificador == itemViagemAereaAeroporto.Identificador).Any())
                {
                    Context.Entry<ViagemAereaAeroporto>(itemViagemAereaAeroporto).State = EntityState.Deleted;
                }
            }
            foreach (ViagemAereaAeroporto itemViagemAereaAeroporto in new List<ViagemAereaAeroporto>(itemGravar.Aeroportos))
            {
                ViagemAereaAeroporto itemBaseViagemAereaAeroporto = !itemViagemAereaAeroporto.Identificador.HasValue ? null : itemBase.Aeroportos.Where(f => f.Identificador == itemViagemAereaAeroporto.Identificador).FirstOrDefault();
                if (itemBaseViagemAereaAeroporto == null)
                {
                    itemBaseViagemAereaAeroporto = Context.ViagemAereaAeroportos.Create();
                    itemBase.Aeroportos.Add(itemBaseViagemAereaAeroporto);
                }
                AtualizarPropriedades<ViagemAereaAeroporto>(itemBaseViagemAereaAeroporto, itemViagemAereaAeroporto);
            }
            foreach (AvaliacaoAerea itemAvaliacaoAerea in new List<AvaliacaoAerea>(itemBase.Avaliacoes))
            {
                if (!itemGravar.Avaliacoes.Where(f => f.Identificador == itemAvaliacaoAerea.Identificador).Any())
                {
                    Context.Entry<AvaliacaoAerea>(itemAvaliacaoAerea).State = EntityState.Deleted;
                }
            }
            foreach (AvaliacaoAerea itemAvaliacaoAerea in new List<AvaliacaoAerea>(itemGravar.Avaliacoes))
            {
                AvaliacaoAerea itemBaseAvaliacaoAerea = !itemAvaliacaoAerea.Identificador.HasValue ? null : itemBase.Avaliacoes.Where(f => f.Identificador == itemAvaliacaoAerea.Identificador).FirstOrDefault();
                if (itemBaseAvaliacaoAerea == null)
                {
                    itemBaseAvaliacaoAerea = Context.AvaliacaoAereas.Create();
                    itemBase.Avaliacoes.Add(itemBaseAvaliacaoAerea);
                }
                AtualizarPropriedades<AvaliacaoAerea>(itemBaseAvaliacaoAerea, itemAvaliacaoAerea);
            }
            foreach (GastoViagemAerea itemGastoViagemAerea in new List<GastoViagemAerea>(itemBase.Gastos))
            {
                if (!itemGravar.Gastos.Where(f => f.Identificador == itemGastoViagemAerea.Identificador).Any())
                {
                    Context.Entry<GastoViagemAerea>(itemGastoViagemAerea).State = EntityState.Deleted;
                }
            }
            foreach (GastoViagemAerea itemGastoViagemAerea in new List<GastoViagemAerea>(itemGravar.Gastos))
            {
                GastoViagemAerea itemBaseGastoViagemAerea = !itemGastoViagemAerea.Identificador.HasValue ? null : itemBase.Gastos.Where(f => f.Identificador == itemGastoViagemAerea.Identificador).FirstOrDefault();
                if (itemBaseGastoViagemAerea == null)
                {
                    itemBaseGastoViagemAerea = Context.GastoViagemAereas.Create();
                    itemBase.Gastos.Add(itemBaseGastoViagemAerea);
                }
                AtualizarPropriedades<GastoViagemAerea>(itemBaseGastoViagemAerea, itemGastoViagemAerea);
            }
            Context.SaveChanges();
            itemGravar.Identificador = itemBase.Identificador;
        }
        public ViagemAerea SelecionarViagemAerea_Completa(int? Identificador)
        {
            IQueryable<ViagemAerea> query = Context.ViagemAereas
.Include("Aeroportos").Include("Avaliacoes").Include("Avaliacoes.ItemUsuario").Include("Gastos").Include("Gastos.ItemGasto").Include("Gastos.ItemGasto.ItemUsuario");
            if (Identificador.HasValue)
                query = query.Where(d => d.Identificador == Identificador);
            return query.FirstOrDefault();
        }
        public Carro SelecionarCarro_Completo(int? Identificador)
        {
            IQueryable<Carro> query = Context.Carros
.Include("Avaliacoes").Include("Gastos").Include("Gastos.ItemGasto").Include("Gastos.ItemGasto.ItemUsuario").Include("Reabastecimentos").Include("Reabastecimentos.Gastos").Include("Reabastecimentos.Gastos.ItemGasto").Include("Reabastecimentos.Gastos.ItemGasto.ItemUsuario").Include("Deslocamentos").Include("Deslocamentos.ItemCarroEventoChegada").Include("Deslocamentos.ItemCarroEventoChegada.ItemCidade").Include("Deslocamentos.ItemCarroEventoPartida").Include("Deslocamentos.ItemCarroEventoPartida.ItemCidade").Include("Deslocamentos.Usuarios").Include("ItemCarroEventoDevolucao").Include("ItemCarroEventoRetirada");
            if (Identificador.HasValue)
                query = query.Where(d => d.Identificador == Identificador);
            return query.FirstOrDefault();
        }
        public CarroDeslocamento SelecionarCarroDeslocamento(int? Identificador)
        {
            IQueryable<CarroDeslocamento> query = Context.CarroDeslocamentos
.Include("ItemCarroEventoChegada").Include("ItemCarroEventoChegada.ItemCidade").Include("ItemCarroEventoPartida").Include("ItemCarroEventoPartida.ItemCidade").Include("Usuarios");
            if (Identificador.HasValue)
                query = query.Where(d => d.Identificador == Identificador);
            return query.FirstOrDefault();
        }
        public IList<CarroDeslocamento> ListarCarroDeslocamento()
        {
            IQueryable<CarroDeslocamento> query = Context.CarroDeslocamentos
;
            return query.ToList();
        }
        public void SalvarCarroDeslocamento(CarroDeslocamento itemGravar)
        {
            CarroDeslocamento itemBase = Context.CarroDeslocamentos
.Include("Usuarios").Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            if (itemBase == null)
            {
                itemBase = Context.CarroDeslocamentos.Create();
                itemBase.Usuarios = new List<CarroDeslocamentoUsuario>();
                Context.Entry<CarroDeslocamento>(itemBase).State = System.Data.Entity.EntityState.Added;
            }
            AtualizarPropriedades<CarroDeslocamento>(itemBase, itemGravar);
            foreach (CarroDeslocamentoUsuario itemCarroDeslocamentoUsuario in new List<CarroDeslocamentoUsuario>(itemBase.Usuarios))
            {
                if (!itemGravar.Usuarios.Where(f => f.Identificador == itemCarroDeslocamentoUsuario.Identificador).Any())
                {
                    Context.Entry<CarroDeslocamentoUsuario>(itemCarroDeslocamentoUsuario).State = EntityState.Deleted;
                }
            }
            foreach (CarroDeslocamentoUsuario itemCarroDeslocamentoUsuario in new List<CarroDeslocamentoUsuario>(itemGravar.Usuarios))
            {
                CarroDeslocamentoUsuario itemBaseCarroDeslocamentoUsuario = !itemCarroDeslocamentoUsuario.Identificador.HasValue ? null : itemBase.Usuarios.Where(f => f.Identificador == itemCarroDeslocamentoUsuario.Identificador).FirstOrDefault();
                if (itemBaseCarroDeslocamentoUsuario == null)
                {
                    itemBaseCarroDeslocamentoUsuario = Context.CarroDeslocamentoUsuarios.Create();
                    itemBase.Usuarios.Add(itemBaseCarroDeslocamentoUsuario);
                }
                AtualizarPropriedades<CarroDeslocamentoUsuario>(itemBaseCarroDeslocamentoUsuario, itemCarroDeslocamentoUsuario);
            }
            Context.SaveChanges();
            itemGravar.Identificador = itemBase.Identificador;
        }
        public void ExcluirCarroDeslocamento(CarroDeslocamento itemGravar)
        {
            CarroDeslocamento itemExcluir = Context.CarroDeslocamentos
        .Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            Context.Entry<CarroDeslocamento>(itemExcluir).State = EntityState.Deleted;
            Context.SaveChanges();
        }
        public CarroDeslocamentoUsuario SelecionarCarroDeslocamentoUsuario(int? Identificador)
        {
            IQueryable<CarroDeslocamentoUsuario> query = Context.CarroDeslocamentoUsuarios
;
            if (Identificador.HasValue)
                query = query.Where(d => d.Identificador == Identificador);
            return query.FirstOrDefault();
        }
        public IList<CarroDeslocamentoUsuario> ListarCarroDeslocamentoUsuario()
        {
            IQueryable<CarroDeslocamentoUsuario> query = Context.CarroDeslocamentoUsuarios
;
            return query.ToList();
        }
        public Loja SelecionarLoja_Completo(int? Identificador)
        {
            IQueryable<Loja> query = Context.Lojas
.Include("Avaliacoes");
            if (Identificador.HasValue)
                query = query.Where(d => d.Identificador == Identificador);
            return query.FirstOrDefault();
        }
        public void SalvarLoja_Completo(Loja itemGravar)
        {
            Loja itemBase = Context.Lojas
.Include("Avaliacoes").Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            if (itemBase == null)
            {
                itemBase = Context.Lojas.Create();
                itemBase.Avaliacoes = new List<AvaliacaoLoja>();
                Context.Entry<Loja>(itemBase).State = System.Data.Entity.EntityState.Added;
            }
            AtualizarPropriedades<Loja>(itemBase, itemGravar);
            foreach (AvaliacaoLoja itemAvaliacaoLoja in new List<AvaliacaoLoja>(itemBase.Avaliacoes))
            {
                if (!itemGravar.Avaliacoes.Where(f => f.Identificador == itemAvaliacaoLoja.Identificador).Any())
                {
                    Context.Entry<AvaliacaoLoja>(itemAvaliacaoLoja).State = EntityState.Deleted;
                }
            }
            foreach (AvaliacaoLoja itemAvaliacaoLoja in new List<AvaliacaoLoja>(itemGravar.Avaliacoes))
            {
                AvaliacaoLoja itemBaseAvaliacaoLoja = !itemAvaliacaoLoja.Identificador.HasValue ? null : itemBase.Avaliacoes.Where(f => f.Identificador == itemAvaliacaoLoja.Identificador).FirstOrDefault();
                if (itemBaseAvaliacaoLoja == null)
                {
                    itemBaseAvaliacaoLoja = Context.AvaliacaoLojas.Create();
                    itemBase.Avaliacoes.Add(itemBaseAvaliacaoLoja);
                }
                AtualizarPropriedades<AvaliacaoLoja>(itemBaseAvaliacaoLoja, itemAvaliacaoLoja);
            }
           Context.SaveChanges();
            itemGravar.Identificador = itemBase.Identificador;
        }
       
        public void SalvarCidadeGrupo_Lista(IList<CidadeGrupo> ListaSalvar)
        {
            foreach (CidadeGrupo itemGravar in ListaSalvar)
            {
                CidadeGrupo itemBase = Context.CidadeGrupos
                .Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
                if (itemBase == null)
                {
                    itemBase = Context.CidadeGrupos.Create();
                    Context.Entry<CidadeGrupo>(itemBase).State = System.Data.Entity.EntityState.Added;
                }
                AtualizarPropriedades<CidadeGrupo>(itemBase, itemGravar);
            }
            Context.SaveChanges();
        }
        public IList<CidadeGrupo> ListarCidadeGrupo_IdentificadorCidadePai(int? IdentificadorCidadePai, int? IdentificadorViagem)
        {
            IQueryable<CidadeGrupo> query = Context.CidadeGrupos
;
            if (IdentificadorCidadePai.HasValue)
                query = query.Where(d => d.IdentificadorCidadePai == IdentificadorCidadePai);
            if (IdentificadorViagem.HasValue)
                query = query.Where(d => d.IdentificadorViagem == IdentificadorViagem);
            return query.ToList();
        }
    }

}
