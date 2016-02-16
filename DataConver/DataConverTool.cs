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
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.ADF.BaseClasses;
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
        public string outPath_Mid;
        private string fhys = null;
        private List<string> GDBfold;
        private string fiedname = "Code";
        DevExpress.XtraEditors.MarqueeProgressBarControl progressBar;
        DevComponents.DotNetBar.Controls.RichTextBoxEx MessageShow;
        DevComponents.DotNetBar.LabelX lab_progress;
        DevComponents.DotNetBar.LabelX lb;
        int max=0;
        AxMapControl axMapControl =null;
        public DataConverTool(DevComponents.DotNetBar.LabelX lab_progress, DevComponents.DotNetBar.LabelX lb, DevExpress.XtraEditors.MarqueeProgressBarControl progressBar, DevComponents.DotNetBar.Controls.RichTextBoxEx MessageShow,AxMapControl axMap)
        {
            this.axMapControl = axMap;
            this.lb = lb;
            this.lab_progress = lab_progress;
            this.progressBar = progressBar;
            this.MessageShow = MessageShow;
        }
        //实时进度信息
        public void pgrs(int p)
        {
            double percent = Convert.ToDouble(p) / Convert.ToDouble(max);
            lb.Text = percent.ToString("0%");
        }
        //实时输出信息
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
        //耗时计算函数
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
        //主处理函数
//=======================================================================================
        public void DealAll(string dataPath, string password)
        {
            set.passWod = password;
            DateTime dt = DateTime.Now;
            string tiff = null;
            string mod = null;
            string fxtb = null;
            string outPath = null;
            string outPath_Mid = null;
            string outPath_Final=null;
            //string mxdPath = null;//完整地图路径
            try
            {
                if (dataPath.Substring(dataPath.Length - 3) == "已处理")
                    return;//判断该编制单元是否被处理，是则返回，不是则继续执行
                di = new DirectoryInfo(dataPath);
                FileInfo[] information = di.GetFiles("基本信息.txt",SearchOption.AllDirectories);
                FileInfo[] yuanData = di.GetFiles("元数据.xml", SearchOption.AllDirectories);
                
                //成果图的获取
                DirectoryInfo[] picture = di.GetDirectories("*成果图件*", SearchOption.AllDirectories);
                //checkPath(picture[0].FullName);
                //地图数据获取
                FileInfo[] mxd = di.GetFiles("完整地图.mxd", SearchOption.AllDirectories);
                //checkPath(mxd[0].FullName);
                //设置淹没过程影像文件路径
                DirectoryInfo[] tiff1 = di.GetDirectories("tiffPath", SearchOption.AllDirectories);
                tiff = tiff1[0].FullName;
                
                
                MessageShow.Clear();
                mod = set.mod;//设置地图模板路径
                //设置编制单元名称，用于信息输出
                fxtb = dataPath.Substring(dataPath.LastIndexOf("\\") + 1);
                datetime = DateTime.Now;//设置开始实时间
                lab_progress.Text = "正在处理：【" + fxtb + "】";
                Msg("正在处理河段：" + fxtb);
                //创建输出对应目录
                outPath = importTool.createFolder(set.deal_path + "\\" + fxtb + "_已处理");
                //创建输出中间数据目录
                outPath_Mid = importTool.createFolder(outPath + "\\" + "中间数据");
                //创建超图结果数据目录
                outPath_Final = importTool.createFolder(outPath + "\\" + fxtb);
                File.Copy(information[0].FullName, outPath_Final +"\\"+ information[0].Name,true);
                File.Copy(yuanData[0].FullName, outPath_Final + "\\"+yuanData[0].Name,true);
               #region 测试
                //测试读取属性数据
                

                DateTime dt1 = DateTime.Now;
                //bhzy(outPath_Mid, outPath_Final);
                //slgc(outPath_Final, outPath_Mid);
                //ztdt(outPath_Final, outPath_Mid);
                CSVImport("bhzyAttr", outPath_Final);
                //yxfx(dataPath, outPath_Final);
                //Feature2Raster(gp, @"D:\移动风险监测\新数据测试数据\6风险图应用业务相关数据\6.2淹没过程动态展示支撑数据\ymss1.shp", @"D:\移动风险监测\新数据测试数据\tiffPath\time1.tif");
                string txtPath = @"D:\移动风险监测\新数据测试数据\6风险图应用业务相关数据\6.2淹没过程动态展示支撑数据\time50.txt";
                string shpFilePath = @"D:\移动风险监测\新数据测试数据\6风险图应用业务相关数据\6.2淹没过程动态展示支撑数据\copy";
                string shpFileName = "ymss1";
                string SavaPath = @"D:\移动风险监测\新数据测试数据\tiffPath\" + shpFileName;
                //ESRI.ArcGIS.ADF.COMSupport.AOUninitialize.Shutdown();
                //Feature2Raster(gp,shpFilePath+"\\"+shpFileName+".shp", SavaPath+"\\time.tif");
                //JoinPoint(gp, shpFilePath+"\\", shpFileName, "czLayer","ymcz");

                MessageBox.Show(caculateCountry("ymcz", shpFilePath).ToString());
                MessageBox.Show(Caculate(shpFileName, shpFilePath, "GRIDAREA").ToString());
                readTXT(txtPath, shpFilePath, shpFileName, SavaPath);
                MessageBox.Show(ExecDateDiff(dt1, DateTime.Now));
               //setValue("ymss1", @"D:\移动风险监测\新数据测试数据\6风险图应用业务相关数据\6.2淹没过程动态展示支撑数据", value);

                /*tlab_progress.Text = "【" + fxtb + "】正在导入避洪转移数据···";
                bhzy(dataPath, outPath_Final);
                lab_progress.Text = "【" + fxtb + "】避洪转移数据导入完成";
                ry
                {
                    lab_progress.Text = "【" + fxtb + "】正在加载地图模板···";
                    Msg("正在加载地图模板···");
                    ImportMapModel(mod, outPath_Final + "\\" + "MapResult", outPath_Final, set.mod);

                }
                catch(Exception ex)
                {
                    Msg(ex.Message);
                }
                //ymgc(fxtb, outPath_Final, outPath_Mid, tiff);//set.TIFFPath
                //ymgc(fxtb, outPath_Final, outPath_Mid, tiff);//淹没过程影像数据导入

                //  ImportMapModel(mod, outPath_Final + "\\" + "MapResult", outPath_Final, set.mod);*/
                #endregion
                //接下来进行mxd的加载；
                //string ph = mxd[0].FullName;
               
                
 //----------------------------------------------------------------------------------
              
//----------------------------------------------------------------------------------------
                Msg(lab_progress.Text = "【" + fxtb + "】正在进行数据转换···");
                axMapControl.LoadMxFile(mxd[0].FullName, 0, Type.Missing);
                //提取数据函数调用
                getGroupLayer(axMapControl, outPath_Mid);
                Msg("本次处理数据总数为：" + (vec_count + ras_count).ToString() + "条" + "\r\n" + "其中处理矢量数据：" + vec_count.ToString() + "条" + "\r\n" + "处理栅格数据：" + ras_count.ToString() + "条");
                Msg("用时：" + ExecDateDiff(datetime, DateTime.Now));//显示用时
//---------------------------------------------------------------------------------------
               
                //System.Threading.Thread.Sleep(100);
                Msg(lab_progress.Text = "【" + fxtb + "】正在导入淹没过程影像数据···");
               //淹没过程影像数据导入
                //ymgcImport(outPath_Final, SavaPath);
                //lab_progress.Text = "【" + fxtb + "】淹没过程影像数据导入完成";
                //System.Threading.Thread.Sleep(100);
//----------------------------------------------------------------------------------------
                //水利工程数据导入
                Msg(lab_progress.Text = "【" + fxtb + "】正在导入水利工程数据···");
                slgcImport(outPath_Final, outPath_Mid);
                //lab_progress.Text = "【" + fxtb + "】水利工程数据导入完成";
                //System.Threading.Thread.Sleep(100);
//----------------------------------------------------------------------------------------
                //专题地图的导入
                Msg(lab_progress.Text = "【" + fxtb + "】正在导入专题地图数据···");
                ztdtImport(outPath_Final, outPath_Mid);
                //lab_progress.Text = "【" + fxtb + "】专题地图导入完成";
                //System.Threading.Thread.Sleep(100);
 //----------------------------------------------------------------------------------------
                //避洪转移数据导入
                Msg(lab_progress.Text = "【" + fxtb + "】正在导入避洪转移数据···");
                bhzyImport(dataPath, outPath_Final);
                //lab_progress.Text = "【" + fxtb + "】避洪转移数据导入完成";
                //System.Threading.Thread.Sleep(100);
 //----------------------------------------------------------------------------------------
                //复合要素数据导入
                Msg(lab_progress.Text = "【" + fxtb + "】正在导入复合要素数据···");
                fhysImport(outPath_Final, outPath_Mid);
                //lab_progress.Text = "【" + fxtb + "】避洪转移数据导入完成";
                //System.Threading.Thread.Sleep(100);
//----------------------------------------------------------------------------------------
//影响分析数据导入
                Msg(lab_progress.Text = "【" + fxtb + "】正在导入影响分析数据···");
                yxfxImport(dataPath, outPath_Final);
                //lab_progress.Text = "【" + fxtb + "】影响分析数据导入完成";
                //System.Threading.Thread.Sleep(100);
                
//----------------------------------------------------------------------------------------
//更新影响分析数据属性表
                Msg(lab_progress.Text = "【" + fxtb + "】正在更新属性数据···");
                try
                {

                    FileInfo[] xlsPath = di.GetFiles("*影响分析支撑数据*.xls*", SearchOption.AllDirectories);
                    foreach (FileInfo xx in xlsPath)
                    {
                        ConnectAttribute(xx.FullName, outPath_Final + "\\" + "yxfx");
                    }
                }
                catch
                {
                    Msg("更新属性数据不存在，跳过更新");
                }
//----------------------------------------------------------------------------------------
              //导入地图模板
                Msg(lab_progress.Text = "【" + fxtb + "】正在加载地图模板···");
                
                datetime = DateTime.Now;
                ImportMapModel(mod, outPath_Final + "\\" + "MapResult", outPath_Final, set.mod);
                //Msg(importTool.mo);
                Msg("用时：" + DataConverTool.ExecDateDiff(datetime, DateTime.Now));
                //lab_progress.Text = "【" + fxtb + "】加载地图模板完成";
//----------------------------------------------------------------------------------------
                //复制成果图
                Msg(lab_progress.Text = "【" + fxtb + "】正在复制成果图···");

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
                //File.Copy(information[0].FullName, outPath_Final + information[0].Name);
                //File.Copy(yuanData[0].FullName, outPath_Final + yuanData[0].Name);

//----------------------------------------------------------------------------------------
//生成处理报告
                Msg("正在生成处理报告····");
                Msg("本编制单元用时：" + DataConverTool.ExecDateDiff(dt, DateTime.Now));
                MessageShow.SaveFile(dataPath + "_处理报告.txt", RichTextBoxStreamType.TextTextOleObjs);
                Msg(dataPath + "_处理报告.txt  已生成成功!");
                lab_progress.Text = "【" + fxtb + "】处理完成";
                Directory.Move(dataPath, dataPath + "_已处理");
                //System.Threading.Thread.Sleep(1000);//等待一秒；
            }
            catch (Exception ex)
            {
                Msg(ex.Message);
            }
        }
//======================================================================================
        //根据名字获得图层
        private IFeatureLayer GetFeatureLayer(string layerName)
        {
            //get the layers from the maps
            IEnumLayer layers = GetLayers();
            layers.Reset();
            ILayer layer = null;
            while ((layer = layers.Next()) != null)
            {
                if (layer.Name == layerName)
                    return layer as IFeatureLayer;
            }

            return null;
        }
        private IRasterLayer GetRasterLayer(string layerName)
        {
            //get the layers from the maps
            IEnumLayer layers = GetLayers();
            layers.Reset();

            ILayer layer = null;
            while ((layer = layers.Next()) != null)
            {
                if (layer.Name == layerName)
                    return layer as IRasterLayer;
            }

            return null;
        }
        private IEnumLayer GetLayers()
        {

            UID uid = new UIDClass();
            uid.Value = "{6CA416B1-E160-11D2-9F4E-00C04F6BC78E}";//获取所有图层
            //uid.Value = "{40A9E885-5533-11d0-98BE-00805F7CED21}";// 代表只获取矢量图层
            //问题在这个地方 解决！
            IEnumLayer layers = axMapControl.ActiveView.FocusMap.get_Layers(uid, true);
            return layers;
        }
        //提取featureclass图层的路径信息
        private string FeatureClassSourse(IFeatureLayer pFeatureLayer)
        { 
            string featureClassSourse=null;
            try
            {
                //pFeatureLayer = GetFeatureLayer(LayerName);
                if (pFeatureLayer.DataSourceType == "File Geodatabase Feature Class")
                {
                    IFeatureClass fc = pFeatureLayer.FeatureClass;
                    string name1 = fc.AliasName;
                    IFeatureDataset fds = fc.FeatureDataset;
                    string name2 = fds.BrowseName;
                    string name3 = fds.Workspace.PathName;
                     featureClassSourse = name3 + "\\" + name2 + "\\" + name1;
                     return featureClassSourse;
                }
                else
                    return null;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }  
        //提取RasterDataSourse图层的路径信息
        private string RasterDataSourse( IRasterLayer pRasterLayer)
        {
            try
            {
                //IRasterLayer pRasterLayer = GetRasterLayer(LayerName);
                return pRasterLayer.FilePath.Substring(0, pRasterLayer.FilePath.LastIndexOf("\\"));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }
        //提取矢量数据
        private void ExtractFeatureClass(string LayerName,Geoprocessor gp, string aimPath, string sourcePath)
        {
            Msg("开始处理..."+LayerName );
            try
            {
                ESRI.ArcGIS.DataManagementTools.CopyFeatures copyFeature = new CopyFeatures();
                copyFeature.in_features = sourcePath;
                copyFeature.out_feature_class = aimPath + "\\" + LayerName;
                gp.OverwriteOutput = true;
                gp.Execute(copyFeature, null);
                Msg(LayerName + "转换完成！");
                vec_count++;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }

        }
        //提取矢量数据
        private void ExtractRaster(string LayerName,Geoprocessor gp, string aimPath, string sourcePath)
        {
            Msg("开始处理..." + LayerName);
            try
            {
                ESRI.ArcGIS.DataManagementTools.CopyRaster copyRaster = new CopyRaster();
                copyRaster.in_raster = sourcePath;
                copyRaster.out_rasterdataset = aimPath + "\\" + LayerName + ".tif";
                gp.OverwriteOutput = true;
                gp.Execute(copyRaster, null);
                Msg(LayerName + "转换完成！");
                ras_count++;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
        }
       //各个udb的导入函数
 //--------------------------------------------------------------------------------------
        //水利工程导入函数
        private void slgcImport(string outPath_Final, string outPath_Mid)
        {
            
            try
            {
                //创建udb并加密
                //DatasourceConnectionInfo info = CreatUDB(outPath_Final, "slgc", set.passWod);
                DatasourceConnectionInfo info = new DatasourceConnectionInfo();
                info.Password = set.passWod;
                info.Server = outPath_Final + "\\" + "slgc.udb"; 
                DataImport("*.shp", "*.tif", outPath_Mid + "\\slgc\\", info);
                wks.Datasources.CloseAll();
            }
            catch (Exception ex)
            {
                Msg(ex.Message);
            }
        }
        //专题地图导入函数
        public void ztdtImport(string outPath_Final, string outPath_Mid)
        {
            try
            {
                //DatasourceConnectionInfo info = CreatUDB(outPath_Final, "ztdt", set.passWod);
                DatasourceConnectionInfo info = new DatasourceConnectionInfo();
                info.Password = set.passWod;
                info.Server = outPath_Final + "\\" + "ztdt.udb";
                DataImport("*.shp", "*.tif", outPath_Mid + "\\ztdt\\", info);
                wks.Datasources.CloseAll();
            }
            catch (Exception ex)
            {
                Msg(ex.Message);
            }
        }
        //复合要素方案导入
        public void fhysImport(string outPath_Final, string outPath_Mid)
        {
            try
            {
                //DatasourceConnectionInfo info = CreatUDB(outPath_Final, "ztdt", set.passWod);
                DatasourceConnectionInfo info = new DatasourceConnectionInfo();
                info.Password = set.passWod;
                info.Server = outPath_Final + "\\" + "fhys.udb";
                DataImport("*.shp", "*.tif", outPath_Mid + "\\fhys\\", info);
                wks.Datasources.CloseAll();
            }
            catch (Exception ex)
            {
                Msg(ex.Message);
            }
        }
        //影响分析导入函数
        public void yxfxImport(string yxfx_path, string outPath_Final)
        {
            try
            {
                string mc;//设置过滤水系面状的数据
                datetime = DateTime.Now;
                //创建数据源
                di = new DirectoryInfo(yxfx_path);
                DirectoryInfo[] yxfxPath = di.GetDirectories("6.3影响分析支撑数据", SearchOption.AllDirectories);
                DatasourceConnectionInfo info = new DatasourceConnectionInfo();
                info.Password = set.passWod;
                info.Server = outPath_Final + "\\" + "yxfx.udb";
                foreach (DirectoryInfo dd in yxfxPath)
                {
                    ////遍历文件夹
                    FileInfo[] info_file = dd.GetFiles("*.shp", SearchOption.AllDirectories);
                    max = info_file.Length; int p = 0;
                    foreach (FileInfo NextFolder in info_file)//"*.shp",
                    {

                        mc = NextFolder.Name;
                        if (mc == "水系面状.shp")
                            continue;
                        importTool.ImportShp(NextFolder.FullName, info);
                        pgrs(++p);
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
        //淹没过程数据的导入
        public void ymgcImport(string outPath_Final,string ymgc_path)
        {
            try
            {
                //DatasourceConnectionInfo info = CreatUDB(outPath_Final, "ztdt", set.passWod);
                DatasourceConnectionInfo info = new DatasourceConnectionInfo();
                info.Password = set.passWod;
                info.Server = outPath_Final + "\\" + "ymgc1.udb";
                DataImport("*.shp", "*.tif", ymgc_path, info);
                wks.Datasources.CloseAll();
            }
            catch (Exception ex)
            {
                Msg(ex.Message);
            }
        }
        //避洪转移数据导入
       /* public void bhzy(string outPath_Mid, string outPath_Final)
        {
            try
            {
                //DatasourceConnectionInfo info = CreatUDB(outPath_Final, "bhzy", set.passWod);
                DatasourceConnectionInfo info = new DatasourceConnectionInfo();
                info.Password = set.passWod;
                info.Server = outPath_Final + "\\" + "bhzy.udb";
                DataImport("*.shp", "*.tif", outPath_Mid + "\\bhzy\\", info);
                wks.Datasources.CloseAll();
            }
            catch (Exception ex)
            {
                Msg(ex.Message);
            }

        }*/
        public void bhzyImport(string bhzy_path, string outPath_Final)
        {
            try
            {
                datetime = DateTime.Now;
                //创建数据源
                di = new DirectoryInfo(bhzy_path);
                DirectoryInfo[] bhzyPath = di.GetDirectories("6.4避洪转移展示支撑数据", SearchOption.AllDirectories);
                DatasourceConnectionInfo info = new DatasourceConnectionInfo();
                info.Password = set.passWod;
                info.Server = outPath_Final + "\\" + "bhzy.udb";
                FileInfo[] info_file = bhzyPath[0].GetFiles("*.shp", SearchOption.AllDirectories);
                max = info_file.Length; int p = 0;
                foreach (FileInfo NextFolder in info_file)//"*.shp",
                {
                    importTool.ImportShp(NextFolder.FullName, info);
                    pgrs(++p);
                    Msg(NextFolder.Name + "导入成功！");
                }
                Msg("本次成功导入数据" + importTool.i.ToString() + "条");
                Msg("用时：" + ExecDateDiff(datetime, DateTime.Now));
                //}
                wks.Datasources.CloseAll();
            }
            catch (Exception ex)
            {
                Msg(ex.Message);
            }
        }
        //excel数据表格导入
        public void CSVImport(string CSVTargetName, string outPath_Final)
        {
            try
            {
               
                DatasourceConnectionInfo info = new DatasourceConnectionInfo();
                info.Password = set.passWod;
                info.Server = outPath_Final + "\\" + "bhzy.udb";
                FileInfo[] info_file = di.GetFiles("*.csv", SearchOption.AllDirectories);
               
                foreach (FileInfo NextFolder in info_file)//"*.shp",
                {
                    importTool.ImportCSV(CSVTargetName, NextFolder.FullName, info);
                   
                    Msg(NextFolder.Name + "导入成功！");
                }
                
              
                wks.Datasources.CloseAll();
            }
            catch (Exception ex)
            {
                Msg(ex.Message);
            }
        }
        //数据导入函数
        private void DataImport(string type1, string type2, string sourPath, DatasourceConnectionInfo info)
        {

            try
            {

                datetime = DateTime.Now;
                List<string> shp = importTool.getAllFileName(sourPath, type1);
                int p = 0;
                max = shp.Count;
                foreach (string l1 in shp)
                {

                    if (l1 == "水系面状.shp")
                        continue;
                    importTool.ImportShp(sourPath + l1, info);// resultData + "\\" + jcdt.Text);
                    pgrs(++p);
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
                Msg("导入任务数据总" + (shp.Count + tif.Count).ToString() + "条");//+ "\r\n" + "本次成功导入数据" + importTool.i.ToString() + "条");
                Msg("用时：" + ExecDateDiff(datetime, DateTime.Now));
            }
            catch (Exception ex)
            {
                Msg(ex.Message);
            }
        }

//--------------------------------------------------------------------------------------
        //读取groupLayer
        public void getGroupLayer(AxMapControl mapControl, string outPath_Mid)
        {
            //IFeatureLayer layer = null;
            max = mapControl.LayerCount;
           
            for (int i = 0; i < mapControl.LayerCount; i++)
            {
                pgrs(i);
                ILayer layers = mapControl.get_Layer(i);
                if (layers is GroupLayer || layers is ICompositeLayer)   //判断是否是groupLayer
                {
                    //MessageBox.Show(layers.Name);
                    //创建文件夹：slgc，ztdt，bhzy
                    if (layers.Name.Equals("水利工程"))
                    {
                        //创建文件夹：slgc
                        string slgcPath= importTool.createFolder(outPath_Mid+"\\slgc");
                        //将该文件路径传入函数中
                        getSubLayer(layers, slgcPath,null,false);  //递归的思想
                    }
                    else if (layers.Name.Substring(0, 2).Equals("方案"))
                    {
                        
                        //创建ztdu文件夹
                        string ztdtPath= importTool.createFolder(outPath_Mid + "\\ztdt");

                        //传入参数  ztdt和方案i

                        getSubLayer(layers, ztdtPath,layers.Name+"_",false);  //递归的思想

                    }
                    else
                    {
                        //创建bhzy文件夹
                        string bhzyPath = importTool.createFolder(outPath_Mid + "\\fhys");
                        ////传入参数
                        getSubLayer(layers, bhzyPath, null,true);  //递归的思想
                    }

                }
                else
                {
                   
                }
            }
            //MessageBox.Show(layer.Name);
            //return layer;
        }
        //读取子图层
        public void  getSubLayer(ILayer layers,string ExportPath,string plan,bool Isfhys)
        {
           
            ICompositeLayer compositeLayer = layers as ICompositeLayer;
            for (int i = 0; i < compositeLayer.Count; i++)
            {
                ILayer layer = compositeLayer.Layer[i];   //递归
                if (layer is GroupLayer || layer is ICompositeLayer)
                {
                    if (Isfhys)
                    {
                        fhys = layer.Name+"_";
                    }
                    //MessageBox.Show(layer.Name);
                    getSubLayer(layer, ExportPath,plan,Isfhys);
                }
                else
                {
                    try
                    {
                        IFeatureLayer l  = layer as IFeatureLayer;
                        IRasterLayer r = layer as IRasterLayer;
                        if (plan != null)
                        {
                            layer.Name = plan+layer.Name;
                        }
                        
                        if (l != null)
                        {
                            if (Isfhys)
                                layer.Name = fhys + layer.Name;
                           ExtractFeatureClass(layer.Name, gp, ExportPath, FeatureClassSourse(l));
                        }
                        else if (r != null)
                        {
                            if (Isfhys)
                                r.Name = fhys + r.Name;
                            ExtractRaster(r.Name, gp, ExportPath, RasterDataSourse(r));
                        }
                        //Msg(layer.Name);
                    }
                    catch
                    {
                        
                       
                    }
                    //}
                }
            }

            //return l;
        }
        //检查路径的完整性
        public void checkPath(string path)
        {
            if (!System.IO.Directory.Exists(path))//|| !System.IO.Directory.Exists(ymgc_tifPath.Text)
            {
                MessageBox.Show("请输入正确编制单元路径···", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
               
            }
        }
//======================================================================================
        //根据txt文件内容提取tif
        //读取txt信息函数  返回一个数组/根据对应的前面数据返回哈希表
        public void readTXT(string txtPath, string shpFilePath, string shpFileName, string SavaPath)
        {
            importTool.createFolder(SavaPath);
            //List<Hashtable> Value = new List<Hashtable>();
            Hashtable hs = null;
            List<string> line = new List<string>();
            string l;
            try
            {
                StreamReader sr = new StreamReader(txtPath);
                while ((l=sr.ReadLine()) != null)
                {
                    string[] split = l.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    
                    if (l.Length < 17)
                    {
                        line.Add(split[0]);
                        if (hs != null)
                        {
                            lab_progress.Text = "正在生成time" + line[line.Count - 2] + ".tif·····";
                            setValue(shpFileName, shpFilePath, hs);
                            //ESRI.ArcGIS.ADF.COMSupport.AOUninitialize.Shutdown();
                            Feature2Raster(gp,shpFilePath+"\\"+shpFileName+".shp", SavaPath+"\\time" + line[line.Count-2] + ".tif");
                        
                        }
                        
                        //line = new List<string>();
                        hs = new Hashtable();
                        continue;
                    }
                        object key = split[0];
                        object value = split[1];
                        hs.Add(key, value);//将读取的txt数值存入
                        
                }
                lab_progress.Text = "正在生成time" + line[line.Count - 1] + ".tif·····";
               setValue(shpFileName, shpFilePath, hs);
                Feature2Raster(gp, shpFilePath + "\\" + shpFileName+".shp", SavaPath + "\\time" + line[line.Count - 1] + ".tif");
                lab_progress.Text = "淹没过程影像数据生成完成！";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            //return hs;
        }
        //对shpfile的赋值函数
        public void setValue(string shpFileName, string shpFilePath, Hashtable hsTable)
        {
            try
            {
                //进行对shp数据的复制然后进行字段为零时的删除  完成后对点数据进行叠加分析
                //IWorkspaceFactory pWorkspaceFactory = new ShapefileWorkspaceFactoryClass();
                //IFeatureWorkspace pFeatureWorkspace = (IFeatureWorkspace)pWorkspaceFactory.OpenFromFile(shpFilePath, 0);
                //IFeatureLayer pFeatureLayer = new FeatureLayerClass();
                //pFeatureLayer.FeatureClass = pFeatureWorkspace.OpenFeatureClass(shpFileName);
                //pFeatureLayer.Name = pFeatureLayer.FeatureClass.AliasName;
                IFeatureClass pFeatureClass = shpToFeatureClass(shpFileName, shpFilePath);
                //IFeatureClass pFeatureClass = pFeatureLayer.FeatureClass;
                //使要素处于编辑状态
                IDataset dataset = (IDataset)pFeatureClass;
                IWorkspace workspace = dataset.Workspace;
                IWorkspaceEdit workspaceEdit = (IWorkspaceEdit)workspace;
                workspaceEdit.StartEditing(true);
                workspaceEdit.StartEditOperation();
                //int i = 0;
                if (pFeatureClass != null)
                {
                    int nIndex = pFeatureClass.FindField("GRIDCODE");
                    int ValueIndex = pFeatureClass.FindField("VALUE");
                    if (nIndex != -1)
                    {
                        IField pField = pFeatureClass.Fields.get_Field(nIndex);
                        IQueryFilter pFilter = new QueryFilterClass();
                        //IFeatureCursor FCursor = pLayer.FeatureClass.Search(new QueryFilterClass(), false);  
                       
                        IFeatureCursor pCursor = pFeatureClass.Update(pFilter, false);
                        IFeature pFeature = pCursor.NextFeature();
                        IFields pFields = pFeature.Fields;
                        //修改根据gridcode给其赋值
                        while (pFeature != null)
                        {
                            string c = pFeature.get_Value(nIndex).ToString();
                            double v = Convert.ToDouble(hsTable[pFeature.get_Value(nIndex).ToString() as object].ToString());
                            
                            pFeature.set_Value(ValueIndex, v);//赋值语句
                            pCursor.UpdateFeature(pFeature);
                            pFeature = pCursor.NextFeature();
                            
                            //i++;
                        }
                        //pFilter.WhereClause = "VALUE=0";//怎么修改呢？
                        //ITable pTable = (ITable)pFeatureClass;
                        //pTable.DeleteSearchedRows(pFilter);
                    }
                }
                //关闭要素编辑状态

                workspaceEdit.StopEditing(true);
                workspaceEdit.StopEditOperation();
                //workspace.Exists();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            //获取shpfile对象
        }
        //table赋值和删除要素。
        public void SetValue(string shpFileName, string shpFilePath, Hashtable hsTable)
        {
            try
            {
                IWorkspaceFactory pWorkspaceFactory = new ShapefileWorkspaceFactoryClass();
                IFeatureWorkspace pFeatureWorkspace = (IFeatureWorkspace)pWorkspaceFactory.OpenFromFile(shpFilePath, 0);
                IFeatureLayer pFeatureLayer = new FeatureLayerClass();
                pFeatureLayer.FeatureClass = pFeatureWorkspace.OpenFeatureClass(shpFileName);
                IFeatureClass pFeatureClass = pFeatureLayer.FeatureClass;
                IWorkspaceEdit workspaceEdit = (IWorkspaceEdit)pFeatureWorkspace;
                workspaceEdit.StartEditing(true);
                workspaceEdit.StartEditOperation();
                ITable pTable = (ITable)pFeatureClass;
                
                ICursor pcursor = pTable.Update(null, false);
                IRow pRow = pcursor.NextRow();
                int nIndex = pFeatureClass.FindField("GRIDCODE");
                    int ValueIndex = pFeatureClass.FindField("VALUE");
                    if (nIndex != -1)
                    {
                        IField pField = pFeatureClass.Fields.get_Field(nIndex);
                        IQueryFilter pFilter = new QueryFilterClass();
                        pFilter.WhereClause = "VALUE =  0";
                        for (int i = 0; i < pTable.RowCount(null); i++)
                        {
                            string c = pRow.get_Value(nIndex).ToString();
                            double v = Convert.ToDouble(hsTable[c as object].ToString());
                            pRow.set_Value(ValueIndex, v);
                            pcursor.UpdateRow(pRow);
                            pRow = pcursor.NextRow();
                        }
                        pTable.DeleteSearchedRows(pFilter);
                        workspaceEdit.StopEditing(true);
                        workspaceEdit.StopEditOperation();
                        
                    }

            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //copy要素赋值
        public void CopyFeature(Geoprocessor gp, string sourseFeature,string aimFeature)
        {
            try
            {
                ESRI.ArcGIS.DataManagementTools.CopyFeatures copyFeature = new CopyFeatures(sourseFeature,aimFeature);
                gp.Execute(copyFeature,null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //Feature2Raster转换函数
        public void Feature2Raster(Geoprocessor gp,string shpFile,string SavaPath)
        {
            try
            {
                ESRI.ArcGIS.ConversionTools.FeatureToRaster feature2Raster = new ESRI.ArcGIS.ConversionTools.FeatureToRaster();
                feature2Raster.in_features = shpFile;
                feature2Raster.field = "VALUE";
                feature2Raster.out_raster = SavaPath;
                gp.Execute(feature2Raster,null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //计算属性中某字段和
        public Double Caculate(string shpFileName, string shpFilePath,string fileName)
        {
            try
            {
                IFeatureClass pFeatureClass = shpToFeatureClass(shpFileName, shpFilePath);
                ITable pTable = (ITable)pFeatureClass;
                double count=0;
                ICursor pcursor = pTable.Update(null, false);
                IRow pRow = pcursor.NextRow();
                int nIndex = pFeatureClass.FindField(fileName);
                if (nIndex != -1)
                {
                    IField pField = pFeatureClass.Fields.get_Field(nIndex);
                    for (int i = 0; i < pTable.RowCount(null); i++)
                    {
                        string c = pRow.get_Value(nIndex).ToString();
                        double v = Convert.ToDouble(c);
                        count += v;
                        pRow = pcursor.NextRow();
                    }
                    
                    return count;
                   
                }
                else
                    return 0;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return 0;
            }
        }
        //村庄点面叠置
        public void JoinPoint(Geoprocessor gp,string environment,string SourseName1,string SourseName2,string SavaName)
        {
            try
            {
                ESRI.ArcGIS.AnalysisTools.Intersect  intersect = new Intersect();
                intersect.in_features = environment + SourseName1 + ".shp;" + environment + SourseName2 + ".shp";
                    //@"D:\移动风险监测\新数据测试数据\6风险图应用业务相关数据\6.2淹没过程动态展示支撑数据\copy\ymss1.shp;D:\移动风险监测\新数据测试数据\6风险图应用业务相关数据\6.2淹没过程动态展示支撑数据\copy\czLayer.shp";
                intersect.out_feature_class = environment + SavaName;
                intersect.output_type = "POINT";
                gp.Execute(intersect, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //村庄点面叠置统计
        public Double caculateCountry(string shpFileName, string shpFilePath)
        {
            try
            {
                IFeatureClass pFeatureClass= shpToFeatureClass(shpFileName, shpFilePath);
                ITable pTable = (ITable)pFeatureClass;
                double count = pTable.RowCount(null);
                return count;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return 0;
            }
        }
        //处理shp文件
        public IFeatureClass shpToFeatureClass(string shpFileName, string shpFilePath)
        {
            try
            {
                IWorkspaceFactory pWorkspaceFactory = new ShapefileWorkspaceFactoryClass();
                IFeatureWorkspace pFeatureWorkspace = (IFeatureWorkspace)pWorkspaceFactory.OpenFromFile(shpFilePath, 0);
                IFeatureLayer pFeatureLayer = new FeatureLayerClass();
                pFeatureLayer.FeatureClass = pFeatureWorkspace.OpenFeatureClass(shpFileName);
                //IFeatureClass pFeatureClass = pFeatureLayer.FeatureClass;
                return pFeatureLayer.FeatureClass;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }
//---------------------------------------------------------------------------------------------------
        //excel数据转CSV
        public void TransferCSV()
        {
            try
            {

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //csv数据导入数据源
        public void ImportCSV()
        {
            try
            {

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
//======================================================================================
        
        
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
                        importTool.refreshMod(wsp, nn.Name.Substring(0, nn.Name.Length - 4), set.passWod, @"D:\洪水风险图移动端数据预处理工具\Reference\参考专题图模板.xml");
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
                Msg(lab_progress.Text = "正在导入符号库···");
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
                    Msg(lab_progress.Text = "添加数据源：" + ds.Alias);
                    
                    ds.Server = sources + "\\" + fl[s].ToString();
                    ds.Password = set.passWod;
                    
                    Datasource datasource = workspace.Datasources.Open(ds);
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
            IGpEnumList featureClasses = gp.ListFeatureClasses("", "", "");//直接搜索featureClass；
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
            //RasterCatalog rasterCatalog = ;

            //IGpEnumList rasterCatalog = gp.ListDatasets("", "");
            //string rasterCalog = rasterCatalog.Next();
            //while (rasterCalog != "")
            //{
            //    MessageBox.Show(rasterCalog);
            //    rasterCalog = rasterCatalog.Next();
            //}
            IGpEnumList dataSets = gp.ListDatasets("", "");//搜寻dataset；
            string dataSet = dataSets.Next();
            while (dataSet != "")
            {
                try
                {
                    IFeatureDataset pFeatureDataset = pFeatureWorkspace.OpenFeatureDataset(dataSet);
                    IFeatureClassContainer pFCC = (IFeatureClassContainer)pFeatureDataset;
                    IEnumFeatureClass pEnumFeatureClass = pFCC.Classes;
                    IFeatureClass m_FeatureClass = pEnumFeatureClass.Next();
                
                if(dataSet=="SchemeRasterData")//提取栅格数据集
                {/*说明：每个方案中对应的需要提取出来的数据有：
                  * 1)到达时间2）淹没水深3）淹没历时4）淹没图。
                  * 四种数据在不同方案中对应不同的数据
                  * 要求：方案1_“四种数据名称”;
                  * 在方案1中：OBJECTID=9（到达时间）
                  */

                }
                else if (dataSet == "RiskDataUnk")//专题图数据提取
                {
                    string ztdtFolder=importTool.createFolder(outPath_Mid + "\\ztdt");
                    string bxzyFolder = importTool.createFolder(outPath_Mid + "\\bhzy");
                    while (m_FeatureClass != null)
                    {
                        if (m_FeatureClass.AliasName == "进水口" || m_FeatureClass.AliasName == "进水口线" || m_FeatureClass.AliasName == "洪水流速" || m_FeatureClass.AliasName == "等时线")
                        {
                            ExtractShpData(gp, m_FeatureClass, ztdtFolder);
                            m_FeatureClass = pEnumFeatureClass.Next();
                        }
                        else
                        {
                            if (m_FeatureClass.AliasName.Substring(0, 3) == "等时线")
                                m_FeatureClass = pEnumFeatureClass.Next();
                            else
                            {
                                ExtractShpData(gp, m_FeatureClass, bxzyFolder);
                                m_FeatureClass = pEnumFeatureClass.Next();
                            }
                        }
                        
                    }
                    dataSet = dataSets.Next();
                }
                else if (dataSet == "HydroBaseDataUnk")//定量的寻找该数据集
                {
                    try
                    {
                        //对dataset中的fetureclass的一个循环获取
                        while (m_FeatureClass != null)
                        {
                            string na = m_FeatureClass.AliasName.Substring(m_FeatureClass.AliasName.Length - 2);
                            if (na == "A3")
                            {
                                m_FeatureClass = pEnumFeatureClass.Next();
                            }
                            else
                            {
                                int f = m_FeatureClass.FindField(FiledName);//判断code字段是否存在
                                try
                                {
                                    if (f != -1)//判断是否有code字段
                                    {
                                        Export2Shp(gp, m_FeatureClass, FiledName, outPath);
                                        m_FeatureClass = pEnumFeatureClass.Next();
                                    }
                                    else
                                        m_FeatureClass = pEnumFeatureClass.Next();
                                }
                                catch(Exception ex)
                                {
                                    MessageBox.Show(ex.Message + "1号监视");
                                    m_FeatureClass = pEnumFeatureClass.Next();
                                }
                            }
                        }
                        dataSet = dataSets.Next();
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show(ex.Message + "2号监视");
                        dataSet = dataSets.Next();
                    }
                }
                else
                    dataSet = dataSets.Next();
                }
                catch
                {
                    dataSet = dataSets.Next();
                }
            }
        }
        //判断code值是否有记录
        public void Export2Shp(Geoprocessor gp, IFeatureClass featureClass, string FiledName, string outPath)
        {
            string[] code = GetUniqueValue(featureClass, FiledName);
            if(code.Length!=0)
            {
               ExtractShpData(gp, featureClass, outPath);
            }
        }
        //提取数据
        public void ExtractShpData(Geoprocessor gp, IFeatureClass featureClass, string savePath)
        {
            Msg("开始处理..." + featureClass.AliasName);
            try
            {
                ESRI.ArcGIS.DataManagementTools.CopyFeatures copyFeature = new CopyFeatures();
                copyFeature.in_features = featureClass;
                copyFeature.out_feature_class = savePath + "\\" + featureClass.AliasName;
                gp.OverwriteOutput = true;
                gp.Execute(copyFeature, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            Msg(featureClass.AliasName + "转换完成！");
            vec_count++;
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

        /*public void ConnectAttribute(string xlsPath, string udbPath)
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
                    delField.DefaultValue = "0.0";
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


        }*/
        public void ConnectAttribute(string xlsPath, string udbPath)
        {
            datetime = DateTime.Now;
            string[] m_newFieldName = { "生产总值", "耕地面积", "人口总数" };
            //定义工作空间
            SuperMap.Data.Workspace wps = new SuperMap.Data.Workspace();
            //工作空间打开数据源
            wps.Datasources.Open(new DatasourceConnectionInfo(udbPath, "yxfx", set.passWod));
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
                    delField.DefaultValue = "0.0";
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
        private void UpdateFieldValueTest(Recordset recordset, string xlsPath)
        {
            try
            {
                DataTable dt = importTool.ExcelToDataTable(xlsPath, "6.5.1综合经济指标表");

                int length = recordset.RecordCount;
                if (length != 0)
                {

                    /*需求：根据乡镇名字确定更新数据
                     1、选择成行改变：1)循环判断工作表中name列的所有记录，是否等于属性表中name列的第一行的内容   
                                     2)记录工作表中的对应的行数h
                                     3）对数据集中特定字段逐行赋值*/
                    try
                    {
                        for (int m = 1; m <= length; m++)//行循环更新
                        {
                            recordset.SeekID(m);
                            recordset.Edit();
                            object name = recordset.GetObject("T_NAME");
                            seekExcel(name.ToString(), dt, recordset, m);

                        }

                        recordset.Update();

                        Msg("修改属性字段完成");
                    }
                    catch (Exception ex)
                    {
                        Msg(ex.Message);
                    }
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
        
        private void seekExcel(string Name, DataTable dt, Recordset recordset, int m)
        {
            //Hashtable h1 = new Hashtable();//存储乡镇和GDP
            //Hashtable h2 = new Hashtable();//存储乡镇和人口
            //Hashtable h3 = new Hashtable();//存储乡镇和耕地面积
            bool next = false;
            try
            {
                //循环查找 乡镇和GDP装入h1中
                for (int seekXZ = 0; seekXZ < dt.Rows.Count; seekXZ++)
                {
                    for (int seekColumn = 0; seekColumn < dt.Columns.Count; seekColumn++)
                    {
                        string s = dt.Rows[seekXZ][seekColumn].ToString();
                        if (s == Name)
                        {
                            
                            double valueGDP = Convert.ToDouble(dt.Rows[seekXZ][seekColumn + 1]);
                            double valueAC = Convert.ToDouble(dt.Rows[seekXZ][seekColumn + 2]);
                            double valuePE = Convert.ToDouble(dt.Rows[seekXZ][seekColumn + 3]);
                            recordset.SeekID(m);
                            recordset.Edit();
                            recordset.SetDouble("生产总值", valueGDP);
                            recordset.Update();
                            recordset.SeekID(m);
                            recordset.Edit();
                            recordset.SetDouble("耕地面积", valueAC);
                            recordset.Update();
                            recordset.SeekID(m);
                            recordset.Edit();
                            recordset.SetDouble("人口总数", valuePE);
                            recordset.Update();
                            recordset.Refresh();
                            next = true;
                            break;
                        }
                        if (next)
                            break;
                    }
                    if (next)
                        break;
                }
                /*MessageBox.Show(valueGDP.ToString() + recordset.GetObject("GDP").ToString());

                            //continue;
                            for (int r = 0; r < dt.Rows.Count; r++)
                            {
                                string n = dt.Rows[r][seekColumn].ToString();
                                if (dt.Rows[r][seekColumn].ToString() == Name)
                                {
                                    for (int recircle = 0; recircle < dt.Columns.Count; recircle++)
                                    {
                                        if (dt.Columns[recircle].ColumnName == "生产总值")
                                        {
                                            recordset.SeekID(m);
                                            recordset.Edit();
                                            object valueGDP = dt.Rows[r][recircle];
                                            recordset.SetFieldValue("GDP", valueGDP);
                                            recordset.Update();
                                            MessageBox.Show(valueGDP.ToString() + recordset.GetObject("GDP").ToString());

                                        }
                                        else
                                            if (dt.Columns[recircle].ColumnName == "耕地面积")
                                            {
                                                recordset.SeekID(m);
                                                recordset.Edit();
                                                object valueAC = dt.Rows[r][recircle];
                                                recordset.SetFieldValue("耕地面积", valueAC);
                                                recordset.Update();

                                                MessageBox.Show(valueAC.ToString() + recordset.GetObject("耕地面积").ToString());

                                            }
                                            else if (dt.Columns[recircle].ColumnName == "人口总数")
                                            {
                                                recordset.SeekID(m);
                                                recordset.Edit();
                                                object valuePE = dt.Rows[r][recircle];
                                                recordset.SetFieldValue("总人口", valuePE);
                                                recordset.Update();
                                                MessageBox.Show(valuePE.ToString() + recordset.GetObject("总人口").ToString());

                                            }
                                            else
                                                continue;
                                    }
                                }
                            }

                        }*/


                //for (int i = 0; i < dt.Columns.Count; i++)
                //{
                //    if (dt.Columns[i].ColumnName == "乡镇名称")
                //    {

                //    }
                //}
                //}

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        
    }
}
