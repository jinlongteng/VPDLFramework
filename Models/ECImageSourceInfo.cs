using Cognex.VisionPro.ToolBlock;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VPDLFramework.Models
{
    public class ECImageSourceInfo:ObservableObject
    {
        /// <summary>
        /// 创建图像源信息
        /// </summary>
        /// <param name="name">图像源名称</param>
        public ECImageSourceInfo(string name)
        {
            ImageSourceName = name;
        }

        /// <summary>
        /// 图像源名称
        /// </summary>
        private string _imageSourceName;

        public string ImageSourceName
        {
            get { return _imageSourceName; }
            set
            {
                _imageSourceName = value;
                RaisePropertyChanged();
            }
        }


        /// <summary>
        /// 图像文件夹路径
        /// </summary>
        private string imageFilePath;

        public string ImageFilePath
        {
            get { return imageFilePath; }
            set
            {
                imageFilePath = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 是否使用相机作为图像源
        /// </summary>
        private bool isUseCam;

        public bool IsUseCam
        {
            get { return isUseCam; }
            set
            {
                isUseCam = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 相机在线,当使用物理相机时使用该参数
        /// </summary>
        private bool _isOnline;

        public bool IsOnline
        {
            get { return _isOnline; }
            set { _isOnline = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 启用光源控制
        /// </summary>
        private bool _isLightControlEnbale;

        public bool IsLightControlEnbale
        {
            get { return _isLightControlEnbale; }
            set { _isLightControlEnbale = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 控制器IP和端口号
        /// </summary>
        private string _controllerIPPort;

        public string ControllerIPPort
        {
            get { return _controllerIPPort; }
            set { _controllerIPPort = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 光源开启控制指令
        /// </summary>
        private string _lightOnCommand;

        public string LightOnCommand
        {
            get { return _lightOnCommand; }
            set {
                _lightOnCommand = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 光源关闭控制指令
        /// </summary>
        private string _lightOffCommand;

        public string LightOffCommand
        {
            get { return _lightOffCommand; }
            set
            {
                _lightOffCommand = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 以太网类型
        /// </summary>
        private int _lightEthernetType;

        public int LightEthernetType
        {
            get { return _lightEthernetType; }
            set {
                _lightEthernetType = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 是否是16进制
        /// </summary>
        private bool _isHex;

        public bool IsHex
        {
            get { return _isHex; }
            set { _isHex = value;
                RaisePropertyChanged();
            }
        }

    }
}
