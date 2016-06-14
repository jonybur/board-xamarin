using Board.Screens.Controls;
using Board.Utilities;
using CoreGraphics;
using Haneke;
using UIKit;

namespace Board.Screens
{
	public class CreditsScreen : UIViewController
	{
		UIMenuBanner Banner;

		public override void ViewDidLoad ()
		{
			LoadBanner ();

			var argentinaFlag = CreateArgentinaFlag();
			argentinaFlag.Center = new CGPoint(AppDelegate.ScreenWidth / 2, AppDelegate.ScreenHeight - argentinaFlag.Frame.Height / 2);

			var settingsView = new CreditsView ();
			View.AddSubviews (settingsView, Banner, argentinaFlag);

			View.BackgroundColor = UIColor.White;
		}

		private UIImageView CreateArgentinaFlag(){
			var fullFlag = new UIImageView ();
			fullFlag.Frame = new CGRect (0, 0, AppDelegate.ScreenWidth, 36);

			fullFlag.BackgroundColor = UIColor.White;

			var topBlue = new UIImageView ();
			topBlue.Frame = new CGRect(0, 0, fullFlag.Frame.Width, fullFlag.Frame.Height / 3);
			topBlue.BackgroundColor = UIColor.FromRGB (114, 171, 225);

			var bottomBlue = new UIImageView ();
			bottomBlue.Frame = new CGRect(0, topBlue.Frame.Height * 2, fullFlag.Frame.Width, fullFlag.Frame.Height / 3);
			bottomBlue.BackgroundColor = UIColor.FromRGB (114, 171, 225);

			fullFlag.AddSubviews (topBlue, bottomBlue); 

			return fullFlag;
		}

		public override void ViewDidDisappear (bool animated)
		{
			MemoryUtility.ReleaseUIViewWithChildren (View);
		}

		class CreditsView : UIScrollView{

			public CreditsView(){
				Frame = new CGRect(0,0,AppDelegate.ScreenWidth, AppDelegate.ScreenHeight);

				var flagView = new UIImageView();
				flagView.Frame = new CGRect(0, 0, 150, 100);
				flagView.Center = new CGPoint(AppDelegate.ScreenWidth / 2, UIMenuBanner.Height * .75f + flagView.Frame.Height - 20);
				flagView.ContentMode = UIViewContentMode.ScaleAspectFit;
				flagView.SetImage("./screens/credits/long_flag.png");
				flagView.Layer.CornerRadius = 10;
				flagView.ClipsToBounds = true;
				flagView.Alpha = .95f;

				var thankYouLabel = new UILabel();
				thankYouLabel.Frame = new CGRect(10, flagView.Frame.Bottom + 20, AppDelegate.ScreenWidth - 20, 30);
				thankYouLabel.Text = "THANK YOU";
				thankYouLabel.Font = AppDelegate.Narwhal30;
				thankYouLabel.TextColor = AppDelegate.BoardOrange;
				thankYouLabel.TextAlignment = UITextAlignment.Center;

				var thankYou2Label = new UILabel();
				thankYou2Label.Frame = new CGRect(10, thankYouLabel.Frame.Bottom + 3, AppDelegate.ScreenWidth - 20, 24);
				thankYou2Label.Text = "FOR BEING ON BOARD";
				thankYou2Label.Font = AppDelegate.Narwhal24;
				thankYou2Label.TextColor = AppDelegate.BoardOrange;
				thankYou2Label.TextAlignment = UITextAlignment.Center;

				var jonathanLabel = new UILabel();
				jonathanLabel.Frame = new CGRect(10, thankYou2Label.Frame.Bottom + 35, AppDelegate.ScreenWidth - 20, 18);
				jonathanLabel.Text = "JONATHAN BURSZTYN";
				jonathanLabel.Font = AppDelegate.Narwhal16;
				jonathanLabel.TextColor = AppDelegate.BoardOrange;
				jonathanLabel.TextAlignment = UITextAlignment.Center;

				var jonathansubtitleLabel = new UILabel();
				jonathansubtitleLabel.Frame = new CGRect(10, jonathanLabel.Frame.Bottom, AppDelegate.ScreenWidth - 20, 18);
				jonathansubtitleLabel.Text = "Lead UI Engineer";
				jonathansubtitleLabel.Font = UIFont.SystemFontOfSize(14, UIFontWeight.Light);
				jonathansubtitleLabel.TextColor = UIColor.Black;
				jonathansubtitleLabel.TextAlignment = UITextAlignment.Center;

				var hernanLabel = new UILabel();
				hernanLabel.Frame = new CGRect(10, jonathansubtitleLabel.Frame.Bottom + 20, AppDelegate.ScreenWidth - 20, 18);
				hernanLabel.Text = "HERNAN FUENTES ARAUJO";
				hernanLabel.Font = AppDelegate.Narwhal16;
				hernanLabel.TextColor = AppDelegate.BoardOrange;
				hernanLabel.TextAlignment = UITextAlignment.Center;

				var hernansubtitleLabel = new UILabel();
				hernansubtitleLabel.Frame = new CGRect(10, hernanLabel.Frame.Bottom, AppDelegate.ScreenWidth - 20, 18);
				hernansubtitleLabel.Text = "Lead Infrastructure Engineer";
				hernansubtitleLabel.Font = UIFont.SystemFontOfSize(14, UIFontWeight.Light);
				hernansubtitleLabel.TextColor = UIColor.Black;
				hernansubtitleLabel.TextAlignment = UITextAlignment.Center;

				var channingLabel = new UILabel();
				channingLabel.Frame = new CGRect(10, hernansubtitleLabel.Frame.Bottom + 20, AppDelegate.ScreenWidth - 20, 18);
				channingLabel.Text = "Channing Miller";
				channingLabel.Font = AppDelegate.Narwhal16;
				channingLabel.TextColor = AppDelegate.BoardOrange;
				channingLabel.TextAlignment = UITextAlignment.Center;

				var channingsubtitleLabel = new UILabel();
				channingsubtitleLabel.Frame = new CGRect(10, channingLabel.Frame.Bottom, AppDelegate.ScreenWidth - 20, 18);
				channingsubtitleLabel.Text = "Head of Business Development";
				channingsubtitleLabel.Font = UIFont.SystemFontOfSize(14, UIFontWeight.Light);
				channingsubtitleLabel.TextColor = UIColor.Black;
				channingsubtitleLabel.TextAlignment = UITextAlignment.Center;

				var magaliLabel = new UILabel();
				magaliLabel.Frame = new CGRect(10, channingsubtitleLabel.Frame.Bottom + 20, AppDelegate.ScreenWidth - 20, 18);
				magaliLabel.Text = "Magali Bursztyn";
				magaliLabel.Font = AppDelegate.Narwhal16;
				magaliLabel.TextColor = AppDelegate.BoardOrange;
				magaliLabel.TextAlignment = UITextAlignment.Center;

				var magalisubtitleLabel = new UILabel();
				magalisubtitleLabel.Frame = new CGRect(10, magaliLabel.Frame.Bottom, AppDelegate.ScreenWidth - 20, 18);
				magalisubtitleLabel.Text = "Head of Communications";
				magalisubtitleLabel.Font = UIFont.SystemFontOfSize(14, UIFontWeight.Light);
				magalisubtitleLabel.TextColor = UIColor.Black;
				magalisubtitleLabel.TextAlignment = UITextAlignment.Center;

				var thanksLabel = new UILabel();
				thanksLabel.Frame = new CGRect(10, magalisubtitleLabel.Frame.Bottom + 20, AppDelegate.ScreenWidth - 20, 18);
				thanksLabel.Text = "THANKS";
				thanksLabel.Font = AppDelegate.Narwhal16;
				thanksLabel.TextColor = AppDelegate.BoardOrange;
				thanksLabel.TextAlignment = UITextAlignment.Center;

				var thanksView = new UITextView();
				thanksView.Frame = new CGRect(10, thanksLabel.Frame.Bottom - 5, AppDelegate.ScreenWidth - 20, 0);
				thanksView.Text = "Diego Zaks, Jessie Emanuel Katz, Alan Ispani, Ezequiel Levinton, " +
					"Andrés Alejandro Peña, Nathan Levinsky, Kevin Cooper, " +
					"Alex de Carvalho, Ingrid Pokropek, Manuel Seoane Torrealba, " +
					"Micaela Padron, Leonardo Rothpflug, Santiago Gonzalez, Leo Lob, Zack Samberg";
				
				thanksView.Font = UIFont.SystemFontOfSize(14, UIFontWeight.Light);
				thanksView.TextColor = UIColor.Black;
				thanksView.TextAlignment = UITextAlignment.Center;
				thanksView.Editable = false;
				var size2 = thanksView.SizeThatFits(thanksView.Frame.Size);
				thanksView.Frame = new CGRect(thanksView.Frame.X, thanksView.Frame.Y,
					thanksView.Frame.Width, size2.Height);
				
				AddSubviews(flagView, thankYouLabel, thankYou2Label, jonathanLabel, jonathansubtitleLabel,
					hernanLabel, hernansubtitleLabel, channingLabel, channingsubtitleLabel, magaliLabel, magalisubtitleLabel,
					thanksLabel, thanksView);

				ContentSize = new CGSize(AppDelegate.ScreenWidth, thanksView.Frame.Bottom + 36 * 2);
			}
		}


		private void LoadBanner()
		{
			Banner = new UIMenuBanner ("CREDITS", "arrow_left");

			bool taps = false;

			var tap = new UITapGestureRecognizer (tg => {
				if (taps){
					return;
				}

				if (tg.LocationInView(this.View).X < AppDelegate.ScreenWidth / 4){
					taps = true;

					var containerScreen = AppDelegate.NavigationController.ViewControllers[AppDelegate.NavigationController.ViewControllers.Length - 2] as ContainerScreen;
					if (containerScreen!= null)
					{
						containerScreen.LoadSettingsScreen();
					}
					AppDelegate.PopViewControllerWithCallback (delegate{
						MemoryUtility.ReleaseUIViewWithChildren (View);
					});
				}
			});

			Banner.AddTap (tap);
			Banner.SuscribeToEvents ();
		}

	}
}

