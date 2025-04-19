using GalaSoft.MvvmLight;
using GalaCmd=GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ViDi2;
using VPDLFramework.Models;
using VPDLFramework.Views;
using System.Windows.Controls;
using System.ComponentModel;
using GalaSoft.MvvmLight.Threading;
using System.Text.RegularExpressions;
using ViDi2.UI;
using Cognex.VisionPro;
using System.Linq;
using static VPDLFramework.Models.ECNativeModeCommand;
using Cognex.VisionPro.Comm;
using Cognex.Vision;
using System.IO.Compression;
using ThirdParty.Json.LitJson;

namespace VPDLFramework.ViewModels
{

    public class MainViewModel : ViewModelBase
    {

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            // 注册消息
            RegisterMessenger();

            //绑定命令
            BindCmd();

            // 异步初始化环境
            Task.Factory.StartNew(() =>
                {
                    if (!InitialSystem())
                        Messenger.Default.Send<string>("", ECMessengerManager.MainViewModelMessengerKeys.InitialFailed);
                    else
                    {
                        Messenger.Default.Send<string>("", ECMessengerManager.MainViewModelMessengerKeys.InitialCompeleted);
                        ECLog.WriteToLog(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.ProgramStartFinished), LogLevel.Trace);
                    }
                });

        }

        #region 方法

        #region 系统
        /// <summary>
        /// 初始化系统环境
        /// </summary>
        /// <returns></returns>
        public bool InitialSystem()
        {
            try
            {
                GetStartupInfo();

                ECLog.WriteToLog(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.SystemStartupSetupLoaded), LogLevel.Trace);

                // 检查软件授权文件
                //if (!CheckSoftwareLicense())
                //    return false;


                // 检测Default目录
                CheckRootFolder();

                Messenger.Default.Send<string>($"{ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.LoadingVisionModule)}......", ECMessengerManager.MainViewModelMessengerKeys.InitialStepChanged);
                // if (!CheckVisionProLicense()) return false;

                // 初始化ViDi环境
                if (ECDLEnvironment.IsEnable)
                    if (!InitialDLEnvironment()) return false;

                ECLog.WriteToLog(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.VisionModuleLoaded), LogLevel.Trace);

                // 启动Windows系统监控
                Messenger.Default.Send<string>($"{ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.LoadingWindowsMonitor)}......", ECMessengerManager.MainViewModelMessengerKeys.InitialStepChanged);

                WindowsSystemInfo = new ECWMI();
                ECLog.WriteToLog(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.WindowsMonitorLoaded), LogLevel.Trace);

                // 设置Default配置
                SetSystemDefaultConfig();

                // 启动UI定时器
                StartUITimer();

                // 刷新工作列表
                RefreshWorkList();

                _isSystemStartComplete = true;

                // 检查是否需要联机启动Default作业
                CheckStartupOnline();

                return true;
            }
            catch (System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, LogLevel.Error);
                return false;
            }
        }

        /// <summary>
        /// 绑定命令
        /// </summary>
        private void BindCmd()
        {
            // 工作编辑

            CmdLoadContentView = new GalaCmd.RelayCommand<string>(LoadContentView);
            CmdCreateNewWork = new GalaCmd.RelayCommand(CreateNewWork);
            CmdDeleteWork = new GalaCmd.RelayCommand<object>(DeleteWork);
            CmdSelectWork = new GalaCmd.RelayCommand<object>(SelectWork);
            CmdRefreshWorkList = new GalaCmd.RelayCommand(RefreshWorkList);
            CmdCopyWork = new GalaCmd.RelayCommand<object>(CopyWork);
            CmdImportWork = new GalaCmd.RelayCommand(ImportWork);
            CmdExportWork = new GalaCmd.RelayCommand<object>(ExportWork);

            // 顶部功能菜单按钮命令
            CmdOpenSystemSetup = new GalaCmd.RelayCommand(OpenStartupSetup);
            CmdOpenFileManager = new GalaCmd.RelayCommand(OpenFileManager);
            CmdOpenVersionInfo = new GalaCmd.RelayCommand(OpenVersionInfo);
            CmdOpenHelp = new GalaCmd.RelayCommand(OpenHelp);
            CmdLogin = new GalaCmd.RelayCommand(Login);
            CmdEditAdminPassword = new GalaCmd.RelayCommand(EditAdminPassword);
            CmdClearAlarm = new GalaCmd.RelayCommand(ClearAlarm);


            // 工作加载
            CmdLoadWork = new GalaCmd.RelayCommand<object>(LoadWork);
            CmdUnLoadWork = new GalaCmd.RelayCommand(CommandUnLoadWork);

            // 联机模式
            CmdSetSystemOnline = new GalaCmd.RelayCommand(SetSystemOnline);

        }

        /// <summary>
        /// 检测Default工程目录
        /// </summary>
        private void CheckRootFolder()
        {
            try
            {
                string directory = ECFileConstantsManager.RootFolder;
                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);
            }
            catch (System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, LogLevel.Error);
            }
        }

        /// <summary>
        /// 注册订阅的消息
        /// </summary>
        private void RegisterMessenger()
        {
            Messenger.Default.Register<string>(this, ECMessengerManager.MainWindowMessengerKeys.SystemClosing, OnMainWindowClosing);
            Messenger.Default.Register<string>(this, ECMessengerManager.MainViewModelMessengerKeys.AskWorkName, OnAskWorkName);
            Messenger.Default.Register<string>(this, ECMessengerManager.MainViewModelMessengerKeys.CloseWork, OnCloseWork);
            Messenger.Default.Register<string>(this, ECMessengerManager.WorkRuntimeViewModelMessengerKeys.TCPMsgToSystemServer, OnWorkRuntimeTCPMsg);
            Messenger.Default.Register<int>(this, ECMessengerManager.MainViewModelMessengerKeys.WorkLoaded, OnWorkLoaded);
            Messenger.Default.Register<string>(this, ECMessengerManager.ECLogMessengerKeys.OnAlarm, OnAlarm);

            // Ffp消息
            Messenger.Default.Register<bool>(this, ECMessengerManager.CommCardMessengerKeys.FFPSetOnline, SetSystemOnlineMode);
            Messenger.Default.Register<int>(this, ECMessengerManager.CommCardMessengerKeys.FFPJobChangeRequested, LoadWork);
            Messenger.Default.Register<string>(this, ECMessengerManager.CommCardMessengerKeys.FFPClearError, ClearError);

            // Nativemode消息
            //Messenger.Default.Register<bool>(this, ECMessengerManager.ECNativeModeCommandMessengerKeys.LoadRecipeAck, OnNativeModeAck);
            //Messenger.Default.Register<bool>(this, ECMessengerManager.ECNativeModeCommandMessengerKeys.SetUserDataAck, OnNativeModeAck);
        }

        /// <summary>
        /// 警报提示
        /// </summary>
        /// <param name="obj"></param>
        private void OnAlarm(string obj)
        {
            IsAlarm = true;
        }

        /// <summary>
        /// 设置用户数据确认
        /// </summary>
        /// <param name="obj"></param>
        //private void OnNativeModeAck(bool obj)
        //{
        //    int ack = obj ? 1 : 0;
        //    //_systemTCPServer?.Broadcast(ack.ToString());
        //}

        /// <summary>
        /// 工作加载完成
        /// </summary>
        /// <param name="obj"></param>
        private void OnWorkLoaded(int obj)
        {
            if (_isStartupOnline)
            {
                SetSystemOnlineMode(true);
                _isStartupOnline = false;
            }
        }

        /// <summary>
        /// 清除错误
        /// </summary>
        /// <param name="obj"></param>
        private void ClearError(string obj)
        {

        }

        /// <summary>
        /// 注销订阅的消息
        /// </summary>
        private void UnregisterMessenger()
        {
            Messenger.Default.Unregister<string>(this, ECMessengerManager.MainWindowMessengerKeys.SystemClosing, OnMainWindowClosing);
            Messenger.Default.Unregister<string>(this, ECMessengerManager.MainViewModelMessengerKeys.AskWorkName, OnAskWorkName);
            Messenger.Default.Unregister<string>(this, ECMessengerManager.MainViewModelMessengerKeys.CloseWork, OnCloseWork);
            Messenger.Default.Unregister<string>(this, ECMessengerManager.WorkRuntimeViewModelMessengerKeys.TCPMsgToSystemServer, OnWorkRuntimeTCPMsg);
        }

        /// <summary>
        /// 收到工作运行时需要系统TCP转发的消息
        /// </summary>
        /// <param name="obj"></param>
        private void OnWorkRuntimeTCPMsg(string obj)
        {
            if (obj != null)
                _systemTCPServer?.Broadcast(obj);
        }

        /// <summary>
        /// 关闭工作编辑
        /// </summary>
        /// <param name="obj"></param>
        private void OnCloseWork(string obj)
        {
            LoadContentView("");
            Title = ECFileConstantsManager.ProgramName;
            WorkTitle = "";
            IsWorkSelected = false;
            ECLog.WriteToLog($"\"{SelectedWorkName}\" {ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.CloseEdit)}", LogLevel.Trace);
        }

        /// <summary>
        /// 返回选择的工作名称
        /// </summary>
        /// <param name="obj"></param>
        private void OnAskWorkName(string obj)
        {
            Messenger.Default.Send<string>(SelectedWorkName, ECMessengerManager.MainViewModelMessengerKeys.ReplyWorkName);
        }

        /// <summary>
        /// 主窗口关闭
        /// </summary>
        /// <param name="obj"></param>
        private void OnMainWindowClosing(string obj)
        {
            CLoseSystem();
            Messenger.Default.Send<string>("", ECMessengerManager.MainViewModelMessengerKeys.SystemClosed);
        }

        /// <summary>
        /// 检测软件授权
        /// </summary>
        /// <returns></returns>
        private bool CheckSoftwareLicense()
        {
            ECLicense eCLicense = new ECLicense();
            if (eCLicense.CheckLicense())
                return true;
            else
                ECDialogManager.ShowMsg(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.InvalidLicenseFile));

            return false;
        }

        /// <summary>
        /// 获取启动信息并进行配置
        /// </summary>
        private void GetStartupInfo()
        {
            try
            {

                _startupSettings = ECStartupSettings.Instance();

                ECGeneric.CheckLanguage(_startupSettings.SelectedLanguage);

                ECLog.WriteToLog(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.ProgramStart), LogLevel.Trace);

                if (_startupSettings.FrameworkMode == 1)
                {
                    ECDLEnvironment.IsEnable = true;
                    ECLog.WriteToLog(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.DLEnabled), LogLevel.Trace);
                }

                IsLogin = _startupSettings.IsDefaultLoginAdmin;
                _adminPassword = _startupSettings.AdminPassword;

                if (_startupSettings.SystemTCPServerIP != null && _startupSettings.SystemTCPServerPort >= 0)
                {
                    _systemTCPServer = new SimpleTCP.SimpleTcpServer();
                    _systemTCPServer.Start(System.Net.IPAddress.Parse(_startupSettings.SystemTCPServerIP), _startupSettings.SystemTCPServerPort);
                    _systemTCPServer.DataReceived += SystemTCPServer_DataReceived;
                    _systemTCPServer.ClientConnected += ClientConnected;
                    IsSystemTCPOpened = true;
                    ECLog.WriteToLog(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.SystemServerStart) + " " +
                        _startupSettings.SystemTCPServerIP + ":" + _startupSettings.SystemTCPServerPort, LogLevel.Trace);
                }

                // 设置通讯卡
                if (ECCommCard.Bank0 == null)
                    ECLog.WriteToLog(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.NoCC24CommCard), LogLevel.Trace);
                else if (_startupSettings.FfpType != null)
                {
                    CogFfpProtocolConstants type;
                    bool result = Enum.TryParse(_startupSettings.FfpType, out type);

                    if (result && type != CogFfpProtocolConstants.None)
                    {
                        ECCommCard.SetFfpType(type);
                        ECLog.WriteToLog($"{ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.FFPType)} {Enum.GetName(typeof(CogFfpProtocolConstants), type)}", LogLevel.Trace);
                    }
                }

                ECFileConstantsManager.RootDisk = _startupSettings.SelectedImageDiskName;
                ECFileConstantsManager.RootFolder = ECFileConstantsManager.RootDisk + $"\\{ECFileConstantsManager.RootFolderName}\\";
                ECFileConstantsManager.ImageRootFolder = _startupSettings.SelectedImageDiskName + $"\\{ECFileConstantsManager.ImageRootFolderName}\\";

                if (_startupSettings.IsStatupOnline)
                {
                    _defaultWorkName= _startupSettings.WorksStartupInfo.FirstOrDefault(work => work.IsDefaultWork)?.WorkName;
                    if (string.IsNullOrEmpty(_defaultWorkName)&& _startupSettings.WorksStartupInfo.Count!=0)
                    {
                        _defaultWorkName = _startupSettings.WorksStartupInfo.FirstOrDefault()?.WorkName;
                    }
                    _isStartupOnline= true;
                }
            

            }
            catch (System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, LogLevel.Error);
            }
        }

        private void ClientConnected(object sender, System.Net.Sockets.TcpClient e)
        {
            ECLog.WriteToLog($"Connected from  {e.Client.RemoteEndPoint}",LogLevel.Debug);
        }

        /// <summary>
        /// 检查VisionPro授权
        /// </summary>
        //private bool CheckVisionProLicense()
        //{
        //    try
        //    {
        //        int days = 0;
        //        bool license = CogLicense.GetDaysRemaining(out days, true);
        //        if (!license)
        //        {
        //            ECDialogManager.ShowMsg(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.InvalidVProLicense));
        //            return false;
        //        }
        //        else
        //            return true;
        //    }
        //    catch (System.Exception ex)
        //    {
        //        ECLog.WriteToLog(ex.StackTrace + ex.Message, LogLevel.Trace);  
        //        ECDialogManager.ShowMsg(ex.Message);
        //        return false;
        //    }

        //}

        /// <summary>
        /// 初始化ViDi环境
        /// </summary>
        private bool InitialDLEnvironment()
        {
            List<int> gpuList = new List<int>();
            try
            {
                ViDiSuiteServiceLocator.Initialize();
                ECDLEnvironment.Control = new ViDi2.Runtime.Local.Control(GpuMode.Default, gpuList);
                ECLog.WriteToLog($"{ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.VPDLVersion)}：" + ECDLEnvironment.Control.DotNETWrapperVersion.ToString(), LogLevel.Trace);
                ECDLEnvironment.Control.InitializeComputeDevices(GpuMode.SingleDevicePerTool, gpuList);
                foreach (var d in ECDLEnvironment.Control.ComputeDevices)
                {
                    ECDLEnvironment.GPUList.Add(d.Index.ToString() + ": " + d.Name);
                    ECLog.WriteToLog($"GPU{d.Index}: " + d.Name, LogLevel.Trace);
                }
                ECLog.WriteToLog(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.VPDLLoadFinished), LogLevel.Trace);
                return true;
            }
            catch (System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message+ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.VPDLLoadFailed), LogLevel.Trace);
                ECDialogManager.ShowMsg(ex.Message);
                return false;
            }

        }

        /// <summary>
        /// 设置系统Default参数
        /// </summary>
        private void SetSystemDefaultConfig()
        {
            // 显示工作列表
            IsShowWorkList = true;

            //初始工作显示
            Title = ECFileConstantsManager.ProgramName;

            //手动模式
            IsOnlineMode = false;
            IsOnlineMode = false;
            IsWorkLoaded = false;
        }

        /// <summary>
        /// 启动UI定时器,用于更新界面时间和一些系统状态的显示
        /// </summary>
        private void StartUITimer()
        {
            _timer = new System.Timers.Timer();
            _timer.Interval = 2000;
            _timer.Elapsed += Timer_Elapsed;
            _timer.Start();
        }

        /// <summary>
        /// 日期刷新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            CheckSystemTCPStatus();
            if(IsWorkLoaded)
                ClearExpiredData();
        }

        /// <summary>
        /// 清理过期的数据
        /// </summary>
        private void ClearExpiredData()
        {
            if(_startupSettings!=null&&!_isClearingExpiredData)
            {
                _isClearingExpiredData = true;
                Task.Factory.StartNew(() =>
                {
                    // 图片根目录
                    string folder = ECFileConstantsManager.ImageRootFolder + @"\" + SelectedWorkName + @"\" + ECFileConstantsManager.ImageRecordFolderName;

                    if (Directory.Exists(folder))
                    {
                        string[] streamFolders = Directory.GetDirectories(folder);
                        foreach (string file in streamFolders)
                        {
                            // 图片文件夹
                            string originalFolder = file + @"\" + ECFileConstantsManager.OriginalImageFolderName; // 原图文件夹
                            string graphicFolder = file + @"\" + ECFileConstantsManager.GraphicImageFolderName; // 图形图片文件夹

                            // OK/NG文件夹删除
                            DeleteExpiredOKNGDirectory(originalFolder, _startupSettings.ImageRetainedDaysForOK,_startupSettings.ImageRetainedDaysForNG);
                            DeleteExpiredOKNGDirectory(graphicFolder, _startupSettings.ImageRetainedDaysForOK, _startupSettings.ImageRetainedDaysForNG);

                        }
                    }
                    string databaseFolder = ECFileConstantsManager.RootFolder + @"\" + SelectedWorkName + @"\" + ECFileConstantsManager.DatabaseFolderName;

                    // 工作流数据文件夹
                    if (Directory.Exists(databaseFolder + @"\" + ECFileConstantsManager.WorkStreamsDataFolderName))
                    {
                        string[] streamsFolder = Directory.GetDirectories(databaseFolder + @"\" + ECFileConstantsManager.WorkStreamsDataFolderName);
                        foreach (string streamfolder in streamsFolder)
                        {
                            DeleteExpiredDirectory(streamfolder, _startupSettings.DataRetainedDays);
                        }
                    }

                    // 工作日志文件夹
                    if (Directory.Exists(databaseFolder + @"\" + ECFileConstantsManager.WorkLogFolderName))
                    {
                        DeleteExpiredDirectory(databaseFolder + @"\" + ECFileConstantsManager.WorkLogFolderName, _startupSettings.DataRetainedDays);
                    }

                    _isClearingExpiredData = false;
                });
            }
        }

        /// <summary>
        /// 删除过期的OK/NG目录
        /// </summary>
        /// <param name="path">目录路径</param>
        /// <param name="expiredDays">过期天数限定值</param>
        private void DeleteExpiredOKNGDirectory(string path, int expiredDaysForOK, int expiredDaysForNG)
        {
            try
            {
                if (Directory.Exists(path))
                {
                    string[] originalSubs = Directory.GetDirectories(path);
                    foreach (string sub in originalSubs)
                    {
                        DateTime folderTime;
                        string name = System.IO.Path.GetFileName(sub);
                        if (DateTime.TryParseExact(name,
                                                   "yyyy_MM_dd",
                                                   System.Globalization.CultureInfo.InvariantCulture,
                                                   System.Globalization.DateTimeStyles.None,
                                                   out folderTime))
                        {
                            int days = (DateTime.Now - folderTime).Days;

                            string okFolderPath = sub + @"\OK";
                            string ngFolderPath = sub + @"\NG";

                            // 删除过期OK文件夹
                            if (days >= expiredDaysForOK&&Directory.Exists(okFolderPath))
                                Directory.Delete(okFolderPath, true);
                            // 删除过期NG文件夹
                            if (days >= expiredDaysForNG && Directory.Exists(ngFolderPath))
                                Directory.Delete(ngFolderPath, true);

                            // 若文件夹已空，则删除日期的文件夹
                            if(Directory.GetDirectories(sub).Length==0)
                                Directory.Delete(sub, true);
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, NLog.LogLevel.Warn);
            }
        }

        /// <summary>
        /// 删除过期的目录
        /// </summary>
        /// <param name="path">目录路径</param>
        /// <param name="expiredDays">过期天数限定值</param>
        private void DeleteExpiredDirectory(string path,int expiredDays)
        {
            try
            {
                if (Directory.Exists(path))
                {
                    string[] originalSubs = Directory.GetDirectories(path);
                    foreach (string sub in originalSubs)
                    {
                        DateTime folderTime;
                        string name = System.IO.Path.GetFileName(sub);
                        if (DateTime.TryParseExact(name,
                                                   "yyyy_MM_dd",
                                                   System.Globalization.CultureInfo.InvariantCulture,
                                                   System.Globalization.DateTimeStyles.None,
                                                   out folderTime))
                        {
                            int days = (DateTime.Now - folderTime).Days;
                            if (days >= expiredDays)
                            {
                                Directory.Delete(sub, true);
                            }
                        }
                    }
                }
            }
            catch(System.Exception ex) 
            { 
                ECLog.WriteToLog(ex.StackTrace+ex.Message, NLog.LogLevel.Warn);
            }
        }

        /// <summary>
        /// 卸载工作指令执行
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private void CommandUnLoadWork()
        {
            if (!ECDialogManager.Verify($"{ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.UnloadWork)} \"{SelectedWorkName}\"")) return;
            UnLoadWork();
        }

        /// <summary>
        /// 卸载工作
        /// </summary>
        private void UnLoadWork()
        {
            try
            {
                Messenger.Default.Send<int>(-1, ECMessengerManager.MainViewModelMessengerKeys.UnLoadWork);
                ECLog.WriteToLog($"{ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.UnloadWork)} \"{_selectedWorkName}\"", LogLevel.Trace);
                Messenger.Default.Send<string>(SelectedWorkName, ECMessengerManager.MainViewModelMessengerKeys.UnLoadWork);
                LoadContentView("");
                WorkTitle = "";
                IsWorkLoaded = false;
            }
            catch (System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, LogLevel.Error);
            }
        }

        /// <summary>
        /// 装载工作
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private void LoadWork(object obj)
        {
            if (obj == null) return;
            if (IsEditingWork())
            {
                ECDialogManager.ShowMsg(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.CloseEditingWorkFirstly));
                return;
            }
            string workName = (obj as WorkListItemViewModel).WorkInfo.WorkName;
            bool verifyOpen = ECDialogManager.Verify($"{ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.LoadWork)} \"{workName}\"");
            if (verifyOpen)
            {
                ECWorkInfo info = WorkList.FirstOrDefault(w => w.WorkInfo.WorkName == workName)?.WorkInfo;
                if (!CheckWorkDLEnable(info))
                {
                        ECDialogManager.ShowMsg(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.IncompatibleMode));
                    return;
                }
                LoadWork(workName);
            }
        }

        /// <summary>
        /// 装载工作,通过传入的工作名称
        /// </summary>
        /// <param name="workName"></param>
        public void LoadWork(string workName)
        {
            if (!_isSystemStartComplete) return;
            try
            {
                ECWorkInfo info = WorkList.FirstOrDefault(w => w.WorkInfo.WorkName == workName)?.WorkInfo;
                if (!CheckWorkDLEnable(info))
                {
                    ECDialogManager.ShowMsg(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.IncompatibleMode));
                    return;
                }

                ECLog.WriteToLog($"{ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.LoadWork)} \"{workName}\"", LogLevel.Trace);
                SelectedWorkName = workName;
                Title = "VisionPro Framework" + " — " + workName;
                IsWorkLoaded = true;
                ECStartupSettings startupSettings = ECStartupSettings.Instance();

                foreach (ECWorkStartupInfo wi in startupSettings.WorksStartupInfo)
                {
                    if (wi.WorkName == workName)
                    {
                        WorkTitle = wi.Title;
                        break;
                    }
                }
                LoadContentView("WorkRuntimeView");

                // 通知其他加载的模块
                Messenger.Default.Send<ECWorkInfo>(info, ECMessengerManager.MainViewModelMessengerKeys.LoadWorkChanged);
            }
            catch (System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, NLog.LogLevel.Error);
            }
        }

        /// <summary>
        /// 检查工作设置的模式与当前程序启动的模式是否一致,启用了DL或没有启用DL
        /// </summary>
        private bool CheckWorkDLEnable(ECWorkInfo info)
        {
            if (info.IsDLEnable == ECDLEnvironment.IsEnable)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 装载工作,通过传入的工作ID
        /// </summary>
        /// <param name="workName"></param>
        public void LoadWork(int workID)
        {
            if (!_isSystemStartComplete) return;
            try
            {
                foreach (WorkListItemViewModel item in WorkList)
                {
                    if (item.WorkInfo.WorkID == workID)
                    {
                        string workName = item.WorkInfo.WorkName;
                        LoadWork(workName);
                    }
                }
            }
            catch (System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, NLog.LogLevel.Error);
            }

        }

        /// <summary>
        /// 检查系统TCP服务器状态
        /// </summary>
        private void CheckSystemTCPStatus()
        {
            if (_systemTCPServer != null)
            {
                if (_systemTCPServer.GetListeningIPs().Count > 0)
                    IsSystemTCPOpened = true;
                else
                    IsSystemTCPOpened = false;
            }
        }

        /// <summary>
        /// 系统TCP服务器收到消息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SystemTCPServer_DataReceived(object sender, SimpleTCP.Message e)
        {
            ECLog.WriteToLog($"Receive data --{e.MessageString}-- from {e.TcpClient.Client.RemoteEndPoint}", NLog.LogLevel.Info);
            try
            {
                // 收到消息不为空
                if (e != null && e.MessageString.Trim() != "")
                {
                    // 检查命令类型
                    NativeModeCommandTypeConstants constant = ECNativeModeCommand.CheckCommandString(e.MessageString);
                    
                    // 命令类型不为错误
                    if (constant != NativeModeCommandTypeConstants.ERR)
                    {
                        // 执行并返回执行情况
                        object reply = ExecuteNativeCommandAction(constant, e.MessageString);
                        if (Convert.ToString(reply) != "-2")
                            _systemTCPServer?.Broadcast(reply.ToString());
                    }
                    // 命令类型错误,返回0
                    else
                        _systemTCPServer?.Broadcast("0");
                }
            }
            catch (System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, LogLevel.Error);
            }
        }

        /// <summary>
        /// 执行本地指令动作
        /// </summary>
        private object ExecuteNativeCommandAction(NativeModeCommandTypeConstants constant, string content)
        {
            try
            {
                object result = 1;
                string[] commandStr = content.Split(',');
                switch (constant)
                {
                    //触发工作流
                    case NativeModeCommandTypeConstants.TS:
                        if (IsOnlineMode)
                        {
                            //Messenger.Default.Send<string>(commandStr[0], ECMessengerManager.MainViewModelMessengerKeys.NativeModeMsg);
                            Messenger.Default.Send<string>(content, ECMessengerManager.MainViewModelMessengerKeys.NativeModeMsg);
                            result = -2;
                        }
                        else
                            result = 0;
                        break;

                    //触发工作流
                    case NativeModeCommandTypeConstants.TMS:
                        if (IsOnlineMode)
                        {
                            Messenger.Default.Send<string>(content, ECMessengerManager.MainViewModelMessengerKeys.NativeModeMsg);
                            result = -2;
                        }
                        else
                            result = 0;
                        break;

                    // 加载工作流配方
                    case NativeModeCommandTypeConstants.LR:
                        if (IsOnlineMode)
                        {
                            Messenger.Default.Send<string>(content, ECMessengerManager.MainViewModelMessengerKeys.NativeModeMsg);
                            return -2;
                        }
                        else
                            result = 0;
                        break;

                    // 设置用户数据
                    case NativeModeCommandTypeConstants.SUD:
                        if (IsOnlineMode)
                        {
                            Messenger.Default.Send<string>(content, ECMessengerManager.MainViewModelMessengerKeys.NativeModeMsg);
                            return -2;
                        }
                        else
                            result = 0;
                        break;

                    // 设置用户数据
                    case NativeModeCommandTypeConstants.SET:
                        if (IsOnlineMode)
                        {
                            Messenger.Default.Send<string>(content, ECMessengerManager.MainViewModelMessengerKeys.NativeModeMsg);
                            return -2;
                        }
                        else
                            result = 0;
                        break;

                    // 设置用户数据
                    case NativeModeCommandTypeConstants.SIS:
                        if (IsOnlineMode)
                        {
                            Messenger.Default.Send<string>(content, ECMessengerManager.MainViewModelMessengerKeys.NativeModeMsg);
                            return -2;
                        }
                        else
                            result = 0;
                        break;

                    // 工作流自触发启动
                    case NativeModeCommandTypeConstants.TSB:
                        if (IsOnlineMode)
                        {
                            Messenger.Default.Send<string>(content, ECMessengerManager.MainViewModelMessengerKeys.NativeModeMsg);
                            return -2;
                        }
                        else
                            result = 0;
                        break;

                    // 工作流自触发停止
                    case NativeModeCommandTypeConstants.TSE:
                        if (IsOnlineMode)
                        {
                            Messenger.Default.Send<string>(content, ECMessengerManager.MainViewModelMessengerKeys.NativeModeMsg);
                            return -2;
                        }
                        else
                            result = 0;
                        break;

                    // 触发工作流并设置用户数据
                    case NativeModeCommandTypeConstants.TWD:
                        if (IsOnlineMode)
                        {
                            Messenger.Default.Send<string>(content, ECMessengerManager.MainViewModelMessengerKeys.NativeModeMsg);
                            return -2;
                        }
                        else
                            result = 0;
                        break;

                    // 触发工作流并设置曝光时间
                    case NativeModeCommandTypeConstants.TWE:
                        if (IsOnlineMode)
                        {
                            Messenger.Default.Send<string>(content, ECMessengerManager.MainViewModelMessengerKeys.NativeModeMsg);
                            return -2;
                        }
                        else
                            result = 0;
                        break;

                    // 触发工作流并设置曝光时间和用户数据
                    case NativeModeCommandTypeConstants.TWED:
                        if (IsOnlineMode)
                        {
                            Messenger.Default.Send<string>(content, ECMessengerManager.MainViewModelMessengerKeys.NativeModeMsg);
                            return -2;
                        }
                        else
                            result = 0;
                        break;

                    // 触发工作流并设置图像源
                    case NativeModeCommandTypeConstants.TWI:
                        if (IsOnlineMode)
                        {
                            Messenger.Default.Send<string>(content, ECMessengerManager.MainViewModelMessengerKeys.NativeModeMsg);
                            return -2;
                        }
                        else
                            result = 0;
                        break;

                    // 触发图像源
                    case NativeModeCommandTypeConstants.TI:
                        if (IsOnlineMode)
                        {
                            Messenger.Default.Send<string>(content, ECMessengerManager.MainViewModelMessengerKeys.NativeModeMsg);
                            return -2;
                        }
                        else
                            result = 0;
                        break;

                    // 触发图像源
                    case NativeModeCommandTypeConstants.TIWE:
                        if (IsOnlineMode)
                        {
                            Messenger.Default.Send<string>(content, ECMessengerManager.MainViewModelMessengerKeys.NativeModeMsg);
                            return -2;
                        }
                        else
                            result = 0;
                        break;

                    // 加载工作
                    case NativeModeCommandTypeConstants.LW:
                        if (!IsWorkLoaded && WorkList.ToList().Exists(w => w.WorkInfo.WorkName == commandStr[1]))
                            LoadWork(commandStr[1]);
                        else
                            result = 0;
                        break;

                    // 系统联机脱机
                    case NativeModeCommandTypeConstants.SO:
                        if (Convert.ToInt16(commandStr[1]) == 1)
                            IsOnlineMode = true;
                        else if (Convert.ToInt16(commandStr[1]) == 0)
                            IsOnlineMode = false;
                        break;

                    // 卸载工作
                    case NativeModeCommandTypeConstants.UW:
                        if (IsWorkLoaded && !IsOnlineMode)
                            UnLoadWork();
                        else
                            result = 0;
                        break;

                    // 获取联机状态
                    case NativeModeCommandTypeConstants.GO:
                        if (IsOnlineMode)
                            result = 1;
                        else
                            result = 0;
                        break;

                    // 获取当前工作包含的工作流
                    case NativeModeCommandTypeConstants.GS:
                        if (IsWorkLoaded)
                        {
                            string[] streams = Directory.GetDirectories(ECFileConstantsManager.RootFolder + @"\" + SelectedWorkName);
                            result = "";
                            foreach (string stream in streams)
                            {
                                result += stream + "\r\n";
                            }
                        }
                        break;

                    // 获取当前工作名称
                    case NativeModeCommandTypeConstants.GW:
                        if (IsWorkLoaded)
                            result = SelectedWorkName;
                        break;

                    // 获取所有工作名称
                    case NativeModeCommandTypeConstants.GAW:
                        string[] works = Directory.GetDirectories(ECFileConstantsManager.RootFolder);
                        result = "";
                        foreach (string stream in works)
                        {
                            result += System.IO.Path.GetFileName(stream) + "\r\n";
                        }
                        break;
                }
                return result;
            }
            catch
            {
                return -1;
            }
        }

        /// <summary>
        /// 设置系统联机或脱机
        /// </summary>
        private void SetSystemOnline()
        {
            if (IsOnlineMode)
            {
                if (ECDialogManager.Verify(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.VerifyOffline)))
                    SetSystemOnlineMode(false);
            }
            else
            {
                if (ECDialogManager.Verify(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.VerifyOnline)))
                    SetSystemOnlineMode(true);
            }
        }

        /// <summary>
        /// 设置系统联机模式
        /// </summary>
        /// <param name="online"></param>
        public void SetSystemOnlineMode(bool online)
        {
            if (!IsWorkLoaded) return;
            IsOnlineMode = online;
            if (IsOnlineMode)
                Messenger.Default.Send<string>("", ECMessengerManager.MainViewModelMessengerKeys.SystemOnline);
            else
                Messenger.Default.Send<string>("", ECMessengerManager.MainViewModelMessengerKeys.SystemOffline);
        }

        /// <summary>
        /// 检查是否联机启动
        /// </summary>
        private void CheckStartupOnline()
        {
            if (_isStartupOnline && !string.IsNullOrEmpty(_defaultWorkName))
            {
                ECLog.WriteToLog($"{ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.StartupWithOnlineMode)} \"{_defaultWorkName}\"", LogLevel.Trace);
                LoadWork(_defaultWorkName);
            }
        }

        /// <summary>
        /// 加载菜单选的操作界面
        /// </summary>
        /// <param name="viewName"></param>
        private void LoadContentView(string viewName)
        {
            try
            {
                switch (viewName)
                {
                    case "WorkEditView":
                        Task.Factory.StartNew(new Action(() =>
                        {
                            DispatcherHelper.CheckBeginInvokeOnUI(() =>
                            {
                                if(_controlWorkEdit==null)
                                    _controlWorkEdit = new Control_WorkEdit();

                                    ContentView=_controlWorkEdit;
                            });
                        }));
                        break;
                    case "WorkRuntimeView":
                        Task.Factory.StartNew(new Action(() =>
                        {
                            DispatcherHelper.CheckBeginInvokeOnUI(() =>
                            {
                                if(_controlWorkRuntime==null)
                                    _controlWorkRuntime = new Control_WorkRuntime();

                                    ContentView=_controlWorkRuntime;
                            });
                        }));
                        break;

                    case "":
                        ContentView = null;
                        break;
                }
            }
            catch (System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, LogLevel.Error);
            }
        }

        /// <summary>
        /// 关闭系统
        /// </summary>
        private void CLoseSystem()
        {
            UnregisterMessenger();

            // 停止系统TCP服务器
            _systemTCPServer?.Stop();

            // 卸载工作
            if (IsWorkLoaded)
                UnLoadWork();

            // 停止计时
            _timer.Stop();

            // 释放系统信息监视对象
            WindowsSystemInfo.Dispose();

            // 释放DL环境
            ECDLEnvironment.ClearWorkspaces();
            ECDLEnvironment.Control?.Dispose();

            // 释放硬件相机
            DisposeFrameGrabber();

            // 关闭EL相关资源
            Startup.Shutdown();

            ViewModelLocator.Cleanup();
        }

        /// <summary>
        /// 释放所有相机
        /// </summary>
        private void DisposeFrameGrabber()
        {
            try
            {
                CogFrameGrabbers frameGrabbers = new CogFrameGrabbers();
                foreach (ICogFrameGrabber fg in frameGrabbers)
                {
                    if (fg != null)
                        fg.Disconnect(false);
                }
                frameGrabbers.Dispose();
            }
            catch (System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, LogLevel.Error);
            }
        }

        /// <summary>
        /// 清除警报
        /// </summary>
        private void ClearAlarm()
        {
            IsAlarm = false;
        }

        #endregion

        #region 编辑工作

        /// <summary>
        /// 刷新工作列表
        /// </summary>
        private void RefreshWorkList()
        {
            try
            {
                if (IsEditingWork())
                {
                    ECDialogManager.ShowMsg(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.RefreshListAfterCloseEdit));
                    return;
                }
                string path = ECFileConstantsManager.RootFolder;
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                DirectoryInfo root = new DirectoryInfo(path);
                DirectoryInfo[] dics = root.GetDirectories();
                WorkList = new BindingList<WorkListItemViewModel>();
                foreach (DirectoryInfo d in dics)
                {
                    string jsonPath = d.FullName + @"\" + ECFileConstantsManager.WorkInfoFileName;
                    if (File.Exists(jsonPath))
                    {
                        ECWorkInfo workInfo = ECSerializer.LoadObjectFromJson<ECWorkInfo>(jsonPath);
                        WorkListItemViewModel workItemViewModel = new WorkListItemViewModel();
                        workItemViewModel.WorkInfo = workInfo;
                        DispatcherHelper.UIDispatcher.Invoke(() =>
                        {
                            WorkList.Add(workItemViewModel);
                        });
                    }
                }
            }
            catch (System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, LogLevel.Error);
            }
        }

        /// <summary>
        /// 创建新的工作
        /// </summary>
        private void CreateNewWork()
        {
            if(IsEditingWork()) 
            {
                ECDialogManager.ShowMsg(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.CloseEditingWorkFirstly));
                return;
            }
            try
            {
                string workName = ECDialogManager.GetUserInput(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.PleaseInputName));
                bool match = false;
                if (workName != null && workName.Trim() != "")
                {
                    Regex regex = new Regex(@"^[A-Za-z0-9_]{1,}$");
                    match = regex.IsMatch(workName);
                }
                if (workName != null && match)
                {
                    string path = ECFileConstantsManager.RootFolder + @"\" + workName;
                    if (!Directory.Exists(path))
                        CreateNewWorkFoldersAndFiles(workName);
                    
                    else
                    {
                        if (ECDialogManager.Verify(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.OverrideExist)))
                        {
                            Directory.Delete(path, true);

                            CreateNewWorkFoldersAndFiles(workName);
                        }
                    }
                }
                else if (workName != "")
                    ECDialogManager.ShowMsg(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.InvalidName));

                RefreshWorkList();
            }
            catch (System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, NLog.LogLevel.Error);
            }
        }

        /// <summary>
        /// 创建新工作的文件及文件夹
        /// </summary>
        /// <param name="workName"></param>
        private void CreateNewWorkFoldersAndFiles(string workName)
        {
            // 创建工作信息文件
            ECWorkInfo workInfo = new ECWorkInfo();
            workInfo.WorkName = workName;
            workInfo.WorkID = GetNewWorkID();
            workInfo.IsDLEnable = ECDLEnvironment.IsEnable;
            workInfo.ModifiedTime=DateTime.Now;

            // 创建工作各模块文件夹
            string path = ECFileConstantsManager.RootFolder + @"\" + workName;
            Directory.CreateDirectory(path);
            Directory.CreateDirectory(path + @"\" + ECFileConstantsManager.ImageSourceFolderName);
            Directory.CreateDirectory(path + @"\" + ECFileConstantsManager.StreamsFolderName);
            Directory.CreateDirectory(path + @"\" + ECFileConstantsManager.TCPConfigFolderName);
            Directory.CreateDirectory(path + @"\" + ECFileConstantsManager.ImageRecordFolderName);
            Directory.CreateDirectory(path + @"\" + ECFileConstantsManager.DatabaseFolderName);
            Directory.CreateDirectory(path + @"\" + ECFileConstantsManager.WorkspaceFolderName);
            Directory.CreateDirectory(path + @"\" + ECFileConstantsManager.GroupsFolderName);
            Directory.CreateDirectory(path + @"\" + ECFileConstantsManager.CommCardConfigFolderName);

            // 保存工作信息文件
            string jsonPath = path + @"\" + ECFileConstantsManager.WorkInfoFileName;
            ECSerializer.SaveObjectToJson(jsonPath, workInfo);
        }

        /// <summary>
        /// 获取新的工作ID
        /// </summary>
        /// <returns></returns>
        private int GetNewWorkID()
        {
            string path = ECFileConstantsManager.RootFolder;
            string[] folders = Directory.GetDirectories(path);
            if (folders.Count() == 0) return 1;
            int maxID = 1;
            foreach (string folder in folders)
            {
                string jsonPath = folder + @"\" + ECFileConstantsManager.WorkInfoFileName;
                if (File.Exists(jsonPath))
                {
                    ECWorkInfo info = ECSerializer.LoadObjectFromJson<ECWorkInfo>(jsonPath);
                    if (info.WorkID > maxID) maxID = info.WorkID;
                }
            }

            return maxID += 1;
        }

        /// <summary>
        /// 删除工作
        /// </summary>
        /// <param name="work">要删除的工作</param>
        private void DeleteWork(object work)
        {
            try
            {
                if (work == null) return;
                if (IsEditingWork())
                {
                    ECDialogManager.ShowMsg(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.CloseEditingWorkFirstly));
                    return;
                }

                WorkListItemViewModel selectedwork = work as WorkListItemViewModel;
                if (selectedwork.IsEdit)
                {
                    ECDialogManager.ShowMsg(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.CloseEditingWorkFirstly));
                    return;
                }
                if (ECDialogManager.Verify($"{ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.ConfirmRemove)} \"{selectedwork.WorkInfo.WorkName}\""))
                {
                    try
                    {
                        System.IO.DriveInfo[] disk = System.IO.DriveInfo.GetDrives();
                        string path = ECFileConstantsManager.RootFolder + @"\" + selectedwork.WorkInfo.WorkName;
                        string dbPath = path + @"\" + ECFileConstantsManager.DatabaseFolderName + @"\" + ECFileConstantsManager.LogDatabaseFileName;
                        
                        // 删除文件夹
                        if (Directory.Exists(path))
                            Directory.Delete(path, true);
                        ECDialogManager.ShowMsg(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.DeleteFinished));
                    }
                    catch (System.Exception ex)
                    {
                        ECDialogManager.ShowMsg(ex.StackTrace + ex.Message + ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.DeleteWithError) + ECFileConstantsManager.RootFolder + selectedwork.WorkInfo.WorkName);
                    }
                }
                RefreshWorkList();
            }
            catch (System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, LogLevel.Error);
            }
        }

        /// <summary>
        /// 复制工作
        /// </summary>
        /// <param name="work">复制的工作</param>
        private void CopyWork(object work)
        {
            if (work == null) return;
            try
            {
                WorkListItemViewModel selectedwork = work as WorkListItemViewModel;
                if (IsEditingWork())
                {
                    ECDialogManager.ShowMsg(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.CloseEditingWorkFirstly));
                    return;
                }
                string workName = ECDialogManager.GetUserInput(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.PleaseInputName));
                bool match = false;
                if (workName != null && workName.Trim() != "")
                {
                    Regex regex = new Regex(@"^[A-Za-z0-9_]{1,}$");
                    match = regex.IsMatch(workName);
                }
                else
                    return;
                if (workName != null && match)
                {
                    string sourcePath = ECFileConstantsManager.RootFolder + @"\" + selectedwork.WorkInfo.WorkName;
                    string dstPath = ECFileConstantsManager.RootFolder + @"\" + workName;
                    
                    if (!Directory.Exists(dstPath))
                    {
                        Directory.CreateDirectory(dstPath);
                        ECGeneric.CopyDirectory(sourcePath, dstPath, true);

                        ECWorkInfo workInfo = ECSerializer.LoadObjectFromJson<ECWorkInfo>(sourcePath+@"\"+ECFileConstantsManager.WorkInfoFileName);
                        workInfo.WorkName = workName;
                        workInfo.WorkID = GetNewWorkID();

                        // 保存工作信息文件
                        string jsonPath = dstPath + @"\" + ECFileConstantsManager.WorkInfoFileName;
                        ECSerializer.SaveObjectToJson(jsonPath, workInfo);
                    }
                    else
                    {
                        ECDialogManager.ShowMsg("Existed Name");
                        return;
                    }
                }
                else
                    ECDialogManager.ShowMsg("Please Use A Valid Name, Which Can Only Contain Letters, Digits, And Underscores");
                RefreshWorkList();
            }
            catch (System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, LogLevel.Error);
            }
        }

        /// <summary>
        /// 选择工作
        /// </summary>
        /// <param name="obj">选择的工作</param>
        private void SelectWork(object work)
        {
            if (!IsLogin) return;
            try
            {
                if (work == null||IsWorkLoaded) return;
                if (IsEditingWork())
                {
                    ECDialogManager.ShowMsg(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.CloseEditingWorkFirstly));
                    return;
                }
                string workName = (work as WorkListItemViewModel).WorkInfo.WorkName;
                bool verifyOpen = ECDialogManager.Verify($"{ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.EditWork)} \"{workName}\"");
                if (verifyOpen)
                {
                    ECWorkInfo info = WorkList.FirstOrDefault(w => w.WorkInfo.WorkName == workName)?.WorkInfo;
                    if (!CheckWorkDLEnable(info))
                    {
                        if (info.IsDLEnable)
                            ECDialogManager.ShowMsg(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.IncompatibleMode));
                        else
                            ECDialogManager.ShowMsg(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.IncompatibleMode));
                        return;
                    }

                    SelectedWorkName = workName;
                    Title = "VisionPro Framework" + " — " + workName;
                    IsWorkSelected = true;

                    ECStartupSettings startupSettings = ECStartupSettings.Instance();

                    foreach (ECWorkStartupInfo wi in startupSettings.WorksStartupInfo)
                    {
                        if (wi.WorkName == workName)
                        {
                            WorkTitle = wi.Title;
                            break;
                        }
                    }
                    foreach (WorkListItemViewModel workItemViewModel in WorkList)
                    {
                        if (workItemViewModel.WorkInfo.WorkName == workName)
                        {
                            workItemViewModel.IsEdit = true;
                        }
                        else
                            workItemViewModel.IsEdit = false;
                    }
                    LoadContentView("WorkEditView");
                    Messenger.Default.Send<string>(SelectedWorkName, ECMessengerManager.MainViewModelMessengerKeys.EditWorkChanged);

                    ECLog.WriteToLog($"{ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.EditWork)} \"{SelectedWorkName}\"", LogLevel.Trace);
                }
            }
            catch (System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, LogLevel.Error);
            }
        }

        /// <summary>
        /// 检查是否有未关闭的工作,若存在,则返回false
        /// </summary>
        /// <returns></returns>
        private bool IsEditingWork()
        {
            if (WorkList == null) return false;
            foreach (WorkListItemViewModel workItemViewModel in WorkList)
            {
                if (workItemViewModel.IsEdit)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 导入工作
        /// </summary>
        private void ImportWork()
        {
            try
            {
                if (IsEditingWork())
                {
                    ECDialogManager.ShowMsg(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.CloseEditingWorkFirstly));
                    return;
                }
                System.Windows.Forms.OpenFileDialog fileDialog=new System.Windows.Forms.OpenFileDialog();
                //Default打开路径
                fileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                fileDialog.Title = ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.Import);
                fileDialog.Filter = "zip file (*.zip)|*.zip";
               if(fileDialog.ShowDialog()==System.Windows.Forms.DialogResult.OK)
                {
                    
                    string path=fileDialog.FileName;
                    bool isCheckedOK = true;
                    using(ZipArchive zip=System.IO.Compression.ZipFile.OpenRead(path))
                    {
                        // 需要的文件
                        string[] files = {
                            "workInfo.json",
                            "CommCardConfig",
                            "Database",
                            "GroupsConfig",
                            "ImageSourcesConfig",
                            "StreamsConfig",
                            "TCPConfig" };

                        // 压缩包中的文件
                        List<string> zipFiles= new List<string>();
                        foreach(ZipArchiveEntry entry in zip.Entries)
                        {
                            string[] strings=entry.FullName.Split('/');
                            if(strings.Length == 0 &&!zipFiles.Contains(entry.FullName))
                                zipFiles.Add(entry.FullName);
                            else if (!zipFiles.Contains(strings[0]))
                                zipFiles.Add(strings[0]);
                        }

                        // 检查
                        foreach (string file in files)
                        {
                           if(!zipFiles.Contains(file))
                                isCheckedOK = false;
                        }
                    }

                    // 检查未通过
                    if(!isCheckedOK)
                    {
                        ECDialogManager.ShowMsg(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.CheckFailed));
                        return;
                    }

                    // 导入
                    ECDialogManager.LoadWithAnimation(new Action(() =>
                    {
                        string dstPath = ECFileConstantsManager.RootFolder + @"\" + System.IO.Path.GetFileNameWithoutExtension(path);
                        string newPath = "";
                        if(Directory.Exists(dstPath))
                            dstPath += @"\" + "_New";
                        Directory.CreateDirectory(dstPath);

                        ZipFile.ExtractToDirectory(path, dstPath);

                        try
                        {
                            string jsonPath = dstPath + @"\" + ECFileConstantsManager.WorkInfoFileName;
                            ECWorkInfo info = ECSerializer.LoadObjectFromJson<ECWorkInfo>(jsonPath);
                            if (info == null)
                            {
                                Directory.Delete(dstPath,true);
                                return;
                            }
                            else
                            {
                                info.WorkID=GetNewWorkID();
                                ECSerializer.SaveObjectToJson(jsonPath, info);
                                newPath = ECFileConstantsManager.RootFolder + @"\" + info.WorkName;
                                if (Directory.Exists(newPath))
                                {
                                    newPath = newPath + "_New";
                                    info.WorkName = info.WorkName + "_New";
                                    ECSerializer.SaveObjectToJson(jsonPath, info);
                                }

                                DirectoryInfo oldInfo = new DirectoryInfo(dstPath);
                                oldInfo.MoveTo(newPath);
                            }
                        }
                        catch(System.Exception ex)
                        {
                            ECDialogManager.ShowMsg(ex.Message + ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.ImportFailed));
                            if(Directory.Exists(dstPath)) Directory.Delete(dstPath,true);
                            if (newPath != "" && Directory.Exists(newPath)) Directory.Delete(newPath,true);
                        }

                    }), ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.Importing));

                    RefreshWorkList();
                }
            }
            catch (System.Exception ex)
            {
                ECDialogManager.ShowMsg(ex.Message + ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.ErrorOccured));
            }
        }

        /// <summary>
        /// 导出工作
        /// </summary>
        private void ExportWork(object obj)
        {
            try
            {
                if (IsEditingWork())
                {
                    ECDialogManager.ShowMsg(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.CloseEditingWorkFirstly));
                    return;
                }
                WorkListItemViewModel itemViewModel=obj as WorkListItemViewModel;
                string workName = itemViewModel.WorkInfo.WorkName;
                string folderPath = ECFileConstantsManager.RootFolder + @"\" + workName;
                string dstPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\" + DateTime.Now.ToString("yyyyMMddHHmmss") + workName + ".zip";
                if (Directory.Exists(folderPath))
                {
                    ECDialogManager.LoadWithAnimation(new Action(() =>
                    {
                       ZipFile.CreateFromDirectory(folderPath, dstPath);

                    }), ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.Exporting));
                    ECDialogManager.ShowMsg(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.ExportFinished)+": " + dstPath);
                }
            }
            catch
            {
                ECDialogManager.ShowMsg(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.ExportFailed));
            }
        }
        #endregion

        #region 菜单栏
        private void Login()
        {
            if (UserName != null && Password != null)
            {
                if (UserName == "admin" && Password == _adminPassword)
                {
                    if (IsLogin)
                    {
                        IsLogin = false;
                        UserName = "operator";
                        Password = "";
                        ECLog.WriteToLog(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.AdminLogout), LogLevel.Trace);
                    }
                    else
                    {
                        ECLog.WriteToLog(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.AdminLogin), LogLevel.Trace);
                        IsLogin = true;
                    }
                }
                else
                {
                    ECLog.WriteToLog(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.IncorrectUserOrPassword), LogLevel.Trace);
                    ECDialogManager.ShowMsg(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.IncorrectUserOrPassword));
                }
                SaveLoginStatus(IsLogin);
            }
        }

        /// <summary>
        /// 打开文件管理器
        /// </summary>
        private void OpenFileManager()
        {
            Window_FileManager window_FileManager = new Window_FileManager();
            window_FileManager.ShowDialog();
        }

        /// <summary>
        /// 打开帮助文档
        /// </summary>
        private void OpenHelp()
        {
            try
            {
                string path = ECFileConstantsManager.HelpFilePath;
                System.Diagnostics.Process.Start(path);
            }
            catch (System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, LogLevel.Error);
            }
        }

        /// <summary>
        /// 打开启动设置界面
        /// </summary>
        private void OpenStartupSetup()
        {
            Window_SetupStartup window_StartupSettings = new Window_SetupStartup();
            window_StartupSettings.ShowDialog();
        }

        /// <summary>
        /// 打开版本信息界面
        /// </summary>
        private void OpenVersionInfo()
        {
            Window_VersionInfo window_Version = new Window_VersionInfo();
            window_Version.ShowDialog();
        }

        /// <summary>
        /// 编辑管理员密码
        /// </summary>
        private void EditAdminPassword()
        {
            try
            {
                string password = ECDialogManager.GetUserInput(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.PleaseInputPassword));
                bool match = false;
                if (password != null && password.Trim() != "")
                {
                    Regex regex = new Regex(@"^[A-Za-z0-9]{1,}$");
                    match = regex.IsMatch(password);

                    if (match)
                    {
                        ECStartupSettings startupSettings = ECStartupSettings.Instance();
                        startupSettings.AdminPassword = Password;
                        startupSettings.Save();
                        ECDialogManager.ShowMsg($"{ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.NewPassword)}: {Password}");
                        _adminPassword = password;
                    }
                    else
                    {
                        ECDialogManager.ShowMsg(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.InvalidPassword));
                    }
                }
            }
            catch (System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, LogLevel.Error);
            }

        }

        /// <summary>
        /// 保存登录状态
        /// </summary>
        private void SaveLoginStatus(bool isLogin)
        {
            _startupSettings = ECStartupSettings.Instance();
            _startupSettings.IsDefaultLoginAdmin = isLogin;
            _startupSettings.Save();
        }
        #endregion

        #endregion

        #region 字段

        /// <summary>
        /// 显示的时间计时器
        /// </summary>
        private System.Timers.Timer _timer;

        /// <summary>
        /// 管理员账户密码
        /// </summary>
        private string _adminPassword = "";

        /// <summary>
        /// 系统TCP服务器
        /// </summary>
        private static SimpleTCP.SimpleTcpServer _systemTCPServer;

  
        public static SimpleTCP.SimpleTcpServer GetSystemServer()
        {
            return _systemTCPServer;
        }

        /// <summary>
        /// 系统启动完成
        /// </summary>
        private bool _isSystemStartComplete = false;

        /// <summary>
        /// 联机启动
        /// </summary>
        private bool _isStartupOnline = false;

        /// <summary>
        /// Default工作名称
        /// </summary>
        private string _defaultWorkName = "";

        /// <summary>
        /// 启动设置信息
        /// </summary>
        private ECStartupSettings _startupSettings;

        /// <summary>
        /// 正在清理过期数据
        /// </summary>
        private bool _isClearingExpiredData = false;

        /// <summary>
        /// 工作编辑用户控件
        /// </summary>
        private Control_WorkEdit _controlWorkEdit;

        /// <summary>
        /// 工作运行用户控件
        /// </summary>
        private Control_WorkRuntime _controlWorkRuntime;

        #endregion 字段

        #region 命令

        /// <summary>
        /// 命令：卸载工作
        /// </summary>
        public GalaCmd.RelayCommand<object> CmdLoadWork { get; set; }

        /// <summary>
        /// 命令：卸载工作
        /// </summary>
        public GalaCmd.RelayCommand CmdUnLoadWork { get; set; }

        /// <summary>
        /// 命令：创建新的工作
        /// </summary>
        public GalaCmd.RelayCommand CmdCreateNewWork { get; set; }

        /// <summary>
        /// 命令：删除工作
        /// </summary>
        public GalaCmd.RelayCommand<object> CmdDeleteWork { get; set; }

        /// <summary>
        /// 命令：复制工作
        /// </summary>
        public GalaCmd.RelayCommand<object> CmdCopyWork { get; set; }

        /// <summary>
        /// 命令：选择工作
        /// </summary>
        public GalaCmd.RelayCommand<object> CmdSelectWork { get; set; }

        /// <summary>
        /// 命令：导出工作
        /// </summary>
        public GalaCmd.RelayCommand<object> CmdExportWork { get; set; }

        /// <summary>
        /// 命令：导入工作
        /// </summary>
        public GalaCmd.RelayCommand CmdImportWork { get; set; }


        /// <summary>
        /// 命令：打开主界面操作的界面视图
        /// </summary>
        public GalaCmd.RelayCommand<string> CmdLoadContentView { get; set; }

        /// <summary>
        /// 命令：登录
        /// </summary>
        public GalaCmd.RelayCommand CmdLogin { get; set; }

        /// <summary>
        /// 命令：打开文件管理器
        /// </summary>
        public GalaCmd.RelayCommand CmdOpenFileManager { get; set; }

        /// <summary>
        /// 命令：打开帮助文档
        /// </summary>
        public GalaCmd.RelayCommand CmdOpenHelp { get; set; }

        /// <summary>
        /// 命令：打开启动设置
        /// </summary>
        public GalaCmd.RelayCommand CmdOpenSystemSetup { get; set; }

        /// <summary>
        /// 命令：打开版本信息
        /// </summary>
        public GalaCmd.RelayCommand CmdOpenVersionInfo { get; set; }

        /// <summary>
        /// 命令：刷新工作列表
        /// </summary>
        public GalaCmd.RelayCommand CmdRefreshWorkList { get; set; }

        /// <summary>
        /// 命令：所有工作流执行一次
        /// </summary>
        public GalaCmd.RelayCommand CmdRunOnce { get; set; }

        /// <summary>
        /// 命令：设置系统联机或者脱机
        /// </summary>
        public GalaCmd.RelayCommand CmdSetSystemOnline { get; set; }

        /// <summary>
        /// 命令：编辑管理员密码
        /// </summary>
        public GalaCmd.RelayCommand CmdEditAdminPassword { get; set; }

        /// <summary>
        /// 命令：清除警报
        /// </summary>
        public GalaCmd.RelayCommand CmdClearAlarm { get; set; }

        #endregion 命令

        #region 属性

        /// <summary>
        /// 主界面操作的视图控件
        /// </summary>
        private UserControl _contentView;
        public UserControl ContentView
        {
            get { return _contentView; }
            set
            {
                _contentView = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 显示的日期时间
        /// </summary>
        private string _date;
        public string Date
        {
            get { return _date; }
            set
            {
                _date = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 是否已登录
        /// </summary>
        private bool _isLogin;
        public bool IsLogin
        {
            get { return _isLogin; }
            set
            {
                _isLogin = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 系统已联机
        /// </summary>
        private bool _isOnlineMode;
        public bool IsOnlineMode
        {
            get { return _isOnlineMode; }
            set
            {
                _isOnlineMode = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 工作已加载
        /// </summary>
        private bool _isWorkLoaded;
        public bool IsWorkLoaded
        {
            get { return _isWorkLoaded; }
            set
            {
                _isWorkLoaded = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 工作已选择
        /// </summary>
        private bool _isWorkSelected;
        public bool IsWorkSelected
        {
            get { return _isWorkSelected; }
            set
            {
                _isWorkSelected = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 输入的密码
        /// </summary>
        private string _password;
        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 选择的工作名称
        /// </summary>
        private string _selectedWorkName;
        public string SelectedWorkName
        {
            get { return _selectedWorkName; }
            set
            {
                _selectedWorkName = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 输入的用户名
        /// </summary>
        private string _userName;
        public string UserName
        {
            get { return _userName; }
            set
            {
                _userName = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 工作的标题
        /// </summary>
        private string _workTitle;
        public string WorkTitle
        {
            get { return _workTitle; }
            set
            {
                _workTitle = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 程序标题栏显示的内容
        /// </summary>
        private string _title;
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 工作列表(工程目录)
        /// </summary>
        private BindingList<WorkListItemViewModel> _workList;
        public BindingList<WorkListItemViewModel> WorkList
        {
            get { return _workList; }
            set
            {
                _workList = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 是否显示工作列表
        /// </summary>
        private bool _isShowWorkList;
        public bool IsShowWorkList
        {
            get { return _isShowWorkList; }
            set
            {
                _isShowWorkList = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Windows系统信息
        /// </summary>

        //private ECWMI _WindowsSystemInfo;
        public ECWMI WindowsSystemInfo { get; set; } = new ECWMI();

        /// <summary>
        /// 系统TCP服务器是否开启
        /// </summary>
        private bool _isSystemTCPOpened=false;

        public bool IsSystemTCPOpened
        {
            get { return _isSystemTCPOpened; }
            set
            {
                _isSystemTCPOpened = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 是否警报提示
        /// </summary>
        private bool _isAlarm;

        public bool IsAlarm
        {
            get { return _isAlarm; }
            set
            {
                _isAlarm = value;
                RaisePropertyChanged();
            }
        }

        #endregion 属性

    }
}