using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using CV.Model;

namespace CV.Data.Configuration
{
	public partial class ReabastecimentoGastoConfiguration:  EntityTypeConfiguration<ReabastecimentoGasto>
	{
		public ReabastecimentoGastoConfiguration()
		{
			string Schema = System.Configuration.ConfigurationManager.AppSettings["Schema"];
			if (string.IsNullOrEmpty(Schema))

			this.ToTable("ReabastecimentoGasto");
		else
			this.ToTable("ReabastecimentoGasto",Schema);
		this.HasKey(i => new {i.Identificador });
			this.Property(i => i.Identificador).HasColumnName("ID_REABASTECIMENTO_GASTO");
			this.Property(i => i.IdentificadorGasto).HasColumnName("ID_GASTO");
			this.Property(i => i.IdentificadorReabastecimento).HasColumnName("ID_ALUGUEL_REABASTECIMENTO");
			this.HasRequired(i => i.ItemReabastecimento).WithMany().HasForeignKey(d=>d.IdentificadorReabastecimento);
			this.HasRequired(i => i.ItemGasto).WithMany().HasForeignKey(d=>d.IdentificadorGasto);
		MapearCamposManualmente();
		}
	}
}

