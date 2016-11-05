using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CV.Model;
using System.Linq.Expressions;

namespace CV.Data
{
	public partial class ViagemRepository: RepositoryBase
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
            return ListaGrupo.GroupJoin(this.Context.Usuarios, d => d.IdentificadorAmigo, d => d.Identificador, (a, u) => new ConsultaAmigo() { IdentificadorUsuario = a.IdentificadorAmigo, EMail = a.EMail, Nome = u.Select(e=>e.Nome).FirstOrDefault(), Seguido = a.Seguido, Seguidor = a.Seguidor }).ToList();
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
    }
}
