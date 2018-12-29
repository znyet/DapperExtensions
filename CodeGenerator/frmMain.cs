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
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        #region Method

        //加载配置文件
        private void LoadConfig()
        {
            //模板路径
            if (string.IsNullOrEmpty(Config.Template))
            {
                Config.Template = Config.ApplicationPath + "Template\\ModelDapperExtensions.txt";
            }
            txtTemplate.Text = Config.Template;

            //文件输出路径
            if (string.IsNullOrEmpty(Config.OutPutDir))
            {
                Config.OutPutDir = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\Entity";

            }
            txtOutPutDir.Text = Config.OutPutDir;

            txtNameSpace.Text = Config.NameSpace;
            txtClassSuffix.Text = Config.ClassSuffix;
            txtFileType.Text = Config.FileType;
            txtUnknowDbType.Text = Config.UnKnowDbType;
            cbxEncoding.SelectedIndex = cbxEncoding.Items.IndexOf(Config.FileEncoding);
            checkBox1.Checked = Config.TableComment;
            checkBox2.Checked = Config.ColumnComment;
        }

        //保存配置文件
        private void SaveConfig()
        {
            Config.Template = txtTemplate.Text;
            Config.OutPutDir = txtOutPutDir.Text;
            Config.NameSpace = txtNameSpace.Text;
            Config.ClassSuffix = txtClassSuffix.Text;
            Config.FileType = txtFileType.Text;
            Config.FileEncoding = cbxEncoding.Text;
            Config.TableComment = checkBox1.Checked;
            Config.ColumnComment = checkBox2.Checked;
            Config.UnKnowDbType =txtUnknowDbType.Text;

            ConfigHelper.SaveConfigFile();

        }

        #endregion


        //窗体加载
        private void frmMain_Load(object sender, EventArgs e)
        {
            LoadConfig();
        }

        //选择模板
        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.InitialDirectory = Config.ApplicationPath + "Template";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                txtTemplate.Text = openFileDialog1.FileName;
            }
        }

        //输出路径
        private void button3_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                txtOutPutDir.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        //选择数据库按钮
        private void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtTemplate.Text.Trim()))
            {
                MessageBox.Show("please select 模板template");
                return;
            }
            if (string.IsNullOrEmpty(txtNameSpace.Text.Trim()))
            {
                MessageBox.Show("please fill in 命名空间namespace");
                return;
            }

            if (string.IsNullOrEmpty(txtOutPutDir.Text.Trim()))
            {
                MessageBox.Show("please select 输出路径(output dir)");
                return;
            }
            if (string.IsNullOrEmpty(txtFileType.Text.Trim()))
            {
                MessageBox.Show("please fill in 文件类型(filetype)");
                return;
            }
            if (string.IsNullOrEmpty(txtUnknowDbType.Text.Trim()))
            {
                MessageBox.Show("please fill in unknow type");
                return;
            }
            SaveConfig();
            frmDatabase win = new frmDatabase();
            win.ShowDialog();

        }




    }
}
