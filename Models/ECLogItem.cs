using GalaSoft.MvvmLight;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VPDLFramework.Models
{
    public class ECLogItem:ObservableObject
    {
        public ECLogItem() { }

		/// <summary>
		/// 时间
		/// </summary>
		private DateTime _time;

		public DateTime Time
        {
			get { return _time; }
			set { _time = value;
				RaisePropertyChanged();
			}
		}

		/// <summary>
		/// 等级
		/// </summary>
		private NLog.LogLevel _level;

		public NLog.LogLevel Level
        {
			get { return _level; }
			set { _level = value;
				RaisePropertyChanged();
			}
		}

		/// <summary>
		/// 消息
		/// </summary>
		private string _message;

		public string Message
        {
			get { return _message; }
			set { _message = value;
				RaisePropertyChanged();
			}
		}


	}
}
