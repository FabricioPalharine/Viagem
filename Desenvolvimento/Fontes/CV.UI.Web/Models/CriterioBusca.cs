using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CV.UI.Web.Models
{
    public class CriterioBusca
    {

        public int? Index { get; set; }
        public int? Count { get; set; }
        public string[] SortField { get; set; }
        public string[] SortOrder { get; set; }
    }
}