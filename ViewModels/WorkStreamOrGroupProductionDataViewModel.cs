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
using System.Windows.Forms;
using System.Windows.Shapes;
using VPDLFramework.Models;
using Xceed.Wpf.Toolkit;

namespace VPDLFramework.ViewModels
{
    public class WorkStreamOrGroupProductionDataViewModel : ViewModelBase
    {
        /// <summary>
        /// 工作流数据界面视图模型
        /// </summary>
        /// <param name="workName">工作项目名称</param>
        /// <param name="streamName">工作流名称</param>
        public WorkStreamOrGroupProductionDataViewModel(string workName, string streamOrGroupName, bool isGroup)
        {
            CmdQueryAll = new RelayCommand(QueryAll);
            CmdQueryByDate = new RelayCommand(QueryByDate);
            CmdClearAll = new RelayCommand(ClearAll);
            CmdExportDatabaseToCSV = new RelayCommand(ExportDatabaseToCSV);

            WorkName = workName;
            StreamOrGroupName = streamOrGroupName;
            IsGroup = isGroup;
            OKRateStr = "0.00%";
            GoodNum = 0;
            TotalNum = 0;

            StartDate = DateTime.Now;
            EndDate = DateTime.Now;
            SelectedDate = DateTime.Now;
            CheckStartEndDate();
            QueryAll();
        }

        #region 字段

        /// <summary>
        /// 工作项目名称
        /// </summary>
        private string WorkName;

        #endregion 字段

        #region 属性

        /// <summary>
        /// 工作流名称
        /// </summary>
        private string _streamOrGroupName;

        /// <summary>
        /// 工作流名称
        /// </summary>
        public string StreamOrGroupName
        {
            get { return _streamOrGroupName; }
            set
            {
                _streamOrGroupName = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 工作流数据总条数
        /// </summary>
        private int _totalNum;

        /// <summary>
        /// 工作流数据总条数
        /// </summary>
        public int TotalNum
        {
            get { return _totalNum; }
            set
            {
                _totalNum = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 工作流数据总OK数
        /// </summary>
        private int _goodNum;

        /// <summary>
        /// 工作流数据总OK数
        /// </summary>
        public int GoodNum
        {
            get { return _goodNum; }
            set
            {
                _goodNum = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 工作流数据总NG数
        /// </summary>
        private int _badNum;

        /// <summary>
        /// 工作流数据总NG数
        /// </summary>
        public int BadNum
        {
            get { return _badNum; }
            set
            {
                _badNum = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 工作流数据查询起始时间
        /// </summary>
        private DateTime _startDate;

        /// <summary>
        /// 工作流数据查询起始时间
        /// </summary>
        public DateTime StartDate
        {
            get { return _startDate; }
            set
            {
                _startDate = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 工作流数据查询截止时间
        /// </summary>
        private DateTime _endDate;

        /// <summary>
        /// 工作流数据查询截止时间
        /// </summary>
        public DateTime EndDate
        {
            get { return _endDate; }
            set
            {
                _endDate = value;
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
            set
            {
                _selectedDate = value;
                RaisePropertyChanged();
            }
        }


        /// <summary>
        /// 工作流数据通过率
        /// </summary>
        private string _oKRateStr;

        /// <summary>
        /// 工作流数据通过率
        /// </summary>
        public string OKRateStr
        {
            get { return _oKRateStr; }
            set
            {
                _oKRateStr = value;
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


        /// <summary>
        /// 是组类型
        /// </summary>
        private bool _isGroup;

        public bool IsGroup
        {
            get { return _isGroup; }
            set
            {
                _isGroup = value;
                RaisePropertyChanged();
            }
        }


        #endregion 属性

        #region 命令

        /// <summary>
        /// 命令：查询所有数据
        /// </summary>
        public RelayCommand CmdQueryAll { get; set; }

        /// <summary>
        /// 命令：查询时间段内数据
        /// </summary>
        public RelayCommand CmdQueryByDate { get; set; }

        /// <summary>
        /// 命令：清空数据
        /// </summary>
        public RelayCommand CmdClearAll { get; set; }

        /// <summary>
        /// 命令：导出数据库到CSV
        /// </summary>
        public RelayCommand CmdExportDatabaseToCSV { get; set; }

        #endregion 命令

        #region 方法
        /// <summary>
        /// 检查数据库文件起止日期
        /// </summary>
        private void CheckStartEndDate()
        {
            string path = ECFileConstantsManager.RootFolder + @"\" + WorkName + @"\" + ECFileConstantsManager.DatabaseFolderName + @"\" + ECFileConstantsManager.WorkStreamsDataFolderName
            + @"\" + StreamOrGroupName;
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
        private void QueryAll()
        {
            ECDialogManager.LoadWithAnimation(() =>
            {
                string path = ECFileConstantsManager.RootFolder + @"\" + WorkName + @"\" + ECFileConstantsManager.DatabaseFolderName + @"\" + ECFileConstantsManager.WorkStreamsDataFolderName
                + @"\" + StreamOrGroupName + @"\" + DateTime.Now.ToString("yyyy_MM_dd") + @"\" + ECFileConstantsManager.StreamDatabaseFileName;
                if (File.Exists(path))
                {
                    DataView  tmpData = ECSQLiteDataManager.QueryAll(path);
                    DataView okData = ECSQLiteDataManager.QueryByResultStatus(path, ECSQLiteDataManager.DataType.ResultData, 1);

                    DispatcherHelper.UIDispatcher.Invoke(() =>
                    {
                        Data=tmpData;
                        if (okData != null)
                        {
                            GoodNum = okData.Table.Rows.Count;
                            TotalNum = Data.Table.Rows.Count;
                            if (TotalNum == 0)
                                OKRateStr = "0.00%";
                            else
                                OKRateStr = Math.Round(((double)GoodNum / TotalNum * 100), 2).ToString() + "%";
                        }
                    });
                }
                
                SelectedDate = DateTime.Now;
            },ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.Querying));
        }

        /// <summary>
        /// 通过时间查询数据
        /// </summary>
        private void QueryByDate()
        {
            ECDialogManager.LoadWithAnimation(() =>
            {
                string folderName = SelectedDate.ToString("yyyy_MM_dd");

                string path = ECFileConstantsManager.RootFolder + @"\" + WorkName + @"\" + ECFileConstantsManager.DatabaseFolderName + @"\" + ECFileConstantsManager.WorkStreamsDataFolderName
                + @"\" + StreamOrGroupName + @"\" + folderName + @"\" + ECFileConstantsManager.StreamDatabaseFileName;
                if (File.Exists(path))
                {
                    DataView tmpData = ECSQLiteDataManager.QueryAll(path);
                    DataView okData = ECSQLiteDataManager.QueryByResultStatus(path, ECSQLiteDataManager.DataType.ResultData, 1);

                    DispatcherHelper.UIDispatcher.Invoke(() =>
                    {
                        Data = tmpData;
                        if (okData != null)
                        {
                            GoodNum = okData.Table.Rows.Count;
                            TotalNum = Data.Table.Rows.Count;
                            if (TotalNum == 0)
                                OKRateStr = "0.00%";
                            else
                                OKRateStr = Math.Round(((double)GoodNum / TotalNum * 100), 2).ToString() + "%";
                        }
                    });
                }
            }, ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.Querying));
        }

        /// <summary>
        /// 清空数据库数据
        /// </summary>
        private void ClearAll()
        {
            string path = ECFileConstantsManager.RootFolder + @"\" + WorkName + @"\" + ECFileConstantsManager.DatabaseFolderName + @"\" +
            ECFileConstantsManager.WorkStreamsDataFolderName + @"\" + StreamOrGroupName + @"\" + SelectedDate.ToString("yyyy_MM_dd") + @"\" + ECFileConstantsManager.StreamDatabaseFileName;
            if (File.Exists(path))
                ECSQLiteDataManager.ClearAll(path);
            QueryByDate();
        }

        /// <summary>
        /// 导出工作流数据库
        /// </summary>
        /// <param name="workAndStreamIndex">工作项目名称和工作流名称</param>
        private void ExportDatabaseToCSV()
        {
            if (Data == null) return;
            try
            {
                string streamName = StreamOrGroupName;

                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    // 设置Default文件名及其后缀
                    saveFileDialog.FileName = StreamOrGroupName;
                    saveFileDialog.DefaultExt = ".csv";

                    // 显示保存文件对话框并获取结果
                    DialogResult result = saveFileDialog.ShowDialog();

                    if (result == DialogResult.OK)
                    {
                        string filePath = saveFileDialog.FileName;
                        if (filePath != null)
                            ECDialogManager.LoadWithAnimation(new Action(() =>
                            {
                                ECDatabaseHelper.WriteCSV(filePath, Data.Table);
                                ECDialogManager.ShowMsg($"{ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.ExportFinished)}: " + filePath);
                            }), ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.Exporting));
                    }
                }
            }
            catch (Exception ex)
            {
                ECDialogManager.ShowMsg(ex.Message + ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.ExportFailed));
            }
        }

        #endregion 方法
    }
}
