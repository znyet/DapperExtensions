using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Data;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;

namespace Test
{
    public class DbHelper
    {
        public static IDbConnection GetConn()
        {
            return new SqlConnection("server=.;user=sa;password=123456;database=test");
            //return new MySqlConnection("server=.;database=test;uid=sa;pwd=123");
        }
    }
}