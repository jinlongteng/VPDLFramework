using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using VPDLFramework.Models;
using VPDLFramework.ViewModels;
using Cognex.VisionPro;
using Cognex.VisionPro.CogBmpImageWriter;
using Cognex.VisionPro3D;

namespace VPDLFramework.Views
{
    /// <summary>
    /// Interaction logic for Control_ResultViewer.xaml
    /// </summary>
    public partial class Control_WorkEditResultViewer :System.Windows.Controls.UserControl
    {
        public Control_WorkEditResultViewer()
        {
            InitializeComponent();
            _imageWriter = new CogBmpImageWriter();
            host.Child = _display;
            Messenger.Default.Register<string>("", ECMessengerManager.EditWorkStreamViewModelMessengerKeys.Dispose, OnDispose);
            Messenger.Default.Register<string>("", ECMessengerManager.ImageRecordMessagerKeys.RecordGraphic, OnSaveRecordGraphic);
        }

        /// <summary>
        /// 保存显示控件结果图形
        /// </summary>
        /// <param name="obj"></param>
        private void OnSaveRecordGraphic(string obj)
        {
            if(host.Visibility==Visibility.Hidden) return;
            try
            {
                System.Drawing.Image image = null;
                DispatcherHelper.UIDispatcher.Invoke(() =>
                {
                    if ((this.DataContext as WorkStreamItemViewModel).WorkStream.WorkStreamInfo.StreamName == obj.Split(',')[0])
                        image = _display?.CreateContentBitmap(Cognex.VisionPro.Display.CogDisplayContentBitmapConstants.Display);
                });
                if (image != null)
                {
                    Task.Factory.StartNew(() =>
                    {
                        _imageWriter?.WriteBitmap(obj.Split(',')[1] + ".bmp", new System.Drawing.Bitmap(image));
                    });
                }
            }
            catch (Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, NLog.LogLevel.Error);
            }
        }

        /// <summary>
        /// 释放控件资源
        /// </summary>
        /// <param name="obj"></param>
        private void OnDispose(string obj)
        {
            DispatcherHelper.UIDispatcher.Invoke(() =>
            {
                _imageWriter = null;
                _display.Record = null;
                host.Child = null;
                host.Dispose();
                host = null;
                _display.Dispose();
                _display = null;
                display3D.Dispose();
                display3D = null;
                GC.Collect();
            });
            
        }

        /// <summary>
        /// 显示控件
        /// </summary>
        private CogRecordDisplay _display=new CogRecordDisplay();

        /// <summary>
        /// 图片写入器
        /// </summary>
        private CogBmpImageWriter _imageWriter;

        /// <summary>
        /// 绑定的ICogRecord
        /// </summary>
        public ICogRecord DisplayRecord
        {
            get { return (ICogRecord)GetValue(DisplayRecordProperty); }
            set { SetValue(DisplayRecordProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DisplayRecord.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DisplayRecordProperty =
            DependencyProperty.Register("DisplayRecord", typeof(ICogRecord), typeof(Control_WorkEditResultViewer), new PropertyMetadata(null, OnRecordChanged));


        /// <summary>
        /// 刷新Record显示
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnRecordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                if(e.NewValue != null)
                {
                    try
                    {
                        while (true)
                        {
                            if ((d as Control_WorkEditResultViewer)._display.Handle != IntPtr.Zero)
                            {
                                (d as Control_WorkEditResultViewer)._display.Invoke(new Action(() =>
                                {
                                    (d as Control_WorkEditResultViewer)._display.Record = e.NewValue as ICogRecord;
                                    try
                                    {
                                        (d as Control_WorkEditResultViewer)._display.Fit(true);
                                    }
                                    catch { }
                                }));
                                break;
                            }
                            else
                                Thread.Sleep(10);
                        }
                    }
                    catch(Exception ex)
                    {
                        ECLog.WriteToLog(ex.StackTrace+ex.Message,NLog.LogLevel.Error);
                    }
                }
            });
                
        }


        /// <summary>
        /// 显示的3D图像
        /// </summary>
        public CogImage16Range RangeImage
        {
            get { return (CogImage16Range)GetValue(RangeImageProperty); }
            set { SetValue(RangeImageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RangeImage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RangeImageProperty =
            DependencyProperty.Register("RangeImage", typeof(CogImage16Range), typeof(Control_WorkEditResultViewer), new PropertyMetadata(null,OnRangeImageChanged));

        
        /// <summary>
        /// 3D图像刷新
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private static void OnRangeImageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                if (e.NewValue != null)
                {
                    try
                    {
                        // 清空显示
                        (d as Control_WorkEditResultViewer).display3D.Clear();

                        // 刷新显示
                        CogImage16Range rangeImage = (CogImage16Range)e.NewValue;
                        CogImage16Grey greyImage = rangeImage.GetPixelData();
                        Cog3DRangeImageGraphic image =new Cog3DRangeImageGraphic(rangeImage,greyImage);
                        (d as Control_WorkEditResultViewer).display3D.Add(image);
                        (d as Control_WorkEditResultViewer).display3D.FitView();

                    }
                    catch (Exception ex)
                    {
                        ECLog.WriteToLog(ex.StackTrace + ex.Message, NLog.LogLevel.Error);
                    }
                }
            });
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Messenger.Default.Unregister<string>("", ECMessengerManager.EditWorkStreamViewModelMessengerKeys.Dispose, OnDispose);
            Messenger.Default.Unregister<string>("", ECMessengerManager.ImageRecordMessagerKeys.RecordGraphic, OnSaveRecordGraphic);
        }
    }
}
