using CoreGraphics;
using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Board.Schema
{
	public class Poll : Content
	{
		public NSAttributedString Question;
		public List<string> Answers;

		public Poll(){}

		public Poll(NSAttributedString question, float rotation, CGPoint position, string creatorid, DateTime creationdate, params string[] answers)
		{
			Question = question;
			Rotation = rotation;
			Position = position;
			CreatorId = creatorid;
			CreationDate = creationdate;
			Answers = answers.ToList ();
		}

		public Poll(NSAttributedString question, float rotation, CGPoint position, string creatorid, DateTime creationdate, List<string> answers)
		{
			Question = question;
			Rotation = rotation;
			Position = position;
			CreatorId = creatorid;
			CreationDate = creationdate;
			Answers = answers;
		}
	}
}

