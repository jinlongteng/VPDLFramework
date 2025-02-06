using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SimpleTCP;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Documents;
using VPDLFramework.Models;

namespace VPDLFramework.ViewModels
{
    public class WorkTCPItemViewModel : ViewModelBase
    {
        public WorkTCPItemViewModel(string workName,string tcpName,BindingList<string> localIPs) 
        {
            TCPDevice=new ECTCPDevice(workName,tcpName);
            LocalIPBindableList = localIPs;
            TCPTypeConstantsBindableList=new BindingList<string>();
            foreach (string item in Enum.GetNames(typeof(ECTCPType.TCPTypeConstants)))
            {
                TCPTypeConstantsBindableList.Add(item);
            }
            BindCmd();
        }

        #region 方法
        /// <summary>
        /// 绑定命令
        /// </summary>
        private void BindCmd()
        {
            CmdOpenOrCloseTCP = new RelayCommand(OpenOrCloseTCP);
        }

        /// <summary>
        /// 打开TCP
        /// </summary>
        private void OpenOrCloseTCP()
        {
            if(TCPDevice.IsTCPOpened)
                TCPDevice.Dispose();
            else
                TCPDevice.OpenTCP();
        }

        #endregion

        #region 命令

        /// <summary>
        /// 命令：打开TCP连接
        /// </summary>
        public RelayCommand CmdOpenOrCloseTCP { get; set; }

        #endregion

        #region 属性
        /// <summary>
        /// TCP设备
        /// </summary>
        private ECTCPDevice _TCPDevice;

        public ECTCPDevice TCPDevice
        {
            get { return _TCPDevice; }
            set { _TCPDevice = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// TCP类型数据源列表
        /// </summary>
        private BindingList<string>  _TCPTypeConstantsBindableList;

        public BindingList<string> TCPTypeConstantsBindableList
        {
            get { return _TCPTypeConstantsBindableList; }
            set { _TCPTypeConstantsBindableList = value;
                RaisePropertyChanged();
            }
        }

        private BindingList<string> _localIPBindableList;

        public BindingList<string> LocalIPBindableList
        {
            get { return _localIPBindableList; }
            set
            {
                _localIPBindableList = value;
                RaisePropertyChanged();
            }
        }

        #endregion
    }
}
