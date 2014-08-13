//using UnityEngine;
//using UnityEditor;
//using System.Collections;
//
//[CustomPropertyDrawer(typeof(Source))]
//public class SourcePropertyDrawer : PropertyDrawer {
//	
//	int noneParams = 2;
//	int oscillatorParams = 6;
//	int noiseParams = 2;
//	int samplerParams = 3;
//	bool folded;
//	
//	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
//		EditorGUIUtility.labelWidth = 100;
//		if (property.FindPropertyRelative("module").intValue == Source.Modules.Oscillator.GetHashCode()) ShowOscillator(position.Copy(), property, label);
//		else if (property.FindPropertyRelative("module").intValue == Source.Modules.Noise.GetHashCode()) ShowNoise(position.Copy(), property, label);
//		else if (property.FindPropertyRelative("module").intValue == Source.Modules.Sampler.GetHashCode()) ShowSampler(position.Copy(), property, label);
//		else ShowNone(position, property, label);
//	}
//	
//	public override float GetPropertyHeight (SerializedProperty property, GUIContent label) {
//		float height = EditorGUIUtility.singleLineHeight;
//		if (property.FindPropertyRelative("module").intValue == Source.Modules.Oscillator.GetHashCode()) height *= oscillatorParams;
//		else if (property.FindPropertyRelative("module").intValue == Source.Modules.Noise.GetHashCode()) height *= noiseParams;
//		else if (property.FindPropertyRelative("module").intValue == Source.Modules.Sampler.GetHashCode()) height *= samplerParams;
//		else height *= noneParams;
//		return height;
//	}
//	
//	void ShowNone(Rect position, SerializedProperty property, GUIContent label){
//		position.height /= noneParams;
//		
//		EditorGUI.PropertyField(position, property.FindPropertyRelative("module"), GUIContent.none);
//	}
//	
//	void ShowOscillator(Rect position, SerializedProperty property, GUIContent label){
//		Rect propPosition;
//		position.height /= oscillatorParams;
//		
//		EditorGUI.PropertyField(position, property.FindPropertyRelative("module"), GUIContent.none);
//		
//		position.y += EditorGUIUtility.singleLineHeight;
//		
//		label.text = "Wave Shape";
//		propPosition = EditorGUI.PrefixLabel(position, label);
//		EditorGUI.PropertyField(propPosition, property.FindPropertyRelative("waveShape"), GUIContent.none);
//		
//		position.y += EditorGUIUtility.singleLineHeight;
//		
//		label.text = "Custom Shape";
//		propPosition = EditorGUI.PrefixLabel(position, label);
//		EditorGUI.BeginDisabledGroup(property.FindPropertyRelative("waveShape").intValue != Source.WaveShapes.Custom.GetHashCode());
//		EditorGUI.PropertyField(propPosition, property.FindPropertyRelative("customShape"), GUIContent.none);
//		EditorGUI.EndDisabledGroup();
//		
//		position.y += EditorGUIUtility.singleLineHeight;
//		
//		label.text = "Frequency";
//		propPosition = EditorGUI.PrefixLabel(position, label);
//		EditorGUI.PropertyField(propPosition, property.FindPropertyRelative("frequency"), GUIContent.none);
//		
//		position.y += EditorGUIUtility.singleLineHeight;
//		
//		label.text = "Phase";
//		propPosition = EditorGUI.PrefixLabel(position, label);
//		EditorGUI.PropertyField(propPosition, property.FindPropertyRelative("phase"), GUIContent.none);
//	}
//	
//	void ShowNoise(Rect position, SerializedProperty property, GUIContent label){
//		//Rect propPosition;
//		position.height /= noiseParams;
//		
//		EditorGUI.PropertyField(position, property.FindPropertyRelative("module"), GUIContent.none);
//	}
//
//	void ShowSampler(Rect position, SerializedProperty property, GUIContent label){
//		Rect propPosition;
//		position.height /= samplerParams;
//		
//		EditorGUI.PropertyField(position, property.FindPropertyRelative("module"), GUIContent.none);
//		
//		position.y += EditorGUIUtility.singleLineHeight;
//		
//		label.text = "Audio Clips";
//		//propPosition = EditorGUI.PrefixLabel(position, label);
//		EditorGUI.PropertyField(position, property.FindPropertyRelative("audioClips"), label);
//	}
//}
//		