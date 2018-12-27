using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator
{
    internal interface IBuilder
    {
        List<TableEntity> GetTableList();
        List<ColumnEntity> GetColumnList(TableEntity tableEntity);
    }
}
