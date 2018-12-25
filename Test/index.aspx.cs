using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using DapperExtensions;
using System.Web.Script.Serialization;

namespace Test
{
    public partial class index : System.Web.UI.Page
    {
        JavaScriptSerializer js = new JavaScriptSerializer();

        int second = DateTime.Now.Second;
        int roweffect;

        string txt = null;

        public void ShowEffect()
        {
            TextBox1.Text = roweffect.ToString();
        }

        public void ShowText()
        {
            TextBox1.Text = txt;
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        //Insert
        protected void Button1_Click(object sender, EventArgs e)
        {
            using (var conn = DbHelper.GetConn())
            {
                People p = new People();
                p.Name = "张三" + second;
                p.Sex = second;
                p.Age = second;
                p.AddTime = DateTime.Now;
                roweffect += conn.Insert(p);

                School s = new School();
                s.Id = Guid.NewGuid().ToString();
                s.Name = "李四" + second;
                roweffect += conn.Insert(s);

                Student ss = new Student();
                ss.Id = second;
                ss.Name = "马六" + second;
                roweffect += conn.Insert(ss);


                ShowEffect();
            }
        }

        //InsertAsync
        protected async void Button3_Click(object sender, EventArgs e)
        {
            using (var conn = DbHelper.GetConn())
            {
                //People p = new People();
                //p.Name = "王五" + second + "Async";
                //p.Sex = second;
                //p.Age = second;
                //p.AddTime = DateTime.Now;
                //roweffect += await conn.InsertAsync(p);
                //ShowEffect();
            }
        }

        //InsertIdentityKey
        protected void Button2_Click(object sender, EventArgs e)
        {
            using (var conn = DbHelper.GetConn())
            {
                People p = new People();
                p.Id = 1;
                p.Name = "张三" + second;
                p.Sex = second;
                p.Age = second;
                p.AddTime = DateTime.Now;
                roweffect = conn.InsertIdentity(p);
                
                ShowEffect();

            }

        }

        //DeleteByIds
        protected void Button4_Click(object sender, EventArgs e)
        {
            using (var conn = DbHelper.GetConn())
            {
                IEnumerable<int> ids = new List<int>() { 1, 2, 3 };
                //int[] ids = new int[] {1,2,3 };
                roweffect += conn.DeleteByIds<People>(ids);
                ShowEffect();
            }
        }





    }
}