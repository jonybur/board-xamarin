using UIKit;
using CoreGraphics;
using System;

namespace Clubby.Screens.Controls
{
	// action button is always on bottom of screen
	public class UIActionButton : UIButton
	{
		public const int Height = 50;
		public EventHandler TouchDownEvent;
		UILabel CustomTitleLabel;

		public UIActionButton(string title)
		{
			Frame = new CGRect(0, AppDelegate.ScreenHeight - Height, AppDelegate.ScreenWidth, Height);

			var background = GenerateBackground ();
			CustomTitleLabel = GenerateTitle (title);

			TouchDownEvent = new EventHandler (delegate {
				Alpha = .75f;
			});
			TouchDownRepeat += (sender, e) => {
				Alpha = .75f;
			};

			TouchUpInside += (sender, e) => {
				Alpha = 1f;
			};
			TouchUpOutside += (sender, e) => {
				Alpha = 1f;
			};

			TouchDown += TouchDownEvent;

			AddSubviews (background, CustomTitleLabel);
		}

		public void ChangeTitle(string title){
			CustomTitleLabel.Text = title;
		}

		private UIImageView GenerateBackground(){
			var background = new UIImageView (new CGRect (0, 0, AppDelegate.ScreenWidth, Height));
			background.BackgroundColor = UIColor.FromRGB(249, 249, 249);
			background.Alpha = .95f;
			return background;
		}

		private UILabel GenerateTitle(string title){
			var label = new UILabel ();
			label.Font = AppDelegate.Narwhal24;
			label.TextColor = AppDelegate.ClubbyBlue;
			label.Text = title;
			label.TextAlignment = UITextAlignment.Center;
			label.Frame = new CGRect (0, 0, AppDelegate.ScreenWidth, title.StringSize(label.Font).Height);
			label.Center = new CGPoint (Frame.Width / 2, (Frame.Height / 2) + 3);
			return label;
		}

		protected override void Dispose (bool disposing)
		{
			base.Dispose (disposing);
			TouchDown -= TouchDownEvent;
		}
	}
}

