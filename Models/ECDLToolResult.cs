using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViDi2.UI.ViewModels;
using ViDi2;

namespace VPDLFramework.Models
{
    public class ECDLToolResult
    {
        /// <summary>
        /// 获取BlueLocate工具的结果：Model名称,得分,缩放,位置X,位置Y,角度,视图Pose Matrix[M11,M12,M21,M22,OffsetX,OffsetY]
        /// </summary>
        /// <param name="marking">BlueLocate工具结果Marking</param>
        /// <returns></returns>
        public static List<Tuple<string, double, double, double, double, double, double[]>> GetBlueLocateResult(IMarking marking)
        {
            List<Tuple<string, double, double, double, double, double, double[]>> tuples = new List<Tuple<string, double, double, double, double, double, double[]>>();
            IBlueMarking blueMarking = marking as IBlueMarking;
            if (blueMarking.Views.Count > 0)
            {
                // 视图的ROI Pose
                var pose = blueMarking.Views.First().Pose;
                double[] viewmatrix = new double[] { pose.M11, pose.M12, pose.M21, pose.M22, pose.OffsetX, pose.OffsetY };

                int count = blueMarking.Views.First().Matches.Count;
                foreach (IMatch match in blueMarking.Views.First().Matches)
                {
                    if ((match as INodeModelMatch) != null)
                    {
                        var p = (match as INodeModelMatch).Pose;
                        double[] matrix = new double[] { p.M11, p.M12, p.M21, p.M22, p.OffsetX, p.OffsetY };
                        tuples.Add(new
                            Tuple<string, double, double, double, double, double, double[]>
                            (match.ModelName, match.Score, (match as INodeModelMatch).Scale, p.OffsetX, p.OffsetY, (match as INodeModelMatch).Orientation, viewmatrix));
                    }
                    else if((match as ILayoutModelMatch)!=null)
                    {
                        tuples.Add(new
                            Tuple<string, double, double, double, double, double, double[]>
                            (match.ModelName, (match as ILayoutModelMatch).AllRegionsPassed?1:0, 0, 0, 0, 0, viewmatrix));
                    }
                }
            }
            return tuples;
        }

        /// <summary>
        /// 获取BlueRead工具的结果：Model名称,字符内容,视图Pose Matrix[M11,M12,M21,M22,OffsetX,OffsetY]
        /// </summary>
        /// <param name="marking">BlueRead工具结果Marking</param>
        /// <returns></returns>
        public static List<Tuple<string, string, double[]>> GetBlueReadResult(IMarking marking)
        {
            IBlueMarking blueMarking = marking as IBlueMarking;
            List<Tuple<string, string, double[]>> tuples = new List<Tuple<string, string, double[]>>();
            if (blueMarking.Views.Count > 0)
                foreach (IMatch match in blueMarking.Views.First().Matches)
                {
                    // 视图的ROI Pose
                    var pose = blueMarking.Views.First().Pose;
                    double[] matrix = new double[] { pose.M11, pose.M12, pose.M21, pose.M22, pose.OffsetX, pose.OffsetY };
                    var readmodelmatch = match as IReadModelMatch;
                    var resultString = readmodelmatch.FeatureString;
                    tuples.Add(new
                        Tuple<string, string, double[]>
                        (match.ModelName, resultString, matrix));
                }
            return tuples;
        }

        /// <summary>
        /// 获取Red工具的结果：得分,面积,周长,中心坐标X,中心坐标Y,视图Pose Matrix[M11,M12,M21,M22,OffsetX,OffsetY],轮廓Point[]
        /// </summary>
        /// <param name="marking">Red工具结果Marking</param>
        /// <returns></returns>
        public static List<Tuple<double, double, double, double, double, double[], PointF[]>> GetRedResult(IMarking marking)
        {
            List<Tuple<double, double, double, double, double, double[], PointF[]>> tuples = new List<Tuple<double, double, double, double, double, double[], PointF[]>>();
            IRedMarking redMarking = marking as IRedMarking;
            if (redMarking.Views.Count > 0)
                foreach (ViDi2.IRegion region in redMarking.Views.First().Regions)
                {
                    // 视图的ROI Pose
                    var pose = redMarking.Views.First().Pose;
                    double[] matrix = new double[] { pose.M11, pose.M12, pose.M21, pose.M22, pose.OffsetX, pose.OffsetY };

                    // 缺陷轮廓
                    var points = region.Outer;
                    PointF[] _points = new PointF[points.Count];

                    Parallel.For(0, points.Count, i =>
                    {
                        _points[i].X = (float)points[i].X;
                        _points[i].Y = (float)points[i].Y;
                    });

                    tuples.Add(new Tuple<double, double, double, double, double, double[], PointF[]>(
                       region.Score, region.Area, region.Perimeter, region.Center.X, region.Center.Y, matrix, _points));
                }
            return tuples;
        }

        /// <summary>
        /// Green工具结果：标签名称,得分,视图Pose Matrix[M11,M12,M21,M22,OffsetX,OffsetY]
        /// </summary>
        /// <param name="marking">Green工具的结果Marking</param>
        /// <returns></returns>
        public static List<Tuple<string, double, double[]>> GetGreenResult(IMarking marking)
        {
            IGreenMarking greenMarking = marking as IGreenMarking;
            List<Tuple<string, double, double[]>> tuples = new List<Tuple<string, double, double[]>>();
            if (greenMarking.Views.Count > 0)
            {
                foreach (IGreenView view in greenMarking.Views)
                {
                    // 视图的ROI Pose
                    var pose = view.Pose;
                    double[] matrix = new double[] { pose.M11, pose.M12, pose.M21, pose.M22, pose.OffsetX, pose.OffsetY };
                    tuples.Add(new Tuple<string, double, double[]>(
                      view.BestTag == null ? "" : view.BestTag.Name, view.BestTag == null ? 0.0 : view.BestTag.Score, matrix));
                }
            }
            return tuples;
        }
    }
}
