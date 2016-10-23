using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using CV.Model;

namespace CV.Data.Configuration
{
	public partial class ItemCompraConfiguration:  EntityTypeConfiguration<ItemCompra>
	{
		public ItemCompraConfiguration()
		{
			string Schema = System.Configuration.ConfigurationManager.AppSettings["Schema"];
			if (string.IsNullOrEmpty(Schema))

			this.ToTable("ItemCompra");
		else
			this.ToTable("ItemCompra",Schema);
		this.HasKey(i => new {i.Identificador });
			this.Property(i => i.Identificador).HasColumnName("ID_ITEM_COMPRA");
			this.Property(i => i.IdentificadorGastoCompra).HasColumnName("ID_GASTO_COMPRA");
			this.Property(i => i.IdentificadorListaCompra).HasColumnName("ID_LISTA_COMPRA");
			this.Property(i => i.Descricao).HasColumnName("DS_ITEM");
			this.Property(i => i.Marca).HasColumnName("DS_MARCA");
			this.Property(i => i.Valor).HasColumnName("VL_ITEM").HasPrecision(9,2);
			this.Property(i => i.Reembolsavel).HasColumnName("FL_REEMBOLSAVEL");
			this.Property(i => i.Destinatario).HasColumnName("NM_DESTINATARIO");
			this.HasRequired(i => i.ItemGastoCompra).WithMany().HasForeignKey(d=>d.IdentificadorGastoCompra);
			this.HasMany(i => i.Fotos).WithRequired().HasForeignKey(d=>d.IdentificadorItemCompra);
			this.HasOptional(i => i.ItemListaCompra).WithMany().HasForeignKey(d=>d.IdentificadorListaCompra);
		MapearCamposManualmente();
		}
	}
}

