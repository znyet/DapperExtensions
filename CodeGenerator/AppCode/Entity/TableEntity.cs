using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator
{
    public class TableEntity
    {
        public string Name { get; set; }
        public string NameUpper { get; set; }
        public string NameLower { get; set; }
        public string Comment { get; set; }
        public string KeyName { get; set; }
        public string IsIdentity { get; set; } 
    }
}
