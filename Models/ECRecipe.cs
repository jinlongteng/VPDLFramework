using GalaSoft.MvvmLight;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VPDLFramework.Models
{
    public class ECRecipe:ObservableObject
    {
        public ECRecipe(string name) {

			RecipeName = name;
			Values = new BindingList<ECKeyValuePair>();
		}

		/// <summary>
		/// 配方名称
		/// </summary>
		private string _recipeName;

		public string RecipeName
        {
			get { return _recipeName; }
			set { _recipeName = value;
				RaisePropertyChanged();
			}
		}

		/// <summary>
		/// 值列表
		/// </summary>
		private BindingList<ECKeyValuePair> _values;

		public BindingList<ECKeyValuePair> Values
		{
			get { return _values; }
			set { _values = value;
				RaisePropertyChanged();
			}
		}

		/// <summary>
		/// 禁用描述标签修改
		/// </summary>
		private bool _disableLabelModify;

		public bool DisableLabelModify
        {
			get { return _disableLabelModify; }
			set { _disableLabelModify = value;
				RaisePropertyChanged();
			}
		}

	}
}
