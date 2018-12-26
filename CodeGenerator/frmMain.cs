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

        //窗体加载
        private void frmMain_Load(object sender, EventArgs e)
        {

        }

        //选择模板
        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = openFileDialog1.FileName;
            }
        }

        //输出路径
        private void button3_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox4.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        //开始
        private void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text.Trim()))
            {
                MessageBox.Show("please select 模板template");
                return;
            }
            if (string.IsNullOrEmpty(textBox2.Text.Trim()))
            {
                MessageBox.Show("please fill in 命名空间namespace");
                return;
            }

            if (string.IsNullOrEmpty(textBox4.Text.Trim()))
            {
                MessageBox.Show("please select 输出路径(output dir)");
                return;
            }

            frmDatabase win = new frmDatabase();
            win.ShowDialog();

        }




    }
}
