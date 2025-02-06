using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace VPDLFramework.Models
{
    public class ECWorkStreamsGroupInfo:ObservableObject
    {
        /// <summary>
        /// 工作流组配置信息
        /// </summary>
        /// <param name="groupName">组名称</param>
        public ECWorkStreamsGroupInfo()
        {
            TimeoutDuration = 1000;
            StreamsList = new BindingList<string>();
            IsValid = false;
        }

        /// <summary>
        /// 选择的工作流组名称
        /// </summary>
        private string groupName;

        /// <summary>
        /// 选择的工作流组名称
        /// </summary>
        public string GroupName
        {
            get { return groupName; }
            set
            {
                groupName = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 超时时间
        /// </summary>
        private int timeoutDuration;

        /// <summary>
        /// 超时时间
        /// </summary>
        public int TimeoutDuration
        {
            get { return timeoutDuration; }
            set
            {
                timeoutDuration = value;
                RaisePropertyChanged();
            }
        }

        private BindingList<string> _streamsList;

        public BindingList<string> StreamsList
        {
            get { return _streamsList; }
            set {
                _streamsList = value;
                
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 组有效性,如果包含的工作流数量小于2则无效
        /// </summary>
        private bool _isValid;

        public bool IsValid
        {
            get { return _isValid; }
            set { _isValid = value;
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
        /// 结果图表系列数量
        /// </summary>
        private int _resultChartSeriesCount;

        public int ResultChartSeriesCount
        {
            get { return _resultChartSeriesCount; }
            set
            {
                _resultChartSeriesCount = value;
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
            set
            {
                _ToolBlockRecordKey = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 是否在运行界面显示图像框
        /// </summary>
        private bool _isVisibleInRuntime;

        public bool IsVisibleInRuntime
        {
            get { return _isVisibleInRuntime; }
            set { _isVisibleInRuntime = value;
                RaisePropertyChanged();
            }
        }

    }
}
