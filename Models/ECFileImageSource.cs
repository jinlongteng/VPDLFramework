using Cognex.VisionPro.ImageFile;
using Cognex.VisionPro;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using NLog;
using System.Windows.Shapes;

namespace VPDLFramework.Models
{
    public class ECFileImageSource
    {
        #region 构造方法

        /// <summary>
        /// 创建本地磁盘图像源
        /// </summary>
        /// <param name="folder">本地图像文件夹路径</param>
        public ECFileImageSource(string folder)
        {
            ImagePath = folder;
            IsReady = ReadImgInfo(folder);
        }
        /// <summary>
        /// 构造方法
        /// </summary>
        public ECFileImageSource() { }
        #endregion 构造方法

        #region 命令

        /// <summary>
        /// 打开文件夹选择对话框
        /// </summary>
        public GalaSoft.MvvmLight.Command.RelayCommand CmdOpenImgDialog { get; set; }

        #endregion 命令

        #region 方法

        /// <summary>
        /// 获取本地文件夹中的图像信息,支持bmp,png,jpg,读取文件夹正常返回True,异常返回False
        /// </summary>
        /// <param name="path">本地文件夹路径</param>
        /// <returns></returns>
        private bool ReadImgInfo(string path)
        {
            try
            {
                if (path.EndsWith(".bmp") || path.EndsWith(".png")|| path.EndsWith(".jpg")|| path.EndsWith(".tiff"))
                {
                    if (!File.Exists(path))
                        return false;
                }
                else if (path.EndsWith(".idb") || path.EndsWith(".cdb"))
                {
                    IDBCDBImageFileTool = new CogImageFileTool();
                    IDBCDBImageFileTool.Operator.Open(path, CogImageFileModeConstants.Read);
                    TotalNum = IDBCDBImageFileTool.Operator.Count;

                    DirectoryInfo folder = Directory.GetParent(path);
                    CheckConfig(folder.FullName);
                    ImageSourceFolderConfig config = GetFolderConfig(folder.FullName);
                    CurrentIndex = config.LastIndex;
                }
                else
                {
                    string[] bmpImages = Directory.GetFiles(path, "*.bmp");
                    string[] pngImages = Directory.GetFiles(path, "*.png");
                    string[] jpgImages = Directory.GetFiles(path, "*.jpg");
                    string[] tifImages = Directory.GetFiles(path, "*.tif");
                    ImagesInfo = bmpImages.Concat(pngImages).Concat(jpgImages).Concat(tifImages).ToArray();
                    TotalNum = ImagesInfo.Length;

                    CheckConfig(path);
                    ImageSourceFolderConfig config = GetFolderConfig(path);
                    CurrentIndex = config.LastIndex;
                }
                return true;
            }
            catch (Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, LogLevel.Error);
                return false;
            }
        }

        /// <summary>
        /// 从本地文件夹中获取一张图片,图片有效返回True，获取异常或者无图像则返回False
        /// </summary>
        /// <returns></returns>
        public ICogImage RunOnce()
        {
            try
            {
                if (IsReady)
                {
                    if (ImagePath.EndsWith(".bmp") || ImagePath.EndsWith(".png") || ImagePath.EndsWith(".jpg") || ImagePath.EndsWith(".tiff"))
                    {
                        if (File.Exists(ImagePath))
                        {
                            // 获取图像
                            CogImageFileTool imageFileTool = new CogImageFileTool();
                            imageFileTool.Operator.Open(ImagePath, CogImageFileModeConstants.Read);
                            imageFileTool.Run();
                            LastOutputImage = imageFileTool.OutputImage;
                        }

                    }
                    else if (TotalNum != 0 && CurrentIndex < TotalNum)
                    {
                        if (ImagePath.EndsWith(".idb") || ImagePath.EndsWith(".cdb"))
                        {
                            // 获取文件夹配置信息
                            var config = GetFolderConfig(Directory.GetParent(ImagePath).FullName);
                            if(config != null) { CurrentIndex = config.LastIndex; }

                            // 获取图像
                            IDBCDBImageFileTool.NextImageIndex = CurrentIndex;
                            IDBCDBImageFileTool.Run();
                            LastOutputImage = IDBCDBImageFileTool.OutputImage;

                            try
                            {
                                //索引下一张
                                CurrentIndex += 1;
                                if (CurrentIndex == TotalNum)
                                    CurrentIndex = 0;
                                UpdateConfig(Directory.GetParent(ImagePath).FullName);
                            }
                            catch (System.Exception ex)
                            {
                                ECLog.WriteToLog(ex.StackTrace + ex.Message,LogLevel.Error);
                            }
                        }
                        else
                        {
                            // 获取文件夹配置信息
                            var config = GetFolderConfig(ImagePath);
                            if (config != null) { CurrentIndex = config.LastIndex; }

                            // 获取图像
                            CogImageFileTool imageFileTool = new CogImageFileTool();
                            imageFileTool.Operator.Open(ImagesInfo[CurrentIndex], CogImageFileModeConstants.Read);
                            imageFileTool.Run();
                            LastOutputImage = imageFileTool.OutputImage;
                            try
                            {
                                //索引下一张
                                CurrentIndex += 1;
                                if (CurrentIndex == TotalNum)
                                    CurrentIndex = 0;
                                UpdateConfig(ImagePath);
                            }
                            catch (System.Exception ex)
                            {
                                ECLog.WriteToLog(ex.StackTrace + ex.Message,LogLevel.Error);
                            }
                        }        
                    }
                }
                
            }
            catch(Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, NLog.LogLevel.Error);
            }
            return LastOutputImage;
        }

        /// <summary>
        /// 更新图像文件夹的配置，实时跟踪运行到第几张图片
        /// </summary>
        /// <param name="folderPath"></param>
        private void UpdateConfig(string folderPath)
        {
            if (File.Exists(folderPath + @"\" + ECFileConstantsManager.ImageFolderConfigName))
            {
                ImageSourceFolderConfig sourceFolderConfig = GetFolderConfig(folderPath);
                sourceFolderConfig.LastIndex = CurrentIndex;
                CreateFolderConfig(folderPath, sourceFolderConfig);
            }
        }

        /// <summary>
        /// 重置图像起始位置,从第一张图像开始
        /// </summary>
        /// <param name="folderPath"></param>
        public void ResetImageIndexOfConfig(string folderPath)
        {
            if (folderPath == null) return;
            if (folderPath.EndsWith(".idb") || folderPath.EndsWith(".cdb"))
                folderPath = Directory.GetParent(folderPath).FullName;
            if (File.Exists(folderPath + @"\" + ECFileConstantsManager.ImageFolderConfigName))
            {
                ImageSourceFolderConfig sourceFolderConfig = GetFolderConfig(folderPath);
                sourceFolderConfig.LastIndex = 0;
                CreateFolderConfig(folderPath, sourceFolderConfig);
                CurrentIndex = 0;
            }
        }

        /// <summary>
        /// 检查图像文件夹是否存在配置文件，如无则创建
        /// </summary>
        /// <param name="folderPath"></param>
        private void CheckConfig(string folderPath)
        {
            if (File.Exists(folderPath + @"\" + ECFileConstantsManager.ImageFolderConfigName))
            {
                ImageSourceFolderConfig sourceFolderConfig = GetFolderConfig(folderPath);
                if (sourceFolderConfig != null)
                {
                    if (TotalNum == sourceFolderConfig.ImageFilesCount)
                    {
                        CurrentIndex = sourceFolderConfig.LastIndex;
                    }
                    else
                    {
                        ImageSourceFolderConfig newConfig = new ImageSourceFolderConfig();
                        newConfig.ImageFilesCount = TotalNum;
                        newConfig.LastIndex = 0;
                        newConfig.ModifyDate = DateTime.Now;
                        CreateFolderConfig(folderPath, newConfig);
                        CurrentIndex = 0;
                    }
                }
            }
            else
            {
                ImageSourceFolderConfig newConfig = new ImageSourceFolderConfig();
                newConfig.ImageFilesCount = TotalNum;
                newConfig.LastIndex = 0;
                newConfig.ModifyDate = DateTime.Now;
                CreateFolderConfig(folderPath, newConfig);
                CurrentIndex = 0;
            }
        }

        /// <summary>
        /// 获取图像文件夹配置信息
        /// </summary>
        /// <param name="folderPath"></param>
        private ImageSourceFolderConfig GetFolderConfig(string folderPath)
        {
            if (File.Exists(folderPath + @"\" + ECFileConstantsManager.ImageFolderConfigName))
            {
                using (System.IO.StreamReader file = System.IO.File.OpenText(folderPath + @"\" + ECFileConstantsManager.ImageFolderConfigName))
                {
                    using (JsonTextReader reader = new JsonTextReader(file))
                    {
                        var o = (JObject)JToken.ReadFrom(reader);
                        //反序列化Json文件
                        ImageSourceFolderConfig folderConfig = o.ToObject<ImageSourceFolderConfig>();

                        return folderConfig;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 生成图像文件夹配置信息
        /// </summary>
        /// <param name="folderPath"></param>
        private void CreateFolderConfig(string folderPath, ImageSourceFolderConfig config)
        {
            try
            {
                config.ModifyDate = DateTime.Now;
                string jsonData = JsonConvert.SerializeObject(config);
                if (File.Exists(folderPath + @"\" + ECFileConstantsManager.ImageFolderConfigName))
                    File.Delete(folderPath + @"\" + ECFileConstantsManager.ImageFolderConfigName);
                File.WriteAllText(folderPath + @"\" + ECFileConstantsManager.ImageFolderConfigName, jsonData);

                File.SetAttributes(folderPath + @"\" + ECFileConstantsManager.ImageFolderConfigName, FileAttributes.Hidden);
            }
            catch (Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message,LogLevel.Error);
            }
        }

        #endregion 方法

        #region 属性

        /// <summary>
        /// 图像总数量
        /// </summary>
        public int TotalNum;

        /// <summary>
        /// 当前图像的索引
        /// </summary>
        public int CurrentIndex;

        /// <summary>
        /// 所有图像的路径集合
        /// </summary>
        public string[] ImagesInfo;

        /// <summary>
        /// 本地图像文件夹路径
        /// </summary>
        public string ImagePath;

        /// <summary>
        /// 最新获取的一张图像
        /// </summary>
        public ICogImage LastOutputImage;

        /// <summary>
        /// 本地图像源创建成功
        /// </summary>
        public bool IsReady;

        /// <summary>
        /// 读取idb/cdb文件
        /// </summary>
        public CogImageFileTool IDBCDBImageFileTool;

        #endregion 属性
    }
    /// <summary>
    /// 图像文件夹配置
    /// </summary>
    public class ImageSourceFolderConfig
    {
        public ImageSourceFolderConfig()
        { }

        /// <summary>
        /// 图像数量
        /// </summary>
        public int ImageFilesCount { get; set; }

        /// <summary>
        /// 最后一次运行的图片索引
        /// </summary>
        public int LastIndex { get; set; }

        /// <summary>
        /// 修改日期
        /// </summary>
        public DateTime ModifyDate { get; set; }
    }
}
