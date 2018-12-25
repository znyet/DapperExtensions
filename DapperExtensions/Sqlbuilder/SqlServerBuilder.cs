using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DapperExtensions
{
    public class SqlServerBuilder : ISqlBuilder
    {

        public string SchemaTable<T>(string returnFields)
        {
            var table = SqlServerCache.GetTableEntity<T>();
            string sql;
            if (returnFields == "*" || string.IsNullOrEmpty(returnFields))
            {
                sql = string.Format("SELECT TOP 0 {0} FROM [{1}]", table.AllFields, table.TableName);
            }
            else
            {
                sql = string.Format("SELECT TOP 0 {0} FROM [{1}]", returnFields, table.TableName);
            }
            return sql;
        }

        public string InsertSql<T>()
        {
            return SqlServerCache.GetTableEntity<T>().InsertSql;
        }

        public string InsertWithKeySql<T>()
        {
            var table = SqlServerCache.GetTableEntity<T>();
            CommonUtil.CheckTableKey(table);
            return table.InsertIdentityKeySql;
        }

        public string UpdateSql<T>(string updateFields)
        {
            var table = SqlServerCache.GetTableEntity<T>();
            CommonUtil.CheckTableKey(table);
            if (string.IsNullOrEmpty(updateFields) || updateFields == "*")
            {
                return table.UpdateSql;
            }
            return CommonUtil.CreateUpdateSql(table, updateFields, "[", "]");
        }

        public string UpdateByWhere<T>(string where, string updateFields)
        {
            var table = SqlServerCache.GetTableEntity<T>();
            return CommonUtil.CreateUpdateByWhereSql(table, where, updateFields, "[", "]");
        }

        public string ExistsKeySql<T>()
        {
            var table = SqlServerCache.GetTableEntity<T>();
            CommonUtil.CheckTableKey(table);
            return string.Format("SELECT COUNT(1) FROM [{0}] WITH(NOLOCK) WHERE [{1}]=@{1}", table.TableName, table.KeyName);
        }

        public string DeleteById<T>()
        {
            return SqlServerCache.GetTableEntity<T>().DeleteByIdSql;
        }

        public string DeleteByIds<T>()
        {
            return SqlServerCache.GetTableEntity<T>().DeleteByIdsSql;
        }

        public string DeleteAllSql<T>()
        {
            return SqlServerCache.GetTableEntity<T>().DeleteAllSql;
        }
    }
}
