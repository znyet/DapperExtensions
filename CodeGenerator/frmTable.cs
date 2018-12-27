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
    public partial class frmTable : Form
    {

        #region Method

        private void GetSelectRows()
        {
            // DataGridCell cel=(sender as DataGridCell).
            int count = Convert.ToInt16(this.dataGridView1.Rows.Count.ToString());
            for (int i = 0; i < count; i++)
            {
                DataGridViewCheckBoxCell checkCell = (DataGridViewCheckBoxCell)dataGridView1.Rows[i].Cells["Check"];
                Boolean flag = Convert.ToBoolean(checkCell.Value);
                if (flag == true) //查找被选择的数据行
                {
                    MessageBox.Show(i.ToString());
                }
                continue;
            }
        }

        //全选
        private void SelectAll()
        {
            int count = Convert.ToInt16(this.dataGridView1.Rows.Count.ToString());
            for (int i = 0; i < count; i++)
            {
                DataGridViewCheckBoxCell checkCell = (DataGridViewCheckBoxCell)dataGridView1.Rows[i].Cells["Check"];
                Boolean flag = Convert.ToBoolean(checkCell.Value);
                if (flag == false) //查找被选择的数据行
                {
                    checkCell.Value = true;
                }
                continue;
            }

        }

        //全不选
        private void UnSelectAll()
        {
            int count = Convert.ToInt16(this.dataGridView1.Rows.Count.ToString());
            for (int i = 0; i < count; i++)
            {
                DataGridViewCheckBoxCell checkCell = (DataGridViewCheckBoxCell)dataGridView1.Rows[i].Cells["Check"];
                Boolean flag = Convert.ToBoolean(checkCell.Value);
                if (flag == true) //查找被选择的数据行
                {
                    checkCell.Value = false;

                }
                continue;
            }

        }


        #endregion

        public frmTable()
        {
            InitializeComponent();
        }

        private void frmTable_Load(object sender, EventArgs e)
        {
            dataGridView1.DefaultCellStyle.SelectionBackColor = Color.White;
            dataGridView1.DefaultCellStyle.SelectionForeColor = Color.Black;

            //禁止以列排序;
            for (int i = 0; i < dataGridView1.Columns.Count; i++)
            {
                dataGridView1.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            //AddCheckBox();
            dataGridView1.Rows.Add();
            dataGridView1.Rows[0].Cells[1].Value = "People";
            dataGridView1.Rows[0].Cells[2].Value = "人类表";
            dataGridView1.Rows.Add();
            dataGridView1.Rows[1].Cells[1].Value = "Student";
            dataGridView1.Rows[1].Cells[2].Value = "学生表";



        }

        //开始
        private void button1_Click(object sender, EventArgs e)
        {
            //System.Diagnostics.Process.Start(@"");
            UTF8Encoding utf8 = new UTF8Encoding(false); //no bom
            GetSelectRows();

        }

        //行鼠标点击事件
        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex < 0)
            {

                return;
            }
            //checkbox 勾上
            if ((bool)dataGridView1.Rows[e.RowIndex].Cells[0].EditedFormattedValue == true)
            {
                this.dataGridView1.Rows[e.RowIndex].Cells[0].Value = false;
            }
            else
            {
                this.dataGridView1.Rows[e.RowIndex].Cells[0].Value = true;
            }
        }

        //全选
        private void button3_Click(object sender, EventArgs e)
        {
            SelectAll();
        }

        //不选
        private void button2_Click(object sender, EventArgs e)
        {
            UnSelectAll();
        }

        //添加行号
        private void dataGridView1_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
             System.Drawing.Rectangle rectangle = new Rectangle(e.RowBounds.Location.X, e.RowBounds.Y, this.dataGridView1.RowHeadersWidth - 4, e.RowBounds.Height);
             TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(), this.dataGridView1.RowHeadersDefaultCellStyle.Font, rectangle, this.dataGridView1.RowHeadersDefaultCellStyle.ForeColor, TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        }

        //退出
        private void button4_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }



    }
}
