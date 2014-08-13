using UnityEngine;
using System.Collections;

static public class Logger {

	static public void Log(params object[] toLog){
		string log = "";
		foreach (object item in toLog){
			log += ObjectToString(item);
			log += ", ";
		}
		if (!string.IsNullOrEmpty(log)) log = log.Substring(0, log.Length - 2);
		Debug.Log(log);
	}
		
	static public void LogWarning(params object[] toLog){
		string log = "";
		foreach (object item in toLog){
			log += ObjectToString(item);
			log += ", ";
		}
		if (!string.IsNullOrEmpty(log)) log = log.Substring(0, log.Length - 2);
		Debug.LogWarning(log);
	}
		
	static public void LogError(params object[] toLog){
		string log = "";
		foreach (object item in toLog){
			log += ObjectToString(item);
			log += ", ";
		}
		if (!string.IsNullOrEmpty(log)) log = log.Substring(0, log.Length - 2);
		Debug.LogError(log);
	}
		
	static public string ObjectToString(object obj){
		string str = "";
		
		if (obj is System.Array){
			str += "(";
			foreach (object item in (System.Array) obj) str += ObjectToString(item) + ", ";
			if (str.Length > 1) str = str.Substring(0, str.Length - 2);
			str += ")";
		}
		else if (obj is IList){
			str += "[";
			foreach (object item in (IList) obj) str += ObjectToString(item) + ", ";
			if (str.Length > 1) str = str.Substring(0, str.Length - 2);
			str += "]";
		}
		else if (obj is IDictionary){
			str += "{";
			foreach (object key in ((IDictionary) (IDictionary) obj).Keys) str += ObjectToString(key) + " : " + ObjectToString(((IDictionary) obj)[key]) + ", ";
			if (str.Length > 1) str = str.Substring(0, str.Length - 2);
			str += "}";
		}
		else if (obj is IEnumerator) str += ObjectToString(((IEnumerator) obj).Current);
		else if (obj is Vector2 || obj is Vector3 || obj is Vector4) str += VectorToString(obj);
		else if (obj != null) str += obj.ToString();
		else str += "null";
		return str;
	}
		
	static public string VectorToString(object v){
		string str = "";
		if (v is Vector2){
			Vector2 v2 = (Vector2) v;
			v2 = v2.Round(0.001);
			str += "Vector2(" + v2.x + ", " + v2.y;
		}
		else if (v is Vector3){
			Vector3 v3 = (Vector3) v;
			v3 = v3.Round(0.001);
			str += "Vector3(" + v3.x + ", " + v3.y + ", " + v3.z;
		}
		else if (v is Vector4){
			Vector4 v4 = (Vector4) v;
			v4 = v4.Round(0.001);
			str += "Vector4(" + v4.x + ", " + v4.y + ", " + v4.z + ", " + v4.w;
		}
		return str + ")";
	}
}
