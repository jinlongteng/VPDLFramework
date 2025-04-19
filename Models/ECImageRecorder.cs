using Cognex.VisionPro.ImageFile;
using Cognex.VisionPro;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViDi2;
using Svg;
using System.Windows.Media.Imaging;
using System.Drawing;
using GalaSoft.MvvmLight.Threading;
using System.Threading;
using VPDLFramework.Views;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight;
using Cognex.VisionPro.CogBmpImageWriter;
using System.Drawing.Imaging;

namespace VPDLFramework.Models
{
    public class ECImageRecorder:ObservableObject
    {
        public ECImageRecorder() 
        {
            
        }

        /// <summary>
        /// Record显示控件
        /// </summary>
        public CogRecordDisplay _recordDisplay;

        /// <summary>
        /// 写入图像
        /// </summary>
        /// <param name="image"></param>
        /// <param name="originalFullNameWithoutExtension"></param>
        /// <param name="graphicFullNameWithoutExtension"></param>
        /// <param name="graphics"></param>
        /// <param name="recordType"></param>
        /// <param name="record"></param>
        public void WriteImage(ICogImage image, string originalFullNameWithoutExtension,string graphicFullNameWithoutExtension,CogGraphicCollection graphics, ECWorkStreamInfo streamInfo)
        {
            ;
            ECWorkOptionManager.ImageRecordConstants recordType;
            if (!Enum.TryParse(streamInfo.ImageRecordOption, out recordType)) return;
            switch (recordType)
            {
                case ECWorkOptionManager.ImageRecordConstants.Original:
                    WriteOriginalImage(image, originalFullNameWithoutExtension,streamInfo);
                    break;

                case ECWorkOptionManager.ImageRecordConstants.Graphic:
                    
                    SaveRecordGraphicImage(streamInfo.StreamName, graphicFullNameWithoutExtension);
                    break;

                case ECWorkOptionManager.ImageRecordConstants.OriginalAndGraphic:
                    WriteOriginalImage(image, originalFullNameWithoutExtension, streamInfo);
                    SaveRecordGraphicImage(streamInfo.StreamName, graphicFullNameWithoutExtension);
                    break;
            }
        }
       
        /// <summary>
        /// 保存Record的结果图形
        /// </summary>
        /// <param name="record"></param>
        /// <param name="fullNameWithoutExtension"></param>
        private void SaveRecordGraphicImage(string streamName,string fullNameWithoutExtension)
        {
            Messenger.Default.Send(streamName+","+ fullNameWithoutExtension,ECMessengerManager.ImageRecordMessagerKeys.RecordGraphic);
        }

        /// <summary>
        /// 写入原图
        /// </summary>
        /// <param name="image">图像</param>
        /// <param name="path">路径</param>
        private  void WriteOriginalImage(ICogImage image, string fullNameWithoutExtension,ECWorkStreamInfo streamInfo)
        {
            if (image == null) return;
            Task.Factory.StartNew
                 (new Action(() =>
                 {
                     try
                     {
                         CogImageFileTool tool = new CogImageFileTool();
                         tool.InputImage = image;
                         string fullFileName = "";
                         if (tool.InputImage.GetType() == typeof(CogImage16Range))
                             fullFileName = fullNameWithoutExtension + ".idb";
                         else if(streamInfo.OriginalImageTypeConstant!=null&& streamInfo.OriginalImageTypeConstant == "BMP")
                             fullFileName = fullNameWithoutExtension + ".bmp";
                         else
                             fullFileName = fullNameWithoutExtension + ".png";

                         if (!(new FileInfo(fullFileName).Directory.Exists))
                             Directory.CreateDirectory(new FileInfo(fullFileName).Directory.FullName);

                         // bmp图片(高速存图)
                         if (fullFileName.EndsWith(".bmp"))
                         {
                             CogBmpImageWriter bmpImageWriter = new CogBmpImageWriter();
                             Bitmap bitmap = null;

                             if (image.GetType().Name == "CogImage8Grey")
                                 bmpImageWriter.WriteCogImage8Grey(fullFileName, image as CogImage8Grey);
                             else if (image.GetType().Name == "CogImage16Grey")
                             {
                                 ICogImage16PixelMemory pm16 = (image as CogImage16Grey).Get16GreyPixelMemory(CogImageDataModeConstants.Read, 0, 0, image.Width, image.Height);
                                 bitmap = new Bitmap(image.Width, image.Height, pm16.Stride, PixelFormat.Format16bppGrayScale, pm16.Scan0);
                                 tool.Operator.Open(fullFileName.Replace(".bmp", ".png"), CogImageFileModeConstants.Write);
                                 tool.Operator.Append(tool.InputImage);
                                 return;
                             }
                             else
                                 bitmap = image.ToBitmap();
                             bmpImageWriter.WriteBitmap(fullFileName, bitmap);
                             return;
                         }

                         // png及idb图片
                         tool.Operator.Open(fullFileName, CogImageFileModeConstants.Write);
                         tool.Operator.Append(tool.InputImage);
                         if (fullFileName.EndsWith(".idb"))
                             tool.Operator.Close();
                     }
                     catch(System.Exception ex) 
                     { 
                         ECLog.WriteToLog(ex.StackTrace + ex.Message, NLog.LogLevel.Error); 
                     }
                 }));
        }

        /// <summary>
        /// Bitmap转BitmapImage
        /// </summary>
        /// <param name="bitmap">输入的Bitmap</param>
        /// <returns></returns>
        private BitmapImage BitmapToBitmapImage(System.Drawing.Bitmap bitmap)
        {
            var bitmapImage = new BitmapImage();
            using (MemoryStream ms = new MemoryStream())
            {
                //bitmap.Save(ms, bitmap.RawFormat);
                bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = ms;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
            }
            return bitmapImage;
        }
    }
}
