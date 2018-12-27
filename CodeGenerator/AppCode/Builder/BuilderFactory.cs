using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator
{
    internal class BuilderFactory
    {
        public static IBuilder GetBuilder(IDbConnection conn)
        {
            string dbType = conn.ToString();
            if (dbType == "System.Data.SqlClient.SqlConnection") 
            {
                return new SqlServerBuilder(conn);
            }
            else if (dbType == "MySql.Data.MySqlClient.MySqlConnection")
            {
                return new MySqlBuilder(conn);
            }
            else if (dbType == "System.Data.SQLite.SQLiteConnection")
            {
                return new SqliteBuilder(conn);
            }
            else if (dbType == "Npgsql.NpgsqlConnection")
            {
                return new PostgresqlBuilder(conn);
            }
            return null;
        }
    }
}
