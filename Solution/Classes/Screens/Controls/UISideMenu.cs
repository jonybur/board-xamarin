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
		ProfilePictureView profileView;
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

			CABasicAnimation animation =  new CABasicAnimation();
			animation.KeyPath = "position.x";
			animation.From = new NSNumber(0);
			animation.TimingFunction = CAMediaTimingFunction.FromName(CAMediaTimingFunction.EaseInEaseOut);
			animation.To = new NSNumber(View.Frame.Width/2);
			animation.Duration = .2f;
			View.Layer.AddAnimation(animation, "basic");

			using (UIImage bannerImage = UIImage.FromFile ("./screens/" + FromScreen + "/sidemenu/" + AppDelegate.PhoneVersion + ".png")) {
				sidemenu = new UIImageView (new CGRect (0, 0, bannerImage.Size.Width / 2, bannerImage.Size.Height / 2));
				sidemenu.Image = bannerImage;
			}

			float[] buttonLocations = new float[4];
			if (AppDelegate.PhoneVersion == "6") {
				buttonLocations [0] = 350;
				buttonLocations [1] = 440;
				buttonLocations [2] = 525;
				buttonLocations [3] = 605;
			} else {
				buttonLocations [0] = 390;
				buttonLocations [1] = 470;
				buttonLocations [2] = 550;
				buttonLocations [3] = 630;
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

			profileView = new ProfilePictureView (new CGRect (11, 78, 149, 149));
			profileView.ProfileId = Profile.CurrentProfile.UserID;

			View.AddSubviews (profileView, sidemenu);

			UIFont namefont = AppDelegate.Narwhal20;
			UIFont lastnamefont = AppDelegate.Narwhal24;

			UILabel name = new UILabel (new CGRect(10, profileView.Frame.Bottom + 15, sidemenu.Frame.Width - 20, 20));
			name.Font = namefont;
			name.Text = Profile.CurrentProfile.FirstName;
			name.TextColor = UIColor.White;
			name.TextAlignment = UITextAlignment.Center;
			name.AdjustsFontSizeToFitWidth = true;
			sidemenu.AddSubview (name);

			UILabel lastname = new UILabel (new CGRect(10, name.Frame.Bottom + 3, sidemenu.Frame.Width - 20, 24));
			lastname.Font = lastnamefont;
			lastname.AdjustsFontSizeToFitWidth = true;
			lastname.Text = Profile.CurrentProfile.LastName;
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
			View.RemoveGestureRecognizer (SideMenuTap);
			MemoryUtility.ReleaseUIViewWithChildren (this.View, true); 
		}

	}
}

