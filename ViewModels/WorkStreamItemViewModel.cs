using GalaSoft.MvvmLight;
using VPDLFramework.Models;
using static VPDLFramework.Models.ECWorkOptionManager;
using System.ComponentModel;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Cognex.VisionPro.ToolBlock;
using System.Collections.Generic;
using Cognex.VisionPro;
using System.Drawing;
using System;
using ViDi2;
using Microsoft.International.Converters.PinYinConverter;
using System.Linq;
using NLog;
using System.Windows;
using VPDLFramework.Views;
using System.Text.RegularExpressions;
using System.Reflection;

namespace VPDLFramework.ViewModels
{
    public class WorkStreamItemViewModel : ViewModelBase
    {
        public WorkStreamItemViewModel(string workName, string streamName)
        {
            WorkStream = new ECWorkStream(workName, streamName);
            GPUList = ECDLEnvironment.GPUList;
            BindCmd();
            InitialBindableList();
        }
        public WorkStreamItemViewModel(string workName, string streamName,WorkStreamItemViewModel item)
        {
            try
            {
                WorkStream = new ECWorkStream(workName, streamName);
                GPUList = ECDLEnvironment.GPUList;
                BindCmd();
                InitialBindableList();
                // 复制输入、输出ToolBlock
                WorkStream.DLInputTB = ECGeneric.DeepCopy(item.WorkStream.DLInputTB);
                WorkStream.DLOutputTB = ECGeneric.DeepCopy(item.WorkStream.DLOutputTB);
                WorkStream.Recipes = item.WorkStream.Recipes;
                WorkStream.AdvancedDLSteps = new BindingList<ECAdvancedStep>();
                // 复制DL高级模式步骤
                foreach (ECAdvancedStep step in item.WorkStream.AdvancedDLSteps)
                {
                    ECAdvancedStep tmpStep = new ECAdvancedStep(step.StepName);
                    foreach (ECAdvancedTool tool in step.Tools)
                    {
                        if (tool.ToolInfo.IsDLType)
                            tmpStep.Tools.Add(tool);
                        else
                        {
                            ECAdvancedTool tmpTool = new ECAdvancedTool(tool.ToolInfo.ToolName);
                            tmpTool.ToolInfo.IsDLType = false;
                            tmpTool.ToolBlock = ECGeneric.DeepCopy(tool.ToolBlock);
                        }

                    }
                    WorkStream.AdvancedDLSteps.Add(step);
                }
                // 深度复制工作流信息
                WorkStream.WorkStreamInfo=ECGeneric.DeepCopyByReflection(item.WorkStream.WorkStreamInfo);
                WorkStream.WorkStreamInfo.StreamName = streamName;
            }
            catch(System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace+ex.Message,NLog.LogLevel.Error);
            }
        }

        #region 命令

        /// <summary>
        /// 选择组
        /// </summary>
        public RelayCommand CmdSelectGroup { get; set; }

        /// <summary>
        /// 添加DL结果到输出工具块
        /// </summary>
        public RelayCommand<object> CmdAddDLResultTerminalsDLOutputTB { get; set; }

        /// <summary>
        /// 编辑输入工具块
        /// </summary>
        public RelayCommand<object> CmdEditInputToolBlock { get; set; }

        /// <summary>
        /// 编辑输出工具块
        /// </summary>
        public RelayCommand<object> CmdEditOutputToolBlock { get; set; }

        /// <summary>
        /// 编辑高级DL模式
        /// </summary>
        public RelayCommand<object[]> CmdEditAdvancedDLModel { get; set; }

        /// <summary>
        /// 添加高级模式步骤
        /// </summary>
        public RelayCommand CmdAddAdvancedDLModelStep { get; set; }

        /// <summary>
        /// 删除高级模式步骤
        /// </summary>
        public RelayCommand<object> CmdRemoveAdvancedDLModelStep { get; set; }

        /// <summary>
        /// 添加DL类型高级工具
        /// </summary>
        public RelayCommand<object> CmdAddAdvancedDLToolDLTypeToStep { get; set; }

        /// <summary>
        /// 添加非DL类型高级工具
        /// </summary>
        public RelayCommand<object> CmdAddAdvancedDLToolNotDLTypeToStep { get; set; }

        /// <summary>
        /// 编辑高级工具TB
        /// </summary>
        public RelayCommand<object> CmdEditAdvancedToolTB { get; set; }

        /// 删除高级工具
        /// </summary>
        public RelayCommand<object[]> CmdRemoveAdvancedTool { get; set; }

        /// <summary>
        /// 刷新高级模式参数列表
        /// </summary>
        public RelayCommand CmdUpdateAdvancedModelParaList { get; set; }

        /// <summary>
        /// 传递输入参数列表到工具
        /// </summary>
        public RelayCommand<object> CmdTransferInputParaListToTools { get; set; }

        /// <summary>
        /// 添加工作流配方
        /// </summary>
        public RelayCommand CmdAddWorkStreamRecipe { get; set; }

        /// <summary>
        /// 删除工作流配方
        /// </summary>
        public RelayCommand<object> CmdRemoveWorkStreamRecipe { get; set; }

        /// <summary>
        /// 加载工作流配方
        /// </summary>
        public RelayCommand<object> CmdLoadWorkStreamRecipe { get; set; }

        #endregion

        #region 方法

        /// <summary>
        /// 绑定命令
        /// </summary>
        private void BindCmd()
        {
            CmdSelectGroup = new RelayCommand(SelectGroup);
            CmdEditInputToolBlock = new RelayCommand<object>(EditInputToolBlock);
            CmdEditOutputToolBlock = new RelayCommand<object>(EditOutputToolBlock);
            CmdAddDLResultTerminalsDLOutputTB = new RelayCommand<object>(AddDLResultTermianalsToDLOutputTB);
            CmdEditAdvancedDLModel = new RelayCommand<object[]>(EditAdvancedDLModel);
            CmdAddAdvancedDLModelStep = new RelayCommand(AddAdvancedDLModelStep);
            CmdRemoveAdvancedDLModelStep = new RelayCommand<object>(RemoveAdvancedDLModelStep);
            CmdAddAdvancedDLToolDLTypeToStep = new RelayCommand<object>(AddAdvancedToolDLTypeToStep);
            CmdAddAdvancedDLToolNotDLTypeToStep = new RelayCommand<object>(AddAdvancedToolNotDLTypeToStep);
            CmdEditAdvancedToolTB = new RelayCommand<object>(EditAdvancedToolTB);
            CmdRemoveAdvancedTool=new RelayCommand<object[]> (RemoveAdvancedTool);
            CmdUpdateAdvancedModelParaList = new RelayCommand(UpdateAdvancedModelParaList);
            CmdTransferInputParaListToTools=new RelayCommand<object>(TransferInputParaListToTools);
            CmdAddWorkStreamRecipe = new RelayCommand(AddWorkStreamRecipe);
            CmdRemoveWorkStreamRecipe=new RelayCommand<object>(RemoveWorkStreamRecipe);
            CmdLoadWorkStreamRecipe=new RelayCommand<object> (LoadWorkStreamRecipe);
        }

        /// <summary>
        /// 初始化绑定列表
        /// </summary>
        private void InitialBindableList()
        {
            ResultGraphicConstantsBindableList = ECGeneric.GetConstantsBindableList<ResultGraphiConstants>();
            ImageRecordConstantsBindableList = ECGeneric.GetConstantsBindableList<ImageRecordConstants>();
            ResultSendTypeConstantsBindableList = ECGeneric.GetConstantsBindableList<ResultSendTypeConstants>();
            IOOutputConstantsBindableList = ECGeneric.GetConstantsBindableList<IOOutputConstants>();
            ImageRecordConditionConstantsBindableList = ECGeneric.GetConstantsBindableList<ImageRecordConditionConstants>();
            TriggerTypeConstantsBindableList = ECGeneric.GetConstantsBindableList<TriggerTypeConstants>();
            IOInputConstantsBindableList = ECGeneric.GetConstantsBindableList<IOInputConstants>();
            SoftEventConstantsBindableList=ECGeneric.GetConstantsBindableList<SoftEventConstants>();
            OriginalImageTypeContstantsBindableList = ECGeneric.GetConstantsBindableList<QriginalImageTypeContstans>();
        }

        /// <summary>
        /// 选择组
        /// </summary>
        private void SelectGroup()
        {
            Messenger.Default.Send<string>("", ECMessengerManager.WorkStreamItemViewModelMessengerKeys.SelectedGroupChanged);
        }

        /// <summary>
        /// 编辑输入工具块
        /// </summary>
        private void EditInputToolBlock(object obj)
        {
            WorkStreamItemViewModel viewModel = obj as WorkStreamItemViewModel;
            viewModel.WorkStream.DLInputTB= ECDialogManager.EditToolBlock(viewModel.WorkStream.DLInputTB);
        }

        /// <summary>
        /// 编辑输出工具块
        /// </summary>
        private void EditOutputToolBlock(object obj)
        {
            WorkStreamItemViewModel viewModel = obj as WorkStreamItemViewModel;
            viewModel.WorkStream.DLOutputTB=ECDialogManager.EditToolBlock(viewModel.WorkStream.DLOutputTB);
        }

        /// <summary>
        /// 编辑DL高级模式
        /// </summary>
        private void EditAdvancedDLModel(object[] obj)
        {
            if (obj.Length != 2 || obj[0] == null || obj[1] == null) return;
            try
            {
                WorkStreamItemViewModel viewModel = obj[0] as WorkStreamItemViewModel;
                DLWorkspaces = obj[1] as BindingList<WorkDLWorkspaceItemViewModel>;
                Window_EditAdvancedDLModel winEditAdvancedDL = new Window_EditAdvancedDLModel();
                winEditAdvancedDL.DataContext = obj[0];
                winEditAdvancedDL.ShowDialog();
            }
            catch (System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, LogLevel.Error);
            }
        }

        /// <summary>
        /// 编辑DL高级工具工具块
        /// </summary>
        private void EditAdvancedToolTB(object obj)
        {
            (obj as ECAdvancedTool).ToolBlock= ECDialogManager.EditToolBlock((obj as ECAdvancedTool).ToolBlock);
        }

        /// <summary>
        /// 删除高级工具
        /// </summary>
        private void RemoveAdvancedTool(object[] obj)
        {
            try
            {
                if (obj != null)
                {
                    ECAdvancedStep step = obj[0] as ECAdvancedStep;
                    ECAdvancedTool tool = obj[1] as ECAdvancedTool;

                    if (ECDialogManager.Verify($"{ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.ConfirmRemove)} \"{tool.ToolInfo.ToolName}\""))
                        step.Tools.Remove(tool);
                }
            }
            catch(System.Exception ex) 
            { 
                ECLog.WriteToLog(ex.StackTrace+ex.Message,NLog.LogLevel.Error);
            }
        }

        /// <summary>
        /// 添加高级DL模式步骤
        /// </summary>
        private void AddAdvancedDLModelStep()
        {
            WorkStream.AdvancedDLSteps.Add(new ECAdvancedStep("STEP"+(WorkStream.AdvancedDLSteps.Count + 1).ToString()));
        }

        /// <summary>
        /// 添加高级DL模式步骤
        /// </summary>
        private void RemoveAdvancedDLModelStep(object obj)
        {
            if (WorkStream.AdvancedDLSteps.IndexOf(obj as ECAdvancedStep) == WorkStream.AdvancedDLSteps.Count - 1)
            {
               if(ECDialogManager.Verify($"{ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.ConfirmRemove)} \"{(obj as ECAdvancedStep).StepName}\""))
                    WorkStream.AdvancedDLSteps.Remove(obj as ECAdvancedStep);
            }
            else
                ECDialogManager.ShowMsg(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.PleaseRemoveLatterStep));
        }

        /// <summary>
        /// 添加高级DL模式工具到步骤
        /// </summary>
        private void AddAdvancedToolDLTypeToStep(object obj)
        {
            if (obj == null) return;
            string name = ECDialogManager.GetUserInput(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.PleaseInputName));
            if (name == null || name.Trim() == "") return;
            if ((new Regex(@"^[A-Za-z_]{1}\w{0,100}$").IsMatch(name)))
            {
                ECAdvancedStep step = obj as ECAdvancedStep;
                if (CheckNameUniqueness(step, name))
                {
                    ECAdvancedTool tool = new ECAdvancedTool(name);
                    tool.ToolInfo.IsDLType = true;
                    tool.ToolBlock.Inputs.Add(new CogToolBlockTerminal("DefaultOutputImage", typeof(ICogImage)));
                    tool.ToolBlock.Outputs.Add(new CogToolBlockTerminal("DefaultToolEnable",typeof(Boolean)));
                    tool.ToolBlock.Outputs.Add(new CogToolBlockTerminal("DefaultOutputImage", typeof(ICogImage)));
                    step.Tools.Add(tool);
                }
                else
                    ECDialogManager.ShowMsg(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.DuplicateName));
            }
            else
                ECDialogManager.ShowMsg(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.InvalidName));
        }

        /// <summary>
        /// 添加高级DL模式工具到步骤
        /// </summary>
        private void AddAdvancedToolNotDLTypeToStep(object obj)
        {
            if (obj == null) return;
            string name = ECDialogManager.GetUserInput(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.PleaseInputName));
            if (name == null || name.Trim() == "") return;
            if ((new Regex(@"^[A-Za-z_]{1}\w{0,100}$").IsMatch(name)))
            {
                ECAdvancedStep step = obj as ECAdvancedStep;
                if (CheckNameUniqueness(step, name))
                {
                    ECAdvancedTool tool = new ECAdvancedTool(name);
                    tool.ToolInfo.IsDLType = false;
                    tool.ToolBlock.Outputs.Add(new CogToolBlockTerminal("DefaultToolEnable", typeof(Boolean)));
                    step.Tools.Add(tool);
                }
                else
                {
                    ECDialogManager.ShowMsg(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.DuplicateName));
                }
            }
            else
                ECDialogManager.ShowMsg(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.InvalidName));
        }

        /// <summary>
        /// 传递DL工具结果参数到输出工具块
        /// </summary>
        private void AddDLResultTermianalsToDLOutputTB(object obj)
        {
            try
            {
                if (obj == null) return;
                WorkStreamItemViewModel item = obj as WorkStreamItemViewModel;
                CogToolBlockTerminalCollection collection = ECDLEnvironment.GetDLStreamResultTerminals(item.WorkStream.WorkStreamInfo.WorkspaceName, item.WorkStream.WorkStreamInfo.WorkspaceStreamName);
                foreach (CogToolBlockTerminal terminal in collection)
                {
                    if (item.WorkStream.DLOutputTB.Inputs.Contains(terminal.Name))
                        item.WorkStream.DLOutputTB.Inputs.Remove(terminal.Name);
                    item.WorkStream.DLOutputTB.Inputs.Add(new CogToolBlockTerminal(terminal.Name, terminal.ValueType));
                }
                ECDialogManager.ShowMsg(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.ResultsOfDLToolAddedToOutputTB));
            }
            catch(System.Exception ex)
            {
                ECDialogManager.ShowMsg(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.ErrorOccured));
                ECLog.WriteToLog(ex.StackTrace+ex.Message,NLog.LogLevel.Error);
            }
        }

        /// <summary>
        /// 刷新DL高级模式步骤参数列表
        /// </summary>
        /// <param name="obj"></param>
        private void UpdateAdvancedModelParaList()
        {
            WorkStream.UpdateAdvancedModelParaList();
            
        }

        /// <summary>
        /// 传递输入参数列表到工具
        /// </summary>
        /// <param name="obj"></param>
        private void TransferInputParaListToTools(object obj)
        {
            if(obj == null) return;
            ECAdvancedStep step = obj as ECAdvancedStep;
            foreach(ECAdvancedTool tool in step.Tools)
            {
                foreach(var input in step.InputParaList)
                {
                    if(tool.ToolBlock.Inputs.Contains(input.Key))
                        tool.ToolBlock.Inputs.Remove(input.Key);
                    tool.ToolBlock.Inputs.Add(new CogToolBlockTerminal(input.Key, input.Value));
                }
            }

        }

        /// <summary>
        /// 检查是否重名
        /// </summary>
        private bool CheckNameUniqueness(ECAdvancedStep step, string name)
        {
            foreach (var item in step.Tools)
            {
                if (item.ToolInfo.ToolName == name)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 添加工作流配方
        /// </summary>
        /// <param name="obj"></param>
        private void AddWorkStreamRecipe()
        {
            try
            {
                string name = ECDialogManager.GetUserInput(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.PleaseInputName));
                if (name == null || name.Trim() == "") return;
                if ((new Regex(@"^[A-Za-z_]{1}\w{0,100}$").IsMatch(name)))
                {
                    // 检查是否存在重复名称
                    foreach (ECRecipe recipe in WorkStream.Recipes)
                    {
                        if (name == recipe.RecipeName)
                        {
                            ECDialogManager.ShowMsg(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.DuplicateName));
                            return;
                        }
                    }
                    // 添加新的配方
                    if (WorkStream.Recipes == null)
                        WorkStream.Recipes = new BindingList<ECRecipe>();
                    ECRecipe newRecipe = ECRecipesManager.FilterValidRecipe(WorkStream.WorkStreamInfo.IsOnlyVpro ? WorkStream.DLOutputTB.Inputs : WorkStream.DLInputTB.Inputs, name);
                    if (newRecipe != null && newRecipe.Values.Count > 0)
                        WorkStream.Recipes.Add(newRecipe);
                    else
                        ECDialogManager.ShowMsg($"{ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.ValidType)} (int,double,string)");
                }
                else
                    ECDialogManager.ShowMsg(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.InvalidName));
            }
            catch(System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, NLog.LogLevel.Error);
            }

        }

        /// <summary>
        /// 添加工作流配方
        /// </summary>
        /// <param name="obj"></param>
        private void RemoveWorkStreamRecipe(object obj)
        {
            if (obj == null) { return; }
            ECRecipe recipe=obj as ECRecipe;
            if(ECDialogManager.Verify($"{ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.ConfirmRemove)} \"{recipe.RecipeName}\""))
                WorkStream.Recipes.Remove(recipe);

        }

        /// <summary>
        /// 加载工作流配方
        /// </summary>
        /// <param name="obj"></param>
        private void LoadWorkStreamRecipe(object obj)
        {
            if (obj == null) { return; }
            ECRecipe recipe = obj as ECRecipe;
            if (WorkStream.LoadRecipe(recipe.RecipeName))
                ECDialogManager.ShowMsg(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.ImportFinished));
            else
                ECDialogManager.ShowMsg(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.ImportFailed));
        }

        #endregion

        #region 属性

        /// <summary>
        /// 工作流
        /// </summary>
        private ECWorkStream _workStream;

        public ECWorkStream WorkStream
        {
            get { return _workStream; }
            set
            {
                _workStream = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// GPU列表
        /// </summary>
        private BindingList<string> _GPUList;

        public BindingList<string> GPUList
        {
            get { return _GPUList; }
            set
            {
                _GPUList = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// DL工作区视图列表
        /// </summary>
        private BindingList<WorkDLWorkspaceItemViewModel> _DLWorkspaces;

        public BindingList<WorkDLWorkspaceItemViewModel> DLWorkspaces
        {
            get { return _DLWorkspaces; }
            set
            {
                _DLWorkspaces = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 可绑定的结果图形选项列表
        /// </summary>
        private BindingList<string> _resultGraphicConstantsBindableList;

        public BindingList<string> ResultGraphicConstantsBindableList
        {
            get
            { return _resultGraphicConstantsBindableList; }
            set
            {
                _resultGraphicConstantsBindableList = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 可绑定的图像记录选项列表
        /// </summary>
        private BindingList<string> _imageRecordConstantsBindableList;

        public BindingList<string> ImageRecordConstantsBindableList
        {
            get
            { return _imageRecordConstantsBindableList; }
            set
            {
                _imageRecordConstantsBindableList = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 可绑定的图像记录条件列表
        /// </summary>
        private BindingList<string> _imageRecordConditionConstantsBindableList;

        public BindingList<string> ImageRecordConditionConstantsBindableList
        {
            get
            { return _imageRecordConditionConstantsBindableList; }
            set
            {
                _imageRecordConditionConstantsBindableList = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 可绑定触发类型选项列表
        /// </summary>
        private BindingList<string> _triggerTypeConstantsBindableList;

        public BindingList<string> TriggerTypeConstantsBindableList
        {
            get { return _triggerTypeConstantsBindableList; }
            set
            {
                _triggerTypeConstantsBindableList = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 可绑定的IO输入选项列表
        /// </summary>
        private BindingList<string> _IOInputConstantsBindableList;

        public BindingList<string> IOInputConstantsBindableList
        {
            get
            { return _IOInputConstantsBindableList; }
            set
            {
                _IOInputConstantsBindableList = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 可绑定结果输出类型选项列表
        /// </summary>
        private BindingList<string> _resultSendTypeConstantsBindableList;

        public BindingList<string> ResultSendTypeConstantsBindableList
        {
            get { return _resultSendTypeConstantsBindableList; }
            set
            {
                _resultSendTypeConstantsBindableList = value;
                RaisePropertyChanged();
            }
        }


        /// <summary>
        /// 可绑定的IO输出选项列表
        /// </summary>
        private BindingList<string> _IOOutputConstantsBindableList;

        public BindingList<string> IOOutputConstantsBindableList
        {
            get
            { return _IOOutputConstantsBindableList; }
            set
            {
                _IOOutputConstantsBindableList = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 可绑定的SoftEvent选项列表
        /// </summary>
        private BindingList<string> _softEventConstantsBindableList;

        public BindingList<string> SoftEventConstantsBindableList
        {
            get
            { return _softEventConstantsBindableList; }
            set
            {
                _softEventConstantsBindableList = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 可绑定的原图类型选项列表
        /// </summary>
        private BindingList<string> _originalImageTypeContstantsBindableList;

        public BindingList<string> OriginalImageTypeContstantsBindableList
        {
            get
            { return _originalImageTypeContstantsBindableList; }
            set
            {
                _originalImageTypeContstantsBindableList = value;
                RaisePropertyChanged();
            }
        }

        #endregion
    }
}
