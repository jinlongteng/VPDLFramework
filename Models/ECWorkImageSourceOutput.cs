using Cognex.VisionPro.ToolBlock;
using Cognex.VisionPro;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VPDLFramework.Models
{
    public class ECWorkImageSourceOutput : ObservableObject
    {
        /// <summary>
        /// 采集是否成功
        /// </summary>
        private bool _isAcqSucceed;

        public bool IsAcqSucceed
        {
            get { return _isAcqSucceed; }
            set { _isAcqSucceed = value; }
        }

        /// <summary>
        /// 默认输出的图像
        /// </summary>
        private ICogImage _image;

        public ICogImage Image
        {
            get { return _image; }
            set { _image = value; }
        }

        /// <summary>
        /// 其他输出
        /// </summary>
        private CogToolBlockTerminalCollection _otherOutputs;

        public CogToolBlockTerminalCollection OtherOutputs
        {
            get { return _otherOutputs; }
            set { _otherOutputs = value; }
        }

    }
}
