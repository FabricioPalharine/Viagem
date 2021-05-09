using System;
using System.Collections.Generic;
using System.Text;

namespace CV.Mobile.Validations
{
    public class StringLengthRule<T> : IValidationRule<T>
    {
        public string ValidationMessage { get; set; }
        public int? MinLength { get; set; }
        public bool Check(T value)
        {
            if (value == null)
            {
                return true;
            }

            var str = value as string;

            return (!MinLength.HasValue || str.Length >= MinLength.Value);
        }
    }
}
