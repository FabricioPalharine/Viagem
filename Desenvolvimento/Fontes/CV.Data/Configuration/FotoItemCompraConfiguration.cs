using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using CV.Model;

namespace CV.Data.Configuration
{
	public partial class FotoItemCompraConfiguration:  EntityTypeConfiguration<FotoItemCompra>
	{
		public FotoItemCompraConfiguration()
		{
			string Schema = System.Configuration.ConfigurationManager.AppSettings["Schema"];
			if (string.IsNullOrEmpty(Schema))

			this.ToTable("FotoItemCompra");
		else
			this.ToTable("FotoItemCompra",Schema);
		this.HasKey(i => new {i.Identificador });
			this.Property(i => i.Identificador).HasColumnName("ID_FOTO_ITEM_COMPRA");
			this.Property(i => i.IdentificadorFoto).HasColumnName("ID_FOTO");
			this.Property(i => i.IdentificadorItemCompra).HasColumnName("ID_ITEM_COMPRA");
			this.HasRequired(i => i.ItemFoto).WithMany().HasForeignKey(d=>d.IdentificadorFoto);
			this.HasRequired(i => i.ItemItemCompra).WithMany().HasForeignKey(d=>d.IdentificadorItemCompra);
			this.Property(i => i.DataAtualizacao).HasColumnName("DT_ATUALIZACAO");
			this.Property(i => i.DataExclusao).HasColumnName("DT_EXCLUSAO");
		MapearCamposManualmente();
		}
	}
}

