using Cognex.Vision;
using Cognex.VisionPro;
using Cognex.VisionPro.CogBmpImageWriter;
using Cognex.VisionPro.Implementation;
using Cognex.VisionPro3D;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using VPDLFramework.Models;
using VPDLFramework.ViewModels;

namespace VPDLFramework.Views
{
    /// <summary>
    /// Interaction logic for Control_RuntimeResultViewer.xaml
    /// </summary>
    public partial class Control_RuntimeResultViewer :System.Windows.Controls.UserControl
    {
        static bool hasRegister = false;
        public Control_RuntimeResultViewer()
        {
            InitializeComponent();
            _display=new CogRecordDisplay();
            host.Child = _display;
            if (!hasRegister)
            {
                Messenger.Default.Register<string>("", ECMessengerManager.WorkRuntimeViewModelMessengerKeys.Dispose, OnDispose);
                Messenger.Default.Register<string>("", ECMessengerManager.ImageRecordMessagerKeys.RecordGraphic, OnSaveRecordGraphic);
                hasRegister = true;
            }
        }

        /// <summary>
        /// 保存显示控件结果图形
        /// </summary>
        /// <param name="obj"></param>
        private void OnSaveRecordGraphic(string obj)
        {
            if (host.Visibility == Visibility.Hidden) return;
            _showImage?.Save(obj.Split(',')[1] + ".png");
        }

        /// <summary>
        /// 释放控件资源
        /// </summary>
        /// <param name="obj"></param>
        private void OnDispose(string obj)
        {
            DispatcherHelper.UIDispatcher.Invoke(() =>
            {

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
        /// 是否最大化
        /// </summary>
        public bool IsMaxiumn = false;

        /// <summary>
        /// 显示控件
        /// </summary>
        private CogRecordDisplay _display;


        /// <summary>
        /// 绑定的ICogRecord
        /// </summary>
        public ICogRecord DisplayRecord
        {
            get { return (ICogRecord)GetValue(DisplayRecordProperty); }
            set { SetValue(DisplayRecordProperty, value); }
        }
        private System.Drawing.Image _showImage;

        // Using a DependencyProperty as the backing store for DisplayRecord.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DisplayRecordProperty =
            DependencyProperty.Register("DisplayRecord", typeof(ICogRecord), typeof(Control_RuntimeResultViewer), new PropertyMetadata(null,OnRecordChanged));

        /// <summary>
        /// 刷新Record显示
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnRecordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

            if (e.NewValue != null)
            {
                try
                {
                    var resultViewer = (Control_RuntimeResultViewer)d;
                    var display= resultViewer._display;
                    display.Record = (ICogRecord)e.NewValue;
                    resultViewer._showImage = display.CreateContentBitmap(Cognex.VisionPro.Display.CogDisplayContentBitmapConstants.Image);
                    display.Fit();
                }
                catch (Exception ex)
                {
                    ECLog.WriteToLog(ex.StackTrace + ex.Message, NLog.LogLevel.Error);
                }
            }

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
            DependencyProperty.Register("RangeImage", typeof(CogImage16Range), typeof(Control_RuntimeResultViewer), new PropertyMetadata(null, OnRangeImageChanged));


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
                        (d as Control_RuntimeResultViewer).display3D.Clear();

                        // 刷新显示
                        CogImage16Range rangeImage = (CogImage16Range)e.NewValue;
                        CogImage16Grey greyImage = rangeImage.GetPixelData();
                        Cog3DRangeImageGraphic image = new Cog3DRangeImageGraphic(rangeImage, greyImage);
                        (d as Control_RuntimeResultViewer).display3D.Add(image);
                        (d as Control_RuntimeResultViewer).display3D.FitView();

                    }
                    catch (Exception ex)
                    {
                        ECLog.WriteToLog(ex.StackTrace + ex.Message, NLog.LogLevel.Error);
                    }
                }
            });
        }

        private void Button_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            e.Handled=true;
        }

        private void Button_PreviewMouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Messenger.Default.Unregister<string>("", ECMessengerManager.WorkRuntimeViewModelMessengerKeys.Dispose, OnDispose);
            Messenger.Default.Unregister<string>("", ECMessengerManager.ImageRecordMessagerKeys.RecordGraphic, OnSaveRecordGraphic);
        }
    }
}
