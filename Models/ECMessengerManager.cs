using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VPDLFramework.Models
{
    public class ECMessengerManager
    {
        /// <summary>
        /// 系统设置视图消息键值
        /// </summary>
        public enum SystemSetupWindowMessengerKeys
        {
            Show
        }

        /// <summary>
        /// 文件管理视图消息键值
        /// </summary>
        public enum FileManagerWindowMessengerKeys
        {
            Show
        }

        /// <summary>
        /// 主界面视图模型消息键值
        /// </summary>
        public enum MainViewModelMessengerKeys
        {
            InitialCompeleted,
            InitialStepChanged,
            InitialFailed,
            EditWorkChanged,
            LoadWorkChanged,
            WorkLoaded,
            UnLoadWork,
            AskWorkName,
            ReplyWorkName,
            CloseWork,
            SaveWork,
            SystemClosed,
            SystemOnline,
            SystemOffline,
            NativeModeMsg
        }

        /// <summary>
        /// 主界面消息键值
        /// </summary>
        public enum MainWindowMessengerKeys
        {
            ReadyToShow,
            InitialStepChanged,
            InitialFailed,
            SystemClosing,
        }

        /// <summary>
        /// 工作流项视图模型消息键值
        /// </summary>
        public enum WorkStreamItemViewModelMessengerKeys
        {
            SelectedGroupChanged
        }

        /// <summary>
        /// 工作流组项视图模型消息键值
        /// </summary>
        public enum EditWorkGroupViewModelMessengerKeys
        {
            RemoveWorkGroupItem
        }

        /// <summary>
        /// 工作流编辑视图模型消息键值
        /// </summary>
        public enum EditWorkStreamViewModelMessengerKeys
        {
            SelectedGroupChanged,
            RemoveWorkStreamItem,
            RunWorkStreamItem,
            Dispose
        }

        /// <summary>
        /// 展开的工作流胶片信使
        /// </summary>
        public enum ExpandedWorkStreamFilmstripMessengerKeys
        {
            RunSelectedImage
        }

        /// <summary>
        /// 日志消息键值
        /// </summary>
        public enum ECLogMessengerKeys
        {
            LogAdded,
            OnAlarm
        }

        /// <summary>
        /// 本地指令消息键值
        /// </summary>
        public enum ECNativeModeCommandMessengerKeys
        {
            // WorkStream
            TriggerStream, // Trigger Stream
            TriggerMultiStream, // Trigger Multi Stream
            LoadRecipe, // Load Recipe
            SetUserData,
            SetInternalTriggerStatus,
            SetInternalTriggerStatusAck,
            LoadRecipeAck,
            SetUserDataAck,
        }

        /// <summary>
        /// 工作运行时视图模型消息键值
        /// </summary>
        public enum WorkRuntimeViewModelMessengerKeys
        {
            TCPMsgToSystemServer,
            Dispose

        }

        /// <summary>
        /// 通讯板卡视图模型消息键值
        /// </summary>
        public enum CommCardMessengerKeys
        {
            // Control Bit
            FFPJobChangeRequested,
            FFPClearError,
            FFPSetOnline,
            FFPSoftEventOn,
            FFPSoftEventOff,
            FFPProtocolStatusChanged,
            IOTriggerStream,
            FFPTriggerStream,

            // Control Word
            FFPUserData
        }

        /// <summary>
        /// 通讯板卡视图模型消息键值
        /// </summary>
        public enum CommCardViewModelMessengerKeys
        {
            CommTest,
        }

        /// <summary>
        /// 图像记录消息键值
        /// </summary>
        public enum ImageRecordMessagerKeys
        {
            RecordGraphic,
        }

        /// <summary>
        /// 第三方板卡消息键值
        /// </summary>
        public enum ThirdCardMessageKeys
        {
            TCPMessageCome,
            SendTCPMessage,
            ImageSourceCompleted,
        }
    }
}
