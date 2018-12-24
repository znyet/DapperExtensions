using System;

namespace DapperExtensions
{
    /// <summary>
    /// 忽略列(非数据库字段)
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class IgoreAttribute : Attribute
    {

    }
}
