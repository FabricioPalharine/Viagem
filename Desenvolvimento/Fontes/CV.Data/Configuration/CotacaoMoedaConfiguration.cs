using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using CV.Model;

namespace CV.Data.Configuration
{
	public partial class CotacaoMoedaConfiguration:  EntityTypeConfiguration<CotacaoMoeda>
	{
		public CotacaoMoedaConfiguration()
		{
			string Schema = System.Configuration.ConfigurationManager.AppSettings["Schema"];
			if (string.IsNullOrEmpty(Schema))

			this.ToTable("CotacaoMoeda");
		else
			this.ToTable("CotacaoMoeda",Schema);
		this.HasKey(i => new {i.Identificador });
			this.Property(i => i.Identificador).HasColumnName("ID_COTACAO_MOEDA");
			this.Property(i => i.Moeda).HasColumnName("CD_MOEDA");
			this.Property(i => i.DataCotacao).HasColumnName("DT_COTACAO");
			this.Property(i => i.ValorCotacao).HasColumnName("VL_COTACAO").HasPrecision(18,6);
			this.Property(i => i.IdentificadorViagem).HasColumnName("ID_VIAGEM");
			this.HasRequired(i => i.ItemViagem).WithMany().HasForeignKey(d=>d.IdentificadorViagem);
			this.Property(i => i.DataAtualizacao).HasColumnName("DT_ATUALIZACAO");
			this.Property(i => i.DataExclusao).HasColumnName("DT_EXCLUSAO");
		MapearCamposManualmente();
		}
	}
}

