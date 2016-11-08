using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using CV.Model;

namespace CV.Data.Configuration
{
	public partial class CidadeGrupoConfiguration:  EntityTypeConfiguration<CidadeGrupo>
	{
		public CidadeGrupoConfiguration()
		{
			string Schema = System.Configuration.ConfigurationManager.AppSettings["Schema"];
			if (string.IsNullOrEmpty(Schema))

			this.ToTable("CidadeGrupo");
		else
			this.ToTable("CidadeGrupo",Schema);
		this.HasKey(i => new {i.Identificador });
			this.Property(i => i.Identificador).HasColumnName("ID_CIDADE_GRUPO");
			this.Property(i => i.IdentificadorViagem).HasColumnName("ID_VIAGEM");
			this.Property(i => i.IdentificadorCidadeFilha).HasColumnName("ID_CIDADE");
			this.Property(i => i.IdentificadorCidadePai).HasColumnName("ID_CIDADE_MACRO");
			this.HasRequired(i => i.ItemCidadeFilha).WithMany().HasForeignKey(d=>d.IdentificadorCidadeFilha);
			this.HasRequired(i => i.ItemCidadePai).WithMany().HasForeignKey(d=>d.IdentificadorCidadePai);
			this.HasRequired(i => i.ItemViagem).WithMany().HasForeignKey(d=>d.IdentificadorViagem);
			this.Property(i => i.DataAtualizacao).HasColumnName("DT_ATUALIZACAO");
			this.Property(i => i.DataExclusao).HasColumnName("DT_EXCLUSAO");
		MapearCamposManualmente();
		}
	}
}

