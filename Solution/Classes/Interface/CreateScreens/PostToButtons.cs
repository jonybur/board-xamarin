using CoreGraphics;
using UIKit;

using Facebook.CoreKit;
using Facebook.LoginKit;

using System.Collections.Generic;

namespace Board.Interface.CreateScreens
{
	public class PostToButtons : UIViewController
	{
		static string [] publishPermissions = { "publish_actions" };

		bool FBActive;
		bool IGActive;
		bool RSSActive;
		bool TWActive;

		const int ButtonHeight = 50;

		public PostToButtons (float positionY, UIViewController parentViewController)
		{
			UIButton fbButton = LoadFacebookButton (parentViewController);
			UIButton inButton = LoadInstagramButton ();
			UIButton twButton = LoadTwitterButton ();
			UIButton rsButton = LoadRSSButton ();

			View.Frame = new CGRect (0, positionY, AppDelegate.ScreenWidth, ButtonHeight * 2);

			View.AddSubviews (fbButton, inButton, twButton, rsButton);
		}

		public List<int> GetActiveSocialChannels()
		{
			List<int> socialChannels = new List<int> ();
			if (FBActive)
			{
				socialChannels.Add(0);
			}
			if (IGActive)
			{
				socialChannels.Add(1);
			}
			if (TWActive)
			{
				socialChannels.Add(2);
			}
			if (RSSActive)
			{
				socialChannels.Add(3);
			}
			return socialChannels;
		}

		private UIButton LoadFacebookButton(UIViewController parentViewController)
		{
			const string text = "Facebook";

			UIImageView logoView = new UIImageView (new CGRect (10, 10, 30, 30));

			using (UIImage image = UIImage.FromFile ("./screens/share/facebook/logo.png")) {
				logoView.Image = image.ImageWithRenderingMode (UIImageRenderingMode.AlwaysTemplate);;
			}

			logoView.TintColor = UIColor.FromRGB(165, 167, 169);

			UILabel label = new UILabel (new CGRect (logoView.Frame.Right, 0, AppDelegate.ScreenWidth / 2 - logoView.Frame.Right, ButtonHeight));
			label.Text = text;
			label.Font = UIFont.SystemFontOfSize (18);
			label.TextColor = UIColor.FromRGB(34, 36, 39);
			label.TextAlignment = UITextAlignment.Center;

			UIButton composite = new UIButton (new CGRect(0, 0, AppDelegate.ScreenWidth / 2, ButtonHeight));
			composite.AddSubviews (logoView, label);
			composite.BackgroundColor = UIColor.FromRGB(255,255,255);

			FBActive = false;

			composite.TouchUpInside += async (sender, e) => {
				// si no tengo permiso
				if (!AccessToken.CurrentAccessToken.HasGranted(publishPermissions[0]))
				{
					// lo pido
					LoginManager manager = new LoginManager ();
					await manager.LogInWithPublishPermissionsAsync (publishPermissions, parentViewController);
				}

				if (!FBActive)
				{
					// si no esta activo y tengo los permisos
					if (AccessToken.CurrentAccessToken.HasGranted(publishPermissions[0]))
					{
						// TODO: ver si funciona para post a pagina

						label.TextColor = UIColor.FromRGB(59, 89, 152);
						logoView.TintColor = UIColor.FromRGB(59, 89, 152);
						FBActive = true;
					}
				}else{
					// desactivo FB y la lista

					label.TextColor = UIColor.FromRGB(34, 36, 39);
					logoView.TintColor = UIColor.FromRGB(165, 167, 169);
					FBActive = false;
				}
			};

			return composite;
		}

		private UIButton LoadInstagramButton()
		{
			const string text = "Instagram";

			UIImageView logoView = new UIImageView (new CGRect (10, 10, 30, 30));

			using (UIImage image = UIImage.FromFile ("./screens/share/instagram/logo.png")) {
				logoView.Image = image.ImageWithRenderingMode (UIImageRenderingMode.AlwaysTemplate);;
			}

			logoView.TintColor = UIColor.FromRGB(165, 167, 169);

			UILabel label = new UILabel (new CGRect (logoView.Frame.Right, 0, AppDelegate.ScreenWidth / 2 - logoView.Frame.Right, ButtonHeight));
			label.Text = text;
			label.Font = UIFont.SystemFontOfSize (18);
			label.TextColor = UIColor.FromRGB(34, 36, 39);
			label.TextAlignment = UITextAlignment.Center;

			UIButton composite = new UIButton (new CGRect(AppDelegate.ScreenWidth / 2, 0, AppDelegate.ScreenWidth / 2, ButtonHeight));
			composite.AddSubviews (logoView, label);
			composite.BackgroundColor = UIColor.FromRGB(255,255,255);

			IGActive = false;

			composite.TouchUpInside += (sender, e) => {

				if (!IGActive)
				{
					label.TextColor = UIColor.FromRGB(80, 127, 166);
					logoView.TintColor = UIColor.FromRGB(80, 127, 166);
					IGActive = true;
				}else{
					label.TextColor = UIColor.FromRGB(34, 36, 39);
					logoView.TintColor = UIColor.FromRGB(165, 167, 169);
					IGActive = false;
				}
			};

			return composite;
		}

		private UIButton LoadTwitterButton()
		{
			const string text = "Twitter";

			UIImageView logoView = new UIImageView (new CGRect (10, 10, 30, 30));

			using (UIImage image = UIImage.FromFile ("./screens/share/twitter/logo.png")) {
				logoView.Image = image.ImageWithRenderingMode (UIImageRenderingMode.AlwaysTemplate);
			}

			logoView.TintColor = UIColor.FromRGB(165, 167, 169);

			UILabel label = new UILabel (new CGRect (logoView.Frame.Right, 0, AppDelegate.ScreenWidth / 2 - logoView.Frame.Right, ButtonHeight));
			label.Text = text;
			label.Font = UIFont.SystemFontOfSize (18);
			label.TextColor = UIColor.FromRGB(34, 36, 39);
			label.TextAlignment = UITextAlignment.Center;

			UIButton composite = new UIButton (new CGRect(0, ButtonHeight, AppDelegate.ScreenWidth / 2, ButtonHeight));
			composite.AddSubviews (logoView, label);
			composite.BackgroundColor = UIColor.FromRGB(255,255,255);

			TWActive = false;

			composite.TouchUpInside += (sender, e) => {

				if (!TWActive)
				{
					label.TextColor = UIColor.FromRGB(75, 170, 244);
					logoView.TintColor = UIColor.FromRGB(75, 170, 244);
					TWActive = true;
				}else{
					label.TextColor = UIColor.FromRGB(34, 36, 39);
					logoView.TintColor = UIColor.FromRGB(165, 167, 169);
					TWActive = false;
				}
			};

			return composite;
		}

		private UIButton LoadRSSButton()
		{
			const string text = "RSS Feed";

			UIImageView logoView = new UIImageView (new CGRect (10, 10, 30, 30));

			using (UIImage image = UIImage.FromFile ("./screens/share/rss/logo.png")) {
				logoView.Image = image.ImageWithRenderingMode (UIImageRenderingMode.AlwaysTemplate);
			}

			logoView.TintColor = UIColor.FromRGB(165, 167, 169);

			UILabel label = new UILabel (new CGRect (logoView.Frame.Right, 0, AppDelegate.ScreenWidth / 2 - logoView.Frame.Right, ButtonHeight));
			label.Text = text;
			label.Font = UIFont.SystemFontOfSize (18);
			label.TextColor = UIColor.FromRGB(34, 36, 39);
			label.TextAlignment = UITextAlignment.Center;

			UIButton composite = new UIButton (new CGRect(AppDelegate.ScreenWidth / 2, ButtonHeight, AppDelegate.ScreenWidth / 2, ButtonHeight));
			composite.AddSubviews (logoView, label);
			composite.BackgroundColor = UIColor.FromRGB(255,255,255);

			RSSActive = false;

			composite.TouchUpInside += (sender, e) => {
				if (!RSSActive)
				{
					label.TextColor = UIColor.FromRGB(255, 112, 0);
					logoView.TintColor = UIColor.FromRGB(255, 112, 0);
					RSSActive = true;
				}else{
					label.TextColor = UIColor.FromRGB(34, 36, 39);
					logoView.TintColor = UIColor.FromRGB(165, 167, 169);
					RSSActive = false;
				}
			};

			return composite;
		}
	}
}

