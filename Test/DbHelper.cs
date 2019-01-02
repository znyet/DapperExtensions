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

namespace Test
{
    public class DbHelper
    {
        public static IDbConnection GetConn()
        {
            return new SqlConnection("server=127.0.0.1;uid=sa;pwd=123456;database=test;timeout=1");
            //return new MySqlConnection("server=127.0.0.1;uid=root;pwd=123456;database=test;charset=utf8");
            //return new SQLiteConnection(@"Data Source=C:\Users\Administrator\Desktop\1.db;Pooling=true;FailIfMissing=false");
            //return new NpgsqlConnection("server=127.0.0.1;uid=postgres;pwd=123456;database=test");
            //return new OracleConnection("User ID=test;Password=123456;Data Source=(DESCRIPTION =(ADDRESS_LIST =(ADDRESS = (PROTOCOL = TCP)(HOST = localhost)(PORT = 1521)))(CONNECT_DATA =(SERVICE_NAME = XE)))");
        }
    }
}
