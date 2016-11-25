using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using CV.Model.Resource;
using CV.Model.Dominio;

namespace CV.Model
{

	public partial class ListaCompra
	{
        public string MoedaSigla
        {
            get
            {
                return Moeda.HasValue ? ((enumMoeda)Moeda.Value).ToString() : null;
            }
        }

        public string DestinatarioSelecao
        {
            get
            {
                if (IdentificadorUsuarioPedido.HasValue && ItemUsuario != null)
                    return ItemUsuarioPedido.Nome;
                else
                    return Destinatario;
            }
        }

        public string Comprado
        {
            get
            {
                return (Status.GetValueOrDefault(-1) == (int)enumStatusListaCompra.Comprado) ? MensagemModelo.Sim : MensagemModelo.Nao;
            }
        }
    }

}
