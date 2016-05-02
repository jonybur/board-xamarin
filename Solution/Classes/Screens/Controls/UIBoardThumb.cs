using System;
using UIKit;
using MGImageUtilitiesBinding;
using Board.Interface;
using CoreGraphics;

namespace Board.Screens.Controls
{
	public sealed class UIBoardThumb : UIButton
	{
		private EventHandler TouchEvent;
		public Board.Schema.Board Board;

		public UIBoardThumb (Board.Schema.Board board, CGPoint contentOffset, float size)
		{ 
			Board = board;

			float autosize = size;
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
			UIImage circle = CreateThumbImage(Frame.Size);

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

		private UIImage CreateThumbImage(CGSize size)
		{
			UIGraphics.BeginImageContextWithOptions (size, false, 2f);

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

