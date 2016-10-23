using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using CV.Model;

namespace CV.Data.Configuration
{
	public partial class UsuarioConfiguration:  EntityTypeConfiguration<Usuario>
	{
		public UsuarioConfiguration()
		{
			string Schema = System.Configuration.ConfigurationManager.AppSettings["Schema"];
			if (string.IsNullOrEmpty(Schema))

			this.ToTable("Usuario");
		else
			this.ToTable("Usuario",Schema);
		this.HasKey(i => new {i.Identificador });
			this.Property(i => i.Identificador).HasColumnName("ID_USUARIO");
			this.Property(i => i.EMail).HasColumnName("DS_EMAIL");
			this.Property(i => i.Nome).HasColumnName("NM_USUARIO");
			this.Property(i => i.Token).HasColumnName("CD_TOKEN");
			this.Property(i => i.RefreshToken).HasColumnName("CD_REFRESH_TOKEN");
			this.Property(i => i.DataToken).HasColumnName("DT_TOKEN");
			this.Property(i => i.Lifetime).HasColumnName("NR_TOKEN_LIFETIME");
			this.Property(i => i.Codigo).HasColumnName("CD_USUARIO");
			this.HasMany(i => i.Gastos).WithRequired().HasForeignKey(d=>d.IdentificadorUsuario);
		MapearCamposManualmente();
		}
	}
}

