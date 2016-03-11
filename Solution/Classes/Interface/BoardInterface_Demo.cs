using System;
using CoreGraphics;
using System.Linq;

using Foundation;
using UIKit;

using System.Collections.Generic;

using MediaPlayer;

using Facebook.CoreKit;
using Board.Interface.Buttons;
using Board.Interface.Widgets;
using Board.Interface;
using Board.Schema;

using BigTed;

namespace Board.Interface
{
	public partial class BoardInterface
	{

		private void GenerateTestPictures()
		{
			using (UIImage img = UIImage.FromFile ("./demo/pictures/0.jpg")) {
				AddTestPicture (img, 70, 20, -.03f, DateTime.Now.AddHours(-1));
			}

			AddTestVideo ("./demo/videos/0.mp4", 45, 220, -.01f, DateTime.Now.AddHours(-2));

			using (UIImage img = UIImage.FromFile ("./demo/pictures/2.jpg")) {
				AddTestPicture (img, 340, 20, 0f, DateTime.Now.AddMinutes(-31));
			}
			using (UIImage img = UIImage.FromFile ("./demo/pictures/1.jpg")) {
				AddTestPicture (img, 360, 220, -.04f, DateTime.Now.AddMinutes(-39));
			}

			AddTestVideo ("./demo/videos/1.mp4", 610, 25, -.02f, DateTime.Now.AddMinutes(-25));

			using (UIImage img = UIImage.FromFile ("./demo/pictures/3.jpg")) {
				AddTestPicture (img, 655, 225, .01f, DateTime.Now.AddMinutes(-2));
			}

			using (UIImage img = UIImage.FromFile ("./demo/events/0.jpg")) {
				AddTestEvent ("La Roxtar", "★ LA.ROXTAR ★\nTodos los viernes en The Roxy Live\n\nLa fiesta de los viernes en " +
					"Palermo que te invita a bailar toda la noche con los nuevos clásicos del Indie Rock, los himnos eternos " +
					"del Rock y las nuevas tendencias que se escuchan en el resto del mundo. \n\n◥MUSIC◣\n☠ BLAKK (INDIE + ROCK " +
					"+ BRIT + PUNK + METAL)\n☠ BUDAH (FUNK + ROCK + INDIE + DISCO)\n\n\n◥VISUALES◣\n☠ Vj PABLO PAZ\n\n◥FOTOS◣\n☠ " +
					"PH CSA Photography\n\n◥SHOWS EN VIVO desde las 00hs◣\nLas mejores bandas de la escena local, cada viernes " +
					"tocando en uno de los mejores escenarios de la ciudad: THE ROXY\n\n\n♫ ♫ La ciudad bajo la niebla + Regios " +
					"♫ ♫\nLA CIUDAD BAJO LA NIEBLA presenta su show despedida, antes de su primer gira por México, interpretando " +
					"temas de su próximo disco, grabado recientemente en estudios Panda. Los acompañaran REGIOS, la nueva banda de " +
					"rock que se presentara por primera vez en LA ROXTAR en THE ROXY LIVE.\n\nÚnicamente con entrada\n" +
					"http://www.ticketek.com.ar/la-roxtar-shows-lcbln-regios/roxy-live\n\n- Entradas anticipadas $50 por sistema TICKETEK:" +
					"\n- Entradas en puerta $70\n\nLinks LA CIUDAD BAJO LA NIEBLA:\nSitio Web:http://www.laciudadbajolaniebla.com/\nFacebook: " +
					"https://www.facebook.com/La-ciudad-bajo-la-niebla-110692649446/?fref=ts\nTwitter:https://twitter.com/LCBLN?lang=es\n" +
					"Bandcamp:https://laciudadbajolaniebla.bandcamp.com/\nYoutube:https://www.youtube.com/user/laciudadbajolaniebla\n\n" +
					"--------------------------------\n\nLinks REGIOS:\nFacebook:https://www.facebook.com/regiosmusica/?fref=ts\n" +
					"Youtube:https://www.youtube.com/channel/UCbRoGtOnAelMnY-3yM6wwXQ\n\n◣◥◣◥◣◥◣◥◣◥◣◥◣◥◣◥◣◥◣◥◣◥◣◥◣◥◣◥◣◥◣◥◣\n\n\nLA.ROXTAR " +
					"suena a:\nArcticMonkeys + Queen + TheStrokes + TheBeatles + Kingsofleon + TheRamones + FranzFerdinand + The Clash + A" +
					"rcadeFire + TheBravery + TheKooks + Morrissey + Phoenix + The Doors + Kasabian + VampireWeekend + y mucho mas...\n\n\n☠ " +
					"PRECIOS ENTRADAS ROXTAR 2016 ☠\n-00hs a 1.30am: ENTRADA SHOWS (ant $50/ $Puerta$70)\n-1.30 a 2.30am: MARZO FREE (M" +
					"UJERES FREE, HOMBRES 2x1)\n-Desde 2.30am: ENTRADA FIESTA $80 c/cerverza o $100 c/trago\n\n\n-ENTRADA FIESTA\n$80 c/co" +
					"nsumición de Cerveza/agua/gaseosa\n$100 c/consumición de Trago (Fernet/Whiscola/Destornillador)\nSolo mayores de 18 añ" +
					"os\n\n\nMARZO FREE\n-Mujeres GRATIS de 1.30 a 2.30AM (No válido para los shows)\n-Hombres 2x1 de 1.30 a 2.30AM (No vál" +
					"ido para los shows)\n\n\nCUMPLEAÑOS:\nPara festejar tu cumple en LA ROXTAR manda un mail a fiestaroxtar@gmail.com y te " +
					"contamos los beneficios y descuentos especiales para tu noche de cumpleaños!!\n\n\nSeguinos en:\nhttp://www.twitter.com" +
					"/la_roxtar\nhttp://www.facebook.com/laroxtar\nhttp://instagram.com/la_roxtar",img, new DateTime (2016, 3, 4, 23, 58, 0),new DateTime (2016, 3, 5, 7, 0, 0), new CGRect (1650, 29, 0, 0), -.03f, DateTime.Now.AddDays(-7));
			}


			using (UIImage img = UIImage.FromFile ("./demo/events/1.jpg")) {
				AddTestEvent ("RIVERS in the Alley", string.Empty, img, new DateTime (2015, 12, 11, 21, 0, 0), new DateTime (2015, 12, 12, 3, 0, 0), new CGRect (1910, 30, 0, 0), .02f, DateTime.Now.AddMinutes(-8));
			}

			using (UIImage img = UIImage.FromFile ("./demo/events/2.jpg")) {
				AddTestEvent ("Retirement Block Party", "Rad event on Jan 23rd! Be there-", img, new DateTime (2016, 1, 23, 14, 30, 0), new DateTime (2016, 1, 23, 17, 30, 0), new CGRect (2170, 27, 0, 0), .02f, DateTime.Now.AddDays(-12));
			}

			using (UIImage img = UIImage.FromFile ("./demo/pictures/4.jpg")) {
				AddTestPicture (img, 50, 420, .03f, DateTime.Now.AddMinutes(-605));
			}

			AddTestVideo ("./demo/videos/2.mp4", 330, 415, -.02f, DateTime.Now.AddMinutes(-701));

			AddTestVideo ("./demo/videos/3.mp4", 635, 420, .0f, DateTime.Now.AddMinutes(-50));

			var firstAttributes = new UIStringAttributes {
				Font = UIFont.FromName("narwhal-bold", 18f)
			};

			var regularAttributes = new UIStringAttributes {
				Font = UIFont.SystemFontOfSize (18)
			};

			var boldAttributes = new UIStringAttributes {
				Font = UIFont.BoldSystemFontOfSize (18)
			};

			var biggerBoldAttributes = new UIStringAttributes {
				Font = UIFont.BoldSystemFontOfSize (20)
			};

			// set different ranges to different styling!
			var prettyString = new NSMutableAttributedString ("BOARD IS PRETTY!");
			prettyString.SetAttributes (firstAttributes.Dictionary, new NSRange (0, 5));
			prettyString.SetAttributes (regularAttributes.Dictionary, new NSRange (5, 3));
			prettyString.SetAttributes (boldAttributes.Dictionary, new NSRange (8, 8));

			AddTestAnnouncement (new CGRect (1700, 235, 0, 0), -.02f, prettyString, DateTime.Now.AddMinutes(-12));

			const string intro = "Introducing Board\n\nA better way of finding a good time\n\n";
			const string bigText1 = "Promos / Deals\n\nAccess to promotions and discounts. Restaurants, bars and clubs will post deals and specials. Have a good time without hurting your wallet.\n\n";
			const string bigText2 = "Guest List\n\nReceive free entry to clubs and bars around your town, available exclusively on the Board App.\n\n";
			const string bigText3 = "Menus / Specials\n\nCheck out daily specials happening in your area. Browse menus, photos, and happy hours.\n\n";
			const string bigText4 = "Entertainment & Events\n\nFind out the best places for live music, DJ's, and events. Receive up-to-date notifications for fun times in your area.";

			string composite = intro + bigText1 + bigText2 + bigText3 + bigText4;

			// set different ranges to different styling!
			var prettyString2 = new NSMutableAttributedString (composite);
			prettyString2.SetAttributes (boldAttributes.Dictionary, new NSRange (0, 17));
			prettyString2.SetAttributes (regularAttributes.Dictionary, new NSRange (17, intro.Length - 17));

			prettyString2.SetAttributes (boldAttributes.Dictionary, new NSRange (intro.Length, 14));
			prettyString2.SetAttributes (regularAttributes.Dictionary, new NSRange (intro.Length + 14, bigText1.Length - 14));

			prettyString2.SetAttributes (boldAttributes.Dictionary, new NSRange (intro.Length + bigText1.Length, 10));
			prettyString2.SetAttributes (regularAttributes.Dictionary, new NSRange (intro.Length +bigText1.Length + 10, bigText2.Length - 10));

			prettyString2.SetAttributes (boldAttributes.Dictionary, new NSRange (intro.Length + bigText1.Length + bigText2.Length, 16));
			prettyString2.SetAttributes (regularAttributes.Dictionary, new NSRange (intro.Length + bigText1.Length + bigText2.Length + 16, bigText3.Length - 16));

			prettyString2.SetAttributes (boldAttributes.Dictionary, new NSRange (intro.Length + bigText1.Length + bigText2.Length + bigText3.Length, 22));
			prettyString2.SetAttributes (regularAttributes.Dictionary, new NSRange (intro.Length + bigText1.Length + bigText2.Length + bigText3.Length + 22, bigText4.Length - 23));

			AddTestAnnouncement (new CGRect (2220, 240, 0, 0), .03f, prettyString2, DateTime.Now.AddMinutes(-125));

			// set different ranges to different styling!
			var prettyString3 = new NSMutableAttributedString ("OUR MOJITO GAME? STRONG");
			prettyString3.SetAttributes (regularAttributes.Dictionary, new NSRange (0, 17));
			prettyString3.SetAttributes (boldAttributes.Dictionary, new NSRange (17, 6));

			AddTestAnnouncement (new CGRect (1930, 245, 0, 0), .01f, prettyString3, DateTime.Now.AddMinutes(-129));

			// set different ranges to different styling!
			AddTestPoll (new CGRect (1615, 390, 0, 0), .01f, "Couch potato or potato couch?", DateTime.Now.AddMinutes(-35), "Couch Potato", "Potato Couch");

			AddTestMap (new CGRect (1930, 390, 0, 0), -.01f, null, DateTime.Now);
		}

		private void AddTestMap(CGRect frame, float rotation, string creatorid, DateTime creationdate){
			Map map = new Map (rotation, frame, creatorid, creationdate);
			DictionaryContent.Add (map.Id, map);
		}

		private void AddTestPoll(CGRect frame, float rotation, string text, DateTime creationDate, params string[] answers)
		{
			Poll poll = new Poll (text, rotation, frame, null, creationDate, answers);
			DictionaryContent.Add (poll.Id, poll);
		}

		private void AddTestAnnouncement(CGRect frame, float rotation, NSMutableAttributedString text, DateTime creationDate)
		{
			Announcement ann = new Announcement ();
			ann.Frame = frame;
			ann.Rotation = rotation;
			ann.CreationDate = creationDate;
			ann.Text = new NSAttributedString (text);
			DictionaryContent.Add (ann.Id, ann);
		}

		private void AddTestEvent(string name, string description, UIImage img, DateTime startdate, DateTime enddate, CGRect frame, float rotation, DateTime creationDate)
		{
			BoardEvent bevent = new BoardEvent (name, img, startdate, enddate, rotation, frame, null, creationDate);
			bevent.Rotation = rotation;
			bevent.Description = description;
			DictionaryContent.Add (bevent.Id, bevent);
		}

		private void AddTestPicture(UIImage image, float imgx, float imgy, float rotation, DateTime creationDate)
		{
			Picture pic = new Picture ();
			pic.ImageView = new UIImageView(image);
			pic.CreationDate = creationDate;
			pic.Frame = new CGRect(imgx, imgy, 0, 0);
			pic.Rotation = rotation;
			DictionaryContent.Add (pic.Id, pic);
		}

		private void AddTestVideo(string url, float imgx, float imgy, float rotation, DateTime creationDate)
		{
			Video vid = new Video ();

			using (MPMoviePlayerController moviePlayer = new MPMoviePlayerController (NSUrl.FromFilename (url))) {
				vid.ThumbnailView = new UIImageView(moviePlayer.ThumbnailImageAt (0, MPMovieTimeOption.Exact));
				moviePlayer.Pause ();
				moviePlayer.Dispose ();	
			}

			vid.Url = NSUrl.FromFilename (url);
			vid.Frame = new CGRect(imgx, imgy, 0, 0);
			vid.CreationDate = creationDate;
			vid.Rotation = rotation;

			DictionaryContent.Add (vid.Id, vid);
		}

			
	}
}

