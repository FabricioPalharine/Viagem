using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using CV.Model.Resource;

namespace CV.Model
{

    [HasSelfValidation()]
    public partial class HotelEvento
    {
        public HotelEvento()
        {
        }

        public int? Identificador { get; set; }

        [NotNullValidator(MessageTemplateResourceName = "HotelEvento_IdentificadorHotel", MessageTemplateResourceType = typeof(MensagemModelo))]
        public int? IdentificadorHotel { get; set; }

        [NotNullValidator(MessageTemplateResourceName = "HotelEvento_DataEntrada", MessageTemplateResourceType = typeof(MensagemModelo))]
        public DateTime? DataEntrada { get; set; }

        public DateTime? DataSaida { get; set; }

        [NotNullValidator(MessageTemplateResourceName = "HotelEvento_DataAtualizacao", MessageTemplateResourceType = typeof(MensagemModelo))]
        public DateTime? DataAtualizacao { get; set; }

        public Hotel ItemHotel { get; set; }

        public DateTime? DataExclusao { get; set; }

        [NotNullValidator(MessageTemplateResourceName = "HotelEvento_IdentificadorUsuario", MessageTemplateResourceType = typeof(MensagemModelo))]
        public int? IdentificadorUsuario { get; set; }

        public decimal? LatitudeEntrada { get; set; }
        public decimal? LongitudeEntrada { get; set; }
        
        public decimal? LatitudeSaida { get; set; }
        public decimal? LongitudeSaida { get; set; }
        public Usuario ItemUsuario { get; set; }
        public HotelEvento Clone()
        {
            return (HotelEvento)this.MemberwiseClone();
        }
    }

}
