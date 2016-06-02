using CoreGraphics;
using System.Collections.Generic;
using Board.Utilities;
using System;
using System.Runtime.Serialization;
using Board.Interface;

namespace Board.Schema
{
	public class Content
	{
		public string Id;
		public string FacebookId;

		[IgnoreDataMember]
		public bool Seen;

		[IgnoreDataMember]
		public CGPoint Center;

		public float CenterX {
			set { Center = new CGPoint (value, Center.Y); }
			get { return (float)Center.X; }
		}

		public float CenterY {
			set { Center = new CGPoint (Center.X, value); }
			get { return (float)Center.Y; }
		}

		public Content(){
			Id = CommonUtils.GenerateGuid ();
		}

		public float TransformXX {
			set { Transform = new CGAffineTransform (value, Transform.yx, Transform.xy, Transform.yy, Transform.x0, Transform.y0); }
			get { return (float)Transform.xx; }
		}

		public float TransformYX {
			set { Transform = new CGAffineTransform (Transform.xx, value, Transform.xy, Transform.yy, Transform.x0, Transform.y0); }
			get { return (float)Transform.yx; }
		}

		public float TransformXY {
			set { Transform = new CGAffineTransform (Transform.xx, Transform.yx, value, Transform.yy, Transform.x0, Transform.y0); }
			get { return (float)Transform.xy; }
		}

		public float TransformYY {
			set { Transform = new CGAffineTransform (Transform.xx, Transform.yx, Transform.xy, value, Transform.x0, Transform.y0); }
			get { return (float)Transform.yy; }
		}

		public float TransformX0 {
			set { Transform = new CGAffineTransform (Transform.xx, Transform.yx, Transform.xy, Transform.yy, value, Transform.y0); }
			get { return (float)Transform.x0; }
		}

		public float TransformY0 {
			set { Transform = new CGAffineTransform (Transform.xx, Transform.yx, Transform.xy, Transform.yy, Transform.x0, value); }
			get { return (float)Transform.y0; }
		}


		public DateTime CreationDate;

		[IgnoreDataMember]
		public CGAffineTransform Transform;

		[IgnoreDataMember]
		public List<int> SocialChannel;

		[IgnoreDataMember]
		public string CreatorId;
	}
}

