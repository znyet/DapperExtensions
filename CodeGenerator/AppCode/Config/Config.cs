using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator
{
    public class Config
    {
        //模板
        public static string Template = "";

        //文件输出路径
        public static string OutPutDir = "";

        //命名空间
        public static string NameSpace = "";

        //类后缀
        public static string ClassSuffix = "";

        //文件类型
        public static string FileType = "";

        //文件类型
        public static string FileEncoding = "";

        //未知类型
        public static string UnKnowDbType = "";

        //是否开启表注释
        public static bool TableComment = true;

        //是否开启列注释
        public static bool ColumnComment = true;

        //sqlserver连接串
        public static string SqlserverConnectionString = "null";

        //mysql连接串
        public static string MysqlConnectionString = "null";

        //sqlite连接串
        public static string SqliteConnectionString = "null";

        //postgresql连接串
        public static string PostgresqlConnectionString = "null";

        //oracle连接串
        public static string OracleConnectionString = "null";

        //********************Other*************************//

        public static string ApplicationPath;

        //数据库类型
        public static string DbType;

        //当前数据库连接
        public static string ConnectionString;

        //数据类型转换
        public static readonly Dictionary<string, List<DbTypeEntity>> DbTypeDictionary = new Dictionary<string, List<DbTypeEntity>>();

        public static readonly string SqlServerCSharp = "SqlServerCSharp";
        public static readonly string MySqlCSharp = "MySqlCSharp";
        public static readonly string PostgreSqlCSharp = "PostgreSqlCSharp";
        public static readonly string OracleCSharp = "OracleCSharp";
        public static readonly string SQLiteCSharp = "SQLiteCSharp";

        public static readonly string MySqlJava = "MySqlJava";
        public static readonly string PostgreSqlJava = "PostgreSqlJava";
        public static readonly string SqlServerJava = "SqlServerJava";
        public static readonly string OracleJava = "OracleJava";
        public static readonly string SQLiteJava = "SQLiteJava";

    }
}
