﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DapperExtensions
{
    internal class SqliteBuilder : ISqlBuilder
    {
        public string GetSchemaTableSql<T>(string returnFields)
        {
            throw new NotImplementedException();
        }

        public string GetInsertSql<T>()
        {
            throw new NotImplementedException();
        }

        public string GetInsertReturnIdSql<T>(string sequence = null)
        {
            return SqliteCache.GetTableEntity<T>().InsertReturnIdSql;
        }

        public string GetInsertIdentitySql<T>()
        {
            throw new NotImplementedException();
        }

        public string GetUpdateSql<T>(string updateFields)
        {
            throw new NotImplementedException();
        }

        public string GetUpdateByWhereSql<T>(string where, string updateFields)
        {
            throw new NotImplementedException();
        }

        public string GetExistsKeySql<T>()
        {
            throw new NotImplementedException();
        }

        public string GetDeleteByIdSql<T>()
        {
            throw new NotImplementedException();
        }

        public string GetDeleteByIdsSql<T>()
        {
            throw new NotImplementedException();
        }

        public string GetDeleteByWhereSql<T>(string where)
        {
            throw new NotImplementedException();
        }

        public string GetDeleteAllSql<T>()
        {
            throw new NotImplementedException();
        }

        //public string GetIdentitySql()
        //{
        //    return "SELECT last_insert_rowid()";
        //}

        public string GetSequenceCurrentSql(string sequence)
        {
            return "SELECT last_insert_rowid()";
        }

        public string GetSequenceNextSql(string sequence)
        {
            return "SELECT last_insert_rowid()";
        }

        public string GetTotalSql<T>(string where)
        {
            throw new NotImplementedException();
        }

        public string GetAllSql<T>(string returnFields, string orderBy)
        {
            throw new NotImplementedException();
        }

        public string GetByIdSql<T>(string returnFields)
        {
            throw new NotImplementedException();
        }

        public string GetByIdsSql<T>(string returnFields)
        {
            throw new NotImplementedException();
        }

        public string GetByIdsWithFieldSql<T>(string field, string returnFields)
        {
            throw new NotImplementedException();
        }

        public string GetByWhereSql<T>(string where, string returnFields, string orderBy)
        {
            throw new NotImplementedException();
        }

        public string GetByWhereFirstSql<T>(string where, string returnFields)
        {
            throw new NotImplementedException();
        }

        public string GetBySkipTakeSql<T>(int skip, int take, string where, string returnFields, string orderBy)
        {
            throw new NotImplementedException();
        }

        public string GetByPageIndexSql<T>(int pageIndex, int pageSize, string where, string returnFields, string orderBy)
        {
            throw new NotImplementedException();
        }

        public string GetPageSql<T>(int pageIndex, int pageSize, string where, string returnFields, string orderBy)
        {
            throw new NotImplementedException();
        }
    }
}
