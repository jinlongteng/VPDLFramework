using Cognex.Vision;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using VPDLFramework.Models;
using VPDLFramework.ViewModels;

namespace VPDLFramework.Views
{
    /// <summary>
    /// Interaction logic for Window_Startup.xaml
    /// </summary>
    public partial class Window_StartupPage : Window
    {
        private MainWindow _mainWindow;

        public Window_StartupPage()
        {
            // 检查VProX授权
            CheckVProXLicense();

            InitializeComponent();
            this.DataContext = this;

            // 初始化DispatcherHelper
            DispatcherHelper.Initialize();

            // 注册订阅消息
            RegisterMessenger();

            // 创建主窗口
            _mainWindow = new MainWindow();
            
        }

        /// <summary>
        /// 注册消息
        /// </summary>
        private void RegisterMessenger()
        {
            // 主窗口初始化完成
            Messenger.Default.Register<string>(this, ECMessengerManager.MainWindowMessengerKeys.ReadyToShow, OnMainWindowReadyToShow);
            
            // 主窗口初始化进度更新
            Messenger.Default.Register<string>(this, ECMessengerManager.MainWindowMessengerKeys.InitialStepChanged, OnMainWindowInitialStep);

            // 主窗口初始化失败
            Messenger.Default.Register<string>(this, ECMessengerManager.MainWindowMessengerKeys.InitialFailed, OnMainWindowInitialFailed);
        }

        /// <summary>
        /// 主窗口初始化失败
        /// </summary>
        /// <param name="obj"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void OnMainWindowInitialFailed(string obj)
        {
            DispatcherHelper.UIDispatcher.Invoke(() =>
            {
                Application.Current.Shutdown();
            });
        }

        /// <summary>
        /// 主窗体加载步骤通知
        /// </summary>
        /// <param name="obj"></param>
        private void OnMainWindowInitialStep(string obj)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                textProgress.Text = obj;
            });
        }

        /// <summary>
        /// 主窗体加载完成
        /// </summary>
        /// <param name="obj"></param>
        private void OnMainWindowReadyToShow(string obj)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                this.Close();
                _mainWindow.Show();
            });
            
        }

        /// <summary>
        /// 检查VProX授权
        /// </summary>
        private void CheckVProXLicense()
        {
            // 使用EL需要初始化
            try
            {
                Startup.Initialize(Startup.ProductKey.VProX);
            }
            catch
            { }
        }
    }
}
