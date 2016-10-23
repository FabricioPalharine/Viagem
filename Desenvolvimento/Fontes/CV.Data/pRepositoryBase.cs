using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CV.Data
{
	public abstract partial class RepositoryBase: IDisposable
	
	{
		private string RetornaConexao()
		{
			return System.Configuration.ConfigurationManager.AppSettings["ConexaoBanco"];
		}


	}
}