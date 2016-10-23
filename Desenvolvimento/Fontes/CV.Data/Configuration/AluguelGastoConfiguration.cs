using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using CV.Model;

namespace CV.Data.Configuration
{
	public partial class AluguelGastoConfiguration:  EntityTypeConfiguration<AluguelGasto>
	{
		public AluguelGastoConfiguration()
		{
			string Schema = System.Configuration.ConfigurationManager.AppSettings["Schema"];
			if (string.IsNullOrEmpty(Schema))

			this.ToTable("AluguelGasto");
		else
			this.ToTable("AluguelGasto",Schema);
		this.HasKey(i => new {i.Identificador });
			this.Property(i => i.Identificador).HasColumnName("ID_ALUGUEL_GASTO");
			this.Property(i => i.IdentificadorCarro).HasColumnName("ID_CARRO");
			this.Property(i => i.IdentificadorGasto).HasColumnName("ID_GASTO");
			this.HasRequired(i => i.ItemCarro).WithMany().HasForeignKey(d=>d.IdentificadorCarro);
			this.HasRequired(i => i.ItemGasto).WithMany().HasForeignKey(d=>d.IdentificadorGasto);
		MapearCamposManualmente();
		}
	}
}

