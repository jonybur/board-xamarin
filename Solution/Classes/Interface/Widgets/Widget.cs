using System;
using System.Collections.Generic;
using Board.Infrastructure;
using Board.Interface.Buttons;
using Board.Interface.LookUp;
using Board.Schema;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using MGImageUtilitiesBinding;
using UIKit;

namespace Board.Interface.Widgets
{
	public partial class Widget : UIButton
	{
		public static string IdToUpdate;
		public static CGPoint AuxCenter;

		public static UIImageView ClosedEyeImageView;
		public static UIImageView OpenEyeImageView;

		public static UIColor HighlightColor;

		public Content content;

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

		public void ResetAttributesAndStopEditing(){
			Transform = content.Transform;
			Center = Widget.AuxCenter;
			DisableEditing ();
		}

		public void Unhighlight() {
			Highlighted = false;
			NavigationButton.HighlightedWidget = null;

			if (Layer != null) {
				Layer.ZPosition = 0;
				Layer.ShadowOpacity = 0f;
			}
		}

		private void RemoveWidget(UIAlertAction alertAction){
			UnsuscribeFromEditingEvents ();
			UnsuscribeFromUsabilityEvents ();
			RemoveFromSuperview ();
			UIBoardInterface.DictionaryWidgets.Remove (content.Id);
			string deleteJson = JsonUtilty.GenerateDeleteJson (content);
			ButtonInterface.SwitchButtonLayout (ButtonInterface.ButtonLayout.NavigationBar);
			CloudController.UpdateBoard (UIBoardInterface.board.Id, deleteJson);
		}

	}
}

