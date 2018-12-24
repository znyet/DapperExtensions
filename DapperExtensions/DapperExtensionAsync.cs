using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Dapper;

namespace DapperExtensions
{
    public partial class DapperExtension
    {
        #region common method

        public async Task<int> ExecuteAsync(string sql, object param)
        {
            return await conn.ExecuteAsync(sql, param, tran, commandTimeout, commandType);
        }



        #endregion

        public async Task<int> InsertAsync<T>(T model)
        {
            return await ExecuteAsync(sqlBuilder.InsertSql<T>(), model);
        }

        public async Task<int> InsertWithKeyAsync<T>(T model)
        {
            return await ExecuteAsync(sqlBuilder.InsertWithKeySql<T>(), model);
        }

        public async Task<int> UpdateAsync<T>(T model, string updateFields = null)
        {
            return await ExecuteAsync(sqlBuilder.UpdateSql<T>(updateFields), model);
        }

        public async Task<int> UpdateByWhereAsync<T>(string where, string updateFields, T model)
        {
            return await ExecuteAsync(sqlBuilder.UpdateByWhere<T>(where, updateFields), model);
        }

        public async Task<int> InsertOrUpdateAsync<T>(T model, string updateFields = null, bool update = true)
        {
            return await Task.Run(() =>
            {
                return InsertOrUpdate(model, updateFields, update);
            });
        }

        public async Task<int> InsertWithKeyOrUpdateAsync<T>(T model, string updateFields = null, bool update = true)
        {
            return await Task.Run(() =>
            {
                return InsertWithKeyOrUpdate(model, updateFields, update);
            });
        }

        public async Task<int> DeleteAsync<T>(object id)
        {
            return await Task.Run(() =>
            {
                return Delete<T>(id);
            });
        }

        public async Task<int> DeleteByIdsAsync<T>(object ids)
        {
            return await Task.Run(() =>
            {
                return DeleteByIds<T>(ids);
            });
        }

        public async Task<int> DeleteByWhereAsync<T>(string where, object param)
        {
            return await Task.Run(() =>
            {
                return DeleteByWhere<T>(where, param);
            });
        }

        public async Task<int> DeleteAllAsync<T>()
        {
            return await Task.Run(() =>
            {
                return DeleteAll<T>();
            });
        }

        public async Task<IdType> GetInsertIdAsync<IdType>()
        {
            return await Task.Run(() =>
            {
                return GetInsertId<IdType>();
            });
        }

        public async Task<IEnumerable<T>> GetAllAsync<T>(string returnFields = null, string orderBy = null)
        {
            return await Task.Run(() =>
            {
                return GetAll<T>(returnFields, orderBy);
            });
        }

        public async Task<IEnumerable<dynamic>> GetAllDynamicAsync<T>(string returnFields = null, string orderBy = null)
        {
            return await Task.Run(() =>
            {
                return GetAllDynamic<T>(returnFields, orderBy);
            });
        }

        public async Task<T> GetByIdAsync<T>(object id, string returnFields = null)
        {
            return await Task.Run(() =>
            {
                return GetById<T>(id, returnFields);
            });
        }

        public async Task<dynamic> GetByIdDynamicAsync<T>(object id, string returnFields = null)
        {
            return await Task.Run(() =>
            {
                return GetByIdDynamic<T>(id, returnFields);
            });
        }

        public async Task<IEnumerable<T>> GetByIdsAsync<T>(object ids, string returnFields = null)
        {
            return await Task.Run(() =>
            {
                return GetByIds<T>(ids, returnFields);
            });
        }

        public async Task<IEnumerable<dynamic>> GetByIdsDynamicAsync<T>(object ids, string returnFields = null)
        {
            return await Task.Run(() =>
            {
                return GetByIdsDynamic<T>(ids, returnFields);
            });
        }

        public async Task<IEnumerable<T>> GetByWhereAsync<T>(string where, object param = null, string returnFields = null)
        {
            return await Task.Run(() =>
            {
                return GetByWhere<T>(where, param, returnFields);
            });
        }

        public async Task<IEnumerable<dynamic>> GetByWhereDynamicAsync<T>(string where, object param = null, string returnFields = null)
        {
            return await Task.Run(() =>
            {
                return GetByWhereDynamic<T>(where, param, returnFields);
            });
        }

        public async Task<T> GetByWhereFirstAsync<T>(string where, object param = null, string returnFields = null)
        {
            return await Task.Run(() =>
            {
                return GetByWhereFirst<T>(where, param, returnFields);
            });
        }

        public async Task<dynamic> GetByWhereFirstDynamicAsync<T>(string where, object param = null, string returnFields = null)
        {
            return await Task.Run(() =>
            {
                return GetByWhereFirstDynamic<T>(where, param, returnFields);
            });
        }

        public async Task<dynamic> GetTotalAsync<T>(string where = null, object param = null)
        {
            return await Task.Run(() =>
            {
                return GetTotal<T>(where, param);
            });
        }

        public async Task<IEnumerable<T>> GetByIdsWithFieldAsync<T>(object ids, string field, string returnFields = null)
        {
            return await Task.Run(() =>
            {
                return GetByIdsWithField<T>(ids, field, returnFields);
            });
        }

        public async Task<IEnumerable<dynamic>> GetByIdsWithFieldDynamicAsync<T>(object ids, string field, string returnFields = null)
        {
            return await Task.Run(() =>
            {
                return GetByIdsWithFieldDynamic<T>(ids, field, returnFields);
            });
        }

        public async Task<IEnumerable<T>> GetBySkipTakeAsync<T>(int skip, int take, string where = null, object param = null, string returnFields = null, string orderBy = null)
        {
            return await Task.Run(() =>
            {
                return GetBySkipTake<T>(skip, take, where, param, returnFields, orderBy);
            });
        }

        public async Task<IEnumerable<dynamic>> GetBySkipTakeDynamicAsync<T>(int skip, int take, string where = null, object param = null, string returnFields = null, string orderBy = null)
        {
            return await Task.Run(() =>
            {
                return GetBySkipTakeDynamic<T>(skip, take, where, param, returnFields, orderBy);
            });
        }

        public async Task<IEnumerable<T>> GetByPageIndexAsync<T>(int pageIndex, int pageSize, string where = null, object param = null, string returnFields = null, string orderBy = null)
        {
            return await Task.Run(() =>
            {
                return GetByPageIndex<T>(pageIndex, pageSize, where, param, returnFields, orderBy);
            });
        }

        public async Task<IEnumerable<dynamic>> GetByPageIndexDynamicAsync<T>(int pageIndex, int pageSize, string where = null, object param = null, string returnFields = null, string orderBy = null)
        {
            return await Task.Run(() =>
            {
                return GetByPageIndexDynamic<T>(pageIndex, pageSize, where, param, returnFields, orderBy);
            });
        }

        public async Task<PageEntity<T>> GetPageAsync<T>(int pageIndex, int pageSize, string where = null, object param = null, string returnFields = null, string orderBy = null)
        {
            return await Task.Run(() =>
            {
                return GetPage<T>(pageIndex, pageSize, where, param, returnFields, orderBy);
            });
        }

        public async Task<PageEntity<dynamic>> GetPageDynamicAsync<T>(int pageIndex, int pageSize, string where = null, object param = null, string returnFields = null, string orderBy = null)
        {
            return await Task.Run(() =>
            {
                return GetPageDynamic<T>(pageIndex, pageSize, where, param, returnFields, orderBy);
            });
        }
    }
}
