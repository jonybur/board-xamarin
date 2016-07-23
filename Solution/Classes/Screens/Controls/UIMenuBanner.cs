using System;
using System.Collections.Generic;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using Haneke;
using UIKit;

namespace Clubby.Screens.Controls
{
	public sealed class UIMenuBanner : UIImageView
	{
		List<UITapGestureRecognizer> taps;
		List<UIImageView> ListButtons;
		Action LeftTap, RightTap;
		UIClubbyLogo ClubbyLogo;
		UILabel TitleLabel;
		private const float buttonAlpha = .8f;
		public const int Height = 66;
		bool TappingEnabled;

		public UIMenuBanner (string title, string left_button = null, string right_button = null, int steps_number = 0, int current_step = 0)
		{
			taps = new List<UITapGestureRecognizer> ();
			ListButtons = new List<UIImageView> ();

			var backgroundView = GenerateBackground ();
			Frame = backgroundView.Frame;

			var bottomLineView = new UIImageView ();
			bottomLineView.Frame = new CGRect (0, backgroundView.Frame.Bottom - 1, backgroundView.Frame.Width, 1);
			bottomLineView.BackgroundColor = AppDelegate.ClubbyYellow;
			bottomLineView.Alpha = 1f;

			TitleLabel = GenerateTitle (title);

			AddSubview (backgroundView);

			if (left_button != null) {
				var leftButton = GenerateButton (left_button, true);
				ListButtons.Add (leftButton);
				AddSubview (leftButton);
			}
			if (right_button != null) {
				var rightButton = GenerateButton (right_button, false);
				ListButtons.Add (rightButton);
				AddSubview (rightButton);
			}

			AddSubviews (TitleLabel);

			if (steps_number > 0) {
				var stepsView = GenerateStepsBackground();

				var labels = GenerateStepsLabels (steps_number, current_step, (float)stepsView.Frame.Height);

				stepsView.AddSubviews(labels);

				AddSubview (stepsView);

				Frame = new CGRect (0, 0, AppDelegate.ScreenWidth, stepsView.Frame.Bottom);
			}

			AddSubview (bottomLineView);

			UserInteractionEnabled = true;

			TappingEnabled = true;

			var tapGesture = new UITapGestureRecognizer (delegate(UITapGestureRecognizer obj) {
				if (!TappingEnabled){
					return;
				}

				if (obj.LocationInView(this).X > AppDelegate.ScreenWidth * .75) {
					if (RightTap!=null){
						RightTap();
					}
				} else if (obj.LocationInView(this).X < AppDelegate.ScreenWidth * .25) {
					if (LeftTap !=null){
						LeftTap();
					}
				}
			});

			AddGestureRecognizer (tapGesture);
		}

		public void AddLeftTap(Action action){
			LeftTap = action;
		}

		public void AddRightTap(Action action){
			RightTap = action;
		}

		private UIImageView GenerateStepsBackground(){
			var background = new UIImageView (new CGRect (0, Height + 1, AppDelegate.ScreenWidth, Height / 2));
			background.BackgroundColor = UIColor.FromRGB(16, 16, 16);
			return background;
		}

		private UILabel[] GenerateStepsLabels(int steps_number, int current_step, float frame_height){
			var labels = new UILabel[steps_number];

			for (int i = 0; i < steps_number; i++) {
				var label = new UILabel ();
				int labelText = i + 1;

				label.Font = AppDelegate.Narwhal20;
				label.Text = labelText.ToString();
				label.TextColor = AppDelegate.ClubbyBlack;

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
			float buttonSize = (Height / 2) * 0.65f;
			buttonView = new UIImageView (new CGRect(0, UIStatusBar.Height * (.4f) - 1 + (UIMenuBanner.Height - buttonSize) / 2, buttonSize, buttonSize));
			buttonView.ContentMode = UIViewContentMode.ScaleAspectFit;
			buttonView.SetImage ("./menubanner/" + filename + ".png");
			buttonView.Alpha = buttonAlpha;

			if (ontheleft) {
				buttonView.Center = new CGPoint(buttonView.Frame.Width / 2 + 10, buttonView.Center.Y);
			} else {
				buttonView.Center = new CGPoint(Frame.Width - buttonView.Frame.Width / 2 - 10, buttonView.Center.Y); 
			}

			return buttonView;
		}

		private UIImageView GenerateBackground(){
			var background = new UIImageView (new CGRect (0, 0, AppDelegate.ScreenWidth, Height));
			background.BackgroundColor = UIColor.FromRGB(16, 16, 16);

			return background;
		}

		private UILabel GenerateTitle(string title){
			var label = new UILabel (new CGRect (0, 0, AppDelegate.ScreenWidth, 1));

			label.Font = UIFont.SystemFontOfSize(20, UIFontWeight.Medium);

			label.TextColor = UIColor.White;
			label.Text = title;
			label.TextAlignment = UITextAlignment.Center;

			label.SizeToFit ();

			label.Center = new CGPoint (Center.X, Height / 2 + UIStatusBar.Height / 2);
			return label;
		}

		public void ChangeTitle(string newTitle){
			TitleLabel.Text = newTitle;	
			TitleLabel.SizeToFit ();
			TitleLabel.Center = new CGPoint (AppDelegate.ScreenWidth / 2, Height / 2 + UIStatusBar.Height / 2);

			foreach (var button in ListButtons) {
				button.Alpha = 0f;
			}

			ClubbyLogo.Alpha = 0f;
			TitleLabel.Alpha = 1f;

			TappingEnabled = false;
		}

		public void ChangeTitle(string newTitle, UIFont newFont){
			TitleLabel.Font = newFont;
			ChangeTitle (newTitle);
		}

		public void ChangeTitle(string newTitle, UIFont newFont, UIColor newColor){
			TitleLabel.Font = newFont;
			TitleLabel.TextColor = newColor;
			ChangeTitle (newTitle);
		}

		sealed class UIClubbyLogo : UIImageView{
			public UIClubbyLogo(){
				
				Frame = new CGRect (0, 0, 40, 40);
				Center = new CGPoint (AppDelegate.ScreenWidth / 2, Height / 2 + UIStatusBar.Height / 2);
				this.SetImage ("./Icon-60@2x.png");
				ContentMode = UIViewContentMode.ScaleAspectFill;

				bool hetero = true;

				var tap = new UITapGestureRecognizer(obj => {
					if (hetero){
						this.SetImage ("./LGBTQ.png");
					}else{
						this.SetImage ("./Icon-60@2x.png");
					}
					hetero = !hetero;
				});

				this.AddGestureRecognizer(tap);
			}
		}

		public void SetMainTitle(){

			ClubbyLogo = new UIClubbyLogo ();
			AddSubview (ClubbyLogo);

			TitleLabel.Alpha = 0f;
			TappingEnabled = true;

			foreach (var button in ListButtons) {
				button.Alpha = buttonAlpha;
			}
		}

		public void AddTap(UITapGestureRecognizer tap) {
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

