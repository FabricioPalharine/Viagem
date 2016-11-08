using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using CV.Model;

namespace CV.Data.Configuration
{
	public partial class HotelConfiguration:  EntityTypeConfiguration<Hotel>
	{
		public HotelConfiguration()
		{
			string Schema = System.Configuration.ConfigurationManager.AppSettings["Schema"];
			if (string.IsNullOrEmpty(Schema))

			this.ToTable("Hotel");
		else
			this.ToTable("Hotel",Schema);
		this.HasKey(i => new {i.Identificador });
			this.Property(i => i.Identificador).HasColumnName("ID_HOTEL");
			this.Property(i => i.IdentificadorViagem).HasColumnName("ID_VIAGEM");
			this.Property(i => i.IdentificadorCidade).HasColumnName("ID_CIDADE");
			this.Property(i => i.Nome).HasColumnName("NM_HOTEL");
			this.Property(i => i.CodigoPlace).HasColumnName("CD_PLACE");
			this.Property(i => i.DataEntrada).HasColumnName("DT_ENTRADA");
			this.Property(i => i.DataSaidia).HasColumnName("DT_SAIDA");
			this.Property(i => i.Longitude).HasColumnName("NR_LONGITUDE").HasPrecision(12,8);
			this.Property(i => i.Latitude).HasColumnName("NR_LATITUDE").HasPrecision(12,8);
			this.Property(i => i.EntradaPrevista).HasColumnName("DT_ENTRADA_PREVISTA");
			this.Property(i => i.SaidaPrevista).HasColumnName("DT_SAIDA_PREVISTA");
			this.HasRequired(i => i.ItemCidade).WithMany().HasForeignKey(d=>d.IdentificadorCidade);
			this.HasMany(i => i.Fotos).WithRequired().HasForeignKey(d=>d.IdentificadorHotel);
			this.HasMany(i => i.Gastos).WithRequired().HasForeignKey(d=>d.IdentificadorHotel);
			this.HasMany(i => i.Avaliacoes).WithRequired().HasForeignKey(d=>d.IdentificadorHotel);
			this.HasRequired(i => i.ItemViagem).WithMany().HasForeignKey(d=>d.IdentificadorViagem);
			this.HasMany(i => i.Eventos).WithRequired().HasForeignKey(d=>d.IdentificadorHotel);
			this.Property(i => i.Raio).HasColumnName("NR_RAIO");
			this.Property(i => i.DataAtualizacao).HasColumnName("DT_ATUALIZACAO");
			this.Property(i => i.DataExclusao).HasColumnName("DT_EXCLUSAO");
		MapearCamposManualmente();
		}
	}
}

