using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using CV.Model;

namespace CV.Data.Configuration
{
	public partial class ViagemAereaConfiguration:  EntityTypeConfiguration<ViagemAerea>
	{
		public ViagemAereaConfiguration()
		{
			string Schema = System.Configuration.ConfigurationManager.AppSettings["Schema"];
			if (string.IsNullOrEmpty(Schema))

			this.ToTable("ViagemAerea");
		else
			this.ToTable("ViagemAerea",Schema);
		this.HasKey(i => new {i.Identificador });
			this.Property(i => i.Identificador).HasColumnName("ID_VIAGEM_AEREA");
			this.Property(i => i.IdentificadorViagem).HasColumnName("ID_VIAGEM");
			this.Property(i => i.CompanhiaAerea).HasColumnName("DS_COMPANHIA_AEREA");
			this.Property(i => i.DataPrevista).HasColumnName("DT_PREVISTO");
			this.Property(i => i.DataInicio).HasColumnName("DT_INICIO");
			this.Property(i => i.DataFim).HasColumnName("DT_FIM");
			this.HasRequired(i => i.ItemViagem).WithMany().HasForeignKey(d=>d.IdentificadorViagem);
			this.HasMany(i => i.Gastos).WithRequired().HasForeignKey(d=>d.IdentificadorViagemAerea);
			this.HasMany(i => i.Aeroportos).WithRequired().HasForeignKey(d=>d.IdentificadorViagemAerea);
			this.HasMany(i => i.Avaliacoes).WithRequired().HasForeignKey(d=>d.IdentificadorViagemAerea);
		MapearCamposManualmente();
		}
	}
}

