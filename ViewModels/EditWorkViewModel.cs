using Cognex.VisionPro;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViDi2.UI;
using VPDLFramework.Models;

namespace VPDLFramework.ViewModels
{
    public class EditWorkViewModel : ViewModelBase
    {

        /// <summary>
        /// 工作类
        /// </summary>
        /// <param name="name">工作名称</param>
        public EditWorkViewModel()
        {
            RegisterMessenger();
            Messenger.Default.Send<string>("",ECMessengerManager.MainViewModelMessengerKeys.AskWorkName);
            IsDLEnable = ECDLEnvironment.IsEnable;
        }

        #region 方法
        /// <summary>
        /// 注册订阅的消息
        /// </summary>
        private void RegisterMessenger()
        {
            Messenger.Default.Register<string>(this, ECMessengerManager.MainViewModelMessengerKeys.EditWorkChanged, OnEditWorkChanged);
            Messenger.Default.Register<string>(this, ECMessengerManager.MainViewModelMessengerKeys.ReplyWorkName, OnEditWorkChanged);
            Messenger.Default.Register<string>(this, ECMessengerManager.MainViewModelMessengerKeys.SaveWork, OnSaveWork);
            Messenger.Default.Register<string>(this, ECMessengerManager.MainViewModelMessengerKeys.CloseWork, OnCloseWork);
            Messenger.Default.Register<WorkStreamItemViewModel>(this, ECMessengerManager.EditWorkStreamViewModelMessengerKeys.RunWorkStreamItem, OnWorkStreamRun);
        }

        /// <summary>
        /// 关闭工作
        /// </summary>
        /// <param name="obj"></param>
        private void OnCloseWork(string obj)
        {
            Dispose();
        }

        /// <summary>
        /// 执行工作流消息
        /// </summary>
        /// <param name="obj"></param>
        private void OnWorkStreamRun(WorkStreamItemViewModel obj)
        {
            if (obj == null) return;
            try
            {
                string imageSourceName = obj.WorkStream.WorkStreamInfo.ImageSourceName;
                if (imageSourceName != null)
                {
                    foreach (var imageSourceViewModel in EditWorkImageSourceViewModel.ImageSourceViewModelList)
                    {
                        if (imageSourceViewModel.WorkImageSource.ImageSourceInfo.ImageSourceName == imageSourceName)
                        {
                            ECWorkImageSourceOutput imageSourceResult = imageSourceViewModel.WorkImageSource.GetImage();
                            if(imageSourceResult!=null)
                                obj.WorkStream.Run(imageSourceResult);
                        }
                    }
                }
                else
                    obj.WorkStream.Run();
            }
            catch (Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, LogLevel.Error);
            }

        }

        /// <summary>
        /// 收到保存工作的消息,保存正在编辑的工作设置
        /// </summary>
        /// <param name="obj"></param>
        private void OnSaveWork(string obj)
        {
            bool isSaveOK = true;
            ECDialogManager.LoadWithAnimation(new Action(() =>
            {
                isSaveOK= SaveWork();
            }), ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.Saving));

            // 记录是否保存成功
            if (isSaveOK)
            {
                ECLog.WriteToLog(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.SaveFinished), LogLevel.Trace);
                ECDialogManager.ShowMsg(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.SaveFinished));
            }
            else
            {
                ECLog.WriteToLog(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.SaveFailed), LogLevel.Error);
                ECDialogManager.ShowMsg(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.SaveFailed));
            }
        }

        /// <summary>
        /// 收到回复的工作名称,加载工作设置
        /// </summary>
        /// <param name="workName"></param>
        private void OnEditWorkChanged(string workName)
        {
            WorkName = workName;
            LoadWorkSettings();
        }

        /// <summary>
        /// 加载工作配置
        /// </summary>
        private void LoadWorkSettings()
        {      
            ECDialogManager.LoadWithAnimation(new Action(() =>
            {
                LoadImageSourceSettings();

                if(ECDLEnvironment.IsEnable)
                    LoadDLWorkspaceSettings();

                LoadWorkTCPSettings(); 
                LoadWorkStreamSettings();
                LoadWorkGroupSettings();
                LoadWorkCommCardSettings();

            }),ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.Loading));
            
        }

        /// <summary>
        /// 加载图像源配置
        /// </summary>
        private void LoadImageSourceSettings()
        {
            EditWorkImageSourceViewModel = new EditWorkImageSourceViewModel(WorkName);
        }

        /// <summary>
        /// 加载DL工作区配置
        /// </summary>
        private void LoadDLWorkspaceSettings()
        {
            EditDLWorkspaceViewModel = new EditWorkDLWorkspaceViewModel(WorkName);
        }

        /// <summary>
        /// 加载TCP配置
        /// </summary>
        private void LoadWorkTCPSettings()
        {
            EditTCPViewModel = new EditWorkTCPViewModel(WorkName);
        }

        /// <summary>
        /// 加载IO配置
        /// </summary>
        private void LoadWorkCommCardSettings()
        {
            EditCommCardViewModel = new EditWorkCommCardViewModel(WorkName);
            EditCommCardViewModel.GetExistConfig();
        }

        /// <summary>
        /// 加载TCP配置
        /// </summary>
        private void LoadWorkGroupSettings()
        {
            EditGroupViewModel = new EditWorkGroupViewModel(WorkName);
        }

        /// <summary>
        /// 加载工作流配置
        /// </summary>
        private void LoadWorkStreamSettings()
        {
            EditStreamsViewModel= new EditWorkStreamViewModel(WorkName);
        }

        /// <summary>
        /// 保存工作
        /// </summary>
        public bool SaveWork()
        {
            bool isSaveWithNoError = true;
            try
            {
                if (!EditWorkImageSourceViewModel.SaveConfig())
                {
                    isSaveWithNoError = false;
                    ECLog.WriteToLog(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.SavingImageSourceWithError), LogLevel.Error);
                }
                if (ECDLEnvironment.IsEnable)
                    if (!EditDLWorkspaceViewModel.SaveConfig())
                    {
                        isSaveWithNoError = false;
                        ECLog.WriteToLog(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.SavingDeepLearningWithError), LogLevel.Error);
                    }
                    
                if (!EditTCPViewModel.SaveConfig())
                {
                    isSaveWithNoError=false;
                    ECLog.WriteToLog(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.SavingTCPIPWithError), LogLevel.Error);
                }

                if (!EditCommCardViewModel.SaveConfig())
                {
                    isSaveWithNoError = false;
                    ECLog.WriteToLog(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.SavingCommCardWithError), LogLevel.Error);
                }

                if (!EditGroupViewModel.SaveConfig())
                {
                    isSaveWithNoError=false;
                    ECLog.WriteToLog(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.SavingWorkGroupWithError), LogLevel.Error);
                }
                    
                if (!EditStreamsViewModel.SaveConfig())
                {
                    isSaveWithNoError=false;
                    ECLog.WriteToLog(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.SavingWorkStreamWithError), LogLevel.Error);
                }   
            }
            catch(Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, LogLevel.Error);
            }

            return isSaveWithNoError;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            ECDLEnvironment.ClearWorkspaces();
            EditWorkImageSourceViewModel.Dispose();
            EditTCPViewModel.Dispose();
            EditGroupViewModel.Dispose();
            EditStreamsViewModel.Dispose();

            EditWorkImageSourceViewModel=null;
            EditTCPViewModel=null;
            EditGroupViewModel=null;
            EditStreamsViewModel= null;

            // 释放控件资源
            Messenger.Default.Send<string>("", ECMessengerManager.EditWorkStreamViewModelMessengerKeys.Dispose);

            GC.Collect();
        }

        #endregion

        #region 属性
        /// <summary>
        /// 工作名称
        /// </summary>
        private string _workName;

        public string WorkName
        {
            get { return _workName; }
            set
            {
                _workName = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 工作路径
        /// </summary>
        private string _filePath;

        public string FilePath
        {
            get { return _filePath; }
            set
            {
                _filePath = value;
                RaisePropertyChanged();
            }
        }

        

        private EditWorkImageSourceViewModel _editWorkImageSourceViewModel;

        public EditWorkImageSourceViewModel EditWorkImageSourceViewModel
        {
            get { return _editWorkImageSourceViewModel; }
            set { _editWorkImageSourceViewModel = value;
                RaisePropertyChanged();
            }
        }


        /// <summary>
        /// 编辑VPDL工作区集合视图模型
        /// </summary>
        private EditWorkDLWorkspaceViewModel _editDLWorkspaceViewModel;

        public EditWorkDLWorkspaceViewModel EditDLWorkspaceViewModel
        {
            get { return _editDLWorkspaceViewModel; }
            set
            {
                _editDLWorkspaceViewModel = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 编辑工作TCP集合视图模型
        /// </summary>
        private EditWorkTCPViewModel _editTCPViewModel;

        public EditWorkTCPViewModel EditTCPViewModel
        {
            get { return _editTCPViewModel; }
            set
            {
                _editTCPViewModel = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 编辑工作IO视图模型
        /// </summary>
        private EditWorkCommCardViewModel _editCommCardViewModel;

        public EditWorkCommCardViewModel EditCommCardViewModel
        {
            get { return _editCommCardViewModel; }
            set
            {
                _editCommCardViewModel = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 编辑工作流组集合视图模型
        /// </summary>
        private EditWorkGroupViewModel _editGroupViewModel;

        public EditWorkGroupViewModel EditGroupViewModel
        {
            get { return _editGroupViewModel; }
            set
            {
                _editGroupViewModel = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 编辑工作流集合视图模型
        /// </summary>
        private EditWorkStreamViewModel _editStreamsViewModel;

        public EditWorkStreamViewModel EditStreamsViewModel
        {
            get { return _editStreamsViewModel; }
            set { _editStreamsViewModel = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// DL启用
        /// </summary>
        private bool _isDLEnable;

        public bool IsDLEnable
        {
            get { return _isDLEnable; }
            set {
                _isDLEnable = value;
                RaisePropertyChanged();
            }
        }


        #endregion
    }
}
