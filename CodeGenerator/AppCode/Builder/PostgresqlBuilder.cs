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
                if (Config.TableComment)
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
                string t = columnType.Split('(')[0].ToLower();
                switch (t)
                {
                    case "bool": model.CsType = "bool"; break;
                    case "int2": model.CsType = "short"; break;
                    case "int4": model.CsType = "int"; break;
                    case "int8": model.CsType = "long"; break;
                    case "float4": model.CsType = "float"; break;
                    case "float8": model.CsType = "double"; break;
                    case "numeric": model.CsType = "decimal"; break;
                    case "money": model.CsType = "decimal"; break;
                    case "text": model.CsType = "string"; break;
                    case "varchar": model.CsType = "string"; break;
                    case "bpchar": model.CsType = "string"; break;
                    case "citext": model.CsType = "string"; break;
                    case "json": model.CsType = "string"; break;
                    case "jsonb": model.CsType = "string"; break;
                    case "xml": model.CsType = "string"; break;
                    case "point": model.CsType = "NpgsqlPoint"; break;
                    case "lseg": model.CsType = "NpgsqlLSeg"; break;
                    case "path": model.CsType = "NpgsqlPath"; break;
                    case "polygon": model.CsType = "NpgsqlPolygon"; break;
                    case "line": model.CsType = "NpgsqlLine"; break;
                    case "circle": model.CsType = "NpgsqlCircle"; break;
                    case "box": model.CsType = "NpgsqlBox"; break;
                    case "bit(1)": model.CsType = "bool"; break;
                    case "bit(n)": model.CsType = "BitArray"; break;
                    case "varbit": model.CsType = "BitArray"; break;
                    case "hstore": model.CsType = "IDictionary&lt;string, string&gt;"; break;
                    case "uuid": model.CsType = "Guid"; break;
                    case "cidr": model.CsType = "NpgsqlInet"; break;
                    case "inet": model.CsType = "IPAddress"; break;
                    case "macaddr": model.CsType = "PhysicalAddress"; break;
                    case "tsquery": model.CsType = "NpgsqlTsQuery"; break;
                    case "tsvector": model.CsType = "NpgsqlTsVector"; break;
                    case "date": model.CsType = "DateTime"; break;
                    case "interval": model.CsType = "TimeSpan"; break;
                    case "timestamp": model.CsType = "DateTime"; break;
                    case "timestamptz": model.CsType = "DateTime"; break;
                    case "time": model.CsType = "TimeSpan"; break;
                    case "timetz": model.CsType = "DateTimeOffset"; break;
                    case "bytea": model.CsType = "byte[]"; break;
                    case "oid": model.CsType = "uint"; break;
                    case "xid": model.CsType = "uint"; break;
                    case "cid": model.CsType = "uint"; break;
                    case "oidvector": model.CsType = "uint[]"; break;
                    case "char": model.CsType = "byte"; break;
                    case "geometry": model.CsType = "PostgisGeometry"; break;
                    case "record": model.CsType = "object[]"; break;
                    case "_bool": model.CsType = "bool[]"; break;
                    case "_int2": model.CsType = "short[]"; break;
                    case "_int4": model.CsType = "int[]"; break;
                    case "_int8": model.CsType = "long[]"; break;
                    case "_float4": model.CsType = "float[]"; break;
                    case "_float8": model.CsType = "double[]"; break;
                    case "_numeric": model.CsType = "decimal[]"; break;
                    case "_money": model.CsType = "decimal[]"; break;
                    case "_text": model.CsType = "string[]"; break;
                    case "_varchar": model.CsType = "string[]"; break;
                    case "_bpchar": model.CsType = "string[]"; break;
                    case "_citext": model.CsType = "string[]"; break;
                    case "_json": model.CsType = "string[]"; break;
                    case "_jsonb": model.CsType = "string[]"; break;
                    case "_xml": model.CsType = "string[]"; break;
                    case "_point": model.CsType = "NpgsqlPoint[]"; break;
                    case "_lseg": model.CsType = "NpgsqlLSeg[]"; break;
                    case "_path": model.CsType = "NpgsqlPath[]"; break;
                    case "_polygon": model.CsType = "NpgsqlPolygon[]"; break;
                    case "_line": model.CsType = "NpgsqlLine[]"; break;
                    case "_circle": model.CsType = "NpgsqlCircle[]"; break;
                    case "_box": model.CsType = "NpgsqlBox[]"; break;
                    case "_uuid": model.CsType = "Guid[]"; break;
                    case "_cidr": model.CsType = "NpgsqlInet[]"; break;
                    case "_inet": model.CsType = "IPAddress[]"; break;
                    case "_macaddr": model.CsType = "PhysicalAddress[]"; break;
                    case "_tsquery": model.CsType = "NpgsqlTsQuery[]"; break;
                    case "_tsvector": model.CsType = "NpgsqlTsVector[]"; break;
                    case "_date": model.CsType = "DateTime[]"; break;
                    case "_interval": model.CsType = "TimeSpan[]"; break;
                    case "_timestamp": model.CsType = "DateTime[]"; break;
                    case "_timestamptz": model.CsType = "DateTime[]"; break;
                    case "_time": model.CsType = "TimeSpan[]"; break;
                    case "_timetz": model.CsType = "DateTimeOffset[]"; break;
                    case "_oid": model.CsType = "uint[]"; break;
                    case "_xid": model.CsType = "uint[]"; break;
                    case "_cid": model.CsType = "uint[]"; break;
                    case "_geometry": model.CsType = "PostgisGeometry[]"; break;
                    default:
                        model.CsType = Config.UnKnowDbType;
                        break;
                }

                switch (t)
                {
                    case "bool": model.JavaType = "boolean"; break;
                    case "int2": model.JavaType = "int"; break;
                    case "int4": model.JavaType = "int"; break;
                    case "int8": model.JavaType = "long"; break;
                    case "float4": model.JavaType = "float"; break;
                    case "float8": model.JavaType = "double"; break;
                    case "numeric": model.JavaType = "BigDecimal"; break;
                    case "money": model.JavaType = "BigDecimal"; break;
                    case "text": model.JavaType = "String"; break;
                    case "varchar": model.JavaType = "String"; break;
                    case "bpchar": model.JavaType = "String"; break;
                    case "citext": model.JavaType = "String"; break;
                    case "json": model.JavaType = "String"; break;
                    case "jsonb": model.JavaType = "String"; break;
                    case "xml": model.JavaType = "String"; break;
                    case "point": model.JavaType = "NpgsqlPoint"; break;
                    case "lseg": model.JavaType = "NpgsqlLSeg"; break;
                    case "path": model.JavaType = "NpgsqlPath"; break;
                    case "polygon": model.JavaType = "NpgsqlPolygon"; break;
                    case "line": model.JavaType = "NpgsqlLine"; break;
                    case "circle": model.JavaType = "NpgsqlCircle"; break;
                    case "box": model.JavaType = "NpgsqlBox"; break;
                    case "bit(1)": model.JavaType = "Boolean"; break;
                    case "bit(n)": model.JavaType = "BitArray"; break;
                    case "varbit": model.JavaType = "BitArray"; break;
                    case "hstore": model.JavaType = "IDictionary&lt;string, string&gt;"; break;
                    case "uuid": model.JavaType = "UUID"; break;
                    case "cidr": model.JavaType = "NpgsqlInet"; break;
                    case "inet": model.JavaType = "IPAddress"; break;
                    case "macaddr": model.JavaType = "PhysicalAddress"; break;
                    case "tsquery": model.JavaType = "NpgsqlTsQuery"; break;
                    case "tsvector": model.JavaType = "NpgsqlTsVector"; break;
                    case "date": model.JavaType = "Date"; break;
                    case "interval": model.JavaType = "Timestamp"; break;
                    case "timestamp": model.JavaType = "Timestamp"; break;
                    case "timestamptz": model.JavaType = "Timestamp"; break;
                    case "time": model.JavaType = "TimeSpan"; break;
                    case "timetz": model.JavaType = "Timestamp"; break;
                    case "bytea": model.JavaType = "byte[]"; break;
                    case "oid": model.JavaType = "uint"; break;
                    case "xid": model.JavaType = "uint"; break;
                    case "cid": model.JavaType = "uint"; break;
                    case "oidvector": model.JavaType = "uint[]"; break;
                    case "char": model.JavaType = "Byte"; break;
                    case "geometry": model.JavaType = "PostgisGeometry"; break;
                    case "record": model.JavaType = "Object[]"; break;
                    case "_bool": model.JavaType = "boolean[]"; break;
                    case "_int2": model.JavaType = "short[]"; break;
                    case "_int4": model.JavaType = "int[]"; break;
                    case "_int8": model.JavaType = "long[]"; break;
                    case "_float4": model.JavaType = "float[]"; break;
                    case "_float8": model.JavaType = "double[]"; break;
                    case "_numeric": model.JavaType = "decimal[]"; break;
                    case "_money": model.JavaType = "decimal[]"; break;
                    case "_text": model.JavaType = "string[]"; break;
                    case "_varchar": model.JavaType = "string[]"; break;
                    case "_bpchar": model.JavaType = "string[]"; break;
                    case "_citext": model.JavaType = "string[]"; break;
                    case "_json": model.JavaType = "string[]"; break;
                    case "_jsonb": model.JavaType = "string[]"; break;
                    case "_xml": model.JavaType = "string[]"; break;
                    case "_point": model.JavaType = "NpgsqlPoint[]"; break;
                    case "_lseg": model.JavaType = "NpgsqlLSeg[]"; break;
                    case "_path": model.JavaType = "NpgsqlPath[]"; break;
                    case "_polygon": model.JavaType = "NpgsqlPolygon[]"; break;
                    case "_line": model.JavaType = "NpgsqlLine[]"; break;
                    case "_circle": model.JavaType = "NpgsqlCircle[]"; break;
                    case "_box": model.JavaType = "NpgsqlBox[]"; break;
                    case "_uuid": model.JavaType = "UUID[]"; break;
                    case "_cidr": model.JavaType = "NpgsqlInet[]"; break;
                    case "_inet": model.JavaType = "IPAddress[]"; break;
                    case "_macaddr": model.JavaType = "PhysicalAddress[]"; break;
                    case "_tsquery": model.JavaType = "NpgsqlTsQuery[]"; break;
                    case "_tsvector": model.JavaType = "NpgsqlTsVector[]"; break;
                    case "_date": model.JavaType = "DateTime[]"; break;
                    case "_interval": model.JavaType = "TimeSpan[]"; break;
                    case "_timestamp": model.JavaType = "DateTime[]"; break;
                    case "_timestamptz": model.JavaType = "DateTime[]"; break;
                    case "_time": model.JavaType = "TimeSpan[]"; break;
                    case "_timetz": model.JavaType = "DateTimeOffset[]"; break;
                    case "_oid": model.JavaType = "uint[]"; break;
                    case "_xid": model.JavaType = "uint[]"; break;
                    case "_cid": model.JavaType = "uint[]"; break;
                    case "_geometry": model.JavaType = "PostgisGeometry[]"; break;
                    default:
                        model.JavaType = Config.UnKnowDbType;
                        break;
                }

                model.DbType = item.type;
                if (Config.ColumnComment)
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
