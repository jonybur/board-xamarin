using System.Collections.Generic;
using System.Threading;
using Board.Facebook;
using System;
using CoreGraphics;
using UIKit;
using Board.Screens.Controls;

namespace Board.Interface.CreateScreens
{
	public class ImportScreen : UIViewController
	{
		UIImageView Banner;
		UIScrollView ScrollView;
		UITapGestureRecognizer bannerTap;
		List<ScreenButton> Buttons;
		bool pressed;
		string TypeOfImport;

		public ImportScreen(string typeOfImport)
		{
			TypeOfImport = typeOfImport;
		}

		public override void ViewDidLoad()
		{
			NavigationController.NavigationBar.BarStyle = UIBarStyle.Default;
			NavigationController.NavigationBarHidden = true;

			LoadBanner ();
			View.BackgroundColor = UIColor.White;

			Buttons = new List<ScreenButton> ();

			FacebookUtils.MakeGraphRequest (BoardInterface.board.FBPage.Id, TypeOfImport, LoadContent);
		}

		private void LoadContent(object sender, EventArgs e)
		{
			LoadEvents ();
			SuscribeToEvents ();
		}
			
		private void LoadEvents()
		{
			ScrollView = new UIScrollView (new CGRect (0, 0, AppDelegate.ScreenWidth, AppDelegate.ScreenHeight));
			ScrollView.ContentSize = new CGSize (AppDelegate.ScreenWidth, 80 * FacebookUtils.ElementList.Count + Banner.Frame.Height + FacebookUtils.ElementList.Count + 1);
			float yPosition = (float)Banner.Frame.Height;

			int i = 0;
			foreach (FacebookElement fbelement in FacebookUtils.ElementList) {
				OneLineScreenButton button;

				if (fbelement is FacebookEvent) {
					button = CreateButton (yPosition, ((FacebookEvent)fbelement).Name);
				} else if (fbelement is FacebookPost) {
					FacebookPost fbpost = (FacebookPost)fbelement;
					string text = string.Empty;
					if (fbpost.Message != "<null>") {
						text = fbpost.Message;
					} else {
						continue;
					}
					button = CreateButton (yPosition, text);
				} else {
					button = new OneLineScreenButton ();
				}

				yPosition += (float)button.Frame.Height + 1;
				ScrollView.AddSubview (button);
				Buttons.Add (button);
				i++;
			}

			View.AddSubview (ScrollView);
			View.AddSubview (Banner);
		}

		private OneLineScreenButton CreateButton(float yPosition, string content)
		{
			OneLineScreenButton fbeventButton = new OneLineScreenButton (yPosition);
			fbeventButton.SetLabel (content);
			fbeventButton.SetUnpressedColors ();

			fbeventButton.TapEvent += (sender, e) => {
				if (!pressed)
				{
					pressed = true;
					fbeventButton.SetPressedColors();

					Thread thread = new Thread(new ThreadStart(PopOut));
					thread.Start();
				}
			};

			return fbeventButton;
		}

		public override void ViewDidAppear (bool animated)
		{
			Banner.AddGestureRecognizer (bannerTap);
		}

		public override void ViewDidDisappear (bool animated)
		{
			Banner.RemoveGestureRecognizer (bannerTap);
			UnsuscribeToEvents ();
		}

		protected void LoadBanner()
		{
			using (UIImage bannerImage = UIImage.FromFile ("./screens/import/banner/" + AppDelegate.PhoneVersion + ".jpg")) {
				Banner = new UIImageView(new CGRect(0, 0, bannerImage.Size.Width / 2, bannerImage.Size.Height / 2));
				Banner.Image = bannerImage;	
			}

			bannerTap = new UITapGestureRecognizer (tg => {
				if (tg.LocationInView(this.View).X < AppDelegate.ScreenWidth / 4) {
					NavigationController.PopViewController(true);
				}
			});

			Banner.UserInteractionEnabled = true;
			Banner.Alpha = .95f;
			View.AddSubview (Banner);
		}

		private void PopOut()
		{
			Thread.Sleep (300);
			InvokeOnMainThread(() => NavigationController.PopViewController(true));
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

	}
}

