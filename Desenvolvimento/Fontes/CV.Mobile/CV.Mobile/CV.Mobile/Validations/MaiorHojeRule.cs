using System;
using System.Collections.Generic;
using System.Text;

namespace CV.Mobile.Validations
{
    public class MaiorHojeRule<T> : IValidationRule<T>
    {
        public string ValidationMessage { get; set; }

        public bool Check(T value)
        {
            if (value == null )
            {
                return false;
            }

            var str = Convert.ToDateTime( value);

            return str > DateTime.Today;
        }
    }
}
