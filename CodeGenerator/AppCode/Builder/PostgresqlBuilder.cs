using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Dapper;

namespace CodeGenerator
{
    internal class PostgresqlBuilder : IBuilder
    {

        public PostgresqlBuilder()
        {

        }

        public List<TableEntity> GetTableList()
        {
            string sql = @"select a.relname as name , b.description as value from pg_class a 
left join (select * from pg_description where objsubid =0 ) b on a.oid = b.objoid
where a.relname in (select tablename from pg_tables where schemaname = 'public')
order by a.relname asc";

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
                if (ConfigHelper.TableComment)
                {
                    model.Comment = item.value;
                }
                tableList.Add(model);
            }
            return tableList;
        }

        public List<ColumnEntity> GetColumnList(TableEntity tableEntity)
        {
            string sql1 = "set session \"myapp.name\"='" + tableEntity.Name + "';";  //this only select name and comment
            sql1 += "SELECT ";
            sql1 += "a.attname as name,";
            sql1 += "col_description(a.attrelid,a.attnum) as comment, ";
            sql1 += "concat_ws('',t.typname,SUBSTRING(format_type(a.atttypid,a.atttypmod) from '\\(.*\\)')) as type ";
            sql1 += "FROM pg_class as c,pg_attribute as a, pg_type t ";
            sql1 += "where c.relname=current_setting('myapp.name') and a.attrelid =c.oid and a.attnum>0 and a.atttypid=t.oid ";



            string sql2 = "set session \"myapp.name\" = '" + tableEntity.Name + "';";
            sql2 += @"select 
column_name as name,
is_nullable as cannull,
column_default as defaultval,
case  when position('nextval' in column_default)>0 then '1' else '0' end as isidentity, 
case when b.pk_name is null then '0' else '1' end as ispk,
c.DeText as detext
from information_schema.columns 
left join (
    select pg_attr.attname as colname,pg_constraint.conname as pk_name from pg_constraint  
    inner join pg_class on pg_constraint.conrelid = pg_class.oid 
    inner join pg_attribute pg_attr on pg_attr.attrelid = pg_class.oid and  pg_attr.attnum = pg_constraint.conkey[1] 
    inner join pg_type on pg_type.oid = pg_attr.atttypid
    where pg_class.relname = current_setting('myapp.name') and pg_constraint.contype='p' 
) b on b.colname = information_schema.columns.column_name
left join (
    select attname,description as DeText from pg_class
    left join pg_attribute pg_attr on pg_attr.attrelid= pg_class.oid
    left join pg_description pg_desc on pg_desc.objoid = pg_attr.attrelid and pg_desc.objsubid=pg_attr.attnum
    where pg_attr.attnum>0 and pg_attr.attrelid=pg_class.oid and pg_class.relname=current_setting('myapp.name')
)c on c.attname = information_schema.columns.column_name
where table_schema='public' and table_name=current_setting('myapp.name') order by ordinal_position asc";


            IEnumerable<dynamic> data;
            IEnumerable<dynamic> data2;
            using (var conn = DbHelper.GetConn())
            {
                data = conn.Query(sql1);
                data2 = conn.Query(sql2);
            }
            List<ColumnEntity> columnList = new List<ColumnEntity>();
            foreach (var item in data)
            {
                dynamic ddd = data2.FirstOrDefault(s => s.name == item.name);

                ColumnEntity model = new ColumnEntity();
                model.Name = item.name;
                model.NameUpper = MyUtils.ToUpper(model.Name); //首字母大写
                model.NameLower = MyUtils.ToLower(model.Name); //首字母小写

                if (ddd.ispk == "1")
                {
                    tableEntity.KeyName = model.Name;
                    if (ddd.isidentity == "1")
                    {
                        tableEntity.IsIdentity = "true";
                    }
                }

                string columnType = item.type;//数据类型

                if (string.IsNullOrEmpty(columnType))
                {
                    columnType = "";
                }

                string t = columnType.Split('(')[0].ToLower();

                var cs = ConfigHelper.DbTypeDictionary[ConfigHelper.PostgreSqlCSharp].FirstOrDefault(f => f.Name == t);
                if (cs != null)
                    model.CsType = cs.To;
                else
                    model.CsType = ConfigHelper.UnKnowDbType;


                var java = ConfigHelper.DbTypeDictionary[ConfigHelper.PostgreSqlJava].FirstOrDefault(f => f.Name == t);
                if (java != null)
                    model.JavaType = java.To;
                else
                    model.JavaType = ConfigHelper.UnKnowDbType;

                model.DbType = item.type;
                if (ConfigHelper.ColumnComment)
                {
                    model.Comment = ddd.detext; //说明
                }

                model.AllowNull = ddd.cannull; //是否允许空
                model.DefaultValue = ddd.defaultval; //默认值

                columnList.Add(model);
            }

            return columnList;
        }
    }
}
