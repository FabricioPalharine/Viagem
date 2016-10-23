using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using CV.Model.Resource;

namespace CV.Model
{

	[HasSelfValidation()]
	public partial class FotoItemCompra
	{
		public FotoItemCompra ()
		{
		}

			public int? Identificador { get; set; }

			[NotNullValidator(MessageTemplateResourceName="FotoItemCompra_IdentificadorFoto",MessageTemplateResourceType=typeof(MensagemModelo))]
			public int? IdentificadorFoto { get; set; }

			[NotNullValidator(MessageTemplateResourceName="FotoItemCompra_IdentificadorItemCompra",MessageTemplateResourceType=typeof(MensagemModelo))]
			public int? IdentificadorItemCompra { get; set; }

			public Foto ItemFoto { get; set; }

			public ItemCompra ItemItemCompra { get; set; }
		 public FotoItemCompra Clone()
		{
			 return (FotoItemCompra) this.MemberwiseClone();
		}
	}

}
