using System.Collections.Generic;

using System.Threading;
using Board.Facebook;
using System;
using CoreGraphics;

using Facebook.CoreKit;
using Facebook.LoginKit;
using UIKit;
using Board.Screens.Controls;

namespace Board.Interface
{
	public class PageSelectorScreen : UIViewController
	{
		UIImageView banner;
		UIScrollView ScrollView;
		List<ScreenButton> Buttons;

		bool pressed;

		public override void ViewDidLoad ()
		{
			Buttons = new List<ScreenButton> ();

			View.BackgroundColor = UIColor.White;

			LoadBanner ();
		}

		public override async void ViewDidAppear(bool animated)
		{
			string [] permission = { "pages_show_list" };

			if (!AccessToken.CurrentAccessToken.HasGranted(permission[0]))
			{
				// lo pido
				LoginManager manager = new LoginManager ();
				await manager.LogInWithReadPermissionsAsync (permission, this);
			}

			FacebookUtils.MakeGraphRequest ("me", "accounts", Completion);
		}

		public override void ViewDidDisappear(bool animated)
		{
			UnsuscribeToEvents ();
		}

		private void SuscribeToEvents()
		{
			foreach (ScreenButton sb in Buttons) {
				sb.SuscribeToEvent ();
			}
		}

		private void UnsuscribeToEvents()
		{
			foreach (ScreenButton sb in Buttons) {
				sb.UnsuscribeToEvent ();
			}
		}

		private void Completion(object obj, EventArgs e)
		{
			LoadPages ();
			SuscribeToEvents ();
		}

		private void LoadPages()
		{
			ScrollView = new UIScrollView (new CGRect (0, 0, AppDelegate.ScreenWidth, AppDelegate.ScreenHeight));
			ScrollView.ContentSize = new CGSize (AppDelegate.ScreenWidth, 80 * FacebookUtils.ElementList.Count + banner.Frame.Height + FacebookUtils.ElementList.Count + 1);
			float yPosition = (float)banner.Frame.Height;

			int i = 0;
			foreach (FacebookElement page in FacebookUtils.ElementList) {
				TwoLinesScreenButton pageButton = PageButton (yPosition, (FacebookPage)page);
				i++;
				yPosition += (float)pageButton.Frame.Height + 1;
				Buttons.Add (pageButton);
				ScrollView.AddSubview (pageButton);
			}

			OneLineScreenButton unsyncButton = CreateUnsyncButton (yPosition);
			if (BoardInterface.board.FBPage == null) {
				unsyncButton.Alpha = 0f;
			}
			Buttons.Add (unsyncButton);
			ScrollView.AddSubview (unsyncButton);

			View.AddSubview (ScrollView);
			View.AddSubview (banner);
		}


		private TwoLinesScreenButton PageButton(float yPosition, FacebookPage page)
		{
			TwoLinesScreenButton pageButton = new TwoLinesScreenButton (yPosition);
			pageButton.SetLabels (page.Name, page.Category);
			pageButton.SetUnpressedColors ();

			pageButton.TapEvent += (sender, e) => {
				if (!pressed)
				{
					pressed = true;
					pageButton.SetPressedColors();

					Thread thread = new Thread(new ThreadStart(PopOut));
					thread.Start();

					BoardInterface.board.FBPage = page;
				}
			};

			return pageButton;
		}

		private OneLineScreenButton CreateUnsyncButton(float yPosition)
		{
			OneLineScreenButton unsyncButton = new OneLineScreenButton (yPosition);
			unsyncButton.SetLabel("Unsync");
			unsyncButton.SetUnpressedColors ();

			unsyncButton.TapEvent += (sender, e) => {
				if (!pressed)
				{
					pressed = true;
					unsyncButton.SetPressedColors();

					Thread thread = new Thread(new ThreadStart(PopOut));
					thread.Start();

					BoardInterface.board.FBPage = null;
				}
			};

			return unsyncButton;
		}

		private void PopOut()
		{
			Thread.Sleep (300);
			InvokeOnMainThread(() => NavigationController.PopViewController(true));
		}

		private void LoadBanner()
		{
			using (UIImage bannerImage = UIImage.FromFile ("./screens/pageselector/banner/" + AppDelegate.PhoneVersion + ".jpg")) {
				banner = new UIImageView (new CGRect (0, 0, bannerImage.Size.Width / 2, bannerImage.Size.Height / 2));
				banner.Image = bannerImage;
			}

			UITapGestureRecognizer tap = new UITapGestureRecognizer ((tg) => {
				if (tg.LocationInView(this.View).X < AppDelegate.ScreenWidth / 4){
					NavigationController.PopViewController(true);
				}
			});

			banner.UserInteractionEnabled = true;
			banner.AddGestureRecognizer (tap);
			banner.Alpha = .95f;
		}
	}
}

