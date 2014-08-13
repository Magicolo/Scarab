#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(DSPModule)), CanEditMultipleObjects]
public class DSPModuleEditor : Editor {

	DSPModule dsp;
	
	public override void OnInspectorGUI(){
		dsp = (DSPModule) target;
		
		serializedObject.Update();
		EditorGUILayout.Space();
		dsp.module = (DSPModule.Modules) EditorGUILayout.EnumPopup(dsp.module);
		
		if (dsp.module == DSPModule.Modules.Oscillator) ShowOscillator();
		else if (dsp.module == DSPModule.Modules.Noise) ShowNoise();
		else if (dsp.module == DSPModule.Modules.Sampler) ShowSampler();
		
		else if (dsp.module == DSPModule.Modules.Delay) ShowDelay();
		else if (dsp.module == DSPModule.Modules.Envelope) ShowEnvelope();
		else if (dsp.module == DSPModule.Modules.Gain) ShowGain();
		serializedObject.ApplyModifiedProperties();
	}
	
	void ShowOscillator(){
		EditorGUILayout.PropertyField(serializedObject.FindProperty("waveShape"));
		if (dsp.waveShape == DSPModule.WaveShapes.Custom) dsp.oscCustomShape = EditorGUILayout.CurveField(dsp.oscCustomShape, Color.cyan, new Rect(0, -1, 1, 2), GUILayout.Height(30));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("frequency"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("phase"));
	}
	
	void ShowNoise(){
	}
	
	void ShowSampler(){
		EditorGUILayout.PropertyField(serializedObject.FindProperty("audioClips"), true);
	}
	
	void ShowDelay(){
		EditorGUILayout.PropertyField(serializedObject.FindProperty("feedback"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("delayTime"));
	}
	
	void ShowEnvelope(){
		EditorGUILayout.PropertyField(serializedObject.FindProperty("mode"));
		if (dsp.mode == DSPModule.EnvelopeModes.Custom){
			dsp.envCustomShape = EditorGUILayout.CurveField(dsp.envCustomShape, Color.yellow, new Rect(0, 0, 1, 1), GUILayout.Height(30));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("duration"));
		}
		else {
			EditorGUILayout.PropertyField(serializedObject.FindProperty("attack"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("decay"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("sustain"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("release"));
		}
		EditorGUILayout.PropertyField(serializedObject.FindProperty("smooth"));
	}
	
	void ShowGain(){
		EditorGUILayout.PropertyField(serializedObject.FindProperty("gain"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("rampSpeed"));
	}
}
#endif