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
        public void WriteImage(ICogImage image, string originalFullNameWithoutExtension,string graphicFullNameWithoutExtension,List<ICogGraphic> graphics, ECWorkStreamInfo streamInfo)
        {
            ECWorkOptionManager.ImageRecordConstants recordType;
            if (!Enum.TryParse<ECWorkOptionManager.ImageRecordConstants>(streamInfo.ImageRecordOption, out recordType)) return;
            switch (recordType)
            {
                case ECWorkOptionManager.ImageRecordConstants.Original:
                    WriteOriginalImage(image, originalFullNameWithoutExtension,streamInfo);
                    break;

                case ECWorkOptionManager.ImageRecordConstants.Graphic:
                    //WriteGraphicImage(image, graphicFullNameWithoutExtension, graphics);
                    SaveRecordGraphicImage(streamInfo.StreamName, graphicFullNameWithoutExtension);
                    break;

                case ECWorkOptionManager.ImageRecordConstants.OriginalAndGraphic:
                    WriteOriginalImage(image, originalFullNameWithoutExtension, streamInfo);
                    SaveRecordGraphicImage(streamInfo.StreamName, graphicFullNameWithoutExtension);
                    break;
                case ECWorkOptionManager.ImageRecordConstants.OriginalAndSVG:
                    WriteSVG(image, graphicFullNameWithoutExtension, graphics);
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
            Messenger.Default.Send<string>(streamName+","+ fullNameWithoutExtension,ECMessengerManager.ImageRecordMessagerKeys.RecordGraphic);
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
                                 bmpImageWriter.WriteCogImage8Grey(fullFileName,image as CogImage8Grey);
                             else if (image.GetType().Name == "CogImage16Grey")
                             {
                                 ICogImage16PixelMemory pm16 = (image as CogImage16Grey).Get16GreyPixelMemory(CogImageDataModeConstants.Read, 0, 0, image.Width, image.Height);
                                 bitmap = new Bitmap(image.Width, image.Height, pm16.Stride, PixelFormat.Format16bppGrayScale, pm16.Scan0);
                                 tool.Operator.Open(fullFileName.Replace(".bmp", ".png"), CogImageFileModeConstants.Write);
                                 tool.Operator.Append(tool.InputImage);
                                 return;
                             }
                             else
                                 bitmap =image.ToBitmap();
                             bmpImageWriter.WriteBitmap(fullFileName,bitmap);
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
        /// 写入覆盖图形的图片
        /// </summary>
        /// <param name="image"></param>
        /// <param name="fullNameWithoutExtension"></param>
        private  void WriteGraphicImage(ICogImage image, string fullNameWithoutExtension,List<ICogGraphic> graphics)
        {
            if (image == null) return;
            Task.Factory.StartNew
                 (new Action(() =>
                 {
                     try
                     {
                         string fullFileName = fullNameWithoutExtension + ".png";
                         Bitmap bp = DrawGrphics(image, graphics);
                         bp.Save(fullFileName);
                     }
                     catch (System.Exception ex)
                     {
                         ECLog.WriteToLog(ex.StackTrace + ex.Message, NLog.LogLevel.Error);
                     }
                 }));
        }

        /// <summary>
        /// 写入SVG
        /// </summary>
        private  void WriteSVG(ICogImage image, string fileNameWithoutExtension, List<ICogGraphic> graphics)
        {
            if (image == null) return;
            Task.Factory.StartNew
                 (new Action(() =>
                 {
                     try
                     {
                         ICogImage imageCopy = image.CopyBase(CogImageCopyModeConstants.CopyPixels);
                         string imgPath = fileNameWithoutExtension + ".png";
                         string svgPath = fileNameWithoutExtension + ".svg";

                         // 保存原图
                         imageCopy.ToBitmap().Save(imgPath);

                         // 创建SVG文件
                         SvgDocument svgDocument = new SvgDocument();
                         // SVG图片
                         SvgImage svgImage = new SvgImage();
                         svgImage.X = 0;
                         svgImage.Y = 0;
                         svgImage.Href = Path.GetFileName(imgPath);
                         svgImage.Width = imageCopy.Width;
                         svgImage.Height = imageCopy.Height;
                         svgDocument.ViewBox=new SvgViewBox(0,0,imageCopy.Width,imageCopy.Height);
                         svgDocument.Children.Add(svgImage);

                         foreach (ICogGraphic graphic in graphics)
                         {
                             switch (graphic.GetType().Name)
                             {
                                 case "CogGraphicLabel":
                                     CogGraphicLabel cogGraphicLabel = graphic as CogGraphicLabel;

                                     // SVG文本
                                     SvgText text = new SvgText();
                                     text.Text = cogGraphicLabel.Text;
                                     SvgUnitCollection svgUnitsX = new SvgUnitCollection();
                                     SvgUnitCollection svgUnitsY = new SvgUnitCollection();
                                     svgUnitsX.Add((SvgUnit)cogGraphicLabel.X);
                                     svgUnitsY.Add((SvgUnit)cogGraphicLabel.Y);
                                     text.X = svgUnitsX;
                                     text.Y = svgUnitsY;
                                     text.FontSize=(SvgUnit)cogGraphicLabel.Font.Size;
                                     text.Stroke = new SvgColourServer(System.Drawing.ColorTranslator.FromHtml(cogGraphicLabel.Color.ToString()));
                                     text.Fill= new SvgColourServer(System.Drawing.ColorTranslator.FromHtml(cogGraphicLabel.Color.ToString()));
                                     svgDocument.Children.Add(text);
                                     break;

                                 case "CogPolygon":
                                     CogPolygon cogPolygon = graphic as CogPolygon;

                                     // SVG多边形
                                     SvgPolygon polygon = new SvgPolygon();
                                     polygon.FillOpacity = 0;
                                     polygon.Stroke = new SvgColourServer(System.Drawing.ColorTranslator.FromHtml(cogPolygon.Color.ToString()));
                                     SvgPointCollection svgPoints = new SvgPointCollection();
                                     double[,] points = cogPolygon.GetVertices();
                                     for (int i = 0; i < points.GetLength(0); i++)
                                     {
                                         svgPoints.Add((SvgUnit)points[i, 0]);
                                         svgPoints.Add((SvgUnit)points[i, 1]);
                                     }
                                     polygon.Points = svgPoints;
                                     svgDocument.Children.Add(polygon);
                                     break;
                                 case "CogRectangle":
                                     CogRectangle cogRectangle = graphic as CogRectangle;

                                     // SVG矩形
                                     SvgRectangle rectangle = new SvgRectangle();
                                     rectangle.FillOpacity = 0;
                                     rectangle.Stroke = new SvgColourServer(System.Drawing.ColorTranslator.FromHtml((cogRectangle.Color.ToString())));
                                     rectangle.X = (SvgUnit)cogRectangle.X;
                                     rectangle.Y = (SvgUnit)cogRectangle.Y;
                                     rectangle.Width = (SvgUnit)cogRectangle.Width;
                                     rectangle.Height = (SvgUnit)cogRectangle.Height;
                                     svgDocument.Children.Add(rectangle);
                                     break;
                                 case "CogRectangleAffine":
                                     CogRectangleAffine cogRectangleAffine = graphic as CogRectangleAffine;

                                     // SVG仿射矩形
                                     SvgPolygon rectangleAffine = new SvgPolygon();
                                     rectangleAffine.FillOpacity = 0;
                                     rectangleAffine.Stroke = new SvgColourServer(System.Drawing.ColorTranslator.FromHtml(cogRectangleAffine.Color.ToString()));
                                     SvgPointCollection rectangleAffinePoints = new SvgPointCollection();
                                     double[,] cogPoints = new double[,] { {cogRectangleAffine.CornerOriginX,cogRectangleAffine.CornerOriginY},
                                                                      { cogRectangleAffine.CornerXX, cogRectangleAffine.CornerXY},
                                                                      { cogRectangleAffine.CornerOppositeX, cogRectangleAffine.CornerOppositeY},
                                                                      {cogRectangleAffine.CornerYX,cogRectangleAffine.CornerYY  } };

                                     for (int i = 0; i < cogPoints.GetLength(0); i++)
                                     {
                                         rectangleAffinePoints.Add((SvgUnit)cogPoints[i, 0]);
                                         rectangleAffinePoints.Add((SvgUnit)cogPoints[i, 1]);
                                     }
                                     rectangleAffine.Points = rectangleAffinePoints;
                                     svgDocument.Children.Add(rectangleAffine);
                                     break;
                                 case "CogCircle":
                                     CogCircle cogCircle = graphic as CogCircle;

                                     // SVG圆
                                     SvgCircle circle = new SvgCircle();
                                     circle.FillOpacity = 0;
                                     circle.Stroke = new SvgColourServer(System.Drawing.ColorTranslator.FromHtml(cogCircle.Color.ToString()));
                                     circle.CenterX = (SvgUnit)cogCircle.CenterX;
                                     circle.CenterY = (SvgUnit)cogCircle.CenterY;
                                     circle.Radius = (SvgUnit)cogCircle.Radius;
                                     svgDocument.Children.Add((circle));
                                     break;
                                 case "CogEllipse":
                                     CogEllipse cogEllipse = graphic as CogEllipse;

                                     // SVG椭圆
                                     SvgEllipse svgEllipse = new SvgEllipse();
                                     svgEllipse.FillOpacity = 0;
                                     svgEllipse.Stroke = new SvgColourServer(System.Drawing.ColorTranslator.FromHtml(cogEllipse.Color.ToString()));
                                     svgEllipse.CenterX = (SvgUnit)cogEllipse.CenterX;
                                     svgEllipse.CenterY = (SvgUnit)cogEllipse.CenterY;
                                     svgEllipse.RadiusX = (SvgUnit)cogEllipse.RadiusX;
                                     svgEllipse.RadiusY = (SvgUnit)cogEllipse.RadiusY;
                                     Svg.Transforms.SvgTransformCollection transCollection=new Svg.Transforms.SvgTransformCollection();
                                     Svg.Transforms.SvgRotate rotate =new Svg.Transforms.SvgRotate((float)CogMisc.RadToDeg(cogEllipse.Rotation),(float)cogEllipse.CenterX, (float)cogEllipse.CenterY);
                                     transCollection.Add(rotate);
                                     svgEllipse.Transforms=transCollection;
                                     svgDocument.Children.Add((svgEllipse));
                                     break;
                                 case "CogLineSegment":
                                     CogLineSegment cogLineSegment = graphic as CogLineSegment;

                                     // SVG直线
                                     SvgLine svgLine = new SvgLine();
                                     svgLine.Stroke = new SvgColourServer(System.Drawing.ColorTranslator.FromHtml(cogLineSegment.Color.ToString()));
                                     svgLine.StartX = (SvgUnit)cogLineSegment.StartX;
                                     svgLine.StartY = (SvgUnit)cogLineSegment.StartY;
                                     svgLine.EndX = (SvgUnit)cogLineSegment.EndX;
                                     svgLine.EndY = (SvgUnit)cogLineSegment.EndY;
                                     svgDocument.Children.Add((svgLine));
                                     break;
                             }
                             svgDocument.Write(svgPath);
                         }
                     }
                     catch (System.Exception ex)
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

        /// <summary>
        /// GDI绘图
        /// </summary>
        /// <param name="cogImage"></param>
        /// <param name="graphics"></param>
        /// <returns></returns>
        private  Bitmap DrawGrphics(ICogImage cogImage, List<ICogGraphic> graphics)
        {
            //判断ICogImage有效
            if (cogImage == null)
                return null;
            //获得Graphics
            Bitmap bitmap = cogImage.ToBitmap();
            try
            {
                Graphics g = Graphics.FromImage(bitmap);

                //绘制
                foreach (ICogGraphic graphic in graphics)
                {
                    switch (graphic.GetType().Name)
                    {
                        case "CogRectangle":
                            CogRectangle rect = graphic as CogRectangle;
                            Color rectColor = System.Drawing.ColorTranslator.FromHtml(rect.Color.ToString());
                            Pen pen = new Pen(rectColor, rect.LineWidthInScreenPixels);
                            g.DrawRectangle(pen, (float)rect.X, (float)rect.Y, (float)rect.Width, (float)rect.Height);
                            break;
                        case "CogCircle":
                            CogCircle circle = graphic as CogCircle;
                            Color circleColor = System.Drawing.ColorTranslator.FromHtml(circle.Color.ToString());
                            Pen circlePen = new Pen(circleColor, circle.LineWidthInScreenPixels);
                            g.DrawEllipse(circlePen, (float)(circle.CenterX - circle.Radius), (float)(circle.CenterY - circle.Radius),
                                (float)circle.Radius, (float)circle.Radius);
                            break;
                        case "CogPolygon":
                            CogPolygon polygon = graphic as CogPolygon;
                            Color polygonColor = System.Drawing.ColorTranslator.FromHtml(polygon.Color.ToString());
                            Pen polygonPen = new Pen(polygonColor, polygon.LineWidthInScreenPixels);
                            double[,] points = polygon.GetVertices();
                            PointF[] pointsF = new PointF[points.Length / 2];
                            for (int i = 0; i < points.Length / 2; i++)
                            {
                                pointsF[i] = new PointF((float)points[i, 0], (float)points[i, 1]);
                            }
                            g.DrawPolygon(polygonPen, pointsF);
                            break;
                        case "CogRectangleAffine":
                            CogRectangleAffine rectAffine = graphic as CogRectangleAffine;
                            Color rectAffineColor = System.Drawing.ColorTranslator.FromHtml(rectAffine.Color.ToString());
                            Pen rectAffinePen = new Pen(rectAffineColor, rectAffine.LineWidthInScreenPixels);
                            double[,] rectAffinePoints = new double[,] { {rectAffine.CornerOriginX,rectAffine.CornerOriginY},
                                                                      { rectAffine.CornerXX, rectAffine.CornerXY},
                                                                      { rectAffine.CornerOppositeX, rectAffine.CornerOppositeY},
                                                                      {rectAffine.CornerYX,rectAffine.CornerYY  } };
                            PointF[] rectAffinePointsF = new PointF[rectAffinePoints.Length / 2];
                            for (int i = 0; i < rectAffinePoints.Length / 2; i++)
                            {
                                rectAffinePointsF[i] = new PointF((float)rectAffinePoints[i, 0], (float)rectAffinePoints[i, 1]);
                            }
                            g.DrawPolygon(rectAffinePen, rectAffinePointsF);
                            break;
                        case "CogGraphicLabel":
                            CogGraphicLabel graphicLabel = graphic as CogGraphicLabel;
                            Color graphicLabelColor = System.Drawing.ColorTranslator.FromHtml(graphicLabel.Color.ToString());
                            Brush brush = new SolidBrush(graphicLabelColor);
                            g.DrawString(graphicLabel.Text, new Font("Microsoft YaHei", graphicLabel.Font.Size), brush, (float)graphicLabel.X, (float)graphicLabel.Y);
                            break;
                        case "CogEllipse":
                            CogEllipse ellipse = graphic as CogEllipse;
                            Color ellipseColor = System.Drawing.ColorTranslator.FromHtml(ellipse.Color.ToString());
                            Pen ellipsePen = new Pen(ellipseColor, ellipse.LineWidthInScreenPixels);
                            CogRectangleAffine boundrect = ellipse.GetBoundingRectangleAffine(true);
                            double x, y, width, height, rotation, skew;
                            boundrect.GetOriginLengthsRotationSkew(out x, out y, out width, out height, out rotation, out skew);
                            g.TranslateTransform((float)ellipse.CenterX,(float)ellipse.CenterY);//把画板(Graphics对象)平移到旋转中心
                            g.RotateTransform((float)CogMisc.RadToDeg(ellipse.Rotation));
                            g.DrawEllipse(ellipsePen, -(float)ellipse.RadiusX, -(float)ellipse.RadiusY, (float)width, (float)height);
                            g.ResetTransform();
                            break;
                        case "CogLineSegment":
                            CogLineSegment lineSegment = graphic as CogLineSegment;
                            Color lineSegmentColor = System.Drawing.ColorTranslator.FromHtml(lineSegment.Color.ToString());
                            Pen lineSegmentPen = new Pen(lineSegmentColor, lineSegment.LineWidthInScreenPixels);
                            g.DrawLine(lineSegmentPen, (float)lineSegment.StartX, (float)lineSegment.StartY, (float)lineSegment.EndX, (float)lineSegment.EndY);
                            break;
                    }
                }
                g.Save();
                g.Dispose();
            }
            catch(System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, NLog.LogLevel.Error);
            }
            return bitmap;
        }
    }
}
