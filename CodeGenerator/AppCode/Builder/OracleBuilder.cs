using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Oracle.ManagedDataAccess.Client;
using System.Data;
using Dapper;

namespace CodeGenerator
{
    internal class OracleBuilder : IBuilder
    {

        public OracleBuilder()
        {

        }

        public List<TableEntity> GetTableList()
        {
            string sql = "SELECT T.TABLE_NAME AS \"Name\",'T' AS \"TypeName\",NVL(C.COMMENTS,T.TABLE_NAME) AS \"Description\" ";
            sql += "FROM USER_TABLES T LEFT JOIN USER_TAB_COMMENTS C ON T.TABLE_NAME = C.TABLE_NAME UNION ALL ";
            sql += "SELECT T.VIEW_NAME AS \"Name\",'V' AS \"TypeName\",NVL(C.COMMENTS,T.VIEW_NAME) AS \"Description\" FROM USER_VIEWS T ";
            sql += "LEFT JOIN USER_TAB_COMMENTS C ON T.VIEW_NAME = C.TABLE_NAME";

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
                    model.Comment = item.Description;
                }
                tableList.Add(model);
            }
            return tableList;
        }

        public List<ColumnEntity> GetColumnList(TableEntity tableEntity)
        {
            string sql = "SELECT C.COLUMN_ID AS \"Id\",C.TABLE_NAME AS \"TableId\",C.COLUMN_NAME AS \"Name\",C.DATA_TYPE AS \"DbType\",C.DATA_LENGTH AS \"DataLength\",NVL(CC.COMMENTS,C.COLUMN_NAME) AS \"Description\",";
            sql += "to_number(CASE C.NULLABLE WHEN 'N' THEN '1' ELSE '0' END) AS \"IsNullable\",";
            sql += "to_number('0') AS \"AutoIncrement\",";
            sql += "to_number(CASE WHEN P.COLUMN_NAME = C.COLUMN_NAME THEN '1' ELSE '0' END) AS \"IsPrimaryKey\" ";
            sql += "FROM USER_TAB_COLUMNS C ";
            sql += "LEFT JOIN USER_COL_COMMENTS CC ON C.TABLE_NAME = CC.TABLE_NAME AND C.COLUMN_NAME = CC.COLUMN_NAME ";
            sql += "LEFT JOIN ( ";
            sql += "SELECT CU.COLUMN_NAME FROM USER_CONS_COLUMNS CU ";
            sql += "LEFT JOIN USER_CONSTRAINTS AU ON CU.CONSTRAINT_NAME = AU.CONSTRAINT_NAME ";
            sql += "WHERE CU.TABLE_NAME = :name AND AU.CONSTRAINT_TYPE='P' ";
            sql += ")P ON C.COLUMN_NAME = P.COLUMN_NAME ";
            sql += "WHERE C.TABLE_NAME = :name ORDER BY C.COLUMN_ID";

            IEnumerable<dynamic> data;
            using (var conn = DbHelper.GetConn())
            {
                data = conn.Query(sql, new { name = tableEntity.Name });
            }

            List<ColumnEntity> columnList = new List<ColumnEntity>();
            foreach (var item in data)
            {
                ColumnEntity model = new ColumnEntity();
                model.Name = item.Name; //列表
                model.NameUpper = MyUtils.ToUpper(model.Name); //首字母大写
                model.NameLower = MyUtils.ToLower(model.Name); //首字母小写

                if (item.IsPrimaryKey.ToString() == "1")
                {
                    tableEntity.KeyName = model.Name;
                    if (item.AutoIncrement.ToString() == "1")
                    {
                        tableEntity.IsIdentity = "true";
                    }
                }
                string columnType = item.DbType;//数据类型
                switch (columnType)
                {
                    case "IMAGE": model.CsType = "byte[]"; break;
                    case "TEXT": model.CsType = "string"; break;
                    case "NTEXT": model.CsType = "string"; break;
                    case "VARCHAR": model.CsType = "string"; break;
                    case "NVARCHAR": model.CsType = "string"; break;
                    case "VARCHAR2": model.CsType = "string"; break;
                    case "NVARCHAR2": model.CsType = "string"; break;
                    case "CLOB": model.CsType = "string"; break;
                    case "XML": model.CsType = "string"; break;
                    case "UNIQUEIDENTIFIER": model.CsType = "Guid"; break;
                    case "DATE": model.CsType = "DateTime"; break;
                    case "SMALLDATETIME": model.CsType = "DateTime"; break;
                    case "DATETIME": model.CsType = "DateTime"; break;
                    case "DATETIME2": model.CsType = "DateTime"; break;
                    case "TIME": model.CsType = "TimeSpan"; break;
                    case "DATETIMEOFFSET": model.CsType = "DateTimeOffset"; break;
                    case "TINYINT": model.CsType = "byte"; break;
                    case "SMALLINT": model.CsType = "short"; break;
                    case "INT": model.CsType = "int"; break;
                    case "BIGINT": model.CsType = "long"; break;
                    case "BIT": model.CsType = "bool"; break;
                    case "CHAR": model.CsType = "string"; break;
                    case "NCHAR": model.CsType = "string"; break;
                    case "UNIQUEIDE": model.CsType = "Guid"; break;
                    case "NUMERIC": model.CsType = "decimal"; break;
                    case "INTEGER": model.CsType = "int"; break;
                    case "MONEY": model.CsType = "decimal"; break;
                    case "REAL": model.CsType = "decimal"; break;
                    case "BLOB": model.CsType = "byte[]"; break;
                    case "SINGLE": model.CsType = "float"; break;
                    case "SMALLMONEY": model.CsType = "decimal"; break;
                    case "DECIMAL": model.CsType = "decimal"; break;
                    case "BFILE": model.CsType = "byte[]"; break;
                    case "FLOAT": model.CsType = "float"; break;
                    case "BINARY": model.CsType = "byte[]"; break;
                    case "NUMBER": model.CsType = "decimal"; break;
                    case "VARBINARY": model.CsType = "byte[]"; break;
                    case "LONG": model.CsType = "byte[]"; break;
                    case "LONGTEXT": model.CsType = "string"; break;
                    case "TIMESTAMP": model.CsType = "DateTime"; break;
                    case "SDO_GEOMETRY": model.CsType = "string"; break;
                    default:
                        model.CsType = Config.UnKnowDbType;
                        break;
                }

                model.JavaType = model.CsType;

                model.DbType = item.DbType + "," + item.DataLength;
                if (Config.ColumnComment)
                {
                    model.Comment = item.Description; //说明
                }
                model.AllowNull = item.IsNullable.ToString(); //是否允许空
                model.DefaultValue = item.DefaultValue; //默认值

                columnList.Add(model);
            }

            return columnList;
        }
    }
}
