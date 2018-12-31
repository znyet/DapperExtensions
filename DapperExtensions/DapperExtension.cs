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

        public static DataTable GetSchemaTable<T>(this IDbConnection conn, string returnFields = null, IDbTransaction tran = null, int? commandTimeout = null)
        {
            var builder = BuilderFactory.GetBuilder(conn);
            return GetDataTable(conn, builder.GetSchemaTableSql<T>(returnFields), null, tran, commandTimeout);
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
        /// <returns></returns>
        public static void BulkCopy(this IDbConnection conn, IDbTransaction tran, DataTable dt, string tableName, string copyFields = null, bool insert_identity = false, int batchSize = 20000, int timeout = 100)
        {
            if (conn.ToString() != "System.Data.SqlClient.SqlConnection")
            {
                throw new Exception("only sqlserver can use BulkCopy");
            }
            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlBulkCopyOptions option = SqlBulkCopyOptions.Default;
            if (insert_identity)
            {
                option = SqlBulkCopyOptions.KeepIdentity;
            }
            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(conn as SqlConnection, option, tran as SqlTransaction)) //SqlBulkCopyOptions.Default
            {
                bulkCopy.BatchSize = batchSize;
                bulkCopy.BulkCopyTimeout = timeout;
                bulkCopy.DestinationTableName = tableName;

                if (!string.IsNullOrEmpty(copyFields))
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
        }

        public static void BulkCopy<T>(this IDbConnection conn, IDbTransaction tran, DataTable dt, string copyFields = null, bool insert_identity = false, int batchSize = 20000, int timeout = 100)
        {
            var table = SqlServerCache.GetTableEntity<T>();
            BulkCopy(conn, tran, dt, table.TableName, copyFields, insert_identity, batchSize, timeout);
        }

        public static void BulkUpdate(this IDbConnection conn, IDbTransaction tran, DataTable dt, string tableName, string column = "*", int batchSize = 20000, int timeout = 100)
        {
            if (conn.ToString() != "System.Data.SqlClient.SqlConnection")
            {
                throw new Exception("only sqlserver can use BulkUpdate");
            }
            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlConnection cnn = conn as SqlConnection;
            SqlCommand comm = cnn.CreateCommand();
            comm.CommandTimeout = timeout;
            comm.CommandType = CommandType.Text;
            SqlDataAdapter adapter = new SqlDataAdapter(comm);
            SqlCommandBuilder commandBulider = new SqlCommandBuilder(adapter);
            commandBulider.ConflictOption = ConflictOption.OverwriteChanges;
            try
            {
                adapter.UpdateBatchSize = batchSize;
                adapter.SelectCommand.Transaction = tran as SqlTransaction;
                adapter.SelectCommand.CommandText = "SELECT TOP 0 " + column + " FROM " + tableName;
                adapter.Update(dt.GetChanges());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                comm.Dispose();
                adapter.Dispose();
            }
        }

        public static void BulkUpdate<T>(this IDbConnection conn, IDbTransaction tran, DataTable dt, string column = "*", int batchSize = 20000, int timeOut = 100)
        {
            var table = SqlServerCache.GetTableEntity<T>();
            BulkUpdate(conn, tran, dt, table.TableName, column, batchSize, timeOut);
        }

        #endregion

        #region method (Insert Update Delete)

        public static int Insert<T>(this IDbConnection conn, T model, IDbTransaction tran = null, int? commandTimeout = null)
        {
            var builder = BuilderFactory.GetBuilder(conn);
            return conn.Execute(builder.GetInsertSql<T>(), model, tran, commandTimeout);
        }

        /// <summary>
        /// for sqlserver insert identity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int InsertIdentity<T>(this IDbConnection conn, T model, IDbTransaction tran = null, int? commandTimeout = null)
        {
            var builder = BuilderFactory.GetBuilder(conn);
            return conn.Execute(builder.GetInsertIdentitySql<T>(), model, tran, commandTimeout);
        }

        public static int Update<T>(this IDbConnection conn, T model, string updateFields = null, IDbTransaction tran = null, int? commandTimeout = null)
        {
            var builder = BuilderFactory.GetBuilder(conn);
            return conn.Execute(builder.GetUpdateSql<T>(updateFields), model, tran, commandTimeout);
        }

        public static int UpdateByWhere<T>(this IDbConnection conn, string where, string updateFields, T model, IDbTransaction tran = null, int? commandTimeout = null)
        {
            var builder = BuilderFactory.GetBuilder(conn);
            return conn.Execute(builder.GetUpdateByWhereSql<T>(where, updateFields), model, tran, commandTimeout);
        }

        public static int InsertOrUpdate<T>(this IDbConnection conn, T model, string updateFields = null, bool update = true, IDbTransaction tran = null, int? commandTimeout = null)
        {
            var builder = BuilderFactory.GetBuilder(conn);
            int effectRow = 0;
            dynamic total = conn.ExecuteScalar<dynamic>(builder.GetExistsKeySql<T>(), model, tran, commandTimeout);
            if (total > 0)
            {
                if (update)
                {
                    effectRow += Update(conn, model, updateFields, tran, commandTimeout);
                }
            }
            else
            {
                effectRow += Insert(conn, model, tran, commandTimeout);
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
        public static int InsertIdentityOrUpdate<T>(this IDbConnection conn, T model, string updateFields = null, bool update = true, IDbTransaction tran = null, int? commandTimeout = null)
        {
            var builder = BuilderFactory.GetBuilder(conn);
            int effectRow = 0;
            dynamic total = conn.ExecuteScalar<dynamic>(builder.GetExistsKeySql<T>(), model, tran, commandTimeout);
            if (total > 0)
            {
                if (update)
                {
                    effectRow += Update(conn, model, updateFields, tran, commandTimeout);
                }
            }
            else
            {
                effectRow += InsertIdentity(conn, model, tran, commandTimeout);
            }

            return effectRow;
        }

        public static int Delete<T>(this IDbConnection conn, object id, IDbTransaction tran = null, int? commandTimeout = null)
        {
            var builder = BuilderFactory.GetBuilder(conn);
            return conn.Execute(builder.GetDeleteByIdSql<T>(), new { id = id }, tran, commandTimeout);
        }

        public static int DeleteByIds<T>(this IDbConnection conn, object ids, IDbTransaction tran = null, int? commandTimeout = null)
        {
            if (CommonUtil.ObjectIsEmpty(ids))
                return 0;
            var builder = BuilderFactory.GetBuilder(conn);
            DynamicParameters dpar = new DynamicParameters();
            dpar.Add("@ids", ids);
            return conn.Execute(builder.GetDeleteByIdsSql<T>(), dpar, tran, commandTimeout);
        }

        public static int DeleteByWhere<T>(this IDbConnection conn, string where, object param, IDbTransaction tran = null, int? commandTimeout = null)
        {
            var builder = BuilderFactory.GetBuilder(conn);
            return conn.Execute(builder.GetDeleteByWhereSql<T>(where), param, tran, commandTimeout);
        }

        public static int DeleteAll<T>(this IDbConnection conn, IDbTransaction tran = null, int? commandTimeout = null)
        {
            var builder = BuilderFactory.GetBuilder(conn);
            return conn.Execute(builder.GetDeleteAllSql<T>(), null, tran, commandTimeout);

        }

        #endregion

        #region method (Query)

        public static IdType GetInsertId<IdType>(this IDbConnection conn, IDbTransaction tran = null, int? commandTimeout = null)
        {
            var builder = BuilderFactory.GetBuilder(conn);
            return conn.ExecuteScalar<IdType>(builder.GetInsertIdSql(), null, tran, commandTimeout);
        }

        public static dynamic GetTotal<T>(this IDbConnection conn, string where = null, object param = null, IDbTransaction tran = null, int? commandTimeout = null)
        {
            var builder = BuilderFactory.GetBuilder(conn);
            return conn.ExecuteScalar<dynamic>(builder.GetTotalSql<T>(where), param, tran, commandTimeout);
        }

        public static IEnumerable<T> GetAll<T>(this IDbConnection conn, string returnFields = null, string orderBy = null, IDbTransaction tran = null, int? commandTimeout = null)
        {
            var builder = BuilderFactory.GetBuilder(conn);
            return conn.Query<T>(builder.GetAllSql<T>(returnFields, orderBy), null, tran, true, commandTimeout);
        }

        public static IEnumerable<dynamic> GetAllDynamic<T>(this IDbConnection conn, string returnFields = null, string orderBy = null, IDbTransaction tran = null, int? commandTimeout = null)
        {
            var builder = BuilderFactory.GetBuilder(conn);
            return conn.Query(builder.GetAllSql<T>(returnFields, orderBy), null, tran, true, commandTimeout);
        }

        public static T GetById<T>(this IDbConnection conn, object id, string returnFields = null, IDbTransaction tran = null, int? commandTimeout = null)
        {
            var builder = BuilderFactory.GetBuilder(conn);
            return conn.QueryFirstOrDefault<T>(builder.GetByIdSql<T>(returnFields), new { id = id }, tran, commandTimeout);
        }

        public static dynamic GetByIdDynamic<T>(this IDbConnection conn, object id, string returnFields = null, IDbTransaction tran = null, int? commandTimeout = null)
        {
            var builder = BuilderFactory.GetBuilder(conn);
            return conn.QueryFirstOrDefault(builder.GetByIdSql<T>(returnFields), new { id = id }, tran, commandTimeout);
        }

        public static IEnumerable<T> GetByIds<T>(this IDbConnection conn, object ids, string returnFields = null, IDbTransaction tran = null, int? commandTimeout = null)
        {
            if (CommonUtil.ObjectIsEmpty(ids))
                return Enumerable.Empty<T>();
            var builder = BuilderFactory.GetBuilder(conn);
            DynamicParameters dpar = new DynamicParameters();
            dpar.Add("@ids", ids);
            return conn.Query<T>(builder.GetByIdsSql<T>(returnFields), dpar, tran, true, commandTimeout);
        }

        public static IEnumerable<dynamic> GetByIdsDynamic<T>(this IDbConnection conn, object ids, string returnFields = null, IDbTransaction tran = null, int? commandTimeout = null)
        {
            if (CommonUtil.ObjectIsEmpty(ids))
                return Enumerable.Empty<dynamic>();
            var builder = BuilderFactory.GetBuilder(conn);
            DynamicParameters dpar = new DynamicParameters();
            dpar.Add("@ids", ids);
            return conn.Query(builder.GetByIdsSql<T>(returnFields), dpar, tran, true, commandTimeout);
        }

        public static IEnumerable<T> GetByIdsWithField<T>(this IDbConnection conn, object ids, string field, string returnFields = null, IDbTransaction tran = null, int? commandTimeout = null)
        {
            if (CommonUtil.ObjectIsEmpty(ids))
                return Enumerable.Empty<T>();
            var builder = BuilderFactory.GetBuilder(conn);
            DynamicParameters dpar = new DynamicParameters();
            dpar.Add("@ids", ids);
            return conn.Query<T>(builder.GetByIdsWithFieldSql<T>(field, returnFields), dpar, tran, true, commandTimeout);
        }

        public static IEnumerable<dynamic> GetByIdsWithFieldDynamic<T>(this IDbConnection conn, object ids, string field, string returnFields = null, IDbTransaction tran = null, int? commandTimeout = null)
        {
            if (CommonUtil.ObjectIsEmpty(ids))
                return Enumerable.Empty<dynamic>();
            var builder = BuilderFactory.GetBuilder(conn);
            DynamicParameters dpar = new DynamicParameters();
            dpar.Add("@ids", ids);
            return conn.Query(builder.GetByIdsWithFieldSql<T>(field, returnFields), dpar, tran, true, commandTimeout);
        }


        public static IEnumerable<T> GetByWhere<T>(this IDbConnection conn, string where, object param = null, string returnFields = null, string orderBy = null, IDbTransaction tran = null, int? commandTimeout = null)
        {
            var builder = BuilderFactory.GetBuilder(conn);
            return conn.Query<T>(builder.GetByWhereSql<T>(where, returnFields, orderBy), param, tran, true, commandTimeout);
        }

        public static IEnumerable<dynamic> GetByWhereDynamic<T>(this IDbConnection conn, string where, object param = null, string returnFields = null, string orderBy = null, IDbTransaction tran = null, int? commandTimeout = null)
        {
            var builder = BuilderFactory.GetBuilder(conn);
            return conn.Query(builder.GetByWhereSql<T>(where, returnFields, orderBy), param, tran, true, commandTimeout);
        }

        public static T GetByWhereFirst<T>(this IDbConnection conn, string where, object param = null, string returnFields = null, IDbTransaction tran = null, int? commandTimeout = null)
        {
            var builder = BuilderFactory.GetBuilder(conn);
            return conn.QueryFirstOrDefault<T>(builder.GetByWhereFirstSql<T>(where, returnFields), param, tran, commandTimeout);
        }

        public static dynamic GetByWhereFirstDynamic<T>(this IDbConnection conn, string where, object param = null, string returnFields = null, IDbTransaction tran = null, int? commandTimeout = null)
        {
            var builder = BuilderFactory.GetBuilder(conn);
            return conn.QueryFirstOrDefault(builder.GetByWhereFirstSql<T>(where, returnFields), param, tran, commandTimeout);
        }

        public static IEnumerable<T> GetBySkipTake<T>(this IDbConnection conn, int skip, int take, string where = null, object param = null, string returnFields = null, string orderBy = null, IDbTransaction tran = null, int? commandTimeout = null)
        {
            var builder = BuilderFactory.GetBuilder(conn);
            return conn.Query<T>(builder.GetBySkipTakeSql<T>(skip, take, where, returnFields, orderBy), param, tran, true, commandTimeout);
        }

        public static IEnumerable<dynamic> GetBySkipTakeDynamic<T>(this IDbConnection conn, int skip, int take, string where = null, object param = null, string returnFields = null, string orderBy = null, IDbTransaction tran = null, int? commandTimeout = null)
        {
            var builder = BuilderFactory.GetBuilder(conn);
            return conn.Query(builder.GetBySkipTakeSql<T>(skip, take, where, returnFields, orderBy), param, tran, true, commandTimeout);
        }

        public static IEnumerable<T> GetByPageIndex<T>(this IDbConnection conn, int pageIndex, int pageSize, string where = null, object param = null, string returnFields = null, string orderBy = null, IDbTransaction tran = null, int? commandTimeout = null)
        {
            var builder = BuilderFactory.GetBuilder(conn);
            return conn.Query<T>(builder.GetByPageIndexSql<T>(pageIndex, pageSize, where, returnFields, orderBy), param, tran, true, commandTimeout);
        }

        public static IEnumerable<dynamic> GetByPageIndexDynamic<T>(this IDbConnection conn, int pageIndex, int pageSize, string where = null, object param = null, string returnFields = null, string orderBy = null, IDbTransaction tran = null, int? commandTimeout = null)
        {
            var builder = BuilderFactory.GetBuilder(conn);
            return conn.Query(builder.GetByPageIndexSql<T>(pageIndex, pageSize, where, returnFields, orderBy), param, tran, true, commandTimeout);
        }

        public static PageEntity<T> GetPage<T>(this IDbConnection conn, int pageIndex, int pageSize, string where = null, object param = null, string returnFields = null, string orderBy = null, IDbTransaction tran = null, int? commandTimeout = null)
        {
           var builder = BuilderFactory.GetBuilder(conn);
            PageEntity<T> pageEntity = new PageEntity<T>();
            using (var reader = conn.QueryMultiple(builder.GetPageSql<T>(pageIndex, pageSize, where, returnFields, orderBy), param, tran, commandTimeout))
            {           
                pageEntity.Total = reader.ReadFirstOrDefault<int>();
                if (pageEntity.Total > 0)
                    pageEntity.Data = reader.Read<T>();
                else
                    pageEntity.Data = Enumerable.Empty<T>();
            }
            return pageEntity;
        }

        public static PageEntity<dynamic> GetPageDynamic<T>(this IDbConnection conn, int pageIndex, int pageSize, string where = null, object param = null, string returnFields = null, string orderBy = null, IDbTransaction tran = null, int? commandTimeout = null)
        {
            var builder = BuilderFactory.GetBuilder(conn);
            PageEntity<dynamic> pageEntity = new PageEntity<dynamic>();
            using (var reader = conn.QueryMultiple(builder.GetPageSql<T>(pageIndex, pageSize, where, returnFields, orderBy), param, tran, commandTimeout))
            {
                pageEntity.Total = reader.ReadFirstOrDefault<dynamic>();
                if (pageEntity.Total > 0)
                    pageEntity.Data = reader.Read<dynamic>();
                else
                    pageEntity.Data = Enumerable.Empty<dynamic>();
            }
            return pageEntity;
        }


        #endregion
    }
}
