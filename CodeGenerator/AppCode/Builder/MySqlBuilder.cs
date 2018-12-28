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
                if (Config.TableComment)
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
                        model.CsType = Config.UnKnowDbType;
                        break;
                }

                switch (t)
                {
                    case "text": model.JavaType = "String"; break;
                    case "varchar": model.JavaType = "String"; break;
                    case "nvarchar": model.CsType = "String"; break;
                    case "varchar2": model.CsType = "String"; break;
                    case "nvarchar2": model.CsType = "String"; break;
                    case "enum": model.CsType = "String"; break;
                    case "date": model.JavaType = "Date"; break;
                    case "datetime": model.JavaType = "Timestamp"; break;
                    case "time": model.JavaType = "Time"; break;
                    case "tinyint": model.JavaType = "int"; break;
                    case "smallint": model.JavaType = "int"; break;
                    case "int": model.JavaType = "int"; break;
                    case "bigint": model.JavaType = "BigInteger"; break;
                    case "bit": model.JavaType = "boolean"; break;
                    case "char": model.JavaType = "String"; break;
                    case "integer": model.JavaType = "long"; break;
                    case "decimal": model.JavaType = "BigDecimal"; break;
                    case "float": model.JavaType = "float"; break;
                    case "timestamp": model.JavaType = "Timestamp"; break;
                    default:
                        model.JavaType = Config.UnKnowDbType;
                        break;
                }

                model.DbType = item.Type;
                if (Config.ColumnComment)
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
