using CoreGraphics;
using Foundation;
using UIKit;

namespace Board.Interface.Buttons
{
	public class FacebookButton : BIButton
	{
		public FacebookButton ()
		{
			using (UIImage uiImage = UIImage.FromFile ("./boardinterface/strokebuttons/facebook2.png")) {
				SetImage (uiImage, UIControlState.Normal);
			}
			Frame = new CGRect (0,0, ButtonSize, ButtonSize);
			Center = new CGPoint ((AppDelegate.ScreenWidth + ButtonSize) / 2 + 
				(AppDelegate.ScreenWidth - ButtonSize) / 8 * 3, AppDelegate.ScreenHeight - ButtonSize / 2);

			eventHandlers.Add ((sender, e) => {
				NSUrl url= new NSUrl("https://www.facebook.com/" + UIBoardInterface.board.FBPage.Id);
				UIApplication.SharedApplication.OpenUrl(url);

			});

		}
	}
}
