using System;
using System.Drawing;
using System.Linq;

using CoreGraphics;
using Foundation;
using UIKit;

using CoreAnimation;
using CoreText;

using System.Net.Http;

using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using Facebook.CoreKit;
using Facebook.LoginKit;

using Google.Maps;

namespace Solution
{
	public class PageSelectorScreen : UIViewController
	{
		UIImageView banner;
		UIScrollView scrollView;

		string [] fbPermissions = new [] { "pages_show_list" };

		public PageSelectorScreen ()
		{
		}

		public override async void ViewDidLoad ()
		{
			LoadBanner ();

			View.BackgroundColor = UIColor.White;

			if (!AccessToken.CurrentAccessToken.HasGranted(fbPermissions[0]))
			{
				// lo pido
				LoginManager manager = new LoginManager ();
				await manager.LogInWithReadPermissionsAsync (fbPermissions, this);
			}

			Facebook.CoreKit.GraphRequest graph = new GraphRequest ("me/accounts", null, AccessToken.CurrentAccessToken.TokenString, "v2.5", "GET");
			graph.Start (LoadList);
		}

		private void LoadList(Facebook.CoreKit.GraphRequestConnection connection, Foundation.NSObject obj, Foundation.NSError err)
		{
			scrollView = new UIScrollView (new CGRect (0, 0, AppDelegate.ScreenWidth, AppDelegate.ScreenHeight));

			List<string> lstNames = NSObjectToString ("data.name", obj);
			List<string> lstCategories = NSObjectToString ("data.category", obj);

			scrollView.ContentSize = new CGSize (AppDelegate.ScreenWidth, 80 * (int)lstNames.Count + banner.Frame.Height + lstNames.Count);

			float yPosition = (float)banner.Frame.Height;
			int i = 0;
			foreach (string name in lstNames) {
				UIButton pageButton = PageButton (yPosition, name, lstCategories[i]);
				i++;
				yPosition += (float)pageButton.Frame.Height + 1;
				scrollView.AddSubview (pageButton);
			}

			View.AddSubview (scrollView);

			View.AddSubview (banner);
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

		private UIButton PageButton(float yPosition, string name, string category)
		{
			UIButton pageButton = new UIButton (new CGRect (0, yPosition, AppDelegate.ScreenWidth, 80));
			pageButton.BackgroundColor = UIColor.FromRGB (250, 250, 250);	

			UIFont nameFont = UIFont.SystemFontOfSize (20);
			UILabel nameLabel = new UILabel (new CGRect (40, 20, AppDelegate.ScreenWidth - 50, 20));
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

			pageButton.TouchUpInside += (object sender, EventArgs e) => {
				if (!pressed)
				{
					pressed = true;
					pageButton.BackgroundColor = AppDelegate.BoardLightBlue;
					nameLabel.TextColor = UIColor.White;
					categoryLabel.TextColor = UIColor.White;

					Thread thread = new Thread(new ThreadStart(PopOut));
					thread.Start();
				}
			};

			pageButton.UserInteractionEnabled = true;
			pageButton.AddSubviews (nameLabel, categoryLabel);

			return pageButton;
		}

		bool pressed = false;

		private void PopOut()
		{
			Thread.Sleep (300);
			InvokeOnMainThread(() => NavigationController.PopViewController(true));
		}

		private void LoadBanner()
		{
			UIImage bannerImage = UIImage.FromFile ("./screens/pageselector/banner/" + AppDelegate.PhoneVersion + ".jpg");

			banner = new UIImageView(new CGRect(0,0, bannerImage.Size.Width / 2, bannerImage.Size.Height / 2));
			banner.Image = bannerImage;

			UITapGestureRecognizer tap = new UITapGestureRecognizer ((tg) => {
				if (tg.LocationInView(this.View).X < AppDelegate.ScreenWidth / 4){
					NavigationController.PopViewController(false);
				}
			});

			banner.UserInteractionEnabled = true;
			banner.AddGestureRecognizer (tap);
			banner.Alpha = .95f;
		}
	}
}

