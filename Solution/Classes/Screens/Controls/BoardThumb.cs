using System;
using UIKit;
using Board.Utilities;
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
			float imgx, imgy, imgw, imgh;

			float autosize = Size;
			float scale = (float)(board.ImageView.Frame.Height / board.ImageView.Frame.Width);

			if (scale > 1) {
				scale = (float)(board.ImageView.Frame.Width / board.ImageView.Frame.Height);
				imgh = autosize;
				imgw = autosize * scale;
			} else {
				imgw = autosize;
				imgh = autosize * scale;	
			}

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

			UIImage img = CreateThumbImage (board.ImageView.Image, new CGSize(autosize, autosize));
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

		// TODO: implement this
		private UIImage CreateThumbImage(UIImage logo, CGSize size)
		{
			UIGraphics.BeginImageContext (size);

			CGContext current = UIGraphics.GetCurrentContext ();

			current.SetFillColor (UIColor.White.CGColor);
			current.FillEllipseInRect (new CGRect(0,0,size.Width, size.Height));

			float imgw, imgh;

			float scale = (float)(logo.Size.Height/logo.Size.Width);
			imgw = (float)size.Width * .69f;
			imgh = imgw * scale;

			logo.Draw (new CGRect (size.Width / 2 - imgw / 2, size.Height / 2 - imgh / 2, imgw, imgh));

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

