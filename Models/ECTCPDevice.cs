using Cognex.VisionPro.ToolBlock;
using Cognex.VisionPro;
using GalaSoft.MvvmLight;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using VPDLFramework.ViewModels;
using GalaSoft.MvvmLight.Command;
using SimpleTCP;
using System.Net;
using GalaSoft.MvvmLight.Messaging;
using System.Net.Sockets;

namespace VPDLFramework.Models
{
    public class ECTCPDevice:ObservableObject
    {
        public ECTCPDevice(string workName,string tcpName) 
        {
            _workName = workName;
            _tcpName = tcpName;
            IsTCPOpened = false;
            IsReconnecting = false;
        }

        #region 字段

        /// <summary>
        /// 工作名
        /// </summary>
        private string _workName;

        /// <summary>
        /// 工作流名称
        /// </summary>
        private string _tcpName;
        
        /// <summary>
        /// TCP客户端
        /// </summary>
        private SimpleTcpClient _tcpClient;

        /// <summary>
        /// TCP服务器
        /// </summary>
        private SimpleTcpServer _tcpServer;

        /// <summary>
        /// 客户端连接时本地的IP端口
        /// </summary>
        public EndPoint LocalIPPort;

        #endregion

        #region 方法
        /// <summary>
        /// 加载工作流配置文件，检查配置是否正常,正常返回True，否则返回False
        /// </summary>
        /// <returns>初始化成功返回True，否则返回False</returns>
        public bool LoadConfig()
        {

            try
            {
                // 加载配置信息
                string jsonPath = $"{ECFileConstantsManager.RootFolder}\\{ _workName}\\{ECFileConstantsManager.TCPConfigFolderName}\\{_tcpName}\\{ECFileConstantsManager.TCPConfigName}";

                //反序列化Json文件
                TCPDeviceInfo = ECSerializer.LoadObjectFromJson<ECTCPDeviceInfo>(jsonPath);

                if (TCPDeviceInfo == null)
                    return false;

                return true;
            }
            catch (System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, LogLevel.Error);
                return false;
            }

        }

        /// <summary>
        /// 创建新的配置
        /// </summary>
        /// <returns>初始化成功返回True，否则返回False</returns>
        public bool CreateNewConfig()
        {
            try
            {
                // 加载配置信息
                TCPDeviceInfo = new ECTCPDeviceInfo(_tcpName);

                return true;
            }
            catch (System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, LogLevel.Error);
                return false;
            }

        }

        /// <summary>
        /// 打开TCP
        /// </summary>
        public void OpenTCP()
        {
            if (TCPDeviceInfo.Type == null) return;
            try
            {
                if (TCPDeviceInfo.Type == Enum.GetName(typeof(ECTCPType.TCPTypeConstants), ECTCPType.TCPTypeConstants.TCPServer))
                {
                    _tcpServer = new SimpleTcpServer();
                    _tcpServer.DataReceived += TCP_DataReceived;
                    _tcpServer.ClientConnected += _tcpServer_ClientConnected;
                    _tcpServer.ClientDisconnected += _tcpServer_ClientDisconnected;
                    _tcpServer.Start(System.Net.IPAddress.Parse(TCPDeviceInfo.IPAddress), TCPDeviceInfo.Port);
                }
                else if (TCPDeviceInfo.Type == Enum.GetName(typeof(ECTCPType.TCPTypeConstants), ECTCPType.TCPTypeConstants.TCPClient))
                {
                    _tcpClient = new SimpleTcpClient();
                    _tcpClient.Connect(TCPDeviceInfo.IPAddress, TCPDeviceInfo.Port);
                    _tcpClient.DataReceived += TCP_DataReceived;
                    LocalIPPort = _tcpClient.TcpClient.Client.LocalEndPoint;
                }
                IsTCPOpened = true;
            }
            catch(Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, LogLevel.Error);
            }
        }

        /// <summary>
        /// 客户端断开连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _tcpServer_ClientDisconnected(object sender, System.Net.Sockets.TcpClient e)
        {
            ECLog.WriteToLog($"\"{TCPDeviceInfo.TCPDeviceName}\""+$" {ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.ClientDisconnected)} {e.Client.RemoteEndPoint.ToString()}", LogLevel.Warn);
        }

        /// <summary>
        /// 客户端连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _tcpServer_ClientConnected(object sender, System.Net.Sockets.TcpClient e)
        {
            ECLog.WriteToLog($"\"{TCPDeviceInfo.TCPDeviceName}\"" + $" {ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.ClientConnected)} {e.Client.RemoteEndPoint.ToString()}", LogLevel.Trace);
        }

        /// <summary>
        /// 收到结果
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TCP_DataReceived(object sender, Message e)
        {
            if(e.MessageString!=null&&e.MessageString.Trim()!="")
            {
                TCPMsg = e.MessageString;
                Messenger.Default.Send<KeyValuePair<string,string>>
                    (new KeyValuePair<string, string>(this.TCPDeviceInfo.TCPDeviceName, TCPMsg),ECMessengerManager.ThirdCardMessageKeys.TCPMessageCome);
                DataReceived?.Invoke(this, TCPMsg);
            }
        }

        /// <summary>
        ///  发送消息
        /// </summary>
        /// <param name="msg"></param>
        public void SendMessage(string msg)
        {
            try
            {
                if (msg != null)
                {
                    if (TCPDeviceInfo.Type == Enum.GetName(typeof(ECTCPType.TCPTypeConstants), ECTCPType.TCPTypeConstants.TCPServer))
                    {
                        _tcpServer?.Broadcast(msg);
                    }
                    else
                    {
                        _tcpClient?.Write(msg);
                    }
                }
            }
            catch (Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, LogLevel.Error);
            }
        }

        /// <summary>
        /// 当TCP类型时客户端时,进行掉线重连的方法
        /// </summary>
        public void ReconnectTCPClient()
        {
            try
            {
                if (TCPDeviceInfo.Type == Enum.GetName(typeof(ECTCPType.TCPTypeConstants), ECTCPType.TCPTypeConstants.TCPClient))
                {
                    if (!IsReconnecting)
                    {
                        IsReconnecting = true;
                        ECLog.WriteToLog($"TCP Reconnecting:{TCPDeviceInfo.IPAddress}:{TCPDeviceInfo.Port}",NLog.LogLevel.Trace);
                        _tcpClient?.Disconnect();
                        OpenTCP();
                        IsReconnecting =false;
                    }
                }
            }
            catch (Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace+ex.Message+$"TCP Reconnecting With Error:{TCPDeviceInfo.IPAddress}:{TCPDeviceInfo.Port}", NLog.LogLevel.Trace);
            }
        }

        /// <summary>
        /// 注销TCP
        /// </summary>
        public void Dispose()
        {
             _tcpClient?.Disconnect();
             _tcpServer?.Stop();
            IsTCPOpened = false;
        }
        #endregion

        #region 事件

        public event EventHandler<string> DataReceived;

        #endregion

        #region 属性
        /// <summary>
        /// TCP信息
        /// </summary>
        private ECTCPDeviceInfo _TCPDeviceInfo;

        public ECTCPDeviceInfo TCPDeviceInfo
        {
            get { return _TCPDeviceInfo; }
            set
            {
                _TCPDeviceInfo = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// TCP是否打开
        /// </summary>
        private bool _isTCPOpened;

        public bool IsTCPOpened
        {
            get { return _isTCPOpened; }
            set { _isTCPOpened = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// TCP收到的信息
        /// </summary>
        private string _TCPMsg;

        public string TCPMsg
        {
            get { return _TCPMsg; }
            set { _TCPMsg = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 正在重连
        /// </summary>
        public bool IsReconnecting { get; set; }
        #endregion
    }
}
