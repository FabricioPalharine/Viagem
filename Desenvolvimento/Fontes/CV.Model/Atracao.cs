using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using CV.Model.Resource;

namespace CV.Model
{

    [HasSelfValidation()]
    public partial class Atracao
    {
        public Atracao()
        {
        }

        public int? Identificador { get; set; }

        public int? IdentificadorAtracaoPai { get; set; }

        public int? IdentificadorViagem { get; set; }

        public int? IdentificadorCidade { get; set; }

        public string Nome { get; set; }
        [SelfValidation]
        private void ValidarNome(Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResults results)
        {
            if (Nome == null)
            {
                Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult result =
                      new Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult(Resource.MensagemModelo.Atracao_Nome, this, "Nome", null, null);
                results.AddResult(result);
            }
            else if (Nome.Length > 100)
            {
                Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult result =
                      new Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult(Resource.MensagemModelo.Atracao_Nome_Tamanho, this, "Nome", null, null);
                results.AddResult(result);
            }
        }

        public string CodigoPlace { get; set; }

        public decimal? Latitude { get; set; }

        public decimal? Longitude { get; set; }

        public DateTime? Chegada { get; set; }

        public DateTime? Partida { get; set; }

        public string Tipo { get; set; }

        public IList<Atracao> Atracoes { get; set; }

        public IList<AvaliacaoAtracao> Avaliacoes { get; set; }

        public IList<FotoAtracao> Fotos { get; set; }

        public Cidade ItemCidade { get; set; }

        public Viagem ItemViagem { get; set; }

        public Atracao ItemAtracaoPai { get; set; }

        public DateTime? DataAtualizacao { get; set; }

        public DateTime? DataExclusao { get; set; }
        public decimal? Distancia { get; set; }

        public IList<GastoAtracao> Gastos { get; set; }
        public Atracao Clone()
        {
            return (Atracao)this.MemberwiseClone();
        }
    }

}
