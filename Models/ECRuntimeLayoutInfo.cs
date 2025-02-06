using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VPDLFramework.Models
{
    public class ECRuntimeLayoutInfo:ObservableObject
    {
        public ECRuntimeLayoutInfo() 
        { 
        }

        // 是否显示图表
        private bool _isChartVisible;

        public bool IsChartVisible
        {
            get { return _isChartVisible; }
            set
            {
                _isChartVisible = value;
                RaisePropertyChanged();
            }
        }


        /// <summary>
        /// 显示的控件行数
        /// </summary>
        private int _displayRows;

        public int DisplayRows
        {
            get { return _displayRows; }
            set
            {
                _displayRows = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 显示控件列数
        /// </summary>
        private int _displayColumns;

        public int DisplayColumns
        {
            get { return _displayColumns; }
            set
            {
                _displayColumns = value;
                RaisePropertyChanged();
            }
        }

    }
}
