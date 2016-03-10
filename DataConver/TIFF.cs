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
        private DataConver.DataConverTool dct ;
        private string savaPathInitialize;
        private string shpTrueName;
        //private ESRI.ArcGIS.Geoprocessor.Geoprocessor gp = new ESRI.ArcGIS.Geoprocessor.Geoprocessor();
        public TIFF(string outPut_Path)
        {
            InitializeComponent();
            this.savaPathInitialize = outPut_Path;
            SuperMap.Data.Workspace m_workspace = new SuperMap.Data.Workspace();
            importTool = new ImportTool(m_workspace);
            dct = new DataConverTool(this.ExtractMesg, null, this.progressView,null,  null);
            this.progressView.Properties.Stopped = true;
            
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

        private void but_Sava_Click(object sender, EventArgs e)
        {
            importTool.openFolder(SavaFolder);
        }

        private void but_tif_Click(object sender, EventArgs e)
        {
            if (checkFile(ReferTxt.Text) && checkPath(SavaFolder.Text) && checkFile(ReferShp.Text))
            {
                this.progressView.Properties.Stopped = false;
                string shpfilePath = ReferShp.Text.Substring(0, ReferShp.Text.LastIndexOf("\\"));
                string shpFileName = ReferShp.Text.Substring(ReferShp.Text.LastIndexOf("\\") + 1);
                shpTrueName = shpFileName.Substring(0, shpFileName.Length - 4);
                dct.readTXT(ReferTxt.Text, shpfilePath, shpFileName, SavaFolder.Text + "\\tiffPath\\" + shpTrueName);
                this.progressView.Properties.Stopped = true;
                progressView.EditValue = "完成！";
                MessageBox.Show("生成淹没过程完成！");
            }
            else
                return;

        }

        private void buttonX2_Click(object sender, EventArgs e)
        {

            if (checkPath(SavaFolder.Text) && checkFile(ReferCz.Text) && checkFile(ReferShp.Text))
            {
                string shpFileName = ReferShp.Text.Substring(ReferShp.Text.LastIndexOf("\\") + 1);
                string shpTrueName = shpFileName.Substring(0, shpFileName.Length - 4);
                this.progressView.Properties.Stopped = false;
                dct.ymStatistics(SavaFolder.Text + "\\tiffPath\\" + shpTrueName + "\\shp文件", ReferCz.Text);
                this.progressView.Properties.Stopped = true;
                progressView.EditValue = "完成！";
                MessageBox.Show("统计完成！");

            }
            else
                return;
        }
        private bool checkFile(string FilePath)
        {
            if (!System.IO.File.Exists(FilePath) || FilePath == null)//|| !System.IO.Directory.Exists(ymgc_tifPath.Text)
            {
                MessageBox.Show("请输入正确编制单元路径···", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            else
                return true;
        }
        private bool checkPath(string Path)
        {
            if (!System.IO.Directory.Exists(Path) || Path == null)//|| !System.IO.Directory.Exists(ymgc_tifPath.Text)
            {
                MessageBox.Show("请输入正确编制单元路径···", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            else
                return true;
        }

        private void TIFF_Load(object sender, EventArgs e)
        {
            SavaFolder.Text = savaPathInitialize;
        }

        private void buttonX3_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }
    }
}
