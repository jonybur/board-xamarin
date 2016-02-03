using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using CoreGraphics;

using Foundation;
using UIKit;

using CoreAnimation;
using CoreText;
using System.Threading.Tasks;

namespace Solution
{
	public class ChatInterface : UIViewController
	{
		private const int sizePicture = 60;
		private const string fontName = "Roboto-Regular";
		private const string fontHeadlineName = "Roboto-Bold";
		private const int fontSize = 18;

		private const int headlineSize = 12;
		private const int separation = 20;
		private const int lineSize = 18;
		private const int messageborder = 10;
		private float lastPosition;

		const int wborder = 50;
		const int hborder = 65;

		private UIScrollView scrollView;
		private UITextView writingTextView;
		private UIView writingField;


		private List<Message> lstMessages;
		private UIButton sendButton;

		public ChatInterface (string contentId, UINavigationController navigationController)
		{
			scrollView = new UIScrollView (new CGRect (0, 0, AppDelegate.ScreenWidth, AppDelegate.ScreenHeight));
			scrollView.BackgroundColor = UIColor.White;

			CreateWritingField (contentId);

			Colors = new UIColor[7];
			colorIndex = 0;

			Colors[0] = UIColor.Blue;
			Colors[1] = UIColor.Brown;
			Colors[2] = UIColor.Green;
			Colors[3] = UIColor.Magenta;
			Colors[4] = UIColor.Orange;
			Colors[5] = UIColor.Purple;
			Colors[6] = UIColor.Red;

			LoadChat (contentId);

			UINavigationBar uiNavigationBar = CreateNavigationBar (navigationController);

			float dy = 0;

			UITapGestureRecognizer tapGesture= new UITapGestureRecognizer ((tg) => {
				writingTextView.ResignFirstResponder();

				CloseKeyboard(writingField.Frame);
			});

			scrollView.AddGestureRecognizer (tapGesture);

			View.AddSubviews (scrollView, writingField);
			View.AddSubview (uiNavigationBar);


			// Keyboard popup
			NSNotificationCenter.DefaultCenter.AddObserver
			(UIKeyboard.DidShowNotification,KeyBoardUpNotification);

			// Keyboard Down
			NSNotificationCenter.DefaultCenter.AddObserver
			(UIKeyboard.WillHideNotification,KeyBoardDownNotification);

		}

		public async void LoadChat(string contentId)
		{
			lstMessages = StorageController.ReturnConversation (contentId);
			scrollView.ContentSize = new CGSize (AppDelegate.ScreenWidth, hborder + writingTextView.Bounds.Height);
			lstChatUsers = new List<ChatUser> ();


			if (lstMessages.Count == 0) {
				return;
			}

			string lastUserId = lstMessages [0].UserId;
			lastPosition = separation;
				
			for (int i = 0; i < lstMessages.Count; i++)
			{
				if (i == 0 && lstMessages[i].UserId != CloudController.BoardUser.Id){
					await AddHeadline (lstMessages[i], lastPosition);
					lastPosition += separation;
				}

				if (i > 0) {
					lastPosition += fontSize + lineSize;

					if (lastUserId != lstMessages [i].UserId) {

						if (lstMessages[i].UserId != CloudController.BoardUser.Id) {
							lastPosition += separation;
							await AddHeadline (lstMessages[i], lastPosition);
						}
						lastPosition += separation;

					}
				}

				AddMessage (lstMessages [i], lastPosition);
				lastUserId = lstMessages [i].UserId;
			}

			ScrollToBottom (false);
		}

		private class ChatUser
		{
			public string UserID;
			public string Name;
			public UIColor Color;

			public ChatUser(string userID, string name, UIColor color)
			{
				UserID = userID;
				Name = name;
				Color = color;
			}
		}
		List<ChatUser> lstChatUsers;


		private async Task AddHeadline(Message msg, float yposition)
		{
			UITextView headline = await CreateHeadline (msg, yposition);

			//scrollView.ContentSize = new SizeF(AppDelegate.ScreenWidth, yposition + hborder + writingTextView.Bounds.Height + headline.Bounds.Height);

			scrollView.AddSubview (headline);
		}

		private void AddMessage(Message msg, float yposition)
		{
			UITextView messagebox = CreateMessageBox (msg, yposition);

			scrollView.ContentSize = new CGSize(AppDelegate.ScreenWidth, yposition + hborder + writingTextView.Bounds.Height + messagebox.Bounds.Height);

			scrollView.AddSubview (messagebox);
		}


		private async Task<ChatUser> GetChatUser(string userID)
		{
			foreach (ChatUser chtusr in lstChatUsers) {
				if (chtusr.UserID == userID) {
					return chtusr;
				}
			}

			User user = await AppDelegate.CloudController.LookupUser (userID);
			ChatUser aux = new ChatUser (userID, user.Name, AssignColor());
			lstChatUsers.Add (aux);
			return aux;
		}	

		private UIColor[] Colors;
		private int colorIndex;
		private UIColor AssignColor()
		{
			if (colorIndex > Colors.Length - 1)
			{
				colorIndex = 0;
			}

			UIColor aux = Colors [colorIndex];
			colorIndex++;
			return aux;
		}

		private async Task<UITextView> CreateHeadline(Message msg, float position)
		{
			UITextView textview = new UITextView();

			textview.Font = UIFont.FromName (fontHeadlineName,headlineSize);
			textview.BackgroundColor = UIColor.FromRGB(229,229,234);

			ChatUser chtusr = await GetChatUser (msg.UserId);

			CGRect frame = new CGRect (messageborder, position + hborder, 
				chtusr.Name.StringSize(textview.Font).Width + 10, fontSize + 12);
			
			textview.TextColor = chtusr.Color;
			textview.Frame = frame;
			textview.ScrollEnabled = false;
			textview.Editable = false;

			textview.Text = chtusr.Name;

			return textview;
		}

		private UITextView CreateMessageBox(Message msg, float position)
		{
			UITextView textview = new UITextView();

			textview.Font = UIFont.FromName (fontName,fontSize);
			textview.BackgroundColor = UIColor.FromRGB(229,229,234);
			textview.TextColor = UIColor.Black;

			CGRect frame = new CGRect (messageborder, position + hborder, 
												msg.Text.StringSize(textview.Font).Width + 10, fontSize + lineSize);
			
			if (msg.UserId == CloudController.BoardUser.Id) {
				textview.TextAlignment = UITextAlignment.Right;

				frame.X = AppDelegate.ScreenWidth - frame.Width - messageborder;

				textview.BackgroundColor = UIColor.FromRGB(240,74,45);
				//textview.BackgroundColor = UIColor.FromRGB(58,166,247);
				textview.TextColor = UIColor.White;

			}

			textview.Frame = frame;
			textview.ScrollEnabled = false;
			textview.Editable = false;
			textview.Text = msg.Text;



			return textview;
		}

		private UINavigationBar CreateNavigationBar (UINavigationController navigationController)
		{
			var frame = new CGRect (0,0,AppDelegate.ScreenWidth, hborder);
			UINavigationBar navBar = new UINavigationBar (frame);
			UINavigationItem[] item = new UINavigationItem[1];

			UIBarButtonItem leftBarButtonItem = new UIBarButtonItem ("Back", UIBarButtonItemStyle.Plain, null);

			leftBarButtonItem.Clicked += (sender, e) => {
				if (writingTextView.Text.Length > 0) {
					var alert = new UIAlertView ("Discard Comment?", "Your message will be discarded", null, "Keep", new string[] { "Discard" });
					alert.Clicked += (s, b) => {
						if (b.ButtonIndex == 0) {
							return;
						}
						CloseTextCreator (navigationController);
					};
					alert.Show ();
				} else {
					CloseTextCreator (navigationController);
				}

			};

			item [0] = new UINavigationItem ("Comments");
			item [0].LeftBarButtonItem = leftBarButtonItem;

			navBar.SetItems (item,false);

			return navBar;
		}

		private void CloseTextCreator (UINavigationController navigationController)
		{				
			navigationController.DismissViewController(true, null);
			View.Dispose();
		}


		private UITextView CreateWritingTextView()
		{
			var frame = new CGRect(10, 0, 
				AppDelegate.ScreenWidth - 80, fontSize + lineSize);

			UITextView textview = new UITextView(frame);

			textview.KeyboardType = UIKeyboardType.Default;
			textview.ReturnKeyType = UIReturnKeyType.Default;
			textview.EnablesReturnKeyAutomatically = true;
			textview.BackgroundColor = UIColor.White;
			textview.TextColor = UIColor.Black;
			textview.Font = UIFont.FromName (fontName,fontSize);


			return textview;
		}


		private void CreateWritingField(string contentId)
		{
			var frame = new CGRect(0, AppDelegate.ScreenHeight - fontSize - lineSize, 
				AppDelegate.ScreenWidth, fontSize + lineSize);

			writingField = new UIView (frame);
			writingField.BackgroundColor = UIColor.FromRGB (247, 247, 247);

			writingTextView = CreateWritingTextView ();
			sendButton = CreateSendButton (contentId);

			writingField.AddSubviews (writingTextView, sendButton);
		}

		private UIButton CreateSendButton(string contentId)
		{
			CGPoint buttonPosition = new CGPoint(writingTextView.Frame.X + writingTextView.Frame.Width, 0);
			CGSize buttonSize = new CGSize(AppDelegate.ScreenWidth - (writingTextView.Frame.X + writingTextView.Frame.Width), fontSize + lineSize);
			
			UIGraphics.BeginImageContextWithOptions (new CGSize(buttonSize.Width, buttonSize.Height), false, 0);
			CGContext context = UIGraphics.GetCurrentContext ();
			UIFont font = UIFont.FromName (fontName,fontSize);

			context.SetFillColor (UIColor.FromRGB(240,74,45).CGColor);
			new NSString("Send").DrawString (new CGRect(0, lineSize / 2, buttonSize.Width, buttonSize.Height), font, UILineBreakMode.WordWrap, UITextAlignment.Center);

			UIButton button = new UIButton (new CGRect(buttonPosition, buttonSize));
			button.SetImage (UIGraphics.GetImageFromCurrentImageContext (), UIControlState.Normal);

			bool sending = false;

			UITapGestureRecognizer tapGesture= new UITapGestureRecognizer (async (tg) => {
				if (writingTextView.Text.Length > 0 && !sending)
				{
					sending = true;

					string uploadText = writingTextView.Text;

					writingTextView.Text = "";

					Message msg = new Message(CloudController.BoardUser.Id, contentId, uploadText, DateTimeOffset.Now);

					if (lstMessages.Count > 0) {
						lastPosition += fontSize + lineSize;
						if (lstMessages[lstMessages.Count-1].UserId != msg.UserId) {
							lastPosition += 15;
						}
					}

					lstMessages.Add(msg);

					AddMessage(msg, lastPosition);
					ScrollToBottom(true); 

					await AppDelegate.CloudController.InsertMessageAsync (msg);

					sending = false;
				}
			});

			button.UserInteractionEnabled = true;
			button.AddGestureRecognizer (tapGesture);

			return button;
		}

		private void KeyBoardUpNotification(NSNotification notification)
		{
			// get the keyboard size
			CGRect r = UIKeyboard.BoundsFromNotification (notification);

			// Bottom of the controller = initial position + height      
			bottom = (float)(writingField.Frame.Y + writingField.Frame.Height);

			// Calculate how far we need to scroll
			scroll_amount = (float)(r.Height - (View.Frame.Size.Height - bottom)) ;

			// Perform the scrolling
			if (scroll_amount > 0) {
				moveViewUp = true;
				ScrollTheView (moveViewUp);
			} else {
				moveViewUp = false;
			}

		}
		private float scroll_amount = 0.0f;   
		private bool moveViewUp = false;
		private float bottom = 0.0f;

		private void ScrollTheView(bool move)
		{
			// scroll the view up or down
			UIView.BeginAnimations (string.Empty, System.IntPtr.Zero);
			UIView.SetAnimationDuration (0.3);

			if (move) {
				OpenKeyboard (writingField.Frame);
			} else {
				CloseKeyboard (writingField.Frame);
			}

			UIView.CommitAnimations();
		}

		private void OpenKeyboard(CGRect writingFieldFrame)
		{
			writingFieldFrame.Y -= scroll_amount;

			ScrollToBottom (true);

			writingField.Frame = writingFieldFrame;
		}

		private void CloseKeyboard(CGRect writingFieldFrame)
		{
			writingFieldFrame.Y += scroll_amount;

			scroll_amount = 0;

			writingField.Frame = writingFieldFrame;
		}

		float preOffset;
		private void ScrollToBottom(bool animated)
		{
			if (scrollView.ContentSize.Height > (AppDelegate.ScreenHeight - scroll_amount)) {
				preOffset = (float)scrollView.ContentOffset.Y;
				scrollView.SetContentOffset (new CGPoint (0, scrollView.ContentSize.Height - (AppDelegate.ScreenHeight - scroll_amount) + separation), animated);
			}
		}

		private void KeyBoardDownNotification(NSNotification notification)
		{
			// Calculate how far we need to scroll
			scrollView.SetContentOffset(new CGPoint(0,preOffset), true);

			if(moveViewUp){ScrollTheView(false);}
		}

	}
}

