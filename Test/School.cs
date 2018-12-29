using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using DapperExtensions;

namespace Test
{
    [Table(TableName = "School", KeyName = "Id", IsIdentity = false)]
    public class School
    {
        public string Id { get; set; }

        public string Name { get; set; }

        [Igore]
        public string Address { get; set; }


    }
}