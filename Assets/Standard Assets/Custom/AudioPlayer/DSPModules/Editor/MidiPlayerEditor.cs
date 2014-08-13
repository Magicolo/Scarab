#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(MidiPlayer)), CanEditMultipleObjects]
public class MidiPlayerEditor : Editor {

	MidiPlayer midiPlayer;
	List<float> whiteKeysMidiNotes = new List<float>(){0, 2, 4, 5, 7, 9, 11, 12, 14, 16, 17, 19, 21, 23, 24, 26, 28, 29, 31, 33, 35, 36};
	List<float> blackKeysMidiNotes = new List<float>(){1, 3, -1, 6, 8, 10, -1, 13, 15, -1, 18, 20, 22, -1, 25, 27, -1, 30, 32, 34, -1, 37};
	
	public override void OnInspectorGUI(){
		midiPlayer = (MidiPlayer) target;
		serializedObject.Update();
		ShowKeyboard();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("octave"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("voices"), true);
		serializedObject.ApplyModifiedProperties();
	}
	
	void ShowKeyboard(){
		Rect position = EditorGUILayout.BeginHorizontal();
		Rect whitePosition = position.Copy();
		whitePosition.width = 10;
		whitePosition.height = 36;
		whitePosition.y += 4;
		GUILayout.Box("", GUILayout.Height(40));
		for (int i = 0; i < 21; i++){
			if (GUI.Button(whitePosition, "")) midiPlayer.Play(whiteKeysMidiNotes[i] + midiPlayer.octave * 12);
			if (i % 7 != 2 && i % 7 != 6) whitePosition.x += whitePosition.width + 4;
			else whitePosition.x += whitePosition.width - 3;
		}
		Rect blackPosition = position.Copy();
		blackPosition.width = 10;
		blackPosition.height = 25;
		blackPosition.x += (whitePosition.width + 4) / 2;
		for (int i = 0; i < 20; i++){
			if (i % 7 != 2 && i % 7 != 6) {
				if (GUI.Button(blackPosition, "")) midiPlayer.Play(blackKeysMidiNotes[i] + midiPlayer.octave * 12);
				blackPosition.x += whitePosition.width + 4;
			}
			else blackPosition.x += whitePosition.width - 3;
		}
		EditorGUILayout.EndHorizontal();
	}
}
#endif