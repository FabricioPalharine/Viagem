using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using CV.Model;

namespace CV.Data.Configuration
{
	public partial class RequisicaoAmizadeConfiguration:  EntityTypeConfiguration<RequisicaoAmizade>
	{
		public RequisicaoAmizadeConfiguration()
		{
			string Schema = System.Configuration.ConfigurationManager.AppSettings["Schema"];
			if (string.IsNullOrEmpty(Schema))

			this.ToTable("RequisicaoAmizade");
		else
			this.ToTable("RequisicaoAmizade",Schema);
		this.HasKey(i => new {i.Identificador });
			this.Property(i => i.Identificador).HasColumnName("ID_REQUISICAO_AMIZADE");
			this.Property(i => i.IdentificadorUsuario).HasColumnName("ID_USUARIO");
			this.Property(i => i.IdentificadorUsuarioRequisitado).HasColumnName("ID_USUARIO_REQUISITADO");
			this.Property(i => i.EMail).HasColumnName("DS_EMAIL");
			this.Property(i => i.Status).HasColumnName("CD_STATUS");
			this.HasRequired(i => i.ItemUsuario).WithMany().HasForeignKey(d=>d.IdentificadorUsuario);
			this.HasOptional(i => i.ItemUsuarioRequisitado).WithMany().HasForeignKey(d=>d.IdentificadorUsuarioRequisitado);
		MapearCamposManualmente();
		}
	}
}

