using EasyConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            Config.FileType = group.Settings["FileType"].GetValueAsString();
            Config.FileEncoding = group.Settings["FileEncoding"].GetValueAsString();
            Config.TableComment = group.Settings["TableComment"].GetValueAsBool();
            Config.ColumnComment = group.Settings["ColumnComment"].GetValueAsBool();
            Config.SqlserverConnectionString = group.Settings["SqlserverConnectionString"].GetValueAsString();
            Config.MysqlConnectionString = group.Settings["MysqlConnectionString"].GetValueAsString();
            Config.SqliteConnectionString = group.Settings["SqliteConnectionString"].GetValueAsString();
            Config.PostgresqlConnectionString = group.Settings["PostgresqlConnectionString"].GetValueAsString();
            Config.OracleConnectionString = group.Settings["OracleConnectionString"].GetValueAsString();

            Config.ApplicationPath = System.AppDomain.CurrentDomain.BaseDirectory;
        }

        public static void SaveConfigFile()
        {
            ConfigFile cfg = GetConfigFile();
            var group = cfg.SettingGroups["config"];
            group.Settings["Template"].SetValue(Config.Template);
            group.Settings["OutPutDir"].SetValue(Config.OutPutDir);
            group.Settings["NameSpace"].SetValue(Config.NameSpace);
            group.Settings["ClassSuffix"].SetValue(Config.ClassSuffix);
            group.Settings["FileType"].SetValue(Config.FileType);
            group.Settings["FileEncoding"].SetValue(Config.FileEncoding);
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
