using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Dapper;
using System.Data;

namespace CodeGenerator
{
    internal class SqlServerBuilder : IBuilder
    {
        public SqlServerBuilder()
        {

        }

        public List<TableEntity> GetTableList()
        {
            string sql = @"select ROW_NUMBER() OVER (ORDER BY a.name) AS Num, 
a.name AS Name,
CONVERT(NVARCHAR(100),isnull(g.[value],'')) AS Comment
from
sys.tables a left join sys.extended_properties g
on (a.object_id = g.major_id AND g.minor_id = 0)";
            using (var conn = DbHelper.GetConn())
            {
                return conn.Query<TableEntity>(sql).ToList();
            }

        }

        public List<ColumnEntity> GetColumnList(TableEntity tableEntity)
        {
            string sql = @"SELECT  
ColumnName=a.name, 
IsKey=case when exists(SELECT 1 FROM sysobjects where xtype='PK' and name in (
  SELECT name FROM sysindexes WHERE indid in(
   SELECT indid FROM sysindexkeys WHERE id = a.id AND colid=a.colid 
   ))) then 1 else 0 end, 
IsIdentity=case when COLUMNPROPERTY(a.id,a.name,'IsIdentity')=1 then 1 else 0 end, 
ColumnType=b.name, 
ColumnLength=COLUMNPROPERTY(a.id,a.name,'PRECISION'), 
DecimalDigit =isnull(COLUMNPROPERTY(a.id,a.name,'Scale'),0), 
ColumnCommnent=isnull(g.[value],''),
AllowNull=case when a.isnullable=1 then 1 else 0 end, 
DefaultValue=isnull(e.text,'')
FROM syscolumns a 
left join systypes b on a.xtype=b.xusertype 
inner join sysobjects d on a.id=d.id and d.xtype='U' and d.name<>'dtproperties' 
left join syscomments e on a.cdefault=e.id 
left join sys.extended_properties g on a.id=g.major_id and a.colid=g.minor_id 
left join sys.extended_properties f on d.id=f.major_id and f.minor_id =0 
where d.name=@name
order by a.id,a.colorder";

            IEnumerable<dynamic> data;
            using (var conn = DbHelper.GetConn())
            {
                data = conn.Query(sql, new { name = tableEntity.Name });
            }

            List<ColumnEntity> columnList = new List<ColumnEntity>();
            foreach (var item in data)
            {
                ColumnEntity model = new ColumnEntity();
                model.Name = item.ColumnName; //列表
                model.NameUpper = MyUtils.ToUpper(model.Name); //首字母大写
                model.NameLower = MyUtils.ToLower(model.Name); //首字母小写

                if (item.IsKey == 1)
                {
                    tableEntity.KeyName = model.Name;
                    if (item.IsIdentity == 1)
                    {
                        tableEntity.IsIdentity = "true";
                    }
                }
                string columnType = item.ColumnType;//数据类型
                if (string.IsNullOrEmpty(columnType)) 
                {
                    columnType = "";
                }
                string t = columnType.ToLower();

                var cs = ConfigHelper.DbTypeDictionary[ConfigHelper.SqlServerCSharp].FirstOrDefault(f => f.Name == t);
                if (cs != null)
                    model.CsType = cs.To;
                else
                    model.CsType = ConfigHelper.UnKnowDbType;


                var java = ConfigHelper.DbTypeDictionary[ConfigHelper.SqlServerJava].FirstOrDefault(f => f.Name == t);
                if (java != null)
                    model.JavaType = java.To;
                else
                    model.JavaType = ConfigHelper.UnKnowDbType;


                model.DbType = item.ColumnType + "," + item.ColumnLength + "," + item.DecimalDigit;
                if (ConfigHelper.ColumnComment)
                {
                    model.Comment = item.ColumnCommnent; //说明
                }
                model.AllowNull = item.AllowNull.ToString(); //是否允许空
                model.DefaultValue = item.DefaultValue; //默认值

                columnList.Add(model);
            }

            return columnList;

        }
    }
}
