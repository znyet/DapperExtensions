using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator
{
    public class ColumnEntity
    {
        public string Name { get; set; }
        public string NameUpper { get; set; }
        public string NameLower { get; set; }
        public string CsType { get; set; }
        public string JavaType { get; set; }
        public string Comment { get; set; }
        public string DbType { get; set; }
        public string AllowNull { get; set; }
        public string DefaultValue { get; set; }
    }
}
