using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DapperExtensions
{
    public interface ISqlBuilder
    {
        string SchemaTable<T>(string returnFields);

        string InsertSql<T>();

        string InsertIdentitySql<T>();

        string UpdateSql<T>(string updateFields);

        string UpdateByWhere<T>(string where, string updateFields);

        string ExistsKeySql<T>();

        string DeleteById<T>();

        string DeleteByIds<T>();

        string DeleteAllSql<T>();


    }
}
