using GalaSoft.MvvmLight;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViDi2.UI;

namespace VPDLFramework.Models
{
	public class ECWorkStreamOrGroupResultChart : ObservableObject
	{
		public ECWorkStreamOrGroupResultChart(string streamOrGroupName, int seriesCount)
		{
			StreamOrGroupName = streamOrGroupName;
			Model = new PlotModel();
			Model.Title = streamOrGroupName;
			Model.TitleColor = OxyColor.Parse("#fffffb");
			Model.TitleFontSize = 12;
			Model.PlotAreaBackground = OxyColor.Parse("#2A2B34");
			Model.Background=OxyColor.Parse("#2A2B34");
			Model.TextColor= OxyColor.Parse("#fffffb");
			Model.PlotAreaBorderColor= OxyColor.Parse("#fffffb");
			
            // 添加图例说明
            Model.Legends.Add(new Legend
			{
				LegendPlacement = LegendPlacement.Outside,
				LegendPosition = LegendPosition.BottomCenter,
				LegendOrientation = LegendOrientation.Horizontal,
			});

			var linearAxis1 = new LinearAxis();
            linearAxis1.MinorGridlineThickness = 0.5;
            linearAxis1.MajorGridlineColor = OxyColor.Parse("#4f5555");
            linearAxis1.MinorGridlineColor = OxyColor.Parse("#4f5555");
            linearAxis1.MajorGridlineStyle = LineStyle.Solid;
			linearAxis1.MinorGridlineStyle = LineStyle.Dot;
			linearAxis1.Title = "Y";
			linearAxis1.AxislineColor = OxyColor.Parse("#fffffb");
			linearAxis1.TicklineColor = OxyColor.Parse("#fffffb");
			linearAxis1.TitleColor = OxyColor.Parse("#fffffb");
            linearAxis1.TextColor = OxyColor.Parse("#fffffb");
            Model.Axes.Add(linearAxis1);

			var linearAxis2 = new LinearAxis();
			linearAxis2.MinorGridlineThickness = 0.5;
            linearAxis2.MajorGridlineColor = OxyColor.Parse("#4f5555");
            linearAxis2.MinorGridlineColor = OxyColor.Parse("#4f5555");
            linearAxis2.MajorGridlineStyle = LineStyle.Solid;
			linearAxis2.MinorGridlineStyle = LineStyle.Dot;
			linearAxis2.Position = AxisPosition.Bottom;
			linearAxis2.Title = "X";
			linearAxis2.AxislineColor = OxyColor.Parse("#fffffb");
			linearAxis2.TicklineColor = OxyColor.Parse("#fffffb");
			linearAxis2.TitleColor = OxyColor.Parse("#fffffb");
			linearAxis2.TextColor = OxyColor.Parse("#fffffb");
			
            Model.Axes.Add(linearAxis2);

			for (int i = 0; i < seriesCount; i++)
			{
				Model.Series.Add(new LineSeries());
				(Model.Series[i] as LineSeries).MarkerType = MarkerType.None;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="doubles"></param>
		public void AddData(List<double> seriesYData, List<string> seriesName)
		{
			if (seriesYData != null && seriesName != null && seriesYData.Count == seriesName.Count)
			{
				if (Model != null && Model.Series.Count == seriesName.Count)
				{
					for (int i = 0; i < seriesYData.Count; i++)
					{
						Model.Series[i].Title = seriesName[i];
						LineSeries serie = Model.Series[i] as LineSeries;
						if(serie.Points.Count>=100000)
							serie.Points.RemoveAt(0);
						serie.Points.Add(new DataPoint(serie.Points.Count + 1, seriesYData[i]));
						Model.InvalidatePlot(true);
					}
                }
			}
		}

		/// <summary>
		/// 图表模型
		/// </summary>
		private PlotModel _model;

		public PlotModel Model
		{
			get { return _model; }
			set
			{
				_model = value;
				RaisePropertyChanged();

			}
		}

		/// <summary>
		/// 工作流或组的名称
		/// </summary>
		private string _streamOrGroupName;

		public string StreamOrGroupName
		{
			get { return _streamOrGroupName; }
			set
			{
				_streamOrGroupName = value;
				RaisePropertyChanged();
			}
		}

	}
}
