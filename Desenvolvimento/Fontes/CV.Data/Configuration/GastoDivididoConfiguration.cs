using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using CV.Model;

namespace CV.Data.Configuration
{
	public partial class GastoDivididoConfiguration:  EntityTypeConfiguration<GastoDividido>
	{
		public GastoDivididoConfiguration()
		{
			string Schema = System.Configuration.ConfigurationManager.AppSettings["Schema"];
			if (string.IsNullOrEmpty(Schema))

			this.ToTable("GastoDividido");
		else
			this.ToTable("GastoDividido",Schema);
		this.HasKey(i => new {i.Identificador });
			this.Property(i => i.Identificador).HasColumnName("ID_GASTO_DIVIDIDO");
			this.Property(i => i.IdentificadorUsuario).HasColumnName("ID_USUARIO");
			this.Property(i => i.IdentificadorGasto).HasColumnName("ID_GASTO");
			this.HasOptional(i => i.ItemUsuario).WithMany().HasForeignKey(d=>d.IdentificadorUsuario);
		MapearCamposManualmente();
		}
	}
}

