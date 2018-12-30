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
using Oracle.ManagedDataAccess.Client;

namespace CodeGenerator
{
    internal class DbHelper
    {
        //获取数据库连接
        public static IDbConnection GetConn()
        {
            if (ConfigHelper.DbType == "sqlserver")
            {
                SqlConnection conn = new SqlConnection(ConfigHelper.SqlserverConnectionString);
                conn.Open();
                return conn;
            }
            else if (ConfigHelper.DbType == "mysql")
            {
                MySqlConnection conn = new MySqlConnection(ConfigHelper.MysqlConnectionString);
                conn.Open();
                return conn;
            }
            else if (ConfigHelper.DbType == "sqlite")
            {
                SQLiteConnection conn = new SQLiteConnection(ConfigHelper.SqliteConnectionString);
                conn.Open();
                return conn;
            }
            else if (ConfigHelper.DbType == "postgresql")
            {
                NpgsqlConnection conn = new NpgsqlConnection(ConfigHelper.PostgresqlConnectionString);
                conn.Open();
                return conn;
            }
            else
            {
                OracleConnection conn = new OracleConnection(ConfigHelper.OracleConnectionString);
                conn.Open();
                return conn;
            }

        }

        public static IBuilder GetBuilder()
        {
            if (ConfigHelper.DbType == "sqlserver")
            {
                return new SqlServerBuilder();
            }
            else if (ConfigHelper.DbType == "mysql")
            {
                return new MySqlBuilder();
            }
            else if (ConfigHelper.DbType == "sqlite")
            {
                return new SqliteBuilder();
            }
            else if (ConfigHelper.DbType == "postgresql")
            {
                return new PostgresqlBuilder();
            }
            else
            {
                return new OracleBuilder();
            }

        }

    }
}
