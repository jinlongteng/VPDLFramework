using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace VPDLFramework.Models
{
    public class ECWorkInfo:ObservableObject
    {
		/// <summary>
		/// 工作名称
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
		/// 工作ID
		/// </summary>
		private int _workID;

		public int WorkID
		{
			get { return _workID; }
			set { _workID = value;
				RaisePropertyChanged();
			}
		}

		/// <summary>
		/// 是否启用DL
		/// </summary>
		private bool _isDLEnable;

		public bool IsDLEnable
		{
			get { return _isDLEnable; }
			set { _isDLEnable = value;
				RaisePropertyChanged();
			}
		}

        /// <summary>
        /// 更新的时间
        /// </summary>
        private DateTime _modifiedTime;

        public DateTime ModifiedTime
        {
            get { return _modifiedTime; }
            set
            {
                _modifiedTime = value;
                RaisePropertyChanged();
            }
        }
    }
}
