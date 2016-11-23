using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using CV.Model;

namespace CV.Data.Configuration
{
	public partial class AvaliacaoAluguelConfiguration:  EntityTypeConfiguration<AvaliacaoAluguel>
	{
		public AvaliacaoAluguelConfiguration()
		{
			string Schema = System.Configuration.ConfigurationManager.AppSettings["Schema"];
			if (string.IsNullOrEmpty(Schema))

			this.ToTable("AvaliacaoAluguel");
		else
			this.ToTable("AvaliacaoAluguel",Schema);
		this.HasKey(i => new {i.Identificador });
			this.Property(i => i.Identificador).HasColumnName("ID_AVALIACAO_ALUGUEL");
			this.Property(i => i.IdentificadorUsuario).HasColumnName("ID_USUARIO");
			this.Property(i => i.IdentificadorCarro).HasColumnName("ID_CARRO");
			this.Property(i => i.Nota).HasColumnName("NR_NOTA");
			this.Property(i => i.Comentario).HasColumnName("DS_COMENTARIO");
			this.HasRequired(i => i.ItemCarro).WithMany().HasForeignKey(d=>d.IdentificadorCarro);
			this.HasRequired(i => i.ItemUsuario).WithMany().HasForeignKey(d=>d.IdentificadorUsuario);
			this.Property(i => i.DataAtualizacao).HasColumnName("DT_ATUALIZACAO");
			this.Property(i => i.DataExclusao).HasColumnName("DT_EXCLUSAO");
		MapearCamposManualmente();
		}
	}
}

