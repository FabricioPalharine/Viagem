using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using CV.Model;

namespace CV.Data.Configuration
{
	public partial class HotelAvaliacaoConfiguration:  EntityTypeConfiguration<HotelAvaliacao>
	{
		public HotelAvaliacaoConfiguration()
		{
			string Schema = System.Configuration.ConfigurationManager.AppSettings["Schema"];
			if (string.IsNullOrEmpty(Schema))

			this.ToTable("HotelAvaliacao");
		else
			this.ToTable("HotelAvaliacao",Schema);
		this.HasKey(i => new {i.Identificador });
			this.Property(i => i.Identificador).HasColumnName("ID_HOTEL_AVALIACAO");
			this.Property(i => i.IdentificadorUsuario).HasColumnName("ID_USUARIO");
			this.Property(i => i.IdentificadorHotel).HasColumnName("ID_HOTEL");
			this.Property(i => i.Nota).HasColumnName("NR_NOTA");
			this.Property(i => i.Comentario).HasColumnName("DS_COMENTARIO");
			this.HasRequired(i => i.ItemHotel).WithMany().HasForeignKey(d=>d.IdentificadorHotel);
			this.HasRequired(i => i.ItemUsuario).WithMany().HasForeignKey(d=>d.IdentificadorUsuario);
		MapearCamposManualmente();
		}
	}
}

