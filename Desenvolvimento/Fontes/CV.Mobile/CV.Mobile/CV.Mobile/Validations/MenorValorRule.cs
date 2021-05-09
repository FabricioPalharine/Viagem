using System;
using System.Collections.Generic;
using System.Text;

namespace CV.Mobile.Validations
{
    public class MenorValorRule<T> : IValidationRule<T>
    {
        public string ValidationMessage { get; set; }
        public decimal Valor { get; set; }

        public bool Check(T value)
        {
            if (value == null)
            {
                return false;
            }

            var str = Convert.ToDecimal( value);

            return str <= Valor;
        }
    }
}
