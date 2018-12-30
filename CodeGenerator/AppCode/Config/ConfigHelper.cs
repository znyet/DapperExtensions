using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CodeGenerator
{
    public class ConfigHelper
    {
        //模板
        public static string Template;

        //文件输出路径
        public static string OutPutDir;

        //命名空间
        public static string NameSpace;

        //类后缀
        public static string ClassSuffix;

        //文件类型
        public static string FileType;

        //文件类型
        public static string FileEncoding;

        //未知类型
        public static string UnKnowDbType;

        //是否开启表注释
        public static bool TableComment = true;

        //是否开启列注释
        public static bool ColumnComment = true;

        //sqlserver连接串
        public static string SqlserverConnectionString;

        //mysql连接串
        public static string MysqlConnectionString;

        //sqlite连接串
        public static string SqliteConnectionString;

        //postgresql连接串
        public static string PostgresqlConnectionString;

        //oracle连接串
        public static string OracleConnectionString;

        //********************Other*************************//

        public static string ApplicationPath = System.AppDomain.CurrentDomain.BaseDirectory;

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

        private static readonly string configFile = ApplicationPath + "Config.ini";
        private static SharpConfig.Configuration config;
        private static SharpConfig.Section section;

        public static void ReadConfigFile()
        {
            config = SharpConfig.Configuration.LoadFromFile(configFile);
            section = config["config"];

            Template = section["Template"].StringValue;
            OutPutDir = section["OutPutDir"].StringValue;
            NameSpace = section["NameSpace"].StringValue;
            ClassSuffix = section["ClassSuffix"].StringValue;
            FileType = section["FileType"].StringValue;
            FileEncoding = section["FileEncoding"].StringValue;
            UnKnowDbType = section["UnKnowDbType"].StringValue;
            TableComment = section["TableComment"].BoolValue;
            ColumnComment = section["ColumnComment"].BoolValue;
            SqlserverConnectionString = section["SqlserverConnectionString"].StringValue;
            MysqlConnectionString = section["MysqlConnectionString"].StringValue;
            SqliteConnectionString = section["SqliteConnectionString"].StringValue;
            PostgresqlConnectionString = section["PostgresqlConnectionString"].StringValue;
            OracleConnectionString = section["OracleConnectionString"].StringValue;

            XDocument doc = XDocument.Load(ApplicationPath + "Template\\DbTypeMap.xml");
            XElement DbTypeMapElement = doc.Element("DbTypeMap");

            foreach (XElement element in DbTypeMapElement.Elements("Database")) //遍历数据库
            {
                string dbProvider = element.Attribute("DbProvider").Value;
                string language = element.Attribute("Language").Value;
                string key = dbProvider + language;

                List<DbTypeEntity> list;
                if (!DbTypeDictionary.ContainsKey(key))
                {
                    list = new List<DbTypeEntity>();
                    DbTypeDictionary[key] = list;
                }
                else
                {
                    list = DbTypeDictionary[key];
                }

                foreach (XElement el in element.Elements("DbType")) //遍历语言转换
                {
                    string Name = el.Attribute("Name").Value;
                    string To = el.Attribute("To").Value.Replace("&lt;", "<").Replace("&gt;", ">");
                    DbTypeEntity model = new DbTypeEntity();
                    model.Name = Name;
                    model.To = To;
                    list.Add(model);
                }

            }

        }

        public static void SaveConfigFile()
        {

            section["Template"].SetValue(Template);
            section["OutPutDir"].SetValue(OutPutDir);
            section["NameSpace"].SetValue(NameSpace);
            section["ClassSuffix"].SetValue(ClassSuffix);
            section["FileType"].SetValue(FileType);
            section["FileEncoding"].SetValue(FileEncoding);
            section["UnKnowDbType"].SetValue(UnKnowDbType);
            section["TableComment"].SetValue(TableComment);
            section["ColumnComment"].SetValue(ColumnComment);
            section["SqlserverConnectionString"].SetValue(SqlserverConnectionString);
            section["MysqlConnectionString"].SetValue(MysqlConnectionString);
            section["SqliteConnectionString"].SetValue(SqliteConnectionString);
            section["PostgresqlConnectionString"].SetValue(PostgresqlConnectionString);
            section["OracleConnectionString"].SetValue(OracleConnectionString);

            config.SaveToFile(configFile);
        }


    }
}
