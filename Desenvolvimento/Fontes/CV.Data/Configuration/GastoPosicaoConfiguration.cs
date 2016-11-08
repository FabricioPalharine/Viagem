using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using CV.Model;

namespace CV.Data.Configuration
{
	public partial class GastoPosicaoConfiguration:  EntityTypeConfiguration<GastoPosicao>
	{
		public GastoPosicaoConfiguration()
		{
			string Schema = System.Configuration.ConfigurationManager.AppSettings["Schema"];
			if (string.IsNullOrEmpty(Schema))

			this.ToTable("GastoPosicao");
		else
			this.ToTable("GastoPosicao",Schema);
		this.HasKey(i => new {i.Identificador });
			this.Property(i => i.Identificador).HasColumnName("ID_GASTO_POSICAO");
			this.Property(i => i.Latitude).HasColumnName("NR_LATITUDE").HasPrecision(12,8);
			this.Property(i => i.Longitude).HasColumnName("NR_LONGITUDE").HasPrecision(18,8);
			this.Property(i => i.IdentificadorGasto).HasColumnName("ID_GASTO");
			this.Property(i => i.IdentificadorCidade).HasColumnName("ID_CIDADE");
			this.HasOptional(i => i.ItemCidade).WithMany().HasForeignKey(d=>d.IdentificadorCidade);
			this.HasRequired(i => i.ItemGasto).WithMany().HasForeignKey(d=>d.IdentificadorGasto);
			this.Property(i => i.DataAtualizacao).HasColumnName("DT_ATUALIZACAO");
			this.Property(i => i.DataExclusao).HasColumnName("DT_EXCLUSAO");
		MapearCamposManualmente();
		}
	}
}

