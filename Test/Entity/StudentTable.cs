using System;
using DapperExtensions;
//using System.Dynamic;

namespace Test
{
    /// <summary>
    /// 
    /// </summary>
    [Table(TableName = "Student", KeyName = "Id", IsIdentity = false)]
    public class StudentTable
    {

        /// <summary>
        /// Descript: 
        /// DbType: nvarchar,50,0
        /// AllowNull: 0
        /// Defaultval: 
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Descript: 
        /// DbType: nvarchar,50,0
        /// AllowNull: 1
        /// Defaultval: 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Descript: 
        /// DbType: int,10,0
        /// AllowNull: 1
        /// Defaultval: 
        /// </summary>
        public int Sex { get; set; }

        //[Igore]
        //public dynamic OtherData = new ExpandoObject();

    }

}