using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DapperExtensions
{
    internal class BuilderFactory
    {
        public static ISqlBuilder Sqlserver = new SqlServerBuilder();
        //public static IBuilder MySql = new MySqlBuilder();
        //public static IBuilder Sqlite = new SqliteBuilder();
        //public static IBuilder Postgre = new PostgreBuilder();
        //public static IBuilder Oracle = new OracleBuilder();
    }
}
