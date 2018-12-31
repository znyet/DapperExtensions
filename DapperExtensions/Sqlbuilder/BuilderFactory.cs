using System.Data;

namespace DapperExtensions
{
    internal class BuilderFactory
    {
        private static readonly ISqlBuilder Sqlserver = new SqlServerBuilder();
        //private static readonly ISqlBuilder MySql = new MySqlBuilder();
        //private static readonly ISqlBuilder Sqlite = new SqliteBuilder();
        //private static readonly ISqlBuilder Postgre = new PostgreBuilder();
        //private static readonly ISqlBuilder Oracle = new OracleBuilder();

        public static ISqlBuilder GetBuilder(IDbConnection conn)
        {
            string dbType = conn.ToString();
            if (dbType == "System.Data.SqlClient.SqlConnection")
            {
                return Sqlserver;
            }
            //else if (dbType == "MySql.Data.MySqlClient.MySqlConnection")
            //{
            //    return MySql;
            //}
            //else if(dbType=="Npgsql.NpgsqlConnection")
            //{
            //    return Postgre;
            //}
            //else if (dbType.Contains("OracleConnection"))
            //{
            //    return Oracle;
            //}
            //else if(dbType=="System.Data.SQLite.SQLiteConnection")
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
