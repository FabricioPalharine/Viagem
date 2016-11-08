using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using CV.Model;

namespace CV.Data.Configuration
{
	public partial class CarroConfiguration:  EntityTypeConfiguration<Carro>
	{
		public CarroConfiguration()
		{
			string Schema = System.Configuration.ConfigurationManager.AppSettings["Schema"];
			if (string.IsNullOrEmpty(Schema))

			this.ToTable("Carro");
		else
			this.ToTable("Carro",Schema);
		this.HasKey(i => new {i.Identificador });
			this.Property(i => i.Identificador).HasColumnName("ID_CARRO");
			this.Property(i => i.IdentificadorViagem).HasColumnName("ID_VIAGEM");
			this.Property(i => i.Locadora).HasColumnName("DS_LOCADORA");
			this.Property(i => i.Modelo).HasColumnName("DS_CARRO");
			this.Property(i => i.KM).HasColumnName("FL_KM");
			this.Property(i => i.Alugado).HasColumnName("FL_ALUGADO");
			this.HasMany(i => i.Gastos).WithRequired().HasForeignKey(d=>d.IdentificadorCarro);
			this.HasMany(i => i.Reabastecimentos).WithRequired().HasForeignKey(d=>d.IdentificadorCarro);
			this.HasMany(i => i.Avaliacoes).WithRequired().HasForeignKey(d=>d.IdentificadorCarro);
			this.HasMany(i => i.Eventos).WithRequired().HasForeignKey(d=>d.IdentificadorCarro);
			this.HasRequired(i => i.ItemViagem).WithMany().HasForeignKey(d=>d.IdentificadorViagem);
			this.Property(i => i.DataAtualizacao).HasColumnName("DT_ATUALIZACAO");
			this.Property(i => i.DataExclusao).HasColumnName("DT_EXCLUSAO");
		MapearCamposManualmente();
		}
	}
}

