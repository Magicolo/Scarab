#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class CustomPropertyDrawerBase : PropertyDrawer {
	
	protected bool noFieldLabel = false;
	protected bool noPrefixLabel = false;
	protected bool noIndex = false;
	protected string prefixLabel = "";
	protected int index = 0;
	
	protected Rect Initialize(Rect position, GUIContent label){
		noFieldLabel = ((CustomAttributeBase) attribute).NoFieldLabel;
		noPrefixLabel = ((CustomAttributeBase) attribute).NoPrefixLabel;
		noIndex = ((CustomAttributeBase) attribute).NoIndex;
		prefixLabel = ((CustomAttributeBase) attribute).PrefixLabel;
		
		if (fieldInfo.FieldType.IsArray){
 			index = AttributeUtility.GetIndexFromLabel(label);
 			if (!noIndex && !string.IsNullOrEmpty(prefixLabel)) prefixLabel += " " + index;
 			else if (noIndex && string.IsNullOrEmpty(prefixLabel)) prefixLabel = "Element";
		}
		
		if (!noPrefixLabel){
			if (!string.IsNullOrEmpty(prefixLabel)) position = EditorGUI.PrefixLabel(position, new GUIContent(prefixLabel));
			else position = EditorGUI.PrefixLabel(position, label);
			EditorGUI.indentLevel = 0;
		}
		return position;
	}
}
#endif