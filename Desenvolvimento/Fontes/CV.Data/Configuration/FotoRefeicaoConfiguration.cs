using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using CV.Model;

namespace CV.Data.Configuration
{
	public partial class FotoRefeicaoConfiguration:  EntityTypeConfiguration<FotoRefeicao>
	{
		public FotoRefeicaoConfiguration()
		{
			string Schema = System.Configuration.ConfigurationManager.AppSettings["Schema"];
			if (string.IsNullOrEmpty(Schema))

			this.ToTable("FotoRefeicao");
		else
			this.ToTable("FotoRefeicao",Schema);
		this.HasKey(i => new {i.Identificador });
			this.Property(i => i.Identificador).HasColumnName("ID_FOTO_REFEICAO");
			this.Property(i => i.IdentificadorFoto).HasColumnName("ID_FOTO");
			this.Property(i => i.IdentificadorRefeicao).HasColumnName("ID_REFEICAO");
			this.HasRequired(i => i.ItemFoto).WithMany().HasForeignKey(d=>d.IdentificadorFoto);
			this.HasRequired(i => i.ItemRefeicao).WithMany().HasForeignKey(d=>d.IdentificadorRefeicao);
		MapearCamposManualmente();
		}
	}
}

