using UIKit;
using MGImageUtilitiesBinding;
using System.Linq;
using System.Collections.Generic;
using Board.Interface;
using CoreGraphics;
using Board.Utilities;

namespace Board.Screens.Controls
{
	public class UITimelineContentDisplay : UIContentDisplay
	{
		public const int TopAndBottomSeparation = 20;
		// change for UITrendingBlock?
		//public List<UIBoardThumbComponent> ListContentComponents;

		public UITimelineContentDisplay (List<Board.Schema.Board> boardList,
			float extraTopMargin = 0, float extraLowMargin = 0)
		{
			GenerateList (boardList, extraTopMargin, extraLowMargin);
			UserInteractionEnabled = true;
		}

		private void GenerateList(List<Board.Schema.Board> boardList,
			float extraTopMargin = 0, float extraLowMargin = 0){
			SuscribeToEvents ();
		}
	}
}

