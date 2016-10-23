using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using CV.Model;

namespace CV.Data.Configuration
{
	public partial class CarroEventoConfiguration:  EntityTypeConfiguration<CarroEvento>
	{
		public CarroEventoConfiguration()
		{
			string Schema = System.Configuration.ConfigurationManager.AppSettings["Schema"];
			if (string.IsNullOrEmpty(Schema))

			this.ToTable("CarroEvento");
		else
			this.ToTable("CarroEvento",Schema);
		this.HasKey(i => new {i.Identificador });
			this.Property(i => i.Identificador).HasColumnName("ID_CARRO_EVENTO");
			this.Property(i => i.Data).HasColumnName("DT_EVENTO");
			this.Property(i => i.Tipo).HasColumnName("CD_TIPO_EVENTO");
			this.Property(i => i.Odometro).HasColumnName("VL_ODOMETRO");
			this.Property(i => i.Latitude).HasColumnName("NR_LATITUDE").HasPrecision(12,8);
			this.Property(i => i.Longitude).HasColumnName("NR_LONGITUDE").HasPrecision(12,8);
			this.Property(i => i.IdentificadorCarro).HasColumnName("ID_CARRO");
			this.Property(i => i.IdentificadorCidade).HasColumnName("ID_CIDADE");
			this.HasRequired(i => i.ItemCarro).WithMany().HasForeignKey(d=>d.IdentificadorCarro);
			this.HasRequired(i => i.ItemCidade).WithMany().HasForeignKey(d=>d.IdentificadorCidade);
		MapearCamposManualmente();
		}
	}
}

