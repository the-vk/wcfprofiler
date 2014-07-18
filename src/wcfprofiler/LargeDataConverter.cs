using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace wcfprofiler
{
	class LargeDataConverter : JsonConverter
	{
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			if (value is byte[])
			{
				var array = value as byte[];
				var jObject = new JObject();
				jObject.Add("type", new JValue(value.GetType().ToString()));
				jObject.Add("length", new JValue(array.Length));
				jObject.WriteTo(writer);
			}
			if (value is string)
			{
				var str = value as string;
				if (str.Length > 1024)
				{
					var jObject = new JObject();
					jObject.Add("type", new JValue(value.GetType().ToString()));
					jObject.Add("length", new JValue(str.Length));
					jObject.WriteTo(writer);
				}
				else
				{
					var token = JToken.FromObject(value);
					token.WriteTo(writer);
				}
			}
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}

		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof (byte[]) || objectType == typeof (string);
		}

		public override bool CanRead
		{
			get { return false; }
		}
	}
}
