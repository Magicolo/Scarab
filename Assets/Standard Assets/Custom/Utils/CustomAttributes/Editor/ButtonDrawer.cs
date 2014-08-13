#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomPropertyDrawer(typeof(ButtonAttribute))]
public class ButtonDrawer : CustomPropertyDrawerBase {

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		position = base.Initialize(position, label);
		
		if (property.type == "UInt8"){
			string buttonLabel = ((ButtonAttribute) attribute).label;
			if (string.IsNullOrEmpty(buttonLabel)) buttonLabel = label.text;
			string buttonPressMethodName = ((ButtonAttribute) attribute).methodName;
			string buttonIndexVariableName = ((ButtonAttribute) attribute).indexVariableName;
			GUIStyle buttonStyle = ((ButtonAttribute) attribute).style;
			
			position = AttributeUtility.BeginIndentation(position);
			
			if (noFieldLabel) buttonLabel = "";
			
			bool pressed;
			if (buttonStyle != null) pressed = GUI.Button(position, buttonLabel, buttonStyle);
			else pressed = GUI.Button(position, buttonLabel);
			
			AttributeUtility.EndIndentation();
			
			if (pressed){
				if (!string.IsNullOrEmpty(buttonIndexVariableName)) property.serializedObject.FindProperty(buttonIndexVariableName).intValue = index;
			    if (!string.IsNullOrEmpty(buttonPressMethodName)) ((MonoBehaviour) property.serializedObject.targetObject).Invoke(buttonPressMethodName, 0);
				EditorUtility.SetDirty(property.serializedObject.targetObject);
			}
			property.boolValue = pressed;
		}
		else EditorGUI.LabelField(position, "Button variable must be of type boolean.");
	}
}
#endif