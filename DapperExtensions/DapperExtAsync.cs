#define net45
#if net45
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DapperExtensions
{
    public abstract partial class DapperExt
    {
        public abstract Task<int> InsertAsync<T>(T model);
        public abstract Task<int> InsertWithKeyAsync<T>(T model);
        public abstract Task<int> UpdateAsync<T>(T model, string updateFields = null);
        public abstract Task<int> UpdateByWhereAsync<T>(string where, string updateFields, T model);
        public abstract Task<int> InsertOrUpdateAsync<T>(T model, string updateFields = null, bool update = true);
        public abstract Task<int> InsertWithKeyOrUpdateAsync<T>(T model, string updateFields = null, bool update = true);
        public abstract Task<int> DeleteAsync<T>(object id);
        public abstract Task<int> DeleteByIdsAsync<T>(object ids);
        public abstract Task<int> DeleteByWhereAsync<T>(string where, object param);
        public abstract Task<int> DeleteAllAsync<T>();

        public abstract Task<IdType> GetInsertIdAsync<IdType>();

        public abstract Task<IEnumerable<T>> GetAllAsync<T>(string returnFields = null, string orderBy = null);
        public abstract Task<IEnumerable<dynamic>> GetAllDynamicAsync<T>(string returnFields = null, string orderBy = null);

        public abstract Task<T> GetByIdAsync<T>(object id, string returnFields = null);
        public abstract Task<dynamic> GetByIdDynamicAsync<T>(object id, string returnFields = null);

        public abstract Task<IEnumerable<T>> GetByIdsAsync<T>(object ids, string returnFields = null);
        public abstract Task<IEnumerable<dynamic>> GetByIdsDynamicAsync<T>(object ids, string returnFields = null);

        public abstract Task<IEnumerable<T>> GetByWhereAsync<T>(string where, object param = null, string returnFields = null);
        public abstract Task<IEnumerable<dynamic>> GetByWhereDynamicAsync<T>(string where, object param = null, string returnFields = null);

        public abstract Task<T> GetByWhereFirstAsync<T>(string where, object param = null, string returnFields = null);
        public abstract Task<dynamic> GetByWhereFirstDynamicAsync<T>(string where, object param = null, string returnFields = null);

        public abstract Task<object> GetTotalAsync<T>(string where = null, object param = null);

        public abstract Task<IEnumerable<T>> GetByIdsWithFieldAsync<T>(object ids, string field, string returnFields = null);
        public abstract Task<IEnumerable<dynamic>> GetByIdsWithFieldDynamicAsync<T>(object ids, string field, string returnFields = null);

        public abstract Task<IEnumerable<T>> GetBySkipTakeAsync<T>(int skip, int take, string where = null, object param = null, string returnFields = null, string orderBy = null);
        public abstract Task<IEnumerable<dynamic>> GetBySkipTakeDynamicAsync<T>(int skip, int take, string where = null, object param = null, string returnFields = null, string orderBy = null);

        public abstract Task<IEnumerable<T>> GetByPageIndexAsync<T>(int pageIndex, int pageSize, string where = null, object param = null, string returnFields = null, string orderBy = null);
        public abstract Task<IEnumerable<dynamic>> GetByPageIndexDynamicAsync<T>(int pageIndex, int pageSize, string where = null, object param = null, string returnFields = null, string orderBy = null);

        public abstract Task<PageEntity<T>> GetPageAsync<T>(int pageIndex, int pageSize, string where = null, object param = null, string returnFields = null, string orderBy = null);
        public abstract Task<PageEntity<dynamic>> GetPageDynamicAsync<T>(int pageIndex, int pageSize, string where = null, object param = null, string returnFields = null, string orderBy = null);

    }
}
#endif