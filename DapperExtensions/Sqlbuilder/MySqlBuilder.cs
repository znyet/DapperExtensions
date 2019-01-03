using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DapperExtensions
{
    internal class MySqlBuilder : ISqlBuilder
    {

        #region common

        private static void InitPage(StringBuilder sb, TableEntity table, int skip, int take, string where, string returnFields, string orderBy)
        {
            if (string.IsNullOrEmpty(returnFields))
                returnFields = table.AllFields;

            if (string.IsNullOrEmpty(orderBy))
            {
                if (!string.IsNullOrEmpty(table.KeyName))
                {
                    orderBy = string.Format("ORDER BY `{0}`", table.KeyName);
                }
                else
                {
                    orderBy = string.Format("ORDER BY `{0}`", table.AllFieldList.First());
                }
            }

            sb.AppendFormat("SELECT {0} FROM `{1}` {2} {3} LIMIT {4},{5}", returnFields, table.TableName, where, orderBy, skip, take);

        }

        #endregion

        public string GetSchemaTableSql<T>(string returnFields)
        {
            var table = MySqlCache.GetTableEntity<T>();
            if (string.IsNullOrEmpty(returnFields))
                return string.Format("SELECT {0} FROM `{1}` LIMIT 0", table.AllFields, table.TableName);
            else
                return string.Format("SELECT {0} FROM `{1}` LIMIT 0", returnFields, table.TableName);
        }

        public string GetInsertSql<T>()
        {
            return MySqlCache.GetTableEntity<T>().InsertSql;
        }

        public string GetInsertReturnIdSql<T>(string sequence = null)
        {
            return MySqlCache.GetTableEntity<T>().InsertReturnIdSql;
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

        //public string GetIdentitySql()
        //{
        //    return "SELECT @@IDENTITY";
        //}

        public string GetSequenceCurrentSql(string sequence)
        {
            return "SELECT LAST_INSERT_ID()";
        }

        public string GetSequenceNextSql(string sequence)
        {
            return "SELECT @@IDENTITY";
        }

        public string GetTotalSql<T>(string where)
        {
            var table = MySqlCache.GetTableEntity<T>();
            return string.Format("SELECT COUNT(1) FROM `{0}` {1}", table.TableName, where);
        }

        public string GetAllSql<T>(string returnFields, string orderBy)
        {
            var table = MySqlCache.GetTableEntity<T>();
            if (string.IsNullOrEmpty(returnFields))
                return table.GetAllSql + orderBy;
            else
                return string.Format("SELECT {0} FROM `{1}` {2}", returnFields, table.TableName, orderBy);
        }

        public string GetByIdSql<T>(string returnFields)
        {
            var table = MySqlCache.GetTableEntity<T>();
            CommonUtil.CheckTableKey(table);
            if (string.IsNullOrEmpty(returnFields))
                return table.GetByIdSql;
            else
                return string.Format("SELECT {0} FROM `{1}` WHERE `{2}`=@id", returnFields, table.TableName, table.KeyName);
        }

        public string GetByIdsSql<T>(string returnFields)
        {
            var table = MySqlCache.GetTableEntity<T>();
            CommonUtil.CheckTableKey(table);
            if (string.IsNullOrEmpty(returnFields))
                return table.GetByIdsSql;
            else
                return string.Format("SELECT {0} FROM `{1}` WHERE `{2}` IN @ids", returnFields, table.TableName, table.KeyName);
        }

        public string GetByIdsWithFieldSql<T>(string field, string returnFields)
        {
            var table = MySqlCache.GetTableEntity<T>();
            if (string.IsNullOrEmpty(returnFields))
                returnFields = table.AllFields;
            return string.Format("SELECT {0} FROM `{1}` WHERE `{2}` IN @ids", returnFields, table.TableName, field);
        }

        public string GetByWhereSql<T>(string where, string returnFields, string orderBy)
        {
            var table = MySqlCache.GetTableEntity<T>();
            if (string.IsNullOrEmpty(returnFields))
                returnFields = table.AllFields;
            return string.Format("SELECT {0} FROM `{1}` {2} {3}", returnFields, table.TableName, where, orderBy);
        }

        public string GetByWhereFirstSql<T>(string where, string returnFields)
        {
            var table = MySqlCache.GetTableEntity<T>();
            if (string.IsNullOrEmpty(returnFields))
                returnFields = table.AllFields;
            return string.Format("SELECT {0} FROM `{1}` {2} LIMIT 1", returnFields, table.TableName, where);
        }

        public string GetBySkipTakeSql<T>(int skip, int take, string where, string returnFields, string orderBy)
        {
            var table = MySqlCache.GetTableEntity<T>();
            StringBuilder sb = new StringBuilder();
            InitPage(sb, table, skip, take, where, returnFields, orderBy);
            return sb.ToString();
        }

        public string GetByPageIndexSql<T>(int pageIndex, int pageSize, string where, string returnFields, string orderBy)
        {
            int skip = 0;
            if (pageIndex > 0)
            {
                skip = (pageIndex - 1) * pageSize;
            }
            return GetBySkipTakeSql<T>(skip, pageSize, where, returnFields, orderBy);
        }

        public string GetPageSql<T>(int pageIndex, int pageSize, string where, string returnFields, string orderBy)
        {
            int skip = 0;
            if (pageIndex > 0)
            {
                skip = (pageIndex - 1) * pageSize;
            }
            var table = MySqlCache.GetTableEntity<T>();
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("SELECT COUNT(1) FROM `{0}` {1};", table.TableName, where);
            InitPage(sb, table, skip, pageSize, where, returnFields, orderBy);
            return sb.ToString();
        }

    }
}
