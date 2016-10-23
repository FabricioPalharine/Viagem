using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using CV.Model;

namespace CV.Data.Configuration
{
	public partial class PaisConfiguration:  EntityTypeConfiguration<Pais>
	{
		public PaisConfiguration()
		{
			string Schema = System.Configuration.ConfigurationManager.AppSettings["Schema"];
			if (string.IsNullOrEmpty(Schema))

			this.ToTable("Pais");
		else
			this.ToTable("Pais",Schema);
		this.HasKey(i => new {i.Identificador });
			this.Property(i => i.Identificador).HasColumnName("ID_PAIS");
			this.Property(i => i.Sigla).HasColumnName("SG_PAIS");
			this.Property(i => i.Nome).HasColumnName("NM_PAIS");
			this.HasMany(i => i.Cidades).WithRequired().HasForeignKey(d=>d.IdentificadorPais);
		MapearCamposManualmente();
		}
	}
}

