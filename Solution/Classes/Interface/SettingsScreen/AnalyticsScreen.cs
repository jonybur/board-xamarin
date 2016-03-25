using PNChartBinding;
using Foundation;
using CoreGraphics;

using UIKit;
using Board.Screens.Controls;
using System;
using Board.Utilities;

namespace Board.Interface
{
	public class AnalyticsScreen : UIViewController
	{
		MenuBanner Banner;
		UIScrollView ScrollView;
		PNBarChart BarChart;
		PNPieChart PieChart;
		PNScatterChart ScatterChart;

		public override void ViewDidLoad ()
		{
			ScrollView = new UIScrollView (new CGRect (0, 0, AppDelegate.ScreenWidth, AppDelegate.ScreenHeight));

			View.BackgroundColor = UIColor.White;

			LoadBanner ();

			View.AddSubviews (ScrollView, Banner);
		}

		public override void ViewDidAppear (bool animated)
		{
			BarChart = GenerateBarChart (100);
			PieChart = GeneratePieChart ((float)BarChart.Frame.Bottom + 50);
			ScatterChart = GenerateScatterChart ((float)PieChart.Frame.Bottom + 50);

			ScrollView.AddSubviews (BarChart, PieChart, ScatterChart);
			ScrollView.ContentSize = new CGSize (AppDelegate.ScreenWidth, ScatterChart.Frame.Bottom + 30);

			Banner.SuscribeToEvents ();
		}

		private void LoadBanner()
		{
			Banner = new MenuBanner ("./boardinterface/screens/analytics/banner/" + AppDelegate.PhoneVersion + ".jpg");

			var tap = new UITapGestureRecognizer (tg => {
				if (tg.LocationInView(this.View).X < AppDelegate.ScreenWidth / 4){
					NavigationController.PopViewController(true);
					Banner.UnsuscribeToEvents ();
					MemoryUtility.ReleaseUIViewWithChildren (View);
				}
			});

			Banner.AddTap (tap);
		}

		// boardloads per day
		private PNBarChart GenerateBarChart(float yposition)
		{
			PNBarChart bar = new PNBarChart ();

			bar.Frame = new CGRect (10, yposition, AppDelegate.ScreenWidth, AppDelegate.ScreenWidth);

			Random rnd = new Random ();

			bar.YValues= new []{new NSNumber(rnd.Next(25,150)),
				new NSNumber(rnd.Next(25,150)),
				new NSNumber(rnd.Next(25,150)),
				new NSNumber(rnd.Next(25,150)),
				new NSNumber(rnd.Next(25,150)),
				new NSNumber(rnd.Next(25,150)),
				new NSNumber(rnd.Next(25,150))};

			bar.XLabels = new []{ new NSString ("MON"),
								new NSString ("TUE"),
								new NSString ("WED"),
								new NSString ("THU"),
								new NSString ("FRI"),
								new NSString ("SAT"),
								new NSString ("SUN")};
			
			bar.ShowLabel = false;
			bar.IsShowNumbers = false;
			bar.ShowLevelLine = false;
			bar.DisplayAnimated = true;
			bar.ChartMarginBottom = 35;
			bar.ChartMarginTop = 10;

			// removes all labels
			//foreach (var vv in bar.Subviews) {
				//vv.RemoveFromSuperview ();
			//}

			UILabel yDescription = new UILabel (new CGRect(0, 0, 100, 14));
			yDescription.Text = "# of visits";
			yDescription.Font = UIFont.SystemFontOfSize (12);
			yDescription.TextColor = AppDelegate.BoardBlack;
			yDescription.BackgroundColor = UIColor.FromRGBA (0, 0, 0, 0);
			bar.AddSubview (yDescription);

			bar.StrokeChart ();

			return bar;
		}

		// percentage of men vs women population
		private PNPieChart GeneratePieChart(float yposition)
		{
			Random random = new Random ();

			PNPieChartDataItem chartData, chartData2;
			chartData = new PNPieChartDataItem ();
			chartData2 = new PNPieChartDataItem ();

			chartData.TextDescription = "MEN";
			//chartData.Color = AppDelegate.BoardLightBlue;
			chartData.Color = UIColor.FromRGB (63, 168, 108);
			chartData.Value = random.Next (40, 50);

			chartData2.TextDescription = "WOMEN";
			//chartData2.Color = AppDelegate.BoardOrange;
			chartData2.Color = UIColor.FromRGB(61, 190, 107);
			chartData2.Value = random.Next (40, 50);

			float width = AppDelegate.ScreenWidth - 80;
			PNPieChart piechart = new PNPieChart (new CGRect (40, yposition, width, width),
												new []{ chartData, chartData2 });

			piechart.StrokeChart ();

			return piechart;
		}

		// age x content likes
		private PNScatterChart GenerateScatterChart(float yposition)
		{
			Random random = new Random ();

			var scatterchart = new PNScatterChart ();
			scatterchart.Frame = new CGRect (10, yposition, AppDelegate.ScreenWidth-20, AppDelegate.ScreenWidth-20);
			scatterchart.SetupDefaultValues ();

			int minAge = 21, maxAge = 30;
			int minLikes = 1, maxLikes = 14;

			scatterchart.SetAxisXWithMinimumValue (minAge, maxAge, 6);
			scatterchart.SetAxisYWithMinimumValue (minLikes, maxLikes, 6);
			scatterchart.ShowLabel = true;
			scatterchart.XLabelFormat = "Tasty";

			var chartData = new PNScatterChartData ();

			chartData.ItemCount = 40;
			chartData.Size = 4;
			chartData.FillColor = UIColor.FromRGB (63, 168, 108);

			NSMutableArray XAr1 = new NSMutableArray (chartData.ItemCount);
			NSMutableArray YAr1 = new NSMutableArray (chartData.ItemCount);
			for (int i = 0; i < (int)chartData.ItemCount; i++) {
				XAr1.Add (new NSNumber (random.Next(minAge, maxAge)));
				YAr1.Add (new NSNumber (random.Next(minLikes, maxLikes)));
			}

			chartData.GetData = new LCScatterChartDataGetter (delegate(nuint arg0) {
				nfloat xValue = XAr1.GetItem<NSNumber>(arg0).NFloatValue;
				nfloat yValue = YAr1.GetItem<NSNumber>(arg0).NFloatValue;
				return PNScatterChartDataItem.DataItemWithX(xValue, yValue);
			});

			scatterchart.Setup ();
			scatterchart.ChartData = new [] { chartData };

			UILabel yDescription = new UILabel (new CGRect(35, 0, 100, 14));
			yDescription.Text = "# of likes";
			yDescription.Font = UIFont.SystemFontOfSize (12);
			yDescription.TextColor = AppDelegate.BoardBlack;
			yDescription.BackgroundColor = UIColor.FromRGBA (0, 0, 0, 0);
			scatterchart.AddSubview (yDescription);

			UILabel xDescription = new UILabel (new CGRect(scatterchart.Frame.Width - 50, scatterchart.Frame.Height - 50, 50, 14));
			xDescription.Text = "user age";
			xDescription.Font = UIFont.SystemFontOfSize (12);
			xDescription.TextColor = AppDelegate.BoardBlack;
			xDescription.BackgroundColor = UIColor.FromRGBA (0, 0, 0, 0);
			scatterchart.AddSubview (xDescription);


			return scatterchart;
		}

	}
}

