using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DapperExtensions
{
    internal class SqlServerBuilder : ISqlBuilder
    {

        public string SchemaTableSql<T>(string returnFields)
        {
            var table = SqlServerCache.GetTableEntity<T>();
            string sql;
            if (returnFields == null)
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

        public string InsertIdentitySql<T>()
        {
            var table = SqlServerCache.GetTableEntity<T>();
            CommonUtil.CheckTableKey(table);
            return table.InsertIdentitySql;
        }

        public string UpdateSql<T>(string updateFields)
        {
            var table = SqlServerCache.GetTableEntity<T>();
            CommonUtil.CheckTableKey(table);
            if (string.IsNullOrEmpty(updateFields))
            {
                return table.UpdateSql;
            }
            return CommonUtil.CreateUpdateSql(table, updateFields, "[", "]");
        }

        public string UpdateByWhereSql<T>(string where, string updateFields)
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

        public string DeleteByIdSql<T>()
        {
            var table = SqlServerCache.GetTableEntity<T>();
            CommonUtil.CheckTableKey(table);
            return table.DeleteByIdSql;
        }

        public string DeleteByIdsSql<T>()
        {
            var table = SqlServerCache.GetTableEntity<T>();
            CommonUtil.CheckTableKey(table);
            return table.DeleteByIdsSql;
        }

        public string DeleteByWhereSql<T>(string where)
        {
            return DeleteAllSql<T>() + where;
        }

        public string DeleteAllSql<T>()
        {
            return SqlServerCache.GetTableEntity<T>().DeleteAllSql;
        }

        public string GetInsertIdSql()
        {
            return "SELECT @@IDENTITY";
        }

        public string GetAllSql<T>(string returnFields, string orderBy)
        {
            var table = SqlServerCache.GetTableEntity<T>();
            if (string.IsNullOrEmpty(returnFields))
            {
                return table.GetAllSql + orderBy;
            }
            else
            {
                return string.Format("SELECT {0} FROM [{1}] WITH(NOLOCK) {3}" + returnFields, table.TableName, orderBy);
            }
        }

        public string GetByIdSql<T>(string returnFields)
        {
            var table = SqlServerCache.GetTableEntity<T>();
            CommonUtil.CheckTableKey(table);
            if (string.IsNullOrEmpty(returnFields))
                return table.GetByIdSql;
            else
                return string.Format("SELECT {0} FROM [{1}] WITH(NOLOCK) WHERE [{2}]=@id", returnFields, table.TableName, table.KeyName);
        }

        public string GetByIdsSql<T>(string returnFields)
        {
            var table = SqlServerCache.GetTableEntity<T>();
            CommonUtil.CheckTableKey(table);
            if (string.IsNullOrEmpty(returnFields))
                return table.GetByIdsSql;
            else
                return string.Format("SELECT {0} FROM [{1}] WITH(NOLOCK) WHERE [{2}] IN @ids", returnFields, table.TableName, table.KeyName);

        }
    }
}
