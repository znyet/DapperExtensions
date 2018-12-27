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
            txtSqlserver.Text = Config.SqlserverConnectionString;
            txtMysql.Text = Config.MysqlConnectionString;
            txtSqlite.Text = Config.SqliteConnectionString;
            txtPostgresql.Text = Config.PostgresqlConnectionString;
            txtOracle.Text = Config.OracleConnectionString;
        }

        //保存配置文件
        private void SaveConfig(string dbType, string connectionString)
        {
            Config.SqlserverConnectionString = txtSqlserver.Text;
            Config.MysqlConnectionString = txtMysql.Text;
            Config.SqliteConnectionString = txtSqlite.Text;
            Config.PostgresqlConnectionString = txtPostgresql.Text;
            Config.OracleConnectionString = txtOracle.Text;

            Config.DbType = dbType;
            Config.ConnectionString = connectionString;

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
                MessageBox.Show(ex.Message);
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
            SaveConfig("sqlserver", txtSqlserver.Text.Trim());
            if (!CheckConn())
                return;
            ShowSelectTable();
        }

        //MySql
        private void button2_Click(object sender, EventArgs e)
        {
            SaveConfig("mysql", txtMysql.Text.Trim());
            if (!CheckConn())
                return;
            ShowSelectTable();
        }

        //Sqlite
        private void button3_Click(object sender, EventArgs e)
        {
            SaveConfig("sqlite", txtSqlite.Text.Trim());
            if (!CheckConn())
                return;
            ShowSelectTable();
        }

        //Postgresql
        private void button4_Click(object sender, EventArgs e)
        {
            SaveConfig("postgresql", txtPostgresql.Text.Trim());
            if (!CheckConn())
                return;
            ShowSelectTable();
        }

        //Oracle
        private void button5_Click(object sender, EventArgs e)
        {
            SaveConfig("oracle", txtOracle.Text.Trim());
            if (!CheckConn())
                return;
            ShowSelectTable();
        }


    }
}
