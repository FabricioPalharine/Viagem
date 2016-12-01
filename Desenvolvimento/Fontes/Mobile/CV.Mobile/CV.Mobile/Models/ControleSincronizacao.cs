using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV.Mobile.Models
{
    public class ControleSincronizacao 
    {
        private int? id;
        private int? _IdentificadorViagem;
        private DateTime? ultimaDataEnvio;
        private DateTime? ultimaDataRecepcao;

        public int? Id
        {
            get
            {
                return id;
            }

            set
            {
                id = value;
            }
        }

        public int? IdentificadorViagem
        {
            get
            {
                return _IdentificadorViagem;
            }

            set
            {
                _IdentificadorViagem = value;
            }
        }

        public DateTime? UltimaDataEnvio
        {
            get
            {
                return ultimaDataEnvio;
            }

            set
            {
                ultimaDataEnvio = value;
            }
        }

        public DateTime? UltimaDataRecepcao
        {
            get
            {
                return ultimaDataRecepcao;
            }

            set
            {
                ultimaDataRecepcao = value;
            }
        }
    }
}
