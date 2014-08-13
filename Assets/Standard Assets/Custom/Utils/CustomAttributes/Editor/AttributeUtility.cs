#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class AttributeUtility {
	
	static public float indentWidth = 0;
	
	static int indentationDepth = 0;
	static int indentLevel = 0;
	
	static public int GetIndexFromLabel(GUIContent label){
		string strIndex = "";
		for (int i = label.text.Length - 1; i >= 0; i--){
			if (label.text[i] == 't') break;
			else strIndex += label.text[i];
		}
		strIndex = strIndex.Reverse();
		return int.Parse(strIndex);
	}
	
	static public Rect BeginIndentation(Rect position){
		indentationDepth += 1;
		indentLevel = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;
		indentWidth = indentLevel * 16;
		return (new Rect(position.x + indentWidth, position.y, position.width - indentWidth, position.height));
	}
	
	static public void EndIndentation(){
		indentationDepth -= 1;
		EditorGUI.indentLevel = indentLevel;
	}
}
#endif