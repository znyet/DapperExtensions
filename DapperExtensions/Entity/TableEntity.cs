using System;
using System.Collections.Generic;

namespace DapperExtensions
{
    internal class TableEntity
    {
        public string TableName { get; set; }
        public string KeyName { get; set; } // 主键名称
        public Type KeyType { get; set; } //主键类型
        public bool IsIdentity { get; set; } //是否是自增键


        public List<string> AllFieldList { get; set; } //所有列
        public List<string> ExceptKeyFieldList { get; set; } //除主键列

        //保留主键
        public string AllFields { get; set; }//所有列逗号分隔[name],[sex]
        public string AllFieldsAt { get; set; } //@name,@sex
        public string AllFieldsAtEq { get; set; }//[name]=@name,[sex]=@sex

        //去除主键
        public string AllFieldsExceptKey { get; set; }//所有列逗号分隔[name],[sex]
        public string AllFieldsAtExceptKey { get; set; } //@name,@sex
        public string AllFieldsAtEqExceptKey { get; set; }//[name]=@name,[sex]=@sex

        public string InsertSql { get; set; } //添加记录,返回受影响行数
        public string InsertIdentitySql { get; set; } //添加有自增id表,返回受影响行数
        public string InsertReturnIdSql { get; set; } //添加记录返回自增id
        public string GetByIdSql { get; set; } //根据id获取记录
        public string GetByIdsSql { get; set; } //根据ids获取记录
        public string GetAllSql { get; set; } //获取所有记录
        public string DeleteByIdSql { get; set; } //根据id删除记录
        public string DeleteByIdsSql { get; set; } //根据ids删除记录
        public string DeleteAllSql { get; set; } //删除整张表的记录
        public string UpdateSql { get; set; } //修改记录
    }
}
