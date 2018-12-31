using System.Data;

namespace DapperExtensions
{
    internal class BuilderFactory
    {
        private static readonly ISqlBuilder Sqlserver = new SqlServerBuilder();
        private static readonly ISqlBuilder MySql = new MySqlBuilder();
        //private static readonly ISqlBuilder Sqlite = new SqliteBuilder();
        //private static readonly ISqlBuilder Postgre = new PostgreBuilder();
        //private static readonly ISqlBuilder Oracle = new OracleBuilder();

        public static ISqlBuilder GetBuilder(IDbConnection conn)
        {
            string dbType = conn.ToString();
            if (dbType.EndsWith("SqlConnection"))
            {
                return Sqlserver;
            }
            else if (dbType.EndsWith("MySqlConnection"))
            {
                return MySql;
            }
            //else if (dbType.EndsWith("NpgsqlConnection"))
            //{
            //    return Postgre;
            //}
            //else if (dbType.EndsWith("OracleConnection"))
            //{
            //    return Oracle;
            //}
            //else if (dbType.EndsWith("SQLiteConnection"))
            //{
            //    return Sqlite;
            //}
            //else
            //{
            //    throw new Exception("Unknown DbType:" + dbType);
            //}
            return null;
        }

    }
}
