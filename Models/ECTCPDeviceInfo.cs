using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VPDLFramework.Models
{
    public class ECTCPDeviceInfo : ObservableObject
    {
        /// <summary>
        /// TCP设备信息
        /// </summary>
        /// <param name="name"></param>
        public ECTCPDeviceInfo(string name)
        {
            _TCPDeviceName = name;
        }

        /// <summary>
        /// 设备名称
        /// </summary>
        private string _TCPDeviceName;

        /// <summary>
        /// 设备名称
        /// </summary>
        public string TCPDeviceName
        {
            get { return _TCPDeviceName; }
            set
            {
                _TCPDeviceName = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// IP地址
        /// </summary>
        private string iPAddress;

        /// <summary>
        /// IP地址
        /// </summary>
        public string IPAddress
        {
            get { return iPAddress; }
            set
            {
                iPAddress = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 端口号
        /// </summary>
        private int port;

        /// <summary>
        /// 端口号
        /// </summary>
        public int Port
        {
            get { return port; }
            set
            {
                port = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// TCP类型,0服务器,1客户端
        /// </summary>
        private string type;

        /// <summary>
        /// TCP类型,0服务器,1客户端
        /// </summary>
        public string Type
        {
            get { return type; }
            set
            {
                type = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 状态
        /// </summary>
        private bool _status;

        public bool Status
        {
            get { return _status; }
            set { _status = value;
                RaisePropertyChanged();
            }
        }

    }
}
