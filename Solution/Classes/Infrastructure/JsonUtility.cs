using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Board.Infrastructure
{
	public static class JsonUtilty{
		public static string ObjectToJson(object obj)
		{
			string test = JsonConvert.SerializeObject (obj);
			return test;
		}
	}

	public abstract class JsonCreationConverter<T> : JsonConverter
	{
		protected abstract T Create(Type objectType, JObject jObject);

		public override bool CanConvert(Type objectType)
		{
			return typeof(T).IsAssignableFrom(objectType);
		}

		public override object ReadJson(JsonReader reader, 
			Type objectType, 
			object existingValue, 
			JsonSerializer serializer)
		{
			// Load JObject from stream
			JObject jObject = JObject.Load(reader);

			// Create target object based on JObject
			T target = Create(objectType, jObject);

			// Populate the object properties
			serializer.Populate(jObject.CreateReader(), target);

			return target;
		}

		public override void WriteJson(JsonWriter writer, 
			object value,
			JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}
	}
}

