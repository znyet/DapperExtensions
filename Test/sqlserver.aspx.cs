using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Data.SqlClient;

namespace Test
{
    public partial class sqlserver : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            IDbConnection conn = new SqlConnection();
            Response.Write(conn.ToString());
        }
    }
}