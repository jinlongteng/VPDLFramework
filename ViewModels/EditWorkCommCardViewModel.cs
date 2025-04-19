using GalaSoft.MvvmLight;
using System;
using System.ComponentModel;
using System.IO;
using VPDLFramework.Models;
using NLog;
using static VPDLFramework.Models.ECWorkOptionManager;
using Cognex.VisionPro.Comm;
using GalaSoft.MvvmLight.Command;
using VPDLFramework.Views;
using GalaSoft.MvvmLight.Messaging;

namespace VPDLFramework.ViewModels
{
    public class EditWorkCommCardViewModel:ViewModelBase
    {
        public EditWorkCommCardViewModel(string workName) 
        {
            _workName = workName;
            InputSignalTypeConstantsBindableList = ECGeneric.GetConstantsBindableList<IOInputSignalTypeConstants>();
            OutputSignalTypeConstantsBindableList=ECGeneric.GetConstantsBindableList<IOOutputSignalTypeConstants>();
            BindCmd();
            IsThirdCard= CheckThirdCardEnable();
            SetFilePath();
        }

        #region 命令
        /// <summary>
        /// 命令：编辑工厂协议脚本
        /// </summary>
        public RelayCommand CmdEditFfpScript { get; set; }

        /// <summary>
        /// 命令：重置脚本文件
        /// </summary>
        public RelayCommand CmdResetScriptFile { get; set; }

        /// <summary>
        /// 命令：测试CC24
        /// </summary>
        public RelayCommand CmdTestCC24 { get; set; }

        #endregion

        #region 方法
        /// <summary>
        /// 绑定命令
        /// </summary>
        private void BindCmd()
        {
            CmdEditFfpScript = new RelayCommand(EditFfpScript);
            CmdResetScriptFile=new RelayCommand(ResetScriptFile);
            CmdTestCC24 = new RelayCommand(TestCC24);
        }

        /// <summary>
        /// 检测是否使用第三方板卡
        /// </summary>
        /// <returns></returns>
        private bool CheckThirdCardEnable()
        {
            ECStartupSettings _startupSettings = ECStartupSettings.Instance();
            if (_startupSettings != null)
            {
                if (_startupSettings.EnableThirdCard) return true;
            }
            return false;
        }

        /// <summary>
        /// 设置文件路径
        /// </summary>
        private void SetFilePath()
        {
            if (_isThirdCard)
            {
                _scriptPath = ECFileConstantsManager.RootFolder + @"\" + _workName + @"\" + ECFileConstantsManager.CommCardConfigFolderName + @"\" + ECFileConstantsManager.ThirdCardScriptName;
                _stdScriptPath = ECFileConstantsManager.StdFilesFolder + @"\" + ECFileConstantsManager.Std_ThirdCardScriptName;
                _assemblyConfigPath= ECFileConstantsManager.RootFolder + @"\" + _workName + @"\" + ECFileConstantsManager.CommCardConfigFolderName + @"\" + ECFileConstantsManager.ThirdCardScriptConfigName;
            }
            else
            {
                _scriptPath = ECFileConstantsManager.RootFolder + @"\" + _workName + @"\" + ECFileConstantsManager.CommCardConfigFolderName + @"\" + ECFileConstantsManager.FfpScriptName;
                _stdScriptPath = ECFileConstantsManager.StdFilesFolder + @"\" + ECFileConstantsManager.Std_FfpScriptName;
                _assemblyConfigPath = ECFileConstantsManager.RootFolder + @"\" + _workName + @"\" + ECFileConstantsManager.CommCardConfigFolderName + @"\" + ECFileConstantsManager.FfpScriptConfigName;
            }
        }


        /// <summary>
        /// 编辑工厂协议脚本
        /// </summary>
        private void EditFfpScript()
        {
            CheckScriptFile();
            Window_EditScript window_EditScript = new Window_EditScript(_workName,_isThirdCard,_scriptPath,_assemblyConfigPath);
            window_EditScript.ShowDialog();
        }

        /// <summary>
        /// 检查脚本文件是否存在
        /// </summary>
        private void CheckScriptFile()
        {
            if (!File.Exists(_scriptPath))
                ResetScriptFile();
        }

        /// <summary>
        /// 测试CC24
        /// </summary>
        private void TestCC24()
        {
            if(ECCommCard.Bank0 == null)
            {
                ECDialogManager.ShowMsg(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.NoCC24CommCard));
                return;
            }
            ECCommCard.GetScriptMethod(_workName);
            ECCommCard.ConfigureDefaultEvents();
            Messenger.Default.Send<string>(_workName, ECMessengerManager.CommCardViewModelMessengerKeys.CommTest);
        }

        /// <summary>
        /// 重置脚本文件
        /// </summary>
        private void ResetScriptFile()
        {
            try
            {
                // 拷贝标准脚本文件
                if (File.Exists(_stdScriptPath))
                    File.Copy(_stdScriptPath, _scriptPath, true);
                // 删除引用配置
                if(File.Exists(_assemblyConfigPath))
                    File.Delete(_assemblyConfigPath);
            }
            catch(Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace+ex.Message, LogLevel.Error);
            }
        }

        /// <summary>
        /// 初始化IO配置
        /// </summary>
        public void CreatNewConfig()
        {
            InputItems = new BindingList<ECCommCardInputLineInfo>();
            OutputItems = new BindingList<ECCommCardOutputLineInfo>();
            for (int i = 0; i < 8; i++)
            {
                InputItems.Add(new ECCommCardInputLineInfo()
                {
                    LineNumber = i,
                    SignalType = Enum.GetName(typeof(ECWorkOptionManager.IOInputSignalTypeConstants), ECWorkOptionManager.IOInputSignalTypeConstants.Any)
                });
            }

            for (int i = 0; i < 16; i++)
            {
                OutputItems.Add(new ECCommCardOutputLineInfo()
                {
                    LineNumber = i,
                    SignalType = Enum.GetName(typeof(ECWorkOptionManager.IOOutputSignalTypeConstants), ECWorkOptionManager.IOOutputSignalTypeConstants.High),
                    Duration = 100
                });
            }
            ResetScriptFile();
            SaveConfig();
        }

        /// <summary>
        /// 从文件获取整体字符串
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string GetStringFromFile(string path)
        {
            if(!File.Exists(path)) return null;
            string script = "";

            StreamReader streamReader = new StreamReader(path);
            script= streamReader.ReadToEnd();
            streamReader.Close();
            return script;
        }

        /// <summary>
        /// 加载已有的配置
        /// </summary>
        public void GetExistConfig()
        {
            try
            {
                string folder = $"{ECFileConstantsManager.RootFolder}\\{_workName}\\{ECFileConstantsManager.CommCardConfigFolderName}";
                string inputConfigPath = folder + @"\" + ECFileConstantsManager.IOInputConfigName;
                string outputConfigPath = folder + @"\" + ECFileConstantsManager.IOOutputConfigName;

                if(!File.Exists(inputConfigPath)||!File.Exists(outputConfigPath)) CreatNewConfig();

                InputItems = new BindingList<ECCommCardInputLineInfo>();
                OutputItems = new BindingList<ECCommCardOutputLineInfo>();


                if (File.Exists(inputConfigPath))
                    InputItems = ECSerializer.LoadObjectFromJson<BindingList<ECCommCardInputLineInfo>>(inputConfigPath);
                if (File.Exists(outputConfigPath))
                    OutputItems = ECSerializer.LoadObjectFromJson<BindingList<ECCommCardOutputLineInfo>>(outputConfigPath);

                if (ECCommCard.Bank0 != null && ECCommCard.Bank0.FfpAccess != null)
                {
                    FfpType = ECCommCard.Bank0.FfpAccess.GetActiveNetworkDataModel()?.GetType().Name.Replace("CogNdm", "");
                }
            }
            catch (Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, LogLevel.Error);
            }

        }

        /// <summary>
        /// 保存配置
        /// </summary>
        public bool SaveConfig()
        {
            try
            {
                string folder = $"{ECFileConstantsManager.RootFolder}\\{_workName}\\{ECFileConstantsManager.CommCardConfigFolderName}";
                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
                string inputConfigPath = folder + @"\" + ECFileConstantsManager.IOInputConfigName;
                string outputConfigPath = folder + @"\" + ECFileConstantsManager.IOOutputConfigName;

                ECSerializer.SaveObjectToJson(inputConfigPath, InputItems);
                ECSerializer.SaveObjectToJson(outputConfigPath, OutputItems);

                return true;
            }
            catch (Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, LogLevel.Error);
                return false;
            }
        }
        #endregion

        #region 字段
        /// <summary>
        /// 工作名
        /// </summary>
        private string _workName;

        /// <summary>
        /// 脚本文件路径
        /// </summary>
        private string _scriptPath;

        /// <summary>
        /// 标准脚本文件路径
        /// </summary>
        private string _stdScriptPath;

        /// <summary>
        /// 引用配置文件路径
        /// </summary>
        private string _assemblyConfigPath;

        #endregion

        #region 属性

        /// <summary>
        /// 输入
        /// </summary>
        private BindingList<ECCommCardInputLineInfo> _inputItems;

        public BindingList<ECCommCardInputLineInfo> InputItems
        {
            get { return _inputItems; }
            set
            {
                _inputItems = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 输出
        /// </summary>
        private BindingList<ECCommCardOutputLineInfo> _outputItems;

        public BindingList<ECCommCardOutputLineInfo> OutputItems
        {
            get { return _outputItems; }
            set
            {
                _outputItems = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 可绑定的IO信号类型选项列表
        /// </summary>
        private BindingList<string> _InputSignalTypeConstantsBindableList;

        public BindingList<string> InputSignalTypeConstantsBindableList
        {
            get
            { return _InputSignalTypeConstantsBindableList; }
            set
            {
                _InputSignalTypeConstantsBindableList = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 可绑定的IO信号类型选项列表
        /// </summary>
        private BindingList<string> _OutputSignalTypeConstantsBindableList;

        public BindingList<string> OutputSignalTypeConstantsBindableList
        {
            get
            { return _OutputSignalTypeConstantsBindableList; }
            set
            {
                _OutputSignalTypeConstantsBindableList = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 工厂协议类型
        /// </summary>
        private string _FfpType;

        public string FfpType
        {
            get { return _FfpType; }
            set { _FfpType = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 启用第三方板卡
        /// </summary>
        private bool _isThirdCard;

        public bool IsThirdCard
        {
            get { return _isThirdCard; }
            set { _isThirdCard = value;
                RaisePropertyChanged();
            }
        }

        #endregion
    }
}
