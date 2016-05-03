using CoreGraphics;
using UIKit;
using System.Collections.Generic;

namespace Board.Screens.Controls
{
	public class UIMenuBanner : UIImageView
	{
		List<UITapGestureRecognizer> taps;

		public const int MenuHeight = 66;

		public UIMenuBanner (string imagePath)
		{
			taps = new List<UITapGestureRecognizer> ();

			// TODO: add 1-2-3 for createscreen functionality

			using (UIImage bannerImage = UIImage.FromFile (imagePath)) {
				Frame = new CGRect (0, 0, bannerImage.Size.Width / 2, bannerImage.Size.Height / 2);
				Image = bannerImage;
				Alpha = .95f;
			}
			//Frame = new CGRect (0, 0, AppDelegate.ScreenWidth, MenuHeight);
			//BackgroundColor = AppDelegate.BoardOrange;

			//var background = GenerateBackground ();
			//Frame = background.Frame;

			//var title = GenerateTitle (imagePath);

			//AddSubviews (background, title);

			UserInteractionEnabled = true;
		}

		private UIImageView GenerateBackground(){
			var background = new UIImageView (new CGRect (0, 0, AppDelegate.ScreenWidth, 66));
			background.BackgroundColor = AppDelegate.BoardOrange;
			background.Alpha = .95f;
			return background;
		}

		private UILabel GenerateTitle(string title){
			var label = new UILabel (new CGRect (0, 0, AppDelegate.ScreenWidth, 30));
			label.Font = AppDelegate.Narwhal30;
			label.TextColor = UIColor.White;
			label.Text = title;
			label.TextAlignment = UITextAlignment.Center;
			label.SizeToFit ();
			label.Center = new CGPoint (Center.X, Center.Y + 12);
			return label;
		}

		public void AddTap(UITapGestureRecognizer tap)
		{
			taps.Add (tap);
		}

		public void SuscribeToEvents()
		{
			foreach (UITapGestureRecognizer tap in taps) {
				AddGestureRecognizer (tap);
			}
		}

		public void UnsuscribeToEvents()
		{
			foreach (UITapGestureRecognizer tap in taps) {
				RemoveGestureRecognizer (tap);
			}
		}
	}
}

