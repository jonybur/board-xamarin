
using Board.Infrastructure;
using Board.Interface.Buttons;
using Board.Schema;
using Haneke;
using CoreAnimation;
using CoreGraphics;
using Foundation;
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

		public static float Autosize{
			get{

				float percentageCorrection = 0f;

				switch (AppDelegate.PhoneVersion) {
				case AppDelegate.PhoneVersions.iPhone4:
					percentageCorrection = -.7f;
					break;
				case AppDelegate.PhoneVersions.iPhone5:
					//percentageCorrection = .07f;
					break;
				case AppDelegate.PhoneVersions.iPhone6:
					//percentageCorrection = 0f;
					break;
				case AppDelegate.PhoneVersions.iPhone6Plus:
					//percentageCorrection = -.055f;
					break;
				}

				return 230f * (UIBoardScroll.AspectPercentage + percentageCorrection);
			}
		}

		private UIColor WidgetGrey;

		bool liked;
		int likes;

		public Widget()
		{
			HighlightColor = AppDelegate.BoardBlack;
			WidgetGrey = UIColor.FromRGB (100, 100, 100);
		
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
			Layer.CornerRadius = 10f;
			Layer.ShadowRadius = 10f;
			Layer.ShadowOpacity = 0f;
			Layer.ShadowColor = UIColor.Black.CGColor;
		}

		public void SetTransforms(float xOffset = 0){
			Center = new CGPoint (content.Center.X + xOffset, content.Center.Y);
			Transform = content.Transform;
		}

		protected UITextView CreateDescriptionView(string description, CGSize contentSize){
			var descriptionView = new UITextView ();
			descriptionView.Frame = new CGRect (0, 0,
				contentSize.Width, 10);
			if (description != "<null>" && !string.IsNullOrEmpty (description)) {
				descriptionView.Text = description;
			} else {
				descriptionView.Alpha = 0f;
			}
			descriptionView.BackgroundColor = UIColor.FromRGBA (255, 255, 255, 180);
			descriptionView.Layer.AllowsEdgeAntialiasing = true;
			descriptionView.Font = UIFont.SystemFontOfSize (12, UIFontWeight.Light);
			descriptionView.Selectable = false;
			descriptionView.Editable = false;
			var size = descriptionView.SizeThatFits (descriptionView.Frame.Size);
			float newHeight = size.Height > 50 ? 50 : (float)size.Height;
			descriptionView.Frame = new CGRect (SideMargin, TopMargin + contentSize.Height - newHeight,
				descriptionView.Frame.Width, newHeight);
			
			return descriptionView;
		}

		protected void CreateMounting(CGSize size) {
			MountingView = new UIImageView (new CGRect (0, 0, size.Width + SideMargin * 2, size.Height + 40 + TopMargin));
			MountingView.BackgroundColor = UIColor.White;

			CreateEye ();
			CreateLikeComponent ();
			CreateTimeStamp ();

			MountingView.Layer.AllowsEdgeAntialiasing = true;
			MountingView.Layer.CornerRadius = 10;

			MountingView.AddSubviews (LikeComponent, EyeView, TimeStamp);
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

	public class UIWidgetHeader : UIImageView{
		public UIImageView Logo;

		public void CancelSetImage(){
			Logo.CancelSetImage ();
			this.CancelSetImage ();
		}

		public UIWidgetHeader(float width, int topMargin){
			Frame = new CGRect (0, 0, width, topMargin);

			var LogoBackground = new UIImageView ();
			LogoBackground.Frame = new CGRect (0, 0, topMargin - 5, topMargin - 5);
			LogoBackground.Center = new CGPoint (Frame.Width / 2, Center.Y);
			LogoBackground.BackgroundColor = UIColor.White;
			AddSubviews (LogoBackground);

			Logo = new UIImageView ();
			Logo.Frame = new CGRect (0, 0, topMargin - 8, topMargin - 8);
			Logo.Center = new CGPoint (Frame.Width / 2, Center.Y);
			Logo.SetImage (new NSUrl(UIBoardInterface.board.LogoUrl));
			AddSubviews (Logo);

			ClipsToBounds = true;

			this.ContentMode = UIViewContentMode.ScaleAspectFill;
			this.SetImage (new NSUrl(UIBoardInterface.board.CoverImageUrl));
			
			this.Layer.AllowsEdgeAntialiasing = true;
			LogoBackground.Layer.AllowsEdgeAntialiasing = true;
			Logo.Layer.AllowsEdgeAntialiasing = true;

			Center = new CGPoint (Frame.Width / 2, topMargin / 2);
		}
	}
}

