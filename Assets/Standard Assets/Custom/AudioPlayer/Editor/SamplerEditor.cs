#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(Sampler))]
public class SamplerEditor : Editor {
	
	Sampler sampler;
	
	public override void OnInspectorGUI(){
		sampler = (Sampler) target;
		
		EditorGUI.BeginChangeCheck();
		
		serializedObject.Update();
		
		EditorGUILayout.Space();
		EditorGUILayout.BeginHorizontal();
		EditorGUI.BeginDisabledGroup(Application.isPlaying);
		if (ShowLargeAddElementButton(serializedObject.FindProperty("instruments"), "Add New Instrument")){
			sampler.instruments[sampler.instruments.Length - 1] = new Instrument();
			sampler.instruments[sampler.instruments.Length - 1].name = Sampler.GetUniqueName(sampler.instruments, "default");
		}
		EditorGUI.EndDisabledGroup();
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.Space();
		
		ShowInstruments();
		
		serializedObject.ApplyModifiedProperties();
		
		if (EditorGUI.EndChangeCheck()) EditorUtility.SetDirty(target);
	}
	
	void ShowInstruments(){
		if (sampler.instruments != null){
			for (int i = 0; i < sampler.instruments.Length; i++){
				Instrument instrument = sampler.instruments[i];
				int noteCounter = 0;
				int octaveCounter = 0;
				
				EditorGUILayout.BeginHorizontal();
				instrument.showing = EditorGUILayout.Foldout(instrument.showing, instrument.name);
				EditorGUI.BeginDisabledGroup(Application.isPlaying);
				if (ShowDeleteElementButton(serializedObject.FindProperty("instruments"), i)) break;
				EditorGUI.EndDisabledGroup();
				EditorGUILayout.EndHorizontal();
				
				if (instrument.showing){
					EditorGUI.indentLevel += 1;
					instrument.minNote = Mathf.Round(instrument.minNote);
					instrument.maxNote = Mathf.Round(instrument.maxNote);
					
					EditorGUI.BeginDisabledGroup(Application.isPlaying);
					instrument.name = EditorGUILayout.TextField(instrument.name);
					instrument.generateMode = (Instrument.GenerateModes) EditorGUILayout.EnumPopup("Generate Mode", instrument.generateMode);
					
					if (instrument.generateMode == Instrument.GenerateModes.GenerateAtRuntime){
						EditorGUI.indentLevel += 1;
						instrument.destroyIdle = EditorGUILayout.Toggle("Destroy Idle", instrument.destroyIdle);
						if (instrument.destroyIdle) instrument.idleThreshold = Mathf.Max(EditorGUILayout.FloatField("Idle Threshold", instrument.idleThreshold), 0);
						EditorGUI.indentLevel -= 1;
					}
					instrument.is3D = EditorGUILayout.Toggle("3D Clips", instrument.is3D);
					EditorGUI.EndDisabledGroup();
					
					instrument.maxVoices = EditorGUILayout.IntSlider("Max Voices", instrument.maxVoices, 1, 64);
					
					instrument.velocitySettingsShowing = EditorGUILayout.Foldout(instrument.velocitySettingsShowing, "Velocity");
					if (instrument.velocitySettingsShowing){
						EditorGUI.indentLevel += 1;
						instrument.velocityAffectsVolume = EditorGUILayout.Toggle("Affects Volume", instrument.velocityAffectsVolume);
						instrument.velocityCurve = EditorGUILayout.CurveField("Curve", instrument.velocityCurve, Color.cyan, new Rect(0, 0, 1, 1), GUILayout.Height(16));
						EditorGUI.BeginDisabledGroup(Application.isPlaying);
						instrument.velocityLayers = (int) EditorGUILayout.Slider("Layers", instrument.velocityLayers, 1, 16);
						EditorGUI.EndDisabledGroup();
						EditorGUI.indentLevel -= 1;
					}
					
					EditorGUILayout.BeginHorizontal();
					instrument.notesShowing = EditorGUILayout.Foldout(instrument.notesShowing, "Audio Clips");
					EditorGUI.BeginDisabledGroup(Application.isPlaying);
					if (GUILayout.Button("Reset", EditorStyles.miniButton, GUILayout.Width(50))) instrument.Reset();
					EditorGUI.EndDisabledGroup();
					EditorGUILayout.EndHorizontal();
					
					if (instrument.notesShowing){
						EditorGUI.indentLevel -= 1;
						instrument.InitializeClips();
						
						EditorGUI.BeginDisabledGroup(Application.isPlaying);
						EditorGUILayout.BeginHorizontal();
						GUILayout.Space(16);
						instrument.minNote = EditorGUILayout.FloatField(instrument.minNote, GUILayout.Width(50));
						EditorGUILayout.MinMaxSlider(ref instrument.minNote, ref instrument.maxNote, 0, 127);
						instrument.maxNote = EditorGUILayout.FloatField(instrument.maxNote, GUILayout.Width(50));
						EditorGUILayout.EndHorizontal();
						EditorGUI.EndDisabledGroup();
						
						for (int j = 0; j < 128; j++){
							if (j >= instrument.minNote && j <= instrument.maxNote){
								EditorGUILayout.BeginHorizontal();
								GUILayout.Space(16);
								if (GUILayout.Button(instrument.noteNames[noteCounter].ToString() + octaveCounter.ToString() + " (" + j.ToString() + ")", GUILayout.MinWidth(80), GUILayout.Height(14))){
									if (instrument.GetClipCount() != 0){
										if (Application.isPlaying) Sampler.Play(instrument.name, j, 127, sampler.gameObject);
									}
								}
								for (int k = 0; k < instrument.velocityLayers; k++){
									EditorGUI.BeginDisabledGroup(Application.isPlaying);
									instrument.audioClips[j + (k * 128)] = (AudioClip) EditorGUILayout.ObjectField(instrument.audioClips[j + (k * 128)], typeof(AudioClip), true, GUILayout.Height(14));
									EditorGUI.EndDisabledGroup();
								}
								EditorGUILayout.EndHorizontal();
							}
							noteCounter += 1;
							if (noteCounter == 12){octaveCounter += 1; noteCounter = 0;}
						}
						EditorGUI.indentLevel += 1;
					}
					ShowSeparator(false);
					EditorGUI.indentLevel -= 1;
				}
			}
		}
	}
	
	void ShowSeparator(bool showAboveSpace = true, bool showBelowSpace = true){
		if (showAboveSpace) EditorGUILayout.Space();
		EditorGUILayout.LabelField("", new GUIStyle("RL DragHandle"), GUILayout.Height(4));
		if (showBelowSpace) EditorGUILayout.Space();
	}
	
	bool ShowAddElementButton(SerializedProperty property){
		if (GUILayout.Button("+", EditorStyles.toolbarButton, GUILayout.Width(20))){
			property.arraySize += 1;
			serializedObject.ApplyModifiedProperties();
			EditorUtility.SetDirty(target);
			return true;
		}
		return false;
	}
	
	bool ShowLargeAddElementButton(SerializedProperty property, string name){
		bool pressed = false;
		
		GUILayout.BeginHorizontal();
		GUILayout.Space(EditorGUI.indentLevel * 16);
		if (GUILayout.Button(name, EditorStyles.toolbarButton)){
			pressed = true;
			property.arraySize += 1;
			serializedObject.ApplyModifiedProperties();
			EditorUtility.SetDirty(target);
		}
		GUILayout.EndHorizontal();
		
		return pressed;
	}
	
	bool ShowDeleteElementButton(SerializedProperty property, int indexToRemove){
		if (GUILayout.Button("-", EditorStyles.toolbarButton, GUILayout.Width(20))){
			property.DeleteArrayElementAtIndex(indexToRemove);
			serializedObject.ApplyModifiedProperties();
			EditorUtility.SetDirty(target);
			return true;
		}
		return false;
	}
}
#endif
