using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DapperExtensions
{
    internal class OracleBuilder : ISqlBuilder
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
                    orderBy = string.Format("ORDER BY \"{0}\"", table.KeyName);
                }
                else
                {
                    orderBy = string.Format("ORDER BY \"{0}\"", table.AllFieldList.First());
                }
            }
            sb.AppendFormat("SELECT * FROM(SELECT A.*,ROWNUM RN FROM (SELECT {0} FROM \"{1}\" {2} {3}) A  WHERE ROWNUM <= {4}) WHERE RN > {5}", returnFields, table.TableName, where, orderBy, skip + take, skip);
        }

        #endregion

        public string GetSchemaTableSql<T>(string returnFields)
        {
            var table = OracleCache.GetTableEntity<T>();
            if (string.IsNullOrEmpty(returnFields))
                return string.Format("SELECT {0} FROM \"{1}\" WHERE rownum=0", table.AllFields, table.TableName);
            else
                return string.Format("SELECT {0} FROM \"{1}\" WHERE rownum=0", returnFields, table.TableName);
        }

        public string GetInsertSql<T>()
        {
            return OracleCache.GetTableEntity<T>().InsertSql;
        }

        public string GetInsertReturnIdSql<T>(string sequence = null)
        {
            if (string.IsNullOrEmpty(sequence))
                throw new Exception("oracle [sequence] can't no be null or empty");
            return (OracleCache.GetTableEntity<T>().InsertReturnIdSql).Replace("```seq```", sequence);
        }

        public string GetInsertIdentitySql<T>()
        {
            var table = OracleCache.GetTableEntity<T>();
            CommonUtil.CheckTableKey(table);
            return table.InsertIdentitySql;
        }

        public string GetUpdateSql<T>(string updateFields)
        {
            var table = OracleCache.GetTableEntity<T>();
            CommonUtil.CheckTableKey(table);
            if (string.IsNullOrEmpty(updateFields))
            {
                return table.UpdateSql;
            }
            return CommonUtil.CreateUpdateSql(table, updateFields, "\"", "\"",":");
        }

        public string GetUpdateByWhereSql<T>(string where, string updateFields)
        {
            var table = OracleCache.GetTableEntity<T>();
            return CommonUtil.CreateUpdateByWhereSql(table, where, updateFields, "\"", "\"",":");
        }

        public string GetExistsKeySql<T>()
        {
            var table = OracleCache.GetTableEntity<T>();
            CommonUtil.CheckTableKey(table);
            return string.Format("SELECT COUNT(1) FROM \"{0}\" WHERE \"{1}\"=:{1}", table.TableName, table.KeyName);
        }

        public string GetDeleteByIdSql<T>()
        {
            var table = OracleCache.GetTableEntity<T>();
            CommonUtil.CheckTableKey(table);
            return table.DeleteByIdSql;
        }

        public string GetDeleteByIdsSql<T>()
        {
            var table = OracleCache.GetTableEntity<T>();
            CommonUtil.CheckTableKey(table);
            return table.DeleteByIdsSql;
        }

        public string GetDeleteByWhereSql<T>(string where)
        {
            return GetDeleteAllSql<T>() + where;
        }

        public string GetDeleteAllSql<T>()
        {
            return OracleCache.GetTableEntity<T>().DeleteAllSql;
        }

        public string GetIdentitySql()
        {
            throw new Exception("for oracle please use [GetSequenceNext] or [GetSequenceCurrent] or use [InsertReturnId]");
        }

        public string GetSequenceCurrentSql(string sequence)
        {
            return string.Format("SELECT {0}.CURRVAL FROM DUAL", sequence);
        }

        public string GetSequenceNextSql(string sequence)
        {
            return string.Format("SELECT {0}.NEXTVAL FROM DUAL", sequence);
        }

        public string GetTotalSql<T>(string where)
        {
            var table = OracleCache.GetTableEntity<T>();
            return string.Format("SELECT COUNT(1) FROM \"{0}\" {1}", table.TableName, where);
        }

        public string GetAllSql<T>(string returnFields, string orderBy)
        {
            var table = OracleCache.GetTableEntity<T>();
            if (string.IsNullOrEmpty(returnFields))
                return table.GetAllSql + orderBy;
            else
                return string.Format("SELECT {0} FROM \"{1}\" {2}", returnFields, table.TableName, orderBy);
        }

        public string GetByIdSql<T>(string returnFields)
        {
            var table = OracleCache.GetTableEntity<T>();
            CommonUtil.CheckTableKey(table);
            if (string.IsNullOrEmpty(returnFields))
                return table.GetByIdSql;
            else
                return string.Format("SELECT {0} FROM \"{1}\" WHERE \"{2}\"=:id", returnFields, table.TableName, table.KeyName);
        }

        public string GetByIdsSql<T>(string returnFields)
        {
            var table = OracleCache.GetTableEntity<T>();
            CommonUtil.CheckTableKey(table);
            if (string.IsNullOrEmpty(returnFields))
                return table.GetByIdsSql;
            else
                return string.Format("SELECT {0} FROM \"{1}\" WHERE \"{2}\" IN :ids", returnFields, table.TableName, table.KeyName);
        }

        public string GetByIdsWithFieldSql<T>(string field, string returnFields)
        {
            var table = OracleCache.GetTableEntity<T>();
            if (string.IsNullOrEmpty(returnFields))
                returnFields = table.AllFields;
            return string.Format("SELECT {0} FROM \"{1}\" WHERE \"{2}\" IN :ids", returnFields, table.TableName, field);
        }

        public string GetByWhereSql<T>(string where, string returnFields, string orderBy)
        {
            var table = OracleCache.GetTableEntity<T>();
            if (string.IsNullOrEmpty(returnFields))
                returnFields = table.AllFields;
            return string.Format("SELECT {0} FROM \"{1}\" {2} {3}", returnFields, table.TableName, where, orderBy);
        }

        public string GetByWhereFirstSql<T>(string where, string returnFields)
        {
            var table = OracleCache.GetTableEntity<T>();
            if (string.IsNullOrEmpty(returnFields))
                returnFields = table.AllFields;
            if (!string.IsNullOrEmpty(where))
                where += " AND rownum=1";
            else
                where = "WHERE rownum=1";
            return string.Format("SELECT {0} FROM \"{1}\" {2}", returnFields, table.TableName, where);
        }

        public string GetBySkipTakeSql<T>(int skip, int take, string where, string returnFields, string orderBy)
        {
            var table = OracleCache.GetTableEntity<T>();
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
            throw new Exception("for oracle please use [GetPageForOracle]");
        }
    }
}
