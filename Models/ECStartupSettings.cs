using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;

namespace VPDLFramework.Models
{
    public class ECStartupSettings:ObservableObject
    {
        /// <summary>
        /// 启动设置
        /// </summary>
        public ECStartupSettings()
        {
            IsStatupOnline = false;
            WorksStartupInfo = new BindingList<ECWorkStartupInfo>();
            ImageRetainedDaysForOK = 30;
            ImageRetainedDaysForNG = 30;
            DataRetainedDays = 30;
            GetLocalIPs();
            GetDiskInfo();
            GetLanguagesInfo();
        }
        #region 方法
        /// <summary>
        /// 获取本机可用的IP
        /// </summary>
        private void GetLocalIPs()
        {
            List<string> ips = ECGeneric.GetLocalIPs();
            LocalIPs?.Clear();
            LocalIPs = new BindingList<string>(ips.ToArray());
        }

        /// <summary>
        /// 获取磁盘信息
        /// </summary>
        private void GetDiskInfo()
        {
            DriveInfo[] disks = ECFileConstantsManager.Disks;
            BindingList<string> tmpList = new BindingList<string>();
            
            foreach(DriveInfo disk in disks)
            {
               tmpList.Add(disk.Name);
            }
            DiskList?.Clear();

            DiskList = new BindingList<string>(tmpList.ToArray());
        }

        /// <summary>
        /// 获取语言信息
        /// </summary>
        private void GetLanguagesInfo()
        {
            BindingList<string> tmpList = new BindingList<string>();
            string[] lanFiles = Directory.GetFiles(ECFileConstantsManager.LanguagesFolder);
            foreach(string lanFile in lanFiles)
            {
                tmpList.Add(Path.GetFileNameWithoutExtension(lanFile));
            }
            LanguageList?.Clear();
            LanguageList = new BindingList<string>(tmpList.ToArray());
        }
        #endregion

        #region 属性
        /// <summary>
        /// 启动自动联机
        /// </summary>
        private bool _isStartupOnline;

        public bool IsStatupOnline
        {
            get { return _isStartupOnline; }
            set { _isStartupOnline = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 管理员账户密码
        /// </summary>
        private string _adminPassword;

        public string AdminPassword
        {
            get { return _adminPassword; }
            set
            {
                _adminPassword = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 工作项目启动配置列表
        /// </summary>
        private BindingList<ECWorkStartupInfo> _worksStartupInfo;

        public BindingList<ECWorkStartupInfo> WorksStartupInfo
        {
            get { return _worksStartupInfo; }
            set {
                _worksStartupInfo = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 本机IP
        /// </summary>
        private BindingList<string> _localIPs;

        public BindingList<string> LocalIPs
        {
            get { return _localIPs; }
            set { _localIPs = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 系统TCP服务器IP
        /// </summary>
        private string _systemTCPServerIP;

        public string SystemTCPServerIP
        {
            get { return _systemTCPServerIP; }
            set { _systemTCPServerIP = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 系统TCP服务器端口号
        /// </summary>
        private int _systemTCPServerPort;

        public int SystemTCPServerPort
        {
            get { return _systemTCPServerPort; }
            set {
                _systemTCPServerPort = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 工厂协议类型
        /// </summary>
        private string _FfpType;

        public string FfpType
        {
            get { return _FfpType; }
            set { _FfpType = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 框架程序模式
        /// </summary>
        private int _frameworkMode;

        public int FrameworkMode
        {
            get { return _frameworkMode; }
            set { _frameworkMode = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 磁盘列表
        /// </summary>
        private BindingList<string> _diskList;

        public BindingList<string> DiskList
        {
            get { return _diskList; }
            set { _diskList = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 选择的磁盘
        /// </summary>
        private string _selectedProjectDiskName;

        public string SelectedProjectDiskName
        {
            get { return _selectedProjectDiskName; }
            set {
                _selectedProjectDiskName = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 选择的磁盘
        /// </summary>
        private string _selectedImageDiskName;

        public string SelectedImageDiskName
        {
            get { return _selectedImageDiskName; }
            set
            {
                _selectedImageDiskName = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// OK图片保留天数
        /// </summary>
        private int _imageRetainedDaysForOK;

        public int ImageRetainedDaysForOK
        {
            get { return _imageRetainedDaysForOK; }
            set
            {
                _imageRetainedDaysForOK = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// NG图片保留天数
        /// </summary>
        private int _imageRetainedDaysForNG;

        public int ImageRetainedDaysForNG
        {
            get { return _imageRetainedDaysForNG; }
            set
            {
                _imageRetainedDaysForNG = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 数据保留天数
        /// </summary>
        private int _dataRetainedDays;

        public int DataRetainedDays
        {
            get { return _dataRetainedDays; }
            set { _dataRetainedDays = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 语言列表
        /// </summary>
        private BindingList<string> _languageList;

        public BindingList<string> LanguageList
        {
            get { return _languageList; }
            set { _languageList = value;
                RaisePropertyChanged();
            }
        }

        private string _selectedLanguage;

        public string SelectedLanguage
        {
            get { return _selectedLanguage; }
            set { _selectedLanguage = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 启用第三方板卡
        /// </summary>
        private bool _enableThirdCard;

        public bool EnableThirdCard
        {
            get { return _enableThirdCard; }
            set { _enableThirdCard = value;
                RaisePropertyChanged();
            }
        }


        /// <summary>
        /// 默认登录管理员权限
        /// </summary>
        private bool _isDefaultLoginAdmin;

        public bool IsDefaultLoginAdmin
        {
            get { return _isDefaultLoginAdmin; }
            set {
                _isDefaultLoginAdmin = value;
                RaisePropertyChanged();
            }
        }

        #endregion
    }
}
