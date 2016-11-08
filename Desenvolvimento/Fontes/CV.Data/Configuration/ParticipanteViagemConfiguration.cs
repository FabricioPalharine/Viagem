using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using CV.Model;

namespace CV.Data.Configuration
{
	public partial class ParticipanteViagemConfiguration:  EntityTypeConfiguration<ParticipanteViagem>
	{
		public ParticipanteViagemConfiguration()
		{
			string Schema = System.Configuration.ConfigurationManager.AppSettings["Schema"];
			if (string.IsNullOrEmpty(Schema))

			this.ToTable("ParticipanteViagem");
		else
			this.ToTable("ParticipanteViagem",Schema);
		this.HasKey(i => new {i.Identificador });
			this.Property(i => i.Identificador).HasColumnName("ID_PARTICIPANTE_VIAGEM");
			this.Property(i => i.IdentificadorViagem).HasColumnName("ID_VIAGEM");
			this.Property(i => i.IdentificadorUsuario).HasColumnName("ID_USUARIO");
			this.HasOptional(i => i.ItemUsuario).WithMany().HasForeignKey(d=>d.IdentificadorUsuario);
			this.HasRequired(i => i.ItemViagem).WithMany().HasForeignKey(d=>d.IdentificadorViagem);
		MapearCamposManualmente();
		}
	}
}

