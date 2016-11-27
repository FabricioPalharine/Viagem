using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using CV.Model;

namespace CV.Data.Configuration
{
	public partial class AporteDinheiroConfiguration:  EntityTypeConfiguration<AporteDinheiro>
	{
		public AporteDinheiroConfiguration()
		{
			string Schema = System.Configuration.ConfigurationManager.AppSettings["Schema"];
			if (string.IsNullOrEmpty(Schema))

			this.ToTable("AporteDinheiro");
		else
			this.ToTable("AporteDinheiro",Schema);
		this.HasKey(i => new {i.Identificador });
			this.Property(i => i.Identificador).HasColumnName("ID_APORTE_DINHEIRO");
			this.Property(i => i.IdentificadorViagem).HasColumnName("ID_VIAGEM");
			this.Property(i => i.IdentificadorUsuario).HasColumnName("ID_USUARIO");
			this.Property(i => i.Valor).HasColumnName("VL_QUANTIDADE").HasPrecision(18,2);
			this.Property(i => i.Moeda).HasColumnName("CD_MOEDA");
			this.Property(i => i.DataAporte).HasColumnName("DT_APORTE");
			this.Property(i => i.Cotacao).HasColumnName("VL_COTACAO").HasPrecision(18,6);
			this.HasRequired(i => i.ItemUsuario).WithMany().HasForeignKey(d=>d.IdentificadorUsuario);
			this.HasRequired(i => i.ItemViagem).WithMany().HasForeignKey(d=>d.IdentificadorViagem);
			this.Property(i => i.DataAtualizacao).HasColumnName("DT_ATUALIZACAO");
			this.Property(i => i.DataExclusao).HasColumnName("DT_EXCLUSAO");
			this.Property(i => i.IdentificadorGasto).HasColumnName("ID_GASTO");
			this.HasOptional(i => i.ItemGasto).WithMany().HasForeignKey(d=>d.IdentificadorGasto);
		MapearCamposManualmente();
		}
	}
}

