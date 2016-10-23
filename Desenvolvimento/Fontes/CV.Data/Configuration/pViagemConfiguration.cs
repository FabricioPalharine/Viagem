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
		public void MapearCamposManualmente()
		{
		}
	}
}
