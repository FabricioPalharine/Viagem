using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using CV.Model;

namespace CV.Data.Configuration
{
	public partial class GastoRefeicaoConfiguration:  EntityTypeConfiguration<GastoRefeicao>
	{
		public GastoRefeicaoConfiguration()
		{
			string Schema = System.Configuration.ConfigurationManager.AppSettings["Schema"];
			if (string.IsNullOrEmpty(Schema))

			this.ToTable("GastoRefeicao");
		else
			this.ToTable("GastoRefeicao",Schema);
		this.HasKey(i => new {i.Identificador });
			this.Property(i => i.Identificador).HasColumnName("ID_GASTO_REFEICAO");
			this.Property(i => i.IdentificadorRefeicao).HasColumnName("ID_REFEICAO");
			this.Property(i => i.IdentificadorGasto).HasColumnName("ID_GASTO");
			this.HasRequired(i => i.ItemGasto).WithMany().HasForeignKey(d=>d.IdentificadorGasto);
			this.HasRequired(i => i.ItemRefeicao).WithMany().HasForeignKey(d=>d.IdentificadorRefeicao);
			this.Property(i => i.DataAtualizacao).HasColumnName("DT_ATUALIZACAO");
			this.Property(i => i.DataExclusao).HasColumnName("DT_EXCLUSAO");
		MapearCamposManualmente();
		}
	}
}

