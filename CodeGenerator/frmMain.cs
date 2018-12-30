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
            if (string.IsNullOrEmpty(ConfigHelper.Template))
            {
                ConfigHelper.Template = ConfigHelper.ApplicationPath + "Template\\ModelDapperExtensions.txt";
            }
            txtTemplate.Text = ConfigHelper.Template;

            //文件输出路径
            if (string.IsNullOrEmpty(ConfigHelper.OutPutDir))
            {
                ConfigHelper.OutPutDir = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\Entity";

            }
            txtOutPutDir.Text = ConfigHelper.OutPutDir;

            txtNameSpace.Text = ConfigHelper.NameSpace;
            txtClassSuffix.Text = ConfigHelper.ClassSuffix;
            txtFileType.Text = ConfigHelper.FileType;
            txtUnknowDbType.Text = ConfigHelper.UnKnowDbType;
            cbxEncoding.SelectedIndex = cbxEncoding.Items.IndexOf(ConfigHelper.FileEncoding);
            checkBox1.Checked = ConfigHelper.TableComment;
            checkBox2.Checked = ConfigHelper.ColumnComment;
        }

        //保存配置文件
        private void SaveConfig()
        {
            ConfigHelper.Template = txtTemplate.Text;
            ConfigHelper.OutPutDir = txtOutPutDir.Text;
            ConfigHelper.NameSpace = txtNameSpace.Text;
            ConfigHelper.ClassSuffix = txtClassSuffix.Text;
            ConfigHelper.FileType = txtFileType.Text;
            ConfigHelper.FileEncoding = cbxEncoding.Text;
            ConfigHelper.TableComment = checkBox1.Checked;
            ConfigHelper.ColumnComment = checkBox2.Checked;
            ConfigHelper.UnKnowDbType = txtUnknowDbType.Text;

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
            openFileDialog1.InitialDirectory = ConfigHelper.ApplicationPath + "Template";
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
