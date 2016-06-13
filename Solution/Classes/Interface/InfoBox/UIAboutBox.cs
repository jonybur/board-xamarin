using CoreGraphics;
using UIKit;

namespace Board.Interface
{
	class UIAboutBox : UITextView {
		public UIAboutBox(string about, float yposition, float infoboxwidth){
			Frame = new CGRect (UIInfoBox.XMargin, yposition, infoboxwidth - UIInfoBox.XMargin * 2, 10);
			Text = about;
			Font = UIFont.SystemFontOfSize (14);
			BackgroundColor = UIColor.FromRGBA (0, 0, 0, 0);

			Editable = false;
			//Selectable = false;
			ScrollEnabled = false;

			DataDetectorTypes = UIDataDetectorType.Link;

			var size = SizeThatFits (Frame.Size);
			Frame = new CGRect (Frame.X, Frame.Y, Frame.Width, size.Height);
		}
	}
}

