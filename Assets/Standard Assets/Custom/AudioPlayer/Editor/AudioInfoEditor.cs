#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(AudioInfo)), CanEditMultipleObjects]
public class AudioInfoEditor : Editor {

	AudioInfo audioInfo;
	
	public override void OnInspectorGUI(){
		audioInfo = (AudioInfo) target;
		
		EditorGUI.BeginChangeCheck();
		
		if (!audioInfo.init) audioInfo.Start();
		else Update();
		serializedObject.Update();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("fadeIn"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("fadeInCurve"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("fadeOut"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("fadeOutCurve"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("randomVolume"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("randomPitch"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("delay"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("syncMode"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("doNotKill"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("effects"), true);
		ShowRTPCs();
		ShowBuses();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("clipInfo"), true);
		serializedObject.ApplyModifiedProperties();
		
		if (EditorGUI.EndChangeCheck()) EditorUtility.SetDirty(target);
	}
	
	void ShowRTPCs(){
		audioInfo.rTPCsShowing = EditorGUILayout.Foldout(audioInfo.rTPCsShowing, "RTPCs");
		if (audioInfo.rTPCsShowing){
			EditorGUI.indentLevel += 1;
			
			if (audioInfo.rTPCs.Length == 0){
				EditorGUILayout.HelpBox("Add RTPCs in the AudioPlayer.", MessageType.Info);
			}
			else {
				for (int i = 0; i < audioInfo.rTPCs.Length; i++){
					AudioInfo.RTPC rtpc = audioInfo.rTPCs[i];
					
					rtpc.showing = EditorGUILayout.Foldout(rtpc.showing, rtpc.name);
					if (rtpc.showing){
						EditorGUI.indentLevel += 1;
						
						for (int j = 0; j < rtpc.parameters.Length; j++){
							AudioInfo.RTPCParameter parameter = rtpc.parameters[j];
							
							EditorGUILayout.BeginHorizontal();
							parameter.showing = EditorGUILayout.Foldout(parameter.showing, parameter.name);
							if (!parameter.showing){
								parameter.enabled = EditorGUILayout.Toggle(parameter.enabled, GUILayout.Width(45));
								EditorGUI.BeginDisabledGroup(!parameter.enabled);
								parameter.curve = EditorGUILayout.CurveField(parameter.curve, Color.green, new Rect(0, 0, 1, 1));
								EditorGUI.EndDisabledGroup();
							}
							EditorGUILayout.EndHorizontal();
							if (parameter.showing){
								EditorGUI.indentLevel += 1;
								parameter.enabled = EditorGUILayout.Toggle("Enabled", parameter.enabled);
								EditorGUI.BeginDisabledGroup(!parameter.enabled);
								parameter.curve = EditorGUILayout.CurveField("Curve", parameter.curve, Color.green, new Rect(0, 0, 1, 1));
								EditorGUI.EndDisabledGroup();
								EditorGUI.indentLevel -= 1;
							}
						}
						EditorGUI.indentLevel -= 1;
					}
				}
			}
			EditorGUI.indentLevel -= 1;
		}
	}
	
	void ShowBuses(){
		audioInfo.busesShowing = EditorGUILayout.Foldout(audioInfo.busesShowing, "Buses");
		if (audioInfo.busesShowing){
			EditorGUI.indentLevel += 1;
			
			if (audioInfo.buses.Length == 0){
				EditorGUILayout.HelpBox("Add Buses in the AudioPlayer.", MessageType.Info);
			}
			else {
				for (int i = 0; i < audioInfo.buses.Length; i++){
					AudioInfo.AudioBus bus = audioInfo.buses[i];
					SerializedProperty busProperty = serializedObject.FindProperty("buses").GetArrayElementAtIndex(i);
					
					EditorGUILayout.BeginHorizontal();
					bus.showing = EditorGUILayout.Foldout(bus.showing, bus.name);
					GUILayout.Space(40);
					if (!bus.showing) bus.sendVolume = EditorGUILayout.Slider(bus.sendVolume, 0, 100);
					EditorGUILayout.EndHorizontal();
					
					if (bus.showing){
						EditorGUI.indentLevel += 1;
						EditorGUILayout.PropertyField(busProperty.FindPropertyRelative("sendVolume"));
						EditorGUI.indentLevel -= 1;
					}
				}
			}
			EditorGUI.indentLevel -= 1;
		}
	}
	
	void Update(){
		audioInfo.transform.position = Vector3.zero;
		audioInfo.transform.rotation = Quaternion.identity;
		audioInfo.transform.localScale = Vector3.one;
		audioInfo.UpdateInfo();
	}
}
#endif
