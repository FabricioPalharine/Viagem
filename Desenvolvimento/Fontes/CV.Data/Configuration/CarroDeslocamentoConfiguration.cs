using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using CV.Model;

namespace CV.Data.Configuration
{
	public partial class CarroDeslocamentoConfiguration:  EntityTypeConfiguration<CarroDeslocamento>
	{
		public CarroDeslocamentoConfiguration()
		{
			string Schema = System.Configuration.ConfigurationManager.AppSettings["Schema"];
			if (string.IsNullOrEmpty(Schema))

			this.ToTable("CarroDeslocamento");
		else
			this.ToTable("CarroDeslocamento",Schema);
		this.HasKey(i => new {i.Identificador });
			this.Property(i => i.Identificador).HasColumnName("ID_CARRO_DESLOCAMENTO");
			this.Property(i => i.IdentificadorCarro).HasColumnName("ID_CARRO");
			this.Property(i => i.IdentificadorCarroEventoPartida).HasColumnName("ID_CARRO_EVENTO_PARTIDA");
			this.Property(i => i.IdentificadorCarroEventoChegada).HasColumnName("ID_CARRO_EVENTO_CHEGADA");
			this.HasOptional(i => i.ItemCarroEventoPartida).WithMany().HasForeignKey(d=>d.IdentificadorCarroEventoPartida);
			this.HasOptional(i => i.ItemCarroEventoChegada).WithMany().HasForeignKey(d=>d.IdentificadorCarroEventoChegada);
			this.HasMany(i => i.Usuarios).WithRequired().HasForeignKey(d=>d.IdentificadorCarroDeslocamento);
			this.Property(i => i.DataAtualizacao).HasColumnName("DT_ATUALIZACAO");
			this.Property(i => i.DataExclusao).HasColumnName("DT_EXCLUSAO");
			this.Property(i => i.Observacao).HasColumnName("DS_OBSERVACAO");
			this.HasRequired(i => i.ItemCarro).WithMany().HasForeignKey(d=>d.IdentificadorCarro);
		MapearCamposManualmente();
		}
	}
}

