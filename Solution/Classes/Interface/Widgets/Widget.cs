using UIKit;
using System;
using System.Collections.Generic;
using CoreGraphics;
using Board.Schema;
using System.Threading;

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

		private UIImageView likeView;
		private UILabel likeLabel;

		private const int iconSize = 30;
		int randomLike;
		bool liked;

		public Widget()
		{
			using (UIImage image = UIImage.FromFile ("./boardinterface/widget/closedeye.png")) {
				ClosedEyeImageView = new UIImageView(image.ImageWithRenderingMode (UIImageRenderingMode.AlwaysTemplate));
			}

			using (UIImage image = UIImage.FromFile ("./boardinterface/widget/openeye.png")) {
				OpenEyeImageView = new UIImageView(image.ImageWithRenderingMode (UIImageRenderingMode.AlwaysTemplate));
			}

			View = new UIButton ();
			GestureRecognizers = new List<UIGestureRecognizer> ();
		}

		protected void CreateMounting(CGRect frame)
		{
			MountingView = new UIImageView (new CGRect (0, 0, frame.Width + 20, frame.Height + 50));
			MountingView.BackgroundColor = UIColor.FromRGB(250,250,250);

			CreateEye ();
			CreateLikeComponent ();

			MountingView.AddSubviews (LikeComponent, EyeView);
		}

		private void CreateLikeComponent()
		{
			LikeComponent = new UIImageView (new CGRect (MountingView.Frame.Width - 100,
				MountingView.Frame.Height - 40, 100, 40));

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

			using (UIImage image = UIImage.FromFile ("./boardinterface/widget/like.png")){
				likeView.Image = image;
			}

			likeView.Image = likeView.Image.ImageWithRenderingMode (UIImageRenderingMode.AlwaysTemplate);
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
			EyeView.Image = Widget.ClosedEyeImageView.Image;
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

		public UITapGestureRecognizer CreateDoubleTapToLikeGesture()
		{
			UITapGestureRecognizer doubletap = new UITapGestureRecognizer (tg => {
				tg.NumberOfTapsRequired = 2;

				if (Preview.View != null) { return; }

				Like();
			});

			return doubletap;
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

