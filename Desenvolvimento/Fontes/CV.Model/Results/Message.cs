using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CV.Model.Results
{
	public class Message
	{
		public string Field { get; set; }
		public List<string> Description { get; set; }

		public Message()
		{
			Field = string.Empty;
			Description = new List<string>();
		}
	}

}