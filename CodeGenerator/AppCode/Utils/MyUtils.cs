using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator
{
    public class MyUtils
    {
        public static string ToUpper(string txt)
        {
            return txt.Substring(0, 1).ToUpper() + txt.Substring(1);
        }

        public static string ToLower(string txt)
        {
            return txt.Substring(0, 1).ToLower() + txt.Substring(1);
        }
    }
}
