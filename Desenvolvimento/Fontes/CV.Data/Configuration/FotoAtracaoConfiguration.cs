using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using CV.Model;

namespace CV.Data.Configuration
{
	public partial class FotoAtracaoConfiguration:  EntityTypeConfiguration<FotoAtracao>
	{
		public FotoAtracaoConfiguration()
		{
			string Schema = System.Configuration.ConfigurationManager.AppSettings["Schema"];
			if (string.IsNullOrEmpty(Schema))

			this.ToTable("FotoAtracao");
		else
			this.ToTable("FotoAtracao",Schema);
		this.HasKey(i => new {i.Identificador });
			this.Property(i => i.Identificador).HasColumnName("ID_FOTO_ATRACAO");
			this.Property(i => i.IdentificadorAtracao).HasColumnName("ID_ATRACAO");
			this.Property(i => i.IdentificadorFoto).HasColumnName("ID_FOTO");
			this.HasRequired(i => i.ItemAtracao).WithMany().HasForeignKey(d=>d.IdentificadorAtracao);
			this.HasRequired(i => i.ItemFoto).WithMany().HasForeignKey(d=>d.IdentificadorFoto);
			this.Property(i => i.DataAtualizacao).HasColumnName("DT_ATUALIZACAO");
			this.Property(i => i.DataExclusao).HasColumnName("DT_EXCLUSAO");
		MapearCamposManualmente();
		}
	}
}

