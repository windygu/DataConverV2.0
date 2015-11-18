using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Collections;
using System.Threading;

using SuperMap.Data;
using ESRI.ArcGIS.AnalysisTools;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataManagementTools;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geoprocessor;
using ESRI.ArcGIS.Geoprocessing;
using SuperMapTool;


namespace DataConver
{
    class DataConverTool
    {
        private Geoprocessor gp = new Geoprocessor();
        private string featureClassName;
        private bool ajust = false;
        private int vec_count = 0;//矢量数据处理计数
        private int ras_count = 0;//栅格数据处理计数
        private DirectoryInfo di;
        private DateTime datetime;
        static SuperMap.Data.Workspace wks = new SuperMap.Data.Workspace();
        private Recordset recordset;
        private ImportTool importTool = new ImportTool(wks);
        private Setting set = new Setting();
        
        private List<string> GDBfold;
        private string fiedname = "Code";
        DevExpress.XtraEditors.MarqueeProgressBarControl progressBar;
        DevComponents.DotNetBar.Controls.RichTextBoxEx MessageShow;
        DevComponents.DotNetBar.LabelX lab_progress;
        DevComponents.DotNetBar.LabelX lb;
        int max=0;
        public DataConverTool(DevComponents.DotNetBar.LabelX lab_progress, DevComponents.DotNetBar.LabelX lb, DevExpress.XtraEditors.MarqueeProgressBarControl progressBar, DevComponents.DotNetBar.Controls.RichTextBoxEx MessageShow)
        {
           
            this.lb = lb;
            this.lab_progress = lab_progress;
            this.progressBar = progressBar;
            this.MessageShow = MessageShow;
        }
        public void pgrs(int p)
        {
            //lb.Text = p.ToString() + "/" + max.ToString();
            double percent = Convert.ToDouble(p) / Convert.ToDouble(max);
            lb.Text= percent.ToString("0%");
            //double r = Convert.ToDouble(p / max);
            //string rs= (p / max).ToString();
            //string ss = (100 / max).ToString();
             
        }
        public void Msg(string msg)
        {
            try
            {
                Application.DoEvents();
                progressBar.EditValue = msg;
                MessageShow.AppendText(msg + "\r\n");
                MessageShow.Focus();
                MessageShow.SelectionStart = MessageShow.Text.Length;///焦点在最后
            }
            catch
            { }
        }
        //时间计算
        public static string ExecDateDiff(DateTime dateBegin, DateTime dateEnd)
        {
            TimeSpan ts1 = new TimeSpan(dateBegin.Ticks);
            TimeSpan ts2 = new TimeSpan(dateEnd.Ticks);
            TimeSpan ts3 = ts1.Subtract(ts2).Duration();
            //你想转的格式
            return ts3.Hours.ToString() + "时" + ts3.Minutes.ToString() + "分" + ts3.Seconds.ToString() + "秒。";
        }
        /*主处理函数需要执行的操作：
         * 1、传入参数：河段路径
         * 2、方法：数据转换、溃口数据导入、淹没过程数据导入、影响分析数据导入、模板、属性替换、生成处理报告、更改文件名
         * 3、定义参数：输出路径
         * 
         */
        public void DealAll(string dataPath,string password)
        {
            set.passWod = password;
            DateTime dt = new DateTime();
            dt = DateTime.Now;
            try
            {
                if (dataPath.Substring(dataPath.Length - 3) == "已处理")
                    return;
                di = new DirectoryInfo(dataPath);
                //成果图的获取
                DirectoryInfo[] picture = di.GetDirectories("*成果图件*", SearchOption.AllDirectories);
                //设置淹没过程影像文件路径
           
                DirectoryInfo[] tiff1 = di.GetDirectories("tiffPath", SearchOption.AllDirectories);
               
               string tiff = tiff1[0].FullName;
                
                MessageShow.Clear();

                string mod = set.mod;//设置地图模板路径
                //设置编制单元名称，用于信息输出和对照风险点编号
                int ll = dataPath.LastIndexOf("\\");
                string riverName = dataPath.Substring(ll + 1);
                //string ml = dataPath.Substring(0, ll);
                //int lll = ml.LastIndexOf("\\");
                //string riverFather = ml.Substring(lll + 1);
                string fxtb = riverName;

                //if (riverFather == "未处理编制单元")
                   // fxtb = riverName;
                datetime = DateTime.Now;//设置开始实时间
                lab_progress.Text = "正在处理：【" + fxtb + "】";

                Msg("正在处理河段：" + fxtb);

                string outPath = importTool.createFolder(set.deal_path + "\\" + fxtb + "_已处理");//创建输出对应目录
                string outPath_Mid = importTool.createFolder(outPath + "\\" + "中间数据");
                string outPath_Final = importTool.createFolder(outPath + "\\" + fxtb);
                #region 测试
                try
                {
                    FileInfo[] xlsPath = di.GetFiles("*社会经济*.xls*");

                    foreach (FileInfo xx in xlsPath)
                    {
                        ConnectAttribute(xx.FullName, outPath_Final + "\\" + "yxfx");
                    }
                }
                catch
                {
                    Msg("更新属性数据不存在，跳过更新");
                }
                //ymgc(fxtb, outPath_Final, outPath_Mid, tiff);//set.TIFFPath
                //ymgc(fxtb, outPath_Final, outPath_Mid, tiff);//淹没过程影像数据导入

                //  ImportMapModel(mod, outPath_Final + "\\" + "MapResult", outPath_Final, set.mod);
                #endregion
                GDBfold = importTool.getAllFolderName(dataPath, "GBCarto50K*");
                foreach (string nn in GDBfold)
                {
                    int l = nn.LastIndexOf("\\");
                    string t = nn.Substring(l);
                    if (nn.Substring(l + 1) == "GBCarto50K_DangerEvaluateData.gdb")
                    {
                        outPath = importTool.createFolder(outPath_Mid + "\\" + "ztdt");
                    }
                    else
                        outPath = importTool.createFolder(outPath_Mid + "\\" + "jcdt");
                    Select(nn, outPath, fiedname);//按照code值筛选要素
                    Tiff(nn, outPath);//转换gdb中栅格文件

                }
                Msg("本次处理数据总数为：" + (vec_count + ras_count).ToString() + "条" + "\r\n" + "其中处理矢量数据：" + vec_count.ToString() + "条" + "\r\n" + "处理栅格数据：" + ras_count.ToString() + "条");
                Msg("用时：" + ExecDateDiff(datetime, DateTime.Now));//显示用时
                lab_progress.Text = "【" + fxtb + "】数据转换完成";
                System.Threading.Thread.Sleep(100);
                lab_progress.Text = "【" + fxtb + "】正在导入淹没过程影像数据···";
               //淹没过程影像数据导入
                ymgc(fxtb, outPath_Final, outPath_Mid, tiff);
                lab_progress.Text = "【" + fxtb + "】淹没过程影像数据导入完成";
                System.Threading.Thread.Sleep(100);

                lab_progress.Text = "【" + fxtb + "】正在导入溃口数据···";

                kuiko(dataPath, outPath_Final);
                lab_progress.Text = "【" + fxtb + "】溃口导入完成";
                System.Threading.Thread.Sleep(100);

                lab_progress.Text = "【" + fxtb + "】正在导入基础地图数据···";
                jcdt(outPath_Final, outPath_Mid);
                lab_progress.Text = "【" + fxtb + "】基础地图导入完成";
                System.Threading.Thread.Sleep(100);

                lab_progress.Text = "【" + fxtb + "】正在导入专题地图数据···";
                ztdt(outPath_Final, outPath_Mid);
                lab_progress.Text = "【" + fxtb + "】专题地图导入完成";
                System.Threading.Thread.Sleep(100);

                lab_progress.Text = "【" + fxtb + "】正在导入影响分析数据···";
                yxfx(dataPath, outPath_Final);
                lab_progress.Text = "【" + fxtb + "】影响分析数据导入完成";
                System.Threading.Thread.Sleep(100);

                lab_progress.Text = "【" + fxtb + "】正在更新属性数据···";
                try
                {
                    FileInfo[] xlsPath = di.GetFiles("*社会经济*.xls*");
                    foreach (FileInfo xx in xlsPath)
                    {
                        ConnectAttribute(xx.FullName, outPath_Final + "\\" + "yxfx");
                    }
                }
                catch
                {
                    Msg("更新属性数据不存在，跳过更新");
                }
                lab_progress.Text = "【" + fxtb + "】正在加载地图模板···";
                Msg("正在加载地图模板···");
                datetime = DateTime.Now;
                ImportMapModel(mod, outPath_Final + "\\" + "MapResult", outPath_Final, set.mod);
                //Msg(importTool.mo);
                Msg("用时：" + DataConverTool.ExecDateDiff(datetime, DateTime.Now));
                lab_progress.Text = "【" + fxtb + "】加载地图模板完成";

                importTool.createFolder(outPath_Final + "\\成果图");
                di = new DirectoryInfo(picture[0].FullName);
                max = di.GetFiles().Length;
                foreach (FileInfo jpg in di.GetFiles())
                {
                    int p = 0;
                    File.Copy(jpg.FullName, outPath_Final + "\\成果图\\" + jpg.Name, true);
                    pgrs(p++);
                }
                Msg("复制成果图成功！");
                
                Msg("正在生成处理报告····");
                Msg("本河段用时：" + DataConverTool.ExecDateDiff(dt, DateTime.Now));
                MessageShow.SaveFile(dataPath + "_处理报告.txt", RichTextBoxStreamType.TextTextOleObjs);
                Msg(dataPath + "_处理报告.txt  已生成成功!");
                lab_progress.Text = "【" + fxtb + "】处理完成";
                Directory.Move(dataPath, dataPath + "_已处理");
                System.Threading.Thread.Sleep(1000);//等待一秒；
            }
            catch (Exception ex)
            {
                Msg(ex.Message);
            }
        }
        #region  导入地图模板和投影转换

        public void ImportMapModel(string mod, string wsp, string sources, string symbol)
        {
            //string wsp = @"G:\数据转换\项目数据管理\结果数据\MapResult";
            CreateWorkspace(wsp, sources, symbol);
            try
            {
                DirectoryInfo di = new DirectoryInfo(mod);
                FileInfo[] xml = di.GetFiles("*.xml");
                max = xml.Length;int p = 0;
                foreach (FileInfo nn in xml)
                {
                    
                    string Mapxml = readXML(nn.FullName);
                    ModelApplication(Mapxml, wsp + ".smwu", nn.Name.Substring(0, nn.Name.Length - 4));
                    pgrs(p++);
                    Msg(nn.Name + "模板导入成功");
                    if (nn.Name.Substring(0, 4) == "ymgc")
                    {
                        importTool.refreshMod(wsp, nn.Name.Substring(0, nn.Name.Length - 4), set.passWod, @"D:\移动风险监测\参考专题图模板.xml");
                        Msg("地图"+nn.Name.Substring (0,nn.Name.Length-4)+"更新成功！");
                    }
                    System.Threading.Thread.Sleep(100);

                    //cv.Msg(nn.Name + "模板导入成功");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            //description.Text=readXML(mod);
            //  string xml =readXML(mod); 
            //importTool.model(xml, wsp, "ymgc1");
        }
        //读取xml中 的内容
        public String readXML(string pathOfXML)
        {
            string ss = File.ReadAllText(pathOfXML, Encoding.Default);
            return ss;
        }
        public void ModelApplication(String xml, string wksPath, string name)
        {
            //初始化工作空间
            try
            {
                SuperMap.Data.Workspace sWorkspace1 = new SuperMap.Data.Workspace();
                WorkspaceConnectionInfo info = new WorkspaceConnectionInfo(wksPath);//@"G:\数据转换\测试数据\Test1\test.smwu");
                info.Password = set.passWod;
                sWorkspace1.Open(info);
                SuperMap.Data.Maps maps = sWorkspace1.Maps;
                
                maps.Add(name, xml);
                sWorkspace1.Save();
                sWorkspace1.Dispose();
            }
            catch (Exception ex)
            {
                Msg(ex.Message);
            }

        }
        public void CreateWorkspace(string wpsPath, string sources, string symbol)
        {
            // 创建工作空间，弹出 “关于”对话框
            Msg("正在创建工作空间···");
            SuperMap.Data.Workspace workspace = new SuperMap.Data.Workspace();
            WorkspaceConnectionInfo workspaceConnectionInfo = new WorkspaceConnectionInfo();
            workspaceConnectionInfo.Type = WorkspaceType.SMWU;

            workspaceConnectionInfo.Name = "MapResult";
            workspaceConnectionInfo.Password = set.passWod;
            String file = wpsPath;
            workspaceConnectionInfo.Server = file;

            if (workspace.Create(workspaceConnectionInfo))
            {
                //MessageBox.Show("创建工作空间成功");
                workspace.Caption = "MapResult";
                workspace.Save();
                Msg("工作空间创建成功：" + workspace.Caption);
                System.Threading.Thread.Sleep(500);
                lab_progress.Text = "正在导入符号库···";
                DirectoryInfo di = new DirectoryInfo(symbol);
                foreach (FileInfo fill in di.GetFiles("*.bru"))
                {
                    File.Copy(fill.FullName, sources + "//" + fill.Name, true);
                    SymbolFillLibrary sf = workspace.Resources.FillLibrary;
                    sf.FromFile(sources + "//" + fill.Name);
                    workspace.Save();
                }
                foreach (FileInfo point in di.GetFiles("*.sym"))
                {
                    File.Copy(point.FullName, sources + "//" + point.Name, true);

                    SymbolMarkerLibrary sf = workspace.Resources.MarkerLibrary;
                    sf.FromFile(sources + "//" + point.Name);
                    workspace.Save();
                } foreach (FileInfo Line in di.GetFiles("*.lsl"))
                {
                    File.Copy(Line.FullName, sources + "//" + Line.Name, true);

                    SymbolLineLibrary sf = workspace.Resources.LineLibrary;
                    sf.FromFile(sources + "//" + Line.Name);
                    workspace.Save();
                }
                Msg("符号库导入成功");

                System.Threading.Thread.Sleep(500);

                di = new DirectoryInfo(sources);
                FileInfo[] fl = di.GetFiles("*.udb");
                for (int s = 0; s < fl.Length; s++)
                {

                    DatasourceConnectionInfo ds = new DatasourceConnectionInfo();
                    ds.Alias = fl[s].Name.Substring(0, fl[s].Name.Length - 4);
                    lab_progress.Text = "添加数据源：" + ds.Alias;
                    
                    ds.Server = sources + "\\" + fl[s].ToString();
                    ds.Password = set.passWod;
                    //ds.Password = "aaaazzzz";
                    Datasource datasource = workspace.Datasources.Open(ds);
                    if (ds.Alias.Substring(0, 4) == "ymgc")
                    {
                        Msg("正在进行投影转换···");
                        ProjectConverTest(set.CoorRef, datasource);
                        System.Threading.Thread.Sleep(10);
                    }
                    //= "ymgc1";
                    if (datasource == null)
                    {
                        MessageBox.Show("打开数据源失败");
                    }
                    else
                    {
                        //MessageBox.Show(fl[s].Name+"数据源打开成功！");
                    }
                    workspace.Save();

                }
                workspace.Close();
                workspace.Dispose();
                workspaceConnectionInfo.Dispose();
            }

        }
        public void ProjectConverTest(string prjRef, Datasource dc)
        {
            try
            {
                Application.DoEvents();
                string name = "";
                //Datasource dc = wps.Datasources[UDBname];
                PrjCoordSys prj = new PrjCoordSys();
                prj.FromFile(prjRef, PrjFileType.SuperMap);
                max = dc.Datasets.Count;
                for (int i = 1; i <= dc.Datasets.Count; i++)
                {

                    name = "time" + i.ToString();
                    
                    try
                    {
                        Dataset dset = dc.Datasets[name];
                        dset = CoordSysTranslator.Convert(dset, prj, dset.Datasource, name + "_", new CoordSysTransParameter(), CoordSysTransMethod.GeocentricTranslation);
                        dset.Datasource.Datasets.Delete(name);
                        dset.Datasource.CopyDataset(dset, name, dset.EncodeType);
                        dset.Datasource.Datasets.Delete(name + "_");
                        //wps.Save();
                        pgrs(i);
                        Msg(name + "投影转换成功！");
                    }
                    catch
                    {
                        continue;
                    }

                }
                //wps.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        #endregion
        /// <summary>
        /// 按Code值进行拆分
        /// </summary>
        /// <param name="path">源数据路径</param>
        /// <param name="outPath">输出路径</param>
        /// <param name="FiledName">字段名称</param>
        public void Select(string path, string outPath, string FiledName)
        {
            //创建工作空间
            IWorkspaceFactory pwokspace = new FileGDBWorkspaceFactoryClass();
            IWorkspace workspace = pwokspace.OpenFromFile(path, 0);
            IFeatureWorkspace pFeatureWorkspace = (IFeatureWorkspace)workspace;
            //新建GP
            gp.SetEnvironmentValue("workspace", path);
            //获取工作空间下要素类
            IGpEnumList featureClasses = gp.ListFeatureClasses("", "", "");
            
            string featureClass = featureClasses.Next();
            
            while (featureClass != "")
            {
                IFeatureClass pFeatureClass = pFeatureWorkspace.OpenFeatureClass(featureClass);
                int f = pFeatureClass.FindField(FiledName);

                if (f == -1)//判断字段是否存在
                {
                    featureClass = featureClasses.Next();
                    continue;
                }
                Export2Shp(gp, pFeatureClass, FiledName, outPath);//转换函数

                featureClass = featureClasses.Next();
            }
            IGpEnumList dataSets = gp.ListDatasets("", "");
            string dataSet = dataSets.Next();
            while (dataSet != "")
            {
                try
                {
                    IFeatureDataset pFeatureDataset = pFeatureWorkspace.OpenFeatureDataset(dataSet);
                    IFeatureClassContainer pFCC = (IFeatureClassContainer)pFeatureDataset;
                    IEnumFeatureClass pEnumFeatureClass = pFCC.Classes;
                    IFeatureClass m_FeatureClass = pEnumFeatureClass.Next();
                    //对dataset中的fetureclass的一个循环获取
                    while (m_FeatureClass != null)
                    {
                        featureClassName = m_FeatureClass.AliasName.Substring(0, 2);
                        int d = m_FeatureClass.FindField("Shape_Area");//判断是否是面文件
                        int f = m_FeatureClass.FindField(FiledName);//判断code字段是否存在
                        int value = m_FeatureClass.FindField("Value");
                        int contour = m_FeatureClass.FindField("CONTOUR");
                        try
                        {
                            if (f != -1)//判断是否有code字段
                            {
                                if (d != -1)//判断是否为面文件
                                    ajust = true;
                                if (featureClassName == "方案")//判断为方案数据
                                {
                                    featureClassName = dataSet + "_溃口";
                                    solution(gp, m_FeatureClass, outPath + "\\" + dataSet + "_溃口.shp");
                                    m_FeatureClass = pEnumFeatureClass.Next();

                                }
                                else
                                {
                                    Export2Shp(gp, m_FeatureClass, FiledName, outPath);
                                    m_FeatureClass = pEnumFeatureClass.Next();
                                }
                            }
                            else if (value != -1)//判断是否含有value字段（水深和流速）
                            {
                                string[] value1 = GetUniqueValue(m_FeatureClass, "Value");
                                if (value1.Length != 0)
                                {
                                    featureClassName = m_FeatureClass.AliasName;
                                    solution(gp, m_FeatureClass, outPath + "\\" + featureClassName + ".shp");
                                    m_FeatureClass = pEnumFeatureClass.Next();
                                }
                                else
                                    m_FeatureClass = pEnumFeatureClass.Next();

                            }
                            else if (contour != -1)//判断是否含有contour字段（等深线）
                            {
                                string[] Contour1 = GetUniqueValue(m_FeatureClass, "CONTOUR");
                                if (Contour1.Length != 0)
                                {
                                    featureClassName = m_FeatureClass.AliasName;
                                    solution(gp, m_FeatureClass, outPath + "\\" + featureClassName + ".shp");
                                    m_FeatureClass = pEnumFeatureClass.Next();
                                }
                                else
                                    m_FeatureClass = pEnumFeatureClass.Next();

                            }
                            else
                                m_FeatureClass = pEnumFeatureClass.Next();

                        }
                        catch
                        {
                            m_FeatureClass = pEnumFeatureClass.Next();
                        }
                    }
                    dataSet = dataSets.Next();
                }
                catch
                {
                    dataSet = dataSets.Next();
                }
            }

        }
        public void Export2Shp(Geoprocessor gp, IFeatureClass featureClass, string FiledName, string outPath)
        {

            string[] code = GetUniqueValue(featureClass, FiledName);
            string CN_code;
            for (int i = 0; i < code.Length; i++)//已经判断code是否有值
            {
                if (ajust)//判断是否为面文件
                {
                    if (code[i] == "100102")
                    {

                        CN_code = "省界";
                        deal(code, i, CN_code, gp, featureClass, FiledName, outPath);
                        //feture2line(CN_code, gp, outPath + "\\" + "省界线.shp", outPath, CN_code + ".shp");
                    }
                    else if (code[i] == "100103")
                    {
                        CN_code = "地市界";
                        deal(code, i, CN_code, gp, featureClass, FiledName, outPath);
                        //feture2line(CN_code, gp, outPath + "\\" + "地市界线.shp", outPath, CN_code + ".shp");
                    }
                    else if (code[i] == "100104")
                    {
                        CN_code = "县界";
                        deal(code, i, CN_code, gp, featureClass, FiledName, outPath);
                        //feture2line(CN_code, gp, outPath + "\\" + "县界线.shp", outPath, CN_code + ".shp");
                    }
                    else if (code[i] == "100105")
                    {
                        CN_code = "乡镇界";
                        deal(code, i, CN_code, gp, featureClass, FiledName, outPath);
                        //feture2line(CN_code, gp, outPath + "\\" + "乡镇界线.shp", outPath, CN_code + ".shp");
                    }
                    else if (code[i] == "120100")
                    {
                        CN_code = dictionary(code[i]);
                        if (CN_code == "")
                            continue;
                        deal(code, i, CN_code, gp, featureClass, FiledName, outPath);
                    }
                    else if (code[i] == "120201")
                    {
                        CN_code = dictionary(code[i]);
                        if (CN_code == "")
                            continue;
                        deal(code, i, CN_code, gp, featureClass, FiledName, outPath);
                    }
                }

                else
                {
                    CN_code = dictionary(code[i]);
                    if (CN_code == "")
                        continue;
                    deal(code, i, CN_code, gp, featureClass, FiledName, outPath);
                }

            }
            ajust = false;
        }
        public void deal(string[] code, int i, string CN_code, Geoprocessor gp, IFeatureClass featureClass, string FiledName, string outPath)
        {
            vec_count++;
            Msg("正在处理···" + CN_code);
            ESRI.ArcGIS.AnalysisTools.Select select = new Select(featureClass, outPath + "\\" + CN_code + ".shp");
            select.where_clause = FiledName + "=" + code[i].ToString();
            gp.OverwriteOutput = true;
            gp.Execute(select, null);
            Msg(CN_code + "   处理完成！");
        }
        /// <summary>
        /// 获取字段唯一值
        /// </summary>
        public static string[] GetUniqueValue(IFeatureClass pFeatureClass, string strFld)
        {
            //得到IFeatureCursor游标
            IFeatureCursor pCursor = pFeatureClass.Search(null, false);
            //coClass对象实例生成
            IDataStatistics pData = new DataStatisticsClass();
            pData.Field = strFld;
            pData.Cursor = pCursor as ICursor;
            //枚举唯一值
            System.Collections.IEnumerator pEnumVar = pData.UniqueValues;
            //记录总数
            int RecordCount = pData.UniqueValueCount;
            //字符数组
            string[] strValue = new string[RecordCount];
            pEnumVar.Reset();
            int i = 0;
            while (pEnumVar.MoveNext())
            {
                strValue[i++] = pEnumVar.Current.ToString();
            }
            return strValue;
        }
        /// <summary>
        /// 方案
        /// </summary>
        /// <param name="gp"></param>
        /// <param name="featureClass"></param>
        /// <param name="savePath"></param>
        public void solution(Geoprocessor gp, IFeatureClass featureClass, string savePath)
        {
            Msg("开始处理..." + featureClassName);

            ESRI.ArcGIS.DataManagementTools.CopyFeatures copyFeature = new CopyFeatures();
            copyFeature.in_features = featureClass;
            copyFeature.out_feature_class = savePath;
            gp.OverwriteOutput = true;
            gp.Execute(copyFeature, null);
            Msg(featureClassName + "转换完成！");
            vec_count++;

        }
        public string dictionary(string codeValue)
        {
            switch (codeValue)
            {
                case "100202":
                    return "省会";

                case "100203":
                    return "地市";

                case "100204":
                    return "县";

                case "100205":
                    return "乡镇";

                case "100206":
                    return "村庄";

                case "100102":
                    return "省界线";

                case "100103":
                    return "地市界线";

                case "100104":
                    return "县界线";

                case "100105":
                    return "乡镇界线";

                case "100301":
                    return "铁路";

                case "100401":
                    return "国道";

                case "100501":
                    return "省道";

                case "100502":
                    return "县道";

                case "100503":
                    return "乡道";

                case "100504":
                    return "高速";

                case "100505":
                    return "其他道路";

                case "140100":
                    return "一级堤防";

                case "140200":
                    return "二级堤防";

                case "120401":
                    return "干流";

                case "120402":
                    return "支流";


                case "120100":
                    return "河流渠道";

                case "120101":
                    return "水库";

                case "560001":
                    return "_溃口";

                default:
                    return "";

            }

        }
        /// <summary>
        /// 栅格数据转换
        /// </summary>
        public void Tiff(string dirName, string saveName)
        {
            try
            {
                gp.SetEnvironmentValue("workspace", dirName);
                IGpEnumList RasterClasses = gp.ListRasters("", "");
                string rasterClass = RasterClasses.Next();
                while (rasterClass != "")
                {
                    //Msg("开始转换TIFF..." + rasterClass);
                    IRasterLayer rasterLayer = new RasterLayerClass();
                    rasterLayer.CreateFromFilePath(dirName + "\\" + rasterClass);

                    //rasterLayer.SpatialReference = rasterLayer1.SpatialReference;
                    ESRI.ArcGIS.DataManagementTools.CopyRaster copyRaster = new CopyRaster();
                    copyRaster.in_raster = rasterLayer;
                    copyRaster.out_rasterdataset = saveName + "\\" + rasterClass + ".tif";
                    gp.OverwriteOutput = true;
                    gp.Execute(copyRaster, null);
                    Msg(rasterClass + "转换完成！");
                    ras_count++;
                    rasterClass = RasterClasses.Next();
                }
            }
            catch (Exception ex)
            {
                Msg(ex.Message);
            }
        }
        //溃口数据导入函数
        public void kuiko(string kkpath, string outPath_Final)
        {
            try
            {
                //创建数据源
                di = new DirectoryInfo(kkpath);//批次处理需要更改

                FileInfo[] kk = di.GetFiles("kuiko.shp", SearchOption.AllDirectories);//得到溃口数据
                if (kk.Length == 0)
                {
                    Msg("溃口数据不存在，系统自动跳过");
                    return;
                }
                DatasourceConnectionInfo info = new DatasourceConnectionInfo();
                info.Password = set.passWod;
                info.Server = outPath_Final + "\\" + "kuikou.udb";
                datetime = DateTime.Now;
                foreach (FileInfo kuikou in kk)
                {
                    string way = kuikou.Directory + "\\" + kuikou.Name;
                    //导入数据

                    importTool.ImportShp(way, info);
                    Msg(kuikou.Name + "导入成功！");
                }

                Msg("所有任务数据总" + kk.Length.ToString() + "条");
                Msg("用时：" + ExecDateDiff(datetime, DateTime.Now));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private DatasourceConnectionInfo creatUDB(string outPath_Final, string name, string password)
        {
            try
            {
                DatasourceConnectionInfo info = new DatasourceConnectionInfo();
                info.Server = outPath_Final + "\\" + name + ".udb";
                info.Password = password;
                //info.Alias = name;
                Datasource ds = wks.Datasources.Create(info);
                return info;
            }
            catch (Exception ex)
            {
                Msg(ex.Message);
                return null;
            }
        }
        //基础地图导入函数
        public void jcdt(string outPath_Final, string outPath_Mid)
        {
            try
            {
                //创建udb并加密
                DatasourceConnectionInfo info = new DatasourceConnectionInfo();
                info.Password = set.passWod;
                info.Server = outPath_Final + "\\jcdt";
                createAndImport("*.shp", "*.tif", outPath_Mid + "\\jcdt\\", info);
                wks.Datasources.CloseAll();
            }
            catch (Exception ex)
            {
                Msg(ex.Message);
            }
        }
        //专题地图导入函数
        public void ztdt(string outPath_Final, string outPath_Mid)
        {
            try
            {
                DatasourceConnectionInfo info = new DatasourceConnectionInfo();
                info.Password = set.passWod;
                info.Server = outPath_Final + "\\ztdt";
                createAndImport("*.shp", "*.tif", outPath_Mid + "\\ztdt\\", info);
                wks.Datasources.CloseAll();
            }
            catch (Exception ex)
            {
                Msg(ex.Message);
            }
        }
        //影响分析导入函数
        public void yxfx(string yxfx_path, string outPath_Final)
        {
            try
            {
                string mc;//设置过滤水系面状的数据
                datetime = DateTime.Now;
                //创建数据源
                //CreateUdb(outPath_Final, txt_yxfx.Text);
                di = new DirectoryInfo(yxfx_path);
                DirectoryInfo[] yxfxPath = di.GetDirectories("6.3影响分析支撑数据", SearchOption.AllDirectories);

                DatasourceConnectionInfo info = new DatasourceConnectionInfo();
                info.Password = set.passWod;
                info.Server = outPath_Final + "\\yxfx";
                foreach (DirectoryInfo dd in yxfxPath)
                {
                    ////遍历文件夹
                    FileInfo[]info_file=dd.GetFiles("*.shp", SearchOption.AllDirectories);
                    max = info_file.Length;int p = 0;
                    foreach (FileInfo NextFolder in info_file )//"*.shp",
                    {
                        
                        mc = NextFolder.Name;
                        if (mc == "水系面状.shp")
                            continue;
                        importTool.ImportShp(NextFolder.Directory + "\\" + mc, info);
                        pgrs(p++);
                        Msg(mc + "导入成功！");
                    }
                    Msg("本次成功导入数据" + importTool.i.ToString() + "条");
                    Msg("用时：" + ExecDateDiff(datetime, DateTime.Now));
                }
                wks.Datasources.CloseAll();
            }
            catch (Exception ex)
            {
                Msg(ex.Message);
            }
        }
        //淹没过程导入函数
        public void ymgc(string yxtb, string outPath_Final, string outPath_Mid, string ymgc_tifPath)
        {

            string fail = "";//记录失败数据。
            datetime = DateTime.Now;
            IRasterLayer rasterLayer = new RasterLayerClass();
            ESRI.ArcGIS.DataManagementTools.CopyRaster copyRaster = new CopyRaster();//定义copy工具
            int i = 1, j = 0;
            try
            {
                string aimPh = outPath_Final + "\\" + " ymgc";
                string resultPath = outPath_Mid + "\\" + " ymgc";
                di = new DirectoryInfo(ymgc_tifPath);//
                List<string> Solution = new List<string>();

                
                Hashtable tb = new Hashtable();
               
                FileInfo[] fileOfTIFF = null;
                try
                {
                    string test = tb[yxtb].ToString();
                    fileOfTIFF = di.GetFiles(test + "*.tif");
                }
                catch
                {
                    fileOfTIFF = di.GetFiles("*.tif");
                }


                for (int Count = 0; Count < fileOfTIFF.Length; Count++)
                {
                    int leng_1 = fileOfTIFF[Count].Name.LastIndexOf('_');
                    string te = fileOfTIFF[Count].Name.Substring(0, leng_1);

                    if (Count + 1 == fileOfTIFF.Length)
                    {
                        Solution.Add(te);
                        break;
                    }
                    int leng_2 = fileOfTIFF[Count].Name.LastIndexOf('_');

                    string te1 = fileOfTIFF[Count + 1].Name.Substring(0, leng_2);
                    if (te != te1)
                    {
                        Solution.Add(te);
                    }

                }
                foreach (string fang in Solution)
                {
                    int leng1 = fang.LastIndexOf('-');
                    string fa = fang.Substring(leng1 + 1);
                    //}
                    //for (int fa = 1; fa < Solution.Count; fa++)

                    //    {
                    DateTime datetimeBegain = new DateTime();
                    datetimeBegain = DateTime.Now;
                    //创建一个文件夹
                    //string FA=
                    string create1 = resultPath + fa;
                    importTool.createFolder(create1);
                    Hashtable table = new Hashtable();//创建一个哈希表
                    string projecCode = "";
                    if (tb[yxtb] == null)
                    {
                        projecCode = fang + "*.tif";
                    }
                    else
                        projecCode = tb[yxtb].ToString() + "-" + fa + "*.tif";
                    FileInfo[] arrFi = di.GetFiles(projecCode);
                    max = arrFi.Length;
                    for (int s = 0; s < arrFi.Length; s++)
                    {
                        int length = arrFi[s].Name.LastIndexOf('_');
                        string jq = arrFi[s].Name.Substring(length + 1);//.Replace("_","")
                        string jq2 = jq.Substring(0, jq.Length - 4);//去掉“.tif”
                        double m = double.Parse(jq2);
                        table.Add(m, arrFi[s].Name);
                        j++;
                    }

                    ArrayList akeys = new ArrayList(table.Keys);
                    akeys.Sort();//对哈希表中的关键字排序，排序
                    //DatasourceConnectionInfo info = creatUDB(outPath_Final, "ymgc" + fa.Substring(1), "aaaazzzz");
                    //info.Password = "aaaazzzz";
                    //wks.Datasources.Open(info);
                    DatasourceConnectionInfo info = new DatasourceConnectionInfo();
                    info.Password = set.passWod;
                    info.Server = outPath_Final + "\\" + "ymgc" + fa.Substring(1) + ".udb";
                    foreach (double qq in akeys)
                    {
                        try
                        {
                            rasterLayer.CreateFromFilePath(ymgc_tifPath + "\\" + table[qq].ToString());
                        }
                        catch
                        {
                            fail += table[qq].ToString() + "\n";
                            i = i + 1;
                            continue;
                        }
                        copyRaster.in_raster = rasterLayer;
                        copyRaster.out_rasterdataset = create1 + "\\" + "time" + i.ToString() + ".tif";
                        gp.OverwriteOutput = true;
                        gp.Execute(copyRaster, null);
                        //添加进udb

                        importTool.ImportTIFF(create1 + "\\" + "time" + i.ToString() + ".tif", info);
                        pgrs(i);
                        Msg("方案" + fa + "_time" + i.ToString() + "导入数据源成功");
                        i++;
                    }
                    Msg("项目编号：" + fang + "\n" + "任务总数：" + arrFi.Length + "条\n" + "此次完成：" + (i - 1).ToString() + "条\n" + "处理失败：" + fail);

                    i = 1; max = 1;
                    //wks.Datasources.CloseAll();
                    Msg("用时：" + ExecDateDiff(datetimeBegain, DateTime.Now));
                }
                //Msg("项目编号：" + tb[yxtb].ToString() + "\n" + "任务总数：" + j + "条\n" + "此次完成：" + j.ToString() + "条\n" + "处理失败为：" + fail);
                //Msg("用时：" + ExecDateDiff(datetime, DateTime.Now));
            }
            catch (Exception ex)
            {
                Msg(ex.Message);
            }
        }
        /*1、打开数据集
                  * 2、获取记录集  没有就增加字段
                  * 3、读取xls对应导入
                  * 4、更新属性表
                  * 5、关闭数据源
                  */

        public void ConnectAttribute(string xlsPath, string udbPath)
        {
            datetime = DateTime.Now;
            string[] m_newFieldName = { "GDP", "耕地面积", "总人口" };
            //定义工作空间
            SuperMap.Data.Workspace wps = new SuperMap.Data.Workspace();
            //工作空间打开数据源
            wps.Datasources.Open(new DatasourceConnectionInfo(udbPath, "yxfx", "aaaazzzz"));
            //临时数据集
            Dataset srcDataset = wps.Datasources[0].Datasets["xzjLayer"];
            DatasetVector dv = (DatasetVector)srcDataset;
            FieldInfos m_fieldInfos = dv.FieldInfos;
            for (int i = 0; i < m_newFieldName.Length; i++)
            {
                try
                {
                    if (m_fieldInfos.IndexOf(m_newFieldName[i]) != -1)
                    {
                        Msg("数据集中已经有指定字段！" + m_newFieldName[i]);

                        continue;
                    }

                    SuperMap.Data.FieldInfo delField = new SuperMap.Data.FieldInfo(m_newFieldName[i], SuperMap.Data.FieldType.Double);
                    delField.IsRequired = false;
                    delField.DefaultValue = "100.0";
                    delField.Caption = m_newFieldName[i];
                    m_fieldInfos.Add(delField);
                    Msg("成功添加字段：" + m_newFieldName[i]);

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

            }
            recordset = dv.GetRecordset(false, CursorType.Dynamic);
            UpdateFieldValueTest(recordset, xlsPath);
            wps.Dispose();
            Msg("用时：" + ExecDateDiff(datetime, DateTime.Now));
        }

        private void UpdateFieldValue(Recordset recordset, string xlsPath)
        {
            try
            {
                DataTable dt = importTool.ExcelToDataTable(xlsPath, "sheet1");

                int length = recordset.RecordCount;
                if (length != 0)
                {

                    //for (Int32 i = 1; i <length; i++)
                    //{   

                    //recordset.Edit();
                    //UpdateFieldValue(recordset);
                    try
                    {
                        for (int m = 0; m < dt.Columns.Count; m++)
                        {
                            if (dt.Columns[m].ColumnName == null)
                                continue;
                            if (dt.Columns[m].ColumnName == "GDP")
                            {
                                for (Int32 i = 1; i < length; i++)
                                {

                                    recordset.SeekID(i);
                                    recordset.Edit();
                                    if (i > dt.Rows.Count)
                                        continue;
                                    object valueGDP = dt.Rows[i - 1][m];
                                    recordset.SetFieldValue("GDP", valueGDP);
                                }
                            }
                            else
                                if (dt.Columns[m].ColumnName == "FARMLAND")
                                {
                                    for (Int32 i = 1; i < length; i++)
                                    {
                                        recordset.SeekID(i);
                                        recordset.Edit();
                                        if (i > dt.Rows.Count)
                                            continue;
                                        object valueAC = dt.Rows[i - 1][m];
                                        recordset.SetFieldValue("耕地面积", valueAC);
                                    }
                                }
                                else
                                    if (dt.Columns[m].ColumnName == "POPUL")
                                    {
                                        for (Int32 i = 1; i < length; i++)
                                        {
                                            recordset.SeekID(i);
                                            recordset.Edit();
                                            if (i > dt.Rows.Count)
                                                continue;
                                            object valuePE = dt.Rows[i - 1][m];
                                            recordset.SetFieldValue("总人口", valuePE);
                                        }
                                    }
                                    else
                                        continue;


                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    recordset.Update();

                    Msg("修改属性字段完成");
                }
                else
                {
                    Msg("记录集中没有记录");
                }
            }
            catch (Exception ex)
            {
                Msg(ex.Message);
            }


        }
        private void UpdateFieldValueTest(Recordset recordset, string xlsPath)
        {
            try
            {
                DataTable dt = importTool.ExcelToDataTable(xlsPath, "sheet1");

                int length = recordset.RecordCount;
                if (length != 0)
                {

                    /*需求：根据乡镇名字确定更新数据
                     1、选择成行改变：1)循环判断工作表中name列的所有记录，是否等于属性表中name列的第一行的内容   
                                     2)记录工作表中的对应的行数h
                                     3）对数据集中特定字段逐行赋值*/
                    try
                    {
                        for (int c = 0; c < dt.Columns.Count; c++)
                        {
                            if (dt.Columns[c].ColumnName == "NAME")
                            {
                                int s = 1;
                                for (int r = 0; r < dt.Rows.Count; r++)
                                {
                                    
                                    recordset.SeekID(s);

                                    object name = recordset.GetObject("T_NAME");
                                    //更新属性
                                    if (dt.Rows[r][c] == name)
                                    {
                                        MessageBox.Show("");
                                    }

                                }
                                break;
                            }
                            else
                                continue;
                        }
                            for (int m = 0; m < dt.Columns.Count; m++)
                            {
                                if (dt.Columns[m].ColumnName == null)
                                    continue;

                                if (dt.Columns[m].ColumnName == "GDP")
                                {
                                    for (Int32 i = 1; i < length; i++)
                                    {

                                        recordset.SeekID(i);
                                        recordset.Edit();
                                        if (i > dt.Rows.Count)
                                            continue;
                                        object valueGDP = dt.Rows[i - 1][m];
                                        recordset.SetFieldValue("GDP", valueGDP);
                                    }
                                }
                                else
                                    if (dt.Columns[m].ColumnName == "FARMLAND")
                                    {
                                        for (Int32 i = 1; i < length; i++)
                                        {
                                            recordset.SeekID(i);
                                            recordset.Edit();
                                            if (i > dt.Rows.Count)
                                                continue;
                                            object valueAC = dt.Rows[i - 1][m];
                                            recordset.SetFieldValue("耕地面积", valueAC);
                                        }
                                    }
                                    else
                                        if (dt.Columns[m].ColumnName == "POPUL")
                                        {
                                            for (Int32 i = 1; i < length; i++)
                                            {
                                                recordset.SeekID(i);
                                                recordset.Edit();
                                                if (i > dt.Rows.Count)
                                                    continue;
                                                object valuePE = dt.Rows[i - 1][m];
                                                recordset.SetFieldValue("总人口", valuePE);
                                            }
                                        }
                                        else
                                            continue;


                            }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    recordset.Update();

                    Msg("修改属性字段完成");
                }
                else
                {
                    Msg("记录集中没有记录");
                }
            }
            catch (Exception ex)
            {
                Msg(ex.Message);
            }


        }
        private void createAndImport(string type1, string type2, string sourPath, DatasourceConnectionInfo info)
        {

            try
            {   
               
                datetime = DateTime.Now;
                List<string> shp = importTool.getAllFileName(sourPath, type1);
                int p=0;
                max = shp.Count;
                foreach (string l1 in shp)
                {
                    
                    if (l1 == "水系面状.shp")
                        continue;
                    importTool.ImportShp(sourPath + l1, info);// resultData + "\\" + jcdt.Text);
                    pgrs(p++);
                    Msg(l1 + "导入成功！");
                }
                List<string> tif = importTool.getAllFileName(sourPath, type2);
                max = tif.Count;
                int q = 0;
                foreach (string l2 in tif)
                {
                    
                    importTool.ImportTIFF(sourPath + l2, info);
                    pgrs(q++);
                    Msg(l2 + "导入成功！");
                }
                Msg("所有任务数据总" + (shp.Count + tif.Count).ToString() + "条");//+ "\r\n" + "本次成功导入数据" + importTool.i.ToString() + "条");
                Msg("用时：" + ExecDateDiff(datetime, DateTime.Now));
            }
            catch (Exception ex)
            {
                Msg(ex.Message);
            }
        }
    }
}
