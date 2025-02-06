using GalaSoft.MvvmLight;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VPDLFramework.Models
{
    public class ECCommCardInputLineInfo : ObservableObject
    {
        /// <summary>
        /// IO输入信息类
        /// </summary>
        public ECCommCardInputLineInfo()
        {

        }

        /// <summary>
        /// 线号
        /// </summary>
        private int _lineNumber;

        public int LineNumber
        {
            get { return _lineNumber; }
            set { _lineNumber = value;
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

    }
}
