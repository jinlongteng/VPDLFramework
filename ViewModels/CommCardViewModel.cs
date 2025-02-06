using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cognex.VisionPro.Comm;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using ValueConverters;
using VPDLFramework.Models;
using VPDLFramework.Views;

namespace VPDLFramework.ViewModels
{
    public class CommCardViewModel : ViewModelBase
    {
        public CommCardViewModel()
        {
            RegisterMessenger();
            CommCard = new ECCommCard();
            FfpTypeContstantsBindableList = ECGeneric.GetConstantsBindableList<CogFfpProtocolConstants>();
            FfpNdmStatusBitBindableList = ECGeneric.GetConstantsBindableList<ECCommCardFfpNdm.FfpNdmStatusBitConstants>();
            Messages = new BindingList<string>();
            InspectionResultOffset = 0;
            UserDataOffset=0;
            if (ECCommCard.Bank0 != null)
            {
                IsCardExist = true;
                SerialNo = ECCommCard.Bank0.SerialNumber;
            }
            BindCmd();
        }

        private System.Timers.Timer _timer=new System.Timers.Timer(100);

        #region 命令
        /// <summary>
        /// 命令：写入检测结果
        /// </summary>
        public RelayCommand CmdWriteInspectionResult { get; set; }

        /// <summary>
        /// 命令：写入状态位
        /// </summary>
        public RelayCommand<string> CmdWriteStatusBit { get; set; }

        /// <summary>
        /// 命令：读取用户数据
        /// </summary>
        public RelayCommand CmdReadUserData { get; set; }

        /// <summary>
        /// 命令：清空消息
        /// </summary>
        public RelayCommand CmdClearMessages { get; set; }

        /// <summary>
        /// 命令：强制输出
        /// </summary>
        public RelayCommand<int> CmdForceOutput { get; set; }

        #endregion

        #region 方法
        /// <summary>
        /// 注册消息
        /// </summary>
        private void RegisterMessenger()
        {
            Messenger.Default.Register<string>(this, ECMessengerManager.CommCardViewModelMessengerKeys.CommTest, OnCommTest);
            Messenger.Default.Register<int>(this, ECMessengerManager.MainViewModelMessengerKeys.WorkLoaded, OnWorkLoaded);
            Messenger.Default.Register<int>(this, ECMessengerManager.MainViewModelMessengerKeys.UnLoadWork, OnUnLoadWork) ;
            Messenger.Default.Register<string>(this, ECMessengerManager.MainViewModelMessengerKeys.SystemOnline, OnSystemOnline);
            Messenger.Default.Register<string>(this, ECMessengerManager.MainViewModelMessengerKeys.SystemOffline, OnSystemOffline);
            Messenger.Default.Register<bool>(this, ECMessengerManager.ECNativeModeCommandMessengerKeys.LoadRecipeAck, OnLoadRecipeAck);
            Messenger.Default.Register<bool>(this, ECMessengerManager.ECNativeModeCommandMessengerKeys.SetUserDataAck, OnSetUserDataAck);
        }

        /// <summary>
        /// 设置用户数据确认
        /// </summary>
        /// <param name="obj"></param>
        private void OnSetUserDataAck(bool obj)
        {
            ECCommCard.FfpNdm?.NotifySetUserDataAck(obj);
        }

        /// <summary>
        /// 加载配方确认
        /// </summary>
        /// <param name="obj"></param>
        private void OnLoadRecipeAck(bool obj)
        {
            ECCommCard.FfpNdm?.NotifyLoadRecipeAck(obj);
        }

        /// <summary>
        /// 系统脱机
        /// </summary>
        /// <param name="obj"></param>
        private void OnSystemOffline(string obj)
        {
            ECCommCard.FfpNdm?.WriteStatusBit(ECCommCardFfpNdm.FfpNdmStatusBitConstants.Offline);
        }

        /// <summary>
        /// 系统联机
        /// </summary>
        /// <param name="obj"></param>
        private void OnSystemOnline(string obj)
        {
            ECCommCard.FfpNdm?.WriteStatusBit(ECCommCardFfpNdm.FfpNdmStatusBitConstants.Online);
        }

        /// <summary>
        /// 卸载工作
        /// </summary>
        /// <param name="obj">工作ID</param>
        private void OnUnLoadWork(int obj)
        {
            ECCommCard.FfpNdm?.SetJobLoadedComplete(-1);
        }

        /// <summary>
        /// 工作已加载
        /// </summary>
        /// <param name="obj">工作ID</param>
        private void OnWorkLoaded(int obj)
        {
            ECCommCard.FfpNdm?.SetJobLoadedComplete(obj);
        }

        /// <summary>
        /// 绑定命令
        /// </summary>
        private void BindCmd()
        {
            CmdWriteInspectionResult = new RelayCommand(WriteInspectionResult);
            CmdReadUserData = new RelayCommand(ReadUserData);
            CmdWriteStatusBit = new RelayCommand<string>(WriteFfpStatusBit);
            CmdClearMessages = new RelayCommand(ClearMessages);
            CmdForceOutput = new RelayCommand<int>(ForceOutput);
        }

        /// <summary>
        /// 测试工作的通讯
        /// </summary>
        /// <param name="workName"></param>
        private void OnCommTest(string workName)
        {
            AddTestEvent();
            StartTimer();
            Window_CommCard window_CommCard = new Window_CommCard();
            window_CommCard.ShowDialog();
            StopTimer();
            RemoveTestEvent();
        }

        /// <summary>
        /// 开启定时器
        /// </summary>
        private void StartTimer()
        {
            _timer.Elapsed += _timer_Elapsed;
            _timer.Start();
        }

        /// <summary>
        /// 停止计时器
        /// </summary>
        private void StopTimer()
        {
            _timer.Stop();
            _timer.Elapsed-= _timer_Elapsed;
        }

        /// <summary>
        /// 刷新IO状态
        /// </summary>
        private void UpdateIOState()
        {
            if(ECCommCard.Bank0!=null)
            {
                ECCommCard.ReadIOState();
                BindingList<bool> iStates=new BindingList<bool>(ECCommCard.InputState.Values.ToArray());
                BindingList<bool> oStates = new BindingList<bool>(ECCommCard.OutputState.Values.ToArray());
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    IOInputsState = iStates;
                    IOOutputsState = oStates;
                });
            }
        }

        /// <summary>
        /// 定时器事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            UpdateIOState();
        }

        /// <summary>
        /// 发送到PLC
        /// </summary>
        private void WriteInspectionResult()
        {
            try
            {
                if (InspectionResultData == null) return;
                int offset = InspectionResultOffset;
                byte[] result = null;
                if (IsOutputScriptEnable)
                {
                    if (ECCommCard.FfpScriptProcessor != null)
                    {
                        ECCommCard.FfpScriptProcessor.ProcessOutputData(InspectionResultData);
                    }
                    else
                        Messages.Add(DateTime.Now.ToString() + ":  " + "Invalid Script");
                }
                else
                {
                    result = Encoding.Unicode.GetBytes(InspectionResultData);
                }
                if (result != null)
                    ECCommCard.FfpNdm?.WriteInspectionResult(offset, result);
            }
            catch(Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, NLog.LogLevel.Error);
            }
        }

        /// <summary>
        /// 发送到PLC
        /// </summary>
        private void WriteFfpStatusBit(string str)
        {
            if (str == null) return;
            ECCommCardFfpNdm.FfpNdmStatusBitConstants status;
            if(Enum.TryParse(str, out status))
            {
                ECCommCard.FfpNdm?.WriteStatusBit(status);
            }
        }

        /// <summary>
        /// 添加测试事件
        /// </summary>
        private void AddTestEvent()
        {
            if (ECCommCard.Bank0 != null && ECCommCard.IOAccess)
            {
                ECCommCard.InputChanged += CommCardTest_InputChanged;
            }
            if (ECCommCard.Bank0?.FfpAccess?.GetActiveNetworkDataModel() != null)
            {
                ECCommCard.FfpNdm.FfpEvent += CommCardTest_FfpEvent;
            }
        }

        /// <summary>
        /// 删除通讯板卡事件
        /// </summary>
        private void RemoveTestEvent()
        {
            if (ECCommCard.Bank0 != null && ECCommCard.IOAccess)
            {
                ECCommCard.InputChanged -= CommCardTest_InputChanged;
            }

            if (ECCommCard.Bank0?.FfpAccess?.GetActiveNetworkDataModel() != null)
            {
                ECCommCard.FfpNdm.FfpEvent -= CommCardTest_FfpEvent;
            }
        }

        /// <summary>
        /// 通讯测试IO输入事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CommCardTest_InputChanged(object sender, CogPrioEventArgs e)
        {
            DispatcherHelper.UIDispatcher.Invoke(() =>
            {
                Messages.Add(DateTime.Now.ToString() + ":  " + "IO," + e.EventName);
            });

        }

        /// <summary>
        /// 通讯测试Ffp事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CommCardTest_FfpEvent(object sender, KeyValuePair<ECCommCardFfpNdm.FfpNdmConrolBitConstants, string> e)
        {
            DispatcherHelper.UIDispatcher.Invoke(() =>
            {
                Messages.Add(DateTime.Now.ToString() + ":  " + Enum.GetName(typeof(ECCommCardFfpNdm.FfpNdmConrolBitConstants), e.Key) + "," + e.Value);
            });

        }

        /// <summary>
        /// 读取用户数据
        /// </summary>
        private void ReadUserData()
        {
            try
            {
                if (UserDataOffset >= 0 && UserDataSize > 0)
                {
                    byte[] bytes = ECCommCard.FfpNdm?.ReadUserData(UserDataOffset, UserDataSize);
                    string str = "";
                    if (IsInputScriptEnable)
                    {
                        if (ECCommCard.FfpScriptProcessor != null)
                            ECCommCard.FfpScriptProcessor.ProcessInputData(bytes);
                        else
                            Messages.Add(DateTime.Now.ToString() + ":  " + "Invalid Script");
                    }
                    else
                    {
                        foreach (byte b in bytes)
                        {
                            str += ((short)b).ToString() + " ";
                        }
                    }
                    UserDataStr = str;
                }
            }
            catch(Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, NLog.LogLevel.Error);
            }
        }

        /// <summary>
        /// 强制输出
        /// </summary>
        /// <param name="index"></param>
        private void ForceOutput(int index)
        {
            if(index>=0)
                ECCommCard.PulseOutput(index);
        }

        /// <summary>
        /// 清空消息
        /// </summary>
        private void ClearMessages()
        {
            Messages.Clear();
        }
        #endregion

        #region 属性

        /// <summary>
        /// 通讯卡
        /// </summary>
        private ECCommCard _commCard;

        public ECCommCard CommCard
        {
            get { return _commCard; }
            set
            {
                _commCard = value;
                RaisePropertyChanged();
            }
        }


        /// <summary>
        /// 工厂协议类型选项列表
        /// </summary>
        private BindingList<string> _FfpTypeContstantsBindableList;

        public BindingList<string> FfpTypeContstantsBindableList
        {
            get { return _FfpTypeContstantsBindableList; }
            set
            {
                _FfpTypeContstantsBindableList = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 卡存在
        /// </summary>
        private bool _isCardExist;

        public bool IsCardExist
        {
            get { return _isCardExist; }
            set
            {
                _isCardExist = value;
                RaisePropertyChanged();
            }
        }


        /// <summary>
        /// 序列号
        /// </summary>
        private string _serialNo;

        public string SerialNo
        {
            get { return _serialNo; }
            set
            {
                _serialNo = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 消息集合
        /// </summary>
        private BindingList<string> _messages;

        public BindingList<string> Messages
        {
            get { return _messages; }
            set
            {
                _messages = value;
                RaisePropertyChanged();
            }
        }


        /// <summary>
        /// 检测结果偏移
        /// </summary>
        private int _inspectionResultOffset;

        public int InspectionResultOffset
        {
            get { return _inspectionResultOffset; }
            set
            {
                _inspectionResultOffset = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 检测结果数据
        /// </summary>
        private string _inspectionResultData;

        public string InspectionResultData
        {
            get { return _inspectionResultData; }
            set
            {
                _inspectionResultData = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 用户数据偏移
        /// </summary>
        private int _userDataOffset;

        public int UserDataOffset
        {
            get { return _userDataOffset; }
            set { _userDataOffset = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 用户数据字节大小
        /// </summary>
        private int _userDataSize;

        public int UserDataSize
        {
            get { return _userDataSize; }
            set { _userDataSize = value; }
        }

        /// <summary>
        /// Ffp状态位列表
        /// </summary>
        private BindingList<string> _FfpNdmStatusBitBindableList;

        public BindingList<string> FfpNdmStatusBitBindableList
        {
            get { return _FfpNdmStatusBitBindableList; }
            set {
                _FfpNdmStatusBitBindableList = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 用户数据字符串
        /// </summary>
        private string _userDataStr;

        public string UserDataStr
        {
            get { return _userDataStr; }
            set { _userDataStr = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 输入脚本启用
        /// </summary>
        private bool _isInputScriptEnable;

        public bool IsInputScriptEnable
        {
            get { return _isInputScriptEnable; }
            set { _isInputScriptEnable = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 输出脚本启用
        /// </summary>
        private bool _isOutputScriptEnable;

        public bool IsOutputScriptEnable
        {
            get { return _isOutputScriptEnable; }
            set {
                _isOutputScriptEnable = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// IO输入状态
        /// </summary>
        private BindingList<bool> _IOInputsState;

        public BindingList<bool> IOInputsState
        {
            get { return _IOInputsState; }
            set { _IOInputsState = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// IO输出状态
        /// </summary>
        private BindingList<bool> _IOOutputsState;

        public BindingList<bool> IOOutputsState
        {
            get { return _IOOutputsState; }
            set { _IOOutputsState = value;
                RaisePropertyChanged();
            }
        }

        #endregion
    }
}
