using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CV.Model;
using System.Linq.Expressions;
using CV.Model.Dominio;
using System.Data.Entity;

namespace CV.Data
{
    public partial class ViagemRepository : RepositoryBase
    {
        #region Save

        public void ExcluirCidadeGrupo_Lista(List<CidadeGrupo> ListaExcluir)
        {
            foreach (var itemGravar in ListaExcluir)
            {
                CidadeGrupo itemExcluir = Context.CidadeGrupos
            .Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
                Context.Entry<CidadeGrupo>(itemExcluir).State = EntityState.Deleted;
            }
            Context.SaveChanges();
        }

        public void SalvarGastoCompra_Completo(GastoCompra itemGravar)
        {
            GastoCompra itemBase = Context.GastoCompras
.Include("ItemGasto").Include("ItemGasto.Usuarios").Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            if (itemBase == null)
            {
                itemBase = Context.GastoCompras.Create();
                Context.Entry<GastoCompra>(itemBase).State = System.Data.Entity.EntityState.Added;
            }
            AtualizarPropriedades<GastoCompra>(itemBase, itemGravar);
            Gasto itemGasto = itemGravar.ItemGasto;
            Gasto itemBaseGasto = null;
            if (itemGasto != null)
            {
                itemBaseGasto = Context.Gastos.Where(f => f.Identificador == itemGasto.Identificador).FirstOrDefault();
                if (itemBaseGasto == null)
                {
                    itemBaseGasto = Context.Gastos.Create();
                    itemBaseGasto.Usuarios = new List<GastoDividido>();
                    Context.Entry<Gasto>(itemBaseGasto).State = System.Data.Entity.EntityState.Added;
                }
                AtualizarPropriedades<Gasto>(itemBaseGasto, itemGasto);
                itemBase.ItemGasto = itemBaseGasto;
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
            Context.SaveChanges();
            itemGravar.Identificador = itemBase.Identificador;
        }

        public void SalvarCarro_Completo(Carro itemGravar)
        {
            Carro itemBase = Context.Carros
.Include("Avaliacoes").Include("Gastos").Include("Reabastecimentos").Include("Reabastecimentos.Gastos").Include("Deslocamentos").Include("Deslocamentos.ItemCarroEventoChegada").Include("Deslocamentos.ItemCarroEventoPartida").Include("Deslocamentos.Usuarios").Include("ItemCarroEventoDevolucao").Include("ItemCarroEventoRetirada").Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            if (itemBase == null)
            {
                itemBase = Context.Carros.Create();
                itemBase.Avaliacoes = new List<AvaliacaoAluguel>();
                itemBase.Gastos = new List<AluguelGasto>();
                itemBase.Reabastecimentos = new List<Reabastecimento>();
                itemBase.Deslocamentos = new List<CarroDeslocamento>();
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
            foreach (AluguelGasto itemAluguelGasto in new List<AluguelGasto>(itemBase.Gastos))
            {
                if (!itemGravar.Gastos.Where(f => f.Identificador == itemAluguelGasto.Identificador).Any())
                {
                    Context.Entry<AluguelGasto>(itemAluguelGasto).State = EntityState.Deleted;
                }
            }
            foreach (AluguelGasto itemAluguelGasto in new List<AluguelGasto>(itemGravar.Gastos))
            {
                AluguelGasto itemBaseAluguelGasto = !itemAluguelGasto.Identificador.HasValue ? null : itemBase.Gastos.Where(f => f.Identificador == itemAluguelGasto.Identificador).FirstOrDefault();
                if (itemBaseAluguelGasto == null)
                {
                    itemBaseAluguelGasto = Context.AluguelGastos.Create();
                    itemBase.Gastos.Add(itemBaseAluguelGasto);
                }
                AtualizarPropriedades<AluguelGasto>(itemBaseAluguelGasto, itemAluguelGasto);
            }
            foreach (Reabastecimento itemReabastecimento in new List<Reabastecimento>(itemBase.Reabastecimentos))
            {
                if (!itemGravar.Reabastecimentos.Where(f => f.Identificador == itemReabastecimento.Identificador).Any())
                {
                    foreach (ReabastecimentoGasto itemReabastecimentoGasto in new List<ReabastecimentoGasto>(itemReabastecimento.Gastos))
                    {
                        Context.Entry<ReabastecimentoGasto>(itemReabastecimentoGasto).State = EntityState.Deleted;
                    }
                    Context.Entry<Reabastecimento>(itemReabastecimento).State = EntityState.Deleted;
                }
            }
            foreach (Reabastecimento itemReabastecimento in new List<Reabastecimento>(itemGravar.Reabastecimentos))
            {
                Reabastecimento itemBaseReabastecimento = !itemReabastecimento.Identificador.HasValue ? null : itemBase.Reabastecimentos.Where(f => f.Identificador == itemReabastecimento.Identificador).FirstOrDefault();
                if (itemBaseReabastecimento == null)
                {
                    itemBaseReabastecimento = Context.Reabastecimentos.Create();
                    itemBaseReabastecimento.Gastos = new List<ReabastecimentoGasto>();
                    itemBase.Reabastecimentos.Add(itemBaseReabastecimento);
                }
                AtualizarPropriedades<Reabastecimento>(itemBaseReabastecimento, itemReabastecimento);
                foreach (ReabastecimentoGasto itemReabastecimentoGasto in new List<ReabastecimentoGasto>(itemBaseReabastecimento.Gastos))
                {
                    if (!itemReabastecimento.Gastos.Where(f => f.Identificador == itemReabastecimentoGasto.Identificador).Any())
                    {
                        Context.Entry<ReabastecimentoGasto>(itemReabastecimentoGasto).State = EntityState.Deleted;
                    }
                }
                foreach (ReabastecimentoGasto itemReabastecimentoGasto in new List<ReabastecimentoGasto>(itemReabastecimento.Gastos))
                {
                    ReabastecimentoGasto itemBaseReabastecimentoGasto = !itemReabastecimentoGasto.Identificador.HasValue ? null : itemBaseReabastecimento.Gastos.Where(f => f.Identificador == itemReabastecimentoGasto.Identificador).FirstOrDefault();
                    if (itemBaseReabastecimentoGasto == null)
                    {
                        itemBaseReabastecimentoGasto = Context.ReabastecimentoGastos.Create();
                        itemBaseReabastecimento.Gastos.Add(itemBaseReabastecimentoGasto);
                    }
                    AtualizarPropriedades<ReabastecimentoGasto>(itemBaseReabastecimentoGasto, itemReabastecimentoGasto);
                }
            }
            foreach (CarroDeslocamento itemCarroDeslocamento in new List<CarroDeslocamento>(itemBase.Deslocamentos))
            {
                if (!itemGravar.Deslocamentos.Where(f => f.Identificador == itemCarroDeslocamento.Identificador).Any())
                {
                    foreach (CarroDeslocamentoUsuario itemCarroDeslocamentoUsuario in new List<CarroDeslocamentoUsuario>(itemCarroDeslocamento.Usuarios))
                    {
                        Context.Entry<CarroDeslocamentoUsuario>(itemCarroDeslocamentoUsuario).State = EntityState.Deleted;
                    }
                    Context.Entry<CarroDeslocamento>(itemCarroDeslocamento).State = EntityState.Deleted;
                }
            }
            foreach (CarroDeslocamento itemCarroDeslocamento in new List<CarroDeslocamento>(itemGravar.Deslocamentos))
            {
                CarroDeslocamento itemBaseCarroDeslocamento = !itemCarroDeslocamento.Identificador.HasValue ? null : itemBase.Deslocamentos.Where(f => f.Identificador == itemCarroDeslocamento.Identificador).FirstOrDefault();
                if (itemBaseCarroDeslocamento == null)
                {
                    itemBaseCarroDeslocamento = Context.CarroDeslocamentos.Create();
                    itemBaseCarroDeslocamento.Usuarios = new List<CarroDeslocamentoUsuario>();
                    itemBase.Deslocamentos.Add(itemBaseCarroDeslocamento);
                }
                AtualizarPropriedades<CarroDeslocamento>(itemBaseCarroDeslocamento, itemCarroDeslocamento);
                CarroEvento itemCarroEventoChegada = itemCarroDeslocamento.ItemCarroEventoChegada;
                CarroEvento itemBaseCarroEventoChegada = null;
                if (itemCarroEventoChegada != null)
                {
                    itemBaseCarroEventoChegada = Context.CarroEventos.Where(f => f.Identificador == itemCarroEventoChegada.Identificador).FirstOrDefault();
                    if (itemBaseCarroEventoChegada == null)
                    {
                        itemBaseCarroEventoChegada = Context.CarroEventos.Create();
                        Context.Entry<CarroEvento>(itemBaseCarroEventoChegada).State = System.Data.Entity.EntityState.Added;
                    }
                    AtualizarPropriedades<CarroEvento>(itemBaseCarroEventoChegada, itemCarroEventoChegada);
                    itemBaseCarroDeslocamento.ItemCarroEventoChegada = itemBaseCarroEventoChegada;
                }
                CarroEvento itemCarroEventoPartida = itemCarroDeslocamento.ItemCarroEventoPartida;
                CarroEvento itemBaseCarroEventoPartida = null;
                if (itemCarroEventoPartida != null)
                {
                    itemBaseCarroEventoPartida = Context.CarroEventos.Where(f => f.Identificador == itemCarroEventoPartida.Identificador).FirstOrDefault();
                    if (itemBaseCarroEventoPartida == null)
                    {
                        itemBaseCarroEventoPartida = Context.CarroEventos.Create();
                        Context.Entry<CarroEvento>(itemBaseCarroEventoPartida).State = System.Data.Entity.EntityState.Added;
                    }
                    AtualizarPropriedades<CarroEvento>(itemBaseCarroEventoPartida, itemCarroEventoPartida);
                    itemBaseCarroDeslocamento.ItemCarroEventoPartida = itemBaseCarroEventoPartida;
                }
                foreach (CarroDeslocamentoUsuario itemCarroDeslocamentoUsuario in new List<CarroDeslocamentoUsuario>(itemBaseCarroDeslocamento.Usuarios))
                {
                    if (!itemCarroDeslocamento.Usuarios.Where(f => f.Identificador == itemCarroDeslocamentoUsuario.Identificador).Any())
                    {
                        Context.Entry<CarroDeslocamentoUsuario>(itemCarroDeslocamentoUsuario).State = EntityState.Deleted;
                    }
                }
                foreach (CarroDeslocamentoUsuario itemCarroDeslocamentoUsuario in new List<CarroDeslocamentoUsuario>(itemCarroDeslocamento.Usuarios))
                {
                    CarroDeslocamentoUsuario itemBaseCarroDeslocamentoUsuario = !itemCarroDeslocamentoUsuario.Identificador.HasValue ? null : itemBaseCarroDeslocamento.Usuarios.Where(f => f.Identificador == itemCarroDeslocamentoUsuario.Identificador).FirstOrDefault();
                    if (itemBaseCarroDeslocamentoUsuario == null)
                    {
                        itemBaseCarroDeslocamentoUsuario = Context.CarroDeslocamentoUsuarios.Create();
                        itemBaseCarroDeslocamento.Usuarios.Add(itemBaseCarroDeslocamentoUsuario);
                    }
                    AtualizarPropriedades<CarroDeslocamentoUsuario>(itemBaseCarroDeslocamentoUsuario, itemCarroDeslocamentoUsuario);
                }
            }
            CarroEvento itemCarroEventoDevolucao = itemGravar.ItemCarroEventoDevolucao;
            CarroEvento itemBaseCarroEventoDevolucao = null;
            if (itemCarroEventoDevolucao != null)
            {
                itemBaseCarroEventoDevolucao = Context.CarroEventos.Where(f => f.Identificador == itemCarroEventoDevolucao.Identificador).FirstOrDefault();
                if (itemBaseCarroEventoDevolucao == null)
                {
                    itemBaseCarroEventoDevolucao = Context.CarroEventos.Create();
                    Context.Entry<CarroEvento>(itemBaseCarroEventoDevolucao).State = System.Data.Entity.EntityState.Added;
                }
                AtualizarPropriedades<CarroEvento>(itemBaseCarroEventoDevolucao, itemCarroEventoDevolucao);
                itemBase.ItemCarroEventoDevolucao = itemBaseCarroEventoDevolucao;
            }
            CarroEvento itemCarroEvento = itemGravar.ItemCarroEventoRetirada;
            CarroEvento itemBaseCarroEvento = null;
            if (itemCarroEvento != null)
            {
                itemBaseCarroEvento = Context.CarroEventos.Where(f => f.Identificador == itemCarroEvento.Identificador).FirstOrDefault();
                if (itemBaseCarroEvento == null)
                {
                    itemBaseCarroEvento = Context.CarroEventos.Create();
                    Context.Entry<CarroEvento>(itemBaseCarroEvento).State = System.Data.Entity.EntityState.Added;
                }
                AtualizarPropriedades<CarroEvento>(itemBaseCarroEvento, itemCarroEvento);
                itemBase.ItemCarroEventoRetirada = itemBaseCarroEvento;
            }
            Context.SaveChanges();
            itemGravar.Identificador = itemBase.Identificador;
        }

        public void SalvarCarro_Evento(Carro itemGravar)
        {
            Carro itemBase = Context.Carros
.Include("Avaliacoes").Include("ItemCarroEventoDevolucao").Include("ItemCarroEventoRetirada").Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            if (itemBase == null)
            {
                itemBase = Context.Carros.Create();
                itemBase.Avaliacoes = new List<AvaliacaoAluguel>();
                itemBase.Gastos = new List<AluguelGasto>();
                itemBase.Reabastecimentos = new List<Reabastecimento>();
                itemBase.Deslocamentos = new List<CarroDeslocamento>();
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
            CarroEvento itemCarroEventoDevolucao = itemGravar.ItemCarroEventoDevolucao;
            CarroEvento itemBaseCarroEventoDevolucao = null;
            if (itemCarroEventoDevolucao != null)
            {
                itemBaseCarroEventoDevolucao = Context.CarroEventos.Where(f => f.Identificador == itemCarroEventoDevolucao.Identificador).FirstOrDefault();
                if (itemBaseCarroEventoDevolucao == null)
                {
                    itemBaseCarroEventoDevolucao = Context.CarroEventos.Create();
                    Context.Entry<CarroEvento>(itemBaseCarroEventoDevolucao).State = System.Data.Entity.EntityState.Added;
                }               

                AtualizarPropriedades<CarroEvento>(itemBaseCarroEventoDevolucao, itemCarroEventoDevolucao);
                itemBase.ItemCarroEventoDevolucao = itemBaseCarroEventoDevolucao;
                if (itemBaseCarroEventoDevolucao.Identificador.HasValue)
                    itemBase.IdentificadorCarroEventoDevolucao = itemBaseCarroEventoDevolucao.Identificador;
            }
            else if (itemBase.ItemCarroEventoDevolucao != null)
                Context.CarroEventos.Remove(itemBase.ItemCarroEventoDevolucao);

            CarroEvento itemCarroEvento = itemGravar.ItemCarroEventoRetirada;
            CarroEvento itemBaseCarroEvento = null;
            if (itemCarroEvento != null)
            {
                itemBaseCarroEvento = Context.CarroEventos.Where(f => f.Identificador == itemCarroEvento.Identificador).FirstOrDefault();
                if (itemBaseCarroEvento == null)
                {
                    itemBaseCarroEvento = Context.CarroEventos.Create();
                    Context.Entry<CarroEvento>(itemBaseCarroEvento).State = System.Data.Entity.EntityState.Added;
                }
                AtualizarPropriedades<CarroEvento>(itemBaseCarroEvento, itemCarroEvento);
                itemBase.ItemCarroEventoRetirada = itemBaseCarroEvento;
                if (itemBaseCarroEvento.Identificador.HasValue)

                    itemBase.Identificador = itemBaseCarroEvento.Identificador;

            }
            else if (itemBase.ItemCarroEventoRetirada != null)
                Context.CarroEventos.Remove(itemBase.ItemCarroEventoRetirada);
            Context.SaveChanges();
            itemGravar.Identificador = itemBase.Identificador;
        }


        public void SalvarCarroDeslocamento_Evento(CarroDeslocamento itemGravar)
        {
            CarroDeslocamento itemBase = Context.CarroDeslocamentos
.Include("Usuarios").Include("ItemCarroEventoChegada").Include("ItemCarroEventoPartida").Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
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
            CarroEvento itemCarroEventoChegada = itemGravar.ItemCarroEventoChegada;
            CarroEvento itemBaseCarroEventoChegada = null;
            if (itemCarroEventoChegada != null)
            {
                itemBaseCarroEventoChegada = Context.CarroEventos.Where(f => f.Identificador == itemCarroEventoChegada.Identificador).FirstOrDefault();
                if (itemBaseCarroEventoChegada == null)
                {
                    itemBaseCarroEventoChegada = Context.CarroEventos.Create();
                    Context.Entry<CarroEvento>(itemBaseCarroEventoChegada).State = System.Data.Entity.EntityState.Added;
                }
                AtualizarPropriedades<CarroEvento>(itemBaseCarroEventoChegada, itemCarroEventoChegada);
                itemBase.ItemCarroEventoChegada = itemBaseCarroEventoChegada;
                if (itemBaseCarroEventoChegada.Identificador.HasValue)
                    itemBase.IdentificadorCarroEventoChegada = itemBaseCarroEventoChegada.Identificador;
            }
            CarroEvento itemCarroEvento = itemGravar.ItemCarroEventoPartida;
            CarroEvento itemBaseCarroEvento = null;
            if (itemCarroEvento != null)
            {
                itemBaseCarroEvento = Context.CarroEventos.Where(f => f.Identificador == itemCarroEvento.Identificador).FirstOrDefault();
                if (itemBaseCarroEvento == null)
                {
                    itemBaseCarroEvento = Context.CarroEventos.Create();
                    Context.Entry<CarroEvento>(itemBaseCarroEvento).State = System.Data.Entity.EntityState.Added;
                }
                AtualizarPropriedades<CarroEvento>(itemBaseCarroEvento, itemCarroEvento);
                itemBase.ItemCarroEventoPartida = itemBaseCarroEvento;
                if (itemBaseCarroEvento.Identificador.HasValue)

                    itemBase.IdentificadorCarroEventoPartida = itemBaseCarroEvento.Identificador;
            }
            Context.SaveChanges();
            itemGravar.Identificador = itemBase.Identificador;
        }

        public void SalvarAporteDinheiroCompleto(AporteDinheiro itemGravar)
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
                itemBaseGasto = itemBase.ItemGasto;
                if (itemBaseGasto == null)
                {
                    itemBaseGasto = Context.Gastos.Create();
                    Context.Entry<Gasto>(itemBaseGasto).State = System.Data.Entity.EntityState.Added;
                }
                AtualizarPropriedades<Gasto>(itemBaseGasto, itemGasto);
                itemBase.ItemGasto = itemBaseGasto;
            }
            else
            {
                if (itemBase.ItemGasto != null)
                {
                    itemBase.ItemGasto.DataExclusao = DateTime.Now;
                    itemBase.ItemGasto = null;
                }
                itemBase.IdentificadorGasto = null;
            }
            Context.SaveChanges();
            itemGravar.Identificador = itemBase.Identificador;
        }

        #endregion

        public void SalvarPosicaoLista(List<Posicao> Lista)
        {
            this.Context.Posicoes.AddRange(Lista);
            this.Context.SaveChanges();
        }

        public void SalvarAtracaoSimples(Atracao itemGravar)
        {
            Atracao itemBase = Context.Atracoes.Where(f => f.Identificador == itemGravar.Identificador).FirstOrDefault();
            if (itemBase == null)
            {
                itemBase = Context.Atracoes.Create();
                itemBase.Avaliacoes = new List<AvaliacaoAtracao>();
                Context.Entry<Atracao>(itemBase).State = System.Data.Entity.EntityState.Added;
            }
            AtualizarPropriedades<Atracao>(itemBase, itemGravar);
        
            Context.SaveChanges();
            itemGravar.Identificador = itemBase.Identificador;
        }

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
            return Context.Amigos.Include("ItemAmigo").Where(predicate).ToList();
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

        public List<Usuario> ListarUsuarioAmigoComigo(int identificadorUsuario)
        {
            IQueryable<Usuario> Query = this.Context.Usuarios;
            IQueryable<Amigo> QueryAmigo = this.Context.Amigos.Where(d => d.IdentificadorUsuario == identificadorUsuario);
            Query = Query.Where(d => QueryAmigo.Where(e => e.IdentificadorAmigo == d.Identificador).Any());
            Query = Query.Union(this.Context.Usuarios.Where(d=>d.Identificador == identificadorUsuario));
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

        public List<Cidade> CarregarCidadeViagemAerea(int? IdentificadorViagem)
        {
            IQueryable<Cidade> query = this.Context.ViagemAereaAeroportos.Where(d => d.ItemViagemAerea.IdentificadorViagem == IdentificadorViagem).Where(d => d.TipoPonto != (int)enumTipoParada.Escala).Select(d => d.ItemCidade);

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


        public List<Cidade> CarregarCidadeSugestao(int? IdentificadorViagem)
        {
            IQueryable<Cidade> query = this.Context.Sugestoes.Where(d => d.IdentificadorViagem == IdentificadorViagem).Select(d => d.ItemCidade);

            var queryJoin = query.GroupJoin(this.Context.CidadeGrupos.Where(d => d.IdentificadorViagem == IdentificadorViagem),
                d => d.Identificador,
                d => d.IdentificadorCidadeFilha,
                (c, g) => new { cidade = g.Any() ? g.Select(d => d.ItemCidadePai).FirstOrDefault() : c });

            return queryJoin.Where(d => d.cidade != null).Select(d => d.cidade).Distinct().OrderBy(d => d.Nome).ToList();


        }
        public List<Cidade> CarregarCidadeComentario(int? IdentificadorViagem)
        {
            IQueryable<Cidade> query = this.Context.Comentarios.Where(d => d.IdentificadorViagem == IdentificadorViagem).Select(d => d.ItemCidade);

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

        public List<CotacaoMoeda> ListarCotacaoMoeda(Expression<Func<CotacaoMoeda, bool>> predicate)
        {
            return Context.CotacaoMoedas.Where(predicate).ToList();
        }

        public List<Comentario> ListarComentario(Expression<Func<Comentario, bool>> predicate)
        {
            return Context.Comentarios.Include("ItemCidade").Where(predicate).ToList();
        }


        public List<AporteDinheiro> ListarAporteDinheiro(Expression<Func<AporteDinheiro, bool>> predicate)
        {
            return Context.AporteDinheiros.Include("ItemGasto").Where(predicate).ToList();
        }

        public List<Sugestao> ListarSugestao(Expression<Func<Sugestao, bool>> predicate)
        {
            return Context.Sugestoes.Include("ItemCidade").Include("ItemUsuario").Where(predicate).ToList();
        }

        public List<CalendarioPrevisto> ListarCalendarioPrevisto(Expression<Func<CalendarioPrevisto, bool>> predicate)
        {
            return Context.CalendarioPrevistos.Where(predicate).ToList();
        }

        public List<ListaCompra> ListarListaCompra(int? IdentificadorUsuario, Expression<Func<ListaCompra, bool>> predicate)
        {
            return Context.ListaCompras.Include("ItemUsuario").Include("ItemUsuarioPedido").Where(d=>d.IdentificadorUsuario == IdentificadorUsuario || d.IdentificadorUsuarioPedido == IdentificadorUsuario)
                .Where(predicate).ToList();
        }

        public List<Gasto> ListarGasto(Expression<Func<Gasto, bool>> predicate)
        {
            return Context.Gastos.Include("Usuarios").Include("Alugueis").Include("Atracoes").Include("Hoteis").Include("Refeicoes").Include("ViagenAereas")
                         .Where(d => !d.Compras.Any()).Where(d => !d.Reabastecimentos.Any()).Where(d =>  !d.ApenasBaixa.Value).Where(predicate).ToList();
        }


        public List<Atracao> ListarAtracao_Completo(Expression<Func<Atracao, bool>> predicate)
        {
            return Context.Atracoes.Include("ItemCidade").Include("Avaliacoes")
                        .Where(predicate).ToList();
        }

        public List<Hotel> ListarHotel_Completo(Expression<Func<Hotel, bool>> predicate)
        {
            return Context.Hoteis.Include("ItemCidade").Include("Avaliacoes").Where(predicate).ToList();
        }

        public List<Refeicao> ListarRefeicao_Completo(Expression<Func<Refeicao, bool>> predicate)
        {
            return Context.Refeicoes.Include("ItemCidade").Include("Pedidos").Where(predicate).ToList();
        }

        public List<Loja> ListarLoja(Expression<Func<Loja, bool>> predicate)
        {
            return Context.Lojas.Include("ItemCidade").Include("Avaliacoes").Where(predicate).ToList();
        }

        public List<Carro> ListarCarro(Expression<Func<Carro, bool>> predicate)
        {
            return Context.Carros.Include("Avaliacoes").Include("ItemCarroEventoDevolucao").Include("ItemCarroEventoRetirada").Where(predicate).ToList();
        }

        public List<ViagemAerea> ListarViagemAerea(Expression<Func<ViagemAerea, bool>> predicate)
        {
            return Context.ViagemAereas.Include("Avaliacoes").Include("Aeroportos").Include("Aeroportos.ItemCidade").Where(predicate).ToList();
        }

        public List<GastoCompra> ListarGastoCompra(int? IdentificadorViagem,int? IdentificadorUsuario, Expression<Func<GastoCompra, bool>> predicate)
        {
            return Context.GastoCompras.Include("ItemGasto")
                .Where(d=>d.ItemLoja.IdentificadorViagem == IdentificadorViagem)
                .Where(d => d.ItemGasto.IdentificadorUsuario == IdentificadorUsuario)

                .Where(predicate).ToList();
        }


        public List<GastoCompra> ListarGastoCompra( Expression<Func<GastoCompra, bool>> predicate)
        {
            return Context.GastoCompras

                .Where(predicate).ToList();
        }

        public List<ReabastecimentoGasto> ListarReabastecimentoGastos(Expression<Func<ReabastecimentoGasto, bool>> predicate)
        {
            return Context.ReabastecimentoGastos

                .Where(predicate).ToList();
        }
        public List<Reabastecimento> ListarReabastecimento(int? IdentificadorViagem, Expression<Func<Reabastecimento, bool>> predicate)
        {
            return Context.Reabastecimentos.Include("Gastos").Include("Gastos.ItemGasto").Include("Gastos.ItemGasto.Usuarios")
                .Where(d => d.ItemCarro.IdentificadorViagem == IdentificadorViagem)
                .Where(predicate).ToList();
        }

        public List<HotelEvento> ListarHotelEvento(int? IdentificadorViagem, Expression<Func<HotelEvento, bool>> predicate)
        {
            return Context.HotelEventos
                .Where(d => d.ItemHotel.IdentificadorViagem == IdentificadorViagem)
                .Where(predicate).ToList();
        }

        public List<CarroDeslocamento> ListarCarroDeslocamento(int? IdentificadorViagem, Expression<Func<CarroDeslocamento, bool>> predicate)
        {
            return Context.CarroDeslocamentos.Include("ItemCarroEventoPartida").Include("ItemCarroEventoChegada").Include("Usuarios")
                .Where(d => d.ItemCarro.IdentificadorViagem == IdentificadorViagem )
                .Where(predicate).ToList();
        }

        public List<ItemCompra> ListarItemCompra(int? IdentificadorViagem, Expression<Func<ItemCompra, bool>> predicate)
        {
            return Context.ItemCompras.Include("ItemUsuario")
                .Where(d => d.ItemGastoCompra.ItemLoja.IdentificadorViagem == IdentificadorViagem)
                .Where(predicate).ToList();
        }

        public List<AluguelGasto> ListarAluguelGasto(int? IdentificadorViagem, Expression<Func<AluguelGasto, bool>> predicate)
        {
            return Context.AluguelGastos
                .Where(d => d.ItemGasto.IdentificadorViagem == IdentificadorViagem)
                .Where(predicate).ToList();
        }

        public List<GastoAtracao> ListarGastoAtracao(int? IdentificadorViagem, Expression<Func<GastoAtracao, bool>> predicate)
        {
            return Context.GastoAtracoes
                .Where(d => d.ItemGasto.IdentificadorViagem == IdentificadorViagem)
                .Where(predicate).ToList();
        }
        public List<GastoHotel> ListarGastoHotel(int? IdentificadorViagem, Expression<Func<GastoHotel, bool>> predicate)
        {
            return Context.GastoHoteis
                .Where(d => d.ItemGasto.IdentificadorViagem == IdentificadorViagem)
                .Where(predicate).ToList();
        }

        public List<GastoRefeicao> ListarGastoRefeicao(int? IdentificadorViagem, Expression<Func<GastoRefeicao, bool>> predicate)
        {
            return Context.GastoRefeicoes
                .Where(d => d.ItemGasto.IdentificadorViagem == IdentificadorViagem)
                .Where(predicate).ToList();
        }
        public List<GastoViagemAerea> ListarGastoViagemAerea(int? IdentificadorViagem, Expression<Func<GastoViagemAerea, bool>> predicate)
        {
            return Context.GastoViagemAereas
                .Where(d => d.ItemGasto.IdentificadorViagem == IdentificadorViagem)
                .Where(predicate).ToList();
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
        public IList<Usuario> CarregarParticipantesViagem(int? IdentificadorViagem, int? IdentificadorAmigo)
        {
            return this.Context.ParticipanteViagemes.Where(d => d.IdentificadorViagem == IdentificadorViagem).Select(e => e.ItemUsuario)
                .Where(d => this.Context.Amigos.Where(e => e.IdentificadorAmigo == IdentificadorAmigo && d.Identificador == e.IdentificadorUsuario).Any() || d.Identificador == IdentificadorAmigo)
                .ToList();
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
            query = query.Where(d => !d.Compras.Any());
            query = query.Where(d => !d.Reabastecimentos.Any());
            query = query.Where(d => !d.ApenasBaixa.Value);
            query = query.Where(d => !d.DataExclusao.HasValue);
            query = query.OrderByDescending(d => d.Data);
            return query.ToList();
        }

        public List<Refeicao> ListarRefeicao(int IdentificadorViagem, DateTime? DataDe, DateTime? DataAte,
             string Nome, string Tipo, int? IdentificadorCidade, int? IdentificadorRefeicao)
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

        public List<ViagemAerea> ListarViagemAerea(int IdentificadorViagem, DateTime? DataDe, DateTime? DataAte, string Companhia, int? Tipo,
            int Situacao, int? IdentificadorCidadeOrigem, int? IdentificadorCidadeDestino, int? IdentificadorViagemAerea)
        {
            IQueryable<ViagemAerea> query = this.Context.ViagemAereas;
            //.Include("ItemAtracaoPai").Include("Fotos.ItemFoto")
            //    .Include("Gastos").Include("Gastos.ItemGasto").Include("Gastos.ItemGasto.ItemUsuario")
            //    .Include("Avaliacoes");
            if (IdentificadorViagemAerea.HasValue)
                query = query.Where(d => d.Identificador == IdentificadorViagemAerea);
            query = query.Where(d => d.IdentificadorViagem == IdentificadorViagem);
            if (DataDe.HasValue)
                query = query.Where(d => d.DataPrevista >= DataDe);
            if (DataAte.HasValue)
            {
                DataAte = DataAte.GetValueOrDefault().AddDays(1);
                query = query.Where(d => d.DataPrevista < DataAte);
            }
            if (Tipo.HasValue)
                query = query.Where(d => d.Tipo == Tipo);

            if (!string.IsNullOrWhiteSpace(Companhia))
                query = query.Where(d => d.CompanhiaAerea.Contains(Companhia));


            if (Situacao == 1)
                query = query.Where(d => d.Aeroportos.Where(e => e.TipoPonto == (int)enumTipoParada.Origem && e.DataChegada.HasValue).Any())
                    .Where(d => d.Aeroportos.Where(e => e.TipoPonto == (int)enumTipoParada.Destino && !e.DataPartida.HasValue).Any());
            else if (Situacao == 2)
                query = query.Where(d => d.Aeroportos.Where(e => e.TipoPonto == (int)enumTipoParada.Origem && e.DataChegada.HasValue).Any())
                    .Where(d => d.Aeroportos.Where(e => e.TipoPonto == (int)enumTipoParada.Destino && e.DataPartida.HasValue).Any());
            else if (Situacao == 3)
                query = query.Where(d => d.Aeroportos.Where(e => e.TipoPonto == (int)enumTipoParada.Origem && !e.DataChegada.HasValue).Any());
            query = query.Where(d => !d.DataExclusao.HasValue);

            if (IdentificadorCidadeOrigem.HasValue)
                query = query.Where(d => d.Aeroportos.Where(e => e.TipoPonto == (int)enumTipoParada.Origem).Where(e => e.IdentificadorCidade == IdentificadorCidadeOrigem || this.Context.CidadeGrupos.Where(f => f.IdentificadorCidadeFilha == e.IdentificadorCidade).Where(f => f.IdentificadorViagem == IdentificadorViagem).Where(f => f.IdentificadorCidadePai == IdentificadorCidadeOrigem).Any()).Any());
            if (IdentificadorCidadeDestino.HasValue)
                query = query.Where(d => d.Aeroportos.Where(e => e.TipoPonto == (int)enumTipoParada.Destino).Where(e => e.IdentificadorCidade == IdentificadorCidadeDestino || this.Context.CidadeGrupos.Where(f => f.IdentificadorCidadeFilha == e.IdentificadorCidade).Where(f => f.IdentificadorViagem == IdentificadorViagem).Where(f => f.IdentificadorCidadePai == IdentificadorCidadeDestino).Any()).Any());

            query = query.OrderByDescending(d => d.DataPrevista);



            return query.ToList();
        }

        public List<Carro> ListarCarro(int IdentificadorViagem, string Locadora, string Descricao, string Modelo,
             DateTime? DataRetiradaDe, DateTime? DataRetiradaAte, DateTime? DataDevolucaoDe, DateTime? DataDevolucaoAte, int? IdentificadorCarro)
        {
            IQueryable<Carro> query = this.Context.Carros.Include("ItemCarroEventoDevolucao").Include("ItemCarroEventoRetirada");
            //.Include("ItemAtracaoPai").Include("Fotos.ItemFoto")
            //    .Include("Gastos").Include("Gastos.ItemGasto").Include("Gastos.ItemGasto.ItemUsuario")
            //    .Include("Avaliacoes");
            if (IdentificadorCarro.HasValue)
                query = query.Where(d => d.Identificador == IdentificadorCarro);
            query = query.Where(d => d.IdentificadorViagem == IdentificadorViagem);
            query = query.Where(d => !d.DataExclusao.HasValue);
            if (!string.IsNullOrEmpty(Locadora))
                query = query.Where(d => d.Alugado.Value && d.Locadora.Contains(Locadora));
            if (!string.IsNullOrEmpty(Modelo))
                query = query.Where(d => d.Modelo.Contains(Modelo));
            if (!string.IsNullOrEmpty(Descricao))
                query = query.Where(d => d.Descricao.Contains(Descricao));

            if (DataRetiradaDe.HasValue)
                query = query.Where(d => d.DataRetirada >= DataRetiradaDe);
            if (DataRetiradaAte.HasValue)
            {
                DataRetiradaAte = DataRetiradaAte.GetValueOrDefault().AddDays(1);
                query = query.Where(d => d.DataRetirada < DataRetiradaAte);
            }

            if (DataDevolucaoDe.HasValue)
                query = query.Where(d => d.DataDevolucao >= DataDevolucaoDe);
            if (DataDevolucaoAte.HasValue)
            {
                DataDevolucaoAte = DataDevolucaoAte.GetValueOrDefault().AddDays(1);
                query = query.Where(d => d.DataDevolucao < DataDevolucaoAte);
            }

            query = query.OrderByDescending(d => d.DataRetirada);



            return query.ToList();
        }


        public List<Loja> ListarLoja(int IdentificadorViagem, DateTime? DataDe, DateTime? DataAte,
           string Nome, int? IdentificadorCidade, int? IdentificadorLoja)
        {
            IQueryable<Loja> query = this.Context.Lojas;

            if (IdentificadorLoja.HasValue)
                query = query.Where(d => d.Identificador == IdentificadorLoja);
            query = query.Where(d => d.IdentificadorViagem == IdentificadorViagem);
            if (DataDe.HasValue)
                query = query.Where(f => f.Compras.Where(d => d.ItemGasto.Data >= DataDe).Any());
            if (DataAte.HasValue)
            {
                DataAte = DataAte.GetValueOrDefault().AddDays(1);
                query = query.Where(f => f.Compras.Where(d => d.ItemGasto.Data < DataAte).Any());
            }

            if (!string.IsNullOrWhiteSpace(Nome))
                query = query.Where(d => d.Nome.Contains(Nome));

            query = query.Where(d => !d.DataExclusao.HasValue);
            if (IdentificadorCidade.HasValue)
                query = query.Where(d => d.IdentificadorCidade == IdentificadorCidade || this.Context.CidadeGrupos.Where(e => e.IdentificadorCidadeFilha == d.IdentificadorCidade).Where(f => f.IdentificadorViagem == IdentificadorViagem).Where(e => e.IdentificadorCidadePai == IdentificadorCidade).Any());
            query = query.OrderBy(d => d.Nome);

            return query.ToList();
        }

        public List<ListaCompra> ListarListaCompra(int? IdentificadorUsuario, int? IdentificadorViagem, List<int?> Status, int? IdentificadorListaCompra)
        {
            IQueryable<ListaCompra> query = this.Context.ListaCompras;

            if (IdentificadorListaCompra.HasValue)
                query = query.Where(d => d.Identificador == IdentificadorListaCompra);
            query = query.Where(d => d.IdentificadorViagem == IdentificadorViagem);
            if (IdentificadorUsuario.HasValue)
                query = query.Where(d => d.IdentificadorUsuarioPedido == IdentificadorUsuario || d.IdentificadorUsuario == IdentificadorUsuario);
            query = query.Where(d => !d.DataExclusao.HasValue);
            if (Status != null && Status.Any())
                query = query.Where(d => Status.Contains(d.Status));
            return query.OrderBy(d => d.Marca).ThenBy(d => d.Descricao).ToList();
        }

        public List<CalendarioPrevisto> ListarCalendarioPrevisto( int? IdentificadorViagem)
        {
            IQueryable<CalendarioPrevisto> query = this.Context.CalendarioPrevistos;

            query = query.Where(d => d.IdentificadorViagem == IdentificadorViagem);
            query = query.Where(d => !d.DataExclusao.HasValue);
            return query.ToList();
        }

        public List<Comentario> ListarComentario(int? IdentificadorUsuario, int? IdentificadorViagem, DateTime? DataDe, DateTime? DataAte, int? IdentificadorCidade, int? IdentificadorComentario)
        {

            IQueryable<Comentario> query = this.Context.Comentarios;

            if (IdentificadorComentario.HasValue)
                query = query.Where(d => d.Identificador == IdentificadorComentario);
            query = query.Where(d => d.IdentificadorViagem == IdentificadorViagem);
            if (DataDe.HasValue)
                query = query.Where(d => d.Data >= DataDe);
            if (DataAte.HasValue)
            {
                DataAte = DataAte.GetValueOrDefault().AddDays(1);
                query = query.Where(d => d.Data < DataAte);
            }

            if (IdentificadorUsuario.HasValue)
                query = query.Where(d => d.IdentificadorUsuario == IdentificadorUsuario);

            query = query.Where(d => !d.DataExclusao.HasValue);
            if (IdentificadorCidade.HasValue)
                query = query.Where(d => d.IdentificadorCidade == IdentificadorCidade || this.Context.CidadeGrupos.Where(e => e.IdentificadorCidadeFilha == d.IdentificadorCidade).Where(f => f.IdentificadorViagem == IdentificadorViagem).Where(e => e.IdentificadorCidadePai == IdentificadorCidade).Any());
            query = query.OrderByDescending(d => d.Data);

            return query.ToList();

        }


        public List<ListaCompra> ListarListaCompra(int? IdentificadorUsuario, int? IdentificadorViagem, int? Status, string Destinatario, int? IdentificadorUsuarioPedido,
            string Marca, string Descricao)
        {
            IQueryable<ListaCompra> query = this.Context.ListaCompras.Include("ItemUsuario").Include("ItemUsuarioPedido");
            if (!string.IsNullOrEmpty(Marca))
                query = query.Where(d => d.Marca.Contains(Marca));

            if (!string.IsNullOrEmpty(Descricao))
                query = query.Where(d => d.Descricao.Contains(Descricao));

            if (IdentificadorUsuario.HasValue)
                query = query.Where(d => d.IdentificadorUsuario == IdentificadorUsuario);
            if (IdentificadorUsuarioPedido.HasValue)
                query = query.Where(d => d.IdentificadorUsuarioPedido == IdentificadorUsuarioPedido);
            if (!string.IsNullOrEmpty(Destinatario))
                query = query.Where(d => d.ItemUsuarioPedido.Nome.Contains(Destinatario) || d.Destinatario.Contains(Destinatario));
            query = query.Where(d => d.IdentificadorViagem == IdentificadorViagem);
            query = query.Where(d => !d.DataExclusao.HasValue);
            if (Status.GetValueOrDefault(-1) >= 0)
            {
                if (Status == 1)
                    query = query.Where(d => d.Status <= Status);
                else
                    query = query.Where(d => d.Status == Status);
            }
            return query.OrderBy(d => d.Marca).ThenBy(d => d.Descricao).ToList();
        }



        public List<AporteDinheiro> ListarAporteDinheiro(int? IdentificadorUsuario, int? IdentificadorViagem, int? Moeda, DateTime? DataDe, DateTime? DataAte)
        {
            IQueryable<AporteDinheiro> query = this.Context.AporteDinheiros.Include("ItemGasto");

            if (IdentificadorUsuario.HasValue)
                query = query.Where(d => d.IdentificadorUsuario == IdentificadorUsuario);
            if (Moeda.HasValue)
                query = query.Where(d => d.Moeda == Moeda);
            query = query.Where(d => d.IdentificadorViagem == IdentificadorViagem);
            query = query.Where(d => !d.DataExclusao.HasValue);
            if (DataDe.HasValue)
                query = query.Where(d => d.DataAporte >= DataDe);
            if (DataAte.HasValue)
            {
                DataAte = DataAte.GetValueOrDefault().AddDays(1);
                query = query.Where(d => d.DataAporte < DataAte);
            }
            return query.OrderByDescending(d => d.DataAporte).ToList();
        }

        public List<Sugestao> ListarSugestao(int? IdentificadorUsuario, int? IdentificadorViagem, string Nome, string Tipo, int? IdentificadorCidade, int? Situacao)
        {
            IQueryable<Sugestao> query = this.Context.Sugestoes.Include("ItemUsuario").Include("ItemCidade");

            if (IdentificadorUsuario.HasValue)
                query = query.Where(d => d.IdentificadorUsuario == IdentificadorUsuario);
            query = query.Where(d => d.IdentificadorViagem == IdentificadorViagem);
            query = query.Where(d => !d.DataExclusao.HasValue);
            if (!string.IsNullOrEmpty(Nome))
                query = query.Where(d => d.Local.Contains(Nome));
            if (!string.IsNullOrEmpty(Tipo))
                query = query.Where(d => d.Tipo.Contains(Tipo));
            if (Situacao.HasValue && Situacao > -1)
            {
                if (Situacao == 1)
                    query = query.Where(d => d.Status <= 1);
                else
                    query = query.Where(d=>d.Status == Situacao);
            }
            if (IdentificadorCidade.HasValue)
                query = query.Where(d => d.IdentificadorCidade == IdentificadorCidade || this.Context.CidadeGrupos.Where(e => e.IdentificadorCidadeFilha == d.IdentificadorCidade).Where(f => f.IdentificadorViagem == IdentificadorViagem).Where(e => e.IdentificadorCidadePai == IdentificadorCidade).Any());

            return query.OrderBy(f => f.Local).ToList();
        }

        public List<Cidade> ListarCidadePai(int? IdentificadorViagem)
        {
            IQueryable<CidadeGrupo> query = this.Context.CidadeGrupos.Where(d => d.IdentificadorViagem == IdentificadorViagem);
            IQueryable<Cidade> queryCidade = this.Context.Cidades.Include("ItemPais")
                .Where(d => query.Where(e => e.IdentificadorCidadePai == d.Identificador).Any())
                .OrderBy(d => d.Nome);
            return queryCidade.ToList();
        }

        public List<Cidade> ListarCidadeNaoAssociadasFilho(int? IdentificadorViagem)
        {
            var queryTodasCidades = ListarCidadesViagem(IdentificadorViagem);
            return queryTodasCidades.Where(d => !this.Context.CidadeGrupos.Where(e => e.IdentificadorViagem == IdentificadorViagem).Where(e => e.IdentificadorCidadeFilha == d.Identificador).Any())
                  .OrderBy(d => d.Nome).ToList();
        }

        public List<Cidade> ListarCidadeNaoAssociadasPai(int? IdentificadorViagem,int? IdentificadorCidadePai)
        {
            int? IdentificadorCidade = IdentificadorCidadePai.GetValueOrDefault(0);
            var queryTodasCidades = ListarCidadesViagem(IdentificadorViagem);
            return queryTodasCidades.Where(d => !this.Context.CidadeGrupos.Where(e => e.IdentificadorViagem == IdentificadorViagem).Where(e => e.IdentificadorCidadePai == d.Identificador).Any())
                .Where(d => !this.Context.CidadeGrupos.Where(e => e.IdentificadorViagem == IdentificadorViagem).Where(e=>e.IdentificadorCidadePai != IdentificadorCidade).Where(e => e.IdentificadorCidadeFilha == d.Identificador).Any())
                  .OrderBy(d => d.Nome).ToList();
        }

        public List<ManutencaoCidadeGrupo> ListarManutencaoCidadeGrupo(int? IdentificadorViagem, int? IdentificadorCidadePai)
        {
            IQueryable<CidadeGrupo> query = this.Context.CidadeGrupos.Where(d => d.IdentificadorCidadePai == IdentificadorCidadePai).Where(d => d.IdentificadorViagem == IdentificadorViagem);
            return query.GroupBy(d => d.IdentificadorCidadePai)
                .Select(d => new ManutencaoCidadeGrupo() { Edicao = true, IdentificadorCidade = d.Key, CidadesFilhas = d.Select(f => f.IdentificadorCidadeFilha) })
                .ToList();
        }

        public List<CidadeGrupo> ListarCidadeGrupo(int? IdentificadorViagem, int? IdentificadorCidadePai)
        {
            IQueryable<CidadeGrupo> query = this.Context.CidadeGrupos.Where(d => d.IdentificadorCidadePai == IdentificadorCidadePai).Where(d => d.IdentificadorViagem == IdentificadorViagem);
            return query.ToList();
        }

        private IQueryable<Cidade>  ListarCidadesViagem(int? IdentificadorViagem)
        {
            IQueryable<Cidade> query = this.Context.Atracoes.Where(d=>d.IdentificadorViagem == IdentificadorViagem).Where(d => d.IdentificadorCidade.HasValue).Select(d => d.ItemCidade);
           // query = query.Union(this.Context.CalendarioPrevistos.Where(d => d.IdentificadorViagem == IdentificadorViagem).Where(d => d.IdentificadorCidade.HasValue).Select(d => d.ItemCidade));
            query = query.Union(this.Context.CarroDeslocamentos.Where(d => d.ItemCarro.IdentificadorViagem == IdentificadorViagem).Where(d=>d.IdentificadorCarroEventoChegada.HasValue).Where(d => d.ItemCarroEventoChegada.IdentificadorCidade.HasValue).Select(d => d.ItemCarroEventoChegada.ItemCidade));
            query = query.Union(this.Context.CarroDeslocamentos.Where(d => d.ItemCarro.IdentificadorViagem == IdentificadorViagem).Where(d => d.IdentificadorCarroEventoPartida.HasValue).Where(d => d.ItemCarroEventoPartida.IdentificadorCidade.HasValue).Select(d => d.ItemCarroEventoPartida.ItemCidade));
            query = query.Union(this.Context.Carros.Where(d => d.IdentificadorViagem == IdentificadorViagem).Where(d => d.IdentificadorCarroEventoDevolucao.HasValue).Where(d => d.ItemCarroEventoDevolucao.IdentificadorCidade.HasValue).Select(d => d.ItemCarroEventoDevolucao.ItemCidade));
            query = query.Union(this.Context.Carros.Where(d => d.IdentificadorViagem == IdentificadorViagem).Where(d => d.IdentificadorCarroEventoRetirada.HasValue).Where(d => d.ItemCarroEventoRetirada.IdentificadorCidade.HasValue).Select(d => d.ItemCarroEventoRetirada.ItemCidade));
            query = query.Union(this.Context.Comentarios.Where(d => d.IdentificadorViagem == IdentificadorViagem).Where(d => d.IdentificadorCidade.HasValue).Select(d => d.ItemCidade));
            query = query.Union(this.Context.Fotos.Where(d => d.IdentificadorViagem == IdentificadorViagem).Where(d => d.IdentificadorCidade.HasValue).Select(d => d.ItemCidade));
            query = query.Union(this.Context.Gastos.Where(d => d.IdentificadorViagem == IdentificadorViagem).Where(d => d.IdentificadorCidade.HasValue).Select(d => d.ItemCidade));
            query = query.Union(this.Context.Hoteis.Where(d => d.IdentificadorViagem == IdentificadorViagem).Where(d => d.IdentificadorCidade.HasValue).Select(d => d.ItemCidade));
            query = query.Union(this.Context.Lojas.Where(d => d.IdentificadorViagem == IdentificadorViagem).Where(d => d.IdentificadorCidade.HasValue).Select(d => d.ItemCidade));
            query = query.Union(this.Context.Reabastecimentos.Where(d => d.ItemCarro.IdentificadorViagem == IdentificadorViagem).Where(d => d.IdentificadorCidade.HasValue).Select(d => d.ItemCidade));
            query = query.Union(this.Context.Refeicoes.Where(d => d.IdentificadorViagem == IdentificadorViagem).Where(d => d.IdentificadorCidade.HasValue).Select(d => d.ItemCidade));
            query = query.Union(this.Context.Sugestoes.Where(d => d.IdentificadorViagem == IdentificadorViagem).Where(d => d.IdentificadorCidade.HasValue).Select(d => d.ItemCidade));
            query = query.Union(this.Context.ViagemAereaAeroportos.Where(d => d.ItemViagemAerea.IdentificadorViagem == IdentificadorViagem).Where(d => d.IdentificadorCidade.HasValue).Select(d => d.ItemCidade));
            return query.Distinct().OrderBy(d => d.Nome);
        }

        public void SalvarReabastecimentoCompleto(Reabastecimento itemGravar)
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
                        itemBaseGasto.Usuarios = new List<GastoDividido>();
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

    }
}
