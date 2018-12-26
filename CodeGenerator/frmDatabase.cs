using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CodeGenerator
{
    public partial class frmDatabase : Form
    {
        public frmDatabase()
        {
            InitializeComponent();
        }

        //ConnectionString Helper
        private void button6_Click(object sender, EventArgs e)
        {

        }

        //SqlServer
        private void button1_Click(object sender, EventArgs e)
        {
            ShowSelectTable("sqlserver", textBox1.Text.Trim());
        }

        //MySql
        private void button2_Click(object sender, EventArgs e)
        {
            ShowSelectTable("mysql", textBox2.Text.Trim());
        }

        //Sqlite
        private void button3_Click(object sender, EventArgs e)
        {
            ShowSelectTable("sqlite", textBox3.Text.Trim());
        }

        //Postgresql
        private void button4_Click(object sender, EventArgs e)
        {
            ShowSelectTable("postgresql", textBox4.Text.Trim());
        }

        //Oracle
        private void button5_Click(object sender, EventArgs e)
        {
            ShowSelectTable("oracle", textBox5.Text.Trim());
        }

        #region Method

        private void ShowSelectTable(string dbType, string connectionString)
        {
            frmTable win = new frmTable();
            win.ShowDialog();
        }

        #endregion

    }
}
