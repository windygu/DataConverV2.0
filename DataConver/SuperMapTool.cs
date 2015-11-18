using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using SuperMap.Data;
using SuperMap.UI;
using SuperMap.Data.Conversion;
using SuperMap.Mapping;
using SuperMap.Layout;
using System.IO;
using System.Data.OleDb;
using System.Data;


using ESRI.ArcGIS.AnalysisTools;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataManagementTools;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geoprocessor;
using ESRI.ArcGIS.Geoprocessing;
using DataConver;
namespace SuperMapTool
{
    public class ImportTool
    {
        private SuperMap.Data.Workspace m_workspace;//定义超图工作空间
        private DataImport m_dataImport;//定义超图数据导入类
        public String information;//定义返回信息
        public int i = 0;//定义计数器：导入shp计数
        public string mo;//定义全局变量：模板路径
        /// <summary>
        /// 初始化
        /// </summary>
        public ImportTool(SuperMap.Data.Workspace workspace)
        {
            m_workspace = workspace;
            m_dataImport = new DataImport();

        }


        /// <summary>
        ///udb中导入shp
        /// </summary>
        /// <param name="importPath"></param>
        public void ImportShp(string importPath, DatasourceConnectionInfo info)
        {
            try
            {
                // 1. 构建数据源连接对象。 // info.Database = @"G:\数据转换\测试数据\Test\text.udb";数据库型
                //DatasourceConnectionInfo info = new DatasourceConnectionInfo();
                //info.Server = sourceUDB;//如@"G:\数据转换\测试数据\Test\text.udb";
                ImportSettingSHP importSettingSHP = new ImportSettingSHP();
                importSettingSHP.ImportMode = ImportMode.Overwrite;//可复写
                importSettingSHP.SourceFilePath = importPath;
                importSettingSHP.TargetDatasourceConnectionInfo = info;
                //importSettingSHP.IsAttributeIgnored = false;
                // 3. 获取导入设置对象的导入信息集合（ImportDataInfos），设置目标数据集的名字。默认为原名
                //ImportDataInfo dataInfos = importSettingSHP.GetTargetDataInfos("");
                //importSettingSHP.SetTargetDataInfos(dataInfos);
                // 4. 构建数据导入类对象（DataImport），构建并设置导入设置对象集合。
                DataImport import1 = new DataImport();
                ImportSettings settings = import1.ImportSettings;
                settings.Add(importSettingSHP);
                import1.Run();
                i++;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// udb中导入tiff
        /// </summary>
        /// <param name="importPath"></param>
        public void ImportTIFF(string importPath, DatasourceConnectionInfo info)
        {
            try
            {
                // 1. 构建数据源连接对象。
                //DatasourceConnectionInfo info = new DatasourceConnectionInfo();
                //info.Server = sourceUDB;
                // 2. 构建SHP导入设置对象（ImportSettingSHP），设置数据源，设置导入数据路径。
                //info.Password = "aaaazzzz";
                ImportSettingTIF importSettingTIF = new ImportSettingTIF();
                //m_workspace.Datasources.Open(info);
                importSettingTIF.ImportMode = ImportMode.Overwrite;//可复写
                importSettingTIF.SourceFilePath = importPath;
                importSettingTIF.TargetDatasourceConnectionInfo = info;
                importSettingTIF.ImportingAsGrid = true;//栅格数据集形式
                // 3. 获取导入设置对象的导入信息集合（ImportDataInfos），设置目标数据集的名字。
                // ImportDataInfo dataInfos = importSettingSHP.GetTargetDataInfos("");
                //importSettingSHP.SetTargetDataInfos(dataInfos);
                // 4. 构建数据导入类对象（DataImport），构建并设置导入设置对象集合。
                //string prjRef = @"G:\移动风险监测\参考坐标\CGCS_2000.xml";
                PrjCoordSys prj = new PrjCoordSys();
                // prj.FromFile(prjRef, PrjFileType.SuperMap);
                prj.Type = PrjCoordSysType.SphereMercator;
                importSettingTIF.TargetPrjCoordSys = prj;//设置了参考投影，还需改变其投影转换
                DataImport import1 = new DataImport();
                ImportSettings settings = import1.ImportSettings;
                settings.Add(importSettingTIF);
                import1.Run();
                i++;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public void ImportTIFFTest(string importPath, string sourceUDB)
        {
            try
            {
                // 1. 构建数据源连接对象。
                DatasourceConnectionInfo info = new DatasourceConnectionInfo();
                info.Server = sourceUDB;
                WorkspaceConnectionInfo ConnectionInfo = new WorkspaceConnectionInfo(sourceUDB);
                SuperMap.Data.Workspace wps = new SuperMap.Data.Workspace();
                wps.Create(ConnectionInfo);
                Datasources ds = wps.Datasources;
                //ds.Open(info);
                Datasource dss = ds.Create(info); // new Datasource();
                //dss.Connect();
                // 2. 构建SHP导入设置对象（ImportSettingSHP），设置数据源，设置导入数据路径。
                ImportSettingTIF importSettingTIF = new ImportSettingTIF();
                importSettingTIF.ImportMode = ImportMode.Overwrite;//可复写
                importSettingTIF.SourceFilePath = importPath;
                importSettingTIF.TargetDatasourceConnectionInfo = info;
                importSettingTIF.ImportingAsGrid = true;//栅格数据集形式
                // 3. 获取导入设置对象的导入信息集合（ImportDataInfos），设置目标数据集的名字。
                // ImportDataInfo dataInfos = importSettingSHP.GetTargetDataInfos("");
                //importSettingSHP.SetTargetDataInfos(dataInfos);
                // 4. 构建数据导入类对象（DataImport），构建并设置导入设置对象集合。
                PrjCoordSys prj = new PrjCoordSys();
                prj.Type = PrjCoordSysType.SphereMercator;
                importSettingTIF.TargetPrjCoordSys = prj;//设置了参考投影，还需改变其投影转换
                //prj.GeoCoordSys.FromXML(readXML())
                DataImport import1 = new DataImport();
                ImportSettings settings = import1.ImportSettings;
                settings.Add(importSettingTIF);
                import1.Run();
                try
                {
                    int m = importPath.LastIndexOf('\\');
                    string dsName = importPath.Substring(m + 1);
                    int n = dsName.LastIndexOf('.');
                    string dsname = dsName.Substring(0, n);
                    Datasets datasets = dss.Datasets;
                    Dataset m_processDataset = datasets[dsname];
                    PrjCoordSys prj1 = new PrjCoordSys();
                    prj1.FromXML(readXML(@"G:\移动风险监测\参考坐标\CGCS_2000.xml"));
                    Boolean result = CoordSysTranslator.Convert(m_processDataset, prj1, new CoordSysTransParameter(), CoordSysTransMethod.GeocentricTranslation);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                i++;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public void ProjectConver(string wpsPath, string prjRef, string name)
        {
            try
            {
                SuperMap.Data.Workspace wps = new SuperMap.Data.Workspace();
                WorkspaceConnectionInfo info = new WorkspaceConnectionInfo(wpsPath);
                wps.Open(info);
                Datasource dc = wps.Datasources["ymgc3"];

                //Dataset dset = wps.Datasources["ymgc1"].Datasets[name];// dsets["time1"];

                //dset.Datasource.Datasets.Delete("time2");
                //Dataset dst = dset.Datasource.CopyDataset(dset, "time2", dset.EncodeType);
                PrjCoordSys prj = new PrjCoordSys();
                prj.FromFile(prjRef, PrjFileType.SuperMap);

                for (int i = 1; i <= dc.Datasets.Count; i++)
                {
                    name = "time" + i.ToString();
                    try
                    {
                        Dataset dset = dc.Datasets[name];
                        //bool ok = CoordSysTranslator.Convert(dset, prj, new CoordSysTransParameter(), CoordSysTransMethod.GeocentricTranslation);
                        dset = CoordSysTranslator.Convert(dset, prj, dset.Datasource, name + "_", new CoordSysTransParameter(), CoordSysTransMethod.GeocentricTranslation);
                        dset.Datasource.Datasets.Delete(name);
                        dset.Datasource.CopyDataset(dset, name, dset.EncodeType);
                        dset.Datasource.Datasets.Delete(name + "_");
                        wps.Save();
                    }
                    catch
                    {
                        continue;
                    }

                }
                wps.Dispose();
                MessageBox.Show("OVER");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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
        /// <summary>
        /// 获得地图模板：从已有地图中
        /// </summary>
        /// <param name="wksPath">工作空间</param>
        /// <param name="index">地图索引</param>
        /// <returns>xml的字符</returns>
        public String gainXML(string wksPath, int index)
        {
            SuperMap.Data.Workspace sWorkspace = new SuperMap.Data.Workspace();
            WorkspaceConnectionInfo infomation = new WorkspaceConnectionInfo(wksPath);//@"G:\数据转换\测试数据\Test\test.smwu");
            sWorkspace.Open(infomation);
            if (sWorkspace.Maps.Count == 0)
            {
                MessageBox.Show("当前工作空间中不存在地图!");
                return "";
            }
            SuperMap.Mapping.Map map = new SuperMap.Mapping.Map(sWorkspace);
            map.Open(sWorkspace.Maps[index]);
            String xml = map.ToXML();
            string name = map.Name;
            map.Close();
            sWorkspace.Dispose();
            return xml;
        }
        //
        /// <summary>
        /// 模板应用
        /// </summary>
        /// <param name="xml">模板文件字符</param>
        /// <param name="wksPath">工作空间路径</param>
        /// <param name="name">地图名称</param>
        public void ModelApplication(String xml, string wksPath, string name)
        {
            //初始化工作空间
            SuperMap.Data.Workspace sWorkspace1 = new SuperMap.Data.Workspace();
            WorkspaceConnectionInfo info = new WorkspaceConnectionInfo(wksPath);//@"G:\数据转换\测试数据\Test1\test.smwu");
            info.Password ="aaaazzzz";
            sWorkspace1.Open(info);
            SuperMap.Data.Maps maps = sWorkspace1.Maps;
            maps.Add(name, xml);
            sWorkspace1.Save();
            sWorkspace1.Dispose();
        }
        /*专题图制作部分 
         * 一、SuperMap中
         *   1、将一个数据集拖拽到地图
         *   2、进来的是默认的栅格图层  然后右键制作专题图  选择栅格分段专题图
         *   3、加载专题图模板。
         *   二、代码部分
         *   1、将数据集添加进地图
         *   2、
         */
        public void refreshMod(string wksPath, string name,string password,string ref_zt)
        {
            try
            {
                SuperMap.Data.Workspace sWorkspace1 = new SuperMap.Data.Workspace();
                WorkspaceConnectionInfo info = new WorkspaceConnectionInfo(wksPath + ".smwu");//@"G:\数据转换\测试数据\Test1\test.smwu");
                info.Password = password;
                sWorkspace1.Open(info);
                if (sWorkspace1.Datasources[name] == null)
                    return;
                Datasource ds = sWorkspace1.Datasources[name];
                int con = ds.Datasets.Count;
               
                SuperMap.Mapping.Map map = new SuperMap.Mapping.Map(sWorkspace1);
                map.Open(name);
                
                Layers layers = map.Layers;
                
                ThemeGridRange tgr = new ThemeGridRange();
                tgr.FromXML(readXML(ref_zt));
                
                for (int i = 1; i <= con; i++)
                {
                    
                    if (map.Layers[i].Name.Substring(0, 4) == "time")
                        continue;
                    DatasetGrid dataSet = ds.Datasets["time" + i.ToString()] as DatasetGrid;
                    Layer layer =map.Layers.Insert(i, dataSet,tgr);
                    // map.Layers.Add(dataSet, tgr, true);
                    layer.IsVisible = false;
                    layer.Caption = "time" + i.ToString();
                    map.Refresh();
                    

                }
                sWorkspace1.Maps.SetMapXML(name, map.ToXML());
                //sWorkspace1.Maps.Add(name+"_x", map.ToXML());
                map.Workspace.Save();
                sWorkspace1.Dispose();
               
                //MessageBox.Show("Over");
                //  sWorkspace1.Save(); MessageBox.Show(layers.Count + "");
                 //layer.FromXML(readXML(@"D:\移动风险监测\time1.xml"));
                //layerSettingGrid.ColorTable = Colors.MakeGradient(20, ColorGradientType.BlueWhite, false);

                //layerSettingGrid.SpecialValue = -9999;
                //layerSettingGrid.IsSpecialValueTransparent = true;
                //LayerSettingVector layerSettingVector = new LayerSettingVector();
                //GeoStyle style = new GeoStyle();
                //style.FillForeColor = Color.HotPink;
                //layerSettingGrid.Style = style;
                
                
                
                //ThemeCustom tc = new ThemeCustom();
                //tc.FromXML(readXML(@"D:\移动风险监测\time53.xml"));
               
                //Theme theme=
                //string themes = theme.ToXML();
                //DevComponents.DotNetBar.Controls.RichTextBoxEx re = new DevComponents.DotNetBar.Controls.RichTextBoxEx();
                //re.Text = themes;
                //re.SaveFile(@"D:\移动风险监测\test.txt",RichTextBoxStreamType.TextTextOleObjs);
                //theme.FromXML(readXML(@"D:\移动风险监测\time53.xml"));
               

                   

                  //  layer.Theme.FromXML(readXML(@"D:\移动风险监测\time1.xml"));
                   //layer.FromXML(readXML(@"D:\移动风险监测\time1.xml"));
                   // lyy1.Caption = ;
                    
                    //Layer lyy = layers.Add(dataSet, layerSettingGrid, true);
                    
                 
                    //MessageBox.Show(layers.Count + "");
                    //if(layers.Count==400)
                    //    j=layers.Count;
                    //layers.Add(dataSet,new LayerSettingGrid(), true);
                   //Layer ly=  layers.Insert(i, dataSet, tgr);
                   //MessageBox.Show(layers.Count + "");
                   //layers.Add(dataSet, true);
                   //MessageBox.Show(layers.Count + "");
                   //map.EnsureVisible(ly);
                 
                //sWorkspace1.Maps.Add(sWorkspace1.Maps.GetAvailableMapName(name), map.ToXML()); 
              
               
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// 得到指定目录下所有  文件夹   的名称列表
        /// </summary>
        /// <param name="strPath">文件夹路径</param>
        /// <returns></returns>
        public List<string> getAllFolderName(string strPath, string type)
        {
            List<string> mList = new List<string>();
            DirectoryInfo TheFolder = new DirectoryInfo(strPath);
            //遍历文件夹
            foreach (DirectoryInfo NextFolder in TheFolder.GetDirectories(type, SearchOption.AllDirectories))//, SearchOption.AllDirectories
            {
                mList.Add(NextFolder.FullName);
            }

            return mList;
        }
        //获取文件夹下所有  文件夹  的名字包括子文件夹  利用treeview来实现
        public void loadFolder(string foderPath, string type, DevComponents.AdvTree.Node node)
        {
            List<string> FolderName = getAllFolderName(foderPath, type);
            //node.Text = foderPath;
            foreach (string nn in FolderName)
            {
                DevComponents.AdvTree.Node node2 = new DevComponents.AdvTree.Node();
                node2.Text = nn.Substring(foderPath.Length);//.ToString();
                node.Nodes.AddRange(new DevComponents.AdvTree.Node[] { node2 });
                loadFolder(nn + "\\", type, node2);
            }


        }
        /// <summary>
        /// 得到指定目录下所有   文件   的名称列表
        /// </summary>
        /// <param name="strPath">文件目录路径</param>
        /// <returns></returns>
        public List<string> getAllFileName(string strPath, string type)
        {
            List<string> mList = new List<string>();
            DirectoryInfo TheFolder = new DirectoryInfo(strPath);
            //遍历文件夹
            foreach (FileInfo NextFolder in TheFolder.GetFiles(type))//"*.shp",
            {
                mList.Add(NextFolder.Name);
            }
            return mList;
        }

        /// <summary>
        /// 打开文件位置
        /// </summary>
        /// <param name="textEdit"></param>
        public void openFolder(DevComponents.DotNetBar.Controls.TextBoxX textEdit)
        {
            System.Windows.Forms.FolderBrowserDialog folder = new System.Windows.Forms.FolderBrowserDialog();

            if (folder.ShowDialog() == DialogResult.OK)
            {
                textEdit.Text = folder.SelectedPath;
            }
        }
        ///
        public void openFile(DevComponents.DotNetBar.Controls.TextBoxX textEdit)
        {
            //System.Windows.Forms.FileDialog File = new  System.Windows.Forms.FileDialog();
            OpenFileDialog File = new OpenFileDialog();
            if (File.ShowDialog() == DialogResult.OK)
            {
                textEdit.Text = File.FileName;
            }
        }
        /// 创建文件夹
        /// </summary>
        /// <param name="path">文件路径（含文件名）</param>
        public string createFolder(string path)
        {
            try
            {
                // Determine whether the directory exists.
                if (!Directory.Exists(path))
                {
                    //Create the directory it does not exist.
                    Directory.CreateDirectory(path);
                    return path;
                }
                else
                    return path;
                ////if (Directory.Exists(path))
                //{
                //    // Delete the target to ensure it is not there.
                //    if (MessageBox.Show("发现同名文件是否创建并覆盖？", "询问") == DialogResult.OK)
                //    {
                //        Directory.Delete(path, true);
                //        Directory.CreateDirectory(path);
                //        return path;
                //    }
                //    else
                //    {
                //        return path;
                //    }
                //}

                // Move the directory.
                //Directory.Move(path, target);

                //// Create a file in the directory.
                //File.CreateText(target + @"\myfile.txt");

                //// Count the files in the target directory.
                //Console.WriteLine("The number of files in {0} is {1}",
                //    target, Directory.GetFiles(target).Length);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return null;
            }
        }
        //读取excel文件
        public DataTable ExcelToDataTable(string strExcelFileName, string strSheetName)
        {
            string strConn;
            FileInfo file = new FileInfo(strExcelFileName);
            if (!file.Exists)
            {
                throw new Exception("文件不存在");
            }
            string extension = file.Extension;
            //源的定义
            switch (extension)
            {
                case ".xls":
                    strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + strExcelFileName + ";Extended Properties='Excel 8.0;HDR=Yes;IMEX=1;'";
                    break;
                case ".xlsx":
                    strConn = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + strExcelFileName + ";Extended Properties='Excel 12.0;HDR=Yes;IMEX=1;'";
                    break;
                default:
                    strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + strExcelFileName + ";Extended Properties='Excel 8.0;HDR=Yes;IMEX=1;'";
                    break;
            }
            //Sql语句
            //string strExcel = string.Format("select * from [{0}$]", strSheetName); 这是一种方法
            string strExcel = "select * from   [sheet1$]";

            //定义存放的数据表
            DataSet ds = new DataSet();

            //连接数据源
            OleDbConnection conn = new OleDbConnection(strConn);

            //conn.Open();

            //适配到数据源
            OleDbDataAdapter adapter = new OleDbDataAdapter(strExcel, conn);// strConn);
            adapter.Fill(ds, strSheetName);
            //conn.Close();
            return ds.Tables[strSheetName];
        }
        //线转面：测试不成功
        private void feture2line(string CN_code, Geoprocessor gp, string outPath, string path, string name)
        {
            IWorkspaceFactory pwokspace = new ShapefileWorkspaceFactoryClass();
            IWorkspace workspace = pwokspace.OpenFromFile(path, 0);
            IFeatureWorkspace pFeatureWorkspace = (IFeatureWorkspace)workspace;
            IFeatureClass featureClass = pFeatureWorkspace.OpenFeatureClass(name);
            //Msg("正在处理···" + CN_code + "···面转线");
            ESRI.ArcGIS.DataManagementTools.FeatureToLine fe2li = new FeatureToLine();
            fe2li.in_features = featureClass;
            fe2li.out_feature_class = outPath;
            gp.OverwriteOutput = true;
            gp.Execute(fe2li, null);
            MessageBox.Show(CN_code + "···面转线处理完成！");
        }

        /// <summary>
        /// 导入地图模板
        /// </summary>
        public void ImportMapModel(string mod, string wsp, string sources, string symbol)
        {
            //string wsp = @"G:\数据转换\项目数据管理\结果数据\MapResult";
            CreateWorkspace(wsp, sources, symbol);
            try
            {
                DirectoryInfo di = new DirectoryInfo(mod);
                FileInfo[] xml = di.GetFiles("*.xml");
                foreach (FileInfo nn in xml)
                {
                    string Mapxml = readXML(nn.FullName);
                    ModelApplication(Mapxml, wsp + ".smwu", nn.Name.Substring(0, nn.Name.Length - 4));
                    mo += nn.Name + "模板导入成功" + "\r\n";
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

        private Theme getTheme(SuperMap.Data.Workspace wks, string mapName)
        {
            try
            {
                SuperMap.Mapping.Map map = new SuperMap.Mapping.Map(wks);
                map.Open(mapName);
                Layers layers = map.Layers;
                Layer layer = null;
                for (Int32 i = 0; i < layers.Count; i++)
                {
                    layer = layers[i];
                    if (layer.Theme != null)
                    {
                        break;
                    }
                }
                Theme theme = layer.Theme;
                return theme;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }
        //创建工作空间（超图）
        public void CreateWorkspace(string wpsPath, string sources, string symbol)
        {
            // 创建工作空间，弹出 “关于”对话框
            SuperMap.Data.Workspace workspace = new SuperMap.Data.Workspace();
            WorkspaceConnectionInfo workspaceConnectionInfo = new WorkspaceConnectionInfo();
            workspaceConnectionInfo.Type = WorkspaceType.SMWU;

            workspaceConnectionInfo.Name = "MapResult";
            workspaceConnectionInfo.Password = "aaaazzzz";
            String file = wpsPath;
            workspaceConnectionInfo.Server = file;

            if (workspace.Create(workspaceConnectionInfo))
            {
                //MessageBox.Show("创建工作空间成功");
                workspace.Caption = "MapResult";
                workspace.Save();
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
                di = new DirectoryInfo(sources);
                FileInfo[] fl = di.GetFiles("*.udb");
                for (int s = 0; s < fl.Length; s++)
                {
                    DatasourceConnectionInfo ds = new DatasourceConnectionInfo();
                    ds.Alias = fl[s].Name.Substring(0, fl[s].Name.Length - 4);
                    ds.Password = "aaaazzzz";
                    ds.Server = sources + "\\" + fl[s].ToString();
                    //ds.Password = "aaaazzzz";
                    Datasource datasource = workspace.Datasources.Open(ds);
                    if (ds.Alias.Substring(0, 4) == "ymgc")
                    {
                        ProjectConverTest(@"G:\移动风险监测\参考坐标\CGCS_2000.xml", datasource);
                        System.Threading.Thread.Sleep(100);
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
    }
}






