using System;
using Newtonsoft.Json;

namespace Solution
{
	public class User
	{
		public User()
		{
			Id = "";
			Name = "";
		}

		public User(string id, string name)
		{
			Id = id;
			Name = name;
		}

		[JsonProperty(PropertyName = "id")]
		public string Id { get; set; }

		[JsonProperty(PropertyName = "name")]
		public string Name { get; set; }
	}
}