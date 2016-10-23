using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using CV.Model;

namespace CV.Data.Configuration
{
	public partial class RefeicaoPedidoConfiguration:  EntityTypeConfiguration<RefeicaoPedido>
	{
		public RefeicaoPedidoConfiguration()
		{
			string Schema = System.Configuration.ConfigurationManager.AppSettings["Schema"];
			if (string.IsNullOrEmpty(Schema))

			this.ToTable("RefeicaoPedido");
		else
			this.ToTable("RefeicaoPedido",Schema);
		this.HasKey(i => new {i.Identificador });
			this.Property(i => i.Identificador).HasColumnName("ID_REFEICAO_PEDIDO");
			this.Property(i => i.IdentificadorRefeicao).HasColumnName("ID_REFEICAO");
			this.Property(i => i.IdentificadorUsuario).HasColumnName("ID_USUARIO");
			this.Property(i => i.Pedido).HasColumnName("DS_PEDIDO");
			this.Property(i => i.Nota).HasColumnName("NR_NOTA");
			this.Property(i => i.Comentario).HasColumnName("DS_COMENTARIO");
			this.HasRequired(i => i.ItemRefeicao).WithMany().HasForeignKey(d=>d.IdentificadorRefeicao);
			this.HasRequired(i => i.ItemUsuario).WithMany().HasForeignKey(d=>d.IdentificadorUsuario);
		MapearCamposManualmente();
		}
	}
}

