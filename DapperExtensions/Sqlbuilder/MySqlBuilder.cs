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

        public string GetInsertReturnIdSql<T>(string sequence = null)
        {
            return MySqlCache.GetTableEntity<T>().InsertSql + ";SELECT @@IDENTITY";
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
            var table = MySqlCache.GetTableEntity<T>();
            CommonUtil.CheckTableKey(table);
            return string.Format("SELECT COUNT(1) FROM `{0}` WHERE `{1}`=@{1}", table.TableName, table.KeyName);
        }

        public string GetDeleteByIdSql<T>()
        {
            var table = MySqlCache.GetTableEntity<T>();
            CommonUtil.CheckTableKey(table);
            return table.DeleteByIdSql;
        }

        public string GetDeleteByIdsSql<T>()
        {
            var table = MySqlCache.GetTableEntity<T>();
            CommonUtil.CheckTableKey(table);
            return table.DeleteByIdsSql;
        }

        public string GetDeleteByWhereSql<T>(string where)
        {
            return GetDeleteAllSql<T>() + where;
        }

        public string GetDeleteAllSql<T>()
        {
            return MySqlCache.GetTableEntity<T>().DeleteAllSql;
        }

        public string GetIdentitySql()
        {
            return "SELECT @@IDENTITY";
        }

        public string GetIdentityCurrentSql(string sequence, string dual = "DUAL")
        {
            return "SELECT LAST_INSERT_ID()";
        }

        public string GetIdentityNextSql(string sequence, string dual = "DUAL")
        {
            return "SELECT @@IDENTITY";
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
