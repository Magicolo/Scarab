using UnityEngine;
using System.Collections;
using System.Collections.Generic;

static public class Extensions {

	// Int
	static public int Round(this int i, double step = 1){
		if (step == 0) return i;
		return (int) (Mathf.Round((float) (i * (1D / step))) / (1D / step));
	}
	
	// Float
	static public float Round(this float f, double step = 1){
		if (step == 0) return f;
		return (float) (Mathf.Round((float) (f * (1D / step))) / (1D / step));
	}
			
	// Double	
	static public double Round(this double d, double step = 1){
		if (step == 0) return d;
		return (double) (Mathf.Round((float) (d * (1D / step))) / (1D / step));
	}
	
	// String
	static public string Reverse(this string s){
		string reversed = "";
		for (int i = s.Length - 1; i >= 0; i--){
			reversed += s[i];
		}
		return reversed;
	}
	
	// Transform
	static public void LookAt2D(this Transform transform, Transform target){
		transform.LookAt2D(target.position, 0, 100);
	}
	
	static public void LookAt2D(this Transform transform, Vector3 target){
		transform.LookAt2D(target, 0, 100);
	}
	
	static public void LookAt2D(this Transform transform, Transform target, float angleOffset = 0, float damping = 100){
		transform.LookAt2D(target.position, angleOffset, damping);
	}
	
	static public void LookAt2D(this Transform transform, Vector3 target, float angleOffset = 0, float damping = 100){
		transform.rotation = transform.LookingAt2D(target, angleOffset, damping);
	}
	
	static public Quaternion LookingAt2D(this Transform transform, Transform target){
		return transform.LookingAt2D(target.position, 0, 100);
	}
	
	static public Quaternion LookingAt2D(this Transform transform, Vector3 target){
		return transform.LookingAt2D(target, 0, 100);
	}
	
	static public Quaternion LookingAt2D(this Transform transform, Transform target, float angleOffset = 0, float damping = 100){
		return transform.LookingAt2D(target.position, angleOffset, damping);
	}
	
	static public Quaternion LookingAt2D(this Transform transform, Vector3 target, float angleOffset = 0, float damping = 100){
		Vector3 targetDirection = (target - transform.position).normalized;
		float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg + angleOffset;
		return Quaternion.Lerp(transform.rotation, Quaternion.AngleAxis(angle, Vector3.forward), damping * Time.deltaTime);
	}
	
	static public Transform[] GetChildren(this Transform parent){
		List<Transform> children = new List<Transform>();
		if (parent != null){
			if (parent.childCount > 0){
				for (int i = 0; i < parent.childCount; i++){
					Transform child = parent.GetChild(i);
					children.Add(child);
				}
			}
		}
		return children.ToArray();
	}
	
	static public Transform[] GetChildrenRecursive(this Transform parent){
		List<Transform> children = new List<Transform>();
		if (parent != null){
			foreach (Transform child in parent.GetChildren()){
				children.Add(child);
				if (child.childCount > 0){children.AddRange(child.GetChildrenRecursive());}
			}
		}
		return children.ToArray();
	}
	
	static public void DestroyChildren(this Transform parent){
		foreach (Transform child in parent.GetChildren()){
			GameObject.Destroy(child.gameObject);
		}
	}
	
	static public void DestroyImmediateChildren(this Transform parent){
		foreach (Transform child in parent.GetChildren()){
			GameObject.DestroyImmediate(child.gameObject);
		}
	}
	
	// Vectors
	static public Vector2 Round(this Vector2 vector, double step = 1){
		if (step == 0) return vector;
		vector.x = (float) (Mathf.Round((float) (vector.x * (1D / step))) / (1D / step));
		vector.y = (float) (Mathf.Round((float) (vector.y * (1D / step))) / (1D / step));
		return vector;
	}
	
	static public Vector3 Round(this Vector3 vector, double step = 1){
		if (step == 0) return vector;
		vector.x = (float) (Mathf.Round((float) (vector.x * (1D / step))) / (1D / step));
		vector.y = (float) (Mathf.Round((float) (vector.y * (1D / step))) / (1D / step));
		vector.z = (float) (Mathf.Round((float) (vector.z * (1D / step))) / (1D / step));
		return vector;
	}
	
	static public Vector4 Round(this Vector4 vector, double step = 1){
		if (step == 0) return vector;
		vector.x = (float) (Mathf.Round((float) (vector.x * (1D / step))) / (1D / step));
		vector.y = (float) (Mathf.Round((float) (vector.y * (1D / step))) / (1D / step));
		vector.z = (float) (Mathf.Round((float) (vector.z * (1D / step))) / (1D / step));
		vector.w = (float) (Mathf.Round((float) (vector.w * (1D / step))) / (1D / step));
		return vector;
	}
		
	static public Vector2 Rotate(this Vector2 vector, float angle){
		Vector3 vec = new Vector3(vector.x, vector.y, 0).Rotate(angle);
		return new Vector2(vec.x, vec.y);
	}
	
	static public Vector3 Rotate(this Vector3 vector, float angle){
		return vector.Rotate(angle, Vector3.forward);
	}
	
	static public Vector3 Rotate(this Vector3 vector, float angle, Vector3 axis){
		angle %= 360;
		return Quaternion.AngleAxis(-angle, axis) * vector;
	}
	
	static public Vector2 SquareClamp(this Vector2 vector, float size = 1){
		Vector3 v = new Vector3(vector.x, vector.y, 0).SquareClamp(size);
		return new Vector2(v.x, v.y);
	}
		
	static public Vector3 SquareClamp(this Vector3 vector, float size = 1){
		float clamped;
		if (vector.x < -size || vector.x > size){
			clamped = Mathf.Clamp(vector.x, -size, size);
			vector.y *= clamped / vector.x;
			vector.x = clamped;
		}
		if (vector.y < -size || vector.y > size){
			clamped = Mathf.Clamp(vector.y, -size, size);
			vector.x *= clamped / vector.y;
			vector.y = clamped;
		}
		return vector;
	}
	
	// Color
	static public Color Round(this Color c , double step = 1){
		if (step == 0) return c;
		c.r = (float) (Mathf.Round((float) (c.r * (1D / step))) / (1D / step));
		c.g = (float) (Mathf.Round((float) (c.g * (1D / step))) / (1D / step));
		c.b = (float) (Mathf.Round((float) (c.b * (1D / step))) / (1D / step));
		c.a = (float) (Mathf.Round((float) (c.a * (1D / step))) / (1D / step));
		return c;
	}
	
	static public Color ToHSV (this Color RGBColor){
		float R = RGBColor.r;
		float G = RGBColor.g;
		float B = RGBColor.b;
		float H = 0;
		float S = 0;
		float V = 0;
		float d = 0;
		float h = 0;
		
		float minRGB = Mathf.Min(R, Mathf.Min(G, B));
		float maxRGB = Mathf.Max(R, Mathf.Max(G, B));
	
		if (minRGB == maxRGB) return new Color(0, 0, minRGB, RGBColor.a);

		if (R == minRGB) d = G - B;
		else if (B == minRGB) d = R - G;
		else d = B - R;
			
		if (R == minRGB) h = 3;
		else if (B == minRGB) h = 1;
		else h = 5;
			
		H = (60 * (h - d / (maxRGB - minRGB))) / 360;
		S = (maxRGB - minRGB) / maxRGB;
		V = maxRGB;
		
		return new Color(H, S, V, RGBColor.a);
	}
	
	static public Color ToRGB (this Color HSVColor){
		float H = HSVColor.r;
		float S = HSVColor.g;
		float V = HSVColor.b;
		float R = 0;
		float G = 0;
		float B = 0;
		float maxHSV = 255 * V;
		float minHSV = maxHSV * (1 - S);
		float h = H * 360;
		float z = (maxHSV - minHSV) * (1 - Mathf.Abs((h / 60) % 2 - 1));
		
		if (0 <= h && h < 60){
			R = maxHSV;
			G = z + minHSV;
			B = minHSV;
		}
		else if (60 <= h && h < 120){
			R = z + minHSV;
			G = maxHSV;
			B = minHSV;
		}
		else if (120 <= h && h < 180){
			R = minHSV;
			G = maxHSV;
			B = z + minHSV;	
		}
		else if (180 <= h && h < 240){
			R = minHSV;
			G = z + minHSV;;
			B = maxHSV;
		}
		else if (240 <= h && h < 300){
			R = z + minHSV;
			G = minHSV;
			B = maxHSV;
		}
		else if (300 <= h && h < 360){
			R = maxHSV;
			G = minHSV;
			B = z + minHSV;
		}
		return new Color(R / 255, G / 255, B / 255, HSVColor.a);
	}
	
	// Rect
	static public Rect Copy(this Rect rect){
		return new Rect(rect.x, rect.y, rect.width, rect.height);
	}
	
	static public void Copy(this Rect rect, Rect otherRect){
		rect.x = otherRect.x;
		rect.y = otherRect.y;
		rect.width = otherRect.width;
		rect.height = otherRect.height;
	}
	
	// AudioSource
	static public void Copy(this AudioSource audioSource, AudioSource otherSource){
		audioSource.clip = otherSource.clip;
		audioSource.mute = otherSource.mute;
		audioSource.bypassEffects = otherSource.bypassEffects;
		audioSource.bypassListenerEffects = otherSource.bypassListenerEffects;
		audioSource.bypassReverbZones = otherSource.bypassReverbZones;
		audioSource.playOnAwake = otherSource.playOnAwake;
		audioSource.loop = otherSource.loop;
		audioSource.priority = otherSource.priority;
		audioSource.volume = otherSource.volume;
		audioSource.pitch = otherSource.pitch;
		audioSource.dopplerLevel = otherSource.dopplerLevel;
		audioSource.rolloffMode = otherSource.rolloffMode;
		audioSource.minDistance = otherSource.minDistance;
		audioSource.panLevel = otherSource.panLevel;
		audioSource.spread = otherSource.spread;
		audioSource.maxDistance = otherSource.maxDistance;
		audioSource.pan = otherSource.pan;
	}
	
	// AnimationCurve
	static public void Clamp(this AnimationCurve curve, float minTime, float maxTime, float minValue, float maxValue){
		for (int i = 0; i < curve.keys.Length; i++){
			Keyframe key = curve.keys[i];
			if (key.time < minTime || key.time > maxTime || key.value < minValue || key.value > maxValue){
				Keyframe newKey = new Keyframe(Mathf.Clamp(key.time, minTime, maxTime), Mathf.Clamp(key.value, minValue, maxValue));
				newKey.inTangent = key.inTangent;
				newKey.outTangent = key.outTangent;
				curve.MoveKey(i, newKey);
			}
		}
	}

	// GameObject
	static public void DisconnectPrefab(this GameObject gameObject){
		#if UNITY_EDITOR
		if (gameObject.transform.parent == null){
			UnityEditor.PrefabUtility.DisconnectPrefabInstance(gameObject);
		}
		#endif
	}
	
	// MonoBehaviour
	static public void SetExecutionOrder(this MonoBehaviour script, int order){
		#if UNITY_EDITOR
		foreach (UnityEditor.MonoScript s in UnityEditor.MonoImporter.GetAllRuntimeMonoScripts()) {
			if (s.name == script.name){
				if (UnityEditor.MonoImporter.GetExecutionOrder(s) != -1){
					UnityEditor.MonoImporter.SetExecutionOrder(s, -1);
				}
			}
		}
		#endif
	}
	
	
}
