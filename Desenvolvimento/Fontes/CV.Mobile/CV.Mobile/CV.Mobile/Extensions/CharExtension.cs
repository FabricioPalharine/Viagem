using System;
using System.Collections.Generic;
using System.Text;

namespace CV.Mobile.Extensions
{
    public static class CharExtension
    {
        public static bool IsHex(this char c)
        {
            return (c >= '0' && c <= '9') ||
                     (c >= 'a' && c <= 'f') ||
                     (c >= 'A' && c <= 'F');
        }
    }
}
