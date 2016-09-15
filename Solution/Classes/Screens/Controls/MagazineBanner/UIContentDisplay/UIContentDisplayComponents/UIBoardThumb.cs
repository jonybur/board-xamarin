using System;
using Board.Utilities;
using CoreGraphics;
using Foundation;
using Haneke;
using UIKit;
using CoreAnimation;

namespace Board.Screens.Controls
{
	public class UIBoardThumbComponent : UIView{
		public UIBoardThumb BoardThumb;
		public UILabel NameLabel;
		public const int TextSpace = 60;
		public readonly float Size;

		public UIBoardThumbComponent(Board.Schema.Board board, CGPoint contentOffset, float size){
			Size = size;
			BoardThumb = new UIBoardThumb (board, contentOffset, size);

			Frame = new CGRect(BoardThumb.Frame.X, BoardThumb.Frame.Y, BoardThumb.Frame.Width, BoardThumb.Frame.Height + TextSpace);
			BoardThumb.Frame = new CGRect (0, 0, BoardThumb.Frame.Width, BoardThumb.Frame.Height);
			UserInteractionEnabled = true;

			NameLabel = CreateNameLabel (board.Name, CommonUtils.GetDistanceFromUserToBoard(board), size);

			AddSubviews (BoardThumb, NameLabel);
		}

		public void UpdateDistanceLabel ()
		{
			NameLabel.RemoveFromSuperview ();
			NameLabel = CreateNameLabel (BoardThumb.Board.Name, CommonUtils.GetDistanceFromUserToBoard (BoardThumb.Board), Size);
			AddSubview (NameLabel);
		}

		private string NameLimiter(string nameString){

			if (CommonUtils.IsStringAllUpper(nameString) && nameString.Length > 14) {
				nameString = nameString.Substring (0, 14) + "...";
				return nameString;
			}

			if (nameString.Length > 13 && (AppDelegate.PhoneVersion == AppDelegate.PhoneVersions.iPhone5 || AppDelegate.PhoneVersion == AppDelegate.PhoneVersions.iPhone4)) {
				nameString = nameString.Substring (0, 13) + "...";
				return nameString;
			}

			if (nameString.Length > 20) {
				nameString = nameString.Substring (0, 20) + "...";
				return nameString;
			}

			return nameString;

		}

		private UILabel CreateNameLabel(string nameString, double distance, float width)
		{
			var label = new UILabel ();

			label.BackgroundColor = UIColor.FromRGBA (0, 0, 0, 0);

			nameString = CommonUtils.FirstLetterOfEveryWordToUpper (nameString);
			nameString = NameLimiter (nameString);

			var distanceTotalString = CommonUtils.GetFormattedDistance (distance);

			string compositeString = nameString + "\n" + distanceTotalString;

			var nameAttributes = new UIStringAttributes {
				Font = UIFont.SystemFontOfSize(14),
				ForegroundColor = UIColor.Black
			};

			var distanceAttributes = new UIStringAttributes {
				Font = UIFont.SystemFontOfSize(14),
				ForegroundColor = UIColor.FromRGB(100,100,100)
			};

			var attributedString = new NSMutableAttributedString (compositeString);
			attributedString.SetAttributes (nameAttributes.Dictionary, new NSRange (0, nameString.Length));
			attributedString.SetAttributes (distanceAttributes, new NSRange (nameString.Length, distanceTotalString.Length + 1));

			label.TextColor = AppDelegate.BoardBlack;
			label.Lines = 0;
			label.AttributedText = attributedString;
			label.AdjustsFontSizeToFitWidth = false;
			label.Font = UIFont.SystemFontOfSize(14);
			label.Frame = new CGRect (5, width, width - 10, TextSpace);
			label.SizeToFit ();

			return label;
		}
	}

	public class UIBoardThumb : UIContentThumb
	{
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

			var circle = CreateThumbImage(Frame.Size);
			SetBackgroundImage (circle, UIControlState.Normal);

			var imageView = new UIImageView ();
			imageView.Frame = new CGRect(0, 0, autosize * .7f, autosize * .7f);
			imageView.ContentMode = UIViewContentMode.ScaleAspectFit;
			imageView.SetImage (new NSUrl(board.LogoUrl));
			imageView.Center = new CGPoint (Frame.Size.Width / 2, Frame.Size.Height / 2);
			imageView.Layer.CornerRadius = imageView.Frame.Width / 2;
			imageView.ClipsToBounds = true;

			AddSubview (imageView);

			TouchEvent = delegate {
				AppDelegate.OpenBoard(board);
			};

			UserInteractionEnabled = true;
			ClipsToBounds = true;
		}

		private UIImage CreateThumbImage(CGSize size)
		{
			UIGraphics.BeginImageContextWithOptions (size, false, 2f);

			CGContext current = UIGraphics.GetCurrentContext ();

			current.SetFillColor (UIColor.White.CGColor);
			current.FillEllipseInRect (new CGRect(0, 0, size.Width, size.Height));

			return UIGraphics.GetImageFromCurrentImageContext ();
		}
	}


}

