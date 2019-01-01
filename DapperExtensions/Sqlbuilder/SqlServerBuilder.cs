using System;
using System.Linq;
using System.Text;

namespace DapperExtensions
{
    internal class SqlServerBuilder : ISqlBuilder
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
                    orderBy = string.Format("ORDER BY [{0}]", table.KeyName);
                }
                else
                {
                    orderBy = string.Format("ORDER BY [{0}]", table.AllFieldList.First());
                }
            }

            if (skip == 0) //第一页,使用Top语句
            {
                sb.AppendFormat("SELECT TOP ({0}) {1} FROM [{2}] WITH(NOLOCK) {3} {4}", take, returnFields, table.TableName, where, orderBy);
            }
            else //使用ROW_NUMBER()
            {
                sb.AppendFormat("WITH cte AS(SELECT ROW_NUMBER() OVER({0}) AS RowNum,{1} FROM [{2}] WITH(NOLOCK) {3})", orderBy, returnFields, table.TableName, where);
                if (returnFields.Contains(" AS") || returnFields.Contains(" as"))
                {
                    sb.AppendFormat("SELECT * FROM cte WHERE cte.RowNum BETWEEN {0} AND {2}", skip + 1, skip + take);
                }
                else
                {
                    sb.AppendFormat("SELECT {0} FROM cte WHERE cte.RowNum BETWEEN {1} AND {2}", returnFields, skip + 1, skip + take);
                }
            }

        }

        #endregion

        public string GetSchemaTableSql<T>(string returnFields)
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

        public string GetInsertSql<T>()
        {
            return SqlServerCache.GetTableEntity<T>().InsertSql;
        }

        public string GetInsertReturnIdSql<T>(string sequence = null)
        {
            return SqlServerCache.GetTableEntity<T>().InsertSql + ";SELECT @@IDENTITY";
        }

        public string GetInsertIdentitySql<T>()
        {
            var table = SqlServerCache.GetTableEntity<T>();
            CommonUtil.CheckTableKey(table);
            return table.InsertIdentitySql;
        }

        public string GetUpdateSql<T>(string updateFields)
        {
            var table = SqlServerCache.GetTableEntity<T>();
            CommonUtil.CheckTableKey(table);
            if (string.IsNullOrEmpty(updateFields))
            {
                return table.UpdateSql;
            }
            return CommonUtil.CreateUpdateSql(table, updateFields, "[", "]");
        }

        public string GetUpdateByWhereSql<T>(string where, string updateFields)
        {
            var table = SqlServerCache.GetTableEntity<T>();
            return CommonUtil.CreateUpdateByWhereSql(table, where, updateFields, "[", "]");
        }

        public string GetExistsKeySql<T>()
        {
            var table = SqlServerCache.GetTableEntity<T>();
            CommonUtil.CheckTableKey(table);
            return string.Format("SELECT COUNT(1) FROM [{0}] WITH(NOLOCK) WHERE [{1}]=@{1}", table.TableName, table.KeyName);
        }

        public string GetDeleteByIdSql<T>()
        {
            var table = SqlServerCache.GetTableEntity<T>();
            CommonUtil.CheckTableKey(table);
            return table.DeleteByIdSql;
        }

        public string GetDeleteByIdsSql<T>()
        {
            var table = SqlServerCache.GetTableEntity<T>();
            CommonUtil.CheckTableKey(table);
            return table.DeleteByIdsSql;
        }

        public string GetDeleteByWhereSql<T>(string where)
        {
            return GetDeleteAllSql<T>() + where;
        }

        public string GetDeleteAllSql<T>()
        {
            return SqlServerCache.GetTableEntity<T>().DeleteAllSql;
        }

        public string GetIdentitySql()
        {
            return "SELECT @@IDENTITY";
        }


        public string GetIdentityCurrentSql(string sequence, string dual = "DUAL")
        {
            return "SELECT @@IDENTITY";
        }

        public string GetIdentityNextSql(string sequence, string dual = "DUAL")
        {
            return "SELECT @@IDENTITY";
        }

        public string GetTotalSql<T>(string where)
        {
            var table = SqlServerCache.GetTableEntity<T>();
            return string.Format("SELECT COUNT(1) FROM [{0}] WITH(NOLOCK) {1}", table.TableName, where);
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
                return string.Format("SELECT {0} FROM [{1}] WITH(NOLOCK) {2}" + returnFields, table.TableName, orderBy);
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

        public string GetByIdsWithFieldSql<T>(string field, string returnFields)
        {
            var table = SqlServerCache.GetTableEntity<T>();
            CommonUtil.CheckTableKey(table);
            if (string.IsNullOrEmpty(returnFields))
                returnFields = table.AllFields;
            return string.Format("SELECT {0} FROM [{1}] WITH(NOLOCK) WHERE [{2}] IN @ids", returnFields, table.TableName, field);
        }

        public string GetByWhereSql<T>(string where, string returnFields, string orderBy)
        {
            var table = SqlServerCache.GetTableEntity<T>();
            if (string.IsNullOrEmpty(returnFields))
                returnFields = table.AllFields;
            return string.Format("SELECT {0} FROM [{1}] WITH(NOLOCK) {2} {3}", returnFields, table.TableName, where, orderBy);
        }

        public string GetByWhereFirstSql<T>(string where, string returnFields)
        {
            var table = SqlServerCache.GetTableEntity<T>();
            if (string.IsNullOrEmpty(returnFields))
                returnFields = table.AllFields;
            return string.Format("SELECT {0} FROM [{1}] WITH(NOLOCK) {2}", returnFields, table.TableName, where);
        }

        public string GetBySkipTakeSql<T>(int skip, int take, string where, string returnFields, string orderBy)
        {
            var table = SqlServerCache.GetTableEntity<T>();
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
            var table = SqlServerCache.GetTableEntity<T>();
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("DECLARE @total INT;SELECT @total = COUNT(1) FROM [{0}] WITH(NOLOCK) {1};SELECT @total;", table.TableName, where);
            sb.Append("IF(@total>0) BEGIN ");
            InitPage(sb, table, skip, pageSize, where, returnFields, orderBy);
            sb.Append(" END");

            throw new NotImplementedException();
        }
    
    }
}
