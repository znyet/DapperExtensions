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
                if (ConfigHelper.TableComment)
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
                string t = item.DbType;//数据类型

                if (string.IsNullOrEmpty(t))
                {
                    t = "";
                }

                var cs = ConfigHelper.DbTypeDictionary[ConfigHelper.OracleCSharp].FirstOrDefault(f => f.Name == t);
                if (cs != null)
                    model.CsType = cs.To;
                else
                    model.CsType = ConfigHelper.UnKnowDbType;

                var java = ConfigHelper.DbTypeDictionary[ConfigHelper.OracleJava].FirstOrDefault(f => f.Name == t);
                if (java != null)
                    model.JavaType = java.To;
                else
                    model.JavaType = ConfigHelper.UnKnowDbType;

                model.DbType = item.DbType + "," + item.DataLength;
                if (ConfigHelper.ColumnComment)
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
