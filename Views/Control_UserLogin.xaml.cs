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

namespace VPDLFramework.Views
{
    /// <summary>
    /// Interaction logic for Control_UserLogin.xaml
    /// </summary>
    public partial class Control_UserLogin : UserControl
    {
        public Control_UserLogin()
        {
            InitializeComponent();
        }

        public string InputPwd
        {
            get { return (string)GetValue(InputPwdProperty); }
            set { SetValue(InputPwdProperty, value); }
        }

        // Using a DependencyProperty as the backing store for InputPwd.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InputPwdProperty =
            DependencyProperty.Register("InputPwd", typeof(string), typeof(Control_UserLogin), new PropertyMetadata("", new PropertyChangedCallback(OnValueChanged)));

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //(d as Control_User).pwd.Password = (string)e.NewValue;
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            InputPwd = pwd.Password;
        }
    }
}
