using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using CV.Model;

namespace CV.Data.Configuration
{
	public partial class AvaliacaoAereaConfiguration:  EntityTypeConfiguration<AvaliacaoAerea>
	{
		public AvaliacaoAereaConfiguration()
		{
			string Schema = System.Configuration.ConfigurationManager.AppSettings["Schema"];
			if (string.IsNullOrEmpty(Schema))

			this.ToTable("AvaliacaoAerea");
		else
			this.ToTable("AvaliacaoAerea",Schema);
		this.HasKey(i => new {i.Identificador });
			this.Property(i => i.Identificador).HasColumnName("ID_AVALIACAO_AEREA");
			this.Property(i => i.IdentificadorUsuario).HasColumnName("ID_USUARIO");
			this.Property(i => i.IdentificadorViagemAerea).HasColumnName("ID_VIAGEM_AEREA");
			this.Property(i => i.Nota).HasColumnName("NR_NOTA");
			this.Property(i => i.Comentario).HasColumnName("DS_COMENTARIO");
			this.HasRequired(i => i.ItemUsuario).WithMany().HasForeignKey(d=>d.IdentificadorUsuario);
			this.HasRequired(i => i.ItemViagemAerea).WithMany().HasForeignKey(d=>d.IdentificadorViagemAerea);
		MapearCamposManualmente();
		}
	}
}

