using System;
using CoreGraphics;

using Foundation;
using UIKit;

using MediaPlayer;
using Board.Schema;

namespace Board.Interface
{
	public partial class BoardInterface
	{

		private void GenerateTestPictures()
		{
			board.FBPage = new Board.Facebook.FacebookPage("SatelitesOficial", "Satelites Oficial", "Music Band");

			const float fix = 100;
			const float fix2 = 90;

			using (UIImage img = UIImage.FromFile ("./demo/pictures/0.jpg")) {
				AddTestPicture (img, new CGPoint(70+fix, 20+fix), -.03f, DateTime.Now.AddHours(-1));
			}

			AddTestVideo ("./demo/videos/0.mp4", new CGPoint(45+fix, 215+fix), -.01f, DateTime.Now.AddHours(-2));

			using (UIImage img = UIImage.FromFile ("./demo/pictures/2.jpg")) {
				AddTestPicture (img, new CGPoint(340+fix, 20+fix), 0f, DateTime.Now.AddMinutes(-31));
			}
			using (UIImage img = UIImage.FromFile ("./demo/pictures/1.jpg")) {
				AddTestPicture (img, new CGPoint(350+fix, 225+fix), -.03f, DateTime.Now.AddMinutes(-39));
			}

			AddTestVideo ("./demo/videos/1.mp4", new CGPoint(610+fix, 25+fix), -.02f, DateTime.Now.AddMinutes(-25));

			using (UIImage img = UIImage.FromFile ("./demo/pictures/3.jpg")) {
				AddTestPicture (img, new CGPoint(625+fix, 225+fix), .01f, DateTime.Now.AddMinutes(-2));
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
					"/la_roxtar\nhttp://www.facebook.com/laroxtar\nhttp://instagram.com/la_roxtar",img, new DateTime (2016, 3, 4, 23, 58, 0),new DateTime (2016, 3, 5, 7, 0, 0), new CGPoint (1700+fix+fix2, 29+fix), -.03f, DateTime.Now.AddDays(-7));
			}

			using (UIImage img = UIImage.FromFile ("./demo/events/1.jpg")) {
				AddTestEvent ("RIVERS in the Alley", string.Empty, img, new DateTime (2015, 12, 11, 21, 0, 0), new DateTime (2015, 12, 12, 3, 0, 0), new CGPoint (1955+fix+fix2, 30+fix), .02f, DateTime.Now.AddMinutes(-8));
			}

			using (UIImage img = UIImage.FromFile ("./demo/events/2.jpg")) {
				AddTestEvent ("Retirement Block Party", "Rad event on Jan 23rd! Be there-", img, new DateTime (2016, 1, 23, 14, 30, 0), new DateTime (2016, 1, 23, 17, 30, 0), new CGPoint (2210+fix+fix2, 27+fix), .02f, DateTime.Now.AddDays(-12));
			}

			using (UIImage img = UIImage.FromFile ("./demo/pictures/4.jpg")) {
				AddTestPicture (img, new CGPoint(50+fix, 410+fix), .03f, DateTime.Now.AddMinutes(-605));
			}

			AddTestVideo ("./demo/videos/2.mp4", new CGPoint(330+fix, 415+fix), -.02f, DateTime.Now.AddMinutes(-701));

			AddTestVideo ("./demo/videos/3.mp4", new CGPoint(615+fix, 420+fix), .0f, DateTime.Now.AddMinutes(-50));

			var firstAttributes = new UIStringAttributes {
				Font = AppDelegate.Narwhal20
			};

			var regularAttributes = new UIStringAttributes {
				Font = AppDelegate.SystemFontOfSize18
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

			AddTestAnnouncement (new CGPoint(1680+fix+fix2, 215+fix), -.02f, prettyString, DateTime.Now.AddMinutes(-12));

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

			AddTestAnnouncement (new CGPoint(2250+fix+fix2, 260+fix), .03f, prettyString2, DateTime.Now.AddMinutes(-125));

			// set different ranges to different styling!
			var prettyString3 = new NSMutableAttributedString ("OUR MOJITO GAME? STRONG");
			prettyString3.SetAttributes (regularAttributes.Dictionary, new NSRange (0, 17));
			prettyString3.SetAttributes (boldAttributes.Dictionary, new NSRange (17, 6));

			AddTestAnnouncement (new CGPoint(1950+fix+fix2, 210+fix), .01f, prettyString3, DateTime.Now.AddMinutes(-129));

			var prettyString4 = new NSMutableAttributedString("If you could travel through time where would you go?");
			prettyString4.SetAttributes(regularAttributes.Dictionary, new NSRange(0, prettyString4.Length));

			// set different ranges to different styling!
			AddTestPoll (new CGPoint(1678+fix+fix2, 414+fix), -.03f, prettyString4, DateTime.Now.AddMinutes(-35), "The future", "The past");

			//AddTestMap (new CGPoint(1960+fix+fix2, 400+fix), -.01f, null, DateTime.Now);
			//AddTestMap (new CGPoint(988, 135), -.0245f, null, DateTime.Now);

			AddTestVideo ("./demo/videos/3.mp4", new CGPoint(985, 335), -.04f, DateTime.Now.AddMinutes(-50));
			using (UIImage img = UIImage.FromFile ("./demo/pictures/1.jpg")) {
				AddTestPicture (img, new CGPoint (995, 520), -.04f, DateTime.Now.AddMinutes (-39));
			}

			using (UIImage img = UIImage.FromFile ("./demo/events/2.jpg")) {
				AddTestEvent ("Retirement Block Party", "Rad event on Jan 23rd! Be there-", img, new DateTime (2016, 1, 23, 14, 30, 0), new DateTime (2016, 1, 23, 17, 30, 0), new CGPoint (1550+fix2, 15+fix), -.04f, DateTime.Now.AddDays(-12));
			}

			using (UIImage img = UIImage.FromFile ("./demo/pictures/3.jpg")) {
				AddTestPicture (img, new CGPoint(1425+fix+fix2, 215+fix), .01f, DateTime.Now.AddMinutes (-39));
			}

			using (UIImage img = UIImage.FromFile ("./demo/pictures/4.jpg")) {
				AddTestPicture (img, new CGPoint(1510+fix2, 515), .01f, DateTime.Now.AddMinutes (-39));
			}
		}

		private void AddTestMap(CGPoint position, float rotation, string creatorid, DateTime creationdate){
			Map map = new Map (rotation, position, creatorid, creationdate);
			DictionaryContent.Add (map.Id, map);
		}

		private void AddTestPoll(CGPoint position, float rotation, NSMutableAttributedString text, DateTime creationDate, params string[] answers)
		{
			Poll poll = new Poll (text, rotation, position, null, creationDate, answers);
			DictionaryContent.Add (poll.Id, poll);
		}

		private void AddTestAnnouncement(CGPoint position, float rotation, NSMutableAttributedString text, DateTime creationDate)
		{
			Announcement ann = new Announcement (text, rotation, position, null, creationDate);
			DictionaryContent.Add (ann.Id, ann);
		}

		private void AddTestEvent(string name, string description, UIImage img, DateTime startdate, DateTime enddate, CGPoint position, float rotation, DateTime creationDate)
		{
			BoardEvent bevent = new BoardEvent (name, img, startdate, enddate, rotation, position, null, creationDate);
			bevent.Rotation = rotation;
			bevent.Description = description;
			DictionaryContent.Add (bevent.Id, bevent);
		}

		private void AddTestPicture(UIImage image, CGPoint center, float rotation, DateTime creationDate)
		{
			Picture pic = new Picture ();
			pic.ImageView = new UIImageView(image);
			pic.CreationDate = creationDate;
			pic.Center = center;
			pic.Rotation = rotation;
			DictionaryContent.Add (pic.Id, pic);
		}

		private void AddTestVideo(string url, CGPoint center, float rotation, DateTime creationDate)
		{
			Video vid = new Video ();

			using (MPMoviePlayerController moviePlayer = new MPMoviePlayerController (NSUrl.FromFilename (url))) {
				vid.ThumbnailView = new UIImageView(moviePlayer.ThumbnailImageAt (0, MPMovieTimeOption.Exact));
				moviePlayer.Pause ();
				moviePlayer.Dispose ();	
			}

			vid.Url = NSUrl.FromFilename (url);
			vid.UrlText = vid.Url.AbsoluteString;
			vid.Center = center;
			vid.CreationDate = creationDate;
			vid.Rotation = rotation;

			DictionaryContent.Add (vid.Id, vid);
		}

			
	}
}

