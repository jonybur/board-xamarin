﻿//
//  Copyright 2012  Xamarin Inc.
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
using System;

using CoreGraphics;
using Foundation;
using UIKit;

namespace Solution
{
	/// <summary>
	/// A quick subclass of UITextView to enable placeholder text
	/// </summary>
	public class PlaceholderTextView : UITextView
	{
		/// <summary>
		/// Gets or sets the placeholder to show prior to editing - doesn't exist on UITextView by default
		/// </summary>
		public string Placeholder { get; set; }

		public PlaceholderTextView (CGRect frame, string placeholder)
			: base(frame)
		{
			Placeholder = placeholder;
			Initialize ();
		}

		public bool IsPlaceHolder;

		public void Initialize ()
		{
			IsPlaceHolder = true;
			Text = Placeholder;
			Alpha = .7f;

			ShouldBeginEditing = t => {
				if (Text == Placeholder)
				{
					IsPlaceHolder = false;
					Text = string.Empty;
					Alpha = 1f;
				}
				
				return true;
			};
			ShouldEndEditing = t => {
				if (string.IsNullOrEmpty (Text))
				{
					IsPlaceHolder = true;
					Text = Placeholder;
					Alpha = .7f;
				}

				return true;
			};
		}
	}
}
