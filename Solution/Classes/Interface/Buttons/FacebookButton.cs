using CoreGraphics;
using Foundation;
using UIKit;

namespace Board.Interface.Buttons
{
	public class FacebookButton : Button
	{
		public FacebookButton ()
		{
			uiButton = new UIButton (UIButtonType.Custom);

			using (UIImage uiImage = UIImage.FromFile ("./boardinterface/strokebuttons/facebook2.png")) {
				uiButton.SetImage (uiImage, UIControlState.Normal);
			}
			uiButton.Frame = new CGRect (0,0, ButtonSize, ButtonSize);
			uiButton.Center = new CGPoint ((AppDelegate.ScreenWidth + ButtonSize) / 2 + 
				(AppDelegate.ScreenWidth - ButtonSize) / 8 * 3, AppDelegate.ScreenHeight - ButtonSize / 2);

			eventHandlers.Add ((sender, e) => {
				NSUrl url= new NSUrl("https://www.facebook.com/" + UIBoardInterface.board.FBPage.Id);
				UIApplication.SharedApplication.OpenUrl(url);

			});

		}
	}
}
