using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SuperMapTool;

namespace DataConver
{
    public partial class TIFF : Form
    {
        private ImportTool importTool;//定义工具类
        public TIFF()
        {
            InitializeComponent();
            SuperMap.Data.Workspace m_workspace = new SuperMap.Data.Workspace();
            importTool = new ImportTool(m_workspace);
        }


        private void but_output_Click(object sender, EventArgs e)
        {
            importTool.openFile(ReferCz);
        }

        private void but_input_Click(object sender, EventArgs e)
        {
            importTool.openFile(ReferShp);
        }

        private void buttonX1_Click(object sender, EventArgs e)
        {
            importTool.openFile(ReferTxt);
        }
    }
}
