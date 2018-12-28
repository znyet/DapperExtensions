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
                string t = columnType.ToLower();
                switch (t)
                {
                    case "bigint": model.CsType = "long"; break;
                    case "biguint": model.CsType = "long"; break;
                    case "binary": model.CsType = "byte[]"; break;
                    case "bit": model.CsType = "boolean"; break;
                    case "blob": model.CsType = "byte[]"; break;
                    case "bool": model.CsType = "boolean"; break;
                    case "boolean": model.CsType = "boolean"; break;
                    case "char": model.CsType = "ansistringfixedlength"; break;
                    case "clob": model.CsType = "string"; break;
                    case "counter": model.CsType = "long"; break;
                    case "currency": model.CsType = "decimal"; break;
                    case "date": model.CsType = "datetime"; break;
                    case "datetime": model.CsType = "datetime"; break;
                    case "decimal": model.CsType = "decimal"; break;
                    case "double": model.CsType = "double"; break;
                    case "float": model.CsType = "double"; break;
                    case "general": model.CsType = "byte[]"; break;
                    case "guid": model.CsType = "guid"; break;
                    case "identity": model.CsType = "long"; break;
                    case "image": model.CsType = "byte[]"; break;
                    case "int": model.CsType = "int"; break;
                    case "int8": model.CsType = "sbyte"; break;
                    case "int16": model.CsType = "short"; break;
                    case "int32": model.CsType = "int"; break;
                    case "int64": model.CsType = "long"; break;
                    case "integer": model.CsType = "long"; break;
                    case "integer8": model.CsType = "sbyte"; break;
                    case "integer16": model.CsType = "short"; break;
                    case "integer32": model.CsType = "int"; break;
                    case "integer64": model.CsType = "long"; break;
                    case "logical": model.CsType = "boolean"; break;
                    case "long": model.CsType = "long"; break;
                    case "longchar": model.CsType = "string"; break;
                    case "longtext": model.CsType = "string"; break;
                    case "longvarchar": model.CsType = "string"; break;
                    case "memo": model.CsType = "string"; break;
                    case "money": model.CsType = "decimal"; break;
                    case "nchar": model.CsType = "stringfixedlength"; break;
                    case "note": model.CsType = "string"; break;
                    case "ntext": model.CsType = "string"; break;
                    case "number": model.CsType = "decimal"; break;
                    case "numeric": model.CsType = "decimal"; break;
                    case "nvarchar": model.CsType = "string"; break;
                    case "oleobject": model.CsType = "byte[]"; break;
                    case "raw": model.CsType = "byte[]"; break;
                    case "real": model.CsType = "double"; break;
                    case "single": model.CsType = "single"; break;
                    case "smalldate": model.CsType = "datetime"; break;
                    case "smallint": model.CsType = "short"; break;
                    case "smalluint": model.CsType = "ushort"; break;
                    case "string": model.CsType = "string"; break;
                    case "text": model.CsType = "string"; break;
                    case "time": model.CsType = "datetime"; break;
                    case "timestamp": model.CsType = "datetime"; break;
                    case "tinyint": model.CsType = "byte"; break;
                    case "tinysint": model.CsType = "sbyte"; break;
                    case "uint": model.CsType = "uint"; break;
                    case "uint8": model.CsType = "byte"; break;
                    case "uint16": model.CsType = "ushort"; break;
                    case "uint32": model.CsType = "uint"; break;
                    case "uint64": model.CsType = "ulong"; break;
                    case "ulong": model.CsType = "ulong"; break;
                    case "uniqueidentifier": model.CsType = "guid"; break;
                    case "unsignedinteger": model.CsType = "ulong"; break;
                    case "unsignedinteger8": model.CsType = "byte"; break;
                    case "unsignedinteger16": model.CsType = "ushort"; break;
                    case "unsignedinteger32": model.CsType = "ulong"; break;
                    case "unsignedinteger64": model.CsType = "ulong"; break;
                    case "varbinary": model.CsType = "byte[]"; break;
                    case "varchar": model.CsType = "ansistring"; break;
                    case "varchar2": model.CsType = "ansistring"; break;
                    case "yesno": model.CsType = "boolean"; break;
                    default:
                        model.CsType = Config.UnKnowDbType;
                        break;
                }

                model.JavaType = model.CsType;

                model.DbType = item.type;
                model.AllowNull = Convert.ToString(item.notnull); //是否允许空
                model.DefaultValue = item.dflt_value; //默认值

                columnList.Add(model);
            }
            return columnList;
        }
    }
}
