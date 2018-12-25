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

        public string InsertSql { get; set; }
        public string InsertIdentitySql { get; set; }
        public string GetByIdSql { get; set; }
        public string GetByIdsSql { get; set; }
        public string GetAllSql { get; set; }
        public string DeleteByIdSql { get; set; }
        public string DeleteByIdsSql { get; set; }
        public string DeleteAllSql { get; set; }
        public string UpdateSql { get; set; }
    }
}
