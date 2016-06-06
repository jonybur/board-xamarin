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
		public string Question;

		public List<string> Answers;

		public string Type {
			get { return "polls"; }
		}

		public Poll(){
			CreationDate = DateTime.Now;
		}

		public Poll(string question, CGAffineTransform transform, CGPoint center, string creatorid, params string[] answers)
		{
			CreationDate = DateTime.Now;
			Question = question;
			Transform = transform;
			Center = center;
			CreatorId = creatorid;
			Answers = answers.ToList ();
		}

		public Poll(string question, CGAffineTransform transform, CGPoint center, string creatorid, DateTime creationdate, List<string> answers)
		{
			CreationDate = DateTime.Now;
			Question = question;
			Transform = transform;
			Center = center;
			CreatorId = creatorid;
			CreationDate = creationdate;
			Answers = answers;
		}
	}
}

