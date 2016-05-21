using UIKit;
using Board.Utilities;
using CoreAnimation;
using CoreGraphics;
using Facebook.CoreKit;
using System;
using Foundation;

namespace Board.Screens.Controls
{
	public class UISideMenu : UIViewController
	{
		UIImageView sidemenu;
		UITapGestureRecognizer SideMenuTap;
		UIImageView profileView;
		string FromScreen;

		public UISideMenu(string fromScreen)
		{
			FromScreen = fromScreen;
		}

		public override void ViewDidLoad()
		{
			View.Layer.ShadowOffset = new CGSize (1, 0);
			View.Layer.ShadowRadius = 5f;
			View.Layer.ShadowColor = UIColor.Black.CGColor;
			View.Layer.ShadowOpacity = .75f;
			View.Alpha = .95f;

			CABasicAnimation animation =  new CABasicAnimation();
			animation.KeyPath = "position.x";
			animation.From = new NSNumber(0);
			animation.TimingFunction = CAMediaTimingFunction.FromName(CAMediaTimingFunction.EaseInEaseOut);
			animation.To = new NSNumber(View.Frame.Width/2);
			animation.Duration = .2f;
			View.Layer.AddAnimation(animation, "basic");

			using (UIImage bannerImage = UIImage.FromFile ("./screens/sidemenus/"+ AppDelegate.PhoneVersion + "/" + FromScreen + ".png")) {
				sidemenu = new UIImageView ();
				sidemenu.Frame = new CGRect (0, 0, bannerImage.Size.Width / 2, bannerImage.Size.Height / 2);
				sidemenu.Image = bannerImage;
			}

			float[] buttonLocations = new float[4];
			if (AppDelegate.PhoneVersion == "6") {
				buttonLocations [0] = 350;
				buttonLocations [1] = 440;
				buttonLocations [2] = 525;
				buttonLocations [3] = 605;
			} else if (AppDelegate.PhoneVersion == "6plus") {
				buttonLocations [0] = 390;
				buttonLocations [1] = 470;
				buttonLocations [2] = 550;
				buttonLocations [3] = 630;
			} else if (AppDelegate.PhoneVersion == "4") {
				buttonLocations [0] = 245;
				buttonLocations [1] = 310;
				buttonLocations [2] = 370;
				buttonLocations [3] = 435;
			} else if (AppDelegate.PhoneVersion == "5") {
				buttonLocations [0] = 285;
				buttonLocations [1] = 360;
				buttonLocations [2] = 436;
				buttonLocations [3] = 518;
			}

			SideMenuTap = new UITapGestureRecognizer (tg => {
				if (tg.LocationInView(this.View).X < sidemenu.Frame.Width)
				{
					if (tg.LocationInView(this.View).Y > buttonLocations[0]-35 && tg.LocationInView(this.View).Y < buttonLocations[0]+35 ){
						if (FromScreen == "main") {
							SwitchScreens(AppDelegate.containerScreen.ChangeToBusinessScreen);
						} else {
							SwitchScreens(AppDelegate.containerScreen.ChangeToMainScreen);
						}
					}
					else if (tg.LocationInView(this.View).Y > buttonLocations[1]-35 && tg.LocationInView(this.View).Y < buttonLocations[1]+35){
						if (FromScreen == "main" || FromScreen == "business") {
							SwitchScreens(AppDelegate.containerScreen.ChangeToSettingsScreen);
						} else {
							SwitchScreens(AppDelegate.containerScreen.ChangeToBusinessScreen);
						}
					}
					else if (tg.LocationInView(this.View).Y > buttonLocations[2]-35 && tg.LocationInView(this.View).Y < buttonLocations[2]+35){
						if (FromScreen == "support") {
							SwitchScreens(AppDelegate.containerScreen.ChangeToSettingsScreen);
						} else {
							SwitchScreens(AppDelegate.containerScreen.ChangeToSupportScreen);
						}
					}
					else if (tg.LocationInView(this.View).Y > buttonLocations[3]-35 && tg.LocationInView(this.View).Y < buttonLocations[3]+35){
						if (FromScreen == "invite") {
							SwitchScreens(AppDelegate.containerScreen.ChangeToSettingsScreen);
						} else {
							SwitchScreens(AppDelegate.containerScreen.ChangeToInviteScreen);
						}
					}
				}else{
					SwitchScreens(null);
				}
			});

			sidemenu.UserInteractionEnabled = true;

			float yposition, imagesize;
			UIFont namefont, lastnamefont;
			int nameYPosition;

			if (AppDelegate.PhoneVersion == "4") {
				yposition = 35;
				imagesize = 110;

				namefont = AppDelegate.Narwhal18;
				lastnamefont = AppDelegate.Narwhal20;
				nameYPosition = 10;
			} else {
				if (AppDelegate.PhoneVersion == "5") {
					yposition = 30;
					imagesize = 135;
				} else {
					yposition = 60;
					imagesize = 150;
				}

				namefont = AppDelegate.Narwhal20;
				lastnamefont = AppDelegate.Narwhal24;

				nameYPosition = 15;
			}

			profileView = new UIImageView (new CGRect (0, 0, imagesize, imagesize));
			profileView.Center = new CGPoint (sidemenu.Frame.Width / 2, yposition + imagesize / 2);
			profileView.Image = AppDelegate.BoardUser.ProfilePictureUIImage;
			profileView.Layer.CornerRadius = profileView.Frame.Width / 2;
			profileView.Layer.MasksToBounds = true;
			View.AddSubviews (sidemenu, profileView);


			// TODO: usar user en lugar de profile
			UILabel name = new UILabel (new CGRect(10, profileView.Frame.Bottom + nameYPosition, sidemenu.Frame.Width - 20, 20));
			name.Font = namefont;
			name.Text = AppDelegate.BoardUser.FirstName;
			name.TextColor = UIColor.White;
			name.TextAlignment = UITextAlignment.Center;
			name.AdjustsFontSizeToFitWidth = true;
			sidemenu.AddSubview (name);

			UILabel lastname = new UILabel (new CGRect(10, name.Frame.Bottom + 3, sidemenu.Frame.Width - 20, 24));
			lastname.Font = lastnamefont;
			lastname.AdjustsFontSizeToFitWidth = true;
			lastname.Text = AppDelegate.BoardUser.LastName;
			lastname.TextColor = UIColor.White;
			lastname.TextAlignment = UITextAlignment.Center;
			sidemenu.AddSubview (lastname);
		}

		private void SwitchScreens(Action action)
		{
			CATransaction.Begin();

			CATransaction.CompletionBlock = delegate {
				if (action != null)
				{
					action();
				}
				View.RemoveFromSuperview();
			};

			View.Frame = new CGRect(- View.Frame.Width, View.Frame.Y, View.Frame.Width, View.Frame.Height);
			CreateCloseAnimation();
			CATransaction.Commit();
		}

		private void CreateCloseAnimation()
		{
			View.Frame = new CGRect(- View.Frame.Width, View.Frame.Y, View.Frame.Width, View.Frame.Height);
			CABasicAnimation closeanimation =  new CABasicAnimation();
			closeanimation.TimingFunction = CAMediaTimingFunction.FromName(CAMediaTimingFunction.EaseInEaseOut);
			closeanimation.KeyPath = "position.x";
			closeanimation.From = new NSNumber(View.Frame.Width/2);
			closeanimation.To = new NSNumber(0);
			closeanimation.Duration = .2f;
			View.Layer.AddAnimation(closeanimation, "basic");
		}

		public override void ViewDidAppear(bool animated)
		{	
			View.AddGestureRecognizer (SideMenuTap);
		}

		public override void ViewDidDisappear(bool animated)
		{
			profileView.Image = null;
			View.RemoveGestureRecognizer (SideMenuTap);
			MemoryUtility.ReleaseUIViewWithChildren (View); 
		}

	}
}

