using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VPDLFramework.Models;

namespace VPDLFramework.ViewModels
{
    public class LogViewModel:ViewModelBase
    {
        public LogViewModel() { 
            RegisterMessenger();
            LogItems=new BindingList<ECLogItem>();
            CmdClear=new RelayCommand(Clear);
        }

        #region 命令
        /// <summary>
        /// 指令:清空日志
        /// </summary>
        public RelayCommand CmdClear { get;set; }

        #endregion

        #region 方法
        /// <summary>
        /// 注册订阅的消息
        /// </summary>
        private void RegisterMessenger()
        {
            Messenger.Default.Register<ECLogItem>(this, ECMessengerManager.ECLogMessengerKeys.LogAdded, OnLogAdded);
        }

        /// <summary>
        /// 添加日志条目
        /// </summary>
        /// <param name="obj"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void OnLogAdded(ECLogItem obj)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                if (obj != null) { 

                    CheckLogItemsCount();

                    // 添加日志显示
                    LogItems.Insert(0,obj);

                    // 如果是错误日志，发出报警信息
                    if (obj.Level == LogLevel.Error)
                        Messenger.Default.Send<string>("", ECMessengerManager.ECLogMessengerKeys.OnAlarm);
                }
            });
            
        }

        /// <summary>
        /// 检查Log信息条目数量，超出10w条则开始删除
        /// </summary>
        public void CheckLogItemsCount()
        {
            if(LogItems?.Count>=100000)
                LogItems.RemoveAt(LogItems.Count-1);
        }

        /// <summary>
        /// 清空日志
        /// </summary>
        private void Clear()
        { 
            LogItems.Clear();
        }
        #endregion

        #region 属性
        /// <summary>
        /// 日志条目集合
        /// </summary>
        private BindingList<ECLogItem> _logItems;

        public BindingList<ECLogItem> LogItems
        {
            get { return _logItems; }
            set { _logItems = value;
                RaisePropertyChanged();
            }
        }
        #endregion
    }
}
