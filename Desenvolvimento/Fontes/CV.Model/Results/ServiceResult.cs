using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CV.Model.Results
{
	public class ServiceResult
	{
		public bool Success { get; set; }
		public List<Message> Messages { get; set; }

		public ServiceResult()
		{
			Success = false;
			Messages = new List<Message>();
		}
	}
}