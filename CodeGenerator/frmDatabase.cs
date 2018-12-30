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


        #region Method

        //打开选择表窗体
        private void ShowSelectTable()
        {
            frmTable win = new frmTable();
            win.ShowDialog();
        }

        //加载配置文件
        private void LoadConfig()
        {
            txtSqlserver.Text = ConfigHelper.SqlserverConnectionString;
            txtMysql.Text = ConfigHelper.MysqlConnectionString;
            txtSqlite.Text = ConfigHelper.SqliteConnectionString;
            txtPostgresql.Text = ConfigHelper.PostgresqlConnectionString;
            txtOracle.Text = ConfigHelper.OracleConnectionString;
        }

        //保存配置文件
        private void SaveConfig(string dbType, string connectionString)
        {
            ConfigHelper.SqlserverConnectionString = txtSqlserver.Text;
            ConfigHelper.MysqlConnectionString = txtMysql.Text;
            ConfigHelper.SqliteConnectionString = txtSqlite.Text;
            ConfigHelper.PostgresqlConnectionString = txtPostgresql.Text;
            ConfigHelper.OracleConnectionString = txtOracle.Text;

            ConfigHelper.DbType = dbType;
            ConfigHelper.ConnectionString = connectionString;

            ConfigHelper.SaveConfigFile();
        }

        //检查数据库连接是否正常
        private bool CheckConn()
        {
            bool ok = true;
            try
            {
                using (var conn = DbHelper.GetConn())
                {

                }
            }
            catch (Exception ex)
            {
                ok = false;
                MessageBox.Show(this, ex.Message);
            }

            return ok;

        }


        #endregion

        //窗体加载事件
        private void frmDatabase_Load(object sender, EventArgs e)
        {
            LoadConfig();
        }

        //SqlServer
        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtSqlserver.Text.Trim()))
                return;
            SaveConfig("sqlserver", txtSqlserver.Text.Trim());
            if (!CheckConn())
                return;
            ShowSelectTable();
        }

        //MySql
        private void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtMysql.Text.Trim()))
                return;
            SaveConfig("mysql", txtMysql.Text.Trim());
            if (!CheckConn())
                return;
            ShowSelectTable();
        }

        //Sqlite
        private void button3_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtSqlite.Text.Trim()))
                return;
            SaveConfig("sqlite", txtSqlite.Text.Trim());
            if (!CheckConn())
                return;
            ShowSelectTable();
        }

        //Postgresql
        private void button4_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtPostgresql.Text.Trim()))
                return;
            SaveConfig("postgresql", txtPostgresql.Text.Trim());
            if (!CheckConn())
                return;
            ShowSelectTable();
        }

        //Oracle
        private void button5_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtOracle.Text.Trim()))
                return;
            SaveConfig("oracle", txtOracle.Text.Trim());
            if (!CheckConn())
                return;
            ShowSelectTable();
        }


    }
}
