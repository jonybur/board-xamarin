using System.Collections.Generic;
using Board.Interface.Widgets;
using Board.Infrastructure;
using Board.Schema;
using UIKit;
using BigTed;

namespace Board.Interface.Buttons
{
	public static class ButtonInterface
	{
		public const int ButtonBarHeight = 45;
		// buttons are square-shaped and must be the same height all-around
		// its height is hardcoded
		public enum ButtonLayout : byte { NavigationBar = 1, ConfirmationBar, MoveWidgetBar, Disable };

		static ActionsButtonSet actionsButtonSet;
		static ConfirmationButtonSet confirmationButtonSet;
		static ConfirmationButtonSet moveWidgetButtonSet;
		public static NavigationButton navigationButton;

		public static void Initialize()
		{
			actionsButtonSet = new ActionsButtonSet ();
			confirmationButtonSet = new ConfirmationButtonSet (async delegate {
				Content content;
				BTProgressHUD.Show("Uploading...");

				if (Preview.IsAlive){
					// remove interaction capabilities from the preview
					Preview.RemoveUserInteraction ();

					// takes out the confirmation bar and resets navigation
					ButtonInterface.SwitchButtonLayout (ButtonInterface.ButtonLayout.NavigationBar);

					content = await Preview.GetContent ();
				} else {
					UIPreviewSticker.PreviewSticker.UserInteractionEnabled = false;

					ButtonInterface.SwitchButtonLayout(ButtonInterface.ButtonLayout.NavigationBar);

					content = UIPreviewSticker.GetContent();
				}

				string jsonString = JsonUtilty.GenerateUpdateJson (content);
				System.Console.WriteLine(jsonString);
				bool wasUploaded = CloudController.UpdateBoard (UIBoardInterface.board.Id, jsonString);
				if (!wasUploaded){

					BTProgressHUD.Dismiss();
					return;
				}

				BTProgressHUD.Dismiss();

				UIBoardInterface.DictionaryContent.Add (content.Id, content);

				if (Preview.IsAlive){
					// remove the preview imageview from the superview
					Preview.RemoveFromSuperview ();

					// adds widget to dictionary
					AppDelegate.BoardInterface.AddWidgetToDictionaryFromContent (content);
				} else {
					// remove previewsticker
					UIPreviewSticker.PreviewSticker.RemoveFromSuperview();

					// add sticker to dictionary
					AppDelegate.BoardInterface.AddStickerToDictionaryFromContent (content as Sticker);
				}

				// renders scrollview
				AppDelegate.BoardInterface.BoardScroll.SelectiveRendering ();
			}, 
			delegate {
				if (Preview.IsAlive){
					// discards preview
					Preview.RemoveFromSuperview ();
				} else {
					// discards sticker
					UIPreviewSticker.PreviewSticker.RemoveFromSuperview();
				}

				// resets navigation
				ButtonInterface.SwitchButtonLayout (ButtonLayout.NavigationBar);
			});
			
			moveWidgetButtonSet = new ConfirmationButtonSet (delegate{

				// takes widget that has been being edited
				var widget = UIBoardInterface.DictionaryWidgets[Widget.IdToUpdate];
				var content = widget.content;

				content.Transform = widget.Transform;
				content.Center = AppDelegate.BoardInterface.BoardScroll.ConvertPointToBoardScrollPoint(widget.Center);

				widget.DisableEditing();
				// sends update json with new transform/frame

				string jsonString = JsonUtilty.GenerateUpdateJson (content);
				CloudController.UpdateBoard (UIBoardInterface.board.Id, jsonString);

				ButtonInterface.SwitchButtonLayout (ButtonLayout.NavigationBar);

			}, delegate{

				// renders scrollview
				var widget = UIBoardInterface.DictionaryWidgets[Widget.IdToUpdate];
			
				widget.ResetAttributesAndStopEditing();

				ButtonInterface.SwitchButtonLayout (ButtonLayout.NavigationBar);
				
			});
			navigationButton = new NavigationButton ();			
		}


		public static List<UIView> GetUserButtons()
		{
			List<UIView> views = new List<UIView>();
			views.Add(navigationButton);

			return views;
		}

		public static List<UIView> GetCreatorButtons()
		{
			List<UIView> views = new List<UIView> ();
			views.Add(actionsButtonSet.arrayButtons [0]);
			views.Add(actionsButtonSet.arrayButtons [1]);
			views.Add(actionsButtonSet.arrayButtons [2]);
			views.Add(actionsButtonSet.arrayButtons [3]);
			views.Add(confirmationButtonSet.arrayButtons [0]);
			views.Add(confirmationButtonSet.arrayButtons [1]);
			views.Add(moveWidgetButtonSet.arrayButtons [0]);
			views.Add(moveWidgetButtonSet.arrayButtons [1]);
			views.Add(navigationButton);
			return views;
		}

		public static void DisableAllLayouts()
		{	
			actionsButtonSet.DisableAllButtons ();
			confirmationButtonSet.DisableAllButtons ();
			moveWidgetButtonSet.DisableAllButtons ();
			navigationButton.DisableButton ();
		}

		public static void SwitchButtonLayout(ButtonLayout newLayout)
		{
			DisableAllLayouts ();

			switch (newLayout) {
			case ButtonLayout.ConfirmationBar:
					confirmationButtonSet.EnableAllButtons ();
					break;

			case ButtonLayout.NavigationBar:
				actionsButtonSet.EnableAllButtons ();
				navigationButton.EnableButton ();
					break;

			case ButtonLayout.MoveWidgetBar:
					moveWidgetButtonSet.EnableAllButtons ();
					break;
			}
		}

	}
}