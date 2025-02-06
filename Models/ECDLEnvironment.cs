using Cognex.VisionPro;
using Cognex.VisionPro.Implementation;
using Cognex.VisionPro.ToolBlock;
using GalaSoft.MvvmLight;
using Microsoft.International.Converters.PinYinConverter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using ViDi2;
using ViDi2.VisionPro;

namespace VPDLFramework.Models
{
    public class ECDLEnvironment:ObservableObject
    {
        public static bool IsEnable=false;

        /// <summary>
        /// ViDi Runtime Control,用于加载工作区
        /// </summary>
        public static ViDi2.Runtime.IControl Control{ get; set; }

        /// <summary>
        /// GPU列表
        /// </summary>

        public static BindingList<string> GPUList;

        /// <summary>
        /// 清空工作区
        /// </summary>
        public static void ClearWorkspaces()
        {
            try
            {
                if (ECDLEnvironment.IsEnable)
                    foreach (IWorkspace workspace in Control.Workspaces)
                    {
                        Control.Workspaces.Remove(workspace.DisplayName);
                    }
            }
            catch(System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message,NLog.LogLevel.Error);
            }
        }

        /// <summary>
        /// 处理图片
        /// </summary>
        /// <param name="workspaceName">工作区名称</param>
        /// <param name="streamName">流名称</param>
        /// <param name="image">图片</param>
        /// <param name="gpuIndex">GPU索引</param>
        /// <returns></returns>
        public static ISample Process(string workspaceName,string streamName, int gpuIndex,IImage image)
        {
            ISample sample = null;
            try
            {
                if(!Control.Workspaces.Names.Contains(workspaceName)) return sample;
                if(!Control.Workspaces[workspaceName].Streams.Names.Contains(streamName)) return sample;
                sample=  Control.Workspaces[workspaceName].Streams[streamName].Process(image, "", new List<int> { gpuIndex });
            }
            catch (ViDi2.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message,NLog.LogLevel.Error);
            }

            return sample;
        }

        /// <summary>
        /// 使用ISample生成结果ICogRecord
        /// </summary>
        /// <param name="sample"></param>
        /// <returns></returns>
        public static ICogRecord CreateStreamRecord(ISample sample)
        {
            ICogRecord cogRecord = new CogRecord();
            cogRecord.RecordKey = "VPDLRecord";
            if (sample == null) return cogRecord;
            foreach (ITool tool in sample.Tools)
            {
                if (sample.Markings.ContainsKey(tool.Name))
                {
                    IMarking marking = sample.Markings[tool.Name];
                    switch (marking.ToolType)
                    {
                        case ToolType.Blue:
                            cogRecord.SubRecords.Add(new BlueToolRecord((marking as IBlueMarking), marking.ViewImage(0), tool.Name));
                            break;

                        case ToolType.BlueRead:
                            cogRecord.SubRecords.Add(new BlueToolRecord((marking as IBlueMarking), marking.ViewImage(0), tool.Name));
                            break;

                        case ToolType.Red:
                            cogRecord.SubRecords.Add(new RedToolRecord((marking as IRedMarking), marking.ViewImage(0), tool.Name));
                            break;

                        case ToolType.RedHighDetail:
                            cogRecord.SubRecords.Add(new RedToolRecord((marking as IRedMarking), marking.ViewImage(0), tool.Name));
                            break;

                        case ToolType.Green:
                            cogRecord.SubRecords.Add(new GreenToolRecord((marking as IGreenMarking), marking.ViewImage(0), tool.Name));
                            break;

                        case ToolType.GreenHighDetail:
                            cogRecord.SubRecords.Add(new GreenToolRecord((marking as IGreenMarking), marking.ViewImage(0), tool.Name));
                            break;
                    }
                    
                }
            }
            return cogRecord;
        }

        /// <summary>
        /// 获取DL流产生的结果
        /// </summary>
        /// <param name="workspaceName"></param>
        /// <param name="streamName"></param>
        /// <returns></returns>
        public static CogToolBlockTerminalCollection GetDLStreamResultTerminals(string workspaceName,string streamName)
        {
            CogToolBlockTerminalCollection collection=new CogToolBlockTerminalCollection();
            if (workspaceName == null || streamName == null) return collection;
            IStream stream = ECDLEnvironment.Control.Workspaces[workspaceName].Streams[streamName];
            foreach (ITool tool in stream.Tools.Descendants)
            {
                CogToolBlockTerminal result = new CogToolBlockTerminal("NULL", "");
                string outputToolName = CreateOutputName(tool.Name);
                switch (tool.Type)
                {
                    case ToolType.Blue:
                        result = new CogToolBlockTerminal(outputToolName + "ToolResult", new List<Tuple<string, double, double, double, double, double, double[]>>());
                        break;

                    case ToolType.BlueRead:
                        result = new CogToolBlockTerminal(outputToolName + "ToolResult", new List<Tuple<string, string, double[]>>());
                        break;

                    case ToolType.Red:
                        result = new CogToolBlockTerminal(outputToolName + "ToolResult", new List<Tuple<double, double, double, double, double, double[], PointF[]>>());
                        break;

                    case ToolType.RedHighDetail:
                        result = new CogToolBlockTerminal(outputToolName + "ToolResult", new List<Tuple<double, double, double, double, double, double[], PointF[]>>());
                        break;

                    case ToolType.Green:
                        result = new CogToolBlockTerminal(outputToolName + "ToolResult", new List<Tuple<string, double, double[]>>());
                        break;

                    case ToolType.GreenHighDetail:
                        result = new CogToolBlockTerminal(outputToolName + "ToolResult", new List<Tuple<string, double, double[]>>());
                        break;
                }
                if (result.Name != "NULL")
                    collection.Add(result);
                if (tool.Type == ToolType.Red)
                    collection.Add(new CogToolBlockTerminal(outputToolName + "HeatMapImage", typeof(ICogImage)));
            }
            return collection;
        }

        /// <summary>
        /// 生成输出名称，针对ViDi工具中包含汉字的会转换成拼音
        /// </summary>
        /// <param name="chinese">传入的中文名称</param>
        /// <returns></returns>
        private static string CreateOutputName(string chinese)
        {
            string outputToolName = "";
            try
            {
                string inputToolName = chinese.Replace(" ", "_");

                foreach (char c in inputToolName)
                {
                    if (ChineseChar.IsValidChar(c))
                    {
                        ChineseChar chineseChar = new ChineseChar(c);

                        if (inputToolName.Length < 4)
                        {
                            string name = chineseChar.Pinyins[0].Remove(chineseChar.Pinyins[0].Length - 1, 1).First().ToString().ToUpper()
                                + chineseChar.Pinyins[0].Remove(chineseChar.Pinyins[0].Length - 1, 1).Substring(1);
                            outputToolName += name;
                        }
                        else
                            outputToolName += chineseChar.Pinyins[0][0];
                    }
                    else
                        outputToolName += c.ToString();
                }
            }
            catch (System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, NLog.LogLevel.Error);
            }
            return outputToolName;
        }

        /// <summary>
        /// 处理ViDi工具的结果
        /// </summary>
        /// <returns></returns>
        public static CogToolBlockTerminalCollection GetViDiToolsResult(ISample sample)
        {
            CogToolBlockTerminalCollection toolResults = new CogToolBlockTerminalCollection();
            foreach (ITool tool in sample.Tools.Descendants)
            {
                if (sample.Markings.ContainsKey(tool.Name))
                {
                    IMarking marking = sample.Markings[tool.Name];
                    string outputName = CreateOutputName(tool.Name);
                    CogToolBlockTerminal result = new CogToolBlockTerminal(outputName + "ToolResult", "");
                    switch (marking.ToolType)
                    {
                        case ToolType.Blue:
                            result = new CogToolBlockTerminal(outputName + "ToolResult", ECDLToolResult.GetBlueLocateResult(marking));
                            break;

                        case ToolType.BlueRead:
                            result = new CogToolBlockTerminal(outputName + "ToolResult", ECDLToolResult.GetBlueReadResult(marking));
                            break;

                        case ToolType.Red:
                            result = new CogToolBlockTerminal(outputName + "ToolResult", ECDLToolResult.GetRedResult(marking));
                            break;

                        case ToolType.RedHighDetail:
                            result = new CogToolBlockTerminal(outputName + "ToolResult", ECDLToolResult.GetRedResult(marking));
                            break;

                        case ToolType.Green:
                            result = new CogToolBlockTerminal(outputName + "ToolResult", ECDLToolResult.GetGreenResult(marking));
                            break;

                        case ToolType.GreenHighDetail:
                            result = new CogToolBlockTerminal(outputName + "ToolResult", ECDLToolResult.GetGreenResult(marking));
                            break;
                    }

                    //添加Red工具热图
                    if (marking is IRedMarking)
                    {
                        var redToolRecord = new RedToolRecord((IRedMarking)marking, sample.Image, outputName);

                        if (redToolRecord.HasHeatMap())
                        {
                            IImage heatImage = (marking as IRedMarking).Views.First().HeatMap;
                            ICogImage mCogHeatMapImg = new CogImage24PlanarColor(heatImage.Bitmap);

                            toolResults.Add(new CogToolBlockTerminal(outputName + "HeatMapImage", mCogHeatMapImg));
                        }
                    }
                    toolResults.Add(result);
                }
            }
            return toolResults;
        }
    }
}
