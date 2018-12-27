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
        private IDbConnection conn;

        public MySqlBuilder(IDbConnection conn)
        {
            this.conn = conn;
        }

        public List<TableEntity> GetTableList()
        {
            string sql = "SHOW TABLE STATUS";
            IEnumerable<dynamic> data = conn.Query(sql);
            List<TableEntity> tableList = new List<TableEntity>();
            foreach (var item in data)
            {
                TableEntity model = new TableEntity();
                model.Name = item.Name;
                model.Comment = item.Comment;
                tableList.Add(model);
            }
            return tableList;
        }

        public List<ColumnEntity> GetColumnList(TableEntity tableEntity)
        {
            string sql = "SHOW FULL COLUMNS FROM " + tableEntity.Name;
            List<ColumnEntity> columnList = new List<ColumnEntity>();
            IEnumerable<dynamic> data = conn.Query(sql);
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
                string t = columnType.Split('(')[0].ToLower();
                switch (t)
                {
                    case "image": model.CsType = "byte[]"; break;
                    case "text": model.CsType = "string"; break;
                    case "ntext": model.CsType = "string"; break;
                    case "varchar": model.CsType = "string"; break;
                    case "nvarchar": model.CsType = "string"; break;
                    case "varchar2": model.CsType = "string"; break;
                    case "nvarchar2": model.CsType = "string"; break;
                    case "xml": model.CsType = "string"; break;
                    case "uniqueidentifier": model.CsType = "Guid"; break;
                    case "date": model.CsType = "DateTime"; break;
                    case "smalldatetime": model.CsType = "DateTime"; break;
                    case "datetime": model.CsType = "DateTime"; break;
                    case "datetime2": model.CsType = "DateTime"; break;
                    case "time": model.CsType = "TimeSpan"; break;
                    case "datetimeoffset": model.CsType = "DateTimeOffset"; break;
                    case "tinyint": model.CsType = "byte"; break;
                    case "smallint": model.CsType = "short"; break;
                    case "int": model.CsType = "int"; break;
                    case "bigint": model.CsType = "long"; break;
                    case "bit": model.CsType = "bool"; break;
                    case "char": model.CsType = "string"; break;
                    case "nchar": model.CsType = "string"; break;
                    case "uniqueide": model.CsType = "Guid"; break;
                    case "numeric": model.CsType = "decimal"; break;
                    case "integer": model.CsType = "int"; break;
                    case "money": model.CsType = "decimal"; break;
                    case "real": model.CsType = "decimal"; break;
                    case "blob": model.CsType = "byte[]"; break;
                    case "single": model.CsType = "float"; break;
                    case "smallmoney": model.CsType = "decimal"; break;
                    case "decimal": model.CsType = "decimal"; break;
                    case "bfile": model.CsType = "byte[]"; break;
                    case "float": model.CsType = "float"; break;
                    case "binary": model.CsType = "byte[]"; break;
                    case "number": model.CsType = "decimal"; break;
                    case "varbinary": model.CsType = "byte[]"; break;
                    case "boolean": model.CsType = "bool"; break;
                    case "long": model.CsType = "byte[]"; break;
                    case "longtext": model.CsType = "string"; break;
                    case "timestamp": model.CsType = "DateTime"; break;
                    case "enum": model.CsType = "string"; break;
                    default:
                        model.CsType = "dynamic";
                        break;
                }


                model.DbType = item.Type;
                model.Comment = item.Comment; //说明
                model.AllowNull = item.Null; //是否允许空
                model.DefaultValue = item.Default; //默认值

                columnList.Add(model);
            }
            return columnList;
        }
    }
}
