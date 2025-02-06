using AForge.Math;
using Cognex.VisionPro.ToolBlock;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace VPDLFramework.Models
{
	public class ECAdvancedStep : ObservableObject,IDisposable
	{
		public ECAdvancedStep(string stepName)
		{
			StepName = stepName;
			Tools = new BindingList<ECAdvancedTool>();
			InputParaList=new BindingList<KeyValuePair<string, Type>>();
			OutputParaList=new BindingList<KeyValuePair<string, Type>>();
			Inputs=new CogToolBlockTerminalCollection();
			Outputs =new CogToolBlockTerminalCollection();
		}

		/// <summary>
		/// 运行
		/// </summary>
		/// <returns></returns>
		public bool Run()
		{
			try
			{
				Outputs = new CogToolBlockTerminalCollection();
				foreach (ECAdvancedTool tool in Tools)
				{
					// 传入新值到ToolBlock输入对应参数
					tool.Inputs = new CogToolBlockTerminalCollection();
					foreach (CogToolBlockTerminal terminal in Inputs)
					{
						if (terminal.Value != null)
						{
							if (terminal.ValueType.IsValueType)
								tool.Inputs.Add(new CogToolBlockTerminal(terminal.Name, terminal.Value));
							else
								tool.Inputs.Add(new CogToolBlockTerminal(terminal.Name, ECGeneric.DeepCopy(terminal.Value)));
						}
					}
				}

				var taskList = new List<Task>();
				// 并行执行所有工具
				Parallel.ForEach(Tools, t =>
				{
					t.Run();
                });

				// 合并所有工具结果
				foreach (ECAdvancedTool tool in Tools)
				{
					// 获取工具结果
					foreach (CogToolBlockTerminal terminal in tool.Outputs)
					{
						if (terminal.Value != null)
							Outputs.Add(new CogToolBlockTerminal(StepName + "_" + tool.ToolInfo.ToolName + "_" + terminal.Name, terminal.Value));
					}
				}

			}
			catch (Exception ex)
			{
				ECLog.WriteToLog(ex.StackTrace + ex.Message, NLog.LogLevel.Error);
			}

			return true;
		}

		/// <summary>
		/// 释放资源
		/// </summary>
        public void Dispose()
        {
			Inputs?.Dispose();
			Inputs = null;
			Outputs?.Dispose();
			Outputs = null;
			foreach(ECAdvancedTool tool in Tools)
			{
				tool.Inputs?.Dispose();
				tool.Outputs?.Dispose();
				tool.ToolBlock?.Dispose();

				tool.Inputs = null;
				tool.Outputs = null;
				tool.ToolBlock = null;
			}
			Tools?.Clear();
        }



        #region 属性
        public CogToolBlockTerminalCollection Outputs { get; set; }

		public CogToolBlockTerminalCollection Inputs { get; set; }

        /// <summary>
        /// 步骤名称
        /// </summary>
        private string _stepName;

		public string StepName
		{
			get { return _stepName; }
			set
			{
				_stepName = value;
				RaisePropertyChanged();
			}
		}

        /// <summary>
        /// 工具列表
        /// </summary>
        private BindingList<ECAdvancedTool> _tools;

		public BindingList<ECAdvancedTool> Tools
		{
			get { return _tools; }
			set
			{
				_tools = value;
				RaisePropertyChanged();
			}
		}

		/// <summary>
		/// 参数列表
		/// </summary>
		private BindingList<KeyValuePair<string, Type>> _inputParaList;

		public BindingList<KeyValuePair<string, Type>> InputParaList
        {
			get { return _inputParaList; }
			set
			{
                _inputParaList = value;
				RaisePropertyChanged();
			}
		}

        /// <summary>
        /// 参数列表
        /// </summary>
        private BindingList<KeyValuePair<string, Type>> _outputParaList;

        public BindingList<KeyValuePair<string, Type>> OutputParaList
		{
			get { return _outputParaList; }
			set
			{
				_outputParaList = value;
				RaisePropertyChanged();
			}
		}

        #endregion
    }
}
