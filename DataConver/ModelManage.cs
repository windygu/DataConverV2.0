using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace DataConver
{
    public partial class ModelManage : Form
    {
        private Setting set;
        public ModelManage()
        {
            InitializeComponent();
            listviewSet(this.listView1);
            set = new Setting();
        }

        private void listviewSet(System.Windows.Forms.ListView Deal_Data)
        {
            Deal_Data.SmallImageList = imageList1;
            Deal_Data.MultiSelect = false;
            Deal_Data.GridLines = true;
            Deal_Data.FullRowSelect = true;
            Deal_Data.CheckBoxes = false;
            Deal_Data.View = View.Details;
        }

        private void ModelManage_Load(object sender, EventArgs e)
        {
            string but_name = "change";
            listView1.Columns.Add("地图模板名称", 150);
            listView1.Columns.Add("描述", 150);
            listView1.Columns.Add("操作");
            DirectoryInfo di = new DirectoryInfo(set.mod);
            int y = 30;
            int i = 1;
            foreach (FileInfo ff in di.GetFiles("*.xml"))
            {
                ListViewItem lv = new ListViewItem();
                Button button = new Button();
                button.Location = new Point(300, y);
                button.Name = but_name + i.ToString();
                button.Text = "更换";
                button.Width = button.Width - 10;
                button.Click += button_Click;
                lv.ImageIndex = 0;
                lv.Text = ff.Name;
                lv.SubItems.Add("这是一个地图模板");
                listView1.Items.Add(lv);
                listView1.Controls.Add(button);
                y = y + button.Height + 10;
                i++;
            }

        }

        void button_Click(object sender, EventArgs e)
        {

            OpenFileDialog sfd = new OpenFileDialog();
            sfd.Title = "打开";
            sfd.Filter = "地图模板|*.xml";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                if (MessageBox.Show("是否确定更换地图模板文件：" + sfd.SafeFileName, "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {
                    File.Replace(sfd.FileName, set.mod + "\\" + sfd.SafeFileName, null, true);
                    MessageBox.Show("地图模板：" + sfd.SafeFileName + "更新完成");
                }
                else
                    return;
            }
        }
    }
}
