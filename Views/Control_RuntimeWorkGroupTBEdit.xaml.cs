using Cognex.VisionPro.ToolBlock;
using System;
using System.Windows;
using System.Windows.Controls;
using VPDLFramework.Models;

namespace VPDLFramework.Views
{
    /// <summary>
    /// Interaction logic for Control_RuntimeWorkGroupTBEdit.xaml
    /// </summary>
    public partial class Control_RuntimeWorkGroupTBEdit : UserControl
    {
        public Control_RuntimeWorkGroupTBEdit()
        {
            InitializeComponent();
            tbEdit = new CogToolBlockEditV2();
            tbHost.Child = tbEdit;
        }

        /// <summary>
        /// ToolBlockEdit
        /// </summary>
        private CogToolBlockEditV2 tbEdit;

        /// <summary>
        /// GroupToolBlock依赖属性
        /// </summary>
        public CogToolBlock GroupToolBlock
        {
            get { return (CogToolBlock)GetValue(GroupToolBlockProperty); }
            set { SetValue(GroupToolBlockProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ToolBlock.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GroupToolBlockProperty =
            DependencyProperty.Register("GroupToolBlock", typeof(CogToolBlock), typeof(Control_RuntimeWorkGroupTBEdit), new PropertyMetadata(null, OnToolBlockChanged));

        private static void OnToolBlockChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                (d as Control_RuntimeWorkGroupTBEdit).tbEdit.Subject = e.NewValue as CogToolBlock;
            }
            catch (Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, NLog.LogLevel.Error);
            }
        }
    }
}
