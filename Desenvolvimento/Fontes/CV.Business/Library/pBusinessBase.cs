using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CV.Model.Results;
using CV.Model;
using Microsoft.Practices.EnterpriseLibrary.Validation;

namespace CV.Business.Library
{
	public  abstract partial class BusinessBase
	{
public List<MensagemErro> RetornarMensagens
        {
            get
            {
                return serviceResult.SelectMany(d => d.Messages).SelectMany(d => d.Description.Select(e => new CV.Model.MensagemErro() { Campo = d.Field, Mensagem = e })).ToList();
            }
        }

        public void AdicionaErroBusiness(string CodigoMensagemErro)
        {
            AdicionaErroBusiness(CodigoMensagemErro, null);
        }
        public void AdicionaErroBusiness(string CodigoMensagemErro, string Campo)
        {
            Message msg = new Message();
            msg.Description = new List<string>(new string[] { MensagemBusiness.RetornaMensagens(CodigoMensagemErro) });
            ServiceResult resultado = new ServiceResult();
            resultado.Success = false;
            if (!string.IsNullOrEmpty(Campo))
                msg.Field = Campo;
            resultado.Messages.Add(msg);
            serviceResult.Add(resultado);
        }
        protected void AdicionaErroBusinessParametro(string CodigoMensagemErro, string[] ParametrosMensagem)
        {
            Message msg = new Message();
            msg.Description = new List<string>(new string[] { MensagemBusiness.RetornaMensagens(CodigoMensagemErro, ParametrosMensagem) });
            ServiceResult resultado = new ServiceResult();
            resultado.Success = false;

            resultado.Messages.Add(msg);
            serviceResult.Add(resultado);
        }

        protected void AdicionaErroBusinessDireto(string MensagemErro)
        {
            Message msg = new Message();
            msg.Description = new List<string>(new string[] { MensagemErro });
            ServiceResult resultado = new ServiceResult();
            resultado.Success = false;

            resultado.Messages.Add(msg);
            serviceResult.Add(resultado);
        }
	}
}