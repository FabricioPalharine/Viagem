using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using CV.Model;

namespace CV.Data.Configuration
{
	public partial class CalendarioPrevistoConfiguration:  EntityTypeConfiguration<CalendarioPrevisto>
	{
		public CalendarioPrevistoConfiguration()
		{
			string Schema = System.Configuration.ConfigurationManager.AppSettings["Schema"];
			if (string.IsNullOrEmpty(Schema))

			this.ToTable("CalendarioPrevisto");
		else
			this.ToTable("CalendarioPrevisto",Schema);
		this.HasKey(i => new {i.Identificador });
			this.Property(i => i.Identificador).HasColumnName("ID_CALENDARIO_PREVISTO");
			this.Property(i => i.IdentificadorViagem).HasColumnName("ID_VIAGEM");
			this.Property(i => i.Data).HasColumnName("DT_CALENDARIO");
			this.Property(i => i.Inicio).HasColumnName("HR_INICIO");
			this.Property(i => i.Fim).HasColumnName("HR_FIM");
			this.Property(i => i.Nome).HasColumnName("NM_LOCAL");
			this.Property(i => i.Latitude).HasColumnName("NR_LATITUDE").HasPrecision(12,8);
			this.Property(i => i.Longitude).HasColumnName("NR_LONGITUDE").HasPrecision(12,8);
			this.Property(i => i.CodigoPlace).HasColumnName("CD_PLACE");
			this.HasRequired(i => i.ItemViagem).WithMany().HasForeignKey(d=>d.IdentificadorViagem);
			this.Property(i => i.Tipo).HasColumnName("DS_TIPO");
			this.Property(i => i.Prioridade).HasColumnName("CD_PRIORIDADE");
		MapearCamposManualmente();
		}
	}
}

