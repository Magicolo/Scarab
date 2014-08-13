#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomPropertyDrawer(typeof(SeparatorAttribute))]
public class SeparatorDrawer : DecoratorDrawer {

	public override void OnGUI(Rect position) {
		position.y += 5;
		EditorGUI.LabelField(position, "", new GUIStyle("RL DragHandle"));
	}
}
#endif