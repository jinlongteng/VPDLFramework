using Cognex.VisionPro;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using VPDLFramework.ViewModels;

namespace VPDLFramework.Models
{
    /// <summary>
    /// bool转登录字符串,0:登出,1:登入
    /// </summary>
    public class BoolToLogin : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
                return ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.Logout);
            else
                return ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.Login);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// bool反转
    /// </summary>
    public class BoolInverse : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
                return false;
            else
                return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 数字转颜色,1：#FF17A355,否则#FFFA1313
    /// </summary>
    public class NumToColorGreenRed : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (System.Convert.ToInt32(value) == 1)
                return "#FF17A355";
            else
                return "#FFFA1313";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 连接两个字符串
    /// </summary>
    public class ConcatStringConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string str = "";
            foreach(object s in values)
            {
                str += s?.ToString()+":";
            }    
            str= str.Remove(str.Length - 1);
            return str;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// bool转可见性,0:Visible,1:Hidden
    /// </summary>
    public class BoolToVisiblityDefaultHidden : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(bool)value)
                return "Visible";
            else
                return "Hidden";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// bool转可见性,0:Hidden,1:Visible
    /// </summary>
    public class BoolToVisiblityDefaultVisible : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
                return "Visible";
            else
                return "Hidden";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// bool转颜色,0:Red,1:SpringGreen
    /// </summary>
    public class BoolToColorSpringGreenRed : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(bool)value)
                return "Green";
            else
                return "Red";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    } 

    /// <summary>
    /// bool转颜色,0:Gold,1:ForestGreen
    /// </summary>
    public class BoolToColorLimeGreenLightGray : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
                return "LimeGreen";
            else
                return "LightGray";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// ICogImage转BitmapImage
    /// </summary>
    public class ICogImageToBitmapImage : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value != null)
                try
                {
                    BitmapImage image= ECGeneric.BitmapToBitmapImage(((FilmstripImage)value).Image.ToBitmap());
                    return image;
                }
                catch
                {
                    return null;
                }
            else
                return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 使用冒号连接连个参数
    /// </summary>
    public class ConcateStringWithColon : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string value0 = System.Convert.ToString(values[0]);
            string value1 = System.Convert.ToString(values[1]);
            return value0 + ":" + value1;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 多参数绑定：object[]{DL高级模式步骤,工具}
    /// </summary>
    public class MultiBindingConverter_ObjectArray : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return values.Clone();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 多参数绑定：object[]{管理员登录,工作加载}
    /// </summary>
    public class MultiBindingConverter_LoginAndWorkLoaded : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values.Length != 2) return false;
            else
               return (bool)values[0] && (!(bool)values[1]); //当用户登录到管理员权限并且此时未装载工作,则可以编辑工作
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


}
