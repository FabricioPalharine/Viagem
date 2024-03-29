using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using CV.Model;

namespace CV.Data.Configuration
{
	public partial class FotoHotelConfiguration:  EntityTypeConfiguration<FotoHotel>
	{
		public FotoHotelConfiguration()
		{
			string Schema = System.Configuration.ConfigurationManager.AppSettings["Schema"];
			if (string.IsNullOrEmpty(Schema))

			this.ToTable("FotoHotel");
		else
			this.ToTable("FotoHotel",Schema);
		this.HasKey(i => new {i.Identificador });
			this.Property(i => i.Identificador).HasColumnName("ID_FOTO_HOTEL");
			this.Property(i => i.IdentificadorFoto).HasColumnName("ID_FOTO");
			this.Property(i => i.IdentificadorHotel).HasColumnName("ID_HOTEL");
			this.HasRequired(i => i.ItemFoto).WithMany().HasForeignKey(d=>d.IdentificadorFoto);
			this.HasRequired(i => i.ItemHotel).WithMany().HasForeignKey(d=>d.IdentificadorHotel);
			this.Property(i => i.DataAtualizacao).HasColumnName("DT_ATUALIZACAO");
			this.Property(i => i.DataExclusao).HasColumnName("DT_EXCLUSAO");
		MapearCamposManualmente();
		}
	}
}

