using Cognex.VisionPro.Comm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VPDLFramework.Models
{
    public class ECCommCardFfpNdm
    {
        public ECCommCardFfpNdm(CogNdm ndm) 
        {
            _mNdm = ndm;
            _mNdmUsedAcquisitionIDCollection=new CogNdmUsedAcquisitionIDCollection();
            _mNdmUsedAcquisitionIDCollection.Add(new CogNdmUsedAcquisitionID(0, 0));
            AddEventHandlers();
            _mNdm.Start();
            WriteStatusBit(FfpNdmStatusBitConstants.SystemBusy);
            WriteStatusBit(FfpNdmStatusBitConstants.Offline);
        }

        #region 字段
        /// <summary>
        /// 工业以太网模块
        /// </summary>
        private CogNdm _mNdm;

        /// <summary>
        /// 工厂协议状态位含义常量
        /// </summary>
        public enum FfpNdmStatusBitConstants
        {
            SystemReady,
            SystemBusy,
            Online,
            Offline,
        }

        /// <summary>
        /// 工厂协议控制位含义常量
        /// </summary>
        public enum FfpNdmConrolBitConstants
        {
            BufferResultsEnable,
            JobChangeRequested,
            ClearError,
            SetOffline,
            SoftEventOn,
            SoftEventOff,
            ProtocolStatusChanged,
            SetUserData
        }
        #endregion

        #region 事件
        /// <summary>
        /// 工业协议事件
        /// </summary>
        public event EventHandler<KeyValuePair<FfpNdmConrolBitConstants, string>> FfpEvent;

        /// <summary>
        /// 设置联机事件
        /// </summary>
        public event EventHandler SetOnlineEvent;

        /// <summary>
        /// 设置脱机事件
        /// </summary>
        public event EventHandler SetOfflineEvent;

        /// <summary>
        /// 设置用户数据事件
        /// </summary>
        public event EventHandler<byte[]> OnSetUserDataEvent;

        /// <summary>
        /// 软事件
        /// </summary>
        public event EventHandler<int> OnSoftEvent;

        /// <summary>
        /// 作业切换事件
        /// </summary>
        public event EventHandler<int> OnJobChangeRequestedEvent;

        /// <summary>
        /// 错误清除事件
        /// </summary>
        public event EventHandler OnClearErrorEvent;

        /// <summary>
        /// 采集ID集合
        /// </summary>
        private CogNdmUsedAcquisitionIDCollection _mNdmUsedAcquisitionIDCollection;
        #endregion

        #region 方法

        /// <summary>
        /// 写入检测结果
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="data"></param>
        public void WriteInspectionResult(int offset, byte[] data)
        {
            _mNdm?.NotifyInspectionComplete(0, _mNdmUsedAcquisitionIDCollection, true, 1, data, offset+6);
        }

        /// <summary>
        /// 写入检测结果
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="data"></param>
        public void WriteStatusBit(FfpNdmStatusBitConstants ffpNdmStatusBit)
        {
            switch (ffpNdmStatusBit)
            {
                case FfpNdmStatusBitConstants.SystemReady:
                    _mNdm.NotifySystemStatus(true, false);
                    break;
                case FfpNdmStatusBitConstants.SystemBusy:
                    _mNdm.NotifySystemStatus(false, true);
                    break;
                case FfpNdmStatusBitConstants.Online:
                    _mNdm.NotifyRunning();
                    break;
                case FfpNdmStatusBitConstants.Offline:
                    _mNdm.NotifyStopped(CogNdmStoppedCodeConstants.None);
                    break;
            }
            
        }

        /// <summary>
        /// 确认加载配方
        /// </summary>
        public void NotifyLoadRecipeAck(bool ack)
        {
            byte[] data = new byte[1];
            if (ack)
            {
                data[0] = Convert.ToByte(1);
                _mNdm?.NotifyInspectionComplete(0, _mNdmUsedAcquisitionIDCollection, true, 1, data, 0);
            }
            else
            {
                data[0] = Convert.ToByte(-1);
                _mNdm?.NotifyInspectionComplete(0, _mNdmUsedAcquisitionIDCollection, true, 1, data, 0);
            }
        }

        /// <summary>
        /// 确认设置用户数据
        /// </summary>
        public void NotifySetUserDataAck(bool ack)
        {
            byte[] data=new byte[1];
            if (ack)
            {
                data[0] = Convert.ToByte(1);
                _mNdm?.NotifyInspectionComplete(0, _mNdmUsedAcquisitionIDCollection, true, 1, data, 2);
            }
            else
            {
                data[0] = Convert.ToByte(-1);
                _mNdm?.NotifyInspectionComplete(0, _mNdmUsedAcquisitionIDCollection, true, 1, data, 2);
            }
        }

        /// <summary>
        /// 设置作业加载完成
        /// </summary>
        /// <param name="id">作业ID</param>
        public void SetJobLoadedComplete(int id)
        {
            _mNdm?.NotifyJobState(new int[] { id });
        }

        /// <summary>
        /// 读取用户数据
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="data"></param>
        public byte[] ReadUserData(int offset, int size)
        {
            return _mNdm?.ReadUserData(offset, size);
        }

        /// <summary>
        /// Sign up for NDM the signal events.
        /// These events are raised when the PLC sends signals to the Vision System.
        /// </summary>
        private void AddEventHandlers()
        {
            _mNdm.ClearError += new CogNdmClearErrorEventHandler(mNdm_ClearError);
            _mNdm.JobChangeRequested += new CogNdmJobChangeRequestedEventHandler(mNdm_JobChangeRequested);
            _mNdm.NewUserData += new CogNdmNewUserDataEventHandler(mNdm_NewUserData);
            _mNdm.OfflineRequested += new CogNdmOfflineRequestedEventHandler(mNdm_OfflineRequested);
            _mNdm.ProtocolStatusChanged += new CogNdmProtocolStatusChangedEventHandler(mNdm_ProtocolStatusChanged);
            _mNdm.TriggerSoftEvent += new CogNdmTriggerSoftEventEventHandler(mNdm_TriggerSoftEvent);
            _mNdm.TriggerSoftEventOff += new CogNdmTriggerSoftEventOffEventHandler(mNdm_TriggerSoftEventOff);
        }

        /// <summary>
        /// Unsubscribe from the NDM signal eventss.
        /// </summary>
        public void RemoveEventHandlers()
        {
            _mNdm.ClearError -= new CogNdmClearErrorEventHandler(mNdm_ClearError);
            _mNdm.JobChangeRequested -= new CogNdmJobChangeRequestedEventHandler(mNdm_JobChangeRequested);
            _mNdm.NewUserData -= new CogNdmNewUserDataEventHandler(mNdm_NewUserData);
            _mNdm.OfflineRequested -= new CogNdmOfflineRequestedEventHandler(mNdm_OfflineRequested);
            _mNdm.ProtocolStatusChanged -= new CogNdmProtocolStatusChangedEventHandler(mNdm_ProtocolStatusChanged);
            _mNdm.TriggerSoftEvent -= new CogNdmTriggerSoftEventEventHandler(mNdm_TriggerSoftEvent);
            _mNdm.TriggerSoftEventOff -= new CogNdmTriggerSoftEventOffEventHandler(mNdm_TriggerSoftEventOff);
        }

        #region NDM singal event handlers

        /// <summary>
        /// The NDM raises the TriggerSoftEvent event to inform the vision system 
        /// that the remote device has requested that a soft event execute.
        /// </summary>
        void mNdm_TriggerSoftEvent(object sender, CogNdmTriggerSoftEventEventArgs e)
        {
            KeyValuePair<FfpNdmConrolBitConstants,string> pair=new KeyValuePair<FfpNdmConrolBitConstants, string>(FfpNdmConrolBitConstants.SoftEventOn,e.SoftEventID.ToString());
            FfpEvent?.BeginInvoke(this, pair,null,null);
            OnSoftEvent?.BeginInvoke(this, e.SoftEventID,null,null);
        }

        /// <summary>
        /// The NDM raises the TriggerSoftEventOff event to tell the vision system
        /// that the soft event trigger bit has been reset.
        /// </summary>
        void mNdm_TriggerSoftEventOff(object sender, CogNdmTriggerSoftEventOffEventArgs e)
        {
            KeyValuePair<FfpNdmConrolBitConstants, string> pair = new KeyValuePair<FfpNdmConrolBitConstants, string>(FfpNdmConrolBitConstants.SoftEventOff, e.SoftEventID.ToString());
            FfpEvent?.BeginInvoke(this, pair, null, null);
        }

        /// <summary>
        /// The NDM raises the ProtocolStatusChanged event to inform the vision
        /// system that the status of the PLC protocol connection has changed. 
        /// </summary>
        void mNdm_ProtocolStatusChanged(object sender, CogNdmProtocolStatusChangedEventArgs e)
        {
            KeyValuePair<FfpNdmConrolBitConstants, string> pair = new KeyValuePair<FfpNdmConrolBitConstants, string>(FfpNdmConrolBitConstants.ProtocolStatusChanged, e.ProtocolStatus.ToString());
            FfpEvent?.BeginInvoke(this, pair, null, null);
        }

        /// <summary>
        /// The NDM raises the OfflineRequested event to tell the vision system 
        /// that it should go offline. 
        /// </summary>
        void mNdm_OfflineRequested(object sender, CogNdmOfflineRequestedEventArgs e)
        {
            KeyValuePair<FfpNdmConrolBitConstants, string> pair = new KeyValuePair<FfpNdmConrolBitConstants, string>(FfpNdmConrolBitConstants.SetOffline, e.ReturnToPreviousState.ToString());
            FfpEvent?.BeginInvoke(this, pair, null, null);
            if(e.ReturnToPreviousState)
                SetOfflineEvent?.BeginInvoke(this,null, null,null);
            else
                SetOnlineEvent?.BeginInvoke(this,null,null,null);
        }

        /// <summary>
        /// The NDM raises the NewUserData event to tell the vision system that
        /// new user data has arrived from the remote device.
        /// </summary>
        void mNdm_NewUserData(object sender, CogNdmNewUserDataEventArgs e)
        {
            byte[] data = _mNdm.ReadUserData(1, 1900);
            KeyValuePair<FfpNdmConrolBitConstants, string> pair = new KeyValuePair<FfpNdmConrolBitConstants, string>(FfpNdmConrolBitConstants.SetUserData, data.ToString());
            FfpEvent?.BeginInvoke(this, pair, null, null);
            OnSetUserDataEvent?.BeginInvoke(this,data,null,null);
        }

        /// <summary>
        /// The NDM raises the JobChangeRequested event to inform the vision system
        /// that the remote device has requested a job change.
        /// </summary>
        void mNdm_JobChangeRequested(object sender, CogNdmJobChangeRequestedEventArgs e)
        {
            KeyValuePair<FfpNdmConrolBitConstants, string> pair = new KeyValuePair<FfpNdmConrolBitConstants, string>(FfpNdmConrolBitConstants.JobChangeRequested, e.JobID.ToString());
            FfpEvent?.BeginInvoke(this, pair, null, null);
            OnJobChangeRequestedEvent?.BeginInvoke(this,e.JobID,null,null);
        }

        /// <summary>
        /// The NDM raises the ClearError event to inform the vision system that 
        /// the remote device has been notified of an error reported by the vision
        /// system and the error has been be cleared. 
        /// </summary>
        void mNdm_ClearError(object sender, CogNdmClearErrorEventArgs e)
        {
            KeyValuePair<FfpNdmConrolBitConstants, string> pair = new KeyValuePair<FfpNdmConrolBitConstants, string>(FfpNdmConrolBitConstants.ClearError, "");
            FfpEvent?.BeginInvoke(this, pair, null, null);
            OnClearErrorEvent?.BeginInvoke(this,null,null,null);
        }
        #endregion
        #endregion
    }
}
