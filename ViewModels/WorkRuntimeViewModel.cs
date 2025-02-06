using Cognex.VisionPro;
using Cognex.VisionPro.ToolBlock;
using ExCSS;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.DwayneNeed.Win32;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using ViDi2;
using VPDLFramework.Models;
using static VPDLFramework.Models.ECNativeModeCommand;

namespace VPDLFramework.ViewModels
{
    public class WorkRuntimeViewModel:ViewModelBase
    {
		public WorkRuntimeViewModel() 
		{
            WorkStreamsRecipe = new BindingList<WorkStreamRecipesViewModel>();
            Results = new BindingList<ECWorkStreamOrGroupResult>();
            ResultsChart = new BindingList<ECWorkStreamOrGroupResultChart>();
            _orignalList = new BindingList<ECWorkStreamOrGroupResult>();
            BindCmd();
            InitialParametersOptionList();
            RegisterMessenger();
            Messenger.Default.Send<string>("", ECMessengerManager.MainViewModelMessengerKeys.AskWorkName);
        }

        #region 字段
        /// <summary>
        /// 工作信息
        /// </summary>
        private ECWorkInfo _workInfo;

        /// <summary>
        /// 工作名
        /// </summary>
        private string _workName;

        /// <summary>
        /// 运行状态准备完成
        /// </summary>
        private bool _isReady=false;

        /// <summary>
        /// 监控定时器
        /// </summary>
        private System.Timers.Timer _timer;

        /// <summary>
        /// 缩放结果显示时暂存的原始结果列表
        /// </summary>
        private BindingList<ECWorkStreamOrGroupResult> _orignalList;

        /// <summary>
        /// 暂时保存图像显示行数
        /// </summary>
        private int _tmpRows = 1;

        /// <summary>
        /// 暂时保存图像显示列数
        /// </summary>
        private int _tmpCols = 1;

        /// <summary>
        /// 工作日志类型
        /// </summary>
        private enum WorkLogType
        { 
            TCP,
            IO,
            FFP,
            Stream,
            Group,
            Script,
        }

        /// <summary>
        /// 内触发线程
        /// </summary>
        private Thread _internalTriggerThread;

        /// <summary>
        /// 实时显示线程
        /// </summary>
        private System.Timers.Timer _liveModeTimer;

        /// <summary>
        /// 内触发线程使能
        /// </summary>
        private bool _isEnableInternalTriggerThread;

        /// <summary>
        /// 数据库锁
        /// </summary>
        private Mutex _workLogMutex = new Mutex();

        /// <summary>
        /// 第三方板卡
        /// </summary>
        private ECThirdCard _thirdCard;

        /// <summary>
        /// 启用第三方板卡
        /// </summary>
        private bool _enableThirdCard=false;

        /// <summary>
        /// 内触发方法正在运行
        /// </summary>
        private bool _isInternalTriggerMethodRunning=false;

        /// <summary>
        /// 图像源触发延迟
        /// </summary>
        private Dictionary<string,int> _imageSourcesTriggerDelay;

        /// <summary>
        /// 工作日志消息等待队列
        /// </summary>
        private List<object[]> _workLogWaitQueue = new List<object[]>();

        #endregion

        #region 命令
        /// <summary>
        /// 缩放工作流图像窗口
        /// </summary>
        public RelayCommand<object> CmdZoomResultItem { get; set; }

        /// <summary>
        /// 运行工作流
        /// </summary>
        public RelayCommand<string> CmdRunWorkStream { get; set; }

        /// <summary>
        /// 显示生产数据
        /// </summary>
        public RelayCommand CmdShowProductData { get; set; }

        /// <summary>
        /// 显示工作日志
        /// </summary>
        public RelayCommand CmdShowWorkLog { get; set; }

        /// <summary>
        /// 保存布局配置
        /// </summary>
        public RelayCommand CmdSaveLayoutConfig { get; set; }

        /// <summary>
        /// 改变行数
        /// </summary>
        public RelayCommand<int> CmdChangeRows { get; set; }

        /// <summary>
        /// 改变列数
        /// </summary>
        public RelayCommand<int> CmdChangeColumns { get; set; }

        /// <summary>
        /// 保存工作流ToolBlock
        /// </summary>
        public RelayCommand<string> CmdSaveWorkStreamTB { get; set; }

        /// <summary>
        /// 保存工作流组ToolBlock
        /// </summary>
        public RelayCommand<string> CmdSaveGroupTB { get; set; }

        /// <summary>
        /// 保存图像源ToolBlock
        /// </summary>
        public RelayCommand<string> CmdSaveImageSourceConfig { get; set; }

        /// <summary>
        /// 工作流实时显示
        /// </summary>
        public RelayCommand<string> CmdLiveMode { get; set; }

        /// <summary>
        /// 选择图像文件
        /// </summary>
        public RelayCommand<string> CmdSelectImageFile { get; set; }

        /// <summary>
        /// 保存工作流参数配置
        /// </summary>
        public RelayCommand<string> CmdSaveWorkStreamConfig { get; set; }
        #endregion

        #region 方法

        #region 工作加载
        /// <summary>
        /// 绑定命令
        /// </summary>
        private void BindCmd()
        {
            CmdZoomResultItem = new RelayCommand<object>(ZoomResultItem);
            CmdRunWorkStream = new RelayCommand<string>(RunWorkStream);
            CmdShowProductData = new RelayCommand(ShowProductData);
            CmdShowWorkLog=new RelayCommand(ShowWorkLog);
            CmdSaveLayoutConfig = new RelayCommand(SaveLayoutConfig);
            CmdChangeRows = new RelayCommand<int>(ChangeRows);
            CmdChangeColumns = new RelayCommand<int>(ChangeColumns);
            CmdSaveWorkStreamTB = new RelayCommand<string>(SaveWorkStreamTB);
            CmdSaveGroupTB = new RelayCommand<string>(SaveGroupTB);
            CmdSaveImageSourceConfig = new RelayCommand<string>(SaveImageSourceConfig);
            CmdLiveMode = new RelayCommand<string>(LiveMode);
            CmdSelectImageFile = new RelayCommand<string>(SelectImageFile);
            CmdSaveWorkStreamConfig = new RelayCommand<string>(SaveWorkStreamConfig);
        }

        /// <summary>
        /// 缩放工作流
        /// </summary>
        /// <param name="obj"></param>
        private void ZoomResultItem(object obj)
        {
            ECWorkStreamOrGroupResult result = obj as ECWorkStreamOrGroupResult;
            if (result == null) return;
            
            // 正在放大状态
            if (IsZooming)
            {
                // 恢复显示布局
                LayoutInfo.DisplayRows = _tmpRows;
                LayoutInfo.DisplayColumns = _tmpCols;

                IsZooming = false;
            }
            else
            {
                DispatcherHelper.UIDispatcher.Invoke(() =>
                {
                    // 清空原始列表
                    ZoomResults = new BindingList<ECWorkStreamOrGroupResult>();

                    // 将结果列表赋值给放大结果列表
                    ZoomResults.Add(result);
                });
                

                IsZooming = true;

                // 保存当前显示布局
                _tmpRows =LayoutInfo.DisplayRows;
                _tmpCols=LayoutInfo.DisplayColumns;

                LayoutInfo.DisplayRows = 1;
                LayoutInfo.DisplayColumns= 1;
            }               
        }

        /// <summary>
        /// 注册订阅的消息
        /// </summary>
        private void RegisterMessenger()
        {
            Messenger.Default.Register<ECWorkInfo>(this, ECMessengerManager.MainViewModelMessengerKeys.LoadWorkChanged, OnLoadWorkChanged);
            Messenger.Default.Register<string>(this, ECMessengerManager.MainViewModelMessengerKeys.UnLoadWork, OnUnLoadWork);
            Messenger.Default.Register<string>(this, ECMessengerManager.MainViewModelMessengerKeys.SystemOnline, OnSystemOnline);
            Messenger.Default.Register<string>(this, ECMessengerManager.MainViewModelMessengerKeys.SystemOffline, OnSystemOffline);
        }

        /// <summary>
        /// 发送TCP消息
        /// </summary>
        /// <param name="obj"></param>
        private void OnSendTCPMessage(KeyValuePair<string, string> obj)
        {
            TCPSendMessage(obj.Key, obj.Value);
        }

        /// <summary>
        /// 系统脱机
        /// </summary>
        /// <param name="obj"></param>
        private void OnSystemOffline(string obj)
        {
            IsSystemOnline = false;
        }

        /// <summary>
        /// 系统联机
        /// </summary>
        /// <param name="obj"></param>
        private void OnSystemOnline(string obj)
        {
            ResetLiveMode();
            IsSystemOnline = true;
        }

        /// <summary>
        /// 注册第三方板卡消息
        /// </summary>
        private void RegisterThirdCardMessenger()
        {
            Messenger.Default.Register<KeyValuePair<string, string>>(this, ECMessengerManager.ThirdCardMessageKeys.SendTCPMessage, OnSendTCPMessage);
        }

        /// <summary>
        /// 注销第三方板卡消息
        /// </summary>
        private void UnRegisterThirdCardMessenger()
        {
            Messenger.Default.Unregister<KeyValuePair<string, string>>(this, ECMessengerManager.ThirdCardMessageKeys.SendTCPMessage, OnSendTCPMessage);
        }

        /// <summary>
        /// 注册通讯板卡消息
        /// </summary>
        private void RegisterCommCardMessenger()
        {
            // 通讯板卡消息
            Messenger.Default.Register<int>(this, ECMessengerManager.CommCardMessengerKeys.FFPTriggerStream, OnFfpTriggerStream);
            Messenger.Default.Register<int>(this, ECMessengerManager.CommCardMessengerKeys.IOTriggerStream, OnIOTriggerStream);
        }

        /// <summary>
        /// 注销通讯板卡消息
        /// </summary>
        private void UnregisterCommCardMessenger()
        {
            // 通讯板卡消息
            Messenger.Default.Register<int>(this, ECMessengerManager.CommCardMessengerKeys.FFPTriggerStream, OnFfpTriggerStream);
            Messenger.Default.Register<int>(this, ECMessengerManager.CommCardMessengerKeys.IOTriggerStream, OnIOTriggerStream);
        }

        /// <summary>
        /// 注册本地指令消息
        /// </summary>
        private void RegisterNativeModeMessenger()
        {
            Messenger.Default.Register<string>(this, ECMessengerManager.MainViewModelMessengerKeys.NativeModeMsg, ExecuteMainViewModelNativeModeMsg);
        }

        /// <summary>
        /// 取消本地指令消息注册
        /// </summary>
        private void UnregisterNativeModeMessenger()
        {
            Messenger.Default.Unregister<string>(this, ECMessengerManager.MainViewModelMessengerKeys.NativeModeMsg, ExecuteMainViewModelNativeModeMsg);
        }

        /// <summary>
        /// 初始化监控定时器
        /// </summary>
        private void InitialTimer()
        {
            _timer = new System.Timers.Timer(1000);
            _timer.Elapsed += _timer_Elapsed;
            _timer.Start();
        }

        /// <summary>
        /// 初始化参数选项列表
        /// </summary>
        private void InitialParametersOptionList()
        {
            ImageRecordConstantsBindableList = ECGeneric.GetConstantsBindableList<ECWorkOptionManager.ImageRecordConstants>();
            ImageRecordConditionConstantsBindableList=ECGeneric.GetConstantsBindableList<ECWorkOptionManager.ImageRecordConditionConstants>();
        }

        /// <summary>
        /// 卸载工作
        /// </summary>
        /// <param name="obj"></param>
        private void OnUnLoadWork(string obj)
        {
            try
            {
                Messenger.Default.Send<int>(-1, ECMessengerManager.MainViewModelMessengerKeys.UnLoadWork);
                if (!_isReady) return;
                _isReady = false;
                _isEnableInternalTriggerThread = false;
                StopLiveModeTimer();
                _timer?.Stop();
                _timer?.Dispose();
                ECDialogManager.LoadWithAnimation(new Action(() =>
                {
                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        // 取消第三方板卡消息注册
                        UnRegisterThirdCardMessenger();

                        // 取消本地指令消息注册
                        UnregisterNativeModeMessenger();

                        // 取消通讯板卡消息注册
                        UnregisterCommCardMessenger();

                        // 关闭第三方板卡
                        _thirdCard?.Dispose();

                        // 释放工作流
                        foreach (ECWorkStream item in WorkStreams)
                            item.Dispose();
                        WorkStreams.Clear();

                        // 释放工作流组
                        foreach (ECWorkStreamsGroup item in WorkGroups)
                            item.Dispose();
                        WorkGroups.Clear();

                        // 释放图像源
                        foreach (ECWorkImageSource item in WorkImageSources)
                            item.Dispose();
                        WorkImageSources.Clear();

                        // 释放TCP设备
                        foreach (ECTCPDevice item in WorkTCPDevices)
                            item.Dispose();
                        WorkTCPDevices.Clear();

                        if(ECDLEnvironment.IsEnable)
                            ECDLEnvironment.ClearWorkspaces();

                        // 释放TCP监控
                        TcpMonitor = null;

                        Messenger.Default.Send<string>("", ECMessengerManager.WorkRuntimeViewModelMessengerKeys.Dispose);
                        GC.Collect();
                    });

                }), ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.Unloading));
            }
            catch(System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace+ex.Message,NLog.LogLevel.Error);
            }
        }

        /// <summary>
        /// 收到回复的工作名称,加载工作配置
        /// </summary>
        /// <param name="obj"></param>
        private void OnLoadWorkChanged(ECWorkInfo obj)
        {
            if(obj == null) return; 
            try
            {
                _isReady = false;
                _workInfo= obj;
                _workName = obj.WorkName;

                DispatcherHelper.UIDispatcher.Invoke(new Action(() =>
                {
                    WorkStreamsRecipe.Clear();
                    Results.Clear();
                    Results = new BindingList<ECWorkStreamOrGroupResult>();
                    ResultsChart = new BindingList<ECWorkStreamOrGroupResultChart>();
                }));
                
                LoadWorkSettings();
            }
            catch (System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, NLog.LogLevel.Error);
            }
        }

        /// <summary>
        /// 加载工作配置
        /// </summary>
        private void LoadWorkSettings()
        {
            ECDialogManager.LoadWithAnimation(new Action(() =>
            {
                try
                {
                    bool bImageSource = true;
                    bool bDLWorkspace= true;
                    bool bTCP= true;
                    bool bWorkStream= true;
                    bool bWorkGroup= true;
                    bool bCommCard= true;

                    ECLog.WriteToLog(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.LoadingImageSourceSetup), LogLevel.Trace);

                    bImageSource = LoadImageSourceSettings();

                    if (ECDLEnvironment.IsEnable)
                    {
                        ECLog.WriteToLog(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.LoadingVPDLSetup), LogLevel.Trace);
                        bDLWorkspace = LoadDLWorkspaceSettings();
                    }

                    ECLog.WriteToLog(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.LoadingTCPIPSetup), LogLevel.Trace);
                    bTCP = LoadWorkTCPSettings();

                    ECLog.WriteToLog(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.LoadingWorkStreamSetup), LogLevel.Trace);
                    bWorkStream = LoadWorkStreamSettings();

                    ECLog.WriteToLog(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.LoadingWorkGroupSetup), LogLevel.Trace);
                    bWorkGroup = LoadWorkGroupSettings();

                    ECLog.WriteToLog(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.LoadingCommCardSetup), LogLevel.Trace);
                    bCommCard = LoadWorkCommCardSettings();

                    InitialTimer();
                    InitialInternalTriggerThread();

                    // 注册本地指令消息
                    RegisterNativeModeMessenger();

                    // 注册通讯板卡消息
                    RegisterCommCardMessenger();

                    // 注册第三方板卡消息
                    RegisterThirdCardMessenger();

                    // 检测填充模式
                    CheckLayoutConfig();

                    // 检测是否启用第三方板卡
                    _enableThirdCard = CheckThirdCardEnable();

                    if (bImageSource && bDLWorkspace && bTCP && bWorkStream && bWorkGroup && bCommCard)
                    {
                        // 发出作业加载完成信息
                        Messenger.Default.Send<int>(_workInfo.WorkID, ECMessengerManager.MainViewModelMessengerKeys.WorkLoaded);
                        _isReady = true;
                        ECLog.WriteToLog(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.LoadFinished), LogLevel.Trace);
                    }
                    else
                    {
                        _isReady = false;
                        ECLog.WriteToLog(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.LoadFailed), LogLevel.Trace);
                    }

                    StartLiveModeTimer();

                }
                catch (System.Exception ex)
                {
                    ECLog.WriteToLog(ex.StackTrace + ex.Message, LogLevel.Error);                    
                    _isReady = false;
                }

            }), ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.Loading));
        }

        /// <summary>
        /// 加载图像源配置
        /// </summary>
        private bool LoadImageSourceSettings()
        {
            try
            {
                WorkImageSources = new BindingList<ECWorkImageSource>();
                ImageSourceNames= new BindingList<string>();
                Dictionary<string, bool> cameras = new Dictionary<string, bool>();

                string folder = $"{ECFileConstantsManager.RootFolder}\\{_workName}\\{ECFileConstantsManager.ImageSourceFolderName}";
                if (Directory.Exists(folder))
                {
                    string[] folders = Directory.GetDirectories(folder);
                    foreach (string item in folders)
                    {
                        try
                        {
                            if (File.Exists($"{item}\\{ECFileConstantsManager.ImageSourceConfigName}"))
                            {
                                ECWorkImageSource imageSource = new ECWorkImageSource(_workName, new DirectoryInfo(item).Name);
                                imageSource.LoadConfig();
                                if (!string.IsNullOrEmpty(imageSource.CameraSerialNumName) &&imageSource.CameraSerialNumName != ":")
                                    if(!cameras.ContainsKey(imageSource.CameraSerialNumName))
                                        cameras.Add(imageSource.CameraSerialNumName, true);
                                else if (imageSource.ImageSourceInfo.IsUseCam && (imageSource.CameraSerialNumName == ":"|| string.IsNullOrEmpty(imageSource.CameraSerialNumName)))
                                    ECLog.WriteToLog($"Image Source \"{imageSource.ImageSourceInfo.ImageSourceName}\" Loaded Failed", NLog.LogLevel.Error);

                                DispatcherHelper.UIDispatcher.Invoke(() =>
                                {
                                    WorkImageSources.Add(imageSource);
                                    ImageSourceNames.Add(imageSource.ImageSourceInfo.ImageSourceName);
                                });
                            }
                        }
                        catch(System.Exception ex)
                        {
                            ECLog.WriteToLog(ex.StackTrace + ex.Message, NLog.LogLevel.Error);
                        }
                    }
                    
                }

                _imageSourcesTriggerDelay = new Dictionary<string, int>();
                foreach(var imageSource in WorkImageSources)
                {
                    imageSource.Completed += imageSource_Completed;
                    if (imageSource.ToolBlock.Inputs.Contains("TriggerDelay"))
                        _imageSourcesTriggerDelay.Add(imageSource.ImageSourceInfo.ImageSourceName, (int)imageSource.ToolBlock.Inputs["TriggerDelay"].Value);
                }

                if(cameras.Count>0)
                {
                    FrameGrabber=new ECFrameGrabber(cameras);
                }
                return true;
            }
            catch(System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace+ex.Message, NLog.LogLevel.Error);
                return false;
            }
        }

        /// <summary>
        /// 加载DL工作区配置
        /// </summary>
        private bool LoadDLWorkspaceSettings()
        {
            try
            {
                string folder = $"{ECFileConstantsManager.RootFolder}\\{_workName}\\{ECFileConstantsManager.WorkspaceFolderName}";
                if (Directory.Exists(folder))
                {
                    string[] files = Directory.GetFiles(folder);
                    foreach (string file in files)
                    {
                        FileInfo fileInfo = new FileInfo(file);
                        if ((fileInfo.Extension == ".vrws") && !ECDLEnvironment.Control.Workspaces.Names.Contains(fileInfo.Name))
                        {
                            IWorkspace workspace = ECDLEnvironment.Control.Workspaces.Add(fileInfo.Name.Replace(".vrws",""), file);  
                        }
                    }
                }
                return true;
            }
            catch (System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, LogLevel.Error);
                return false;
            }
        }

        /// <summary>
        /// 加载TCP配置
        /// </summary>
        private bool LoadWorkTCPSettings()
        {
            try
            {
                WorkTCPDevices = new BindingList<ECTCPDevice>();

                string folder = $"{ECFileConstantsManager.RootFolder}\\{_workName}\\{ECFileConstantsManager.TCPConfigFolderName}";
                if (Directory.Exists(folder))
                {
                    string[] folders = Directory.GetDirectories(folder);
                    foreach (string item in folders)
                    {
                        try
                        {
                            if (File.Exists($"{item}\\{ECFileConstantsManager.TCPConfigName}"))
                            {
                                ECTCPDevice tcpDevice = new ECTCPDevice(_workName, new DirectoryInfo(item).Name);
                                tcpDevice.LoadConfig();
                                tcpDevice.OpenTCP();
                                tcpDevice.DataReceived += TcpDevice_DataReceived;
                                DispatcherHelper.UIDispatcher.Invoke(() =>
                                {
                                    WorkTCPDevices.Add(tcpDevice);
                                });

                            }
                        }
                        catch (System.Exception ex)
                        {
                            ECLog.WriteToLog(ex.StackTrace + ex.Message, NLog.LogLevel.Error);
                        }
                    }
                }
                // 创建TCP监控对象
                if (WorkTCPDevices.Count > 0)
                {
                    TcpMonitor=new ECTCPMonitor(WorkTCPDevices.ToList());
                }
                return true;
            }
            catch (System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, LogLevel.Error);
                return false;
            }
        }

        /// <summary>
        /// 加载IO配置
        /// </summary>
        private bool LoadWorkCommCardSettings()
        {
            try
            {
                BindingList<ECCommCardInputLineInfo> InputItems = new BindingList<ECCommCardInputLineInfo>();
                BindingList<ECCommCardOutputLineInfo> OutputItems = new BindingList<ECCommCardOutputLineInfo>();

                string folder = $"{ECFileConstantsManager.RootFolder}\\{_workName}\\{ECFileConstantsManager.CommCardConfigFolderName}";
                string inputConfigPath = folder + @"\" + ECFileConstantsManager.IOInputConfigName;
                string outputConfigPath = folder + @"\" + ECFileConstantsManager.IOOutputConfigName;

                if (File.Exists(inputConfigPath))
                    InputItems = ECSerializer.LoadObjectFromJson<BindingList<ECCommCardInputLineInfo>>(inputConfigPath);
                if (File.Exists(outputConfigPath))
                    OutputItems = ECSerializer.LoadObjectFromJson<BindingList<ECCommCardOutputLineInfo>>(outputConfigPath);

                if (ECCommCard.Bank0!=null)
                {
                    ECCommCard.GetScriptMethod(_workName);
                    ECCommCard.FfpInputChanged += ECCommCard_FfpInputChanged;
                    if(ECCommCard.IOAccess)
                    {
                        bool succeed=  ECCommCard.RegisterInputOutputEvent(InputItems.ToList(), OutputItems.ToList());
                        if(!succeed) return false;
                    }
                   
                }
                return true;
            }
            catch (System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace+ex.Message, LogLevel.Error);
                return false;
            }
        }

        /// <summary>
        /// 加载TCP配置
        /// </summary>
        private bool LoadWorkGroupSettings()
        {
            try
            {
                WorkGroups = new BindingList<ECWorkStreamsGroup>();
                string folder = ECFileConstantsManager.RootFolder + @"\" + _workName + @"\" + ECFileConstantsManager.GroupsFolderName;
                if (Directory.Exists(folder))
                {
                    string[] folders = Directory.GetDirectories(folder);
                    foreach (string item in folders)
                    {
                        try
                        {
                            ECWorkStreamsGroup group = new ECWorkStreamsGroup(_workName, new DirectoryInfo(item).Name);
                            group.LoadConfig();
                            group.IsEnableDatabase = true;
                            group.Compeleted += Group_Compeleted;
                            DispatcherHelper.UIDispatcher.Invoke(() =>
                            {
                                WorkGroups.Add(group);
                                if (group.GroupInfo.IsVisibleInRuntime)
                                    Results.Add(group.ResultViewModel);
                                if (group.GroupInfo.ResultChartSeriesCount > 0)
                                    ResultsChart.Add(group.ResultChart);
                            });
                        }
                        catch (System.Exception ex)
                        {
                            ECLog.WriteToLog(ex.StackTrace + ex.Message, NLog.LogLevel.Error);
                        }
                    }
                }
                return true;
            }
            catch(System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace+ex.Message, LogLevel.Error);
                return false;
            }
        }

        /// <summary>
        /// 加载工作流配置
        /// </summary>
        private bool LoadWorkStreamSettings()
        {
            try
            {
                WorkStreams = new BindingList<ECWorkStream>();
                WorkStreamNames = new BindingList<string>();

                BindingList<ECWorkStream> tmpList=new BindingList<ECWorkStream>();

                string folder = $"{ECFileConstantsManager.RootFolder}\\{_workName}\\{ECFileConstantsManager.StreamsFolderName}";
                if (Directory.Exists(folder))
                {
                    string[] folders = Directory.GetDirectories(folder);
                    foreach (string item in folders)
                    {
                        try
                        {
                            if (File.Exists($"{item}\\{ECFileConstantsManager.StreamConfigName}"))
                            {
                                ECWorkStream stream = new ECWorkStream(_workName, new DirectoryInfo(item).Name);
                                stream.LoadConfig();
                                tmpList.Add(stream);
                            }
                        }
                        catch (System.Exception ex)
                        {
                            ECLog.WriteToLog(ex.StackTrace + ex.Message, NLog.LogLevel.Error);
                        }
                    }

                    // 按照ID顺序添加
                    for (int i = 0; i < tmpList.Count; i++)
                    {
                        foreach (var item in tmpList)
                        {
                            if (item.WorkStreamInfo.StreamID == i && !item.WorkStreamInfo.IsStreamDisable)
                            {
                                // 初始化多线程
                                if (item.WorkStreamInfo.IsEnableMultiThread)
                                {
                                    item.InitialMultThread();
                                    item.MultiThreadManager.ImageProcessStart += MultiThreadManager_ImageProcessStart;
                                }
                                item.IsEnableDatabase = true; // 连接数据库
                                item.Completed += Stream_Compeleted; // 订阅完成事件
                                DispatcherHelper.UIDispatcher.Invoke(() =>
                                {
                                    WorkStreams.Add(item);
                                    WorkStreamNames.Add(item.WorkStreamInfo.StreamName);
                                    WorkStreamsRecipe.Add(new WorkStreamRecipesViewModel(_workName, item));
                                    Results.Add(item.ResultViewModel);
                                    if (item.WorkStreamInfo.ResultChartSeriesCount > 0) ResultsChart.Add(item.ResultChart);
                                });

                                if (item.WorkStreamInfo.IsEnableInternalTrigger) _isEnableInternalTriggerThread = true;
                            }

                        }
                    }
                }
                return true;
            }
            catch(System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace+ex.Message,LogLevel.Error);
                return false;
            }
        }

        /// <summary>
        /// 工作流多线程开始处理图像
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MultiThreadManager_ImageProcessStart(object sender, KeyValuePair<int, int> e)
        {
            InsertLogData(WorkLogType.Stream, $"\"{(sender as ECWorkStreamMultiThreadManager).WorkStream.WorkStreamInfo.StreamName}\" 正在线程 {e.Value.ToString()} 处理触发序号为{e.Key.ToString()}的图片");
        }

        /// <summary>
        /// 检测是否使用第三方板卡
        /// </summary>
        /// <returns></returns>
        private bool CheckThirdCardEnable()
        {
            string jsonPath = ECFileConstantsManager.ProgramStartupConifgFolder + @"\" + ECFileConstantsManager.StartupConfigName;
            if (File.Exists(jsonPath))
            {
                ECStartupSettings _startupSettings = ECSerializer.LoadObjectFromJson<ECStartupSettings>(jsonPath);
                if (_startupSettings != null)
                {
                    if (_startupSettings.EnableThirdCard)
                    {
                        _thirdCard=new ECThirdCard(_workName);
                        _thirdCard.InputStateChanged += _thirdCard_InputStateChanged;
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 第三方板卡输入事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void _thirdCard_InputStateChanged(object sender, string e)
        {
            try
            {
                if (!string.IsNullOrEmpty(e))
                {
                    bool canExecute = true;
                    InsertLogData(WorkLogType.Script, $"{e}");

                    if(canExecute&&IsSystemOnline)
                        ExecuteTCPNativeModeCommand(e);
                }
            }
            catch (System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace+ex.Message,LogLevel.Error);
            }
        }

        /// <summary>
        /// Ffp输入事件更改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ECCommCard_FfpInputChanged(object sender, string e)
        {
            try
            {
                if (!string.IsNullOrEmpty(e))
                {
                    bool canExecute = true;
                    InsertLogData(WorkLogType.Script, $"{e}");

                    if (canExecute && IsSystemOnline)
                        ExecuteTCPNativeModeCommand(e);
                }
            }
            catch (System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, LogLevel.Error);
            }
        }

        /// <summary>
        /// 显示生产数据
        /// </summary>
        private void ShowProductData()
        {
            if (ProductDataViewModel == null) ProductDataViewModel = new WorkProductionDataViewModel(_workName);
            ProductDataViewModel.UpdateProductData();
        }

        /// <summary>
        /// 显示工作日志
        /// </summary>
        private void ShowWorkLog()
        {
            if(WorkLogViewModel == null) WorkLogViewModel=new WorkRuntimeLogViewModel(_workName);
            WorkLogViewModel.QueryAll();
        }

        /// <summary>
        /// 插入日志数据
        /// </summary>
        /// <param name="logType">日志类型</param>
        /// <param name="logContent">日志内容</param>
        //private void InsertLogData(WorkLogType logType, string logContent)
        //{
        //    try
        //    {
        //        // 数据
        //        string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff");
        //        string type = Enum.GetName(typeof(WorkLogType), logType);
        //        string content = logContent;

        //        // 打包成object[]
        //        object[] datas = new object[3];
        //        datas[0] = time;
        //        datas[1] = type;
        //        datas[2] = content;

        //        // 数据库文件
        //        string folder = ECFileConstantsManager.RootFolder + @"\" + _workName + @"\" +
        //             ECFileConstantsManager.DatabaseFolderName + @"\" + ECFileConstantsManager.WorkLogFolderName + @"\" + DateTime.Now.ToString("yyyy_MM_dd");
        //        string filePath = folder + @"\" + ECFileConstantsManager.LogDatabaseFileName;

        //        Task.Run(() =>
        //        {
        //            try
        //            {
        //                // 获取数据库锁
        //                _workLogMutex.WaitOne();

        //                // 添加数据到文件
        //                ECSQLiteDataManager.AddData(filePath, ECSQLiteDataManager.DataType.WorkLog, datas);
        //            }
        //            catch (System.Exception ex)
        //            {
        //                ECLog.WriteToLog(ex.StackTrace + ex.Message + "Write Data To Work Log Failed", LogLevel.Error);
        //            }
        //            finally { _workLogMutex.ReleaseMutex(); }
                
        //        });
        //    }
        //    catch (System.Exception ex)
        //    {
        //        ECLog.WriteToLog(ex.StackTrace + ex.Message, LogLevel.Error);
        //    }
        //}

        private void InsertLogData(WorkLogType logType, string logContent)
        {
            try
            {
                // 数据
                string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff");
                string type = Enum.GetName(typeof(WorkLogType), logType);
                string content = logContent;

                // 打包成object[]
                object[] datas = new object[3];
                datas[0] = time;
                datas[1] = type;
                datas[2] = content;

                // 数据库文件
                string folder = ECFileConstantsManager.RootFolder + @"\" + _workName + @"\" +
                     ECFileConstantsManager.DatabaseFolderName + @"\" + ECFileConstantsManager.WorkLogFolderName + @"\" + DateTime.Now.ToString("yyyy_MM_dd");
                string filePath = folder + @"\" + ECFileConstantsManager.LogDatabaseFileName;

                ECLog.WriteToLog(logContent,LogLevel.Info);

            }
            catch (System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, LogLevel.Error);
            }
        }

        /// <summary>
        /// 执行MainViewModel传来的本地化指令消息
        /// </summary>
        /// <param name="obj"></param>
        private void ExecuteMainViewModelNativeModeMsg(string obj)
        {
            ExecuteTCPNativeModeCommand(obj);
        }

        /// <summary>
        ///  本地指令触发工作流
        /// </summary>
        /// <param name="streamName"></param>
        private void OnNativeModeTriggerStream(string streamName)
        {
            try
            {
                if(WorkStreamNames.Contains(streamName))
                    RunWorkStream(streamName);
            }
            catch(System.Exception ex) 
            {
                ECLog.WriteToLog(ex.StackTrace+ex.Message,NLog.LogLevel.Error);
            }
        }

        /// <summary>
        /// 本地指令触发多工作流
        /// </summary>
        /// <param name="obj"></param>
        private void OnNativeModeTriggerMultiStream(List<string> obj)
        {
            try
            {
                bool canExecute = true;

                // 判断是否所有工作流都存在
                foreach (string s in obj)
                {
                    if (!WorkStreamNames.Contains(s))
                    {
                        canExecute = false;
                        return;
                    }
                }

                // 执行所有工作流
                if (canExecute)
                {
                    foreach (string s in obj)
                    {
                        Task.Run(() =>
                        {
                            RunWorkStream(s);
                        }); 
                    }
                }
            }
            catch (System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, NLog.LogLevel.Error);
            }
        }

        /// <summary>
        /// 本地指令切换配置
        /// </summary>
        /// <param name="obj"></param>
        private void OnNativeModeLoadRecipe(KeyValuePair<string, string> obj)
        {
            try
            {
                ECWorkStream mStream = WorkStreams.Where(t => t.WorkStreamInfo.StreamName == obj.Key)?.First();
                if (mStream != null && mStream.Recipes.Where(r => r.RecipeName == obj.Value).First() != null)
                {
                    bool ack= mStream.LoadRecipe(obj.Value);
                    Messenger.Default.Send<bool>(ack, ECMessengerManager.ECNativeModeCommandMessengerKeys.LoadRecipeAck);
                }
            }
            catch(System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace+ex.Message,NLog.LogLevel.Error);
            }
        }

        /// <summary>
        /// 本地指令传入用户数据
        /// </summary>
        /// <param name="pair"></param>
        private void OnNativeModeSetUserData(KeyValuePair<string, string> pair)
        {
            try
            {
                ECWorkStream mStream = WorkStreams.Where(t => t.WorkStreamInfo.StreamName == pair.Key)?.First();
                if (mStream != null)
                {
                    bool ack = mStream.SetUserData(pair.Value);
                    Messenger.Default.Send<bool>(ack, ECMessengerManager.ECNativeModeCommandMessengerKeys.SetUserDataAck);
                }
            }
            catch (System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, NLog.LogLevel.Error);
            }
        }

        /// <summary>
        /// 本地指令设置工作流自触发启停
        /// </summary>
        /// <param name="piar"></param>
        private void OnNativeModeSetInternalTrigger(KeyValuePair<string, bool> pair)
        {
            try
            {
                ECWorkStream mStream = WorkStreams.Where(t => t.WorkStreamInfo.StreamName == pair.Key)?.First();
                if (mStream != null)
                {
                    mStream.WorkStreamInfo.IsInternalTriggerBegin = pair.Value;
                }
            }
            catch (System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, NLog.LogLevel.Error);
            }
        }

        /// <summary>
        /// 本地指令设置工作流曝光时间
        /// </summary>
        /// <param name="piar"></param>
        private void OnNativeModeSetExposureTime(string streamName, double exposureTime)
        {
            try
            {
                if (!WorkStreamNames.Contains(streamName)) return;

                ECWorkStream mStream = WorkStreams.Where(t => t.WorkStreamInfo.StreamName == streamName)?.First();

                if (mStream != null && mStream.WorkStreamInfo.ImageSourceName != null && ImageSourceNames.Contains(mStream.WorkStreamInfo.ImageSourceName))
                {
                    ECWorkImageSource mImageSource = WorkImageSources.Where(t => t.ImageSourceInfo.ImageSourceName == mStream.WorkStreamInfo.ImageSourceName)?.First();
                    if (mImageSource != null)
                        mImageSource.SetExposure(exposureTime);
                }
            }
            catch (System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, NLog.LogLevel.Error);
            }
        }

        /// <summary>
        /// 本地指令设置工作流曝光时间
        /// </summary>
        /// <param name="piar"></param>
        private void OnNativeModeSetStreamImageSource(string streamName, string imageSourceName)
        {
            try
            {
                if(!WorkStreamNames.Contains(streamName) || !ImageSourceNames.Contains(imageSourceName)) return;

                ECWorkStream mStream = WorkStreams.Where(t => t.WorkStreamInfo.StreamName == streamName)?.First();

                if (mStream != null)
                    mStream.WorkStreamInfo.ImageSourceName = imageSourceName;
            }
            catch (System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, NLog.LogLevel.Error);
            }
        }

        /// <summary>
        /// 本地指令设置曝光时间并触发工作流
        /// </summary>
        /// <param name="piar"></param>
        private void OnNativeModeTriggerWithExposureTime(string streamName,double exposureTime)
        {
            try
            {
                if (!WorkStreamNames.Contains(streamName)) return;
                
                ECWorkStream mStream = WorkStreams.Where(t => t.WorkStreamInfo.StreamName == streamName)?.First();

                if (mStream != null&&mStream.WorkStreamInfo.ImageSourceName!=null&& ImageSourceNames.Contains(mStream.WorkStreamInfo.ImageSourceName))
                {
                    ECWorkImageSource mImageSource = WorkImageSources.Where(t => t.ImageSourceInfo.ImageSourceName==mStream.WorkStreamInfo.ImageSourceName)?.First();
                    if (mImageSource!=null)
                    {
                        if (mImageSource.SetExposure(exposureTime))
                            RunWorkStream(mStream.WorkStreamInfo.StreamName);
                    }
                    
                }
            }
            catch (System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, NLog.LogLevel.Error);
            }
        }

        /// <summary>
        /// 本地指令设置用户数据并触发工作流
        /// </summary>
        /// <param name="piar"></param>
        private void OnNativeModeTriggerWithUserData(string streamName, string userData)
        {
            try
            {
                if (!WorkStreamNames.Contains(streamName)) return;

                ECWorkStream mStream = WorkStreams.Where(t => t.WorkStreamInfo.StreamName == streamName)?.First();

                if (mStream != null)
                {
                    if (mStream.SetUserData(userData))
                        RunWorkStream(mStream.WorkStreamInfo.StreamName);
                }
            }
            catch (System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, NLog.LogLevel.Error);
            }
        }

        /// <summary>
        /// 本地指令设置用户数据、曝光时间并触发工作流
        /// </summary>
        /// <param name="piar"></param>
        private void OnNativeModeTriggerWithExposureTimeAndUserData(string streamName,double exposureTime,string userData)
        {
            try
            {
                if (!WorkStreamNames.Contains(streamName)) return;

                ECWorkStream mStream = WorkStreams.Where(t => t.WorkStreamInfo.StreamName == streamName)?.First();

                if (mStream != null && mStream.WorkStreamInfo.ImageSourceName != null && ImageSourceNames.Contains(mStream.WorkStreamInfo.ImageSourceName))
                {
                    ECWorkImageSource mImageSource = WorkImageSources.Where(t => t.ImageSourceInfo.ImageSourceName == mStream.WorkStreamInfo.ImageSourceName)?.First();
                    if (mImageSource != null)
                    {
                        if (mImageSource.SetExposure(exposureTime)&& mStream.SetUserData(userData))
                        {
                            RunWorkStream(mStream.WorkStreamInfo.StreamName);
                        }
                            
                    }

                }
            }
            catch (System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, NLog.LogLevel.Error);
            }
        }

        /// <summary>
        /// 本地指令设置图像源并触发工作流
        /// </summary>
        /// <param name="streamName"></param>
        /// <param name="imageSourceName"></param>
        private void OnNativeModeTriggerWithImageSource(string streamName, string imageSourceName)
        {
            try
            {
                if (!WorkStreamNames.Contains(streamName)||!ImageSourceNames.Contains(imageSourceName)) return;

                ECWorkStream mStream = WorkStreams.Where(t => t.WorkStreamInfo.StreamName == streamName)?.First();

                if (mStream != null)
                {
                    mStream.WorkStreamInfo.ImageSourceName = imageSourceName;
                    RunWorkStream(mStream.WorkStreamInfo.StreamName);
                }
            }
            catch (System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, NLog.LogLevel.Error);
            }
        }

        /// <summary>
        /// 本地指令触发图像源
        /// </summary>
        /// <param name="imageSourceName"></param>
        private void OnNativeModeTriggerImageSource(string imageSourceName)
        {
            try
            {
                if (!ImageSourceNames.Contains(imageSourceName)) return;

                ECWorkImageSource imageSource = WorkImageSources.Where(t => t.ImageSourceInfo.ImageSourceName == imageSourceName)?.First();

                if (imageSource != null)
                    imageSource.GetImage();
            }
            catch (System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, NLog.LogLevel.Error);
            }
        }

        /// <summary>
        /// 本地指令设置曝光时间并触发图像源
        /// </summary>
        /// <param name="imageSourceName"></param>
        /// <param name="exposureTime"></param>
        private void OnNativeModeTriggerImageSourceWithExposureTime(string imageSourceName, double exposureTime)
        {
            try
            {
                if (!ImageSourceNames.Contains(imageSourceName)) return;

                ECWorkImageSource imageSource = WorkImageSources.Where(t => t.ImageSourceInfo.ImageSourceName == imageSourceName)?.First();

                if (imageSource != null)
                {
                    if (imageSource.SetExposure(exposureTime))
                        imageSource.GetImage();
                }
            }
            catch (System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, NLog.LogLevel.Error);
            }
        }

        /// <summary>
        /// 定时器事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (_isReady)
            {
                FrameGrabber?.UpdateGrabbersStatus();
                TcpMonitor?.DetectTCPDevicesStatus();
                UpdateMonitorUI(FrameGrabber?.MonitoredCamerasStatus,TcpMonitor?.TCPDevicesStatus);
            }
        }

        /// <summary>
        /// 刷新监控界面
        /// </summary>
        private void UpdateMonitorUI(Dictionary<string, bool> camerasStatus, Dictionary<string, bool> tcpsStatus)
        {
            DispatcherHelper.UIDispatcher.Invoke(() =>
            {
                if (camerasStatus != null)
                    foreach (ECWorkImageSource item in WorkImageSources)
                    {
                        if (item.CameraSerialNumName !=null&&camerasStatus.ContainsKey(item.CameraSerialNumName))
                        {
                            item.ImageSourceInfo.IsOnline = camerasStatus[item.CameraSerialNumName];
                            if(!item.ImageSourceInfo.IsOnline)
                                ECLog.WriteToLog(item.ImageSourceInfo.ImageSourceName+":"+item.CameraSerialNumName+"Lost Connection",LogLevel.Warn);
                        }
                            
                    }
                if (tcpsStatus != null)
                    foreach (ECTCPDevice item in WorkTCPDevices)
                    {
                        if (tcpsStatus.ContainsKey(item.TCPDeviceInfo.TCPDeviceName))
                            item.TCPDeviceInfo.Status = tcpsStatus[item.TCPDeviceInfo.TCPDeviceName];
                    }
            });

        }

        /// <summary>
        /// 初始化内触发线程
        /// </summary>
        private void InitialInternalTriggerThread()
        {
            if(_isEnableInternalTriggerThread)
            {
                _internalTriggerThread = new Thread(InternalTriggerMethod);
                _internalTriggerThread.Start();
            }
        }

        /// <summary>
        /// 内触发方法
        /// </summary>
        private void InternalTriggerMethod()
        {
            Dictionary<string, bool> streamsPairs = WorkStreams.ToDictionary(w => w.WorkStreamInfo.StreamName, w => w.WorkStreamInfo.IsEnableInternalTrigger);
            
            // 工作流Ready状态，工作流自身的IsRunning有可能在内触发线程的下次扫描时还未刷新导致重复触发，此字典的状态在ECRunTimeViewModel中刷新，可避免此问题
            StreamsReadyStatus = new Dictionary<string, bool>();
            foreach (string streamName in streamsPairs.Keys)
                StreamsReadyStatus.Add(streamName, true);

            // 图像源Ready状态，图像源自身的IsRunning有可能在内触发线程的下次扫描时还未刷新导致重复触发，此字典的状态在ECRunTimeViewModel中刷新，可避免此问题
            ImageSourceReadyStatus = new Dictionary<string, bool>();
            foreach (ECWorkImageSource imageSource in WorkImageSources)
            {
                ImageSourceReadyStatus.Add(imageSource.ImageSourceInfo.ImageSourceName, true);
            }

            while (_isEnableInternalTriggerThread)
            {
                if (_isReady&&IsSystemOnline)
                {
                    foreach (var streamPair in streamsPairs)
                    {
                        if (streamPair.Value)
                        {
                            ECWorkStream stream = WorkStreams.Where(t => t.WorkStreamInfo.StreamName == streamPair.Key)?.First();

                            // 获取图像源名称
                            string imageSourceName = stream.WorkStreamInfo.ImageSourceName;

                            // 判断内触发开始启用)
                            if (stream.WorkStreamInfo.IsInternalTriggerBegin)
                            {
                                if (!stream.WorkStreamInfo.IsAsyncMode && StreamsReadyStatus[streamPair.Key])
                                    RunWorkStream(stream.WorkStreamInfo.StreamName);
                                else if (stream.WorkStreamInfo.IsAsyncMode && ImageSourceReadyStatus[imageSourceName])
                                    RunWorkStream(stream.WorkStreamInfo.StreamName);
                            }
                        }
                    }
                }
                else
                    Thread.Sleep(100);
            }
        }

        /// <summary>
        /// 初始化内触发线程
        /// </summary>
        private void StartLiveModeTimer()
        {
            StreamsLiveModeRunning = new Dictionary<string, bool>();
            foreach(ECWorkStream stream in WorkStreams)
                StreamsLiveModeRunning.Add(stream.WorkStreamInfo.StreamName, false);
            _liveModeTimer = new System.Timers.Timer(50);
            _liveModeTimer.Elapsed += _liveModeTimer_Elapsed;
            _liveModeTimer.Start();
        }

        /// <summary>
        /// 停止实时显示定时器
        /// </summary>
        private void StopLiveModeTimer()
        {
            ResetLiveMode();
            _liveModeTimer?.Stop();
            _liveModeTimer.Dispose();
        }

        /// <summary>
        /// 实现显示定时器事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _liveModeTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (IsSystemOnline) return;

            foreach (ECWorkStream stream in WorkStreams)
            {
                if (stream.ResultViewModel.IsLiveMode && !StreamsLiveModeRunning[stream.WorkStreamInfo.StreamName])
                {
                    RunLiveModeWorkStream(stream.WorkStreamInfo.StreamName);
                }
            }
        }

        /// <summary>
        /// 计算显示行数
        /// </summary>
        private void CheckLayoutConfig()
        {
            try
            {
                string jsonPath = ECFileConstantsManager.RootFolder + @"\" + _workName + @"\" + ECFileConstantsManager.LayoutConfigName;
                if (File.Exists(jsonPath))
                {
                    LayoutInfo = ECSerializer.LoadObjectFromJson<ECRuntimeLayoutInfo>(jsonPath);
                    if(LayoutInfo.DisplayColumns*LayoutInfo.DisplayRows<Results.Count)
                    {
                        LayoutInfo = new ECRuntimeLayoutInfo();
                        LayoutInfo.IsChartVisible = false;
                        LayoutInfo.DisplayRows = 1;
                        LayoutInfo.DisplayColumns = Results.Count;
                        ECSerializer.SaveObjectToJson(jsonPath, LayoutInfo);
                    }
                }
                else
                {
                    LayoutInfo=new ECRuntimeLayoutInfo();
                    LayoutInfo.IsChartVisible = false;
                    LayoutInfo.DisplayRows = 1;
                    LayoutInfo.DisplayColumns = Results.Count;
                    ECSerializer.SaveObjectToJson(jsonPath, LayoutInfo);
                }
            }
            catch (System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, NLog.LogLevel.Error);
            }
        }

        /// <summary>
        /// 保存布局配置
        /// </summary>
        private void SaveLayoutConfig()
        {
            try
            {
                string jsonPath = ECFileConstantsManager.RootFolder + @"\" + _workName + @"\" + ECFileConstantsManager.LayoutConfigName;
                if (LayoutInfo != null)
                {
                    ECSerializer.SaveObjectToJson(jsonPath, LayoutInfo);
                    ECDialogManager.ShowMsg(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.SaveFinished));
                }
            }
            catch(System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace+ex.Message,NLog.LogLevel.Error);
                ECDialogManager.ShowMsg(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.SaveFailed));
            }
        }

        /// <summary>
        /// 改变列数
        /// </summary>
        /// <param name="obj"></param>
        private void ChangeColumns(int obj)
        {
            if (_isZooming) return;
            if (!(LayoutInfo.DisplayColumns * LayoutInfo.DisplayRows >= Results.Count))
                LayoutInfo.DisplayColumns = obj;
        }

        /// <summary>
        /// 改变行数
        /// </summary>
        /// <param name="obj"></param>
        private void ChangeRows(int obj)
        {
            if (_isZooming) return;
            if (!(LayoutInfo.DisplayRows * LayoutInfo.DisplayColumns >= Results.Count))
                LayoutInfo.DisplayRows = obj;
        }

        /// <summary>
        /// 保存工作流ToolBlock
        /// </summary>
        /// <param name="workStreamName"></param>
        private void SaveWorkStreamTB(string workStreamName)
        {
            try
            {
                ECWorkStream stream = WorkStreams.Where(t => t.WorkStreamInfo.StreamName == workStreamName)?.First();
                if (stream == null) return;

                string path = ECFileConstantsManager.RootFolder + @"\" + _workName + @"\" + ECFileConstantsManager.StreamsFolderName + 
                    @"\" + stream.WorkStreamInfo.StreamName + @"\" + ECFileConstantsManager.DLOutputTBName;

                if(!Directory.Exists(Directory.GetParent(path).FullName))
                    Directory.CreateDirectory(path);
                CogSerializer.SaveObjectToFile(stream.DLOutputTB, path);
                ECDialogManager.ShowMsg(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.SaveFinished));
            }
            catch(System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, LogLevel.Error);
            }
        }

        /// <summary>
        /// 保存工作流组ToolBlock
        /// </summary>
        /// <param name="groupName"></param>
        private void SaveGroupTB(string groupName)
        {
            try
            {
                ECWorkStreamsGroup group = WorkGroups.Where(t => t.GroupInfo.GroupName == groupName)?.First();
                if (group == null) return;

                string path = ECFileConstantsManager.RootFolder + @"\" + _workName + @"\" + ECFileConstantsManager.GroupsFolderName +
                    @"\" + group.GroupInfo.GroupName + @"\" + ECFileConstantsManager.GroupTBName;

                if (!Directory.Exists(Directory.GetParent(path).FullName))
                    Directory.CreateDirectory(path);
                CogSerializer.SaveObjectToFile(group.ToolBlock, path);
                ECDialogManager.ShowMsg(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.SaveFinished));
            }
            catch (System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, LogLevel.Error);
            }
        }

        /// <summary>
        /// 保存图像源ToolBlock
        /// </summary>
        /// <param name="imageSourceName"></param>
        private void SaveImageSourceConfig(string imageSourceName)
        {
            try
            {
                ECWorkImageSource imageSource = WorkImageSources.Where(t => t.ImageSourceInfo.ImageSourceName == imageSourceName)?.First();
                if (imageSource == null) return;

                string path = ECFileConstantsManager.RootFolder + @"\" + _workName + @"\" + ECFileConstantsManager.ImageSourceFolderName +
                    @"\" + imageSource.ImageSourceInfo.ImageSourceName + @"\" + ECFileConstantsManager.ImageSourceAcqTBName;

                if (!Directory.Exists(Directory.GetParent(path).FullName))
                    Directory.CreateDirectory(path);
                CogSerializer.SaveObjectToFile(imageSource.ToolBlock, path);

                string jsonPath = ECFileConstantsManager.RootFolder + @"\" + _workName + @"\" + ECFileConstantsManager.ImageSourceFolderName +
                    @"\" + imageSource.ImageSourceInfo.ImageSourceName + @"\" + ECFileConstantsManager.ImageSourceConfigName;

                ECSerializer.SaveObjectToJson(jsonPath, imageSource.ImageSourceInfo);


                ECDialogManager.ShowMsg(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.SaveFinished));
            }
            catch (System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, LogLevel.Error);
            }

        }

        /// <summary>
        /// 选择图像源路径
        /// </summary>
        /// <param name="imageSourceName"></param>
        private void SelectImageFile(string imageSourceName)
        {
            try
            {
                ECWorkImageSource imageSource = WorkImageSources.Where(t => t.ImageSourceInfo.ImageSourceName == imageSourceName)?.First();
                MessageBoxResult result = System.Windows.MessageBox.Show(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.FileChooseYesFolderChooseNo), ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.PleaseSelect), MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    var dialog = new System.Windows.Forms.OpenFileDialog();
                    dialog.Multiselect = false;
                    dialog.Filter = "Image File(*.idb,*.cdb)|*.idb;*.cdb";

                    if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        imageSource.ImageSourceInfo.ImageFilePath = dialog.FileName;
                    }
                }
                else if (result == MessageBoxResult.No)
                {
                    var dialog = new System.Windows.Forms.FolderBrowserDialog();

                    if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        imageSource.ImageSourceInfo.ImageFilePath = dialog.SelectedPath;
                    }
                }
            }
            catch (System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, LogLevel.Error);
            }

        }

        /// <summary>
        /// 实时显示
        /// </summary>
        private void LiveMode(string workStreamName)
        {
            if (!WorkStreamNames.Contains(workStreamName)) return;
            try
            {
                ECWorkStream stream = WorkStreams.Where(t => t.WorkStreamInfo.StreamName == workStreamName)?.First();

                // 获取图像源名称
                string imageSourceName = stream.WorkStreamInfo.ImageSourceName;

                // 获取图像源对象
                if (ImageSourceNames.Contains(imageSourceName))
                {
                    ECWorkImageSource imageSource = WorkImageSources.Where(t => t.ImageSourceInfo.ImageSourceName == imageSourceName)?.First();

                    if (stream.ResultViewModel.IsLiveMode)
                    {
                        if (imageSource.ToolBlock.Inputs.Contains("TriggerDelay")&&_imageSourcesTriggerDelay.ContainsKey(imageSourceName))
                            imageSource.ToolBlock.Inputs["TriggerDelay"].Value = _imageSourcesTriggerDelay[imageSourceName];
                        stream.ResultViewModel.IsLiveMode = false;
                    }
                    else
                    {
                        if (imageSource.ToolBlock.Inputs.Contains("TriggerDelay"))
                            imageSource.ToolBlock.Inputs["TriggerDelay"].Value = 0;
                        stream.ResultViewModel.IsLiveMode = true;
                    }
                    
                }
            }
            catch (System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, LogLevel.Error);
            }
        }

        /// <summary>
        /// 重置LiveMode状态
        /// </summary>
        private void ResetLiveMode()
        {
            foreach (ECWorkStream stream in WorkStreams)
            {
                stream.ResultViewModel.IsLiveMode=false;
            }
        }

        /// <summary>
        /// 保存工作流配置
        /// </summary>
        private void SaveWorkStreamConfig(string workStreamName)
        {
            try
            {
                ECWorkStream stream = WorkStreams.Where(t => t.WorkStreamInfo.StreamName == workStreamName)?.First();
                if (stream == null) return;

                string path = ECFileConstantsManager.RootFolder + @"\" + _workName + @"\" + ECFileConstantsManager.StreamsFolderName +
                    @"\" + stream.WorkStreamInfo.StreamName + @"\" + ECFileConstantsManager.StreamConfigName;

                if (!Directory.Exists(Directory.GetParent(path).FullName))
                    Directory.CreateDirectory(path);
                ECSerializer.SaveObjectToJson(path,stream.WorkStreamInfo);
                
                ECDialogManager.ShowMsg(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.SaveFinished));
            }
            catch (System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, LogLevel.Error);
            }
        }

        #endregion

        #region 工作运行

        private void RunLiveModeWorkStream(string workStreamName)
        {
            if (!_isReady || !WorkStreamNames.Contains(workStreamName)) return;
            try
            {
                ECWorkStream stream = WorkStreams.Where(t => t.WorkStreamInfo.StreamName == workStreamName)?.First();

                // 获取图像源名称
                string imageSourceName = stream.WorkStreamInfo.ImageSourceName;
                if (string.IsNullOrEmpty(imageSourceName)) return;

                // 获取图像源对象
                if (!ImageSourceNames.Contains(imageSourceName))
                    return;

                ECWorkImageSource imageSource = WorkImageSources.Where(t => t.ImageSourceInfo.ImageSourceName == imageSourceName)?.First();

                // 工作流Ready状态置FALSE
                if (StreamsLiveModeRunning != null && StreamsLiveModeRunning.ContainsKey(stream.WorkStreamInfo.StreamName))
                    StreamsLiveModeRunning[stream.WorkStreamInfo.StreamName] = true;

                Task.Run(() =>
                {
                    // 采集图像
                    ECWorkImageSourceOutput imageSourceResult = imageSource.GetImage();

                    // 判断图像是否为空
                    if (imageSourceResult != null&&imageSourceResult.Image!=null)
                        stream.RunLive(imageSourceResult.Image);
                });
            }
            catch (System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, LogLevel.Error);
            }
        }

        /// <summary>
        /// 运行工作流
        /// </summary>
        /// <param name="workStreamName"></param>
        private void RunWorkStream(string workStreamName)
        {
            if (!_isReady || !WorkStreamNames.Contains(workStreamName)) return;
            try
            {
                ECWorkStream stream = WorkStreams.Where(t => t.WorkStreamInfo.StreamName == workStreamName)?.First();

                if (stream.WorkStreamInfo.ImageSourceName == null&&!GetStreamIsRunning(stream.WorkStreamInfo.StreamName))
                {
                    InsertLogData(WorkLogType.Stream, $"\"{stream.WorkStreamInfo.StreamName}\" {ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.StartRunning)} " +
                        $"{(stream.ResultViewModel.TriggerCount + 1)}");
                    Task.Run(() =>
                    {
                        stream.Run();
                    });
                    return;
                }

                // 获取图像源名称
                string imageSourceName = stream.WorkStreamInfo.ImageSourceName;

                // 获取图像源对象
                if (!ImageSourceNames.Contains(imageSourceName))
                {
                    ECLog.WriteToLog($"{ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.CannotFind)} {imageSourceName}", LogLevel.Error);
                    return;
                }

                ECWorkImageSource imageSource = WorkImageSources.Where(t => t.ImageSourceInfo.ImageSourceName == imageSourceName)?.First();


                // 判断工作流是否异步执行
                if (!stream.WorkStreamInfo.IsAsyncMode)
                {
                    if ((imageSource.IsRunning || GetStreamIsRunning(workStreamName)))
                    {
                        InsertLogData(WorkLogType.Stream, $"\"{stream.WorkStreamInfo.StreamName}\" {ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.TriggerOverflow)}");
                        return;
                    }

                    // 检查是否包含于某个组
                    if (stream.WorkStreamInfo.GroupName != null)
                    {
                        ECWorkStreamsGroup group = WorkGroups.Where(t => t.GroupInfo.GroupName == stream.WorkStreamInfo.GroupName).First();
                        if (group != null)
                        {
                            group.GroupMutex.WaitOne();

                            if (!group.IsWaiting)
                            {
                                group.Inputs.Clear();
                                group.IsWaiting = true;
                            }
                            stream.GroupTriggerCount = group.TriggerCount;
                            group.GroupMutex.ReleaseMutex();
                        }
                    }

                    SetStreamIsRunning(workStreamName);

                    // 开始运行
                    InsertLogData(WorkLogType.Stream, $"\"{stream.WorkStreamInfo.StreamName}\" {ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.StartRunning)} " +
                        $"{(stream.ResultViewModel.TriggerCount+1)}");

                    // 工作流Ready状态置FALSE
                    if (StreamsReadyStatus != null && StreamsReadyStatus.ContainsKey(stream.WorkStreamInfo.StreamName))
                        StreamsReadyStatus[stream.WorkStreamInfo.StreamName] = false;

                    Task.Run(() =>
                    {
                        // 采集图像
                        ECWorkImageSourceOutput imageSourceResult = imageSource.GetImage();

                        // 判断图像是否为空
                        if (imageSourceResult != null && imageSourceResult.Image != null)
                            stream.Run(imageSourceResult);
                        else
                        {
                            InsertLogData(WorkLogType.Stream, $"\"{stream.WorkStreamInfo.StreamName}\" {ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.ImageIsNull)}");
                            ECLog.WriteToLog($"\"{stream.WorkStreamInfo.StreamName}\" {ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.ImageIsNull)}", LogLevel.Error);
                        }
                    });
                   
                }
                else
                {
                    if (!imageSource.IsRunning)
                    {
                        // 开始运行
                        if (stream.BufferQueue.BufferImages.Count < stream.BufferQueue.MaxBufferCount)
                            InsertLogData(WorkLogType.Stream, $"\"{stream.WorkStreamInfo.StreamName}\" {ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.StartRunning)} ");
                        else
                            return;

                        // 图像源Ready状态置FALSE
                        if (ImageSourceReadyStatus != null && ImageSourceReadyStatus.ContainsKey(imageSource.ImageSourceInfo.ImageSourceName))
                            ImageSourceReadyStatus[imageSource.ImageSourceInfo.ImageSourceName] = false;

                        Task.Run(() =>
                        {
                            // 采集图像
                            ECWorkImageSourceOutput imageSourceResult = imageSource.GetImage();

                            // 判断图像是否为空
                            if (imageSourceResult == null || imageSourceResult.Image == null)
                            {
                                InsertLogData(WorkLogType.Stream, $"\"{stream.WorkStreamInfo.StreamName}\" {ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.ImageIsNull)}");
                                ECLog.WriteToLog($"\"{stream.WorkStreamInfo.StreamName}\" {ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.ImageIsNull)}", LogLevel.Error);
                            }
                            else
                            {
                                if (stream.WorkStreamInfo.IsEnableMultiThread)
                                {
                                    if(stream.MultiThreadManager.BufferQueue.MaxBufferCount > stream.BufferQueue.BufferImages.Count)
                                        stream.RunOnMultiThreadMode(imageSourceResult);
                                    else
                                        ECLog.WriteToLog($"\"{stream.WorkStreamInfo.StreamName}\" {ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.BufferQueueOverflow)}", LogLevel.Error);
                                }
                                else if (!stream.RunAsync(imageSourceResult))
                                    ECLog.WriteToLog($"\"{stream.WorkStreamInfo.StreamName}\" {ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.BufferQueueOverflow)}", LogLevel.Error);
                            }
                        });
                    }
                    else
                        InsertLogData(WorkLogType.Stream, $" \"{stream.WorkStreamInfo.StreamName}\" {ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.TriggerOverflow)}");
                }
            }
            catch (System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, LogLevel.Error);
            }
        }

        /// <summary>
        /// 设置工作流为正在运行状态
        /// </summary>
        /// <param name="streamName"></param>
        private void SetStreamIsRunning(string streamName)
        {
            ECWorkStream stream = WorkStreams.Where(t => t.WorkStreamInfo.StreamName == streamName).First();
            if (stream != null)
            {
                stream.StreamMutex.WaitOne();

                if (!stream.IsRunning)
                {
                    stream.IsRunning = true;
                }
                stream.StreamMutex.ReleaseMutex();
            }
        }

        /// <summary>
        /// 获取工作流是否为正在运行状态
        /// </summary>
        /// <param name="streamName"></param>
        private bool GetStreamIsRunning(string streamName)
        {
            bool isRunning = true;
            ECWorkStream stream = WorkStreams.Where(t => t.WorkStreamInfo.StreamName == streamName).First();
            if (stream != null)
            {
                stream.StreamMutex.WaitOne();
                isRunning = stream.IsRunning;
                stream.StreamMutex.ReleaseMutex();
            }
            return isRunning;
        }

        /// <summary>
        /// 运行工作流,参数包含图像源
        /// </summary>
        /// <param name="workStreamName"></param>
        private void RunWorkStream(string workStreamName,ECWorkImageSourceOutput imageSourceOutput)
        {
            if (!_isReady || !WorkStreamNames.Contains(workStreamName)) return;
            try
            {
                ECWorkStream stream = WorkStreams.Where(t => t.WorkStreamInfo.StreamName == workStreamName)?.First();

                // 判断工作流是否异步执行
                if (!stream.WorkStreamInfo.IsAsyncMode)
                {
                    // 检查是否包含于某个组
                    if (stream.WorkStreamInfo.GroupName != null)
                    {
                        ECWorkStreamsGroup group = WorkGroups.Where(t => t.GroupInfo.GroupName == stream.WorkStreamInfo.GroupName).First();
                        if (group != null)
                        {
                            group.GroupMutex.WaitOne();

                            if (!group.IsWaiting)
                            {
                                group.Inputs.Clear();
                                group.IsWaiting = true;
                            }
                            stream.GroupTriggerCount = group.TriggerCount;
                            group.GroupMutex.ReleaseMutex();
                        }
                    }

                    // 开始运行
                    InsertLogData(WorkLogType.Stream, $"\"{stream.WorkStreamInfo.StreamName}\" {ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.StartRunning)} " +
                        $"{(stream.ResultViewModel.TriggerCount + 1)}");

                    // 工作流Ready状态置FALSE
                    if (StreamsReadyStatus != null && StreamsReadyStatus.ContainsKey(stream.WorkStreamInfo.StreamName))
                        StreamsReadyStatus[stream.WorkStreamInfo.StreamName] = false;

                    Task.Run(() =>
                    {
                        // 判断图像是否为空
                        if (imageSourceOutput != null&&imageSourceOutput.Image!=null)
                            stream.Run(imageSourceOutput);
                        else
                        {
                            InsertLogData(WorkLogType.Stream, $"\"{stream.WorkStreamInfo.StreamName}\" {ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.ImageIsNull)}");
                            ECLog.WriteToLog($"\"{stream.WorkStreamInfo.StreamName}\" {ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.ImageIsNull)}", LogLevel.Error);
                        }
                    });

                }
                else
                {
                    Task.Run(() =>
                    {
                        // 采集图像
                        if (imageSourceOutput == null || imageSourceOutput.Image == null)
                        {
                            InsertLogData(WorkLogType.Stream, $"\"{stream.WorkStreamInfo.StreamName}\" {ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.ImageIsNull)}");
                            ECLog.WriteToLog($"\"{stream.WorkStreamInfo.StreamName}\" {ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.ImageIsNull)}", LogLevel.Error);
                        }
                        else
                        {
                            if (stream.WorkStreamInfo.IsEnableMultiThread)
                            {
                                if (stream.MultiThreadManager.BufferQueue.MaxBufferCount > stream.BufferQueue.BufferImages.Count)
                                    stream.RunOnMultiThreadMode(ECGeneric.DeepCopy(imageSourceOutput));
                                else
                                    ECLog.WriteToLog($"\"{stream.WorkStreamInfo.StreamName}\" {ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.BufferQueueOverflow)}", LogLevel.Error);
                            }
                            else if (!stream.RunAsync(imageSourceOutput))
                                ECLog.WriteToLog($"\"{stream.WorkStreamInfo.StreamName}\" {ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.BufferQueueOverflow)}", LogLevel.Error);
                        }
                    });
                }
            }
            catch (System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, LogLevel.Error);
            }
        }

        /// <summary>
        /// 运行工作流组
        /// </summary>
        /// <param name="workGroupName"></param>
        private void RunWorkGroup(string workGroupName)
        {
            try
            {
                InsertLogData(WorkLogType.Group, $"\"{workGroupName}\" {ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.StartRunning)}");
                ECWorkStreamsGroup group = WorkGroups.Where(t => t.GroupInfo.GroupName == workGroupName).First();
                if (group != null)
                {
                    Task.Run(() =>
                    {
                        group.Run();
                    });
                }
            }
            catch (System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace+ex.Message, LogLevel.Error);
            }
        }

        /// <summary>
        /// 图像源采集完成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void imageSource_Completed(object sender, KeyValuePair<string,ECWorkImageSourceOutput> e)
        {
            // 取像完成事件通知第三方脚本
            ECWorkImageSource imageSource = WorkImageSources.Where(t => t.ImageSourceInfo.ImageSourceName == e.Key)?.First();
            if(imageSource!=null)
                Messenger.Default.Send<KeyValuePair<string,bool>>(new KeyValuePair<string,bool>(e.Key,imageSource.ImageSourceInfo.IsUseCam), ECMessengerManager.ThirdCardMessageKeys.ImageSourceCompleted);
            
            
            if (ImageSourceReadyStatus != null && ImageSourceReadyStatus.ContainsKey(e.Key))
                ImageSourceReadyStatus[e.Key] = true;
            if (!IsSystemOnline) return;
            foreach(ECWorkStream stream in WorkStreams)
            {
                if (stream.ResultViewModel.IsLiveMode) return;

                if (stream.WorkStreamInfo.TriggerType == Enum.GetName(typeof(ECWorkOptionManager.TriggerTypeConstants), ECWorkOptionManager.TriggerTypeConstants.ImageSource)
                    && stream.WorkStreamInfo.ImageSourceName == e.Key&&e.Value!=null&&e.Value.Image!=null)
                        RunWorkStream(stream.WorkStreamInfo.StreamName, e.Value);
            }
        }

        /// <summary>
        /// 工作流完成事件产生
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Stream_Compeleted(object sender, ECWorkStreamOrGroupResult e)
        {
            try
            {
                if (StreamsReadyStatus != null && StreamsReadyStatus.ContainsKey(e.StreamOrGroupName))
                    StreamsReadyStatus[e.StreamOrGroupName] = true;

                if (StreamsLiveModeRunning != null && StreamsLiveModeRunning.ContainsKey(e.StreamOrGroupName))
                    StreamsLiveModeRunning[e.StreamOrGroupName] = false;

                if (e.IsLiveMode) return;

                ECWorkStream stream = WorkStreams.Where(t => t.WorkStreamInfo.StreamName == e.StreamOrGroupName).First();

                double tbTime = stream.DLOutputTB.RunStatus.TotalTime;

                InsertLogData(WorkLogType.Stream, $"\"{e.StreamOrGroupName}\" {ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.RunComplete)} {e.TriggerCount} {tbTime}");
                

                // 如果工作流属于某个组,传递结果给组
                if (stream.WorkStreamInfo.GroupName != null)
                {
                    ECWorkStreamsGroup group = WorkGroups.Where(t => t.GroupInfo.GroupName == stream.WorkStreamInfo.GroupName).First();
                    if (group.TriggerCount == stream.GroupTriggerCount)
                        TransferStreamResultToGroup(stream.WorkStreamInfo.GroupName, e);
                }

                string resultStr = stream.WorkStreamInfo.StreamName + "," + e.ResultForSend?.ToString();

                // 未联机则不发送结果
                if (!IsSystemOnline) return;

                // 结果发送
                if (stream.WorkStreamInfo.ResultSenderType != null)
                {
                    // 判断结果发送类型,TCP或IO
                    ECWorkOptionManager.ResultSendTypeConstants senderType;
                    Enum.TryParse(stream.WorkStreamInfo.ResultSenderType, out senderType);

                    // TCP发送
                    if (senderType == ECWorkOptionManager.ResultSendTypeConstants.TCP)
                        TCPSendMessage(stream.WorkStreamInfo.TCPSenderName, resultStr);

                    // IO发送
                    else if (senderType == ECWorkOptionManager.ResultSendTypeConstants.IO)
                    {
                        ECWorkOptionManager.IOOutputConstants outputConstant;
                        Enum.TryParse(stream.WorkStreamInfo.IOOutputConstant, out outputConstant);
                        IOOutputExecute(outputConstant);
                    }

                    // FFP发送
                    else if (senderType == ECWorkOptionManager.ResultSendTypeConstants.FFP)
                        SendResultToFFP(resultStr);

                    // 脚本发送
                    else if (senderType == ECWorkOptionManager.ResultSendTypeConstants.Script)
                    {
                        // 启用第三方板卡
                        if (_enableThirdCard)
                        {
                            _thirdCard?.SetOutputStatus(resultStr);
                        }
                    }
                }
                else
                    Messenger.Default.Send<string>(resultStr, ECMessengerManager.WorkRuntimeViewModelMessengerKeys.TCPMsgToSystemServer);
            }
            catch(System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace+ex.Message,LogLevel.Error);
            }
        }

        /// <summary>
        /// 工作流组完成事件产生
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Group_Compeleted(object sender, ECWorkStreamOrGroupResult e)
        {
            try
            {
                ECWorkStreamsGroup group = WorkGroups.Where(t => t.GroupInfo.GroupName == e.StreamOrGroupName).First();

                double tbTime = group.ToolBlock.RunStatus.TotalTime;
                InsertLogData(WorkLogType.Group, $"\"{e.StreamOrGroupName}\" {ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.RunComplete)} {tbTime}");
                
                string resultStr = group.GroupInfo.GroupName + "," + e.ResultForSend?.ToString();
                
                // 未联机则不发送结果
                if (!IsSystemOnline) return;

                // 结果发送
                if (group.GroupInfo.ResultSenderType != null)
                {
                    // 判断结果发送类型,TCP或IO
                    ECWorkOptionManager.ResultSendTypeConstants senderType;
                    Enum.TryParse(group.GroupInfo.ResultSenderType, out senderType);

                    // TCP发送
                    if (senderType == ECWorkOptionManager.ResultSendTypeConstants.TCP)
                        TCPSendMessage(group.GroupInfo.TCPSenderName, resultStr);

                    // IO发送
                    else if (senderType == ECWorkOptionManager.ResultSendTypeConstants.IO)
                    {
                        ECWorkOptionManager.IOOutputConstants outputConstant;
                        Enum.TryParse(group.GroupInfo.IOOutputConstant, out outputConstant);
                        IOOutputExecute(outputConstant);
                    }

                    // FFP发送
                    else if (senderType == ECWorkOptionManager.ResultSendTypeConstants.FFP)
                        SendResultToFFP(resultStr);

                    // 脚本发送
                    else if (senderType == ECWorkOptionManager.ResultSendTypeConstants.Script)
                    {
                        // 启用第三方板卡
                        if (_enableThirdCard)
                        {
                            _thirdCard?.SetOutputStatus(resultStr);
                        }
                    }
                }
                else
                    Messenger.Default.Send<string>(resultStr, ECMessengerManager.WorkRuntimeViewModelMessengerKeys.TCPMsgToSystemServer);
            }
            catch(System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace+ex.Message,NLog.LogLevel.Error);
            }
        }

        /// <summary>
        /// 传递工作流结果到组
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="result"></param>
        private void TransferStreamResultToGroup(string groupName,ECWorkStreamOrGroupResult result)
        {
            try
            {
                ECWorkStreamsGroup group = WorkGroups.Where(t => t.GroupInfo.GroupName == groupName).First();

                // 访问锁,若无其他调用,则占有锁
                group.GroupMutex.WaitOne();
                if (result.CustomOutputs != null)
                {
                    foreach (CogToolBlockTerminal terminal in result.CustomOutputs)
                    {
                        if (!group.Inputs.Contains(terminal.Name))
                        {
                            if (terminal.Value == null)
                                group.Inputs.Add(new CogToolBlockTerminal(terminal.Name, terminal.ValueType));
                            else
                            {
                                if (terminal.ValueType.IsValueType)
                                    group.Inputs.Add(new CogToolBlockTerminal(terminal.Name, terminal.Value));
                                else
                                    group.Inputs.Add(new CogToolBlockTerminal(terminal.Name, ECGeneric.DeepCopy(terminal.Value)));
                            }
                        }
                    }
                }
                    group.StreamsResultValid[result.StreamOrGroupName] = true;

                if (group.CanGroupRun())
                    RunWorkGroup(groupName);

                // 释放锁
                group.GroupMutex.ReleaseMutex();
               
            }
            catch(System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace+ex.Message, LogLevel.Error);
            }
        }

        /// <summary>
        /// 收到TCP消息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TcpDevice_DataReceived(object sender, string e)
        {
            string tcpName = (sender as ECTCPDevice).TCPDeviceInfo.TCPDeviceName;
            try
            {
                if (e != null && e.Trim() != "")
                {
                    InsertLogData(WorkLogType.TCP, $"{ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.TCPIP)} \"{(sender as ECTCPDevice).TCPDeviceInfo.TCPDeviceName}\" " +
                        $"{ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.ReceivedMessage)}：{e}");

                    ECNativeModeCommand.NativeModeCommandTypeConstants type = ECNativeModeCommand.CheckCommandString(e);
                    if (type == ECNativeModeCommand.NativeModeCommandTypeConstants.ERR) return;

                    string[] command = e.Split(',');
                    if (command.Length < 2) return;

                    bool isExecuted = ExecuteTCPNativeModeCommand(e);

                    if (isExecuted && type != NativeModeCommandTypeConstants.TS && type != NativeModeCommandTypeConstants.TMS &&
                        type != NativeModeCommandTypeConstants.TWD && type != NativeModeCommandTypeConstants.TWE &&
                        type != NativeModeCommandTypeConstants.TWED && type != NativeModeCommandTypeConstants.TI &&
                        type != NativeModeCommandTypeConstants.TIWE)
                        TCPSendMessage(tcpName, "1");
                    else if (!isExecuted && type != NativeModeCommandTypeConstants.TS && type != NativeModeCommandTypeConstants.TMS &&
                        type != NativeModeCommandTypeConstants.TWD && type != NativeModeCommandTypeConstants.TWE &&
                        type != NativeModeCommandTypeConstants.TWED && type != NativeModeCommandTypeConstants.TI &&
                        type != NativeModeCommandTypeConstants.TIWE)
                        TCPSendMessage(tcpName, "0");
                }
                else
                    TCPSendMessage(tcpName, "-1");
            }
            catch(System.Exception ex)
            {
                TCPSendMessage(tcpName, "-1");
                ECLog.WriteToLog(ex.StackTrace+ex.Message, LogLevel.Error);
            }
        }

        /// <summary>
        /// 执行本地指令动作
        /// </summary>
        private bool ExecuteTCPNativeModeCommand(string msg)
        {
            if(!IsSystemOnline) return false;
            try
            {
                if (string.IsNullOrEmpty(msg)) return false;

                // 检查命令类型
                NativeModeCommandTypeConstants constant = ECNativeModeCommand.CheckCommandString(msg);

                string[] command = msg.Split(',');
                string streamName = command[1];

                switch (constant)
                {
                    //触发工作流
                    case NativeModeCommandTypeConstants.TS:
                        if(command.Length==2)
                        OnNativeModeTriggerStream(command[1]);
                        break;

                    //触发工作流
                    case NativeModeCommandTypeConstants.TMS:
                        if (command.Length >= 3)
                        {
                            List<string> streamNames = new List<string>();

                            // 所有需要触发的工作流列表
                            for (int i = 0; i < command.Length; i++)
                            {
                                if (i > 0)
                                    streamNames.Add(command[i]);
                            }

                            if (streamNames.Count > 0)
                            {
                                OnNativeModeTriggerMultiStream(streamNames);
                            }
                        }
                        break;

                    // 加载工作流配方
                    case NativeModeCommandTypeConstants.LR:
                        if (command.Length == 3)
                        {
                            KeyValuePair<string, string> streamRecipe = new KeyValuePair<string, string>(command[1], command[2]);
                            OnNativeModeLoadRecipe(streamRecipe);
                        }
                        else
                            return false;
                        break;

                    // 设置用户数据
                    case NativeModeCommandTypeConstants.SUD:
                        if (command.Length >= 3)
                        {
                            string userdata = msg.Substring(command[0].Length + 1, msg.Length - command[0].Length - 1);
                            userdata = userdata.Substring(command[1].Length + 1, userdata.Length - command[1].Length - 1);
                            if (!string.IsNullOrEmpty(userdata))
                            {
                                KeyValuePair<string, string> streamUserData = new KeyValuePair<string, string>(command[1], userdata);
                                OnNativeModeSetUserData(streamUserData);
                            }
                        }
                        else
                            return false;
                        break;

                    // 设置用户数据
                    case NativeModeCommandTypeConstants.SET:
                        if (command.Length == 3)
                        {
                            OnNativeModeSetExposureTime(command[1],Convert.ToDouble(command[2]));
                        }
                        else
                            return false;
                        break;

                    // 设置工作流图像源
                    case NativeModeCommandTypeConstants.SIS:
                        if (command.Length == 3)
                        {
                            OnNativeModeSetStreamImageSource(command[1], command[2]);
                        }
                        else
                            return false;
                        break;

                    // 工作流自触发启动
                    case NativeModeCommandTypeConstants.TSB:
                        KeyValuePair<string, bool> streamTSB = new KeyValuePair<string, bool>(command[1], true);
                        OnNativeModeSetInternalTrigger(streamTSB);
                        break;

                    // 工作流自触发停止
                    case NativeModeCommandTypeConstants.TSE:
                        KeyValuePair<string, bool> streamTSE = new KeyValuePair<string, bool>(command[1], false);
                        OnNativeModeSetInternalTrigger(streamTSE);
                        break;
                    
                    // 触发工作流并设置用户数据
                    case NativeModeCommandTypeConstants.TWD:
                        if (command.Length >= 3)
                        {
                            string userdata = msg.Substring(command[0].Length+1, msg.Length - command[0].Length - 1);
                            userdata = userdata.Substring(command[1].Length+1, userdata.Length - command[1].Length - 1);
                            if(!string.IsNullOrEmpty(userdata)) 
                                OnNativeModeTriggerWithUserData(command[1], userdata);
                        }
                        break;

                    // 触发工作流并设置曝光时间
                    case NativeModeCommandTypeConstants.TWE:
                        if (command.Length == 3)
                        {
                            string timeStr = msg.Substring(command[0].Length + 1, msg.Length - command[0].Length - 1);
                            timeStr = timeStr.Substring(command[1].Length + 1, timeStr.Length - command[1].Length - 1);
                            double time = Convert.ToDouble(timeStr);
                            if (time != 0)
                                OnNativeModeTriggerWithExposureTime(command[1], time);
                        }
                        break;

                    // 触发工作流并设置曝光时间和用户数据
                    case NativeModeCommandTypeConstants.TWED:
                        if (command.Length >= 4)
                        {
                            double time = Convert.ToDouble(command[2]);

                            string userdata = msg.Substring(command[0].Length + 1, msg.Length - command[0].Length-1);
                            userdata = userdata.Substring(command[1].Length + 1, userdata.Length - command[1].Length-1);
                            userdata=userdata.Substring(command[2].Length + 1, userdata.Length - command[2].Length - 1);
                            if(time != 0&&!string.IsNullOrEmpty(userdata))
                                OnNativeModeTriggerWithExposureTimeAndUserData(command[1],time, userdata);
                        }
                        break;

                    // 触发工作流并设置图像源
                    case NativeModeCommandTypeConstants.TWI:
                        if (command.Length == 3)
                        {
                            OnNativeModeTriggerWithImageSource(command[1], command[2]);
                        }
                        break;

                    // 触发图像源并设置曝光时间
                    case NativeModeCommandTypeConstants.TI:
                        if (command.Length == 2)
                            OnNativeModeTriggerImageSource(command[1]);
                        break;

                    // 触发图像源并设置曝光时间
                    case NativeModeCommandTypeConstants.TIWE:
                        if (command.Length == 3)
                        {
                            string timeStr = msg.Substring(command[0].Length + 1, msg.Length - command[0].Length - 1);
                            timeStr = timeStr.Substring(command[1].Length + 1, timeStr.Length - command[1].Length - 1);
                            double time = Convert.ToDouble(timeStr);
                            if (time != 0)
                                OnNativeModeTriggerImageSourceWithExposureTime(command[1], time);
                        }
                        break;
                }
                return true;
            }
            catch(System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace+ex.Message, LogLevel.Error);
                return false;
            }
        }

        /// <summary>
        /// TCP发送消息
        /// </summary>
        /// <param name="tcpName"></param>
        /// <param name="message"></param>
        private void TCPSendMessage(string tcpName,string message)
        {
            try
            {
                List<string> tcpNames= WorkTCPDevices.Select(p => p.TCPDeviceInfo.TCPDeviceName).ToList();
                ECTCPDevice tcpDevice = WorkTCPDevices.Where(t => t.TCPDeviceInfo.TCPDeviceName == tcpName).First();
                
                tcpDevice.SendMessage(message);
                InsertLogData(WorkLogType.TCP, $"{ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.TCPIP)} \"{tcpName}\" " +
                            $"{ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.SendMessage)}：{message}");
            }
            catch(System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace+ex.Message, LogLevel.Error);
            }
        }

        /// <summary>
        /// IO收到信号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnIOTriggerStream(int lineIndex)
        {
            try
            {
                InsertLogData(WorkLogType.IO, $"{ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.IOInput)} {lineIndex} {ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.Event)}");
                if (!IsSystemOnline) return;

                foreach (ECWorkStream stream in WorkStreams)
                {
                    if (stream.WorkStreamInfo.IOInputConstant != null && stream.WorkStreamInfo.TriggerType == Enum.GetName(typeof(ECWorkOptionManager.TriggerTypeConstants), ECWorkOptionManager.TriggerTypeConstants.IO))
                    {
                        ECWorkOptionManager.IOInputConstants inputConstant;
                        Enum.TryParse(stream.WorkStreamInfo.IOInputConstant, out inputConstant);
                        if (lineIndex == (int)inputConstant)
                            RunWorkStream(stream.WorkStreamInfo.StreamName);
                    }
                }
            }
            catch (System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, NLog.LogLevel.Error);
            }
        }

        /// <summary>
        /// 工厂协议软事件触发工作流
        /// </summary>
        /// <param name="softEvent"></param>
        private void OnFfpTriggerStream(int softEvent)
        {
            try
            {
                InsertLogData(WorkLogType.FFP, $"{ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.FFP)} {ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.Event)} {softEvent}");
                if (!IsSystemOnline) return;
                    
                string softEventName = "SoftEvent" + softEvent.ToString();
                foreach (ECWorkStream stream in WorkStreams)
                {
                    if (stream.WorkStreamInfo.SoftEventConstant != null && stream.WorkStreamInfo.TriggerType == Enum.GetName(typeof(ECWorkOptionManager.TriggerTypeConstants), ECWorkOptionManager.TriggerTypeConstants.FFP))
                    {
                        if (softEventName == stream.WorkStreamInfo.SoftEventConstant)
                            RunWorkStream(stream.WorkStreamInfo.StreamName);
                    }
                }
            }
            catch(System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, NLog.LogLevel.Error);
            }

        }

        /// <summary>
        /// IO执行动作
        /// </summary>
        /// <param name="tcpName"></param>
        /// <param name="message"></param>
        private void IOOutputExecute(ECWorkOptionManager.IOOutputConstants output)
        {
            ECCommCard.PulseOutput((int)output);
        }

        /// <summary>
        /// 发送结果到FFP设备
        /// </summary>
        private void SendResultToFFP(string resultStr)
        {
            try
            {
                if (resultStr == null) return;

                InsertLogData(WorkLogType.FFP, $"{ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.SendMessage)} {resultStr}");
                if (ECCommCard.FfpNdm != null && ECCommCard.FfpScriptProcessor != null)
                {
                   ECCommCard.FfpScriptProcessor.ProcessOutputData(resultStr);
                }
                else
                    InsertLogData(WorkLogType.FFP, $"{ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.SendFailed)} {resultStr}");
            }
            catch(System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, NLog.LogLevel.Error);
            }
        }

        #endregion

        #endregion

        #region 属性
        /// <summary>
        /// 图像源集合
        /// </summary>
        private BindingList<ECWorkImageSource> _workImageSources;

        public BindingList<ECWorkImageSource> WorkImageSources
        {
            get { return _workImageSources; }
            set { _workImageSources = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// TCP设备集合
        /// </summary>
        private BindingList<ECTCPDevice> _workTCPDevices;

        public BindingList<ECTCPDevice> WorkTCPDevices
        {
            get { return _workTCPDevices; }
            set { _workTCPDevices = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 工作流集合
        /// </summary>
        private BindingList<ECWorkStream> _workStreams;

		public BindingList<ECWorkStream> WorkStreams
        {
			get { return _workStreams; }
			set { _workStreams = value;
				RaisePropertyChanged();
			}
		}

        /// <summary>
        /// 工作流配方集合
        /// </summary>
        private BindingList<WorkStreamRecipesViewModel> _workStreamsRecipe;

        public BindingList<WorkStreamRecipesViewModel> WorkStreamsRecipe
        {
            get { return _workStreamsRecipe; }
            set
            {
                _workStreamsRecipe = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 工作流组集合
        /// </summary>
        private BindingList<ECWorkStreamsGroup> _workGroups;

		public BindingList<ECWorkStreamsGroup> WorkGroups
        {
			get { return _workGroups; }
			set { _workGroups = value;
				RaisePropertyChanged();
			}
		}

		/// <summary>
		/// 显示的结果集合
		/// </summary>
		private BindingList<ECWorkStreamOrGroupResult> _results;

		public BindingList<ECWorkStreamOrGroupResult> Results
		{
			get { return _results; }
			set { _results = value;
				RaisePropertyChanged();
			}
		}

        /// <summary>
        /// 放大的结果
        /// </summary>
        private BindingList<ECWorkStreamOrGroupResult> _zoomResults;

        public BindingList<ECWorkStreamOrGroupResult> ZoomResults
        {
            get { return _zoomResults; }
            set { _zoomResults = value;
                RaisePropertyChanged();
            }
        }


        /// <summary>
        /// 显示的图表集合
        /// </summary>
        private BindingList<ECWorkStreamOrGroupResultChart> _resultsChart;

        public BindingList<ECWorkStreamOrGroupResultChart> ResultsChart
        {
            get { return _resultsChart; }
            set
            {
                _resultsChart = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 生产数据视图模型
        /// </summary>
        private WorkProductionDataViewModel _productDataViewModel;

        public WorkProductionDataViewModel ProductDataViewModel
        {
            get { return _productDataViewModel; }
            set { _productDataViewModel = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 工作日志视图模型
        /// </summary>
        private WorkRuntimeLogViewModel _workLogViewModel;

        public WorkRuntimeLogViewModel WorkLogViewModel
        {
            get { return _workLogViewModel; }
            set { _workLogViewModel = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 硬件图像采集器
        /// </summary>
        private ECFrameGrabber _frameGrabber;

        public ECFrameGrabber FrameGrabber
        {
            get { return _frameGrabber; }
            set { _frameGrabber = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// TCP监视
        /// </summary>
        private ECTCPMonitor _tcpMonitor;

        public ECTCPMonitor TcpMonitor
        {
            get { return _tcpMonitor; }
            set { _tcpMonitor = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 系统联机
        /// </summary>
        private bool _isSystemOnline;

        public bool IsSystemOnline
        {
            get { return _isSystemOnline; }
            set { _isSystemOnline = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 布局信息
        /// </summary>
        private ECRuntimeLayoutInfo _layoutInfo;

        public ECRuntimeLayoutInfo LayoutInfo
        {
            get { return _layoutInfo; }
            set { _layoutInfo = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 正在放大
        /// </summary>
        private bool _isZooming;

        public bool IsZooming
        {
            get { return _isZooming; }
            set { _isZooming = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 工作流名称列表
        /// </summary>
        private BindingList<string> _workStreamNames;

        public BindingList<string> WorkStreamNames
        {
            get { return _workStreamNames; }
            set { _workStreamNames = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 图像源名称列表
        /// </summary>
        private BindingList<string> _imageSourceNames;

        public BindingList<string> ImageSourceNames
        {
            get { return _imageSourceNames; }
            set { _imageSourceNames = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 工作流Ready状态
        /// </summary>
        private Dictionary<string,bool> _streamsReadyStatus;

        public Dictionary<string,bool> StreamsReadyStatus
        {
            get { return _streamsReadyStatus; }
            set { _streamsReadyStatus = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 图像源Ready状态
        /// </summary>
        private Dictionary<string,bool> _imageSourceReadyStatus;

        public Dictionary<string,bool> ImageSourceReadyStatus
        {
            get { return _imageSourceReadyStatus; }
            set { _imageSourceReadyStatus = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 工作流实时显示运行状态
        /// </summary>
        private Dictionary<string,bool> _streamsLiveModeRunning;

        public Dictionary<string,bool> StreamsLiveModeRunning
        {
            get { return _streamsLiveModeRunning; }
            set { _streamsLiveModeRunning = value;
                RaisePropertyChanged();
            }
        }


        /// <summary>
        /// 图像记录选项绑定列表
        /// </summary>
        private BindingList<string> _imageRecordConstantsBindableList;

        public BindingList<string> ImageRecordConstantsBindableList
        {
            get { return _imageRecordConstantsBindableList; }
            set { _imageRecordConstantsBindableList = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 图像记录条件绑定列表
        /// </summary>
        private BindingList<string> _imageRecordConditionConstantsBindableList;

        public BindingList<string> ImageRecordConditionConstantsBindableList
        {
            get { return _imageRecordConditionConstantsBindableList; }
            set { _imageRecordConditionConstantsBindableList = value;
                RaisePropertyChanged();
            }
        }


        #endregion
    }
}
