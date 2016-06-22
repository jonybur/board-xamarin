using System;
using Board.Utilities;

namespace Board.Facebook
{
	public class FacebookPost : FacebookElement
	{
		public string Message;
		public string Story;
		public string PictureURL;
		public string CreatedTime;
		public int Timestamp;

		public FacebookPost(){
		}

		public FacebookPost(string id, string message, string story, string createdtime, string pictureURL)
		{
			Message = message;
			Story = story;
			CreatedTime = createdtime;
			Id = id;
			PictureURL = pictureURL;
			Timestamp = CommonUtils.GetUnixTimeStamp(DateTime.Parse (createdtime));
		}
	}


	public class FacebookCoverUpdatedTime : FacebookElement{
		public string UpdatedTime;
		public string Source;

		public FacebookCoverUpdatedTime(string id, string source, string updatedTime){
			Id = id;
			Source = source;
			UpdatedTime = updatedTime;
		}
	}

	public class FacebookHours : FacebookElement{
		public string Hours;

		public FacebookHours(string id, string hours){
			Id = id;
			Hours = hours;
		}
	}
}

