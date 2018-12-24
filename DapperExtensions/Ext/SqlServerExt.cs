using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Dapper;
using System.Collections.Concurrent;

namespace DapperExtensions
{
    public partial class SqlServerExt
    {

        public SqlServerExt(IDbConnection conn, IDbTransaction tran = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
        {
            //base.conn = conn;
            //base.tran = tran;
            //base.buffered = buffered;
            //base.commandTimeout = commandTimeout;
            //base.commandType = commandType;
        }


    }
}
