using CoreGraphics;
using UIKit;

namespace Clubby.Interface
{
	class UIAboutBox : UITextView {
		public UIAboutBox(string about, float yposition, float infoboxwidth){
			Frame = new CGRect (UIInfoBox.XContentMargin, yposition, infoboxwidth - UIInfoBox.XContentMargin * 2, 10);
			Text = about;
			TextColor = UIColor.White;
			Font = UIFont.SystemFontOfSize (14);
			TintColor = AppDelegate.ClubbyBlue;
			BackgroundColor = UIColor.FromRGBA (0, 0, 0, 0);

			Editable = false;
			ScrollEnabled = false;

			DataDetectorTypes = UIDataDetectorType.Link;

			var size = SizeThatFits (Frame.Size);
			if (size.Height == 33) {
				size.Height = 0;
			}
			Frame = new CGRect (Frame.X, Frame.Y, Frame.Width, size.Height);
		}
	}
}