using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using CV.Model;

namespace CV.Data.Configuration
{
	public partial class GastoHotelConfiguration:  EntityTypeConfiguration<GastoHotel>
	{
		public GastoHotelConfiguration()
		{
			string Schema = System.Configuration.ConfigurationManager.AppSettings["Schema"];
			if (string.IsNullOrEmpty(Schema))

			this.ToTable("GastoHotel");
		else
			this.ToTable("GastoHotel",Schema);
		this.HasKey(i => new {i.Identificador });
			this.Property(i => i.Identificador).HasColumnName("ID_GASTO_HOTEL");
			this.Property(i => i.IdentificadorHotel).HasColumnName("ID_HOTEL");
			this.Property(i => i.IdentificadorGasto).HasColumnName("ID_GASTO");
			this.HasRequired(i => i.ItemGasto).WithMany().HasForeignKey(d=>d.IdentificadorGasto);
			this.HasRequired(i => i.ItemHotel).WithMany().HasForeignKey(d=>d.IdentificadorHotel);
			this.Property(i => i.DataAtualizacao).HasColumnName("DT_ATUALIZACAO");
			this.Property(i => i.DataExclusao).HasColumnName("DT_EXCLUSAO");
		MapearCamposManualmente();
		}
	}
}

