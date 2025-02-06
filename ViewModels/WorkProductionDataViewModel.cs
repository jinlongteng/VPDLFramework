using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VPDLFramework.Models;

namespace VPDLFramework.ViewModels
{
    public class WorkProductionDataViewModel : ViewModelBase
    {
        public WorkProductionDataViewModel(string workName)
        {
            _workName = workName;
        }

        #region 方法

        /// <summary>
        /// 加载工作生产数据
        /// </summary>
        /// <param name="obj"></param>
        public void UpdateProductData()
        {
            try
            {
                string[] streamsFolder = Directory.GetDirectories($"{ECFileConstantsManager.RootFolder}\\{_workName}\\{ECFileConstantsManager.StreamsFolderName}");
                ProductionData = new BindingList<WorkStreamOrGroupProductionDataViewModel>();
                foreach (string streamFolder in streamsFolder)
                {
                    ProductionData.Add(new WorkStreamOrGroupProductionDataViewModel(_workName, Path.GetFileName(streamFolder),false));
                }

                string[] groupsFolder = Directory.GetDirectories($"{ECFileConstantsManager.RootFolder}\\{_workName}\\{ECFileConstantsManager.GroupsFolderName}");
                foreach (string groupFolder in groupsFolder)
                {
                    ProductionData.Add(new WorkStreamOrGroupProductionDataViewModel(_workName, Path.GetFileName(groupFolder), true));
                }
            }
            catch (Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, NLog.LogLevel.Error);
            }

        }
        #endregion

        #region 字段
        /// <summary>
        /// 工作名
        /// </summary>
        private string _workName;
        #endregion

        #region 属性

        /// <summary>
        /// 工作流生产数据视图模型集合
        /// </summary>

        private BindingList<WorkStreamOrGroupProductionDataViewModel> _productionData;

        public BindingList<WorkStreamOrGroupProductionDataViewModel> ProductionData
        {
            get { return _productionData; }
            set
            {
                _productionData = value;
                RaisePropertyChanged();
            }
        }

        #endregion
    }
}
