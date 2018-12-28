using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using DapperExtensions;

namespace Test
{
    [Table(TableName = "People", KeyName = "Id", IsIdentity = true)]
    public class People
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public DateTime AddTime { get; set; }

        [Igore]
        public int Sex { get; set; }

        [Igore]
        public dynamic OtherData { get; set; }
       
    }
}