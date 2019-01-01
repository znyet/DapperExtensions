
namespace DapperExtensions
{
    internal interface ISqlBuilder
    {

        #region method (Insert Update Delete)

        string GetSchemaTableSql<T>(string returnFields);

        string GetInsertSql<T>();

        string GetInsertReturnIdSql<T>(string sequence = null);

        string GetInsertIdentitySql<T>();

        string GetUpdateSql<T>(string updateFields);

        string GetUpdateByWhereSql<T>(string where, string updateFields);

        string GetExistsKeySql<T>();

        string GetDeleteByIdSql<T>();

        string GetDeleteByIdsSql<T>();

        string GetDeleteByWhereSql<T>(string where);

        string GetDeleteAllSql<T>();

        #endregion


        #region method (Query)

        string GetIdentitySql();

        string GetIdentityCurrentSql(string sequence, string dual = "DUAL");

        string GetIdentityNextSql(string sequence, string dual = "DUAL");

        string GetTotalSql<T>(string where);

        string GetAllSql<T>(string returnFields, string orderBy);

        string GetByIdSql<T>(string returnFields);

        string GetByIdsSql<T>(string returnFields);

        string GetByIdsWithFieldSql<T>(string field, string returnFields);

        string GetByWhereSql<T>(string where, string returnFields, string orderBy);

        string GetByWhereFirstSql<T>(string where, string returnFields);

        string GetBySkipTakeSql<T>(int skip, int take, string where, string returnFields, string orderBy);

        string GetByPageIndexSql<T>(int pageIndex, int pageSize, string where, string returnFields, string orderBy);

        string GetPageSql<T>(int pageIndex, int pageSize, string where, string returnFields, string orderBy);

        #endregion


    }
}
