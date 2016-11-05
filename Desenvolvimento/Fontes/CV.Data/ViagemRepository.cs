using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using CV.Model;

namespace CV.Data
{
	public partial class ViagemRepository: RepositoryBase
	{
			public AluguelGasto SelecionarAluguelGasto (int? Identificador)
			{
			IQueryable<AluguelGasto> query =	 Context.AluguelGastos
;
					if (Identificador.HasValue)
					query = query.Where(d=>d.Identificador == Identificador);
					return query.FirstOrDefault();
			}
			public IList<AluguelGasto> ListarAluguelGasto ()
			{
			IQueryable<AluguelGasto> query =	 Context.AluguelGastos
;
				return query.ToList();
			}
			public void SalvarAluguelGasto (AluguelGasto itemGravar)
			{
				AluguelGasto itemBase =  Context.AluguelGastos
				.Where(f=>f.Identificador == itemGravar.Identificador).FirstOrDefault();
				if (itemBase == null)
				{
				itemBase = Context.AluguelGastos.Create();
 			Context.Entry<AluguelGasto>(itemBase).State = System.Data.Entity.EntityState.Added;
				}
 			AtualizarPropriedades<AluguelGasto>(itemBase, itemGravar);
			Context.SaveChanges();
				itemGravar.Identificador = itemBase.Identificador;
			}
			public void ExcluirAluguelGasto (AluguelGasto itemGravar)
			{
				AluguelGasto itemExcluir =  Context.AluguelGastos
			.Where(f=>f.Identificador == itemGravar.Identificador).FirstOrDefault();
						Context.Entry<AluguelGasto>(itemExcluir).State = EntityState.Deleted;
				Context.SaveChanges();
			}
			public Reabastecimento SelecionarReabastecimento (int? Identificador)
			{
			IQueryable<Reabastecimento> query =	 Context.Reabastecimentos
;
					if (Identificador.HasValue)
					query = query.Where(d=>d.Identificador == Identificador);
					return query.FirstOrDefault();
			}
			public IList<Reabastecimento> ListarReabastecimento ()
			{
			IQueryable<Reabastecimento> query =	 Context.Reabastecimentos
;
				return query.ToList();
			}
			public void SalvarReabastecimento (Reabastecimento itemGravar)
			{
				Reabastecimento itemBase =  Context.Reabastecimentos
				.Where(f=>f.Identificador == itemGravar.Identificador).FirstOrDefault();
				if (itemBase == null)
				{
				itemBase = Context.Reabastecimentos.Create();
 			Context.Entry<Reabastecimento>(itemBase).State = System.Data.Entity.EntityState.Added;
				}
 			AtualizarPropriedades<Reabastecimento>(itemBase, itemGravar);
			Context.SaveChanges();
				itemGravar.Identificador = itemBase.Identificador;
			}
			public void ExcluirReabastecimento (Reabastecimento itemGravar)
			{
				Reabastecimento itemExcluir =  Context.Reabastecimentos
			.Where(f=>f.Identificador == itemGravar.Identificador).FirstOrDefault();
						Context.Entry<Reabastecimento>(itemExcluir).State = EntityState.Deleted;
				Context.SaveChanges();
			}
			public Amigo SelecionarAmigo (int? Identificador)
			{
			IQueryable<Amigo> query =	 Context.Amigos
;
					if (Identificador.HasValue)
					query = query.Where(d=>d.Identificador == Identificador);
					return query.FirstOrDefault();
			}
			public IList<Amigo> ListarAmigo ()
			{
			IQueryable<Amigo> query =	 Context.Amigos
;
				return query.ToList();
			}
			public void SalvarAmigo (Amigo itemGravar)
			{
				Amigo itemBase =  Context.Amigos
				.Where(f=>f.Identificador == itemGravar.Identificador).FirstOrDefault();
				if (itemBase == null)
				{
				itemBase = Context.Amigos.Create();
 			Context.Entry<Amigo>(itemBase).State = System.Data.Entity.EntityState.Added;
				}
 			AtualizarPropriedades<Amigo>(itemBase, itemGravar);
			Context.SaveChanges();
				itemGravar.Identificador = itemBase.Identificador;
			}
			public void ExcluirAmigo (Amigo itemGravar)
			{
				Amigo itemExcluir =  Context.Amigos
			.Where(f=>f.Identificador == itemGravar.Identificador).FirstOrDefault();
						Context.Entry<Amigo>(itemExcluir).State = EntityState.Deleted;
				Context.SaveChanges();
			}
			public AporteDinheiro SelecionarAporteDinheiro (int? Identificador)
			{
			IQueryable<AporteDinheiro> query =	 Context.AporteDinheiros
;
					if (Identificador.HasValue)
					query = query.Where(d=>d.Identificador == Identificador);
					return query.FirstOrDefault();
			}
			public IList<AporteDinheiro> ListarAporteDinheiro ()
			{
			IQueryable<AporteDinheiro> query =	 Context.AporteDinheiros
;
				return query.ToList();
			}
			public void SalvarAporteDinheiro (AporteDinheiro itemGravar)
			{
				AporteDinheiro itemBase =  Context.AporteDinheiros
				.Where(f=>f.Identificador == itemGravar.Identificador).FirstOrDefault();
				if (itemBase == null)
				{
				itemBase = Context.AporteDinheiros.Create();
 			Context.Entry<AporteDinheiro>(itemBase).State = System.Data.Entity.EntityState.Added;
				}
 			AtualizarPropriedades<AporteDinheiro>(itemBase, itemGravar);
			Context.SaveChanges();
				itemGravar.Identificador = itemBase.Identificador;
			}
			public void ExcluirAporteDinheiro (AporteDinheiro itemGravar)
			{
				AporteDinheiro itemExcluir =  Context.AporteDinheiros
			.Where(f=>f.Identificador == itemGravar.Identificador).FirstOrDefault();
						Context.Entry<AporteDinheiro>(itemExcluir).State = EntityState.Deleted;
				Context.SaveChanges();
			}
			public Atracao SelecionarAtracao (int? Identificador)
			{
			IQueryable<Atracao> query =	 Context.Atracoes
;
					if (Identificador.HasValue)
					query = query.Where(d=>d.Identificador == Identificador);
					return query.FirstOrDefault();
			}
			public IList<Atracao> ListarAtracao ()
			{
			IQueryable<Atracao> query =	 Context.Atracoes
;
				return query.ToList();
			}
			public void SalvarAtracao (Atracao itemGravar)
			{
				Atracao itemBase =  Context.Atracoes
				.Where(f=>f.Identificador == itemGravar.Identificador).FirstOrDefault();
				if (itemBase == null)
				{
				itemBase = Context.Atracoes.Create();
 			Context.Entry<Atracao>(itemBase).State = System.Data.Entity.EntityState.Added;
				}
 			AtualizarPropriedades<Atracao>(itemBase, itemGravar);
			Context.SaveChanges();
				itemGravar.Identificador = itemBase.Identificador;
			}
			public void ExcluirAtracao (Atracao itemGravar)
			{
				Atracao itemExcluir =  Context.Atracoes
			.Where(f=>f.Identificador == itemGravar.Identificador).FirstOrDefault();
						Context.Entry<Atracao>(itemExcluir).State = EntityState.Deleted;
				Context.SaveChanges();
			}
			public AvaliacaoAerea SelecionarAvaliacaoAerea (int? Identificador)
			{
			IQueryable<AvaliacaoAerea> query =	 Context.AvaliacaoAereas
;
					if (Identificador.HasValue)
					query = query.Where(d=>d.Identificador == Identificador);
					return query.FirstOrDefault();
			}
			public IList<AvaliacaoAerea> ListarAvaliacaoAerea ()
			{
			IQueryable<AvaliacaoAerea> query =	 Context.AvaliacaoAereas
;
				return query.ToList();
			}
			public void SalvarAvaliacaoAerea (AvaliacaoAerea itemGravar)
			{
				AvaliacaoAerea itemBase =  Context.AvaliacaoAereas
				.Where(f=>f.Identificador == itemGravar.Identificador).FirstOrDefault();
				if (itemBase == null)
				{
				itemBase = Context.AvaliacaoAereas.Create();
 			Context.Entry<AvaliacaoAerea>(itemBase).State = System.Data.Entity.EntityState.Added;
				}
 			AtualizarPropriedades<AvaliacaoAerea>(itemBase, itemGravar);
			Context.SaveChanges();
				itemGravar.Identificador = itemBase.Identificador;
			}
			public void ExcluirAvaliacaoAerea (AvaliacaoAerea itemGravar)
			{
				AvaliacaoAerea itemExcluir =  Context.AvaliacaoAereas
			.Where(f=>f.Identificador == itemGravar.Identificador).FirstOrDefault();
						Context.Entry<AvaliacaoAerea>(itemExcluir).State = EntityState.Deleted;
				Context.SaveChanges();
			}
			public AvaliacaoAluguel SelecionarAvaliacaoAluguel (int? Identificador)
			{
			IQueryable<AvaliacaoAluguel> query =	 Context.AvaliacaoAlugueis
;
					if (Identificador.HasValue)
					query = query.Where(d=>d.Identificador == Identificador);
					return query.FirstOrDefault();
			}
			public IList<AvaliacaoAluguel> ListarAvaliacaoAluguel ()
			{
			IQueryable<AvaliacaoAluguel> query =	 Context.AvaliacaoAlugueis
;
				return query.ToList();
			}
			public void SalvarAvaliacaoAluguel (AvaliacaoAluguel itemGravar)
			{
				AvaliacaoAluguel itemBase =  Context.AvaliacaoAlugueis
				.Where(f=>f.Identificador == itemGravar.Identificador).FirstOrDefault();
				if (itemBase == null)
				{
				itemBase = Context.AvaliacaoAlugueis.Create();
 			Context.Entry<AvaliacaoAluguel>(itemBase).State = System.Data.Entity.EntityState.Added;
				}
 			AtualizarPropriedades<AvaliacaoAluguel>(itemBase, itemGravar);
			Context.SaveChanges();
				itemGravar.Identificador = itemBase.Identificador;
			}
			public void ExcluirAvaliacaoAluguel (AvaliacaoAluguel itemGravar)
			{
				AvaliacaoAluguel itemExcluir =  Context.AvaliacaoAlugueis
			.Where(f=>f.Identificador == itemGravar.Identificador).FirstOrDefault();
						Context.Entry<AvaliacaoAluguel>(itemExcluir).State = EntityState.Deleted;
				Context.SaveChanges();
			}
			public AvaliacaoAtracao SelecionarAvaliacaoAtracao (int? Identificador)
			{
			IQueryable<AvaliacaoAtracao> query =	 Context.AvaliacaoAtracoes
;
					if (Identificador.HasValue)
					query = query.Where(d=>d.Identificador == Identificador);
					return query.FirstOrDefault();
			}
			public IList<AvaliacaoAtracao> ListarAvaliacaoAtracao ()
			{
			IQueryable<AvaliacaoAtracao> query =	 Context.AvaliacaoAtracoes
;
				return query.ToList();
			}
			public void SalvarAvaliacaoAtracao (AvaliacaoAtracao itemGravar)
			{
				AvaliacaoAtracao itemBase =  Context.AvaliacaoAtracoes
				.Where(f=>f.Identificador == itemGravar.Identificador).FirstOrDefault();
				if (itemBase == null)
				{
				itemBase = Context.AvaliacaoAtracoes.Create();
 			Context.Entry<AvaliacaoAtracao>(itemBase).State = System.Data.Entity.EntityState.Added;
				}
 			AtualizarPropriedades<AvaliacaoAtracao>(itemBase, itemGravar);
			Context.SaveChanges();
				itemGravar.Identificador = itemBase.Identificador;
			}
			public void ExcluirAvaliacaoAtracao (AvaliacaoAtracao itemGravar)
			{
				AvaliacaoAtracao itemExcluir =  Context.AvaliacaoAtracoes
			.Where(f=>f.Identificador == itemGravar.Identificador).FirstOrDefault();
						Context.Entry<AvaliacaoAtracao>(itemExcluir).State = EntityState.Deleted;
				Context.SaveChanges();
			}
			public CalendarioPrevisto SelecionarCalendarioPrevisto (int? Identificador)
			{
			IQueryable<CalendarioPrevisto> query =	 Context.CalendarioPrevistos
;
					if (Identificador.HasValue)
					query = query.Where(d=>d.Identificador == Identificador);
					return query.FirstOrDefault();
			}
			public IList<CalendarioPrevisto> ListarCalendarioPrevisto ()
			{
			IQueryable<CalendarioPrevisto> query =	 Context.CalendarioPrevistos
;
				return query.ToList();
			}
			public void SalvarCalendarioPrevisto (CalendarioPrevisto itemGravar)
			{
				CalendarioPrevisto itemBase =  Context.CalendarioPrevistos
				.Where(f=>f.Identificador == itemGravar.Identificador).FirstOrDefault();
				if (itemBase == null)
				{
				itemBase = Context.CalendarioPrevistos.Create();
 			Context.Entry<CalendarioPrevisto>(itemBase).State = System.Data.Entity.EntityState.Added;
				}
 			AtualizarPropriedades<CalendarioPrevisto>(itemBase, itemGravar);
			Context.SaveChanges();
				itemGravar.Identificador = itemBase.Identificador;
			}
			public void ExcluirCalendarioPrevisto (CalendarioPrevisto itemGravar)
			{
				CalendarioPrevisto itemExcluir =  Context.CalendarioPrevistos
			.Where(f=>f.Identificador == itemGravar.Identificador).FirstOrDefault();
						Context.Entry<CalendarioPrevisto>(itemExcluir).State = EntityState.Deleted;
				Context.SaveChanges();
			}
			public Carro SelecionarCarro (int? Identificador)
			{
			IQueryable<Carro> query =	 Context.Carros
;
					if (Identificador.HasValue)
					query = query.Where(d=>d.Identificador == Identificador);
					return query.FirstOrDefault();
			}
			public IList<Carro> ListarCarro ()
			{
			IQueryable<Carro> query =	 Context.Carros
;
				return query.ToList();
			}
			public void SalvarCarro (Carro itemGravar)
			{
				Carro itemBase =  Context.Carros
				.Where(f=>f.Identificador == itemGravar.Identificador).FirstOrDefault();
				if (itemBase == null)
				{
				itemBase = Context.Carros.Create();
 			Context.Entry<Carro>(itemBase).State = System.Data.Entity.EntityState.Added;
				}
 			AtualizarPropriedades<Carro>(itemBase, itemGravar);
			Context.SaveChanges();
				itemGravar.Identificador = itemBase.Identificador;
			}
			public void ExcluirCarro (Carro itemGravar)
			{
				Carro itemExcluir =  Context.Carros
			.Where(f=>f.Identificador == itemGravar.Identificador).FirstOrDefault();
						Context.Entry<Carro>(itemExcluir).State = EntityState.Deleted;
				Context.SaveChanges();
			}
			public CarroEvento SelecionarCarroEvento (int? Identificador)
			{
			IQueryable<CarroEvento> query =	 Context.CarroEventos
;
					if (Identificador.HasValue)
					query = query.Where(d=>d.Identificador == Identificador);
					return query.FirstOrDefault();
			}
			public IList<CarroEvento> ListarCarroEvento ()
			{
			IQueryable<CarroEvento> query =	 Context.CarroEventos
;
				return query.ToList();
			}
			public void SalvarCarroEvento (CarroEvento itemGravar)
			{
				CarroEvento itemBase =  Context.CarroEventos
				.Where(f=>f.Identificador == itemGravar.Identificador).FirstOrDefault();
				if (itemBase == null)
				{
				itemBase = Context.CarroEventos.Create();
 			Context.Entry<CarroEvento>(itemBase).State = System.Data.Entity.EntityState.Added;
				}
 			AtualizarPropriedades<CarroEvento>(itemBase, itemGravar);
			Context.SaveChanges();
				itemGravar.Identificador = itemBase.Identificador;
			}
			public void ExcluirCarroEvento (CarroEvento itemGravar)
			{
				CarroEvento itemExcluir =  Context.CarroEventos
			.Where(f=>f.Identificador == itemGravar.Identificador).FirstOrDefault();
						Context.Entry<CarroEvento>(itemExcluir).State = EntityState.Deleted;
				Context.SaveChanges();
			}
			public Cidade SelecionarCidade (int? Identificador)
			{
			IQueryable<Cidade> query =	 Context.Cidades
;
					if (Identificador.HasValue)
					query = query.Where(d=>d.Identificador == Identificador);
					return query.FirstOrDefault();
			}
			public IList<Cidade> ListarCidade ()
			{
			IQueryable<Cidade> query =	 Context.Cidades
;
				return query.ToList();
			}
			public void SalvarCidade (Cidade itemGravar)
			{
				Cidade itemBase =  Context.Cidades
				.Where(f=>f.Identificador == itemGravar.Identificador).FirstOrDefault();
				if (itemBase == null)
				{
				itemBase = Context.Cidades.Create();
 			Context.Entry<Cidade>(itemBase).State = System.Data.Entity.EntityState.Added;
				}
 			AtualizarPropriedades<Cidade>(itemBase, itemGravar);
			Context.SaveChanges();
				itemGravar.Identificador = itemBase.Identificador;
			}
			public void ExcluirCidade (Cidade itemGravar)
			{
				Cidade itemExcluir =  Context.Cidades
			.Where(f=>f.Identificador == itemGravar.Identificador).FirstOrDefault();
						Context.Entry<Cidade>(itemExcluir).State = EntityState.Deleted;
				Context.SaveChanges();
			}
			public CidadeGrupo SelecionarCidadeGrupo (int? Identificador)
			{
			IQueryable<CidadeGrupo> query =	 Context.CidadeGrupos
;
					if (Identificador.HasValue)
					query = query.Where(d=>d.Identificador == Identificador);
					return query.FirstOrDefault();
			}
			public IList<CidadeGrupo> ListarCidadeGrupo ()
			{
			IQueryable<CidadeGrupo> query =	 Context.CidadeGrupos
;
				return query.ToList();
			}
			public void SalvarCidadeGrupo (CidadeGrupo itemGravar)
			{
				CidadeGrupo itemBase =  Context.CidadeGrupos
				.Where(f=>f.Identificador == itemGravar.Identificador).FirstOrDefault();
				if (itemBase == null)
				{
				itemBase = Context.CidadeGrupos.Create();
 			Context.Entry<CidadeGrupo>(itemBase).State = System.Data.Entity.EntityState.Added;
				}
 			AtualizarPropriedades<CidadeGrupo>(itemBase, itemGravar);
			Context.SaveChanges();
				itemGravar.Identificador = itemBase.Identificador;
			}
			public void ExcluirCidadeGrupo (CidadeGrupo itemGravar)
			{
				CidadeGrupo itemExcluir =  Context.CidadeGrupos
			.Where(f=>f.Identificador == itemGravar.Identificador).FirstOrDefault();
						Context.Entry<CidadeGrupo>(itemExcluir).State = EntityState.Deleted;
				Context.SaveChanges();
			}
			public Comentario SelecionarComentario (int? Identificador)
			{
			IQueryable<Comentario> query =	 Context.Comentarios
;
					if (Identificador.HasValue)
					query = query.Where(d=>d.Identificador == Identificador);
					return query.FirstOrDefault();
			}
			public IList<Comentario> ListarComentario ()
			{
			IQueryable<Comentario> query =	 Context.Comentarios
;
				return query.ToList();
			}
			public void SalvarComentario (Comentario itemGravar)
			{
				Comentario itemBase =  Context.Comentarios
				.Where(f=>f.Identificador == itemGravar.Identificador).FirstOrDefault();
				if (itemBase == null)
				{
				itemBase = Context.Comentarios.Create();
 			Context.Entry<Comentario>(itemBase).State = System.Data.Entity.EntityState.Added;
				}
 			AtualizarPropriedades<Comentario>(itemBase, itemGravar);
			Context.SaveChanges();
				itemGravar.Identificador = itemBase.Identificador;
			}
			public void ExcluirComentario (Comentario itemGravar)
			{
				Comentario itemExcluir =  Context.Comentarios
			.Where(f=>f.Identificador == itemGravar.Identificador).FirstOrDefault();
						Context.Entry<Comentario>(itemExcluir).State = EntityState.Deleted;
				Context.SaveChanges();
			}
			public CotacaoMoeda SelecionarCotacaoMoeda (int? Identificador)
			{
			IQueryable<CotacaoMoeda> query =	 Context.CotacaoMoedas
;
					if (Identificador.HasValue)
					query = query.Where(d=>d.Identificador == Identificador);
					return query.FirstOrDefault();
			}
			public IList<CotacaoMoeda> ListarCotacaoMoeda ()
			{
			IQueryable<CotacaoMoeda> query =	 Context.CotacaoMoedas
;
				return query.ToList();
			}
			public void SalvarCotacaoMoeda (CotacaoMoeda itemGravar)
			{
				CotacaoMoeda itemBase =  Context.CotacaoMoedas
				.Where(f=>f.Identificador == itemGravar.Identificador).FirstOrDefault();
				if (itemBase == null)
				{
				itemBase = Context.CotacaoMoedas.Create();
 			Context.Entry<CotacaoMoeda>(itemBase).State = System.Data.Entity.EntityState.Added;
				}
 			AtualizarPropriedades<CotacaoMoeda>(itemBase, itemGravar);
			Context.SaveChanges();
				itemGravar.Identificador = itemBase.Identificador;
			}
			public void ExcluirCotacaoMoeda (CotacaoMoeda itemGravar)
			{
				CotacaoMoeda itemExcluir =  Context.CotacaoMoedas
			.Where(f=>f.Identificador == itemGravar.Identificador).FirstOrDefault();
						Context.Entry<CotacaoMoeda>(itemExcluir).State = EntityState.Deleted;
				Context.SaveChanges();
			}
			public Foto SelecionarFoto (int? Identificador)
			{
			IQueryable<Foto> query =	 Context.Fotos
;
					if (Identificador.HasValue)
					query = query.Where(d=>d.Identificador == Identificador);
					return query.FirstOrDefault();
			}
			public IList<Foto> ListarFoto ()
			{
			IQueryable<Foto> query =	 Context.Fotos
;
				return query.ToList();
			}
			public void SalvarFoto (Foto itemGravar)
			{
				Foto itemBase =  Context.Fotos
				.Where(f=>f.Identificador == itemGravar.Identificador).FirstOrDefault();
				if (itemBase == null)
				{
				itemBase = Context.Fotos.Create();
 			Context.Entry<Foto>(itemBase).State = System.Data.Entity.EntityState.Added;
				}
 			AtualizarPropriedades<Foto>(itemBase, itemGravar);
			Context.SaveChanges();
				itemGravar.Identificador = itemBase.Identificador;
			}
			public void ExcluirFoto (Foto itemGravar)
			{
				Foto itemExcluir =  Context.Fotos
			.Where(f=>f.Identificador == itemGravar.Identificador).FirstOrDefault();
						Context.Entry<Foto>(itemExcluir).State = EntityState.Deleted;
				Context.SaveChanges();
			}
			public FotoAtracao SelecionarFotoAtracao (int? Identificador)
			{
			IQueryable<FotoAtracao> query =	 Context.FotoAtracoes
;
					if (Identificador.HasValue)
					query = query.Where(d=>d.Identificador == Identificador);
					return query.FirstOrDefault();
			}
			public IList<FotoAtracao> ListarFotoAtracao ()
			{
			IQueryable<FotoAtracao> query =	 Context.FotoAtracoes
;
				return query.ToList();
			}
			public void SalvarFotoAtracao (FotoAtracao itemGravar)
			{
				FotoAtracao itemBase =  Context.FotoAtracoes
				.Where(f=>f.Identificador == itemGravar.Identificador).FirstOrDefault();
				if (itemBase == null)
				{
				itemBase = Context.FotoAtracoes.Create();
 			Context.Entry<FotoAtracao>(itemBase).State = System.Data.Entity.EntityState.Added;
				}
 			AtualizarPropriedades<FotoAtracao>(itemBase, itemGravar);
			Context.SaveChanges();
				itemGravar.Identificador = itemBase.Identificador;
			}
			public void ExcluirFotoAtracao (FotoAtracao itemGravar)
			{
				FotoAtracao itemExcluir =  Context.FotoAtracoes
			.Where(f=>f.Identificador == itemGravar.Identificador).FirstOrDefault();
						Context.Entry<FotoAtracao>(itemExcluir).State = EntityState.Deleted;
				Context.SaveChanges();
			}
			public FotoHotel SelecionarFotoHotel (int? Identificador)
			{
			IQueryable<FotoHotel> query =	 Context.FotoHoteis
;
					if (Identificador.HasValue)
					query = query.Where(d=>d.Identificador == Identificador);
					return query.FirstOrDefault();
			}
			public IList<FotoHotel> ListarFotoHotel ()
			{
			IQueryable<FotoHotel> query =	 Context.FotoHoteis
;
				return query.ToList();
			}
			public void SalvarFotoHotel (FotoHotel itemGravar)
			{
				FotoHotel itemBase =  Context.FotoHoteis
				.Where(f=>f.Identificador == itemGravar.Identificador).FirstOrDefault();
				if (itemBase == null)
				{
				itemBase = Context.FotoHoteis.Create();
 			Context.Entry<FotoHotel>(itemBase).State = System.Data.Entity.EntityState.Added;
				}
 			AtualizarPropriedades<FotoHotel>(itemBase, itemGravar);
			Context.SaveChanges();
				itemGravar.Identificador = itemBase.Identificador;
			}
			public void ExcluirFotoHotel (FotoHotel itemGravar)
			{
				FotoHotel itemExcluir =  Context.FotoHoteis
			.Where(f=>f.Identificador == itemGravar.Identificador).FirstOrDefault();
						Context.Entry<FotoHotel>(itemExcluir).State = EntityState.Deleted;
				Context.SaveChanges();
			}
			public FotoItemCompra SelecionarFotoItemCompra (int? Identificador)
			{
			IQueryable<FotoItemCompra> query =	 Context.FotoItemCompras
;
					if (Identificador.HasValue)
					query = query.Where(d=>d.Identificador == Identificador);
					return query.FirstOrDefault();
			}
			public IList<FotoItemCompra> ListarFotoItemCompra ()
			{
			IQueryable<FotoItemCompra> query =	 Context.FotoItemCompras
;
				return query.ToList();
			}
			public void SalvarFotoItemCompra (FotoItemCompra itemGravar)
			{
				FotoItemCompra itemBase =  Context.FotoItemCompras
				.Where(f=>f.Identificador == itemGravar.Identificador).FirstOrDefault();
				if (itemBase == null)
				{
				itemBase = Context.FotoItemCompras.Create();
 			Context.Entry<FotoItemCompra>(itemBase).State = System.Data.Entity.EntityState.Added;
				}
 			AtualizarPropriedades<FotoItemCompra>(itemBase, itemGravar);
			Context.SaveChanges();
				itemGravar.Identificador = itemBase.Identificador;
			}
			public void ExcluirFotoItemCompra (FotoItemCompra itemGravar)
			{
				FotoItemCompra itemExcluir =  Context.FotoItemCompras
			.Where(f=>f.Identificador == itemGravar.Identificador).FirstOrDefault();
						Context.Entry<FotoItemCompra>(itemExcluir).State = EntityState.Deleted;
				Context.SaveChanges();
			}
			public FotoRefeicao SelecionarFotoRefeicao (int? Identificador)
			{
			IQueryable<FotoRefeicao> query =	 Context.FotoRefeicoes
;
					if (Identificador.HasValue)
					query = query.Where(d=>d.Identificador == Identificador);
					return query.FirstOrDefault();
			}
			public IList<FotoRefeicao> ListarFotoRefeicao ()
			{
			IQueryable<FotoRefeicao> query =	 Context.FotoRefeicoes
;
				return query.ToList();
			}
			public void SalvarFotoRefeicao (FotoRefeicao itemGravar)
			{
				FotoRefeicao itemBase =  Context.FotoRefeicoes
				.Where(f=>f.Identificador == itemGravar.Identificador).FirstOrDefault();
				if (itemBase == null)
				{
				itemBase = Context.FotoRefeicoes.Create();
 			Context.Entry<FotoRefeicao>(itemBase).State = System.Data.Entity.EntityState.Added;
				}
 			AtualizarPropriedades<FotoRefeicao>(itemBase, itemGravar);
			Context.SaveChanges();
				itemGravar.Identificador = itemBase.Identificador;
			}
			public void ExcluirFotoRefeicao (FotoRefeicao itemGravar)
			{
				FotoRefeicao itemExcluir =  Context.FotoRefeicoes
			.Where(f=>f.Identificador == itemGravar.Identificador).FirstOrDefault();
						Context.Entry<FotoRefeicao>(itemExcluir).State = EntityState.Deleted;
				Context.SaveChanges();
			}
			public Gasto SelecionarGasto (int? Identificador)
			{
			IQueryable<Gasto> query =	 Context.Gastos
;
					if (Identificador.HasValue)
					query = query.Where(d=>d.Identificador == Identificador);
					return query.FirstOrDefault();
			}
			public IList<Gasto> ListarGasto ()
			{
			IQueryable<Gasto> query =	 Context.Gastos
;
				return query.ToList();
			}
			public void SalvarGasto (Gasto itemGravar)
			{
				Gasto itemBase =  Context.Gastos
				.Where(f=>f.Identificador == itemGravar.Identificador).FirstOrDefault();
				if (itemBase == null)
				{
				itemBase = Context.Gastos.Create();
 			Context.Entry<Gasto>(itemBase).State = System.Data.Entity.EntityState.Added;
				}
 			AtualizarPropriedades<Gasto>(itemBase, itemGravar);
			Context.SaveChanges();
				itemGravar.Identificador = itemBase.Identificador;
			}
			public void ExcluirGasto (Gasto itemGravar)
			{
				Gasto itemExcluir =  Context.Gastos
			.Where(f=>f.Identificador == itemGravar.Identificador).FirstOrDefault();
						Context.Entry<Gasto>(itemExcluir).State = EntityState.Deleted;
				Context.SaveChanges();
			}
			public GastoCompra SelecionarGastoCompra (int? Identificador)
			{
			IQueryable<GastoCompra> query =	 Context.GastoCompras
;
					if (Identificador.HasValue)
					query = query.Where(d=>d.Identificador == Identificador);
					return query.FirstOrDefault();
			}
			public IList<GastoCompra> ListarGastoCompra ()
			{
			IQueryable<GastoCompra> query =	 Context.GastoCompras
;
				return query.ToList();
			}
			public void SalvarGastoCompra (GastoCompra itemGravar)
			{
				GastoCompra itemBase =  Context.GastoCompras
				.Where(f=>f.Identificador == itemGravar.Identificador).FirstOrDefault();
				if (itemBase == null)
				{
				itemBase = Context.GastoCompras.Create();
 			Context.Entry<GastoCompra>(itemBase).State = System.Data.Entity.EntityState.Added;
				}
 			AtualizarPropriedades<GastoCompra>(itemBase, itemGravar);
			Context.SaveChanges();
				itemGravar.Identificador = itemBase.Identificador;
			}
			public void ExcluirGastoCompra (GastoCompra itemGravar)
			{
				GastoCompra itemExcluir =  Context.GastoCompras
			.Where(f=>f.Identificador == itemGravar.Identificador).FirstOrDefault();
						Context.Entry<GastoCompra>(itemExcluir).State = EntityState.Deleted;
				Context.SaveChanges();
			}
			public GastoHotel SelecionarGastoHotel (int? Identificador)
			{
			IQueryable<GastoHotel> query =	 Context.GastoHoteis
;
					if (Identificador.HasValue)
					query = query.Where(d=>d.Identificador == Identificador);
					return query.FirstOrDefault();
			}
			public IList<GastoHotel> ListarGastoHotel ()
			{
			IQueryable<GastoHotel> query =	 Context.GastoHoteis
;
				return query.ToList();
			}
			public void SalvarGastoHotel (GastoHotel itemGravar)
			{
				GastoHotel itemBase =  Context.GastoHoteis
				.Where(f=>f.Identificador == itemGravar.Identificador).FirstOrDefault();
				if (itemBase == null)
				{
				itemBase = Context.GastoHoteis.Create();
 			Context.Entry<GastoHotel>(itemBase).State = System.Data.Entity.EntityState.Added;
				}
 			AtualizarPropriedades<GastoHotel>(itemBase, itemGravar);
			Context.SaveChanges();
				itemGravar.Identificador = itemBase.Identificador;
			}
			public void ExcluirGastoHotel (GastoHotel itemGravar)
			{
				GastoHotel itemExcluir =  Context.GastoHoteis
			.Where(f=>f.Identificador == itemGravar.Identificador).FirstOrDefault();
						Context.Entry<GastoHotel>(itemExcluir).State = EntityState.Deleted;
				Context.SaveChanges();
			}
			public GastoPosicao SelecionarGastoPosicao (int? Identificador)
			{
			IQueryable<GastoPosicao> query =	 Context.GastoPosicoes
;
					if (Identificador.HasValue)
					query = query.Where(d=>d.Identificador == Identificador);
					return query.FirstOrDefault();
			}
			public IList<GastoPosicao> ListarGastoPosicao ()
			{
			IQueryable<GastoPosicao> query =	 Context.GastoPosicoes
;
				return query.ToList();
			}
			public void SalvarGastoPosicao (GastoPosicao itemGravar)
			{
				GastoPosicao itemBase =  Context.GastoPosicoes
				.Where(f=>f.Identificador == itemGravar.Identificador).FirstOrDefault();
				if (itemBase == null)
				{
				itemBase = Context.GastoPosicoes.Create();
 			Context.Entry<GastoPosicao>(itemBase).State = System.Data.Entity.EntityState.Added;
				}
 			AtualizarPropriedades<GastoPosicao>(itemBase, itemGravar);
			Context.SaveChanges();
				itemGravar.Identificador = itemBase.Identificador;
			}
			public void ExcluirGastoPosicao (GastoPosicao itemGravar)
			{
				GastoPosicao itemExcluir =  Context.GastoPosicoes
			.Where(f=>f.Identificador == itemGravar.Identificador).FirstOrDefault();
						Context.Entry<GastoPosicao>(itemExcluir).State = EntityState.Deleted;
				Context.SaveChanges();
			}
			public GastoRefeicao SelecionarGastoRefeicao (int? Identificador)
			{
			IQueryable<GastoRefeicao> query =	 Context.GastoRefeicoes
;
					if (Identificador.HasValue)
					query = query.Where(d=>d.Identificador == Identificador);
					return query.FirstOrDefault();
			}
			public IList<GastoRefeicao> ListarGastoRefeicao ()
			{
			IQueryable<GastoRefeicao> query =	 Context.GastoRefeicoes
;
				return query.ToList();
			}
			public void SalvarGastoRefeicao (GastoRefeicao itemGravar)
			{
				GastoRefeicao itemBase =  Context.GastoRefeicoes
				.Where(f=>f.Identificador == itemGravar.Identificador).FirstOrDefault();
				if (itemBase == null)
				{
				itemBase = Context.GastoRefeicoes.Create();
 			Context.Entry<GastoRefeicao>(itemBase).State = System.Data.Entity.EntityState.Added;
				}
 			AtualizarPropriedades<GastoRefeicao>(itemBase, itemGravar);
			Context.SaveChanges();
				itemGravar.Identificador = itemBase.Identificador;
			}
			public void ExcluirGastoRefeicao (GastoRefeicao itemGravar)
			{
				GastoRefeicao itemExcluir =  Context.GastoRefeicoes
			.Where(f=>f.Identificador == itemGravar.Identificador).FirstOrDefault();
						Context.Entry<GastoRefeicao>(itemExcluir).State = EntityState.Deleted;
				Context.SaveChanges();
			}
			public GastoViagemAerea SelecionarGastoViagemAerea (int? Identificador)
			{
			IQueryable<GastoViagemAerea> query =	 Context.GastoViagemAereas
;
					if (Identificador.HasValue)
					query = query.Where(d=>d.Identificador == Identificador);
					return query.FirstOrDefault();
			}
			public IList<GastoViagemAerea> ListarGastoViagemAerea ()
			{
			IQueryable<GastoViagemAerea> query =	 Context.GastoViagemAereas
;
				return query.ToList();
			}
			public void SalvarGastoViagemAerea (GastoViagemAerea itemGravar)
			{
				GastoViagemAerea itemBase =  Context.GastoViagemAereas
				.Where(f=>f.Identificador == itemGravar.Identificador).FirstOrDefault();
				if (itemBase == null)
				{
				itemBase = Context.GastoViagemAereas.Create();
 			Context.Entry<GastoViagemAerea>(itemBase).State = System.Data.Entity.EntityState.Added;
				}
 			AtualizarPropriedades<GastoViagemAerea>(itemBase, itemGravar);
			Context.SaveChanges();
				itemGravar.Identificador = itemBase.Identificador;
			}
			public void ExcluirGastoViagemAerea (GastoViagemAerea itemGravar)
			{
				GastoViagemAerea itemExcluir =  Context.GastoViagemAereas
			.Where(f=>f.Identificador == itemGravar.Identificador).FirstOrDefault();
						Context.Entry<GastoViagemAerea>(itemExcluir).State = EntityState.Deleted;
				Context.SaveChanges();
			}
			public Hotel SelecionarHotel (int? Identificador)
			{
			IQueryable<Hotel> query =	 Context.Hoteis
;
					if (Identificador.HasValue)
					query = query.Where(d=>d.Identificador == Identificador);
					return query.FirstOrDefault();
			}
			public IList<Hotel> ListarHotel ()
			{
			IQueryable<Hotel> query =	 Context.Hoteis
;
				return query.ToList();
			}
			public void SalvarHotel (Hotel itemGravar)
			{
				Hotel itemBase =  Context.Hoteis
				.Where(f=>f.Identificador == itemGravar.Identificador).FirstOrDefault();
				if (itemBase == null)
				{
				itemBase = Context.Hoteis.Create();
 			Context.Entry<Hotel>(itemBase).State = System.Data.Entity.EntityState.Added;
				}
 			AtualizarPropriedades<Hotel>(itemBase, itemGravar);
			Context.SaveChanges();
				itemGravar.Identificador = itemBase.Identificador;
			}
			public void ExcluirHotel (Hotel itemGravar)
			{
				Hotel itemExcluir =  Context.Hoteis
			.Where(f=>f.Identificador == itemGravar.Identificador).FirstOrDefault();
						Context.Entry<Hotel>(itemExcluir).State = EntityState.Deleted;
				Context.SaveChanges();
			}
			public HotelAvaliacao SelecionarHotelAvaliacao (int? Identificador)
			{
			IQueryable<HotelAvaliacao> query =	 Context.HotelAvaliacoes
;
					if (Identificador.HasValue)
					query = query.Where(d=>d.Identificador == Identificador);
					return query.FirstOrDefault();
			}
			public IList<HotelAvaliacao> ListarHotelAvaliacao ()
			{
			IQueryable<HotelAvaliacao> query =	 Context.HotelAvaliacoes
;
				return query.ToList();
			}
			public void SalvarHotelAvaliacao (HotelAvaliacao itemGravar)
			{
				HotelAvaliacao itemBase =  Context.HotelAvaliacoes
				.Where(f=>f.Identificador == itemGravar.Identificador).FirstOrDefault();
				if (itemBase == null)
				{
				itemBase = Context.HotelAvaliacoes.Create();
 			Context.Entry<HotelAvaliacao>(itemBase).State = System.Data.Entity.EntityState.Added;
				}
 			AtualizarPropriedades<HotelAvaliacao>(itemBase, itemGravar);
			Context.SaveChanges();
				itemGravar.Identificador = itemBase.Identificador;
			}
			public void ExcluirHotelAvaliacao (HotelAvaliacao itemGravar)
			{
				HotelAvaliacao itemExcluir =  Context.HotelAvaliacoes
			.Where(f=>f.Identificador == itemGravar.Identificador).FirstOrDefault();
						Context.Entry<HotelAvaliacao>(itemExcluir).State = EntityState.Deleted;
				Context.SaveChanges();
			}
			public ItemCompra SelecionarItemCompra (int? Identificador)
			{
			IQueryable<ItemCompra> query =	 Context.ItemCompras
;
					if (Identificador.HasValue)
					query = query.Where(d=>d.Identificador == Identificador);
					return query.FirstOrDefault();
			}
			public IList<ItemCompra> ListarItemCompra ()
			{
			IQueryable<ItemCompra> query =	 Context.ItemCompras
;
				return query.ToList();
			}
			public void SalvarItemCompra (ItemCompra itemGravar)
			{
				ItemCompra itemBase =  Context.ItemCompras
				.Where(f=>f.Identificador == itemGravar.Identificador).FirstOrDefault();
				if (itemBase == null)
				{
				itemBase = Context.ItemCompras.Create();
 			Context.Entry<ItemCompra>(itemBase).State = System.Data.Entity.EntityState.Added;
				}
 			AtualizarPropriedades<ItemCompra>(itemBase, itemGravar);
			Context.SaveChanges();
				itemGravar.Identificador = itemBase.Identificador;
			}
			public void ExcluirItemCompra (ItemCompra itemGravar)
			{
				ItemCompra itemExcluir =  Context.ItemCompras
			.Where(f=>f.Identificador == itemGravar.Identificador).FirstOrDefault();
						Context.Entry<ItemCompra>(itemExcluir).State = EntityState.Deleted;
				Context.SaveChanges();
			}
			public ListaCompra SelecionarListaCompra (int? Identificador)
			{
			IQueryable<ListaCompra> query =	 Context.ListaCompras
;
					if (Identificador.HasValue)
					query = query.Where(d=>d.Identificador == Identificador);
					return query.FirstOrDefault();
			}
			public IList<ListaCompra> ListarListaCompra ()
			{
			IQueryable<ListaCompra> query =	 Context.ListaCompras
;
				return query.ToList();
			}
			public void SalvarListaCompra (ListaCompra itemGravar)
			{
				ListaCompra itemBase =  Context.ListaCompras
				.Where(f=>f.Identificador == itemGravar.Identificador).FirstOrDefault();
				if (itemBase == null)
				{
				itemBase = Context.ListaCompras.Create();
 			Context.Entry<ListaCompra>(itemBase).State = System.Data.Entity.EntityState.Added;
				}
 			AtualizarPropriedades<ListaCompra>(itemBase, itemGravar);
			Context.SaveChanges();
				itemGravar.Identificador = itemBase.Identificador;
			}
			public void ExcluirListaCompra (ListaCompra itemGravar)
			{
				ListaCompra itemExcluir =  Context.ListaCompras
			.Where(f=>f.Identificador == itemGravar.Identificador).FirstOrDefault();
						Context.Entry<ListaCompra>(itemExcluir).State = EntityState.Deleted;
				Context.SaveChanges();
			}
			public Loja SelecionarLoja (int? Identificador)
			{
			IQueryable<Loja> query =	 Context.Lojas
;
					if (Identificador.HasValue)
					query = query.Where(d=>d.Identificador == Identificador);
					return query.FirstOrDefault();
			}
			public IList<Loja> ListarLoja ()
			{
			IQueryable<Loja> query =	 Context.Lojas
;
				return query.ToList();
			}
			public void SalvarLoja (Loja itemGravar)
			{
				Loja itemBase =  Context.Lojas
				.Where(f=>f.Identificador == itemGravar.Identificador).FirstOrDefault();
				if (itemBase == null)
				{
				itemBase = Context.Lojas.Create();
 			Context.Entry<Loja>(itemBase).State = System.Data.Entity.EntityState.Added;
				}
 			AtualizarPropriedades<Loja>(itemBase, itemGravar);
			Context.SaveChanges();
				itemGravar.Identificador = itemBase.Identificador;
			}
			public void ExcluirLoja (Loja itemGravar)
			{
				Loja itemExcluir =  Context.Lojas
			.Where(f=>f.Identificador == itemGravar.Identificador).FirstOrDefault();
						Context.Entry<Loja>(itemExcluir).State = EntityState.Deleted;
				Context.SaveChanges();
			}
			public Pais SelecionarPais (int? Identificador)
			{
			IQueryable<Pais> query =	 Context.Paises
;
					if (Identificador.HasValue)
					query = query.Where(d=>d.Identificador == Identificador);
					return query.FirstOrDefault();
			}
			public IList<Pais> ListarPais ()
			{
			IQueryable<Pais> query =	 Context.Paises
;
				return query.ToList();
			}
			public void SalvarPais (Pais itemGravar)
			{
				Pais itemBase =  Context.Paises
				.Where(f=>f.Identificador == itemGravar.Identificador).FirstOrDefault();
				if (itemBase == null)
				{
				itemBase = Context.Paises.Create();
 			Context.Entry<Pais>(itemBase).State = System.Data.Entity.EntityState.Added;
				}
 			AtualizarPropriedades<Pais>(itemBase, itemGravar);
			Context.SaveChanges();
				itemGravar.Identificador = itemBase.Identificador;
			}
			public void ExcluirPais (Pais itemGravar)
			{
				Pais itemExcluir =  Context.Paises
			.Where(f=>f.Identificador == itemGravar.Identificador).FirstOrDefault();
						Context.Entry<Pais>(itemExcluir).State = EntityState.Deleted;
				Context.SaveChanges();
			}
			public ParticipanteViagem SelecionarParticipanteViagem (int? Identificador)
			{
			IQueryable<ParticipanteViagem> query =	 Context.ParticipanteViagemes
;
					if (Identificador.HasValue)
					query = query.Where(d=>d.Identificador == Identificador);
					return query.FirstOrDefault();
			}
			public IList<ParticipanteViagem> ListarParticipanteViagem ()
			{
			IQueryable<ParticipanteViagem> query =	 Context.ParticipanteViagemes
;
				return query.ToList();
			}
			public void SalvarParticipanteViagem (ParticipanteViagem itemGravar)
			{
				ParticipanteViagem itemBase =  Context.ParticipanteViagemes
				.Where(f=>f.Identificador == itemGravar.Identificador).FirstOrDefault();
				if (itemBase == null)
				{
				itemBase = Context.ParticipanteViagemes.Create();
 			Context.Entry<ParticipanteViagem>(itemBase).State = System.Data.Entity.EntityState.Added;
				}
 			AtualizarPropriedades<ParticipanteViagem>(itemBase, itemGravar);
			Context.SaveChanges();
				itemGravar.Identificador = itemBase.Identificador;
			}
			public void ExcluirParticipanteViagem (ParticipanteViagem itemGravar)
			{
				ParticipanteViagem itemExcluir =  Context.ParticipanteViagemes
			.Where(f=>f.Identificador == itemGravar.Identificador).FirstOrDefault();
						Context.Entry<ParticipanteViagem>(itemExcluir).State = EntityState.Deleted;
				Context.SaveChanges();
			}
			public Posicao SelecionarPosicao (int? Identificador)
			{
			IQueryable<Posicao> query =	 Context.Posicoes
;
					if (Identificador.HasValue)
					query = query.Where(d=>d.Identificador == Identificador);
					return query.FirstOrDefault();
			}
			public IList<Posicao> ListarPosicao ()
			{
			IQueryable<Posicao> query =	 Context.Posicoes
;
				return query.ToList();
			}
			public void SalvarPosicao (Posicao itemGravar)
			{
				Posicao itemBase =  Context.Posicoes
				.Where(f=>f.Identificador == itemGravar.Identificador).FirstOrDefault();
				if (itemBase == null)
				{
				itemBase = Context.Posicoes.Create();
 			Context.Entry<Posicao>(itemBase).State = System.Data.Entity.EntityState.Added;
				}
 			AtualizarPropriedades<Posicao>(itemBase, itemGravar);
			Context.SaveChanges();
				itemGravar.Identificador = itemBase.Identificador;
			}
			public void ExcluirPosicao (Posicao itemGravar)
			{
				Posicao itemExcluir =  Context.Posicoes
			.Where(f=>f.Identificador == itemGravar.Identificador).FirstOrDefault();
						Context.Entry<Posicao>(itemExcluir).State = EntityState.Deleted;
				Context.SaveChanges();
			}
			public ReabastecimentoGasto SelecionarReabastecimentoGasto (int? Identificador)
			{
			IQueryable<ReabastecimentoGasto> query =	 Context.ReabastecimentoGastos
;
					if (Identificador.HasValue)
					query = query.Where(d=>d.Identificador == Identificador);
					return query.FirstOrDefault();
			}
			public IList<ReabastecimentoGasto> ListarReabastecimentoGasto ()
			{
			IQueryable<ReabastecimentoGasto> query =	 Context.ReabastecimentoGastos
;
				return query.ToList();
			}
			public void SalvarReabastecimentoGasto (ReabastecimentoGasto itemGravar)
			{
				ReabastecimentoGasto itemBase =  Context.ReabastecimentoGastos
				.Where(f=>f.Identificador == itemGravar.Identificador).FirstOrDefault();
				if (itemBase == null)
				{
				itemBase = Context.ReabastecimentoGastos.Create();
 			Context.Entry<ReabastecimentoGasto>(itemBase).State = System.Data.Entity.EntityState.Added;
				}
 			AtualizarPropriedades<ReabastecimentoGasto>(itemBase, itemGravar);
			Context.SaveChanges();
				itemGravar.Identificador = itemBase.Identificador;
			}
			public void ExcluirReabastecimentoGasto (ReabastecimentoGasto itemGravar)
			{
				ReabastecimentoGasto itemExcluir =  Context.ReabastecimentoGastos
			.Where(f=>f.Identificador == itemGravar.Identificador).FirstOrDefault();
						Context.Entry<ReabastecimentoGasto>(itemExcluir).State = EntityState.Deleted;
				Context.SaveChanges();
			}
			public Refeicao SelecionarRefeicao (int? Identificador)
			{
			IQueryable<Refeicao> query =	 Context.Refeicoes
;
					if (Identificador.HasValue)
					query = query.Where(d=>d.Identificador == Identificador);
					return query.FirstOrDefault();
			}
			public IList<Refeicao> ListarRefeicao ()
			{
			IQueryable<Refeicao> query =	 Context.Refeicoes
;
				return query.ToList();
			}
			public void SalvarRefeicao (Refeicao itemGravar)
			{
				Refeicao itemBase =  Context.Refeicoes
				.Where(f=>f.Identificador == itemGravar.Identificador).FirstOrDefault();
				if (itemBase == null)
				{
				itemBase = Context.Refeicoes.Create();
 			Context.Entry<Refeicao>(itemBase).State = System.Data.Entity.EntityState.Added;
				}
 			AtualizarPropriedades<Refeicao>(itemBase, itemGravar);
			Context.SaveChanges();
				itemGravar.Identificador = itemBase.Identificador;
			}
			public void ExcluirRefeicao (Refeicao itemGravar)
			{
				Refeicao itemExcluir =  Context.Refeicoes
			.Where(f=>f.Identificador == itemGravar.Identificador).FirstOrDefault();
						Context.Entry<Refeicao>(itemExcluir).State = EntityState.Deleted;
				Context.SaveChanges();
			}
			public RequisicaoAmizade SelecionarRequisicaoAmizade (int? Identificador)
			{
			IQueryable<RequisicaoAmizade> query =	 Context.RequisicaoAmizades
;
					if (Identificador.HasValue)
					query = query.Where(d=>d.Identificador == Identificador);
					return query.FirstOrDefault();
			}
			public IList<RequisicaoAmizade> ListarRequisicaoAmizade ()
			{
			IQueryable<RequisicaoAmizade> query =	 Context.RequisicaoAmizades
;
				return query.ToList();
			}
			public void SalvarRequisicaoAmizade (RequisicaoAmizade itemGravar)
			{
				RequisicaoAmizade itemBase =  Context.RequisicaoAmizades
				.Where(f=>f.Identificador == itemGravar.Identificador).FirstOrDefault();
				if (itemBase == null)
				{
				itemBase = Context.RequisicaoAmizades.Create();
 			Context.Entry<RequisicaoAmizade>(itemBase).State = System.Data.Entity.EntityState.Added;
				}
 			AtualizarPropriedades<RequisicaoAmizade>(itemBase, itemGravar);
			Context.SaveChanges();
				itemGravar.Identificador = itemBase.Identificador;
			}
			public void ExcluirRequisicaoAmizade (RequisicaoAmizade itemGravar)
			{
				RequisicaoAmizade itemExcluir =  Context.RequisicaoAmizades
			.Where(f=>f.Identificador == itemGravar.Identificador).FirstOrDefault();
						Context.Entry<RequisicaoAmizade>(itemExcluir).State = EntityState.Deleted;
				Context.SaveChanges();
			}
			public Sugestao SelecionarSugestao (int? Identificador)
			{
			IQueryable<Sugestao> query =	 Context.Sugestoes
;
					if (Identificador.HasValue)
					query = query.Where(d=>d.Identificador == Identificador);
					return query.FirstOrDefault();
			}
			public IList<Sugestao> ListarSugestao ()
			{
			IQueryable<Sugestao> query =	 Context.Sugestoes
;
				return query.ToList();
			}
			public void SalvarSugestao (Sugestao itemGravar)
			{
				Sugestao itemBase =  Context.Sugestoes
				.Where(f=>f.Identificador == itemGravar.Identificador).FirstOrDefault();
				if (itemBase == null)
				{
				itemBase = Context.Sugestoes.Create();
 			Context.Entry<Sugestao>(itemBase).State = System.Data.Entity.EntityState.Added;
				}
 			AtualizarPropriedades<Sugestao>(itemBase, itemGravar);
			Context.SaveChanges();
				itemGravar.Identificador = itemBase.Identificador;
			}
			public void ExcluirSugestao (Sugestao itemGravar)
			{
				Sugestao itemExcluir =  Context.Sugestoes
			.Where(f=>f.Identificador == itemGravar.Identificador).FirstOrDefault();
						Context.Entry<Sugestao>(itemExcluir).State = EntityState.Deleted;
				Context.SaveChanges();
			}
			public Usuario SelecionarUsuario (int? Identificador)
			{
			IQueryable<Usuario> query =	 Context.Usuarios
;
					if (Identificador.HasValue)
					query = query.Where(d=>d.Identificador == Identificador);
					return query.FirstOrDefault();
			}
			public IList<Usuario> ListarUsuario ()
			{
			IQueryable<Usuario> query =	 Context.Usuarios
;
				return query.ToList();
			}
			public void SalvarUsuario (Usuario itemGravar)
			{
				Usuario itemBase =  Context.Usuarios
				.Where(f=>f.Identificador == itemGravar.Identificador).FirstOrDefault();
				if (itemBase == null)
				{
				itemBase = Context.Usuarios.Create();
 			Context.Entry<Usuario>(itemBase).State = System.Data.Entity.EntityState.Added;
				}
 			AtualizarPropriedades<Usuario>(itemBase, itemGravar);
			Context.SaveChanges();
				itemGravar.Identificador = itemBase.Identificador;
			}
			public void ExcluirUsuario (Usuario itemGravar)
			{
				Usuario itemExcluir =  Context.Usuarios
			.Where(f=>f.Identificador == itemGravar.Identificador).FirstOrDefault();
						Context.Entry<Usuario>(itemExcluir).State = EntityState.Deleted;
				Context.SaveChanges();
			}
			public Viagem SelecionarViagem (int? Identificador)
			{
			IQueryable<Viagem> query =	 Context.Viagemes
;
					if (Identificador.HasValue)
					query = query.Where(d=>d.Identificador == Identificador);
					return query.FirstOrDefault();
			}
			public IList<Viagem> ListarViagem ()
			{
			IQueryable<Viagem> query =	 Context.Viagemes
;
				return query.ToList();
			}
			public void SalvarViagem (Viagem itemGravar)
			{
				Viagem itemBase =  Context.Viagemes
				.Where(f=>f.Identificador == itemGravar.Identificador).FirstOrDefault();
				if (itemBase == null)
				{
				itemBase = Context.Viagemes.Create();
 			Context.Entry<Viagem>(itemBase).State = System.Data.Entity.EntityState.Added;
				}
 			AtualizarPropriedades<Viagem>(itemBase, itemGravar);
			Context.SaveChanges();
				itemGravar.Identificador = itemBase.Identificador;
			}
			public void ExcluirViagem (Viagem itemGravar)
			{
				Viagem itemExcluir =  Context.Viagemes
			.Where(f=>f.Identificador == itemGravar.Identificador).FirstOrDefault();
						Context.Entry<Viagem>(itemExcluir).State = EntityState.Deleted;
				Context.SaveChanges();
			}
			public ViagemAerea SelecionarViagemAerea (int? Identificador)
			{
			IQueryable<ViagemAerea> query =	 Context.ViagemAereas
;
					if (Identificador.HasValue)
					query = query.Where(d=>d.Identificador == Identificador);
					return query.FirstOrDefault();
			}
			public IList<ViagemAerea> ListarViagemAerea ()
			{
			IQueryable<ViagemAerea> query =	 Context.ViagemAereas
;
				return query.ToList();
			}
			public void SalvarViagemAerea (ViagemAerea itemGravar)
			{
				ViagemAerea itemBase =  Context.ViagemAereas
				.Where(f=>f.Identificador == itemGravar.Identificador).FirstOrDefault();
				if (itemBase == null)
				{
				itemBase = Context.ViagemAereas.Create();
 			Context.Entry<ViagemAerea>(itemBase).State = System.Data.Entity.EntityState.Added;
				}
 			AtualizarPropriedades<ViagemAerea>(itemBase, itemGravar);
			Context.SaveChanges();
				itemGravar.Identificador = itemBase.Identificador;
			}
			public void ExcluirViagemAerea (ViagemAerea itemGravar)
			{
				ViagemAerea itemExcluir =  Context.ViagemAereas
			.Where(f=>f.Identificador == itemGravar.Identificador).FirstOrDefault();
						Context.Entry<ViagemAerea>(itemExcluir).State = EntityState.Deleted;
				Context.SaveChanges();
			}
			public ViagemAereaAeroporto SelecionarViagemAereaAeroporto (int? Identificador)
			{
			IQueryable<ViagemAereaAeroporto> query =	 Context.ViagemAereaAeroportos
;
					if (Identificador.HasValue)
					query = query.Where(d=>d.Identificador == Identificador);
					return query.FirstOrDefault();
			}
			public IList<ViagemAereaAeroporto> ListarViagemAereaAeroporto ()
			{
			IQueryable<ViagemAereaAeroporto> query =	 Context.ViagemAereaAeroportos
;
				return query.ToList();
			}
			public void SalvarViagemAereaAeroporto (ViagemAereaAeroporto itemGravar)
			{
				ViagemAereaAeroporto itemBase =  Context.ViagemAereaAeroportos
				.Where(f=>f.Identificador == itemGravar.Identificador).FirstOrDefault();
				if (itemBase == null)
				{
				itemBase = Context.ViagemAereaAeroportos.Create();
 			Context.Entry<ViagemAereaAeroporto>(itemBase).State = System.Data.Entity.EntityState.Added;
				}
 			AtualizarPropriedades<ViagemAereaAeroporto>(itemBase, itemGravar);
			Context.SaveChanges();
				itemGravar.Identificador = itemBase.Identificador;
			}
			public void ExcluirViagemAereaAeroporto (ViagemAereaAeroporto itemGravar)
			{
				ViagemAereaAeroporto itemExcluir =  Context.ViagemAereaAeroportos
			.Where(f=>f.Identificador == itemGravar.Identificador).FirstOrDefault();
						Context.Entry<ViagemAereaAeroporto>(itemExcluir).State = EntityState.Deleted;
				Context.SaveChanges();
			}
			public RefeicaoPedido SelecionarRefeicaoPedido (int? Identificador)
			{
			IQueryable<RefeicaoPedido> query =	 Context.RefeicaoPedidos
;
					if (Identificador.HasValue)
					query = query.Where(d=>d.Identificador == Identificador);
					return query.FirstOrDefault();
			}
			public IList<RefeicaoPedido> ListarRefeicaoPedido ()
			{
			IQueryable<RefeicaoPedido> query =	 Context.RefeicaoPedidos
;
				return query.ToList();
			}
			public void SalvarRefeicaoPedido (RefeicaoPedido itemGravar)
			{
				RefeicaoPedido itemBase =  Context.RefeicaoPedidos
				.Where(f=>f.Identificador == itemGravar.Identificador).FirstOrDefault();
				if (itemBase == null)
				{
				itemBase = Context.RefeicaoPedidos.Create();
 			Context.Entry<RefeicaoPedido>(itemBase).State = System.Data.Entity.EntityState.Added;
				}
 			AtualizarPropriedades<RefeicaoPedido>(itemBase, itemGravar);
			Context.SaveChanges();
				itemGravar.Identificador = itemBase.Identificador;
			}
			public void ExcluirRefeicaoPedido (RefeicaoPedido itemGravar)
			{
				RefeicaoPedido itemExcluir =  Context.RefeicaoPedidos
			.Where(f=>f.Identificador == itemGravar.Identificador).FirstOrDefault();
						Context.Entry<RefeicaoPedido>(itemExcluir).State = EntityState.Deleted;
				Context.SaveChanges();
			}
			public AvaliacaoLoja SelecionarAvaliacaoLoja (int? Identificador)
			{
			IQueryable<AvaliacaoLoja> query =	 Context.AvaliacaoLojas
;
					if (Identificador.HasValue)
					query = query.Where(d=>d.Identificador == Identificador);
					return query.FirstOrDefault();
			}
			public IList<AvaliacaoLoja> ListarAvaliacaoLoja ()
			{
			IQueryable<AvaliacaoLoja> query =	 Context.AvaliacaoLojas
;
				return query.ToList();
			}
			public void SalvarAvaliacaoLoja (AvaliacaoLoja itemGravar)
			{
				AvaliacaoLoja itemBase =  Context.AvaliacaoLojas
				.Where(f=>f.Identificador == itemGravar.Identificador).FirstOrDefault();
				if (itemBase == null)
				{
				itemBase = Context.AvaliacaoLojas.Create();
 			Context.Entry<AvaliacaoLoja>(itemBase).State = System.Data.Entity.EntityState.Added;
				}
 			AtualizarPropriedades<AvaliacaoLoja>(itemBase, itemGravar);
			Context.SaveChanges();
				itemGravar.Identificador = itemBase.Identificador;
			}
			public void ExcluirAvaliacaoLoja (AvaliacaoLoja itemGravar)
			{
				AvaliacaoLoja itemExcluir =  Context.AvaliacaoLojas
			.Where(f=>f.Identificador == itemGravar.Identificador).FirstOrDefault();
						Context.Entry<AvaliacaoLoja>(itemExcluir).State = EntityState.Deleted;
				Context.SaveChanges();
			}
			public UsuarioGasto SelecionarUsuarioGasto (string Identificador)
			{
			IQueryable<UsuarioGasto> query =	 Context.UsuarioGastos
;
					if (!string.IsNullOrEmpty(Identificador))
					query = query.Where(d=>d.Identificador == Identificador);
					return query.FirstOrDefault();
			}
			public IList<UsuarioGasto> ListarUsuarioGasto ()
			{
			IQueryable<UsuarioGasto> query =	 Context.UsuarioGastos
;
				return query.ToList();
			}
			public void SalvarUsuarioGasto (UsuarioGasto itemGravar)
			{
				UsuarioGasto itemBase =  Context.UsuarioGastos
				.Where(f=>f.Identificador == itemGravar.Identificador).FirstOrDefault();
				if (itemBase == null)
				{
				itemBase = Context.UsuarioGastos.Create();
 			Context.Entry<UsuarioGasto>(itemBase).State = System.Data.Entity.EntityState.Added;
				}
 			AtualizarPropriedades<UsuarioGasto>(itemBase, itemGravar);
			Context.SaveChanges();
				itemGravar.Identificador = itemBase.Identificador;
			}
			public void ExcluirUsuarioGasto (UsuarioGasto itemGravar)
			{
				UsuarioGasto itemExcluir =  Context.UsuarioGastos
			.Where(f=>f.Identificador == itemGravar.Identificador).FirstOrDefault();
						Context.Entry<UsuarioGasto>(itemExcluir).State = EntityState.Deleted;
				Context.SaveChanges();
			}
			public HotelEvento SelecionarHotelEvento (int? Identificador)
			{
			IQueryable<HotelEvento> query =	 Context.HotelEventos
;
					if (Identificador.HasValue)
					query = query.Where(d=>d.Identificador == Identificador);
					return query.FirstOrDefault();
			}
			public IList<HotelEvento> ListarHotelEvento ()
			{
			IQueryable<HotelEvento> query =	 Context.HotelEventos
;
				return query.ToList();
			}
			public void SalvarHotelEvento (HotelEvento itemGravar)
			{
				HotelEvento itemBase =  Context.HotelEventos
				.Where(f=>f.Identificador == itemGravar.Identificador).FirstOrDefault();
				if (itemBase == null)
				{
				itemBase = Context.HotelEventos.Create();
 			Context.Entry<HotelEvento>(itemBase).State = System.Data.Entity.EntityState.Added;
				}
 			AtualizarPropriedades<HotelEvento>(itemBase, itemGravar);
			Context.SaveChanges();
				itemGravar.Identificador = itemBase.Identificador;
			}
			public void ExcluirHotelEvento (HotelEvento itemGravar)
			{
				HotelEvento itemExcluir =  Context.HotelEventos
			.Where(f=>f.Identificador == itemGravar.Identificador).FirstOrDefault();
						Context.Entry<HotelEvento>(itemExcluir).State = EntityState.Deleted;
				Context.SaveChanges();
			}
	}

}
