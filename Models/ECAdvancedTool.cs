using Cognex.VisionPro;
using Cognex.VisionPro.ToolBlock;
using GalaSoft.MvvmLight;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ViDi2;
using ViDi2.UI;

namespace VPDLFramework.Models
{
    public class ECAdvancedTool:ObservableObject
    {
		public ECAdvancedTool(string toolName) 
		{
			ToolInfo = new ECAdvancedToolInfo(toolName);
			ToolBlock = new CogToolBlock();
			Inputs=new CogToolBlockTerminalCollection();
			Outputs=new CogToolBlockTerminalCollection();
        }

		#region 方法
		/// <summary>
		/// 运行,结果有效返回true,否则返回false
		/// </summary>
		public void Run()
		{
			try
			{
                // 清除结果
				Outputs=new CogToolBlockTerminalCollection();
                
                foreach (CogToolBlockTerminal terminal in ToolBlock.Inputs)
				{
					if (Inputs.Contains(terminal.Name) && 
						(terminal.ValueType == Inputs[terminal.Name].ValueType || terminal.ValueType.IsInstanceOfType(Inputs[terminal.Name].Value)))
					{
						terminal.Value = Inputs[terminal.Name].Value;
                    }
				}

				if (ToolInfo.IsDLType)
				{
					// ToolBlock运行
					ToolBlock.Run();

					// 检查是否使能DL
					if (ToolBlock.Outputs.Contains("DefaultToolEnable") && ToolBlock.Outputs["DefaultToolEnable"].ValueType == typeof(Boolean))
					{
						if (ToolBlock.Outputs["DefaultToolEnable"].Value != null && (bool)ToolBlock.Outputs["DefaultToolEnable"].Value)
						{
							// 检查图像
							if (ToolBlock.Outputs.Contains("DefaultOutputImage"))
							{
								if (ToolBlock.Outputs["DefaultOutputImage"].Value != null)
								{
									// ICogImage转IImage
									IImage image = new WpfImage(new ViDi2.VisionPro.Image((ICogImage)ToolBlock.Outputs["DefaultOutputImage"].Value).BitmapSource());
									ISample sample = ECDLEnvironment.Process(ToolInfo.DLWorkspaceName, ToolInfo.DLStreamName, ToolInfo.GPUIndex, image);
									if (sample != null)
										Outputs = ECDLEnvironment.GetViDiToolsResult(sample);

									foreach (CogToolBlockTerminal terminal in ToolBlock.Inputs)
										Outputs.Add(new CogToolBlockTerminal(terminal.Name, terminal.Value));
								}
							}
						}
						else
						{
							Outputs.Add(new CogToolBlockTerminal("DefaultToolEnable", false));
						}
					}
				}
				// 非DL类型,只允许ToolBlock
				else
				{
					ToolBlock.Run();
					Outputs = ToolBlock.Outputs;
				}
            }
			catch (System.Exception ex)
			{
				if (Outputs.Contains("DefaultToolEnable"))
					Outputs.Remove("DefaultToolEnable");
				Outputs.Add(new CogToolBlockTerminal("DefaultToolEnable", false));
				ECLog.WriteToLog(ex.StackTrace + ex.Message, NLog.LogLevel.Error);
			}

		}


        #endregion

        #region 属性
        public CogToolBlockTerminalCollection Outputs { get; set; }

		public CogToolBlockTerminalCollection Inputs { get; set; }

        /// <summary>
        /// 工具块
        /// </summary>
        private CogToolBlock _toolBlock;

		public CogToolBlock ToolBlock
        {
			get { return _toolBlock; }
			set { _toolBlock = value;
				RaisePropertyChanged();
			}
		}


		/// <summary>
		/// 工具信息
		/// </summary>
		private ECAdvancedToolInfo _toolInfo;

		public ECAdvancedToolInfo ToolInfo
		{
			get { return _toolInfo; }
			set { _toolInfo = value;
				RaisePropertyChanged();
			}
		}
        #endregion

    }
}
