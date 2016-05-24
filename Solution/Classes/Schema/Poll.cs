using CoreGraphics;
using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Board.Schema
{
	public class Poll : Content
	{
		[IgnoreDataMember]
		public NSAttributedString Question;

		public string QuestionText;

		public List<string> Answers;

		public const string Type = "polls";

		public Poll(){
		}

		public Poll(NSAttributedString question, CGAffineTransform transform, CGPoint center, string creatorid, params string[] answers)
		{
			Question = question;
			QuestionText = question.Value;
			Transform = transform;
			Center = center;
			CreatorId = creatorid;
			Answers = answers.ToList ();
		}

		public Poll(NSAttributedString question, CGAffineTransform transform, CGPoint center, string creatorid, DateTime creationdate, List<string> answers)
		{
			Question = question;
			QuestionText = question.Value;
			Transform = transform;
			Center = center;
			CreatorId = creatorid;
			CreationDate = creationdate;
			Answers = answers;
		}
	}
}

