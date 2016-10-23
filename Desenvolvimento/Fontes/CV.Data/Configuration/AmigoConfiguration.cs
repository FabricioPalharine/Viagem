using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using CV.Model;

namespace CV.Data.Configuration
{
	public partial class AmigoConfiguration:  EntityTypeConfiguration<Amigo>
	{
		public AmigoConfiguration()
		{
			string Schema = System.Configuration.ConfigurationManager.AppSettings["Schema"];
			if (string.IsNullOrEmpty(Schema))

			this.ToTable("Amigo");
		else
			this.ToTable("Amigo",Schema);
		this.HasKey(i => new {i.Identificador });
			this.Property(i => i.Identificador).HasColumnName("ID_AMIGO");
			this.Property(i => i.IdentificadorUsuario).HasColumnName("ID_USUARIO");
			this.Property(i => i.IdentificadorAmigo).HasColumnName("ID_USUARIO_AMIGO");
			this.Property(i => i.EMail).HasColumnName("DS_EMAIL");
			this.HasRequired(i => i.ItemUsuario).WithMany().HasForeignKey(d=>d.IdentificadorUsuario);
			this.HasOptional(i => i.ItemAmigo).WithMany().HasForeignKey(d=>d.IdentificadorAmigo);
		MapearCamposManualmente();
		}
	}
}

