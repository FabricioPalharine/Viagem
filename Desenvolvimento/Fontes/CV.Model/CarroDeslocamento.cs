using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using CV.Model.Resource;

namespace CV.Model
{

	[HasSelfValidation()]
	public partial class CarroDeslocamento
	{
		public CarroDeslocamento ()
		{
		}

			public int? Identificador { get; set; }

			[NotNullValidator(MessageTemplateResourceName="CarroDeslocamento_IdentificadorCarro",MessageTemplateResourceType=typeof(MensagemModelo))]
			public int? IdentificadorCarro { get; set; }

			public int? IdentificadorCarroEventoPartida { get; set; }

			public int? IdentificadorCarroEventoChegada { get; set; }

			public CarroEvento ItemCarroEventoPartida { get; set; }

			public CarroEvento ItemCarroEventoChegada { get; set; }

			public IList<CarroDeslocamentoUsuario> Usuarios { get; set; }

			[NotNullValidator(MessageTemplateResourceName="CarroDeslocamento_DataAtualizacao",MessageTemplateResourceType=typeof(MensagemModelo))]
			public DateTime? DataAtualizacao { get; set; }

			public DateTime? DataExclusao { get; set; }

			public string Observacao { get; set; }

            public decimal? Distancia { get; set; }
			public Carro ItemCarro { get; set; }
		 public CarroDeslocamento Clone()
		{
			 return (CarroDeslocamento) this.MemberwiseClone();
		}
	}

}
