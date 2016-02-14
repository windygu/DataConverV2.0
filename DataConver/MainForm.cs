using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

using System.Xml;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Output;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.EditorExt;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesFile;
using System.Web.Http;
using SuperMapTool;
using SuperMap.Data;
using ESRI.ArcGIS.Analyst3DTools;

using System.Threading;


namespace DataConver
{
    public partial class MainForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {
       
        public string userName;
        private SuperMap.Data.Workspace m_workspace;
        private ImportTool importTool;
        public string Undeal_heduanX;
        private string Undeal_heduan;
        private string Deal_heduanX;
        private string Deal_heduan;
        
        private Setting set = new Setting();
        private int vec_count = 0;//矢量数据处理计数
        private int ras_count = 0;//栅格数据处理计数
        private string TIFFPath = "";
        private List<string> list = new List<string>();
        private string undeal_Path = "";
        private DataConverTool DCT;
        private DirectoryInfo di;
        public string passWord = "aaaazzzz";
        private string heduan ;
        private string deal_data;
        
        //构造函数
        public MainForm()
        {
            InitializeComponent();
         
           
            importTool = new ImportTool(m_workspace);
            listviewSet(Lis_UndealData);
            listviewSet(Lis_DealData);
            DataManager();
            UISetting();

        }
        private void DataManager()
        {
            back.Enabled = back2.Enabled = true;
            heduan = set.undeal_Path;
            deal_data = set.deal_path;
            importTool.createFolder(heduan);
            importTool.createFolder(deal_data);
            //Pag_ProcessedData.Text = deal_data;
            //Pag_UndealData.Text = heduan;
            openFolder(Lis_UndealData, heduan);
            openFolder1(Lis_DealData, deal_data);

        }
        private void UISetting()
        {
            
            MessageShow.Text = "说明：此转换工具仅适用于洪水风险图——移动风险监测平板标准编制单元预处理工作,请单击开始处理执行操作···";

            //设置进度条
            progressBar.Properties.ShowTitle = true;
            progressBar.EditValue = "待命···";
            progressBar.Properties.Stopped = true;
            //设置路径
            //undeal_path.Text = System.IO.Path.Combine(undeal_Path + "\\" + list[0]);
            //passWord = textEdit1.Text;
            DCT = new DataConverTool(this.lab_progress, this.labelX3, this.progressBar, this.MessageShow,axMapControl1);
            undeal_Path = Undeal_heduan;
        }
        //用户退出
        private void barButtonItem7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DialogResult result = MessageBox.Show("您是否退出并保存所有操作？", "退出", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (result == DialogResult.OK)
            {
                this.Close();
            }
            else
                return;
           
        }
        //窗口载入
        private void MainForm_Load(object sender, EventArgs e)
        {
            int l = this.Width;
            //splitContainerControl1.SplitterPosition = l / 3;
            if (set.report != "OK")
            {
                set.StartPosition = FormStartPosition.CenterScreen;
                set.Show();
                set.TopMost = true;
            }
          
        }
        
       
        //数据转换工具
        private void Data_Cov_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //ConverTool converTool = new ConverTool(set.undeal_Path,new List<string>() );//+ "\\" + "辽河干流"
            //converTool.StartPosition = FormStartPosition.CenterScreen;
            //converTool.Show();
        }

        
        //数据管理
        private void Data_vec_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            set.set_Value();
            DataManager();
        }
        
        private void listviewSet(System.Windows.Forms.ListView Deal_Data)
        {
            Deal_Data.SmallImageList = imageList1;
            Deal_Data.MultiSelect = true;
            Deal_Data.GridLines = true;
            Deal_Data.FullRowSelect = true;
            //Deal_Data.CheckBoxes = false;
            Deal_Data.View = View.List;
        }

       
        //帮助
        private void User_help_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            string wksPath = @"D:\移动风险监测\洪湖东分块_测试数据洪湖东分块\MapResult.smwu";
            string name = "ymgc2";
            importTool.refreshMod(wksPath, name, "aaaazzzz", @"D:\移动风险监测\参考专题图模板.xml");
            ////string udbPath = @"G:\移动风险监测\已处理数据\洪湖东分块_已处理\洪湖东分块\ymgc1";
            //importTool.refreshMod(wksPath, name,"aaaazzzz");
            //importTool.ProjectConver(@"G:\移动风险监测\已处理数据\辽河干流张靠段_已处理\辽河干流张靠段\TestOfPrj.smwu", @"G:\移动风险监测\参考坐标\CGCS_2000.xml","time");
            MessageBox.Show("有关技术帮助请咨询" + "\r\n" + "北京超图有限公司", "帮助", MessageBoxButtons.OK, MessageBoxIcon.Information);
        
        }
        private void OpenFolderAndSelectFile(String fileFullName)
        {
            System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo("Explorer.exe");
            psi.Arguments = "/e,/select/," + fileFullName;
            System.Diagnostics.Process.Start(psi);
        }
        //读取XML
        public String readXML(string pathOfXML)
        {
           string ss = File.ReadAllText(pathOfXML, Encoding.Default);
            return ss;
        }
       
        private void User_set_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Setting set = new Setting();
            set.StartPosition=FormStartPosition.CenterScreen;
            set.ShowDialog();
            set.TopMost=true;
        }

        private void Model_Manage_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ModelManage model = new ModelManage();
            model.StartPosition = FormStartPosition.CenterScreen;
            model.Show();
            model.TopMost = true;
        }
        //处理函数
        private void openFolder(System.Windows.Forms.ListView lisview, string hd)
        {
            try
            {
                lisview.Items.Clear();
                DirectoryInfo di = new DirectoryInfo(hd);
                foreach (DirectoryInfo dd in di.GetDirectories())
                {
                    ListViewItem li = new ListViewItem();
                    Undeal_heduanX = dd.Parent.FullName;
                    Undeal_heduan = dd.Parent.Parent.FullName;
                    li.Text = dd.Name;
                    li.ImageIndex = 5;
                    lisview.Items.Add(li);
                }

                foreach (FileInfo mm in di.GetFiles("*.shp"))
                {
                    ListViewItem lii = new ListViewItem();
                    lii.Text = mm.Name;
                    lii.ImageIndex = 1;
                    lisview.Items.Add(lii);
                }
                foreach (FileInfo mm in di.GetFiles("*.tif"))
                {
                    ListViewItem lii = new ListViewItem();
                    lii.Text = mm.Name;
                    lii.ImageIndex = 2;
                    lisview.Items.Add(lii);
                }
                foreach (FileInfo mm in di.GetFiles("*.udb"))
                {
                    ListViewItem lii = new ListViewItem();
                    lii.Text = mm.Name;
                    lii.ImageIndex = 4;
                    lisview.Items.Add(lii);
                }
                foreach (FileInfo mm in di.GetFiles("*.xls"))
                {
                    ListViewItem lii = new ListViewItem();
                    lii.Text = mm.Name;
                    lii.ImageIndex = 6;
                    lisview.Items.Add(lii);
                }
                foreach (FileInfo mm in di.GetFiles("*.txt"))
                {
                    ListViewItem lii = new ListViewItem();
                    lii.Text = mm.Name;
                    lii.ImageIndex = 0;
                    lisview.Items.Add(lii);
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show("已是主目录");
                //Pag_UndealData.Text = set.undeal_Path;
                openFolder(Lis_UndealData, set.undeal_Path);
                
            }
        }
        private void openFolder1(System.Windows.Forms.ListView lisview, string hd)
        {
            try
            {
                lisview.Items.Clear();
                DirectoryInfo di = new DirectoryInfo(hd);
                foreach (DirectoryInfo dd in di.GetDirectories())
                {
                    ListViewItem li = new ListViewItem();
                    Deal_heduanX = dd.Parent.FullName;
                    Deal_heduan = dd.Parent.Parent.FullName;
                    li.Text = dd.Name;
                    li.ImageIndex = 5;
                    lisview.Items.Add(li);
                }
                foreach (FileInfo mm in di.GetFiles("*.shp"))
                {
                    ListViewItem lii = new ListViewItem();
                    lii.Text = mm.Name;
                    lii.ImageIndex = 1;
                    lisview.Items.Add(lii);
                }
                foreach (FileInfo mm in di.GetFiles("*.tif"))
                {
                    ListViewItem lii = new ListViewItem();
                    lii.Text = mm.Name;
                    lii.ImageIndex = 2;
                    lisview.Items.Add(lii);
                }
                foreach (FileInfo mm in di.GetFiles("*.udb"))
                {
                    ListViewItem lii = new ListViewItem();
                    lii.Text = mm.Name;
                    lii.ImageIndex = 4;
                    lisview.Items.Add(lii);
                }
                foreach (FileInfo mm in di.GetFiles("*.txt"))
                {
                    ListViewItem lii = new ListViewItem();
                    lii.Text = mm.Name;
                    lii.ImageIndex = 0;
                    lisview.Items.Add(lii);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("已是主目录");
                //Pag_ProcessMsg.Text = set.deal_path;
                openFolder1(Lis_DealData,  set.deal_path);
              
            }
        }
        //未处理数据的实现功能
        private void UnDeal_Data_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Undeal_Menu_Tool.Show(this.Lis_UndealData, e.Location);
            }
        }
        private void 打开Undeal_Menu_Tool_Click(object sender, EventArgs e)
        {
            try
            {
                if (Lis_UndealData.FocusedItem == null)
                {
                    return;
                }
                else
                {
                    Undeal_heduanX = Undeal_heduanX + "\\" + Lis_UndealData.FocusedItem.Text;
                    //Pag_UndealData.Text = Undeal_heduanX;
                    openFolder(Lis_UndealData, Undeal_heduanX);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void UnDeal_Data_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (Lis_UndealData.SelectedItems == null)
            {
                return;
            }
            else
            {
                Undeal_heduanX = Undeal_heduanX + "\\" + Lis_UndealData.FocusedItem.Text;
                //Pag_UndealData.Text = Undeal_heduanX;
                openFolder(Lis_UndealData, Undeal_heduanX);
            }
        }
        private void 返回Undeal_Menu_Tool_Click(object sender, EventArgs e)
        {
            try
            { 
                //Pag_UndealData.Text = Undeal_heduan;
                openFolder(Lis_UndealData, Undeal_heduan);
            }
            catch
            {
                MessageBox.Show("已是主目录");
                openFolder(Lis_UndealData, set.undeal_Path);
            }
        }
        private void 删除Undeal_Menu_Tool_Click(object sender, EventArgs e)
        {
            try
            {
                if (Lis_UndealData.FocusedItem == null)
                    return;
                else
                {
                    if (MessageBox.Show("确定要删除文件：" + Lis_UndealData.FocusedItem.Text, "询问", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                        Lis_UndealData.FocusedItem.Remove();
                    else
                        return;
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void 大图标Undeal_Menu_Tool_Click(object sender, EventArgs e)
        {
            Lis_UndealData.LargeImageList = imageList1;
            Lis_UndealData.View = View.LargeIcon;
        }
        private void 列表Undeal_Menu_Tool_Click(object sender, EventArgs e)
        {
            Lis_UndealData.LargeImageList = imageList1;
            Lis_UndealData.View = View.List;
        }
        private void Undeal_back_Click(object sender, EventArgs e)
        {
            try
            { 
                //Pag_UndealData.Text = Undeal_heduan;
                openFolder(Lis_UndealData, Undeal_heduan);
            }
            catch
            {
                MessageBox.Show("已是主目录");
                openFolder(Lis_UndealData, set.undeal_Path);
            }
        }
        private void 处理Undeal_Menu_Tool_Click(object sender, EventArgs e)
        {

            List<string> L = new List<string>();
            for (int i = 0; i < Lis_UndealData.Items.Count; i++)
            {
                if (Lis_UndealData.Items[i].Checked)
                {
                    L.Add(Lis_UndealData.Items[i].Text);
                }
            }
            if (L.Count == 0)
            {
                return;
            }
            else
            {

                Undeal_heduanX = Undeal_heduanX + "\\" + Lis_UndealData.FocusedItem.Text;
                //ConverTool ll = new ConverTool(Undeal_heduan, L);
                //ll.StartPosition = FormStartPosition.CenterScreen;
                //ll.Show();
                Undeal_heduanX = set.undeal_Path;
            }
        }
        //已处理数据管理
        private void Deal_Data_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Deal_Menu_Tool.Show(this.Lis_DealData, e.Location);
            }

        }
        private void 打开Deal_Menu_Tool_Click(object sender, EventArgs e)
        {
            try
            {
                if (Lis_DealData.FocusedItem == null)
                {
                    return;
                }
                else
                {
                    Deal_heduanX = Deal_heduanX + "\\" + Lis_DealData.FocusedItem.Text;
                    //Pag_ProcessMsg.Text = Deal_heduanX;
                    openFolder1(Lis_DealData, Deal_heduanX);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void Deal_Data_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (Lis_DealData.SelectedItems == null)
            {
                return;
            }
            else
            {
                Deal_heduanX = Deal_heduanX + "\\" + Lis_DealData.FocusedItem.Text;
                //Pag_ProcessMsg.Text = Deal_heduanX;
                openFolder1(Lis_DealData, Deal_heduanX);
            }
        }
        private void 返回Deal_Menu_Tool_Click(object sender, EventArgs e)
        {
            try
            {
                //Pag_ProcessMsg.Text = Deal_heduan;
                openFolder1(Lis_DealData, Deal_heduan);
            }
            catch
            {
                MessageBox.Show("已是主目录");
                openFolder1(Lis_DealData, set.deal_path);
            }
        }
        private void 删除Deal_Menu_Tool_Click(object sender, EventArgs e)
        {
            try
            {
                if (Lis_DealData.FocusedItem == null)
                    return;
                else
                {
                    if (MessageBox.Show("确定要删除文件：" + Lis_DealData.FocusedItem.Text, "询问", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                    {
                        deleteFiles(Deal_heduanX + "\\" + Lis_DealData.FocusedItem.Text);
                        Lis_DealData.FocusedItem.Remove();
                    }
                    else
                        return;
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        private void 大图标Deal_Menu_Tool_Click(object sender, EventArgs e)
        {
            Lis_DealData.LargeImageList = imageList1;
            Lis_DealData.View = View.LargeIcon;
        }
        private void 列表Deal_Menu_Tool_Click(object sender, EventArgs e)
        {
            Lis_DealData.LargeImageList = imageList1;
            Lis_DealData.View = View.List;
        }
        public static void deleteFiles(string strDir)
        {
            if (Directory.Exists(strDir))
            {
                Directory.Delete(strDir, true);
                MessageBox.Show("删除成功！");
            }
            else if (File.Exists(strDir))
            {
                File.Delete(strDir);
                MessageBox.Show("删除成功！");
            }
            else
            {
                MessageBox.Show("此目录不存在！");
            }
        }
        private void Deal_back_Click(object sender, EventArgs e)
        {
            try
            {
                //Pag_ProcessMsg.Text = Deal_heduan;
                openFolder1(Lis_DealData, Deal_heduan);
            }
            catch
            {
                MessageBox.Show("已是主目录");
                openFolder1(Lis_DealData, set.deal_path);
            }
        }

        private void buttonX1_Click(object sender, EventArgs e)
        {
            OpenFolderAndSelectFile(set.undeal_Path);
        }

        private void buttonX2_Click(object sender, EventArgs e)
        {
            OpenFolderAndSelectFile(set.deal_path);
        }

        private void but_Process_Click(object sender, EventArgs e)
        {
            test();

        }

        private void but_pw_Click(object sender, EventArgs e)
        {
            set.passWod = textEdit1.Text;
            MessageBox.Show("密码：" + set.passWod + "\r\n 设置成功！");
            password(false);
        }
        private void password(bool Y)
        {
            textEdit1.Visible = Y;
            but_pw.Enabled = Y;
            but_pw.Visible = Y;
        }

        private void but_Password_Click(object sender, EventArgs e)
        {

            passWord = Microsoft.VisualBasic.Interaction.InputBox("请输入保护数据安全的密码：", "设置密码", set.passWod);
           
            //set.passWod = password;
            // password(true);
            
        }

        private void but_Cancel_Click(object sender, EventArgs e)
        {
            //if (!but_Start.Enabled)
            //{
                if (MessageBox.Show("系统" + lab_progress.Text + "是否要终止操作？\r\n并且将会退出整个系统！", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {

                    System.Diagnostics.Process.GetCurrentProcess().Kill();
                    //this.Close();
                }
                else
                    return;

            //}
        }

        private void but_Start_Click(object sender, EventArgs e)
        {
            vec_count = 0;
            ras_count = 0;
            this.but_Start.Enabled = false;
            this.but_Start.Text = "正在处理";
            MessageShow.Clear();
            if (!System.IO.Directory.Exists(undeal_path.Text))//|| !System.IO.Directory.Exists(ymgc_tifPath.Text)
            {
                MessageBox.Show("请输入正确编制单元路径···", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);

                this.but_Start.Enabled = true;
                this.but_Start.Text = "开始处理"; return;
            }
            //this.Cursor = Cursors.WaitCursor;
            progressBar.Properties.Stopped = false;
            DateTime dtAll = DateTime.Now;
            foreach (string manyDeal in list)
            {
                undeal_path.Text = this.Undeal_heduanX + "\\" + manyDeal;
                di = new DirectoryInfo(undeal_path.Text);

                //数据检查工具
                if (di.GetDirectories("*地图数据库与图件成果").Length == 0 || di.GetDirectories("tiffPath").Length == 0 || di.GetDirectories("*风险图应用业务相关数据").Length == 0)
                {
                    MessageBox.Show("请检查" + manyDeal + "文件下数据完整性！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    continue;
                }
                //DirectoryInfo tif = new DirectoryInfo(di.GetDirectories("tiffPath")[0].FullName);
                //FileInfo[] tifFile = tif.GetFiles("*.tif", SearchOption.AllDirectories);
                //if (tifFile.Length == 0)
                //{
                //    if (MessageBox.Show("系统检查当前无淹没过程影像数据，是否进行影像数据提取？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                //    {
                //        TIFF tf = new TIFF();
                //        tf.StartPosition = FormStartPosition.CenterScreen;
                //        tf.Show();
                //        return;
                //    }
                //}
                try
                {

                    DCT.DealAll(undeal_path.Text,passWord);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

            }
            progressBar.Properties.Stopped = true;

            //this.Cursor = Cursors.Default;
            string dtall = "总用时：" + DataConverTool.ExecDateDiff(dtAll, DateTime.Now);
            //MessageBox.Show("处理完成!" + "\r\n" + dtall, "提示");
            progressBar.EditValue = lab_progress.Text;
            MessageBox.Show(lab_progress.Text + "\r\n" + dtall, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.but_Start.Enabled = true;
            this.but_Start.Text = "开始处理";
        }

        private void but_undeal_Click(object sender, EventArgs e)
        {
            importTool.openFolder(undeal_path);
        }

        private void test()
        {
            list = new List<string>();
            for (int i = 0; i < Lis_UndealData.Items.Count; i++)
            {
                if (Lis_UndealData.Items[i].Checked)
                {
                    list.Add(Lis_UndealData.Items[i].Text);
                }
               
            }
            if (list.Count == 0)
            {
                
                undeal_path.Text = null;return;
            }
            else
            {
                //Undeal_heduanX = Undeal_heduanX + "\\" + Lis_UndealData.FocusedItem.Text;
                //ConverTool ll = new ConverTool(Undeal_heduan, L);
                //ll.StartPosition = FormStartPosition.CenterScreen;
                //ll.Show();
                //Undeal_heduanX = set.undeal_Path;
                undeal_path.Text = System.IO.Path.Combine(Undeal_heduanX + "\\" + list[0]);

            }
        }

        private void Lis_UndealData_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            test();
        }

        private void Lis_UndealData_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            test();
        }

        private void barButtonItem7_ItemClick_1(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            TIFF tif = new TIFF();
            tif.StartPosition = FormStartPosition.CenterScreen;
            tif.Show();
        }
    }
    public abstract class SplashScreenApplicationContext : ApplicationContext
    {

        private Form _SplashScreenForm;//启动窗体 

        private Form _PrimaryForm;//主窗体 

        private System.Timers.Timer _SplashScreenTimer;

        private int _SplashScreenTimerInterVal = 5000;//默认是启动窗体显示5秒 

        private bool _bSplashScreenClosed = false;
        private delegate void DisposeDelegate();
        public SplashScreenApplicationContext()
        {

            this.ShowSplashScreen();//这里创建和显示启动窗体 

            this.MainFormLoad();//这里创建和显示启动主窗体 

        }
        protected abstract void OnCreateSplashScreenForm();
        protected abstract void OnCreateMainForm();
        protected abstract void SetSeconds();
        protected Form SplashScreenForm
        {
            set
            {

                this._SplashScreenForm = value;

            }

        }
        protected Form PrimaryForm
        {
            set
            {

                this._PrimaryForm = value;

            }
        }
        protected int SecondsShow
        {
            set
            {

                if (value != 0)
                {

                    this._SplashScreenTimerInterVal = 1000 * value;

                }

            }
        }

        private void ShowSplashScreen()
        {

            this.SetSeconds();

            this.OnCreateSplashScreenForm();

            this._SplashScreenTimer = new System.Timers.Timer(((double)(this._SplashScreenTimerInterVal)));

            _SplashScreenTimer.Elapsed += new System.Timers.ElapsedEventHandler(new System.Timers.ElapsedEventHandler(this.SplashScreenDisplayTimeUp));



            this._SplashScreenTimer.AutoReset = false;

            Thread DisplaySpashScreenThread = new Thread(new ThreadStart(DisplaySplashScreen));



            DisplaySpashScreenThread.Start();

        }
        private void DisplaySplashScreen()
        {

            this._SplashScreenTimer.Enabled = true;

            Application.Run(this._SplashScreenForm);

        }
        private void SplashScreenDisplayTimeUp(object sender, System.Timers.ElapsedEventArgs e)
        {

            this._SplashScreenTimer.Dispose();

            this._SplashScreenTimer = null;

            this._bSplashScreenClosed = true;

        }
        private void MainFormLoad()
        {

            this.OnCreateMainForm();



            while (!(this._bSplashScreenClosed))
            {

                Application.DoEvents();

            }

        
        DisposeDelegate SplashScreenFormDisposeDelegate = new DisposeDelegate(this._SplashScreenForm.Dispose ); 

        this._SplashScreenForm.Invoke(SplashScreenFormDisposeDelegate); 

        this._SplashScreenForm = null;

        //必须先显示，再激活，否则主窗体不能在启动窗体消失后出现 

        this._PrimaryForm.Show();

        this._PrimaryForm.Activate();



        this._PrimaryForm.Closed += new EventHandler(_PrimaryForm_Closed); 
        }
        private void _PrimaryForm_Closed(object sender, EventArgs e)
        {

            base.ExitThread();

        } 
    }
    public class StartUpClass
    {

        [STAThread]

        static void Main()
        {

          
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);



            ESRI.ArcGIS.RuntimeManager.Bind(ESRI.ArcGIS.ProductCode.EngineOrDesktop);
            Application.Run(new mycontext());
        }

    }
    public class mycontext : SplashScreenApplicationContext
    {

        protected override void OnCreateSplashScreenForm()
        {

            this.SplashScreenForm = new FormStart();//启动窗体 

        }



        protected override void OnCreateMainForm()
        {

            this.PrimaryForm = new MainForm();//主窗体 

        }



        protected override void SetSeconds()
        {

            this.SecondsShow = 2;//启动窗体显示的时间(秒) 

        }

    } 

}
