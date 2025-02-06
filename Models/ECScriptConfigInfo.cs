using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VPDLFramework.Models
{
    public class ECScriptConfigInfo:ObservableObject
    {
        /// <summary>
        /// 输入脚本引用的dlls
        /// </summary>
        private BindingList<string> _scriptAssembly;

        public BindingList<string> ScriptAssembly
        {
            get { return _scriptAssembly; }
            set
            {
                _scriptAssembly = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 是否是Debug模式
        /// </summary>
        private bool _isDebugMode;

        public bool IsDebugMode
        {
            get { return _isDebugMode; }
            set { _isDebugMode = value;
                RaisePropertyChanged();
            }
        }

    }
}
