using GalaSoft.MvvmLight;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel.Channels;
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
        private ECStartupSettings()
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

        private static ECStartupSettings _instance;
        public static string jsonPath = ECFileConstantsManager.ProgramStartupConifgFolder + @"\" + ECFileConstantsManager.StartupConfigName;
        public static ECStartupSettings Instance()
        {
            if (_instance is null)
            {
                if (!File.Exists(jsonPath))
                {
                    return new ECStartupSettings();
                }
                _instance = JsonConvert.DeserializeObject<ECStartupSettings>(File.ReadAllText(jsonPath));
                return _instance;
            }
            return _instance;
        }

        public void Save()
        {
            if (_instance != null)
            {
                Directory.CreateDirectory(ECFileConstantsManager.ProgramStartupConifgFolder);
                var jsonText = JsonConvert.SerializeObject(_instance, Formatting.Indented);
                File.WriteAllText(jsonPath, jsonText);
            }
        }


        /// <summary>
        /// 获取本机可用的IP
        /// </summary>
        private void GetLocalIPs()
        {
            if (LocalIPs?.Count!=0)
            {
                return;
            }
            IPHostEntry hostEntry = Dns.GetHostEntry(Dns.GetHostName());
            
            hostEntry.AddressList.
                Where(p => p.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).
                Select(p => p.ToString()).ToList().
                ForEach(ipName=>LocalIPs.Add(ipName));
        }

        /// <summary>
        /// 获取磁盘信息
        /// </summary>
        private void GetDiskInfo()
        {
            if (DiskList?.Count!=0)
            {
                return;
            }
            ECFileConstantsManager.Disks.
                Select(disk=>disk.Name).ToList().
                ForEach(diskName=>DiskList.Add(diskName));
        }

        /// <summary>
        /// 获取语言信息
        /// </summary>
        private void GetLanguagesInfo()
        {
            if (LanguageList?.Count!=0)
            {
                return;
            }
            Directory.GetFiles(ECFileConstantsManager.LanguagesFolder).
                Select(file=>Path.GetFileNameWithoutExtension(file)).ToList().
                ForEach(languageName=>LanguageList.Add(languageName));

        }
        #endregion

        #region 属性
        /// <summary>
        /// 启动自动联机
        /// </summary>
        private bool _isStartupOnline=false;

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

        public string AdminPassword { get; set; }=string.Empty;

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
        [JsonIgnore]
        public ObservableCollection<string> LocalIPs { get; set; } = new ObservableCollection<string>();

        /// <summary>
        /// 系统TCP服务器IP
        /// </summary>

        public string SystemTCPServerIP { get; set; }

        /// <summary>
        /// 系统TCP服务器端口号
        /// </summary>

        public int SystemTCPServerPort { get; set; }

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
        [JsonIgnore]
        public ObservableCollection<string> DiskList { get; set; }=new ObservableCollection<string>();

        /// <summary>
        /// 选择的磁盘
        /// </summary>
        private string _selectedProjectDiskName="C:\\";

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
        private int _imageRetainedDaysForOK=30;

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
        private int _imageRetainedDaysForNG=30;

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
        private int _dataRetainedDays = 30;

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
        [JsonIgnore]
        public BindingList<string> LanguageList { get; set; } = new BindingList<string>();

        private string _selectedLanguage= "SimplifiedChinese";

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
