using Cognex.VisionPro.ToolBlock;
using Cognex.VisionPro;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.Direct3D9;

namespace VPDLFramework.Models
{
    public class ECWorkStreamOrGroupResult : ObservableObject,IDisposable
    {
        /// <summary>
        /// 工作流名称
        /// </summary>
        private string _streamOrGroupName;

        /// <summary>
        /// 工作流名称
        /// </summary>
        public string StreamOrGroupName
        {
            get { return _streamOrGroupName; }
            set
            {
                _streamOrGroupName = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 触发时间
        /// </summary>
        private DateTime _triggerTime;

        public DateTime TriggerTime
        {
            get { return _triggerTime; }
            set { _triggerTime = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 触发计数
        /// </summary>
        private int _triggerCount;

        public int TriggerCount
        {
            get { return _triggerCount; }
            set {
                _triggerCount = value;
                RaisePropertyChanged();
            }
        }


        /// <summary>
        /// 工作流结果状态,True或False
        /// </summary>
        private bool _resultStatus;

        /// <summary>
        /// 工作流结果状态,True或False
        /// </summary>
        public bool ResultStatus
        {
            get { return _resultStatus; }
            set
            {
                _resultStatus = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 工作流用于显示的结果字符串
        /// </summary>
        private string _resultForDisplay;

        /// <summary>
        /// 工作流用于显示的结果字符串
        /// </summary>
        public string ResultForDisplay
        {
            get { return _resultForDisplay; }
            set
            {
                _resultForDisplay = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 工作流用于发送的结果字符串
        /// </summary>
        private string resultForSend;

        /// <summary>
        /// 工作流用于发送的结果字符串
        /// </summary>
        public string ResultForSend
        {
            get { return resultForSend; }
            set
            {
                resultForSend = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 工作流结果图像
        /// </summary>
        private ICogRecord resultRecord;

        /// <summary>
        /// 工作流结果图像
        /// </summary>
        public ICogRecord ResultRecord
        {
            get { return resultRecord; }
            set
            {
                resultRecord = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 工作流耗时
        /// </summary>
        private double elapsedTime;

        /// <summary>
        /// 工作流耗时
        /// </summary>
        public double ElapsedTime
        {
            get { return elapsedTime; }
            set
            {
                elapsedTime = value;
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// 是否是工作流类型
        /// </summary>
        private bool _isWrokStreamType;

        public bool IsWrokStreamType
        {
            get { return _isWrokStreamType; }
            set { _isWrokStreamType = value; }
        }

        /// <summary>
        /// 生成结果异常
        /// </summary>
        private bool _isResultError;

        public bool IsResultError
        {
            get { return _isResultError; }
            set {
                _isResultError = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Custom输出
        /// </summary>
        private CogToolBlockTerminalCollection _customOutputs;

        public CogToolBlockTerminalCollection CustomOutputs
        {
            get { return _customOutputs; }
            set { _customOutputs = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 缓存的图片数量
        /// </summary>
        private int _bufferedImagesCount;

        public int BufferedImagesCount
        {
            get { return _bufferedImagesCount; }
            set {
                _bufferedImagesCount = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 是否是实时显示
        /// </summary>
        private bool _isLiveMode;

        public bool IsLiveMode
        {
            get { return _isLiveMode; }
            set { _isLiveMode = value;
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
        /// 显示的3D图像
        /// </summary>
        private CogImage16Range _rangeImage;

        public CogImage16Range RangeImage
        {
            get { return _rangeImage; }
            set { _rangeImage = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            ResultRecord?.SubRecords.Clear();
            ResultRecord = null;
            CustomOutputs?.Dispose();
            CustomOutputs = null;
            RangeImage?.Dispose();
            RangeImage = null;
            
        }
    }

}
