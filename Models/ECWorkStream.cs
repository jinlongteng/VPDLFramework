using Cognex.VisionPro.ToolBlock;
using Cognex.VisionPro;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViDi2.UI;
using ViDi2.VisionPro;
using ViDi2;
using NLog;
using Cognex.VisionPro.ImageProcessing;
using System.ComponentModel;
using Cognex.VisionPro.Implementation.Internal;
using Cognex.VisionPro.Dimensioning;
using System.Windows.Data;
using System.Threading;

namespace VPDLFramework.Models
{
    public class ECWorkStream : ObservableObject
    {

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="workName">工作项目名称</param>
        /// <param name="streamName">工作流名称</param>
        public ECWorkStream(string workName, string streamName)
        {
            _workName = workName;
            _workStreamName = streamName;
            _streamFolderPath = ECFileConstantsManager.RootFolder + @"\" + _workName + @"\" + ECFileConstantsManager.StreamsFolderName + @"\" + _workStreamName;
            ResultViewModel = new ECWorkStreamOrGroupResult() { StreamOrGroupName = _workStreamName,IsWrokStreamType=true };
            _triggerCount = 0;
            AdvancedDLSteps = new BindingList<ECAdvancedStep>();
            Recipes = new BindingList<ECRecipe>();
            _imageRecord=new ECImageRecorder();
            InitialGraphicTB();
            InitialLiveModeTB();
            StreamMutex = new Mutex(false, streamName);
            _databaseMutex = new Mutex(false, streamName+"_db");
        }

        #region 方法
        /// <summary>
        /// 初始化图形ToolBlock
        /// </summary>
        private void InitialGraphicTB()
        {
            try
            {
                _graphicTB = new CogToolBlock();
                CogImage8Grey image8Grey = new CogImage8Grey();
                Type type = typeof(CogImage8Grey);
                System.Reflection.Assembly assembly = type.Assembly;
                Type[] types = assembly.GetTypes();
                foreach (Type t in types)
                {
                    if (t.Name == "ICogImage")
                    {
                        _graphicTB.Inputs.Add(new CogToolBlockTerminal("InputImage", t));
                        break;
                    }
                }
                CogIPOneImageTool oneImageTool = new CogIPOneImageTool();
                oneImageTool.Name = "CogIPOneImageTool1";
                _graphicTB.Tools.Add(oneImageTool);
            }
            catch (System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, LogLevel.Error);
            }
        }

        /// <summary>
        /// 初始化图形ToolBlock
        /// </summary>
        private void InitialLiveModeTB()
        {
            try
            {
                if(File.Exists(ECFileConstantsManager.StdTB_LiveModePath))
                    _liveModeTB = CogSerializer.LoadObjectFromFile(ECFileConstantsManager.StdTB_LiveModePath) as CogToolBlock;
            }
            catch (System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, LogLevel.Error);
            }
        }

        /// <summary>
        /// 加载工作流配置文件，检查配置是否正常,正常返回True，否则返回False
        /// </summary>
        /// <returns>初始化成功返回True，否则返回False</returns>
        public bool LoadConfig()
        {
            try
            {
                // 加载配置信息
                string jsonPath = _streamFolderPath + @"\" + ECFileConstantsManager.StreamConfigName;
                WorkStreamInfo = ECSerializer.LoadObjectFromJson<ECWorkStreamInfo>(jsonPath);
                if (WorkStreamInfo == null)
                    return false;

                // 加载ToolBlock
                string tbDLInputPath = _streamFolderPath + @"\" + ECFileConstantsManager.DLInputTBName;
                string tbDLOutputPath = _streamFolderPath + @"\" + ECFileConstantsManager.DLOutputTBName;
                if (File.Exists(tbDLInputPath))
                    DLInputTB = CogSerializer.LoadObjectFromFile(tbDLInputPath) as CogToolBlock;
                else
                    return false;
                if (File.Exists(tbDLOutputPath))
                {
                    DLOutputTB = CogSerializer.LoadObjectFromFile(tbDLOutputPath) as CogToolBlock;
                    ToolBlockRecordsKeyList=GetRecordKeyList(DLOutputTB.CreateLastRunRecord());
                }
                else
                    return false;

                // 加载高级DL模式配置
                if (!GetAdvancedDLModelConfig())
                    return false;

                // 加载配方
                if(!GetRecipes())
                    return false;

                // 初始化显示图表
                InitialResultChart();

                // 判断是否是异步模式
                if (WorkStreamInfo.IsAsyncMode)
                {
                    BufferCount= BufferCount == 0 ? 30 : BufferCount;
                    BufferQueue = new ECBufferQueue(BufferCount);
                }

                // 是否显示3D模式
                ResultViewModel.IsDisplay3D = WorkStreamInfo.IsDisplay3D;
                    
                return true;
            }
            catch (System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, LogLevel.Error);
                return false;
            }

        }

        /// <summary>
        /// 初始化结果图表
        /// </summary>
        private void InitialResultChart()
        {
            try
            {
                if (WorkStreamInfo.ResultChartSeriesCount == 0) return;
                ResultChart = new ECWorkStreamOrGroupResultChart(WorkStreamInfo.StreamName, WorkStreamInfo.ResultChartSeriesCount);
            }
            catch (System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace+ex.Message, LogLevel.Error);
            }
        }

        /// <summary>
        /// 获取所有配方
        /// </summary>
        /// <returns></returns>
        private bool GetRecipes()
        {
            try
            {
                string recipesFolder = _streamFolderPath + @"\" + ECFileConstantsManager.RecipesFolderName;
                Recipes = new BindingList<ECRecipe>();
                if (Directory.Exists(recipesFolder))
                {
                    BindingList<ECRecipe> tmp = ECRecipesManager.GetRecipes(recipesFolder);
                    if (tmp != null)
                        Recipes = tmp;
                }
                return true;
            }
            catch(System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, LogLevel.Error);
                return false;
            }
        }

        /// <summary>
        /// 获取高级DL模式配置
        /// </summary>
        /// <returns></returns>
        private bool GetAdvancedDLModelConfig()
        {
            try
            {
                string modelFolder = _streamFolderPath + @"\" + ECFileConstantsManager.AdvancedDLModelFolderName;
                AdvancedDLSteps = new BindingList<ECAdvancedStep>();
                if (Directory.Exists(modelFolder))
                {
                    string[] stepsFolder = Directory.GetDirectories(modelFolder);
                    // 遍历所有步骤
                    foreach (string stepFolder in stepsFolder)
                    {
                        ECAdvancedStep step = new ECAdvancedStep(Path.GetFileName(stepFolder));
                        string[] toolsFolder = Directory.GetDirectories(stepFolder);
                        // 遍历所有工具
                        foreach (string toolFolder in toolsFolder)
                        {
                            string jsonPath = toolFolder + @"\" + ECFileConstantsManager.AdvancedToolConfigName;
                            string tbPath = toolFolder + @"\" + ECFileConstantsManager.AdvancedToolTBName;
                            ECAdvancedToolInfo info = ECSerializer.LoadObjectFromJson<ECAdvancedToolInfo>(jsonPath);
                            ECAdvancedTool tool = new ECAdvancedTool(info.ToolName);
                            tool.ToolInfo = info;
                            tool.ToolBlock = CogSerializer.LoadObjectFromFile(tbPath) as CogToolBlock;
                            step.Tools.Add(tool);
                        }
                        AdvancedDLSteps.Add(step);
                    }
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
        /// 创建新的配置
        /// </summary>
        /// <returns>初始化成功返回True，否则返回False</returns>
        public bool CreateNewConfig()
        {
            try
            {
                // 加载配置信息
                WorkStreamInfo = new ECWorkStreamInfo(_workStreamName);

                // 加载ToolBlock
                if (File.Exists(ECFileConstantsManager.StdTB_DLInputPath))
                    DLInputTB = CogSerializer.LoadObjectFromFile(ECFileConstantsManager.StdTB_DLInputPath) as CogToolBlock;
                else
                    return false;
                if (File.Exists(ECFileConstantsManager.StdTB_DLOutputPath))
                    DLOutputTB = CogSerializer.LoadObjectFromFile(ECFileConstantsManager.StdTB_DLOutputPath) as CogToolBlock;
                else
                    return false;

                return true;
            }
            catch (System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, LogLevel.Error);
                return false;
            }

        }

        /// <summary>
        /// 加载配方
        /// </summary>
        /// <param name="recipeName">配方名称</param>
        /// <returns></returns>
        public bool LoadRecipe(string recipeName)
        {
            try
            {
                if(Recipes!=null)
                {
                    foreach (ECRecipe recipe in Recipes)
                    {
                        if (recipe.RecipeName==recipeName)
                        {
                            var valuesList = recipe.Values.ToDictionary(v => v.Key, v => v.Value);
                            if (!WorkStreamInfo.IsOnlyVpro)
                            {
                                foreach (CogToolBlockTerminal terminal in DLInputTB.Inputs)
                                {
                                    if (valuesList.Keys.Contains(terminal.Name))
                                    {
                                        ECKeyValuePair keyValue= recipe.Values.Where(r => r.Key == terminal.Name)?.First();
                                        Type type=Type.GetType(keyValue.Type);
                                        terminal.Value = Convert.ChangeType(keyValue.Value, type);
                                    }                                       
                                }
                            }
                            else
                            {
                                foreach (CogToolBlockTerminal terminal in DLOutputTB.Inputs)
                                {
                                    if (valuesList.Keys.Contains(terminal.Name))
                                    {
                                        ECKeyValuePair keyValue = recipe.Values.Where(r => r.Key == terminal.Name)?.First();
                                        Type type = Type.GetType(keyValue.Type);
                                        terminal.Value = Convert.ChangeType(keyValue.Value,type);
                                    }
                                }
                            }
                        }
                    }
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
        /// 设置用户数据
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns></returns>
        public bool SetUserData(string data)
        {
            try
            {
                if (data != null)
                {
                    if (!WorkStreamInfo.IsOnlyVpro)
                    {
                        if(DLInputTB.Inputs.Contains("DefaultUserData"))
                        {
                            DLInputTB.Inputs["DefaultUserData"].Value=data;
                        }
                    }
                    else
                    {
                        if (DLOutputTB.Inputs.Contains("DefaultUserData"))
                        {
                            DLOutputTB.Inputs["DefaultUserData"].Value = data;
                        }
                    }
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
        /// 
        /// </summary>
        /// <returns></returns>
        public bool RunLive(ICogImage image)
        {
            try
            {
                if (image == null)
                {
                    ResultViewModel.ResultRecord = null;
                    Completed?.Invoke(this, ResultViewModel);
                    return false;
                }

                _liveModeTB.Inputs["InputImage"].Value = image;
                _liveModeTB.Run();

                // 工具块LastRunRecord
                _liveModeTB.LastRunRecordEnable = CogUserToolLastRunRecordConstants.CompositeSubToolRecords | CogUserToolLastRunRecordConstants.CompositeSubToolSharedGraphics;
                ResultViewModel.ResultRecord = _liveModeTB.CreateLastRunRecord().SubRecords[0];
                Completed?.Invoke(this, ResultViewModel);
                return true;
            }
            catch (System.Exception ex)
            {
                ResultViewModel.ResultRecord = null;
                Completed?.Invoke(this, ResultViewModel);
                ECLog.WriteToLog(ex.StackTrace+ex.Message,LogLevel.Error);
                return false;
            }

        }

        /// <summary>
        /// 异步运行,如果正在处理上一张图片,则新的图片进入缓存队列,按照顺序逐一处理
        /// </summary>
        /// <returns></returns>
        public bool RunAsync(ECWorkImageSourceOutput imageSourceOutput)
        {
            if(!IsRunning)
                Run(imageSourceOutput);
            else  if(BufferQueue!=null&&imageSourceOutput !=null&&imageSourceOutput.Image!=null)
            {
                // 计数加1
                _triggerCount += 1;

                // 触发时间
                _triggerTime = DateTime.Now;
                bool isAdd= BufferQueue.AddImage(imageSourceOutput,_triggerCount,_triggerTime);
                ResultViewModel.BufferedImagesCount=BufferQueue.BufferImages.Count;
                return isAdd;
            }
               
            return true;
        }

        /// <summary>
        /// 多线程模式运行一张图片
        /// </summary>
        public void RunOnMultiThreadMode(ECWorkImageSourceOutput imageSourceOutput)
        {
            // 当启用多线程时
            if (WorkStreamInfo.IsEnableMultiThread)
            {
                // 计数加1
                _triggerCount += 1;
                // 触发时间
                _triggerTime = DateTime.Now;
                MultiThreadManager.ProcessImage(imageSourceOutput, _triggerCount, _triggerTime);
            }
        }

        /// <summary>
        /// 运行处理一张图片
        /// </summary>
        /// <param name="inputImage"></param>
        public void Run(ECWorkImageSourceOutput imageSourceOutput)
        {
            try
            {
                // 当使用单线程处理时
                //if (!IsRunning)
                //{
                //    // 正在运行
                //    IsRunning = true;

                    // 计数加1
                    _triggerCount += 1;

                    // 触发时间
                    _triggerTime = DateTime.Now;

                    // 计时器启动
                    _stopwatch.Reset();
                    _stopwatch.Start();

                    // 结果异常复位
                    ResultViewModel.IsResultError = false;

                    // 触发时间赋值
                    ResultViewModel.TriggerTime = _triggerTime;

                    // 触发计数赋值
                    ResultViewModel.TriggerCount= _triggerCount;

                    // 采集图像
                    _inputImage = imageSourceOutput.Image;
                    if (_inputImage == null)
                    {
                        CreateResultWithError(ECWorkStreamErrorConstants.ImageSource);
                        return;
                    }

                    // 只使用Vpro
                    if (WorkStreamInfo.IsOnlyVpro)
                    {
                        // 运行输出工具块
                        DLOutputTB.Inputs["DefaultOutputImage"].Value = _inputImage;

                        // 图像源其他数据
                        TransferToolBlockTerminalsToToolBlock(imageSourceOutput.OtherOutputs, DLOutputTB);

                        DLOutputTB.Run();

                        CreateResult();
                    }
                    // 使用Vpro+DL
                    else
                    {
                        // 运行输入工具块
                        DLInputTB.Inputs["DefaultInputImage"].Value = _inputImage;

                        // 图像源其他数据
                        TransferToolBlockTerminalsToToolBlock(imageSourceOutput.OtherOutputs, DLInputTB);

                        DLInputTB.Run();

                        // 运行DL
                        if (DLInputTB.Outputs.Contains("DefaultOutputImage") && DLInputTB.Outputs["DefaultOutputImage"].ValueType.Name == "ICogImage")
                        {
                            // 高级DL模式运行
                            if (WorkStreamInfo.IsUseAdvancedDLModel)
                            {
                                TransferDLResultToDLOutputTB(AdvancedDLModelRun());
                                TransferDLInputTBOutputsToDLOutputTBInputs();
                            }
                            // DefaultDL模式运行
                            else
                            {
                                // Default模式下输入工具块的输出参数传给输出工具块的输入参数
                                TransferDLResultToDLOutputTB(DLRun());
                                TransferDLInputTBOutputsToDLOutputTBInputs();
                            }
                            DLOutputTB.Run();
                            // 生成结果
                            CreateResult();
                        }
                    }
                //}
            }
            catch (System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, LogLevel.Error);
                CreateResultWithError(ECWorkStreamErrorConstants.Unknow);
            }
        }

        /// <summary>
        /// 运行处理,没有输入图像
        /// </summary>
        /// <param name="inputImage"></param>
        public void Run()
        {
            try
            {
                // 当使用单线程处理时
                //if (!IsRunning)
                //{
                //    // 正在运行
                //    IsRunning = true;

                    // 计数加1
                    _triggerCount += 1;

                    // 触发时间
                    _triggerTime = DateTime.Now;

                    // 计时器启动
                    _stopwatch.Reset();
                    _stopwatch.Start();

                    // 结果异常复位
                    ResultViewModel.IsResultError = false;

                    // 触发时间赋值
                    ResultViewModel.TriggerTime = _triggerTime;

                    // 触发计数赋值
                    ResultViewModel.TriggerCount = _triggerCount;

                    // 只使用Vpro
                    if (WorkStreamInfo.IsOnlyVpro)
                    {
                        // 运行输出工具块
                        DLOutputTB.Run();
                        CreateResult();
                    }
                    // 使用Vpro+DL
                    else
                    {
                        // 运行输入工具块
                        DLInputTB.Run();

                        // 运行DL
                        if (DLInputTB.Outputs.Contains("DefaultOutputImage") && DLInputTB.Outputs["DefaultOutputImage"].ValueType.Name == "ICogImage")
                        {
                            // 高级DL模式运行
                            if (WorkStreamInfo.IsUseAdvancedDLModel)
                            {
                                if(DLInputTB.Outputs["DefaultOutputImage"].Value!=null)
                                    TransferDLResultToDLOutputTB(AdvancedDLModelRun());
                                TransferDLInputTBOutputsToDLOutputTBInputs();
                            }
                            // DefaultDL模式运行
                            else
                            {
                                // Default模式下输入工具块的输出参数传给输出工具块的输入参数
                                if (DLInputTB.Outputs["DefaultOutputImage"].Value != null)
                                    TransferDLResultToDLOutputTB(DLRun());
                                TransferDLInputTBOutputsToDLOutputTBInputs();
                            }
                            DLOutputTB.Run();
                            // 生成结果
                            CreateResult();
                        }
                    }
                //}
            }
            catch (System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, LogLevel.Error);
                CreateResultWithError(ECWorkStreamErrorConstants.Unknow);
            }
        }

        /// <summary>
        /// 检测缓存队列
        /// </summary>
        private ECWorkImageSourceOutput CheckBufferQueue()
        {
            try
            {
                if(BufferQueue==null) return null;
                ECWorkImageSourceOutput imageSourceOutput= BufferQueue?.GetNextImage();
                ResultViewModel.BufferedImagesCount=BufferQueue.BufferImages.Count;
                return imageSourceOutput;
            }
            catch (System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, LogLevel.Error);
                return null;
            }
        }

        /// <summary>
        /// DefaultDL模式运行,返回DL工具结果
        /// </summary>
        /// <returns></returns>
        public CogToolBlockTerminalCollection DLRun()
        {
            CogToolBlockTerminalCollection result=new CogToolBlockTerminalCollection();
            // 判断图像是否存在
            if ((ICogImage)DLInputTB.Outputs["DefaultOutputImage"].Value == null)
            {
                ECLog.WriteToLog("Output ToolBlock's Outputs [DefaultOutputImage] Is Null",LogLevel.Error);
                CreateResultWithError(ECWorkStreamErrorConstants.DLInputTB);
                return result;
            }

            // ICogImage转IImage
            IImage image = new WpfImage(new ViDi2.VisionPro.Image((ICogImage)DLInputTB.Outputs["DefaultOutputImage"].Value).BitmapSource());

            _DLSample = ECDLEnvironment.Process(WorkStreamInfo.WorkspaceName, WorkStreamInfo.WorkspaceStreamName, WorkStreamInfo.GpuIndex, image);

            // 运行输出工具块
            if (_DLSample == null)
            {
                ECLog.WriteToLog("Deep Learning Stream Result Is Null", LogLevel.Error);
                CreateResultWithError(ECWorkStreamErrorConstants.DL);
                return result;
            }
            result = ECDLEnvironment.GetViDiToolsResult(_DLSample);
            return result;
        }

        /// <summary>
        /// DL高级模式运行,返回最后一个步骤执行的结果
        /// </summary>
        public CogToolBlockTerminalCollection AdvancedDLModelRun()
        {
            CogToolBlockTerminalCollection result= new CogToolBlockTerminalCollection();
            try
            {
                // 执行所有步骤
                foreach (ECAdvancedStep step in AdvancedDLSteps)
                {
                    bool isFirstStep = AdvancedDLSteps.IndexOf(step) == 0;
                    bool isLastStep = AdvancedDLSteps.IndexOf(step) == AdvancedDLSteps.Count - 1;

                    // 第一步骤输入是DLInputTB的输出
                    if (isFirstStep)
                    {
                        step.Inputs = new CogToolBlockTerminalCollection();
                        step.Inputs = DLInputTB.Outputs;
                    }
                    // 其他步骤的输入是上一步骤的输出
                    else
                    {
                        step.Inputs = new CogToolBlockTerminalCollection();
                        step.Inputs = AdvancedDLSteps[AdvancedDLSteps.IndexOf(step) - 1].Outputs;
                    }
                    step.Run();

                    // 最后一步的输出作为整体结果返回
                    if (isLastStep)
                        result=step.Outputs;
                }
            }
            catch(System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace+ex.Message+ "Error Occurred While DLAdvanced Mode Running", NLog.LogLevel.Error);
                CreateResultWithError(ECWorkStreamErrorConstants.DL);
                ECLog.WriteToLog(ex.StackTrace+ex.Message, LogLevel.Error);
            }
            return result;
        }

        /// <summary>
        /// 获取Record键值列表
        /// </summary>
        private BindingList<string> GetRecordKeyList(ICogRecord record)
        {
            BindingList<string> strings = new BindingList<string>();
            if (record != null && record.SubRecords.Count > 0)
            {
                foreach (ICogRecord r in record.SubRecords)
                {
                    strings.Add(record.Annotation+"."+ r.RecordKey);
                }
            }
            return strings;
        }

        /// <summary>
        /// 将ToolBlock节点值集合传递到指定的ToolBlock
        /// </summary>
        private void TransferToolBlockTerminalsToToolBlock(CogToolBlockTerminalCollection terminals, CogToolBlock toolBlock)
        {
            if (terminals == null || terminals.Count == 0 || toolBlock == null) return;
            try
            {
                foreach (CogToolBlockTerminal terminal in toolBlock.Inputs)
                {
                    if (terminals.Contains(terminal.Name) && (terminal.ValueType == toolBlock.Inputs[terminal.Name].ValueType || terminal.ValueType.IsInstanceOfType(toolBlock.Inputs[terminal.Name].Value)))
                    {
                        if (terminal.ValueType.IsValueType)
                            terminal.Value = terminals[terminal.Name].Value;
                        else
                            terminal.Value = ECGeneric.DeepCopy(terminals[terminal.Name].Value);
                    }
                }
            }
            catch (System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, LogLevel.Error);
            }
        }

        /// <summary>
        /// 生成结果
        /// </summary>
        private void CreateResult()
        {
            try
            {
                // 从输出工具块生成结果
                ResultViewModel.StreamOrGroupName = WorkStreamInfo.StreamName;
                if (DLOutputTB.Outputs.Contains("ResultStatus") && DLOutputTB.Outputs["ResultStatus"].ValueType == typeof(Boolean))
                    ResultViewModel.ResultStatus = (bool)DLOutputTB.Outputs["ResultStatus"].Value;
                if (DLOutputTB.Outputs.Contains("ResultForDisplay") && DLOutputTB.Outputs["ResultForDisplay"].ValueType == typeof(string))
                    ResultViewModel.ResultForDisplay = (string)DLOutputTB.Outputs["ResultForDisplay"].Value;
                if (DLOutputTB.Outputs.Contains("ResultForSend") && DLOutputTB.Outputs["ResultForSend"].ValueType == typeof(string))
                    ResultViewModel.ResultForSend = (string)DLOutputTB.Outputs["ResultForSend"].Value;

                // 添加结果到结果图表
                AddResultToResultChart(DLOutputTB);

                // 用户自定义图形
                _graphicList = new List<ICogGraphic>();
                if (DLOutputTB.Outputs.Contains("CustomGraphics") && DLOutputTB.Outputs["CustomGraphics"].ValueType.Name == "List`1"
                    && DLOutputTB.Outputs["CustomGraphics"].Value!=null)
                {
                    foreach (ICogGraphic graphic in (List<ICogGraphic>)DLOutputTB.Outputs["CustomGraphics"].Value)
                        _graphicList.Add(graphic);
                }

                // 获取指定的存图名称
                if (DLOutputTB.Outputs.Contains("SpecifiedImageName") && DLOutputTB.Outputs["SpecifiedImageName"].ValueType == typeof(string))
                    _specifiedImageName = (string)DLOutputTB.Outputs["SpecifiedImageName"].Value;

                // 输出工具块LastRunRecord
                ICogRecord defaultRecord = DLOutputTB.CreateLastRunRecord();

                // 深度学习结果Record
                if (!WorkStreamInfo.IsOnlyVpro && !WorkStreamInfo.IsUseAdvancedDLModel)
                {
                    List<ICogRecord> records = new List<ICogRecord>();
                    foreach (ICogRecord record in ECDLEnvironment.CreateStreamRecord(_DLSample).SubRecords)
                        records.Add(record);
                    defaultRecord.SubRecords.Add(CreateCombinedRecord(records, _inputImage));
                }

                // 最终显示Record
                ResultViewModel.ResultRecord = CreateDisplayRecord(defaultRecord, _graphicList, _inputImage);

                // 显示3D图像
                if (WorkStreamInfo.IsDisplay3D)
                {
                    if (DLOutputTB.Outputs.Contains("Display3DImage") && DLOutputTB.Outputs["Display3DImage"].ValueType == typeof(CogImage16Range))
                        ResultViewModel.RangeImage = (CogImage16Range)DLOutputTB.Outputs["Display3DImage"].Value;
                }

                // ToolBlock显示项列表
                ToolBlockRecordsKeyList=GetRecordKeyList(defaultRecord);

                // 传递输出工具块Custom的输出参数到结果中
                TransferCustomOutputsToResult();

                // 写入结果
                WriteData(_triggerTime,_triggerCount,ResultViewModel);

                // 存图
                SaveImage(_inputImage,ResultViewModel.ResultStatus);
                
                // 结束计时
                _stopwatch.Stop();
                ResultViewModel.ElapsedTime = Math.Round((double)_stopwatch.Milliseconds + (WorkStreamInfo.IsOnlyVpro ? 0 : GetDLDuration()), 3);

                // 发出完成事件
                Completed?.Invoke(this, ResultViewModel);
                
                // Stream状态恢复可用
                IsRunning = false;

                // 检测是否需要执行缓存队列
                if(WorkStreamInfo.IsAsyncMode)
                {
                    ECWorkImageSourceOutput imageSourceOutput = CheckBufferQueue();
                    if(imageSourceOutput != null&&imageSourceOutput.Image!=null)
                    {
                        Run(imageSourceOutput);
                    }
                }
                GC.Collect();
            }
            catch (System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, LogLevel.Error);
            }
        }

        /// <summary>
        /// 从输入的数据生成结果
        /// </summary>
        private void CreateResult(ECWorkStreamMultiThreadResult result)
        {
            try
            {
                _stopwatch.Reset();
                _stopwatch.Start();
                // 从输出工具块生成结果
                ResultViewModel.StreamOrGroupName = WorkStreamInfo.StreamName;
                ResultViewModel.TriggerCount = result.TriggerIndex;
                ResultViewModel.TriggerTime = result.TriggerTime;
                if (result.ToolBlock.Outputs.Contains("ResultStatus") && result.ToolBlock.Outputs["ResultStatus"].ValueType == typeof(Boolean))
                    ResultViewModel.ResultStatus = (bool)result.ToolBlock.Outputs["ResultStatus"].Value;
                if (result.ToolBlock.Outputs.Contains("ResultForDisplay") && result.ToolBlock.Outputs["ResultForDisplay"].ValueType == typeof(string))
                    ResultViewModel.ResultForDisplay = (string)result.ToolBlock.Outputs["ResultForDisplay"].Value;
                if (result.ToolBlock.Outputs.Contains("ResultForSend") && result.ToolBlock.Outputs["ResultForSend"].ValueType == typeof(string))
                    ResultViewModel.ResultForSend = (string)result.ToolBlock.Outputs["ResultForSend"].Value;

                // 添加结果到结果图表
                AddResultToResultChart(result.ToolBlock);

                // 用户自定义图形
                _graphicList = new List<ICogGraphic>();
                if (result.ToolBlock.Outputs.Contains("CustomGraphics") && result.ToolBlock.Outputs["CustomGraphics"].ValueType.Name == "List`1"
                    && result.ToolBlock.Outputs["CustomGraphics"].Value != null)
                {
                    foreach (ICogGraphic graphic in (List<ICogGraphic>)result.ToolBlock.Outputs["CustomGraphics"].Value)
                        _graphicList.Add(graphic);
                }

                // 获取指定的存图名称
                if (result.ToolBlock.Outputs.Contains("SpecifiedImageName") && result.ToolBlock.Outputs["SpecifiedImageName"].ValueType == typeof(string))
                    _specifiedImageName = (string)result.ToolBlock.Outputs["SpecifiedImageName"].Value;

                // 输出工具块LastRunRecord
                //DLOutputTB.LastRunRecordEnable = CogUserToolLastRunRecordConstants.CompositeSubToolRecords| CogUserToolLastRunRecordConstants.CompositeSubToolSharedGraphics;
                ICogRecord defaultRecord = result.ToolBlock.CreateLastRunRecord();

                // 深度学习结果Record
                if (!WorkStreamInfo.IsOnlyVpro && !WorkStreamInfo.IsUseAdvancedDLModel) defaultRecord.SubRecords.Add(ECDLEnvironment.CreateStreamRecord(_DLSample));

                // 最终显示Record
                ResultViewModel.ResultRecord = CreateDisplayRecord(defaultRecord, _graphicList, _inputImage);

                // ToolBlock显示项列表
                ToolBlockRecordsKeyList = GetRecordKeyList(defaultRecord);

                // 传递输出工具块Custom的输出参数到结果中
                TransferCustomOutputsToResult();

                // 写入结果
                WriteData(result.TriggerTime, result.TriggerIndex, ResultViewModel);

                // 存图
                SaveImage(result.ImageSourceOutput.Image,ResultViewModel.ResultStatus);

                // 结束计时
                _stopwatch.Stop();
                ResultViewModel.ElapsedTime = Math.Round((double)_stopwatch.Milliseconds + (result.ToolBlock.RunStatus.TotalTime), 3);

                // 发出完成事件
                Completed?.Invoke(this, ResultViewModel);

                // 检测是否需要执行缓存队列
                if (WorkStreamInfo.IsAsyncMode)
                {
                    ECWorkImageSourceOutput imageSourceOutput = CheckBufferQueue();
                    if (imageSourceOutput != null)
                    {
                        Run(imageSourceOutput);
                    }
                }
                GC.Collect();
            }
            catch (System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, LogLevel.Error);
            }
        }

        /// <summary>
        /// 生成包含错误的结果
        /// </summary>
        private void CreateResultWithError(ECWorkStreamErrorConstants error)
        {
            try
            {
                ResultViewModel.IsResultError = true;
                ECLog.WriteToLog($"\"{_workStreamName}\" {ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.CatchException)} {Enum.GetName(typeof(ECWorkStreamErrorConstants), error)}", NLog.LogLevel.Error);
                //结束计时
                _stopwatch.Stop();
                ResultViewModel.ElapsedTime += Math.Round((double)_stopwatch.Milliseconds, 3);
                ResultViewModel.ResultForDisplay = "ERROR";
                ResultViewModel.ResultForSend = $"{_workStreamName}:ERROR";
                // Stream状态恢复可用
                IsRunning = false;
                //发出完成事件
                Completed?.Invoke(this, ResultViewModel);
            }
            catch(System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace+ex.Message, LogLevel.Error);
            }
        }

        /// <summary>
        /// 传递输入工具块输出到输出工具块
        /// </summary>
        private void TransferDLInputTBOutputsToDLOutputTBInputs()
        {
            try
            {
                foreach (CogToolBlockTerminal terminal in DLOutputTB.Inputs)
                {
                    if (DLInputTB.Outputs.Contains(terminal.Name))
                        DLOutputTB.Inputs[terminal.Name].Value = DLInputTB.Outputs[terminal.Name].Value;

                }
            }
            catch(System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace+ex.Message, LogLevel.Error);
            }
        }

        /// <summary>
        /// 传递DL检测结果到输出工具块
        /// </summary>
        private void TransferDLResultToDLOutputTB(CogToolBlockTerminalCollection collection)
        {
            try
            {
                foreach (CogToolBlockTerminal tbTerminal in DLOutputTB.Inputs)
                {
                    if (collection.Contains(tbTerminal.Name))
                    {
                        tbTerminal.Value = collection[tbTerminal.Name].Value;
                    }
                }
            }
            catch(System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace+ex.Message, LogLevel.Error);
            }
        }

        /// <summary>
        /// 将DL输出工具块中用户自己添加的输出传递给结果
        /// </summary>
        private void TransferCustomOutputsToResult()
        {
            // 传入新值到ToolBlock输入对应参数
            ResultViewModel.CustomOutputs = new CogToolBlockTerminalCollection();
            foreach (CogToolBlockTerminal terminal in DLOutputTB.Outputs)
            {
                if (terminal.Name != "ResultStatus" && terminal.Name != "ResultForDisplay" && terminal.Name != "ResultForSend" && terminal.Name != "SpecifiedImageName"&&
                    terminal.Name != "ResultForSend" && terminal.Name != "CustomGraphics" && terminal.Name != "ResultChartSeries" && terminal.Name != "ResultChartValues" && terminal.Name != "Display3DImage")
                {
                    if (terminal.Value == null)
                        ResultViewModel.CustomOutputs.Add(new CogToolBlockTerminal(terminal.Name, terminal.ValueType));
                    else
                    {
                        if (terminal.ValueType.IsValueType)
                            ResultViewModel.CustomOutputs.Add(new CogToolBlockTerminal(terminal.Name, terminal.Value));
                        else
                            ResultViewModel.CustomOutputs.Add(new CogToolBlockTerminal(terminal.Name, ECGeneric.DeepCopy(terminal.Value)));
                    }
                }
            }
        }

        /// <summary>
        /// 获取输出工具块中用户自己添加的输出
        /// </summary>
        /// <returns></returns>
        public CogToolBlockTerminalCollection GetCustomOutputs()
        {
            // 传入新值到ToolBlock输入对应参数
            CogToolBlockTerminalCollection collection = new CogToolBlockTerminalCollection();
            foreach (CogToolBlockTerminal terminal in DLOutputTB.Outputs)
            {
                if (terminal.Name != "ResultStatus" && terminal.Name != "ResultForDisplay" && terminal.Name != "ResultForSend" && terminal.Name != "SpecifiedImageName" &&
                    terminal.Name != "ResultForSend" && terminal.Name != "CustomGraphics" && terminal.Name != "ResultChartSeries" && terminal.Name != "ResultChartValues"&& terminal.Name != "Display3DImage")
                {
                    if (terminal.Value != null)
                    {
                        if (terminal.ValueType.IsValueType)
                            collection.Add(new CogToolBlockTerminal(terminal.Name, terminal.Value));
                        else
                            collection.Add(new CogToolBlockTerminal(terminal.Name, ECGeneric.DeepCopy(terminal.Value)));
                    }
                    else
                        collection.Add(new CogToolBlockTerminal(terminal.Name, terminal.ValueType));
                }
            }
            return collection;
        }

        /// <summary>
        /// 获取DL处理时间
        /// </summary>
        /// <returns></returns>
        private double GetDLDuration()
        {
            if(_DLSample==null) return 0;
            double duration = 0;
            foreach (var marking in _DLSample.Markings)
            {
                double time = marking.Value.Duration;
                duration += time == -1 ? 0 : time * 1000;
            }
            return duration;
        }

        /// <summary>
        /// 添加结果到结果图表
        /// </summary>
        private void AddResultToResultChart(CogToolBlock toolBlock)
        {
            if(ResultChart==null) return;
            List<string> resultSerires= new List<string>();
            List<double> resultValues= new List<double>();

            // 添加输出工具块中的结果图表系列
            if (toolBlock.Outputs.Contains("ResultChartSeries") && toolBlock.Outputs["ResultChartSeries"].ValueType.Name == "IList")
            {
                if ((List<string>)toolBlock.Outputs["ResultChartSeries"].Value != null)
                {
                    foreach (string str in (List<string>)toolBlock.Outputs["ResultChartSeries"].Value)
                        resultSerires.Add(str);
                }
            }
            // 添加输出工具块中的结果图表数值
            if (toolBlock.Outputs.Contains("ResultChartValues") && toolBlock.Outputs["ResultChartValues"].ValueType.Name == "IList")
            {
                if ((List<double>)toolBlock.Outputs["ResultChartValues"].Value != null)
                {
                    foreach (double d in (List<double>)toolBlock.Outputs["ResultChartValues"].Value)
                        resultValues.Add(d);
                }
            }
            ResultChart.AddData(resultValues, resultSerires);
        }

        /// <summary>
        /// 生成显示Record
        /// </summary>
        /// <param name="lastRunRecords">工具块LastRunRecord</param>
        /// <param name="customGraphics">用户图形集合</param>
        /// <param name="image">输入图像</param>
        /// <returns></returns>
        private ICogRecord CreateDisplayRecord(ICogRecord defaultRecord, List<ICogGraphic> customGraphics, ICogImage image)
        {
            ICogRecord record = null;
            ECWorkOptionManager.ResultGraphiConstants option;

            if (!Enum.TryParse<ECWorkOptionManager.ResultGraphiConstants>(WorkStreamInfo.ResultGraphicOption, out option)) { return record; }
            switch (option)
            {
                case ECWorkOptionManager.ResultGraphiConstants.Default:
                    if(WorkStreamInfo.ToolBlockRecordKey!=null)
                    {
                        foreach(ICogRecord r in defaultRecord.SubRecords)
                        {
                            string key = defaultRecord.Annotation + "." + r.RecordKey;
                            if(key==WorkStreamInfo.ToolBlockRecordKey)
                            {
                                record = r;
                                break;
                            }       
                        }
                    }
                    else if(defaultRecord.SubRecords.Count>0) 
                        record = defaultRecord.SubRecords[0];
                    break;

                case ECWorkOptionManager.ResultGraphiConstants.Custom:
                    record = CreateCustomRecord(customGraphics, image);
                    break;
            }
            return record;
        }

        /// <summary>
        /// Custom图形
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="image"></param>
        /// <returns></returns>
        private ICogRecord CreateCustomRecord(List<ICogGraphic> graphics, ICogImage image)
        {
            if (_graphicTB == null) return null;
            _graphicTB.Inputs["InputImage"].Value = image;
            (_graphicTB.Tools["CogIPOneImageTool1"] as CogIPOneImageTool).InputImage = _graphicTB.Inputs["InputImage"].Value as ICogImage;
            _graphicTB.Run();
            _graphicTB.LastRunRecordEnable = Cognex.VisionPro.Implementation.Internal.CogUserToolLastRunRecordConstants.CompositeSubToolRecords;
            ICogRecord record = _graphicTB.CreateLastRunRecord().SubRecords[0];
            foreach (ICogGraphic graphic in graphics)
            {
                _graphicTB.AddGraphicToRunRecord(graphic, record,
                   "CogIPOneImageTool1.InputImage", "script");
            }
            return record;
        }

        /// <summary>
        /// 合并多个record
        /// </summary>
        /// <param name="records">传入的record列表</param>
        /// <param name="image">传入的图像</param>
        /// <returns></returns>DLInputTB
        private ICogRecord CreateCombinedRecord(List<ICogRecord> records, ICogImage image)
        {
            var containerRecord = new ViDi2.VisionPro.Records.Record(image, "VPDLRecord");
            containerRecord.RecordKey = "VPDLRecord";

            foreach (var cogRecord in records)
            {
                containerRecord.SubRecords.Add(cogRecord);
            }

            return containerRecord;
        }

        /// <summary>
        /// 保存运行结果到数据库
        /// </summary>
        private void WriteData(DateTime triggerTime,int triggerCount,ECWorkStreamOrGroupResult result)
        {
            // 判断是否启用数据库
            if (!IsEnableDatabase) return;

            // 生成Default值
            string data_TriggerTime = triggerTime.ToString("yyyy-MM-dd HH:mm:ss:fff");
            int data_TriggerCount = _triggerCount;
            string data_ResultForDisplay = "";
            string data_ResultForTCP = "";
            int data_ResultStatus = 0;

            // 传入工作流结果数据
            if (result.ResultForDisplay != null)
                data_ResultForDisplay = result.ResultForDisplay;
            if (result.ResultForSend != null)
                data_ResultForTCP = result.ResultForSend;
            data_ResultStatus = result.ResultStatus == true ? 1 : 0;

            // 打包成object[]
            object[] datas = new object[5];
            datas[0] = data_TriggerTime;
            datas[1] = data_TriggerCount;
            datas[2] = data_ResultForDisplay;
            datas[3] = data_ResultForTCP;
            datas[4] = data_ResultStatus;

            // 数据库文件路径
            string folder = ECFileConstantsManager.RootFolder + @"\" + _workName + @"\" + ECFileConstantsManager.DatabaseFolderName + @"\"
                            +ECFileConstantsManager.WorkStreamsDataFolderName+@"\"+ WorkStreamInfo.StreamName+@"\"+DateTime.Now.ToString("yyyy_MM_dd");
            string filePath = folder + @"\" + ECFileConstantsManager.StreamDatabaseFileName;

            // 添加数据到文件
            Task.Run(() =>
            {
                try
                {
                    // 获取数据库锁
                    _databaseMutex.WaitOne();

                    // 添加数据到文件
                    ECSQLiteDataManager.AddData(filePath, ECSQLiteDataManager.DataType.ResultData, datas);
                }
                catch (System.Exception ex)
                {
                    ECLog.WriteToLog(ex.StackTrace + ex.Message+ "Write Data To Result Log Failed", LogLevel.Error);
                }
                finally { _databaseMutex.ReleaseMutex(); }

            });
            
        }

        /// <summary>
        /// 保存图像
        /// </summary>
        private void SaveImage(ICogImage cogImage,bool isOK)
        {
            if(cogImage==null) return;
            // 检查是否启用了存图并且图像有效
            ECWorkOptionManager.ImageRecordConstants recordType;
            if (!Enum.TryParse<ECWorkOptionManager.ImageRecordConstants>(WorkStreamInfo.ImageRecordOption, out recordType) || cogImage == null) return;
            ICogImage image = cogImage.CopyBase(CogImageCopyModeConstants.CopyPixels);
            ICogRecord record = ECGeneric.DeepCopy(ResultViewModel.ResultRecord);

            // 存图

            // 指定图像名称为空时使用时间加触发计数作为名称,否则使用指定的名称
            string imageName = _specifiedImageName == "" || _specifiedImageName == null ? _triggerTime.ToString("yyyy_MM_dd_HH_mm_ss_fff_") + _triggerCount.ToString() : _specifiedImageName;

            // 类型名称,OK/NG分开保存
            string className = isOK ? "OK" : "NG";

            // 原图文件夹
            string originalPath = ECFileConstantsManager.ImageRootFolder + @"\" + _workName + @"\" + ECFileConstantsManager.ImageRecordFolderName + @"\" + _workStreamName + @"\" + ECFileConstantsManager.OriginalImageFolderName + @"\" + DateTime.Now.ToString("yyyy_MM_dd")+$"\\{className}";
            // 图形文件夹
            string graphicPath = ECFileConstantsManager.ImageRootFolder + @"\" + _workName + @"\" + ECFileConstantsManager.ImageRecordFolderName + @"\" + _workStreamName + @"\" + ECFileConstantsManager.GraphicImageFolderName + @"\" + DateTime.Now.ToString("yyyy_MM_dd")+ $"\\{className}";

            // 创建文件夹
            if (!Directory.Exists(originalPath)) Directory.CreateDirectory(originalPath);
            if (!Directory.Exists(graphicPath)) Directory.CreateDirectory(graphicPath);

            // 生成文件名
            string originalFullFileNameWithoutExtension = originalPath + @"\" + imageName;
            string graphicFullFileNameWithoutExtension = graphicPath + @"\" + imageName;

            ECWorkOptionManager.ImageRecordConditionConstants option;

            if (!Enum.TryParse<ECWorkOptionManager.ImageRecordConditionConstants>(WorkStreamInfo.ImageRecordConditionOption, out option)) return;

            // 判断存图条件
            switch (option)
            {
                case ECWorkOptionManager.ImageRecordConditionConstants.All:
                    _imageRecord?.WriteImage(image, originalFullFileNameWithoutExtension, graphicFullFileNameWithoutExtension, _graphicList, _workStreamInfo);
                    break;

                case ECWorkOptionManager.ImageRecordConditionConstants.True:
                    if (ResultViewModel.ResultStatus)
                        _imageRecord?.WriteImage(image, originalFullFileNameWithoutExtension, graphicFullFileNameWithoutExtension, _graphicList, _workStreamInfo);
                    break;

                case ECWorkOptionManager.ImageRecordConditionConstants.False:
                    if (!ResultViewModel.ResultStatus)
                        _imageRecord?.WriteImage(image, originalFullFileNameWithoutExtension, graphicFullFileNameWithoutExtension, _graphicList, _workStreamInfo);
                    break;
            }
        }

        /// <summary>
		/// 刷新输入参数列表
		/// </summary>
		public void UpdateAdvancedModelParaList()
        {
            try
            {
                foreach (ECAdvancedStep step in AdvancedDLSteps)
                {
                    bool isFirstStep = AdvancedDLSteps.IndexOf(step) == 0;
                    bool isLastStep = AdvancedDLSteps.IndexOf(step) == AdvancedDLSteps.Count - 1;

                    step.InputParaList.Clear();
                    step.OutputParaList.Clear();

                    // 输入参数
                    if (isFirstStep)
                    {
                        foreach (CogToolBlockTerminal item in DLInputTB.Outputs)
                        {
                            step.InputParaList.Add(new KeyValuePair<string, Type>(item.Name, item.ValueType));
                        }
                    }
                    else
                    {
                        foreach (KeyValuePair<string, Type> item in AdvancedDLSteps[AdvancedDLSteps.IndexOf(step) - 1].OutputParaList)
                        {
                            step.InputParaList.Add(item);
                        }
                    }

                    // 输出参数
                    foreach (var tool in step.Tools)
                    {
                        if (tool.ToolInfo.IsDLType)
                        {
                            foreach (CogToolBlockTerminal terminal in ECDLEnvironment.GetDLStreamResultTerminals(tool.ToolInfo.DLWorkspaceName, tool.ToolInfo.DLStreamName))
                            {
                                step.OutputParaList.Add(new KeyValuePair<string, Type>(step.StepName+"_"+tool.ToolInfo.ToolName + "_" + terminal.Name, terminal.ValueType));
                                if (isLastStep)
                                {
                                    if (!DLOutputTB.Inputs.Contains(step.StepName + "_" + tool.ToolInfo.ToolName + "_" + terminal.Name))
                                        DLOutputTB.Inputs.Add(new CogToolBlockTerminal(step.StepName + "_" + tool.ToolInfo.ToolName + "_" + terminal.Name,terminal.ValueType));
                                }
                            }
                            foreach(CogToolBlockTerminal terminal in tool.ToolBlock.Outputs)
                            {
                                step.OutputParaList.Add(new KeyValuePair<string, Type>(step.StepName + "_" + tool.ToolInfo.ToolName + "_" + terminal.Name, terminal.ValueType));
                                if (isLastStep)
                                {
                                    if (!DLOutputTB.Inputs.Contains(step.StepName + "_" + tool.ToolInfo.ToolName + "_" + terminal.Name))
                                        DLOutputTB.Inputs.Add(new CogToolBlockTerminal(step.StepName + "_" + tool.ToolInfo.ToolName + "_" + terminal.Name, terminal.ValueType));
                                }
                            }
                        }
                        else
                        {
                            foreach (CogToolBlockTerminal item in tool.ToolBlock.Outputs)
                            {
                                step.OutputParaList.Add(new KeyValuePair<string, Type>(step.StepName + "_"+tool.ToolInfo.ToolName + "_" + item.Name, item.ValueType));
                                if (isLastStep)
                                {
                                    if (!DLOutputTB.Inputs.Contains(step.StepName + "_" + tool.ToolInfo.ToolName + "_" + item.Name))
                                        DLOutputTB.Inputs.Add(new CogToolBlockTerminal(step.StepName + "_"+tool.ToolInfo.ToolName + "_" + item.Name, item.ValueType));
                                }
                            }
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, LogLevel.Error);
            }

        }

        /// <summary>
        /// 初始化多线程
        /// </summary>
        public void InitialMultThread()
        {
            MultiThreadManager = new ECWorkStreamMultiThreadManager(3, this);
            MultiThreadManager.OnNewResultCome += _multiThreadManager_OnNewResultCome;
        }

        /// <summary>
        /// 新的结果达到
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _multiThreadManager_OnNewResultCome(object sender, KeyValuePair<int, ECWorkStreamMultiThreadResult> result)
        {
            CreateResult(result.Value);
            ResultUsedCompleted.Invoke(this, result.Key);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            ResultViewModel.Dispose();
            _graphicTB?.Dispose();
            DLInputTB?.Dispose();
            DLOutputTB?.Dispose();
            foreach(ECAdvancedStep step in AdvancedDLSteps)
                step.Dispose();
            AdvancedDLSteps?.Clear();
            MultiThreadManager?.Dispose();

            ResultViewModel = null;
            _graphicTB=null;
            DLInputTB=null;
            DLOutputTB = null;
        }


        #endregion 方法

        #region 字段
        /// <summary>
        /// 工作项目名称
        /// </summary>
        private string _workName;

        /// <summary>
        /// 工作流名称
        /// </summary>
        private string _workStreamName;

        /// <summary>
        /// 工作流文件夹路径
        /// </summary>
        private string _streamFolderPath;

        /// <summary>
        /// 输入图像
        /// </summary>
        private ICogImage _inputImage;

        /// <summary>
        /// 工作流运行次数
        /// </summary>
        private int _triggerCount;

        /// <summary>
        /// 触发信号时间戳
        /// </summary>
        private DateTime _triggerTime;

        /// <summary>
        /// 显示图形Toolblock
        /// </summary>
        private CogToolBlock _graphicTB;

        /// <summary>
        /// 指定的图像名称
        /// </summary>
        private string _specifiedImageName="";

        /// <summary>
        /// ViDi输入Toolblock
        /// </summary>
        public CogToolBlock DLInputTB { get; set; }

        /// <summary>
        /// ViDi输出Toolblock
        /// </summary>
        public CogToolBlock DLOutputTB { get; set; }

        /// <summary>
        /// 实时显示ToolBlock
        /// </summary>
        private CogToolBlock _liveModeTB {  get; set; }

        /// <summary>
        /// 启用数据库
        /// </summary>
        public bool IsEnableDatabase { get; set; }

        /// <summary>
        /// DL处理结果
        /// </summary>
        private ISample _DLSample;

        /// <summary>
        /// 自定义图形
        /// </summary>
        private List<ICogGraphic> _graphicList;

        /// <summary>
        /// 定时器
        /// </summary>
        private CogStopwatch _stopwatch = new CogStopwatch();

        /// <summary>
        /// 工作流组触发计数,用于确认工作流执行结果和工作流组次序匹配
        /// </summary>
        public int GroupTriggerCount;

        /// <summary>
        /// 工作流正常运行标志符
        /// </summary>
        public bool IsRunning = false;

        /// <summary>
        /// 工作流错误信息类型
        /// </summary>
        public enum ECWorkStreamErrorConstants
        {
            ImageSource,
            DLInputTB,
            DL,
            DLOutputTB,
            Unknow
        }

        /// <summary>
        /// 图像记录器
        /// </summary>
        private ECImageRecorder _imageRecord;

        /// <summary>
        /// 多线程管理器
        /// </summary>
        public ECWorkStreamMultiThreadManager MultiThreadManager;

        /// <summary>
        /// 工作流锁,用于放置触发信号从多个线程进入时,IsRunning状态不唯一
        /// </summary>
        public Mutex StreamMutex;

        /// <summary>
        /// 数据库锁
        /// </summary>
        private Mutex _databaseMutex;

        #endregion 字段

        #region 事件

        public event EventHandler<ECWorkStreamOrGroupResult> Completed;

        public event EventHandler<int> ResultUsedCompleted;

        #endregion


        #region 属性

        /// <summary>
        /// 工作流信息
        /// </summary>
        private ECWorkStreamInfo _workStreamInfo;

        public ECWorkStreamInfo WorkStreamInfo
        {
            get { return _workStreamInfo; }
            set { _workStreamInfo = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 工作流结果ViewModel
        /// </summary>
        private ECWorkStreamOrGroupResult _resultViewModel;

        public ECWorkStreamOrGroupResult ResultViewModel
        {
            get { return _resultViewModel; }
            set { _resultViewModel = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 结果趋势图表
        /// </summary>
        private ECWorkStreamOrGroupResultChart _resultChart;

        public ECWorkStreamOrGroupResultChart ResultChart
        {
            get { return _resultChart; }
            set { _resultChart = value;
                RaisePropertyChanged();
            }
        }


        /// <summary>
        /// 高级DL模式步骤列表
        /// </summary>

        public BindingList<ECAdvancedStep> _advancedDLSteps;

        public BindingList<ECAdvancedStep> AdvancedDLSteps
        {
            get { return _advancedDLSteps; }
            set
            {
                _advancedDLSteps = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 配方列表
        /// </summary>
        private BindingList<ECRecipe> _recipes;

        public BindingList<ECRecipe> Recipes
        {
            get { return _recipes; }
            set { _recipes = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 缓存队列
        /// </summary>
        private ECBufferQueue _bufferQueue;

        public ECBufferQueue BufferQueue
        {
            get { return _bufferQueue; }
            set { _bufferQueue = value;
                RaisePropertyChanged();
            }
        }


        /// <summary>
        /// 缓存数量
        /// </summary>
        private int _bufferCount;

        public int BufferCount
        {
            get { return _bufferCount; }
            set { _bufferCount = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 工具块Records键值列表
        /// </summary>
        private BindingList<string> _ToolBlockRecordsKeyList;

        public BindingList<string> ToolBlockRecordsKeyList
        {
            get { return _ToolBlockRecordsKeyList; }
            set
            {
                _ToolBlockRecordsKeyList = value;
                RaisePropertyChanged();
            }
        }

        #endregion 属性
    }
}
