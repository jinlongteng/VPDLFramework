using Cognex.VisionPro;
using Cognex.VisionPro.ToolBlock;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.DwayneNeed.Win32;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VPDLFramework.Models;
using static VPDLFramework.Models.ECWorkOptionManager;

namespace VPDLFramework.ViewModels
{
    public class WorkGroupItemViewModel:ViewModelBase
    {
        public WorkGroupItemViewModel(string workName,string groupName) 
        {
            WorkGroup=new ECWorkStreamsGroup(workName,groupName);
            CmdEditGroupToolBlock = new RelayCommand<object>(EditGroupToolBlock);
            ResultSendTypeConstantsBindableList = ECGeneric.GetConstantsBindableList<ResultSendTypeConstants>();
            IOOutputConstantsBindableList = ECGeneric.GetConstantsBindableList<IOOutputConstants>();
            IOInputSignalTypeConstantsBindableList = ECGeneric.GetConstantsBindableList<IOInputSignalTypeConstants>();
            ResultGraphicConstantsBindableList = ECGeneric.GetConstantsBindableList<ResultGraphiConstants>();
        }

        #region 方法

        /// <summary>
        /// 编辑组ToolBlock
        /// </summary>
        /// <param name="obj"></param>
        private void EditGroupToolBlock(object obj)
        {
            try
            {
                if (obj == null) return;
                EditWorkStreamViewModel viewModel = obj as EditWorkStreamViewModel;
                if (ECDialogManager.Verify(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.CheckWorkStreamUpdate)))
                {
                    // 遍历包含的工作流
                    foreach (string streamName in WorkGroup.GroupInfo.StreamsList)
                    {
                        // 获取工作流用户定义的输出集合
                        CogToolBlockTerminalCollection streamCustomOutputs = viewModel.WorkStreamItemViewModelList.Where(t => t.WorkStream.WorkStreamInfo.StreamName == streamName).First().WorkStream.GetCustomOutputs();

                        // 遍历输出集合
                        foreach (CogToolBlockTerminal terminal in streamCustomOutputs)
                        {
                            // 当组ToolBlock不包含此参数时
                            if (!WorkGroup.ToolBlock.Inputs.Contains(terminal.Name))
                            {
                                // 此参数值为null,添加参数名称和类型到组ToolBlock输入
                                if (terminal.Value == null)
                                    WorkGroup.ToolBlock.Inputs.Add(new CogToolBlockTerminal(terminal.Name, terminal.ValueType));
                                else
                                {
                                    // 此参数值不为null且为值类型,可以直接添加其名称和值到组ToolBlock输入
                                    if (terminal.ValueType.IsValueType)
                                        WorkGroup.ToolBlock.Inputs.Add(new CogToolBlockTerminal(terminal.Name, terminal.Value));
                                    // 此参数值不为null且为引用类型,则添加其名称和值的深度拷贝到组ToolBlock输入
                                    else
                                        WorkGroup.ToolBlock.Inputs.Add(new CogToolBlockTerminal(terminal.Name, ECGeneric.DeepCopy(terminal.Value)));
                                }
                            }
                            // 当组包含此参数名称且类型相同或可转换时则进行赋值
                            else if (WorkGroup.ToolBlock.Inputs.Contains(terminal.Name) &&
                               (terminal.ValueType == WorkGroup.ToolBlock.Inputs[terminal.Name].ValueType || WorkGroup.ToolBlock.Inputs[terminal.Name].ValueType.IsInstanceOfType(terminal.Value)))
                            {
                                if(terminal.Value!=null)
                                {
                                    if (terminal.ValueType.IsValueType)
                                        WorkGroup.ToolBlock.Inputs[terminal.Name].Value = terminal.Value;
                                    else
                                        WorkGroup.ToolBlock.Inputs[terminal.Name].Value = ECGeneric.DeepCopy(terminal.Value);
                                } 
                            }
                            else
                                ECLog.WriteToLog(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.DuplicateNameIsNotAllowed), NLog.LogLevel.Warn);
                        }
                    }
                }
                WorkGroup.ToolBlock=ECDialogManager.EditToolBlock(WorkGroup.ToolBlock);
            }
            catch(System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, NLog.LogLevel.Error);
            }
        }

        #endregion

        #region 命令

        /// <summary>
        /// 命令：编辑组ToolBlock
        /// </summary>
        public RelayCommand<object> CmdEditGroupToolBlock { get; set; }

        #endregion

        #region 属性
        /// <summary>
        /// 工作组
        /// </summary>
        private ECWorkStreamsGroup _workGroup;

        public ECWorkStreamsGroup WorkGroup
        {
            get { return _workGroup; }
            set { _workGroup = value;
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
        /// 可绑定的IO信号类型选项列表
        /// </summary>
        private BindingList<string> _IOInputSignalTypeConstantsBindableList;

        public BindingList<string> IOInputSignalTypeConstantsBindableList
        {
            get
            { return _IOInputSignalTypeConstantsBindableList; }
            set
            {
                _IOInputSignalTypeConstantsBindableList = value;
                RaisePropertyChanged();
            }
        }


        #endregion

    }
}
