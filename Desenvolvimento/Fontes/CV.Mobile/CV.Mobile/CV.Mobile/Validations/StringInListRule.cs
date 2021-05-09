using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CV.Mobile.Validations
{
    public class StringInListRule<T> : IValidationRule<T>
    {
        public string ValidationMessage { get; set; }
        public List<string> Valores { get; set; } = new List<string>();
        public bool Check(T value)
        {
            if (value == null)
            {
                return true;
            }

            var str = value as string;
            str = str ?? string.Empty;

            return (!Valores.Any() || !Valores.Where(d => d.Equals(str.Replace(" ",string.Empty), StringComparison.CurrentCultureIgnoreCase)).Any());
        }
    }
}
