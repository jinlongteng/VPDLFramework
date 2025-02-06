using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using SimpleTCP;

namespace VPDLFramework.Models
{
    public class ECTCPMonitor
    {
        /// <summary>
        /// TCP监视器类,用于监视TCP设备状态
        /// </summary>
        /// <param name="devicesList">设备列表</param>
        public ECTCPMonitor(List<ECTCPDevice> devicesList)
        {
            _TCPDevicesList = devicesList;
            TCPDevicesStatus = new Dictionary<string, bool>();
            foreach (ECTCPDevice device in _TCPDevicesList)
                TCPDevicesStatus.Add(device.TCPDeviceInfo.TCPDeviceName, true);
        }

        /// <summary>
        /// 检测TCP设备的连接状态
        /// </summary>
        public void DetectTCPDevicesStatus()
        {
            CheckTCPServersStatus();
            CheckTCPClientStatus();
        }

        /// <summary>
        /// 检查TCP服务器状态
        /// </summary>
        private void CheckTCPServersStatus()
        {
            try
            {
                IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
                IPEndPoint[] tcpConnInfoArray = ipGlobalProperties.GetActiveTcpListeners();
                foreach (ECTCPDevice device in _TCPDevicesList)
                {
                    if (device.TCPDeviceInfo.Type == Enum.GetName(typeof(ECTCPType.TCPTypeConstants), ECTCPType.TCPTypeConstants.TCPServer))
                    {
                        IPEndPoint tcpServer= new IPEndPoint(IPAddress.Parse(device.TCPDeviceInfo.IPAddress), device.TCPDeviceInfo.Port);
                        if (tcpConnInfoArray.Contains(tcpServer))
                            TCPDevicesStatus[device.TCPDeviceInfo.TCPDeviceName] = true;
                        else
                            TCPDevicesStatus[device.TCPDeviceInfo.TCPDeviceName] = false;
                    }
                }
            }
            catch(Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, NLog.LogLevel.Error);
            }
        }

        /// <summary>
        /// 检查TCP客户端状态
        /// </summary>
        private void CheckTCPClientStatus()
        {
            try
            {
                IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
                TcpConnectionInformation[] tcpConnections = ipGlobalProperties.GetActiveTcpConnections();
                //遍历所有的TCP客户端和TCP连接信息
                foreach (ECTCPDevice device in _TCPDevicesList)
                {
                    if (device.TCPDeviceInfo.Type == Enum.GetName(typeof(ECTCPType.TCPTypeConstants), ECTCPType.TCPTypeConstants.TCPClient))
                    {
                        IPEndPoint remoteIPPort = new IPEndPoint(IPAddress.Parse(device.TCPDeviceInfo.IPAddress), device.TCPDeviceInfo.Port);
                        IPEndPoint localIPPort = device.LocalIPPort as IPEndPoint;
                        if (localIPPort == null)
                        {
                            TCPDevicesStatus[device.TCPDeviceInfo.TCPDeviceName] = false;
                            // 重连
                            Task.Factory.StartNew(() =>
                            {
                                device.ReconnectTCPClient();
                            });
                        }
                        else
                        {
                            foreach (TcpConnectionInformation tcpConnection in tcpConnections)
                            {
                                //本地服务器
                                if ((tcpConnection.LocalEndPoint.Address.ToString() == localIPPort.Address.ToString() && tcpConnection.LocalEndPoint.Port.ToString() ==localIPPort.Port.ToString())&&
                                   (tcpConnection.RemoteEndPoint.Address.ToString() ==remoteIPPort.Address.ToString() && tcpConnection.RemoteEndPoint.Port.ToString() == remoteIPPort.Port.ToString()))
                                {
                                    if (tcpConnection.State == TcpState.Established)
                                        TCPDevicesStatus[device.TCPDeviceInfo.TCPDeviceName] = true;
                                    else
                                    {
                                        TCPDevicesStatus[device.TCPDeviceInfo.TCPDeviceName] = false;
                                        // 重连
                                        Task.Factory.StartNew(() =>
                                        {
                                            device.ReconnectTCPClient();
                                        });
                                    }
                                }
                                //远程服务器
                                else if ((tcpConnection.LocalEndPoint.Address.ToString() == remoteIPPort.Address.ToString() && tcpConnection.LocalEndPoint.Port.ToString() == remoteIPPort.Port.ToString()) &&
                                   (tcpConnection.RemoteEndPoint.Address.ToString() == localIPPort.Address.ToString() && tcpConnection.RemoteEndPoint.Port.ToString() == localIPPort.Port.ToString()))
                                {
                                    if (tcpConnection.State == TcpState.Established)
                                        TCPDevicesStatus[device.TCPDeviceInfo.TCPDeviceName] = true;
                                    else
                                    {
                                        TCPDevicesStatus[device.TCPDeviceInfo.TCPDeviceName] = false;

                                        // 重连
                                        Task.Factory.StartNew(() =>
                                        {
                                            device.ReconnectTCPClient();
                                        });
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, NLog.LogLevel.Error);
            }
        }

        /// <summary>
        /// TCP服务器信息列表
        /// </summary>
        private List<ECTCPDevice> _TCPDevicesList;

        /// <summary>
        /// TCP服务器状态集合
        /// </summary>
        public Dictionary<string, bool> TCPDevicesStatus;
    }
}
