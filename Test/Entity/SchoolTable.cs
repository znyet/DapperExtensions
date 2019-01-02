using System;
using DapperExtensions;
//using System.Dynamic;

namespace Test
{
    /// <summary>
    /// 
    /// </summary>
    [Table(TableName = "School", KeyName = "Id", IsIdentity = true)]
    public class SchoolTable
    {

        /// <summary>
        /// Descript: 
        /// DbType: int,10,0
        /// AllowNull: 0
        /// Defaultval: 
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Descript: 
        /// DbType: nvarchar,50,0
        /// AllowNull: 1
        /// Defaultval: 
        /// </summary>
        public string Name { get; set; }

        //[Igore]
        //public dynamic OtherData = new ExpandoObject();

    }

}