using SimpleTCP;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using NLog;
using System.Windows;
using System.Windows.Media.Imaging;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace VPDLFramework.Models
{
    public class ECGeneric
    {
        /// <summary>
        /// 检测语言,如果当前语言与保存的配置不一致则进行切换
        /// </summary>
        /// <param name="languageKey">语言键值</param>
        public static void CheckLanguage(string languageKey)
        {
            if (string.IsNullOrEmpty(languageKey)) return;
            if (!File.Exists(ECFileConstantsManager.LanguagesFolder + $"\\{languageKey}.xaml")) languageKey = "SimplifiedChinese";

            ResourceDictionary dict = App.Current.Resources.MergedDictionaries.Where(r => r.Source.OriginalString.Contains(@"Languages")).FirstOrDefault();
            if(dict == null) return;
            try
            {
                Uri newUri = new Uri(ECFileConstantsManager.LanguagesFolder + $"\\{languageKey}.xaml", UriKind.Absolute);

                ResourceDictionary resource = new ResourceDictionary();
                dict.Source = newUri;
            }
            catch (Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, NLog.LogLevel.Error);
            }

        }

        /// <summary>
        /// 获取枚举类型的可绑定列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static BindingList<string> GetConstantsBindableList<T>() where T : Enum
        {
            BindingList<string> list = new BindingList<string>();
            foreach (string item in Enum.GetNames(typeof(T)))
            {
                list.Add(item);
            }
            return list;
        }

        /// <summary>
		/// 深度复制对象
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj"></param>
		/// <returns></returns>
        public static T DeepCopy<T>(T obj)
        {
            object retval;
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                //序列化成流
                bf.Serialize(ms, obj);
                ms.Seek(0, SeekOrigin.Begin);
                //反序列化成对象
                retval = bf.Deserialize(ms);
                ms.Close();
            }
            return (T)retval;
        }

        /// <summary>
        /// 通过反射深度复制对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T DeepCopyByReflection<T>(T obj)
        {
            if (obj is string || typeof(T).IsValueType)
            {
                return obj;
            }

            var instance = Activator.CreateInstance(obj.GetType());
            PropertyInfo[] propInfos = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var item in propInfos)
            {
                try
                {
                    if(item.GetValue(obj) !=null)
                        item.SetValue(instance, DeepCopy(item.GetValue(obj)));
                }
                catch (Exception ex) {
                    ECLog.WriteToLog(ex.StackTrace + ex.Message, NLog.LogLevel.Error);
                }
            }

            return (T)instance;
        }

        /// <summary>
        /// Bitmap转BitmapImage
        /// </summary>
        /// <param name="bitmap">输入的Bitmap</param>
        /// <returns></returns>
        public static BitmapImage BitmapToBitmapImage(System.Drawing.Bitmap bitmap)
        {
            if (bitmap == null) return null;
            MemoryStream ms = new MemoryStream();
            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            BitmapImage bit3 = new BitmapImage();
            bit3.BeginInit();
            bit3.StreamSource = ms;
            bit3.EndInit();
            return bit3;
        }

        /// <summary>
        /// 复制目录
        /// </summary>
        /// <param name="sourceDir"></param>
        /// <param name="destinationDir"></param>
        /// <param name="recursive"></param>
        /// <exception cref="DirectoryNotFoundException"></exception>
        public static void CopyDirectory(string sourceDir, string destinationDir, bool recursive)
        {
            // Get information about the source directory
            var dir = new DirectoryInfo(sourceDir);

            // Check if the source directory exists
            if (!dir.Exists)
                throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

            // Cache directories before we start copying
            DirectoryInfo[] dirs = dir.GetDirectories();

            // Create the destination directory
            Directory.CreateDirectory(destinationDir);

            // Get the files in the source directory and copy to the destination directory
            foreach (FileInfo file in dir.GetFiles())
            {
                string targetFilePath = Path.Combine(destinationDir, file.Name);
                file.CopyTo(targetFilePath);
            }

            // If recursive and copying subdirectories, recursively call this method
            if (recursive)
            {
                foreach (DirectoryInfo subDir in dirs)
                {
                    string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                    CopyDirectory(subDir.FullName, newDestinationDir, true);
                }
            }
        }

        /// <summary>
        /// 获取本机可用的IP
        /// </summary>
        public static List<string> GetLocalIPs()
        {
            List<string> list = new List<string>();
            try
            {
                SimpleTcpServer tcpServer = new SimpleTcpServer();
                var ips = tcpServer.GetIPAddresses();
                

                foreach (System.Net.IPAddress ip in ips)
                {
                    int num = ip.ToString().Split('.').Length;
                    if (num==4)
                    {
                        list.Add(ip.ToString());
                    }
                }
                return list;
            }
            catch (System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, NLog.LogLevel.Error);
                return list;
            }
        }

        /// <summary>
        /// 获取异常方法名称
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static string GetExceptionMethodName(Exception ex)
        { 
            string methodName = "";
            try
            {
                StackTrace stackTrace = new StackTrace(ex);
                methodName= stackTrace.GetFrame(0).GetMethod().Name;
            }
            catch { }
            return methodName;
        } 
    }
}
