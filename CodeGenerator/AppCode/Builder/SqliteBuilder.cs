using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Dapper;

namespace CodeGenerator
{
    internal class SqliteBuilder : IBuilder
    {

        public SqliteBuilder()
        {

        }

        public List<TableEntity> GetTableList()
        {
            string sql = "select * from sqlite_master where type='table' and name!='sqlite_sequence'";
            IEnumerable<dynamic> data;

            using (var conn = DbHelper.GetConn())
            {
                data = conn.Query(sql);
            }
            
            List<TableEntity> tableList = new List<TableEntity>();
            foreach (var item in data)
            {
                TableEntity model = new TableEntity();
                model.Name = item.name;
                string script = item.sql;
                script = script.ToUpper();
                if (script.Contains("AUTOINCREMENT") && script.Contains("PRIMARY"))
                {
                    model.IsIdentity = "true";
                }

                tableList.Add(model);
            }
            return tableList;
        }

        public List<ColumnEntity> GetColumnList(TableEntity tableEntity)
        {
            string sql = "pragma table_info('" + tableEntity.Name + "')";
 
            IEnumerable<dynamic> data;
            using (var conn = DbHelper.GetConn())
            {
                data = conn.Query(sql);
            }

            List<ColumnEntity> columnList = new List<ColumnEntity>();
            foreach (var item in data)
            {
                ColumnEntity model = new ColumnEntity();
                model.Name = item.name; //列名

                if (item.pk == 1)
                {
                    tableEntity.KeyName = model.Name;
                }

                model.NameUpper = MyUtils.ToUpper(model.Name); //首字母大写
                model.NameLower = MyUtils.ToLower(model.Name); //首字母小写
                string columnType = item.type;//数据类型
                if (string.IsNullOrEmpty(columnType))
                {
                    columnType = "";
                }
                string t = columnType.ToLower();

                var cs = ConfigHelper.DbTypeDictionary[ConfigHelper.SQLiteCSharp].FirstOrDefault(f => f.Name == t);
                if (cs != null)
                    model.CsType = cs.To;
                else
                    model.CsType = ConfigHelper.UnKnowDbType;

                var java = ConfigHelper.DbTypeDictionary[ConfigHelper.SQLiteJava].FirstOrDefault(f => f.Name == t);
                if (java != null)
                    model.JavaType = java.To;
                else
                    model.JavaType = ConfigHelper.UnKnowDbType;


                model.DbType = item.type;
                model.AllowNull = Convert.ToString(item.notnull); //是否允许空
                model.DefaultValue = item.dflt_value; //默认值

                columnList.Add(model);
            }
            return columnList;
        }
    }
}
