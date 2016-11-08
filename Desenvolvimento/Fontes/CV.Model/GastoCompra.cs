using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using CV.Model.Resource;

namespace CV.Model
{

	[HasSelfValidation()]
	public partial class GastoCompra
	{
		public GastoCompra ()
		{
		}

			public int? Identificador { get; set; }

			[NotNullValidator(MessageTemplateResourceName="GastoCompra_IdentificadorLoja",MessageTemplateResourceType=typeof(MensagemModelo))]
			public int? IdentificadorLoja { get; set; }

			[NotNullValidator(MessageTemplateResourceName="GastoCompra_IdentificadorGasto",MessageTemplateResourceType=typeof(MensagemModelo))]
			public int? IdentificadorGasto { get; set; }

			public Gasto ItemGasto { get; set; }

			public IList<ItemCompra> ItensComprados { get; set; }

			public Loja ItemLoja { get; set; }

			public DateTime? DataAtualizacao { get; set; }

			public DateTime? DataExclusao { get; set; }
		 public GastoCompra Clone()
		{
			 return (GastoCompra) this.MemberwiseClone();
		}
	}

}
