using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VPDLFramework.Models
{
    public class ECCommCardOutputLineInfo : ViewModelBase
    {
        /// <summary>
        /// IO输出信息类
        /// </summary>
        public ECCommCardOutputLineInfo()
        {
        }

        /// <summary>
        /// 线号
        /// </summary>
        private int _lineNumber;

        public int LineNumber
        {
            get { return _lineNumber; }
            set
            {
                _lineNumber = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 信号类型
        /// </summary>
        private string _signalType;

        public string SignalType
        {
            get { return _signalType; }
            set
            {
                _signalType = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 脉冲宽度
        /// </summary>
        private int _duration;

        public int Duration
        {
            get { return _duration; }
            set { _duration = value;
                RaisePropertyChanged();
            }
        }

    }
}
