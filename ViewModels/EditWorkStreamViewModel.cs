using Cognex.VisionPro;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using Microsoft.CodeAnalysis;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using System.Xml.Linq;
using ViDi2;
using VPDLFramework.Models;
using VPDLFramework.Views;

namespace VPDLFramework.ViewModels
{
    public class EditWorkStreamViewModel:ViewModelBase
    {
        /// <summary>
        /// 编辑工作包含的工作流
        /// </summary>
        /// <param name="workName"></param>
        public EditWorkStreamViewModel(string workName)
        {
            RegisterMessenger();
            _workName = workName;

            BindCmd();

            GetExistConfig();
            
        }

        #region 命令
        /// <summary>
        /// 添加工作流
        /// </summary>
        public RelayCommand CmdAddWorkStreamItem { get; set; }

        /// <summary>
        /// 删除工作流
        /// </summary>
        public RelayCommand<object> CmdRemoveWorkStreamItem { get; set; }

        /// <summary>
        /// 复制工作流
        /// </summary>
        public RelayCommand<object> CmdCopyWorkStreamItem { get; set; }

        /// <summary>
        /// 展开工作流编辑
        /// </summary>
        public RelayCommand<object[]> CmdExpandWorkStreamItem { get; set; }

        /// <summary>
        /// 运行工作流
        /// </summary>
        public RelayCommand<object> CmdRunWorkStreamItem { get; set; }

        /// <summary>
        /// 上移工作流
        /// </summary>
        public RelayCommand<object> CmdWorkStreamItemUp { get; set; }

        /// <summary>
        /// 下移工作流
        /// </summary>
        public RelayCommand<object> CmdWorkStreamItemDown { get; set; }

        #endregion

        #region 方法

        /// <summary>
        /// 绑定命令
        /// </summary>
        private void BindCmd()
        {
            CmdAddWorkStreamItem = new RelayCommand(AddWorkStreamItem);
            CmdRemoveWorkStreamItem = new RelayCommand<object>(RemoveWorkStreamItem);
            CmdExpandWorkStreamItem = new RelayCommand<object[]>(ExpandWorkStreamItem);
            CmdRunWorkStreamItem = new RelayCommand<object>(RunWorkStreamItem);
            CmdCopyWorkStreamItem = new RelayCommand<object>(CopyWorkStreamItem);
            CmdWorkStreamItemUp = new RelayCommand<object>(WorkStreamItemUp);
            CmdWorkStreamItemDown = new RelayCommand<object>(WorkStreamItemDown);
        }

        /// <summary>
        /// 运行工作流
        /// </summary>
        /// <param name="obj"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void RunWorkStreamItem(object obj)
        {
            Messenger.Default.Send <WorkStreamItemViewModel> (obj as WorkStreamItemViewModel, ECMessengerManager.EditWorkStreamViewModelMessengerKeys.RunWorkStreamItem);
        }

        /// <summary>
        /// 上移工作流
        /// </summary>
        /// <param name="obj"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void WorkStreamItemUp(object obj)
        {
            if (obj == null) return;
            WorkStreamItemViewModel itemViewModel = obj as WorkStreamItemViewModel;
            int oldIndex = WorkStreamItemViewModelList.IndexOf(itemViewModel);
            if (oldIndex == 0) return;
            int newIndex = oldIndex - 1;
            WorkStreamItemViewModelList.RemoveAt(oldIndex);
            WorkStreamItemViewModelList.Insert(newIndex, itemViewModel);
            SelectedWorkStreamIndex = newIndex;
        }

        /// <summary>
        /// 下移工作流
        /// </summary>
        /// <param name="obj"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void WorkStreamItemDown(object obj)
        {
            if (obj == null) return;
            WorkStreamItemViewModel itemViewModel = obj as WorkStreamItemViewModel;
            int oldIndex = WorkStreamItemViewModelList.IndexOf(itemViewModel);
            if (oldIndex == WorkStreamItemViewModelList.Count - 1) return;
            int newIndex = oldIndex + 1;
            WorkStreamItemViewModelList.RemoveAt(oldIndex);
            WorkStreamItemViewModelList.Insert(newIndex, itemViewModel);
            SelectedWorkStreamIndex = newIndex;
        }

        /// <summary>
        /// 注册订阅消息
        /// </summary>
        private void RegisterMessenger()
        {
            Messenger.Default.Register<string>(this, ECMessengerManager.WorkStreamItemViewModelMessengerKeys.SelectedGroupChanged, OnSelectedGroupChanged);
            Messenger.Default.Register<WorkGroupItemViewModel>(this, ECMessengerManager.EditWorkGroupViewModelMessengerKeys.RemoveWorkGroupItem, OnRemoveWorkGroupItem);
        }

        /// <summary>
        /// 组选项修改消息
        /// </summary>
        /// <param name="str"></param>
        private void OnSelectedGroupChanged(string str)
        {
            Messenger.Default.Send<EditWorkStreamViewModel>(this, ECMessengerManager.EditWorkStreamViewModelMessengerKeys.SelectedGroupChanged);
        }

        /// <summary>
        /// 删除工作流组消息
        /// </summary>
        private void OnRemoveWorkGroupItem(WorkGroupItemViewModel viewModel)
        {
            try
            {
                foreach (WorkStreamItemViewModel item in WorkStreamItemViewModelList)
                {
                    if (item.WorkStream.WorkStreamInfo.GroupName != null)
                    {
                        if (item.WorkStream.WorkStreamInfo.GroupName == viewModel.WorkGroup.GroupInfo.GroupName)
                            item.WorkStream.WorkStreamInfo.GroupName = null;
                    }
                }
            }
            catch(System.Exception ex) 
            {
                ECLog.WriteToLog(ex.StackTrace+ex.Message,NLog.LogLevel.Error);
            }
        }

        /// <summary>
        /// 添加工作流
        /// </summary>
        private void AddWorkStreamItem()
        {
            try
            {
                string name = ECDialogManager.GetUserInput(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.PleaseInputName));
                if (name == null || name.Trim() == "") return;
                if ((new Regex(@"^[A-Za-z_]{1}\w{0,100}$").IsMatch(name)))
                {
                    // 检查是否存在重复名称
                    if (!CheckNameUniqueness(name))
                    {
                        ECDialogManager.ShowMsg(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.DuplicateName));
                        return;
                    }

                    ECDialogManager.LoadWithAnimation(new Action(()=>
                        {
                        WorkStreamItemViewModel viewModel = new WorkStreamItemViewModel(_workName, name);
                        viewModel.WorkStream.CreateNewConfig();
                        if (!ECDLEnvironment.IsEnable)
                            viewModel.WorkStream.WorkStreamInfo.IsOnlyVpro = true;

                            DispatcherHelper.UIDispatcher.Invoke(() =>
                            {
                                WorkStreamItemViewModelList.Add(viewModel);
                            });
                        
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
        /// 添加工作流
        /// </summary>
        private void AddWorkStreamItem(WorkStreamItemViewModel item)
        {
            WorkStreamItemViewModelList.Add(item);
        }

        /// <summary>
        /// 删除工作流
        /// </summary>
        private void RemoveWorkStreamItem(object viewModel)
        {
            if (viewModel == null) return;
            if (ECDialogManager.Verify($"{ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.ConfirmRemove)} \"{(viewModel as WorkStreamItemViewModel).WorkStream.WorkStreamInfo.StreamName}\""))
            {
                Messenger.Default.Send<WorkStreamItemViewModel>(viewModel as WorkStreamItemViewModel, ECMessengerManager.EditWorkStreamViewModelMessengerKeys.RemoveWorkStreamItem);
                WorkStreamItemViewModelList.Remove(viewModel as WorkStreamItemViewModel);
            }
        }

        /// <summary>
        /// 复制工作流
        /// </summary>
        private void CopyWorkStreamItem(object viewModel)
        {
            if (viewModel == null) return;
            string name = ECDialogManager.GetUserInput(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.PleaseInputName));
            if (name == null || name.Trim() == "") return;
            if ((new Regex(@"^[A-Za-z_]{1}\w{0,100}$").IsMatch(name)))
            {
                // 检查是否存在重复名称
                if (!CheckNameUniqueness(name))
                {
                    ECDialogManager.ShowMsg(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.DuplicateName));
                    return;
                }
                WorkStreamItemViewModel tmp = new WorkStreamItemViewModel(_workName, name,viewModel as WorkStreamItemViewModel);
                AddWorkStreamItem(tmp);
            }
            else
                ECDialogManager.ShowMsg(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.InvalidName));
            
        }

        /// <summary>
        /// 检查是否重名
        /// </summary>
        private bool CheckNameUniqueness(string name)
        {
            foreach(var item in WorkStreamItemViewModelList)
            {
                if(item.WorkStream.WorkStreamInfo.StreamName==name)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private void GetExistConfig()
        {
            
            WorkStreamItemViewModelList = new BindingList<WorkStreamItemViewModel>();

            BindingList<WorkStreamItemViewModel> tmpList=new BindingList<WorkStreamItemViewModel>();

            string folder = $"{ECFileConstantsManager.RootFolder}\\{_workName}\\{ECFileConstantsManager.StreamsFolderName}";
            if (Directory.Exists(folder))
            {
                string[] folders = Directory.GetDirectories(folder);
                foreach (string item in folders)
                {
                    if (File.Exists($"{item}\\{ECFileConstantsManager.StreamConfigName}"))
                    {
                        WorkStreamItemViewModel streamViewModel = new WorkStreamItemViewModel(_workName, new DirectoryInfo(item).Name);
                        streamViewModel.WorkStream.LoadConfig();
                        tmpList.Add(streamViewModel);
                    }
                }

                // 按照ID顺序添加
                for(int i = 0;i<tmpList.Count;i++)
                {
                    foreach(var item in tmpList)
                    {
                        if(item.WorkStream.WorkStreamInfo.StreamID==i)
                            WorkStreamItemViewModelList.Add(item);
                    }
                }
            }
        }

        /// 保存配置
        /// </summary>
        public bool SaveConfig()
        {
            try
            {
                string folder = $"{ECFileConstantsManager.RootFolder}\\{_workName}\\{ECFileConstantsManager.StreamsFolderName}";
                if(!Directory.Exists(folder)) Directory.CreateDirectory(folder);
                // 彻底删除不在保存列表中的工作流文件夹
                if (Directory.Exists(folder))
                {
                    string[] folders = Directory.GetDirectories(folder);
                    foreach (string diretory in folders)
                    {
                        bool bDelete = true;
                        foreach (var item in _workStreamItemViewModelList)
                        {
                            if (item.WorkStream.WorkStreamInfo.StreamName == new DirectoryInfo(diretory).Name)
                            {
                                bDelete = false;
                                break;
                            }
                        }
                        if (bDelete)
                            Directory.Delete(diretory, true);
                    }
                }

                int id = 0; // 工作流ID
                foreach (var item in _workStreamItemViewModelList)
                {
                    item.WorkStream.WorkStreamInfo.StreamID = id;
                    string directory = $"{ECFileConstantsManager.RootFolder}\\{_workName}\\{ECFileConstantsManager.StreamsFolderName}\\{item.WorkStream.WorkStreamInfo.StreamName}";
                    
                    // 文件路径
                    string jsonPath = directory + $"\\{ECFileConstantsManager.StreamConfigName}";
                    string tbDLInputPath = directory + $"\\{ECFileConstantsManager.DLInputTBName}";
                    string tbDLOutputPath = directory + $"\\{ECFileConstantsManager.DLOutputTBName}";
                    
                    // 检查目录
                    if (!Directory.Exists(directory)) { Directory.CreateDirectory(directory); }

                    // 保存配置信息
                    ECSerializer.SaveObjectToJson(jsonPath, item.WorkStream.WorkStreamInfo); // 配置文件
                    if (item.WorkStream.DLInputTB != null)
                    {
                        CogSerializer.SaveObjectToFile(item.WorkStream.DLInputTB, tbDLInputPath);
                    }
                     // 输入ToolBlock
                    CogSerializer.SaveObjectToFile(item.WorkStream.DLOutputTB, tbDLOutputPath); // 输出ToolBlock

                    // 保存配方
                    string recipesDirectory = directory + @"\" + ECFileConstantsManager.RecipesFolderName;
                    ECRecipesManager.SaveRecipe(recipesDirectory, item.WorkStream.Recipes);                    

                    // 保存高级DL模式设置
                    if (item.WorkStream.WorkStreamInfo.IsUseAdvancedDLModel)
                    {
                        string modelFolder= $"{ECFileConstantsManager.RootFolder}\\{_workName}\\{ECFileConstantsManager.StreamsFolderName}\\{item.WorkStream.WorkStreamInfo.StreamName}\\{ECFileConstantsManager.AdvancedDLModelFolderName}";
                        
                        // 如果文件夹已经存在则删除
                        if (Directory.Exists(modelFolder))
                            Directory.Delete(modelFolder, true);

                        // 创建文件夹
                        Directory.CreateDirectory(modelFolder); 
                        
                        // 遍历所有的步骤
                        foreach(ECAdvancedStep step in item.WorkStream.AdvancedDLSteps)
                        {
                            string stepFolder = modelFolder + @"\" + step.StepName;
                            if (!Directory.Exists(stepFolder)) { Directory.CreateDirectory(stepFolder); }

                            // 遍历步骤包含的所有工具
                            foreach(ECAdvancedTool tool in step.Tools)
                            {
                                string toolFolder = stepFolder + @"\" + tool.ToolInfo.ToolName;
                                if (!Directory.Exists(toolFolder)) { Directory.CreateDirectory(toolFolder); }
                                string toolJsonPath = toolFolder + @"\" + ECFileConstantsManager.AdvancedToolConfigName;
                                string toolTBPath = toolFolder + @"\" + ECFileConstantsManager.AdvancedToolTBName;
                                ECSerializer.SaveObjectToJson(toolJsonPath, tool.ToolInfo);
                                CogSerializer.SaveObjectToFile(tool.ToolBlock, toolTBPath);
                            }
                        }
                    }

                    id++; // 工作ID累加1
                }

                
                return true;
            }
            catch (System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, LogLevel.Error);
                return false;
            }

        }

        /// <summary>
        /// 展开对工作流项目的编辑
        /// </summary>
        /// <param name="obj"></param>
        private void ExpandWorkStreamItem(object[] obj)
        {
            if(obj == null) { return; }
            try
            {
                WorkStreamItemViewModel streamItemViewModel = obj[0] as WorkStreamItemViewModel;
                _expandedStream = streamItemViewModel;
                WorkImageSourceItemViewModel imageSourceViewModel = obj[1] as WorkImageSourceItemViewModel;

                if (imageSourceViewModel == null || imageSourceViewModel.WorkImageSource.ImageSourceInfo.ImageFilePath == null) { ECDialogManager.ShowMsg(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.ImageSourceIsNullOrNoImages)); return; }

                // 显示窗口
                Window_ExpandWorkStream window = new Window_ExpandWorkStream();
                window.Filmstrip.DataContext = imageSourceViewModel;
                window.InputToolBlock = _expandedStream.WorkStream.DLInputTB;
                window.OutputToolBlock = _expandedStream.WorkStream.DLOutputTB;
                window.DataContext = _expandedStream;

                // 注册运行选中图片的消息
                Messenger.Default.Register<ICogImage>(this, ECMessengerManager.ExpandedWorkStreamFilmstripMessengerKeys.RunSelectedImage, OnFilmstripRunSelectedImage);
                window.ShowDialog();

                // 清空加载的图片
                imageSourceViewModel.ImageList?.Clear();

                // 注销消息
                Messenger.Default.Unregister<ICogImage>(this, ECMessengerManager.ExpandedWorkStreamFilmstripMessengerKeys.RunSelectedImage, OnFilmstripRunSelectedImage);
            }
            catch(System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace+ex.Message,LogLevel.Error);
            }
        }

        /// <summary>
        /// 胶片运行选中的图片
        /// </summary>
        /// <param name="obj"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void OnFilmstripRunSelectedImage(ICogImage obj)
        {
            if(obj != null)
            {
                ECWorkImageSourceOutput imageSourceOutput = new ECWorkImageSourceOutput();
                imageSourceOutput.Image=obj;
                _expandedStream.WorkStream.Run(imageSourceOutput);
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            foreach (var item in WorkStreamItemViewModelList)
            {
                item.WorkStream.Dispose();
            }
            WorkStreamItemViewModelList.Clear();
        }
        #endregion

        #region 属性

        /// <summary>
        /// 图像源集合视图
        /// </summary>
        private BindingList<WorkStreamItemViewModel> _workStreamItemViewModelList;

        public BindingList<WorkStreamItemViewModel> WorkStreamItemViewModelList
        {
            get { return _workStreamItemViewModelList; }
            set
            {
                _workStreamItemViewModelList = value;
                RaisePropertyChanged();
            }
        }


        /// <summary>
        /// 选择的工作流索引
        /// </summary>
        private int _selectedWorkStreamIndex;

        public int SelectedWorkStreamIndex
        {
            get { return _selectedWorkStreamIndex; }
            set { _selectedWorkStreamIndex = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region 字段
        /// <summary>
        /// 工作名
        /// </summary>
        private string _workName;

        /// <summary>
        /// 展开的工作流
        /// </summary>
        private WorkStreamItemViewModel _expandedStream;
        #endregion

    }
}
