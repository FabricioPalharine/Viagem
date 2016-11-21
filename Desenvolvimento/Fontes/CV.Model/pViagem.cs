using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using CV.Model.Resource;

namespace CV.Model
{

	public partial class Viagem
	{
        public string AbertoTexto
        {
            get
            {
                return Aberto.GetValueOrDefault() ? MensagemModelo.Sim : MensagemModelo.Nao;
            }
        }
	}

}
