using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Reflection;


namespace  CV.Data
{
	public abstract partial class RepositoryBase: IDisposable
	{
		public RepositoryBase()
		{
			this.Context = new EntityContext("Sistema");

			this.Context.Database.Connection.ConnectionString = RetornaConexao();
		}

		~RepositoryBase()
		{
			Dispose(false);
		}

		protected EntityContext Context { get; private set; }

		private bool Disposed { get; set; }

		public void CommitChanges()
		{
			if (this.Context != null)
			{
				Context.SaveChanges();
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!Disposed)
			{
				if (disposing)
				{
					this.Context.Dispose();
				}

				this.Disposed = true;
			}
		}



		protected void AtualizarPropriedades<T>(T ClasseEF, T ClassePoco) where T:class
		{
			ObjectContext objectContext = ((IObjectContextAdapter)Context).ObjectContext;
			ObjectSet<T> set = objectContext.CreateObjectSet<T>();
			IEnumerable<string> keyNames = set.EntitySet.ElementType
																									.KeyMembers
																									.Select(k => k.Name);
			Type Tipo = typeof(T);
			PropertyInfo[] propriedades =  Tipo.GetProperties();
			foreach (PropertyInfo propriedade in propriedades)
			{
				if (propriedade.CanWrite && !keyNames.Where(d => d == propriedade.Name).Any())
				{
					Type p = propriedade.PropertyType;
					if (((!p.Namespace.StartsWith("CV.Model") && p.Name != "IList`1") || p.IsEnum))
					{					
						propriedade.SetValue(ClasseEF, propriedade.GetValue(ClassePoco, null), null);
					}					
					
				}
			}
		}


		protected object VerificarValorNulo<T>(T valor)
		{
			if (valor == null)
				return DBNull.Value;
			else
				return valor;
		}

		protected T AjustarValorObjeto<T>(object ValorBanco)
		{
			if (ValorBanco == null || ValorBanco == DBNull.Value)
				return default(T);
			else if (typeof(T) == typeof(string))
				return (T)Convert.ChangeType(Convert.ToString(ValorBanco), typeof(T));
			else if (typeof(T) == typeof(Int32))
				return (T)Convert.ChangeType(Convert.ToInt32(ValorBanco), typeof(T));
			else if (typeof(T) == typeof(Double))
				return (T)Convert.ChangeType(Convert.ToDouble(ValorBanco), typeof(T));
			else
				return (T)ValorBanco;

		}

		protected decimal? AjustarValorDecimal(object ValorBanco)
		{
			if (ValorBanco == null || ValorBanco == DBNull.Value)
				return null;
			else
				return Convert.ToDecimal(ValorBanco);
		}
		
	}

}