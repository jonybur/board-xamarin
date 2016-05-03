using System;
using UIKit;
using MGImageUtilitiesBinding;
using System.Linq;
using System.Collections.Generic;
using Board.Interface;
using CoreGraphics;

namespace Board.Screens.Controls
{
	public class UIThumbsContentDisplay : UIContentDisplay
	{
		private interface IBoardComparer : IComparer<Board.Schema.Board> {
			string GetComparisonPropertyDescription(Board.Schema.Board target);
		}

		private class AlphabeticComparer : IBoardComparer
		{
			public int Compare (Board.Schema.Board x, Board.Schema.Board y)
			{
				return String.Compare(x.Name[0].ToString(), y.Name[0].ToString());
			}

			public string GetComparisonPropertyDescription(Board.Schema.Board target) {
				return target.Name [0].ToString ();
			}
		}

		private class NeighbourhoodComparer : IBoardComparer
		{
			public int Compare (Board.Schema.Board x, Board.Schema.Board y)
			{
				return String.Compare(x.GeolocatorObject.Neighborhood, y.GeolocatorObject.Neighborhood);
			}

			public string GetComparisonPropertyDescription(Board.Schema.Board target) {
				return target.GeolocatorObject.Neighborhood;
			}
		}

		public readonly float ThumbSize;
		public enum OrderMode { Neighborhood = 0, Alphabetic, Distance }
		public const int TopAndBottomSeparation = 20;

		public List<UIBoardThumbComponent> ListThumbComponents;

		private IBoardComparer _boardComparer;
		private Dictionary<OrderMode, IBoardComparer> _boardComparersByMode; 

		public UIThumbsContentDisplay (List<Board.Schema.Board> boardList, OrderMode mode, float extraTopMargin = 0, float extraLowMargin = 0)
		{
			if (boardList.Count == 0) {
				return;
			} 

			ThumbSize = AppDelegate.ScreenWidth / 3.5f;
			ListThumbs = new List<UIContentThumb> ();
			float yposition = TopAndBottomSeparation + UIMenuBanner.MenuHeight + extraTopMargin;

			this._boardComparersByMode = new Dictionary<OrderMode, IBoardComparer> ();
			this._boardComparersByMode.Add (OrderMode.Alphabetic, new AlphabeticComparer ());
			this._boardComparersByMode.Add (OrderMode.Neighborhood, new NeighbourhoodComparer ());
			this._boardComparer = this._boardComparersByMode [mode];

			ListThumbComponents = new List<UIBoardThumbComponent> ();

			boardList = boardList.OrderBy(x => x, this._boardComparer).ToList();

			Board.Schema.Board comparer = boardList[0];

			// starting point
			int linecounter = 1, sectionNumber = 0, i = 0;

			foreach (Board.Schema.Board b in boardList) {
				if (this._boardComparer.Compare(comparer, b) != 0 || i == 0) {

					if (sectionNumber > 0) {
						yposition += ThumbSize / 2 + UIBoardThumbComponent.TextSpace + 10;
					}

					// draw new location string
					var locationLabel = new UILocationLabel (this._boardComparer.GetComparisonPropertyDescription(b), 
						new CGPoint(UICarouselController.ItemSeparation, yposition), UITextAlignment.Center);
					
					yposition += (float)locationLabel.Frame.Height + ThumbSize / 2 + 10;
					comparer = b;
					AddSubview(locationLabel);

					linecounter = 1;
					sectionNumber++;
				}

				if (linecounter >= 4) {
					linecounter = 1;
					// nueva linea de thumbs
					yposition += ThumbSize + UIBoardThumbComponent.TextSpace;
				}

				var btComponent = new UIBoardThumbComponent (b, new CGPoint ((AppDelegate.ScreenWidth/ 4) * linecounter, yposition), ThumbSize);
				ListThumbs.Add (btComponent.BoardThumb);
				ListThumbComponents.Add (btComponent);
				AddSubview (btComponent);
				linecounter++;
				i++;
			}

			UserInteractionEnabled = true;
			Frame = new CGRect (0, 0, AppDelegate.ScreenWidth, yposition + ThumbSize / 2 + TopAndBottomSeparation + extraLowMargin + UIBoardThumbComponent.TextSpace);
		}

	}
}

