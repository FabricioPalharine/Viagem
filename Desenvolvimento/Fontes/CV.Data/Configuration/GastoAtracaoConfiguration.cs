using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using CV.Model;

namespace CV.Data.Configuration
{
	public partial class GastoAtracaoConfiguration:  EntityTypeConfiguration<GastoAtracao>
	{
		public GastoAtracaoConfiguration()
		{
			string Schema = System.Configuration.ConfigurationManager.AppSettings["Schema"];
			if (string.IsNullOrEmpty(Schema))

			this.ToTable("GastoAtracao");
		else
			this.ToTable("GastoAtracao",Schema);
		this.HasKey(i => new {i.Identificador });
			this.Property(i => i.Identificador).HasColumnName("ID_GASTO_ATRACAO");
			this.Property(i => i.IdentificadorGasto).HasColumnName("ID_GASTO");
			this.Property(i => i.IdentificadorAtracao).HasColumnName("ID_ATRACAO");
			this.Property(i => i.DataAtualizacao).HasColumnName("DT_ATUALIZACAO");
			this.Property(i => i.DataExclusao).HasColumnName("DT_EXCLUSAO");
			this.HasRequired(i => i.ItemGasto).WithMany().HasForeignKey(d=>d.IdentificadorGasto);
			this.HasRequired(i => i.ItemAtracao).WithMany().HasForeignKey(d=>d.IdentificadorAtracao);
		MapearCamposManualmente();
		}
	}
}

