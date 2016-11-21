using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using CV.Model;

namespace CV.Data.Configuration
{
	public partial class RefeicaoConfiguration:  EntityTypeConfiguration<Refeicao>
	{
		public RefeicaoConfiguration()
		{
			string Schema = System.Configuration.ConfigurationManager.AppSettings["Schema"];
			if (string.IsNullOrEmpty(Schema))

			this.ToTable("Refeicao");
		else
			this.ToTable("Refeicao",Schema);
		this.HasKey(i => new {i.Identificador });
			this.Property(i => i.Identificador).HasColumnName("ID_REFEICAO");
			this.Property(i => i.IdentificadorViagem).HasColumnName("ID_VIAGEM");
			this.Property(i => i.IdentificadorCidade).HasColumnName("ID_CIDADE");
			this.Property(i => i.Nome).HasColumnName("NM_RESTAURANTE");
			this.Property(i => i.CodigoPlace).HasColumnName("CD_PLACE");
			this.Property(i => i.Data).HasColumnName("DT_REFEICAO");
			this.Property(i => i.Latitude).HasColumnName("NR_LATITUDE").HasPrecision(12,8);
			this.Property(i => i.Longitude).HasColumnName("NR_LONGITUDE").HasPrecision(12,8);
			this.Property(i => i.Tipo).HasColumnName("DS_TIPO_RESTAURANTE");
			this.HasOptional(i => i.ItemCidade).WithMany().HasForeignKey(d=>d.IdentificadorCidade);
			this.HasMany(i => i.Fotos).WithRequired().HasForeignKey(d=>d.IdentificadorRefeicao);
			this.HasMany(i => i.Gastos).WithRequired().HasForeignKey(d=>d.IdentificadorRefeicao);
			this.HasRequired(i => i.ItemViagem).WithMany().HasForeignKey(d=>d.IdentificadorViagem);
			this.Property(i => i.IdentificadorAtracao).HasColumnName("ID_ATRACAO");
			this.HasOptional(i => i.ItemAtracao).WithMany().HasForeignKey(d=>d.IdentificadorAtracao);
			this.HasMany(i => i.Pedidos).WithRequired().HasForeignKey(d=>d.IdentificadorRefeicao);
			this.Property(i => i.DataAtualizacao).HasColumnName("DT_ATUALIZACAO");
			this.Property(i => i.DataExclusao).HasColumnName("DT_EXCLUSAO");
		MapearCamposManualmente();
		}
	}
}

