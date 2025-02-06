using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VPDLFramework.Models
{
    public class ECLog:ObservableObject
    {
        public static NLog.Logger LogManager = NLog.LogManager.GetCurrentClassLogger();

        public static void WriteToLog(string msg,NLog.LogLevel level)
        {
            try
            {
                // 显示到界面日志控件
                Messenger.Default.Send<ECLogItem>(new ECLogItem() {Time=DateTime.Now,Level=level,Message=msg },ECMessengerManager.ECLogMessengerKeys.LogAdded);
                
                // 写入本地文件
                switch(level.Name)
                {
                    case "Trace":
                        LogManager.Trace(msg);
                        break;
                    case "Debug":
                        LogManager.Debug(msg);
                        break;
                    case "Info":
                        LogManager.Info(msg);
                        break;
                    case "Warn":
                        LogManager.Warn(msg);
                        break;
                    case "Error":
                        LogManager.Error(msg);
                        break;
                    case "Fatal":
                        LogManager.Fatal(msg);
                        break;
                }
            }
            catch(Exception ex)
            {
                LogManager.Error(ex.ToString());
            }
        }
    }
}
