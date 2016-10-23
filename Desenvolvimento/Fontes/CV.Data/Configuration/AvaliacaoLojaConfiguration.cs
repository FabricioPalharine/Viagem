using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using CV.Model;

namespace CV.Data.Configuration
{
	public partial class AvaliacaoLojaConfiguration:  EntityTypeConfiguration<AvaliacaoLoja>
	{
		public AvaliacaoLojaConfiguration()
		{
			string Schema = System.Configuration.ConfigurationManager.AppSettings["Schema"];
			if (string.IsNullOrEmpty(Schema))

			this.ToTable("AvaliacaoLoja");
		else
			this.ToTable("AvaliacaoLoja",Schema);
		this.HasKey(i => new {i.Identificador });
			this.Property(i => i.Identificador).HasColumnName("ID_AVALIACAO_LOJA");
			this.Property(i => i.IdentificadorLoja).HasColumnName("ID_LOJA");
			this.Property(i => i.IdentificadorUsuario).HasColumnName("ID_USUARIO");
			this.Property(i => i.Nota).HasColumnName("NR_NOTA");
			this.Property(i => i.Comentario).HasColumnName("DS_COMENTARIO");
			this.HasRequired(i => i.ItemLoja).WithMany().HasForeignKey(d=>d.IdentificadorLoja);
			this.HasRequired(i => i.ItemUsuario).WithMany().HasForeignKey(d=>d.IdentificadorUsuario);
		MapearCamposManualmente();
		}
	}
}

