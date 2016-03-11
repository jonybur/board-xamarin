using CoreGraphics;
using Foundation;
using System;

namespace Board.Schema
{
	public class Poll : Content
	{
		public string Question;
		public string[] Answers;

		public Poll(){}

		public Poll(string question, float rotation, CGRect frame, string creatorid, DateTime creationdate, params string[] answers)
		{
			Question = question;
			Rotation = rotation;
			Frame = frame;
			CreatorId = creatorid;
			CreationDate = creationdate;
			Answers = answers;
		}
	}
}

