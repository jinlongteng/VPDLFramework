using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VPDLFramework.Models
{
    public class ECWorkFileInfo:ObservableObject
    {
        public ECWorkFileInfo(string workName)
        {
            WorkName = workName;
            StreamsFileInfo = new BindingList<ECStreamFileInfo>();
        }

        /// <summary>
        /// 工作名
        /// </summary>
        private string _workName;

        public string WorkName
        {
            get { return _workName; }
            set { _workName = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 工作流文件信息列表
        /// </summary>
        private BindingList<ECStreamFileInfo> _streamsFileInfo;

        public BindingList<ECStreamFileInfo> StreamsFileInfo
        {
            get { return _streamsFileInfo; }
            set { _streamsFileInfo = value; }
        }
    }

}
