using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DapperExtensions
{
    public interface ISqlBuilder
    {
        string InsertSql<T>();
        string InsertWithKeySql<T>();
        string UpdateSql<T>(string updateFields);
        string UpdateByWhere<T>(string where, string updateFields);
        string ExistsKeySql<T>();
    }
}
