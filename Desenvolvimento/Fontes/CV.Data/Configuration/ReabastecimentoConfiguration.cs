using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using CV.Model;

namespace CV.Data.Configuration
{
	public partial class ReabastecimentoConfiguration:  EntityTypeConfiguration<Reabastecimento>
	{
		public ReabastecimentoConfiguration()
		{
			string Schema = System.Configuration.ConfigurationManager.AppSettings["Schema"];
			if (string.IsNullOrEmpty(Schema))

			this.ToTable("AluguelReabastecimento");
		else
			this.ToTable("AluguelReabastecimento",Schema);
		this.HasKey(i => new {i.Identificador });
			this.Property(i => i.Identificador).HasColumnName("ID_ALUGUEL_REABASTECIMENTO");
			this.Property(i => i.IdentificadorCarro).HasColumnName("ID_CARRO");
			this.Property(i => i.IdentificadorCidade).HasColumnName("ID_CIDADE");
			this.Property(i => i.Latitude).HasColumnName("NR_LATITUDE").HasPrecision(12,8);
			this.Property(i => i.Longitude).HasColumnName("NR_LONGITUDE").HasPrecision(12,8);
			this.Property(i => i.Litro).HasColumnName("FL_LITRO");
			this.Property(i => i.QuantidadeReabastecida).HasColumnName("VL_REABASTECIDO").HasPrecision(6,2);
			this.HasMany(i => i.Gastos).WithRequired().HasForeignKey(d=>d.IdentificadorReabastecimento);
			this.HasRequired(i => i.ItemCarro).WithMany().HasForeignKey(d=>d.IdentificadorCarro);
			this.HasRequired(i => i.Cidade).WithMany().HasForeignKey(d=>d.IdentificadorCidade);
		MapearCamposManualmente();
		}
	}
}

