using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DapperExtensions
{
    internal class MySqlBuilder : ISqlBuilder
    {

        public string GetSchemaTableSql<T>(string returnFields)
        {
            var table = MySqlCache.GetTableEntity<T>();
            string sql;
            if (returnFields == null)
            {
                sql = string.Format("SELECT {0} FROM `{1}` LIMIT 0", table.AllFields, table.TableName);
            }
            else
            {
                sql = string.Format("SELECT {0} FROM `{1}` LIMIT 0", returnFields, table.TableName);
            }
            return sql;
        }

        public string GetInsertSql<T>()
        {
            return MySqlCache.GetTableEntity<T>().InsertSql;
        }

        public string GetInsertIdentitySql<T>()
        {
            var table = MySqlCache.GetTableEntity<T>();
            CommonUtil.CheckTableKey(table);
            return table.InsertIdentitySql;
        }

        public string GetUpdateSql<T>(string updateFields)
        {
            var table = MySqlCache.GetTableEntity<T>();
            CommonUtil.CheckTableKey(table);
            if (string.IsNullOrEmpty(updateFields))
            {
                return table.UpdateSql;
            }
            return CommonUtil.CreateUpdateSql(table, updateFields, "`", "`");
        }

        public string GetUpdateByWhereSql<T>(string where, string updateFields)
        {
            var table = MySqlCache.GetTableEntity<T>();
            return CommonUtil.CreateUpdateByWhereSql(table, where, updateFields, "`", "`");
        }

        public string GetExistsKeySql<T>()
        {
            throw new NotImplementedException();
        }

        public string GetDeleteByIdSql<T>()
        {
            throw new NotImplementedException();
        }

        public string GetDeleteByIdsSql<T>()
        {
            throw new NotImplementedException();
        }

        public string GetDeleteByWhereSql<T>(string where)
        {
            throw new NotImplementedException();
        }

        public string GetDeleteAllSql<T>()
        {
            throw new NotImplementedException();
        }

        public string GetInsertIdSql()
        {
            throw new NotImplementedException();
        }

        public string GetTotalSql<T>(string where)
        {
            throw new NotImplementedException();
        }

        public string GetAllSql<T>(string returnFields, string orderBy)
        {
            throw new NotImplementedException();
        }

        public string GetByIdSql<T>(string returnFields)
        {
            throw new NotImplementedException();
        }

        public string GetByIdsSql<T>(string returnFields)
        {
            throw new NotImplementedException();
        }

        public string GetByIdsWithFieldSql<T>(string field, string returnFields)
        {
            throw new NotImplementedException();
        }

        public string GetByWhereSql<T>(string where, string returnFields, string orderBy)
        {
            throw new NotImplementedException();
        }

        public string GetByWhereFirstSql<T>(string where, string returnFields)
        {
            throw new NotImplementedException();
        }

        public string GetBySkipTakeSql<T>(int skip, int take, string where, string returnFields, string orderBy)
        {
            throw new NotImplementedException();
        }

        public string GetByPageIndexSql<T>(int pageIndex, int pageSize, string where, string returnFields, string orderBy)
        {
            throw new NotImplementedException();
        }

        public string GetPageSql<T>(int pageIndex, int pageSize, string where, string returnFields, string orderBy)
        {
            throw new NotImplementedException();
        }
    }
}
