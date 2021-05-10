using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using CV.Model;

namespace CV.Data.Configuration
{
	public partial class FotoConfiguration:  EntityTypeConfiguration<Foto>
	{
		public FotoConfiguration()
		{
			string Schema = System.Configuration.ConfigurationManager.AppSettings["Schema"];
			if (string.IsNullOrEmpty(Schema))

			this.ToTable("Foto");
		else
			this.ToTable("Foto",Schema);
		this.HasKey(i => new {i.Identificador });
			this.Property(i => i.Identificador).HasColumnName("ID_FOTO");
			this.Property(i => i.IdentificadorViagem).HasColumnName("ID_VIAGEM");
			this.Property(i => i.IdentificadorUsuario).HasColumnName("ID_USUARIO");
			this.Property(i => i.IdentificadorCidade).HasColumnName("ID_CIDADE");
			this.Property(i => i.Comentario).HasColumnName("DS_COMENTARIO");
			this.Property(i => i.Latitude).HasColumnName("NR_LATITUDE").HasPrecision(12,8);
			this.Property(i => i.Longitude).HasColumnName("NR_LONGITUDE").HasPrecision(12,8);
			this.Property(i => i.Data).HasColumnName("DT_FOTO");
			this.Property(i => i.LinkThumbnail).HasColumnName("DS_LINK_THUMBNAIL");
			this.Property(i => i.LinkFoto).HasColumnName("DS_LINK_FOTO");
			//this.Property(i => i.CodigoFoto).HasColumnName("CD_CODIGO_DRIVE");
			this.Property(i => i.Video).HasColumnName("FL_VIDEO");
			this.Property(i => i.TipoArquivo).HasColumnName("CD_TIPO_ARQUIVO");
			this.HasOptional(i => i.ItemCidade).WithMany().HasForeignKey(d=>d.IdentificadorCidade);
			this.HasMany(i => i.Hoteis).WithRequired().HasForeignKey(d=>d.IdentificadorFoto);
			this.HasMany(i => i.ItensCompra).WithRequired().HasForeignKey(d=>d.IdentificadorFoto);
			this.HasMany(i => i.Refeicoes).WithRequired().HasForeignKey(d=>d.IdentificadorFoto);
			this.HasMany(i => i.Atracoes).WithRequired().HasForeignKey(d=>d.IdentificadorFoto);
			this.HasOptional(i => i.ItemUsuario).WithMany().HasForeignKey(d=>d.IdentificadorUsuario);
			this.HasRequired(i => i.ItemViagem).WithMany().HasForeignKey(d=>d.IdentificadorViagem);
			this.Property(i => i.DataAtualizacao).HasColumnName("DT_ATUALIZACAO");
			this.Property(i => i.DataExclusao).HasColumnName("DT_EXCLUSAO");
            this.Property(d => d.NomeArquivo).HasColumnName("NM_ARQUIVO");
            this.Ignore(d => d.CodigoFoto);
            this.HasMany(d => d.FotoUsuarios).WithOptional(d => d.ItemFoto).HasForeignKey(d => d.IdentificadorFoto);
		MapearCamposManualmente();
		}
	}
}

