using System;
using UIKit;
using MGImageUtilitiesBinding;
using Board.Interface;
using CoreGraphics;

namespace Board.Screens.Controls
{
	public sealed class BoardThumb : UIButton
	{
		public static float Size;
		private EventHandler TouchEvent;
		public Board.Schema.Board Board;

		public BoardThumb (Board.Schema.Board board, CGPoint contentOffset)
		{ 
			Board = board;

			float autosize = Size;
			float imgx, imgy;

			imgx = (float)(contentOffset.X);

			if (imgx < AppDelegate.ScreenWidth / 2) {
				imgx -= autosize / 4;
			} else if (AppDelegate.ScreenWidth / 2 < imgx) {
				imgx += autosize / 4;
			}

			imgy = (float)(contentOffset.Y);

			// launches the image preview
			Frame = new CGRect (0, 0, autosize, autosize);
			Center = new CGPoint (imgx, imgy);

			CGSize iconsize = new CGSize (autosize * .7f, autosize * .7f);

			UIImage img = board.ImageView.Image.ImageScaledToFitSize (iconsize);
			UIImage circle = CreateThumbImage(img, Frame.Size);

			SetBackgroundImage (circle, UIControlState.Normal);
			SetImage(img, UIControlState.Normal);

			TouchEvent = (sender, e) => {
				if (AppDelegate.boardInterface == null)
				{
					AppDelegate.boardInterface = new BoardInterface (board, false);
					AppDelegate.NavigationController.PushViewController (AppDelegate.boardInterface, true);
				}
			};

			this.UserInteractionEnabled = true;
		}

		private UIImage CreateThumbImage(UIImage logo, CGSize size)
		{
			UIGraphics.BeginImageContext (size);

			CGContext current = UIGraphics.GetCurrentContext ();

			current.SetFillColor (UIColor.White.CGColor);
			current.FillEllipseInRect (new CGRect(0, 0, size.Width, size.Height));

			return UIGraphics.GetImageFromCurrentImageContext ();
		}

		public void SuscribeToEvent()
		{
			TouchUpInside += TouchEvent;	
		}

		public void UnsuscribeToEvent()
		{
			TouchUpInside -= TouchEvent;
		}
	}
}

