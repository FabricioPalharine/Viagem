using CV.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV.Data.Configuration
{
    public class FotoUsuarioConfiguration : EntityTypeConfiguration<FotoUsuario>
    {
        public FotoUsuarioConfiguration()
        {
            string Schema = System.Configuration.ConfigurationManager.AppSettings["Schema"];
            if (string.IsNullOrEmpty(Schema))

                this.ToTable("FotoUsuario");
            else
                this.ToTable("FotoUsuario", Schema);
            this.HasKey(i => new { i.Identificador });
            this.Property(i => i.Identificador).HasColumnName("ID_FOTO_USUARIO");
            this.Property(i => i.IdentificadorUsuario).HasColumnName("ID_USUARIO");
            this.Property(i => i.IdentificadorFoto).HasColumnName("ID_FOTO");
            this.Property(i => i.CodigoGoogle).HasColumnName("CD_GOOGLE");
        }
    }
}
