using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DapperExtensions
{
    public class SqlServerBuilder : ISqlBuilder
    {

        string ISqlBuilder.InsertSql<T>()
        {
            return SqlServerCache.GetTableEntity<T>().InsertSql;
        }

        string ISqlBuilder.InsertWithKeySql<T>()
        {
            var table = SqlServerCache.GetTableEntity<T>();
            CommonUtil.CheckTableKey(table);
            if (table.IsIdentity)
            {
                return string.Format("SET IDENTITY_INSERT [{0}] ON;{1};SET IDENTITY_INSERT [{0}] OFF;", table.TableName, table.InsertKeySql);
            }
            return table.InsertKeySql;
        }

        string ISqlBuilder.UpdateSql<T>(string updateFields)
        {
            var table = SqlServerCache.GetTableEntity<T>();
            CommonUtil.CheckTableKey(table);
            if (string.IsNullOrEmpty(updateFields) || updateFields == "*")
            {
                return table.UpdateSql;
            }
            return CommonUtil.CreateUpdateSql(table, updateFields, "[", "]");
        }
    }
}
