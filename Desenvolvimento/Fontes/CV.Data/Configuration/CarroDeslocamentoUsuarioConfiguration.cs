using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using CV.Model;

namespace CV.Data.Configuration
{
	public partial class CarroDeslocamentoUsuarioConfiguration:  EntityTypeConfiguration<CarroDeslocamentoUsuario>
	{
		public CarroDeslocamentoUsuarioConfiguration()
		{
			string Schema = System.Configuration.ConfigurationManager.AppSettings["Schema"];
			if (string.IsNullOrEmpty(Schema))

			this.ToTable("CarroDeslocamentoUsuario");
		else
			this.ToTable("CarroDeslocamentoUsuario",Schema);
		this.HasKey(i => new {i.Identificador });
			this.Property(i => i.Identificador).HasColumnName("ID_CARRO_DESLOCAMENTO_USUARIO");
			this.Property(i => i.IdentificadorCarroDeslocamento).HasColumnName("ID_CARRO_DESLOCAMENTO");
			this.Property(i => i.IdentificadorUsuario).HasColumnName("ID_USUARIO");
			this.HasRequired(i => i.ItemCarroDeslocamento).WithMany().HasForeignKey(d=>d.IdentificadorCarroDeslocamento);
			this.HasRequired(i => i.ItemUsuario).WithMany().HasForeignKey(d=>d.IdentificadorUsuario);
		MapearCamposManualmente();
		}
	}
}

