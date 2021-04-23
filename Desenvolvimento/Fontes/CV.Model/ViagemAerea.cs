using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using CV.Model.Resource;

namespace CV.Model
{

    [HasSelfValidation()]
    public partial class ViagemAerea
    {
        public ViagemAerea()
        {
        }

        public int? Identificador { get; set; }

        [NotNullValidator(MessageTemplateResourceName = "ViagemAerea_IdentificadorViagem", MessageTemplateResourceType = typeof(MensagemModelo))]
        public int? IdentificadorViagem { get; set; }

        public string CompanhiaAerea { get; set; }
       

        [NotNullValidator(MessageTemplateResourceName = "ViagemAerea_DataPrevista", MessageTemplateResourceType = typeof(MensagemModelo))]
        public DateTime? DataPrevista { get; set; }

        public Viagem ItemViagem { get; set; }

        public IList<GastoViagemAerea> Gastos { get; set; }

        public IList<ViagemAereaAeroporto> Aeroportos { get; set; }

        public IList<AvaliacaoAerea> Avaliacoes { get; set; }

        public DateTime? DataAtualizacao { get; set; }

        public DateTime? DataExclusao { get; set; }

        public decimal? Distancia { get; set; }


        [NotNullValidator(MessageTemplateResourceName = "ViagemAerea_Tipo", MessageTemplateResourceType = typeof(MensagemModelo))]
        public int? Tipo { get; set; }

        public string Descricao { get; set; }
        public ViagemAerea Clone()
        {
            return (ViagemAerea)this.MemberwiseClone();
        }
    }

}
