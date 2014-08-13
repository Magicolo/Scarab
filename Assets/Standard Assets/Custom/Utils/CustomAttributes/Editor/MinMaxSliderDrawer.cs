#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomPropertyDrawer(typeof(MinMaxSliderAttribute))]
public class MinMaxSliderDrawer : CustomPropertyDrawerBase {

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label){
		position = base.Initialize(position, label);
		
		float x = property.FindPropertyRelative("x").floatValue;
		float y = property.FindPropertyRelative("y").floatValue;
		float min = 0;
		float max = 0;
		string minLabel = ((MinMaxSliderAttribute) attribute).minLabel;
		string maxLabel = ((MinMaxSliderAttribute) attribute).maxLabel;
		
		if (property.FindPropertyRelative("z") != null) min = property.FindPropertyRelative("z").floatValue;
		else min = ((MinMaxSliderAttribute) attribute).min;
		if (property.FindPropertyRelative("w") != null) max = property.FindPropertyRelative("w").floatValue;
		else max = ((MinMaxSliderAttribute) attribute).max;
		
		position = AttributeUtility.BeginIndentation(position);
		
		float width = position.width;
		
		position.width = width / 4;
		if (!noFieldLabel && !string.IsNullOrEmpty(minLabel) && width / 8 >= 16){
			position.width = width / 8;
			EditorGUI.LabelField(position, minLabel);
			position.x += position.width;
			x = EditorGUI.FloatField(position, x);
		}
		else x = EditorGUI.FloatField(position, x);
		position.x += position.width + 2;
		
		position.width = width / 2;
		EditorGUI.MinMaxSlider(position, ref x, ref y, min, max);
		
		position.x += position.width + 2;
		position.width = width / 4;
		if (!noFieldLabel && !string.IsNullOrEmpty(maxLabel) && width / 8 >= 16){
			position.width = width / 8;
			GUIStyle style = new GUIStyle(EditorStyles.label);
			style.alignment = TextAnchor.MiddleRight;
			y = EditorGUI.FloatField(position, y);
			position.x += position.width;
			EditorGUI.LabelField(position, maxLabel, style);
			
		}
		else y = EditorGUI.FloatField(position, y);
		AttributeUtility.EndIndentation();
		
		property.FindPropertyRelative("x").floatValue = Mathf.Clamp(x, min, y);
		property.FindPropertyRelative("y").floatValue = Mathf.Clamp(y, x, max);
		
	}
}
#endif