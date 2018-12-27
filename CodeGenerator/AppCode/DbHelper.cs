using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using System.Data.SQLite;
using Npgsql;
using System.Data.OracleClient;

namespace CodeGenerator
{
    public class DbHelper
    {
        //获取数据库连接
        public static IDbConnection GetConn()
        {
            if (Config.DbType == "sqlserver")
            {
                SqlConnection conn = new SqlConnection(Config.SqlserverConnectionString);
                conn.Open();
                return conn;
            }
            else if (Config.DbType == "mysql")
            {
                MySqlConnection conn = new MySqlConnection(Config.MysqlConnectionString);
                conn.Open();
                return conn;
            }
            else if (Config.DbType == "sqlite")
            {
                SQLiteConnection conn = new SQLiteConnection(Config.SqliteConnectionString);
                conn.Open();
                return conn;
            }
            else if (Config.DbType == "postgresql")
            {
                NpgsqlConnection conn = new NpgsqlConnection(Config.PostgresqlConnectionString);
                conn.Open();
                return conn;
            }
            else
            {
                OracleConnection conn = new OracleConnection(Config.OracleConnectionString);
                conn.Open();
                return conn;
            }

        }
    }
}
