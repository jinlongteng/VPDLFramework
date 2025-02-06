using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using VPDLFramework.Models;
using NLog;
using System.ComponentModel;
using GalaSoft.MvvmLight.Messaging;
using Svg;
using System.Data;
using System.Windows.Shapes;
using System.IO.Compression;

namespace VPDLFramework.ViewModels
{
    public class FileManagerViewModel:ViewModelBase
    {
        /// <summary>
        /// 文件管理界面视图模型
        /// </summary>
        public FileManagerViewModel()
        {
            BindCmd();
            Messenger.Default.Register<string>(this, ECMessengerManager.FileManagerWindowMessengerKeys.Show,OnFileManagerWindowShow);
        }

        #region 命令
        /// <summary>
        /// 命令：导出数据库
        /// </summary>
        public RelayCommand<string> CmdExportDatabase { get; set; }

        /// <summary>
        /// 命令：清空数据库
        /// </summary>
        public RelayCommand<string> CmdClearDatabase { get; set; }

        /// <summary>
        /// 命令：导出图片
        /// </summary>
        public RelayCommand<string> CmdExportImages { get; set; }

        /// <summary>
        /// 命令：清空图片
        /// </summary>
        public RelayCommand<string> CmdClearImages { get; set; }

        /// <summary>
        /// 批量导出工作日志到CSV
        /// </summary>
        public RelayCommand<string> CmdWorkLogBatchExportToCSV { get; set; }

        /// <summary>
        /// 生产数据批量导出到CSV
        /// </summary>
        public RelayCommand<string> CmdProductDataBatchExportToCSV { get; set; }

        #endregion 命令

        #region 方法

        /// <summary>
        /// 绑定命令
        /// </summary>
        private void BindCmd()
        {
            CmdExportDatabase = new RelayCommand<string>(ExportDatabase);
            CmdClearDatabase = new RelayCommand<string>(ClearDatabase);
            CmdExportImages = new RelayCommand<string>(ExportImages);
            CmdClearImages = new RelayCommand<string>(ClearImages);
            CmdWorkLogBatchExportToCSV=new RelayCommand<string>(WorkLogBatchExportToCSV);
            CmdProductDataBatchExportToCSV=new RelayCommand<string>(ProductDataBatchExportToCSV);
        }

        /// <summary>
        /// 文件管理窗口显示消息达到
        /// </summary>
        /// <param name="str"></param>
        private void OnFileManagerWindowShow(string str)
        {
            GetWorksFileInfo();
        }

        /// <summary>
        /// 获取所有的工作文件信息
        /// </summary>
        private void GetWorksFileInfo()
        {
            try
            {
                WorksFileList=new BindingList<ECWorkFileInfo>();

                string[] worksDirectory= Directory.GetDirectories(ECFileConstantsManager.RootFolder);

                foreach (string workPath in worksDirectory)
                {
                    if (File.Exists(workPath + @"\" + ECFileConstantsManager.WorkInfoFileName))
                    {
                        ECWorkFileInfo wi = new ECWorkFileInfo(System.IO.Path.GetFileName(workPath));
                        string streamsDirectory = workPath + @"\" + ECFileConstantsManager.StreamsFolderName;
                        string[] streamsPath = Directory.GetDirectories(streamsDirectory);
                        foreach (string streamPath in streamsPath)
                        {
                            string imgPath = ECFileConstantsManager.ImageRootFolder + @"\" + wi.WorkName + @"\" + ECFileConstantsManager.ImageRecordFolderName + @"\" + System.IO.Path.GetFileName(streamPath);
                            string dbPath = workPath + @"\" + ECFileConstantsManager.DatabaseFolderName + @"\" + ECFileConstantsManager.WorkStreamsDataFolderName + @"\" + System.IO.Path.GetFileName(streamPath);
                            int imgSize = 0, dbSize = 0;
                            if (Directory.Exists(imgPath))
                                imgSize = (int)GetDirectorySize(imgPath) / 1024;
                            if (Directory.Exists(dbPath))
                                dbSize = (int)(GetDirectorySize(dbPath)) / 1024;
                            wi.StreamsFileInfo.Add(new ECStreamFileInfo(System.IO.Path.GetFileName(streamPath), ConvertKBToString(imgSize), ConvertKBToString(dbSize)));
                        }
                        WorksFileList.Add(wi);
                    }
                }

            }
            catch (Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message,LogLevel.Error);
            }
        }

        /// <summary>
        /// 将kb数值转换为字符串,大于1024则显示为MB为单位
        /// </summary>
        /// <param name="kbSize"></param>
        /// <returns></returns>
        private string ConvertKBToString(int kbSize)
        {
            if (kbSize > 1024)
                return Math.Round(((double)kbSize / 1024), 2).ToString() + " MB";
            else
                return kbSize.ToString() + " KB";
        }

        /// <summary>
        /// 获取文件夹大小
        /// </summary>
        /// <param name="dirPath"></param>
        /// <returns></returns>
        private long GetDirectorySize(string dirPath)
        {
            if (!System.IO.Directory.Exists(dirPath))
                return 0;
            long len = 0;
            DirectoryInfo di = new DirectoryInfo(dirPath);
            //获取di目录中所有文件的大小
            foreach (FileInfo item in di.GetFiles())
            {
                len += item.Length;
            }

            //获取di目录中所有的文件夹,并保存到一个数组中,以进行递归
            DirectoryInfo[] dis = di.GetDirectories();
            if (dis.Length > 0)
            {
                for (int i = 0; i < dis.Length; i++)
                {
                    len += GetDirectorySize(dis[i].FullName);//递归dis.Length个文件夹,得到每隔dis[i]
                }
            }
            return len;
        }

        /// <summary>
        /// 导出工作流图片
        /// </summary>
        /// <param name="workAndStreamIndex">工作项目名称和工作流名称</param>
        private void ExportImages(string workAndStreamIndex)
        {
            try
            {
                string[] names = workAndStreamIndex.Split(':');
                string workName = names[0];
                string streamName = GetStreamName(workName, Convert.ToInt16(names[1]));
                string imgPath = ECFileConstantsManager.ImageRootFolder + @"\" + workName + @"\" + ECFileConstantsManager.ImageRecordFolderName + @"\" + streamName;
                string dstPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\" +"Images"+ DateTime.Now.ToString("yyyyMMddHHmmss") + workName + @"_" + streamName + ".zip";
                if (Directory.Exists(imgPath))
                {
                    ECDialogManager.LoadWithAnimation(new Action(() =>
                    {
                        ZipFile.CreateFromDirectory(imgPath, dstPath);                 
                    }), ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.Exporting));
                    ECDialogManager.ShowMsg($"{ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.ExportFinished)}: " + dstPath);
                }
                else
                    ECDialogManager.ShowMsg(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.NoValidFile));
            }
            catch(Exception ex)
            {
                ECDialogManager.ShowMsg(ex.Message+ ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.ExportFailed));
            }
        }

        /// <summary>
        /// 清空工作流图片
        /// </summary>
        /// <param name="workAndStreamIndex">工作项目名称和工作流名称</param>
        private void ClearImages(string workAndStreamIndex)
        {
            try
            {
                string[] names = workAndStreamIndex.Split(':');
                string workName = names[0];
                string streamName = GetStreamName(workName, Convert.ToInt16(names[1]));
                string imgPath = ECFileConstantsManager.ImageRootFolder+ @"\" + workName + @"\" + ECFileConstantsManager.ImageRecordFolderName + @"\" + streamName;
                if (Directory.Exists(imgPath))
                {
                    ECDialogManager.LoadWithAnimation(new Action(() =>
                    {
                        Directory.Delete(imgPath, true);
                        ECDialogManager.ShowMsg(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.ClearFinished));
                        GetWorksFileInfo();
                    }),ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.Clearing));
                }
            }
            catch
            {
                ECDialogManager.ShowMsg(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.ClearFailed));
            }
        }

        /// <summary>
        /// 导出工作流数据库到CSV
        /// </summary>
        /// <param name="workAndStreamIndex">工作项目名称和工作流名称</param>
        private void ExportDatabase(string workAndStreamIndex)
        {
            try
            {
                string[] names = workAndStreamIndex.Split(':');
                string workName = names[0];
                string streamName = GetStreamName(workName, Convert.ToInt16(names[1]));
                string dbPath = ECFileConstantsManager.RootFolder + @"\" + workName + @"\" + ECFileConstantsManager.DatabaseFolderName +
                    @"\" + ECFileConstantsManager.WorkStreamsDataFolderName + @"\" + streamName + @"\";
                string dstPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\" +"Databases"+ DateTime.Now.ToString("yyyyMMddHHmmss") + workName + @"_" + streamName + ".zip";
                if (Directory.Exists(dbPath))
                {
                    ECDialogManager.LoadWithAnimation(new Action(() =>
                    {
                       ZipFile.CreateFromDirectory(dbPath, dstPath);
                    }), ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.Exporting));
                    ECDialogManager.ShowMsg($"{ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.ExportFinished)}: " + dstPath);
                }
                else
                    ECDialogManager.ShowMsg(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.NoValidFile));
            }
            catch(Exception ex)
            {
                ECDialogManager.ShowMsg(ex.StackTrace+ex.Message+ ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.ExportFailed));
            }
        }

        /// <summary>
        /// 清空工作流数据库
        /// </summary>
        /// <param name="workAndStreamIndex">工作项目名称和工作流名称</param>
        private void ClearDatabase(string workAndStreamIndex)
        {
            try
            {
                string[] names = workAndStreamIndex.Split(':');
                string workName = names[0];
                string streamName = GetStreamName(workName, Convert.ToInt16(names[1]));
                string dbPath = ECFileConstantsManager.RootFolder + @"\" + workName + @"\"+ECFileConstantsManager.DatabaseFolderName+
                    @"\"+ECFileConstantsManager.WorkStreamsDataFolderName + @"\" + streamName + @"\";

                if (File.Exists(dbPath))
                {
                    Directory.Delete(dbPath, true);
                    ECDialogManager.ShowMsg(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.ClearFinished));
                }
            }
            catch(Exception ex)
            {
                ECDialogManager.ShowMsg(ex.StackTrace+ex.Message+ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.ClearFailed));
            }
        }

        /// <summary>
        /// 通过工作项目名称和工作流索引获取工作流名称
        /// </summary>
        /// <param name="workName">工作项目名称</param>
        /// <param name="streamIndex">工作流索引</param>
        /// <returns></returns>
        private string GetStreamName(string workName, int streamIndex)
        {
            string name = "";
            try
            {
                foreach (ECWorkFileInfo wi in WorksFileList)
                {
                    if (wi.WorkName == workName)
                        name = wi.StreamsFileInfo[streamIndex].StreamName;
                }
            }
            catch (Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message,LogLevel.Error);
            }
            return name;
        }

        /// <summary>
        /// 工作日志批量导出到CSV
        /// </summary>
        /// <param name="workName"></param>
        private void WorkLogBatchExportToCSV(string workName)
        {
            try
            {
                if (string.IsNullOrEmpty(workName)) return;
                string srcLogFolder = ECFileConstantsManager.RootFolder + @"\" + workName + @"\" + ECFileConstantsManager.DatabaseFolderName +
                    @"\" + ECFileConstantsManager.WorkLogFolderName;
                string dstLogFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\" + DateTime.Now.ToString("yyyyMMdd_HHmmss")+"_" + workName + @"_WorkLog";
                if (Directory.Exists(srcLogFolder))
                {
                    ECDialogManager.LoadWithAnimation(new Action(() =>
                    {
                        Directory.CreateDirectory(dstLogFolder);
                        string[] folders = Directory.GetDirectories(srcLogFolder);
                        foreach (string folder in folders)
                        {
                            string dstFileFolder = dstLogFolder + @"\" + System.IO.Path.GetFileName(folder);
                            Directory.CreateDirectory(dstFileFolder);
                            string[] files = Directory.GetFiles(folder);
                            if (files.Length > 0)
                            {
                                foreach (string file in files)
                                {
                                    string csvPath = dstFileFolder + @"\" + System.IO.Path.GetFileNameWithoutExtension(file) + ".csv";
                                    DataTable table = ECSQLiteDataManager.QueryAll(file).Table;
                                    ECDatabaseHelper.WriteCSV(csvPath, table);
                                }
                            }
                        }
                    }), ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.Exporting));
                    ECDialogManager.ShowMsg($"{ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.ExportFinished)}: " + dstLogFolder);
                }
                else
                    ECDialogManager.ShowMsg(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.NoValidFile));
            }
            catch (Exception ex)
            {
                ECDialogManager.ShowMsg(ex.StackTrace + ex.Message + ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.ExportFailed));
            }
        }

        /// <summary>
        /// 生产数据批量导出到CSV
        /// </summary>
        /// <param name="workName"></param>
        private void ProductDataBatchExportToCSV(string workName)
        {
            try
            {
                if (string.IsNullOrEmpty(workName)) return;
                string srcDataFolder = ECFileConstantsManager.RootFolder + @"\" + workName + @"\" + ECFileConstantsManager.DatabaseFolderName +
                    @"\" + ECFileConstantsManager.WorkStreamsDataFolderName;
                string dstDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + "_" + workName + @"_ProductData";
                if (Directory.Exists(srcDataFolder))
                {
                    ECDialogManager.LoadWithAnimation(new Action(() =>
                    {
                        Directory.CreateDirectory(dstDataFolder);
                        string[] streamFolders = Directory.GetDirectories(srcDataFolder);
                        foreach (string streamFolder in streamFolders)
                        {
                            string[] fileFolders = Directory.GetDirectories(streamFolder);
                            {
                                foreach (string fileFolder in fileFolders)
                                {
                                    string dstFileFolder = dstDataFolder + @"\" + System.IO.Path.GetFileName(streamFolder)+@"\"+ System.IO.Path.GetFileName(fileFolder);
                                    Directory.CreateDirectory(dstFileFolder);
                                    string[] files = Directory.GetFiles(fileFolder);
                                    if (files.Length > 0)
                                    {
                                        foreach (string file in files)
                                        {
                                            string csvPath = dstFileFolder + @"\" + System.IO.Path.GetFileNameWithoutExtension(file) + ".csv";
                                            DataTable table = ECSQLiteDataManager.QueryAll(file).Table;
                                            ECDatabaseHelper.WriteCSV(csvPath, table);
                                        }
                                    }
                                }
                            }
                        }
                    }), ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.Exporting));
                    ECDialogManager.ShowMsg($"{ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.ExportFinished)}: " + dstDataFolder);
                }
                else
                    ECDialogManager.ShowMsg(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.NoValidFile));
            }
            catch (Exception ex)
            {
                ECDialogManager.ShowMsg(ex.StackTrace + ex.Message + ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.ExportFailed));
            }
        }

        #endregion 方法

        #region 属性

        /// <summary>
        /// 工作项目文件信息字典
        /// </summary>
        private BindingList<ECWorkFileInfo> worksFileList;

        public BindingList<ECWorkFileInfo> WorksFileList
        {
            get { return worksFileList; }
            set
            {
                worksFileList = value;
                RaisePropertyChanged();
            }
        }

        #endregion 属性
    }
}
