using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using DapperExtensions;

namespace Test
{
    [Table(TableName = "Student", KeyName = "", IsIdentity = false)]
    public class Student
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }
}