using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Dapper;

namespace CodeGenerator
{
    internal class MySqlBuilder : IBuilder
    {

        public MySqlBuilder()
        {

        }

        public List<TableEntity> GetTableList()
        {
            string sql = "SHOW TABLE STATUS";
            IEnumerable<dynamic> data;
            using (var conn = DbHelper.GetConn())
            {
                data = conn.Query(sql);
            }

            List<TableEntity> tableList = new List<TableEntity>();
            foreach (var item in data)
            {
                TableEntity model = new TableEntity();
                model.Name = item.Name;
                if (ConfigHelper.TableComment)
                {
                    model.Comment = item.Comment;
                }
                tableList.Add(model);
            }
            return tableList;
        }

        public List<ColumnEntity> GetColumnList(TableEntity tableEntity)
        {
            string sql = "SHOW FULL COLUMNS FROM " + tableEntity.Name;
            IEnumerable<dynamic> data;
            using (var conn = DbHelper.GetConn())
            {
                data = conn.Query(sql);
            }

            List<ColumnEntity> columnList = new List<ColumnEntity>();
            foreach (var item in data)
            {
                ColumnEntity model = new ColumnEntity();
                model.Name = item.Field; //列名

                if (!string.IsNullOrEmpty(item.Key) && item.Key == "PRI")
                {
                    tableEntity.KeyName = model.Name;
                    if (!string.IsNullOrEmpty(item.Extra) && item.Extra == "auto_increment")
                    {
                        tableEntity.IsIdentity = "true";
                    }
                }

                model.NameUpper = MyUtils.ToUpper(model.Name); //首字母大写
                model.NameLower = MyUtils.ToLower(model.Name); //首字母小写

                string columnType = item.Type;//数据类型

                if (string.IsNullOrEmpty(columnType))
                {
                    columnType = "";
                }

                string t = columnType.Split('(')[0].ToLower();

                var cs = ConfigHelper.DbTypeDictionary[ConfigHelper.MySqlCSharp].FirstOrDefault(f => f.Name == t);
                if (cs != null)
                    model.CsType = cs.To;
                else
                    model.CsType = ConfigHelper.UnKnowDbType;


                var java = ConfigHelper.DbTypeDictionary[ConfigHelper.MySqlJava].FirstOrDefault(f => f.Name == t);
                if (java != null)
                    model.JavaType = java.To;
                else
                    model.JavaType = ConfigHelper.UnKnowDbType;


                model.DbType = item.Type;
                if (ConfigHelper.ColumnComment)
                {
                    model.Comment = item.Comment; //说明
                }
                model.AllowNull = item.Null; //是否允许空
                model.DefaultValue = item.Default; //默认值

                columnList.Add(model);
            }
            return columnList;
        }
    }
}
