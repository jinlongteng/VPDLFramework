using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;
using Cognex.Vision;
using Cognex.VisionPro.ToolBlock;
using Cognex.VisionPro;
using System.Runtime.Serialization.Formatters.Binary;
using ViDi2;
using System.Diagnostics;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Net;
using VPDLFramework.Models;

namespace VPDLFramework
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        SimpleTCP.SimpleTcpServer server = new SimpleTCP.SimpleTcpServer();

        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                server.Start(IPAddress.Parse(ECStartupSettings.Instance().SystemTCPServerIP), 3000);
                server.ClientConnected += Server_ClientConnected;
                server.ClientDisconnected += Server_ClientDisconnected;
                server.DataReceived += Server_DataReceived;
            }
            catch (System.Exception exc)
            {
                ECLog.WriteToLog($"Start server monitor LightController failed!{exc}",NLog.LogLevel.Error);
            }
            
            


            base.OnStartup(e);
        }

        private void Server_ClientDisconnected(object sender, TcpClient e)
        {
            Debug.WriteLine($"Local:{e.Client.LocalEndPoint},remote:{e.Client.RemoteEndPoint} is disconnected");
        }

        private void Server_DataReceived(object sender, SimpleTCP.Message e)
        {
            Debug.WriteLine($"Received {e.MessageString} from {e.TcpClient.Client.RemoteEndPoint}");
            var lightControllers = NetworkInterface.GetAllNetworkInterfaces().
                Where(net => net.Name.ToLower().Contains("light")).OrderBy(p=>p.Name);
            string lightState = string.Empty;
            foreach (var lightController in lightControllers)
            {
                if (lightController.OperationalStatus== OperationalStatus.Up)
                {
                    lightState += "Online,";
                }
                else 
                {
                    lightState += "Offline,";
                }
            }
            e.Reply(lightState);

        }

        private void Server_ClientConnected(object sender, TcpClient e)
        {
            Debug.WriteLine($"Local:{e.Client.LocalEndPoint},remote:{e.Client.RemoteEndPoint} is connected");
        }



        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
        }
    }
}
