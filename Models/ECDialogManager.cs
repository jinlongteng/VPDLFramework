using Cognex.VisionPro.ToolBlock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using VPDLFramework.Views;

namespace VPDLFramework.Models
{
    public class ECDialogManager
    {
        /// <summary>
        /// 用户交互过程中的子窗口对话框类
        /// </summary>
        /// 
        /// <summary>
        /// 显示一条提示信息
        /// </summary>
        /// <param name="msg">显示的提示信息内容</param>
        public static void ShowMsg(string msg)
        {
            App.Current.Dispatcher.Invoke(new Action(() =>
            {
                MessageBox.Show(msg, ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.Message), MessageBoxButton.OK, MessageBoxImage.Information);
            }));
        }

        /// <summary>
        /// 用户输入对话框，返回用户输入的信息内容
        /// </summary>
        /// <param name="titleName">提示用户输入的信息用途</param>
        /// <returns></returns>
        public static string GetUserInput(string titleName)
        {
            string str = "";
            Window_UserInput window_UserInput = new Window_UserInput(titleName);
            if ((bool)window_UserInput.ShowDialog())
            {
                str = window_UserInput.UserInput;
            }
            return str;
        }

        /// <summary>
        /// 用户确认信息对话框，提示一条信息，确认返回True，取消返回False
        /// </summary>
        /// <param name="verifyMessage">确认的信息内容</param>
        /// <returns></returns>
        public static bool Verify(string verifyMessage)
        {
            MessageBoxResult result = MessageBox.Show(verifyMessage, ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.Verify), MessageBoxButton.YesNo, MessageBoxImage.Question);
            return (bool)(result == MessageBoxResult.Yes ? true : false);
        }

        /// <summary>
        /// 带有加载动画的窗体
        /// </summary>
        private static Window_Waiting window_Loading;

        /// <summary>
        /// 在后台进行操作，前台显示加载动画直到后台操作完成
        /// </summary>
        /// <param name="action">后台执行的操作</param>
        /// <param name="processName">操作的名称</param>
        public static void LoadWithAnimation(Action action, string processName)
        {
            App.Current.Dispatcher.Invoke(new Action(() =>
            {
                window_Loading = new Window_Waiting(processName);
            }));

            Task.Factory.StartNew(action).ContinueWith(new Action<Task>(Task =>
            {
                App.Current.Dispatcher.Invoke(new Action(() =>
                {
                    window_Loading.Close();
                }));
            }));
            App.Current.Dispatcher.Invoke(new Action(() =>
            {
                window_Loading.ShowDialog();
            }));
        }

        /// <summary>
        /// 编辑ToolBlock,确认保存返回<true,新的ToolBlock>,否则返回<false,原始的ToolBlock>
        /// </summary>
        /// <param name="toolBlock"></param>
        /// <returns></returns>
        public static CogToolBlock EditToolBlock(CogToolBlock toolBlock)
        {
            Window_EditToolBlock window_EditToolBlock = new Window_EditToolBlock(toolBlock);
            window_EditToolBlock.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            window_EditToolBlock.ShowDialog();
            if (window_EditToolBlock.TBEdit.Subject != null)
                toolBlock =ECGeneric.DeepCopy<CogToolBlock>(window_EditToolBlock.TBEdit.Subject);

            return toolBlock;
        }
    }
}
