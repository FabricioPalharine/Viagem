﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV.Model
{
    public class DadosGoogleToken
    {
        public string access_token { get; set; }
        public string refresh_token { get; set; }
        public string expires_in { get; set; }
        public string CodigoGoogle { get; set; }
        public string EMail { get; set; }
        public string Nome { get; set; }
    }
}
