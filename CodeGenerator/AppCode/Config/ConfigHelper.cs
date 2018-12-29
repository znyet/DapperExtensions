using EasyConfig;
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
        private static ConfigFile GetConfigFile()
        {
            ConfigFile cfg = new ConfigFile(Config.ApplicationPath + "Config.ini");
            return cfg;
        }

        public static void ReadConfigFile()
        {
            ConfigFile cfg = GetConfigFile();
            var group = cfg.SettingGroups["config"];

            Config.Template = group.Settings["Template"].GetValueAsString();
            Config.OutPutDir = group.Settings["OutPutDir"].GetValueAsString();
            Config.NameSpace = group.Settings["NameSpace"].GetValueAsString();
            Config.ClassSuffix = group.Settings["ClassSuffix"].GetValueAsString();
            if (Config.ClassSuffix == "@")
            {
                Config.ClassSuffix = "";
            }
            Config.FileType = group.Settings["FileType"].GetValueAsString();
            Config.FileEncoding = group.Settings["FileEncoding"].GetValueAsString();
            Config.UnKnowDbType = group.Settings["UnKnowDbType"].GetValueAsString();
            Config.TableComment = group.Settings["TableComment"].GetValueAsBool();
            Config.ColumnComment = group.Settings["ColumnComment"].GetValueAsBool();
            Config.SqlserverConnectionString = group.Settings["SqlserverConnectionString"].GetValueAsString();
            Config.MysqlConnectionString = group.Settings["MysqlConnectionString"].GetValueAsString();
            Config.SqliteConnectionString = group.Settings["SqliteConnectionString"].GetValueAsString();
            Config.PostgresqlConnectionString = group.Settings["PostgresqlConnectionString"].GetValueAsString();
            Config.OracleConnectionString = group.Settings["OracleConnectionString"].GetValueAsString();

            Config.ApplicationPath = System.AppDomain.CurrentDomain.BaseDirectory;

            XDocument doc = XDocument.Load(Config.ApplicationPath + "Template\\DbTypeMap.xml");
            XElement DbTypeMapElement = doc.Element("DbTypeMap");

            foreach (XElement element in DbTypeMapElement.Elements("Database")) //遍历数据库
            {
                string dbProvider = element.Attribute("DbProvider").Value;
                string language = element.Attribute("Language").Value;
                string key = dbProvider + language;

                List<DbTypeEntity> list;
                if (!Config.DbTypeDictionary.ContainsKey(key))
                {
                    list = new List<DbTypeEntity>();
                    Config.DbTypeDictionary[key] = list;
                }
                else
                {
                    list = Config.DbTypeDictionary[key];
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
            ConfigFile cfg = GetConfigFile();
            var group = cfg.SettingGroups["config"];
            group.Settings["Template"].SetValue(Config.Template);
            group.Settings["OutPutDir"].SetValue(Config.OutPutDir);
            group.Settings["NameSpace"].SetValue(Config.NameSpace);
            if (string.IsNullOrEmpty(Config.ClassSuffix))
            {
                group.Settings["ClassSuffix"].SetValue("@");
            }
            else
            {
                group.Settings["ClassSuffix"].SetValue(Config.ClassSuffix);
            }
            group.Settings["FileType"].SetValue(Config.FileType);
            group.Settings["FileEncoding"].SetValue(Config.FileEncoding);
            group.Settings["UnKnowDbType"].SetValue(Config.UnKnowDbType);
            group.Settings["TableComment"].SetValue(Config.TableComment);
            group.Settings["ColumnComment"].SetValue(Config.ColumnComment);
            group.Settings["SqlserverConnectionString"].SetValue(Config.SqlserverConnectionString);
            group.Settings["MysqlConnectionString"].SetValue(Config.MysqlConnectionString);
            group.Settings["SqliteConnectionString"].SetValue(Config.SqliteConnectionString);
            group.Settings["PostgresqlConnectionString"].SetValue(Config.PostgresqlConnectionString);
            group.Settings["OracleConnectionString"].SetValue(Config.OracleConnectionString);
            cfg.Save(Config.ApplicationPath + "Config.ini");


        }




    }
}
