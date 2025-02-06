using GalaSoft.MvvmLight.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using VPDLFramework.ViewModels;
using VPDLFramework.Models;
using GalaSoft.MvvmLight.Messaging;
using System.Windows.Controls.Primitives;
using Cognex.Vision;

namespace VPDLFramework.Views
{
    /// <summary>
    /// Interaction logic for Window_Main.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        /// <summary>
        /// 是否因未检测到授权而退出
        /// </summary>
        private bool _isExistWithoutLicense = false;

        /// <summary>
        /// 日志显示
        /// </summary>
        private bool _isLogShow=true;

        /// <summary>
        /// 关闭窗口等待
        /// </summary>
        private Window_Waiting _closeWin;

        public MainWindow()
        {
            // 注册消息
            RegisterMessenger();
            InitializeComponent();
            logSplitter.IsEnabled = true;
        }

        /// <summary>
        /// 注册消息
        /// </summary>
        private void RegisterMessenger()
        {
            // MainViewModel初始化完成
            Messenger.Default.Register<string>(this, ECMessengerManager.MainViewModelMessengerKeys.InitialCompeleted, OnMainViewModelInitialCompeleted);
            
            // MainViewModel初始化进度更新
            Messenger.Default.Register<string>(this, ECMessengerManager.MainViewModelMessengerKeys.InitialStepChanged, OnMainViewModelInitalStepChanged);

            // MainViewModel初始化失败
            Messenger.Default.Register<string>(this, ECMessengerManager.MainViewModelMessengerKeys.InitialFailed, OnMainViewModelFailed);

            // MainViewModel关闭
            Messenger.Default.Register<string>(this, ECMessengerManager.MainViewModelMessengerKeys.SystemClosed, OnMainViewModelClosed);
        }

        /// <summary>
        /// 主界面视图模型关闭
        /// </summary>
        /// <param name="obj"></param>
        private void OnMainViewModelClosed(string obj)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                ViewModelLocator.Cleanup();
                _closeWin.Close();
            });           
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void OnMainViewModelFailed(string obj)
        {
            _isExistWithoutLicense=true;
            Messenger.Default.Send(obj, ECMessengerManager.MainWindowMessengerKeys.InitialFailed);
            DispatcherHelper.UIDispatcher.Invoke(() =>
            {
                ViewModelLocator.Cleanup();
                Cognex.Vision.Startup.Shutdown();
                this.Close();
            });
        }

        /// <summary>
        /// 主程序视图模型初始化步骤变化
        /// </summary>
        /// <param name="obj"></param>
        private void OnMainViewModelInitalStepChanged(string obj)
        {
            Messenger.Default.Send(obj, ECMessengerManager.MainWindowMessengerKeys.InitialStepChanged);
        }

        /// <summary>
        /// 主程序视图模型初始化完成
        /// </summary>
        /// <param name="obj"></param>
        private void OnMainViewModelInitialCompeleted(string obj)
        {
            Messenger.Default.Send("", ECMessengerManager.MainWindowMessengerKeys.ReadyToShow);
        }

        /// <summary>
        /// 程序关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mainwin_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // 授权未检测到而退出,此时界面未显示,直接退出
            if (_isExistWithoutLicense) return;
            // 界面已经打开时,确认是否关闭
            if (!ECDialogManager.Verify(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.ProgramExit)))
            {
                e.Cancel = true;
                return;
            }

            Task.Factory.StartNew(new System.Action(() =>
            {
                Messenger.Default.Send("", ECMessengerManager.MainWindowMessengerKeys.SystemClosing);
            }));

            _closeWin = new Window_Waiting(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.Closing));
            _closeWin.ShowDialog();
        }

        /// <summary>
        /// 显示日志
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLog_Click(object sender, RoutedEventArgs e)
        {
            if (_isLogShow)
            {
                logRow.Height = new GridLength(0);
                logSplitter.IsEnabled = false;
            }
            else
            {
                logRow.Height = new GridLength(200);
                logSplitter.IsEnabled = true;
                
            }
            _isLogShow = !_isLogShow;
        }
    }

}
