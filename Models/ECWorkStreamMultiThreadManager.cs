using Cognex.VisionPro;
using Cognex.VisionPro.ToolBlock;
using Microsoft.CodeAnalysis.Scripting.Hosting;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NLog;

namespace VPDLFramework.Models
{
    public class ECWorkStreamMultiThreadManager
    {
        public ECWorkStreamMultiThreadManager(int threadCount, ECWorkStream workStream)
        {
            ThreadCount = threadCount;
            WorkStream = workStream;
            workStream.ResultUsedCompleted += WorkStream_ResultUsedCompleted;
            InitailManager();
        }

        /// <summary>
        /// 结果使用完成,重置状态为可用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WorkStream_ResultUsedCompleted(object sender, int e)
        {
            if(_TBStatusCollection !=null&&_TBStatusCollection.Count>e)
            {
                _TBStatusCollection[e] = true;
            }
        }

        /// <summary>
        /// 初始化管理器,按照所设置的线程数量拷贝需要的ToolBlock及其他对象
        /// </summary>
        private void InitailManager()
        {
            BufferQueue = new ECBufferQueue(WorkStream.BufferCount);
            _TBCollection = new Dictionary<int, CogToolBlock>();
            _TBStatusCollection = new Dictionary<int, bool>();
            for (int i = 0; i < ThreadCount; i++)
            {
                _TBCollection.Add(i, CogSerializer.DeepCopyObject(WorkStream.DLOutputTB) as CogToolBlock);
                _TBStatusCollection[i] = true;
            }
            _monitorThread = new Thread(MonitorBuffer);
        }

        /// <summary>
        /// 线程数量
        /// </summary>
        public int ThreadCount;

        /// <summary>
        /// 对应的工作流
        /// </summary>
        public ECWorkStream WorkStream;

        /// <summary>
        /// ToolBlock集合
        /// </summary>
        private Dictionary<int,CogToolBlock> _TBCollection;

        /// <summary>
        /// ToolBlock状态集合
        /// </summary>
        private Dictionary<int,bool> _TBStatusCollection;

        /// <summary>
        /// 缓存队列
        /// </summary>
        public ECBufferQueue BufferQueue;

        /// <summary>
        /// 监视线程
        /// </summary>
        private Thread _monitorThread;

        /// <summary>
        /// 是否监视
        /// </summary>
        private bool _isMonitorBuffer = true;

        /// <summary>
        /// 新结果产生事件
        /// </summary>
        public event EventHandler<KeyValuePair<int,ECWorkStreamMultiThreadResult>> OnNewResultCome;

        /// <summary>
        /// 图像开始被处理,参数1表示图片触发序号,参数2表示处理的线程序号
        /// </summary>
        public event EventHandler<KeyValuePair<int, int>> ImageProcessStart;

        /// <summary>
        /// 处理一张图像,按照ToolBlock空闲的状态分配不同的线程
        /// </summary>
        /// <param name="cogImage"></param>
        public void ProcessImage(ECWorkImageSourceOutput imageSourceOutput,int triggerIndex,DateTime triggerTime)
        {
            bool bProceessed=false;
            int id=GetValidTBID();
            if (id >= 0)
            {
                bProceessed = true;
                _TBStatusCollection[id] = false;
                ImageProcessStart.Invoke(this,new KeyValuePair<int, int>(triggerIndex, id));
                ECWorkStreamMultiThreadResult info=new ECWorkStreamMultiThreadResult();
                info.TriggerIndex = triggerIndex;
                info.TriggerTime = triggerTime;
                info.ImageSourceOutput = imageSourceOutput;
                RunTB(info, id);

            }

            if (!bProceessed)
                BufferQueue.AddImage(imageSourceOutput, triggerIndex, triggerTime);
        }

        /// <summary>
        /// 运行ToolBlock
        /// </summary>
        /// <param name="toolBlock"></param>
        /// <param name="image"></param>
        /// <param name="id"></param>
        private void RunTB(ECWorkStreamMultiThreadResult result, int id)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    if (_TBCollection[id].Inputs.Contains("DefaultOutputImage"))
                    {
                        _TBCollection[id].Inputs["DefaultOutputImage"].Value = result.ImageSourceOutput.Image;
                        TransferToolBlockTerminalsToToolBlock(result.ImageSourceOutput.OtherOutputs, _TBCollection[id]);
                        _TBCollection[id].Run();
                        result.ToolBlock = _TBCollection[id];
                        OnNewResultCome.Invoke(this, new KeyValuePair<int, ECWorkStreamMultiThreadResult>(id, result));
                    }
                }
                catch (Exception ex)
                {
                    ECLog.WriteToLog(ex.StackTrace + ex.Message, NLog.LogLevel.Error);
                }
            });
        }

        /// <summary>
        /// 将ToolBlock节点值集合传递到指定的ToolBlock
        /// </summary>
        private void TransferToolBlockTerminalsToToolBlock(CogToolBlockTerminalCollection terminals, CogToolBlock toolBlock)
        {
            if (terminals == null || terminals.Count == 0||toolBlock==null) return;
            try
            {
                foreach (CogToolBlockTerminal terminal in toolBlock.Inputs)
                {
                    if (terminals.Contains(terminal.Name) && terminals[terminal.Name].ValueType == terminal.ValueType)
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
        /// 获取最靠前的可用ToolBlock ID
        /// </summary>
        /// <returns></returns>
        private int GetValidTBID()
        {
            int id = -1;
            foreach(var tbKeyPair in _TBStatusCollection)
            {
                if (tbKeyPair.Value)
                {
                    id = tbKeyPair.Key;
                    break;
                }
            }
            return id;
        }

        /// <summary>
        /// 监视Buffer,如果Buffer中包含缓存的图像,并且有空闲的ToolBlock则进行处理
        /// </summary>
        private void MonitorBuffer()
        {
            while(_isMonitorBuffer)
            {
                if(BufferQueue.BufferImages.Count > 0)
                {
                    int id=GetValidTBID();
                    if (id >= 0)
                        ProcessImage(BufferQueue.GetNextImage(),BufferQueue.GetNextImageTriggerIndex(),BufferQueue.GetNextImageTriggerTime());
                }
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            _isMonitorBuffer = false;
            foreach(var kv in _TBCollection)
            {
                kv.Value.Dispose();
                _TBCollection[kv.Key]=null;
            }
            _TBCollection?.Clear();
            
            _TBStatusCollection?.Clear();
        }
    }
}
