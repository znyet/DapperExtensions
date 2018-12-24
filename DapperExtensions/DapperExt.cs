using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Dapper;

namespace DapperExtensions
{
    public abstract partial class DapperExt
    {
        protected IDbConnection conn;
        protected IDbTransaction tran { get; set; }
        protected bool buffered { get; set; }
        protected int? commandTimeout { get; set; }
        protected CommandType? commandType { get; set; }

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

        #endregion

        #region abstract method (Insert Update Delete)

        public abstract int Insert<T>(T model);
        public abstract int InsertWithKey<T>(T model);
        public abstract int Update<T>(T model, string updateFields = null);
        public abstract int UpdateByWhere<T>(string where, string updateFields, T model);
        public abstract int InsertOrUpdate<T>(T model, string updateFields = null, bool update = true);
        public abstract int InsertWithKeyOrUpdate<T>(T model, string updateFields = null, bool update = true);
        public abstract int Delete<T>(object id);
        public abstract int DeleteByIds<T>(object ids);
        public abstract int DeleteByWhere<T>(string where, object param);
        public abstract int DeleteAll<T>();

        #endregion

        #region abstract method (Query)

        public abstract DataTable GetSchemaTable<T>(string returnFields = null);

        public abstract IdType GetInsertId<IdType>();

        public abstract IEnumerable<T> GetAll<T>(string returnFields = null, string orderBy = null);
        public abstract IEnumerable<dynamic> GetAllDynamic<T>(string returnFields = null, string orderBy = null);

        public abstract T GetById<T>(object id, string returnFields = null);
        public abstract dynamic GetByIdDynamic<T>(object id, string returnFields = null);

        public abstract IEnumerable<T> GetByIds<T>(object ids, string returnFields = null);
        public abstract IEnumerable<dynamic> GetByIdsDynamic<T>(object ids, string returnFields = null);

        public abstract IEnumerable<T> GetByWhere<T>(string where, object param = null, string returnFields = null);
        public abstract IEnumerable<dynamic> GetByWhereDynamic<T>(string where, object param = null, string returnFields = null);

        public abstract T GetByWhereFirst<T>(string where, object param = null, string returnFields = null);
        public abstract dynamic GetByWhereFirstDynamic<T>(string where, object param = null, string returnFields = null);

        public abstract object GetTotal<T>(string where = null, object param = null);

        public abstract IEnumerable<T> GetByIdsWithField<T>(object ids, string field, string returnFields = null);
        public abstract IEnumerable<dynamic> GetByIdsWithFieldDynamic<T>(object ids, string field, string returnFields = null);

        public abstract IEnumerable<T> GetBySkipTake<T>(int skip, int take, string where = null, object param = null, string returnFields = null, string orderBy = null);
        public abstract IEnumerable<dynamic> GetBySkipTakeDynamic<T>(int skip, int take, string where = null, object param = null, string returnFields = null, string orderBy = null);

        public abstract IEnumerable<T> GetByPageIndex<T>(int pageIndex, int pageSize, string where = null, object param = null, string returnFields = null, string orderBy = null);
        public abstract IEnumerable<dynamic> GetByPageIndexDynamic<T>(int pageIndex, int pageSize, string where = null, object param = null, string returnFields = null, string orderBy = null);

        public abstract PageEntity<T> GetPage<T>(int pageIndex, int pageSize, string where = null, object param = null, string returnFields = null, string orderBy = null);
        public abstract PageEntity<dynamic> GetPageDynamic<T>(int pageIndex, int pageSize, string where = null, object param = null, string returnFields = null, string orderBy = null);


        #endregion

    }
}
