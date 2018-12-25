using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Dapper;
using System.Data;
using System.Data.SqlClient;


namespace DapperExtensions
{
    public static partial class DapperExtension
    {

        #region common method for ado.net


        public static DataTable GetDataTable(this IDbConnection conn, string sql, object param = null, IDbTransaction tran = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            using (IDataReader reader = conn.ExecuteReader(sql, param, tran, commandTimeout, commandType))
            {
                DataTable dt = new DataTable();
                dt.Load(reader);
                return dt;
            }
        }

        public static DataSet GetDataSet(this IDbConnection conn, string sql, object param = null, IDbTransaction tran = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            using (IDataReader reader = conn.ExecuteReader(sql, param, tran, commandTimeout, commandType))
            {
                DataSet ds = new DataSet();
                int i = 0;
                while (!reader.IsClosed)
                {
                    i++;
                    DataTable dt = new DataTable();
                    dt.TableName = "T" + i;
                    dt.Load(reader);
                    ds.Tables.Add(dt);
                }
                return ds;
            }
        }

        public static DataTable GetSchemaTable<T>(this IDbConnection conn, string returnFields = "*", IDbTransaction tran = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            var builder = BuilderFactory.GetBuilder(conn);
            return GetDataTable(conn, builder.SchemaTable<T>(returnFields), null, tran, commandTimeout, commandType);
        }

        /// <summary>
        /// only sqlserver use BulkCopy
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="dt"></param>
        /// <param name="tableName"></param>
        /// <param name="copyFields"></param>
        /// <param name="insert_identity">false</param>
        /// <param name="tran"></param>
        /// <param name="batchSize">default 20000</param>
        /// <param name="timeOut">second default 100s</param>
        /// <returns>true means ok,if false you must rollback tran</returns>
        /// 
        public static bool BulkCopy(this IDbConnection conn, DataTable dt, string tableName, string copyFields = "*", bool insert_identity = false, IDbTransaction tran = null, int batchSize = 20000, int timeOut = 100)
        {
            if (conn.ToString() != "System.Data.SqlClient.SqlConnection")
            {
                throw new Exception("only sqlserver use BulkCopy");
            }
            bool isOk = true;
            SqlBulkCopyOptions option = SqlBulkCopyOptions.Default;
            if (insert_identity == true)
            {
                option = SqlBulkCopyOptions.KeepIdentity;
            }
            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(conn as SqlConnection, option, tran as SqlTransaction)) //SqlBulkCopyOptions.Default
            {
                bulkCopy.BatchSize = batchSize;
                bulkCopy.BulkCopyTimeout = timeOut;
                bulkCopy.DestinationTableName = tableName;
                try
                {
                    if (copyFields != "*" || !string.IsNullOrEmpty(copyFields))
                    {
                        foreach (var item in copyFields.Split(','))
                        {
                            bulkCopy.ColumnMappings.Add(item, item);
                        }
                    }
                    else
                    {
                        foreach (DataColumn col in dt.Columns)
                        {
                            bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                        }
                    }
                    bulkCopy.WriteToServer(dt);
                }
                catch (Exception ex)
                {
                    isOk = false;
                }

            }
            return isOk;
        }

        public static bool BulkCopy<T>(this IDbConnection conn, DataTable dt, string copyFields = "*", bool insert_identity = false, IDbTransaction tran = null, int batchSize = 20000, int timeOut = 100)
        {
            var table = SqlServerCache.GetTableEntity<T>();
            return BulkCopy(conn, dt, table.TableName, copyFields, insert_identity, tran, batchSize, timeOut);
        }

        #endregion

        #region method (Insert Update Delete)

        public static int Insert<T>(this IDbConnection conn, T model, IDbTransaction tran = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            var builder = BuilderFactory.GetBuilder(conn);
            return conn.Execute(builder.InsertSql<T>(), model, tran, commandTimeout, commandType);
        }

        /// <summary>
        /// for sqlserver insert identity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int InsertIdentity<T>(this IDbConnection conn, T model, IDbTransaction tran = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            var builder = BuilderFactory.GetBuilder(conn);
            return conn.Execute(builder.InsertIdentitySql<T>(), model, tran, commandTimeout, commandType);
        }

        public static int Update<T>(this IDbConnection conn, T model, string updateFields = null, IDbTransaction tran = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            var builder = BuilderFactory.GetBuilder(conn);
            return conn.Execute(builder.UpdateSql<T>(updateFields), model, tran, commandTimeout, commandType);
        }

        public static int UpdateByWhere<T>(this IDbConnection conn, string where, string updateFields, T model, IDbTransaction tran = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            var builder = BuilderFactory.GetBuilder(conn);
            return conn.Execute(builder.UpdateByWhere<T>(where, updateFields), model, tran, commandTimeout, commandType);
        }

        public static int InsertOrUpdate<T>(this IDbConnection conn, T model, string updateFields = null, bool update = true, IDbTransaction tran = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            var builder = BuilderFactory.GetBuilder(conn);
            int effectRow = 0;
            dynamic total = conn.ExecuteScalar<dynamic>(builder.ExistsKeySql<T>(), model, tran, commandTimeout, commandType);
            if (total > 0)
            {
                if (update)
                {
                    effectRow += Update(conn, model, updateFields, tran, commandTimeout, commandType);
                }
            }
            else
            {
                effectRow += Insert(conn, model, tran, commandTimeout, commandType);
            }

            return effectRow;
        }

        /// <summary>
        /// for sqlserver insert identity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="updateFields"></param>
        /// <param name="update"></param>
        /// <returns></returns>
        public static int InsertIdentityOrUpdate<T>(this IDbConnection conn, T model, string updateFields = null, bool update = true, IDbTransaction tran = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            var builder = BuilderFactory.GetBuilder(conn);
            int effectRow = 0;
            dynamic total = conn.ExecuteScalar<dynamic>(builder.ExistsKeySql<T>(), model, tran, commandTimeout, commandType);
            if (total > 0)
            {
                if (update)
                {
                    effectRow += Update(conn, model, updateFields, tran, commandTimeout, commandType);
                }
            }
            else
            {
                effectRow += InsertIdentity(conn, model, tran, commandTimeout, commandType);
            }

            return effectRow;
        }

        public static int Delete<T>(this IDbConnection conn, object id, IDbTransaction tran = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            var builder = BuilderFactory.GetBuilder(conn);
            return conn.Execute(builder.DeleteById<T>(), new { id = id }, tran, commandTimeout, commandType);
        }

        public static int DeleteByIds<T>(this IDbConnection conn, object ids, IDbTransaction tran = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            var builder = BuilderFactory.GetBuilder(conn);
            DynamicParameters dpar = new DynamicParameters();
            dpar.Add("@ids", ids);
            return conn.Execute(builder.DeleteByIds<T>(), dpar, tran, commandTimeout, commandType);
        }

        public static int DeleteByWhere<T>(this IDbConnection conn, string where, object param, IDbTransaction tran = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            var builder = BuilderFactory.GetBuilder(conn);
            return conn.Execute(builder.DeleteAllSql<T>() + where, param, tran, commandTimeout, commandType);
        }

        public static int DeleteAll<T>(this IDbConnection conn, IDbTransaction tran = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            var builder = BuilderFactory.GetBuilder(conn);
            return conn.Execute(builder.DeleteAllSql<T>(), null, tran, commandTimeout, commandType);
            
        }



        #endregion

        #region method (Query)

        public static IdType GetInsertId<IdType>()
        {
            return default(IdType);
        }

        public static IEnumerable<T> GetAll<T>(string returnFields = null, string orderBy = null)
        {
            return null;
        }

        public static IEnumerable<dynamic> GetAllDynamic<T>(string returnFields = null, string orderBy = null)
        {
            return null;
        }

        public static T GetById<T>(object id, string returnFields = null)
        {
            return default(T);
        }

        public static dynamic GetByIdDynamic<T>(object id, string returnFields = null)
        {
            return null;
        }

        public static IEnumerable<T> GetByIds<T>(object ids, string returnFields = null)
        {
            return null;
        }

        public static IEnumerable<dynamic> GetByIdsDynamic<T>(object ids, string returnFields = null)
        {
            return null;
        }

        public static IEnumerable<T> GetByWhere<T>(string where, object param = null, string returnFields = null)
        {
            return null;
        }

        public static IEnumerable<dynamic> GetByWhereDynamic<T>(string where, object param = null, string returnFields = null)
        {
            return null;
        }

        public static T GetByWhereFirst<T>(string where, object param = null, string returnFields = null)
        {
            return default(T);
        }

        public static dynamic GetByWhereFirstDynamic<T>(string where, object param = null, string returnFields = null)
        {
            return null;
        }

        public static dynamic GetTotal<T>(string where = null, object param = null)
        {
            return null;
        }

        public static IEnumerable<T> GetByIdsWithField<T>(object ids, string field, string returnFields = null)
        {
            return null;
        }

        public static IEnumerable<dynamic> GetByIdsWithFieldDynamic<T>(object ids, string field, string returnFields = null)
        {
            return null;
        }

        public static IEnumerable<T> GetBySkipTake<T>(int skip, int take, string where = null, object param = null, string returnFields = null, string orderBy = null)
        {
            return null;
        }

        public static IEnumerable<dynamic> GetBySkipTakeDynamic<T>(int skip, int take, string where = null, object param = null, string returnFields = null, string orderBy = null)
        {
            return null;
        }

        public static IEnumerable<T> GetByPageIndex<T>(int pageIndex, int pageSize, string where = null, object param = null, string returnFields = null, string orderBy = null)
        {
            return null;
        }

        public static IEnumerable<dynamic> GetByPageIndexDynamic<T>(int pageIndex, int pageSize, string where = null, object param = null, string returnFields = null, string orderBy = null)
        {
            return null;
        }

        public static PageEntity<T> GetPage<T>(int pageIndex, int pageSize, string where = null, object param = null, string returnFields = null, string orderBy = null)
        {
            return null;
        }

        public static PageEntity<dynamic> GetPageDynamic<T>(int pageIndex, int pageSize, string where = null, object param = null, string returnFields = null, string orderBy = null)
        {
            return null;
        }


        #endregion
    }
}
