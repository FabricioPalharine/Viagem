using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using CV.Model;

namespace CV.Data.Configuration
{
	public partial class GastoViagemAereaConfiguration:  EntityTypeConfiguration<GastoViagemAerea>
	{
		public GastoViagemAereaConfiguration()
		{
			string Schema = System.Configuration.ConfigurationManager.AppSettings["Schema"];
			if (string.IsNullOrEmpty(Schema))

			this.ToTable("GastoViagemAerea");
		else
			this.ToTable("GastoViagemAerea",Schema);
		this.HasKey(i => new {i.Identificador });
			this.Property(i => i.Identificador).HasColumnName("ID_GASTO_VIAGEM_AEREA");
			this.Property(i => i.IdentificadorGasto).HasColumnName("ID_GASTO");
			this.Property(i => i.IdentificadorViagemAerea).HasColumnName("ID_VIAGEM_AEREA");
			this.HasRequired(i => i.ItemGasto).WithMany().HasForeignKey(d=>d.IdentificadorGasto);
			this.HasRequired(i => i.ItemViagemAerea).WithMany().HasForeignKey(d=>d.IdentificadorViagemAerea);
		MapearCamposManualmente();
		}
	}
}

