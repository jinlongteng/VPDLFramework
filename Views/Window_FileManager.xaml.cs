using GalaSoft.MvvmLight.Messaging;
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
using VPDLFramework.Models;

namespace VPDLFramework.Views
{
    /// <summary>
    /// Interaction logic for Window_WorkFileManagement.xaml
    /// </summary>
    public partial class Window_FileManager : Window
    {
        public Window_FileManager()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Messenger.Default.Send<string>("", ECMessengerManager.FileManagerWindowMessengerKeys.Show);
        }
    }
}
