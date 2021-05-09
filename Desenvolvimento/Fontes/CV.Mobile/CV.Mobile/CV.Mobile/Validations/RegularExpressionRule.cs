using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace CV.Mobile.Validations
{
    public class RegularExpressionRule<T> : IValidationRule<T>
    {
        public string ValidationMessage { get; set; }
        public string RegularExpression { get; set; }
        public bool Check(T value)
        {
            if (value == null || string.IsNullOrEmpty(Convert.ToString(value)))
            {
                return true;
            }

            var str = value as string;

            return string.IsNullOrEmpty(RegularExpression) || Regex.IsMatch(str,RegularExpression);
        }
    }
}
