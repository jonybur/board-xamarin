using System.Collections.Generic;
using BigTed;
using Board.Infrastructure;
using Board.Screens.Controls;
using Board.Utilities;
using CoreGraphics;
using UIKit;

namespace Board.Screens
{
	public class BusinessScreen : UIViewController
	{
		UIMenuBanner Banner;
		UIScrollView ScrollView;
		List<Board.Schema.Board> boardList;
		UIThumbsContentDisplay ThumbsScreen;

		public override void ViewDidLoad ()
		{
			BTProgressHUD.Show ();
			boardList = new List<Board.Schema.Board> ();
			ScrollView = new UIScrollView (new CGRect (0, 0, AppDelegate.ScreenWidth, AppDelegate.ScreenHeight));
			ScrollView.BackgroundColor = UIColor.White;

			View.AddSubview (ScrollView);

			LoadBanner ();
		}

		public override void ViewDidAppear(bool animated)
		{
			Banner.SuscribeToEvents ();

			BTProgressHUD.Show ();

			boardList = CloudController.GetUserBoards ();
			InitializeInterface ();

			BTProgressHUD.Dismiss ();
		}

		public override void ViewDidDisappear(bool animated)
		{
			Banner.UnsuscribeToEvents ();

			if (ThumbsScreen != null) {
				ThumbsScreen.UnsuscribeToEvents ();
			}

			MemoryUtility.ReleaseUIViewWithChildren (View);
		}

		private void InitializeInterface()
		{
			foreach (UIView v in ScrollView.Subviews) {
				v.RemoveFromSuperview ();
			}

			if (boardList.Count == 0) {
				LoadNoContent ();
			} else {
				LoadContent ();
			}
		}

		private void LoadContent()
		{
			ThumbsScreen = new UIThumbsContentDisplay (boardList, UIThumbsContentDisplay.OrderMode.Neighborhood);
			ScrollView.AddSubview (ThumbsScreen);
			ScrollView.ContentSize = new CGSize (AppDelegate.ScreenWidth, ThumbsScreen.Frame.Bottom);

			ScrollView.ScrollEnabled = true;
			ScrollView.UserInteractionEnabled = true;
		}

		private void LoadNoContent()
		{
			var contactView = new UIContactView (UIContactView.ScreenContact.BusinessScreen);
			View.AddSubview (contactView);

		}

		private void LoadBanner()
		{
			Banner = new UIMenuBanner ("BUSINESS", "menu_left");

			UITapGestureRecognizer tap = new UITapGestureRecognizer (tg => {
				if (tg.LocationInView(this.View).X < AppDelegate.ScreenWidth / 4){
					AppDelegate.containerScreen.BringSideMenuUp("business");
				}
				/*else if (AppDelegate.ScreenWidth / 4 * 3 < tg.LocationInView(this.View).X){

					UIAlertController alert = UIAlertController.Create("Facebook Page Importer", null, UIAlertControllerStyle.Alert);
					alert.AddAction (UIAlertAction.Create ("Cancel", UIAlertActionStyle.Cancel, null));
					alert.AddAction (UIAlertAction.Create ("OK", UIAlertActionStyle.Default, delegate {

						if (alert.TextFields.Length == 0){
							return;
						}

						var textField = alert.TextFields[0];

						if (textField.Text == string.Empty || textField.Text == null){ 
							return;
						}

						Board.Facebook.FacebookAutoImporter.ImportPage(textField.Text);
					}));

					alert.AddTextField(delegate(UITextField obj) {

						obj.Placeholder = "Facebook Page ID";
					});

					NavigationController.PresentViewController(alert, true, null);
				}*/
			});

			Banner.AddTap (tap);

			View.AddSubview (Banner);
		}

		/*
		BUENOS AIRES BOARDS:
		"973824289314042", "muulecheria", "urbanstationba", "chupitosbar", "barsoria",
		"TheRoxyArg", "latrastiendaclubBSAS", "tazzbars",
		"budabar.bsas", "lafabricadeltaco", "SheldonPub", "Sullivans.irish.bar",
		"BurgerJointPalermo", "FelixFelicisCoffee", 
		"SocialParaiso", "cronicobar", "thames.galeria", "lodejesus",
		"templebarargentina", "SugarBuenosAires"
		*/

		/*
		NANTUCKET BOARDS: 
		"crunantucket", "ThePearlNantucket", "straightwharfrestaurant", "TheBoardingHouseNantucket",
		"TheBoxNantucket", "ackbackyardbbq", "Lola41Nantucket", "lolaburger", "thenautilusnantucket",
		"milliesrestaurant", "183044393276", "107261144123", "fogislandcafe", "ProprietorsBarTable",
		"1061400640541340", "TheClubCarNantucket", "177475355647426", "galleybeachnantucket", "CiscoBrewers",
		"108013392550036", "slip14nantucket", "ACKTrap", "108593769177258", "118917288160401", "theroseandcrownma",
		"TOPPERS.Wauwinet", "WhiteElephantHotel", "TheNantucket", "NantucketIslandSurfSchool", "ackstubbys", 
		"AmericanSeasons", "blackeyedsusans.nantucket", "264846700041", "73131111727", "OranMorBistro", 
		"272985902713182", "LanguedocBistro", "CompanyOfTheCauldron", "AtlasNantucket", "handlebarcafe",
		"NativMade", "PiPizzeriaNantucket", "ventunoresto", "lemonpressnantucket"*/


	}
}