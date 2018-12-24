using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Dapper;
using System.Data;

namespace DapperExtensions
{
    public partial class DapperExtension
    {
        protected IDbConnection conn;
        protected IDbTransaction tran;
        protected bool buffered;
        protected int? commandTimeout;
        protected CommandType? commandType;
        protected ISqlBuilder sqlBuilder;

        public DapperExtension(IDbConnection conn, IDbTransaction tran = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
        {
            this.conn = conn;
            this.tran = tran;
            this.buffered = buffered;
            this.commandTimeout = commandTimeout;
            this.commandType = commandType;
            string dbType = conn.ToString();
            if (dbType == "System.Data.SqlClient.SqlConnection")
            {
                this.sqlBuilder = BuilderFactory.Sqlserver;
            }
            else if (dbType == "")
            {
                this.sqlBuilder = null;
            }
            else
            {
                throw new Exception("Unknown DbType:" + dbType);
            }
        }

        #region common method

        public IDataReader GetDataReader(string sql, object param = null)
        {
            return conn.ExecuteReader(sql, param, tran, commandTimeout, commandType);
        }

        public DataTable GetDataTable(string sql, object param = null)
        {
            using (IDataReader reader = this.GetDataReader(sql, param))
            {
                DataTable dt = new DataTable();
                dt.Load(reader);
                return dt;
            }
        }

        public DataSet GetDataSet(string sql, object param = null)
        {
            using (IDataReader reader = GetDataReader(sql, param))
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

        public int Execute(string sql, object param)
        {
            return conn.Execute(sql, param, tran, commandTimeout, commandType);
        }

        public T ExecuteScalar<T>(string sql, object param)
        {
            return conn.ExecuteScalar<T>(sql, param, tran, commandTimeout, commandType);
        }

        #endregion

        #region method (Insert Update Delete)

        public int Insert<T>(T model)
        {
            return Execute(sqlBuilder.InsertSql<T>(), model);
        }

        /// <summary>
        /// for sqlserver insert identity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public int InsertWithKey<T>(T model)
        {
            return Execute(sqlBuilder.InsertWithKeySql<T>(), model);
        }
        public int Update<T>(T model, string updateFields = null)
        {
            return Execute(sqlBuilder.UpdateSql<T>(updateFields), model);
        }
        public int UpdateByWhere<T>(string where, string updateFields, T model)
        {
            return Execute(sqlBuilder.UpdateByWhere<T>(where, updateFields), model);
        }
        public int InsertOrUpdate<T>(T model, string updateFields = null, bool update = true)
        {
            int effectRow = 0;
            dynamic total = ExecuteScalar<dynamic>(sqlBuilder.ExistsKeySql<T>(), model);
            if (total > 0)
            {
                if (update)
                {
                    effectRow += Update(model, updateFields);
                }
            }
            else
            {
                effectRow += Insert(model);
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
        public int InsertWithKeyOrUpdate<T>(T model, string updateFields = null, bool update = true)
        {
            int effectRow = 0;
            dynamic total = ExecuteScalar<dynamic>(sqlBuilder.ExistsKeySql<T>(), model);
            if (total > 0)
            {
                if (update)
                {
                    effectRow += Update(model, updateFields);
                }
            }
            else
            {
                effectRow += InsertWithKey(model);
            }

            return effectRow;
        }
        public int Delete<T>(object id)
        {
            return 0;
        }
        public int DeleteByIds<T>(object ids)
        {
            return 0;
        }
        public int DeleteByWhere<T>(string where, object param)
        {
            return 0;
        }
        public int DeleteAll<T>()
        {
            return 0;
        }

        #endregion

        #region method (Query)

        public DataTable GetSchemaTable<T>(string returnFields = null)
        {
            return null;
        }

        public IdType GetInsertId<IdType>()
        {
            return default(IdType);
        }

        public IEnumerable<T> GetAll<T>(string returnFields = null, string orderBy = null)
        {
            return null;
        }
        public IEnumerable<dynamic> GetAllDynamic<T>(string returnFields = null, string orderBy = null)
        {
            return null;
        }

        public T GetById<T>(object id, string returnFields = null)
        {
            return default(T);
        }
        public dynamic GetByIdDynamic<T>(object id, string returnFields = null)
        {
            return null;
        }

        public IEnumerable<T> GetByIds<T>(object ids, string returnFields = null)
        {
            return null;
        }
        public IEnumerable<dynamic> GetByIdsDynamic<T>(object ids, string returnFields = null)
        {
            return null;
        }

        public IEnumerable<T> GetByWhere<T>(string where, object param = null, string returnFields = null)
        {
            return null;
        }
        public IEnumerable<dynamic> GetByWhereDynamic<T>(string where, object param = null, string returnFields = null)
        {
            return null;
        }

        public T GetByWhereFirst<T>(string where, object param = null, string returnFields = null)
        {
            return default(T);
        }
        public dynamic GetByWhereFirstDynamic<T>(string where, object param = null, string returnFields = null)
        {
            return null;
        }

        public dynamic GetTotal<T>(string where = null, object param = null)
        {
            return null;
        }

        public IEnumerable<T> GetByIdsWithField<T>(object ids, string field, string returnFields = null)
        {
            return null;
        }
        public IEnumerable<dynamic> GetByIdsWithFieldDynamic<T>(object ids, string field, string returnFields = null)
        {
            return null;
        }

        public IEnumerable<T> GetBySkipTake<T>(int skip, int take, string where = null, object param = null, string returnFields = null, string orderBy = null)
        {
            return null;
        }
        public IEnumerable<dynamic> GetBySkipTakeDynamic<T>(int skip, int take, string where = null, object param = null, string returnFields = null, string orderBy = null)
        {
            return null;
        }

        public IEnumerable<T> GetByPageIndex<T>(int pageIndex, int pageSize, string where = null, object param = null, string returnFields = null, string orderBy = null)
        {
            return null;
        }
        public IEnumerable<dynamic> GetByPageIndexDynamic<T>(int pageIndex, int pageSize, string where = null, object param = null, string returnFields = null, string orderBy = null)
        {
            return null;
        }

        public PageEntity<T> GetPage<T>(int pageIndex, int pageSize, string where = null, object param = null, string returnFields = null, string orderBy = null)
        {
            return null;
        }
        public PageEntity<dynamic> GetPageDynamic<T>(int pageIndex, int pageSize, string where = null, object param = null, string returnFields = null, string orderBy = null)
        {
            return null;
        }


        #endregion
    }
}
