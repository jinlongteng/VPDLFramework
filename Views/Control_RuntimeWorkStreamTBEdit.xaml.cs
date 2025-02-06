using Cognex.VisionPro.ToolBlock;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using VPDLFramework.Models;

namespace VPDLFramework.Views
{
    /// <summary>
    /// Interaction logic for Control_RuntimeTBEdit.xaml
    /// </summary>
    public partial class Control_RuntimeWorkStreamTBEdit : UserControl
    {
        public Control_RuntimeWorkStreamTBEdit()
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
        /// ToolBlock依赖属性
        /// </summary>
        public CogToolBlock ToolBlock
        {
            get { return (CogToolBlock)GetValue(ToolBlockProperty); }
            set { SetValue(ToolBlockProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ToolBlock.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ToolBlockProperty =
            DependencyProperty.Register("ToolBlock", typeof(CogToolBlock), typeof(Control_RuntimeWorkStreamTBEdit), new PropertyMetadata(null,OnToolBlockChanged));

        private static void OnToolBlockChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                (d as Control_RuntimeWorkStreamTBEdit).tbEdit.Subject = e.NewValue as CogToolBlock;
            }
            catch(Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, NLog.LogLevel.Error);
            }
        }
    }
}
