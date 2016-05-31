using CoreGraphics;
using UIKit;

namespace Board.Interface
{
	class UIAboutBox : UITextView {
		public UIAboutBox(string about, float yposition, float infoboxwidth){
			Frame = new CGRect (UIInfoBox.XMargin, yposition, infoboxwidth - UIInfoBox.XMargin * 2, 100);
			Text = about;
			Font = UIFont.SystemFontOfSize (14);
			BackgroundColor = UIColor.FromRGBA (0, 0, 0, 0);
			Editable = false;
			Selectable = false;
		}
	}
}

