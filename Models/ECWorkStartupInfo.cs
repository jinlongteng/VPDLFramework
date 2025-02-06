using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VPDLFramework.Models
{
    public class ECWorkStartupInfo : ViewModelBase
    {
        /// <summary>
        /// 工作项目启动信息
        /// </summary>
        /// <param name="inputWorkName"></param>
        /// <param name="inputTilte"></param>
        /// <param name="inputIsDefault"></param>
        public ECWorkStartupInfo(string inputWorkName, string inputTilte, bool inputIsDefault)
        {
            WorkName = inputWorkName;
            Title = inputTilte;
            IsDefaultWork = inputIsDefault;

        }

        /// <summary>
        /// 工作名称
        /// </summary>
        private string workName;

        /// <summary>
        /// 工作名称
        /// </summary>
        public string WorkName
        {
            get { return workName; }
            set
            {
                workName = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 工作标题
        /// </summary>
        private string title;

        /// <summary>
        /// 工作标题
        /// </summary>
        public string Title
        {
            get { return title; }
            set
            {
                title = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 是否是Default加载的工作
        /// </summary>
        private bool isDefaultWork;

        public bool IsDefaultWork
        {
            get { return isDefaultWork; }
            set
            {
                isDefaultWork = value;
                RaisePropertyChanged();
            }
        }
    }
}
