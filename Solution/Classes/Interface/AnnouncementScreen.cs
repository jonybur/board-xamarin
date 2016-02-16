﻿using System;

using CoreGraphics;
using Foundation;
using UIKit;

using System.Collections.Generic;

using Facebook.CoreKit;
using Facebook.LoginKit;

using Board.Interface;
using Board.Interface.Buttons;
using Board.Utilities;
using Board.Schema;

namespace Board.Interface
{
	public class AnnouncementScreen : UIViewController
	{
		UIImageView banner;
		UIImageView nextbutton;
		UIScrollView content;
		UILabel instructionsLabel;
		PlaceholderTextView textview;
		List<UIButton> fbPageButtons;

		bool FBActive;
		bool IGActive;
		bool RSSActive;
		bool TWActive;

		float listStartPositionY;
		float positionY;

		string [] publishPermissions = new [] { "publish_actions" };
		string [] readPermissions = new [] { "pages_show_list" };

		public AnnouncementScreen (){
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			NavigationController.NavigationBar.BarStyle = UIBarStyle.Default;
			NavigationController.NavigationBarHidden = true;

			InitializeInterface ();
		}

		private async void InitializeInterface()
		{
			LoadContent ();
			LoadBanner ();
			LoadNextButton ();
			LoadTextView ();

			positionY = 230;

			LoadFacebookButton ();
			LoadInstagramButton ();

			positionY += 50;

			LoadTwitterButton ();
			LoadRSSButton ();

			positionY += 145;
			listStartPositionY = positionY;
		}

		private async System.Threading.Tasks.Task CheckPageReadPermissions()
		{
			if (!AccessToken.CurrentAccessToken.HasGranted(readPermissions[0]))
			{
				// lo pido
				LoginManager manager = new LoginManager ();
				await manager.LogInWithReadPermissionsAsync (readPermissions, this);
			}

			Facebook.CoreKit.GraphRequest graph = new GraphRequest ("me/accounts", null, AccessToken.CurrentAccessToken.TokenString, "v2.5", "GET");
			graph.Start (LoadList);
		}

		private void LoadList(Facebook.CoreKit.GraphRequestConnection connection, Foundation.NSObject obj, Foundation.NSError err)
		{
			List<string> lstNames = NSObjectToString ("data.name", obj);
			List<string> lstCategories = NSObjectToString ("data.category", obj);

			content.ContentSize = new CGSize (AppDelegate.ScreenWidth, 60 * ((int)lstNames.Count + 1) + banner.Frame.Height + nextbutton.Frame.Height + lstNames.Count + positionY + 1);

			UIFont font = UIFont.FromName ("narwhal-bold", 20);
			instructionsLabel = new UILabel (new CGRect(20, positionY, AppDelegate.ScreenWidth - 40, 20));
			instructionsLabel.Text = "SELECT FACEBOOK PAGES";
			instructionsLabel.Font = font;
			instructionsLabel.TextColor = AppDelegate.BoardOrange;
			instructionsLabel.AdjustsFontSizeToFitWidth = true;
			content.AddSubview (instructionsLabel);

			positionY += (float)instructionsLabel.Frame.Height + 10;
			fbPageButtons = new List<UIButton> ();

			UIButton nameButton = ProfileButton(positionY, Profile.CurrentProfile.Name + "'s Profile");
			fbPageButtons.Add (nameButton);
			content.AddSubview (nameButton);
			positionY += (float)nameButton.Frame.Height + 1;
			int i = 0;
			foreach (string name in lstNames) {
				UIButton pageButton = PageButton (positionY, name, lstCategories[i]);
				fbPageButtons.Add (pageButton);
				i++;
				positionY += (float)pageButton.Frame.Height + 1;
				content.AddSubview (pageButton);
			}
		}

		private List<string> NSObjectToString(string fetch, NSObject obj)
		{
			NSString nsString = new NSString (fetch);

			NSArray array = (NSArray)obj.ValueForKeyPath (nsString);
			List<string> list = new List<string> ();

			for (int i = 0; i < (int)array.Count; i++) {
				var item = array.GetItem<NSObject> ((nuint)i);
				list.Add(item.ToString());
			}

			return list;
		}

		private UIButton ProfileButton(float yPosition, string name)
		{
			UIButton pageButton = new UIButton (new CGRect (0, yPosition, AppDelegate.ScreenWidth, 60));
			pageButton.BackgroundColor = UIColor.White;	

			UIFont nameFont = UIFont.SystemFontOfSize (20);
			UILabel nameLabel = new UILabel (new CGRect (40, 20, AppDelegate.ScreenWidth - 50, 20));
			nameLabel.Font = nameFont;
			nameLabel.Text = name;
			nameLabel.AdjustsFontSizeToFitWidth = true;
			nameLabel.TextColor = AppDelegate.BoardBlue;

			bool pressed = false;

			pageButton.TouchUpInside += (object sender, EventArgs e) => {
				if (!pressed)
				{
					pressed = true;
					pageButton.BackgroundColor = AppDelegate.BoardLightBlue;
					nameLabel.TextColor = UIColor.White;

					/*Thread thread = new Thread(new ThreadStart(PopOut));
					thread.Start();*/
				} else {
					pressed = false;
					pageButton.BackgroundColor = UIColor.White;
					nameLabel.TextColor = AppDelegate.BoardBlue;
				}				
			};

			pageButton.UserInteractionEnabled = true;
			pageButton.AddSubviews (nameLabel);

			return pageButton;
		}

		private UIButton PageButton(float yPosition, string name, string category)
		{
			UIButton pageButton = new UIButton (new CGRect (0, yPosition, AppDelegate.ScreenWidth, 60));
			pageButton.BackgroundColor = UIColor.White;

			UIFont nameFont = UIFont.SystemFontOfSize (20);
			UILabel nameLabel = new UILabel (new CGRect (40, 10, AppDelegate.ScreenWidth - 50, 20));
			nameLabel.Font = nameFont;
			nameLabel.Text = name;
			nameLabel.AdjustsFontSizeToFitWidth = true;
			nameLabel.TextColor = AppDelegate.BoardBlue;

			UIFont categoryFont = UIFont.SystemFontOfSize(14);
			UILabel categoryLabel = new UILabel (new CGRect (40, nameLabel.Frame.Bottom + 5, AppDelegate.ScreenWidth - 50, 16));
			categoryLabel.Font = categoryFont;
			categoryLabel.Text = category;
			categoryLabel.AdjustsFontSizeToFitWidth = true;
			categoryLabel.TextColor = AppDelegate.BoardBlue;

			bool pressed = false;

			pageButton.TouchUpInside += (object sender, EventArgs e) => {
				if (!pressed)
				{
					pressed = true;
					pageButton.BackgroundColor = AppDelegate.BoardLightBlue;
					nameLabel.TextColor = UIColor.White;
					categoryLabel.TextColor = UIColor.White;

					/*Thread thread = new Thread(new ThreadStart(PopOut));
					thread.Start();*/
				} else {
					pressed = false;
					pageButton.BackgroundColor = UIColor.White;
					nameLabel.TextColor = AppDelegate.BoardBlue;
					categoryLabel.TextColor = AppDelegate.BoardBlue;
				}				
			};

			pageButton.UserInteractionEnabled = true;
			pageButton.AddSubviews (nameLabel, categoryLabel);

			return pageButton;
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

			UIButton composite = new UIButton (new CGRect(0, positionY, AppDelegate.ScreenWidth / 2, 50));
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

						// obtengo lista de paginas
						await CheckPageReadPermissions();
					}
				}else{
					// desactivo FB y la lista

					label.TextColor = UIColor.FromRGB(34, 36, 39);
					logoView.TintColor = UIColor.FromRGB(165, 167, 169);
					FBActive = false;
					instructionsLabel.RemoveFromSuperview();

					foreach(UIButton uib in fbPageButtons)
					{
						uib.RemoveFromSuperview();
						positionY = listStartPositionY;
						content.ContentSize = new CGSize (AppDelegate.ScreenWidth, banner.Frame.Height + positionY);
					}
				}
			};

			content.AddSubview(composite);
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

			UIButton composite = new UIButton (new CGRect(AppDelegate.ScreenWidth / 2, positionY, AppDelegate.ScreenWidth / 2, 50));
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

			content.AddSubview(composite);
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

			UIButton composite = new UIButton (new CGRect(0, positionY, AppDelegate.ScreenWidth / 2, 50));
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

			content.AddSubview(composite);
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

			UIButton composite = new UIButton (new CGRect(AppDelegate.ScreenWidth / 2, positionY, AppDelegate.ScreenWidth / 2, 50));
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

			content.AddSubview(composite);
		}

		private void LoadContent()
		{
			content = new UIScrollView(new CGRect(0, 0, AppDelegate.ScreenWidth, AppDelegate.ScreenHeight));
			content.BackgroundColor = UIColor.FromRGB(249, 250, 249);

			UITapGestureRecognizer tap = new UITapGestureRecognizer ((UITapGestureRecognizer obj) => {
				textview.ResignFirstResponder();
			});

			content.UserInteractionEnabled = true;
			content.AddGestureRecognizer (tap);

			View.AddSubview (content);
		}

		private void LoadBanner()
		{
			UIImage bannerImage = UIImage.FromFile ("./screens/announcement/banner/" + AppDelegate.PhoneVersion + ".jpg");

			banner = new UIImageView(new CGRect(0, 0, bannerImage.Size.Width / 2, bannerImage.Size.Height / 2));
			banner.Image = bannerImage;

			UITapGestureRecognizer tap = new UITapGestureRecognizer ((tg) => {
				if (tg.LocationInView(this.View).X < AppDelegate.ScreenWidth / 4){

					if (textview.Text.Length > 0 && !textview.IsPlaceHolder)
					{
						var alert = new UIAlertView ("Discard Announcement?", "Your message will be discarded", null, "Keep", new string[] {"Discard"});
						alert.Clicked += (s, b) => {
							if (b.ButtonIndex == 0)
							{ return; }

							NavigationController.PopViewController(true);
						};
						alert.Show();
					} 
					else 
					{
						NavigationController.PopViewController(true);
					}

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

				if (textview.IsPlaceHolder || textview.Text.Length == 0)
				{
					UIAlertController alert = UIAlertController.Create("Can't create announcement", "Please write a caption", UIAlertControllerStyle.Alert);

					alert.AddAction (UIAlertAction.Create ("OK", UIAlertActionStyle.Default, null));

					NavigationController.PresentViewController (alert, true, null);

					return;
				}

				Announcement ann = new Announcement();

				ann.SocialChannel = new List<int>();

				if (FBActive)
				{
					ann.SocialChannel.Add(0);
				}
				if (IGActive)
				{
					ann.SocialChannel.Add(1);
				}
				if (TWActive)
				{
					ann.SocialChannel.Add(2);
				}
				if (RSSActive)
				{
					ann.SocialChannel.Add(3);
				}

				ann.Text = textview.Text;

				Preview.Initialize(ann, NavigationController);

				// shows the image preview so that the user can position the image
				BoardInterface.scrollView.AddSubview(Preview.View);

				// switches to confbar
				ButtonInterface.SwitchButtonLayout ((int)ButtonInterface.ButtonLayout.ConfirmationBar);

				NavigationController.PopViewController(false);
			});

			nextbutton.UserInteractionEnabled = true;
			nextbutton.AddGestureRecognizer (tap);
			nextbutton.Alpha = .95f;
			View.AddSubview (nextbutton);
		}

		private void LoadTextView()
		{
			var frame = new CGRect(10, banner.Frame.Height, 
				AppDelegate.ScreenWidth - 50 - 23,
				140);

			textview = new PlaceholderTextView(frame, "Write a caption...");

			textview.KeyboardType = UIKeyboardType.Default;
			textview.ReturnKeyType = UIReturnKeyType.Default;
			textview.EnablesReturnKeyAutomatically = true;
			textview.BackgroundColor = UIColor.White;
			textview.TextColor = AppDelegate.BoardBlue;
			textview.Font = UIFont.SystemFontOfSize (20);

			UIImageView colorWhite = CreateColorView (new CGRect (0, 0, AppDelegate.ScreenWidth, frame.Bottom), UIColor.White.CGColor);

			content.AddSubviews (colorWhite, textview);
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