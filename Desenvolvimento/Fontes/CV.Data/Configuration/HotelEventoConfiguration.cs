using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using CV.Model;

namespace CV.Data.Configuration
{
	public partial class HotelEventoConfiguration:  EntityTypeConfiguration<HotelEvento>
	{
		public HotelEventoConfiguration()
		{
			string Schema = System.Configuration.ConfigurationManager.AppSettings["Schema"];
			if (string.IsNullOrEmpty(Schema))

			this.ToTable("HotelEvento");
		else
			this.ToTable("HotelEvento",Schema);
		this.HasKey(i => new {i.Identificador });
			this.Property(i => i.Identificador).HasColumnName("ID_HOTEL_EVENTO");
			this.Property(i => i.IdentificadorHotel).HasColumnName("ID_HOTEL");
			this.Property(i => i.DataEvento).HasColumnName("DT_EVENTO");
			this.Property(i => i.Tipo).HasColumnName("CD_TIPO_EVENTO");
			this.Property(i => i.DataAtualizacao).HasColumnName("DT_ATUALIZACAO");
			this.HasRequired(i => i.ItemHotel).WithMany().HasForeignKey(d=>d.IdentificadorHotel);
		MapearCamposManualmente();
		}
	}
}

