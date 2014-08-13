#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomPropertyDrawer(typeof(SingleLineVectorAttribute))]
public class SingleLineVectorDrawer : CustomPropertyDrawerBase {

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		position = base.Initialize(position, label);
		
		float x = property.FindPropertyRelative("x").floatValue;
		float y = property.FindPropertyRelative("y").floatValue;
		float z = 0;
		float w = 0;
		string xName = ((SingleLineVectorAttribute) attribute).x;
		string yName = ((SingleLineVectorAttribute) attribute).y;
		string zName = ((SingleLineVectorAttribute) attribute).z;
		string wName = ((SingleLineVectorAttribute) attribute).w;
		
		int nbOfFields = 2;
		if (property.FindPropertyRelative("z") != null){
			nbOfFields += 1;
			z = property.FindPropertyRelative("z").floatValue;
		}
		if (property.FindPropertyRelative("w") != null){
			nbOfFields += 1;
			w = property.FindPropertyRelative("w").floatValue;
		}
		
		position = AttributeUtility.BeginIndentation(position);
		
		float width = position.width;
		
		position.width /= nbOfFields;
		EditorGUIUtility.labelWidth = width / (nbOfFields * 2);
		
		if (noFieldLabel) x = EditorGUI.FloatField(position, x);
		else x = EditorGUI.FloatField(position, xName, x);
		property.FindPropertyRelative("x").floatValue = x;
		
		position.x += position.width;
		if (noFieldLabel) y = EditorGUI.FloatField(position, y);
		else y = EditorGUI.FloatField(position, yName, y);
		property.FindPropertyRelative("y").floatValue = y;
		
		if (property.FindPropertyRelative("z") != null){
			position.x += position.width;
			if (noFieldLabel) z = EditorGUI.FloatField(position, z);
			else z = EditorGUI.FloatField(position, zName, z);
			property.FindPropertyRelative("z").floatValue = z;
		}
		if (property.FindPropertyRelative("w") != null){
			position.x += position.width;
			if (noFieldLabel) w = EditorGUI.FloatField(position, w);
			else w = EditorGUI.FloatField(position, wName, w);
			property.FindPropertyRelative("w").floatValue = w;
		}
		AttributeUtility.EndIndentation();
	}
}
#endif