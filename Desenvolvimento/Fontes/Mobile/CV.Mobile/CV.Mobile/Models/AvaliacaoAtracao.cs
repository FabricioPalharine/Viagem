﻿using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV.Mobile.Models
{
    public class AvaliacaoAtracao: ObservableObject
    {
        private string _Comentario;
        private int? _Nota;
        

        public int? Id { get; set; }
        public int? Identificador { get; set; }
        public int? IdAtracao { get; set; }
        public int? IdentificadorAtracao { get; set; }
        public int? IdentificadorUsuario { get; set; }
        public string NomeUsuario { get; set; }
        public Usuario ItemUsuario { get; set; }
        public DateTime? DataAtualizacao { get; set; }
        public DateTime? DataExclusao { get; set; }

        public string Comentario
        {
            get
            {
                return _Comentario;
            }

            set
            {
                SetProperty(ref _Comentario, value);
            }
        }

        public int? Nota
        {
            get
            {
                return _Nota;
            }

            set
            {
                SetProperty(ref _Nota, value);
            }
        }
    }
}