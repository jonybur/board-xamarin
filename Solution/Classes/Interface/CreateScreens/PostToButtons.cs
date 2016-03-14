using CoreGraphics;
using UIKit;

using Facebook.CoreKit;
using System;
using Facebook.LoginKit;
using Board.Facebook;

using System.Collections.Generic;

namespace Board.Interface.CreateScreens
{
	public class PostToButtons : UIViewController
	{
		const int ButtonHeight = 50;

		SocialMediaButton FBButton, IGButton, TWButton, RSButton;

		public PostToButtons (float positionY)
		{
			FBButton = LoadFacebookButton ();
			IGButton = LoadInstagramButton ();
			TWButton = LoadTwitterButton ();
			RSButton = LoadRSSButton ();

			View.Frame = new CGRect (0, positionY, AppDelegate.ScreenWidth, ButtonHeight * 2);

			View.AddSubviews (FBButton, IGButton, TWButton, RSButton);
		}

		public List<int> GetActiveSocialChannels()
		{
			List<int> socialChannels = new List<int> ();
			if (FBButton.Active)
			{
				socialChannels.Add(0);
			}
			if (IGButton.Active)
			{
				socialChannels.Add(1);
			}
			if (TWButton.Active)
			{
				socialChannels.Add(2);
			}
			if (RSButton.Active)
			{
				socialChannels.Add(3);
			}
			return socialChannels;
		}

		class SocialMediaButton : UIButton
		{
			public UIImageView Logo;
			public UILabel Label;
			public UIColor Color;
			public bool Active;
			public bool ActiveFromImport;

			string ImgPath;

			public void Activate()
			{
				Label.TextColor = Color;
				Logo.TintColor = Color;
				Active = true;
			}

			public void ActivateFromImport()
			{
				Label.TextColor = Color;

				using (UIImage image = UIImage.FromFile ("./boardinterface/screens/share/link/logo.png")) {
					Logo.Image = image.ImageWithRenderingMode (UIImageRenderingMode.AlwaysTemplate);
				}

				Logo.TintColor = Color;
				ActiveFromImport = true;
			}

			public void Deactivate(UIAlertAction action){
				Deactivate ();
			}

			public void Deactivate()
			{
				if (ActiveFromImport) {
					ActiveFromImport = false;
					SetDefaultImage ();
				}

				Label.TextColor = UIColor.FromRGB(34, 36, 39);
				Logo.TintColor = UIColor.FromRGB(165, 167, 169);
				Active = false;
			}

			private void SetDefaultImage()
			{
				using (UIImage image = UIImage.FromFile (ImgPath)) {
					Logo.Image = image.ImageWithRenderingMode (UIImageRenderingMode.AlwaysTemplate);
				}
			}

			public SocialMediaButton(string text, string imgpath, UIColor color, CGRect frame)
			{
				Logo = new UIImageView (new CGRect (10, 10, 30, 30));
				Color = color;
				ImgPath = imgpath;
				SetDefaultImage();

				Label = new UILabel (new CGRect (Logo.Frame.Right, 0, AppDelegate.ScreenWidth / 2 - Logo.Frame.Right, ButtonHeight));
				Label.Text = text;
				Label.Font = AppDelegate.SystemFontOfSize18;
				Label.TextAlignment = UITextAlignment.Center;

				Frame = frame;
				AddSubviews (Logo, Label);
				BackgroundColor = UIColor.FromRGB(255,255,255);

				Deactivate();
			}
		}

		private SocialMediaButton LoadFacebookButton()
		{
			SocialMediaButton smButton = new SocialMediaButton ("Facebook", "./boardinterface/screens/share/facebook/logo.png", 
				UIColor.FromRGB (59, 89, 152), new CGRect(0, 0, AppDelegate.ScreenWidth / 2, ButtonHeight));

			smButton.TouchUpInside += async (sender, e) => {
				
				await FacebookUtils.GetPublishPermission(this, "publish_actions");

				if (!smButton.Active)
				{
					// si no esta activo y tengo los permisos
					if (FacebookUtils.HasPermission("publish_actions"))
					{
						// TODO: ver si funciona para post a pagina
						smButton.Activate();
					}
				} else if (smButton.ActiveFromImport) {
					
					UIAlertController alert = UIAlertController.Create("Warning", "This will unlink your content from Facebook, continue?", UIAlertControllerStyle.Alert);
					alert.AddAction (UIAlertAction.Create ("Cancel", UIAlertActionStyle.Cancel, null));
					alert.AddAction (UIAlertAction.Create ("OK", UIAlertActionStyle.Default, smButton.Deactivate));
					AppDelegate.NavigationController.PresentViewController (alert, true, null);

				} else{
					// desactivo FB y la lista
					smButton.Deactivate();
				}
			};

			return smButton;
		}

		private SocialMediaButton LoadInstagramButton()
		{
			SocialMediaButton smButton = new SocialMediaButton ("Instagram", "./boardinterface/screens/share/instagram/logo.png", 
				UIColor.FromRGB(80, 127, 166), new CGRect(AppDelegate.ScreenWidth / 2, 0, AppDelegate.ScreenWidth / 2, ButtonHeight));

			smButton.TouchUpInside += (sender, e) => {
				if (!smButton.Active)
				{
					smButton.Activate();
				}else{
					smButton.Deactivate();
				}
			};

			return smButton;
		}

		private SocialMediaButton LoadTwitterButton()
		{
			SocialMediaButton smButton = new SocialMediaButton ("Twitter", "./boardinterface/screens/share/twitter/logo.png", 
				UIColor.FromRGB(75, 170, 244), new CGRect(0, ButtonHeight, AppDelegate.ScreenWidth / 2, ButtonHeight));

			smButton.TouchUpInside += (sender, e) => {
				if (!smButton.Active)
				{
					smButton.Activate();
				}else{
					smButton.Deactivate();
				}
			};

			return smButton;
		}

		private SocialMediaButton LoadRSSButton()
		{
			SocialMediaButton smButton = new SocialMediaButton ("RSS Feed", "./boardinterface/screens/share/rss/logo.png", 
				UIColor.FromRGB(255, 112, 0), new CGRect(AppDelegate.ScreenWidth / 2, ButtonHeight, AppDelegate.ScreenWidth / 2, ButtonHeight));

			smButton.TouchUpInside += (sender, e) => {
				if (!smButton.Active)
				{
					smButton.Activate();
				}else{
					smButton.Deactivate();
				}
			};

			return smButton;
		}

		public void ActivateFacebook()
		{
			FBButton.ActivateFromImport ();
		}
	}
}

