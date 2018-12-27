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
        public static string Template="";

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



    }
}
