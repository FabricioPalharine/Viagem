using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using CV.Model;

namespace CV.Data.Configuration
{
	public partial class ViagemConfiguration:  EntityTypeConfiguration<Viagem>
	{
		public ViagemConfiguration()
		{
			string Schema = System.Configuration.ConfigurationManager.AppSettings["Schema"];
			if (string.IsNullOrEmpty(Schema))

			this.ToTable("Viagem");
		else
			this.ToTable("Viagem",Schema);
		this.HasKey(i => new {i.Identificador });
			this.Property(i => i.Identificador).HasColumnName("ID_VIAGEM");
			this.Property(i => i.IdentificadorUsuario).HasColumnName("ID_USUARIO");
			this.Property(i => i.Nome).HasColumnName("NM_VIAGEM");
			this.Property(i => i.DataInicio).HasColumnName("DT_INICIO");
			this.Property(i => i.DataFim).HasColumnName("DT_FIM");
			this.Property(i => i.Aberto).HasColumnName("FL_ABERTO");
			this.Property(i => i.UnidadeMetrica).HasColumnName("FL_METRICA");
			this.Property(i => i.QuantidadeParticipantes).HasColumnName("NR_PARTICIPANTES");
			this.Property(i => i.PublicaGasto).HasColumnName("FL_PUBLICA_GASTO");
			this.Property(i => i.PercentualIOF).HasColumnName("PC_IOF").HasPrecision(9,4);
			this.HasRequired(i => i.ItemUsuario).WithMany().HasForeignKey(d=>d.IdentificadorUsuario);
			this.HasMany(i => i.Participantes).WithRequired().HasForeignKey(d=>d.IdentificadorViagem);
			this.Property(i => i.Moeda).HasColumnName("CD_MOEDA");
			this.HasMany(i => i.UsuariosGastos).WithRequired().HasForeignKey(d=>d.IdentificadorViagem);
			this.Property(i => i.DataAlteracao).HasColumnName("DT_ATUALIZACAO");
			this.Property(i => i.DataExclusao).HasColumnName("DT_EXCLUSAO");
			this.Property(i => i.CodigoAlbum).HasColumnName("CD_ALBUM");
            this.Property(d => d.ShareToken).HasColumnName("CD_SHARE_TOKEN");
		MapearCamposManualmente();
		}
	}
}

