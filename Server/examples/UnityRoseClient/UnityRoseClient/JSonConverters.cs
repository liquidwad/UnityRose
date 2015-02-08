using System.Collections;
using System.Collections.Generic;
using System;
using JsonFx.Json;

namespace Network.JsonConverters
{
	public class VectorConverter : JsonConverter {
		public override bool CanConvert (Type t) {
			return t == typeof(Vector3);
		}
		
		public override Dictionary<string,object> WriteJson (Type type, object value) {
			Vector3 v = (Vector3)value;
			Dictionary<string,object> dict = new Dictionary<string, object>();
			dict.Add ("x",v.x);
			dict.Add ("y",v.y);
			dict.Add ("z",v.z);
			return dict;
		}
		
		public override object ReadJson (Type type, Dictionary<string,object> value) {
			Vector3 v = new Vector3(CastFloat(value["x"]),CastFloat(value["y"]),CastFloat(value["z"]));
			return v;
		}
	}
}
