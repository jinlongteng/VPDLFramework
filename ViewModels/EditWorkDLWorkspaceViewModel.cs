using Cognex.VisionPro;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using ViDi2;
using VPDLFramework.Models;

namespace VPDLFramework.ViewModels
{
    public class EditWorkDLWorkspaceViewModel:ViewModelBase
    {
        public EditWorkDLWorkspaceViewModel(string workName)
        {
            _workName = workName;
            _control = ECDLEnvironment.Control;
           
            CmdAddWorkspace = new RelayCommand(AddWorkspace);
            CmdRemoveWorkspace = new RelayCommand<object>(RemoveWorkspace);

            // 获取已有的配置信息
            GetExistConfig();
        }

        #region 命令
        /// <summary>
        /// 添加Workspace
        /// </summary>
        public RelayCommand CmdAddWorkspace { get; set; }

        /// <summary>
        /// 删除Workspace
        /// </summary>
        public RelayCommand<object> CmdRemoveWorkspace { get; set; }

        #endregion

        #region 方法

        /// <summary>
        /// 添加工作区
        /// </summary>
        private void AddWorkspace()
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".vrws",
                Filter = "Cognex Deep Learning Studio Runtime Workspaces (*.vrws)|*.vrws",
                ValidateNames = false
            };
            try
            {
                if ((bool)dialog.ShowDialog() == true)
                {
                    using (var fs = new System.IO.FileStream(dialog.FileName, System.IO.FileMode.Open, FileAccess.Read))
                    {
                        if (dialog.FileName.EndsWith(".vrws"))
                        {
                            // 检查是否存在重复名称
                            if(!CheckNameUniqueness(Path.GetFileName(Path.GetFileNameWithoutExtension(dialog.FileName)))) 
                            {
                                ECDialogManager.ShowMsg(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.DuplicateName));
                                return;
                            }
                            ECDialogManager.LoadWithAnimation(new Action(() =>
                            {
                                IWorkspace workspace = _control.Workspaces.Add(Path.GetFileNameWithoutExtension(dialog.FileName), dialog.FileName);
                                DispatcherHelper.CheckBeginInvokeOnUI(() => { DLWorkSpaceViewModelList.Add(new WorkDLWorkspaceItemViewModel(workspace));});
                            }), ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.Loading));
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message,LogLevel.Error);
                ECDialogManager.ShowMsg(ex.StackTrace + ex.Message);
            }
        }

        /// <summary>
        /// 删除工作区
        /// </summary>
        private void RemoveWorkspace(object workspaceViewModel)
        {
            if (workspaceViewModel == null) return;
            string workspaceName = (workspaceViewModel as WorkDLWorkspaceItemViewModel).WorkspaceName;
            if (ECDialogManager.Verify($"{ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.ConfirmRemove)} \"{workspaceName}\""))
            {
                // 删除vidi control运行时工作区
                if (_control.Workspaces.Names.Contains(workspaceName))
                    _control.Workspaces.Remove(workspaceName);

                // 删除显示的工作区信息
                DLWorkSpaceViewModelList.Remove(workspaceViewModel as WorkDLWorkspaceItemViewModel);

                // 删除文件
                string directory = $"{ECFileConstantsManager.RootFolder}\\{_workName}\\{ECFileConstantsManager.WorkspaceFolderName}";
                string filePath = directory + @"\" + workspaceName;
                if (File.Exists(filePath))
                    File.Delete(filePath);
            }
        }

        /// <summary>
        /// 获取VPDL工作区视图模型
        /// </summary>
        /// <param name="workName"></param>
        /// <returns></returns>
        private void GetExistConfig()
        {
            try
            {
                if(ECDLEnvironment.IsEnable)
                    ECDLEnvironment.ClearWorkspaces();
                BindingList<WorkDLWorkspaceItemViewModel> tempList = new BindingList<WorkDLWorkspaceItemViewModel>();

                string folder = $"{ECFileConstantsManager.RootFolder}\\{_workName}\\{ECFileConstantsManager.WorkspaceFolderName}";
                if (Directory.Exists(folder))
                {
                    string[] files = Directory.GetFiles(folder);
                    foreach (string file in files)
                    {
                        FileInfo fileInfo = new FileInfo(file);
                        if ((fileInfo.Extension == ".vrws")&&!_control.Workspaces.Names.Contains(fileInfo.Name))
                        {
                            IWorkspace workspace = _control.Workspaces.Add(fileInfo.Name.Replace(".vrws",""), file);
                            tempList.Add(new WorkDLWorkspaceItemViewModel(workspace));
                        }
                    }
                }

                DLWorkSpaceViewModelList = tempList;
            }
            catch (System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message,LogLevel.Error);
            }
        }

        /// <summary>
        /// 检查是否重名
        /// </summary>
        private bool CheckNameUniqueness(string name)
        {
            foreach (var item in DLWorkSpaceViewModelList)
            {
                if (item.WorkspaceName == name)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 保存配置
        /// </summary>
        /// <returns></returns>
        public bool SaveConfig()
        {
            try
            {
                string directory = $"{ECFileConstantsManager.RootFolder}\\{_workName}\\{ECFileConstantsManager.WorkspaceFolderName}";
                if (!Directory.Exists(directory)) { Directory.CreateDirectory(directory); }           
                foreach(var item in ECDLEnvironment.Control.Workspaces)
                {
                    string wsPath = directory + @"\" + item.DisplayName+".vrws";
                    item.Save(wsPath);
                }
                return true;
            }
            catch (System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, LogLevel.Error);
                return false;
            }
        }
        #endregion

        #region 字段
        /// <summary>
        /// 工作名
        /// </summary>
        private string _workName;

        /// <summary>
        /// ViDi Contorl
        /// </summary>
        private ViDi2.Runtime.IControl _control;

        #endregion

        #region 属性
        private BindingList<WorkDLWorkspaceItemViewModel> _DLWorkSpaceViewModelList;

        public BindingList<WorkDLWorkspaceItemViewModel> DLWorkSpaceViewModelList
        {
            get { return _DLWorkSpaceViewModelList; }
            set
            {
                _DLWorkSpaceViewModelList = value;
                RaisePropertyChanged();
            }
        }

        #endregion
    }
}
