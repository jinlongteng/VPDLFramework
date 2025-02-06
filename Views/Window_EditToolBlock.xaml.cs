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
using Cognex.VisionPro;
using Cognex.VisionPro.ToolBlock;
using VPDLFramework.Models;

namespace VPDLFramework.Views
{
    /// <summary>
    /// Interaction logic for Window_EditToolBlock.xaml
    /// </summary>
    public partial class Window_EditToolBlock : Window
    {
        /// <summary>
        /// ToolBlock编辑控件
        /// </summary>
        public CogToolBlockEditV2 TBEdit = new CogToolBlockEditV2();

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="toolBlock"></param>
        public Window_EditToolBlock(CogToolBlock toolBlock)
        {
            InitializeComponent();
            host.Child = TBEdit;
            TBEdit.Subject = toolBlock;
        }
    }
}
