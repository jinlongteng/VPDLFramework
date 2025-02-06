using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;
using System.Web.Management;
using VPDLFramework.Views;
using VPDLFrameworkLib;
using CSScriptLib;
using System.ComponentModel;
using GalaSoft.MvvmLight.Messaging;

namespace VPDLFramework.Models
{
    public class ECThirdCard:IDisposable
    {
        public ECThirdCard(string workName) 
        {
            _workName = workName;
            GetScriptMethod(workName);
            RegisterMessenger();
        }

        #region 方法
        /// <summary>
        /// 注册消息
        /// </summary>
        private void RegisterMessenger()
        {
            Messenger.Default.Register<KeyValuePair<string, string>>(this, ECMessengerManager.ThirdCardMessageKeys.TCPMessageCome, OnTCPMessageCome);
            Messenger.Default.Register<KeyValuePair<string, bool>>(this, ECMessengerManager.ThirdCardMessageKeys.ImageSourceCompleted, OnImageSourceCompleted);
        }

        /// <summary>
        /// 注销消息
        /// </summary>
        private void UnRegisterMessenger()
        {
            Messenger.Default.Unregister<KeyValuePair<string, string>>(this, ECMessengerManager.ThirdCardMessageKeys.TCPMessageCome, OnTCPMessageCome);
            Messenger.Default.Unregister<KeyValuePair<string, bool>>(this, ECMessengerManager.ThirdCardMessageKeys.ImageSourceCompleted, OnImageSourceCompleted);
        }

        /// <summary>
        /// 收到TCP消息
        /// </summary>
        /// <param name="obj"></param>
        private void OnTCPMessageCome(KeyValuePair<string, string> obj)
        {
            if (!string.IsNullOrEmpty(obj.Key) && !string.IsNullOrEmpty(obj.Value))
                _scriptPocessor?.OnTCPMessageCome(obj.Key, obj.Value);
        }

        /// <summary>
        /// 图像源取像完成
        /// </summary>
        /// <param name="obj"></param>
        private void OnImageSourceCompleted(KeyValuePair<string, bool> obj)
        {
            if (!string.IsNullOrEmpty(obj.Key))
                _scriptPocessor?.OnImageSourceCompelted(obj.Key,obj.Value);
        }

        /// <summary>
        /// 初始化定时器
        /// </summary>
        private void InitialTimer()
        {
            _timer = new System.Timers.Timer(20);
            _timer.Elapsed += _timer_Elapsed;
            _timer.Start();
        }

        /// <summary>
        /// 定时器事件方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                if (!_isReading)
                {
                    _isReading = true;
                    string str = _scriptPocessor?.ReadInputStatus();
                    if (!string.IsNullOrEmpty(str))
                        InputStateChanged?.BeginInvoke(this, str, null, null);
                    _isReading = false;
                }
            }
            catch (Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, NLog.LogLevel.Error);
            }
        }

        /// <summary>
        /// 设置输出状态
        /// </summary>
        /// <param name="result"></param>
        public void SetOutputStatus(string result)
        {
            try
            {
                Task.Run(() =>
                {
                    _scriptPocessor?.WriteOutputStatus(result);
                });
                
            }
            catch (Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, NLog.LogLevel.Error);
            }
        }

        /// <summary>
        /// 获取脚本方法
        /// </summary>
        public void GetScriptMethod(string workName)
        {
            try
            {
                string scriptPath = $"{ECFileConstantsManager.RootFolder}\\{workName}\\{ECFileConstantsManager.CommCardConfigFolderName}\\{ECFileConstantsManager.ThirdCardScriptName}";

                CSScript.GlobalSettings.AddSearchDir(Directory.GetCurrentDirectory());

                if (File.Exists(scriptPath))
                {
                    ECScriptConfigInfo configInfo= ReadAssemblyConfig();
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

                    _scriptPocessor = evaluator.LoadFile<IECThirdCardScriptProcessor>(scriptPath);

                    if(_scriptPocessor != null)
                    {
                        _scriptPocessor.InputChanged += _inputProcessor_InputChanged;
                        _scriptPocessor.SendTCPMessage += _scriptPocessor_SendTCPMessage;
                        _scriptPocessor.PrintLog += _scriptPocessor_PrintLog;
                        
                        if (_scriptPocessor.IsEnableTimer)
                            InitialTimer();
                    }
                }
            }
            catch (Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message,NLog.LogLevel.Error);
            }
        }

        /// <summary>
        /// 打印一条Log信息到系统日志
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _scriptPocessor_PrintLog(object sender, string e)
        {
            if(!string.IsNullOrEmpty(e))
                ECLog.WriteToLog(e,NLog.LogLevel.Debug);
        }

        /// <summary>
        /// 发送TCP消息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _scriptPocessor_SendTCPMessage(object sender, KeyValuePair<string, string> e)
        {
            if (!string.IsNullOrEmpty(e.Key) && !string.IsNullOrEmpty(e.Value))
                Messenger.Default.Send<KeyValuePair<string, string>>(e, ECMessengerManager.ThirdCardMessageKeys.SendTCPMessage);
        }

        /// <summary>
        /// 读取输入脚本引用配置
        /// </summary>
        private ECScriptConfigInfo ReadAssemblyConfig()
        {
            string path = $"{ECFileConstantsManager.RootFolder}\\{_workName}\\" +
                $"{ECFileConstantsManager.CommCardConfigFolderName}\\{ECFileConstantsManager.ThirdCardScriptConfigName}";
            if (File.Exists(path))
                return ECSerializer.LoadObjectFromJson<ECScriptConfigInfo>(path);
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _inputProcessor_InputChanged(object sender, string e)
        {
            if (!string.IsNullOrEmpty(e))
                InputStateChanged?.BeginInvoke(this, e, null, null);
        }

        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            UnRegisterMessenger();
            _scriptPocessor?.Dispose();
            _timer?.Stop();
            _timer?.Dispose();
        }
        #endregion

        #region 字段

        private string _workName;

        /// <summary>
        /// 定时器
        /// </summary>
        private System.Timers.Timer  _timer;
    
        /// <summary>
        /// 正在读取
        /// </summary>
        private bool _isReading = false;

        /// <summary>
        /// 输入处理器
        /// </summary>
        private IECThirdCardScriptProcessor _scriptPocessor;

        /// <summary>
        /// 输入状态更改事件
        /// </summary>
        public event EventHandler<string> InputStateChanged;

        #endregion
    }
}
