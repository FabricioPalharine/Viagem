﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV.Model
{
    public class LocaisDetalhes
    {
        public DateTime? DataDe { get; set; }
        public DateTime? DataAte { get; set; }
        public decimal? Distancia { get; set; }
        public string NomeUsuario { get; set; }
        public int? Nota { get; set; }
        public string Comentario { get; set; }
        public string Pedido { get; set; }
    }
}
