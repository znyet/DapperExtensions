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
        IDbConnection conn;
        public PostgresqlBuilder(IDbConnection conn)
        {
            this.conn = conn;
        }

        public List<TableEntity> GetTableList()
        {
            string sql = @"select a.relname as name , b.description as value from pg_class a 
left join (select * from pg_description where objsubid =0 ) b on a.oid = b.objoid
where a.relname in (select tablename from pg_tables where schemaname = 'public')
order by a.relname asc";
            IEnumerable<dynamic> data = conn.Query(sql);
            List<TableEntity> tableList = new List<TableEntity>();
            foreach (var item in data)
            {
                TableEntity model = new TableEntity();
                model.Name = item.name;
                model.Comment = item.value;
                tableList.Add(model);
            }
            return tableList;
        }

        public List<ColumnEntity> GetColumnList(TableEntity tableEntity)
        {
            throw new NotImplementedException();
        }
    }
}
