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

namespace VPDLFramework.Views
{
    /// <summary>
    /// Interaction logic for Window_UserInput.xaml
    /// </summary>
    public partial class Window_UserInput : Window
    {
        public Window_UserInput(string titleName)
        {
            InitializeComponent();
            Title = titleName;
        }

        private string userInput;

        public string UserInput
        {
            get { return userInput; }
            set { userInput = value; }
        }

        private void contenTextBlock_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            UserInput = tb.Text;
        }

        private void Border_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void contenTextBlock_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key==Key.Enter)
            {
                DialogResult = true;
            }
        }
    }
}

