using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using Board.Screens.Controls;
using Board.Utilities;
using CoreGraphics;
using Board.Infrastructure;
using UIKit;

namespace Board.Interface
{
	public class ColorPickerScreen : UIViewController
	{
		UIMenuBanner Banner;
		UIScrollView ScrollView;

		public override void ViewDidLoad ()
		{
			NavigationController.NavigationBar.BarStyle = UIBarStyle.Default;
			NavigationController.NavigationBarHidden = true;

			LoadBanner ();
			View.BackgroundColor = UIColor.White;

			ScrollView = new UIScrollView (new CGRect (0, 0, AppDelegate.ScreenWidth, AppDelegate.ScreenHeight));
			View.AddSubviews (ScrollView, Banner);

			var sortedColors = SortUIColors (UIColor.FromRGB(83, 6, 11), UIColor.FromRGB(146, 94, 70), UIColor.FromRGB(28, 56, 18),
				UIColor.FromRGB(213, 101, 40), UIColor.FromRGB(94, 69, 98), UIColor.FromRGB(82, 58, 27), UIColor.FromRGB(16, 22, 40),
				UIColor.FromRGB(42, 75, 119), UIColor.FromRGB(99, 8, 1), UIColor.FromRGB(123, 104, 97), UIColor.FromRGB(30, 20, 11),
				UIColor.FromRGB(19, 39, 52), UIColor.FromRGB(250, 122, 52), UIColor.FromRGB(255, 213, 134), UIColor.FromRGB(111, 144, 203),
				UIColor.FromRGB(80, 89, 132), UIColor.FromRGB(18, 28, 62), UIColor.FromRGB(122, 104, 103), UIColor.FromRGB(52, 34, 33),
				UIColor.FromRGB(0, 36, 53), UIColor.FromRGB(1, 10, 61), UIColor.FromRGB(65, 34, 88), UIColor.FromRGB(125, 86, 131),
				UIColor.FromRGB(170, 183, 192), UIColor.FromRGB(232, 207, 90), UIColor.FromRGB(178, 27, 78), UIColor.FromRGB(217, 123, 156),
				UIColor.FromRGB(140, 57, 27), UIColor.FromRGB(116, 181, 201), UIColor.FromRGB(250, 253, 162), UIColor.FromRGB(168, 21, 2),
				UIColor.FromRGB(206, 164, 78), UIColor.FromRGB(249, 162, 151), UIColor.FromRGB(132, 90, 125), UIColor.FromRGB(147, 202, 209),
				UIColor.FromRGB(234, 190, 246), UIColor.FromRGB(196, 170, 147), UIColor.FromRGB(0,0,0));

			CreateColorButtons (sortedColors.ToArray());
		}

		private List<UIColor> SortUIColors(params UIColor[] uicolors){
			var listColors = new List<Color> ();

			foreach (var uicolor in uicolors) {
				nfloat red, green, blue, alpha;
				uicolor.GetRGBA (out red, out green, out blue, out alpha); 
				var color = Color.FromArgb ((int)(alpha * 255), (int)(red * 255),
					(int)(green * 255), (int)(blue * 255));
				listColors.Add (color);
			}
			var orderedColors = listColors.OrderByDescending (x => x.GetHue());

			var sortedColors = new List<UIColor> ();
			foreach (var color in orderedColors.ToList()) {
				var uicolor = UIColor.FromRGBA (color.R, color.G, color.B, color.A);
				sortedColors.Add (uicolor);
			}

			return sortedColors;
		}

		private void CreateColorButtons (params UIColor[] colors){
			float yposition = (float)Banner.Frame.Bottom - 20 + 1;
			var listButtons = new List<UIButton> ();
			foreach (var color in colors) {
				var button = CreateColorButton (color, yposition);
				yposition += (float)button.Frame.Height + 1;
				listButtons.Add (button);
				ScrollView.AddSubview (button);
			}
			ScrollView.ContentSize = new CGSize (AppDelegate.ScreenWidth, yposition);
		}

		bool tapped = false;

		private UIButton CreateColorButton(UIColor color, float yposition){
			var button = new UIButton ();
			button.Frame = new CGRect (0, yposition, AppDelegate.ScreenWidth, 80);
			button.BackgroundColor = color;

			button.TouchUpInside += (sender, e) => {
				if (tapped){
					return;
				}

				var boardWithColor = UIBoardInterface.board;
				boardWithColor.MainColor = color;
				CloudController.EditBoard(boardWithColor);

				tapped = true;
				button.Alpha = .5f;
				var thread = new Thread(new ThreadStart(PopOut));
				thread.Start();
			};
			return button;
		}

		private void PopOut()
		{
			Thread.Sleep (300);
			InvokeOnMainThread(delegate {
				AppDelegate.NavigationController.PopViewController(true);
				MemoryUtility.ReleaseUIViewWithChildren(View);
			});
		}

		private void LoadBanner()
		{
			Banner = new UIMenuBanner ("Select", "arrow_left");

			var tap = new UITapGestureRecognizer (tg => {
				if (tg.LocationInView(this.View).X < AppDelegate.ScreenWidth / 4){
					Banner.UnsuscribeToEvents ();
					AppDelegate.NavigationController.PopViewController(true);
					MemoryUtility.ReleaseUIViewWithChildren(View);
				}
			});

			Banner.AddTap (tap);

			Banner.SuscribeToEvents (); 
		}
	}
}