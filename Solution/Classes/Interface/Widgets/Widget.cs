using UIKit;
using System;
using System.Collections.Generic;
using CoreGraphics;
using Board.Schema;
using System.Threading;
using Board.Interface.LookUp;

namespace Board.Interface.Widgets
{
	public class Widget
	{
		public static UIImageView ClosedEyeImageView;
		public static UIImageView OpenEyeImageView;

		public UIView View;

		public Content content;

		public List<UIGestureRecognizer> GestureRecognizers;

		public bool EyeOpen;

		private bool highlighted;

		// mounting, likeheart, likelabel and eye
		protected UIImageView MountingView;
		protected UIImageView LikeComponent;
		protected UIImageView EyeView;
		protected UILabel TimeStamp;

		private UIImageView likeView;
		private UILabel likeLabel;

		private const int iconSize = 30;

		public static int Autosize = 220;

		int randomLike;
		bool liked;

		public Widget()
		{
			if (AppDelegate.PhoneVersion == "6") {
				Autosize = 220;
			} else if (AppDelegate.PhoneVersion == "6plus") {
				Autosize = 220;
			}

			if (ClosedEyeImageView == null) {
				using (UIImage image = UIImage.FromFile ("./boardinterface/widget/closedeye.png")) {
					ClosedEyeImageView = new UIImageView (image.ImageWithRenderingMode (UIImageRenderingMode.AlwaysTemplate));
				}
			}

			if (OpenEyeImageView == null) {
				using (UIImage image = UIImage.FromFile ("./boardinterface/widget/openeye.png")) {
					OpenEyeImageView = new UIImageView (image.ImageWithRenderingMode (UIImageRenderingMode.AlwaysTemplate));
				}
			}

			View = new UIButton ();
			GestureRecognizers = new List<UIGestureRecognizer> ();
		}

		public void SetTransforms(){
			View.Frame = new CGRect (0, 0, MountingView.Frame.Width, MountingView.Frame.Height);
			View.Center = content.Center;
			View.Transform = CGAffineTransform.MakeRotation(content.Rotation);
		}

		public void SetTransforms(float xOffset){
			View.Frame = new CGRect (0, 0, MountingView.Frame.Width, MountingView.Frame.Height);
			View.Center = new CGPoint (content.Center.X + xOffset, content.Center.Y);
			View.Transform = CGAffineTransform.MakeRotation(content.Rotation);
		}

		protected void CreateMounting(CGRect frame)
		{
			MountingView = new UIImageView (new CGRect (0, 0, frame.Width + 20, frame.Height + 50));
			MountingView.BackgroundColor = UIColor.FromRGB(250,250,250);

			CreateEye ();
			CreateLikeComponent ();
			CreateTimeStamp ();

			MountingView.Layer.AllowsEdgeAntialiasing = true;

			MountingView.AddSubviews (LikeComponent, EyeView, TimeStamp);
		}

		private void CreateTimeStamp()
		{
			TimeStamp = new UILabel (new CGRect (0, 0, MountingView.Frame.Width, 40));
			TimeStamp.Font = UIFont.SystemFontOfSize (14);
			TimeStamp.Center = new CGPoint (MountingView.Frame.Width / 2, LikeComponent.Center.Y);

			TimeSpan timeDifference = DateTime.Now.Subtract (content.CreationDate);

			if (timeDifference.TotalSeconds < 60) {
				TimeStamp.Text = timeDifference.Seconds + "s";
			} else if (timeDifference.TotalMinutes < 60) {
				TimeStamp.Text = timeDifference.Minutes + "m";
			} else if (timeDifference.TotalHours < 24) {
				TimeStamp.Text = timeDifference.Hours + "h";
			} else if (timeDifference.TotalDays < 7) {
				TimeStamp.Text = timeDifference.Days + "d";
			} else {
				TimeStamp.Text = (timeDifference.Days/7) + "w";
			}


			TimeStamp.TextAlignment = UITextAlignment.Center;
			TimeStamp.TextColor = UIColor.FromRGB (140, 140, 140);
		}

		private void CreateLikeComponent()
		{
			LikeComponent = new UIImageView (new CGRect (MountingView.Frame.Width - 80,
				MountingView.Frame.Height - 40, 80, 40));

			likeView = CreateLike (LikeComponent.Frame);
			likeLabel = CreateLikeLabel (likeView.Center);

			LikeComponent.AddSubviews (likeView, likeLabel);
		}

		public void Highlight(){
			if (!highlighted)
			{
				highlighted = true;

				MountingView.BackgroundColor = UIColor.FromRGB (80, 80, 80);

				Thread thread = new Thread (new ThreadStart (Unhighlight));
				thread.Start ();
			}
		}

		private void Unhighlight()
		{
			Thread.Sleep (1000);

			highlighted = false;
			View.InvokeOnMainThread (() => {
				MountingView.BackgroundColor = UIColor.FromRGB (250, 250, 250);
			});
		}

		public void InstantUnhighlight()
		{
			highlighted = false;
			View.InvokeOnMainThread (() => {
				MountingView.BackgroundColor = UIColor.FromRGB (250, 250, 250);
			});	
		}
	
		protected UIImageView CreateLike(CGRect frame)
		{
			UIImageView likeView = new UIImageView(new CGRect(0, 0, iconSize, iconSize));

			using (UIImage img = UIImage.FromFile ("./boardinterface/widget/like.png")) {
				likeView.Image = img;
				likeView.Image = likeView.Image.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
			}

			likeView.TintColor = UIColor.FromRGB(140,140,140);
			likeView.Center = new CGPoint (frame.Width - 5 - iconSize / 2, frame.Height / 2);
			return likeView;
		}

		protected UILabel CreateLikeLabel(CGPoint center)
		{
			int fontSize = 14;

			UIFont likeFont = UIFont.SystemFontOfSize (fontSize);
			Random rand = new Random ();
			randomLike = rand.Next (16, 98);

			string likeText = randomLike.ToString();
			CGSize likeLabelSize = likeText.StringSize (likeFont);

			UILabel likeLabel = new UILabel(new CGRect(0, 0, likeLabelSize.Width + 20, likeLabelSize.Height));
			likeLabel.TextColor = UIColor.FromRGB (140, 140, 140);
			likeLabel.Font = likeFont;
			likeLabel.Text = likeText;
			likeLabel.Center = new CGPoint (center.X - likeLabel.Frame.Width / 2 - 5, center.Y);
			likeLabel.TextAlignment = UITextAlignment.Center;

			return likeLabel;
		}

		protected void CreateEye()
		{
			EyeView = new UIImageView(new CGRect (MountingView.Frame.X + 10, MountingView.Frame.Height - iconSize - 5, iconSize, iconSize));
			EyeView.Image = ClosedEyeImageView.Image;
			EyeView.TintColor = UIColor.FromRGB(140,140,140);
		}

		public void SuscribeToEvents ()
		{
			foreach (UIGestureRecognizer gr in GestureRecognizers) {
				View.AddGestureRecognizer(gr);
			}
		}

		public void UnsuscribeToEvents()
		{
			foreach (UIGestureRecognizer gr in GestureRecognizers) {
				View.RemoveGestureRecognizer(gr);
			}
		}

		protected void CreateGestures()
		{
			UITapGestureRecognizer doubleTap = new UITapGestureRecognizer (tg => {
				if (Preview.IsAlive) { return; }

				Like();

				// focus on widget on doubletap like
				//CGPoint position = new CGPoint ((float)(View.Frame.X - AppDelegate.ScreenWidth/2 + View.Frame.Width/2), 0f);
				//BoardInterface.scrollView.SetContentOffset (position, true);
			});

			UITapGestureRecognizer tap = new UITapGestureRecognizer (tg => {
				if (Preview.IsAlive) { return;	}

				if (LikeComponent.Frame.Left < tg.LocationInView(this.View).X &&
					LikeComponent.Frame.Top < tg.LocationInView(this.View).Y)
				{
					Like();
				}
				else{

					LookUp.LookUp lookUp;

					if (content is Video)
					{
						lookUp = new VideoLookUp((Video)content);
					} 
					else if (content is Picture)
					{
						lookUp = new PictureLookUp((Picture)content);
					}
					else if (content is Announcement)
					{
						lookUp = new AnnouncementLookUp((Announcement)content);
					}
					else if (content is BoardEvent)
					{
						lookUp = new EventLookUp((BoardEvent)content);
					}
					else if (content is Map)
					{
						lookUp = new MapLookUp((Map)content);
					}
					else if (content is Poll)
					{
						return;
						//lookUp = new PollLookUp((Poll)content);
					}
					else {
						lookUp = new LookUp.LookUp();
					}


					AppDelegate.PushViewLikePresentView(lookUp);
					//AppDelegate.NavigationController.PresentViewController(lookUp, true, null);
				}
			});

			tap.NumberOfTapsRequired = 1;
			doubleTap.NumberOfTapsRequired = 2;

			tap.DelaysTouchesBegan = true;
			doubleTap.DelaysTouchesBegan = true;

			tap.RequireGestureRecognizerToFail (doubleTap);

			GestureRecognizers.Add (tap);
			GestureRecognizers.Add (doubleTap);
		}

		public void Like()
		{
			if (!liked)
			{
				randomLike ++;
				likeLabel.Text = randomLike.ToString();
				likeView.TintColor = AppDelegate.BoardOrange;
				likeLabel.TextColor = AppDelegate.BoardOrange;
				liked = true;
			}
			else {
				randomLike --;
				likeLabel.Text = randomLike.ToString();
				likeView.TintColor = UIColor.FromRGB (140, 140, 140);
				likeLabel.TextColor = UIColor.FromRGB (140, 140, 140);
				liked = false;
			}
		}

		public void OpenEye()
		{
			EyeView.Image = OpenEyeImageView.Image;
			EyeView.TintColor = BoardInterface.board.MainColor;
			EyeOpen = true;
		}
	}
}

