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
using GalaSoft.MvvmLight;
using VPDLFramework.ViewModels;

namespace VPDLFramework.Views
{
    /// <summary>
    /// Interaction logic for Window_ExpandWorkStream.xaml
    /// </summary>
    public partial class Window_ExpandWorkStream : Window
    {

        public Window_ExpandWorkStream()
        {
            InitializeComponent();
            tbInputHost.Child = tbInputEdit;
            tbOutputHost.Child = tbOutputEdit;
        }

        /// <summary>
        /// 输入ToolBlock编辑控件
        /// </summary>
        private CogToolBlockEditV2 tbInputEdit=new CogToolBlockEditV2();

        /// <summary>
        /// 输出ToolBlock编辑控件
        /// </summary>
        private CogToolBlockEditV2 tbOutputEdit=new CogToolBlockEditV2();

        /// <summary>
        /// 输入ToolBlock
        /// </summary>
        private CogToolBlock _inputToolBlock;

        public CogToolBlock InputToolBlock
        {
            get { return _inputToolBlock; }
            set { _inputToolBlock = value;
                tbInputEdit.Subject = _inputToolBlock;
            }
        }


        /// <summary>
        /// 输出ToolBlock
        /// </summary>
        private CogToolBlock _outputToolBlock;

        public CogToolBlock OutputToolBlock
        {
            get { return _outputToolBlock; }
            set { _outputToolBlock = value;
                tbOutputEdit.Subject = _outputToolBlock;
            }
        }

        /// <summary>
        /// 胶片视图模型
        /// </summary>
        public WorkImageSourceItemViewModel FilmstripViewModel;

    }
}
