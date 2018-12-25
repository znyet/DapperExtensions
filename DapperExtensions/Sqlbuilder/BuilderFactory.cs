using System.Data;

namespace DapperExtensions
{
    internal class BuilderFactory
    {
        private static readonly ISqlBuilder Sqlserver = new SqlServerBuilder();
        //public static IBuilder MySql = new MySqlBuilder();
        //public static IBuilder Sqlite = new SqliteBuilder();
        //public static IBuilder Postgre = new PostgreBuilder();
        //public static IBuilder Oracle = new OracleBuilder();

        public static ISqlBuilder GetBuilder(IDbConnection conn)
        {
            string dbType = conn.ToString();
            if (dbType == "System.Data.SqlClient.SqlConnection")
            {
                return Sqlserver;
            }
            return null;
            //else if (dbType == "MySql.Data.MySqlClient.MySqlConnection")
            //{
            //    this.sqlBuilder = null;
            //}
            //else
            //{
            //    throw new Exception("Unknown DbType:" + dbType);
            //}
        }

    }
}
