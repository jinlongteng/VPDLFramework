using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VPDLFramework.Models;

namespace VPDLFramework.ViewModels
{
    public class WorkRuntimeLogViewModel:ObservableObject
    {
        /// <summary>
        /// 工作日志界面视图模型
        /// </summary>
        /// <param name="workName"></param>
        public WorkRuntimeLogViewModel(string workName)
        {
            CmdQueryAll = new RelayCommand(QueryAll);
            CmdQueryByDate = new RelayCommand(QueryByDate);
            CmdClearAll = new RelayCommand(ClearAll);
            _workName = workName;
            StartDate = DateTime.Now;
            EndDate = DateTime.Now;
            SelectedDate = DateTime.Now;
            CheckStartEndDate();
            QueryAll();
        }

        #region 字段

        /// <summary>
        /// 工作名称
        /// </summary>
        private string _workName;

        #endregion 字段

        #region 命令

        /// <summary>
        /// 查询所有数据
        /// </summary>
        public RelayCommand CmdQueryAll { get; set; }

        /// <summary>
        /// 按日期查询数据
        /// </summary>
        public RelayCommand CmdQueryByDate { get; set; }

        /// <summary>
        /// 清空数据
        /// </summary>
        public RelayCommand CmdClearAll { get; set; }
        #endregion 命令

        #region 方法
        /// <summary>
        /// 检查数据库文件起止日期
        /// </summary>
        private void CheckStartEndDate()
        {
            string path = ECFileConstantsManager.RootFolder + @"\" + _workName + @"\" + ECFileConstantsManager.DatabaseFolderName + @"\" + ECFileConstantsManager.WorkLogFolderName;
            if (Directory.Exists(path))
            {
                string[] folderNames = Directory.GetDirectories(path);
                DateTime folderTime;
                foreach (string name in folderNames)
                {
                    if (DateTime.TryParseExact(System.IO.Path.GetFileName(name),
                    "yyyy_MM_dd",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None,
                    out folderTime))
                    {
                        if (folderTime.CompareTo(StartDate) < 0)
                            StartDate = folderTime;
                    }
                }
            }
        }

        /// <summary>
        /// 查询所有数据
        /// </summary>
        public void QueryAll()
        {
            string path = ECFileConstantsManager.RootFolder + @"\" + _workName + @"\" + ECFileConstantsManager.DatabaseFolderName + @"\" +ECFileConstantsManager.WorkLogFolderName+@"\"+
                DateTime.Now.ToString("yyyy_MM_dd")+@"\"+ ECFileConstantsManager.LogDatabaseFileName;
            if (File.Exists(path))
            {
                ECDialogManager.LoadWithAnimation(() =>
                {
                    DataView tmpData = ECSQLiteDataManager.QueryAll(path);
                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        Data = tmpData;
                    });
                }, ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.Querying));
            }
            SelectedDate = DateTime.Now;
        }

        /// <summary>
        /// 按日期查询数据
        /// </summary>
        private void QueryByDate()
        {
            string path = ECFileConstantsManager.RootFolder + @"\" + _workName + @"\" + ECFileConstantsManager.DatabaseFolderName + @"\" + ECFileConstantsManager.WorkLogFolderName + @"\" +
                SelectedDate.ToString("yyyy_MM_dd") + @"\" + ECFileConstantsManager.LogDatabaseFileName;
            if (File.Exists(path))
            {
                ECDialogManager.LoadWithAnimation(() =>
                {
                    DataView tmpData = ECSQLiteDataManager.QueryAll(path);
                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        Data = tmpData;
                    });
                }, ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.Querying));

            }
        }

        /// <summary>
        /// 清空数据库
        /// </summary>
        private void ClearAll()
        {
            string path = ECFileConstantsManager.RootFolder + @"\" + _workName + @"\" + ECFileConstantsManager.DatabaseFolderName + @"\" + ECFileConstantsManager.WorkLogFolderName + @"\" +
                SelectedDate.ToString("yyyy_MM_dd") + @"\" + ECFileConstantsManager.LogDatabaseFileName;
            if (File.Exists(path))
            {
                ECSQLiteDataManager.ClearAll(path);
            }
        }

        #endregion 方法

        #region 属性

        /// <summary>
        /// 查询起始日期
        /// </summary>
        private DateTime startDate;

        /// <summary>
        /// 查询起始日期
        /// </summary>
        public DateTime StartDate
        {
            get { return startDate; }
            set
            {
                startDate = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 查询截止日期
        /// </summary>
        private DateTime endDate;

        /// <summary>
        /// 查询截止日期
        /// </summary>
        public DateTime EndDate
        {
            get { return endDate; }
            set
            {
                endDate = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 选择的日期
        /// </summary>
        private DateTime _selectedDate;

        public DateTime SelectedDate
        {
            get { return _selectedDate; }
            set { _selectedDate = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 数据表
        /// </summary>
        private DataView _data;

        public DataView Data
        {
            get { return _data; }
            set
            {
                _data = value;
                RaisePropertyChanged();
            }
        }
        #endregion 属性

    }
}
