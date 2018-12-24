using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DapperExtensions
{
    public partial class MySqlExt
    {
        public MySqlExt(IDbConnection conn, IDbTransaction tran = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
        {
            //base.conn = conn;
            //base.tran = tran;
            //base.buffered = buffered;
            //base.commandTimeout = commandTimeout;
            //base.commandType = commandType;
        }
    }
}
