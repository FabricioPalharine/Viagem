using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using CV.Model;

namespace CV.Data.Configuration
{
	public partial class AtracaoConfiguration:  EntityTypeConfiguration<Atracao>
	{
		public AtracaoConfiguration()
		{
			string Schema = System.Configuration.ConfigurationManager.AppSettings["Schema"];
			if (string.IsNullOrEmpty(Schema))

			this.ToTable("Atracao");
		else
			this.ToTable("Atracao",Schema);
		this.HasKey(i => new {i.Identificador });
			this.Property(i => i.Identificador).HasColumnName("ID_ATRACAO");
			this.Property(i => i.IdentificadorAtracaoPai).HasColumnName("ID_ATRACAO_PAI");
			this.Property(i => i.IdentificadorViagem).HasColumnName("ID_VIAGEM");
			this.Property(i => i.IdentificadorCidade).HasColumnName("ID_CIDADE");
			this.Property(i => i.Nome).HasColumnName("NM_ATRACAO");
			this.Property(i => i.CodigoPlace).HasColumnName("CD_PLACE");
			this.Property(i => i.Latitude).HasColumnName("NR_LATITUDE").HasPrecision(12,8);
			this.Property(i => i.Longitude).HasColumnName("NR_LONGITUDE").HasPrecision(18,2);
			this.Property(i => i.Chegada).HasColumnName("DT_CHEGADA");
			this.Property(i => i.Partida).HasColumnName("DT_PARTIDA");
			this.Property(i => i.Tipo).HasColumnName("DS_TIPO_ATRACAO");
			this.HasMany(i => i.Atracoes).WithOptional().HasForeignKey(d=>d.IdentificadorAtracaoPai);
			this.HasMany(i => i.Avaliacoes).WithRequired().HasForeignKey(d=>d.IdentificadorAtracao);
			this.HasMany(i => i.Fotos).WithRequired().HasForeignKey(d=>d.IdentificadorAtracao);
			this.HasRequired(i => i.ItemCidade).WithMany().HasForeignKey(d=>d.IdentificadorCidade);
			this.HasRequired(i => i.ItemViagem).WithMany().HasForeignKey(d=>d.IdentificadorViagem);
			this.HasOptional(i => i.ItemAtracaoPai).WithMany().HasForeignKey(d=>d.IdentificadorAtracaoPai);
		MapearCamposManualmente();
		}
	}
}

