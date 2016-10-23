using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using CV.Model;

namespace CV.Data.Configuration
{
	public partial class AvaliacaoAtracaoConfiguration:  EntityTypeConfiguration<AvaliacaoAtracao>
	{
		public AvaliacaoAtracaoConfiguration()
		{
			string Schema = System.Configuration.ConfigurationManager.AppSettings["Schema"];
			if (string.IsNullOrEmpty(Schema))

			this.ToTable("AvaliacaoAtracao");
		else
			this.ToTable("AvaliacaoAtracao",Schema);
		this.HasKey(i => new {i.Identificador });
			this.Property(i => i.Identificador).HasColumnName("ID_AVALIACAO_ATRACAO");
			this.Property(i => i.IdentificadorUsuario).HasColumnName("ID_USUARIO");
			this.Property(i => i.Nota).HasColumnName("NR_NOTA");
			this.Property(i => i.Comentario).HasColumnName("DS_COMENTARIO");
			this.Property(i => i.IdentificadorAtracao).HasColumnName("ID_ATRACAO");
			this.HasRequired(i => i.ItemAtracao).WithMany().HasForeignKey(d=>d.IdentificadorAtracao);
			this.HasRequired(i => i.ItemUsuario).WithMany().HasForeignKey(d=>d.IdentificadorUsuario);
		MapearCamposManualmente();
		}
	}
}

