using GalaSoft.MvvmLight;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VPDLFramework.Models
{
    public class ECKeyValuePair:ObservableObject
    {
        public ECKeyValuePair(string key, object value,string valueType)
        {
            Key = key;
            Value = value;
            Type = valueType;
        }

        /// <summary>
        /// 键值
        /// </summary>
        private string _key;

        public string Key
        {
            get { return _key; }
            set { _key = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 值
        /// </summary>
        private object _value;

        public object Value
        {
            get { return _value; }
            set { _value = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 类型
        /// </summary>
        private string _type;

        public string Type
        {
            get { return _type; }
            set { _type = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 描述标签
        /// </summary>
        private string _label;

        public string Label
        {
            get { return _label; }
            set { _label = value;
                RaisePropertyChanged();
            }
        }

    }
}
