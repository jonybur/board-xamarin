using CoreGraphics;
using System.Globalization;
using System.Collections.Generic;
using Board.Utilities;
using System;
using Board.Interface;
using System.Runtime.Serialization;

namespace Board.Schema
{
	public class Content
	{
		public string Id;
		public string FacebookId;

		private double _latitude;
		private double _longitude;

		public double latitude{
			get{ return _latitude; }
			set{ _latitude = value; }
		}
		public double longitude {
			get{ return _longitude; }
			set{ _longitude = value; }
		}
		public int timestamp{
			get { return CommonUtils.GetUnixTimeStamp (CreationDate); }
		}

		private string _boardId;
		public string boardId{
			get{ return _boardId; }
			set{ _boardId = value; }
		}

		[IgnoreDataMember]
		public bool Seen;

		private CGPoint center;
		[IgnoreDataMember]
		public CGPoint Center{
			get{ return new CGPoint(center.X * UIBoardScroll.AspectPercentage, center.Y * UIBoardScroll.AspectPercentage); }
			set{ center = value; }
		}

		public float CenterX {
			get { return (float)Center.X; }
			set { Center = new CGPoint (value, Center.Y); }
		}

		public float CenterY {
			get { return (float)Center.Y; }
			set { Center = new CGPoint (Center.X, value); }
		}

		public Content(){
			Id = CommonUtils.GenerateGuid ();
		}

		public float TransformXX {
			get { return (float)Transform.xx; }
			set { Transform = new CGAffineTransform (value, Transform.yx, Transform.xy, Transform.yy, Transform.x0, Transform.y0); }
		}

		public float TransformYX {
			get { return (float)Transform.yx; }
			set { Transform = new CGAffineTransform (Transform.xx, value, Transform.xy, Transform.yy, Transform.x0, Transform.y0); }
		}

		public float TransformXY {
			get { return (float)Transform.xy; }
			set { Transform = new CGAffineTransform (Transform.xx, Transform.yx, value, Transform.yy, Transform.x0, Transform.y0); }
		}

		public float TransformYY {
			get { return (float)Transform.yy; }
			set { Transform = new CGAffineTransform (Transform.xx, Transform.yx, Transform.xy, value, Transform.x0, Transform.y0); }
		}

		public float TransformX0 {
			get { return (float)Transform.x0; }
			set { Transform = new CGAffineTransform (Transform.xx, Transform.yx, Transform.xy, Transform.yy, value, Transform.y0); }
		}

		public float TransformY0 {
			get { return (float)Transform.y0; }
			set { Transform = new CGAffineTransform (Transform.xx, Transform.yx, Transform.xy, Transform.yy, Transform.x0, value); }
		}

		[IgnoreDataMember]
		public DateTime CreationDate;

		public int CreationDateTimestamp{
			get { return CommonUtils.GetUnixTimeStamp(CreationDate); }
			set { CreationDate = CommonUtils.UnixTimeStampToDateTime(value);  }
		}
		
		[IgnoreDataMember]
		public CGAffineTransform Transform;

		[IgnoreDataMember]
		public List<int> SocialChannel;

		[IgnoreDataMember]
		public string CreatorId;
	}
}

