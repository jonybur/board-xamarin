using UIKit;
using System;
using System.Collections.Generic;
using CoreGraphics;
using System.Threading;

namespace Board.Interface.Widgets
{
	public class Widget
	{
		public UIView View;

		public static UIImageView ClosedEyeImageView;
		public static UIImageView OpenEyeImageView;

		public List<UIGestureRecognizer> gestureRecognizers;

		public UIImageView mountingView;

		protected UIImageView eye;

		public bool EyeOpen;

		private bool highlighted;

		public void Highlight(){
			if (!highlighted)
			{
				highlighted = true;

				mountingView.BackgroundColor = UIColor.FromRGB (100, 100, 100);

				Thread thread = new Thread (new ThreadStart (Unhighlight));
				thread.Start ();
			}
		}

		private void Unhighlight()
		{
			Thread.Sleep (1000);

			highlighted = false;
			View.InvokeOnMainThread (() => {
				mountingView.BackgroundColor = UIColor.FromRGB (250, 250, 250);
			});
		}

		public Widget()
		{
			using (UIImage image = UIImage.FromFile ("./boardinterface/widget/closedeye.png")) {
				ClosedEyeImageView = new UIImageView(image.ImageWithRenderingMode (UIImageRenderingMode.AlwaysTemplate));
			}

			using (UIImage image = UIImage.FromFile ("./boardinterface/widget/openeye.png")) {
				OpenEyeImageView = new UIImageView(image.ImageWithRenderingMode (UIImageRenderingMode.AlwaysTemplate));
			}
			
			View = new UIButton ();
			gestureRecognizers = new List<UIGestureRecognizer> ();
		}

		protected void CreateMounting(CGRect frame)
		{
			mountingView = new UIImageView (new CGRect (0, 0, frame.Width + 20, frame.Height + 50));
			mountingView.BackgroundColor = UIColor.FromRGB(250,250,250);
		}

		protected static UIImageView CreateLike(CGRect frame)
		{
			CGSize iconSize = new CGSize (30, 30);

			UIImageView likeView = new UIImageView(new CGRect (frame.Width - iconSize.Width - 10,
				frame.Height - iconSize.Height - 5, iconSize.Width, iconSize.Height));

			using (UIImage image = UIImage.FromFile ("./boardinterface/widget/like.png")){
				likeView.Image = image;
			}
			likeView.Image = likeView.Image.ImageWithRenderingMode (UIImageRenderingMode.AlwaysTemplate);
			likeView.TintColor = UIColor.FromRGB(140,140,140);

			return likeView;
		}

		protected static UILabel CreateLikeLabel(CGRect frame)
		{
			int fontSize = 16;
			UIFont likeFont = UIFont.SystemFontOfSize (fontSize);
			Random rand = new Random ();
			string likeText = rand.Next(16, 98).ToString();
			CGSize likeLabelSize = likeText.StringSize (likeFont);
			UILabel likeLabel = new UILabel(new CGRect(frame.X - likeLabelSize.Width - 4, frame.Y + 5, likeLabelSize.Width, likeLabelSize.Height));
			likeLabel.TextColor = UIColor.FromRGB (140, 140, 140);
			likeLabel.Font = likeFont;
			likeLabel.Text = likeText;
			likeLabel.TextAlignment = UITextAlignment.Right;

			return likeLabel;
		}

		protected static UIImageView CreateEye(CGRect frame)
		{
			CGSize iconSize = new CGSize (30, 30);

			UIImageView eyeView = new UIImageView(new CGRect (frame.X + 10, frame.Height - iconSize.Height - 5, iconSize.Width, iconSize.Height));
			eyeView.Image = Widget.ClosedEyeImageView.Image;
			eyeView.TintColor = UIColor.FromRGB(140,140,140);

			return eyeView;
		}

		public void SuscribeToEvents ()
		{
			foreach (UIGestureRecognizer gr in gestureRecognizers) {
				View.AddGestureRecognizer(gr);
			}
		}

		public void UnsuscribeToEvents()
		{
			foreach (UIGestureRecognizer gr in gestureRecognizers) {
				View.RemoveGestureRecognizer(gr);
			}
		}

		public void OpenEye()
		{
			eye.Image = OpenEyeImageView.Image;
			eye.TintColor = BoardInterface.board.MainColor;
			EyeOpen = true;
		}
	}
}

