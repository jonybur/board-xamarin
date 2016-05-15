using UIKit;
using System;
using System.Collections.Generic;
using CoreGraphics;
using MGImageUtilitiesBinding;
using CoreAnimation;
using Board.Interface.Buttons;
using Board.Schema;
using Foundation;
using Board.Interface.LookUp;
using Board.Infrastructure;
using Board.Utilities;

namespace Board.Interface.Widgets
{
	public class Widget
	{
		public static UIImageView ClosedEyeImageView;
		public static UIImageView OpenEyeImageView;

		public UIView View;

		public static UIColor HighlightColor;

		public Content content;

		public List<UIGestureRecognizer> GestureRecognizers;

		public bool EyeOpen;
		public static bool Highlighted;

		// mounting, likeheart, likelabel and eye
		protected UIImageView MountingView;
		protected UIImageView LikeComponent;
		protected UIImageView EyeView;
		protected UILabel TimeStamp;

		private UIImageView likeView;
		private UILabel likeLabel;

		private const int IconSize = 30;

		public int TopMargin = 5;
		public int SideMargin = 5;
		public static int Autosize;

		private UIColor WidgetGrey;

		int randomLike;
		bool liked;

		public Widget()
		{
			HighlightColor = AppDelegate.BoardBlack;
			WidgetGrey = UIColor.FromRGB (100, 100, 100);

			if (AppDelegate.PhoneVersion == "6") {
				Autosize = 230;
			} else if (AppDelegate.PhoneVersion == "6plus") {
				Autosize = 220;
			} else {
				Autosize = 230;
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

			View.Layer.CornerRadius = 10;

			View.Layer.ShadowOffset = new CGSize (0, 0);
			View.Layer.ShadowRadius = 10f;
			View.Layer.ShadowColor = UIColor.Black.CGColor;
			View.Layer.ShadowOpacity = 0f;

			GestureRecognizers = new List<UIGestureRecognizer> ();
		}

		public void SetTransforms(float xOffset = 0){

			View.Transform = CGAffineTransform.MakeRotation(0);
			MountingView.Transform = CGAffineTransform.MakeRotation(0);

			View.Frame = new CGRect (0, 0, MountingView.Frame.Width, MountingView.Frame.Height);
			View.Center = new CGPoint (content.Center.X + xOffset, content.Center.Y);
			View.Transform = CGAffineTransform.MakeRotation(content.Rotation);
		}

		protected void CreateMounting(CGSize size)
		{
			MountingView = new UIImageView (new CGRect (0, 0, size.Width + SideMargin * 2, size.Height + 40 + TopMargin));
			MountingView.BackgroundColor = UIColor.White;

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


		public UIView CreateLogoHeader(){
			var header = new UIView ();
			header.Frame = new CGRect (0, 0, View.Frame.Width - SideMargin * 2 - 10, TopMargin);

			var headerLogo = new UIImageView ();
			var size = new CGSize(TopMargin, TopMargin);
			headerLogo.Image = UIBoardInterface.board.Image.ImageScaledToFitSize (size);
			headerLogo.Frame = new CGRect (0, 0, headerLogo.Image.Size.Width, headerLogo.Image.Size.Height);
			headerLogo.Center = new CGPoint (headerLogo.Center.X, header.Center.Y);

			header.AddSubviews (headerLogo);

			header.Frame = new CGRect (0, 0, headerLogo.Frame.Right, TopMargin);
			header.Center = new CGPoint (View.Frame.Width / 2, TopMargin / 2 + 2);

			return header;
		}


		public UIView CreateFullHeader(){
			var header = new UIView ();
			header.Frame = new CGRect (0, 0, View.Frame.Width - SideMargin * 2 - 10, TopMargin);

			var headerLogo = new UIImageView ();
			var size = new CGSize(TopMargin, TopMargin);
			headerLogo.Image = UIBoardInterface.board.Image.ImageScaledToFitSize (size);
			headerLogo.Frame = new CGRect (0, 0, headerLogo.Image.Size.Width, headerLogo.Image.Size.Height);
			headerLogo.Center = new CGPoint (headerLogo.Center.X, header.Center.Y);

			var headerText = new UILabel ();
			float sizeOfHeaderLogo = (float)headerLogo.Frame.Right + 10;
			headerText.Frame = new CGRect (sizeOfHeaderLogo, 0, header.Frame.Width - sizeOfHeaderLogo, header.Frame.Height);
			headerText.Text = UIBoardInterface.board.Name;
			headerText.Font = UIFont.BoldSystemFontOfSize (14);
			headerText.SizeToFit ();
			headerText.Center = new CGPoint (headerText.Center.X, header.Center.Y);

			header.AddSubviews (headerLogo, headerText);

			header.Frame = new CGRect (0, 0, headerText.Frame.Right, TopMargin);
			header.Center = new CGPoint (View.Frame.Width / 2 - TopMargin / 2, TopMargin / 2 + 2);

			return header;
		}

		public void Highlight(){
			if (!Highlighted)
			{
				Highlighted = true;

				CATransaction.Begin();

				CATransaction.CompletionBlock = delegate {
					Unhighlight();
				};

				CAKeyFrameAnimation scale = new CAKeyFrameAnimation ();
				scale.KeyPath = "transform";

				var identity = CATransform3D.MakeScale(1f, 1f, 1f);
				var scaled = CATransform3D.MakeScale (1.2f, 1.2f, 1.2f);

				scale.Values = new NSObject[]{ 
					NSValue.FromCATransform3D (identity),
					NSValue.FromCATransform3D (scaled),
					NSValue.FromCATransform3D (identity)
				};

				scale.KeyTimes = new NSNumber[]{0, 1/2, 1};
				scale.Additive = true;
				scale.Duration = .5f;
				scale.RemovedOnCompletion = false;

				View.Layer.AddAnimation (scale, "highlight");
				View.Layer.ZPosition = 1;
				View.Layer.ShadowOpacity = .75f;

				CATransaction.Commit();
			}
		}

		public void Unhighlight() {
			Highlighted = false;
			NavigationButton.HighlightedWidget = null;

			if (View.Layer != null) {
				View.Layer.ZPosition = 0;
				View.Layer.ShadowOpacity = 0f;
			}
		}
	
		protected UIImageView CreateLike(CGRect frame)
		{
			var likeView = new UIImageView(new CGRect(0, 0, IconSize, IconSize));

			using (UIImage img = UIImage.FromFile ("./boardinterface/widget/like.png")) {
				likeView.Image = img;
				likeView.Image = likeView.Image.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
			}

			likeView.TintColor = WidgetGrey;
			likeView.Center = new CGPoint (frame.Width - 5 - IconSize / 2, frame.Height / 2);
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
			likeLabel.TextColor = WidgetGrey;
			likeLabel.Font = likeFont;
			likeLabel.Text = likeText;
			likeLabel.Center = new CGPoint (center.X - likeLabel.Frame.Width / 2 - 5, center.Y);
			likeLabel.TextAlignment = UITextAlignment.Center;

			return likeLabel;
		}

		protected void CreateEye()
		{
			EyeView = new UIImageView(new CGRect (MountingView.Frame.X + 10, MountingView.Frame.Height - IconSize - 5, IconSize, IconSize));
			EyeView.Image = ClosedEyeImageView.Image;
			EyeView.TintColor = WidgetGrey;
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
			var doubleTap = new UITapGestureRecognizer (tg => {
				if (Preview.IsAlive) { return; }

				Like();

				// focus on widget on doubletap like
				//CGPoint position = new CGPoint ((float)(View.Frame.X - AppDelegate.ScreenWidth/2 + View.Frame.Width/2), 0f);
				//BoardInterface.scrollView.SetContentOffset (position, true);
			});

			var tap = new UITapGestureRecognizer (tg => {
				if (Preview.IsAlive) { return;	}

				if (LikeComponent.Frame.Left < tg.LocationInView(this.View).X &&
					LikeComponent.Frame.Top < tg.LocationInView(this.View).Y)
				{
					Like();
				}
				else{

					UILookUp lookUp;

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
						lookUp = new MapLookUp(UIBoardInterface.board.GeolocatorObject);
					}
					else if (content is Poll)
					{
						return;
						//lookUp = new PollLookUp((Poll)content);
					}
					else {
						lookUp = new UILookUp();
					}


					AppDelegate.PushViewLikePresentView(lookUp);
					//AppDelegate.NavigationController.PresentViewController(lookUp, true, null);
				}
			});

			var longPress = new UILongPressGestureRecognizer (tg => {
				UIAlertController alert = UIAlertController.Create(null, null, UIAlertControllerStyle.ActionSheet);

				alert.AddAction (UIAlertAction.Create ("Remove Widget", UIAlertActionStyle.Default, RemoveWidget));
				alert.AddAction (UIAlertAction.Create ("Cancel", UIAlertActionStyle.Cancel, null));	

				AppDelegate.NavigationController.PresentViewController(alert, true, null);	
			});

			tap.NumberOfTapsRequired = 1;
			doubleTap.NumberOfTapsRequired = 2;

			tap.DelaysTouchesBegan = true;
			doubleTap.DelaysTouchesBegan = true;

			tap.RequireGestureRecognizerToFail (doubleTap);

			GestureRecognizers.Add (tap);
			GestureRecognizers.Add (doubleTap);

			if (UIBoardInterface.UserCanEditBoard) {
				GestureRecognizers.Add (longPress);
			}
		}

		private void RemoveWidget(UIAlertAction alertAction){
			UnsuscribeToEvents ();
			View.RemoveFromSuperview ();
			MemoryUtility.ReleaseUIViewWithChildren (View);
			UIBoardInterface.DictionaryWidgets.Remove (content.Id);
			string deleteJson = JsonUtilty.GenerateDeleteJson (content);
			CloudController.UpdateBoard (UIBoardInterface.board.Id, deleteJson);
		}

		public void Like()
		{
			if (!liked)
			{
				randomLike ++;
				likeLabel.Text = randomLike.ToString();
				likeView.TintColor = AppDelegate.BoardOrange;
				likeLabel.TextColor = AppDelegate.BoardOrange;

				CAKeyFrameAnimation scale = new CAKeyFrameAnimation ();
				scale.KeyPath = "transform";

				var identity = CATransform3D.MakeScale(1f, 1f, 1f);
				var scaled = CATransform3D.MakeScale (1.3f, 1.3f, 1.3f);

				scale.Values = new NSObject[]{ 
					NSValue.FromCATransform3D (identity),
					NSValue.FromCATransform3D (scaled),
					NSValue.FromCATransform3D (identity)
				};

				scale.KeyTimes = new NSNumber[]{0, 1/2, 1};
				scale.Additive = true;
				scale.Duration = .5f;
				scale.RemovedOnCompletion = false;

				likeView.Layer.AddAnimation (scale, "highlight");

				liked = true;
			}
			else {
				randomLike --;
				likeLabel.Text = randomLike.ToString();
				likeView.TintColor = WidgetGrey;
				likeLabel.TextColor = WidgetGrey;
				liked = false;
			}
		}

		public void OpenEye()
		{
			EyeView.Image = OpenEyeImageView.Image;
			EyeView.TintColor = AppDelegate.BoardOrange;
			EyeOpen = true;

			CAKeyFrameAnimation scale = new CAKeyFrameAnimation ();
			scale.KeyPath = "transform";

			var identity = CATransform3D.MakeScale(1f, 1f, 1f);
			var scaled = CATransform3D.MakeScale (1.3f, 1.3f, 1.3f);

			scale.Values = new NSObject[]{ 
				NSValue.FromCATransform3D (identity),
				NSValue.FromCATransform3D (scaled),
				NSValue.FromCATransform3D (identity)
			};

			scale.KeyTimes = new NSNumber[]{0, 1/2, 1};
			scale.Duration = .5f;
			scale.RemovedOnCompletion = false;

			EyeView.Layer.AddAnimation (scale, "highlight");

			ButtonInterface.navigationButton.SubtractNavigationButtonText();
		}
	}
}

