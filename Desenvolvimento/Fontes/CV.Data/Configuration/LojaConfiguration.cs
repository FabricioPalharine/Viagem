using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using CV.Model;

namespace CV.Data.Configuration
{
	public partial class LojaConfiguration:  EntityTypeConfiguration<Loja>
	{
		public LojaConfiguration()
		{
			string Schema = System.Configuration.ConfigurationManager.AppSettings["Schema"];
			if (string.IsNullOrEmpty(Schema))

			this.ToTable("Loja");
		else
			this.ToTable("Loja",Schema);
		this.HasKey(i => new {i.Identificador });
			this.Property(i => i.Identificador).HasColumnName("ID_LOJA");
			this.Property(i => i.IdentificadorViagem).HasColumnName("ID_VIAGEM");
			this.Property(i => i.Nome).HasColumnName("NM_LOJA");
			this.Property(i => i.Latitude).HasColumnName("NR_LATITUDE").HasPrecision(12,8);
			this.Property(i => i.Longitude).HasColumnName("NR_LONGITUDE").HasPrecision(12,8);
			this.Property(i => i.CodigoPlace).HasColumnName("CD_PLACE");
			this.Property(i => i.Data).HasColumnName("DT_VISITA");
			this.HasMany(i => i.Gastos).WithRequired().HasForeignKey(d=>d.IdentificadorLoja);
			this.HasRequired(i => i.ItemViagem).WithMany().HasForeignKey(d=>d.IdentificadorViagem);
			this.Property(i => i.IdentificadorAtracao).HasColumnName("ID_ATRACAO");
			this.HasOptional(i => i.ItemAtracao).WithMany().HasForeignKey(d=>d.IdentificadorAtracao);
			this.HasMany(i => i.Avaliacoes).WithRequired().HasForeignKey(d=>d.IdentificadorLoja);
			this.Property(i => i.DataAtualizacao).HasColumnName("DT_ATUALIZACAO");
			this.Property(i => i.DataExclusao).HasColumnName("DT_EXCLUSAO");
			this.Property(i => i.IdentificadorCidade).HasColumnName("ID_CIDADE");
			this.HasOptional(i => i.ItemCidade).WithMany().HasForeignKey(d=>d.IdentificadorCidade);
		MapearCamposManualmente();
		}
	}
}

