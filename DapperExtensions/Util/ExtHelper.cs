using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DapperExtensions
{
    public static class ExtHelper
    {
        public static string ToUpperCase(this string txt)
        {
            return txt.Substring(0, 1).ToUpper() + txt.Substring(1);
        }

        public static string ToLowerCase(this string txt)
        {
            return txt.Substring(0, 1).ToLower() + txt.Substring(1);
        }
    }
}
