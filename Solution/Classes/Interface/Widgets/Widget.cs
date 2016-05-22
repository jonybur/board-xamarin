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
	public partial class Widget : UIButton
	{
		public static UIImageView ClosedEyeImageView;
		public static UIImageView OpenEyeImageView;

		public static UIColor HighlightColor;

		public Content content;

		public List<UIGestureRecognizer> GestureRecognizers;

		public bool EyeOpen;
		public static bool Highlighted;

		// mounting, likeheart, likelabel and eye
		protected UIButton DeleteButton;
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

		bool liked;
		int likes;


		public override UIView HitTest (CGPoint point, UIEvent uievent)
		{
			if (DeleteButton == null) {
				return base.HitTest(point, uievent);
			}

			var pointForTargetView = DeleteButton.ConvertPointFromView (point, this);
			
			if (DeleteButton.Frame.Contains (pointForTargetView)) {
				return DeleteButton.HitTest(pointForTargetView, uievent);
			}

			return base.HitTest(point, uievent);
		}

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

			Layer.ShadowOffset = new CGSize (0, 0);
			Layer.CornerRadius = 10;
			Layer.ShadowRadius = 10f;
			Layer.ShadowOpacity = 0f;
			Layer.ShadowColor = UIColor.Black.CGColor;

			GestureRecognizers = new List<UIGestureRecognizer> ();
		}

		public void SetTransforms(float xOffset = 0){

			Transform = CGAffineTransform.MakeRotation(0);
			MountingView.Transform = CGAffineTransform.MakeRotation(0);

			Frame = new CGRect (0, 0, MountingView.Frame.Width, MountingView.Frame.Height);
			Center = new CGPoint (content.Center.X + xOffset, content.Center.Y);
			Transform = content.Transform;
		}

		protected void CreateMounting(CGSize size)
		{
			MountingView = new UIImageView (new CGRect (0, 0, size.Width + SideMargin * 2, size.Height + 40 + TopMargin));
			MountingView.BackgroundColor = UIColor.White;

			CreateEye ();
			CreateLikeComponent ();
			CreateTimeStamp ();

			MountingView.Layer.AllowsEdgeAntialiasing = true;
			MountingView.Layer.CornerRadius = 10;

			MountingView.AddSubviews (LikeComponent, EyeView, TimeStamp);
		}

		protected void CreateDeleteButton(){
			DeleteButton = new UIButton ();

			using (var image = UIImage.FromFile ("./boardinterface/widget/deletebut.png")) {
				DeleteButton.Frame = new CGRect (new CGPoint(), new CGSize(50, 50));

				var imageView = new UIImageView ();
				imageView.Frame = new CGRect (0, 0, image.Size.Width / 2, image.Size.Height / 2);
				imageView.Image = image;
				imageView.Center = new CGPoint (DeleteButton.Frame.Width / 2, DeleteButton.Frame.Height / 2);

				DeleteButton.AddSubview (imageView);
			}

			DeleteButton.TouchUpInside += (sender, e) => {
				
				var alert = UIAlertController.Create("Delete widget?", null, UIAlertControllerStyle.Alert);

				alert.AddAction (UIAlertAction.Create ("Delete", UIAlertActionStyle.Default, RemoveWidget));
				alert.AddAction (UIAlertAction.Create ("Cancel", UIAlertActionStyle.Cancel, null));	

				AppDelegate.NavigationController.PresentViewController(alert, true, null);
			};

			DeleteButton.Center = new CGPoint (0, 0);
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
			header.Frame = new CGRect (0, 0, Frame.Width - SideMargin * 2 - 10, TopMargin);

			var headerLogo = new UIImageView ();
			var size = new CGSize(TopMargin, TopMargin);
			headerLogo.Image = UIBoardInterface.board.Image.ImageScaledToFitSize (size);
			headerLogo.Frame = new CGRect (0, 0, headerLogo.Image.Size.Width, headerLogo.Image.Size.Height);
			headerLogo.Center = new CGPoint (headerLogo.Center.X, header.Center.Y);

			header.AddSubviews (headerLogo);

			header.Frame = new CGRect (0, 0, headerLogo.Frame.Right, TopMargin);
			header.Center = new CGPoint (Frame.Width / 2, TopMargin / 2 + 2);

			return header;
		}


		public UIView CreateFullHeader(){
			var header = new UIView ();
			header.Frame = new CGRect (0, 0, Frame.Width - SideMargin * 2 - 10, TopMargin);

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
			header.Center = new CGPoint (Frame.Width / 2 - TopMargin / 2, TopMargin / 2 + 2);

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

				Layer.AddAnimation (scale, "highlight");
				Layer.ZPosition = 1;
				Layer.ShadowOpacity = .75f;

				CATransaction.Commit();
			}
		}

		public void Unhighlight() {
			Highlighted = false;
			NavigationButton.HighlightedWidget = null;

			if (Layer != null) {
				Layer.ZPosition = 0;
				Layer.ShadowOpacity = 0f;
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

			CGSize likeLabelSize = likes.ToString().StringSize (likeFont);

			UILabel likeLabel = new UILabel(new CGRect(0, 0, likeLabelSize.Width + 20, likeLabelSize.Height));
			likeLabel.TextColor = WidgetGrey;
			likeLabel.Font = likeFont;
			likeLabel.Text = likes.ToString();
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
				AddGestureRecognizer(gr);
			}
		}

		public void UnsuscribeToEvents()
		{
			foreach (UIGestureRecognizer gr in GestureRecognizers) {
				RemoveGestureRecognizer(gr);
			}
		}

		protected void CreateGestures()
		{
			var doubleTap = new UITapGestureRecognizer (tg => {
				if (Preview.IsAlive) { return; }

				Like();
			});

			var tap = new UITapGestureRecognizer (tg => {
				if (Preview.IsAlive) { return;	}

				if (LikeComponent.Frame.Left < tg.LocationInView(this).X &&
					LikeComponent.Frame.Top < tg.LocationInView(this).Y)
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
				// TODO: allow widget movement, transform, etc, then update.


				AddGestureRecognizer(SetNewPanGestureRecognizer());
				AddGestureRecognizer(SetNewPinchGestureRecognizer());
				AddGestureRecognizer(SetNewRotationGestureRecognizer());

				CreateDeleteButton ();
				AddSubview(DeleteButton);

				ButtonInterface.SwitchButtonLayout(ButtonInterface.ButtonLayout.ConfirmationBar);

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
	
		public UIPanGestureRecognizer SetNewPanGestureRecognizer()
		{
			float dx = 0;
			float dy = 0;

			var panGesture = new UIPanGestureRecognizer (pg => {
				if ((pg.State == UIGestureRecognizerState.Began || pg.State == UIGestureRecognizerState.Changed)) {
					var p0 = pg.LocationInView(Superview);

					if (dx == 0)
						dx = (float)(p0.X - Center.X);

					if (dy == 0)
						dy = (float)(p0.Y - Center.Y);

					var p1 = new CGPoint (p0.X - dx, p0.Y - dy);

					Center = p1;

				} else if (pg.State == UIGestureRecognizerState.Ended) {
					dx = 0;
					dy = 0;
				}

			});

			panGesture.Delegate = new CustomGestureRecognizerDelegate ();

			return panGesture;
		}

		private UIRotationGestureRecognizer SetNewRotationGestureRecognizer()
		{
			var rotateGesture = new UIRotationGestureRecognizer (rg => {
				if ((rg.State == UIGestureRecognizerState.Began || rg.State == UIGestureRecognizerState.Changed) && (rg.NumberOfTouches == 2)) {

					Transform = CGAffineTransform.Rotate(Transform, rg.Rotation);

					rg.Rotation = 0;

				}
			});

			rotateGesture.Delegate = new CustomGestureRecognizerDelegate ();

			return rotateGesture;
		}


		private UIPinchGestureRecognizer SetNewPinchGestureRecognizer(){

			var panGesture = new UIPinchGestureRecognizer (pinch => {
				if ((pinch.State == UIGestureRecognizerState.Began || pinch.State == UIGestureRecognizerState.Changed) && (pinch.NumberOfTouches == 2)) {
					
					var bounds = Bounds;
					var pinchCenter = pinch.LocationInView(this);
					pinchCenter.X -= bounds.GetMidX();
					pinchCenter.Y -= bounds.GetMidY();
					var transform = Transform;
					transform = CGAffineTransform.Translate(transform, pinchCenter.X, pinchCenter.Y);
					var scale = pinch.Scale;
					transform = CGAffineTransform.Scale(transform, scale, scale);
					transform = CGAffineTransform.Translate(transform, -pinchCenter.X, -pinchCenter.Y);

					if (transform.xx < 1.5f && transform.xx > .75f && transform.yx < 1.5f && transform.yy > .75f) {
						Transform = transform;
					}

					pinch.Scale = 1f;
				} 

			});

			panGesture.Delegate = new CustomGestureRecognizerDelegate ();

			return panGesture;
		}

		class CustomGestureRecognizerDelegate : UIGestureRecognizerDelegate{
			public override bool ShouldRecognizeSimultaneously (UIGestureRecognizer gestureRecognizer, UIGestureRecognizer otherGestureRecognizer)
			{
				if (gestureRecognizer.View != otherGestureRecognizer.View) {
					return false;
				}
				return true;
			}
		}

		private void RemoveWidget(UIAlertAction alertAction){
			UnsuscribeToEvents ();
			RemoveFromSuperview ();
			UIBoardInterface.DictionaryWidgets.Remove (content.Id);
			string deleteJson = JsonUtilty.GenerateDeleteJson (content);
			CloudController.UpdateBoard (UIBoardInterface.board.Id, deleteJson);
		}

		public void Like()
		{
			if (!liked)
			{
				likes ++;
				likeLabel.Text = likes.ToString();
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
				likes --;
				likeLabel.Text = likes.ToString();
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

