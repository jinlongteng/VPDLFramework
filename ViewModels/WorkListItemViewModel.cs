using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViDi2;
using VPDLFramework.Models;
using VPDLFramework.Views;

namespace VPDLFramework.ViewModels
{
    public class WorkListItemViewModel:ViewModelBase
    {
        public WorkListItemViewModel()
        {
            CmdSaveWork = new RelayCommand<object>(SaveWork);
            CmdCloseWork = new RelayCommand<object>(CloseWork);
        }

        #region 命令
        /// <summary>
        /// 命令：保存工作
        /// </summary>
        public RelayCommand<object> CmdSaveWork { get; set; }

        /// <summary>
        /// 命令：关闭工作
        /// </summary>
        public RelayCommand<object> CmdCloseWork { get; set; }

        #endregion

        /// <summary>
        /// 保存工作
        /// </summary>
        /// <param name="work">要删除的工作</param>
        private void SaveWork(object work)
        {
            try
            {
                WorkListItemViewModel userWorkViewModel = work as WorkListItemViewModel;
                string workName = userWorkViewModel.WorkInfo.WorkName;
                userWorkViewModel.WorkInfo.ModifiedTime = DateTime.Now;
                ECSerializer.SaveObjectToJson(ECFileConstantsManager.RootFolder + $"\\{workName}\\{ECFileConstantsManager.WorkInfoFileName}", userWorkViewModel.WorkInfo);
                Messenger.Default.Send<string>("", ECMessengerManager.MainViewModelMessengerKeys.SaveWork);
            }
            catch (System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, NLog.LogLevel.Error);
            }

        }

        /// <summary>
        /// 关闭工作
        /// </summary>
        /// <param name="work">要删除的工作</param>
        private void CloseWork(object work)
        {
            if (!ECDialogManager.Verify(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.CloseEdit))) return;
            WorkListItemViewModel userWorkViewModel = work as WorkListItemViewModel;
            string workName = userWorkViewModel.WorkInfo.WorkName;
            userWorkViewModel.IsEdit = false;
            Messenger.Default.Send<string>("", ECMessengerManager.MainViewModelMessengerKeys.CloseWork);
        }

        /// <summary>
        /// 工作信息
        /// </summary>
        private ECWorkInfo _workInfo;

        public ECWorkInfo WorkInfo
        {
            get { return _workInfo; }
            set { _workInfo = value;
                RaisePropertyChanged();
            }
        }


        /// <summary>
        /// 是否在编辑
        /// </summary>
        private bool _isEdit;

        public bool IsEdit
        {
            get { return _isEdit; }
            set
            {
                _isEdit = value;
                RaisePropertyChanged();
            }
        }
    }
}
