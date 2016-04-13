using System.Collections.Generic;
using Facebook.CoreKit;
using UIKit;

namespace Board.Screens
{
	public partial class MainMenuScreen
	{
		private static List<Board.Schema.Board> GenerateBoardList()
		{
			List<Board.Schema.Board> boardList = new List<Board.Schema.Board> ();
			/*
			using (UIImage img = UIImage.FromFile("./logos/americansocial.png")){
				Board.Schema.Board board = new Board.Schema.Board ("American Social", new UIImageView(img), UIColor.FromRGB (0, 6, 67), UIColor.FromRGB (177, 23, 0), "Brickell", string.Empty);
				boardList.Add(board);
			}
			using (UIImage img = UIImage.FromFile("./logos/doghouse.jpeg")){
				boardList.Add(new Board.Schema.Board ("Dog House", new UIImageView(img), UIColor.FromRGB (35, 32, 35), UIColor.FromRGB (220, 31, 24), "Brickell", string.Empty));
			}
			using (UIImage img = UIImage.FromFile("./logos/doloreslolita.jpg")){
				Board.Schema.Board demoboard = new Board.Schema.Board ("Dolores Lolita", new UIImageView(img), UIColor.FromRGB (185, 143, 6), UIColor.FromRGB (2, 0, 6), "Brickell", string.Empty);	
				boardList.Add (demoboard);
			}
			using (UIImage img = UIImage.FromFile ("./logos/bluemartini.png")) {
				boardList.Add (new Board.Schema.Board ("Blue Martini", new UIImageView(img), UIColor.FromRGB (0, 0, 0), UIColor.FromRGB (0, 165, 216), "Brickell", string.Empty));
			}
			using (UIImage img = UIImage.FromFile ("./logos/tavernopa.png")) {
				Board.Schema.Board demoboard = new Board.Schema.Board ("Taverna Opa", new UIImageView(img), UIColor.FromRGB (140, 52, 50), UIColor.FromRGB (77, 185, 155), "Brickell", string.Empty);
				boardList.Add (demoboard);
			}
			using (UIImage img = UIImage.FromFile ("./logos/clevelander.png")) {
				boardList.Add (new Board.Schema.Board ("Clevelander", new UIImageView(img), UIColor.FromRGB (0, 158, 216), UIColor.FromRGB (158, 208, 96), "South Beach", string.Empty));
			}
			using (UIImage img = UIImage.FromFile ("./logos/fattuesdays.jpg")) {
				boardList.Add (new Board.Schema.Board ("Fat Tuesdays", new UIImageView(img), UIColor.FromRGB (52, 59, 155), UIColor.FromRGB (201, 30, 67), "South Beach", string.Empty));	
			}
			using (UIImage img = UIImage.FromFile ("./logos/liv.jpg")) {
				boardList.Add (new Board.Schema.Board ("LIV", new UIImageView(img), UIColor.Black, UIColor.FromRGB (20, 20, 20), "South Beach", string.Empty));
			}
			using (UIImage img = UIImage.FromFile ("./logos/mangos.png")) {
				boardList.Add (new Board.Schema.Board ("Mangos", new UIImageView(img), UIColor.FromRGB (240, 35, 0), UIColor.FromRGB (0, 168, 67), "South Beach", string.Empty));
			}
			using (UIImage img = UIImage.FromFile ("./logos/coyo.jpeg")) {
				boardList.Add (new Board.Schema.Board ("Coyo", new UIImageView(img), UIColor.FromRGB (33, 58, 171), UIColor.FromRGB (100, 215, 223), "Wynwood", string.Empty));	
			}
			using (UIImage img = UIImage.FromFile ("./logos/mansion.jpg")) {
				boardList.Add (new Board.Schema.Board ("Mansion", new UIImageView(img), UIColor.Black, UIColor.FromRGB (20, 20, 20), "South Beach", string.Empty));
			}
			using (UIImage img = UIImage.FromFile ("./logos/wetwillies.jpg")) {
				boardList.Add (new Board.Schema.Board ("Wet Willies", new UIImageView(img), UIColor.Black, UIColor.White, "South Beach", string.Empty));	
			}
			using (UIImage img = UIImage.FromFile ("./logos/brickhouse.png")) {
				boardList.Add (new Board.Schema.Board ("Brickhouse", new UIImageView(img), UIColor.FromRGB (35, 30, 32), UIColor.Black, "Wynwood", string.Empty));	
			}
			using (UIImage img = UIImage.FromFile ("./logos/panther.JPG")) {
				boardList.Add (new Board.Schema.Board ("Panther Coffee", new UIImageView(img), UIColor.Black, UIColor.FromRGB (20, 20, 20), "Wynwood", string.Empty));	
			}
			using (UIImage img = UIImage.FromFile("./logos/wood.png")){
				boardList.Add (new Board.Schema.Board ("Wood Tavern", new UIImageView(img), UIColor.Black, UIColor.FromRGB (20, 20, 20), "Wynwood", string.Empty));
			}
			using (UIImage img = UIImage.FromFile ("./logos/electricpickle.jpg")) {
				boardList.Add (new Board.Schema.Board ("Electric Pickle", new UIImageView(img), UIColor.Black, UIColor.FromRGB (20, 20, 20), "Wynwood", string.Empty));	
			}*/

			return boardList;
		}  
	}
}

