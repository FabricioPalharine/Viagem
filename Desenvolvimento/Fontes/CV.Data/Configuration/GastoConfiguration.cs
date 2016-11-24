using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using CV.Model;

namespace CV.Data.Configuration
{
	public partial class GastoConfiguration:  EntityTypeConfiguration<Gasto>
	{
		public GastoConfiguration()
		{
			string Schema = System.Configuration.ConfigurationManager.AppSettings["Schema"];
			if (string.IsNullOrEmpty(Schema))

			this.ToTable("Gasto");
		else
			this.ToTable("Gasto",Schema);
		this.HasKey(i => new {i.Identificador });
			this.Property(i => i.Identificador).HasColumnName("ID_GASTO");
			this.Property(i => i.IdentificadorViagem).HasColumnName("ID_VIAGEM");
			this.Property(i => i.IdentificadorUsuario).HasColumnName("ID_USUARIO");
			this.Property(i => i.Descricao).HasColumnName("DS_GASTO");
			this.Property(i => i.Data).HasColumnName("DT_GASTO");
			this.Property(i => i.Valor).HasColumnName("VL_GASTO").HasPrecision(18,2);
			this.Property(i => i.Especie).HasColumnName("FL_ESPECIE");
			this.Property(i => i.Moeda).HasColumnName("CD_MOEDA");
			this.Property(i => i.DataPagamento).HasColumnName("DT_PAGAMENTO");
			this.Property(i => i.Dividido).HasColumnName("FL_DIVIDIDO");
			this.Property(i => i.ApenasBaixa).HasColumnName("FL_APENAS_BAIXA");
			this.HasRequired(i => i.ItemUsuario).WithMany().HasForeignKey(d=>d.IdentificadorUsuario);
			this.HasRequired(i => i.ItemViagem).WithMany().HasForeignKey(d=>d.IdentificadorViagem);
			this.Property(i => i.DataAtualizacao).HasColumnName("DT_ATUALIZACAO");
			this.Property(i => i.DataExclusao).HasColumnName("DT_EXCLUSAO");
			this.Property(i => i.Latitude).HasColumnName("NR_LATITUDE").HasPrecision(12,6);
			this.Property(i => i.Longitude).HasColumnName("NR_LONGITUDE");
			this.Property(i => i.IdentificadorCidade).HasColumnName("ID_CIDADE");
			this.HasMany(i => i.Atracoes).WithRequired().HasForeignKey(d=>d.IdentificadorGasto);
			this.HasMany(i => i.Hoteis).WithRequired().HasForeignKey(d=>d.IdentificadorGasto);
			this.HasMany(i => i.Compras).WithOptional().HasForeignKey(d=>d.IdentificadorGasto);
			this.HasMany(i => i.Alugueis).WithRequired().HasForeignKey(d=>d.IdentificadorGasto);
			this.HasMany(i => i.Refeicoes).WithRequired().HasForeignKey(d=>d.IdentificadorGasto);
			this.HasMany(i => i.ViagenAereas).WithRequired().HasForeignKey(d=>d.IdentificadorGasto);
			this.HasMany(i => i.Usuarios).WithOptional().HasForeignKey(d=>d.IdentificadorGasto);
			this.HasMany(i => i.Reabastecimentos).WithRequired().HasForeignKey(d=>d.IdentificadorGasto);
		MapearCamposManualmente();
		}
	}
}

