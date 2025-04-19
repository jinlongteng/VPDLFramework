using Cognex.VisionPro.Comm;
using System;
using System.Collections.Generic;
using System.IO;
using System.ComponentModel;
using GalaSoft.MvvmLight;
using NLog;
using GalaSoft.MvvmLight.Messaging;
using System.Web.UI.WebControls;
using CSScriptLib;
using VPDLFrameworkLib;
using Cognex.VisionProUI.ViDiEL.Classify.Controls.Controls;
using System.Reflection;

namespace VPDLFramework.Models
{
    public class ECCommCard : ObservableObject
    {
        static ECCommCard()
        {
            InitCommCard();
        }

        #region 字段

        /// <summary>
        /// 通讯卡集合
        /// </summary>
        private static CogCommCards _mCards;

        /// <summary>
        /// 当前操作的卡
        /// </summary>
        public static CogCommCard Bank0;

        /// <summary>
        /// IO交互的接口
        /// </summary>
        private static CogPrio _mPrio;

        /// <summary>
        /// IO状态
        /// </summary>
        private static CogPrioState _mT0;

        /// <summary>
        /// 卡数量
        /// </summary>
        public static int CardsCount=0;

        /// <summary>
        /// IO状态可用
        /// </summary>
        public static bool IOAccess=false;

        /// <summary>
        /// 工厂协议网络数据模块
        /// </summary>
        public static ECCommCardFfpNdm FfpNdm;

        /// <summary>
        /// 工厂协议输入数据处理器
        /// </summary>
        public static IECFfpScriptProcessor FfpScriptProcessor;

        /// <summary>
        /// IO输入状态
        /// </summary>
        public static Dictionary<int, bool> InputState;

        /// <summary>
        /// IO输出状态
        /// </summary>
        public static Dictionary<int, bool> OutputState;

        #endregion 字段

        #region 事件
        /// <summary>
        /// IO输入事件
        /// </summary>
        public static event EventHandler<CogPrioEventArgs> InputChanged;

        /// <summary>
        /// Ffp输入更改事件
        /// </summary>
        public static event EventHandler<string> FfpInputChanged;
        #endregion

        #region 方法

        /// <summary>
        /// 初始化IO卡
        /// </summary>
        public static void InitCommCard()
        {
            //获取通讯卡集合
            _mCards = new CogCommCards();

            //检查是否没有卡
            if (_mCards.Count == 0)
                return;
            else
            {
                CardsCount = _mCards.Count;

                //选取0号卡
                Bank0 = _mCards[0];

                //检查IO
                if (Bank0.DiscreteIOAccess == null)
                    IOAccess = false;
                else
                    IOAccess = true;

                //创建与IO交互的接口
                _mPrio = Bank0.DiscreteIOAccess.CreatePrecisionIO();

                //禁用事件
                _mPrio.DisableEvents();

                //配置Default事件
                ConfigureDefaultEvents();

                _mPrio.EnableEvents();
            }
        }

        /// <summary>
        /// 释放IO卡资源
        /// </summary>
        public static void DisposeCommCard()
        {
            if (_mCards != null)
            {
                foreach (CogCommCard card in _mCards)
                {
                    card.Dispose();
                }
            }
        }

        /// <summary>
        /// 配置DefaultIO事件
        /// </summary>
        /// <param name="prio">IO交互接口</param>
        public static void ConfigureDefaultEvents()
        {
            try
            {
                //IO事件集合
                CogPrioEventCollection prioEvents = new CogPrioEventCollection();

                //为Input配置事件
                for (int i = 0; i < _mPrio.GetNumLines(CogPrioBankConstants.InputBank0); i++)
                {
                    CogPrioEvent prioEvent = new CogPrioEvent()
                    {
                        //事件名称
                        Name = String.Format("InputChanged_{0}", i),

                        //产生事件的引脚
                        CausesLine = new CogPrioEventCauseLineCollection()
                    {
                        new CogPrioEventCauseLine()
                        {
                          LineBank = CogPrioBankConstants.InputBank0,
                          LineNumber = i,
                          LineTransition = CogPrioLineTransitionConstants.Any
                        }
                    }
                    };

                    //订阅每一个Input事件
                    prioEvent.HostNotification += new CogPrioEventHandler(InputChanged_HostNotification);

                    //添加改事件到事件集合
                    prioEvents.Add(prioEvent);

                }

                //配置Output事件
                for (int i = 0; i < _mPrio.GetNumLines(CogPrioBankConstants.OutputBank0); i++)
                {
                    CogPrioEvent prioEvent = new CogPrioEvent()
                    {
                        //输出事件名称
                        Name = String.Format("PulseOutput_{0}", i),

                        //返回产生事件的引脚
                        ResponsesLine = new CogPrioEventResponseLineCollection()
                    {
                        new CogPrioEventResponseLine()
                        {
                          OutputLineBank = CogPrioBankConstants.OutputBank0,
                          OutputLineNumber = i,
                          OutputLineValue = CogPrioOutputLineValueConstants.SetHigh,
                          PulseDuration = 1000.0,
                          DelayType = CogPrioDelayTypeConstants.None,
                          DelayValue = 0.0
                        }
                    }
                    };

                    //将输出事件添加到事件集合
                    prioEvents.Add(prioEvent);
                }

                //消除所有的事件
                _mPrio.Events.Clear();

                //将事件集合设置到卡的事件集合
                _mPrio.Events = prioEvents;

                //确认事件是否设置成功
                if (!_mPrio.Valid)
                {
                    throw new Exception(_mPrio.ValidationErrorMsg[0]);
                }
            }
            catch(System.Exception ex) 
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, NLog.LogLevel.Error);
            }
        }

        /// <summary>
        /// 输入事件产生
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="e">IO信息</param>
        private static void InputChanged_HostNotification(object sender, CogPrioEventArgs e)
        {
            InputChanged?.BeginInvoke(null,e,null,null);
            string eventName = e.EventName; //"InputChanged_{0}"
            int lineIndex = Convert.ToInt16(eventName.Replace("InputChanged_", ""));
            Messenger.Default.Send<int>(lineIndex, ECMessengerManager.CommCardMessengerKeys.IOTriggerStream);
        }

        /// <summary>
        /// 注册输入输出事件
        /// </summary>
        /// <returns></returns>
        public static bool RegisterInputOutputEvent(List<ECCommCardInputLineInfo> inputItems,List<ECCommCardOutputLineInfo> outputItems)
        {
            _mPrio.DisableEvents();
            //IO事件集合
            CogPrioEventCollection prioEvents = new CogPrioEventCollection();

            if (inputItems != null)
            {
                //为Input配置事件
                foreach (ECCommCardInputLineInfo info in inputItems)
                {
                    ECWorkOptionManager.IOInputSignalTypeConstants signalType;
                    Enum.TryParse(info.SignalType, out signalType);

                    CogPrioEvent prioEvent = new CogPrioEvent();
                    //事件名称
                    prioEvent.Name = String.Format("InputChanged_{0}", info.LineNumber);

                    //产生事件的引脚
                    CogPrioEventCauseLineCollection lines = new CogPrioEventCauseLineCollection();
                    CogPrioEventCauseLine line = new CogPrioEventCauseLine();
                    // 卡
                    line.LineBank = CogPrioBankConstants.InputBank0;

                    // 信号引脚
                    line.LineNumber = info.LineNumber;
                    // 信号类型
                    if (signalType == ECWorkOptionManager.IOInputSignalTypeConstants.Any)
                        line.LineTransition = CogPrioLineTransitionConstants.Any;
                    else if (signalType == ECWorkOptionManager.IOInputSignalTypeConstants.Rise)
                        line.LineTransition = CogPrioLineTransitionConstants.LowToHigh;
                    else if (signalType == ECWorkOptionManager.IOInputSignalTypeConstants.Fall)
                        line.LineTransition = CogPrioLineTransitionConstants.HighToLow;
                    lines.Add(line);

                    prioEvent.CausesLine = lines;

                    //订阅每一个Input事件
                    prioEvent.HostNotification += new CogPrioEventHandler(InputChanged_HostNotification);

                    //添加改事件到事件集合
                    prioEvents.Add(prioEvent);

                }
            }

            if (outputItems != null)
            {
                //配置Output事件
                foreach (ECCommCardOutputLineInfo info in outputItems)
                {
                    ECWorkOptionManager.IOOutputSignalTypeConstants signalType;
                    Enum.TryParse(info.SignalType, out signalType);
                    CogPrioEvent prioEvent = new CogPrioEvent()
                    {
                        //输出事件名称
                        Name = String.Format("PulseOutput_{0}", info.LineNumber),

                        //返回产生事件的引脚
                        ResponsesLine = new CogPrioEventResponseLineCollection()
                    {
                        new CogPrioEventResponseLine()
                        {
                          // 卡
                          OutputLineBank = CogPrioBankConstants.OutputBank0,
                          // 输出引脚
                          OutputLineNumber = info.LineNumber,
                          // 输出电平
                          OutputLineValue =signalType==ECWorkOptionManager.IOOutputSignalTypeConstants.High?CogPrioOutputLineValueConstants.SetHigh:
                                                                    CogPrioOutputLineValueConstants.SetLow,
                          // 脉冲宽度
                          PulseDuration = info.Duration,

                          DelayType = CogPrioDelayTypeConstants.None,
                          DelayValue = 0.0
                        }
                    }
                    };
                    //将输出事件添加到事件集合
                    prioEvents.Add(prioEvent);
                }
            }
            //消除所有的事件
            _mPrio.Events.Clear();

            //将事件集合设置到卡的事件集合
            _mPrio.Events = prioEvents;

            //确认事件是否设置成功
            if (!_mPrio.Valid)
            {
                return false;
            }

            _mPrio.EnableEvents();
            return true;
        }

        /// <summary>
        /// 输出事件产生
        /// </summary>
        /// <param name="outputIndex">输出Pin脚索引</param>
        public static void PulseOutput(int outputIndex)
        {
            try
            {
                //输出脉冲
                _mPrio?.Events["PulseOutput_" + outputIndex.ToString()].Schedule();
            }
            catch (System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, NLog.LogLevel.Error);
            }
        }

        /// <summary>
        /// 读取IO状态
        /// </summary>
        public static void ReadIOState()
        {
            if (!(Bank0 != null && IOAccess))
                return;
            InputState = new Dictionary<int, bool>();
            OutputState = new Dictionary<int, bool>();
            _mT0 = _mPrio.ReadState();
            for (int i = 0; i < 8; i++)
            {
                InputState.Add(i, _mT0[CogPrioBankConstants.InputBank0, i]);
            }
            for (int i = 0; i < 16; i++)
            {
                OutputState.Add(i, _mT0[CogPrioBankConstants.OutputBank0, i]);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static void SetFfpType(CogFfpProtocolConstants type)
        {
            if(Bank0!= null)
            {
                FfpNdm = new ECCommCardFfpNdm(Bank0?.FfpAccess.CreateNetworkDataModel(type));
                AddFfpEvent();
            }
            
        }

        /// <summary>
        /// 添加通讯板卡事件
        /// </summary>
        private static void AddFfpEvent()
        {
            if (ECCommCard.Bank0?.FfpAccess?.GetActiveNetworkDataModel() != null)
            {
                ECCommCard.FfpNdm.OnSoftEvent += FfpNdm_OnSoftEvent;
                ECCommCard.FfpNdm.OnJobChangeRequestedEvent += FfpNdm_OnJobChangeRequestedEvent;
                ECCommCard.FfpNdm.OnSetUserDataEvent += FfpNdm_OnSetUserDataEvent;
                ECCommCard.FfpNdm.OnClearErrorEvent += FfpNdm_OnClearErrorEvent;
                ECCommCard.FfpNdm.SetOfflineEvent += FfpNdm_SetOfflineEvent;
                ECCommCard.FfpNdm.SetOnlineEvent += FfpNdm_SetOnlineEvent;
            }
        }

        #region Ffp事件

        /// <summary>
        /// 设置联机
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void FfpNdm_SetOnlineEvent(object sender, EventArgs e)
        {
            Messenger.Default.Send<bool>(true, ECMessengerManager.CommCardMessengerKeys.FFPSetOnline);
        }

        /// <summary>
        /// 设置脱机
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void FfpNdm_SetOfflineEvent(object sender, EventArgs e)
        {
            Messenger.Default.Send<bool>(false, ECMessengerManager.CommCardMessengerKeys.FFPSetOnline);
        }

        /// <summary>
        /// 清楚错误
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void FfpNdm_OnClearErrorEvent(object sender, EventArgs e)
        {
            Messenger.Default.Send<string>("", ECMessengerManager.CommCardMessengerKeys.FFPClearError);
        }

        /// <summary>
        /// 用户数据事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void FfpNdm_OnSetUserDataEvent(object sender, byte[] e)
        {
            if(FfpScriptProcessor != null)
            {
                FfpScriptProcessor.ProcessInputData(e);
            }
        }

        /// <summary>
        /// 作业切换请求
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void FfpNdm_OnJobChangeRequestedEvent(object sender, int e)
        {
            Messenger.Default.Send<int>(e, ECMessengerManager.CommCardMessengerKeys.FFPJobChangeRequested);
        }

        /// <summary>
        /// 软事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void FfpNdm_OnSoftEvent(object sender, int e)
        {
            Messenger.Default.Send<int>(e, ECMessengerManager.CommCardMessengerKeys.FFPTriggerStream);
        }
        #endregion

        /// <summary>
        /// 获取脚本方法
        /// </summary>
        public static void GetScriptMethod(string workName)
        {
            try
            {
                string inputScriptPath = $"{ECFileConstantsManager.RootFolder}\\{workName}\\{ECFileConstantsManager.CommCardConfigFolderName}\\{ECFileConstantsManager.FfpScriptName}";

                if (File.Exists(inputScriptPath))
                {
                    ECScriptConfigInfo configInfo = ReadAssemblyConfig(workName);
                    if (configInfo == null) return;

                    Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                    // 必须使用创建的接口,才能将添加的dll引用上
                    IEvaluator evaluator = CSScript.Evaluator;
                    evaluator.DebugBuild = configInfo.IsDebugMode;
                    evaluator.ReferenceDomainAssemblies();
                    foreach (string name in configInfo.ScriptAssembly)
                    {
                        bool isAdd = true;
                        foreach (Assembly assembly in assemblies)
                        {
                            if (assembly.ManifestModule.Name == name)
                                isAdd = false;
                        }
                        if (isAdd)
                        {
                            string path = Directory.GetCurrentDirectory() + $"\\{name}";
                            if (File.Exists(path))
                                evaluator.ReferenceAssembly(path);
                        }
                    }
                    FfpScriptProcessor = evaluator.LoadFile<IECFfpScriptProcessor>(inputScriptPath);
                    
                    FfpScriptProcessor.InputChanged += FfpScriptProcessor_InputChanged;
                    FfpScriptProcessor.SendToPLC += FfpScriptProcessor_SendToPLC;
                    FfpScriptProcessor.PrintLog += FfpScriptProcessor_PrintLog;
                }

            }
            catch (Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, LogLevel.Error);
            }
        }

        /// <summary>
        /// 打印一条Log信息到系统日志
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void FfpScriptProcessor_PrintLog(object sender, string e)
        {
            if (!string.IsNullOrEmpty(e))
                ECLog.WriteToLog(e, NLog.LogLevel.Debug);
        }

        /// <summary>
        /// 发送数据到PLC
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void FfpScriptProcessor_SendToPLC(object sender, KeyValuePair<int, byte[]> e)
        {
            try
            {
                ECCommCard.FfpNdm.WriteInspectionResult(e.Key, e.Value);
            }
            catch (System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, NLog.LogLevel.Error);
            }
        }

        /// <summary>
        /// PLC发来数据产生了本地化质量
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void FfpScriptProcessor_InputChanged(object sender, string e)
        {
            if (!string.IsNullOrEmpty(e))
                FfpInputChanged?.Invoke(sender, e);
        }

        /// <summary>
        /// 读取输入脚本引用配置
        /// </summary>
        private static ECScriptConfigInfo ReadAssemblyConfig(string workName)
        {
            string path = $"{ECFileConstantsManager.RootFolder}\\{workName}\\" +
                $"{ECFileConstantsManager.CommCardConfigFolderName}\\{ECFileConstantsManager.FfpScriptConfigName}";
            if (File.Exists(path))
                return ECSerializer.LoadObjectFromJson<ECScriptConfigInfo>(path);
            return null;
        }

        #endregion 方法
    }

}
