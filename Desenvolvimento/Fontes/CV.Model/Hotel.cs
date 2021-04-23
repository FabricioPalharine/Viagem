using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using CV.Model.Resource;

namespace CV.Model
{

    [HasSelfValidation()]
    public partial class Hotel
    {
        public Hotel()
        {
        }

        public int? Identificador { get; set; }

        [NotNullValidator(MessageTemplateResourceName = "Hotel_IdentificadorViagem", MessageTemplateResourceType = typeof(MensagemModelo))]
        public int? IdentificadorViagem { get; set; }

        public int? IdentificadorCidade { get; set; }

        public string Nome { get; set; }

        public bool Movel { get; set; }
        [SelfValidation]
        private void ValidarNome(Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResults results)
        {
            if (Nome == null)
            {
                Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult result =
                      new Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult(Resource.MensagemModelo.Hotel_Nome, this, "Nome", null, null);
                results.AddResult(result);
            }
            else if (Nome.Length > 200)
            {
                Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult result =
                      new Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult(Resource.MensagemModelo.Hotel_Nome_Tamanho, this, "Nome", null, null);
                results.AddResult(result);
            }
        }

        public string CodigoPlace { get; set; }

        public DateTime? DataEntrada { get; set; }

        public DateTime? DataSaidia { get; set; }

        public decimal? Longitude { get; set; }

        public decimal? Latitude { get; set; }

         public DateTime? EntradaPrevista { get; set; }

        public DateTime? SaidaPrevista { get; set; }

        public Cidade ItemCidade { get; set; }

        public IList<FotoHotel> Fotos { get; set; }

        public IList<GastoHotel> Gastos { get; set; }

        public IList<HotelAvaliacao> Avaliacoes { get; set; }

        public Viagem ItemViagem { get; set; }

        public IList<HotelEvento> Eventos { get; set; }

        [NotNullValidator(MessageTemplateResourceName = "Hotel_Raio", MessageTemplateResourceType = typeof(MensagemModelo))]
        public int? Raio { get; set; }

        public DateTime? DataAtualizacao { get; set; }

        public DateTime? DataExclusao { get; set; }
        public Hotel Clone()
        {
            return (Hotel)this.MemberwiseClone();
        }
    }

}
