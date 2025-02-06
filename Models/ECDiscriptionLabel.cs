using System;
using System.Linq;
using System.Windows;

namespace VPDLFramework.Models
{
    public class ECDescriptionLabel
    {
        /// <summary>
        /// 查找标签
        /// </summary>
        /// <param name="key">标签键值</param>
        /// <returns></returns>
        public static string FindLabel(LabelConstants key)
        {
            string label = "";
            try
            {
                ResourceDictionary dict = App.Current.Resources.MergedDictionaries.Where(r => r.Source.OriginalString.Contains(@"Languages")).FirstOrDefault();
                string keyName = nameof(LabelConstants) +"."+key.ToString();
                if (dict.Contains(keyName))
                    label = dict[keyName].ToString();
            }
            catch(Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, NLog.LogLevel.Warn);
            }
            return label;
        }

        /// <summary>
        /// 标签键值枚举
        /// </summary>
        public enum LabelConstants
        {
            ProgramStart,
            ProgramStartFinished,
            LoadingSystemStartupSetup,
            SystemStartupSetupLoaded,
            LoadingVisionModule,
            VisionModuleLoaded,
            LoadingWindowsMonitor,
            WindowsMonitorLoaded,
            VPDLVersion,
            VPDLLoadFinished,
            VPDLLoadFailed,
            ProgramExit,
            EditWork,
            CloseEdit,
            LoadWork,
            UnloadWork,
            ConfirmRemove,
            PleaseInputName,
            DuplicateName,
            InvalidName,
            SaveFinished,
            SaveFailed,
            ExportFinished,
            ExportFailed,
            ImportFinished,
            ImportFailed,
            ErrorOccured,
            Loading,
            Closing,
            Creating,
            PleaseWait,
            LoadingImageSourceSetup,
            LoadingVPDLSetup,
            LoadingTCPIPSetup,
            LoadingWorkStreamSetup,
            LoadingWorkGroupSetup,
            LoadingCommCardSetup,
            ImageSource,
            DeepLearning,
            TCPIP,
            WorkStream,
            WorkGroup,
            CommCard,
            NativeModeCommand,
            Trigger,
            ExposureTime,
            UserData,
            Recipe,
            OverrideExist,
            CloseEditingWorkFirstly,
            DeleteFinished,
            DeleteWithError,
            Import,
            CheckFailed,
            Exporting,
            AdminLogin,
            AdminLogout,
            NewPassword,
            InvalidPassword,
            PleaseInputPassword,
            IncorrectUserOrPassword,
            Importing,
            NoCC24CommCard,
            Saving,
            SavingImageSourceWithError,
            SavingDeepLearningWithError,
            SavingTCPIPWithError,
            SavingCommCardWithError,
            SavingWorkGroupWithError,
            SavingWorkStreamWithError,
            LoadFinished,
            LoadFailed,
            TriggerOverflow,
            StartRunning,
            RunComplete,
            CannotFind,
            ImageIsNull,
            ReceivedMessage,
            SendMessage,
            IO,
            IOInput,
            IOOutput,
            Event,
            FFP,
            SendFailed,
            NoValidFile,
            ClearFinished,
            ClearFailed,
            SavingTipOfSystemSetup,
            CheckWorkStreamUpdate,
            DuplicateNameIsNotAllowed,
            FileChooseYesFolderChooseNo,
            PleaseSelect,
            PleaseRemoveLatterStep,
            ResultsOfDLToolAddedToOutputTB,
            ValidType,
            SystemServerStart,
            InvalidVProLicense,
            IncompatibleMode,
            DLEnabled,
            FFPType,
            Message,
            Verify,
            Unloading,
            Clearing,
            Querying,
            RefreshListAfterCloseEdit,
            VerifyOffline,
            VerifyOnline,
            StartupWithOnlineMode,
            Login,
            Logout,
            InvalidLicenseFile,
            ClientConnected,
            ClientDisconnected,
            BufferQueueOverflow,
            OccupancyTooHigh,
            CatchException,
            ImageSourceIsNullOrNoImages,
        }
    }
}
