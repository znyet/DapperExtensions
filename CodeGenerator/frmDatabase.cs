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

        #endregion

        private void frmDatabase_Load(object sender, EventArgs e)
        {
            LoadConfig();
        }

        //SqlServer
        private void button1_Click(object sender, EventArgs e)
        {
            SaveConfig("sqlserver", txtSqlserver.Text.Trim());
            ShowSelectTable();
        }

        //MySql
        private void button2_Click(object sender, EventArgs e)
        {
            SaveConfig("mysql", txtMysql.Text.Trim());
            ShowSelectTable();
        }

        //Sqlite
        private void button3_Click(object sender, EventArgs e)
        {
            SaveConfig("sqlite", txtSqlite.Text.Trim());
            ShowSelectTable();
        }

        //Postgresql
        private void button4_Click(object sender, EventArgs e)
        {
            SaveConfig("postgresql", txtPostgresql.Text.Trim());
            ShowSelectTable();
        }

        //Oracle
        private void button5_Click(object sender, EventArgs e)
        {
            SaveConfig("oracle", txtOracle.Text.Trim());
            ShowSelectTable();
        }


    }
}
