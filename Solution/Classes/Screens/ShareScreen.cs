using System;

using CoreGraphics;

using UIKit;

using Facebook.CoreKit;
using Facebook.LoginKit;

using Board.Interface;
using Board.Interface.Buttons;
using Board.Schema;

using Board.Utilities;

namespace Board.Interface
{
	public class ShareScreen : UIViewController
	{
		UIImageView banner;
		UIImageView nextbutton;
		UIScrollView scrollView;
		PlaceholderTextView textview;
		UIImage image;
		float buttonY = 230;
		Content content;

		bool IGActive;
		bool TWActive;
		bool FBActive;
		bool RSSActive;

		string [] publishPermissions = new [] { "publish_actions" };

		public ShareScreen (UIImage _image, Content _content){
			image = _image; content = _content;
		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			NavigationController.NavigationBar.BarStyle = UIBarStyle.Default;
			NavigationController.NavigationBarHidden = true;

			InitializeInterface ();
		}

		private void InitializeInterface()
		{
			LoadContent ();
			LoadBanner ();
			LoadNextButton ();
			LoadTextView ();
			LoadFacebookButton ();
			LoadInstagramButton ();
			LoadTwitterButton ();
			LoadRSSButton ();
		}


		private void LoadFacebookButton()
		{
			string text = "Facebook";
			UIImage image = UIImage.FromFile ("./screens/share/facebook/logo.png");
			image = image.ImageWithRenderingMode (UIImageRenderingMode.AlwaysTemplate);

			UIImageView logoView = new UIImageView (new CGRect (10, 10, 30, 30));
			logoView.Image = image;
			logoView.TintColor = UIColor.FromRGB(165, 167, 169);

			UILabel label = new UILabel (new CGRect (logoView.Frame.Right, 0, AppDelegate.ScreenWidth / 2 - logoView.Frame.Right, 50));
			label.Text = text;
			label.Font = UIFont.SystemFontOfSize (20);
			label.TextColor = UIColor.FromRGB(34, 36, 39);
			label.TextAlignment = UITextAlignment.Center;

			UIButton composite = new UIButton (new CGRect(0, buttonY, AppDelegate.ScreenWidth / 2, 50));
			composite.AddSubviews (logoView, label);
			composite.BackgroundColor = UIColor.FromRGB(255,255,255);

			FBActive = false;

			composite.TouchUpInside += async (object sender, EventArgs e) => {
				// si no tengo permiso
				if (!AccessToken.CurrentAccessToken.HasGranted("publish_actions"))
				{
					// lo pido
					LoginManager manager = new LoginManager ();
					await manager.LogInWithPublishPermissionsAsync (publishPermissions, this);
				}

				if (!FBActive)
				{
					// si no esta activo y tengo los permisos
					if (AccessToken.CurrentAccessToken.HasGranted("publish_actions"))
					{
						// lo prendo
						label.TextColor = UIColor.FromRGB(59, 89, 152);
						logoView.TintColor = UIColor.FromRGB(59, 89, 152);
						FBActive = true;
					}
				}else{
					label.TextColor = UIColor.FromRGB(34, 36, 39);
					logoView.TintColor = UIColor.FromRGB(165, 167, 169);
					FBActive = false;
				}
			};

			scrollView.AddSubview(composite);
		}


		private void LoadInstagramButton()
		{
			string text = "Instagram";
			UIImage image = UIImage.FromFile ("./screens/share/instagram/logo.png");
			image = image.ImageWithRenderingMode (UIImageRenderingMode.AlwaysTemplate);

			UIImageView logoView = new UIImageView (new CGRect (10, 10, 30, 30));
			logoView.Image = image;
			logoView.TintColor = UIColor.FromRGB(165, 167, 169);

			UILabel label = new UILabel (new CGRect (logoView.Frame.Right, 0, AppDelegate.ScreenWidth / 2 - logoView.Frame.Right, 50));
			label.Text = text;
			label.Font = UIFont.SystemFontOfSize (20);
			label.TextColor = UIColor.FromRGB(34, 36, 39);
			label.TextAlignment = UITextAlignment.Center;

			UIButton composite = new UIButton (new CGRect(AppDelegate.ScreenWidth / 2, buttonY, AppDelegate.ScreenWidth / 2, 50));
			composite.AddSubviews (logoView, label);
			composite.BackgroundColor = UIColor.FromRGB(255,255,255);

			IGActive = false;

			composite.TouchUpInside += (object sender, EventArgs e) => {
				
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

			scrollView.AddSubview(composite);
		}

		private void LoadTwitterButton()
		{
			string text = "Twitter";
			UIImage image = UIImage.FromFile ("./screens/share/twitter/logo.png");
			image = image.ImageWithRenderingMode (UIImageRenderingMode.AlwaysTemplate);

			UIImageView logoView = new UIImageView (new CGRect (10, 10, 30, 30));
			logoView.Image = image;
			logoView.TintColor = UIColor.FromRGB(165, 167, 169);

			UILabel label = new UILabel (new CGRect (logoView.Frame.Right, 0, AppDelegate.ScreenWidth / 2 - logoView.Frame.Right, 50));
			label.Text = text;
			label.Font = UIFont.SystemFontOfSize (20);
			label.TextColor = UIColor.FromRGB(34, 36, 39);
			label.TextAlignment = UITextAlignment.Center;

			UIButton composite = new UIButton (new CGRect(0, buttonY + 50, AppDelegate.ScreenWidth / 2, 50));
			composite.AddSubviews (logoView, label);
			composite.BackgroundColor = UIColor.FromRGB(255,255,255);

			TWActive = false;

			composite.TouchUpInside += (object sender, EventArgs e) => {

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

			scrollView.ScrollEnabled = false;
			scrollView.AddSubview(composite);
		}

		private void LoadRSSButton()
		{
			string text = "RSS Feed";
			UIImage image = UIImage.FromFile ("./screens/share/rss/logo.png");
			image = image.ImageWithRenderingMode (UIImageRenderingMode.AlwaysTemplate);

			UIImageView logoView = new UIImageView (new CGRect (10, 10, 30, 30));
			logoView.Image = image;
			logoView.TintColor = UIColor.FromRGB(165, 167, 169);

			UILabel label = new UILabel (new CGRect (logoView.Frame.Right, 0, AppDelegate.ScreenWidth / 2 - logoView.Frame.Right, 50));
			label.Text = text;
			label.Font = UIFont.SystemFontOfSize (20);
			label.TextColor = UIColor.FromRGB(34, 36, 39);
			label.TextAlignment = UITextAlignment.Center;

			UIButton composite = new UIButton (new CGRect(AppDelegate.ScreenWidth / 2, buttonY + 50, AppDelegate.ScreenWidth / 2, 50));
			composite.AddSubviews (logoView, label);
			composite.BackgroundColor = UIColor.FromRGB(255,255,255);

			RSSActive = false;

			composite.TouchUpInside += (object sender, EventArgs e) => {

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

			scrollView.AddSubview(composite);
		}

		private void LoadContent()
		{
			scrollView = new UIScrollView(new CGRect(0, 0, AppDelegate.ScreenWidth, AppDelegate.ScreenHeight));
			scrollView.BackgroundColor = UIColor.FromRGB(249, 250, 249);
			scrollView.ContentSize = new CGSize (AppDelegate.ScreenWidth, AppDelegate.ScreenHeight);

			UITapGestureRecognizer tap = new UITapGestureRecognizer ((UITapGestureRecognizer obj) => {
				textview.ResignFirstResponder();
			});

			scrollView.UserInteractionEnabled = true;
			scrollView.AddGestureRecognizer (tap);

			View.AddSubview (scrollView);
		}

		private void LoadBanner()
		{
			UIImage bannerImage = UIImage.FromFile ("./screens/share/banner/" + AppDelegate.PhoneVersion + ".jpg");

			banner = new UIImageView(new CGRect(0, 0, bannerImage.Size.Width / 2, bannerImage.Size.Height / 2));
			banner.Image = bannerImage;

			UITapGestureRecognizer tap = new UITapGestureRecognizer ((tg) => {
				if (tg.LocationInView(this.View).X < AppDelegate.ScreenWidth / 4){
					NavigationController.PopViewController(true);
				}
			});

			banner.UserInteractionEnabled = true;
			banner.AddGestureRecognizer (tap);
			banner.Alpha = .95f;
			View.AddSubview (banner);
		}


		private void LoadNextButton()
		{
			UIImage mapImage = UIImage.FromFile ("./screens/share/next/" + AppDelegate.PhoneVersion + ".jpg");;

			nextbutton = new UIImageView(new CGRect(0,AppDelegate.ScreenHeight - (mapImage.Size.Height / 2),
				mapImage.Size.Width / 2, mapImage.Size.Height / 2));
			nextbutton.Image = mapImage;

			UITapGestureRecognizer tap = new UITapGestureRecognizer ((tg) => {
				NavigationController.PopViewController(false);

				if (FBActive)
				{
					content.SocialChannel.Add(0);
				}
				if (IGActive)
				{
					content.SocialChannel.Add(1);
				}
				if (TWActive)
				{
					content.SocialChannel.Add(2);
				}
				if (RSSActive)
				{
					content.SocialChannel.Add(3);
				}

				// shows the image preview so that the user can position the image
				BoardInterface.scrollView.AddSubview(Preview.View);

				// switches to confbar
				ButtonInterface.SwitchButtonLayout ((int)ButtonInterface.ButtonLayout.ConfirmationBar);
			});

			nextbutton.UserInteractionEnabled = true;
			nextbutton.AddGestureRecognizer (tap);
			nextbutton.Alpha = .95f;
			View.AddSubview (nextbutton);
		}

		private void LoadTextView()
		{
			float autosize = 50;
			float imgw, imgh;

			float scale = (float)(image.Size.Height / image.Size.Width);
			imgw = autosize;
			imgh = autosize * scale;

			UIImageView imageView = new UIImageView (new CGRect (10, banner.Frame.Height + 10, imgw, imgh));
			imageView.Image = image;

			var frame = new CGRect(70, banner.Frame.Height, 
				AppDelegate.ScreenWidth - 50 - 23,
				140);

			textview = new PlaceholderTextView(frame, "Write a caption...");

			textview.KeyboardType = UIKeyboardType.Default;
			textview.ReturnKeyType = UIReturnKeyType.Default;
			textview.EnablesReturnKeyAutomatically = true;
			textview.BackgroundColor = UIColor.White;
			textview.TextColor = AppDelegate.BoardBlue;
			textview.Font = UIFont.SystemFontOfSize (20);;

			UIImageView colorWhite = CreateColorView (new CGRect (0, 0, AppDelegate.ScreenWidth, frame.Bottom), UIColor.White.CGColor);

			scrollView.AddSubviews (colorWhite, textview, imageView);
		}

		private UIImageView CreateColorView(CGRect frame, CGColor color)
		{
			UIGraphics.BeginImageContext (new CGSize(frame.Size.Width, frame.Size.Height));
			CGContext context = UIGraphics.GetCurrentContext ();

			context.SetFillColor(color);
			context.FillRect(frame);

			UIImage orange = UIGraphics.GetImageFromCurrentImageContext ();
			UIImageView uiv = new UIImageView (orange);
			uiv.Frame = frame;

			return uiv;
		}

	}
}