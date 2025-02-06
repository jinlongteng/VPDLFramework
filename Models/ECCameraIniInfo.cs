using GalaSoft.MvvmLight;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace VPDLFramework.Models
{
    public class ECCameraIniInfo:ObservableObject,IComparable<ECCameraIniInfo>
    {
        public ECCameraIniInfo() 
		{
        }

		/// <summary>
		/// 索引
		/// </summary>
        private int _index;

		public int Index
        {
			get { return _index; }
			set { _index = value;
                RaisePropertyChanged();
            }
		}

		/// <summary>
		/// 名称
		/// </summary>
		private string _name;

		public string name
		{
			get { return _name; }
			set { _name = value;
				RaisePropertyChanged();
			}
		}

		/// <summary>
		/// 序列号
		/// </summary>
		private string _serialNo;

		public string serialNo
        {
			get { return _serialNo; }
			set {
                _serialNo = value;
				RaisePropertyChanged();
			}
		}

		/// <summary>
		/// IP地址
		/// </summary>
		private string _IP_Addr;

		public string IP_Addr
		{
			get { return _IP_Addr; }
			set { _IP_Addr = value;
				RaisePropertyChanged();
			}
		}

		private string _subnet_mask;

		public string subnet_mask
        {
			get { return _subnet_mask; }
			set { _subnet_mask = value;
                RaisePropertyChanged();
            }
		}

		private string _IPCurrentConfig;

		public string IPCurrentConfig
        {
			get { return _IPCurrentConfig; }
			set { _IPCurrentConfig = value;
                RaisePropertyChanged();
            }
		}

		private string _MacAddr;

		public string MacAddr
        {
			get { return _MacAddr; }
			set { _MacAddr = value;
                RaisePropertyChanged();
            }
		}

		private string _Host_IPAddr;

		public string Host_IPAddr
        {
			get { return _Host_IPAddr; }
			set { _Host_IPAddr = value;
                RaisePropertyChanged();
            }
		}

		private string _Host_subnet_mask;

		public string Host_subnet_mask
        {
			get { return _Host_subnet_mask; }
			set { _Host_subnet_mask = value;
                RaisePropertyChanged();
            }
		}

		private string _Host_macAddr;

		public string Host_macAddr
        {
			get { return _Host_macAddr; }
			set { _Host_macAddr = value;
                RaisePropertyChanged();
            }
		}

		private string _Host_mtu;

		public string Host_mtu
        {
			get { return _Host_mtu; }
			set { _Host_mtu = value;
                RaisePropertyChanged();
            }
		}

		private string _bigEndian;

		public string bigEndian
        {
			get { return _bigEndian; }
			set { _bigEndian = value;
                RaisePropertyChanged();
            }
		}

        public int CompareTo(ECCameraIniInfo other)
        {
            if (this.serialNo == other.serialNo)
            {
                return 1;
            }
            return -1;
        }
    }
}
