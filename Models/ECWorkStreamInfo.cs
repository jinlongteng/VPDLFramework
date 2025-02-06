using Cognex.VisionPro;
using GalaSoft.MvvmLight;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VPDLFramework.Models
{
    [Serializable]
    public class ECWorkStreamInfo:ObservableObject
    {
        /// <summary>
        /// 工作流信息类
        /// </summary>
        /// <param name="streamName"></param>
        public ECWorkStreamInfo(string streamName)
        {
            StreamName = streamName;
        }

        /// <summary>
        /// 无参构造,用于深度复制
        /// </summary>
        public ECWorkStreamInfo()
        {

        }

        #region 属性

        /// <summary>
        /// 工作流ID
        /// </summary>
        private int _streamID;

        public int StreamID
        {
            get { return _streamID; }
            set { _streamID = value;
                RaisePropertyChanged();
            }
        }


        /// <summary>
        /// 工作流名称
        /// </summary>
        private string _streamName;

        /// <summary>
        /// 工作流名称
        /// </summary>
        public string StreamName
        {
            get { return _streamName; }
            set { _streamName = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 启用自触发
        /// </summary>
        private bool _isEnableInternalTrigger;

        public bool IsEnableInternalTrigger
        {
            get { return _isEnableInternalTrigger; }
            set {  
                _isEnableInternalTrigger = value;
                if (!_isEnableInternalTrigger)
                    IsInternalTriggerBegin = false;
                if (_isEnableInternalTrigger)
                    IsAsyncMode = false;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 内部自触发启动
        /// </summary>
        private bool _isInternalTriggerBegin;

        public bool IsInternalTriggerBegin
        {
            get { return _isInternalTriggerBegin; }
            set
            {
                _isInternalTriggerBegin = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 组名
        /// </summary>
        private string _groupName;

        public string GroupName
        {
            get { return _groupName; }
            set { _groupName = value;
                RaisePropertyChanged();
            }
        }


        /// <summary>
        /// 仅Vpro
        /// </summary>
        private bool _isOnlyVpro;

        public bool IsOnlyVpro
        {
            get { return _isOnlyVpro; }
            set
            {
                _isOnlyVpro = value;
                RaisePropertyChanged();
            }
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
        /// 是否使用高级深度学习模式
        /// </summary>
        private bool _isUseAdvancedDLModel;

        public bool IsUseAdvancedDLModel
        {
            get { return _isUseAdvancedDLModel; }
            set { _isUseAdvancedDLModel = value;
                RaisePropertyChanged();
            }
        }


        /// <summary>
        /// DL工作区名称
        /// </summary>
        private string _workspaceName;

        public string WorkspaceName
        {
            get { return _workspaceName; }
            set
            {
                _workspaceName = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// DL流名称
        /// </summary>
        private string _workspaceStreamName;

        public string WorkspaceStreamName
        {
            get { return _workspaceStreamName; }
            set
            {
                _workspaceStreamName = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// GPU索引
        /// </summary>
        private int _gpuIndex;

        public int GpuIndex
        {
            get { return _gpuIndex; }
            set
            {
                _gpuIndex = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 结果图形选项
        /// </summary>
        private string _resultGraphicOption;

        public string ResultGraphicOption
        {
            get { return _resultGraphicOption; }
            set
            {
                _resultGraphicOption = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 显示的ToolBlockRecord键值
        /// </summary>
        private string _ToolBlockRecordKey;

        public string ToolBlockRecordKey
        {
            get { return _ToolBlockRecordKey; }
            set { _ToolBlockRecordKey = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 存图记录选项
        /// </summary>
        private string _imageRecordOption;

        public string ImageRecordOption
        {
            get { return _imageRecordOption; }
            set
            {
                _imageRecordOption = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 存图记录条件
        /// </summary>
        private string _imageRecordConditionOption;

        public string ImageRecordConditionOption
        {
            get { return _imageRecordConditionOption; }
            set
            {
                _imageRecordConditionOption = value;
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// 触发类型
        /// </summary>
        private string _triggerType;

        public string TriggerType
        {
            get { return _triggerType; }
            set
            {
                _triggerType = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// TCP触发接收者名称
        /// </summary>
        private string _TCPReceiverName;

        public string TCPReceiverName
        {
            get { return _TCPReceiverName; }
            set
            {
                _TCPReceiverName = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// IO输入Line号
        /// </summary>
        private string _IOInputConstant;

        public string IOInputConstant
        {
            get { return _IOInputConstant; }
            set
            {
                _IOInputConstant = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// SoftEvent名称
        /// </summary>
        private string _softEventConstant;

        public string SoftEventConstant
        {
            get { return _softEventConstant; }
            set { _softEventConstant = value;
                RaisePropertyChanged();
            }
        }


        /// <summary>
        /// 结果发送者类型
        /// </summary>
        private string _resultSenderType;

        public string ResultSenderType
        {
            get { return _resultSenderType; }
            set
            {
                _resultSenderType = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// TCP发送者名称
        /// </summary>
        private string _TCPSenderName;

        public string TCPSenderName
        {
            get { return _TCPSenderName; }
            set
            {
                _TCPSenderName = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// IO输出Line号
        /// </summary>
        private string _IOOutputConstant;

        public string IOOutputConstant
        {
            get { return _IOOutputConstant; }
            set
            {
                _IOOutputConstant = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// IO输出信号类型
        /// </summary>
        private string _IOOutputSignalType;

        public string IOOutputSignalType
        {
            get { return _IOOutputSignalType; }
            set
            {
                _IOOutputSignalType = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 是否禁用工作流
        /// </summary>
        private bool _isStreamDisable;

        public bool IsStreamDisable
        {
            get { return _isStreamDisable; }
            set
            {
                _isStreamDisable = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 结果图表系列数量
        /// </summary>
        private int _resultChartSeriesCount;

        public int ResultChartSeriesCount
        {
            get { return _resultChartSeriesCount; }
            set { _resultChartSeriesCount = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 异步模式
        /// </summary>
        private bool _isAsyncMode;

        public bool IsAsyncMode
        {
            get { return _isAsyncMode; }
            set { _isAsyncMode = value;
                if (!_isAsyncMode)
                    IsEnableMultiThread = false;
                if (_isAsyncMode)
                    IsEnableInternalTrigger = false;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 线程数量
        /// </summary>
        private int _threadCount;

        public int ThreadCount
        {
            get { return _threadCount; }
            set { _threadCount = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 启用多线程
        /// </summary>
        private bool _isEnableMultiThread;

        public bool IsEnableMultiThread
        {
            get { return _isEnableMultiThread; }
            set { _isEnableMultiThread = value;
                if(_isEnableMultiThread) 
                    IsAsyncMode = true;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 是否显示3D
        /// </summary>
        private bool _isDisplay3D;

        public bool IsDisplay3D
        {
            get { return _isDisplay3D; }
            set
            {
                _isDisplay3D = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 图像源是否是多数据
        /// </summary>
        private bool _isImageSourceManyData;

        public bool IsImageSourceManyData
        {
            get { return _isImageSourceManyData; }
            set { _isImageSourceManyData = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 原图类型
        /// </summary>
        private string _originalImageTypeConstant;

        public string OriginalImageTypeConstant
        {
            get { return _originalImageTypeConstant; }
            set {
                _originalImageTypeConstant = value;
                RaisePropertyChanged();
            }
        }

        #endregion
    }
}
