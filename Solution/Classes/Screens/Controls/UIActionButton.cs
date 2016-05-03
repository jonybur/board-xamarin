using System;
using UIKit;
using CoreGraphics;

namespace Board.Screens.Controls
{
	// action button is always on bottom of screen
	public class UIActionButton : UIButton
	{
		public const int Height = 60;

		// TODO: complete this
		public UIActionButton()
		{
			Frame = new CGRect(0, AppDelegate.ScreenHeight - Height, AppDelegate.ScreenWidth, Height);

			var background = GenerateBackground ();
			var title = GenerateTitle ("MAP");

			AddSubviews (background, title);

			/*
			MapButtonEvent = (sender, e) => {

				if (map.Alpha == 0f)
				{ 
					map.Alpha = 1f; 

					using (UIImage listImage = UIImage.FromFile ("./screens/main/list/" + AppDelegate.PhoneVersion + ".jpg")) {
						map_button.SetImage(listImage, UIControlState.Normal);
					}
				} else {
					map.Alpha = 0f;

					using (UIImage mapImage = UIImage.FromFile ("./screens/main/map/" + AppDelegate.PhoneVersion + ".jpg")) {
						map_button.SetImage(mapImage, UIControlState.Normal);
					}
				} 
			};

			map_button.Alpha = .95f;*/
		}


		private UIImageView GenerateBackground(){
			var background = new UIImageView (new CGRect (0, 0, AppDelegate.ScreenWidth, Height));
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
	}
}

