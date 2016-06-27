using CoreGraphics;
using UIKit;
using System.Collections.Generic;
using CoreAnimation;
using Foundation;

namespace Board.Screens.Controls
{
	public sealed class UIMenuBanner : UIImageView
	{
		List<UITapGestureRecognizer> taps;

		public const int Height = 66;


		public UIMenuBanner (string title, string left_button = null, string right_button = null, int steps_number = 0, int current_step = 0)
		{
			taps = new List<UITapGestureRecognizer> ();

			var backgroundView = GenerateBackground ();
			Frame = backgroundView.Frame;

			var titleLabel = GenerateTitle (title);

			AddSubview (backgroundView);

			if (left_button != null) {
				var leftButton = GenerateButton (left_button, true);	
				AddSubview (leftButton);
			}
			if (right_button != null) {
				var rightButton = GenerateButton (right_button, false);
				AddSubview (rightButton);
			}

			AddSubviews (titleLabel);

			if (steps_number > 0) {
				var stepsView = GenerateStepsBackground();

				var labels = GenerateStepsLabels (steps_number, current_step, (float)stepsView.Frame.Height);

				stepsView.AddSubviews(labels);

				AddSubview (stepsView);

				Frame = new CGRect (0, 0, AppDelegate.ScreenWidth, stepsView.Frame.Bottom);
			}

			UserInteractionEnabled = true;
		}

		private UIImageView GenerateStepsBackground(){
			var background = new UIImageView (new CGRect (0, Height + 1, AppDelegate.ScreenWidth, 33));
			background.BackgroundColor = UIColor.FromRGB(249, 249, 249);
			//background.Alpha = .95f;

			return background;
		}

		private UILabel[] GenerateStepsLabels(int steps_number, int current_step, float frame_height){
			var labels = new UILabel[steps_number];

			for (int i = 0; i < steps_number; i++) {
				var label = new UILabel ();
				int labelText = i + 1;

				label.Font = AppDelegate.Narwhal20;
				label.Text = labelText.ToString();
				label.TextColor = AppDelegate.BoardOrange;//UIColor.White;

				if (labelText != current_step) {
					label.Alpha = .5f;
				}

				var size = label.Text.StringSize (label.Font);

				float xposition = AppDelegate.ScreenWidth * ((float)(i + 1) / (float)(steps_number + 1));

				label.Frame = new CGRect(0, 0, size.Width, size.Height);
				label.Center = new CGPoint(xposition, (frame_height / 2) + 2);

				labels [i] = label;
			}

			return labels;
		}

		public void AnimateShow(){
			var animation =  new CABasicAnimation();
			animation.KeyPath = "position.y";
			animation.From = new NSNumber(Frame.Y + Frame.Height / 2);
			animation.TimingFunction = CAMediaTimingFunction.FromName(CAMediaTimingFunction.EaseInEaseOut);
			animation.To = new NSNumber(Frame.Height / 2);
			animation.Duration = .2f;
			Layer.AddAnimation(animation, "show");

			Frame = new CGRect (0, 0, Frame.Width, Frame.Height);
		}

		public void AnimateHide(){
			var animation =  new CABasicAnimation();
			animation.KeyPath = "position.y";
			animation.From = new NSNumber(Frame.Y + Frame.Height / 2);
			animation.TimingFunction = CAMediaTimingFunction.FromName(CAMediaTimingFunction.EaseInEaseOut);
			animation.To = new NSNumber (- Frame.Height / 2);
			animation.Duration = .2f;
			Layer.AddAnimation(animation, "hide");

			Frame = new CGRect (0, -Frame.Height, Frame.Width, Frame.Height);
		}

		private UIImageView GenerateButton(string filename, bool ontheleft){

			var buttonView = new UIImageView ();

			using (var img = UIImage.FromFile ("./menubanner/"+filename+".png")) {
				float imgw = (float)img.Size.Width / 2, imgh = (float)img.Size.Height / 2;
				float xposition = ontheleft ? 0 : (float)Frame.Width - imgw;

				buttonView.Image = img.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
				buttonView.Frame = new CGRect (xposition, 0, imgw, imgh);
				buttonView.TintColor = AppDelegate.BoardOrange;
			}

			return buttonView;
		}

		private UIImageView GenerateBackground(){
			var background = new UIImageView (new CGRect (0, 0, AppDelegate.ScreenWidth, 66));
			background.BackgroundColor = UIColor.FromRGB(249, 249, 249);

			return background;
		}

		private UILabel GenerateTitle(string title){
			var label = new UILabel (new CGRect (0, 0, AppDelegate.ScreenWidth, 30));
			label.Font = AppDelegate.Narwhal30;
			label.TextColor = AppDelegate.BoardOrange;
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

