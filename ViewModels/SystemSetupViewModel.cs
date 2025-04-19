using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VPDLFramework.Models;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.ComponentModel;
using GalaSoft.MvvmLight.Messaging;
using Cognex.VisionPro.Comm;
using System.Windows;
using Microsoft.Win32;
using System.Windows.Forms;
using System.Windows.Markup;
using System.Xml.Linq;
using System.Xml;
using System.Data.DwayneNeed.Win32;

namespace VPDLFramework.ViewModels
{
    public class SystemSetupViewModel : ViewModelBase
    {
        /// <summary>
        /// 工作启动设置界面视图模型
        /// </summary>
        public SystemSetupViewModel()
        {
            CmdExportLanguageFile =new RelayCommand(ExportLanguageFile);
            CmdImportLanguageFile = new RelayCommand(ImportLanguageFile);
            CmdSave = new RelayCommand(SaveStartupConfig);
            Messenger.Default.Register<string>(this, ECMessengerManager.SystemSetupWindowMessengerKeys.Show, OnSystemSteupWindowShow);
        }

        private void OnSystemSteupWindowShow(string str)
        {
            UpdateStartupSettings();
            CameraOrderViewModel = new EditCameraOrderViewModel();
        }

        #region 命令

        /// <summary>
        /// 命令：保存启动配置
        /// </summary>
        public RelayCommand CmdSave { get; set; }

        /// <summary>
        /// 命令：导出语言文件
        /// </summary>
        public RelayCommand CmdExportLanguageFile { get; set; }

        /// <summary>
        /// 命令：导入语言文件
        /// </summary>
        public RelayCommand CmdImportLanguageFile { get; set; }

        #endregion 命令

        #region 方法

        /// <summary>
        /// 合并读取的配置信息和当前所有的工作,保存的配置工作可能已经被删除或者添加了新的工作
        /// </summary>
        private void UpdateStartupSettings()
        {
            LoadStartupConfig();
            List<string> workNames = GetWorkNamesList();

            BindingList<ECWorkStartupInfo> tempList = new BindingList<ECWorkStartupInfo>();
            // 删除不存在的工作信息
            foreach (var work in StartupSettings.WorksStartupInfo)
            {
                if (workNames.Contains(work.WorkName))
                {
                    tempList.Add(work);
                }
            }

            // 检查是否有新的工作,若存在则添加信息
            Dictionary<string, ECWorkStartupInfo> worksInfo = tempList.ToDictionary(s => s.WorkName, s => s);

            foreach (string workName in workNames)
            {
                if (!worksInfo.ContainsKey(workName))
                    tempList.Add(new ECWorkStartupInfo(workName, "", false));
            }
            StartupSettings.WorksStartupInfo.Clear();
            StartupSettings.WorksStartupInfo= tempList;
        }

        // <summary>
        // 加载启动配置
        // </summary>
        public void LoadStartupConfig()
        {
            StartupSettings = ECStartupSettings.Instance();
            if (ECCommCard.Bank0!=null&&ECCommCard.Bank0.FfpAccess!=null)
            {
                StartupSettings.FfpType=ECCommCard.Bank0.FfpAccess.GetActiveNetworkDataModel()?.GetType().Name.Replace("CogNdm","");
            }
        }

        // <summary>
        // 保存启动配置
        // </summary>
        public void SaveStartupConfig()
        {
            StartupSettings.Save();
            if (CameraOrderViewModel.CamerasInfo.Count > 0)
                CameraOrderViewModel.WriteIniFile();
            ECGeneric.CheckLanguage(StartupSettings.SelectedLanguage);
            SetCommCardFfp();
            ECDialogManager.ShowMsg(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.SavingTipOfSystemSetup));

        }

        /// <summary>
        /// 设置通讯板卡Ffp
        /// </summary>
        private void SetCommCardFfp()
        {
            try
            {
                if(ECCommCard.Bank0!=null)
                {
                    if(StartupSettings.FfpType!=null&& StartupSettings.FfpType.Insert(0,"CogNdm") != ECCommCard.Bank0.FfpAccess.GetActiveNetworkDataModel()?.GetType().Name)
                    {
                        CogFfpProtocolConstants type;
                        Enum.TryParse(StartupSettings.FfpType, out type);
                        ECCommCard.SetFfpType(type);
                    }
                        
                }
            }
            catch(Exception ex) 
            {
                ECLog.WriteToLog(ex.StackTrace+ex.Message, NLog.LogLevel.Error);
            }
        }

        // <summary>
        // 检查工作列表
        // </summary>
        // <param name = "loadedWorkInfo" ></ param >
        public List<string> GetWorkNamesList()
        {
            string[] worksPath = Directory.GetDirectories(ECFileConstantsManager.RootFolder);
            List<string> list = new List<string>(); 
            foreach (string workPath in worksPath)
            {
                list.Add(Path.GetFileName(workPath));
            }
            return list;
            
        }

        /// <summary>
        /// 导出语言文件
        /// </summary>
        private void ExportLanguageFile()
        {
            try
            {
                string filePath = ECFileConstantsManager.LanguagesFolder + @"\SimplifiedChinese.xaml";
                if (File.Exists(filePath))
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(filePath);

                    //添加命名空间，这一步一定要有，否则读取不了
                    XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(xmlDoc.NameTable);
                    xmlNamespaceManager.AddNamespace("x", "http://schemas.micorsoft.com/winfx/2006/xaml");
                    xmlNamespaceManager.AddNamespace("sys", "clr-namespace:System;assembly=mscorlib");

                    string dstPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\SimplifiedChinese.txt";
                    StreamWriter sw = new StreamWriter(dstPath, true);
                    foreach (XmlNode item in xmlDoc.DocumentElement.ChildNodes)
                    {
                        if (item.NodeType == XmlNodeType.Element)
                        {
                            sw.WriteLine(item.InnerText);
                            sw.Flush();
                        }
                    }
                    sw.Close();
                    ECDialogManager.ShowMsg(dstPath);
                }
                else
                    ECDialogManager.ShowMsg(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.NoValidFile));
            }
            catch (Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace+ex.Message,NLog.LogLevel.Error);
            }
        }

        /// <summary>
        /// 导入语言文件
        /// </summary>
        private void ImportLanguageFile()
        {
            try
            {
                System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
                openFileDialog.Filter = "txt file (*.txt)|*.txt";
                openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                openFileDialog.Multiselect = false;
                // 选择文件
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;
                    string fileName = Path.GetFileNameWithoutExtension(filePath);
                    string chineseXamlPath = ECFileConstantsManager.LanguagesFolder + @"\SimplifiedChinese.xaml";
                    string newXamlPath = ECFileConstantsManager.LanguagesFolder + @"\" + $"{fileName}.xaml";

                    // 判断文件名称不能是简体中文的文件名
                    if (chineseXamlPath == newXamlPath)
                    {
                        ECDialogManager.ShowMsg(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.InvalidName));
                        return;
                    }

                    // 检查词条数量
                    List<string> wordList = new List<string>();
                    StreamReader sr = new StreamReader(filePath);
                    int lineCount = 0;
                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();
                        if (!string.IsNullOrEmpty(line))
                            wordList.Add(line);
                        lineCount++;
                    }

                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(chineseXamlPath);

                    //添加命名空间，这一步一定要有，否则读取不了
                    XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(xmlDoc.NameTable);
                    xmlNamespaceManager.AddNamespace("x", "http://schemas.micorsoft.com/winfx/2006/xaml");
                    xmlNamespaceManager.AddNamespace("sys", "clr-namespace:System;assembly=mscorlib");

                    int wordsCount = 0;
                    foreach (XmlNode item in xmlDoc.DocumentElement.ChildNodes)
                    {
                        if (item.NodeType == XmlNodeType.Element)
                            wordsCount++;
                    }

                    if (lineCount == wordsCount)
                    {
                        File.Copy(chineseXamlPath, newXamlPath, true);
                        xmlDoc = new XmlDocument();
                        xmlDoc.Load(chineseXamlPath);

                        //添加命名空间，这一步一定要有，否则读取不了
                        xmlNamespaceManager = new XmlNamespaceManager(xmlDoc.NameTable);
                        xmlNamespaceManager.AddNamespace("x", "http://schemas.micorsoft.com/winfx/2006/xaml");
                        xmlNamespaceManager.AddNamespace("sys", "clr-namespace:System;assembly=mscorlib");
                        int i = 0;
                        foreach (XmlNode item in xmlDoc.DocumentElement.ChildNodes)
                        {
                            if (item.NodeType == XmlNodeType.Element)
                            {
                                item.InnerText = wordList[i];
                                i++;
                            }
                        }
                        xmlDoc.Save(newXamlPath);
                        ECDialogManager.ShowMsg(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.ImportFinished));
                    }
                    else
                        ECDialogManager.ShowMsg(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.CheckFailed));
                }
            }
            catch(Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, NLog.LogLevel.Error);
            }
        }

        #endregion 方法

        #region 属性

        /// <summary>
        /// 启动设置
        /// </summary>
        private ECStartupSettings _startupSettings;

        public ECStartupSettings StartupSettings
        {
            get { return _startupSettings; }
            set
            {
                _startupSettings = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 相机顺序视图模型
        /// </summary>
        private EditCameraOrderViewModel _cameraOrderViewModel;

        public EditCameraOrderViewModel CameraOrderViewModel
        {
            get { return _cameraOrderViewModel; }
            set { _cameraOrderViewModel = value;
                RaisePropertyChanged();
            }
        }


        #endregion 属性

    }
}
