using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using CV.Model;

namespace CV.Data.Configuration
{
	public partial class GastoCompraConfiguration:  EntityTypeConfiguration<GastoCompra>
	{
		public GastoCompraConfiguration()
		{
			string Schema = System.Configuration.ConfigurationManager.AppSettings["Schema"];
			if (string.IsNullOrEmpty(Schema))

			this.ToTable("GastoCompra");
		else
			this.ToTable("GastoCompra",Schema);
		this.HasKey(i => new {i.Identificador });
			this.Property(i => i.Identificador).HasColumnName("ID_GASTO_COMPRA");
			this.Property(i => i.IdentificadorLoja).HasColumnName("ID_LOJA");
			this.Property(i => i.IdentificadorGasto).HasColumnName("ID_GASTO");
			this.HasRequired(i => i.ItemGasto).WithMany().HasForeignKey(d=>d.IdentificadorGasto);
			this.HasMany(i => i.ItensComprados).WithRequired().HasForeignKey(d=>d.IdentificadorGastoCompra);
			this.HasRequired(i => i.ItemLoja).WithMany().HasForeignKey(d=>d.IdentificadorLoja);
		MapearCamposManualmente();
		}
	}
}

