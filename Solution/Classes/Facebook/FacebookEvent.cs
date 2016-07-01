namespace Clubby.Facebook
{
	public class FacebookEvent : FacebookElement
	{
		public string Description;
		public string EndTime;
		public string StartTime;
		public int StartTimestamp;
		public string Name;

		public FacebookEvent(string id, string name, string description, string starttime, string endtime)
		{
			Name = name;
			Id = id;
			Description = description;
			if (endtime != "<null>") {
				EndTime = endtime;
			}
			if (starttime != "<null>") {
				StartTime = starttime;
				StartTimestamp = Clubby.Utilities.CommonUtils.GetUnixTimeStamp (System.DateTime.Parse (StartTime));
			}
		}
	}
}

