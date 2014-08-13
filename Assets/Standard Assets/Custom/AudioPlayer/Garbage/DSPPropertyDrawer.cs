//using UnityEngine;
//using UnityEditor;
//using System.Collections;
//
//[CustomPropertyDrawer(typeof(DSP))]
//public class DSPPropertyDrawer : PropertyDrawer {
//	
//	int noneParams = 2;
//	int gainParams = 4;
//	int delayParams = 4;
//	
//	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
//		EditorGUIUtility.labelWidth = 100;
//		if (property.FindPropertyRelative("module").intValue == DSP.Modules.Gain.GetHashCode()) ShowGain(position.Copy(), property, label);
//		else if (property.FindPropertyRelative("module").intValue == DSP.Modules.Delay.GetHashCode()) ShowDelay(position.Copy(), property, label);
//		else ShowNone(position, property, label);
//	}
//	
//	public override float GetPropertyHeight (SerializedProperty property, GUIContent label) {
//		float height = EditorGUIUtility.singleLineHeight;
//		if (property.FindPropertyRelative("module").intValue == DSP.Modules.Gain.GetHashCode()) height *= gainParams;
//		else if (property.FindPropertyRelative("module").intValue == DSP.Modules.Delay.GetHashCode()) height *= delayParams;
//		else height *= noneParams;
//		return height;
//	}
//	
//	void ShowNone(Rect position, SerializedProperty property, GUIContent label){
//		EditorGUI.PropertyField(position, property.FindPropertyRelative("module"), GUIContent.none);
//	}
//	
//	void ShowGain(Rect position, SerializedProperty property, GUIContent label){
//		Rect propPosition;
//		position.height /= gainParams;
//		
//		EditorGUI.PropertyField(position, property.FindPropertyRelative("module"), GUIContent.none);
//		
//		position.y += EditorGUIUtility.singleLineHeight;
//		
//		label.text = "Gain";
//		propPosition = EditorGUI.PrefixLabel(position, label);
//		EditorGUI.PropertyField(propPosition, property.FindPropertyRelative("gain"), GUIContent.none);
//		
//		position.y += EditorGUIUtility.singleLineHeight;
//		
//		label.text = "Ramp Speed";
//		propPosition = EditorGUI.PrefixLabel(position, label);
//		EditorGUI.PropertyField(propPosition, property.FindPropertyRelative("rampSpeed"), GUIContent.none);
//	}
//	
//	void ShowDelay(Rect position, SerializedProperty property, GUIContent label){
//		Rect propPosition;
//		position.height /= delayParams;
//		
//		EditorGUI.PropertyField(position, property.FindPropertyRelative("module"), GUIContent.none);
//		
//		position.y += EditorGUIUtility.singleLineHeight;
//		
//		label.text = "Feedback";
//		propPosition = EditorGUI.PrefixLabel(position, label);
//		EditorGUI.PropertyField(propPosition, property.FindPropertyRelative("feedback"), GUIContent.none);
//		
//		position.y += EditorGUIUtility.singleLineHeight;
//		
//		label.text = "Delay Time";
//		propPosition = EditorGUI.PrefixLabel(position, label);
//		EditorGUI.PropertyField(propPosition, property.FindPropertyRelative("delayTime"), GUIContent.none);
//	}
//}