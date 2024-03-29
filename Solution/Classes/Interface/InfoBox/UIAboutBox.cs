﻿using CoreGraphics;
using UIKit;

namespace Board.Interface
{
	class UIAboutBox : UITextView {
		public UIAboutBox(string about, float yposition, float infoboxwidth){
			Frame = new CGRect (10, yposition, infoboxwidth - 10 * 2, 10);
			Text = about;
			Font = UIFont.SystemFontOfSize (14);
			BackgroundColor = UIColor.FromRGBA (0, 0, 0, 0);

			Editable = false;
			ScrollEnabled = false;

			DataDetectorTypes = UIDataDetectorType.Link;

			var size = SizeThatFits (Frame.Size);
			Frame = new CGRect (Frame.X, Frame.Y, Frame.Width, size.Height);
		}
	}
}

