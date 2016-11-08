using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using CV.Model;

namespace CV.Data.Configuration
{
	public partial class ViagemAereaAeroportoConfiguration:  EntityTypeConfiguration<ViagemAereaAeroporto>
	{
		public ViagemAereaAeroportoConfiguration()
		{
			string Schema = System.Configuration.ConfigurationManager.AppSettings["Schema"];
			if (string.IsNullOrEmpty(Schema))

			this.ToTable("ViagemAereaAeroporto");
		else
			this.ToTable("ViagemAereaAeroporto",Schema);
		this.HasKey(i => new {i.Identificador });
			this.Property(i => i.Identificador).HasColumnName("ID_VIAGEM_AEREA_AEROPORTO");
			this.Property(i => i.IdentificadorViagemAerea).HasColumnName("ID_VIAGEM_AEREA");
			this.Property(i => i.IdentificadorCidade).HasColumnName("ID_CIDADE");
			this.Property(i => i.Aeroporto).HasColumnName("DS_AEROPORTO");
			this.Property(i => i.Latitude).HasColumnName("NR_LATITUDE").HasPrecision(12,8);
			this.Property(i => i.Longitude).HasColumnName("NR_LONGITUDE").HasPrecision(12,8);
			this.Property(i => i.TipoPonto).HasColumnName("CD_TIPO_PONTO");
			this.Property(i => i.DataChegada).HasColumnName("DT_CHEGADA");
			this.Property(i => i.DataPartida).HasColumnName("DT_SAIDA");
			this.HasRequired(i => i.ItemCidade).WithMany().HasForeignKey(d=>d.IdentificadorCidade);
			this.HasRequired(i => i.ItemViagemAerea).WithMany().HasForeignKey(d=>d.IdentificadorViagemAerea);
			this.Property(i => i.CodigoPlace).HasColumnName("CD_PLACE");
			this.Property(i => i.DataAtualizacao).HasColumnName("DT_ATUALIZACAO");
			this.Property(i => i.DataExclusao).HasColumnName("DT_EXCLUSAO");
		MapearCamposManualmente();
		}
	}
}

