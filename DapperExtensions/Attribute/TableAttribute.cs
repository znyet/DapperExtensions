using System;

namespace DapperExtensions
{
    /// <summary>
    /// TableName：表名称，KeyName：主键名称，IsIdentity：是否自增
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class TableAttribute : Attribute
    {
        public string TableName { get; set; }
        public string KeyName { get; set; }
        public bool IsIdentity { get; set; }
    }
}
