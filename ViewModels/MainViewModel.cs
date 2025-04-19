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
            // ע����Ϣ
            RegisterMessenger();

            //������
            BindCmd();

            // �첽��ʼ������
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

        #region ����

        #region ϵͳ
        /// <summary>
        /// ��ʼ��ϵͳ����
        /// </summary>
        /// <returns></returns>
        public bool InitialSystem()
        {
            try
            {
                GetStartupInfo();

                ECLog.WriteToLog(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.SystemStartupSetupLoaded), LogLevel.Trace);

                // ��������Ȩ�ļ�
                //if (!CheckSoftwareLicense())
                //    return false;


                // ���DefaultĿ¼
                CheckRootFolder();

                Messenger.Default.Send<string>($"{ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.LoadingVisionModule)}......", ECMessengerManager.MainViewModelMessengerKeys.InitialStepChanged);
                // if (!CheckVisionProLicense()) return false;

                // ��ʼ��ViDi����
                if (ECDLEnvironment.IsEnable)
                    if (!InitialDLEnvironment()) return false;

                ECLog.WriteToLog(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.VisionModuleLoaded), LogLevel.Trace);

                // ����Windowsϵͳ���
                Messenger.Default.Send<string>($"{ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.LoadingWindowsMonitor)}......", ECMessengerManager.MainViewModelMessengerKeys.InitialStepChanged);

                WindowsSystemInfo = new ECWMI();
                ECLog.WriteToLog(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.WindowsMonitorLoaded), LogLevel.Trace);

                // ����Default����
                SetSystemDefaultConfig();

                // ����UI��ʱ��
                StartUITimer();

                // ˢ�¹����б�
                RefreshWorkList();

                _isSystemStartComplete = true;

                // ����Ƿ���Ҫ��������Default��ҵ
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
        /// ������
        /// </summary>
        private void BindCmd()
        {
            // �����༭

            CmdLoadContentView = new GalaCmd.RelayCommand<string>(LoadContentView);
            CmdCreateNewWork = new GalaCmd.RelayCommand(CreateNewWork);
            CmdDeleteWork = new GalaCmd.RelayCommand<object>(DeleteWork);
            CmdSelectWork = new GalaCmd.RelayCommand<object>(SelectWork);
            CmdRefreshWorkList = new GalaCmd.RelayCommand(RefreshWorkList);
            CmdCopyWork = new GalaCmd.RelayCommand<object>(CopyWork);
            CmdImportWork = new GalaCmd.RelayCommand(ImportWork);
            CmdExportWork = new GalaCmd.RelayCommand<object>(ExportWork);

            // �������ܲ˵���ť����
            CmdOpenSystemSetup = new GalaCmd.RelayCommand(OpenStartupSetup);
            CmdOpenFileManager = new GalaCmd.RelayCommand(OpenFileManager);
            CmdOpenVersionInfo = new GalaCmd.RelayCommand(OpenVersionInfo);
            CmdOpenHelp = new GalaCmd.RelayCommand(OpenHelp);
            CmdLogin = new GalaCmd.RelayCommand(Login);
            CmdEditAdminPassword = new GalaCmd.RelayCommand(EditAdminPassword);
            CmdClearAlarm = new GalaCmd.RelayCommand(ClearAlarm);


            // ��������
            CmdLoadWork = new GalaCmd.RelayCommand<object>(LoadWork);
            CmdUnLoadWork = new GalaCmd.RelayCommand(CommandUnLoadWork);

            // ����ģʽ
            CmdSetSystemOnline = new GalaCmd.RelayCommand(SetSystemOnline);

        }

        /// <summary>
        /// ���Default����Ŀ¼
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
        /// ע�ᶩ�ĵ���Ϣ
        /// </summary>
        private void RegisterMessenger()
        {
            Messenger.Default.Register<string>(this, ECMessengerManager.MainWindowMessengerKeys.SystemClosing, OnMainWindowClosing);
            Messenger.Default.Register<string>(this, ECMessengerManager.MainViewModelMessengerKeys.AskWorkName, OnAskWorkName);
            Messenger.Default.Register<string>(this, ECMessengerManager.MainViewModelMessengerKeys.CloseWork, OnCloseWork);
            Messenger.Default.Register<string>(this, ECMessengerManager.WorkRuntimeViewModelMessengerKeys.TCPMsgToSystemServer, OnWorkRuntimeTCPMsg);
            Messenger.Default.Register<int>(this, ECMessengerManager.MainViewModelMessengerKeys.WorkLoaded, OnWorkLoaded);
            Messenger.Default.Register<string>(this, ECMessengerManager.ECLogMessengerKeys.OnAlarm, OnAlarm);

            // Ffp��Ϣ
            Messenger.Default.Register<bool>(this, ECMessengerManager.CommCardMessengerKeys.FFPSetOnline, SetSystemOnlineMode);
            Messenger.Default.Register<int>(this, ECMessengerManager.CommCardMessengerKeys.FFPJobChangeRequested, LoadWork);
            Messenger.Default.Register<string>(this, ECMessengerManager.CommCardMessengerKeys.FFPClearError, ClearError);

            // Nativemode��Ϣ
            //Messenger.Default.Register<bool>(this, ECMessengerManager.ECNativeModeCommandMessengerKeys.LoadRecipeAck, OnNativeModeAck);
            //Messenger.Default.Register<bool>(this, ECMessengerManager.ECNativeModeCommandMessengerKeys.SetUserDataAck, OnNativeModeAck);
        }

        /// <summary>
        /// ������ʾ
        /// </summary>
        /// <param name="obj"></param>
        private void OnAlarm(string obj)
        {
            IsAlarm = true;
        }

        /// <summary>
        /// �����û�����ȷ��
        /// </summary>
        /// <param name="obj"></param>
        //private void OnNativeModeAck(bool obj)
        //{
        //    int ack = obj ? 1 : 0;
        //    //_systemTCPServer?.Broadcast(ack.ToString());
        //}

        /// <summary>
        /// �����������
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
        /// �������
        /// </summary>
        /// <param name="obj"></param>
        private void ClearError(string obj)
        {

        }

        /// <summary>
        /// ע�����ĵ���Ϣ
        /// </summary>
        private void UnregisterMessenger()
        {
            Messenger.Default.Unregister<string>(this, ECMessengerManager.MainWindowMessengerKeys.SystemClosing, OnMainWindowClosing);
            Messenger.Default.Unregister<string>(this, ECMessengerManager.MainViewModelMessengerKeys.AskWorkName, OnAskWorkName);
            Messenger.Default.Unregister<string>(this, ECMessengerManager.MainViewModelMessengerKeys.CloseWork, OnCloseWork);
            Messenger.Default.Unregister<string>(this, ECMessengerManager.WorkRuntimeViewModelMessengerKeys.TCPMsgToSystemServer, OnWorkRuntimeTCPMsg);
        }

        /// <summary>
        /// �յ���������ʱ��ҪϵͳTCPת������Ϣ
        /// </summary>
        /// <param name="obj"></param>
        private void OnWorkRuntimeTCPMsg(string obj)
        {
            if (obj != null)
                _systemTCPServer?.Broadcast(obj);
        }

        /// <summary>
        /// �رչ����༭
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
        /// ����ѡ��Ĺ�������
        /// </summary>
        /// <param name="obj"></param>
        private void OnAskWorkName(string obj)
        {
            Messenger.Default.Send<string>(SelectedWorkName, ECMessengerManager.MainViewModelMessengerKeys.ReplyWorkName);
        }

        /// <summary>
        /// �����ڹر�
        /// </summary>
        /// <param name="obj"></param>
        private void OnMainWindowClosing(string obj)
        {
            CLoseSystem();
            Messenger.Default.Send<string>("", ECMessengerManager.MainViewModelMessengerKeys.SystemClosed);
        }

        /// <summary>
        /// ��������Ȩ
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
        /// ��ȡ������Ϣ����������
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

                // ����ͨѶ��
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
        /// ���VisionPro��Ȩ
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
        /// ��ʼ��ViDi����
        /// </summary>
        private bool InitialDLEnvironment()
        {
            List<int> gpuList = new List<int>();
            try
            {
                ViDiSuiteServiceLocator.Initialize();
                ECDLEnvironment.Control = new ViDi2.Runtime.Local.Control(GpuMode.Default, gpuList);
                ECLog.WriteToLog($"{ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.VPDLVersion)}��" + ECDLEnvironment.Control.DotNETWrapperVersion.ToString(), LogLevel.Trace);
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
        /// ����ϵͳDefault����
        /// </summary>
        private void SetSystemDefaultConfig()
        {
            // ��ʾ�����б�
            IsShowWorkList = true;

            //��ʼ������ʾ
            Title = ECFileConstantsManager.ProgramName;

            //�ֶ�ģʽ
            IsOnlineMode = false;
            IsOnlineMode = false;
            IsWorkLoaded = false;
        }

        /// <summary>
        /// ����UI��ʱ��,���ڸ��½���ʱ���һЩϵͳ״̬����ʾ
        /// </summary>
        private void StartUITimer()
        {
            _timer = new System.Timers.Timer();
            _timer.Interval = 2000;
            _timer.Elapsed += Timer_Elapsed;
            _timer.Start();
        }

        /// <summary>
        /// ����ˢ��
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
        /// ������ڵ�����
        /// </summary>
        private void ClearExpiredData()
        {
            if(_startupSettings!=null&&!_isClearingExpiredData)
            {
                _isClearingExpiredData = true;
                Task.Factory.StartNew(() =>
                {
                    // ͼƬ��Ŀ¼
                    string folder = ECFileConstantsManager.ImageRootFolder + @"\" + SelectedWorkName + @"\" + ECFileConstantsManager.ImageRecordFolderName;

                    if (Directory.Exists(folder))
                    {
                        string[] streamFolders = Directory.GetDirectories(folder);
                        foreach (string file in streamFolders)
                        {
                            // ͼƬ�ļ���
                            string originalFolder = file + @"\" + ECFileConstantsManager.OriginalImageFolderName; // ԭͼ�ļ���
                            string graphicFolder = file + @"\" + ECFileConstantsManager.GraphicImageFolderName; // ͼ��ͼƬ�ļ���

                            // OK/NG�ļ���ɾ��
                            DeleteExpiredOKNGDirectory(originalFolder, _startupSettings.ImageRetainedDaysForOK,_startupSettings.ImageRetainedDaysForNG);
                            DeleteExpiredOKNGDirectory(graphicFolder, _startupSettings.ImageRetainedDaysForOK, _startupSettings.ImageRetainedDaysForNG);

                        }
                    }
                    string databaseFolder = ECFileConstantsManager.RootFolder + @"\" + SelectedWorkName + @"\" + ECFileConstantsManager.DatabaseFolderName;

                    // �����������ļ���
                    if (Directory.Exists(databaseFolder + @"\" + ECFileConstantsManager.WorkStreamsDataFolderName))
                    {
                        string[] streamsFolder = Directory.GetDirectories(databaseFolder + @"\" + ECFileConstantsManager.WorkStreamsDataFolderName);
                        foreach (string streamfolder in streamsFolder)
                        {
                            DeleteExpiredDirectory(streamfolder, _startupSettings.DataRetainedDays);
                        }
                    }

                    // ������־�ļ���
                    if (Directory.Exists(databaseFolder + @"\" + ECFileConstantsManager.WorkLogFolderName))
                    {
                        DeleteExpiredDirectory(databaseFolder + @"\" + ECFileConstantsManager.WorkLogFolderName, _startupSettings.DataRetainedDays);
                    }

                    _isClearingExpiredData = false;
                });
            }
        }

        /// <summary>
        /// ɾ�����ڵ�OK/NGĿ¼
        /// </summary>
        /// <param name="path">Ŀ¼·��</param>
        /// <param name="expiredDays">���������޶�ֵ</param>
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

                            // ɾ������OK�ļ���
                            if (days >= expiredDaysForOK&&Directory.Exists(okFolderPath))
                                Directory.Delete(okFolderPath, true);
                            // ɾ������NG�ļ���
                            if (days >= expiredDaysForNG && Directory.Exists(ngFolderPath))
                                Directory.Delete(ngFolderPath, true);

                            // ���ļ����ѿգ���ɾ�����ڵ��ļ���
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
        /// ɾ�����ڵ�Ŀ¼
        /// </summary>
        /// <param name="path">Ŀ¼·��</param>
        /// <param name="expiredDays">���������޶�ֵ</param>
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
        /// ж�ع���ָ��ִ��
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private void CommandUnLoadWork()
        {
            if (!ECDialogManager.Verify($"{ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.UnloadWork)} \"{SelectedWorkName}\"")) return;
            UnLoadWork();
        }

        /// <summary>
        /// ж�ع���
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
        /// װ�ع���
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
        /// װ�ع���,ͨ������Ĺ�������
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
                Title = "VisionPro Framework" + " �� " + workName;
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

                // ֪ͨ�������ص�ģ��
                Messenger.Default.Send<ECWorkInfo>(info, ECMessengerManager.MainViewModelMessengerKeys.LoadWorkChanged);
            }
            catch (System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, NLog.LogLevel.Error);
            }
        }

        /// <summary>
        /// ��鹤�����õ�ģʽ�뵱ǰ����������ģʽ�Ƿ�һ��,������DL��û������DL
        /// </summary>
        private bool CheckWorkDLEnable(ECWorkInfo info)
        {
            if (info.IsDLEnable == ECDLEnvironment.IsEnable)
                return true;
            else
                return false;
        }

        /// <summary>
        /// װ�ع���,ͨ������Ĺ���ID
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
        /// ���ϵͳTCP������״̬
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
        /// ϵͳTCP�������յ���Ϣ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SystemTCPServer_DataReceived(object sender, SimpleTCP.Message e)
        {
            ECLog.WriteToLog($"Receive data --{e.MessageString}-- from {e.TcpClient.Client.RemoteEndPoint}", NLog.LogLevel.Info);
            try
            {
                // �յ���Ϣ��Ϊ��
                if (e != null && e.MessageString.Trim() != "")
                {
                    // �����������
                    NativeModeCommandTypeConstants constant = ECNativeModeCommand.CheckCommandString(e.MessageString);
                    
                    // �������Ͳ�Ϊ����
                    if (constant != NativeModeCommandTypeConstants.ERR)
                    {
                        // ִ�в�����ִ�����
                        object reply = ExecuteNativeCommandAction(constant, e.MessageString);
                        if (Convert.ToString(reply) != "-2")
                            _systemTCPServer?.Broadcast(reply.ToString());
                    }
                    // �������ʹ���,����0
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
        /// ִ�б���ָ���
        /// </summary>
        private object ExecuteNativeCommandAction(NativeModeCommandTypeConstants constant, string content)
        {
            try
            {
                object result = 1;
                string[] commandStr = content.Split(',');
                switch (constant)
                {
                    //����������
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

                    //����������
                    case NativeModeCommandTypeConstants.TMS:
                        if (IsOnlineMode)
                        {
                            Messenger.Default.Send<string>(content, ECMessengerManager.MainViewModelMessengerKeys.NativeModeMsg);
                            result = -2;
                        }
                        else
                            result = 0;
                        break;

                    // ���ع������䷽
                    case NativeModeCommandTypeConstants.LR:
                        if (IsOnlineMode)
                        {
                            Messenger.Default.Send<string>(content, ECMessengerManager.MainViewModelMessengerKeys.NativeModeMsg);
                            return -2;
                        }
                        else
                            result = 0;
                        break;

                    // �����û�����
                    case NativeModeCommandTypeConstants.SUD:
                        if (IsOnlineMode)
                        {
                            Messenger.Default.Send<string>(content, ECMessengerManager.MainViewModelMessengerKeys.NativeModeMsg);
                            return -2;
                        }
                        else
                            result = 0;
                        break;

                    // �����û�����
                    case NativeModeCommandTypeConstants.SET:
                        if (IsOnlineMode)
                        {
                            Messenger.Default.Send<string>(content, ECMessengerManager.MainViewModelMessengerKeys.NativeModeMsg);
                            return -2;
                        }
                        else
                            result = 0;
                        break;

                    // �����û�����
                    case NativeModeCommandTypeConstants.SIS:
                        if (IsOnlineMode)
                        {
                            Messenger.Default.Send<string>(content, ECMessengerManager.MainViewModelMessengerKeys.NativeModeMsg);
                            return -2;
                        }
                        else
                            result = 0;
                        break;

                    // �������Դ�������
                    case NativeModeCommandTypeConstants.TSB:
                        if (IsOnlineMode)
                        {
                            Messenger.Default.Send<string>(content, ECMessengerManager.MainViewModelMessengerKeys.NativeModeMsg);
                            return -2;
                        }
                        else
                            result = 0;
                        break;

                    // �������Դ���ֹͣ
                    case NativeModeCommandTypeConstants.TSE:
                        if (IsOnlineMode)
                        {
                            Messenger.Default.Send<string>(content, ECMessengerManager.MainViewModelMessengerKeys.NativeModeMsg);
                            return -2;
                        }
                        else
                            result = 0;
                        break;

                    // �����������������û�����
                    case NativeModeCommandTypeConstants.TWD:
                        if (IsOnlineMode)
                        {
                            Messenger.Default.Send<string>(content, ECMessengerManager.MainViewModelMessengerKeys.NativeModeMsg);
                            return -2;
                        }
                        else
                            result = 0;
                        break;

                    // �����������������ع�ʱ��
                    case NativeModeCommandTypeConstants.TWE:
                        if (IsOnlineMode)
                        {
                            Messenger.Default.Send<string>(content, ECMessengerManager.MainViewModelMessengerKeys.NativeModeMsg);
                            return -2;
                        }
                        else
                            result = 0;
                        break;

                    // �����������������ع�ʱ����û�����
                    case NativeModeCommandTypeConstants.TWED:
                        if (IsOnlineMode)
                        {
                            Messenger.Default.Send<string>(content, ECMessengerManager.MainViewModelMessengerKeys.NativeModeMsg);
                            return -2;
                        }
                        else
                            result = 0;
                        break;

                    // ����������������ͼ��Դ
                    case NativeModeCommandTypeConstants.TWI:
                        if (IsOnlineMode)
                        {
                            Messenger.Default.Send<string>(content, ECMessengerManager.MainViewModelMessengerKeys.NativeModeMsg);
                            return -2;
                        }
                        else
                            result = 0;
                        break;

                    // ����ͼ��Դ
                    case NativeModeCommandTypeConstants.TI:
                        if (IsOnlineMode)
                        {
                            Messenger.Default.Send<string>(content, ECMessengerManager.MainViewModelMessengerKeys.NativeModeMsg);
                            return -2;
                        }
                        else
                            result = 0;
                        break;

                    // ����ͼ��Դ
                    case NativeModeCommandTypeConstants.TIWE:
                        if (IsOnlineMode)
                        {
                            Messenger.Default.Send<string>(content, ECMessengerManager.MainViewModelMessengerKeys.NativeModeMsg);
                            return -2;
                        }
                        else
                            result = 0;
                        break;

                    // ���ع���
                    case NativeModeCommandTypeConstants.LW:
                        if (!IsWorkLoaded && WorkList.ToList().Exists(w => w.WorkInfo.WorkName == commandStr[1]))
                            LoadWork(commandStr[1]);
                        else
                            result = 0;
                        break;

                    // ϵͳ�����ѻ�
                    case NativeModeCommandTypeConstants.SO:
                        if (Convert.ToInt16(commandStr[1]) == 1)
                            IsOnlineMode = true;
                        else if (Convert.ToInt16(commandStr[1]) == 0)
                            IsOnlineMode = false;
                        break;

                    // ж�ع���
                    case NativeModeCommandTypeConstants.UW:
                        if (IsWorkLoaded && !IsOnlineMode)
                            UnLoadWork();
                        else
                            result = 0;
                        break;

                    // ��ȡ����״̬
                    case NativeModeCommandTypeConstants.GO:
                        if (IsOnlineMode)
                            result = 1;
                        else
                            result = 0;
                        break;

                    // ��ȡ��ǰ���������Ĺ�����
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

                    // ��ȡ��ǰ��������
                    case NativeModeCommandTypeConstants.GW:
                        if (IsWorkLoaded)
                            result = SelectedWorkName;
                        break;

                    // ��ȡ���й�������
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
        /// ����ϵͳ�������ѻ�
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
        /// ����ϵͳ����ģʽ
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
        /// ����Ƿ���������
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
        /// ���ز˵�ѡ�Ĳ�������
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
        /// �ر�ϵͳ
        /// </summary>
        private void CLoseSystem()
        {
            UnregisterMessenger();

            // ֹͣϵͳTCP������
            _systemTCPServer?.Stop();

            // ж�ع���
            if (IsWorkLoaded)
                UnLoadWork();

            // ֹͣ��ʱ
            _timer.Stop();

            // �ͷ�ϵͳ��Ϣ���Ӷ���
            WindowsSystemInfo.Dispose();

            // �ͷ�DL����
            ECDLEnvironment.ClearWorkspaces();
            ECDLEnvironment.Control?.Dispose();

            // �ͷ�Ӳ�����
            DisposeFrameGrabber();

            // �ر�EL�����Դ
            Startup.Shutdown();

            ViewModelLocator.Cleanup();
        }

        /// <summary>
        /// �ͷ��������
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
        /// �������
        /// </summary>
        private void ClearAlarm()
        {
            IsAlarm = false;
        }

        #endregion

        #region �༭����

        /// <summary>
        /// ˢ�¹����б�
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
        /// �����µĹ���
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
        /// �����¹������ļ����ļ���
        /// </summary>
        /// <param name="workName"></param>
        private void CreateNewWorkFoldersAndFiles(string workName)
        {
            // ����������Ϣ�ļ�
            ECWorkInfo workInfo = new ECWorkInfo();
            workInfo.WorkName = workName;
            workInfo.WorkID = GetNewWorkID();
            workInfo.IsDLEnable = ECDLEnvironment.IsEnable;
            workInfo.ModifiedTime=DateTime.Now;

            // ����������ģ���ļ���
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

            // ���湤����Ϣ�ļ�
            string jsonPath = path + @"\" + ECFileConstantsManager.WorkInfoFileName;
            ECSerializer.SaveObjectToJson(jsonPath, workInfo);
        }

        /// <summary>
        /// ��ȡ�µĹ���ID
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
        /// ɾ������
        /// </summary>
        /// <param name="work">Ҫɾ���Ĺ���</param>
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
                        
                        // ɾ���ļ���
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
        /// ���ƹ���
        /// </summary>
        /// <param name="work">���ƵĹ���</param>
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

                        // ���湤����Ϣ�ļ�
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
        /// ѡ����
        /// </summary>
        /// <param name="obj">ѡ��Ĺ���</param>
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
                    Title = "VisionPro Framework" + " �� " + workName;
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
        /// ����Ƿ���δ�رյĹ���,������,�򷵻�false
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
        /// ���빤��
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
                //Default��·��
                fileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                fileDialog.Title = ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.Import);
                fileDialog.Filter = "zip file (*.zip)|*.zip";
               if(fileDialog.ShowDialog()==System.Windows.Forms.DialogResult.OK)
                {
                    
                    string path=fileDialog.FileName;
                    bool isCheckedOK = true;
                    using(ZipArchive zip=System.IO.Compression.ZipFile.OpenRead(path))
                    {
                        // ��Ҫ���ļ�
                        string[] files = {
                            "workInfo.json",
                            "CommCardConfig",
                            "Database",
                            "GroupsConfig",
                            "ImageSourcesConfig",
                            "StreamsConfig",
                            "TCPConfig" };

                        // ѹ�����е��ļ�
                        List<string> zipFiles= new List<string>();
                        foreach(ZipArchiveEntry entry in zip.Entries)
                        {
                            string[] strings=entry.FullName.Split('/');
                            if(strings.Length == 0 &&!zipFiles.Contains(entry.FullName))
                                zipFiles.Add(entry.FullName);
                            else if (!zipFiles.Contains(strings[0]))
                                zipFiles.Add(strings[0]);
                        }

                        // ���
                        foreach (string file in files)
                        {
                           if(!zipFiles.Contains(file))
                                isCheckedOK = false;
                        }
                    }

                    // ���δͨ��
                    if(!isCheckedOK)
                    {
                        ECDialogManager.ShowMsg(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.CheckFailed));
                        return;
                    }

                    // ����
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
        /// ��������
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

        #region �˵���
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
        /// ���ļ�������
        /// </summary>
        private void OpenFileManager()
        {
            Window_FileManager window_FileManager = new Window_FileManager();
            window_FileManager.ShowDialog();
        }

        /// <summary>
        /// �򿪰����ĵ�
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
        /// ���������ý���
        /// </summary>
        private void OpenStartupSetup()
        {
            Window_SetupStartup window_StartupSettings = new Window_SetupStartup();
            window_StartupSettings.ShowDialog();
        }

        /// <summary>
        /// �򿪰汾��Ϣ����
        /// </summary>
        private void OpenVersionInfo()
        {
            Window_VersionInfo window_Version = new Window_VersionInfo();
            window_Version.ShowDialog();
        }

        /// <summary>
        /// �༭����Ա����
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
        /// �����¼״̬
        /// </summary>
        private void SaveLoginStatus(bool isLogin)
        {
            _startupSettings = ECStartupSettings.Instance();
            _startupSettings.IsDefaultLoginAdmin = isLogin;
            _startupSettings.Save();
        }
        #endregion

        #endregion

        #region �ֶ�

        /// <summary>
        /// ��ʾ��ʱ���ʱ��
        /// </summary>
        private System.Timers.Timer _timer;

        /// <summary>
        /// ����Ա�˻�����
        /// </summary>
        private string _adminPassword = "";

        /// <summary>
        /// ϵͳTCP������
        /// </summary>
        private static SimpleTCP.SimpleTcpServer _systemTCPServer;

  
        public static SimpleTCP.SimpleTcpServer GetSystemServer()
        {
            return _systemTCPServer;
        }

        /// <summary>
        /// ϵͳ�������
        /// </summary>
        private bool _isSystemStartComplete = false;

        /// <summary>
        /// ��������
        /// </summary>
        private bool _isStartupOnline = false;

        /// <summary>
        /// Default��������
        /// </summary>
        private string _defaultWorkName = "";

        /// <summary>
        /// ����������Ϣ
        /// </summary>
        private ECStartupSettings _startupSettings;

        /// <summary>
        /// ���������������
        /// </summary>
        private bool _isClearingExpiredData = false;

        /// <summary>
        /// �����༭�û��ؼ�
        /// </summary>
        private Control_WorkEdit _controlWorkEdit;

        /// <summary>
        /// ���������û��ؼ�
        /// </summary>
        private Control_WorkRuntime _controlWorkRuntime;

        #endregion �ֶ�

        #region ����

        /// <summary>
        /// ���ж�ع���
        /// </summary>
        public GalaCmd.RelayCommand<object> CmdLoadWork { get; set; }

        /// <summary>
        /// ���ж�ع���
        /// </summary>
        public GalaCmd.RelayCommand CmdUnLoadWork { get; set; }

        /// <summary>
        /// ��������µĹ���
        /// </summary>
        public GalaCmd.RelayCommand CmdCreateNewWork { get; set; }

        /// <summary>
        /// ���ɾ������
        /// </summary>
        public GalaCmd.RelayCommand<object> CmdDeleteWork { get; set; }

        /// <summary>
        /// ������ƹ���
        /// </summary>
        public GalaCmd.RelayCommand<object> CmdCopyWork { get; set; }

        /// <summary>
        /// ���ѡ����
        /// </summary>
        public GalaCmd.RelayCommand<object> CmdSelectWork { get; set; }

        /// <summary>
        /// �����������
        /// </summary>
        public GalaCmd.RelayCommand<object> CmdExportWork { get; set; }

        /// <summary>
        /// ������빤��
        /// </summary>
        public GalaCmd.RelayCommand CmdImportWork { get; set; }


        /// <summary>
        /// ���������������Ľ�����ͼ
        /// </summary>
        public GalaCmd.RelayCommand<string> CmdLoadContentView { get; set; }

        /// <summary>
        /// �����¼
        /// </summary>
        public GalaCmd.RelayCommand CmdLogin { get; set; }

        /// <summary>
        /// ������ļ�������
        /// </summary>
        public GalaCmd.RelayCommand CmdOpenFileManager { get; set; }

        /// <summary>
        /// ����򿪰����ĵ�
        /// </summary>
        public GalaCmd.RelayCommand CmdOpenHelp { get; set; }

        /// <summary>
        /// �������������
        /// </summary>
        public GalaCmd.RelayCommand CmdOpenSystemSetup { get; set; }

        /// <summary>
        /// ����򿪰汾��Ϣ
        /// </summary>
        public GalaCmd.RelayCommand CmdOpenVersionInfo { get; set; }

        /// <summary>
        /// ���ˢ�¹����б�
        /// </summary>
        public GalaCmd.RelayCommand CmdRefreshWorkList { get; set; }

        /// <summary>
        /// ������й�����ִ��һ��
        /// </summary>
        public GalaCmd.RelayCommand CmdRunOnce { get; set; }

        /// <summary>
        /// �������ϵͳ���������ѻ�
        /// </summary>
        public GalaCmd.RelayCommand CmdSetSystemOnline { get; set; }

        /// <summary>
        /// ����༭����Ա����
        /// </summary>
        public GalaCmd.RelayCommand CmdEditAdminPassword { get; set; }

        /// <summary>
        /// ����������
        /// </summary>
        public GalaCmd.RelayCommand CmdClearAlarm { get; set; }

        #endregion ����

        #region ����

        /// <summary>
        /// �������������ͼ�ؼ�
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
        /// ��ʾ������ʱ��
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
        /// �Ƿ��ѵ�¼
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
        /// ϵͳ������
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
        /// �����Ѽ���
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
        /// ������ѡ��
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
        /// ���������
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
        /// ѡ��Ĺ�������
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
        /// ������û���
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
        /// �����ı���
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
        /// �����������ʾ������
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
        /// �����б�(����Ŀ¼)
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
        /// �Ƿ���ʾ�����б�
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
        /// Windowsϵͳ��Ϣ
        /// </summary>

        //private ECWMI _WindowsSystemInfo;
        public ECWMI WindowsSystemInfo { get; set; } = new ECWMI();

        /// <summary>
        /// ϵͳTCP�������Ƿ���
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
        /// �Ƿ񾯱���ʾ
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

        #endregion ����

    }
}