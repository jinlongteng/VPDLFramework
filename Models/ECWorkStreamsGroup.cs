using Cognex.VisionPro;
using Cognex.VisionPro.ImageProcessing;
using Cognex.VisionPro.ToolBlock;
using GalaSoft.MvvmLight;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.DwayneNeed.Win32;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static VPDLFramework.Models.ECWorkStream;

namespace VPDLFramework.Models
{
    public class ECWorkStreamsGroup:ObservableObject
    {
        public ECWorkStreamsGroup(string workName,string groupName) 
        {
            _workName = workName;
            _groupName = groupName;
            GroupMutex = new Mutex(false, groupName);
            _databaseMutex = new Mutex(false, groupName+"_db");
            ResultViewModel=new ECWorkStreamOrGroupResult() { StreamOrGroupName = _groupName,IsWrokStreamType=false };
            InitialGraphicTB();
            Inputs=new CogToolBlockTerminalCollection();
        }


        #region 方法
        /// <summary>
        /// 加载工作流配置文件，检查配置是否正常,正常返回True，否则返回False
        /// </summary>
        /// <returns>初始化成功返回True，否则返回False</returns>
        public bool LoadConfig()
        {
            try
            {
                string folder = $"{ECFileConstantsManager.RootFolder}\\{_workName}\\{ECFileConstantsManager.GroupsFolderName}\\{_groupName}";

                // 加载配置信息
                string jsonPath = folder + @"\" + ECFileConstantsManager.GroupConfigName;
                GroupInfo = ECSerializer.LoadObjectFromJson<ECWorkStreamsGroupInfo>(jsonPath);
                if (GroupInfo == null)
                    return false;

                // 加载ToolBlock
                string path = folder + @"\" + ECFileConstantsManager.GroupTBName;

                if (File.Exists(path))
                {
                    ToolBlock = CogSerializer.LoadObjectFromFile(path) as CogToolBlock;
                    ToolBlock.Ran += ToolBlock_Ran;
                    // ToolBlock显示项列表
                    ToolBlockRecordsKeyList = GetRecordKeyList(ToolBlock.CreateLastRunRecord());
                }
                else
                    return false;

                // 初始化结果图表
                InitialResultChart();

                // 初始化工作流结果状态字典
                StreamsResultValid = new Dictionary<string, bool>();
                foreach (string stream in GroupInfo.StreamsList)
                {
                    StreamsResultValid.Add(stream, false);
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
        /// 工具块运行完成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToolBlock_Ran(object sender, EventArgs e)
        {
            // ToolBlock显示项列表
            ToolBlockRecordsKeyList = GetRecordKeyList(ToolBlock.CreateLastRunRecord());
        }

        /// <summary>
        /// 重置工作流结果有效性
        /// </summary>
        public void ResetStreamsResultValid()
        {
            foreach (string item in GroupInfo.StreamsList)
            {
                StreamsResultValid[item]= false;
            }
        }

        /// <summary>
        /// 组是否可运行,当所有工作流结果都可用时,组可运行,可运行返回true,否则返回false
        /// </summary>
        /// <returns></returns>
        public bool CanGroupRun()
        {
            bool canRun=true;
            foreach (var item in StreamsResultValid)
            {
               if(StreamsResultValid[item.Key] == false)
                    canRun=false;
            }
            return canRun;
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
                GroupInfo = new ECWorkStreamsGroupInfo();
                GroupInfo.GroupName = _groupName;
                GroupInfo.ResultGraphicOption =Enum.GetName(typeof(ECWorkOptionManager.ResultGraphiConstants), ECWorkOptionManager.ResultGraphiConstants.Default);
                // 加载ToolBlock
                if (File.Exists(ECFileConstantsManager.StdTB_GroupPath))
                {
                    ToolBlock = CogSerializer.LoadObjectFromFile(ECFileConstantsManager.StdTB_GroupPath) as CogToolBlock;
                    ToolBlock.Ran += ToolBlock_Ran;
                }
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
        /// 初始化结果图表
        /// </summary>
        private void InitialResultChart()
        {
            if (GroupInfo.ResultChartSeriesCount == 0) return;
            ResultChart = new ECWorkStreamOrGroupResultChart(GroupInfo.GroupName, GroupInfo.ResultChartSeriesCount);
        }

        /// <summary>
        /// 保存运行结果到数据库
        /// </summary>
        private void WriteData(DateTime triggerTime, int triggerCount, ECWorkStreamOrGroupResult result)
        {
            if (!IsEnableDatabase) return;

            string data_TriggerTime = triggerTime.ToString("yyyy-MM-dd HH:mm:ss:fff");
            int data_TriggerCount = triggerCount;
            string data_ResultForDisplay = "";
            string data_ResultForTCP = "";
            int data_ResultStatus = 0;

            if (result.ResultForDisplay != null)
                data_ResultForDisplay = result.ResultForDisplay;
            if (result.ResultForSend != null)
                data_ResultForTCP = result.ResultForSend;
            data_ResultStatus = result.ResultStatus == true ? 1 : 0;

            object[] datas = new object[5];
            datas[0] = data_TriggerTime;
            datas[1] = data_TriggerCount;
            datas[2] = data_ResultForDisplay;
            datas[3] = data_ResultForTCP;
            datas[4] = data_ResultStatus;

            string folder = ECFileConstantsManager.RootFolder + @"\" + _workName + @"\" + ECFileConstantsManager.DatabaseFolderName + @"\" + ECFileConstantsManager.WorkStreamsDataFolderName
                + @"\" + GroupInfo.GroupName + @"\" + DateTime.Now.ToString("yyyy_MM_dd");
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
        /// 工作流组运行
        /// </summary>
        public void Run()
        {
            try
            {
                _triggerTime = DateTime.Now;
                ResultViewModel.TriggerTime = _triggerTime;

                // 计数加1
                TriggerCount += 1;
                ResultViewModel.TriggerCount = TriggerCount;

                // 开始计时
                _stopwatch.Reset();
                _stopwatch.Start();

                // 刷新输入参数
                TransferInputsToToolBlock();

                ToolBlock.Run();

                // 从输出工具块生成结果
                ResultViewModel.StreamOrGroupName = GroupInfo.GroupName;
                if (ToolBlock.Outputs.Contains("ResultStatus") && ToolBlock.Outputs["ResultStatus"].ValueType == typeof(Boolean))
                    ResultViewModel.ResultStatus = (bool)ToolBlock.Outputs["ResultStatus"].Value;
                if (ToolBlock.Outputs.Contains("ResultForDisplay") && ToolBlock.Outputs["ResultForDisplay"].ValueType == typeof(string))
                    ResultViewModel.ResultForDisplay = (string)ToolBlock.Outputs["ResultForDisplay"].Value;
                if (ToolBlock.Outputs.Contains("ResultForSend") && ToolBlock.Outputs["ResultForSend"].ValueType == typeof(string))
                    ResultViewModel.ResultForSend = (string)ToolBlock.Outputs["ResultForSend"].Value;

                // 添加结果到结果图表
                AddResultToResultChart();

                // Custom图形
                _graphicList = new List<ICogGraphic>();
                if (ToolBlock.Outputs.Contains("CustomGraphics") && ToolBlock.Outputs["CustomGraphics"].ValueType.Name == "List`1"
                    && ToolBlock.Outputs["CustomGraphics"].Value != null)
                {
                    foreach (ICogGraphic graphic in (List<ICogGraphic>)ToolBlock.Outputs["CustomGraphics"].Value)
                        _graphicList.Add(graphic);
                }

                // 输出工具块LastRunRecord
                //ToolBlock.LastRunRecordEnable = Cognex.VisionPro.Implementation.Internal.CogUserToolLastRunRecordConstants.CompositeSubToolRecords;
                ICogRecord defaultRecord = ToolBlock.CreateLastRunRecord();

                if (ToolBlock.Inputs.Contains("DefaultOutputImage") && typeof(ICogImage).IsInstanceOfType(ToolBlock.Inputs["DefaultOutputImage"].Value))
                    _inputImage = (ICogImage)ToolBlock.Inputs["DefaultOutputImage"].Value;
                // 最终显示Record
                if (_inputImage != null)
                    ResultViewModel.ResultRecord = CreateDisplayRecord(defaultRecord, _graphicList, _inputImage);

                //写入结果
                WriteData(_triggerTime, TriggerCount, ResultViewModel);

                //结束计时
                _stopwatch.Stop();
                ResultViewModel.ElapsedTime = Math.Round((double)_stopwatch.Milliseconds, 3);

                // 等待状态可用
                IsWaiting = false;

                //发出完成事件
                Compeleted.Invoke(this, ResultViewModel);

                // 重置工作流结果状态
                ResetStreamsResultValid();

                GC.Collect();
            }
            catch (Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, LogLevel.Error);
                CreateResultWithError();
            }
        }

        /// <summary>
        /// 运行产生异常时,发出包含错误信息的结果
        /// </summary>
        private void CreateResultWithError()
        {
            ECLog.WriteToLog($"Error Occured While \"{_groupName}\" Running", NLog.LogLevel.Error);
            _stopwatch.Stop();
            ResultViewModel.ElapsedTime = Math.Round((double)_stopwatch.Milliseconds, 3);

            ResultViewModel.ResultForDisplay = "ERROR";
            ResultViewModel.ResultForSend = $"{_groupName}:ERROR";

            //写入结果
            WriteData(_triggerTime, TriggerCount, ResultViewModel);


            //完成计数加1
            TriggerCount += 1;

            // 等待状态可用
            IsWaiting = false;

            //发出完成事件
            Compeleted.Invoke(this, ResultViewModel);

            // 重置工作流结果状态
            ResetStreamsResultValid();

            GC.Collect();
        }

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

            if (!Enum.TryParse<ECWorkOptionManager.ResultGraphiConstants>(GroupInfo.ResultGraphicOption, out option)) { return record; }
            switch (option)
            {
                case ECWorkOptionManager.ResultGraphiConstants.Default:
                    if (GroupInfo.ToolBlockRecordKey != null)
                    {
                        foreach (ICogRecord r in defaultRecord.SubRecords)
                        {
                            string key = defaultRecord.Annotation + "." + r.RecordKey;
                            if (key == GroupInfo.ToolBlockRecordKey)
                            {
                                record = r;
                                break;
                            }
                        }
                    }
                    else if(defaultRecord.SubRecords.Count > 0)
                        record = defaultRecord.SubRecords[0];
                    break;

                case ECWorkOptionManager.ResultGraphiConstants.Custom:
                    record = CreateCustomRecord(customGraphics, image);
                    break;
            }
            return record;
        }

        /// <summary>
        /// 将工作流返回的结果集合赋值给ToolBlock输入
        /// </summary>
        private void TransferInputsToToolBlock()
        {
            foreach (CogToolBlockTerminal terminal in ToolBlock.Inputs)
            {
                if(Inputs.Contains(terminal.Name))
                {
                    if (Inputs[terminal.Name].Value==null)
                        terminal.Value = null;
                    else if ((terminal.ValueType == Inputs[terminal.Name].ValueType || terminal.ValueType.IsInstanceOfType(Inputs[terminal.Name].Value)))
                    {
                        terminal.Value = Inputs[terminal.Name].Value;
                    }
                }
            }
        }

        /// <summary>
        /// 添加结果到结果图表
        /// </summary>
        private void AddResultToResultChart()
        {
            if (ResultChart == null) return;
            List<string> resultSerires = new List<string>();
            List<double> resultValues = new List<double>();

            // 添加输出工具块中的结果图表系列
            if (ToolBlock.Outputs.Contains("ResultChartSeries") && ToolBlock.Outputs["ResultChartSeries"].ValueType.Name == "IList")
            {
                if ((List<string>)ToolBlock.Outputs["ResultChartSeries"].Value != null)
                {
                    foreach (string str in (List<string>)ToolBlock.Outputs["ResultChartSeries"].Value)
                        resultSerires.Add(str);
                }
            }
            // 添加输出工具块中的结果图表数值
            if (ToolBlock.Outputs.Contains("ResultChartValues") && ToolBlock.Outputs["ResultChartValues"].ValueType.Name == "IList")
            {
                if ((List<double>)ToolBlock.Outputs["ResultChartValues"].Value != null)
                {
                    foreach (double d in (List<double>)ToolBlock.Outputs["ResultChartValues"].Value)
                        resultValues.Add(d);
                }
            }
            ResultChart.AddData(resultValues, resultSerires);
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
                    strings.Add(record.Annotation + "." + r.RecordKey);
                }
            }
            return strings;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            Inputs?.Dispose();
            Inputs = null;
            ToolBlock?.Dispose();
            ToolBlock = null;
        }
        #endregion

        #region 字段

        /// <summary>
        /// 工作名
        /// </summary>
        private string _workName;

        /// <summary>
        /// 组名
        /// </summary>
        private string _groupName;

        /// <summary>
        /// 输入图像
        /// </summary>
        private ICogImage _inputImage;

        /// <summary>
        /// 工作流运行次数
        /// </summary>
        public int TriggerCount=0;

        /// <summary>
        /// 触发信号时间戳
        /// </summary>
        private DateTime _triggerTime;

        /// <summary>
        /// 显示图形Toolblock
        /// </summary>
        private CogToolBlock _graphicTB;

        /// <summary>
        /// 自定义图形
        /// </summary>
        private List<ICogGraphic> _graphicList;

        /// <summary>
        /// 正在运行
        /// </summary>
        public bool IsRunning=false;

        /// <summary>
        /// 定时器
        /// </summary>
        private CogStopwatch _stopwatch = new CogStopwatch();

        /// <summary>
        /// 工作流组锁,用于多工作流并行访问组属性时,限定一个时间只能有一个访问
        /// </summary>
        public Mutex GroupMutex;

        /// <summary>
        /// 数据库锁
        /// </summary>
        private Mutex _databaseMutex;

        #endregion

        #region 事件
        /// <summary>
        /// 组运行完成事件
        /// </summary>
        public event EventHandler<ECWorkStreamOrGroupResult> Compeleted;

        #endregion

        #region 属性
        /// <summary>
        /// 组ToolBlock
        /// </summary>
        public CogToolBlock ToolBlock { get; set; }

        /// <summary>
        /// 启用数据库
        /// </summary>
        public bool IsEnableDatabase { get; set; }

        /// <summary>
        /// 组信息
        /// </summary>
        private ECWorkStreamsGroupInfo _groupInfo;

        public ECWorkStreamsGroupInfo GroupInfo
        {
            get { return _groupInfo; }
            set
            {
                _groupInfo = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 结果
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
            set
            {
                _resultChart = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 输入参数集合,保存每一个工作流输出的参数,所有工作流返回结果后统一传递给组ToolBlock输入
        /// </summary>
        public CogToolBlockTerminalCollection Inputs { get; set; }

        /// <summary>
        /// 正在等到工作流结果
        /// </summary>
        private bool _isWaiting;

        public bool IsWaiting
        {
            get { return _isWaiting; }
            set { _isWaiting = value;
                RaisePropertyChanged();
            }
        }


        /// <summary>
        /// 工作流结果可用状态字典
        /// </summary>
        private Dictionary<string,bool> _streamsResultValid;

        public Dictionary<string,bool> StreamsResultValid
        {
            get { return _streamsResultValid; }
            set { _streamsResultValid = value;
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

        #endregion
    }
}
