using System;
using System.Collections.Generic;
using System.Linq;
using CoreGraphics;
using Facebook.CoreKit;
using Foundation;
using Google.Maps;
using UIKit;
using Board.Screens.Controls;

namespace Board.Screens
{
	public partial class MainMenuScreen : UIViewController
	{
		private static List<Board.Schema.Board> GenerateBoardList()
		{
			List<Board.Schema.Board> boardList = new List<Board.Schema.Board> ();

			using (UIImage img = UIImage.FromFile("./logos/americansocial.jpeg")){
				boardList.Add(new Board.Schema.Board ("American Social", img, UIColor.FromRGB (67, 15, 0), UIColor.FromRGB (221, 169, 91), "Brickell", Profile.CurrentProfile.UserID));
			}
			using (UIImage img = UIImage.FromFile("./logos/doghouse.jpeg")){
				boardList.Add(new Board.Schema.Board ("Dog House", img, UIColor.FromRGB (35, 32, 35), UIColor.FromRGB (220, 31, 24), "Brickell", string.Empty));
			}
			using (UIImage img = UIImage.FromFile("./logos/doloreslolita.jpg")){
				boardList.Add(new Board.Schema.Board ("Dolores Lolita", img, UIColor.FromRGB (185, 143, 6), UIColor.FromRGB (2, 0, 6), "Brickell", string.Empty));	
			}
			using (UIImage img = UIImage.FromFile ("./logos/bluemartini.png")) {
				boardList.Add (new Board.Schema.Board ("Blue Martini", img, UIColor.FromRGB (0, 0, 0), UIColor.FromRGB (0, 165, 216), "Brickell", string.Empty));
			}
			using (UIImage img = UIImage.FromFile ("./logos/tavernopa.png")) {
				boardList.Add (new Board.Schema.Board ("Taverna Opa", img, UIColor.FromRGB (140, 52, 50), UIColor.FromRGB (77, 185, 155), "Brickell", string.Empty));	
			}
			using (UIImage img = UIImage.FromFile ("./logos/clevelander.png")) {
				boardList.Add (new Board.Schema.Board ("Clevelander", img, UIColor.FromRGB (0, 158, 216), UIColor.FromRGB (158, 208, 96), "South Beach", string.Empty));
			}
			using (UIImage img = UIImage.FromFile ("./logos/fattuesdays.jpg")) {
				boardList.Add (new Board.Schema.Board ("Fat Tuesdays", img, UIColor.FromRGB (52, 59, 155), UIColor.FromRGB (201, 30, 67), "South Beach", string.Empty));	
			}
			using (UIImage img = UIImage.FromFile ("./logos/liv.jpg")) {
				boardList.Add (new Board.Schema.Board ("LIV", img, UIColor.Black, UIColor.FromRGB (20, 20, 20), "South Beach", string.Empty));
			}
			using (UIImage img = UIImage.FromFile ("./logos/mangos.png")) {
				boardList.Add (new Board.Schema.Board ("Mangos", img, UIColor.FromRGB (240, 35, 0), UIColor.FromRGB (0, 168, 67), "South Beach", string.Empty));
			}
			using (UIImage img = UIImage.FromFile ("./logos/coyo.jpeg")) {
				boardList.Add (new Board.Schema.Board ("Coyo", img, UIColor.FromRGB (33, 58, 171), UIColor.FromRGB (100, 215, 223), "Wynwood", string.Empty));	
			}
			using (UIImage img = UIImage.FromFile ("./logos/mansion.jpg")) {
				boardList.Add (new Board.Schema.Board ("Mansion", img, UIColor.Black, UIColor.FromRGB (20, 20, 20), "South Beach", string.Empty));
			}
			using (UIImage img = UIImage.FromFile ("./logos/nikki.jpg")) {
				boardList.Add (new Board.Schema.Board ("Nikki Beach", img, UIColor.FromRGB (1, 73, 159), UIColor.White, "South Beach", string.Empty));
			}
			using (UIImage img = UIImage.FromFile ("./logos/wetwillies.jpg")) {
				boardList.Add (new Board.Schema.Board ("Wet Willies", img, UIColor.Black, UIColor.White, "South Beach", string.Empty));	
			}
			using (UIImage img = UIImage.FromFile ("./logos/brickhouse.png")) {
				boardList.Add (new Board.Schema.Board ("Brickhouse", img, UIColor.FromRGB (35, 30, 32), UIColor.Black, "Wynwood", string.Empty));	
			}
			using (UIImage img = UIImage.FromFile ("./logos/panther.JPG")) {
				boardList.Add (new Board.Schema.Board ("Panther Coffee", img, UIColor.Black, UIColor.FromRGB (20, 20, 20), "Wynwood", string.Empty));	
			}
			using (UIImage img = UIImage.FromFile("./logos/wood.png")){
				boardList.Add (new Board.Schema.Board ("Wood Tavern", img, UIColor.Black, UIColor.FromRGB (20, 20, 20), "Wynwood", string.Empty));
			}
			using (UIImage img = UIImage.FromFile ("./logos/electricpickle.jpg")) {
				boardList.Add (new Board.Schema.Board ("Electric Pickle", img, UIColor.Black, UIColor.FromRGB (20, 20, 20), "Wynwood", string.Empty));	
			}

			return boardList;
		}  
	}
}

