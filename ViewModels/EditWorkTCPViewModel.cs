using GalaSoft.MvvmLight.Messaging;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using SimpleTCP;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VPDLFramework.Models;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight;
using System.IO;
using System.ComponentModel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using NLog;

namespace VPDLFramework.ViewModels
{
    public class EditWorkTCPViewModel:ViewModelBase
    {
        /// <summary>
        /// TCP通讯设置界面视图模型
        /// </summary>
        /// <param name="workName"></param>
        public EditWorkTCPViewModel(string workName)
        {
            BindCmd();
            GetLocalIPs();
            _workName = workName;
            GetExistConfig();
        }

        #region 命令

        /// <summary>
        /// 命令：添加TCP服务器
        /// </summary>
        public RelayCommand<object> CmdAddTCPDevice { get; set; }

        /// <summary>
        /// 命令：删除TCP服务器
        /// </summary>
        public RelayCommand<object> CmdRemoveTCPDevice { get; set; }


        #endregion 命令

        #region 方法
        /// <summary>
        /// 绑定命令
        /// </summary>
        private void BindCmd()
        {
            CmdAddTCPDevice = new RelayCommand<object>(AddTCPDevice);
            CmdRemoveTCPDevice = new RelayCommand<object>(RemoveTCPDevice);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            foreach (var item in WorkTCPItemList)
            {
                item.TCPDevice.Dispose();
            }
        }

        /// <summary>
        /// 获取本机可用的IP
        /// </summary>
        private void GetLocalIPs()
        {
            List<string> ips = ECGeneric.GetLocalIPs();
            LocalIPs = new BindingList<string>();
            foreach (var ip in ips)
            {
                LocalIPs.Add(ip);
            }
        }

        /// <summary>
        /// 删除TCP设备
        /// </summary>
        /// <param name="obj"></param>
        private void RemoveTCPDevice(object obj)
        {
            if (obj == null) return;
            if (ECDialogManager.Verify($"{ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.ConfirmRemove)} \"{(obj as WorkTCPItemViewModel).TCPDevice.TCPDeviceInfo.TCPDeviceName}\""))
            {
                WorkTCPItemViewModel tcpViewModel = obj as WorkTCPItemViewModel;
                WorkTCPItemList.Remove(tcpViewModel);
            }
        }

        /// <summary>
        /// 添加TCP设备
        /// </summary>
        /// <param name="obj"></param>
        private void AddTCPDevice(object obj)
        {
            string name = ECDialogManager.GetUserInput(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.PleaseInputName));
            if (name == null || name.Trim() == "") return;
            if ((new Regex(@"^\w+$").IsMatch(name)))
            {
                // 检查是否存在重复名称
                if (!CheckNameUniqueness(name))
                {
                    ECDialogManager.ShowMsg(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.PleaseInputName));
                    return;
                }
                WorkTCPItemViewModel viewModel=  new WorkTCPItemViewModel(_workName, name, LocalIPs);
                viewModel.TCPDevice.CreateNewConfig();
                WorkTCPItemList.Add(viewModel);
            }
            else
                ECDialogManager.ShowMsg(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.InvalidName));
        }

        /// <summary>
        /// 保存配置
        /// </summary>
        public bool SaveConfig()
        {
            try
            {
                string folder = $"{ECFileConstantsManager.RootFolder}\\{_workName}\\{ECFileConstantsManager.TCPConfigFolderName}";
                if (Directory.Exists(folder))
                {
                    string[] folders = Directory.GetDirectories(folder);
                    foreach (string diretory in folders)
                    {
                        bool bDelete = true;
                        foreach (var item in WorkTCPItemList)
                        {
                            if (item.TCPDevice.TCPDeviceInfo.TCPDeviceName == new DirectoryInfo(diretory).Name)
                            {
                                bDelete = false;
                                break;
                            }
                        }
                        if (bDelete)
                            Directory.Delete(diretory, true);
                    }
                }
                foreach (var item in _workTCPItemList)
                {
                    string directory = $"{ECFileConstantsManager.RootFolder}\\{_workName}\\{ECFileConstantsManager.TCPConfigFolderName}\\{item.TCPDevice.TCPDeviceInfo.TCPDeviceName}";
                    string jsonPath = directory + $"\\{ECFileConstantsManager.TCPConfigName}";
                    if (!Directory.Exists(directory)) { Directory.CreateDirectory(directory); }
                    ECSerializer.SaveObjectToJson(jsonPath, item.TCPDevice.TCPDeviceInfo);
                }
                return true;
            }
            catch(Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message,LogLevel.Error);
                return false;
            }
        }

        /// <summary>
        /// 加载TCP客户端配置
        /// </summary>
        public void GetExistConfig()
        {
            try
            {
                WorkTCPItemList = new BindingList<WorkTCPItemViewModel>();

                string folder = $"{ECFileConstantsManager.RootFolder}\\{_workName}\\{ECFileConstantsManager.TCPConfigFolderName}";
                if (Directory.Exists(folder))
                {
                    string[] folders = Directory.GetDirectories(folder);
                    foreach (string item in folders)
                    {
                        if (File.Exists($"{item}\\{ECFileConstantsManager.TCPConfigName}"))
                        {
                            WorkTCPItemViewModel viewModel = new WorkTCPItemViewModel(_workName, new DirectoryInfo(item).Name, LocalIPs);
                            viewModel.TCPDevice.LoadConfig();
                            WorkTCPItemList.Add(viewModel);
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace+ex.Message,LogLevel.Error);
            }
        }

        /// <summary>
        /// 检查是否重名
        /// </summary>
        private bool CheckNameUniqueness(string name)
        {
            foreach (var item in WorkTCPItemList)
            {
                if (item.TCPDevice.TCPDeviceInfo.TCPDeviceName == name)
                {
                    return false;
                }
            }
            return true;
        }

        #endregion

        #region 字段

        /// <summary>
        /// 工作名称
        /// </summary>
        private string _workName;

        #endregion 字段

        #region 属性

        private BindingList<WorkTCPItemViewModel> _workTCPItemList;

        public BindingList<WorkTCPItemViewModel> WorkTCPItemList
        {
            get { return _workTCPItemList; }
            set { _workTCPItemList = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 选择的TCP服务器
        /// </summary>
        private int selectTCPServer;

        /// <summary>
        /// 选择的TCP服务器
        /// </summary>
        public int SelectTCPServer
        {
            get { return selectTCPServer; }
            set { selectTCPServer = value; }
        }


        /// <summary>
        /// 本机可用的IP地址
        /// </summary>
        private BindingList<string> _localIPs;

        public BindingList<string> LocalIPs
        {
            get { return _localIPs; }
            set
            {
                _localIPs = value;
                RaisePropertyChanged();
            }
        }

        #endregion 属性
    }
}
