using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using CV.Model;

namespace CV.Data.Configuration
{
	public partial class CidadeConfiguration:  EntityTypeConfiguration<Cidade>
	{
		public CidadeConfiguration()
		{
			string Schema = System.Configuration.ConfigurationManager.AppSettings["Schema"];
			if (string.IsNullOrEmpty(Schema))

			this.ToTable("Cidade");
		else
			this.ToTable("Cidade",Schema);
		this.HasKey(i => new {i.Identificador });
			this.Property(i => i.Identificador).HasColumnName("ID_CIDADE");
			this.Property(i => i.IdentificadorPais).HasColumnName("ID_PAIS");
			this.Property(i => i.Nome).HasColumnName("NM_CIDADE");
			this.Property(i => i.Estado).HasColumnName("NM_ESTADO");
			this.HasRequired(i => i.ItemPais).WithMany().HasForeignKey(d=>d.IdentificadorPais);
		MapearCamposManualmente();
		}
	}
}

