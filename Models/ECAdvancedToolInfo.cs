using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValueConverters;

namespace VPDLFramework.Models
{
    public class ECAdvancedToolInfo:ObservableObject
    {
        public ECAdvancedToolInfo(string toolName)
        { 
            ToolName = toolName;
            var rime = DateTime.Now;
        }

        /// <summary>
        /// 是否是DL类型
        /// </summary>
        private bool _isDLType;

        public bool IsDLType
        {
            get { return _isDLType; }
            set { _isDLType = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 工具名称
        /// </summary>
        private string _toolName;

        public string ToolName
        {
            get { return _toolName; }
            set { _toolName = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 深度学习工作区名称
        /// </summary>
        private string _DLWorkspaceName;

        public string DLWorkspaceName
        {
            get { return _DLWorkspaceName; }
            set
            {
                _DLWorkspaceName = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 深度学习流名称
        /// </summary>
        private string _DLStreamName;

        public string DLStreamName
        {
            get { return _DLStreamName; }
            set
            {
                _DLStreamName = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// GPU索引
        /// </summary>
        private int _GPUIndex;

        public int GPUIndex
        {
            get { return _GPUIndex; }
            set
            {
                _GPUIndex = value;
                RaisePropertyChanged();
            }
        }
    }
}
