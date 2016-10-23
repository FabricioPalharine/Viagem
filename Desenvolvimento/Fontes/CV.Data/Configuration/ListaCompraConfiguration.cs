using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using CV.Model;

namespace CV.Data.Configuration
{
	public partial class ListaCompraConfiguration:  EntityTypeConfiguration<ListaCompra>
	{
		public ListaCompraConfiguration()
		{
			string Schema = System.Configuration.ConfigurationManager.AppSettings["Schema"];
			if (string.IsNullOrEmpty(Schema))

			this.ToTable("ListaCompra");
		else
			this.ToTable("ListaCompra",Schema);
		this.HasKey(i => new {i.Identificador });
			this.Property(i => i.Identificador).HasColumnName("ID_LISTA_COMPRA");
			this.Property(i => i.IdentificadorViagem).HasColumnName("ID_VIAGEM");
			this.Property(i => i.IdentificadorUsuario).HasColumnName("ID_USUARIO");
			this.Property(i => i.IdentificadorUsuarioPedido).HasColumnName("ID_USUARIO_PEDIDO");
			this.Property(i => i.Descricao).HasColumnName("DS_ITEM");
			this.Property(i => i.Marca).HasColumnName("DS_MARCA");
			this.Property(i => i.ValorMaximo).HasColumnName("VL_MAXIMO").HasPrecision(9,2);
			this.Property(i => i.Moeda).HasColumnName("CD_MOEDA");
			this.Property(i => i.Reembolsavel).HasColumnName("FL_REEMBOLSAVEL");
			this.Property(i => i.Comprado).HasColumnName("FL_COMPRADO");
			this.HasMany(i => i.ItensCompra).WithOptional().HasForeignKey(d=>d.IdentificadorListaCompra);
			this.HasRequired(i => i.ItemUsuario).WithMany().HasForeignKey(d=>d.IdentificadorUsuario);
			this.HasOptional(i => i.ItemUsuarioPedido).WithMany().HasForeignKey(d=>d.IdentificadorUsuarioPedido);
			this.HasRequired(i => i.Viagem).WithMany().HasForeignKey(d=>d.IdentificadorViagem);
			this.Property(i => i.Destinatario).HasColumnName("DS_DESTINATARIO");
		MapearCamposManualmente();
		}
	}
}

