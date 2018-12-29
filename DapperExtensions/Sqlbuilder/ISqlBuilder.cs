using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DapperExtensions
{
    internal interface ISqlBuilder
    {
        string SchemaTableSql<T>(string returnFields);

        string InsertSql<T>();

        string InsertIdentitySql<T>();

        string UpdateSql<T>(string updateFields);

        string UpdateByWhereSql<T>(string where, string updateFields);

        string ExistsKeySql<T>();

        string DeleteByIdSql<T>();

        string DeleteByIdsSql<T>();

        string DeleteByWhereSql<T>(string where);

        string DeleteAllSql<T>();


    }
}
