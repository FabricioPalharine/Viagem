using System;
using System.Collections.Generic;
using System.Text;

namespace CV.Mobile.Validations
{
    public class MaiorZeroRule<T> : IValidationRule<T>
    {
        public string ValidationMessage { get; set; }

        public bool Check(T value)
        {
            if (value == null)
            {
                return false;
            }

            if (typeof(T) == typeof(DateTime))
            {
                var dta = Convert.ToDateTime(value);
                return dta > new DateTime(1900, 1, 1);
            }
            else
            {
                var str = Convert.ToDecimal(value);

                return str > 0;
            }
        }
    }
}
