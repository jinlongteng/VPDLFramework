using Cognex.VisionPro.ToolBlock;
using Cognex.VisionPro;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using VPDLFramework.Models;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight;
using System.ComponentModel;
using System.Text.RegularExpressions;
using NLog;
using ViDi2;
using Amazon.S3.Model;
using GalaSoft.MvvmLight.Messaging;
using ViDi2.Training;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using GalaSoft.MvvmLight.Threading;

namespace VPDLFramework.ViewModels
{
    public class EditWorkGroupViewModel:ViewModelBase
    { /// <summary>
      /// 工作流组界面视图模型
      /// </summary>
      /// <param name="workName">工作项目名称</param>
        public EditWorkGroupViewModel(string workName)
        {
            _workName = workName;
            
            // 绑定命令
            BindCmd();

            // 注册消息
            RegisterMessenger();

            // 获取已有的配置
            GetExistConfig();
        }

        #region 方法
        /// <summary>
        /// 绑定指令
        /// </summary>
        private void BindCmd()
        {
            CmdAddGroup = new RelayCommand(AddGroup);
            CmdRemoveGroup = new RelayCommand<object>(RemoveGroup);
        }

        /// <summary>
        /// 注册订阅消息
        /// </summary>
        private void RegisterMessenger()
        {
            Messenger.Default.Register<EditWorkStreamViewModel>(this, ECMessengerManager.EditWorkStreamViewModelMessengerKeys.SelectedGroupChanged, OnSelectedGroupChanged);
            Messenger.Default.Register<WorkStreamItemViewModel>(this, ECMessengerManager.EditWorkStreamViewModelMessengerKeys.RemoveWorkStreamItem, OnRemoveWorkStreamItem);
        }

        /// <summary>
        /// 选择组改变
        /// </summary>
        /// <param name="obj"></param>
        private void OnSelectedGroupChanged(EditWorkStreamViewModel obj)
        {
            EditWorkStreamViewModel editWorkStreamViewModel = obj;
            foreach (var group in WorkGroupItemList)
            {
                if (group.WorkGroup.GroupInfo.StreamsList == null)
                    group.WorkGroup.GroupInfo.StreamsList = new BindingList<string>();
                foreach (var stream in editWorkStreamViewModel.WorkStreamItemViewModelList)
                {
                    if (stream.WorkStream.WorkStreamInfo.GroupName == group.WorkGroup.GroupInfo.GroupName && !group.WorkGroup.GroupInfo.StreamsList.Contains(stream.WorkStream.WorkStreamInfo.StreamName))
                        group.WorkGroup.GroupInfo.StreamsList.Add(stream.WorkStream.WorkStreamInfo.StreamName);
                    else if (stream.WorkStream.WorkStreamInfo.GroupName != group.WorkGroup.GroupInfo.GroupName && group.WorkGroup.GroupInfo.StreamsList.Contains(stream.WorkStream.WorkStreamInfo.StreamName))
                        group.WorkGroup.GroupInfo.StreamsList.Remove(stream.WorkStream.WorkStreamInfo.StreamName);
                }
                if (group.WorkGroup.GroupInfo.StreamsList.Count >= 2)
                    group.WorkGroup.GroupInfo.IsValid = true;
                else
                    group.WorkGroup.GroupInfo.IsValid = false;
            }

        }

        /// <summary>
        /// 删除了工作流
        /// </summary>
        /// <param name="obj"></param>
        /// 
        private void OnRemoveWorkStreamItem(WorkStreamItemViewModel obj)
        {
            if (obj == null) return;
            string name = (obj as WorkStreamItemViewModel).WorkStream.WorkStreamInfo.StreamName;
            foreach (var groupItem in WorkGroupItemList)
            {
                if (groupItem.WorkGroup.GroupInfo.StreamsList.Contains(name))
                    groupItem.WorkGroup.GroupInfo.StreamsList.Remove(name);
            }
        }

        /// <summary>
        /// 添加组
        /// </summary>
        private void AddGroup()
        {
            try
            {
                string name = ECDialogManager.GetUserInput(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.PleaseInputName));
                if (name == null || name.Trim() == "") return;
                if ((new Regex(@"^[A-Za-z_]{1}\w{0,100}$").IsMatch(name)))
                {
                    // 检查是否存在重复名称
                    if (!CheckNameUniqueness(Path.GetFileName(name)))
                    {
                        ECDialogManager.ShowMsg(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.DuplicateName));
                        return;
                    }

                    ECDialogManager.LoadWithAnimation(new Action(() =>
                    {
                        WorkGroupItemViewModel viewModel = new WorkGroupItemViewModel(_workName, name);
                        viewModel.WorkGroup.CreateNewConfig();

                        DispatcherHelper.UIDispatcher.Invoke(new Action(() =>
                        {
                            WorkGroupItemList.Add(viewModel);
                        }));
                        
                    }),ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.Creating));
                }
                else
                    ECDialogManager.ShowMsg(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.InvalidName));
            }
            catch (System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace+ex.Message,LogLevel.Error);
            }
        }

        /// <summary>
        /// 删除组
        /// </summary>
        /// <param name="obj"></param>
        private void RemoveGroup(object obj)
        {
            if (obj == null) return;
            if (ECDialogManager.Verify($"{ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.ConfirmRemove)} \"{(obj as WorkGroupItemViewModel).WorkGroup.GroupInfo.GroupName}\""))
            {
                WorkGroupItemViewModel viewModel = obj as WorkGroupItemViewModel;
                Messenger.Default.Send<WorkGroupItemViewModel>(viewModel, ECMessengerManager.EditWorkGroupViewModelMessengerKeys.RemoveWorkGroupItem);
                WorkGroupItemList.Remove(viewModel);
            }
        }

        /// <summary>
        /// 加载工作流组的配置
        /// </summary>
        /// <returns></returns>
        public void GetExistConfig()
        {
            WorkGroupItemList = new BindingList<WorkGroupItemViewModel>();
            string folder = ECFileConstantsManager.RootFolder + @"\" + _workName + @"\" + ECFileConstantsManager.GroupsFolderName;
            if (Directory.Exists(folder))
            {
                string[] folders = Directory.GetDirectories(folder);
                foreach (string item in folders)
                {
                    WorkGroupItemViewModel groupViewModel = new WorkGroupItemViewModel(_workName,new DirectoryInfo(item).Name);
                    groupViewModel.WorkGroup.LoadConfig();
                    WorkGroupItemList.Add(groupViewModel);
                }
            }
        }

        /// <summary>
        /// 保存配置
        /// </summary>
        public bool SaveConfig()
        {
            try
            {
                string folder = $"{ECFileConstantsManager.RootFolder}\\{_workName}\\{ECFileConstantsManager.GroupsFolderName}";
                if (Directory.Exists(folder))
                {
                    string[] folders = Directory.GetDirectories(folder);
                    foreach (string diretory in folders)
                    {
                        bool bDelete = true;
                        foreach (var item in _workGroupItemList)
                        {
                            if (item.WorkGroup.GroupInfo.GroupName == new DirectoryInfo(diretory).Name)
                            {
                                bDelete = false;
                                break;
                            }
                        }
                        if (bDelete)
                            Directory.Delete(diretory, true);
                    }
                }
                foreach (var item in _workGroupItemList)
                {
                    string directory = $"{ECFileConstantsManager.RootFolder}\\{_workName}\\{ECFileConstantsManager.GroupsFolderName}\\{item.WorkGroup.GroupInfo.GroupName}";
                    string jsonPath = directory + $"\\{ECFileConstantsManager.GroupConfigName}";
                    string tbPath= directory + $"\\{ECFileConstantsManager.GroupTBName}";
                    if (!Directory.Exists(directory)) { Directory.CreateDirectory(directory); }
                    ECSerializer.SaveObjectToJson(jsonPath, item.WorkGroup.GroupInfo);
                    CogSerializer.SaveObjectToFile(item.WorkGroup.ToolBlock, tbPath);
                }
                return true;
            }
            catch(System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message,LogLevel.Error);
                return false;
            }
        }

        /// <summary>
        /// 检查是否重名
        /// </summary>
        private bool CheckNameUniqueness(string name)
        {
            foreach (var item in WorkGroupItemList)
            {
                if (item.WorkGroup.GroupInfo.GroupName == name)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            foreach(var item in WorkGroupItemList)
            {
                item.WorkGroup.Dispose();
            }
            WorkGroupItemList.Clear();
        }
        #endregion 方法

        #region 字段

        /// <summary>
        /// 工作名称
        /// </summary>
        private string _workName;


        #endregion 字段

        #region 命令

        /// <summary>
        /// 命令：添加组
        /// </summary>
        public RelayCommand CmdAddGroup { get; set; }

        /// <summary>
        /// 命令：删除组
        /// </summary>
        public RelayCommand<object> CmdRemoveGroup { get; set; }

        #endregion 命令

        #region 属性

        private BindingList<WorkGroupItemViewModel> _workGroupItemList;

        public BindingList<WorkGroupItemViewModel> WorkGroupItemList
        {
            get { return _workGroupItemList; }
            set { _workGroupItemList = value;
                RaisePropertyChanged();
            }
        }

        #endregion 属性
    }
}
