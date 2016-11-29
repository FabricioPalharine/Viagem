using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using CV.Model;

namespace CV.Data.Configuration
{
	public partial class SugestaoConfiguration:  EntityTypeConfiguration<Sugestao>
	{
		public SugestaoConfiguration()
		{
			string Schema = System.Configuration.ConfigurationManager.AppSettings["Schema"];
			if (string.IsNullOrEmpty(Schema))

			this.ToTable("Sugestao");
		else
			this.ToTable("Sugestao",Schema);
		this.HasKey(i => new {i.Identificador });
			this.Property(i => i.Identificador).HasColumnName("ID_SUGESTAO");
			this.Property(i => i.Local).HasColumnName("NM_LOCAL");
			this.Property(i => i.Latitude).HasColumnName("NR_LATITUDE").HasPrecision(12,8);
			this.Property(i => i.Longitude).HasColumnName("NR_LONGITUDE").HasPrecision(12,8);
			this.Property(i => i.Comentario).HasColumnName("DS_COMENTARIO");
			this.Property(i => i.IdentificadorViagem).HasColumnName("ID_VIAGEM");
			this.Property(i => i.IdentificadorUsuario).HasColumnName("ID_USUARIO");
			this.Property(i => i.IdentificadorCidade).HasColumnName("ID_CIDADE");
			this.Property(i => i.Status).HasColumnName("CD_STATUS");
			this.HasOptional(i => i.ItemCidade).WithMany().HasForeignKey(d=>d.IdentificadorCidade);
			this.HasRequired(i => i.ItemUsuario).WithMany().HasForeignKey(d=>d.IdentificadorUsuario);
			this.HasRequired(i => i.ItemViagem).WithMany().HasForeignKey(d=>d.IdentificadorViagem);
			this.Property(i => i.Tipo).HasColumnName("DS_TIPO");
			this.Property(i => i.DataAtualizacao).HasColumnName("DT_ATUALIZACAO");
			this.Property(i => i.DataExclusao).HasColumnName("DT_EXCLUSAO");
			this.Property(i => i.CodigoPlace).HasColumnName("CD_PLACE");
		MapearCamposManualmente();
		}
	}
}

