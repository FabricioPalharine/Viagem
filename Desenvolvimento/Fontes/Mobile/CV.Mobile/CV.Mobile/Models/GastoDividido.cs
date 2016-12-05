﻿using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV.Mobile.Models
{
    public class GastoDividido : ObservableObject
    {
        public int? Id { get; set; }
        public int? Identificador { get; set; }
        public int? IdGasto { get; set; }

        public int? IdentificadorGasto { get; set; }
        public int? IdentificadorUsuario { get; set; }
        public string NomeUsuario { get; set; }
        public Usuario ItemUsuario { get; set; }

    }
}