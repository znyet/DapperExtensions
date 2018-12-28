using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace CodeGenerator
{
    internal class OracleBuilder : IBuilder
    {

        public OracleBuilder()
        {

        }

        public List<TableEntity> GetTableList()
        {
            throw new NotImplementedException();
        }

        public List<ColumnEntity> GetColumnList(TableEntity tableEntity)
        {
            throw new NotImplementedException();
        }
    }
}
