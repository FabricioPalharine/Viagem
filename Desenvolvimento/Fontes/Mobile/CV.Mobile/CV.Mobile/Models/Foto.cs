using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV.Mobile.Models
{
    public class Foto: ObservableObject
    {
        private string _Comentario;
        private decimal? _Latitude;
        private decimal? _Longitude;
        private DateTime? _Data;
        private string _LinkThumbnail;
        private string _LinkFoto;
        private string _CaminhoDispositivo;
        private string _CodigoFoto;
        private bool _Video;
        private string _TipoArquivo;


        public int? Id { get; set; }
        public int? Identificador { get; set; }
        public int? IdentificadorViagem { get; set; }
        public int? IdentificadorUsuario { get; set; }
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

        public decimal? Latitude
        {
            get
            {
                return _Latitude;
            }

            set
            {
                SetProperty(ref _Latitude, value);
            }
        }

        public decimal? Longitude
        {
            get
            {
                return _Longitude;
            }

            set
            {
                SetProperty(ref _Longitude, value);
            }
        }

        public DateTime? Data
        {
            get
            {
                return _Data;
            }

            set
            {
                SetProperty(ref _Data, value);
            }
        }

        public string LinkThumbnail
        {
            get
            {
                return _LinkThumbnail;
            }

            set
            {
                SetProperty(ref _LinkThumbnail, value);
            }
        }

        public string LinkFoto
        {
            get
            {
                return _LinkFoto;
            }

            set
            {
                SetProperty(ref _LinkFoto, value);
            }
        }

        public string CaminhoDispositivo
        {
            get
            {
                return _CaminhoDispositivo;
            }

            set
            {
                SetProperty(ref _CaminhoDispositivo, value);
            }
        }

        public string CodigoFoto
        {
            get
            {
                return _CodigoFoto;
            }

            set
            {
                SetProperty(ref _CodigoFoto, value);
            }
        }

        public bool Video
        {
            get
            {
                return _Video;
            }

            set
            {
                SetProperty(ref _Video, value);
            }
        }

        public string TipoArquivo
        {
            get
            {
                return _TipoArquivo;
            }

            set
            {
                SetProperty(ref _TipoArquivo, value);
            }
        }

        public string LinkControle
        {
            get
            {
                return Video ? LinkThumbnail : LinkFoto;
            }
        }
    }
}
