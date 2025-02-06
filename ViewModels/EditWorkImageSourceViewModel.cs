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
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VPDLFramework.Models;

namespace VPDLFramework.ViewModels
{
    public class EditWorkImageSourceViewModel : ViewModelBase
    {

        /// <summary>
        /// 编辑工作包含的图像源
        /// </summary>
        /// <param name="workName"></param>
        public EditWorkImageSourceViewModel(string workName)
        {
            _workName = workName;
            
            CmdAddImageSource = new RelayCommand(AddImageSourceViewModels);
            CmdRemoveImageSource = new RelayCommand<object>(RemoveImageSource);

            GetExistConfig();
        }

        #region 命令
        /// <summary>
        /// 添加图像源
        /// </summary>
        public RelayCommand CmdAddImageSource { get; set; }

        /// <summary>
        /// 删除图像源
        /// </summary>
        public RelayCommand<object> CmdRemoveImageSource { get; set; }

        #endregion

        #region 方法

        /// <summary>
        /// 添加图像源
        /// </summary>
        private void AddImageSourceViewModels()
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
                    ECDialogManager.LoadWithAnimation(new Action(() =>
                    {
                        WorkImageSourceItemViewModel viewModel = new WorkImageSourceItemViewModel(_workName, name);
                        viewModel.WorkImageSource.CreateNewConfig();

                        DispatcherHelper.UIDispatcher.Invoke(() =>
                        {
                            ImageSourceViewModelList.Add(viewModel);
                        });
                    }) ,ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.Creating));
                    
                }
                else
                    ECDialogManager.ShowMsg(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.InvalidName));
            }
            catch(System.Exception ex) 
            {
                ECLog.WriteToLog(ex.StackTrace+ex.Message,LogLevel.Error);
            }
        }

        /// <summary>
        /// 删除图像源
        /// </summary>
        private void RemoveImageSource(object imageSourceViewModel)
        {
            if (imageSourceViewModel == null) return;
            if (ECDialogManager.Verify($"{ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.ConfirmRemove)} " +
                $"\"{(imageSourceViewModel as WorkImageSourceItemViewModel).WorkImageSource.ImageSourceInfo.ImageSourceName}\""))
                ImageSourceViewModelList.Remove(imageSourceViewModel as WorkImageSourceItemViewModel);
        }

        /// <summary>
        /// 获取已有的配置
        /// </summary>
        /// <returns></returns>
        private void GetExistConfig()
        {
            try
            {
                ImageSourceViewModelList = new BindingList<WorkImageSourceItemViewModel>();

                string folder = $"{ECFileConstantsManager.RootFolder}\\{_workName}\\{ECFileConstantsManager.ImageSourceFolderName}";
                if (Directory.Exists(folder))
                {
                    string[] folders = Directory.GetDirectories(folder);
                    foreach (string item in folders)
                    {
                        if (File.Exists($"{item}\\{ECFileConstantsManager.ImageSourceConfigName}"))
                        {
                            WorkImageSourceItemViewModel viewModel = new WorkImageSourceItemViewModel(_workName, new DirectoryInfo(item).Name);
                            viewModel.WorkImageSource.LoadConfig();
                            ImageSourceViewModelList.Add(viewModel);
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, NLog.LogLevel.Error);
            }
        }

        /// <summary>
        /// 保存配置
        /// </summary>
        public bool SaveConfig()
        {
            try
            {
                string folder = $"{ECFileConstantsManager.RootFolder}\\{_workName}\\{ECFileConstantsManager.ImageSourceFolderName}";
                if (Directory.Exists(folder))
                {
                    string[] folders = Directory.GetDirectories(folder);
                    foreach (string diretory in folders)
                    {
                        bool bDelete = true;
                        foreach (var item in _imageSourceViewModelList)
                        {
                            if (item.WorkImageSource.ImageSourceInfo.ImageSourceName == new DirectoryInfo(diretory).Name)
                            {
                                bDelete = false;
                                break;
                            }
                        }
                        if (bDelete)
                            Directory.Delete(diretory,true);
                    }
                }
                foreach (var item in _imageSourceViewModelList)
                {
                    string directory = $"{ECFileConstantsManager.RootFolder}\\{_workName}\\{ECFileConstantsManager.ImageSourceFolderName}\\{item.WorkImageSource.ImageSourceInfo.ImageSourceName}";
                    string jsonPath = directory + $"\\{ECFileConstantsManager.ImageSourceConfigName}";
                    string tbPath = directory + $"\\{ECFileConstantsManager.ImageSourceAcqTBName}";
                    if (!Directory.Exists(directory)) { Directory.CreateDirectory(directory); }
                    ECSerializer.SaveObjectToJson(jsonPath, item.WorkImageSource.ImageSourceInfo);
                    CogSerializer.SaveObjectToFile(item.WorkImageSource.ToolBlock, tbPath);
                }
                return true;
            }
            catch (Exception ex)
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
            foreach (var item in ImageSourceViewModelList)
            {
                if (item.WorkImageSource.ImageSourceInfo.ImageSourceName == name)
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
            foreach(var item in ImageSourceViewModelList)
            {
                item.WorkImageSource.Dispose();
            }
            ImageSourceViewModelList.Clear();
        }

        #endregion

        #region 属性
        /// <summary>
        /// 图像源集合视图
        /// </summary>
        private BindingList<WorkImageSourceItemViewModel> _imageSourceViewModelList;

        public BindingList<WorkImageSourceItemViewModel> ImageSourceViewModelList
        {
            get { return _imageSourceViewModelList; }
            set
            {
                _imageSourceViewModelList = value;
                RaisePropertyChanged();
            }
        }


        #endregion

        #region 字段

        private string _workName;
        #endregion
    }

}