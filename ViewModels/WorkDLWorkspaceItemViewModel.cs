using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViDi2;

namespace VPDLFramework.ViewModels
{
    public class WorkDLWorkspaceItemViewModel:ViewModelBase
    {
        public WorkDLWorkspaceItemViewModel(IWorkspace workspace)
        {
            WorkspaceName = workspace.UniqueName.Replace(".vrws", "");
            Streams = new BindingList<string>();
            foreach (IStream stream in workspace.Streams)
            {
                Streams.Add(stream.Name);
            }
        }

        #region 属性
        /// <summary>
        /// 工作区名称
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
        /// 工作区包含的streams
        /// </summary>

        private BindingList<string> _streams;

        public BindingList<string> Streams
        {
            get { return _streams; }
            set
            {
                _streams = value;
                RaisePropertyChanged();
            }
        }

        #endregion
    }
}
